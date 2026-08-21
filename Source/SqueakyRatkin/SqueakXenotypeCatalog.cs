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
                // 0.3.1 域闸（主闸）：非装配域 raceDefName 的包拒绝加载，dev 可见日志 reason=domain_filtered。
                // 白名单在 SqueakProductDomainFilter 集中一处；此处不写任何 race 特判。
                if (!SqueakProductDomainFilter.Contains(pack.raceDefName))
                {
                    if (pack.TryGetPackKey(out string filteredKey)) SqueakLog.PackRejected(filteredKey, 1, "domain_filtered");
                    continue;
                }
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
            // Pack eligibility is strictly the declared, case-sensitive raceDefName/target strings;
            // HAR discovery is dev diagnostics only (0.3.1 assembled-only).
            Dictionary<string, List<SqueakVoicePackDef>> xenotypePacks = new(StringComparer.Ordinal);
            foreach (SqueakVoicePackDef pack in packsByKey.Values)
            {
                if (pack.scope != SqueakVoicePackScope.Xenotype || string.IsNullOrWhiteSpace(pack.targetDefName)) continue;
                if (!xenotypePacks.TryGetValue(pack.targetDefName, out List<SqueakVoicePackDef>? target)) { target = new List<SqueakVoicePackDef>(); xenotypePacks.Add(pack.targetDefName, target); }
                target.Add(pack);
            }

            Dictionary<string, XenotypeDef> canonical = new(StringComparer.Ordinal);
            HashSet<string> ambiguousCanonicalNames = new(StringComparer.Ordinal);
            HashSet<string> harHints = new(StringComparer.Ordinal);
            HashSet<string> officialHarHints = new(StringComparer.Ordinal);
            bool discoveryAvailable = false;
            if (ModsConfig.BiotechActive)
            {
                try
                {
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
                    // 0.3.1 catalog = 装配域，不是发现域：canonical 只登记装配域异种 =
                    // HAR Ratkin 限定集（0.2.x 现状机制）∪ 声明了 pack 的目标；非装配域
                    // XenotypeDef（他族/原版人类等）不注册，杜绝 canonical 泄漏。
                    HashSet<string> assembledXenotypes = new(StringComparer.Ordinal);
                    foreach (string name in harHints) assembledXenotypes.Add(name);
                    foreach (string name in officialHarHints) assembledXenotypes.Add(name);
                    foreach (string name in xenotypePacks.Keys) assembledXenotypes.Add(name);
                    foreach (XenotypeDef xenotype in DefDatabase<XenotypeDef>.AllDefs)
                    {
                        if (string.IsNullOrEmpty(xenotype.defName) || !assembledXenotypes.Contains(xenotype.defName)) continue;
                        if (canonical.ContainsKey(xenotype.defName))
                        {
                            canonical.Remove(xenotype.defName);
                            ambiguousCanonicalNames.Add(xenotype.defName);
                        }
                        else if (!ambiguousCanonicalNames.Contains(xenotype.defName)) canonical.Add(xenotype.defName, xenotype);
                    }
                }
                catch (Exception ex) { if (SqueakLog.ShouldEmitDev) SqueakLog.XenotypeDiscoveryFailed(ex); discoveryAvailable = false; }
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
    /// <summary>HAR discovery hints for dev diagnostics only (0.3.1 assembled-only projection);
    /// never projected as candidate rows, never a VoicePack eligibility gate.</summary>
    public readonly IReadOnlyCollection<string> HarHintDefNames;
    /// <summary>Official HAR-only hints; dev diagnostics only, never projected as candidate rows.</summary>
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

    /// <summary>UI-facing assembled-only target union (0.3.1 decision B). Projects only assembled
    /// content (declared packs) plus explicit references (selections/presets keep orphan/dormant rows);
    /// HAR discovery is dev diagnostics only and never projects rows (canonical/har-hint leak fix).</summary>
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
        if (SqueakLog.ShouldEmitDev)
        {
            foreach (KeyValuePair<string, HashSet<string>> entry in sources.OrderBy(x => x.Key, StringComparer.Ordinal))
                SqueakLog.XenotypeDiscoveryCandidate(entry.Key, string.Join("+", entry.Value.OrderBy(x => x, StringComparer.Ordinal)), true);
            // 0.3.1：HAR 发现不再投影为 UI 行；仅 dev 诊断记录被过滤的候选（enabled=false）。
            foreach (string hint in HarHintDefNames.OrderBy(x => x, StringComparer.Ordinal))
                if (!sources.ContainsKey(hint)) SqueakLog.XenotypeDiscoveryCandidate(hint, "har_hint_filtered", false);
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
