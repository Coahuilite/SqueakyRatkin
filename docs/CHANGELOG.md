# Changelog

## Changelog Template

Use this file as the canonical English changelog for Squeaky Ratkin.

Rules:
- Keep entries in chronological order, oldest first and newest last.
- Use local release time in the heading: `[YYYY-MM-DD HH:MM UTC+8] Version X.Y.Z`.
- For the first Steam Workshop upload, use `Initial Workshop Upload` instead of a version number.
- Keep items short and visible. Prefer one change per bullet.
- Separate feature additions, changes, fixes, packaging notes, and release notes when useful.
- Bug fixes should be concise unless the fix changes player-facing behavior.
- Mention both GitHub and Steam only when the entry affects both release surfaces.
- Do not put unreleased plans here. Use `TODO.md` for pending work.
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
