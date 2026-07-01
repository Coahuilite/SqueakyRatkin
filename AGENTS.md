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
- **Never scan paths outside the project root.** `grep/glob/read` touching `C:\Program Files`, Steam install dirs, or sibling mod folders triggers a privilege prompt and fails. Required RimWorld APIs must be supplied by the caller in the prompt — do not search for them yourself.
- **`SR_` prefix on all Defs** (global Def-database collision avoidance); C# classes carry no prefix (namespace isolation).
- Code license MPL-2.0; audio All Rights Reserved; **vanilla assets are referenced by defName/clipFolderPath only, never redistributed** (Ludeon policy).

## Architecture Red Lines (read before editing; do not regress)
- **Data-driven**: `actions` (trigger mode EachTime/RandomOneShot/External + interval + probability) and `moodMods` (mood modulation) live in `1.6/Patches/Ratkin_AddSqueakComp.xml`; CompSqueaker adapts generically. Changing behavior = editing XML, no recompile.
- **Runtime modulation**: mood is applied via `SoundInfo.pitchFactor/volumeFactor` at runtime (one SoundDef per action + one neutral audio set). **Do not regress to a mood×action SoundDef matrix** (previously corrected as over-engineering).
- **Three-layer config** (bottom → top): `CompProperties` (XML default) ← `ModSettings.moodOverrides` (player override) ← `useCustomOnly` (source switch).
- Camera reuses the vanilla `Pawn_CallTracker` idiom: `CurrentViewRect.ExpandedBy(10).Contains()` view culling + `CurrentZoom <= Close` zoom gating + `TickRateMultiplier` time-speed volume.

## Junction Dev Mechanism (local dev core)
Make `RimWorld/Mods/SqueakyRatkin` a junction to this workspace root so builds load instantly.
- `scripts/validate-junction.ps1` runs at `BeforeBuild`, **non-blocking** (warns and prints the fix command if missing).
- Developer chooses the RimWorld Mods location (priority high → low):
  1. Env var `$env:RIMWORLD_DIR` (pointing at the Mods dir)
  2. Script params `-wsRoot <root> -modName <name>`
  3. Auto-detect common Steam paths (`I:\SteamLibrary\...`, `C:\Program Files (x86)\Steam\...`)
- **To create the junction for a developer**: run `validate-junction.ps1`, then execute the `New-Item -ItemType Junction -Path '<Mods>/SqueakyRatkin' -Target '<root>'` it prints. **Never hardcode paths; never assume where RimWorld is installed.**

## Build Flavor
`-p:SqueakyBuildFlavor=Dev|Steam|GitHub` → constants `SQUEAKY_DEV/STEAM/GITHUB`, only affects the `Mod.cs` startup-log banner (`[dev|steam|github]`). Default Dev. Runtime behavior is identical across flavors.

## Build Verification (mandatory)
```
dotnet build Source/SqueakyRatkin/SqueakyRatkin.csproj
```
workdir = project root. Must be 0 errors. A missing-junction WARNING is normal (non-blocking).

## Pack (distribution)
- `scripts/pack-steam.ps1` → `dist/steam/SqueakyRatkin/` (Steam flavor)
- `scripts/pack-github.ps1` → `dist/github/SqueakyRatkin-v<ver>.zip` (GitHub flavor)
- Content filter: only `About/`, `LoadFolders.xml`, `1.6/`. Excludes `Source/`, `*.pdb`, `About/PublishedFileId.txt`, `dist/`, `*.md`, `LICENSE`, `scripts/`.

## Key File Map
```
Source/SqueakyRatkin/
  CompSqueaker.cs          data-driven squeak Comp + three-layer mood merge
  SqueakyRatkinSettings.cs ModSettings modulation workbench UI
  SqueakLabels.cs          localization helper
  Mod.cs                   entry + flavor + load log
  Patches/                 Wounded + Select Harmony patches
  Debug/                   dev debug (overlay / browser / menu)
1.6/
  Defs/SoundDefs/          16 SoundDefs (8 actions × 2 sets, guinea-pig default)
  Defs/MoteDefs/           white-bg overlay mote
  Patches/Ratkin_AddSqueakComp.xml  actions + moodMods (data-driven core)
  Sounds/Squeak/<Action>/  custom audio placeholders (players place custom audio here)
  Languages/{EN,SC}/Keyed/ localization
scripts/                   validate-junction / pack-steam / pack-github
```

## Debug Entry (development mode)
Developer menu → "Squeaky Ratkin" category: overlay toggle ×2, sound browser. DevMode plays auto-log.
