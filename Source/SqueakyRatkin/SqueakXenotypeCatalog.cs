using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using RimWorld;
using Verse;

namespace SqueakyRatkin;

/// <summary>Atomically published VoicePack catalog. Race discovery never touches Biotech Def databases.</summary>
public static class SqueakXenotypeCatalog
{
    private static SqueakXenotypeCatalogSnapshot current = SqueakXenotypeCatalogSnapshot.Empty;
    public static SqueakXenotypeCatalogSnapshot Current => Volatile.Read(ref current);

    public static void Refresh()
    {
        try
        {
            Dictionary<string, List<SqueakVoicePackDef>> groups = new(StringComparer.Ordinal);
            foreach (SqueakVoicePackDef pack in DefDatabase<SqueakVoicePackDef>.AllDefs)
            {
                if (!SqueakVoicePackValidator.IsValid(pack)) continue;
                if (pack.scope != SqueakVoicePackScope.Race && pack.scope != SqueakVoicePackScope.Xenotype) continue;
                if (!pack.TryGetPackKey(out string key)) continue;
                if (!groups.TryGetValue(key, out List<SqueakVoicePackDef>? entries)) { entries = new List<SqueakVoicePackDef>(); groups.Add(key, entries); }
                entries.Add(pack);
            }

            Dictionary<string, SqueakVoicePackDef> packsByKey = new(StringComparer.Ordinal);
            List<SqueakVoicePackDef> racePacks = new();
            foreach (KeyValuePair<string, List<SqueakVoicePackDef>> group in groups)
            {
                if (group.Value.Count != 1) { WarnDuplicatePackKey(group.Key, group.Value.Count); continue; }
                SqueakVoicePackDef pack = group.Value[0];
                packsByKey.Add(group.Key, pack);
                if (pack.scope == SqueakVoicePackScope.Race) racePacks.Add(pack);
            }

            // The Race domain is published before and independently from every optional DLC step.
            // Pack eligibility is strictly the declared, case-sensitive target string: HAR is UI-only.
            Dictionary<string, XenotypeDef> canonical = new(StringComparer.Ordinal);
            HashSet<string> ambiguousCanonicalNames = new(StringComparer.Ordinal);
            HashSet<string> harHints = new(StringComparer.Ordinal);
            HashSet<string> officialHarHints = new(StringComparer.Ordinal);
            bool discoveryAvailable = false;
            if (ModsConfig.BiotechActive)
            {
                try
                {
                    foreach (XenotypeDef xenotype in DefDatabase<XenotypeDef>.AllDefs)
                    {
                        if (string.IsNullOrEmpty(xenotype.defName)) continue;
                        if (canonical.ContainsKey(xenotype.defName))
                        {
                            canonical.Remove(xenotype.defName);
                            ambiguousCanonicalNames.Add(xenotype.defName);
                        }
                        else if (!ambiguousCanonicalNames.Contains(xenotype.defName)) canonical.Add(xenotype.defName, xenotype);
                    }
                    HarRatkinXenotypeDiscoveryResult discovery = HarRatkinXenotypeDiscovery.Discover();
                    discoveryAvailable = discovery.available;
                    if (discovery.available)
                        foreach (XenotypeDef found in discovery.xenotypes)
                        {
                            if (string.IsNullOrEmpty(found.defName)) continue;
                            bool officialOnly = found.modContentPack?.IsOfficialMod == true;
                            if (officialOnly) officialHarHints.Add(found.defName);
                            else harHints.Add(found.defName);
                        }
                }
                catch (Exception ex) { if (SqueakLog.ShouldEmitDev) SqueakLog.XenotypeDiscoveryFailed(ex); discoveryAvailable = false; }
            }

            Dictionary<string, List<SqueakVoicePackDef>> xenotypePacks = new(StringComparer.Ordinal);
            foreach (SqueakVoicePackDef pack in packsByKey.Values)
            {
                if (pack.scope != SqueakVoicePackScope.Xenotype || string.IsNullOrWhiteSpace(pack.targetDefName)) continue;
                if (!xenotypePacks.TryGetValue(pack.targetDefName, out List<SqueakVoicePackDef>? target)) { target = new List<SqueakVoicePackDef>(); xenotypePacks.Add(pack.targetDefName, target); }
                target.Add(pack);
            }
            Volatile.Write(ref current, new SqueakXenotypeCatalogSnapshot(discoveryAvailable, canonical, ambiguousCanonicalNames, harHints, officialHarHints, packsByKey, racePacks, xenotypePacks));
        }
        catch (Exception ex)
        {
            SqueakLog.CatalogRefreshFailed(ex);
            Volatile.Write(ref current, SqueakXenotypeCatalogSnapshot.Empty);
        }
    }

    private static void WarnDuplicatePackKey(string key, int count)
    {
        SqueakLog.PackRejected(key, count);
    }
}

public sealed class SqueakXenotypeCatalogSnapshot
{
    public static readonly SqueakXenotypeCatalogSnapshot Empty = new(false, new Dictionary<string, XenotypeDef>(StringComparer.Ordinal), new HashSet<string>(StringComparer.Ordinal), new HashSet<string>(StringComparer.Ordinal), new HashSet<string>(StringComparer.Ordinal), new Dictionary<string, SqueakVoicePackDef>(StringComparer.Ordinal), new List<SqueakVoicePackDef>(), new Dictionary<string, List<SqueakVoicePackDef>>(StringComparer.Ordinal));
    public readonly bool DiscoveryAvailable;
    public readonly IReadOnlyList<XenotypeDef> Xenotypes;
    public readonly IReadOnlyDictionary<string, XenotypeDef> XenotypeByDefName;
    /// <summary>Names with multiple loaded Def instances. Runtime must fail closed for these.</summary>
    public readonly IReadOnlyCollection<string> AmbiguousCanonicalDefNames;
    /// <summary>HAR discovery hints for UI ordering/filtering only; never a VoicePack eligibility gate.</summary>
    public readonly IReadOnlyCollection<string> HarHintDefNames;
    /// <summary>Official HAR-only hints suppressed from the UI unless another explicit source retains the target.</summary>
    public readonly IReadOnlyCollection<string> OfficialHarHintDefNames;
    public readonly IReadOnlyDictionary<string, SqueakVoicePackDef> PackByKey;
    public readonly IReadOnlyList<SqueakVoicePackDef> RacePacks;
    public readonly IReadOnlyDictionary<string, IReadOnlyList<SqueakVoicePackDef>> XenotypePacksByDefName;
    internal SqueakXenotypeCatalogSnapshot(bool discoveryAvailable, Dictionary<string, XenotypeDef> xenotypes, HashSet<string> ambiguousCanonicalNames, HashSet<string> harHints, HashSet<string> officialHarHints, Dictionary<string, SqueakVoicePackDef> packs, List<SqueakVoicePackDef> racePacks, Dictionary<string, List<SqueakVoicePackDef>> xenotypePacks)
    {
        DiscoveryAvailable = discoveryAvailable;
        Dictionary<string, XenotypeDef> canonical = new(xenotypes, StringComparer.Ordinal);
        XenotypeByDefName = new ReadOnlyDictionary<string, XenotypeDef>(canonical);
        AmbiguousCanonicalDefNames = new ReadOnlyCollection<string>(ambiguousCanonicalNames.OrderBy(x => x, StringComparer.Ordinal).ToList());
        HarHintDefNames = new ReadOnlyCollection<string>(harHints.OrderBy(x => x, StringComparer.Ordinal).ToList());
        OfficialHarHintDefNames = new ReadOnlyCollection<string>(officialHarHints.OrderBy(x => x, StringComparer.Ordinal).ToList());
        Xenotypes = new ReadOnlyCollection<XenotypeDef>(canonical.Values.OrderBy(x => x.defName, StringComparer.Ordinal).ToList());
        PackByKey = new ReadOnlyDictionary<string, SqueakVoicePackDef>(new Dictionary<string, SqueakVoicePackDef>(packs, StringComparer.Ordinal));
        racePacks.Sort((a, b) => StringComparer.Ordinal.Compare(a.defName, b.defName));
        RacePacks = new ReadOnlyCollection<SqueakVoicePackDef>(new List<SqueakVoicePackDef>(racePacks));
        Dictionary<string, IReadOnlyList<SqueakVoicePackDef>> copy = new(StringComparer.Ordinal);
        foreach (KeyValuePair<string, List<SqueakVoicePackDef>> entry in xenotypePacks)
        {
            entry.Value.Sort((a, b) => StringComparer.Ordinal.Compare(a.defName, b.defName));
            copy.Add(entry.Key, new ReadOnlyCollection<SqueakVoicePackDef>(new List<SqueakVoicePackDef>(entry.Value)));
        }
        XenotypePacksByDefName = new ReadOnlyDictionary<string, IReadOnlyList<SqueakVoicePackDef>>(copy);
    }

    /// <summary>UI-facing target union. DefName remains the sole identity; Canonical is presentation-only.</summary>
    public IReadOnlyList<SqueakXenotypeTargetCandidate> GetTargetCandidates(IEnumerable<VoicePackSelectionRecord> selections, IEnumerable<XenotypePresetRecord> presets)
    {
        Dictionary<string, HashSet<string>> sources = new(StringComparer.Ordinal);
        void AddSource(string name, string source)
        {
            if (string.IsNullOrEmpty(name)) return;
            if (!sources.TryGetValue(name, out HashSet<string>? values)) { values = new HashSet<string>(StringComparer.Ordinal); sources.Add(name, values); }
            values.Add(source);
        }
        foreach (string name in XenotypePacksByDefName.Keys) AddSource(name, "declared_pack");
        foreach (VoicePackSelectionRecord selection in selections ?? Array.Empty<VoicePackSelectionRecord>())
            if (selection != null && selection.scope == SqueakVoicePackScope.Xenotype) AddSource(selection.targetDefName, "selection");
        foreach (XenotypePresetRecord preset in presets ?? Array.Empty<XenotypePresetRecord>())
            if (preset != null) AddSource(preset.xenotypeDefName, "preset");
        foreach (string hint in HarHintDefNames) AddSource(hint, "har_hint");
        foreach (string officialHint in OfficialHarHintDefNames)
            if (sources.ContainsKey(officialHint)) AddSource(officialHint, "har_official_hint");
        if (SqueakLog.ShouldEmitDev)
        {
            foreach (KeyValuePair<string, HashSet<string>> entry in sources.OrderBy(x => x.Key, StringComparer.Ordinal))
                SqueakLog.XenotypeDiscoveryCandidate(entry.Key, string.Join("+", entry.Value.OrderBy(x => x, StringComparer.Ordinal)), true);
            foreach (string officialHint in OfficialHarHintDefNames.OrderBy(x => x, StringComparer.Ordinal))
                if (!sources.ContainsKey(officialHint)) SqueakLog.XenotypeDiscoveryCandidate(officialHint, "har_official_filtered", false);
        }
        return new ReadOnlyCollection<SqueakXenotypeTargetCandidate>(sources.Keys.OrderBy(x => x, StringComparer.Ordinal)
            .Select(name => new SqueakXenotypeTargetCandidate(name, XenotypeByDefName.TryGetValue(name, out XenotypeDef? canonical) ? canonical : null, HarHintDefNames.Contains(name), AmbiguousCanonicalDefNames.Contains(name), XenotypePacksByDefName.ContainsKey(name))).ToList());
    }

    /// <summary>Returns only packs declared for the exact selectable domain; never use PackByKey for domain eligibility.</summary>
    internal IReadOnlyList<SqueakVoicePackDef> GetVoicePackDomainPacks(SqueakVoicePackScope scope, string targetDefName)
    {
        if (scope == SqueakVoicePackScope.Race) return RacePacks;
        return scope == SqueakVoicePackScope.Xenotype && XenotypePacksByDefName.TryGetValue(targetDefName ?? "", out IReadOnlyList<SqueakVoicePackDef>? packs)
            ? packs
            : Array.Empty<SqueakVoicePackDef>();
    }
}

/// <summary>Candidate DTO for settings UI: use DefName for selection/search identity, Canonical only for LabelCap/Icon.</summary>
public readonly struct SqueakXenotypeTargetCandidate
{
    public readonly string DefName;
    public readonly XenotypeDef? Canonical;
    public readonly bool IsHarHint;
    public readonly bool HasCanonicalConflict;
    public readonly bool HasDeclaredPacks;
    internal SqueakXenotypeTargetCandidate(string defName, XenotypeDef? canonical, bool isHarHint, bool hasCanonicalConflict, bool hasDeclaredPacks)
    { DefName = defName; Canonical = canonical; IsHarHint = isHarHint; HasCanonicalConflict = hasCanonicalConflict; HasDeclaredPacks = hasDeclaredPacks; }
}
