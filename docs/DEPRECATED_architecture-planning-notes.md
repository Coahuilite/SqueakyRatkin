# DEPRECATED — Architecture Planning Notes

> **Historical record — do not use as current guidance.** Superseded by [`project-architecture-contract.md`](./project-architecture-contract.md) and [`settings-ui-product-contract-zh.md`](./settings-ui-product-contract-zh.md). The original text below is retained for archaeology only.

> **Status — 2026-07-21:** This is the current technical planning and implementation-boundary note for the 0.2.0 worktree. Static implementation and build checks are complete for the items marked implemented below; they are **not** a release claim. The RimWorld Dev-run release matrix, especially the zero-DLC baseline, remains a release gate.

## Product and Loading Boundary

Squeaky Ratkin targets **NewRatkinPlus only**: HAR's `Ratkin` race supplied by package `Solaris.RatkinRaceMod`. It is not a generic HAR-race sound framework and does not attempt to mediate compatibility between third-party Ratkin xenotype packs.

`LoadFolders.xml` gates the packaged content on NewRatkinPlus being active. When it is absent, the mod's content (including the assembly and Defs) is skipped quietly rather than producing a missing-dependency error. Harmony and HAR/NewRatkinPlus are the intended baseline; HugsLib is not required.

The hard compatibility baseline is RimWorld Core + Harmony + HAR/NewRatkinPlus + Squeaky Ratkin with **all official DLC disabled**. Biotech is an optional enhancement, never a dependency. The static boundary is in place, but a real no-DLC load, settings round-trip, event, and playback run is still required before release. No documentation should claim that the complete no-DLC matrix has passed until that run is observed.

## Eligibility and Optional Xenotypes

The XML patch attaches `CompSqueaker` to the NewRatkinPlus Ratkin race. Harmony event entry points dispatch by component presence; they do not apply a second `IsRatkin`/race-name eligibility gate. Consequently, an unavailable, unknown, or failed Xenotype lookup must never deny an otherwise applicable Ratkin event.

The sole technical Xenotype identity is the exact, case-sensitive `XenotypeDef.defName`. When Biotech is available, HAR discovery may provide candidate and diagnostic hints through an isolated reflection adapter, but it is not an eligibility authority. A `CompSqueaker` Ratkin pawn's current Xenotype `defName` is the runtime authority for matching. Each exact `defName` has one independent preset. The settings list presents player-facing localized `LabelCap` text and the xenotype icon, with `defName` as technical information; localized text, labels, and icons are never used for eligibility or persistence, although search may match both localized text and `defName`.

There is no gene-target or gene-whitelist configuration. The main mod neither patches nor resolves conflicts among third-party xenotype mods. If Biotech, the catalog, a pawn's genes, a preset, or an audio pool is unavailable, resolution is `GlobalOnly`; the normal XML/global behavior and audio fallback remain available. Biotech-inactive paths do not execute Xenotype DefDatabase or pawn-gene work.

## Configuration Merge and Audio Resolution

Behavior is data-driven by `1.6/Patches/Ratkin_AddSqueakComp.xml`: `actions` specify fixed action behavior and `moodMods` specify neutral-base runtime modulation. Effective configuration merges field by field in this order:

1. XML `CompProperties` defaults;
2. global `ModSettings` overrides;
3. per-Xenotype preset deltas.

An absent Xenotype preset, or an absent field within one, inherits the lower layer. This applies to mood and behavior deltas; it is not an all-or-nothing profile replacement.

The audio-pack model below supersedes the earlier Xenotype-pool → bundled-Official → Vanilla design. Audio packs only contribute audio; they never override actions, trigger scope, intervals, probability, cooldowns, vocal capability, distance, or mood.

`SqueakVoicePackDef` is the one unified pack Def. One Def represents exactly one scope/target, selection, weight, and validation-error unit; it is never a multi-target pool. It has two valid scopes:

1. **Race packs** apply to every NewRatkinPlus Ratkin carrying `CompSqueaker`. They do not depend on Biotech.
2. **Xenotype packs** apply to one configurable Ratkin Xenotype and are optional Biotech deltas.

Race scope has no target and therefore works without Biotech. Xenotype scope explicitly opts in to one string `targetDefName`: an exact, case-sensitive `XenotypeDef.defName`, not an XML cross-reference. Players explicitly select one or more Race packs globally and one or more Xenotype packs per target. Pack targets, saved selections/presets, and HAR hints form the UI candidate union; a target that is not currently loaded is retained as unavailable/dormant and its binding resumes automatically when the same `defName` returns. Same-name ambiguity fails closed. The stable identity is `PackageIdNonUnique:SqueakVoicePackDef.defName`, but Race and Xenotype selections are stored in separate domains. Within one playable tier and action, eligible PackDefs are selected uniformly before their SoundDefs/grains, so shipping more clips does not grant a package more tier weight.

The main mod contains only Vanilla fallback/preview transport and has no custom voice pack. An independently released Official example pack is an ordinary race pack: the main mod does not recognize its packageId, DefName, author, brand, priority, or weight, and does not auto-enable it.

The destructive audio-schema update defines three modes; old audio selections and old remix values are not migrated, while global and per-Xenotype behavior/mood settings remain intact:

- **OFF**: Vanilla only. Saved pack choices remain but do not participate.
- **FALLBACK**: per action, Xenotype tier → race tier → Vanilla.
- **REMIX**: all currently playable tiers are equally weighted. Xenotype/race/Vanilla are each one tier; three available tiers are each 1/3, two are each 1/2, and one is 100%. A selected pack tier then performs its normal pack-first uniform choice.

Selected keys that temporarily disappear are retained as orphan choices, shown persistently in settings, and excluded from resolution. Xenotype choices that cannot currently be evaluated because Biotech, the target, or the catalog is unavailable are dormant rather than orphaned. A matching pack/binding returning automatically resumes selection; players may explicitly forget a choice without affecting behavior/mood presets or other pack choices.

HAR final-whitelist/`CanUseXenotype` data, source-package metadata, and Core/Ludeon/unknown-source classification are UI hints or diagnostics only. They must never qualify or disqualify an audio tier, a pack target, or a Xenotype preset. A correctly declared pack target and a current `CompSqueaker` Ratkin pawn with the same exact `defName` remain valid even when HAR discovery is unavailable or disagrees. This does not add gene-level configuration or broaden the mod to arbitrary HAR races.

Mood pitch and volume remain runtime `SoundInfo` modulation rather than a race/Xenotype/action/mood SoundDef matrix.

## Settings and Player Troubleshooting Surface

The current settings surface has three pages:

- **Basics**: trigger/cooldown and distance behavior, voice-pack mode, accessibility/support options, and the version seven-click unlock.
- **Sound & Mood**: canonical global mood overrides, explicit Apply/Revert behavior, XML inheritance, and resolved-clip preview.
- **Xenotypes**: optional Biotech catalog, LabelCap/icon preset list, per-Xenotype behavior and mood deltas, voice-pack selection, filtering, and orphan/dormant-aware persistence.

The Animal Voice Workbench is available in every build flavor after the seven-click unlock. It is a player troubleshooting tool, not a Dev-only feature: it indexes loaded animal voice references and previews resolved clips without writing game data. The existing DevMode overlay, camera indicator, and diagnostic actions also compile in Dev, GitHub, and Steam flavors, while their runtime availability remains appropriately gated by Dev Mode and map state.

UI layout, confirmation dialogs, save/load round-trips, workbench interactions, and scaling still require the pending real-game matrix; their static completion is not evidence that every UI state has been observed.

## Vocal Capability and Cooldown Contract

Vocal capability has two independent gates:

- A sanitized vocal-organ efficiency gate is a hard mute for every action, including Death.
- When the player option is enabled, ordinary actions use the pawn's clamped `Talking` capacity as a chance gate. Death is exempt from the Talking gate only.

The capability check happens after action enablement, probability, and timing decisions. A vocal rejection and a successful playback both consume the same per-action and shared attempt cooldowns; this prevents a muted or speech-impaired pawn from retrying every tick. The `Talking` and organ/Death boundary, including RNG and cooldown consumption, remains a Dev-run verification item.

## Fixed 15-Action Model and Production Policy

The action model is intentionally fixed rather than an open registry. It has 15 built-in Core definitions and constructs a fixed 15-slot action plan from the XML DTOs using last-wins semantics. Definitions hold stable display/audio keys, scope support, and vocal-gate policy; plans hold configured behavior; trigger invocations carry the periodic or external origin. XML remains responsible for trigger mode, interval, probability, cooldown clock, and global-cooldown bypass fields.

The settings-owned fixed 15-slot global policy is published independently of `SqueakRuntimeResolver`. `CompSqueaker.TryTrigger` consults it before resolver, context, RNG, timing, vocal, or playback work. Thus Disabled is a production-only absolute gate even when resolver construction or its GlobalOnly exception fallback is involved; explicit preview paths deliberately bypass it. Draft and Undraft are independently persisted controls presented as one combined settings item. Draft/Undraft accept only the player gizmo commands; Attack defaults to eligible Core attack successes; Work defaults to player-forced work; Equip accepts only the player-issued Core Equip job and exposes no invalid “any occurrence” setting.

Attack coverage is deliberately bounded: Harmony dynamically patches successful `TryCastShot` implementations declared by Core `Verse`/`RimWorld` `Verb` classes, including standard melee/ranged and Core special verbs that use that method. It excludes Ability-named verbs and every DLC assembly. This is not a claim to cover DLC, generic Ability, or attack systems that do not use Core `Verb.TryCastShot`.

Adding a new action is therefore a coordinated source/Def/localization/trigger change. This is deliberate: there is no plugin registry, and `Sustained`/long-form voice playback is not implemented or advertised. Existing Move and Sleep behavior remains occasional one-shot activity, not sustained audio.

## Testing and Release Evidence

## Logging Protocol and Implementation Boundary

The structured logging implementation is statically present in the single closed typed `SqueakLog` facade and registry. The registry fixes every event's ID, visibility, level, English human sentence, and payload schema; business code cannot submit free-form equivalents. Daily records retain the fixed human-first sentence; detailed diagnostics append the versioned `srdiag` suffix only when detailed logging is effective. `SqueakDevLoggingMode` defaults Auto to enabled in Dev and disabled in GitHub/Steam, while Enabled and Disabled override Auto. This control is independent of both RimWorld Dev Mode and the seven-click player troubleshooting unlock; the unlock only exposes the setting. A mode-change record has a human result even when the selected mode makes detailed diagnostics ineffective.

The dispatch-success diagnostic path emits its first detail record immediately, then limits detail to one record per action per five seconds and emits an aggregate summary at most once per 60 seconds. A change in effective detailed-logging state resets audio sampling and the thread-safe exact once-key session registry, so re-enabling starts with a fresh immediate per-action detail record. There is no map-lifecycle reset at present. DevOnly callers must use the `ShouldEmitDev` gate before constructing diagnostic strings or fields; the facade also rejects disabled DevOnly records before emission. Once suppression applies to Info, Warning, and Error, with a 1024-key session cap; Error remains a Verse red error. The protocol, event registry, field ordering, encoding, and privacy constraints are specified in [`logging-protocol.md`](./logging-protocol.md).

This is a static implementation statement only. It does not establish that output appearance, rate limiting, session resets, player-log collection, or the Dev/GitHub/Steam matrix has been observed in a live RimWorld session.

Local testing is distribution-only: build the Release Dev flavor, run `scripts/pack-dev.ps1` to stage `dist/dev/SqueakyRatkin/`, then manually install or copy that staged package into RimWorld. Builds and pack scripts do not deploy to the game Mods directory. GitHub and Steam flavors are reserved for their release paths.

Before 0.2.0 can be released, the outstanding real-game evidence includes:

- zero-DLC baseline load, settings save/load, all applicable actions, fallback playback, and clean logs;
- HAR reflection and Biotech Xenotype discovery, presets, icons, audio-pool selection, orphan retention, and reminder behavior;
- settings layout and interaction at supported UI scales, confirmation-button placement, and Scribe round-trips;
- all flavor troubleshooting/workbench behavior; and
- vocal organ/Talking/Death/cooldown/RNG behavior plus normal in-map playback.

The voice-pack redesign has a complete static implementation closure. It remains subject to the real-game acceptance matrix above; this is not a claim that RimWorld runtime acceptance has passed:

1. **P0 — schema/semantics (implemented statically):** unified Def and OFF/FALLBACK/REMIX contract; destructive audio-selection reset; no legacy audio-schema migration; non-audio behavior/mood presets remain.
2. **P1 — Race-pack closure (implemented statically):** Race Def/Catalog/selection/orphan/reminder/resolver path is designed to work with Biotech disabled.
3. **P2 — Xenotype delta (implemented statically):** Xenotype Def/Catalog/selection with exact-defName pack matching; HAR/source information is UI-only, and unavailable targets retain dormant bindings.
4. **P3 — content/contract (implemented statically):** independent example package, third-party XML contract, bilingual documentation, and packaging/release audit surface.

Static builds, XML checks, and diff checks establish implementation health, but they do not replace these observations.
