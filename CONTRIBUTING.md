# 贡献指南 · Contributing

This project accepts code and audio contributions. Current contracts are [`docs/project-architecture-contract.md`](./docs/project-architecture-contract.md), [`docs/settings-ui-product-contract-zh.md`](./docs/settings-ui-product-contract-zh.md), and [`docs/voice-pack-author-guide-zh.md`](./docs/voice-pack-author-guide-zh.md).

## Audio / 音频

The official Example has exactly one manually maintained source:
`Extras/SqueakyRatkinExampleVoices/1.6/Race/Sounds/coahuilite.squeakyratkin.examplevoices/SR_ExampleTemplate_Race/`.
Maintain its current 15-action/41-OGG reference baseline there only: Attack 3, Call 4, Death 2, Draft 3, Eat 2, Equip 2, Joy 3, MentalBreak 1, Move 3, Select 3, Sleep 3, Social 3, Undraft 3, Work 3, and Wounded 3. These counts are a current Example reference, not a fixed contract—only the 15 runtime actions are fixed, and pack totals or per-action counts may vary. `scripts/stage-package.ps1` generates and verifies the built-in mirror. Do not submit a second main-mod audio source.

For a third-party pack, copy `Extras/SqueakyRatkinExampleVoices/`, replace its package identity, PackDefs, SoundDefs, metadata, and audio rights, then follow the author guide. Use `<lowercase packageId>/<PackDef.defName>/<Action>/` for every loaded audio root. The Template's Xenotype directory is TXT-only guidance; it is not an audio placeholder or a loadable pack. Never install or submit third-party custom audio inside the main mod directory.

The Example clips are public-domain material: the project and its contributors claim no copyright or related rights in them, and they may be used, copied, modified, and redistributed. See `AUDIO_RIGHTS.txt`; it does not warrant provenance or status in every jurisdiction. Contributors must provide audio they have the right to publish. Use mono OGG for the official Example (22050 Hz); a third-party pack may use supported real audio formats as documented by the guide.

## Code / 代码

1. Fork and clone the repository; branch from `dev`.
2. Keep the No-DLC Core baseline, fixed 15 actions, XML-driven behavior, and VoicePack boundaries intact.
3. Use `SR_` for every Def; C# classes use namespace isolation. Localize player-facing strings in English and Simplified Chinese.
4. Verify the build:

   ```text
   dotnet build Source/SqueakyRatkin/SqueakyRatkin.csproj
   ```

5. Submit a PR to `dev` with the change and rationale. Do not bundle vanilla assets; reference them only by Def/path.

## Local test package / 本地测试包

Build the Dev flavor, then stage only—builds never deploy to RimWorld:

```powershell
dotnet build Source/SqueakyRatkin/SqueakyRatkin.csproj -c Release -p:SqueakyBuildFlavor=Dev
pwsh scripts/pack-dev.ps1
```

Manually install/copy `dist/dev/SqueakyRatkin/` to test. The maintained scripts are `stage-package.ps1`, `pack-dev.ps1`, `pack-steam.ps1`, and `pack-github.ps1`; pack scripts do not compile. `<Version>` in the csproj is the single manually maintained product version.

Code contributions are MPL-2.0. Historical files marked `DEPRECATED` are archaeology, not implementation guidance.
