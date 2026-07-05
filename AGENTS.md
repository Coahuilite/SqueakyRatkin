# AGENTS.md — Squeaky Ratkin

> This file is for AI agents. Human developers, please read `README.md`.
> 本文件给协助开发的 AI agent。人类开发者请读 `README.md`。

## Overview
Ratkin squeak QOL mod. RimWorld 1.6, C# (Harmony patch + ThingComp) + XML. namespace `SqueakyRatkin`, packageId `coahuilite.squeakyratkin`.

## Memory Protocol

This repo uses three root memory files. Maintain them as part of every non-trivial session.

| File | Role | Read Rule | Write Rule |
| --- | --- | --- | --- |
| `MEMORY.md` | Current durable project knowledge: repo state, confirmed constraints, architecture decisions | Read at session start before claiming project context | Add only facts/decisions that should guide future work; keep compact |
| `TODO.md` | Current task surface: goal, in-progress, pending, blocked, done | Read at session start before continuing work | Update whenever tasks start/finish/block/drop/change |
| `OBLIVIONIS.md` | Cold archive for downgraded memory | Do **not** read at session start; read only for historical conflict or explicit request | Move inactive-but-useful summaries here from `MEMORY.md` with date/reason/status |

Maintenance rules:
- Keep `MEMORY.md` stable and short: facts, signed-off decisions, constraints, evidence pointers only.
- Keep `TODO.md` operational: current goal, next actions, blockers, recent results.
- Downgrade stale `MEMORY.md` items into `OBLIVIONIS.md` with date/reason/status when they no longer guide current work.
- If an `OBLIVIONIS.md` item becomes relevant again, re-summarize into `MEMORY.md` with `source: OBLIVIONIS.md`.
- Editing README / CONTRIBUTING / AUDIO_GUIDE does NOT by itself require changing memory files; update them only when durable project facts, tasks, blockers, or archival state change.

## Hard Constraints (violation fails the task)
- **Project-root scope by default.** Do not scan or read paths outside the project root unless the user explicitly authorizes the exact external path(s) for the current task. When authorized, keep access read-only, limited to the named path(s), and do not broaden to parent directories, sibling mod folders, Steam-wide scans, or global search roots. If more external context is needed, ask for a new explicit authorization first.
- **`SR_` prefix on all Defs** (global Def-database collision avoidance); C# classes carry no prefix (namespace isolation).
- Code license MPL-2.0; audio All Rights Reserved; **vanilla assets are referenced by defName/clipFolderPath only, never redistributed** (Ludeon policy).

## Architecture Red Lines (read before editing; do not regress)
- **Data-driven**: `actions` (trigger mode EachTime/RandomOneShot/External + interval + probability) and `moodMods` (mood modulation) live in `1.6/Patches/Ratkin_AddSqueakComp.xml`; CompSqueaker adapts generically. Changing behavior = editing XML, no recompile.
- **Field-driven action behavior**: paper categories such as ambient/feedback/critical are documentation aids only. Runtime behavior must be driven by XML fields such as `ignoreGlobalCooldown` and `cooldownClock`; do **not** hardcode behavior by action name unless no data expression exists.
- **Runtime modulation**: mood is applied via `SoundInfo.pitchFactor/volumeFactor` at runtime (one SoundDef per action + one neutral audio set). **Do not regress to a mood×action SoundDef matrix** (previously corrected as over-engineering).
- **Three-layer config** (bottom → top): `CompProperties` (XML default) ← `ModSettings.moodOverrides` (player override) ← `useCustomOnly` (source switch).
- **Distance presets are XML-driven**: Conservative/Balanced/Strong camera-height attenuation defaults live in `CompProperties_Squeaker.distancePresets` via `1.6/Patches/Ratkin_AddSqueakComp.xml`; settings copy the selected preset's actual `distanceRange`, and manual edits switch to Custom.
- Camera reuses the vanilla `Pawn_CallTracker` idiom: `CurrentViewRect.ExpandedBy(10).Contains()` view culling (perf) + **distRange distance attenuation** (`SoundInfo.InMap(TargetInfo(Pawn))`, 15~70 cells linear fade, >70 silent; 2026-07 removed `CurrentZoom<=Close` zoom gating which blocked distRange). High-speed noise control is done by cooldown scaling, not by lowering per-sound `volumeFactor`.
- **Build identity in logs**: startup logs must show the intended channel identity. Dev builds need the strongest distinction and identify by commit (`AssemblyInformationalVersion` source revision); GitHub builds identify by tag plus the tag commit; Steam builds identify only by the Steam package version. This prevents old-version bug reports being mistaken for current builds while keeping Steam logs concise.

## Junction Dev Mechanism (local dev core)
Make `RimWorld/Mods/SqueakyRatkin` a junction to this workspace root so builds load instantly.
- `scripts/validate-junction.ps1` runs at `BeforeBuild`, **non-blocking** (warns and prints the fix command if missing).
- Developer chooses the RimWorld Mods location (priority high → low):
  1. Env var `$env:RIMWORLD_DIR` (pointing at the Mods dir)
  2. Script params `-wsRoot <root> -modName <name>`
  3. Auto-detect common Steam paths (`I:\SteamLibrary\...`, `C:\Program Files (x86)\Steam\...`)
- **To create the junction for a developer**: run `validate-junction.ps1`, then execute the `New-Item -ItemType Junction -Path '<Mods>/SqueakyRatkin' -Target '<root>'` it prints. **Never hardcode paths; never assume where RimWorld is installed.**

## Build Flavor
`-p:SqueakyBuildFlavor=Dev|Steam|GitHub` → constants `SQUEAKY_DEV/STEAM/GITHUB`, default Dev. Startup-log banner differs by flavor. Player-facing troubleshooting DebugAction entries and the camera-height indicator must compile in all flavors; they are gated at runtime by RimWorld Dev Mode and map state, because players need them to tune distance attenuation and report issues. Internal-only instrumentation may still be `SQUEAKY_DEV`-gated.
- Build commands/workflows pass flavor-specific `InformationalVersion` values into the DLL: dev uses the SDK source revision, GitHub uses `<tag>+<commit>`, Steam uses only the package version.

## Build Verification (mandatory)
```
dotnet build Source/SqueakyRatkin/SqueakyRatkin.csproj
```
workdir = project root. Must be 0 errors. A missing-junction WARNING is normal (non-blocking).

## Pack (distribution)
- Pack scripts are packaging-only: build the intended flavor first, then stage/zip. They must not compile; this keeps CI from building twice.
- `dotnet build Source/SqueakyRatkin/SqueakyRatkin.csproj -c Release -p:SqueakyBuildFlavor=Dev` then `scripts/pack-dev.ps1` → `dist/dev/SqueakyRatkin/` (**local testing only**, Dev flavor; use this when copying a local test build into RimWorld/Mods).
- Build Steam flavor with `-p:SqueakyBuildFlavor=Steam -p:SqueakyInformationalVersion=<version> -p:IncludeSourceRevisionInInformationalVersion=false`, then `scripts/pack-steam.ps1` → `dist/steam/SqueakyRatkin/` (Steam flavor)
- CI release builds GitHub flavor once with `<tag>+<commit>`, then `scripts/pack-github.ps1` → `dist/github/SqueakyRatkin-v<ver>.zip` (GitHub flavor)
- Content filter: only `About/`, `LoadFolders.xml`, `1.6/`. Excludes `Source/`, `*.pdb`, `About/PublishedFileId.txt`, `dist/`, `*.md`, `LICENSE`, `scripts/`.
- **Flavor discipline**: local dev/test directory packages must use Dev flavor (`pack-dev.ps1`). GitHub flavor packages must be produced by CI release/tag flow only. Steam flavor is reserved for Steam Workshop packaging/upload, not local dev testing.

## Key File Map
```
Source/SqueakyRatkin/
  CompSqueaker.cs          data-driven squeak Comp + three-layer mood merge
  SqueakyRatkinSettings.cs ModSettings modulation workbench UI
  SqueakLabels.cs          localization helper
  Mod.cs                   entry + flavor + load log
  Patches/                 Wounded + Select Harmony patches + player troubleshooting HUD
  Debug/                   DevMode troubleshooting actions / overlay / mote maker
1.6/
  Defs/SoundDefs/          27 SoundDefs (9 actions × 3 sets: base/Pure/Preview, guinea-pig default)
  Defs/MoteDefs/           white-bg overlay mote
  Patches/Ratkin_AddSqueakComp.xml  actions + moodMods (data-driven core)
  Sounds/Squeak/<Action>/  custom audio placeholders (players place custom audio here)
  Languages/{English,ChineseSimplified}/Keyed/ localization
scripts/                   validate-junction / stage-package / pack-dev / pack-steam / pack-github
scripts/pack-dev.ps1       local Dev-flavor directory package for manual testing
```

## Debug Entry (development mode)
Developer menu → "Squeaky Ratkin" category: overlay toggle ×2 + camera indicator toggle ×2. These entries must ship in GitHub/Steam builds as player troubleshooting tools, while still requiring RimWorld Dev Mode and an active map. The `GlobalControlsUtility.DoDate` postfix can draw localized camera height/view size (SC: 高度/视野) in the vanilla right-side status stack, reading actual runtime `Find.Camera.transform.position.y` / `Find.Camera.orthographicSize` rather than re-deriving vanilla height from `RootSize`. Sound preview moved to ModSettings workbench (no DevMode needed).

## Release Flow (dev → main → tag → CI)
1. dev: atomic commits, all development here.
2. PR dev→main: `gh pr create --base main --head dev --title "<type>: <desc>"`.
3. Squash merge: `gh pr merge --squash --subject "<type>: <desc>"` (main protected: require PR + enforce_admins, no direct push).
4. Tag **main** (not dev): `git tag v<x.y.z>` stable, `v<x.y.z>-rc<N>` pre-release. Tag must point at main's squash commit.
5. Push tag: `git push origin v<x.y.z>-rc<N>` → triggers `release.yml` → pack-github + GitHub Release.
6. Pre-release detection: `release.yml` marks `prerelease: true` if tag contains `-` (e.g. v0.1.0-rc1); stable (v0.1.0) is not.
7. **Never tag dev for release.** dev is development; main is the release surface.
