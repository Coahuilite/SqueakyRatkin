using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace SqueakyRatkin;

/// <summary>心情→音色调制参数。既用于 CompProperties 默认层(XML),也用于 ModSettings override 层(序列化)。</summary>
public class SqueakMoodMod : IExposable
{
    public SqueakMood mood = SqueakMood.Neutral;
    public float pitchFactor = 1f;
    public float volumeFactor = 1f;
    public FloatRange pitchJitter = FloatRange.One;

    public void ExposeData()
    {
        Scribe_Values.Look(ref mood, "mood");
        Scribe_Values.Look(ref pitchFactor, "pitchFactor", 1f);
        Scribe_Values.Look(ref volumeFactor, "volumeFactor", 1f);
        Scribe_Values.Look(ref pitchJitter, "pitchJitter", FloatRange.One);
    }

    public SqueakMoodMod Clone() => new() { mood = mood, pitchFactor = pitchFactor, volumeFactor = volumeFactor, pitchJitter = pitchJitter };
}

public enum SqueakDistancePreset { Conservative, Balanced, Strong, Custom }

/// <summary>
/// 玩家配置。承载:
///  - voicePackMode:运行时 VoicePack 音源策略
///  - moodOverrides:心情调制 override(字段级覆盖 CompProperties 默认,用于换音频后补偿)
/// 所有业务设置实时发布到运行时；磁盘写入由 Mod 层防抖合并。
/// </summary>
public partial class SqueakyRatkinSettings : ModSettings
{
    private const int CurrentSettingsSchemaVersion = 2;
    private static readonly FloatRange FallbackBalancedDistanceRange = new(15f, 50f);

    public SqueakVoicePackMode voicePackMode = SqueakVoicePackMode.Off;
    public int voicePackSchemaVersion = 1;
    // v2 folds the old temporary 1.2 cooldown baseline into shipped XML intervals and restores multiplier 1.0.
    public int settingsSchemaVersion = CurrentSettingsSchemaVersion;
    public bool scaleCooldownWithTimeSpeed = true;
    public bool scaleFrequencyWithTalking = true;
    public bool scalePeriodicWithAudiblePopulation = true;
    public bool localizeDebugActions = false;
    public bool developerToolsEnabled = false;
    public SqueakDevLoggingMode devLoggingMode = SqueakDevLoggingMode.Auto;
    public float globalCooldownMultiplier = 1f;
    public SqueakDistancePreset distancePreset = SqueakDistancePreset.Balanced;
    public FloatRange distanceRange = new(15f, 50f);
    public bool distanceSectionOpen = true;
    public bool globalActionSectionOpen = true;
    public Dictionary<SqueakMood, SqueakMoodMod> moodOverrides = new();
    public List<VoicePackSelectionRecord> voicePackSelections = new();
    public List<XenotypePresetRecord> xenotypePresets = new();
    // Canonical persisted list keeps old saves (with no records) enabled by default.
    public List<GlobalActionEnabledRecord> globalActionEnabled = new();
    public bool EffectiveDevLogging => SqueakLog.EffectiveDevLogging;
    public void SetDevLoggingMode(SqueakDevLoggingMode value)
    {
        devLoggingMode = Enum.IsDefined(typeof(SqueakDevLoggingMode), value) ? value : SqueakDevLoggingMode.Auto;
        ApplyDevLoggingModeToRuntime(true);
    }

    private void ApplyDevLoggingModeToRuntime(bool announceChange)
    {
        bool wasEnabled = SqueakLog.EffectiveDevLogging;
        SqueakLog.Configure(devLoggingMode);
        if (wasEnabled != SqueakLog.EffectiveDevLogging) SqueakDebug.ResetLoggingSession();
        if (announceChange) SqueakLog.LoggingModeChanged(devLoggingMode, SqueakLog.EffectiveDevLogging);
    }

    // 数据驱动:mood/action 列表从所有挂 CompProperties_Squeaker 的 ThingDef 读(XML actions/moodMods)。
    // XML 加配置自动出现在工作台,无需改 C# 数组。DefDatabase 加载后不变,首次访问懒加载缓存。
    private static List<SqueakMood>? _configuredMoods;
    private static List<SqueakAction>? _configuredActions;

    private SqueakMood selectedMood = SqueakMood.Neutral;
    private SqueakAction selectedAction = SqueakAction.Call;
    private Vector2 scrollPos;
    private readonly Dictionary<string, string> numericBuffers = new();
    private bool distanceRangeWasLoaded;
    private bool scaleFrequencyWithTalkingWasLoaded;
    private bool settingsSchemaWasLoaded;
    private bool migrationPersistencePending;
    private bool xenotypeTabRequested;
    private SettingsTab activeTab;
    private int versionClickCount;
    private bool settingsSessionActive;
    private readonly HashSet<SoundDef> moodExplicitlyResolved = new();
    private int moodClipIndex;
    private SqueakSettingsGameContext drawContext;

    private enum SettingsTab { Basics, SoundMood, Xenotype, Developer }

    // IMGUI 数字输入缓冲；业务值仍实时同步到正式 settings。
    private SqueakMoodMod? editBuffer;
    private SqueakMood? bufferForMood;
    private bool editBufferOverrideEnabled;

    public override void ExposeData()
    {
        base.ExposeData();
        // ExposeData is called for multiple Scribe modes. Reset presence only at the beginning of LoadingVars,
        // so its result survives through PostLoadInit.
        if (Scribe.mode == LoadSaveMode.LoadingVars)
        {
            settingsSchemaWasLoaded = false;
            distanceRangeWasLoaded = false;
            scaleFrequencyWithTalkingWasLoaded = false;
        }
        Scribe_Values.Look(ref voicePackMode, "voicePackMode", SqueakVoicePackMode.Off);
        Scribe_Values.Look(ref voicePackSchemaVersion, "voicePackSchemaVersion", 1);
        // The marker must survive even at its default v2 value; otherwise a future explicit 1.2 is indistinguishable
        // from an unmarked pre-v2 test profile on the next load.
        Scribe_Values.Look(ref settingsSchemaVersion, "settingsSchemaVersion", CurrentSettingsSchemaVersion, forceSave: true);
        Scribe_Values.Look(ref scaleCooldownWithTimeSpeed, "scaleCooldownWithTimeSpeed", true);
        Scribe_Values.Look(ref scaleFrequencyWithTalking, "scaleFrequencyWithTalking", GetDefaultScaleFrequencyWithTalking());
        Scribe_Values.Look(ref scalePeriodicWithAudiblePopulation, "scalePeriodicWithAudiblePopulation", true);
        Scribe_Values.Look(ref localizeDebugActions, "localizeDebugActions", false);
        Scribe_Values.Look(ref developerToolsEnabled, "developerToolsEnabled", false);
        Scribe_Values.Look(ref devLoggingMode, "devLoggingMode", SqueakDevLoggingMode.Auto);
        Scribe_Values.Look(ref globalCooldownMultiplier, "globalCooldownMultiplier", 1f);
        Scribe_Values.Look(ref distancePreset, "distancePreset", SqueakDistancePreset.Balanced);
        Scribe_Values.Look(ref distanceRange, "distanceRange", GetDistancePresetRange(SqueakDistancePreset.Balanced));
        if (Scribe.mode == LoadSaveMode.LoadingVars)
        {
            scaleFrequencyWithTalkingWasLoaded = Scribe.loader?.curXmlParent?["scaleFrequencyWithTalking"] != null;
            distanceRangeWasLoaded = Scribe.loader?.curXmlParent?["distanceRange"] != null;
            settingsSchemaWasLoaded = Scribe.loader?.curXmlParent?["settingsSchemaVersion"] != null;
        }
        Scribe_Collections.Look(ref moodOverrides, "moodOverrides", LookMode.Value, LookMode.Deep);
        Scribe_Collections.Look(ref voicePackSelections, "voicePackSelections", LookMode.Deep);
        Scribe_Collections.Look(ref xenotypePresets, "xenotypePresets", LookMode.Deep);
        Scribe_Collections.Look(ref globalActionEnabled, "globalActionEnabled", LookMode.Deep);
        if (Scribe.mode == LoadSaveMode.LoadingVars && moodOverrides == null)
        {
            moodOverrides = new Dictionary<SqueakMood, SqueakMoodMod>();
        }

        if (Scribe.mode == LoadSaveMode.PostLoadInit)
        {
            if (!Enum.IsDefined(typeof(SqueakVoicePackMode), voicePackMode))
            {
                voicePackMode = SqueakVoicePackMode.Off;
            }
            if (!Enum.IsDefined(typeof(SqueakDevLoggingMode), devLoggingMode)) devLoggingMode = SqueakDevLoggingMode.Auto;
            SqueakLog.Configure(devLoggingMode);
            voicePackSchemaVersion = 1;
            // Every missing or pre-v2 marker is made durable once. Only the historical 1.2 baseline
            // changes value; all other explicit multipliers survive the schema marker writeback.
            bool schemaUpgradeNeeded = !settingsSchemaWasLoaded || settingsSchemaVersion < CurrentSettingsSchemaVersion;
            if (schemaUpgradeNeeded)
            {
                if (Mathf.Abs(globalCooldownMultiplier - 1.2f) <= .0001f) globalCooldownMultiplier = 1f;
                settingsSchemaVersion = CurrentSettingsSchemaVersion;
                migrationPersistencePending = true;
            }
            globalCooldownMultiplier = Mathf.Clamp(globalCooldownMultiplier, 0f, 3f);
            if (!scaleFrequencyWithTalkingWasLoaded)
            {
                scaleFrequencyWithTalking = GetDefaultScaleFrequencyWithTalking();
            }
            if (!distanceRangeWasLoaded)
            {
                distanceRange = GetDistancePresetRange(SqueakDistancePreset.Balanced);
            }
            distanceRange = ClampDistanceRange(distanceRange);
            if (voicePackSelections == null) voicePackSelections = new List<VoicePackSelectionRecord>();

            if (xenotypePresets == null)
            {
                xenotypePresets = new List<XenotypePresetRecord>();
            }
            if (globalActionEnabled == null) globalActionEnabled = new List<GlobalActionEnabledRecord>();
            foreach (GlobalActionEnabledRecord record in globalActionEnabled)
            {
                if (record == null || !SqueakActionDefinitions.IsKnown(record.action)) continue;
                record.scope = record.scopeWasLoaded
                    ? SqueakActionDefinitions.NormalizeScope(record.action, record.scope)
                    : record.enabled ? SqueakActionDefinitions.Get(record.action).DefaultScope : SqueakActionScope.Disabled;
                record.enabled = record.scope != SqueakActionScope.Disabled;
            }
        }
    }

    public void ApplyToRuntime()
    {
        // Publish this independent gate before resolver rebuilding. A disabled production action must not
        // need resolver/catalog/context access merely to decide that it is silent.
        SqueakGlobalActionPolicy.Publish(this);
        SqueakRuntimeResolver.NotifyDiscreteResolverChange(this, SqueakXenotypeCatalog.Current);
        CompSqueaker.ScaleCooldownWithTimeSpeed = scaleCooldownWithTimeSpeed;
        CompSqueaker.ScaleFrequencyWithTalking = scaleFrequencyWithTalking;
        CompSqueaker.ScalePeriodicWithAudiblePopulation = scalePeriodicWithAudiblePopulation;
        CompSqueaker.GlobalCooldownMultiplier = Mathf.Clamp(globalCooldownMultiplier, 0f, 3f);
        CompSqueaker.ApplyDistanceRange(ClampDistanceRange(distanceRange));
    }

    /// <summary>Cheap controls are same-frame static runtime values and never rebuild the resolver.</summary>
    public void NotifyCheapRuntimeChanged()
    {
        CompSqueaker.ScaleCooldownWithTimeSpeed = scaleCooldownWithTimeSpeed;
        CompSqueaker.ScaleFrequencyWithTalking = scaleFrequencyWithTalking;
        CompSqueaker.ScalePeriodicWithAudiblePopulation = scalePeriodicWithAudiblePopulation;
        CompSqueaker.GlobalCooldownMultiplier = Mathf.Clamp(globalCooldownMultiplier, 0f, 3f);
    }

    public void NotifyDistanceRuntimeChanged() => CompSqueaker.ApplyDistanceRange(ClampDistanceRange(distanceRange));
    /// <summary>Global mood is read directly by CompSqueaker during playback; no resolver rebuild is needed.</summary>
    public void NotifyGlobalMoodRuntimeChanged() { }
    public void NotifyContinuousXenotypeRuntimeChanged() => SqueakRuntimeResolver.NotifyContinuousResolverChange(this, SqueakXenotypeCatalog.Current);
    public void NotifyDiscreteResolverRuntimeChanged() => SqueakRuntimeResolver.NotifyDiscreteResolverChange(this, SqueakXenotypeCatalog.Current);
    public void QueuePersistence() => SqueakyRatkinMod.Instance?.QueueSettingsSave();
    public void FlushPendingRuntimeForPreview() => SqueakRuntimeResolver.FlushPendingRuntimeChanges(true);
    public bool TryGetSelectedSqueaker(out Pawn? pawn, out CompSqueaker? squeaker) => drawContext.TryGetSelectedSqueaker(out pawn, out squeaker);
    public SqueakSettingsGameContext CurrentDrawContext => drawContext;

    /// <summary>Consumes a PostLoadInit migration only from the main-thread startup callback.</summary>
    internal void QueuePendingMigrationPersistence()
    {
        if (!migrationPersistencePending || SqueakyRatkinMod.Instance == null) return;
        SqueakyRatkinMod.Instance.QueueSettingsSave();
        // Keep the pending flag if QueueSettingsSave ever throws, so the startup path remains recoverable.
        migrationPersistencePending = false;
    }

    internal void ApplySettingsRuntimeSideEffects(bool announceLoggingChange = true)
    {
        Patch_DebugTabMenu_Actions.SetEnabled(localizeDebugActions);
        ApplyDevLoggingModeToRuntime(announceLoggingChange);
    }

    public bool IsActionGloballyEnabled(SqueakAction action)
    {
        return GetActionGlobalScope(action) != SqueakActionScope.Disabled;
    }

    public SqueakActionScope GetActionGlobalScope(SqueakAction action)
    {
        SqueakActionScope scope = SqueakActionDefinitions.Get(action).DefaultScope;
        foreach (GlobalActionEnabledRecord record in globalActionEnabled ?? new List<GlobalActionEnabledRecord>())
            if (record != null && record.action == action) scope = SqueakActionDefinitions.NormalizeScope(action, record.scope);
        return scope;
    }

    internal void SetActionGloballyEnabled(SqueakAction action, bool enabled)
    {
        SetActionGlobalScope(action, enabled ? SqueakActionDefinitions.Get(action).DefaultScope : SqueakActionScope.Disabled);
    }

    internal void SetActionGlobalScope(SqueakAction action, SqueakActionScope scope)
    {
        scope = SqueakActionDefinitions.NormalizeScope(action, scope);
        GlobalActionEnabledRecord? record = null;
        foreach (GlobalActionEnabledRecord candidate in globalActionEnabled)
            if (candidate != null && candidate.action == action) record = candidate;
        if (record == null) globalActionEnabled.Add(new GlobalActionEnabledRecord { action = action, enabled = scope != SqueakActionScope.Disabled, scope = scope, scopeWasLoaded = true });
        else { record.enabled = scope != SqueakActionScope.Disabled; record.scope = scope; record.scopeWasLoaded = true; }
    }

    /// <summary>Explicit future D2 refresh entry; intentionally does not evaluate notifications or normalize saved records.</summary>
    public void RefreshCatalogAndRuntime()
    {
        SqueakXenotypeCatalog.Refresh();
        NotifyDiscreteResolverRuntimeChanged();
    }

    /// <summary>Canonical last-wins selection write. Unknown keys remain persisted for future pack restoration.</summary>
    public void SetVoicePackSelection(SqueakVoicePackScope scope, string targetDefName, IEnumerable<string> enabledKeys)
    {
        if (scope != SqueakVoicePackScope.Race && scope != SqueakVoicePackScope.Xenotype) return;
        string target = scope == SqueakVoicePackScope.Race ? "" : targetDefName ?? "";
        if (scope == SqueakVoicePackScope.Xenotype && target.Length == 0) return;
        string domain = VoicePackSelectionRecord.ComposeDomainKey(scope, target);
        voicePackSelections.RemoveAll(x => x != null && x.DomainKey == domain);
        List<string> keys = (enabledKeys ?? Array.Empty<string>()).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ToList();
        if (keys.Count > 0) voicePackSelections.Add(new VoicePackSelectionRecord { scope = scope, targetDefName = target, enabledPackKeys = keys });
        NotifyDiscreteResolverRuntimeChanged();
        QueuePersistence();
    }

    public void ForgetVoicePackSelection(SqueakVoicePackScope scope, string targetDefName)
    {
        string domain = VoicePackSelectionRecord.ComposeDomainKey(scope, scope == SqueakVoicePackScope.Race ? "" : targetDefName ?? "");
        voicePackSelections.RemoveAll(x => x != null && x.DomainKey == domain);
        NotifyDiscreteResolverRuntimeChanged();
        QueuePersistence();
    }

    public void ForgetXenotypeTarget(string targetDefName)
    {
        if (string.IsNullOrEmpty(targetDefName)) return;
        xenotypePresets.RemoveAll(x => x != null && string.Equals(x.xenotypeDefName, targetDefName, StringComparison.Ordinal));
        string domain = VoicePackSelectionRecord.ComposeDomainKey(SqueakVoicePackScope.Xenotype, targetDefName);
        voicePackSelections.RemoveAll(x => x != null && string.Equals(x.DomainKey, domain, StringComparison.Ordinal));
        NotifyDiscreteResolverRuntimeChanged();
        QueuePersistence();
    }

    public SqueakVoicePackDomainStatus GetVoicePackSelectionStatus(SqueakVoicePackScope scope, string targetDefName)
    {
        string target = scope == SqueakVoicePackScope.Race ? "" : targetDefName ?? "";
        VoicePackSelectionRecord? record = voicePackSelections.LastOrDefault(x => x != null && x.DomainKey == VoicePackSelectionRecord.ComposeDomainKey(scope, target));
        List<string> keys = new(record?.enabledPackKeys ?? new List<string>());
        SqueakXenotypeCatalogSnapshot catalog = SqueakXenotypeCatalog.Current;
        if (scope == SqueakVoicePackScope.Xenotype && !ModsConfig.BiotechActive) return new SqueakVoicePackDomainStatus(SqueakVoicePackDomainState.Dormant, keys);
        IEnumerable<SqueakVoicePackDef> domainPacks = catalog.GetVoicePackDomainPacks(scope, target);
        HashSet<string> domainKeys = new(StringComparer.Ordinal);
        foreach (SqueakVoicePackDef pack in domainPacks)
        {
            if (pack.TryGetPackKey(out string key)) domainKeys.Add(key);
        }
        if (scope == SqueakVoicePackScope.Xenotype && !catalog.XenotypeByDefName.ContainsKey(target)) return new SqueakVoicePackDomainStatus(SqueakVoicePackDomainState.TargetUnavailable, keys);
        foreach (string key in keys) if (!domainKeys.Contains(key)) return new SqueakVoicePackDomainStatus(SqueakVoicePackDomainState.Orphan, keys);
        return new SqueakVoicePackDomainStatus(SqueakVoicePackDomainState.Available, keys);
    }

    public void RequestXenotypeTabOnNextDraw()
    {
        xenotypeTabRequested = true;
    }

    public bool ConsumeXenotypeTabRequest()
    {
        bool requested = xenotypeTabRequested;
        xenotypeTabRequested = false;
        return requested;
    }

    public void ClearXenotypeTabRequest()
    {
        xenotypeTabRequested = false;
    }

    internal static List<SqueakMood> ConfiguredMoods
    {
        get
        {
            if (_configuredMoods == null) { RefreshConfigured(); }

            return _configuredMoods!;
        }
    }

    internal static List<SqueakAction> ConfiguredActions
    {
        get
        {
            if (_configuredActions == null) { RefreshConfigured(); }

            return _configuredActions!;
        }
    }

    internal static IEnumerable<CompProperties_Squeaker> ConfiguredSqueakers()
    {
        foreach (ThingDef def in DefDatabase<ThingDef>.AllDefs)
        {
            if (def.comps == null)
            {
                continue;
            }

            foreach (CompProperties cp in def.comps)
            {
                if (cp is CompProperties_Squeaker sq)
                {
                    yield return sq;
                }
            }
        }
    }

    private static void RefreshConfigured()
    {
        var moods = new List<SqueakMood>();
        var actions = new List<SqueakAction>();
        foreach (CompProperties_Squeaker sq in ConfiguredSqueakers())
        {
            foreach (SqueakActionConfig cfg in sq.actions)
            {
                if (!actions.Contains(cfg.action)) { actions.Add(cfg.action); }
            }

            foreach (SqueakMoodMod mod in sq.moodMods)
            {
                if (!moods.Contains(mod.mood)) { moods.Add(mod.mood); }
            }
        }

        _configuredMoods = moods;
        _configuredActions = actions;
    }

    /// <summary>从 CompProperties_Squeaker(XML 分发默认)取指定 mood 的默认 moodMod,供「还原默认」按钮用。</summary>
    private static SqueakMoodMod? GetDefaultMoodMod(SqueakMood mood)
    {
        foreach (CompProperties_Squeaker sq in ConfiguredSqueakers())
        {
            foreach (SqueakMoodMod m in sq.moodMods)
            {
                if (m.mood == mood) { return m; }
            }
        }

        return null;
    }

    internal SqueakMoodMod GetCanonicalMoodMod(SqueakMood mood)
    {
        return moodOverrides.TryGetValue(mood, out SqueakMoodMod saved)
            ? saved.Clone()
            : (GetDefaultMoodMod(mood)?.Clone() ?? new SqueakMoodMod { mood = mood });
    }

    private static FloatRange GetDistancePresetRange(SqueakDistancePreset preset)
    {
        foreach (CompProperties_Squeaker sq in ConfiguredSqueakers())
        {
            foreach (SqueakDistancePresetConfig cfg in sq.distancePresets)
            {
                if (cfg.preset == preset)
                {
                    return cfg.range;
                }
            }
        }

        return preset switch
        {
            SqueakDistancePreset.Conservative => new FloatRange(15f, 65f),
            SqueakDistancePreset.Strong => new FloatRange(15f, 40f),
            _ => FallbackBalancedDistanceRange,
        };
    }

    private static bool GetDefaultScaleFrequencyWithTalking()
    {
        foreach (CompProperties_Squeaker sq in ConfiguredSqueakers())
        {
            return sq.scaleFrequencyWithTalking;
        }

        return true;
    }

    /// <summary>选 mood 变化或启用状态变时,从 moodOverrides 重建 editBuffer,清 numericBuffers 强制刷新输入框显示。</summary>
    private void SyncBufferFromSaved()
    {
        // 有 override 用 override;否则用 XML 分发默认(而非空白 1/1),让切 mood 时显示该 mood 的实际生效值。
        editBuffer = moodOverrides.TryGetValue(selectedMood, out SqueakMoodMod saved)
            ? saved.Clone()
            : (GetDefaultMoodMod(selectedMood)?.Clone() ?? new SqueakMoodMod { mood = selectedMood });
        editBufferOverrideEnabled = moodOverrides.ContainsKey(selectedMood);
        bufferForMood = selectedMood;
        numericBuffers.Clear();
    }

    private void EnsureBuffer()
    {
        if (editBuffer == null || bufferForMood != selectedMood)
        {
            SyncBufferFromSaved();
        }
    }

    public void DrawSettings(Rect rect)
    {
        drawContext = SqueakSettingsGameContext.Capture();
        SqueakRuntimeResolver.TickPendingRuntimeChanges();
        if (ConsumeXenotypeTabRequest()) RequestSettingsTab(SettingsTab.Xenotype);
        if (!developerToolsEnabled && activeTab == SettingsTab.Developer) activeTab = SettingsTab.Basics;
        SettingsNavigationLayout navigationLayout = MeasureSettingsNavigation(rect.width);
        float navigationHeight = navigationLayout.Height;
        Rect navigation = new(rect.x, rect.y, rect.width, navigationHeight);
        const float shellGap = 6f;
        const float footerHeight = 34f;
        Rect footer = new(rect.x, rect.yMax - footerHeight, Mathf.Max(1f, rect.width), footerHeight);
        DrawSettingsNavigation(navigation, navigationLayout);
        Rect bodyFrame = new(rect.x, navigation.yMax + shellGap, Mathf.Max(1f, rect.width),
            Mathf.Max(1f, footer.y - navigation.yMax - shellGap * 2f));
        DrawSettingsPageFrame(bodyFrame);
        Rect body = new(bodyFrame.x + 8f, bodyFrame.y + 8f,
            Mathf.Max(1f, bodyFrame.width - 16f), Mathf.Max(1f, bodyFrame.height - 16f));
        Rect content = body;
        if (activeTab == SettingsTab.Developer && developerToolsEnabled) DrawDeveloperSettings(content);
        else if (activeTab == SettingsTab.Xenotype) DrawXenotypeSettings(content);
        else if (activeTab == SettingsTab.SoundMood) DrawSoundMoodSettings(content);
        else
        {
            float viewWidth = content.width;
            float basicsContentHeight = MeasureBasicsContentHeight(viewWidth);
            if (basicsContentHeight > content.height)
            {
                viewWidth = Mathf.Max(1f, content.width - 16f);
                basicsContentHeight = MeasureBasicsContentHeight(viewWidth);
            }
            float maxScroll = Mathf.Max(0f, basicsContentHeight - content.height);
            scrollPos.y = Mathf.Clamp(scrollPos.y, 0f, maxScroll);
            Rect viewRect = new(0f, 0f, viewWidth, Mathf.Max(content.height, basicsContentHeight));
            Widgets.BeginScrollView(content, ref scrollPos, viewRect);
            DrawSettingsContents(viewRect);
            Widgets.EndScrollView();
        }
        DrawSettingsFooter(footer);
    }

    internal void BeginSettingsSession()
    {
        if (settingsSessionActive) return;
        settingsSessionActive = true;
        versionClickCount = 0;
    }

    internal void EndSettingsSession()
    {
        settingsSessionActive = false;
        versionClickCount = 0;
    }

    private static void DrawSaveStatus(Rect rect)
    {
        SqueakyRatkinMod? mod = SqueakyRatkinMod.Instance;
        if (mod == null || !mod.SaveStatusVisible) return;
        string key = mod.SaveState switch
        {
            SqueakyRatkinMod.SettingsSaveState.Failed => "SR.Settings.Save.Failed",
            SqueakyRatkinMod.SettingsSaveState.Saved => "SR.Settings.Save.Saved",
            _ => "SR.Settings.Save.Saving"
        };
        Color old = GUI.color; GameFont oldFont = Text.Font; TextAnchor oldAnchor = Text.Anchor;
        Text.Font = GameFont.Tiny; Text.Anchor = TextAnchor.MiddleRight;
        GUI.color = mod.SaveState == SqueakyRatkinMod.SettingsSaveState.Failed ? new Color(1f, .48f, .42f) : SqueakySettingsUI.Muted;
        if (mod.SaveState == SqueakyRatkinMod.SettingsSaveState.Failed)
        {
            SqueakySettingsUI.LabelWithHelp(rect, key.Translate(), "SR.Settings.Save.Tooltip".Translate(), GameFont.Tiny,
                new Color(1f, .48f, .42f));
        }
        else Widgets.Label(rect, key.Translate());
        Text.Anchor = oldAnchor; Text.Font = oldFont; GUI.color = old;
    }

    private void DrawSettingsFooter(Rect rect)
    {
        SqueakySettingsUI.PanelFrame(rect);
        Rect inner = rect.ContractedBy(8f, 4f);
        float statusWidth = Mathf.Clamp(inner.width * .34f, 150f, 230f);
        Rect versionRect = new(inner.x, inner.y, Mathf.Max(1f, inner.width - statusWidth - 12f), inner.height);
        Rect statusRect = new(inner.xMax - statusWidth, inner.y, statusWidth, inner.height);

        string fullVersion = CurrentVersion();
        string versionLabel = "SR.Basics.Version".Translate(ShortVersion(fullVersion));
        string versionTooltip = "SR.Basics.Version.Tooltip".Translate(fullVersion,
            "SR.Basics.Version.Count".Translate(versionClickCount));
        Color oldColor = GUI.color; GameFont oldFont = Text.Font; TextAnchor oldAnchor = Text.Anchor;
        GUI.color = new Color(.72f, .69f, .63f);
        Text.Font = GameFont.Tiny;
        Text.Anchor = TextAnchor.MiddleLeft;
        SqueakySettingsUI.EllipsizedLabel(versionRect, versionLabel);
        TooltipHandler.TipRegion(versionRect, versionTooltip);
        Text.Anchor = oldAnchor; Text.Font = oldFont; GUI.color = oldColor;
        if (Widgets.ButtonInvisible(versionRect) && !developerToolsEnabled)
        {
            versionClickCount++;
            if (versionClickCount >= 7)
            {
                EnableDeveloperToolsNow();
                versionClickCount = 0;
                Messages.Message("SR.DevTools.Enabled".Translate(), MessageTypeDefOf.PositiveEvent, false);
            }
        }
        DrawSaveStatus(statusRect);
    }

    private void RequestSettingsTab(SettingsTab target)
    {
        if (target == activeTab) return;
        // This is an actual top-level transition, not a per-frame draw. The partial commit methods are
        // dirty-gated, so a clean editor stays O(1) and creates no persistence generation.
        if (activeTab == SettingsTab.SoundMood) CommitMoodEditorNow();
        else if (activeTab == SettingsTab.Xenotype) CommitXenotypeEditorNow();
        FlushPendingRuntimeForPreview();
        activeTab = target;
    }


    private sealed class SettingsNavigationLayout
    {
        internal readonly SettingsTab[] Tabs;
        internal readonly Rect[] Items;
        internal readonly bool Wrapped;
        internal readonly float Height;

        internal SettingsNavigationLayout(SettingsTab[] tabs, Rect[] items, bool wrapped, float height)
        {
            Tabs = tabs;
            Items = items;
            Wrapped = wrapped;
            Height = height;
        }
    }

    private SettingsNavigationLayout MeasureSettingsNavigation(float width)
    {
        const float padding = 6f;
        const float gap = 6f;
        const float compactHeight = 36f;
        const float readableHorizontalPadding = 24f;
        SettingsTab[] tabs = VisibleSettingsTabs();
        float innerWidth = Mathf.Max(1f, width - padding * 2f);
        float[] minimumWidths = new float[tabs.Length];
        float minimumTotal = gap * (tabs.Length - 1);
        GameFont oldFont = Text.Font;
        Text.Font = GameFont.Small;
        for (int i = 0; i < tabs.Length; i++)
        {
            minimumWidths[i] = Mathf.Max(116f, Text.CalcSize(SettingsTabDisplayTitle(tabs[i])).x + readableHorizontalPadding);
            minimumTotal += minimumWidths[i];
        }
        Text.Font = oldFont;

        bool wrapped = minimumTotal > innerWidth;
        Rect[] items = new Rect[tabs.Length];
        if (wrapped)
        {
            int columns = tabs.Length == 3 ? 2 : Mathf.Min(2, tabs.Length);
            int rows = Mathf.CeilToInt((float)tabs.Length / columns);
            float cellWidth = Mathf.Max(1f, (innerWidth - gap * (columns - 1)) / columns);
            for (int i = 0; i < tabs.Length; i++)
            {
                int row = i / columns;
                int column = i % columns;
                bool lastAlone = tabs.Length % columns == 1 && i == tabs.Length - 1;
                items[i] = new Rect(padding + column * (cellWidth + gap), padding + row * (compactHeight + gap),
                    lastAlone ? innerWidth : cellWidth, compactHeight);
            }
            return new SettingsNavigationLayout(tabs, items, true, padding * 2f + rows * compactHeight + (rows - 1) * gap);
        }

        float extra = Mathf.Max(0f, innerWidth - minimumTotal) / tabs.Length;
        float x = padding;
        float tallest = 0f;
        for (int i = 0; i < tabs.Length; i++)
        {
            float itemWidth = minimumWidths[i] + extra;
            float textHeight = MeasureSettingsNavigationText(tabs[i], SqueakySettingsUI.SelectableCardTextWidth(itemWidth));
            tallest = Mathf.Max(tallest, Mathf.Max(64f, textHeight + 18f));
            items[i] = new Rect(x, padding, itemWidth, 0f);
            x += itemWidth + gap;
        }
        for (int i = 0; i < items.Length; i++)
        {
            Rect item = items[i];
            item.height = tallest;
            items[i] = item;
        }
        return new SettingsNavigationLayout(tabs, items, false, padding * 2f + tallest);
    }

    private void DrawSettingsNavigation(Rect rect, SettingsNavigationLayout layout)
    {
        SqueakySettingsUI.PanelFrame(rect, true);
        for (int i = 0; i < layout.Tabs.Length; i++)
        {
            SettingsTab tab = layout.Tabs[i];
            Rect local = layout.Items[i];
            Rect item = new(rect.x + local.x, rect.y + local.y, local.width, local.height);
            bool clicked = layout.Wrapped
                ? SqueakySettingsUI.Tab(item, SettingsTabDisplayTitle(tab), activeTab == tab)
                : SqueakySettingsUI.SelectableCard(item, SettingsTabDisplayTitle(tab), SettingsTabSubtitle(tab), activeTab == tab);
            if (clicked) RequestSettingsTab(tab);
        }
    }

    private SettingsTab[] VisibleSettingsTabs() => developerToolsEnabled
        ? new[] { SettingsTab.Basics, SettingsTab.SoundMood, SettingsTab.Xenotype, SettingsTab.Developer }
        : new[] { SettingsTab.Basics, SettingsTab.SoundMood, SettingsTab.Xenotype };

    private float MeasureSettingsNavigationText(SettingsTab tab, float width)
    {
        GameFont oldFont = Text.Font;
        Text.Font = GameFont.Small;
        float titleHeight = Text.CalcHeight(SettingsTabDisplayTitle(tab), width);
        Text.Font = GameFont.Tiny;
        float subtitleHeight = Text.CalcHeight(SettingsTabSubtitle(tab), width);
        Text.Font = oldFont;
        return titleHeight + 3f + subtitleHeight;
    }

    private string SettingsTabDisplayTitle(SettingsTab tab)
    {
        string title = SettingsTabTitle(tab);
        if (tab == SettingsTab.Xenotype && xenotypeDraft?.Dirty == true) title += "  •";
        return title;
    }

    private static string SettingsTabTitle(SettingsTab tab) => (tab switch
    {
        SettingsTab.Developer => "SR.Tab.Developer",
        SettingsTab.Xenotype => "SR.Tab.Xenotype",
        SettingsTab.SoundMood => "SR.Tab.SoundMood",
        _ => "SR.Tab.Basics"
    }).Translate();

    private static string SettingsTabSubtitle(SettingsTab tab) => (tab switch
    {
        SettingsTab.Developer => "SR.Tab.Developer.Desc",
        SettingsTab.Xenotype => "SR.Tab.Xenotype.Desc",
        SettingsTab.SoundMood => "SR.Tab.SoundMood.Desc",
        _ => "SR.Tab.Basics.Desc"
    }).Translate();

    private static void DrawSettingsPageFrame(Rect rect)
    {
        SqueakySettingsUI.PanelFrame(rect);
    }

    private void DrawSettingsContents(Rect rect, bool interactive = true)
    {
        bool previousEnabled = GUI.enabled;
        if (!interactive) GUI.enabled = false;
        Listing_Standard list = new();
        list.maxOneColumn = true;
        list.Begin(rect);
        DrawCompactPageIntro(list, "SR.Rules.Header".Translate(), "SR.Rules.PageDesc".Translate());
        DrawSectionHeader(list, "SR.Rules.Frequency".Translate());
        bool scaleCooldownWithTimeSpeed = this.scaleCooldownWithTimeSpeed;
        bool scaleFrequencyWithTalking = this.scaleFrequencyWithTalking;
        bool scalePeriodicWithAudiblePopulation = this.scalePeriodicWithAudiblePopulation;
        float globalCooldownMultiplier = this.globalCooldownMultiplier;
        if (SqueakySettingsUI.Toggle(list.GetRect(34f), "SR.ScaleCooldownWithTimeSpeed.Label".Translate(), ref scaleCooldownWithTimeSpeed,
                tooltip: "SR.ScaleCooldownWithTimeSpeed.Tooltip".Translate()))
        {
            this.scaleCooldownWithTimeSpeed = scaleCooldownWithTimeSpeed;
            ApplyCheapAndQueue();
        }
        if (SqueakySettingsUI.Toggle(list.GetRect(34f), "SR.ScaleFrequencyWithTalking.Label".Translate(), ref scaleFrequencyWithTalking,
            tooltip: "SR.ScaleFrequencyWithTalking.Tooltip".Translate()))
        {
            this.scaleFrequencyWithTalking = scaleFrequencyWithTalking;
            ApplyCheapAndQueue();
        }
        if (SqueakySettingsUI.Toggle(list.GetRect(34f), "SR.ScalePeriodicWithAudiblePopulation.Label".Translate(), ref scalePeriodicWithAudiblePopulation,
            tooltip: "SR.ScalePeriodicWithAudiblePopulation.Tooltip".Translate()))
        {
            this.scalePeriodicWithAudiblePopulation = scalePeriodicWithAudiblePopulation;
            ApplyCheapAndQueue();
        }
        Color oldTalkingDesc = GUI.color; GUI.color = Color.gray; Text.Font = GameFont.Tiny;
        list.Label("SR.ScaleFrequencyWithTalking.Short".Translate());
        Text.Font = GameFont.Small; GUI.color = oldTalkingDesc;
        SqueakySettingsUI.LabelWithHelp(list.GetRect(28f), "SR.GlobalCooldownMultiplier.Label".Translate(globalCooldownMultiplier.ToString("0.0#")),
            "SR.GlobalCooldownMultiplier.Tooltip".Translate());
        globalCooldownMultiplier = list.Slider(globalCooldownMultiplier, 0f, 3f);
        Color oldIntervalDesc = GUI.color; GUI.color = Color.gray; Text.Font = GameFont.Tiny;
        list.Label("SR.GlobalCooldownMultiplier.Short".Translate());
        Text.Font = GameFont.Small; GUI.color = oldIntervalDesc;
        if (interactive && Math.Abs(this.globalCooldownMultiplier - globalCooldownMultiplier) > .0001f)
        {
            this.globalCooldownMultiplier = globalCooldownMultiplier;
            ApplyCheapAndQueue();
        }
        list.Gap(8f);
        list.GapLine();

        DrawCollapsibleHeader(list, "SR.Distance.HeaderWithSummary".Translate(DistancePresetLabel(distancePreset), distanceRange.min.ToString("0.#"), distanceRange.max.ToString("0.#")), ref distanceSectionOpen);
        if (distanceSectionOpen)
        {
            DrawDistanceSettings(list);
        }

        list.Gap(8f);
        list.GapLine();
        DrawCollapsibleHeader(list, "SR.ActionEnable.HeaderWithSummary".Translate(ConfiguredActions.Count), ref globalActionSectionOpen);
        if (globalActionSectionOpen) DrawGlobalActionEnabledSettings(list);

        DrawAuxiliarySettings(list);
        list.End();
        GUI.enabled = previousEnabled;
    }

    private float MeasureBasicsContentHeight(float width)
    {
        float height = MeasureCompactPageIntroHeight("SR.Rules.Header".Translate(), "SR.Rules.PageDesc".Translate(), width) + 34f;
        height += 34f + 34f + 34f + Text.CalcHeight("SR.ScaleFrequencyWithTalking.Short".Translate(), width);
        height += 28f + 30f + Text.CalcHeight("SR.GlobalCooldownMultiplier.Short".Translate(), width) + 20f;
        height += 34f;
        if (distanceSectionOpen)
        {
            height += 32f + 32f + 32f + (width >= 760f ? 116f : 142f);
            height += Mathf.Max(28f, Text.CalcHeight("SR.Distance.Short".Translate(distanceRange.min.ToString("0.#"), distanceRange.max.ToString("0.#")), width - 24f));
        }
        height += 20f + 34f;
        if (globalActionSectionOpen)
        {
            height += Mathf.Max(28f, Text.CalcHeight("SR.ActionEnable.Short".Translate(), width - 24f)) + 6f;
            height += MeasureActionGroupHeight(width, 5, true) + MeasureActionGroupHeight(width, 10, false);
        }
        height += 20f + 34f + 34f + 8f;
        return height + 24f;
    }

    private static float MeasureCompactPageIntroHeight(string title, string description, float width)
    {
        GameFont oldFont = Text.Font;
        Text.Font = GameFont.Medium;
        float titleHeight = Text.CalcHeight(title, width);
        Text.Font = GameFont.Tiny;
        float descriptionHeight = Text.CalcHeight(description, width);
        Text.Font = oldFont;
        return titleHeight + 2f + descriptionHeight + 6f;
    }

    private static void DrawCompactPageIntro(Listing_Standard list, string title, string description)
    {
        Rect rect = list.GetRect(MeasureCompactPageIntroHeight(title, description, list.ColumnWidth));
        GameFont oldFont = Text.Font;
        Color oldColor = GUI.color;
        Text.Font = GameFont.Medium;
        float titleHeight = Text.CalcHeight(title, rect.width);
        Widgets.Label(new Rect(rect.x, rect.y, rect.width, titleHeight), title);
        Text.Font = GameFont.Tiny;
        GUI.color = SqueakySettingsUI.Muted;
        float descriptionHeight = Text.CalcHeight(description, rect.width);
        Widgets.Label(new Rect(rect.x, rect.y + titleHeight + 2f, rect.width, descriptionHeight), description);
        Text.Font = oldFont;
        GUI.color = oldColor;
    }

    private static float MeasureCompactStatusHeight(string text, float width, float minimum = 32f)
    {
        GameFont oldFont = Text.Font;
        Text.Font = GameFont.Tiny;
        float height = Mathf.Max(minimum, Text.CalcHeight(text, Mathf.Max(1f, width - 16f)) + 10f);
        Text.Font = oldFont;
        return height;
    }

    private static float MeasureActionGroupHeight(float width, int actionCount, bool hasDraftPair)
    {
        float row = ActionScopeRowHeight(width);
        int ordinary = hasDraftPair ? actionCount - 2 : actionCount;
        return 34f + ordinary * (row + 3f) + (hasDraftPair ? 24f + row * 2f + 6f : 0f) + 4f;
    }

    private void DrawGlobalActionEnabledSettings(Listing_Standard list)
    {
        SqueakySettingsUI.LabelWithHelp(list.GetRect(Mathf.Max(28f, Text.CalcHeight("SR.ActionEnable.Short".Translate(), list.ColumnWidth - 24f))),
            "SR.ActionEnable.Short".Translate(), "SR.ActionEnable.Tooltip".Translate());
        list.Gap(6f);
        DrawActionGroup(list, "SR.ActionEnable.PlayerGroup", new[]
        {
            SqueakAction.Draft, SqueakAction.Undraft, SqueakAction.Attack, SqueakAction.Equip, SqueakAction.Select
        });
        DrawActionGroup(list, "SR.ActionEnable.SystemGroup", new[]
        {
            SqueakAction.Call, SqueakAction.Eat, SqueakAction.Sleep, SqueakAction.Wounded, SqueakAction.Move,
            SqueakAction.Work, SqueakAction.Social, SqueakAction.Joy, SqueakAction.Death, SqueakAction.MentalBreak
        });
    }

    private void DrawActionGroup(Listing_Standard list, string headerKey, IReadOnlyList<SqueakAction> actions)
    {
        DrawSectionHeader(list, headerKey.Translate());
        foreach (SqueakAction action in actions)
        {
            if (action == SqueakAction.Draft)
            {
                DrawCombinedDraftScopeSettings(list);
                continue;
            }
            if (action == SqueakAction.Undraft) continue;
            DrawActionScopeRow(list.GetRect(ActionScopeRowHeight(list.ColumnWidth)), action);
            list.Gap(3f);
        }
        list.Gap(4f);
    }

    private void DrawCombinedDraftScopeSettings(Listing_Standard list)
    {
        const float headingHeight = 24f;
        const float rowGap = 3f;
        float rowHeight = ActionScopeRowHeight(list.ColumnWidth);
        Rect group = list.GetRect(headingHeight + rowHeight * 2f + rowGap);

        GameFont oldFont = Text.Font;
        Color oldColor = GUI.color;
        Text.Font = GameFont.Tiny;
        GUI.color = new Color(1f, .78f, .40f, .92f);
        Widgets.Label(new Rect(group.x + 6f, group.y, group.width - 12f, headingHeight), "SR.Action.DraftUndraft".Translate());
        Text.Font = oldFont;
        GUI.color = oldColor;

        Rect first = new(group.x, group.y + headingHeight, group.width, rowHeight);
        DrawActionScopeRow(first, SqueakAction.Draft);
        DrawActionScopeRow(new Rect(first.x, first.yMax + rowGap, first.width, rowHeight), SqueakAction.Undraft);
        list.Gap(3f);
    }

    private static float ActionScopeRowHeight(float width) => width < 620f ? 58f : 38f;

    private void DrawActionScopeRow(Rect rect, SqueakAction action)
    {
        bool narrow = rect.width < 620f;
        bool hovered = Mouse.IsOver(rect);
        Widgets.DrawBoxSolid(rect, hovered ? new Color(.105f, .099f, .087f, .72f) : new Color(.07f, .067f, .061f, .64f));

        const float inset = 6f;
        const float gap = 8f;
        Rect labelRect;
        Rect selectorRect;
        if (narrow)
        {
            labelRect = new Rect(rect.x + inset, rect.y + 2f, rect.width - inset * 2f, 22f);
            selectorRect = new Rect(rect.x + inset, rect.y + 25f, rect.width - inset * 2f, 28f);
        }
        else
        {
            float labelWidth = Mathf.Clamp(rect.width * .44f, 180f, 340f);
            labelRect = new Rect(rect.x + inset, rect.y, labelWidth - inset, rect.height);
            float selectorWidth = Mathf.Min(330f, rect.width - labelWidth - gap - inset);
            selectorRect = new Rect(rect.xMax - inset - selectorWidth, rect.y + 4f, selectorWidth, rect.height - 8f);
        }

        string actionLabel = SqueakLabels.Action(action);
        TextAnchor oldAnchor = Text.Anchor;
        Text.Anchor = TextAnchor.MiddleLeft;
        SqueakySettingsUI.EllipsizedLabel(labelRect, actionLabel, actionLabel);
        Text.Anchor = oldAnchor;

        SqueakActionScope scope = GetActionGlobalScope(action);
        string scopeLabel = ScopeLabel(action, scope);
        if (!SqueakySettingsUI.SettingSelector(selectorRect, scopeLabel,
                scope != SqueakActionScope.Disabled, "SR.ActionScope.Tooltip".Translate(actionLabel))) return;
        OpenActionScopeMenu(action);
    }

    private void OpenActionScopeMenu(SqueakAction action)
    {
        List<FloatMenuOption> options = new();
        AddScopeOption(options, action, SqueakActionScope.Disabled);
        SqueakActionDefinition definition = SqueakActionDefinitions.Get(action);
        if ((definition.SupportedScopes & SqueakActionScopeSupport.AnyOccurrence) != 0) AddScopeOption(options, action, SqueakActionScope.AnyOccurrence);
        if ((definition.SupportedScopes & SqueakActionScopeSupport.ActiveCommand) != 0) AddScopeOption(options, action, SqueakActionScope.ActiveCommand);
        Find.WindowStack.Add(new FloatMenu(options));
    }

    private static string ScopeLabel(SqueakAction action, SqueakActionScope scope)
    {
        if (scope == SqueakActionScope.Disabled) return "SR.ActionScope.Disabled".Translate();
        return ("SR.ActionScope." + action + "." + scope).Translate();
    }

    private void AddScopeOption(List<FloatMenuOption> options, SqueakAction action, SqueakActionScope scope)
    {
        options.Add(new FloatMenuOption(ScopeLabel(action, scope), () =>
        {
            if (GetActionGlobalScope(action) == scope) return;
            SetActionGlobalScope(action, scope);
            ApplyActionScopeAndQueue();
        }));
    }

    private void ApplyCheapAndQueue()
    {
        NotifyCheapRuntimeChanged();
        QueuePersistence();
    }

    private void ApplyDistanceAndQueue()
    {
        NotifyDistanceRuntimeChanged();
        QueuePersistence();
    }

    private void ApplyActionScopeAndQueue()
    {
        SqueakGlobalActionPolicy.Publish(this);
        NotifyDiscreteResolverRuntimeChanged();
        QueuePersistence();
    }

    private void DrawAuxiliarySettings(Listing_Standard list)
    {
        list.Gap(8f); list.GapLine(); DrawSectionHeader(list, "SR.Basics.Auxiliary".Translate());
        bool localize = localizeDebugActions;
        if (SqueakySettingsUI.Toggle(list.GetRect(34f), "SR.LocalizeDebugActions.Label".Translate(), ref localize,
                tooltip: "SR.LocalizeDebugActions.Tooltip".Translate()))
        {
            localizeDebugActions = localize;
            Patch_DebugTabMenu_Actions.SetEnabled(localizeDebugActions);
            QueuePersistence();
        }
        list.Gap(8f);
    }

    private void DrawDevLoggingModeCards(Rect rect)
    {
        SqueakDevLoggingMode[] modes = { SqueakDevLoggingMode.Auto, SqueakDevLoggingMode.Enabled, SqueakDevLoggingMode.Disabled };
        bool stacked = rect.width < 640f;
        const float gap = 8f;
        float cardWidth = stacked ? rect.width : (rect.width - gap * 2f) / 3f;
        float rowHeight = stacked ? 0f : modes.Max(mode => MeasureDevLoggingModeCard(cardWidth, mode));
        float y = rect.y;
        foreach (SqueakDevLoggingMode mode in modes)
        {
            float cardHeight = stacked ? MeasureDevLoggingModeCard(cardWidth, mode) : rowHeight;
            Rect card = stacked ? new Rect(rect.x, y, cardWidth, cardHeight)
                : new Rect(rect.x + (int)mode * (cardWidth + gap), rect.y, cardWidth, cardHeight);
            string title = ("SR.DevTools.Logging.Mode." + mode).Translate();
            string description = ("SR.DevTools.Logging.Mode.Desc." + mode).Translate();
            string tooltip = ("SR.DevTools.Logging.Mode.Tooltip." + mode).Translate();
            if (SqueakySettingsUI.SelectableCard(card, title, description, devLoggingMode == mode, tooltip) && devLoggingMode != mode)
            {
                SetDevLoggingMode(mode);
                QueuePersistence();
            }
            if (stacked) y += cardHeight + gap;
        }
    }

    private void EnableDeveloperToolsNow()
    {
        developerToolsEnabled = true;
        QueuePersistence();
    }

    private void DisableDeveloperToolsNow()
    {
        SqueakActionStatistics.Stop();
        SqueakAudioPathDiagnostics.Enabled = false;
        developerToolsEnabled = false;
        if (activeTab == SettingsTab.Developer) activeTab = SettingsTab.Basics;
        QueuePersistence();
    }

    private static float MeasureDevLoggingModeCards(float width)
    {
        SqueakDevLoggingMode[] modes = { SqueakDevLoggingMode.Auto, SqueakDevLoggingMode.Enabled, SqueakDevLoggingMode.Disabled };
        bool stacked = width < 640f;
        const float gap = 8f;
        float cardWidth = stacked ? width : (width - gap * 2f) / 3f;
        return stacked ? modes.Sum(mode => MeasureDevLoggingModeCard(cardWidth, mode)) + gap * 2f
            : modes.Max(mode => MeasureDevLoggingModeCard(cardWidth, mode));
    }

    private static float MeasureDevLoggingModeCard(float width, SqueakDevLoggingMode mode)
    {
        float textWidth = SqueakySettingsUI.SelectableCardTextWidth(width, hasTooltip: true);
        GameFont oldFont = Text.Font;
        Text.Font = GameFont.Small;
        float titleHeight = Text.CalcHeight(("SR.DevTools.Logging.Mode." + mode).Translate(), textWidth);
        Text.Font = GameFont.Tiny;
        float descriptionHeight = Text.CalcHeight(("SR.DevTools.Logging.Mode.Desc." + mode).Translate(), textWidth);
        Text.Font = oldFont;
        return Mathf.Max(86f, titleHeight + descriptionHeight + 29f);
    }

    private static bool DevLoggingAutoDefault
    {
        get
        {
#if SQUEAKY_DEV
            return true;
#else
            return false;
#endif
        }
    }

    private static string CurrentVersion()
    {
        Assembly asm = typeof(SqueakyRatkinSettings).Assembly;
        return asm.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? asm.GetName().Version?.ToString() ?? "unknown";
    }

    private static string ShortVersion(string informational)
    {
        int plus = informational.IndexOf('+');
        if (plus < 0 || plus + 1 >= informational.Length) return informational;
        string revision = informational[(plus + 1)..];
        int separator = revision.IndexOfAny(new[] { '.', '-', '+' });
        if (separator > 0) revision = revision[..separator];
        if (revision.Length > 8) revision = revision[..8];
        return informational[..plus] + "+" + revision;
    }

    private static void DrawSectionHeader(Listing_Standard list, string label, string tooltip = "")
    {
        Rect rect = list.GetRect(34f);
        SqueakySettingsUI.SectionHeader(rect, label, tooltip: tooltip);
        Widgets.DrawBoxSolid(new Rect(rect.x, rect.yMax - 2f, Mathf.Min(52f, rect.width), 1f), SqueakySettingsUI.Gold);
    }

    private void DrawDistanceSettings(Listing_Standard list)
    {
        Rect presetRect = list.GetRect(32f);
        SqueakDistancePreset distancePreset = this.distancePreset;
        FloatRange distanceRange = this.distanceRange;
        string presetLabel = "SR.Distance.Preset".Translate() + ": " + DistancePresetLabel(distancePreset);
        if (SqueakySettingsUI.SettingSelector(presetRect, presetLabel, distancePreset != SqueakDistancePreset.Custom,
                "SR.Distance.Preset.Tooltip".Translate()))
        {
            List<FloatMenuOption> options = new();
            foreach (SqueakDistancePreset preset in Enum.GetValues(typeof(SqueakDistancePreset)))
            {
                SqueakDistancePreset localPreset = preset;
                options.Add(new FloatMenuOption(DistancePresetLabel(localPreset), () => ApplyDistancePreset(localPreset)));
            }

            Find.WindowStack.Add(new FloatMenu(options));
        }

        FloatRange before = distanceRange;
        distanceRange.min = DrawSliderWithField(list.GetRect(32f), "Distance.Min", "SR.Distance.FullVolume".Translate(), distanceRange.min, 15f, 60f);
        distanceRange.max = DrawSliderWithField(list.GetRect(32f), "Distance.Max", "SR.Distance.Silent".Translate(), distanceRange.max, 20f, 65f);
        distanceRange = ClampDistanceRange(distanceRange);

        if (Math.Abs(before.min - distanceRange.min) > 0.0001f || Math.Abs(before.max - distanceRange.max) > 0.0001f)
        {
            distancePreset = SqueakDistancePreset.Custom;
        }

        if (distancePreset != this.distancePreset || Math.Abs(distanceRange.min - this.distanceRange.min) > .0001f || Math.Abs(distanceRange.max - this.distanceRange.max) > .0001f)
        {
            this.distancePreset = distancePreset;
            this.distanceRange = distanceRange;
            ApplyDistanceAndQueue();
        }

        DrawDistancePreviewChart(list.GetRect(list.ColumnWidth >= 760f ? 116f : 142f), distanceRange);
        SqueakySettingsUI.LabelWithHelp(list.GetRect(Mathf.Max(28f, Text.CalcHeight("SR.Distance.Short".Translate(distanceRange.min.ToString("0.#"), distanceRange.max.ToString("0.#")), list.ColumnWidth - 24f))),
            "SR.Distance.Short".Translate(distanceRange.min.ToString("0.#"), distanceRange.max.ToString("0.#")),
            "SR.Distance.Tooltip".Translate());
    }

    private static void DrawDistancePreviewChart(Rect rect, FloatRange range)
    {
        Rect plotRect = rect.ContractedBy(12f);
        plotRect.xMin += 38f;
        plotRect.yMin += 18f;
        plotRect.yMax -= 34f;
        Rect annotationBand = new(plotRect.x, plotRect.yMax + 8f, plotRect.width, 22f);

        Widgets.DrawBoxSolid(rect, new Color(0.08f, 0.08f, 0.08f, 0.20f));
        Widgets.DrawBox(rect);
        Widgets.DrawBoxSolid(plotRect, new Color(0f, 0f, 0f, 0.18f));

        float x0 = plotRect.x;
        float xFull = Mathf.Lerp(plotRect.xMin, plotRect.xMax, Mathf.InverseLerp(15f, 65f, range.min));
        float xSilent = Mathf.Lerp(plotRect.xMin, plotRect.xMax, Mathf.InverseLerp(15f, 65f, range.max));
        float xEnd = plotRect.xMax;
        float yTop = plotRect.yMin;
        float yZero = plotRect.yMax;
        Color lineColor = new(0.76f, 0.92f, 1f, 0.95f);
        Color markerColor = new(1f, 0.78f, 0.36f, 0.75f);
        Color mutedColor = new(1f, 1f, 1f, 0.22f);

        Widgets.DrawLine(new Vector2(x0, yZero), new Vector2(xEnd, yZero), mutedColor, 1f);
        Widgets.DrawLine(new Vector2(x0, yTop), new Vector2(xEnd, yTop), mutedColor, 1f);
        Widgets.DrawLine(new Vector2(xFull, yTop), new Vector2(xFull, yZero), markerColor, 1f);
        Widgets.DrawLine(new Vector2(xSilent, yTop), new Vector2(xSilent, yZero), markerColor, 1f);
        Widgets.DrawLine(new Vector2(x0, yTop), new Vector2(xFull, yTop), lineColor, 3f);
        Widgets.DrawLine(new Vector2(xFull, yTop), new Vector2(xSilent, yZero), lineColor, 3f);
        Widgets.DrawLine(new Vector2(xSilent, yZero), new Vector2(xEnd, yZero), lineColor, 3f);

        TextAnchor oldAnchor = Text.Anchor;
        GameFont oldFont = Text.Font;
        Text.Font = GameFont.Tiny;
        Color oldColor = GUI.color;
        GUI.color = new Color(1f, 1f, 1f, 0.78f);
        Text.Anchor = TextAnchor.UpperLeft;
        Widgets.Label(new Rect(rect.x + 10f, rect.y + 8f, 72f, 20f), "SR.Distance.Chart.Volume".Translate());
        DrawDistanceAnnotationBand(annotationBand, xFull, (xFull + xSilent) * .5f, xSilent);
        Text.Anchor = TextAnchor.UpperRight;
        Widgets.Label(new Rect(plotRect.xMax - 100f, rect.y + 8f, 90f, 20f), "SR.Distance.Chart.Distance".Translate());
        GUI.color = oldColor;
        Text.Anchor = oldAnchor;
        Text.Font = oldFont;
    }

    private static void DrawDistanceAnnotationBand(Rect band, float fullAnchor, float fadeAnchor, float silentAnchor)
    {
        const float gap = 4f;
        Widgets.DrawBoxSolid(band, new Color(0f, 0f, 0f, .16f));
        string[] labels =
        {
            "SR.Distance.Chart.FullSegment".Translate(),
            "SR.Distance.Chart.FadeSegment".Translate(),
            "SR.Distance.Chart.SilentSegment".Translate()
        };
        float maxWidth = Mathf.Max(1f, (band.width - gap * 2f) / 3f);
        float[] anchors = { fullAnchor, fadeAnchor, silentAnchor };
        float[] widths = new float[labels.Length];
        float[] lefts = new float[labels.Length];
        for (int i = 0; i < labels.Length; i++)
        {
            widths[i] = Mathf.Min(maxWidth, Text.CalcSize(labels[i]).x + 10f);
            lefts[i] = Mathf.Clamp(anchors[i] - widths[i] * .5f, band.xMin, band.xMax - widths[i]);
            if (i > 0) lefts[i] = Mathf.Max(lefts[i], lefts[i - 1] + widths[i - 1] + gap);
        }
        for (int i = labels.Length - 1; i >= 0; i--)
        {
            lefts[i] = Mathf.Min(lefts[i], i == labels.Length - 1
                ? band.xMax - widths[i]
                : lefts[i + 1] - gap - widths[i]);
        }

        TextAnchor oldAnchor = Text.Anchor;
        Text.Anchor = TextAnchor.MiddleCenter;
        for (int i = 0; i < labels.Length; i++)
        {
            Rect labelRect = new(lefts[i], band.y, widths[i], band.height);
            Widgets.DrawLine(new Vector2(anchors[i], band.y - 5f), new Vector2(anchors[i], band.y), new Color(1f, .78f, .36f, .6f), 1f);
            SqueakySettingsUI.EllipsizedLabel(labelRect, labels[i], labels[i], false);
        }
        Text.Anchor = oldAnchor;
    }

    private static string DistancePresetLabel(SqueakDistancePreset preset) => ("SR.Distance.Preset." + preset).Translate();

    private void ApplyDistancePreset(SqueakDistancePreset preset)
    {
        FloatRange range = preset switch
        {
            SqueakDistancePreset.Conservative => GetDistancePresetRange(SqueakDistancePreset.Conservative),
            SqueakDistancePreset.Strong => GetDistancePresetRange(SqueakDistancePreset.Strong),
            SqueakDistancePreset.Custom => ClampDistanceRange(distanceRange),
            _ => GetDistancePresetRange(SqueakDistancePreset.Balanced),
        };
        numericBuffers.Remove("Distance.Min");
        numericBuffers.Remove("Distance.Max");
        distancePreset = preset;
        distanceRange = range;
        ApplyDistanceAndQueue();
    }

    private static FloatRange ClampDistanceRange(FloatRange range)
    {
        float min = Mathf.Clamp(range.min, 15f, 60f);
        float max = Mathf.Clamp(range.max, 20f, 65f);
        if (max < min + 5f)
        {
            max = Mathf.Min(65f, min + 5f);
        }
        return new FloatRange(min, max);
    }

    private void DrawCollapsibleHeader(Listing_Standard list, string label, ref bool open)
    {
        Rect rect = list.GetRect(34f);
        Widgets.DrawBoxSolid(rect, Mouse.IsOver(rect) ? SqueakySettingsUI.Raised : new Color(.07f, .067f, .061f, .52f));
        if (open) Widgets.DrawBoxSolid(new Rect(rect.x, rect.yMax - 2f, Mathf.Min(48f, rect.width), 1f), SqueakySettingsUI.Gold);
        Rect iconRect = new(rect.x, rect.y + 7f, 18f, 18f);
        Rect labelRect = new(iconRect.xMax + 6f, rect.y, rect.width - 24f, rect.height);
        if (Widgets.ButtonImage(iconRect, open ? TexButton.Collapse : TexButton.Reveal) || Widgets.ButtonInvisible(labelRect))
        {
            open = !open;
        }

        TextAnchor oldAnchor = Text.Anchor;
        GameFont oldFont = Text.Font;
        Text.Anchor = TextAnchor.MiddleLeft;
        Text.Font = GameFont.Medium;
        Widgets.Label(labelRect, label);
        Text.Font = oldFont;
        Text.Anchor = oldAnchor;
    }

    private static string MoodFieldKey(SqueakMood mood, string field) => mood + "." + field;

    private float DrawSliderWithField(Rect rect, string fieldKey, string label, float value, float min, float max,
        bool enabled = true, string disabledReason = "")
    {
        Widgets.DrawBoxSolid(rect, new Color(.07f, .067f, .061f, .52f));
        const float gap = 6f;
        float labelWidth = 120f;
        float fieldWidth = Mathf.Clamp(rect.width * .18f, 64f, 92f);
        if (rect.width < 430f) labelWidth = Mathf.Clamp(rect.width * .32f, 94f, 120f);
        float sliderWidth = rect.width - labelWidth - fieldWidth - (gap * 2f);
        Rect labelRect = new(rect.x, rect.y, labelWidth, rect.height);
        Rect sliderRect = new(labelRect.xMax + gap, rect.y, sliderWidth, rect.height);
        Rect fieldRect = new(sliderRect.xMax + gap, rect.y, fieldWidth, rect.height);
        if (!numericBuffers.TryGetValue(fieldKey, out string buffer))
        {
            buffer = value.ToString("0.##");
        }

        bool oldEnabled = GUI.enabled;
        GUI.enabled = oldEnabled && enabled;
        if (!enabled && !disabledReason.NullOrEmpty())
            SqueakySettingsUI.LabelWithHelp(labelRect, label, disabledReason);
        else Widgets.Label(labelRect, label);
        float sliderValue = Widgets.HorizontalSlider(sliderRect, value, min, max);
        if (Math.Abs(sliderValue - value) > 0.0001f)
        {
            value = sliderValue;
            buffer = value.ToString("0.##");
        }

        Widgets.TextFieldNumeric(fieldRect, ref value, ref buffer, min, max);
        GUI.enabled = oldEnabled;
        if (enabled) numericBuffers[fieldKey] = buffer;
        return Mathf.Clamp(value, min, max);
    }
}
