using System;
using System.Collections.Generic;
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
}

/// <summary>
/// 玩家配置。承载:
///  - useCustomOnly:音源开关(纯自定义 override / 混合原版 default)
///  - moodOverrides:心情调制 override(字段级覆盖 CompProperties 默认,用于换音频后补偿)
/// </summary>
public class SqueakyRatkinSettings : ModSettings
{
    public bool useCustomOnly = false;
    public Dictionary<SqueakMood, SqueakMoodMod> moodOverrides = new();

    private static readonly SqueakMood[] Moods = { SqueakMood.Good, SqueakMood.Neutral, SqueakMood.Bad, SqueakMood.Break };
    private static readonly SqueakAction[] Actions = { SqueakAction.Call, SqueakAction.Eat, SqueakAction.Sleep, SqueakAction.Wounded, SqueakAction.Select, SqueakAction.Move, SqueakAction.Social, SqueakAction.Joy };

    private SqueakMood selectedMood = SqueakMood.Neutral;
    private SqueakAction selectedAction = SqueakAction.Call;
    private Vector2 scrollPos;
    private readonly Dictionary<string, string> numericBuffers = new();

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
        float height = 260f;
        if (moodOverrides.ContainsKey(selectedMood))
        {
            height += 170f;
        }

        return height;
    }

    private void DrawWorkbenchContents(Rect rect)
    {
        Listing_Standard list = new();
        list.Begin(rect);

        list.Label("SR.Workbench.Header".Translate());
        list.Label("SR.Workbench.Header.Desc".Translate());
        list.Gap(8f);

        Rect moodRect = list.GetRect(32f);
        if (Widgets.ButtonText(moodRect, "SR.Workbench.Mood".Translate() + ": " + SqueakLabels.Mood(selectedMood)))
        {
            List<FloatMenuOption> options = new();
            foreach (SqueakMood mood in Moods)
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
            foreach (SqueakAction action in Actions)
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
        }

        if (toggle && moodOverrides.TryGetValue(selectedMood, out SqueakMoodMod mod))
        {
            list.Gap(6f);
            list.Label("SR.Workbench.Preset".Translate());
            DrawPresetButtons(list.GetRect(32f), mod);
            list.Gap(6f);

            mod.pitchFactor = DrawSliderWithField(list.GetRect(32f), MoodFieldKey(selectedMood, "PitchFactor"), "SR.Workbench.PitchFactor".Translate(), mod.pitchFactor, 0.5f, 2f);
            mod.volumeFactor = DrawSliderWithField(list.GetRect(32f), MoodFieldKey(selectedMood, "VolumeFactor"), "SR.Workbench.VolumeFactor".Translate(), mod.volumeFactor, 0f, 2f);
            mod.pitchJitter.min = DrawSliderWithField(list.GetRect(32f), MoodFieldKey(selectedMood, "PitchJitterMin"), "SR.Workbench.PitchJitter".Translate() + " " + "SR.Workbench.JitterMin".Translate(), mod.pitchJitter.min, 0.5f, 1.5f);
            mod.pitchJitter.max = DrawSliderWithField(list.GetRect(32f), MoodFieldKey(selectedMood, "PitchJitterMax"), "SR.Workbench.PitchJitter".Translate() + " " + "SR.Workbench.JitterMax".Translate(), mod.pitchJitter.max, 0.5f, 1.5f);
            if (mod.pitchJitter.max < mod.pitchJitter.min)
            {
                mod.pitchJitter.max = mod.pitchJitter.min;
            }
        }

        Rect previewRect = list.GetRect(32f);
        if (Widgets.ButtonText(previewRect, "SR.Workbench.Preview".Translate()))
        {
            SoundDef def = DefDatabase<SoundDef>.GetNamedSilentFail("SR_" + selectedAction);
            if (def != null)
            {
                SoundInfo info = SoundInfo.OnCamera();
                SqueakMoodMod previewMod = moodOverrides.TryGetValue(selectedMood, out SqueakMoodMod currentMod)
                    ? currentMod
                    : new SqueakMoodMod { mood = selectedMood };
                info.pitchFactor = previewMod.pitchFactor * previewMod.pitchJitter.RandomInRange;
                info.volumeFactor = previewMod.volumeFactor;
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
