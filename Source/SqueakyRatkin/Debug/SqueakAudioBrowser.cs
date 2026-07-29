using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace SqueakyRatkin;

internal static class SqueakAudioBrowser
{
    internal static void Open()
    {
        try { Find.WindowStack?.Add(new Dialog_SqueakAudioBrowser()); }
        catch (Exception ex) { SqueakLog.WorkbenchOpenFailed(ex); }
    }
}

internal sealed class Dialog_SqueakAudioBrowser : Window
{
    private const float Row = 54f;
    private static readonly string[] VocalNames = { "Call", "Angry", "Wounded", "Death", "Ambience", "Attack" };
    private static readonly string[] MechanicalNames = { "Pickup", "Drop", "Melee", "Bullet", "Impact" };
    private readonly List<AnimalEntry> animals = new();
    private readonly List<AnimalEntry> filteredAnimals = new();
    private readonly HashSet<SoundDef> explicitlyResolved = new();
    private Vector2 animalScroll, referenceScroll, detailScroll;
    private AnimalEntry? selectedAnimal;
    private VoiceReference? selectedReference;
    private string animalQuery = "", referenceQuery = "", lastAnimalQuery = "", lastReferenceQuery = "";
    private bool hideMechanical = true, lastHideMechanical = true;
    private int clipIndex;
    private SqueakMood previewMood = SqueakMood.Neutral;
    private float previewPitch = 1f, previewVolume = 1f;
    private string pitchBuffer = "1", volumeBuffer = "1", status = "";
    private float detailHeight = 980f;
    private bool hasPreviewSample;
    private int previewSampleClipIndex = -1;
    private string previewSampleSoundDef = "";
    private float previewSampleJitter, previewSampleFinalPitch;

    private sealed class AnimalEntry
    {
        internal ThingDef Def = null!;
        internal string Label = "", Owner = "", PackageId = "", Search = "";
        internal readonly List<VoiceReference> References = new();
        internal readonly List<VoiceReference> Filtered = new();
    }

    private sealed class VoiceReference
    {
        internal SoundDef Sound = null!;
        internal int LifeStageIndex = -1;
        internal string LifeStageDef = "", LifeStageLabel = "", Category = "", Action = "", FieldPath = "", Search = "";
        internal bool Mechanical;
    }

    public override Vector2 InitialSize
    {
        get
        {
            float width = Mathf.Min(1180f, UI.screenWidth - 48f);
            float height = Mathf.Min(780f, UI.screenHeight - 48f);
            return new Vector2(Mathf.Max(620f, width), Mathf.Max(460f, height));
        }
    }

    internal Dialog_SqueakAudioBrowser()
    {
        doCloseX = true;
        closeOnClickedOutside = false;
        BuildIndex();
        RefreshAnimalFilter(true);
        SelectAnimal(filteredAnimals.FirstOrDefault());
        ResetPreviewMood();
    }

    public override void DoWindowContents(Rect inRect)
    {
        SqueakySettingsUI.SectionHeader(new Rect(inRect.x, inRect.y, inRect.width, 66f),
            "SR.AudioBrowser.Title".Translate(), "SR.AudioBrowser.Intro".Translate());
        Rect body = new(inRect.x, inRect.y + 72f, inRect.width, inRect.height - 72f);
        if (body.width >= 900f) DrawThreeColumns(body); else DrawNarrow(body);
    }

    private void DrawThreeColumns(Rect body)
    {
        const float gap = 8f;
        float leftWidth = Mathf.Clamp(body.width * .25f, 210f, 280f);
        float middleWidth = Mathf.Clamp(body.width * .29f, 230f, 330f);
        Rect left = new(body.x, body.y, leftWidth, body.height);
        Rect middle = new(left.xMax + gap, body.y, middleWidth, body.height);
        Rect right = new(middle.xMax + gap, body.y, body.xMax - middle.xMax - gap, body.height);
        SqueakySettingsUI.PanelFrame(left); SqueakySettingsUI.PanelFrame(middle); SqueakySettingsUI.PanelFrame(right);
        DrawAnimals(left.ContractedBy(7f)); DrawReferences(middle.ContractedBy(7f)); DrawDetails(right.ContractedBy(9f));
    }

    private void DrawNarrow(Rect body)
    {
        float chooserHeight = Mathf.Clamp(body.height * .22f, 96f, 170f);
        Rect left = new(body.x, body.y, body.width, chooserHeight);
        Rect middle = new(body.x, left.yMax + 8f, body.width, chooserHeight);
        Rect detail = new(body.x, middle.yMax + 8f, body.width, body.yMax - middle.yMax - 8f);
        SqueakySettingsUI.PanelFrame(left); SqueakySettingsUI.PanelFrame(middle); SqueakySettingsUI.PanelFrame(detail);
        DrawAnimals(left.ContractedBy(6f)); DrawReferences(middle.ContractedBy(6f)); DrawDetails(detail.ContractedBy(8f));
    }

    private void BuildIndex()
    {
        foreach (ThingDef def in DefDatabase<ThingDef>.AllDefs.Where(IsRealAnimalDef)
            .OrderBy(x => x.LabelCap.ToString(), StringComparer.CurrentCultureIgnoreCase))
        {
            AnimalEntry entry = new()
            {
                Def = def,
                Label = def.LabelCap.ToString(),
                Owner = def.modContentPack?.Name ?? "SR.Common.Core".Translate(),
                PackageId = def.modContentPack?.PackageId ?? "core"
            };
            entry.Search = JoinSearch(entry.Label, def.defName, entry.Owner, entry.PackageId);
            CollectReferences(entry);
            entry.Search += "\n" + string.Join("\n", entry.References.Select(reference => reference.Search));
            if (entry.References.Count > 0) animals.Add(entry);
        }
    }

    private static bool IsRealAnimalDef(ThingDef? def)
    {
        if (def?.race?.Animal != true || def.thingClass == null) return false;
        if (typeof(Corpse).IsAssignableFrom(def.thingClass)) return false;
        return true;
    }

    private static void CollectReferences(AnimalEntry animal)
    {
        HashSet<string> unique = new(StringComparer.Ordinal);
        WalkOwner(animal, animal.Def, "ThingDef", -1, "", "", unique, 0);
        WalkOwner(animal, animal.Def.race, "race", -1, "", "", unique, 0);
        object? lifeStages = ReadMember(animal.Def.race, "lifeStageAges");
        if (lifeStages is IEnumerable sequence)
        {
            int index = 0;
            foreach (object? stage in sequence)
            {
                object? stageDefinition = ReadMember(stage, "def");
                string stageDef = stageDefinition is Def def ? def.defName : "";
                string stageLabel = stageDefinition is Def labelDef ? labelDef.LabelCap.ToString() : "";
                WalkOwner(animal, stage, "race.lifeStageAges[" + index + "]", index, stageDef, stageLabel, unique, 0);
                WalkOwner(animal, stageDefinition, "race.lifeStageAges[" + index + "].def", index, stageDef, stageLabel, unique, 0);
                index++;
            }
        }
        animal.References.Sort((a, b) => StringComparer.OrdinalIgnoreCase.Compare(a.FieldPath, b.FieldPath));
    }

    private static void WalkOwner(AnimalEntry animal, object? owner, string path, int stageIndex, string stageDef,
        string stageLabel, HashSet<string> unique, int depth)
    {
        if (owner == null || depth > 2) return;
        foreach (FieldInfo field in owner.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
        {
            object? value;
            try { value = field.GetValue(owner); } catch { continue; }
            string fieldPath = path + "." + field.Name;
            if (value is SoundDef sound)
            {
                string identity = fieldPath + "|" + sound.defName;
                if (!unique.Add(identity)) continue;
                string action = SemanticAction(field.Name);
                bool mechanical = MechanicalNames.Any(x => field.Name.IndexOf(x, StringComparison.OrdinalIgnoreCase) >= 0);
                string category = mechanical ? "Mechanical" : VocalNames.Contains(action) ? "Vocal" : "Other";
                VoiceReference reference = new()
                {
                    Sound = sound, LifeStageIndex = stageIndex, LifeStageDef = stageDef, LifeStageLabel = stageLabel,
                    Category = category, Action = action, FieldPath = fieldPath, Mechanical = mechanical
                };
                reference.Search = JoinSearch(fieldPath, category, action, sound.defName,
                    sound.modContentPack?.Name ?? "Core", sound.modContentPack?.PackageId ?? "core", stageDef, stageLabel);
                animal.References.Add(reference);
            }
            else if (depth < 2 && value != null && !field.FieldType.IsPrimitive && field.FieldType != typeof(string)
                && !typeof(IEnumerable).IsAssignableFrom(field.FieldType) && field.FieldType.Namespace?.StartsWith("Verse", StringComparison.Ordinal) == true)
            {
                WalkOwner(animal, value, fieldPath, stageIndex, stageDef, stageLabel, unique, depth + 1);
            }
        }
    }

    private static string SemanticAction(string field)
    {
        foreach (string name in VocalNames) if (field.IndexOf(name, StringComparison.OrdinalIgnoreCase) >= 0) return name;
        if (field.IndexOf("Eating", StringComparison.OrdinalIgnoreCase) >= 0) return "Eating";
        if (field.IndexOf("Moving", StringComparison.OrdinalIgnoreCase) >= 0) return "Moving";
        return "Other";
    }

    private void DrawAnimals(Rect rect)
    {
        Widgets.Label(new Rect(rect.x, rect.y, rect.width, 24f), "SR.AudioBrowser.Animals".Translate());
        string next = SqueakySettingsUI.SearchField(new Rect(rect.x, rect.y + 26f, rect.width, 30f), animalQuery, "SR.AudioBrowser.SearchAnimals".Translate());
        if (next != animalQuery) { animalQuery = next; RefreshAnimalFilter(); }
        Rect list = new(rect.x, rect.y + 62f, rect.width, rect.height - 62f);
        Rect view = new(0f, 0f, list.width - 16f, Mathf.Max(list.height, filteredAnimals.Count * Row));
        Widgets.BeginScrollView(list, ref animalScroll, view);
        float y = 0f;
        foreach (AnimalEntry animal in filteredAnimals)
        {
            Rect row = new(0f, y, view.width, Row - 4f); y += Row;
            DrawListRowSurface(row, ReferenceEquals(animal, selectedAnimal));
            Rect help = new(row.xMax - SqueakySettingsUI.HelpSize - 5f, row.y + 5f, SqueakySettingsUI.HelpSize, SqueakySettingsUI.HelpSize);
            LabelLine(new Rect(row.x + 5f, row.y + 3f, help.x - row.x - 10f, 24f), animal.Label);
            Text.Font = GameFont.Tiny; LabelLine(new Rect(row.x + 5f, row.y + 27f, row.width - 10f, 19f), animal.Def.defName + " · " + animal.References.Count); Text.Font = GameFont.Small;
            SqueakySettingsUI.HelpIndicator(help, animal.Label + "\n" + animal.Def.defName + "\n" + animal.Owner + "\n" + animal.PackageId);
            if (Widgets.ButtonInvisible(new Rect(row.x, row.y, help.x - row.x - 2f, row.height))) SelectAnimal(animal);
        }
        Widgets.EndScrollView();
    }

    private void DrawReferences(Rect rect)
    {
        Widgets.Label(new Rect(rect.x, rect.y, rect.width, 24f), "SR.AudioBrowser.Sounds".Translate());
        string next = SqueakySettingsUI.SearchField(new Rect(rect.x, rect.y + 26f, rect.width, 30f), referenceQuery, "SR.AudioBrowser.SearchReferences".Translate());
        if (next != referenceQuery) { referenceQuery = next; RefreshReferenceFilter(); }
        if (SqueakySettingsUI.FilterChip(new Rect(rect.x, rect.y + 62f, rect.width, 28f), "SR.AudioBrowser.HideMechanical".Translate(), hideMechanical))
        { hideMechanical = !hideMechanical; RefreshReferenceFilter(); }
        List<VoiceReference> refs = selectedAnimal?.Filtered ?? new List<VoiceReference>();
        Rect list = new(rect.x, rect.y + 96f, rect.width, rect.height - 96f);
        Rect view = new(0f, 0f, list.width - 16f, Mathf.Max(list.height, refs.Count * 66f));
        Widgets.BeginScrollView(list, ref referenceScroll, view);
        float y = 0f;
        foreach (VoiceReference reference in refs)
        {
            Rect row = new(0f, y, view.width, 61f); y += 66f;
            DrawListRowSurface(row, ReferenceEquals(reference, selectedReference));
            Rect help = new(row.xMax - SqueakySettingsUI.HelpSize - 5f, row.y + 5f, SqueakySettingsUI.HelpSize, SqueakySettingsUI.HelpSize);
            LabelLine(new Rect(row.x + 5f, row.y + 3f, help.x - row.x - 10f, 23f), SemanticLabel(reference.Action) + " · " + reference.Sound.defName);
            Text.Font = GameFont.Tiny; LabelLine(new Rect(row.x + 5f, row.y + 27f, row.width - 10f, 18f), ("SR.AudioBrowser.Category." + reference.Category).Translate() + " · " + reference.FieldPath); LabelLine(new Rect(row.x + 5f, row.y + 44f, row.width - 10f, 17f), reference.LifeStageLabel); Text.Font = GameFont.Small;
            SqueakySettingsUI.HelpIndicator(help, reference.FieldPath + "\n" + reference.Sound.defName);
            if (Widgets.ButtonInvisible(new Rect(row.x, row.y, help.x - row.x - 2f, row.height))) SelectReference(reference);
        }
        Widgets.EndScrollView();
    }

    private void DrawDetails(Rect rect)
    {
        Rect view = new(0f, 0f, rect.width - 16f, Mathf.Max(rect.height, detailHeight));
        Widgets.BeginScrollView(rect, ref detailScroll, view);
        float y = 0f, width = view.width;
        if (selectedAnimal == null || selectedReference == null)
        {
            SqueakySettingsUI.EmptyState(new Rect(0f, y, width, Mathf.Min(120f, rect.height)), "SR.AudioBrowser.NoSelection".Translate());
            Widgets.EndScrollView(); return;
        }
        VoiceReference reference = selectedReference;
        SoundDef sound = reference.Sound;
        Text.Font = GameFont.Medium; Wrapped(ref y, width, sound.defName); Text.Font = GameFont.Small;
        Meta(ref y, width, "SR.AudioBrowser.Animal".Translate(), selectedAnimal.Label + " / " + selectedAnimal.Def.defName);
        Meta(ref y, width, "SR.AudioBrowser.Reference".Translate(), reference.FieldPath);
        Meta(ref y, width, "SR.AudioBrowser.Context".Translate(), "SR.AudioBrowser.ContextValue".Translate(sound.context.ToString(), sound.sustain.ToString()));
        SqueakSoundAvailabilityState state = explicitlyResolved.Contains(sound) ? SqueakSoundAvailabilityCache.PeekState(sound) : SqueakSoundAvailabilityState.Unknown;
        Meta(ref y, width, "SR.AudioBrowser.Availability".Translate(), ("SR.AudioBrowser.State." + state).Translate());
        if (SqueakySettingsUI.Button(new Rect(0f, y, width, 32f), "SR.AudioBrowser.Resolve".Translate(), SqueakyButtonKind.Primary, state == SqueakSoundAvailabilityState.Unknown, "SR.AudioBrowser.AlreadyResolved".Translate())) Resolve();
        y += 42f;
        IReadOnlyList<SqueakResolvedClip> clips = VisibleClips(sound);
        if (clipIndex >= clips.Count) clipIndex = 0;
        if (clips.Count > 0)
        {
            SqueakResolvedClip clip = clips[clipIndex];
            Wrapped(ref y, width, "SR.AudioBrowser.Clip".Translate(clipIndex + 1, clips.Count));
            Wrapped(ref y, width, (clip.Clip.name ?? "—") + " · " + clip.GrainType + " · " + clip.SubSoundIndex + "/" + clip.GrainIndex, true);
            float half = (width - 6f) / 2f;
            if (SqueakySettingsUI.Button(new Rect(0f, y, half, 30f), "SR.AudioBrowser.Previous".Translate(), enabled: clips.Count > 1)) { clipIndex = (clipIndex + clips.Count - 1) % clips.Count; hasPreviewSample = false; }
            if (SqueakySettingsUI.Button(new Rect(half + 6f, y, half, 30f), "SR.AudioBrowser.Next".Translate(), enabled: clips.Count > 1)) { clipIndex = (clipIndex + 1) % clips.Count; hasPreviewSample = false; }
            y += 38f;
        }
        DrawMoodTransport(ref y, width, sound, clips);
        if (SqueakySettingsUI.Button(new Rect(0f, y, width, 32f), "SR.AudioBrowser.Copy".Translate(), SqueakyButtonKind.Secondary)) CopyTrace(sound, clips);
        y += 42f;
        if (!status.NullOrEmpty())
        {
            float statusHeight = Mathf.Max(34f, Text.CalcHeight(status, Mathf.Max(1f, width - 16f)) + 10f);
            bool success = status == "SR.Preview.Dispatched".Translate() || status == "SR.AudioBrowser.Copied".Translate();
            bool warning = status == "SR.Preview.NotDispatched".Translate();
            SqueakySettingsUI.StatusPanel(new Rect(0f, y, width, statusHeight), status,
                success ? SqueakySurfaceKind.Success : warning ? SqueakySurfaceKind.Warning : SqueakySurfaceKind.Base);
            y += statusHeight + 5f;
        }
        detailHeight = y + 18f;
        Widgets.EndScrollView();
    }

    private void DrawMoodTransport(ref float y, float width, SoundDef sound, IReadOnlyList<SqueakResolvedClip> clips)
    {
        Wrapped(ref y, width, "SR.AudioBrowser.WorkbenchMoodNote".Translate());
        SqueakMood[] moods = { SqueakMood.Good, SqueakMood.Neutral, SqueakMood.Bad, SqueakMood.Break };
        bool stacked = width < 430f;
        float chip = stacked ? (width - 6f) * .5f : (width - 18f) / 4f;
        for (int i = 0; i < moods.Length; i++)
        {
            SqueakMood mood = moods[i];
            Rect chipRect = stacked
                ? new Rect((i % 2) * (chip + 6f), y + (i / 2) * 36f, chip, 30f)
                : new Rect(i * (chip + 6f), y, chip, 30f);
            if (SqueakySettingsUI.FilterChip(chipRect, SqueakLabels.Mood(mood), previewMood == mood))
            { previewMood = mood; ResetPreviewMood(); }
        }
        y += stacked ? 74f : 38f;
        float nextPitch = SliderField(new Rect(0f, y, width, 30f), "SR.Workbench.PitchFactor".Translate(), previewPitch, ref pitchBuffer, .5f, 2f);
        if (Math.Abs(nextPitch - previewPitch) > .0001f) hasPreviewSample = false;
        previewPitch = nextPitch; y += 34f;
        float nextVolume = SliderField(new Rect(0f, y, width, 30f), "SR.Workbench.VolumeFactor".Translate(), previewVolume, ref volumeBuffer, 0f, 2f);
        if (Math.Abs(nextVolume - previewVolume) > .0001f) hasPreviewSample = false;
        previewVolume = nextVolume; y += 36f;
        SqueakMoodMod canonical = SqueakyRatkinMod.Settings.GetCanonicalMoodMod(previewMood);
        Meta(ref y, width, "SR.Workbench.PitchJitter".Translate(), canonical.pitchJitter.ToString() + " (" + "SR.AudioBrowser.ReadOnly".Translate() + ")");
        bool available = clips.Count > 0;
        if (SqueakySettingsUI.Button(new Rect(0f, y, width, 34f), "SR.AudioBrowser.RequestClip".Translate(), SqueakyButtonKind.Primary, available, "SR.AudioBrowser.ResolveFirst".Translate())) Audition(clips[clipIndex]);
        y += 44f;
    }

    private void Resolve()
    {
        if (selectedReference == null) return;
        explicitlyResolved.Add(selectedReference.Sound);
        SqueakSoundAvailability result = SqueakSoundAvailabilityCache.Resolve(selectedReference.Sound);
        clipIndex = 0;
        status = "SR.AudioBrowser.ResolveResult".Translate(("SR.AudioBrowser.State." + result.State).Translate(), result.Clips.Count);
    }

    private void Audition(SqueakResolvedClip clip)
    {
        try
        {
            SubSoundDef? adapter = SqueakOnCameraPreviewAdapter.Get();
            if (adapter == null) { status = "SR.Preview.NotDispatched".Translate(); return; }
            Rand.PushState();
            try
            {
                float jitter = SqueakyRatkinMod.Settings.GetCanonicalMoodMod(previewMood).pitchJitter.RandomInRange;
                SoundInfo info = SoundInfo.OnCamera();
                info.testPlay = true;
                info.pitchFactor = previewPitch * jitter;
                info.volumeFactor = previewVolume;
                if (SampleOneShot.TryMakeAndPlay(adapter, clip.Clip, info) != null)
                {
                    hasPreviewSample = true;
                    previewSampleClipIndex = clipIndex;
                    previewSampleSoundDef = selectedReference?.Sound.defName ?? "";
                    previewSampleJitter = jitter;
                    previewSampleFinalPitch = info.pitchFactor;
                    status = "SR.Preview.Dispatched".Translate();
                }
                else status = "SR.Preview.NotDispatched".Translate();
            }
            finally { Rand.PopState(); }
        }
        catch (Exception ex) { status = "SR.AudioBrowser.RequestFailed".Translate(ex.GetType().Name); }
    }

    private void CopyTrace(SoundDef sound, IReadOnlyList<SqueakResolvedClip> clips)
    {
        SqueakSoundAvailability? availability = explicitlyResolved.Contains(sound) && SqueakSoundAvailabilityCache.TryGetCached(sound, out SqueakSoundAvailability found) ? found : null;
        SqueakResolvedClip? clip = clips.Count > 0 ? clips[Mathf.Clamp(clipIndex, 0, clips.Count - 1)] : null;
        SqueakMoodMod canonical = SqueakyRatkinMod.Settings.GetCanonicalMoodMod(previewMood);
        float jitter = canonical.pitchJitter.Average;
        bool sampledCurrent = hasPreviewSample && previewSampleClipIndex == clipIndex && previewSampleSoundDef == sound.defName;
        StringBuilder text = new();
        text.AppendLine("SR.AudioBrowser.CopySummary".Translate(selectedAnimal!.Label, SemanticLabel(selectedReference!.Action)));
        text.AppendLine("SR.AudioBrowser.CopySource".Translate(selectedReference.FieldPath, sound.defName, availability?.State.ToString() ?? SqueakSoundAvailabilityState.Unknown.ToString()));
        text.AppendLine("schema=SR.AnimalVoiceSample.v1");
        Add(text, "animal.def", selectedAnimal.Def.defName); Add(text, "animal.label", selectedAnimal.Label); Add(text, "animal.owner", selectedAnimal.Owner); Add(text, "animal.packageId", selectedAnimal.PackageId);
        Add(text, "lifeStage.index", selectedReference.LifeStageIndex); Add(text, "lifeStage.def", selectedReference.LifeStageDef); Add(text, "lifeStage.label", selectedReference.LifeStageLabel);
        Add(text, "semantic.category", selectedReference.Category); Add(text, "semantic.action", selectedReference.Action); Add(text, "semantic.fieldPath", selectedReference.FieldPath); Add(text, "semantic.mechanical", selectedReference.Mechanical);
        Add(text, "soundDef.def", sound.defName); Add(text, "soundDef.owner", sound.modContentPack?.Name ?? "core"); Add(text, "soundDef.context", sound.context); Add(text, "soundDef.sustain", sound.sustain);
        Add(text, "subSound.index", clip?.SubSoundIndex ?? -1); Add(text, "subSound.onCamera", clip?.SubSound.onCamera.ToString() ?? "unknown"); Add(text, "grain.index", clip?.GrainIndex ?? -1); Add(text, "grain.type", clip?.GrainType ?? "unknown"); Add(text, "grain.path", clip?.ClipPath ?? ""); Add(text, "grain.folder", clip?.FolderPath ?? "");
        Add(text, "resolved.name", clip?.Clip.name ?? ""); Add(text, "resolved.index", clip == null ? -1 : clipIndex); Add(text, "resolved.count", clips.Count); Add(text, "availability", availability?.State.ToString() ?? "Unknown"); Add(text, "error", availability?.Diagnostic ?? "not-resolved"); Add(text, "asset.provenance", "unknown/global lookup");
        Add(text, "preview.mode", "resolvedClipOnCameraAdapter/workbench"); Add(text, "preview.adapter.soundDef", SqueakOnCameraPreviewAdapter.SoundDefName); Add(text, "preview.adapter.subSound.onCamera", true); Add(text, "preview.mood", previewMood); Add(text, "preview.source", "canonical formal Mood + temporary workbench adjustment"); Add(text, "preview.base.pitch", 1f); Add(text, "preview.base.volume", 1f); Add(text, "preview.mood.pitch", canonical.pitchFactor); Add(text, "preview.mood.volume", canonical.volumeFactor); Add(text, "preview.adjustment.pitch", previewPitch / Mathf.Max(.0001f, canonical.pitchFactor)); Add(text, "preview.adjustment.volume", previewVolume / Mathf.Max(.0001f, canonical.volumeFactor)); Add(text, "preview.final.pitch", sampledCurrent ? previewSampleFinalPitch : previewPitch * jitter); Add(text, "preview.final.pitch.kind", sampledCurrent ? "sampled" : "planned-average"); Add(text, "preview.final.volume", previewVolume); Add(text, "preview.jitter", canonical.pitchJitter); Add(text, "preview.jitter.sampled", sampledCurrent ? previewSampleJitter : "not-sampled");
        GUIUtility.systemCopyBuffer = text.ToString();
        status = "SR.AudioBrowser.Copied".Translate();
    }

    private static void Add(StringBuilder text, string key, object? value) => text.Append(key).Append('=').AppendLine(value?.ToString()?.Replace("\r", " ").Replace("\n", " ") ?? "");
    private void ResetPreviewMood() { SqueakMoodMod mood = SqueakyRatkinMod.Settings.GetCanonicalMoodMod(previewMood); previewPitch = mood.pitchFactor; previewVolume = mood.volumeFactor; pitchBuffer = previewPitch.ToString("0.##"); volumeBuffer = previewVolume.ToString("0.##"); hasPreviewSample = false; }
    private void RefreshAnimalFilter(bool force = false) { if (!force && lastAnimalQuery == animalQuery) return; lastAnimalQuery = animalQuery; filteredAnimals.Clear(); filteredAnimals.AddRange(animals.Where(x => animalQuery.Trim().Length == 0 || Contains(x.Search, animalQuery.Trim()))); }
    private void RefreshReferenceFilter() { if (selectedAnimal == null) return; if (lastReferenceQuery == referenceQuery && lastHideMechanical == hideMechanical && selectedAnimal.Filtered.Count > 0) return; lastReferenceQuery = referenceQuery; lastHideMechanical = hideMechanical; selectedAnimal.Filtered.Clear(); selectedAnimal.Filtered.AddRange(selectedAnimal.References.Where(x => (!hideMechanical || !x.Mechanical) && (referenceQuery.Trim().Length == 0 || Contains(x.Search, referenceQuery.Trim())))); }
    private void SelectAnimal(AnimalEntry? animal) { selectedAnimal = animal; referenceScroll = Vector2.zero; referenceQuery = ""; lastReferenceQuery = "\0"; RefreshReferenceFilter(); SelectReference(animal?.Filtered.FirstOrDefault()); }
    private void SelectReference(VoiceReference? reference) { selectedReference = reference; clipIndex = 0; detailScroll = Vector2.zero; status = ""; previewSampleSoundDef = ""; previewSampleClipIndex = -1; ResetPreviewMood(); }
    private IReadOnlyList<SqueakResolvedClip> VisibleClips(SoundDef sound) => explicitlyResolved.Contains(sound) && SqueakSoundAvailabilityCache.TryGetCached(sound, out SqueakSoundAvailability availability) ? availability.Clips : Array.Empty<SqueakResolvedClip>();
    private static object? ReadMember(object? owner, string name) { if (owner == null) return null; Type? type = owner.GetType(); while (type != null) { FieldInfo? field = type.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic); if (field != null) { try { return field.GetValue(owner); } catch { return null; } } type = type.BaseType; } return null; }
    private static bool Contains(string text, string query) => text.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0;
    private static string SemanticLabel(string action) => ("SR.AudioBrowser.Semantic." + action).Translate();
    private static string JoinSearch(params string[] values) => string.Join("\n", values.Where(x => !string.IsNullOrEmpty(x)));
    private static void LabelLine(Rect rect, string text) => SqueakySettingsUI.EllipsizedLabel(rect, text, text);
    private static void Wrapped(ref float y, float width, string text, bool tooltip = false)
    {
        float textWidth = tooltip ? Mathf.Max(1f, width - SqueakySettingsUI.HelpSize - SqueakySettingsUI.HelpGap) : width;
        float height = Mathf.Max(22f, Text.CalcHeight(text, textWidth));
        Rect rect = new(0f, y, width, height);
        if (tooltip) SqueakySettingsUI.LabelWithHelp(rect, text, text, Text.Font);
        else Widgets.Label(rect, text);
        y += height + 5f;
    }
    private static void Meta(ref float y, float width, string label, string value) => Wrapped(ref y, width, label + ": " + value, true);
    private static float SliderField(Rect rect, string label, float value, ref string buffer, float min, float max)
    {
        Widgets.DrawBoxSolid(rect, new Color(.07f, .067f, .061f, .52f));
        float labelWidth = Mathf.Min(130f, rect.width * .32f), fieldWidth = 60f;
        Widgets.Label(new Rect(rect.x, rect.y, labelWidth, rect.height), label);
        float next = Widgets.HorizontalSlider(new Rect(rect.x + labelWidth + 6f, rect.y, rect.width - labelWidth - fieldWidth - 12f, rect.height), value, min, max);
        if (Math.Abs(next - value) > .0001f) buffer = next.ToString("0.##");
        Widgets.TextFieldNumeric(new Rect(rect.xMax - fieldWidth, rect.y, fieldWidth, rect.height), ref next, ref buffer, min, max);
        return Mathf.Clamp(next, min, max);
    }

    private static void DrawListRowSurface(Rect rect, bool selected)
    {
        Widgets.DrawBoxSolid(rect, selected ? SqueakySettingsUI.Selected
            : Mouse.IsOver(rect) ? SqueakySettingsUI.Raised : SqueakySettingsUI.Panel);
        SqueakySettingsUI.DrawBorder(rect);
        if (selected) Widgets.DrawBoxSolid(new Rect(rect.x + 1f, rect.y + 1f, 3f, rect.height - 2f), SqueakySettingsUI.Gold);
    }
}
