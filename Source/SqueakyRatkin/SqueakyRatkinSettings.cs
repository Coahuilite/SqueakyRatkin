using System;
using System.Collections.Generic;
using System.Linq;
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
///  - useCustomOnly:音源开关(纯自定义 override / 混合原版 default)
///  - moodOverrides:心情调制 override(字段级覆盖 CompProperties 默认,用于换音频后补偿)
/// 工作台用 editBuffer 编辑副本:slider/preset/预览都作用于 buffer,点「写入」才存 moodOverrides,避免试听时误改保存值。
/// </summary>
public class SqueakyRatkinSettings : ModSettings
{
    private static readonly FloatRange FallbackBalancedDistanceRange = new(15f, 50f);

    public bool useCustomOnly = false;
    public bool scaleCooldownWithTimeSpeed = true;
    public float globalCooldownMultiplier = 1f;
    public SqueakDistancePreset distancePreset = SqueakDistancePreset.Balanced;
    public FloatRange distanceRange = new(15f, 50f);
    public Dictionary<SqueakMood, SqueakMoodMod> moodOverrides = new();

    // 数据驱动:mood/action 列表从所有挂 CompProperties_Squeaker 的 ThingDef 读(XML actions/moodMods)。
    // XML 加配置自动出现在工作台,无需改 C# 数组。DefDatabase 加载后不变,首次访问懒加载缓存。
    private static List<SqueakMood>? _configuredMoods;
    private static List<SqueakAction>? _configuredActions;

    private SqueakMood selectedMood = SqueakMood.Neutral;
    private SqueakAction selectedAction = SqueakAction.Call;
    private Vector2 scrollPos;
    private readonly Dictionary<string, string> numericBuffers = new();
    private bool distanceRangeWasLoaded;

    // 编辑缓冲:slider/preset/预览作用于 editBuffer,「写入」才同步到 moodOverrides。
    private SqueakMoodMod? editBuffer;
    private SqueakMood? bufferForMood;

    public override void ExposeData()
    {
        base.ExposeData();
        distanceRangeWasLoaded = false;
        Scribe_Values.Look(ref useCustomOnly, "useCustomOnly", false);
        Scribe_Values.Look(ref scaleCooldownWithTimeSpeed, "scaleCooldownWithTimeSpeed", true);
        Scribe_Values.Look(ref globalCooldownMultiplier, "globalCooldownMultiplier", 1f);
        Scribe_Values.Look(ref distancePreset, "distancePreset", SqueakDistancePreset.Balanced);
        Scribe_Values.Look(ref distanceRange, "distanceRange", GetDistancePresetRange(SqueakDistancePreset.Balanced));
        if (Scribe.mode == LoadSaveMode.LoadingVars)
        {
            distanceRangeWasLoaded = Scribe.loader?.curXmlParent?["distanceRange"] != null;
        }
        Scribe_Collections.Look(ref moodOverrides, "moodOverrides", LookMode.Value, LookMode.Deep);
        if (Scribe.mode == LoadSaveMode.LoadingVars && moodOverrides == null)
        {
            moodOverrides = new Dictionary<SqueakMood, SqueakMoodMod>();
        }

        if (Scribe.mode == LoadSaveMode.PostLoadInit)
        {
            globalCooldownMultiplier = Mathf.Clamp(globalCooldownMultiplier, 0f, 3f);
            if (!distanceRangeWasLoaded)
            {
                distanceRange = GetDistancePresetRange(SqueakDistancePreset.Balanced);
            }
            distanceRange = ClampDistanceRange(distanceRange);
        }
    }

    public void ApplyToRuntime()
    {
        CompSqueaker.UseCustomOnly = useCustomOnly;
        CompSqueaker.ScaleCooldownWithTimeSpeed = scaleCooldownWithTimeSpeed;
        CompSqueaker.GlobalCooldownMultiplier = Mathf.Clamp(globalCooldownMultiplier, 0f, 3f);
        CompSqueaker.ApplyDistanceRange(ClampDistanceRange(distanceRange));
    }

    private static List<SqueakMood> ConfiguredMoods
    {
        get
        {
            if (_configuredMoods == null) { RefreshConfigured(); }

            return _configuredMoods!;
        }
    }

    private static List<SqueakAction> ConfiguredActions
    {
        get
        {
            if (_configuredActions == null) { RefreshConfigured(); }

            return _configuredActions!;
        }
    }

    private static IEnumerable<CompProperties_Squeaker> ConfiguredSqueakers()
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

    /// <summary>选 mood 变化或启用状态变时,从 moodOverrides 重建 editBuffer,清 numericBuffers 强制刷新输入框显示。</summary>
    private void SyncBufferFromSaved()
    {
        // 有 override 用 override;否则用 XML 分发默认(而非空白 1/1),让切 mood 时显示该 mood 的实际生效值。
        editBuffer = moodOverrides.TryGetValue(selectedMood, out SqueakMoodMod saved)
            ? saved.Clone()
            : (GetDefaultMoodMod(selectedMood)?.Clone() ?? new SqueakMoodMod { mood = selectedMood });
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
        const float topHeight = 340f;
        float workbenchHeight = Mathf.Max(0f, rect.height - topHeight);

        Listing_Standard topList = new();
        topList.Begin(new Rect(rect.x, rect.y, rect.width, topHeight));
        topList.Label("SR.Global.Header".Translate());
        topList.CheckboxLabeled("SR.UseCustomOnly.Label".Translate(), ref useCustomOnly);
        topList.CheckboxLabeled("SR.ScaleCooldownWithTimeSpeed.Label".Translate(), ref scaleCooldownWithTimeSpeed);
        topList.Label("SR.GlobalCooldownMultiplier.Label".Translate(globalCooldownMultiplier.ToString("0.##")));
        globalCooldownMultiplier = topList.Slider(globalCooldownMultiplier, 0f, 3f);
        topList.Gap(8f);
        topList.GapLine();
        DrawDistanceSettings(topList);
        topList.Gap(8f);
        topList.GapLine();
        topList.End();

        Rect workbenchRect = new(rect.x, rect.y + topHeight, rect.width, workbenchHeight);
        float contentHeight = GetWorkbenchHeight();

        if (contentHeight > workbenchRect.height)
        {
            Rect viewRect = new(0f, 0f, workbenchRect.width - 16f, contentHeight);
            Widgets.BeginScrollView(workbenchRect, ref scrollPos, viewRect);
            DrawWorkbenchContents(viewRect);
            Widgets.EndScrollView();
            return;
        }

        DrawWorkbenchContents(workbenchRect);
    }

    private float GetWorkbenchHeight()
    {
        // 标题区 + mood/action/enable 三行 + 预览按钮
        float height = 260f;
        if (moodOverrides.ContainsKey(selectedMood))
        {
            // preset + 4 slider + 写入/还原按钮行
            height += 170f + 38f;
        }

        return height;
    }

    private void DrawDistanceSettings(Listing_Standard list)
    {
        list.Label("SR.Distance.Header".Translate());
        Rect presetRect = list.GetRect(32f);
        if (Widgets.ButtonText(presetRect, "SR.Distance.Preset".Translate() + ": " + DistancePresetLabel(distancePreset)))
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
            ApplyToRuntime();
        }

        DrawDistancePreviewChart(list.GetRect(78f), distanceRange);
        list.Label("SR.Distance.Desc".Translate(distanceRange.min.ToString("0.#"), distanceRange.max.ToString("0.#")));
    }

    private static void DrawDistancePreviewChart(Rect rect, FloatRange range)
    {
        Rect plotRect = rect.ContractedBy(6f);
        plotRect.xMin += 34f;
        plotRect.yMin += 4f;
        plotRect.yMax -= 18f;

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
        Widgets.Label(new Rect(rect.x + 6f, rect.y + 4f, 44f, 20f), "SR.Distance.Chart.Volume".Translate());
        Widgets.Label(new Rect(plotRect.xMin, plotRect.yMax + 1f, plotRect.width * 0.5f, 18f), "SR.Distance.Chart.Full".Translate(range.min.ToString("0.#")));
        Text.Anchor = TextAnchor.UpperRight;
        Widgets.Label(new Rect(plotRect.xMin + (plotRect.width * 0.5f), plotRect.yMax + 1f, plotRect.width * 0.5f, 18f), "SR.Distance.Chart.Silent".Translate(range.max.ToString("0.#")));
        Text.Anchor = TextAnchor.UpperRight;
        Widgets.Label(new Rect(plotRect.xMax - 100f, rect.y + 4f, 100f, 20f), "SR.Distance.Chart.Distance".Translate());
        GUI.color = oldColor;
        Text.Anchor = oldAnchor;
        Text.Font = oldFont;
    }

    private static string DistancePresetLabel(SqueakDistancePreset preset) => ("SR.Distance.Preset." + preset).Translate();

    private void ApplyDistancePreset(SqueakDistancePreset preset)
    {
        distancePreset = preset;
        distanceRange = preset switch
        {
            SqueakDistancePreset.Conservative => GetDistancePresetRange(SqueakDistancePreset.Conservative),
            SqueakDistancePreset.Strong => GetDistancePresetRange(SqueakDistancePreset.Strong),
            SqueakDistancePreset.Custom => ClampDistanceRange(distanceRange),
            _ => GetDistancePresetRange(SqueakDistancePreset.Balanced),
        };
        numericBuffers.Remove("Distance.Min");
        numericBuffers.Remove("Distance.Max");
        ApplyToRuntime();
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

    private void DrawWorkbenchContents(Rect rect)
    {
        EnsureBuffer();

        Listing_Standard list = new();
        list.Begin(rect);

        list.Label("SR.Workbench.Header".Translate());
        list.Label("SR.Workbench.Header.Desc".Translate());
        list.Gap(8f);

        Rect moodRect = list.GetRect(32f);
        if (Widgets.ButtonText(moodRect, "SR.Workbench.Mood".Translate() + ": " + SqueakLabels.Mood(selectedMood)))
        {
            List<FloatMenuOption> options = new();
            foreach (SqueakMood mood in ConfiguredMoods)
            {
                SqueakMood localMood = mood;
                options.Add(new FloatMenuOption(SqueakLabels.Mood(localMood), () => selectedMood = localMood));
            }

            Find.WindowStack.Add(new FloatMenu(options));
        }

        Rect actionRect = list.GetRect(32f);
        if (Widgets.ButtonText(actionRect, "SR.Workbench.Action".Translate() + ": " + SqueakLabels.Action(selectedAction)))
        {
            List<FloatMenuOption> options = new();
            foreach (SqueakAction action in ConfiguredActions)
            {
                SqueakAction localAction = action;
                options.Add(new FloatMenuOption(SqueakLabels.Action(localAction), () => selectedAction = localAction));
            }

            Find.WindowStack.Add(new FloatMenu(options));
        }

        bool enabled = moodOverrides.ContainsKey(selectedMood);
        bool toggle = enabled;
        list.CheckboxLabeled("SR.Workbench.EnableOverride".Translate(), ref toggle);
        if (toggle != enabled)
        {
            if (toggle)
            {
                moodOverrides[selectedMood] = new SqueakMoodMod { mood = selectedMood };
            }
            else
            {
                moodOverrides.Remove(selectedMood);
            }

            SyncBufferFromSaved();
        }

        if (toggle && editBuffer != null)
        {
            list.Gap(6f);
            list.Label("SR.Workbench.Preset".Translate());
            DrawPresetButtons(list.GetRect(32f), editBuffer);
            list.Gap(6f);

            editBuffer.pitchFactor = DrawSliderWithField(list.GetRect(32f), MoodFieldKey(selectedMood, "PitchFactor"), "SR.Workbench.PitchFactor".Translate(), editBuffer.pitchFactor, 0.5f, 2f);
            editBuffer.volumeFactor = DrawSliderWithField(list.GetRect(32f), MoodFieldKey(selectedMood, "VolumeFactor"), "SR.Workbench.VolumeFactor".Translate(), editBuffer.volumeFactor, 0f, 2f);
            editBuffer.pitchJitter.min = DrawSliderWithField(list.GetRect(32f), MoodFieldKey(selectedMood, "PitchJitterMin"), "SR.Workbench.PitchJitter".Translate() + " " + "SR.Workbench.JitterMin".Translate(), editBuffer.pitchJitter.min, 0.5f, 1.5f);
            editBuffer.pitchJitter.max = DrawSliderWithField(list.GetRect(32f), MoodFieldKey(selectedMood, "PitchJitterMax"), "SR.Workbench.PitchJitter".Translate() + " " + "SR.Workbench.JitterMax".Translate(), editBuffer.pitchJitter.max, 0.5f, 1.5f);
            if (editBuffer.pitchJitter.max < editBuffer.pitchJitter.min)
            {
                editBuffer.pitchJitter.max = editBuffer.pitchJitter.min;
            }

            // 写入/还原/默认:slider/preset 只改 editBuffer。写入→存 moodOverrides;还原→回到上次保存;默认→回到 XML 分发默认值。
            Rect btnRect = list.GetRect(32f);
            const float btnGap = 6f;
            float thirdW = (btnRect.width - btnGap * 2) / 3f;
            Rect applyRect = new(btnRect.x, btnRect.y, thirdW, btnRect.height);
            Rect revertRect = new(btnRect.x + thirdW + btnGap, btnRect.y, thirdW, btnRect.height);
            Rect defaultsRect = new(btnRect.x + ((thirdW + btnGap) * 2), btnRect.y, thirdW, btnRect.height);
            if (Widgets.ButtonText(applyRect, "SR.Workbench.Apply".Translate()))
            {
                moodOverrides[selectedMood] = editBuffer.Clone();
            }

            if (Widgets.ButtonText(revertRect, "SR.Workbench.Revert".Translate()))
            {
                SyncBufferFromSaved();
            }

            if (Widgets.ButtonText(defaultsRect, "SR.Workbench.RestoreDefaults".Translate()))
            {
                SqueakMoodMod? def = GetDefaultMoodMod(selectedMood);
                editBuffer = def?.Clone() ?? new SqueakMoodMod { mood = selectedMood };
                numericBuffers.Clear();
            }
        }

        Rect previewRect = list.GetRect(32f);
        if (Widgets.ButtonText(previewRect, "SR.Workbench.Preview".Translate()))
        {
            // 试听走 _Preview def(onCamera=True,无 distRange 衰减),听者恒=相机,任何镜头缩放都全音量。
            // pitch/volume factor 取自 editBuffer(当前编辑值,未写入也能预览效果)。无 _Preview 时 fallback 原 def。
            EnsureBuffer();
            SoundDef def = DefDatabase<SoundDef>.GetNamedSilentFail("SR_" + selectedAction + "_Preview")
                ?? DefDatabase<SoundDef>.GetNamedSilentFail("SR_" + selectedAction);
            if (def != null && editBuffer != null)
            {
                SoundInfo info = SoundInfo.OnCamera();
                info.pitchFactor = editBuffer.pitchFactor * editBuffer.pitchJitter.RandomInRange;
                info.volumeFactor = editBuffer.volumeFactor;
                def.PlayOneShot(info);
            }
        }

        list.End();
    }

    private void DrawPresetButtons(Rect rect, SqueakMoodMod mod)
    {
        const float gap = 6f;
        float buttonWidth = (rect.width - (gap * 3f)) / 4f;
        Rect sharpRect = new(rect.x, rect.y, buttonWidth, rect.height);
        Rect neutralRect = new(rect.x + buttonWidth + gap, rect.y, buttonWidth, rect.height);
        Rect lowRect = new(rect.x + ((buttonWidth + gap) * 2f), rect.y, buttonWidth, rect.height);
        Rect chaosRect = new(rect.x + ((buttonWidth + gap) * 3f), rect.y, buttonWidth, rect.height);

        if (Widgets.ButtonText(sharpRect, "SR.Preset.Sharp".Translate()))
        {
            ApplyPreset(mod, 1.25f, 1.3f, 1f, 1f);
        }

        if (Widgets.ButtonText(neutralRect, "SR.Preset.Neutral".Translate()))
        {
            ApplyPreset(mod, 1f, 1f, 1f, 1f);
        }

        if (Widgets.ButtonText(lowRect, "SR.Preset.Low".Translate()))
        {
            ApplyPreset(mod, 0.8f, 0.75f, 1f, 1f);
        }

        if (Widgets.ButtonText(chaosRect, "SR.Preset.Chaos".Translate()))
        {
            ApplyPreset(mod, 1f, 1.5f, 0.7f, 1.4f);
        }
    }

    private void ApplyPreset(SqueakMoodMod mod, float pitchFactor, float volumeFactor, float jitterMin, float jitterMax)
    {
        mod.pitchFactor = pitchFactor;
        mod.volumeFactor = volumeFactor;
        mod.pitchJitter = new FloatRange(jitterMin, jitterMax);
        // 清输入框缓冲,下帧从 editBuffer(新 preset 值)重新显示,解决 preset 后显示不刷新。
        numericBuffers.Clear();
    }

    private static string MoodFieldKey(SqueakMood mood, string field) => mood + "." + field;

    private float DrawSliderWithField(Rect rect, string fieldKey, string label, float value, float min, float max)
    {
        const float gap = 6f;
        float labelWidth = 120f;
        float fieldWidth = rect.width * 0.3f;
        float sliderWidth = rect.width - labelWidth - fieldWidth - (gap * 2f);
        Rect labelRect = new(rect.x, rect.y, labelWidth, rect.height);
        Rect sliderRect = new(labelRect.xMax + gap, rect.y, sliderWidth, rect.height);
        Rect fieldRect = new(sliderRect.xMax + gap, rect.y, fieldWidth, rect.height);
        if (!numericBuffers.TryGetValue(fieldKey, out string buffer))
        {
            buffer = value.ToString("0.##");
        }

        Widgets.Label(labelRect, label);
        float sliderValue = Widgets.HorizontalSlider(sliderRect, value, min, max);
        if (Math.Abs(sliderValue - value) > 0.0001f)
        {
            value = sliderValue;
            buffer = value.ToString("0.##");
        }

        Widgets.TextFieldNumeric(fieldRect, ref value, ref buffer, min, max);
        numericBuffers[fieldKey] = buffer;
        return Mathf.Clamp(value, min, max);
    }
}
