# Squeaky Ratkin · 鼠辈啁啾

**English** | [中文](./README.zh-CN.md)

Squeaky Ratkin is a lightweight sound effect mod that adds a whimsical medley of chirps and squeaks to Ratkin pawns. Idle calls, eating, sleeping, wounds, selection, movement, social time, joy, and death can all carry a little sound, tinted by mood and softened by camera-distance attenuation.

> Lightweight, optional ambience for Ratkin Races.

> **3A disclosure:** this mod is AI-designed, AI-developed, and AI-illustrated. Human maintainers review, package, and release the work.

## Requirements
- RimWorld 1.6
- [Harmony](https://steamcommunity.com/sharedfiles/filedetails/?id=2009463077)
- [Humanoid Alien Races (HAR)](https://steamcommunity.com/sharedfiles/filedetails/?id=839005762)
- [Ratkin / 鼠族 (NewRatkin)](https://steamcommunity.com/sharedfiles/filedetails/?id=1578693166)

## Install
- **Steam**: subscribe on the Workshop (once published).
- **GitHub Release**: download the release zip and extract into `RimWorld/Mods/SqueakyRatkin/`.

## Audio
Default source is the vanilla guinea-pig sound set. To use custom squeaks:
- Place mono files in `1.6/Sounds/Squeak/<Action>/`, named `SR_<Action>_<n>.ogg`.
- Ogg is recommended; wav (16-bit) is acceptable. Use 22050 or 44100 Hz and normalize peaks around -3dBFS.
- Uncomment the matching grain in `1.6/Defs/SoundDefs/SqueakyRatkin_SoundDefs.xml`.
- Record neutral-base variants only; mood differences are produced at runtime through pitch and volume modulation.
- Use the mod settings workbench to compensate for different custom audio traits.

## Configuration
Core behavior is data-driven from `1.6/Patches/Ratkin_AddSqueakComp.xml`: actions define trigger modes, cooldowns, probabilities, and distance presets; mood mods define runtime pitch and volume changes.

Mod settings provide:
- Custom audio full override, switching between mixed vanilla fallback and custom-only sounds.
- Time-speed cooldown scaling, enabled by default, to reduce high-speed trigger density without lowering each sound's volume.
- Talking capacity frequency scaling, enabled by default, so impaired speech reduces ordinary squeak frequency while death feedback remains protected unless the pawn is organ-mute.
- Global trigger interval multiplier.
- Distance volume fade presets or custom attenuation bands.
- Mood modulation workbench with per-mood overrides, exact inputs, presets, and preview.
- Optional built-in DebugAction localization, disabled by default; reopen the debug actions menu after toggling.

## Dev Menu (development mode)
Developer menu → "Squeaky Ratkin": overlay text toggles and camera indicator toggles. Sound preview lives in the mod settings workbench.

## License
- **Code (C# / XML defs / patches)**: [Mozilla Public License 2.0](./LICENSE) — file-level copyleft, project-level combinable with proprietary code.
- **Audio assets**: custom audio contributors are responsible for declaring the license and rights for their own files. The vanilla guinea-pig audio is owned by Ludeon; this mod only references defName/clipFolderPath per Ludeon's mod policy — no vanilla assets are redistributed.
- **Third-party deps** (Harmony / HAR / Ratkin): belong to their authors, under their own licenses.

## Development

### Junction (recommended for local dev)
Make `RimWorld/Mods/SqueakyRatkin` a junction pointing at this workspace root so builds load instantly. `scripts/validate-junction.ps1` auto-checks before each build (warn-only, non-blocking).

**Choose your RimWorld Mods location** (priority high → low):
1. Env var (recommended, persistent): `$env:RIMWORLD_DIR = "<your RimWorld Mods path>"`
2. Script params: `pwsh scripts/validate-junction.ps1 -wsRoot <root> -modName SqueakyRatkin`
3. Auto-detect common Steam paths

**Create the junction manually** (the script prints this when missing):
```powershell
New-Item -ItemType Junction -Path '<your RimWorld>\Mods\SqueakyRatkin' -Target '<this repo root>'
```

### Build
```
dotnet build Source/SqueakyRatkin/SqueakyRatkin.csproj
```
Must be 0 errors. A missing-junction WARNING is normal (non-blocking).

### Build Flavor (for distribution)
`-p:SqueakyBuildFlavor=Dev|Steam|GitHub` toggles the startup-log banner (`[dev|steam|github]`). Runtime behavior is identical across flavors. Startup logs identify dev builds by commit, GitHub releases by tag plus commit, and Steam builds by package version.

### Pack
- Build the intended flavor first; pack scripts only stage/zip existing build output.
- Dev local test: build Dev flavor, then `pwsh scripts/pack-dev.ps1` → `dist/dev/SqueakyRatkin/`.
- Steam Workshop: build Steam flavor, then `pwsh scripts/pack-steam.ps1` → `dist/steam/SqueakyRatkin/`.
- GitHub Release: CI/tag flow builds GitHub flavor, then `pwsh scripts/pack-github.ps1` → `dist/github/SqueakyRatkin-v<ver>.zip`.
- Content includes only `About/`, `LoadFolders.xml`, `1.6/` (excludes source / pdb / docs).

## Branches & Contributing
- `main`: release branch (stable; tags `v*` trigger the release workflow).
- `dev`: development branch — PRs go here.

See [`CONTRIBUTING.md`](./CONTRIBUTING.md) for audio and code contribution guides.

> AI-agent development guide: [`AGENTS.md`](./AGENTS.md).
