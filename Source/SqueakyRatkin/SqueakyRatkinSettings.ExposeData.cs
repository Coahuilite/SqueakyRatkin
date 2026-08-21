using System;
using System.Collections.Generic;
using Verse;

namespace SqueakyRatkin;

/// <summary>
/// Settings' Scribe boundary. Migration never writes Config directly: it builds replacement records first,
/// publishes them atomically only on success, and lets the startup main-thread bridge queue base.WriteSettings().
/// </summary>
public partial class SqueakyRatkinSettings
{
    private const int CurrentSettingsSchemaVersion = 4;
    private const int CurrentVoicePackSchemaVersion = 2;
    private const int LegacyVoicePackSchemaVersion = 1;
    private const string LegacyDefaultRaceDefName = "Ratkin";

    // Hidden 0.3.x-only switch: an empty/null value keeps the default {Ratkin}; a nonempty list replaces it.
    // It is deliberately never rendered by Settings UI and is deleted together with ProductDomainFilter in 0.4.x.
    public List<string> experimentalRaceAllowlist = new();

    private bool distanceRangeWasLoaded;
    private bool scaleFrequencyWithTalkingWasLoaded;
    private bool settingsSchemaWasLoaded;
    private bool voicePackModeWasLoaded;
    private bool migrationPersistencePending;
    private bool migrationPersistenceBlocked;
    internal bool MigrationPersistencePendingForFixture => migrationPersistencePending;
    internal bool MigrationPersistenceBlockedForFixture => migrationPersistenceBlocked;
    internal bool IsPersistenceBlockedByMigrationFailure => migrationPersistenceBlocked;

    public override void ExposeData()
    {
        base.ExposeData();
        // Presence flags are reset once per LoadingVars pass and survive until PostLoadInit.
        if (Scribe.mode == LoadSaveMode.LoadingVars)
        {
            settingsSchemaWasLoaded = false;
            distanceRangeWasLoaded = false;
            scaleFrequencyWithTalkingWasLoaded = false;
            voicePackModeWasLoaded = false;
        }

        Scribe_Values.Look(ref voicePackMode, "voicePackMode", SqueakVoicePackMode.Fallback);
        // v2 is intentionally non-default at the Scribe boundary so the completed 1→2 migration is durable.
        Scribe_Values.Look(ref voicePackSchemaVersion, "voicePackSchemaVersion", LegacyVoicePackSchemaVersion);
        Scribe_Values.Look(ref voicePackDefaultSeeded, "voicePackDefaultSeeded", false);
        Scribe_Values.Look(ref settingsSchemaVersion, "settingsSchemaVersion", CurrentSettingsSchemaVersion, forceSave: true);
        Scribe_Values.Look(ref scaleCooldownWithTimeSpeed, "scaleCooldownWithTimeSpeed", true);
        Scribe_Values.Look(ref scaleFrequencyWithTalking, "scaleFrequencyWithTalking", GetDefaultScaleFrequencyWithTalking());
        Scribe_Values.Look(ref scalePeriodicWithAudiblePopulation, "scalePeriodicWithAudiblePopulation", true);
        Scribe_Values.Look(ref localizeDebugActions, "localizeDebugActions", false);
        Scribe_Values.Look(ref developerToolsEnabled, "developerToolsEnabled", false);
        Scribe_Values.Look(ref devLoggingMode, "devLoggingMode", SqueakDevLoggingMode.Auto);
        Scribe_Values.Look(ref globalCooldownMultiplier, "globalCooldownMultiplier", 1f);
        Scribe_Values.Look(ref distancePreset, "distancePreset", SqueakDistancePreset.Balanced);
        Scribe_Values.Look(ref distanceRange, "distanceRange", GetDistancePresetRange(SqueakDistancePreset.Balanced));
        // Hidden experimental roster; nonempty data replaces the default roster rather than extending it.
        Scribe_Collections.Look(ref experimentalRaceAllowlist, "experimentalRaceAllowlist", LookMode.Value);

        if (Scribe.mode == LoadSaveMode.LoadingVars)
        {
            scaleFrequencyWithTalkingWasLoaded = Scribe.loader?.curXmlParent?["scaleFrequencyWithTalking"] != null;
            distanceRangeWasLoaded = Scribe.loader?.curXmlParent?["distanceRange"] != null;
            settingsSchemaWasLoaded = Scribe.loader?.curXmlParent?["settingsSchemaVersion"] != null;
            // An absent mode means the player never explicitly selected the voice source policy.
            voicePackModeWasLoaded = Scribe.loader?.curXmlParent?["voicePackMode"] != null;
        }

        Scribe_Collections.Look(ref moodOverrides, "moodOverrides", LookMode.Value, LookMode.Deep);
        Scribe_Collections.Look(ref voicePackSelections, "voicePackSelections", LookMode.Deep);
        Scribe_Collections.Look(ref xenotypePresets, "xenotypePresets", LookMode.Deep);
        Scribe_Collections.Look(ref globalActionEnabled, "globalActionEnabled", LookMode.Deep);
        if (Scribe.mode == LoadSaveMode.LoadingVars && moodOverrides == null)
            moodOverrides = new Dictionary<SqueakMood, SqueakMoodMod>();

        if (Scribe.mode != LoadSaveMode.PostLoadInit) return;

        if (!Enum.IsDefined(typeof(SqueakVoicePackMode), voicePackMode)) voicePackMode = SqueakVoicePackMode.Off;
        if (!Enum.IsDefined(typeof(SqueakDevLoggingMode), devLoggingMode)) devLoggingMode = SqueakDevLoggingMode.Auto;
        SqueakLog.Configure(devLoggingMode);

        // Retain the older v2→v3 numeric repair without treating it as a record-schema commit.
        if (!settingsSchemaWasLoaded || settingsSchemaVersion < 3)
        {
            if (Math.Abs(globalCooldownMultiplier - 1.2f) <= .0001f) globalCooldownMultiplier = 1f;
        }
        globalCooldownMultiplier = Math.Max(0f, Math.Min(globalCooldownMultiplier, 3f));
        if (!scaleFrequencyWithTalkingWasLoaded) scaleFrequencyWithTalking = GetDefaultScaleFrequencyWithTalking();
        if (!distanceRangeWasLoaded) distanceRange = GetDistancePresetRange(SqueakDistancePreset.Balanced);
        distanceRange = ClampDistanceRange(distanceRange);

        if (experimentalRaceAllowlist == null) experimentalRaceAllowlist = new List<string>();
        if (globalActionEnabled == null) globalActionEnabled = new List<GlobalActionEnabledRecord>();
        foreach (GlobalActionEnabledRecord record in globalActionEnabled)
        {
            if (record == null || !SqueakActionDefinitions.IsKnown(record.action)) continue;
            record.scope = record.scopeWasLoaded
                ? SqueakActionDefinitions.NormalizeScope(record.action, record.scope)
                : record.enabled ? SqueakActionDefinitions.Get(record.action).DefaultScope : SqueakActionScope.Disabled;
            record.enabled = record.scope != SqueakActionScope.Disabled;
        }

        bool migrationNeeded = settingsSchemaVersion < CurrentSettingsSchemaVersion || voicePackSchemaVersion < CurrentVoicePackSchemaVersion;
        if (migrationNeeded) MigrateV3RecordsTransactionally();
        else
        {
            if (voicePackSelections == null) voicePackSelections = new List<VoicePackSelectionRecord>();
            if (xenotypePresets == null) xenotypePresets = new List<XenotypePresetRecord>();
        }
    }

    /// <summary>
    /// v3/v1 → v4/v2. No source list or schema marker changes until all cloned records normalize and validate.
    /// A failure deliberately leaves the old in-memory lists/schema available for an idempotent retry next startup.
    /// Internal fixture harnesses call this after Scribe PostLoadInit to prove the same production transaction.
    /// </summary>
    internal bool MigrateV3RecordsTransactionally()
    {
        if (settingsSchemaVersion >= CurrentSettingsSchemaVersion && voicePackSchemaVersion >= CurrentVoicePackSchemaVersion) return true;

        if (!SqueakSettingsMigration.TryCreateV4Records(
                voicePackSelections,
                xenotypePresets,
                LegacyDefaultRaceDefName,
                out List<VoicePackSelectionRecord> migratedSelections,
                out List<XenotypePresetRecord> migratedPresets,
                out string failure))
        {
            // Preserve the on-disk legacy source unchanged: no later UI/framework Config write may erase the
            // load-only targetDefName before the next startup gets to retry this same transaction.
            migrationPersistenceBlocked = true;
            SqueakLog.TargetRejected("settings_schema_migration", "migration_failed:" + failure);
            return false;
        }

        voicePackSelections = migratedSelections;
        xenotypePresets = migratedPresets;
        migrationPersistenceBlocked = false;
        if (settingsSchemaVersion < CurrentSettingsSchemaVersion) settingsSchemaVersion = CurrentSettingsSchemaVersion;
        if (voicePackSchemaVersion < CurrentVoicePackSchemaVersion) voicePackSchemaVersion = CurrentVoicePackSchemaVersion;
        // This is consumed only by the main-thread startup callback, which queues the existing base.WriteSettings path.
        migrationPersistencePending = true;
        return true;
    }
}
