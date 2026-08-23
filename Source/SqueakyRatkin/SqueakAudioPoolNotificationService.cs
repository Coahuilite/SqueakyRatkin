using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace SqueakyRatkin;

/// <summary>One-process notification API for selectable VoicePack domains. UI can render returned state without legacy pool schema.</summary>
public static class SqueakAudioPoolNotificationService
{
    private static bool shownThisProcess;

    public static IReadOnlyList<SqueakVoicePackDomainStatus> GetDomainStatuses(SqueakyRatkinSettings settings, SqueakXenotypeCatalogSnapshot catalog)
    {
        List<SqueakVoicePackDomainStatus> result = new() { settings.GetVoicePackSelectionStatus(SqueakVoicePackScope.Race, "") };
        if (ModsConfig.BiotechActive)
            foreach (string target in catalog.XenotypePacksByDefName.Keys.OrderBy(x => x, StringComparer.Ordinal)) result.Add(settings.GetVoicePackSelectionStatus(SqueakVoicePackScope.Xenotype, target));
        return result;
    }

    public static void EvaluateAndMaybeShow(SqueakyRatkinSettings settings, SqueakXenotypeCatalogSnapshot catalog)
    {
        if (shownThisProcess || Find.WindowStack == null) return;
        List<string> missing = new();
        if (catalog.RacePacks.Count > 0 && !HasEnabledKnownPack(settings, catalog, SqueakVoicePackScope.Race, "")) missing.Add("Race");
        if (ModsConfig.BiotechActive)
            foreach (string target in catalog.XenotypePacksByDefName.Keys.OrderBy(x => x, StringComparer.Ordinal))
                if (!HasEnabledKnownPack(settings, catalog, SqueakVoicePackScope.Xenotype, target))
                    missing.Add(catalog.XenotypeByDefName.TryGetValue(target, out XenotypeDef? xenotype) ? xenotype.LabelCap + " (" + target + ")" : target + " (target unavailable)");
        if (missing.Count == 0) return;
        // Retain the existing localized dialog transport while exposing a unified backend state to the future UI.
        Find.WindowStack.Add(new Dialog_SqueakyCompactMessageBox("SR.AudioPoolNotice.Body".Translate() + "\n• " + string.Join("\n• ", missing), "SR.AudioPoolNotice.OpenSettings".Translate(), () => SqueakyRatkinMod.Instance?.OpenSettings(true), "SR.AudioPoolNotice.Later".Translate(), () => { }, "SR.AudioPoolNotice.Title".Translate()));
        shownThisProcess = true;
    }

    private static bool HasEnabledKnownPack(SqueakyRatkinSettings settings, SqueakXenotypeCatalogSnapshot catalog, SqueakVoicePackScope scope, string target)
    {
        VoicePackSelectionRecord? record = settings.voicePackSelections.LastOrDefault(x => VoicePackSelectionRecord.SameDomain(x, scope, SqueakProductDomainFilter.PrimaryRaceDefName, target));
        if (record == null) return false;
        HashSet<string> enabledKeys = new(record.enabledPackKeys ?? new List<string>(), StringComparer.Ordinal);
        return catalog.GetVoicePackDomainPacks(scope, target).Any(pack => pack.TryGetPackKey(out string key) && enabledKeys.Contains(key));
    }
}
