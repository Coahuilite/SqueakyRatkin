# AGENTS.md — Squeaky Ratkin

> This file is for AI agents. Human developers, please read `README.md`.
> 本文件仅保存每次会话都必须知道的项目身份、记忆协定与隐私安全边界。

## Project identity

- Project: RimWorld 1.6 mod **鼠辈啁啾 / Squeaky Ratkin**.
- Permanent `packageId`: `coahuilite.squeakyratkin`.
- C# namespace: `SqueakyRatkin`.
- Permanent brands are **`鼠辈啁啾`** and **`Squeaky Ratkin`**; never translate, normalize, or copy-edit them. In ordinary Simplified Chinese prose, call the Ratkin race `鼠族`; identifiers, paths, logs, and proper names are exempt.
- Product version source: `Source/SqueakyRatkin/SqueakyRatkin.csproj` `<Version>` is primary; `About/About.xml` `<modVersion>` must follow it; Assembly/File versions derive from the csproj version.

## Project philosophy

- Routing is neutral: a VoicePack's `raceDefName` declaration is the only routing entry — no built-in race special-casing; HAR races, vanilla Human, and any other race route identically.
- Dependency access is reflective, never compiled: HAR is touched only via reflection as a discovery enhancement; missing dependencies must degrade silently, never crash.
- **Uninstall safety is a hard rule: removing the mod must never affect a saved game.** No permanent data is written into saves (defs are injected at runtime via XPath; settings and profile overrides live in the Config folder). After uninstall the save loads and plays normally — squeaks simply stop; leftover Config files are harmless and removable.

## Memory protocol

At every non-trivial session:

- Read `MEMORY.md` before claiming project context; it stores confirmed durable facts, decisions, constraints, and evidence pointers.
- Read `TODO.md` before continuing work; it stores only current goals, open actions, blockers, and explicit deferrals.
- Read `OBLIVIONIS.md` only for a historical conflict or explicit request; it is cold archive evidence and cannot override current sources.

Maintain these boundaries:

- Update `MEMORY.md` only when durable facts or the open action surface changes; keep it compact.
- Compact by default: settled release/implementation details live in `docs/release_review/` (Claim Packs, process review) and runbook; MEMORY keeps only pointers. Do not grow MEMORY with finished work.
- Update `TODO.md` only when its current task surface changes.
- Do not store session narratives, transient artifacts, raw logs, completed test matrices, commit chains, or release checklists in either active memory file.
- Documentation edits alone are not memory events; external-state summaries never override their authoritative source.

## Privacy and security

- Default scope is the repository root. Reading outside it requires authorization for the exact path, is read-only, and must not broaden to parents, siblings, Steam-wide roots, or global search. The named RimWorld `Player.log` troubleshooting directory is the standing read-only exception; do not broaden it without authorization.
- Never place personal local state, expanded local paths, diagnostic-log excerpts, credentials, API keys, tokens, private keys, or `PublishedFileId.txt` in Git, documentation, generated artifacts, staging, or reachable history.
- Every push — release or temporary staging — is preceded by a privacy review of the complete reachable range, not just HEAD. Write documentation privacy-free from the start: no personal local state, expanded paths, log excerpts, credentials, or `PublishedFileId.txt` values.
- If a real secret enters reachable history, stop first; revoke or rotate it, then perform incident-specific history cleanup. Do not claim cleanup alone resolves the secret.
- Commit, push, PR, merge, tag, release, Workshop publication, and other externally effective operations require explicit authorization.
- Repository evidence, stage output, CI output, or one publication channel cannot prove another channel's external state. Treat unknown manual or external state as unverified.
