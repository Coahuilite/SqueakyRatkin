# Squeaky Ratkin · 鼠辈啁啾

**English** | [中文](./README.zh-CN.md)

Squeaky Ratkin adds optional one-shot squeaks to NewRatkinPlus Ratkin pawns. The current repository implements the accepted 0.2.0 feature set plus the 0.2.1 bug-fix release, and the product version is **0.2.1**; this README is not a release announcement.

## Requirements and No-DLC baseline

- RimWorld 1.6, Harmony, HAR, and NewRatkinPlus (`Solaris.RatkinRaceMod`).
- Core + those dependencies is the required baseline; all official DLC, including Biotech, are optional. Without Biotech, global settings, applicable actions, mood modulation, Race packs, and Vanilla fallback still work.

Install a published Steam/Release package into `RimWorld/Mods/` and enable its dependencies. See [`docs/project-architecture-contract.md`](./docs/project-architecture-contract.md) for runtime boundaries.

## Audio and VoicePacks

The fixed 15 actions are `Call`, `Eat`, `Sleep`, `Wounded`, `Select`, `Move`, `Social`, `Joy`, `Death`, `Draft`, `Undraft`, `Attack`, `Work`, `Equip`, and `MentalBreak`.

Audio uses opt-in independent VoicePacks: **Off** plays Vanilla only; **Fallback** resolves Xenotype → Race → Vanilla; **Remix** weights currently playable Xenotype, Race, and Vanilla tiers equally. A Xenotype target is the exact, case-sensitive `XenotypeDef.defName` and is optional.

The main package includes the ordinary Race-only `SR_OfficialExample_Race`; as a current reference baseline it has 15 SoundDefs and 41 OGG clips: Attack 3, Call 4, Death 2, Draft 3, Eat 2, Equip 2, Joy 3, MentalBreak 1, Move 3, Select 3, Sleep 3, Social 3, Undraft 3, Work 3, and Wounded 3. These counts are a current Example baseline rather than a fixed contract and may vary in future releases. It is No-DLC. Since 0.2.3 it is enabled by default on fresh installs (pristine-default old configs migrate once), shares the same weighting rules as third-party packs, and can be disabled in settings at any time. `Extras/SqueakyRatkinExampleVoices/` is a separate directly enableable Race-only Template with its own package ID, PackDef, catalog identity, and resource root. The Template is the only maintained Example audio source; staging mirrors it into the built-in Example.

The Example clips are public-domain material outside the MPL-2.0 code license. The project and contributors claim no copyright or related rights in them; they may be used, copied, modified, and redistributed. See [`AUDIO_RIGHTS.txt`](./Extras/SqueakyRatkinExampleVoices/AUDIO_RIGHTS.txt) for the limited provenance/status disclaimer. Start with [`docs/voice-pack-author-guide-zh.md`](./docs/voice-pack-author-guide-zh.md) and [`Extras/SqueakyRatkinExampleVoices/README.md`](./Extras/SqueakyRatkinExampleVoices/README.md); custom audio is an independent VoicePack, never an installation into the main mod.

## Settings and diagnostics

Settings are immediate with a coalesced save and close flush. There are three regular pages plus a seven-click-unlocked Developer & Diagnostics page. The UI and capability contract is [`docs/settings-ui-product-contract-zh.md`](./docs/settings-ui-product-contract-zh.md). Detailed diagnostic logging is independent of RimWorld Dev Mode; its stable machine/human protocol is [`docs/logging-protocol.md`](./docs/logging-protocol.md).

## Development, packaging, and versioning

The only manually maintained product version is `<Version>` in `Source/SqueakyRatkin/SqueakyRatkin.csproj` (currently 0.2.1). Builds do not install into RimWorld.

```powershell
dotnet build Source/SqueakyRatkin/SqueakyRatkin.csproj -c Release -p:SqueakyBuildFlavor=Dev
pwsh scripts/pack-dev.ps1
```

This stages `dist/dev/SqueakyRatkin/` for manual local testing. Packaging scripts stage existing builds and do not compile: Dev is for local testing, Steam is for Workshop staging, and GitHub release packages are produced by the tag/release CI path. Maintainer release rules are in [`AGENTS.md`](./AGENTS.md). Standard verification is one command:

```powershell
pwsh scripts/verify-local.ps1
```

It runs the three characterization harnesses (kernel purity gate + 43 asserts + 3782-case corpus replay, settings fixtures byte-stable, log protocol v1 in both flavors), the fixtures zero-delta gate, and Dev + Steam flavor builds with warnings-as-errors; add `-PackDev` or `-PackSteam` to also stage the package (Steam requires a clean working tree). Bare build:

```text
dotnet build Source/SqueakyRatkin/SqueakyRatkin.csproj
```


Code is licensed under [MPL-2.0](./LICENSE). Vanilla assets are referenced only by Def/path and are never redistributed. Contributions: [`CONTRIBUTING.md`](./CONTRIBUTING.md).
