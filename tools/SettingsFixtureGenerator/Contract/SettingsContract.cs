using System;
using System.Collections.Generic;
using Verse;

namespace SqueakyRatkin;

/// <summary>
/// Fixture-only host for the production <c>SqueakyRatkinSettings.ExposeData</c> partial.
/// Persisted fields and tiny pure defaults live here because the full UI/runtime partial deliberately is not
/// linked; Scribe behavior and the v3/v1 → v4/v2 transaction are linked from production source exactly once.
/// Frozen input XML remains the 0.2.4 source artifact; this host models the post-migration persisted surface.
/// </summary>
public partial class SqueakyRatkinSettings : ModSettings
{
    private static readonly FloatRange FallbackBalancedDistanceRange = new(15f, 50f);

    public SqueakVoicePackMode voicePackMode = SqueakVoicePackMode.Fallback;
    public int voicePackSchemaVersion = CurrentVoicePackSchemaVersion;
    public bool voicePackDefaultSeeded;
    public int settingsSchemaVersion = CurrentSettingsSchemaVersion;
    public bool scaleCooldownWithTimeSpeed = true;
    public bool scaleFrequencyWithTalking = true;
    public bool scalePeriodicWithAudiblePopulation = true;
    public bool localizeDebugActions;
    public bool developerToolsEnabled;
    public SqueakDevLoggingMode devLoggingMode = SqueakDevLoggingMode.Auto;
    public float globalCooldownMultiplier = 1f;
    public SqueakDistancePreset distancePreset = SqueakDistancePreset.Balanced;
    public FloatRange distanceRange = new(15f, 50f);

    public Dictionary<SqueakMood, SqueakMoodMod> moodOverrides = new();
    public List<VoicePackSelectionRecord> voicePackSelections = new();
    public List<XenotypePresetRecord> xenotypePresets = new();
    public List<GlobalActionEnabledRecord> globalActionEnabled = new();

    public SqueakActionScope GetActionGlobalScope(SqueakAction action)
    {
        SqueakActionScope scope = SqueakActionDefinitions.Get(action).DefaultScope;
        foreach (GlobalActionEnabledRecord record in globalActionEnabled ?? new List<GlobalActionEnabledRecord>())
            if (record != null && record.action == action) scope = SqueakActionDefinitions.NormalizeScope(action, record.scope);
        return scope;
    }

    public bool IsActionGloballyEnabled(SqueakAction action) => GetActionGlobalScope(action) != SqueakActionScope.Disabled;

    private static bool GetDefaultScaleFrequencyWithTalking() => true;

    private static FloatRange GetDistancePresetRange(SqueakDistancePreset preset) => preset switch
    {
        SqueakDistancePreset.Conservative => new FloatRange(15f, 65f),
        SqueakDistancePreset.Strong => new FloatRange(15f, 40f),
        _ => FallbackBalancedDistanceRange,
    };

    public static FloatRange ClampDistanceRange(FloatRange range)
    {
        float min = Math.Max(15f, Math.Min(range.min, 60f));
        float max = Math.Max(20f, Math.Min(range.max, 65f));
        return new FloatRange(min, max);
    }
}
