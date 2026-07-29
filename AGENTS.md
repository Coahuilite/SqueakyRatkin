# AGENTS.md — Squeaky Ratkin

> This file is for AI agents. Human developers, please read `README.md`.
> 本文件给协助开发的 AI agent。人类开发者请读 `README.md`。

## Overview
RimWorld 1.6 Ratkin squeak QOL mod using C# (Harmony + ThingComp) and XML.
Namespace: `SqueakyRatkin`; packageId: `coahuilite.squeakyratkin`.

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
- Editing README / CONTRIBUTING / the voice-pack author guide or Template README does NOT by itself require changing memory files; update them only when durable project facts, tasks, blockers, or archival state change.

## Hard Constraints
- **Project-root scope by default.** Do not read outside it without explicit authorization of the exact path; authorized access is read-only and may not broaden to parents, siblings, Steam-wide roots, or global search. Ask again for further context.
- **Troubleshooting exception:** read-only access to `%USERPROFILE%\AppData\LocalLow\Ludeon Studios\RimWorld by Ludeon Studios` (for example `Player.log`) is authorized; prefer it for startup/runtime issues and do not broaden it unless named.
- The permanent brands are **`鼠辈啁啾`** and **`Squeaky Ratkin`**: never copy-edit, translate, normalize, or replace them. In ordinary Simplified Chinese prose, call the Ratkin race `鼠族`; identifiers, paths, logs, and proper names are exempt.
- All Defs use `SR_`; C# types do not.
- Code is MPL-2.0; the Example audio is public-domain material outside that code license (see `Extras/SqueakyRatkinExampleVoices/AUDIO_RIGHTS.txt`); vanilla assets are referenced only by defName/clipFolderPath, never redistributed.
- **No-DLC is mandatory end-to-end:** Core + Harmony + HAR/NewRatkinPlus + this mod works with all official DLC disabled. HugsLib and all DLC, including Biotech, remain optional in metadata, references, XML, initialization, settings, and runtime.
- Example audio has one formal authoring source: the 41 Race OGG files under `Extras/SqueakyRatkinExampleVoices/1.6/Race/Sounds/coahuilite.squeakyratkin.examplevoices/SR_ExampleTemplate_Race/`, covering exactly 15 actions: Attack 3, Call 4, Death 2, Draft 3, Eat 2, Equip 2, Joy 3, MentalBreak 1, Move 3, Select 3, Sleep 3, Social 3, Undraft 3, Work 3, and Wounded 3. Never maintain a source mirror under `1.6/Sounds/coahuilite.squeakyratkin/SR_OfficialExample_Race/`.
- Formal Example audio is OGG only. Third-party VoicePacks should distribute OGG; WAV is compatible but not recommended (see the author guide).
- If audio changes outside that Template source, changes roots or the 15-action/41-OGG contract and its exact per-action counts, adds a non-OGG format to either formal root, or staging hashes diverge: **stop and report the changed invariant**. Never silently repair/copy back, create a second source, weaken checks, or treat staged/dist audio as authoring source.

## Current Contract Index
- Runtime, actions, resolver, VoicePack model, Xenotype eligibility, Example staging, and camera behavior: [`docs/project-architecture-contract.md`](./docs/project-architecture-contract.md).
- Settings UI, immediate persistence, Xenotype preset presentation, and player diagnostics: [`docs/settings-ui-product-contract-zh.md`](./docs/settings-ui-product-contract-zh.md).
- Closed logging compatibility protocol: [`docs/logging-protocol.md`](./docs/logging-protocol.md).
- Third-party VoicePacks and OGG/WAV guidance: [`docs/voice-pack-author-guide-zh.md`](./docs/voice-pack-author-guide-zh.md).
- Historical `DEPRECATED_*` documents and `OBLIVIONIS.md` are cold evidence only and cannot override these current contracts.
- Contract conflicts are not silently normalized: identify the mismatch and resolve it in the authoritative layer.

## Build, Stage, and Release Safety
- Mandatory build (project root; **0 errors**): `dotnet build Source/SqueakyRatkin/SqueakyRatkin.csproj`.
- Local testing is dist-only: build Dev flavor (`dotnet build Source/SqueakyRatkin/SqueakyRatkin.csproj -c Release -p:SqueakyBuildFlavor=Dev`), then `scripts/pack-dev.ps1`.
- Builds/pack scripts never install to RimWorld Mods; the developer manually installs `dist/dev/SqueakyRatkin/`.
- Pack scripts stage/zip only: never compile, transcode audio, or install.
- The only manually maintained product version is `Source/SqueakyRatkin/SqueakyRatkin.csproj` `<Version>`; Assembly/File versions derive from it.
- Flavor identity: Dev logs the source-revision commit; GitHub logs `<tag>+<commit>`; Steam logs only its package version.
- Local testing is Dev only; GitHub artifacts are CI tag/release only; Steam flavor is Workshop-only.
- `pack-steam.ps1` only stages `dist/steam/SqueakyRatkin/`; staging deliberately excludes `PublishedFileId.txt`. After the first upload, copy that stage to the local RimWorld Mods upload copy (never the `workshop/content` subscription cache), write the existing item ID only to that copy's `About/PublishedFileId.txt`, and Update under the same author account. Never put the ID in Git or staging; after upload verify the same item URL/ID, visibility, preview, and change note. Subsequent versions never use `Initial Workshop Upload`.
- Distribution content is limited to `About/`, `LoadFolders.xml`, `1.6/`, and exactly `Extras/SqueakyRatkinExampleVoices/`.
- Exclude source, PDBs, `PublishedFileId.txt`, dist, docs, license, and scripts.
- Commits and every pending push range must be free of personal local state: concrete usernames or home directories, absolute workspace/Steam/tool paths, local installation or subscription details, diagnostic-log excerpts, credentials, API keys, tokens, private keys, and `PublishedFileId.txt`. Use environment variables or abstract placeholders when a path must be documented; never commit their expanded local values.
- Before pushing, inspect both the final tree and the complete commit range that would enter the remote, not only `HEAD`. If both are clean, use the normal authorized push flow; no history rewrite or special branch workflow is required.
- If sensitive local state has already entered unpushed commits, stop before pushing and rebuild or sanitize that pending history while preserving the original history only in an explicitly local archive ref with no upstream. Never push or merge that archive, including through `--all`, `--mirror`, or `--tags`. This is incident recovery, not the normal release process.
- If any real credential or private key has entered a commit, treat it as compromised: stop, revoke or rotate it first, then remove it from all history that could reach a remote. History cleanup is not a substitute for credential rotation.
- Commit, push, PR creation, squash merge, tag, and release each require explicit authorization.
- Work on `dev`; `main` is protected—never push directly. Release follows authorized `dev` → PR → squash to `main` → tag that main commit → push tag → CI artifact/release.
- Never tag `dev`; do not create a release tag before the authorized squash commit is on `main`.
- Release tags are manually made strict SemVer 2.0 (`v<version>`, optional dot-separated prerelease; no leading zeroes).
- Their base version equals csproj `<Version>` and the tag commit is in `origin/main` history. CI builds the GitHub artifact once from that tag.

## Mandatory Verification
- For code or data changes, run the mandatory build above from the project root and require 0 errors.
- For pure documentation changes, run targeted text/link checks and `git diff --check`.
- Do not run Markdown LSP or a build unless requested.
