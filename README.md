# Squeaky Ratkin · 鼠辈啁啾

**English** | [中文](./README.zh-CN.md)

A QOL mod that makes Ratkin pawns squeak on idle call / eat / sleep / wounded / select / move / social / joy / death, with mood-tinted pitch (good / neutral / bad / breakdown). Off-screen pawns are culled for performance, and in-view sounds fade naturally with distRange distance attenuation.

> The Chinese name 「鼠辈啁啾」is a playful pun; the mod itself is a lightweight, optional ambience enhancement.

## Requirements
- RimWorld 1.6
- [Harmony](https://steamcommunity.com/sharedfiles/filedetails/?id=2009463077)
- [Humanoid Alien Races (HAR)](https://steamcommunity.com/sharedfiles/filedetails/?id=839005762)
- [Ratkin / 鼠族 (NewRatkin)](https://steamcommunity.com/sharedfiles/filedetails/?id=1578693166)

## Install
- **Steam**: subscribe on the Workshop (once published).
- **GitHub Release**: download the release zip and extract into `RimWorld/Mods/SqueakyRatkin/`.

## Audio
Default source is the vanilla guinea-pig (rodent). To use custom squeaks:
- Place files in `1.6/Sounds/Squeak/<Action>/`, named `SR_<Action>_<n>.wav`
- Spec: **mono**, wav (16-bit) or ogg, 22050 or 44100 Hz, normalized to ~-3dBFS
- Uncomment the matching grain in `1.6/Defs/SoundDefs/SqueakyRatkin_SoundDefs.xml` to enable
- Mood differences are produced via runtime pitch/volume modulation — record only neutral-base variants (2–3 per action)
- Compensate per-audio traits in the mod settings "Modulation Workbench"

## Configuration
Three layers (bottom → top):
1. **Default** (author): `actions` / `moodMods` in `1.6/Patches/Ratkin_AddSqueakComp.xml`
2. **Player override**: mod settings "Modulation Workbench" — toggle a mood's override, slider + input + presets (Sharp/Neutral/Low/Chaos) + preview
3. **Source switch**: `Full override` (custom-only vs mixed with vanilla default)

Also includes a global trigger interval slider and a `Scale trigger cooldown with time speed` checkbox (default ON). Accelerated play is quieted by reducing real-time trigger density rather than lowering each sound's volume. Select feedback uses a real-time cooldown, so it remains responsive while the game is paused.

## Dev Menu (development mode)
Developer menu → "Squeaky Ratkin": overlay text on/off ×2. Sound preview lives in the mod settings workbench.

## License
- **Code (C# / XML defs / patches)**: [Mozilla Public License 2.0](./LICENSE) — file-level copyleft, project-level combinable with proprietary code.
- **Audio assets** (custom audio under `1.6/Sounds/Squeak/`): All Rights Reserved (default, pending final confirmation). The vanilla guinea-pig audio is owned by Ludeon; this mod only references defName/clipFolderPath per Ludeon's mod policy — no vanilla assets are redistributed.
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
`-p:SqueakyBuildFlavor=Dev|Steam|GitHub` toggles the startup-log banner (`[dev|steam|github]`). Runtime behavior is identical across flavors. Startup logs include a strong build identifier: dev commit, GitHub tag, or Steam package version.

### Pack
- `pwsh scripts/pack-steam.ps1` → `dist/steam/SqueakyRatkin/` (Workshop upload)
- `pwsh scripts/pack-github.ps1` → `dist/github/SqueakyRatkin-v<ver>.zip` (GitHub Release)
- Content includes only `About/`, `LoadFolders.xml`, `1.6/` (excludes source / pdb / docs).

## Branches & Contributing
- `main`: release branch (stable; tags `v*` trigger the release workflow).
- `dev`: development branch — PRs go here.

See [`CONTRIBUTING.md`](./CONTRIBUTING.md) for audio and code contribution guides.

> AI-agent development guide: [`AGENTS.md`](./AGENTS.md).
