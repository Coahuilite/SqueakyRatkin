# About/ — Mod 元数据与发布边界

## Responsibility

`About/` 是 Squeaky Ratkin 的**元数据身份层**：向 RimWorld 的 mod 列表（`ModLister`/`ModMetaData`）提供包身份、支持版本、依赖声明与排序声明，并向商店页提供图标与描述。本地图同时覆盖与元数据同属一个控制面的两个相邻文件——仓库根的 `../LoadFolders.xml`（加载文件夹声明）与 `../.github/workflows/`（CI/发布流水线）——它们是"mod 如何被游戏加载、如何被打包发布"的集成边界。`About/` 不含任何游戏逻辑代码；状态所有者在游戏侧（`ModMetaData`、`ModContentPack.foldersToLoadDescendingOrder`），打包侧（`../scripts/stage-package.ps1`）只做原样复制。

## Key Files / Symbols

- `About.xml` — 唯一正式元数据源（`ModMetaData` 根元素）。
- `Preview.png` / `ModIcon.png` — 商店页图标，打包时原样复制，无代码依赖。
- `../LoadFolders.xml`（仓库根，本地图覆盖其职责）— 按游戏版本声明加载文件夹及其条件。
- `../.github/workflows/ci.yml` / `release.yml` — 构建/发布流水线（本地图覆盖其职责）。
- `../scripts/stage-package.ps1` — About/、LoadFolders.xml 进入发布包的唯一通道；`../scripts/pack-dev.ps1`、`pack-github.ps1`、`pack-steam.ps1` 为三个 flavor 的入口。
- 相关游戏侧符号（RimWorld 源码，行为依据）：`Verse.ModMetaData`、`Verse.ModLoadFolders`、`Verse.LoadFolder.ShouldLoad`、`Verse.ModContentPack.InitLoadFolders`、`Verse.DirectXmlLoader.XmlAssetsInModFolder`。

## Design

### 依赖分层（准确性核心，勿混淆）

| 层 | 模组 | packageId | 声明位置 |
|---|---|---|---|
| 硬依赖（modDependencies，缺失即告警） | Harmony | `brrainz.harmony` | `About.xml` `<modDependencies>` |
| 硬依赖（同上） | Humanoid Alien Races | `erdelf.HumanoidAlienRaces` | `About.xml` `<modDependencies>` |
| 运行时必需、元数据仅软声明 | NewRatkinPlus | `Solaris.RatkinRaceMod` | 仅 `loadAfter`（def 来源），**不在** modDependencies |
| 可选项（零元数据引用） | 全部官方 DLC（含 Biotech）、HugsLib | — | 元数据、XML、C# 中均不出现 |

- `loadAfter`：`brrainz.harmony`、`erdelf.HumanoidAlienRaces`、`Solaris.RatkinRaceMod`。`loadAfter` 只保证加载顺序，**不强制启用**；NewRatkinPlus 的"必需"由 README/架构合同（`../docs/project-architecture-contract.md` §2）和 XML Patch 目标共同承载，而非元数据。
- **No-DLC 契约**（`../AGENTS.md` 强制项）：`../1.6/` 下 XML 无 `requiredDLC`/DLC packageId 引用；C# 侧 Biotech 路径全部经 `ModsConfig.BiotechActive` 短路（如 `HarRatkinXenotypeDiscovery` 返回 `Unavailable`、`SqueakRuntimeResolver` 跳过 Xenotype 层、设置域返回 `Dormant`）。HugsLib 在全仓库零引用。**任何把 DLC/HugsLib 加入 About.xml 的改动都违反契约。**
- `supportedVersions` 仅 `<li>1.6</li>`；单一版本结构，无版本化 About 分支。
- `packageId`：`coahuilite.squeakyratkin`（大小写敏感，Extras 内嵌包依赖此 ID）。

### LoadFolders.xml 语义（对照 RimWorld 源码验证）

```xml
<loadFolders><v1.6>
  <li>/</li>
  <li>1.6</li>
</v1.6></loadFolders>
```

- `/` 与 `1.6` 无条件加载（不 gated 任何 packageId）。依赖语义全在 `About.xml`：Harmony/HAR 为 `modDependencies` 硬依赖，NewRatkinPlus 仅 `loadAfter` 软声明。
- 内容门是 XPath 而非包名：`../1.6/Patches/Ratkin_AddSqueakComp.xml` 的 XPath 目标 `AlienRace.ThingDef_AlienRace[defName="Ratkin"]` 在缺该 def 时**静默 no-op**（comp 不注入，DLL/Defs 仍加载）。因此保留 `defName="Ratkin"` 的 NewRatkinPlus fork（改 packageId）也能工作。
- 历史教训：旧版曾用 `IfModActive="Solaris.RatkinRaceMod"` 门控 `/` 与 `1.6`。RimWorld 源码（`ModContentPack.InitLoadFolders`）在存在匹配版本块且 `list.Count>0` 时直接 `AddFolders(list); return;`，不再回退默认文件夹；`AddFolders` 只加载 `LoadFolder.ShouldLoad==true` 的文件夹。故官方 packageId 不在时 `1.6/`（DLL + Defs + Patches + Languages）整体不加载，门控是事实硬开关，已移除。
- 加载顺序：`1.6` 优先于 `/`（`AddFolders` 逆序入列、`XmlAssetsInModFolder` 按相对路径去重时先到先得）。
- `"/"` 条目当前不贡献可加载内容：仓库根没有 `Defs/`、`Patches/` 等内容文件夹（全部内容在 `1.6/` 下）；内嵌的 `../Extras/SqueakyRatkinExampleVoices`（自带 About.xml 的独立包）**不会被自动发现/加载**——`ModLister` 只扫 `Mods/` 顶层目录，用户须复制该目录到 `Mods/` 单独启用（其 About.xml 声明依赖本 mod + NewRatkinPlus）。

### 打包与流水线

- `stage-package.ps1`：复制 `About/`、`LoadFolders.xml`、`1.6/`、`Extras/SqueakyRatkinExampleVoices` 到 `dist/<flavor>/SqueakyRatkin/`（剥离 `PublishedFileId.txt`、`*.pdb`、`*.gitkeep`），并校验 Template/内置音频的实际键集合与 SHA256 镜像一致。
- `../.github/workflows/ci.yml`：push（main/master/dev）+ PR → windows-latest + .NET 8 → `dotnet build -p:SqueakyBuildFlavor=Dev`；push 额外 `pack-dev.ps1` + zip + 上传 artifact（14 天保留）。PR 只构建不打包。
- `../.github/workflows/release.yml`：`v*` tag → 校验（严格 SemVer、`csproj <Version>` 与 tag 基版本一致、tag 提交必须是 `origin/main` 祖先）→ `SqueakyBuildFlavor=GitHub` + `SqueakyInformationalVersion=vX.Y.Z+sha` → `pack-github.ps1 -Version` → `softprops/action-gh-release`（tag 含 `-` 自动标 prerelease）。Steam flavor 无流水线，走本地 `pack-steam.ps1`。

## Data & Control Flow

```
游戏启动:
  ModLister 扫描 Mods/ 顶层 → 读 About/About.xml(ModMetaData)
    → 依赖校验(modDependencies 缺失告警) + 排序(loadAfter)
    → ModContentPack.InitLoadFolders 解析 LoadFolders.xml → foldersToLoadDescendingOrder
    → 从各加载文件夹的 Defs/、Patches/、Assemblies/、Sounds/、Languages/ 载入
    → XML Patch(Ratkin_AddSqueakComp.xml) + Harmony PatchAll(DLL) 生效

发布:
  CI/release.yml 或本地 pack-*.ps1 → dotnet build(SqueakyBuildFlavor 决定 SQUEAKY_* 常量)
    → stage-package.ps1(About/ + LoadFolders.xml 原样进包, OGG 校验)
    → dist/<flavor>/SqueakyRatkin/ → zip / 直接上传 Steam
```

调用方向：游戏读 `About/` 与 `LoadFolders.xml`（只读）；打包脚本复制二者（只读）；workflow 只驱动构建与打包，**不修改任何元数据文件**。元数据变更通过提交进仓库，由下次打包自然带入 dist。

## Integration

- 与 `../1.6/`：`1.6/Assemblies/SqueakyRatkin.dll` 由 `../Source/SqueakyRatkin/SqueakyRatkin.csproj` 直接输出（`OutputPath=..\..\1.6\Assemblies`）；`1.6/Defs`、`1.6/Patches` 经 LoadFolders 装载；`1.6/Languages` 由语言系统装载。
- 与 `../Source/`：csproj 引用 `Krafs.Rimworld.Ref 1.6.*` 与 `Lib.Harmony 2.4.*`（`ExcludeAssets=runtime`，运行时 DLL 来自游戏目录的 Harmony 本体）；**无 HAR/DLC 程序集引用**——HAR 交互走 XML def（`AlienRace.ThingDef_AlienRace`）与运行时类型/def 检查，Biotech 走 `ModsConfig.BiotechActive`。
- 与 `../Extras/`：随包分发、默认不加载（见上）；其依赖方向为 Extras → 本 mod。
- 兼容边界：支持版本仅 1.6；`About.xml` 与 `LoadFolders.xml` 的改动必须在下次打包时同步进 dist（无手工维护 dist 的必要，脚本每次重建）。

## Change Guidance

- **新增硬依赖**：仅当缺少该 mod 时本 mod 无法运行才加入 `modDependencies`；只影响顺序的用 `loadAfter`；**DLC/HugsLib 永远不得进入 About.xml**（No-DLC 契约，见 `../AGENTS.md`）。
- **想让"无 NewRatkinPlus 就不加载"成为现实**：不要在 `LoadFolders.xml` 用 `IfModActive` 门控本体（那会变成事实硬开关，且使改 packageId 的 fork 失效）；需改 XML Patch 为条件式（或接受现状：无条件加载、无 Ratkin def 时 XPath 静默 no-op）。改动前先与 `../1.6/` 地图对齐。
- **新增游戏版本（如 2.0）**：`About.xml` `supportedVersions` 加条目；`LoadFolders.xml` 加 `<v2.0>` 块；`stage-package.ps1` 同步纳入复制列表（参考 Extras 包自己的 LoadFolders 模式）。
- **改版本号**：`csproj <Version>` 与 git tag 基版本必须一致（release.yml 强制校验），两处同步改。
- **改图标/描述**：直接改 `About/` 文件后重跑 `pack-*`；CI 不校验图标内容，但 `stage-package.ps1` 会校验 OGG 契约，音频变更需先走 Template 源。

---

### 供根地图汇总（职责摘要，勿复制正文）

- **About/** — mod 元数据与商店身份：packageId `coahuilite.squeakyratkin`、仅支持 1.6；依赖分层 = Harmony/HAR 硬依赖（modDependencies）、NewRatkinPlus 仅 loadAfter、DLC/HugsLib 零引用（No-DLC 契约）。
- **LoadFolders.xml（根）** — 无条件加载 `[1.6, /]`（1.6 优先）；不 gated 任何 packageId，依赖语义在 About.xml，发声注入按 XPath `defName="Ratkin"` 匹配（缺 def 时静默 no-op）。
- **.github/workflows/** — CI（Dev flavor 构建 + dev 包 artifact）与 Release（v* tag 三重校验 → GitHub flavor → zip → GitHub Release）；不修改元数据，打包经 `scripts/stage-package.ps1`。
