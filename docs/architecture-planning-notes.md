# Architecture Planning Notes

This document summarizes the current implementation constraints and proposed direction for expanding Squeaky Ratkin beyond its initial HAR Ratkin target. It is intended as a public technical planning note, not as a binding implementation specification.

## Current Implementation Baseline

The current implementation is data-driven in the areas that were required for the initial release:

- Action trigger parameters are declared in `1.6/Patches/Ratkin_AddSqueakComp.xml` under `actions`.
- Mood modulation is declared in the same XML patch under `moodMods`.
- Runtime pitch and volume modulation are applied through `SoundInfo.pitchFactor` and `SoundInfo.volumeFactor`.
- Sound resources are organized as one neutral runtime SoundDef per action, with mixed and pure custom-source variants.

The implementation still contains fixed assumptions in areas that were not part of the initial release scope:

- The effective target is HAR Ratkin. The XML patch attaches the component to `AlienRace.ThingDef_AlienRace[defName="Ratkin"]`.
- C# also contains `RatkinDefName = "Ratkin"` and `IsRatkin()`, so the eligibility boundary is duplicated in XML and code.
- Actions are represented by the `SqueakAction` enum. XML controls configured behavior for known actions, but it does not define a fully open action registry.
- New actions currently require coordinated changes in C#, SoundDefs, localization, and possibly Harmony patches.
- Trigger throttling is per action. There is no per-pawn global cooldown across all actions.
- Playback uses `SoundDef.PlayOneShot(SoundInfo.InMap(...))`. Repository code does not impose an explicit audio-duration limit, but the lower-level behavior of Verse.Sound and Unity should not be inferred without direct engine-source evidence.

## Target Scope Expansion

Future support may include Biotech xenotypes and xenotypes provided by gene or race extension mods. This changes the eligibility model from “a single HAR ThingDef” to “a configurable set of pawn identity conditions”.

Recommended direction:

1. Treat `CompSqueaker` presence as the primary runtime eligibility marker.
2. Keep identity selection in data or patch definitions whenever possible.
3. Avoid adding more hard-coded defName checks in Harmony patches.
4. If runtime identity filtering is required, model it as explicit allowlists such as pawn defNames, xenotype defNames, and gene defNames.
5. Preserve graceful failure when a target mod, xenotype, or gene definition is absent.

This keeps the Harmony layer compatible: external events such as selection, damage, and death can check whether a pawn has `CompSqueaker` instead of whether it matches one specific race name.

## Action Model Direction

The current action system is partially data-driven: intervals, probabilities, and trigger modes are XML-configured for a fixed action enum. To support “all actions listed in configuration and used as needed,” the model should be split into distinct layers.

Recommended conceptual layers:

1. **Action definition**: stable action identifier, label key, default SoundDef naming, category, and optional priority.
2. **Action enablement**: whether a specific target profile enables the action.
3. **Trigger source**: polling state, external Harmony event, job-state detection, gene/xenotype-specific condition, or future extension hook.
4. **Playback resource**: mixed SoundDef, pure custom SoundDef, preview SoundDef, clip folder path, and source policy.
5. **Display text**: localized Keyed entries independent of trigger logic.

The enum does not have to be removed immediately. A staged approach can preserve the current enum while adding fields such as `enabled`, `soundDef`, `pureSoundDef`, `previewSoundDef`, `labelKey`, `priority`, and `ignoreGlobalCooldown`. This reduces risk while moving the design toward a clearer registry model.

## Global Interval Control

Player feedback indicates that the perceived global interval may be too large. The current implementation exposes only per-action `minIntervalTicks` and uses `lastTriggerTick[action]` for per-action cooldown.

Recommended direction:

- Add a per-pawn `globalMinIntervalTicks` value in `CompProperties_Squeaker`.
- Keep per-action `minIntervalTicks` as the action-specific floor.
- Apply both checks before playback: an action may play only when its own cooldown and the global cooldown allow it.
- Allow selected high-priority actions to bypass or modify the global cooldown. `Death` is the clearest candidate; severe `Wounded` events may also need special handling.
- If a player setting is introduced, preserve the three-layer configuration model: XML default, ModSettings override, and runtime source selection.

The global interval should not replace per-action intervals. It should prevent cross-action overlap and excessive density while per-action intervals preserve semantic frequency.

## Longer Audio and Non-Squeak Voice Content

The repository implementation does not contain explicit duration truncation logic. The visible playback path calls `SoundDef.PlayOneShot` with in-map `SoundInfo`. Therefore, current constraints are frequency and overlap controls rather than hard-coded clip duration limits.

However, long audio or voiced content changes the design requirements:

- Short squeaks tolerate occasional overlap; voiced lines usually do not.
- Selection and damage events can fire rapidly and may require stricter cooldowns.
- One-shot playback is suitable for short non-looping clips but may be unsuitable for sustained ambient vocalization or long speech-like content.
- Long content may require priority rules, approximate playback-lock durations, or a “currently vocalizing” state per pawn.
- Audio directory structure should distinguish short squeaks from longer voice content if both are supported.

Recommended staged policy:

1. Keep the current one-shot model for short clips.
2. Document recommended clip length and format for the current release.
3. Add global cooldown and action priority before promoting long-clip support.
4. If sustained or long-form voice support becomes a primary feature, design it explicitly instead of relying on the unused `Sustained` enum value.

## Release Asset and Local Artifact Policy

`About/Preview.png` and `About/ModIcon.png` are release-facing metadata assets. They should be tracked when their copyright and source status are confirmed.

The temporary `debug/` directory is not part of the mod. It should not be tracked, and it should not enter distribution packages. The pack scripts already copy a restricted content set, which reduces this risk; adding an explicit ignore rule for `debug/` further clarifies the repository boundary.

## Recommended Implementation Order

1. Correct documentation drift and distribution metadata.
2. Track validated release images.
3. Remove local diagnostic artifacts and explicitly ignore `debug/`.
4. Add global per-pawn interval support with high-priority exception handling.
5. Replace hard-coded Ratkin eligibility checks with component-based eligibility.
6. Extend target attachment data for Biotech/xenotype support.
7. Split action definition, enablement, trigger source, audio resource, and localization metadata.
8. Define a separate policy for longer voice content only after cooldown and priority rules are in place.
