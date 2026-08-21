# Logging Protocol

> **Status — 2026-07-21:** This document records the implemented `SqueakLog` v1 contract from source review of `Logging/SqueakLog.cs`, `SqueakyRatkinSettings.cs`, and `Debug/SqueakDebug.cs`. It is a compatibility and machine-parsing contract, not evidence of a live RimWorld-session test.

## Closed Typed Facade

`SqueakLog` is a closed typed facade. Its internal event registry fixes each event ID, visibility, Verse level, English human sentence, and accepted typed data schema. Business code invokes event-specific facade methods and cannot submit a free-form event ID, human sentence, visibility, level, or payload schema. These registry values, and the `srdiag fmt=1` ordering below, are compatibility surfaces.

Every emitted record begins `[SqueakyRatkin] ` followed by its fixed English human sentence. Human text is never localized. A Daily record emits the sentence alone while detailed diagnostics are ineffective, or appends ` || srdiag fmt=1 ...` on the same line while they are effective. A DevOnly record emits nothing while ineffective, or the same human-first shape while effective.

Visibility and level are independent. `Daily` and `DevOnly` govern eligibility; `Info`, `Warning`, and `Error` choose the Verse sink. Warning remains a Verse warning and Error remains a Verse red error. The facade's exact-once behavior applies equally to Info, Warning, and Error; it does not change the selected sink.

## Detailed-Logging Mode and Session State

`SqueakDevLoggingMode` has three persisted values:

| Mode | Effective detailed logging |
| --- | --- |
| `Auto` | On in Dev builds; off in GitHub and Steam builds. |
| `Enabled` | On in every build flavor. |
| `Disabled` | Off in every build flavor. |

The mode is independent of `Prefs.DevMode` and `developerToolsEnabled` (the seven-click troubleshooting unlock). The unlock exposes the setting only. The four mode-result events distinguish explicit Enabled/Disabled from Auto resolving to enabled/disabled. Their Daily human result is emitted independently, including when a mode change turns detailed diagnostics off.

DevOnly facade methods guard on `ShouldEmitDev`; callers must also use that gate before they construct diagnostic strings or fields. Thus disabled detailed logging returns before call-site diagnostic construction and before DevOnly payload formatting.

`SqueakyRatkinSettings.SetDevLoggingMode` compares effective state before and after configuration. If it changed, `SqueakDebug.ResetLoggingSession` clears audio sampling, resets the summary timer, and clears the once-key session registry. After detailed logging is re-enabled, the first successful dispatch for each action can therefore produce detail immediately. No map-lifecycle reset is currently implemented.

## `srdiag` v1 Machine Contract

The suffix is one line and begins exactly `srdiag fmt=1`. Its mandatory core fields appear once, in this exact order:

```
fmt lvl vis evt action target pack build build_id
```

Event-specific fields follow when present, in this fixed formatter order:

```
reason sound source count dispatched suppressed_detail enabled
ex_type ex_inner ex_site ex_msg
```

Values use invariant-culture formatting. Missing values and the literal `N/A` are `-`. Strings are UTF-8 percent-encoded after sanitization, except for `A-Z`, `a-z`, `0-9`, `.`, `_`, `~`, `:`, `/`, `@`, `+`, and `-`; parsers split key/value tokens and then percent-decode values. Boolean values are lowercase `true` and `false`. Consumers must tolerate absent event-specific fields and treat unknown future fields as extensions.

Do not put localized text, pawn labels, filesystem paths, or raw `Exception.ToString()` into the protocol. Exception information is suffix-only: type, inner type, target site, and sanitized message. Sanitization first replaces CR/LF and removes remaining control characters, replaces DOS drive paths, UNC paths, device paths, `file:` URIs, Unix absolute paths, and relative `./` or `../` paths with `<path>`, then truncates to 256 characters. Percent encoding occurs after this sanitization.

## `srdiag` v2 Machine Contract (0.3.1)

The v1 contract above is unchanged and remains the emission format of the original 28-event registry. v2 is a versioned extension of the same one-line suffix: records that carry v2-only facts (settings origin, race/xenotype identity, route tier) begin `srdiag fmt=2` and use the fixed v2 field order below. v1 parsers keep accepting fmt=1 records; **no v1 event ever gains v2 fields** (the closed typed facade cannot express them — v2 fields are not stuffed into fmt=1 records).

The v2 mandatory core fields appear once, in this exact order:

```
fmt lvl vis evt action target pack race [xenotype] build build_id
```

- `action` is the string action key (0.3.1 定型, §2.2 of the architecture decision): built-in = enum name (byte-identical to the v1 action value), external = `packageId.defName`. The percent-encoding whitelist already includes `.`, so external keys are written verbatim.
- `race` is the exact, case-sensitive `ThingDef.defName` of the routing domain; missing is `-`. `xenotype` is present only when the domain has one and is the exact `XenotypeDef.defName`. Only DefNames are written — never labels, HAR package names, or player-mutable text.
- `settings_origin`, `sound`, and `tier` are event-specific fields appended after the core in the fixed per-event order below. Encoding, sanitization, truncation, exception metadata, and DevOnly gating are identical to v1.

Event-specific fields:

| Event | Fields (fixed order) |
| --- | --- |
| `settings.origin` | `settings_origin=FreshCreated\|LoadedFromFile` |
| `audio.route.selected` | `sound=<SoundDef.defName>` `tier=<tier>` |

`settings.origin` is Daily Info, emitted once per session at startup, right after the settings object is read. **Origin detection:** `LoadedFromFile` = a settings file was successfully deserialized through Scribe (ExposeData reached LoadingVars); `FreshCreated` = no file, or an unreadable file — the framework discards the broken parse, warns, and returns a new field-defaults instance, which is reported as FreshCreated. The sentence is parameterized by the closed two-value origin set (precedent: `mod.start.identity` parameterizes by build/build_id).

`audio.route.selected` is DevOnly Info and follows the same success-path volume control as `audio.dispatch.ok` (one record per action per five seconds, emitted alongside it). `tier` vocabulary in 0.3.1: `xenotype_pack`, `race_pack`, `vanilla`, `-` for none; `pack_fallback`/`built_in_fallback` are reserved for the 0.3.2 pack-fallback chain end.

Once keys are namespaced per protocol version: fmt=1 records claim the `log-v1` domain and fmt=2 records the `log-v2` domain, so the two never collide.

## Once and Success-Path Volume Control

The registry's exact-once key comprises the event plus action, target, pack, reason, and exception type, namespaced per protocol version (`log-v1` for fmt=1 records, `log-v2` for fmt=2 records). It is claimed under a lock, making duplicate suppression thread-safe. The session retains at most 1024 keys; when a new claim reaches that limit, the registry clears and accepts the new key. A logging-session reset also clears it.

When detailed logging is effective, successful audio dispatches emit `audio.dispatch.ok` immediately for the first success of each action. Further detail for that action is limited to one record per five seconds; suppressed detail is counted. `trigger.outcome.summary` aggregates dispatched and suppressed-detail counts at most once per 60 seconds. This rate control does not alter operational warning/error visibility or severity.

## Stable Event Registry

The table below is the actual closed registry. The human-sentence column reproduces the fixed registry sentence; the startup identity sentence contains the runtime build and build ID.

| Event ID | Visibility | Level | Fixed English human sentence |
| --- | --- | --- | --- |
| `mod.start.identity` | Daily | Info | `Squeaky Ratkin started with {build} build {build_id}.` |
| `mod.start.ready` | Daily | Info | `Squeaky Ratkin startup completed.` |
| `logging.mode.enabled` | Daily | Info | `Detailed diagnostic logging is enabled.` |
| `logging.mode.disabled` | Daily | Info | `Detailed diagnostic logging is disabled.` |
| `logging.mode.auto_enabled` | Daily | Info | `Detailed diagnostic logging is enabled by Auto mode.` |
| `logging.mode.auto_disabled` | Daily | Info | `Detailed diagnostic logging is disabled by Auto mode.` |
| `settings.open.api_unavailable` | Daily | Warning | `Mod Settings API is unavailable.` |
| `settings.open.failed` | Daily | Warning | `Mod Settings could not be opened.` |
| `voicepack.catalog.refresh_failed` | Daily | Error | `VoicePack catalog refresh failed.` |
| `voicepack.pack.rejected` | Daily | Warning | `A VoicePack was rejected.` |
| `voicepack.resolver.rebuild_failed` | Daily | Error | `VoicePack resolver rebuild failed.` |
| `voicepack.target.rejected` | Daily | Warning | `A Xenotype VoicePack target was rejected.` |
| `xenotype.discovery.unavailable` | DevOnly | Warning | `Xenotype discovery is unavailable.` |
| `xenotype.discovery.failed` | DevOnly | Warning | `Xenotype display discovery failed.` |
| `xenotype.discovery.candidate` | DevOnly | Info | `A HAR Xenotype discovery candidate was evaluated.` |
| `trigger.attempt.failed` | Daily | Error | `Squeak trigger attempt failed.` |
| `audio.dispatch.no_sound` | Daily | Warning | `No fallback SoundDef was found.` |
| `audio.dispatch.failed` | Daily | Error | `Squeak audio dispatch failed.` |
| `audio.dispatch.ok` | DevOnly | Info | `Squeak audio dispatched.` |
| `trigger.outcome.summary` | DevOnly | Info | `Squeak trigger outcome summary was recorded.` |
| `hook.attack.unavailable` | Daily | Error | `Attack squeak hook is unavailable.` |
| `hook.attack.target_skipped` | DevOnly | Warning | `An Attack hook target was skipped.` |
| `hook.mental_break.unavailable` | Daily | Error | `Mental-break squeak hook is unavailable.` |
| `diagnostics.hook.unavailable` | DevOnly | Warning | `Diagnostics overlay hook is unavailable.` |
| `diagnostics.start.failed` | Daily | Warning | `Diagnostics overlay could not start.` |
| `devtools.overlay.changed` | DevOnly | Info | `Diagnostics overlay state changed.` |
| `devtools.camera_indicator.changed` | DevOnly | Info | `Camera indicator state changed.` |
| `devtools.workbench.open_failed` | Daily | Warning | `Animal Voice Workbench could not be opened.` |
| `settings.origin` | Daily | Info | `Mod settings origin: <FreshCreated\|LoadedFromFile>.` |
| `audio.route.selected` | DevOnly | Info | `Squeak audio route was selected.` |

The last two rows are the 0.3.1 v2 extension records (fmt=2); the 28 rows above them are the locked v1 surface (fmt=1) and remain byte-immutable.

`voicepack.pack.rejected` uses the pack key as `pack`. `reason` carries `duplicate_key` (default, same key loaded more than once) or `domain_filtered` (0.3.1 race-aware: `raceDefName` outside the product domain whitelist), with `count` = the number of duplicate instances for the former.

`xenotype.discovery.candidate` uses the exact Xenotype `defName` as `target`. Since 0.3.1 the target union is an assembled-only projection: visible candidates carry the deterministic `+`-joined source set of (`declared_pack`, `selection`, `preset`) with `enabled=true`; HAR discovery is dev diagnostics only and never projects rows, so a HAR-only target that is not retained emits `reason=har_hint_filtered` or `reason=har_official_filtered` (mirrored into `source`) with `enabled=false`. Mirroring the source into `reason` is intentional because the v1 once key includes `reason`; the same target can therefore emit one record for each materially different source set without changing the once-key contract.
