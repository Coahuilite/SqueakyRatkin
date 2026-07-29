using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using UnityEngine;
using RimWorld;
using Verse;
using Verse.Sound;

namespace SqueakyRatkin;

/// <summary>Immutable resolver: audio selection is independent of optional Xenotype behavior and mood deltas.</summary>
public static class SqueakRuntimeResolver
{
    private static SqueakRuntimeSnapshot current = SqueakRuntimeSnapshot.GlobalOnly;
    private const float ContinuousDelaySeconds = .075f;
    private const float ContinuousMaxWaitSeconds = .150f;
    private static SqueakyRatkinSettings? pendingSettings;
    private static SqueakXenotypeCatalogSnapshot? pendingCatalog;
    private static long desiredRevision;
    private static long appliedRevision;
    private static bool pendingIsDiscrete;
    private static float continuousFirstChangeAt;
    private static float continuousDueAt;
    private static float lastContinuousPublishAt = float.NegativeInfinity;
    private static int mainThreadId;
    public static long ResolverRebuildCount { get; private set; }
    public static long RuntimeFlushCount { get; private set; }
    public static SqueakRuntimeSnapshot Current => Volatile.Read(ref current);

    /// <summary>Called by the mod constructor on Unity's main thread. BuildSnapshot reads mutable settings, so this resolver has one publisher.</summary>
    internal static void InitializeMainThread()
    {
        int caller = Thread.CurrentThread.ManagedThreadId;
        if (mainThreadId == 0) mainThreadId = caller;
        else Debug.Assert(mainThreadId == caller, "SqueakRuntimeResolver main thread changed.");
    }

    private static bool EnsureMainThread()
    {
        bool valid = mainThreadId != 0 && mainThreadId == Thread.CurrentThread.ManagedThreadId;
        Debug.Assert(valid, "SqueakRuntimeResolver mutation attempted off Unity's initialized main thread.");
        return valid;
    }

    /// <summary>Use for sliders/editor deltas that affect Xenotype resolver data. Trailing 75 ms, bounded at 150 ms.</summary>
    public static void NotifyContinuousResolverChange(SqueakyRatkinSettings settings, SqueakXenotypeCatalogSnapshot catalog)
    {
        if (!EnsureMainThread()) return;
        float now = Time.realtimeSinceStartup;
        pendingSettings = settings; pendingCatalog = catalog;
        if (desiredRevision == appliedRevision) continuousFirstChangeAt = now;
        desiredRevision++;
        // Next due = max(last publish + 150ms, min(last change + 75ms, first dirty + 150ms)).
        // Thus steady input publishes no faster than 150ms, while each pending burst remains bounded by 150ms.
        continuousDueAt = Mathf.Max(lastContinuousPublishAt + ContinuousMaxWaitSeconds,
            Mathf.Min(now + ContinuousDelaySeconds, continuousFirstChangeAt + ContinuousMaxWaitSeconds));
    }

    /// <summary>Use for audio mode, selection, action scope, and catalog changes. It is effective before returning.</summary>
    public static void NotifyDiscreteResolverChange(SqueakyRatkinSettings settings, SqueakXenotypeCatalogSnapshot catalog)
    {
        if (!EnsureMainThread()) return;
        pendingSettings = settings; pendingCatalog = catalog;
        desiredRevision++;
        pendingIsDiscrete = true;
        continuousFirstChangeAt = Time.realtimeSinceStartup;
        continuousDueAt = continuousFirstChangeAt;
        FlushPendingRuntimeChanges(true);
    }

    public static void TickPendingRuntimeChanges()
    {
        if (!EnsureMainThread()) return;
        FlushPendingRuntimeChanges(false);
    }

    /// <summary>Flushes a due continuous edit, or all pending work when forced. Never writes settings.</summary>
    public static void FlushPendingRuntimeChanges(bool force = true)
    {
        if (!EnsureMainThread()) return;
        if (desiredRevision == appliedRevision || pendingSettings == null || pendingCatalog == null) return;
        float now = Time.realtimeSinceStartup;
        if (!force && !pendingIsDiscrete && now < continuousDueAt) return;
        long revision = desiredRevision;
        if (!TryPublish(pendingSettings, pendingCatalog, out _)) return;
        appliedRevision = revision;
        if (!pendingIsDiscrete) lastContinuousPublishAt = now;
        if (appliedRevision == desiredRevision)
        {
            pendingIsDiscrete = false;
            pendingSettings = null;
            pendingCatalog = null;
        }
    }

    /// <summary>Builds and publishes exactly one immutable snapshot. Counters advance only after publication.</summary>
    private static bool TryPublish(SqueakyRatkinSettings settings, SqueakXenotypeCatalogSnapshot catalog, out SqueakRuntimeSnapshot published)
    {
        if (!EnsureMainThread()) { published = Current; return false; }
        Dictionary<SqueakAction, RuntimeActionDelta> globalActions = BuildGlobalActions(settings);
        try { published = BuildSnapshot(settings, catalog, globalActions); }
        catch (Exception ex) { SqueakLog.ResolverRebuildFailed(ex); published = BuildFallback(globalActions); }
        Volatile.Write(ref current, published);
        ResolverRebuildCount++;
        RuntimeFlushCount++;
        return true;
    }

    private static Dictionary<SqueakAction, RuntimeActionDelta> BuildGlobalActions(SqueakyRatkinSettings settings)
    {
        Dictionary<SqueakAction, RuntimeActionDelta> result = new();
        foreach (SqueakAction action in Enum.GetValues(typeof(SqueakAction))) result[action] = new RuntimeActionDelta(settings.GetActionGlobalScope(action), 1f, 1f);
        return result;
    }

    private static SqueakRuntimeSnapshot BuildSnapshot(SqueakyRatkinSettings settings, SqueakXenotypeCatalogSnapshot catalog, Dictionary<SqueakAction, RuntimeActionDelta> globalActions)
    {
        Dictionary<string, RuntimeBuilder> behavior = BuildBehavior(settings);
        Dictionary<string, HashSet<string>> selection = BuildSelections(settings.voicePackSelections);
        Dictionary<SqueakAction, SoundDef?> vanilla = GetVanilla(out HashSet<SoundDef> known);
        Dictionary<string, ResolvedSqueakContext> contexts = new(StringComparer.Ordinal);
        if (ModsConfig.BiotechActive)
        {
            HashSet<string> targets = new(catalog.XenotypePacksByDefName.Keys, StringComparer.Ordinal);
            foreach (VoicePackSelectionRecord record in settings.voicePackSelections ?? new List<VoicePackSelectionRecord>())
                if (record != null && record.scope == SqueakVoicePackScope.Xenotype && !string.IsNullOrEmpty(record.targetDefName)) targets.Add(record.targetDefName);
            foreach (string target in behavior.Keys) targets.Add(target);
            foreach (string target in catalog.HarHintDefNames) targets.Add(target);
            foreach (string target in targets)
            {
                catalog.XenotypeByDefName.TryGetValue(target, out XenotypeDef? xenotype);
                if (catalog.AmbiguousCanonicalDefNames.Contains(target)) xenotype = null;
                contexts.Add(target, BuildContext(xenotype, behavior.TryGetValue(target, out RuntimeBuilder? builder) ? builder : null, GetSelected(catalog.GetVoicePackDomainPacks(SqueakVoicePackScope.Xenotype, target), selection, SqueakVoicePackScope.Xenotype, target), globalActions));
            }
        }
        List<ResolvedAudioPack> race = GetSelected(catalog.GetVoicePackDomainPacks(SqueakVoicePackScope.Race, ""), selection, SqueakVoicePackScope.Race, "");
        AddKnown(known, catalog.PackByKey.Values.Select(p => new ResolvedAudioPack(p.TryGetPackKey(out string key) ? key : "", p))); foreach (ResolvedSqueakContext context in contexts.Values) context.AddSoundsTo(known);
        return new SqueakRuntimeSnapshot(contexts, race, known, vanilla, NormalizeMode(settings.voicePackMode), globalActions, catalog.AmbiguousCanonicalDefNames);
    }

    private static Dictionary<string, RuntimeBuilder> BuildBehavior(SqueakyRatkinSettings settings)
    {
        Dictionary<string, RuntimeBuilder> builders = new(StringComparer.Ordinal);
        foreach (XenotypePresetRecord record in settings.xenotypePresets ?? new List<XenotypePresetRecord>())
        {
            if (record == null || string.IsNullOrEmpty(record.xenotypeDefName)) continue;
            if (!builders.TryGetValue(record.xenotypeDefName, out RuntimeBuilder? builder)) { builder = new RuntimeBuilder(); builders.Add(record.xenotypeDefName, builder); }
            if (record.hasOverallIntervalMultiplier) builder.overallIntervalMultiplier = Sanitize(record.overallIntervalMultiplier);
            foreach (XenotypeMoodOverride mood in record.moodOverrides ?? new List<XenotypeMoodOverride>()) { if (mood == null) continue; RuntimeMoodBuilder b = builder.GetMood(mood.mood); if (mood.hasPitchFactor) b.SetPitch(mood.pitchFactor); if (mood.hasVolumeFactor) b.SetVolume(mood.volumeFactor); if (mood.hasPitchJitter) b.SetJitter(mood.pitchJitter); }
            foreach (XenotypeActionBehaviorOverride action in record.actionOverrides ?? new List<XenotypeActionBehaviorOverride>()) { if (action == null) continue; RuntimeActionBuilder b = builder.GetAction(action.action); if (action.hasEnabled) b.Enabled = action.enabled; if (action.hasIntervalMultiplier) b.IntervalMultiplier = Sanitize(action.intervalMultiplier); if (action.hasProbabilityMultiplier) b.ProbabilityMultiplier = Sanitize(action.probabilityMultiplier); }
        }
        return builders;
    }

    private static Dictionary<string, HashSet<string>> BuildSelections(IEnumerable<VoicePackSelectionRecord> records)
    {
        Dictionary<string, HashSet<string>> result = new(StringComparer.Ordinal);
        foreach (VoicePackSelectionRecord record in records ?? Array.Empty<VoicePackSelectionRecord>())
        {
            if (record == null || (record.scope != SqueakVoicePackScope.Race && record.scope != SqueakVoicePackScope.Xenotype) || (record.scope == SqueakVoicePackScope.Xenotype && string.IsNullOrEmpty(record.targetDefName))) continue;
            result[record.DomainKey] = new HashSet<string>((record.enabledPackKeys ?? new List<string>()).Where(k => !string.IsNullOrEmpty(k)), StringComparer.Ordinal);
        }
        return result;
    }

    private static List<ResolvedAudioPack> GetSelected(IReadOnlyList<SqueakVoicePackDef> candidates, Dictionary<string, HashSet<string>> selections, SqueakVoicePackScope scope, string target)
    {
        if (!selections.TryGetValue(VoicePackSelectionRecord.ComposeDomainKey(scope, target), out HashSet<string>? keys)) return new List<ResolvedAudioPack>();
        List<ResolvedAudioPack> result = new();
        foreach (SqueakVoicePackDef pack in candidates) if (pack.TryGetPackKey(out string key) && keys.Contains(key)) { ResolvedAudioPack resolved = new(key, pack); if (resolved.HasSounds) result.Add(resolved); }
        result.Sort((a, b) => StringComparer.Ordinal.Compare(a.PackKey, b.PackKey)); return result;
    }

    private static ResolvedSqueakContext BuildContext(XenotypeDef? xenotype, RuntimeBuilder? builder, List<ResolvedAudioPack> packs, Dictionary<SqueakAction, RuntimeActionDelta> globals)
    {
        Dictionary<SqueakAction, RuntimeActionDelta> actions = new(globals);
        foreach (KeyValuePair<SqueakAction, RuntimeActionDelta> item in builder?.BuildActions() ?? new Dictionary<SqueakAction, RuntimeActionDelta>())
        {
            RuntimeActionDelta global = globals[item.Key]; actions[item.Key] = new RuntimeActionDelta(global.Enabled && item.Value.Enabled ? global.Scope : SqueakActionScope.Disabled, item.Value.IntervalMultiplier, item.Value.ProbabilityMultiplier);
        }
        return new ResolvedSqueakContext(xenotype, builder?.overallIntervalMultiplier ?? 1f, actions, builder?.BuildMoods(), packs);
    }

    private static Dictionary<SqueakAction, SoundDef?> GetVanilla(out HashSet<SoundDef> known) { known = new HashSet<SoundDef>(); Dictionary<SqueakAction, SoundDef?> result = new(); foreach (SqueakAction a in Enum.GetValues(typeof(SqueakAction))) { SoundDef? sound = DefDatabase<SoundDef>.GetNamedSilentFail(SqueakActionDefinitions.Get(a).AudioKey); result[a] = sound; if (sound != null) known.Add(sound); } return result; }
    private static SqueakRuntimeSnapshot BuildFallback(Dictionary<SqueakAction, RuntimeActionDelta> actions) { try { Dictionary<SqueakAction, SoundDef?> vanilla = GetVanilla(out HashSet<SoundDef> known); return new SqueakRuntimeSnapshot(new Dictionary<string, ResolvedSqueakContext>(), new List<ResolvedAudioPack>(), known, vanilla, SqueakVoicePackMode.Off, actions, null); } catch { return SqueakRuntimeSnapshot.GlobalOnly; } }
    private static SqueakVoicePackMode NormalizeMode(SqueakVoicePackMode mode) => mode == SqueakVoicePackMode.Fallback || mode == SqueakVoicePackMode.Remix ? mode : SqueakVoicePackMode.Off;
    private static float Sanitize(float value) => float.IsNaN(value) || float.IsInfinity(value) ? 1f : Math.Max(0f, value);
    private static void AddKnown(HashSet<SoundDef> known, IEnumerable<ResolvedAudioPack> packs) { foreach (ResolvedAudioPack pack in packs) foreach (SoundDef sound in pack.AllSounds) known.Add(sound); }

    private sealed class RuntimeBuilder { public float overallIntervalMultiplier = 1f; private readonly Dictionary<SqueakAction, RuntimeActionBuilder> actions = new(); private readonly Dictionary<SqueakMood, RuntimeMoodBuilder> moods = new(); public RuntimeActionBuilder GetAction(SqueakAction a) { if (!actions.TryGetValue(a, out RuntimeActionBuilder? v)) { v = new RuntimeActionBuilder(); actions.Add(a, v); } return v; } public RuntimeMoodBuilder GetMood(SqueakMood m) { if (!moods.TryGetValue(m, out RuntimeMoodBuilder? v)) { v = new RuntimeMoodBuilder(); moods.Add(m, v); } return v; } public Dictionary<SqueakAction, RuntimeActionDelta> BuildActions() => actions.ToDictionary(x => x.Key, x => x.Value.Build()); public Dictionary<SqueakMood, RuntimeMoodDelta> BuildMoods() => moods.ToDictionary(x => x.Key, x => x.Value.Build()); }
    private sealed class RuntimeActionBuilder { public bool Enabled = true; public float IntervalMultiplier = 1f; public float ProbabilityMultiplier = 1f; public RuntimeActionDelta Build() => new(Enabled ? SqueakActionScope.AnyOccurrence : SqueakActionScope.Disabled, IntervalMultiplier, ProbabilityMultiplier); }
    private sealed class RuntimeMoodBuilder { private bool hp, hv, hj; private float p = 1f, v = 1f; private FloatRange j = FloatRange.One; public void SetPitch(float x) { hp = true; p = x; } public void SetVolume(float x) { hv = true; v = x; } public void SetJitter(FloatRange x) { hj = true; j = x; } public RuntimeMoodDelta Build() => new(hp, p, hv, v, hj, j); }
}

public sealed class SqueakRuntimeSnapshot
{
    public static readonly SqueakRuntimeSnapshot GlobalOnly = new(new Dictionary<string, ResolvedSqueakContext>(), new List<ResolvedAudioPack>(), new HashSet<SoundDef>(), new Dictionary<SqueakAction, SoundDef?>(), SqueakVoicePackMode.Off, null, null);
    private readonly IReadOnlyDictionary<string, ResolvedSqueakContext> contexts; private readonly IReadOnlyList<ResolvedAudioPack> racePacks; private readonly IReadOnlyDictionary<SqueakAction, SoundDef?> vanilla; private readonly IReadOnlyDictionary<SqueakAction, RuntimeActionDelta> globalActions; private readonly ResolvedSqueakContext globalContext;
    public readonly SqueakVoicePackMode VoicePackMode; public readonly IReadOnlyCollection<SoundDef> KnownMapSoundDefs;
    private readonly IReadOnlyCollection<string> ambiguousCanonicalNames;
    internal SqueakRuntimeSnapshot(Dictionary<string, ResolvedSqueakContext> contexts, List<ResolvedAudioPack> race, HashSet<SoundDef> known, Dictionary<SqueakAction, SoundDef?> vanilla, SqueakVoicePackMode mode, Dictionary<SqueakAction, RuntimeActionDelta>? globals, IEnumerable<string>? ambiguousNames) { this.contexts = new ReadOnlyDictionary<string, ResolvedSqueakContext>(contexts); racePacks = new ReadOnlyCollection<ResolvedAudioPack>(race); this.vanilla = new ReadOnlyDictionary<SqueakAction, SoundDef?>(vanilla); globalActions = new ReadOnlyDictionary<SqueakAction, RuntimeActionDelta>(globals ?? new Dictionary<SqueakAction, RuntimeActionDelta>()); globalContext = new ResolvedSqueakContext(null, 1f, globals, null, null); VoicePackMode = mode; KnownMapSoundDefs = new ReadOnlyCollection<SoundDef>(known.ToList()); ambiguousCanonicalNames = new ReadOnlyCollection<string>((ambiguousNames ?? Array.Empty<string>()).ToList()); }
    public ResolvedSqueakContext ResolveContext(Pawn pawn)
    {
        if (!ModsConfig.BiotechActive) return globalContext;
        XenotypeDef? xenotype = pawn?.genes?.Xenotype;
        string? defName = xenotype?.defName;
        if (string.IsNullOrEmpty(defName)) return globalContext;
        string exactDefName = defName!;
        if (!contexts.TryGetValue(exactDefName, out ResolvedSqueakContext? context)) return globalContext;
        if (ambiguousCanonicalNames.Contains(exactDefName)) return WarnAndFallback(exactDefName, "multiple loaded XenotypeDef instances");
        if (context.Xenotype != null && !ReferenceEquals(context.Xenotype, xenotype)) return WarnAndFallback(exactDefName, "runtime XenotypeDef differs from the unique canonical instance");
        return context;
    }
    private ResolvedSqueakContext WarnAndFallback(string defName, string reason)
    {
        SqueakLog.TargetRejected(defName, reason);
        return globalContext;
    }
    public SqueakActionScope GetGlobalScope(SqueakAction action) => globalActions.TryGetValue(action, out RuntimeActionDelta? value) ? value.Scope : SqueakActionDefinitions.Get(action).DefaultScope;
    public SqueakSoundChoice ChooseNativeSound(ResolvedSqueakContext context, SqueakAction action, Map? map, TargetInfo? target) => Choose(context, action, null, map, target, false);
    public SqueakSoundChoice ChooseProductionSound(ResolvedSqueakContext context, SqueakAction action, Pawn pawn) => Choose(context, action, pawn, pawn.MapHeld, new TargetInfo(pawn), true);
    private SqueakSoundChoice Choose(ResolvedSqueakContext context, SqueakAction action, Pawn? pawn, Map? map, TargetInfo? target, bool production)
    {
        SoundDef? v = vanilla.TryGetValue(action, out SoundDef? value) ? value : null; if (VoicePackMode == SqueakVoicePackMode.Off) return Choice(v, SqueakSoundSource.Vanilla, pawn, map, target, production);
        SqueakSoundChoice x = ChoosePack(context.Packs, action, SqueakSoundSource.XenotypePack, pawn, map, target, production); SqueakSoundChoice r = ChoosePack(racePacks, action, SqueakSoundSource.RacePack, pawn, map, target, production); SqueakSoundChoice vanillaChoice = Choice(v, SqueakSoundSource.Vanilla, pawn, map, target, production);
        if (VoicePackMode == SqueakVoicePackMode.Fallback) return x.Or(r).Or(vanillaChoice);
        List<SqueakSoundChoice> tiers = new(); if (!x.IsNone) tiers.Add(x); if (!r.IsNone) tiers.Add(r); if (!vanillaChoice.IsNone) tiers.Add(vanillaChoice); return tiers.Count == 0 ? SqueakSoundChoice.None : tiers[tiers.Count == 1 ? 0 : Rand.Range(0, tiers.Count)];
    }
    private static SqueakSoundChoice ChoosePack(IReadOnlyList<ResolvedAudioPack> packs, SqueakAction action, SqueakSoundSource source, Pawn? pawn, Map? map, TargetInfo? target, bool production) { List<ResolvedAudioPack> valid = packs.Where(p => p.HasPlayable(action, pawn, map, target, production)).ToList(); if (valid.Count == 0) return SqueakSoundChoice.None; ResolvedAudioPack pack = valid[valid.Count == 1 ? 0 : Rand.Range(0, valid.Count)]; SoundDef? sound = pack.Choose(action, pawn, map, target, production); return sound == null ? SqueakSoundChoice.None : new SqueakSoundChoice(sound, source, pack.PackKey); }
    private static SqueakSoundChoice Choice(SoundDef? sound, SqueakSoundSource source, Pawn? pawn, Map? map, TargetInfo? target, bool production) => Playable(sound, pawn, map, target, production) ? new SqueakSoundChoice(sound, source, null) : SqueakSoundChoice.None;
    internal static bool Playable(SoundDef? sound, Pawn? pawn, Map? map, TargetInfo? target, bool production) => (production ? SqueakSoundAvailabilityCache.GetProductionPlayability(sound, pawn) : SqueakSoundAvailabilityCache.GetNativePlayability(sound, map, target)) == SqueakSoundPlayability.Playable;
}

public enum SqueakSoundSource { None, XenotypePack, RacePack, Vanilla }
public readonly struct SqueakSoundChoice { public static readonly SqueakSoundChoice None = default; public readonly SoundDef? Sound; public readonly SqueakSoundSource Source; public readonly string? PoolStableKey; public bool IsNone => Sound == null || Source == SqueakSoundSource.None; internal SqueakSoundChoice(SoundDef? sound, SqueakSoundSource source, string? packKey) { Sound = sound; Source = source; PoolStableKey = packKey; } public SqueakSoundChoice Or(SqueakSoundChoice fallback) => IsNone ? fallback : this; }
public sealed class ResolvedSqueakContext { public static readonly ResolvedSqueakContext GlobalOnly = new(null, 1f, null, null, null); public readonly XenotypeDef? Xenotype; public readonly float OverallIntervalMultiplier; public readonly IReadOnlyList<ResolvedAudioPack> Packs; private readonly IReadOnlyDictionary<SqueakAction, RuntimeActionDelta> actions; private readonly IReadOnlyDictionary<SqueakMood, RuntimeMoodDelta> moods; internal ResolvedSqueakContext(XenotypeDef? x, float interval, Dictionary<SqueakAction, RuntimeActionDelta>? a, Dictionary<SqueakMood, RuntimeMoodDelta>? m, List<ResolvedAudioPack>? p) { Xenotype = x; OverallIntervalMultiplier = interval; actions = new ReadOnlyDictionary<SqueakAction, RuntimeActionDelta>(a ?? new Dictionary<SqueakAction, RuntimeActionDelta>()); moods = new ReadOnlyDictionary<SqueakMood, RuntimeMoodDelta>(m ?? new Dictionary<SqueakMood, RuntimeMoodDelta>()); Packs = new ReadOnlyCollection<ResolvedAudioPack>(p ?? new List<ResolvedAudioPack>()); } public RuntimeActionDelta GetAction(SqueakAction a) => actions.TryGetValue(a, out RuntimeActionDelta? v) ? v : RuntimeActionDelta.Default; public bool TryGetMood(SqueakMood m, out RuntimeMoodDelta d) => moods.TryGetValue(m, out d!); public RuntimeMoodDelta? GetMoodDelta(SqueakMood m) => moods.TryGetValue(m, out RuntimeMoodDelta? v) ? v : null; internal void AddSoundsTo(HashSet<SoundDef> known) { foreach (ResolvedAudioPack p in Packs) foreach (SoundDef s in p.AllSounds) known.Add(s); } }
public sealed class ResolvedAudioPack { public readonly string PackKey; private readonly IReadOnlyDictionary<SqueakAction, IReadOnlyList<SoundDef>> sounds; public IEnumerable<SoundDef> AllSounds => sounds.Values.SelectMany(x => x); public bool HasSounds => sounds.Count > 0; internal ResolvedAudioPack(string key, SqueakVoicePackDef pack) { PackKey = key; Dictionary<SqueakAction, IReadOnlyList<SoundDef>> map = new(); foreach (SqueakVoicePackAction entry in pack.actions ?? new List<SqueakVoicePackAction>()) if (entry != null && !map.ContainsKey(entry.action)) { List<SoundDef> list = (entry.sounds ?? new List<SoundDef>()).Where(s => s != null && !s.defName.EndsWith("_Preview", StringComparison.Ordinal)).Distinct().OrderBy(s => s.defName, StringComparer.Ordinal).ToList(); if (list.Count > 0) map.Add(entry.action, list); } sounds = new ReadOnlyDictionary<SqueakAction, IReadOnlyList<SoundDef>>(map); } public bool HasPlayable(SqueakAction a, Pawn? p, Map? m, TargetInfo? t, bool production) => sounds.TryGetValue(a, out IReadOnlyList<SoundDef>? list) && list.Any(s => SqueakRuntimeSnapshot.Playable(s, p, m, t, production)); public SoundDef? Choose(SqueakAction a, Pawn? p, Map? m, TargetInfo? t, bool production) { if (!sounds.TryGetValue(a, out IReadOnlyList<SoundDef>? list)) return null; List<SoundDef> valid = list.Where(s => SqueakRuntimeSnapshot.Playable(s, p, m, t, production)).ToList(); return valid.Count == 0 ? null : valid[valid.Count == 1 ? 0 : Rand.Range(0, valid.Count)]; } }
public sealed class RuntimeActionDelta { public static readonly RuntimeActionDelta Default = new(); public SqueakActionScope Scope { get; } public bool Enabled => Scope != SqueakActionScope.Disabled; public float IntervalMultiplier { get; } public float ProbabilityMultiplier { get; } internal RuntimeActionDelta(SqueakActionScope scope = SqueakActionScope.AnyOccurrence, float intervalMultiplier = 1f, float probabilityMultiplier = 1f) { Scope = scope; IntervalMultiplier = intervalMultiplier; ProbabilityMultiplier = probabilityMultiplier; } }
public sealed class RuntimeMoodDelta { public bool HasPitchFactor { get; } public float PitchFactor { get; } public bool HasVolumeFactor { get; } public float VolumeFactor { get; } public bool HasPitchJitter { get; } public FloatRange PitchJitter { get; } internal RuntimeMoodDelta(bool hp, float p, bool hv, float v, bool hj, FloatRange j) { HasPitchFactor = hp; PitchFactor = p; HasVolumeFactor = hv; VolumeFactor = v; HasPitchJitter = hj; PitchJitter = j; } }
