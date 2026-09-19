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

## Authoritative entry points

- Current status, evidence boundaries and open rulings: `docs/maintenance-status-zh.md`. There is no repository codemap; read the source for structure.
- Binding behavior: `docs/project-architecture-contract.md`, `docs/settings-ui-product-contract-zh.md`, `docs/logging-protocol.md`. Process: `docs/release-runbook-zh.md`. Release evidence: `docs/release_review/`.
- VoicePack authoring (single source, also the agent skill): `.github/skills/squeaky-voicepack-authoring/SKILL.md`.
- Source of truth order when statements conflict: maintainer ruling > contracts/runbook > `MEMORY.md` > `TODO.md` > `OBLIVIONIS.md` (cold evidence).

## Memory protocol

At every non-trivial session:

- Read `MEMORY.md` before claiming project context; it stores confirmed durable facts, decisions, constraints, and evidence pointers.
- Read `TODO.md` before continuing work; it stores only current goals, open actions, blockers, and explicit deferrals.
- Read `OBLIVIONIS.md` only for a historical conflict or explicit request; it is cold archive evidence and cannot override current sources.

Maintain these boundaries:

- Update `MEMORY.md` only when durable facts or the open action surface changes; keep it compact.
- Compact by default: settled release/implementation details live in `docs/release_review/` (Claim Packs, process review) and runbook; MEMORY keeps only pointers. Do not grow MEMORY with finished work.
- Compacted memory lands in `OBLIVIONIS.md`: completed or no-longer-guiding entries from `MEMORY.md` / `TODO.md` move there as dated sections in its existing `date + reason + status` format; if such an entry becomes relevant again, re-summarize it into `MEMORY.md` with `source: OBLIVIONIS.md`. Released-version implementation details still go to `docs/release_review/` (previous bullet) — do not mix the two destinations.
- Update `TODO.md` only when its current task surface changes.
- Do not store session narratives, transient artifacts, raw logs, completed test matrices, commit chains, or release checklists in either active memory file.
- Documentation edits alone are not memory events; external-state summaries never override their authoritative source.

## Privacy and security

- Default scope is the repository root. Reading outside it requires authorization for the exact path, is read-only, and must not broaden to parents, siblings, Steam-wide roots, or global search. The named RimWorld `Player.log` troubleshooting directory is the standing read-only exception; do not broaden it without authorization.
- Never place personal local state, expanded local paths, diagnostic-log excerpts, credentials, API keys, tokens, private keys, or `PublishedFileId.txt` in Git, documentation, generated artifacts, staging, or reachable history.
- Every push — release or temporary staging — is preceded by a privacy review of the complete reachable range, not just HEAD. The check is `pwsh scripts/privacy-audit.ps1 -FullHistory` (three independent vectors: working tree / commit messages / historical blobs, plus the identity face); CI runs its default mode on every push and PR. Write documentation privacy-free from the start: no personal local state, expanded paths, log excerpts, credentials, or `PublishedFileId.txt` values.
- If a real secret enters reachable history, stop first; revoke or rotate it, then perform incident-specific history cleanup. Do not claim cleanup alone resolves the secret.
- Repository evidence, stage output, CI output, or one publication channel cannot prove another channel's external state. Treat unknown manual or external state as unverified.

## External-state boundaries

- **Local commits are permitted** without a separate authorization step: they are reversible, never leave the machine, and are how a release is assembled. The release pipeline in `docs/release-runbook-zh.md` is local up to the first push.
- **Remote-facing operations require explicit maintainer authorization**: `git remote` changes, push (including temporary staging branches), PR, merge, tag, GitHub Release, and any Workshop/Steam publication or edit.
- **Pre-push ceremony is deliberately minimal**: local commits are free; a push needs the runbook's scripted gates to pass (privacy audit; release readiness when the push is a release) plus maintainer authorization. The exact commands and their scope live in `docs/release-runbook-zh.md` — this file only fixes the principle.
- Everything else is automated by `scripts/verify-local.ps1`, `scripts/check-pack-readiness.ps1`, `scripts/privacy-audit.ps1`, and the workflows: **do not re-add manual ritual** (hand-checked version equality, package-content item-by-item lists, repeated privacy scans). A new check is added as a script first, and only then referenced from the runbook. Mutable current status belongs in `docs/maintenance-status-zh.md`, not in this file.
