# Changelog

## Changelog Template

Use this file as the canonical English changelog for Squeaky Ratkin.

Rules:
- Keep released entries in chronological order, oldest first and newest last; place an Unreleased entry at the top.
- Use local release time in the heading: `[YYYY-MM-DD HH:MM UTC+8] Version X.Y.Z`.
- An unreleased top entry may use `Unreleased — X.Y.Z`; replace it with the actual UTC+8 release time when publishing.
- Use `Initial Workshop Upload` only for the first Steam Workshop upload; subsequent updates use ordinary version entries.
- Keep items short and visible. Prefer one change per bullet.
- Separate feature additions, changes, fixes, packaging notes, and release notes when useful.
- Bug fixes should be concise unless the fix changes player-facing behavior.
- Mention both GitHub and Steam only when the entry affects both release surfaces.
- Do not put unaccepted plans here. An accepted, not-yet-released version may use the explicit Unreleased entry above.
- Keep the Simplified Chinese version synchronized in `docs/CHANGELOG.zh-CN.md`.

Recommended entry shape:

```text
## [YYYY-MM-DD HH:MM UTC+8] Version X.Y.Z

Short release summary.

### Added
- ...

### Changed
- ...

### Fixed
- ...

### Packaging
- ...
```

## Unreleased — 0.2.1

Bug-fix release: NewRatkinPlus fork compatibility, seven-click counter reset, and the reworked draggable diagnostics panel.

### Fixed
- Removed the `LoadFolders.xml` `IfModActive="Solaris.RatkinRaceMod"` package gate that silently disabled the mod with NewRatkinPlus forks using a different packageId; content now always loads and the squeak comp is injected by the `defName="Ratkin"` XPath target.
- The seven-click developer unlock counter now resets when the settings window reopens; an incomplete count no longer survives closing settings.

### Changed
- Diagnostics overlay rework: pawns show a single outlined `●` marker instead of multi-line text; full detail, including the race `defName`, lives in a draggable, non-pausing panel with a responsive two-column grid, wrapped long values, content-adaptive height (no scrollbar), screen-edge collision, and double-Esc close.

## [2026-07-30 00:32 UTC+8] Version 0.2.0

Release of the accepted 0.2.0 feature set.

### Added
- Added 15 fixed one-shot actions, including Draft, Undraft, bounded Core attacks, player-ordered Equip, and Mental Break feedback.
- Added opt-in independent Race and Xenotype VoicePacks, the Off/Fallback/Remix source modes, and exact Xenotype `defName` targeting.
- Added the ordinary built-in Race Example and the separately enableable Extras Template; the Template remains the single maintained Example-audio source.
- Added the immediate settings surface with three regular pages, a seven-click Developer & Diagnostics page, coalesced saving, and close flush.

### Changed
- Made the No-DLC Core baseline end-to-end: Xenotype features are optional deltas while Race and Vanilla fallback remain available.
- Moved mood pitch/volume modulation to runtime SoundInfo factors and retained XML-driven action, cooldown, probability, and distance behavior.
- Standardized third-party audio roots as `<lowercase packageId>/<PackDef.defName>/<Action>/`.

### Fixed
- Made vocal-organ rejection and successful playback consume attempt cooldowns; Death remains exempt only from the Talking chance gate.
- Bounded attack and equipment triggers to their supported Core/player-command paths.
- Prevented HAR-only official Xenotypes from appearing as standalone settings candidates, and added an explicit confirmed action to forget unavailable Xenotype settings.
- Made successful-dispatch result motes follow their recording switch while the DebugAction menu remains gated by RimWorld Dev Mode.

### Breaking
- Removed the legacy `1.6/Sounds/Squeak/` audio tree and Pure/custom-only source model.
- Third-party audio is now supplied as an independent VoicePack rather than installed in the main mod.
- Legacy audio-selection values are not automatically migrated. This entry makes no claim about retention of other settings.

### Packaging
- Updated the built-in and Template Example voice set to the 41-clip public-domain delivery, normalized as 22.05 kHz mono Ogg Vorbis: Attack 3, Call 4, Death 2, Draft 3, Eat 2, Equip 2, Joy 3, MentalBreak 1, Move 3, Select 3, Sleep 3, Social 3, Undraft 3, Work 3, Wounded 3.
- Staging mirrors the 15-action/41-OGG Template source into the built-in Example and verifies action keys, exact per-action counts, and SHA-256 identity.
- Local testing remains dist-only; packaging uses the Dev, Steam, GitHub, and shared staging scripts without deployment by builds.

## [2026-07-04 18:35 UTC+8] Version 0.1.0

First public release.

### Added
- Added the full data-driven squeak system for Ratkin pawns.
- Added squeak triggers for idle calls, eating, sleeping, wounds, selection, movement, social time, joy, and death.
- Added action cooldowns and per-pawn global cooldown control.
- Added mood-based runtime pitch and volume modulation.
- Added vanilla guinea-pig fallback sounds.
- Added custom audio folder support.
- Added custom-only audio mode.
- Added distance volume fade presets and custom attenuation bands.
- Added time-speed cooldown scaling to reduce high-speed trigger density.
- Added Talking-capacity frequency scaling.
- Added death feedback.
- Added the mood modulation workbench with exact inputs, presets, and preview.
- Added DebugAction overlay toggles and camera height indicator.
- Added English and Simplified Chinese localization.
- Added Dev, GitHub, and Steam build flavors.
- Added packaging scripts for local test, GitHub release, and Steam staging.

### Fixed
- Fixed startup patch binding issues.
- Fixed XML range parsing.
- Fixed mote definition errors.
- Fixed high-speed volume loss.
- Cleaned release package contents.

## [2026-07-05 08:14 UTC+8] Version 0.1.1

Patch release for release-build troubleshooting and localization polish.

### Changed
- Removed the incorrect Dev-only compile gate from player-facing DebugActions.
- DebugActions now ship in GitHub and Steam builds.
- Overlay toggles and the camera height indicator are still gated by RimWorld Dev Mode and active-map state.
- Added localized in-game mod name and description support.
- About.xml remains the English fallback for mod metadata.
- Renamed the distance settings section to `Distance volume fade`.
- Added the 3A disclosure to the README: AI-designed, AI-developed, and AI-illustrated.
- Custom audio rights are now declared by audio providers, not by this mod.

### Fixed
- Fixed missing DebugActions in release builds.
- Fixed early language-load translation errors for localized mod metadata.
- Removed local debug path data from release DLLs.

## [2026-07-05 08:14 UTC+8] Initial Workshop Upload

Initial Steam Workshop upload for Squeaky Ratkin.

### Included
- Includes all features from Version 0.1.0.
- Includes the Version 0.1.1 release-build DebugAction fix.
- Includes localized in-game mod name and description support.
- Includes the final Steam flavor package prepared for Workshop upload.

### Notes
- The mod uses vanilla guinea-pig audio as fallback by reference only.
- No vanilla audio assets are redistributed.
- Custom audio files, if added later, are licensed by their own providers.
