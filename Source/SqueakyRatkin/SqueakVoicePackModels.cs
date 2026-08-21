using System;
using System.Collections.Generic;
using Verse;
using Verse.Sound;

namespace SqueakyRatkin;

/// <summary>One Def is one selectable pack in one domain. It never carries behavior or mood data.</summary>
public class SqueakVoicePackDef : Def
{
    public SqueakVoicePackScope scope = SqueakVoicePackScope.Unspecified;
    /// <summary>0.3.1 race-aware ABI：pack 唯一路由声明，必填（validator 硬要求），无缺省默认。
    /// 值 = 服务的 race 的精确、区分大小写 ThingDef.defName（如 Ratkin）；装配由 DomainFilter 白名单闸决定。</summary>
    public string raceDefName = "";
    public string targetDefName = "";
    public List<SqueakVoicePackAction> actions = new();

    public bool TryGetPackKey(out string key)
    {
        key = "";
        string? packageId = modContentPack?.ModMetaData?.PackageIdNonUnique;
        if (string.IsNullOrWhiteSpace(packageId) || string.IsNullOrWhiteSpace(defName)) return false;
        key = packageId + ":" + defName;
        return true;
    }

    public override IEnumerable<string> ConfigErrors()
    {
        foreach (string error in base.ConfigErrors()) yield return error;
        foreach (string error in SqueakVoicePackValidator.GetErrors(this)) yield return error;
    }
}

public class SqueakVoicePackAction
{
    public SqueakAction action = SqueakAction.Call;
    public List<SoundDef> sounds = new();
}

/// <summary>Pure production-audio contract shared by Def validation and candidate admission.</summary>
internal static class SqueakVoicePackValidator
{
    internal static bool IsValid(SqueakVoicePackDef? pack)
    {
        foreach (string _ in GetErrors(pack)) return false;
        return true;
    }

    internal static IEnumerable<string> GetErrors(SqueakVoicePackDef? pack)
    {
        if (pack == null) { yield return "SqueakVoicePackDef is null."; yield break; }
        string name = string.IsNullOrWhiteSpace(pack.defName) ? "SqueakVoicePackDef" : pack.defName;
        if (string.IsNullOrWhiteSpace(pack.defName) || !pack.defName.StartsWith("SR_")) yield return name + " defName must begin with SR_.";
        if (string.IsNullOrWhiteSpace(pack.raceDefName)) yield return name + " is missing raceDefName; every VoicePack must declare the exact race defName it serves (e.g. Ratkin).";
        if (pack.scope == SqueakVoicePackScope.Unspecified) yield return name + " has an unspecified scope.";
        if (pack.scope == SqueakVoicePackScope.Race && !string.IsNullOrEmpty(pack.targetDefName)) yield return name + " Race scope must not specify targetDefName.";
        if (pack.scope == SqueakVoicePackScope.Xenotype && string.IsNullOrWhiteSpace(pack.targetDefName)) yield return name + " Xenotype scope requires targetDefName.";
        if (pack.actions == null || pack.actions.Count == 0) { yield return name + " has no action sounds."; yield break; }

        HashSet<SqueakAction> seen = new();
        foreach (SqueakVoicePackAction entry in pack.actions)
        {
            if (entry == null) { yield return name + " contains a null action entry."; continue; }
            if (!SqueakActionDefinitions.IsKnown(entry.action)) yield return name + " contains unknown action " + entry.action + ".";
            if (!seen.Add(entry.action)) yield return name + " contains duplicate action " + entry.action + ".";
            if (entry.sounds == null || entry.sounds.Count == 0) { yield return name + " action " + entry.action + " has no sounds."; continue; }
            foreach (SoundDef sound in entry.sounds)
            {
                if (sound == null) { yield return name + " action " + entry.action + " contains a null SoundDef."; continue; }
                if (string.IsNullOrWhiteSpace(sound.defName) || !sound.defName.StartsWith("SR_")) yield return name + " action " + entry.action + " references a SoundDef without SR_ prefix.";
                if (sound.sustain) yield return name + " action " + entry.action + " references sustained SoundDef " + sound.defName + "; production voice sounds must be one-shot.";
                if (sound.context != SoundContext.MapOnly) yield return name + " action " + entry.action + " references SoundDef " + sound.defName + " with context other than MapOnly.";
                if (sound.subSounds == null || sound.subSounds.Count == 0) { yield return name + " action " + entry.action + " SoundDef " + sound.defName + " has no SubSounds."; continue; }
                foreach (SubSoundDef subSound in sound.subSounds)
                {
                    if (subSound == null) { yield return name + " action " + entry.action + " SoundDef " + sound.defName + " contains a null SubSound."; continue; }
                    if (subSound.onCamera) yield return name + " action " + entry.action + " SoundDef " + sound.defName + " has an onCamera SubSound; production voice SubSounds must be in-map only.";
                    if (subSound.grains == null || subSound.grains.Count == 0) yield return name + " action " + entry.action + " SoundDef " + sound.defName + " has a SubSound without grains.";
                    else foreach (var grain in subSound.grains)
                        if (grain == null) yield return name + " action " + entry.action + " SoundDef " + sound.defName + " has a SubSound containing a null grain.";
                }
            }
        }
    }
}

/// <summary>Canonical, last-wins persisted selection for one exact Race or Xenotype domain.
/// raceDefName is required after v4 migration; Xenotype scope additionally requires xenotypeDefName.
/// targetDefName is a v1 load-only Scribe source and is never serialized or used as record runtime state.</summary>
public class VoicePackSelectionRecord : IExposable
{
    public SqueakVoicePackScope scope = SqueakVoicePackScope.Unspecified;
    public string raceDefName = "";
    public string xenotypeDefName = "";
    [System.NonSerialized] internal string legacyTargetDefName = "";
    public List<string> enabledPackKeys = new();

    public void ExposeData()
    {
        Scribe_Values.Look(ref scope, "scope", SqueakVoicePackScope.Unspecified);
        Scribe_Values.Look(ref raceDefName, "raceDefName", "");
        Scribe_Values.Look(ref xenotypeDefName, "xenotypeDefName", "");
        if (Scribe.mode == LoadSaveMode.LoadingVars)
            Scribe_Values.Look(ref legacyTargetDefName, "targetDefName", "");
        Scribe_Collections.Look(ref enabledPackKeys, "enabledPackKeys", LookMode.Value);
        if (Scribe.mode == LoadSaveMode.PostLoadInit && enabledPackKeys == null) enabledPackKeys = new List<string>();
    }

    // Retained through 2b-1 so existing public settings APIs and string-domain consumers remain source-stable.
    // 2b-2 removes this legacy string key in favor of AudioDomain.
    public static string ComposeDomainKey(SqueakVoicePackScope scope, string targetDefName) => scope == SqueakVoicePackScope.Race ? "Race" : scope == SqueakVoicePackScope.Xenotype ? "Xenotype:" + (targetDefName ?? "") : "";
    public string DomainKey => ComposeDomainKey(scope, xenotypeDefName);
}

public enum SqueakVoicePackDomainState { Available, Dormant, TargetUnavailable, Orphan }

public readonly struct SqueakVoicePackDomainStatus
{
    public readonly SqueakVoicePackDomainState State;
    public readonly IReadOnlyList<string> EnabledKeys;
    public SqueakVoicePackDomainStatus(SqueakVoicePackDomainState state, IReadOnlyList<string> enabledKeys) { State = state; EnabledKeys = enabledKeys; }
}
