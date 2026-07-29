using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace SqueakyRatkin;

public partial class SqueakyRatkinSettings
{
    private Vector2 soundMoodScroll;
    private bool moodPreviewSectionOpen;
    private string materialPreviewStatus = "";
    private bool materialPreviewStatusFailed;
    private string finalPreviewStatus = "";
    private bool finalPreviewStatusFailed;
    private bool moodEditorDirty;

    private void DrawSoundMoodSettings(Rect rect)
    {
        Rect scrollRect = rect;
        float viewWidth = scrollRect.width;
        float contentHeight = MeasureSoundMoodContentHeight(viewWidth);
        if (contentHeight > scrollRect.height)
        {
            viewWidth = Mathf.Max(1f, scrollRect.width - 16f);
            contentHeight = MeasureSoundMoodContentHeight(viewWidth);
        }
        soundMoodScroll.y = Mathf.Clamp(soundMoodScroll.y, 0f, Mathf.Max(0f, contentHeight - scrollRect.height));
        Rect view = new(0f, 0f, viewWidth, Mathf.Max(scrollRect.height, contentHeight));
        Widgets.BeginScrollView(scrollRect, ref soundMoodScroll, view);
        Listing_Standard list = new(); list.maxOneColumn = true; list.Begin(view);
        DrawCompactPageIntro(list, "SR.SoundMood.Header".Translate(), "SR.SoundMood.PageDesc".Translate());
        EnsureBuffer();
        SqueakMood[] moods = { SqueakMood.Good, SqueakMood.Neutral, SqueakMood.Bad, SqueakMood.Break };
        bool stackMoodCards = list.ColumnWidth < 620f;
        float moodRowHeight = stackMoodCards ? 150f : 72f;
        Rect moodRow = list.GetRect(moodRowHeight);
        float moodWidth = stackMoodCards ? (moodRow.width - 6f) * .5f : (moodRow.width - 18f) / 4f;
        for (int i = 0; i < moods.Length; i++)
        {
            SqueakMood mood = moods[i];
            Rect card = stackMoodCards
                ? new Rect(moodRow.x + (i % 2) * (moodWidth + 6f), moodRow.y + (i / 2) * 75f, moodWidth, 69f)
                : new Rect(moodRow.x + i * (moodWidth + 6f), moodRow.y, moodWidth, moodRow.height);
            if (SqueakySettingsUI.SelectableCard(card, SqueakLabels.Mood(mood), moodOverrides.ContainsKey(mood) ? "SR.SoundMood.Override".Translate() : "SR.SoundMood.Xml".Translate(), selectedMood == mood))
            {
                CommitMoodEditorNow();
                selectedMood = mood;
                SyncBufferFromSaved();
            }
        }
        list.Gap(8f);
        DrawSectionHeader(list, "SR.SoundMood.ToneSection".Translate());
        Rect actionRow = list.GetRect(34f);
        string actionLabel = "SR.Workbench.Action".Translate() + ": " + SqueakLabels.Action(selectedAction);
        if (SqueakySettingsUI.SettingSelector(actionRow, actionLabel, true, "SR.SoundMood.Action.Tooltip".Translate()))
        {
            List<FloatMenuOption> options = ConfiguredActions.Select(action =>
            {
                SqueakAction local = action;
                return new FloatMenuOption(SqueakLabels.Action(local) + "  ·  " + local, () => { selectedAction = local; moodClipIndex = 0; });
            }).ToList();
            if (options.Count > 0) Find.WindowStack.Add(new FloatMenu(options));
        }
        bool toggle = editBufferOverrideEnabled;
        SqueakySettingsUI.Toggle(list.GetRect(34f), "SR.Workbench.EnableOverride".Translate(), ref toggle,
            tooltip: "SR.SoundMood.Override.Tooltip".Translate());
        if (toggle != editBufferOverrideEnabled)
        {
            editBufferOverrideEnabled = toggle;
            if (!toggle) SetMoodEditBufferToXmlDefaults();
            moodEditorDirty = true;
        }
        if (editBuffer != null)
        {
            string context = "SR.SoundMood.Context".Translate(SqueakLabels.Mood(selectedMood), SqueakLabels.Action(selectedAction),
                editBufferOverrideEnabled ? "SR.SoundMood.Override".Translate() : "SR.SoundMood.Xml".Translate());
            SqueakySettingsUI.StatusPanel(list.GetRect(MeasureCompactStatusHeight(context, list.ColumnWidth)), context,
                editBufferOverrideEnabled ? SqueakySurfaceKind.Emphasized : SqueakySurfaceKind.Base);
            SqueakySettingsUI.LabelWithHelp(list.GetRect(26f), "SR.Workbench.Preset".Translate(), "SR.SoundMood.Preset.Tooltip".Translate());
            if (DrawMoodPresetButtons(list.GetRect(list.ColumnWidth < 520f ? 70f : 32f), editBuffer)) moodEditorDirty = true;
            string inheritedReason = "SR.SoundMood.InheritedReadOnly".Translate();
            float jitterMin = editBuffer.pitchJitter.min, jitterMax = editBuffer.pitchJitter.max;
            if (list.ColumnWidth >= 760f)
            {
                Rect grid = list.GetRect(70f);
                float half = (grid.width - 8f) * .5f;
                moodEditorDirty |= DrawMoodSlider(new Rect(grid.x, grid.y, half, 32f), MoodFieldKey(selectedMood, "PitchFactor"), "SR.Workbench.PitchFactor".Translate(), ref editBuffer.pitchFactor, .5f, 2f, editBufferOverrideEnabled, inheritedReason);
                moodEditorDirty |= DrawMoodSlider(new Rect(grid.x + half + 8f, grid.y, half, 32f), MoodFieldKey(selectedMood, "VolumeFactor"), "SR.Workbench.VolumeFactor".Translate(), ref editBuffer.volumeFactor, 0f, 2f, editBufferOverrideEnabled, inheritedReason);
                moodEditorDirty |= DrawMoodSlider(new Rect(grid.x, grid.y + 38f, half, 32f), MoodFieldKey(selectedMood, "PitchJitterMin"), "SR.Workbench.JitterMin".Translate(), ref jitterMin, .5f, 1.5f, editBufferOverrideEnabled, inheritedReason);
                moodEditorDirty |= DrawMoodSlider(new Rect(grid.x + half + 8f, grid.y + 38f, half, 32f), MoodFieldKey(selectedMood, "PitchJitterMax"), "SR.Workbench.JitterMax".Translate(), ref jitterMax, .5f, 1.5f, editBufferOverrideEnabled, inheritedReason);
            }
            else
            {
                moodEditorDirty |= DrawMoodSlider(list.GetRect(32f), MoodFieldKey(selectedMood, "PitchFactor"), "SR.Workbench.PitchFactor".Translate(), ref editBuffer.pitchFactor, .5f, 2f, editBufferOverrideEnabled, inheritedReason);
                moodEditorDirty |= DrawMoodSlider(list.GetRect(32f), MoodFieldKey(selectedMood, "VolumeFactor"), "SR.Workbench.VolumeFactor".Translate(), ref editBuffer.volumeFactor, 0f, 2f, editBufferOverrideEnabled, inheritedReason);
                moodEditorDirty |= DrawMoodSlider(list.GetRect(32f), MoodFieldKey(selectedMood, "PitchJitterMin"), "SR.Workbench.JitterMin".Translate(), ref jitterMin, .5f, 1.5f, editBufferOverrideEnabled, inheritedReason);
                moodEditorDirty |= DrawMoodSlider(list.GetRect(32f), MoodFieldKey(selectedMood, "PitchJitterMax"), "SR.Workbench.JitterMax".Translate(), ref jitterMax, .5f, 1.5f, editBufferOverrideEnabled, inheritedReason);
            }
            if (editBufferOverrideEnabled && jitterMax < jitterMin) { jitterMax = jitterMin; moodEditorDirty = true; }
            editBuffer.pitchJitter = new FloatRange(jitterMin, jitterMax);
            Rect defaultsRow = list.GetRect(34f);
            float defaultsWidth = Mathf.Clamp(defaultsRow.width * .34f, 170f, 260f);
            if (SqueakySettingsUI.Button(new Rect(defaultsRow.xMax - defaultsWidth, defaultsRow.y, defaultsWidth, 32f), "SR.SoundMood.UseXmlDefaults".Translate(), SqueakyButtonKind.Secondary))
            {
                SqueakMoodMod defaults = GetDefaultMoodMod(selectedMood)?.Clone() ?? new SqueakMoodMod { mood = selectedMood };
                bool changed = editBufferOverrideEnabled
                    || Math.Abs(editBuffer.pitchFactor - defaults.pitchFactor) > .0001f
                    || Math.Abs(editBuffer.volumeFactor - defaults.volumeFactor) > .0001f
                    || Math.Abs(editBuffer.pitchJitter.min - defaults.pitchJitter.min) > .0001f
                    || Math.Abs(editBuffer.pitchJitter.max - defaults.pitchJitter.max) > .0001f;
                editBufferOverrideEnabled = false;
                SetMoodEditBufferToXmlDefaults();
                moodEditorDirty |= changed;
            }
            list.Gap(10f);
            DrawCollapsibleHeader(list, "SR.SoundMood.PreviewSection".Translate(), ref moodPreviewSectionOpen);
            if (moodPreviewSectionOpen)
            {
                list.Label("SR.SoundMood.PreviewNote".Translate());
                DrawMoodResolvedPreview(list);
            }
        }
        if (moodEditorDirty) CommitMoodEditorNow();
        list.End(); Widgets.EndScrollView();
    }

    private float MeasureSoundMoodContentHeight(float width)
    {
        bool stackMoodCards = width < 620f;
        float height = MeasureCompactPageIntroHeight("SR.SoundMood.Header".Translate(), "SR.SoundMood.PageDesc".Translate(), width);
        string context = "SR.SoundMood.Context".Translate(SqueakLabels.Mood(selectedMood), SqueakLabels.Action(selectedAction),
            editBufferOverrideEnabled ? "SR.SoundMood.Override".Translate() : "SR.SoundMood.Xml".Translate());
        height += (stackMoodCards ? 150f : 72f) + 8f + 34f + 34f + 34f + MeasureCompactStatusHeight(context, width) + 26f;
        height += width < 520f ? 70f : 32f;
        height += (width >= 760f ? 70f : 32f * 4f) + 34f + 10f + 34f;
        if (moodPreviewSectionOpen) height += Text.CalcHeight("SR.SoundMood.PreviewNote".Translate(), width) + 2f + MeasureMoodPreviewHeight(width);
        return height + 24f;
    }

    private float MeasureMoodPreviewHeight(float width)
    {
        float height = width < 520f ? 72f : 34f;
        SqueakActionDefinition definition = SqueakActionDefinitions.Get(selectedAction);
        SoundDef? sound = DefDatabase<SoundDef>.GetNamedSilentFail(definition.AudioKey);
        IReadOnlyList<SqueakResolvedClip> clips = sound != null && moodExplicitlyResolved.Contains(sound)
            && SqueakSoundAvailabilityCache.TryGetCached(sound, out SqueakSoundAvailability available)
            ? available.Clips : Array.Empty<SqueakResolvedClip>();
        if (clips.Count > 0) height += 30f;
        height += Mathf.Max(30f, Text.CalcHeight("SR.SoundMood.MaterialPreview.Short".Translate(), width - 24f));
        if (!materialPreviewStatus.NullOrEmpty()) height += Mathf.Max(44f, Text.CalcHeight(materialPreviewStatus, width - 16f) + 14f);
        height += 34f + Text.CalcHeight("SR.FinalPreview.Short".Translate(), width) + 2f + 40f + 34f;
        if (!finalPreviewStatus.NullOrEmpty()) height += Mathf.Max(54f, Text.CalcHeight(finalPreviewStatus, width - 16f) + 14f);
        return height + 20f;
    }

    private void SetMoodEditBufferToXmlDefaults()
    {
        editBuffer = GetDefaultMoodMod(selectedMood)?.Clone() ?? new SqueakMoodMod { mood = selectedMood };
        bufferForMood = selectedMood;
        numericBuffers.Clear();
    }

    private void CommitMoodEditorNow()
    {
        if (editBuffer == null || !moodEditorDirty) return;
        if (editBufferOverrideEnabled) moodOverrides[selectedMood] = editBuffer.Clone();
        else moodOverrides.Remove(selectedMood);
        moodEditorDirty = false;
        NotifyGlobalMoodRuntimeChanged();
        QueuePersistence();
    }

    private bool DrawMoodPresetButtons(Rect rect, SqueakMoodMod mod)
    {
        const float gap = 6f;
        bool stacked = rect.width < 520f;
        float width = stacked ? (rect.width - gap) * .5f : (rect.width - gap * 3f) * .25f;
        float height = stacked ? 32f : rect.height;
        Rect[] boxes = {
            new(rect.x, rect.y, width, height), new(rect.x + width + gap, rect.y, width, height),
            stacked ? new Rect(rect.x, rect.y + height + gap, width, height) : new Rect(rect.x + (width + gap) * 2f, rect.y, width, height),
            stacked ? new Rect(rect.x + width + gap, rect.y + height + gap, width, height) : new Rect(rect.x + (width + gap) * 3f, rect.y, width, height)
        };
        string[] labels = { "SR.Preset.Sharp", "SR.Preset.Neutral", "SR.Preset.Low", "SR.Preset.Chaos" };
        float[,] values = { { 1.25f, 1.3f, 1f, 1f }, { 1f, 1f, 1f, 1f }, { .8f, .75f, 1f, 1f }, { 1f, 1.5f, .7f, 1.4f } };
        for (int i = 0; i < 4; i++)
        {
            if (!SqueakySettingsUI.Button(boxes[i], labels[i].Translate(), SqueakyButtonKind.Ghost)) continue;
            bool changed = !editBufferOverrideEnabled || Math.Abs(mod.pitchFactor - values[i, 0]) > .0001f
                || Math.Abs(mod.volumeFactor - values[i, 1]) > .0001f || Math.Abs(mod.pitchJitter.min - values[i, 2]) > .0001f
                || Math.Abs(mod.pitchJitter.max - values[i, 3]) > .0001f;
            editBufferOverrideEnabled = true;
            mod.pitchFactor = values[i, 0]; mod.volumeFactor = values[i, 1];
            mod.pitchJitter = new FloatRange(values[i, 2], values[i, 3]); numericBuffers.Clear();
            return changed;
        }
        return false;
    }

    private bool DrawMoodSlider(Rect rect, string key, string label, ref float value, float min, float max, bool enabled, string disabledReason)
    {
        float before = value;
        value = DrawSliderWithField(rect, key, label, value, min, max, enabled, disabledReason);
        return enabled && Math.Abs(value - before) > .0001f;
    }

    private void DrawMoodResolvedPreview(Listing_Standard list)
    {
        SqueakActionDefinition definition = SqueakActionDefinitions.Get(selectedAction);
        SoundDef? sound = DefDatabase<SoundDef>.GetNamedSilentFail(definition.AudioKey);
        SqueakSoundAvailabilityState state = sound != null && moodExplicitlyResolved.Contains(sound) ? SqueakSoundAvailabilityCache.PeekState(sound) : SqueakSoundAvailabilityState.Unknown;
        bool narrow = list.ColumnWidth < 520f;
        Rect row = list.GetRect(narrow ? 72f : 34f); float half = (row.width - 6f) / 2f;
        Rect resolveRect = narrow ? new Rect(row.x, row.y, row.width, 32f) : new Rect(row.x, row.y, half, 32f);
        Rect previewRect = narrow ? new Rect(row.x, row.y + 38f, row.width, 32f) : new Rect(row.x + half + 6f, row.y, half, 32f);
        if (SqueakySettingsUI.Button(resolveRect, "SR.AudioBrowser.Resolve".Translate(), enabled: sound != null && state == SqueakSoundAvailabilityState.Unknown))
        {
            moodExplicitlyResolved.Add(sound!);
            SqueakSoundAvailability resolved = SqueakSoundAvailabilityCache.Resolve(sound);
            moodClipIndex = 0;
            string key = resolved.State switch
            {
                SqueakSoundAvailabilityState.Available when resolved.Clips.Count > 0 => "SR.MaterialPreview.Resolve.Ready",
                SqueakSoundAvailabilityState.Empty => "SR.MaterialPreview.Resolve.NoClips",
                SqueakSoundAvailabilityState.Failed => "SR.MaterialPreview.Resolve.Failed",
                _ => "SR.MaterialPreview.Resolve.NoClips"
            };
            materialPreviewStatus = key.Translate(resolved.Clips.Count, resolved.Diagnostic);
            materialPreviewStatusFailed = resolved.Clips.Count == 0;
        }
        IReadOnlyList<SqueakResolvedClip> clips = sound != null && moodExplicitlyResolved.Contains(sound) && SqueakSoundAvailabilityCache.TryGetCached(sound, out SqueakSoundAvailability available) ? available.Clips : Array.Empty<SqueakResolvedClip>();
        if (moodClipIndex >= clips.Count) moodClipIndex = 0;
        if (clips.Count > 0)
        {
            Rect clipRow = list.GetRect(30f);
            float selectorWidth = Mathf.Max(120f, clipRow.width - 172f);
            string identity = "SR.SoundMood.SelectedClip".Translate(moodClipIndex + 1, clips.Count, clips[moodClipIndex].Clip.name ?? "—");
            DrawClippedLabel(new Rect(clipRow.x, clipRow.y, selectorWidth, 28f), identity, identity);
            bool multiple = clips.Count > 1;
            if (SqueakySettingsUI.Button(new Rect(clipRow.xMax - 166f, clipRow.y, 80f, 28f), "SR.AudioBrowser.Previous".Translate(), enabled: multiple)) moodClipIndex = (moodClipIndex + clips.Count - 1) % clips.Count;
            if (SqueakySettingsUI.Button(new Rect(clipRow.xMax - 80f, clipRow.y, 80f, 28f), "SR.AudioBrowser.Next".Translate(), enabled: multiple)) moodClipIndex = (moodClipIndex + 1) % clips.Count;
        }
        string materialDisabledReason = state == SqueakSoundAvailabilityState.Unknown ? "SR.MaterialPreview.Reason.NotResolved".Translate()
            : state == SqueakSoundAvailabilityState.Failed ? "SR.MaterialPreview.Reason.ResolveFailed".Translate()
            : clips.Count == 0 ? "SR.MaterialPreview.Reason.NoClips".Translate() : "";
        if (SqueakySettingsUI.Button(previewRect, "SR.Workbench.Preview".Translate(), SqueakyButtonKind.Secondary, clips.Count > 0, materialDisabledReason) && editBuffer != null)
        {
            SqueakMoodMod previewMod = editBufferOverrideEnabled
                ? editBuffer
                : (GetDefaultMoodMod(selectedMood)?.Clone() ?? new SqueakMoodMod { mood = selectedMood });
            SubSoundDef? adapter = SqueakOnCameraPreviewAdapter.Get();
            if (adapter == null)
            {
                materialPreviewStatus = "SR.MaterialPreview.Reason.AdapterMissing".Translate();
                materialPreviewStatusFailed = true;
            }
            else
            {
                Rand.PushState();
                try
                {
                    SoundInfo info = SoundInfo.OnCamera();
                    info.testPlay = true;
                    info.pitchFactor = previewMod.pitchFactor * previewMod.pitchJitter.RandomInRange;
                    info.volumeFactor = previewMod.volumeFactor;
                    if (SampleOneShot.TryMakeAndPlay(adapter, clips[moodClipIndex].Clip, info) != null)
                    {
                        materialPreviewStatus = "SR.MaterialPreview.Reason.Dispatched".Translate();
                        materialPreviewStatusFailed = false;
                    }
                    else
                    {
                        materialPreviewStatus = "SR.MaterialPreview.Reason.SampleFailed".Translate();
                        materialPreviewStatusFailed = true;
                    }
                }
                finally { Rand.PopState(); }
            }
        }
        SqueakySettingsUI.LabelWithHelp(list.GetRect(Mathf.Max(30f, Text.CalcHeight("SR.SoundMood.MaterialPreview.Short".Translate(), list.ColumnWidth - 24f))),
            "SR.SoundMood.MaterialPreview.Short".Translate(), "SR.SoundMood.MaterialPreview.Tooltip.Short".Translate());
        if (!materialPreviewStatus.NullOrEmpty())
            SqueakySettingsUI.StatusPanel(list.GetRect(Mathf.Max(44f, Text.CalcHeight(materialPreviewStatus, list.ColumnWidth - 16f) + 14f)), materialPreviewStatus,
                materialPreviewStatusFailed ? SqueakySurfaceKind.Warning : SqueakySurfaceKind.Success);
        DrawFinalProductionPreview(list);
    }

    private void DrawFinalProductionPreview(Listing_Standard list)
    {
        DrawSectionHeader(list, "SR.FinalPreview.Header".Translate(), "SR.FinalPreview.Tooltip.Short".Translate());
        list.Label("SR.FinalPreview.Short".Translate());
        SqueakSettingsGameContext context = CurrentDrawContext;
        bool valid = TryGetSelectedSqueaker(out Pawn? pawn, out CompSqueaker? comp);
        string unavailable = !context.HasPlayableMapUI ? "SR.FinalPreview.MapRequired".Translate() : "SR.FinalPreview.NoPawn".Translate();
        string selected = valid ? "SR.FinalPreview.Selected".Translate(pawn!.LabelShort) : unavailable;
        SqueakySettingsUI.StatusPanel(list.GetRect(40f), selected, valid ? SqueakySurfaceKind.Base : SqueakySurfaceKind.Warning);
        if (SqueakySettingsUI.Button(list.GetRect(34f), "SR.FinalPreview.Play".Translate(), SqueakyButtonKind.Primary,
                valid, unavailable))
        {
            CommitMoodEditorNow();
            FlushPendingRuntimeForPreview();
            context = SqueakSettingsGameContext.Capture();
            if (!context.TryGetSelectedSqueaker(out pawn, out comp))
            {
                finalPreviewStatus = context.HasPlayableMapUI ? "SR.FinalPreview.NoPawn".Translate() : "SR.FinalPreview.MapRequired".Translate();
                finalPreviewStatusFailed = true;
            }
            else
            {
                SqueakFinalPreviewResult result = comp!.PreviewFinal(selectedAction, null, context);
                string reason = FinalPreviewReason(result);
                finalPreviewStatus = reason;
                finalPreviewStatusFailed = result.Status != SqueakFinalPreviewStatus.Dispatched;
            }
        }
        if (!finalPreviewStatus.NullOrEmpty())
            SqueakySettingsUI.StatusPanel(list.GetRect(Mathf.Max(54f, Text.CalcHeight(finalPreviewStatus, list.ColumnWidth - 16f) + 14f)), finalPreviewStatus,
                finalPreviewStatusFailed ? SqueakySurfaceKind.Warning : SqueakySurfaceKind.Success);
    }

    private static string FinalPreviewReason(SqueakFinalPreviewResult result)
    {
        string key = result.Reason switch
        {
            "dispatched" => "SR.FinalPreview.Reason.Dispatched",
            "pawn_or_map_unavailable" => "SR.FinalPreview.Reason.PawnOrMap",
            "resolver_no_eligible_sound" => "SR.FinalPreview.Reason.NoEligible",
            "exception" => "SR.FinalPreview.Reason.Exception",
            _ when result.Reason.StartsWith("sound_not_playable_") => "SR.FinalPreview.Reason.NotPlayable",
            _ when result.Reason.StartsWith("sound_info_failed_") => "SR.FinalPreview.Reason.SoundInfo",
            _ => "SR.FinalPreview.Reason.Other"
        };
        return key.Translate(result.Sound?.defName ?? "—", result.Source.ToString(), result.Playability.ToString(), result.Reason);
    }
}

internal static class SqueakOnCameraPreviewAdapter
{
    internal const string SoundDefName = "SR_Call_Preview";

    internal static SubSoundDef? Get()
    {
        SoundDef? soundDef = DefDatabase<SoundDef>.GetNamedSilentFail(SoundDefName);
        return soundDef?.subSounds?.FirstOrDefault(subSound => subSound != null && subSound.onCamera);
    }
}
