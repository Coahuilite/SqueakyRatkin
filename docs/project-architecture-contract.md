# Squeaky Ratkin Project Architecture Contract

> This is the current project-level architecture contract. It defines implementation authority and compatibility boundaries; historical planning and review records are not substitutes.

## 1. Authority and conflict order

The effective contract is, from highest to lowest: explicit accepted product decisions; this document and the current settings product contract; source/Defs that implement those decisions; and supporting author/reference documentation. `AGENTS.md` remains the repository's operational hard-constraint index. Historical documents are evidence only. When implementation and a current contract disagree, do not silently normalize either: identify the mismatch and resolve it in the authoritative layer.

The supported product is NewRatkinPlus Ratkin only, identified by the XML-attached `CompSqueaker`; it is not a generic HAR-race framework. All Defs use `SR_`; C# type names rely on namespace isolation.

## 2. Baseline, eligibility, and optional Xenotypes

The mandatory baseline is RimWorld Core + Harmony + HAR/NewRatkinPlus + Squeaky Ratkin with every official DLC disabled. Optional DLC is an end-to-end delta, not a quiet catalog failure: settings, applicable Ratkin actions, global XML/settings behavior, mood modulation, and Vanilla fallback must remain usable. Biotech-inactive paths must not enter Xenotype DefDatabase or pawn-gene paths.

Harmony entry points dispatch by `CompSqueaker` presence; they must not add a second race-name or `IsRatkin` gate. Missing Biotech, catalog, preset, selection, pack, pawn Xenotype, or sound pool resolves to GlobalOnly/fallback and never removes ordinary event eligibility.

The sole Xenotype identity is the exact, case-sensitive `XenotypeDef.defName`. A pawn's current `defName` is the runtime authority. Localized labels, icons, HAR discovery, whitelists/`CanUseXenotype`, source-package metadata, and Core/Ludeon classification are display, search, diagnostic, or candidate hints only. A Core/Ludeon Xenotype discovered only through HAR does not create a settings candidate by itself; an explicit preset, persisted Xenotype selection, or declared Xenotype VoicePack still keeps that exact target visible. Duplicate same-name resolution fails closed for that Xenotype layer; Race/Vanilla behavior continues. No gene-level settings, arbitrary HAR races, or third-party Xenotype-conflict mediation are in scope.

## 3. Fixed action and trigger contract

There are exactly 15 built-in one-shot actions: `Call`, `Eat`, `Sleep`, `Wounded`, `Select`, `Move`, `Social`, `Joy`, `Death`, `Draft`, `Undraft`, `Attack`, `Work`, `Equip`, and `MentalBreak`. The fixed definitions provide stable display/audio keys and gate policy; XML supplies the action plan. Adding an action is a coordinated source, Def, localization, trigger, and documentation change—not a plugin registration.

Periodic actions are sampled by `CompSqueaker`; XML decides `EachTime`, `RandomOneShot`, or `External`, interval, probability, cooldown clock, and global-cooldown bypass. External actions are event driven. `Draft` and `Undraft` originate only from player gizmo changes. `Attack` is bounded to successful Core `Verb.TryCastShot` implementations declared in Core `Verse`/`RimWorld` types, excluding Ability-named verbs, DLC assemblies, and attack systems that do not use that method. `Work` can narrow to a player-forced job. `Equip` accepts only a player-issued Core Equip job, never AI, load, or system equipment changes. `Move` and `Sleep` remain occasional one-shots; sustained playback is unsupported.

The settings-owned fixed action policy runs before resolver, RNG, timing, vocal, or playback work. Disabled is absolute production silence; explicit preview is intentionally separate. Action scope is enforced both by the global policy and resolved runtime delta.

## 4. Vocal, timing, mood, and distance

A sanitized vocal-organ efficiency gate mutes every action, including `Death`. When enabled, the clamped `Talking` capacity is an ordinary-action chance gate; `Death` alone is exempt from the Talking gate. Enablement, probability, and timing are evaluated first. A vocal rejection consumes the same per-action and applicable shared attempt cooldown as a successful playback, preventing retry-every-tick behavior.

Mood is applied at dispatch through `SoundInfo.pitchFactor` and `volumeFactor`; there is one sound definition per action/audio set, never a mood × action SoundDef matrix. Camera handling uses `CurrentViewRect.ExpandedBy(10)` for view culling and `SoundInfo.InMap(TargetInfo(Pawn))` with `distRange` attenuation. The underlying SoundDef range is `15–70` cells, while the player settings default to the Balanced preset at `15–50`; beyond the active range is silent. **Do not reintroduce a zoom eligibility gate** (including `CurrentZoom <= Close`): it would incorrectly block distance attenuation. High-speed control scales cooldowns, not individual sound volume.

## 5. XML and settings merge

`1.6/Patches/Ratkin_AddSqueakComp.xml` is the behavior authority: `actions` and `moodMods` are data-driven, and distance presets live in `CompProperties_Squeaker.distancePresets`. Runtime code adapts these fields generically; documentation categories must not become action-name hardcoding where a field can express the behavior.

Configuration merges field-by-field: (1) XML `CompProperties` defaults, (2) global `ModSettings` overrides, then (3) per-Xenotype deltas. Missing preset or field inherits the lower layer. Selecting a distance preset copies its actual range; manual range edits become Custom.

When Biotech is active, maintain one preset per exact `XenotypeDef.defName`; empty or missing preset fields inherit global defaults. The settings presentation uses localized name and Xenotype icon, with `defName` available as technical information. Labels, icons, localization, and discovery remain display/candidate aids, never preset or audio eligibility gates.

The audio-selection schema is a deliberate boundary: legacy audio selections/remix values are not automatically migrated. Do not infer or promise migration for unrelated settings without evidence. Behavior and mood configuration are independent from audio-pack selection.

## 6. VoicePack model and persistence

`SqueakVoicePackDef` is one selection, weight, and validation unit. A Race pack has no target and works without Biotech. A Xenotype pack has exactly one `targetDefName` string. A package may publish several PackDefs, but each remains independent. Stable PackKey identity is package ID plus PackDef defName; Race and Xenotype choices are persisted in separate domains.

Modes are: **Off** = Vanilla only; **Fallback** = Xenotype → Race → Vanilla per action; **Remix** = each currently playable Xenotype, Race, and Vanilla tier receives equal tier weight, then eligible PackDefs in that tier are chosen equally before their sounds. Packs contribute sounds only; they never override action behavior, timing, capability, distance, or mood.

Saved selections whose PackKey disappears are retained as **orphan** choices and excluded from resolution. Xenotype bindings unavailable because Biotech, catalog, or target is absent are **dormant**, not orphaned. Both resume automatically if the same identity returns; players may explicitly forget them. Forgetting an unavailable Xenotype target is a confirmed destructive action that removes both its exact behavior preset and its Xenotype VoicePack selection; catalog refresh never performs that deletion implicitly. Pack targets, persisted bindings, and non-official discovery-only hints form the UI candidate union, not an eligibility whitelist.

## 7. Example audio and implementation paths

The built-in `SR_OfficialExample_Race` and external `SR_ExampleTemplate_Race` are independent Race VoicePacks with distinct Defs, PackKeys, catalog entries, and roots. Neither is privileged by resolver weighting or automatic selection. The Template's Biotech tree is TXT-only guidance, not loadable Xenotype content.

The Template directory is the only manually maintained Example source: `Extras/SqueakyRatkinExampleVoices/1.6/Race/Sounds/coahuilite.squeakyratkin.examplevoices/SR_ExampleTemplate_Race/`. As a current reference baseline it contains 15 action directories and 41 OGG files: Attack 3, Call 4, Death 2, Draft 3, Eat 2, Equip 2, Joy 3, MentalBreak 1, Move 3, Select 3, Sleep 3, Social 3, Undraft 3, Work 3, and Wounded 3. These counts are not a fixed contract: the fixed contract is the 15 runtime action names, while audio-pack totals and per-action counts may vary as Example audio evolves. The source main-mod tree intentionally has no built-in OGG mirror. `scripts/stage-package.ps1` copies this source into the staged built-in root and verifies that the built-in mirrors the Template's actual action/key sets and SHA-256 identity, without asserting fixed totals or per-action counts. Unexpected changes outside that source, a broken Template→built-in mirror relationship or its distribution, altered pack/path roots, extra audio formats, or divergent staged hashes are invariants to report—not silently repair.

Formal Example sources allow OGG only. OGG Vorbis is also the recommended final-distribution format for third-party VoicePacks. WAV is supported by RimWorld/the current loading chain and may be useful for intermediate review, but is not the project's recommended release format; runtime validator or game compatibility must not be represented as a publication recommendation. This policy does not reject third-party WAV solely for being WAV. MP3 is not a recommended guide format.

Authoring resource roots are always `<lowercase packageId>/<PackDef.defName>/<Action>/`. SoundDef `clipFolderPath`, the physical `Sounds/` tree, and installation guidance must agree. RimWorld audio keys are not isolated by package ownership, so the project validates its own distributed roots and duplicate extensions but does not globally arbitrate third-party patches or collisions.

Implementation authority paths: `Source/SqueakyRatkin/CompSqueaker.cs` for the generic component/runtime pipeline; `Source/SqueakyRatkin/Patches/` for bounded event hooks; `1.6/Patches/Ratkin_AddSqueakComp.xml` for action/mood defaults; `Source/SqueakyRatkin/SqueakyRatkinSettings.cs` and adjacent settings UI code for persisted settings; `Source/SqueakyRatkin/SqueakVoicePackModels.cs` and resolver code for pack resolution; `scripts/stage-package.ps1` for staging invariants; and `docs/logging-protocol.md` for the closed logging compatibility surface.
