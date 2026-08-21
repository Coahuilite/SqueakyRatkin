using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace SqueakyRatkin;

/// <summary>
/// 0.2.4 主层设置契约（v0.2.4 tag，SqueakyRatkinSettings.cs）。
/// 注意：本类型不是 0.2.4 文件链接，而是「提取的序列化契约」——
///   * 字段集合与 ExposeData 语句逐行对应 0.2.4（见下方行号注释）；
///   * 序列化语义（省略默认/forceSave/嵌套结构）由 Stubs/ScribeStubs.cs 的真实规则执行；
///   * round-trip 自检（save→load→save 字节稳定）在 Program.cs 强制；
///   * 类名/命名空间与真实类型一致，供真实链接的 SqueakGlobalActionPolicy.Publish 编译。
/// 实机可用后（0.2.4 游戏内保存设置），应以真实 ModSettings XML 替换 fixtures/ 样本并重新断言。
/// </summary>
public sealed class SqueakyRatkinSettings : ModSettings
{
    // v3 (0.2.4): SqueakyRatkinSettings.cs L39-40
    public const int CurrentSettingsSchemaVersion = 3;
    private static readonly FloatRange FallbackBalancedDistanceRange = new(15f, 50f);

    public SqueakVoicePackMode voicePackMode = SqueakVoicePackMode.Fallback;          // L47
    public int voicePackSchemaVersion = 1;                                            // L48
    public bool voicePackDefaultSeeded;                                               // L51
    public int settingsSchemaVersion = CurrentSettingsSchemaVersion;                  // L53
    public bool scaleCooldownWithTimeSpeed = true;                                    // L54
    public bool scaleFrequencyWithTalking = true;                                     // L55 (default; XML override via CompProperties)
    public bool scalePeriodicWithAudiblePopulation = true;                            // L56
    public bool localizeDebugActions;                                                 // L57
    public bool developerToolsEnabled;                                                // L58
    public SqueakDevLoggingMode devLoggingMode = SqueakDevLoggingMode.Auto;           // L59
    public float globalCooldownMultiplier = 1f;                                       // L60
    public SqueakDistancePreset distancePreset = SqueakDistancePreset.Balanced;       // L61
    public FloatRange distanceRange = new(15f, 50f);                                  // L62 (FallbackBalancedDistanceRange)

    public Dictionary<SqueakMood, SqueakMoodMod> moodOverrides = new();               // L65
    public List<VoicePackSelectionRecord> voicePackSelections = new();                // L66
    public List<XenotypePresetRecord> xenotypePresets = new();                        // L67
    public List<GlobalActionEnabledRecord> globalActionEnabled = new();               // L69

    private bool distanceRangeWasLoaded;
    private bool scaleFrequencyWithTalkingWasLoaded;
    private bool settingsSchemaWasLoaded;
    private bool voicePackModeWasLoaded;

    // ExposeData 与 0.2.4 L114-249 逐行对应（桩模式，无 UI 依赖）。
    public override void ExposeData()
    {
        base.ExposeData();
        if (Scribe.mode == LoadSaveMode.LoadingVars)
        {
            // Reset presence flags at the beginning of LoadingVars so they survive through PostLoadInit (0.2.4 L117-124).
            distanceRangeWasLoaded = false;
            scaleFrequencyWithTalkingWasLoaded = false;
            settingsSchemaWasLoaded = false;
            voicePackModeWasLoaded = false;
        }
        Scribe_Values.Look(ref voicePackMode, "voicePackMode", SqueakVoicePackMode.Fallback);
        Scribe_Values.Look(ref voicePackSchemaVersion, "voicePackSchemaVersion", 1);
        Scribe_Values.Look(ref voicePackDefaultSeeded, "voicePackDefaultSeeded", false);
        Scribe_Values.Look(ref settingsSchemaVersion, "settingsSchemaVersion", CurrentSettingsSchemaVersion, forceSave: true);
        Scribe_Values.Look(ref scaleCooldownWithTimeSpeed, "scaleCooldownWithTimeSpeed", true);
        Scribe_Values.Look(ref scaleFrequencyWithTalking, "scaleFrequencyWithTalking", true);
        Scribe_Values.Look(ref scalePeriodicWithAudiblePopulation, "scalePeriodicWithAudiblePopulation", true);
        Scribe_Values.Look(ref localizeDebugActions, "localizeDebugActions", false);
        Scribe_Values.Look(ref developerToolsEnabled, "developerToolsEnabled", false);
        Scribe_Values.Look(ref devLoggingMode, "devLoggingMode", SqueakDevLoggingMode.Auto);
        Scribe_Values.Look(ref globalCooldownMultiplier, "globalCooldownMultiplier", 1f);
        Scribe_Values.Look(ref distancePreset, "distancePreset", SqueakDistancePreset.Balanced);
        Scribe_Values.Look(ref distanceRange, "distanceRange", FallbackBalancedDistanceRange);
        if (Scribe.mode == LoadSaveMode.LoadingVars)
        {
            scaleFrequencyWithTalkingWasLoaded = Scribe.loader.curXmlParent["scaleFrequencyWithTalking"] != null;
            distanceRangeWasLoaded = Scribe.loader.curXmlParent["distanceRange"] != null;
            settingsSchemaWasLoaded = Scribe.loader.curXmlParent["settingsSchemaVersion"] != null;
            voicePackModeWasLoaded = Scribe.loader.curXmlParent["voicePackMode"] != null;
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
            voicePackSchemaVersion = 1;
            bool schemaUpgradeNeeded = !settingsSchemaWasLoaded || settingsSchemaVersion < CurrentSettingsSchemaVersion;
            if (schemaUpgradeNeeded)
            {
                settingsSchemaVersion = CurrentSettingsSchemaVersion;
            }
            globalCooldownMultiplier = Math.Max(0f, Math.Min(globalCooldownMultiplier, 3f));
            if (!scaleFrequencyWithTalkingWasLoaded)
            {
                scaleFrequencyWithTalking = true;
            }
            if (!distanceRangeWasLoaded)
            {
                distanceRange = FallbackBalancedDistanceRange;
            }
            distanceRange = ClampDistanceRange(distanceRange);
            if (voicePackSelections == null) voicePackSelections = new List<VoicePackSelectionRecord>();
            if (xenotypePresets == null) xenotypePresets = new List<XenotypePresetRecord>();
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

    public SqueakActionScope GetActionGlobalScope(SqueakAction action)
    {
        SqueakActionScope scope = SqueakActionDefinitions.Get(action).DefaultScope;
        foreach (GlobalActionEnabledRecord record in globalActionEnabled ?? new List<GlobalActionEnabledRecord>())
            if (record != null && record.action == action) scope = SqueakActionDefinitions.NormalizeScope(action, record.scope);
        return scope;
    }

    public bool IsActionGloballyEnabled(SqueakAction action) => GetActionGlobalScope(action) != SqueakActionScope.Disabled;

    public static FloatRange ClampDistanceRange(FloatRange range)
    {
        float min = Math.Max(15f, Math.Min(range.min, 60f));
        float max = Math.Max(20f, Math.Min(range.max, 65f));
        return new FloatRange(min, max);
    }
}
