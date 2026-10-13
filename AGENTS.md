# AGENTS.md — Squeaky Ratkin

> Keep this file stable. Current versions, build layout, branch policy, evidence and release state belong in `MEMORY.md`; unfinished actions belong in `TODO.md`.

## Identity and product principles

- Preserve the brands **鼠辈啁啾 / Squeaky Ratkin**, permanent `packageId` `coahuilite.squeakyratkin`, and C# namespace `SqueakyRatkin`. Use `鼠族` for the Ratkin race in ordinary Simplified Chinese prose; identifiers, paths, logs and proper names are exempt.
- Routing follows the VoicePack's exact `raceDefName`, without built-in race special-casing. Discovery failures must degrade without crashing; discovery mechanics do not override declared product dependencies.
- Uninstall safety is mandatory: no permanent mod state in saves. Preserve public author XML compatibility and frozen log bytes; distinguish verified behavior from assumptions.
- Read implementation from source and use the maintained scripts. Do not recreate obsolete tooling or duplicate implementation summaries here.

## Authoritative sources

- Behavior: `docs/project-architecture-contract.md`, `docs/settings-ui-product-contract-zh.md`, `docs/logging-protocol.md`.
- Release process and privacy decisions: `docs/release-runbook-zh.md`. Evidence and unresolved rulings: `docs/maintenance-status-zh.md`; historical release evidence: `docs/release_review/`.
- Workshop body: `docs/steam-workshop-page-copy-draft.md`. Release notes belong in `docs/CHANGELOG.md` and `docs/CHANGELOG.zh-CN.md`, without duplication in the page draft. VoicePack authoring: `.github/skills/squeaky-voicepack-authoring/SKILL.md`.
- Source precedence: explicit maintainer ruling > contracts/runbook > `MEMORY.md` > `TODO.md` > `OBLIVIONIS.md`.

## Memory protocol

- Read `MEMORY.md` and `TODO.md` at the start of a non-trivial session. Read `OBLIVIONIS.md` for a historical conflict, a reopened topic or an explicit request.
- `MEMORY.md` stores compact current facts, decisions and authority pointers. `TODO.md` stores concrete unfinished actions; remove completed actions instead of retaining a completed checklist.
- Archive superseded decisions in dated `OBLIVIONIS.md` sections with a reason and status. If reopened, summarize into MEMORY with `source: OBLIVIONIS.md`.
- Release evidence belongs in `docs/release_review/`. Keep raw logs, transient artifacts, test matrices and commit chains out of active memory.

## Privacy and scope

- Default file scope is this repository. Respect task-specific access authorized by the user; otherwise obtain authorization for the needed outside path without broadening the search.
- Standing read-only log exception: RimWorld's troubleshooting directory, limited to root `Player*.log` and `Logs/Player*.log`. Verify the version and session; this does not cover Config, saves or other personal data.
- Do not place expanded personal paths, raw diagnostic logs, pawn data, credentials or Workshop item ID values in Git, documentation or release artifacts. Keep `PublishedFileId.txt` outside the repository and package.
- Scan the complete reachable history before pushing, following the runbook. A green scan does not prove all possible private data is absent.
- Known history debt stays explicit and narrowly scoped; never silently broaden exceptions. If a real secret becomes reachable, stop, revoke or rotate it, and obtain authorization for incident-specific history cleanup.
