# 鼠辈啁啾 · Squeaky Ratkin

[English](./README.md) | **中文**

鼠辈啁啾为 NewRatkinPlus 鼠族 pawn 加入可选的一次性啁啾声。仓库已实现获接受的 0.2.0 功能范围，产品版本现为 **0.2.0**；本文不是发布声明。

## 依赖与 No-DLC 基线

- RimWorld 1.6、Harmony、HAR、NewRatkinPlus（`Solaris.RatkinRaceMod`）。
- Core 加上述依赖是强制基线；所有官方 DLC（含 Biotech）均为可选。无 Biotech 时，全局设置、适用动作、心情调制、Race 包与原版回退仍可用。

将已发布的 Steam/Release 包安装到 `RimWorld/Mods/` 并启用依赖。运行时边界见 [`docs/project-architecture-contract.md`](./docs/project-architecture-contract.md)。

## 音频与 VoicePack

固定 15 个动作是 `Call`、`Eat`、`Sleep`、`Wounded`、`Select`、`Move`、`Social`、`Joy`、`Death`、`Draft`、`Undraft`、`Attack`、`Work`、`Equip`、`MentalBreak`。

音频使用玩家主动选择的独立 VoicePack：**Off** 仅原版；**Fallback** 按 Xenotype → Race → 原版回退；**Remix** 让当前可播放的 Xenotype、Race、原版 tier 等权。Xenotype 目标只能是精确且区分大小写的 `XenotypeDef.defName`，并且是可选增量。

主包内置普通 Race-only `SR_OfficialExample_Race`：15 个 SoundDef、41 个 OGG，分布为 Attack 3、Call 4、Death 2、Draft 3、Eat 2、Equip 2、Joy 3、MentalBreak 1、Move 3、Select 3、Sleep 3、Social 3、Undraft 3、Work 3、Wounded 3。它 No-DLC 可用，不自动选择，也没有特殊权重。`Extras/SqueakyRatkinExampleVoices/` 是可直接启用的独立 Race-only Template，拥有自己的 package ID、PackDef、Catalog 身份和资源根。Template 是 Example 音频的唯一维护源，staging 将其镜像到内置 Example。

Example 音频是 MPL-2.0 代码许可证之外的公共领域素材。项目与贡献者不对其主张版权或相关权利；可使用、复制、修改和再分发。权利状态及有限的来源/法域免责声明见 [`AUDIO_RIGHTS.txt`](./Extras/SqueakyRatkinExampleVoices/AUDIO_RIGHTS.txt)。请从 [`docs/voice-pack-author-guide-zh.md`](./docs/voice-pack-author-guide-zh.md) 和 [`Extras/SqueakyRatkinExampleVoices/README.md`](./Extras/SqueakyRatkinExampleVoices/README.md) 开始；自定义音频必须是独立 VoicePack，绝不安装进主模目录。

## 设置与诊断

设置即时生效，使用合并保存并在关窗时 flush。普通状态有三页；连续点击版本七次后解锁“开发与诊断”第四页。UI/capability 合同见 [`docs/settings-ui-product-contract-zh.md`](./docs/settings-ui-product-contract-zh.md)。详细诊断日志独立于 RimWorld 开发者模式；稳定的人类/机器日志协议见 [`docs/logging-protocol.md`](./docs/logging-protocol.md)。

## 开发、打包与版本

唯一人工维护的产品版本是 `Source/SqueakyRatkin/SqueakyRatkin.csproj` 的 `<Version>`（当前 0.2.0）。构建不会安装到 RimWorld。

```powershell
dotnet build Source/SqueakyRatkin/SqueakyRatkin.csproj -c Release -p:SqueakyBuildFlavor=Dev
pwsh scripts/pack-dev.ps1
```

它会生成 `dist/dev/SqueakyRatkin/`，供开发者手动安装测试。打包脚本只 stage 已有构建，不会编译：Dev 用于本地测试，Steam 用于创意工坊 staging，GitHub 发布包由 tag/release CI 流程生成。维护者发布规则见 [`AGENTS.md`](./AGENTS.md)。标准构建验证：

```text
dotnet build Source/SqueakyRatkin/SqueakyRatkin.csproj
```

代码采用 [MPL-2.0](./LICENSE)。原版资产只通过 Def/path 引用，绝不重新分发。贡献说明见 [`CONTRIBUTING.md`](./CONTRIBUTING.md)。
