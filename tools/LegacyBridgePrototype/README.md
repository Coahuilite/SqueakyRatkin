# Legacy Bridge Prototype (0.3.2 scratch)

Offline prototype for the 2026-08-22 legacy-bridge decision (zero migration, kernel stays
legacy-free, removable anytime). It is **not shipped** by Squeaky Ratkin and is not part of the
main mod build.

## Layout

| Piece | What it proves |
| --- | --- |
| `tools/LegacyBridgePrototype/` | **Compile-only proof against the real Verse API** (`Krafs.Rimworld.Ref`): `SqueakyRatkin.SqueakVoicePackDef : UniversalSqueaker.SqueakVoicePackDef` is a field-free thin subclass of a real `Verse.Def` subclass; `DefDatabase<SqueakyRatkin.SqueakVoicePackDef>.AllDefs` satisfies Verse's generic constraint; canonical fields keep the 0.3.1 author XML shape. Krafs reference assemblies cannot be executed (BadImageFormatException is expected), so this project is build-only. |
| `tools/LegacyBridgeHarness/` | **Runtime semantics on pure stubs**, linking the exact same `LegacyVoicePackBridge.cs` / `LegacyVoicePackSource.cs` sources: hierarchy field fill direction, `SR_` vs `US_` prefix context, `AllDefs` enumeration + single upcast point by reference, and source-level removal surface (only the bridge files reference `SqueakyRatkin`). |

## Run

```powershell
dotnet build tools/LegacyBridgePrototype -c Release
dotnet run --project tools/LegacyBridgeHarness -c Release
```

Both must pass. Offline caches may need `--no-restore` after a local restore.

## Still requires a real RimWorld session (maintainer step)

The reference assemblies cannot run Verse's XML loader. Before finalizing the contract, run in
a real game session with only the future US assembly installed (no SR DLL):

1. Load a legacy XML pack that declares `<SqueakyRatkin.SqueakVoicePackDef>` and `SR_` SoundDefs
   with a normal `<sound>` cross reference.
2. Verify `DefDatabase<SqueakyRatkin.SqueakVoicePackDef>.AllDefs` enumerates it and the cross
   reference resolves without red XML errors in Player.log.
3. Verify the upcast path validates under the `SR_` prefix context and that a canonical `US_`
   pack validates under the `US_` context.
4. Verify removal: delete `SqueakyRatkinLegacyVoicePackDef.cs` + `LegacyVoicePackSource.cs` and
   the one catalog call site; legacy packs then stop loading silently (no lingering types).
