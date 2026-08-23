using System;
using System.Collections.Generic;
using System.Linq;
using SqueakyRatkin.Kernel;

namespace SqueakyRatkin;

/// <summary>
/// 0.3.x version-scoped product-domain filter. A hidden Settings list may replace the default roster;
/// this adapter is removed together with the filter in 0.4.x and is never referenced by Kernel code.
/// </summary>
public static class SqueakProductDomainFilter
{
    public const string PrimaryRaceDefName = "Ratkin";
    private static readonly IReadOnlyCollection<string> DefaultRaceDefNames = Array.AsReadOnly(new[] { PrimaryRaceDefName });

    public static IReadOnlyCollection<string> AllowedRaceDefNames => DefaultRaceDefNames;

    /// <summary>Catalog admission using the same hidden Settings roster as resolver pool assembly.</summary>
    public static bool Contains(string? raceDefName, SqueakyRatkinSettings? settings)
    {
        foreach (string allowed in AllowedRaceDefNamesFor(settings))
            if (string.Equals(allowed, raceDefName, StringComparison.Ordinal)) return true;
        return false;
    }

    /// <summary>Projects the selected roster into the Kernel without making Kernel depend on this product adapter.</summary>
    public static DomainFilter KernelFilterFor(SqueakyRatkinSettings? settings)
    {
        HashSet<RaceKey> allowed = new();
        foreach (string name in AllowedRaceDefNamesFor(settings)) allowed.Add(new RaceKey(name));
        return new DomainFilter(allowed);
    }

    private static IEnumerable<string> AllowedRaceDefNamesFor(SqueakyRatkinSettings? settings)
    {
        List<string>? configured = settings?.experimentalRaceAllowlist;
        if (configured == null || configured.Count == 0) return DefaultRaceDefNames;
        string[] replacement = configured.Where(name => !string.IsNullOrWhiteSpace(name)).Distinct(StringComparer.Ordinal).ToArray();
        return replacement.Length == 0 ? DefaultRaceDefNames : replacement;
    }
}
