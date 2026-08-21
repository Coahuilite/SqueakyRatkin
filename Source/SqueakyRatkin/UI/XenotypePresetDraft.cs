using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace SqueakyRatkin;

/// <summary>UI-only, field-preserving edit copy for one xenotype.</summary>
internal sealed class XenotypePresetDraft
{
    internal readonly string XenotypeDefName;
    internal bool HasOverall;
    internal float Overall = 1f;
    internal readonly Dictionary<SqueakAction, XenotypeActionBehaviorOverride> Actions = new();
    internal readonly Dictionary<SqueakMood, XenotypeMoodOverride> Moods = new();
    private long revision;
    private long committedRevision;

    internal bool Dirty => revision != committedRevision;
    internal long Revision => revision;

    private XenotypePresetDraft(string defName) => XenotypeDefName = defName;

    internal static XenotypePresetDraft FromRecords(IEnumerable<XenotypePresetRecord> presets, string defName,
        IEnumerable<SqueakAction> actions, IEnumerable<SqueakMood> moods)
    {
        XenotypePresetDraft draft = new(defName);
        foreach (SqueakAction action in actions) draft.Actions[action] = new XenotypeActionBehaviorOverride { action = action };
        foreach (SqueakMood mood in moods) draft.Moods[mood] = new XenotypeMoodOverride { mood = mood };

        // 2b-1 preserves the existing Ratkin UI projection; 2b-2 adds explicit race identity to this draft.
        foreach (XenotypePresetRecord record in presets.Where(x => x != null && x.xenotypeDefName == defName))
        {
            if (record.hasOverallIntervalMultiplier) { draft.HasOverall = true; draft.Overall = Safe(record.overallIntervalMultiplier, 1f); }
            foreach (XenotypeActionBehaviorOverride source in record.actionOverrides ?? new())
            {
                if (source == null || !draft.Actions.TryGetValue(source.action, out XenotypeActionBehaviorOverride target)) continue;
                if (source.hasEnabled) { target.hasEnabled = true; target.enabled = source.enabled; }
                if (source.hasIntervalMultiplier) { target.hasIntervalMultiplier = true; target.intervalMultiplier = Safe(source.intervalMultiplier, 1f); }
                if (source.hasProbabilityMultiplier) { target.hasProbabilityMultiplier = true; target.probabilityMultiplier = Safe(source.probabilityMultiplier, 1f); }
            }
            foreach (XenotypeMoodOverride source in record.moodOverrides ?? new())
            {
                if (source == null || !draft.Moods.TryGetValue(source.mood, out XenotypeMoodOverride target)) continue;
                if (source.hasPitchFactor) { target.hasPitchFactor = true; target.pitchFactor = Safe(source.pitchFactor, 1f); }
                if (source.hasVolumeFactor) { target.hasVolumeFactor = true; target.volumeFactor = Safe(source.volumeFactor, 1f); }
                if (source.hasPitchJitter)
                {
                    target.hasPitchJitter = true;
                    float a = Safe(source.pitchJitter.min, 1f), b = Safe(source.pitchJitter.max, 1f);
                    target.pitchJitter = new FloatRange(Mathf.Min(a, b), Mathf.Max(a, b));
                }
            }
        }

        draft.Normalize();
        draft.committedRevision = draft.revision;
        return draft;
    }

    internal void MarkChanged() => revision++;

    internal void Commit(List<XenotypePresetRecord> presets)
    {
        Normalize();
        presets.RemoveAll(x => x != null && x.xenotypeDefName == XenotypeDefName);
        XenotypePresetRecord canonical = new() { raceDefName = SqueakProductDomainFilter.PrimaryRaceDefName, xenotypeDefName = XenotypeDefName, hasOverallIntervalMultiplier = HasOverall, overallIntervalMultiplier = Overall };
        canonical.actionOverrides.AddRange(Actions.Values.Where(HasDelta).Select(Clone));
        canonical.moodOverrides.AddRange(Moods.Values.Where(HasDelta).Select(Clone));
        if (HasOverall || canonical.actionOverrides.Count > 0 || canonical.moodOverrides.Count > 0) presets.Add(canonical);
        committedRevision = revision;
    }
    internal void Normalize()
    {
        Overall = Clamp(Overall, 0f, 5f, 1f);
        foreach (XenotypeActionBehaviorOverride a in Actions.Values) { a.intervalMultiplier = Clamp(a.intervalMultiplier, 0f, 5f, 1f); a.probabilityMultiplier = Clamp(a.probabilityMultiplier, 0f, 5f, 1f); }
        foreach (XenotypeMoodOverride m in Moods.Values)
        {
            m.pitchFactor = Clamp(m.pitchFactor, .5f, 2f, 1f); m.volumeFactor = Clamp(m.volumeFactor, 0f, 2f, 1f);
            float a = Clamp(m.pitchJitter.min, .5f, 1.5f, 1f), b = Clamp(m.pitchJitter.max, .5f, 1.5f, 1f);
            m.pitchJitter = new FloatRange(Mathf.Min(a, b), Mathf.Max(a, b));
        }
    }

    private static bool HasDelta(XenotypeActionBehaviorOverride x) => x.hasEnabled || x.hasIntervalMultiplier || x.hasProbabilityMultiplier;
    private static bool HasDelta(XenotypeMoodOverride x) => x.hasPitchFactor || x.hasVolumeFactor || x.hasPitchJitter;
    private static XenotypeActionBehaviorOverride Clone(XenotypeActionBehaviorOverride x) => new() { action=x.action, hasEnabled=x.hasEnabled, enabled=x.enabled, hasIntervalMultiplier=x.hasIntervalMultiplier, intervalMultiplier=x.intervalMultiplier, hasProbabilityMultiplier=x.hasProbabilityMultiplier, probabilityMultiplier=x.probabilityMultiplier };
    private static XenotypeMoodOverride Clone(XenotypeMoodOverride x) => new() { mood=x.mood, hasPitchFactor=x.hasPitchFactor, pitchFactor=x.pitchFactor, hasVolumeFactor=x.hasVolumeFactor, volumeFactor=x.volumeFactor, hasPitchJitter=x.hasPitchJitter, pitchJitter=x.pitchJitter };
    private static float Safe(float value, float fallback) => float.IsNaN(value) || float.IsInfinity(value) ? fallback : value;
    private static float Clamp(float value, float min, float max, float fallback) => Mathf.Clamp(Safe(value, fallback), min, max);
}
