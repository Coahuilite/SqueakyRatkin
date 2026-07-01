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

/// <summary>
/// 玩家配置。承载:
///  - useCustomOnly:音源开关(纯自定义 override / 混合原版 default)
///  - moodOverrides:心情调制 override(字段级覆盖 CompProperties 默认,用于换音频后补偿)
/// 工作台用 editBuffer 编辑副本:slider/preset/预览都作用于 buffer,点「写入」才存 moodOverrides,避免试听时误改保存值。
/// </summary>
public class SqueakyRatkinSettings : ModSettings
{
    public bool useCustomOnly = false;
    public Dictionary<SqueakMood, SqueakMoodMod> moodOverrides = new();

    // 数据驱动:mood/action 列表从所有挂 CompProperties_Squeaker 的 ThingDef 读(XML actions/moodMods)。
    // XML 加配置自动出现在工作台,无需改 C# 数组。DefDatabase 加载后不变,首次访问懒加载缓存。
    private static List<SqueakMood>? _configuredMoods;
    private static List<SqueakAction>? _configuredActions;

    private SqueakMood selectedMood = SqueakMood.Neutral;
    private SqueakAction selectedAction = SqueakAction.Call;
    private Vector2 scrollPos;
    private readonly Dictionary<string, string> numericBuffers = new();

    // 编辑缓冲:slider/preset/预览作用于 editBuffer,「写入」才同步到 moodOverrides。
    private SqueakMoodMod? editBuffer;
    private SqueakMood? bufferForMood;

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref useCustomOnly, "useCustomOnly", false);
        Scribe_Collections.Look(ref moodOverrides, "moodOverrides", LookMode.Value, LookMode.Deep);
        if (Scribe.mode == LoadSaveMode.LoadingVars && moodOverrides == null)
        {
            moodOverrides = new Dictionary<SqueakMood, SqueakMoodMod>();
        }
    }

    public void ApplyToRuntime()
    {
        CompSqueaker.UseCustomOnly = useCustomOnly;
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

    private static void RefreshConfigured()
    {
        var moods = new List<SqueakMood>();
        var actions = new List<SqueakAction>();
        foreach (ThingDef def in DefDatabase<ThingDef>.AllDefs)
        {
            if (def.comps == null) { continue; }

            foreach (CompProperties cp in def.comps)
            {
                if (cp is not CompProperties_Squeaker sq) { continue; }

                foreach (SqueakActionConfig cfg in sq.actions)
                {
                    if (!actions.Contains(cfg.action)) { actions.Add(cfg.action); }
                }

                foreach (SqueakMoodMod mod in sq.moodMods)
                {
                    if (!moods.Contains(mod.mood)) { moods.Add(mod.mood); }
                }
            }
        }

        _configuredMoods = moods;
        _configuredActions = actions;
    }

    /// <summary>从 CompProperties_Squeaker(XML 分发默认)取指定 mood 的默认 moodMod,供「还原默认」按钮用。</summary>
    private static SqueakMoodMod? GetDefaultMoodMod(SqueakMood mood)
    {
        foreach (ThingDef def in DefDatabase<ThingDef>.AllDefs)
        {
            if (def.comps == null) { continue; }

            foreach (CompProperties cp in def.comps)
            {
                if (cp is not CompProperties_Squeaker sq) { continue; }

                foreach (SqueakMoodMod m in sq.moodMods)
                {
                    if (m.mood == mood) { return m; }
                }
            }
        }

        return null;
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
        const float topHeight = 58f;
        float workbenchHeight = Mathf.Max(0f, rect.height - topHeight);

        Listing_Standard topList = new();
        topList.Begin(new Rect(rect.x, rect.y, rect.width, topHeight));
        topList.CheckboxLabeled("SR.UseCustomOnly.Label".Translate(), ref useCustomOnly);
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
