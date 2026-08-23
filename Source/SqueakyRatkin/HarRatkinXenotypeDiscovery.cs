using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using RimWorld;
using Verse;

namespace SqueakyRatkin;

/// <summary>Optional HAR adapter. All AlienRace interaction stays reflective so HAR is never a build dependency.</summary>
public static class HarRatkinXenotypeDiscovery
{

    public static HarRatkinXenotypeDiscoveryResult Discover()
    {
        if (!ModsConfig.BiotechActive)
        {
            return HarRatkinXenotypeDiscoveryResult.Unavailable;
        }

        ThingDef? ratkin = DefDatabase<ThingDef>.GetNamedSilentFail(SqueakProductDomainFilter.PrimaryRaceDefName);
        if (ratkin == null)
        {
            return HarRatkinXenotypeDiscoveryResult.Unavailable;
        }

        try
        {
            object? alienRace = GetInstanceField(ratkin, "alienRace");
            object? restriction = alienRace == null ? null : GetInstanceField(alienRace, "raceRestriction");
            if (restriction == null)
            {
                WarnUnavailable("HAR Ratkin raceRestriction reflection is unavailable.");
                return HarRatkinXenotypeDiscoveryResult.Unavailable;
            }

            if (GetInstanceField(restriction, "onlyUseRaceRestrictedXenotypes") is not bool enabled || !enabled)
            {
                WarnUnavailable("HAR Ratkin raceRestriction does not require race-restricted xenotypes.");
                return HarRatkinXenotypeDiscoveryResult.Unavailable;
            }

            if (GetInstanceField(restriction, "whiteXenotypeList") is not IEnumerable whiteList)
            {
                WarnUnavailable("HAR Ratkin whiteXenotypeList reflection is unavailable.");
                return HarRatkinXenotypeDiscoveryResult.Unavailable;
            }

            MethodInfo? canUse = FindCanUseXenotypeMethod(restriction.GetType());
            if (canUse == null)
            {
                WarnUnavailable("HAR RaceRestrictionSettings.CanUseXenotype reflection is unavailable.");
                return HarRatkinXenotypeDiscoveryResult.Unavailable;
            }

            Dictionary<string, XenotypeDef> discovered = new(StringComparer.Ordinal);
            foreach (object item in whiteList)
            {
                if (item is not XenotypeDef xenotype)
                {
                    continue;
                }

                if (canUse.Invoke(null, new object[] { xenotype, ratkin }) is not bool accepted)
                {
                    WarnUnavailable("HAR CanUseXenotype returned an incompatible value.");
                    return HarRatkinXenotypeDiscoveryResult.Unavailable;
                }

                if (accepted && !string.IsNullOrEmpty(xenotype.defName))
                {
                    discovered[xenotype.defName] = xenotype;
                }
            }

            List<XenotypeDef> result = new(discovered.Values);
            result.Sort((left, right) => StringComparer.Ordinal.Compare(left.defName, right.defName));
            return new HarRatkinXenotypeDiscoveryResult(true, result);
        }
        catch (Exception ex)
        {
            if (SqueakLog.ShouldEmitDev) WarnUnavailable("HAR Ratkin xenotype reflection failed: " + ex.GetType().Name + ".");
            return HarRatkinXenotypeDiscoveryResult.Unavailable;
        }
    }

    private static object? GetInstanceField(object instance, string fieldName)
    {
        for (Type? type = instance.GetType(); type != null; type = type.BaseType)
        {
            FieldInfo? field = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
            if (field != null)
            {
                return field.GetValue(instance);
            }
        }

        return null;
    }

    private static MethodInfo? FindCanUseXenotypeMethod(Type restrictionType)
    {
        for (Type? type = restrictionType; type != null; type = type.BaseType)
        {
            MethodInfo? method = type.GetMethod(
                "CanUseXenotype",
                BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly,
                null,
                new[] { typeof(XenotypeDef), typeof(ThingDef) },
                null);
            if (method != null && method.ReturnType == typeof(bool))
            {
                return method;
            }
        }

        return null;
    }

    private static void WarnUnavailable(string reason)
    {
        if (SqueakLog.ShouldEmitDev) SqueakLog.XenotypeDiscoveryUnavailable(reason);
    }
}

public sealed class HarRatkinXenotypeDiscoveryResult
{
    public static readonly HarRatkinXenotypeDiscoveryResult Unavailable = new(false, new List<XenotypeDef>());
    public readonly bool available;
    public readonly IReadOnlyList<XenotypeDef> xenotypes;

    public HarRatkinXenotypeDiscoveryResult(bool available, IReadOnlyList<XenotypeDef> xenotypes)
    {
        this.available = available;
        this.xenotypes = xenotypes;
    }
}
