# SqueakyRatkin Engineering Review — 2026-07-03

This review was prepared as an independent engineering assessment by a separate review agent. The reference framework was HunYuan2333/rimworld-mod-engineering-skills, used as advisory guidance rather than as a mandatory standard. The review focuses on project structure, Def/XML configuration, Harmony and Comp design, compatibility, release boundaries, diagnostics, data-driven design, maintainability, and public documentation quality.

## Overall Conclusion

SqueakyRatkin is structurally sound for its current purpose as a Ratkin-focused sound quality-of-life mod. The C# layer provides runtime adaptation, XML and Def files carry action and audio configuration, Harmony patches are limited in scope, and distribution scripts define a clear package boundary.

The principal engineering risk concerns future expansion. Current eligibility is defined as HAR Ratkin through both XML and C# checks. This is adequate for the current release target but should be decoupled before adding Biotech xenotype, gene, or third-party xenotype support.

A second notable risk is that public documentation and implementation details have begun to diverge. This should be corrected before the next public-facing release.

## Findings

### 1. HAR Ratkin to Biotech or Xenotype Expansion

- **Level**: Warning
- **Basis**: `1.6/Patches/Ratkin_AddSqueakComp.xml` attaches the component to `AlienRace.ThingDef_AlienRace[defName="Ratkin"]`. `Source/SqueakyRatkin/CompSqueaker.cs` contains `RatkinDefName = "Ratkin"` and `IsRatkin()`.
- **Recommendation**: Use component presence as the runtime eligibility marker. Keep target selection in XML or configurable allowlists. Avoid adding more hard-coded pawn defName checks.
- **Release impact**: Does not block the current Ratkin-specific release. It is important before xenotype expansion.

### 2. Action System Data-Driven Scope

- **Level**: Warning
- **Basis**: `SqueakAction` is an enum, while XML configures behavior only for known actions. New actions still require changes to C#, SoundDefs, and localization.
- **Recommendation**: Separate action definition, enablement, trigger source, audio resources, and display text. A staged approach may keep the enum while adding explicit metadata fields.
- **Release impact**: Does not block the current release. It affects the cost of future action expansion.

### 3. Global Sound Interval

- **Level**: Recommendation
- **Basis**: Current throttling is per action through `lastTriggerTick[action]` and per-action `minIntervalTicks`. There is no cross-action cooldown.
- **Recommendation**: Add `globalMinIntervalTicks` at the component-property level, with optional per-action bypass or priority for events such as `Death`.
- **Release impact**: Not mandatory for short current clips. Recommended before supporting longer voice content.

### 4. Long Audio and Voice-Like Content

- **Level**: Warning
- **Basis**: Runtime playback uses `SoundDef.PlayOneShot(SoundInfo.InMap(...))`. Repository code contains no explicit clip-duration truncation logic. Current audio guidance recommends short clips.
- **Recommendation**: Treat one-shot playback as the short-clip model. For longer voice content, introduce overlap prevention, action priority, global cooldown, and possibly approximate playback-lock durations.
- **Release impact**: Does not block current short-sound releases. It should be addressed before advertising support for longer voiced lines.

### 5. Harmony Patch Scope

- **Level**: Pass
- **Basis**: Current patches target specific events: damage, selection, and death. They notify the component rather than replacing broad gameplay behavior.
- **Recommendation**: Preserve this minimal patch surface. Improve future compatibility by replacing race-name checks with component checks.
- **Release impact**: No negative impact.

### 6. Project Structure and Distribution Boundary

- **Level**: Pass
- **Basis**: Source, Defs, Patches, Languages, Sounds, and scripts are separated. Pack scripts copy a restricted set of release directories.
- **Recommendation**: Keep the restricted pack boundary. Confirm that future generated or diagnostic artifacts remain outside release packages.
- **Release impact**: No negative impact.

### 7. Temporary Debug Directory

- **Level**: Recommendation
- **Basis**: `debug/` is a local diagnostic artifact and is currently ignored through log-file rules, not through an explicit directory rule.
- **Recommendation**: Remove the local directory when no longer needed and explicitly ignore `debug/`.
- **Release impact**: No runtime impact. Improves repository hygiene.

### 8. About Images

- **Level**: Recommendation
- **Basis**: `About/Preview.png` and `About/ModIcon.png` are release-facing RimWorld metadata assets and were untracked at the time of inspection.
- **Recommendation**: Track them after confirming provenance and license status. They belong with `About/About.xml` in the release package.
- **Release impact**: Recommended before public distribution for complete presentation.

### 9. Public Documentation Consistency

- **Level**: Warning
- **Basis**: Public text still contains references that may no longer match current implementation, including older camera zoom behavior and earlier debug sound-browser behavior. The audio guide also needs to remain synchronized with the current action list.
- **Recommendation**: Update public-facing documentation before release: camera behavior, debug tools, action count, and custom-audio conventions.
- **Release impact**: Recommended before the next public-facing release.

### 10. Diagnostics and Logging

- **Level**: Pass
- **Basis**: Startup logging and DevMode-only squeak diagnostics provide useful information without making diagnostics part of normal gameplay behavior.
- **Recommendation**: Keep DevMode logging restrained. Add throttling if diagnostics become too noisy during large-colony testing.
- **Release impact**: No negative impact.

## Priority Recommendations

1. Correct public documentation drift before the next release.
2. Track validated `About/Preview.png` and `About/ModIcon.png`.
3. Remove the local `debug/` directory and explicitly ignore future debug artifacts.
4. Add global per-pawn sound cooldown with priority or bypass semantics for critical events.
5. Replace hard-coded Ratkin eligibility checks with component-based eligibility.
6. Prepare data-level target definitions for Biotech xenotypes, genes, and third-party xenotype providers.
7. Separate action definition, enablement, trigger source, audio resources, and localization metadata.
8. Define long-audio behavior explicitly before supporting voice-like clips as a public feature.
