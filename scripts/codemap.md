# scripts/

构建与打包流水线：把 `Source/` 编译产物与 `About/`、`1.6/`、`Extras/` 内容组装成 Dev / GitHub / Steam 三种 flavor 的可发布 mod 包（`dist/` 下，gitignored）。

## Responsibility

- **编译（compile）与打包（pack）是两步独立动作**：
  - compile = `dotnet build Source/SqueakyRatkin/SqueakyRatkin.csproj -c Release [-p:SqueakyBuildFlavor=...]`，唯一输出是 `1.6/Assemblies/SqueakyRatkin.dll`（编译产物，gitignored，目录内仅有 `.gitkeep` 提交）。
  - pack = 本目录 PowerShell 脚本，**前提是 DLL 已存在**；`stage-package.ps1` 在缺 DLL 时直接 throw（"Build the desired flavor before staging."），保证 pack 永远吃到先编译的 flavor。
- 四个脚本分工：`stage-package.ps1` 是唯一 staging 引擎；`pack-dev.ps1` / `pack-github.ps1` / `pack-steam.ps1` 是薄包装，各自只负责 flavor 专属的 identity 与输出形态。
- 守护内容契约：Example 音频只能使用已知运行时 action 目录、非空且 OGG key 唯一；Template 与 built-in 镜像必须逐 key SHA256 相同；音频总数和每动作数量仅为当前参考值，不是门禁。GitHub/Dev 包里**不得出现** Steam `PublishedFileId.txt`。

## Key Files & Symbols

| 文件 | 关键符号 / 职责 |
|---|---|
| `stage-package.ps1` | `Assert-FormalExampleAudio`（非空 OGG、已知 action、key 唯一校验）、`Assert-ExampleAudioMirrors`（实际镜像键集合 + SHA256 校验）；`$aboutSource`、`$versionedSource`（`1.6/`）、`$extrasSource`、`$templateAudio`、`$builtInSourceAudio`（仓库内 built-in 源，必须不存在） |
| `pack-dev.ps1` | git `rev-parse --short HEAD` + `status --porcelain` → `-dirty` 后缀；dev label 文件 `dist/dev/SqueakyRatkin-dev-v<版本>-<短sha>[-dirty].txt` |
| `pack-github.ps1` | `-Version` 参数必须匹配严格 SemVer 2.0 tag 正则；产出 `dist/github/SqueakyRatkin-<Version>.zip` |
| `pack-steam.ps1` | 要求 csproj **恰好一个**非空 `<Version>`；**干净工作树硬门**（Steam = 最终发布步，脏树直接 throw）；产出未压缩目录 `dist/steam/SqueakyRatkin`（上传人工，非 SteamCMD） |
| `build-dev.ps1` | 一步 Dev 包：`dotnet build`（默认 Dev flavor）→ `pack-dev`；flavor 由构造保证，pack 永远吃到同 flavor DLL |
| `build-steam.ps1` | 一步 Steam 包：`dotnet build -p:SqueakyBuildFlavor=Steam` → `pack-steam`（含干净树硬门 + 版本断言） |
| `verify-local.ps1` | 一次输入本地自动校验（11 项）：四 harness + `fixtures/` 零 delta 门 + Dev/Steam 双 flavor 0 warning 构建 + ConfigCopy store 生命周期 + VoicePack XML ABI 一致性锁 + VoicePack 脚手架自检（fail-fast + 单检查重跑命令）；`-PackDev`/`-PackSteam` 追加打包（Steam 走干净树硬门）；`-NoRestore` 供离线缓存环境（默认仍常规 restore） |
| `verify-voicepack-xml-abi.ps1` | 0.3.2 第 10 项：示例 XML × validator × 作者指南三向对照；C# `SqueakActionDefinitions` 17 键顺序为源，校验两份提交态 XML（官方内置/Extras 模板）的结构、SR_ 前缀、SoundDef 契约与交叉引用；`dist/SqueakyRatkinEggTestVoices/` 存在时并入校验 `IsEgg`；「作者指南」标记源 = `.github/skills/squeaky-voicepack-authoring/SKILL.md` |
| `verify-voicepack-scaffold.ps1` | 0.3.2 第 11 项：临时目录运行 `new-voicepack.ps1` 生成 Call,Select 最小包，校验目录/XML 结构后清理；只写系统临时目录 |
| `new-voicepack.ps1` | 0.3.2 作者脚手架（SKILL 配套）：`-PackageId -PackDefName [-Actions Call,Select] [-RaceDefName Ratkin] [-OutDir]` 生成 Race-only 包骨架（About/LoadFolders/最小 PackDef+SoundDef XML/音频占位目录/README）；校验 17 内置键与 SR_/小写 packageId 规则；目标已存在拒绝覆盖 |
| `Source/SqueakyRatkin/SqueakyRatkin.csproj` | `<Version>`（identity 唯一来源）、`<SqueakyBuildFlavor>`（默认 `Dev` → `SQUEAKY_<FLAVOR>` DefineConstants）、`<SqueakyInformationalVersion>`（覆盖运行时 informational）、`<OutputPath>..\..\1.6\Assemblies</OutputPath>`、Release 下 `DebugType=none` |
| `LoadFolders.xml`（根） | 主 mod 挂载：无条件加载 `/` 与 `1.6`；发声内容是否命中由 XML Patch 的 `defName="Ratkin"` XPath 决定 |
| `Extras/SqueakyRatkinExampleVoices/LoadFolders.xml` | Extras 独立 mod：`v1.6` 下挂 `1.6/Race` |
| `.github/workflows/ci.yml` | Dev flavor 构建 + dev snapshot 打包 + artifact 上传 |
| `.github/workflows/release.yml` | tag 校验 → GitHub flavor 构建 → `pack-github.ps1` → GitHub Release |

## Design

### Build flavor 矩阵（csproj → DefineConstants → 运行时）

`SqueakyBuildFlavor` 缺省为 `Dev`；`DefineConstants` 追加 `SQUEAKY_<FLAVOR.ToUpper()>`（`SQUEAKY_DEV` / `SQUEAKY_GITHUB` / `SQUEAKY_STEAM`）。运行时消费者：

- `Source/SqueakyRatkin/Mod.cs`：`#if SQUEAKY_STEAM/GITHUB/#else` 选 `BuildFlavor` 字符串（"steam"/"github"/"dev"）；`BuildIdentity()` 在 `SQUEAKY_DEV` 下把 informational 中 `+` 后的 revision 截断为 12 字符，格式化为 `dev-<sha12> (<完整 informational>)`。
- `Source/SqueakyRatkin/Logging/SqueakLog.cs`：`SQUEAKY_DEV` 时 `EffectiveDevLogging` 默认开启；`SQUEAKY_STEAM` 时 `buildId = informational.Split('+')[0]`（纯版本号），`SQUEAKY_GITHUB` 时 `buildId = 完整 informational`（`vX.Y.Z+sha`）。
- `Source/SqueakyRatkin/SqueakyRatkinSettings.cs`：`SQUEAKY_DEV` 才开放 dev-only 设置项。

### 统一 staging（`stage-package.ps1`）

输入（仓库只读侧）：`About/`、根 `LoadFolders.xml`、`1.6/`（含编译好的 DLL、Defs、Languages、Patches）、`Extras/SqueakyRatkinExampleVoices/`（Template 音频、Extras About、README、AUDIO_PROVENANCE/RIGHTS）。输出：`<StageDir>`（三个 packer 分别传 `dist/{dev,github,steam}/SqueakyRatkin`）。彩蛋测试产物 `dist/SqueakyRatkinEggTestVoices/`（0.3.2，gitignored）不在 staging 输入内，按构造永不进任何发布包。

staging 顺序与卫生规则：

1. 前置断言：`1.6/Assemblies/SqueakyRatkin.dll` 必须存在；Extras 包必须存在；**仓库根 `1.6/Sounds/coahuilite.squeakyratkin/SR_OfficialExample_Race` 必须不存在**（built-in 音频唯一维护源是 Template）。
2. `Assert-FormalExampleAudio $templateAudio 'Template'`：目录必须存在、**全部文件必须是 `.ogg`**、至少有一个文件、顶层 action 目录必须属于固定 15 个运行时 action、audio key（`<action>/<去扩展名文件名>`）不得重复（防 `x.ogg` 与 `x.OGG` 同 key 双文件）。音频总数、已覆盖的动作子集和每动作数量均不是门禁；当前 41 OGG / 15 action 分布仅为参考基准。
3. 清空并重建 StageDir，递归拷贝 About / LoadFolders.xml / 1.6 / Extras。
4. 把 Template 音频镜像到 staged 的 `1.6/Sounds/coahuilite.squeakyratkin/SR_OfficialExample_Race`（此时 built-in 源才诞生），然后 `Assert-ExampleAudioMirrors`：两个根必须不同且互不为祖先目录；实际 key 集合完全一致；**每个 key 的 SHA256 逐文件相等**。
5. 从 stage 中删除 `About/PublishedFileId.txt`（**Steam identity 排除**）、所有 `*.pdb`、所有 `*.gitkeep`、所有 `codemap.md`（导航文档是仓库工具，不属于分发内容）。

当前 Example 参考分布为 41 个 OGG，覆盖全部 15 个运行时 action：Attack 3、Call 4、Death 2、Draft 3、Eat 2、Equip 2、Joy 3、MentalBreak 1、Move 3、Select 3、Sleep 3、Social 3、Undraft 3、Work 3、Wounded 3。它描述当前内容，不是脚本或产品数量合同。

运行时 action 名称仍是固定契约：`1.6/Defs/SoundDefs/SqueakyRatkin_OfficialExample_Race.xml` 与 `Extras/.../SqueakyRatkin_ExampleTemplate_Race.xml` 的 `AudioGrain_Folder` 路径按 `<Action>` 指向相应目录。新增运行时 action 时，才需要同步 `$actions` 数组和两份 SoundDefs；单纯变更音频数量无需改脚本或 Def。

### Identity 来源（三路不同）

| flavor | 包外 identity | 运行时 informational | 来源 |
|---|---|---|---|
| dev | label 文件名 `SqueakyRatkin-dev-v<csproj版本>-<短sha>[-dirty].txt` | SDK 自动 `0.2.0+<完整sha>`；`Mod.BuildIdentity` → `dev-<sha12>` | csproj `<Version>` + git（工作树脏则 `-dirty`） |
| github | zip 名 `SqueakyRatkin-<tag>.zip` | `vX.Y.Z+<sha12>`，由 release.yml 传 `-p:SqueakyInformationalVersion` 并关 `IncludeSourceRevisionInInformationalVersion` | 严格 SemVer tag（`pack-github.ps1` 正则再验） |
| steam | 无（发布目录 `dist/steam/SqueakyRatkin`，上传人工） | `0.2.0`（split('+')[0] 即版本号） | csproj 唯一 `<Version>` |

`<Version>` 是全部 flavor 的单一事实来源：`pack-steam.ps1` 与 `release.yml` 都强制"恰好一个非空 `<Version>` 节点"。

### 安全不变式

1. **Steam PublishedFileId 排除**：`About/PublishedFileId.txt` 被 `.gitignore` 排除、永不提交；`stage-package.ps1` 无条件从 stage 删除它 → GitHub/Dev 产物不可能携带 Steam workshop identity；Steam 发布时由上传者在 `dist/steam/SqueakyRatkin/About/` 现场生成。
2. **Template 唯一 OGG 维护源**：仓库内 built-in 音频路径必须不存在（staging 才生成），杜绝"两份音频各自演化"；镜像一致性由 SHA256 强校验。
3. **发布物零调试/零 VCS 残留**：`.pdb`、`.gitkeep` 一律从 stage 删除；Release 构建 `DebugType=none`。
4. **版本纪律**：GitHub 路径双重校验（脚本正则 + workflow 正则），且 release tag 的 base 版本必须等于 csproj `<Version>`、tag commit 必须是 `origin/main` 的祖先（`merge-base --is-ancestor`）。
5. 所有脚本 `Set-StrictMode -Version Latest` + `$ErrorActionPreference = "Stop"`，任何校验失败即 throw，不产出半成品包。

## Data & Control Flow

```
[触发] dotnet build -c Release -p:SqueakyBuildFlavor=<F>   ← compile，写 1.6/Assemblies/SqueakyRatkin.dll
   │
   ▼
pack-dev.ps1 ──┐   pack-github.ps1 ──┐   pack-steam.ps1 ──┐   （每个都先读 csproj <Version> 与自身 identity 前提）
   │ git label  │   │ SemVer -Version │   │ 唯一 <Version> │
   ▼            ▼   ▼                 ▼   ▼               ▼
        stage-package.ps1 -StageDir dist/<flavor>/SqueakyRatkin
           │  1) 断言 DLL / Extras / built-in 源不存在
           │  2) Assert-FormalExampleAudio（非空 OGG / 已知 action / key 唯一）
           │  3) 拷贝 About + LoadFolders.xml + 1.6 + Extras → StageDir
           │  4) 镜像 Template 音频 → stage 内 built-in 路径
           │  5) Assert-ExampleAudioMirrors（key 集合 + SHA256）
           │  6) 删 PublishedFileId.txt / *.pdb / *.gitkeep / codemap.md
           ▼
dist/dev/SqueakyRatkin/            dist/github/SqueakyRatkin/          dist/steam/SqueakyRatkin/
   + label .txt                        → zip SqueakyRatkin-<tag>.zip      （发布目录，上传人工）
```

调用方向：pack-* → stage-package（唯一引擎，禁止复制其逻辑）；pack-* 之间互不调用。状态所有者：`dist/` 与 `1.6/Assemblies/` 是 gitignored 的构建态，脚本全权管理；`About/`、`1.6/`（除 Assemblies）、`Extras/` 是仓库维护的输入态。`stage-package.ps1` 每次从零重建 StageDir（先删后建），无增量。

## Integration

- **CI**：[`.github/workflows/ci.yml`](../.github/workflows/ci.yml) —— push（main/master/dev）与 PR 触发；`setup-dotnet 8.0.x` → Dev flavor Release 构建 → push 时调 `pack-dev.ps1` 并 `Compress-Archive dist/dev/SqueakyRatkin` 为 `dist/dev/SqueakyRatkin-dev-<sha>.zip` → `actions/upload-artifact@v4`（14 天保留）。PR 只构建不打包。
- **Release**：[`.github/workflows/release.yml`](../.github/workflows/release.yml) —— `v*` tag 触发（`fetch-depth: 0`）；先验 tag 格式/版本一致/main 祖先，再以 `SqueakyInformationalVersion=v<tag>+<sha12>` 构建 **GitHub flavor**，调 `pack-github.ps1 -Version $tag`，`softprops/action-gh-release@v2` 上传 zip（`generate_release_notes`；tag 含 `-` 自动标记 prerelease）。
- **Steam**：无 CI 发布环节（上传人工）；本地 `build-steam.ps1`（Steam flavor 构建 + `pack-steam` 干净树硬门 + About==csproj 版本断言）产出 `dist/steam/SqueakyRatkin`，`PublishedFileId.txt` 在 `About/` 现场生成（见安全不变式 1）。
- **内容契约对接**：[`../1.6/Defs/SoundDefs/`](../1.6/Defs/SoundDefs/) 的 `SR_OfficialExample_Race` 与 `Extras/` 的 `SR_ExampleTemplate_Race` 的 `AudioGrain_Folder` 路径使用固定运行时 action 名称；`stage-package.ps1` 校验 Template 音频只使用这些已知目录，并镜像实际 key 集合与 SHA256（见 Change Guidance）。
- **运行时身份消费**：[`../Source/SqueakyRatkin/codemap.md`](../Source/SqueakyRatkin/codemap.md)（`Mod.cs`/`SqueakLog.cs`/`Settings.cs` 的 `SQUEAKY_*` 分支）；发行物布局见 [`../About/codemap.md`](../About/codemap.md) 与 [`../1.6/codemap.md`](../1.6/codemap.md)（注意：staged 的 `1.6/` 比仓库多出 `Sounds/` 镜像与真实 DLL）。

## Change Guidance

- **加/删/改音频**：只改 `Extras/SqueakyRatkinExampleVoices/1.6/Race/Sounds/coahuilite.squeakyratkin.examplevoices/SR_ExampleTemplate_Race/`；音频总数和每动作数量可变，不必修改脚本的数量表。新增运行时 action 时才同步 `stage-package.ps1` 的 `$actions` 数组、两个 SoundDefs XML 的 action 列表/`clipFolderPath`；跑任一 pack 脚本验证实际 Template→built-in 键集合与 SHA256 镜像。
- **改版本**：只改 csproj `<Version>`（保持恰好一个非空节点）；release 需打 `v<同一版本>` tag 于 main 历史。
- **加 flavor**：csproj 默认值 + `DefineConstants` 已通配；需在 `Mod.cs`/`SqueakLog.cs`/`Settings.cs` 加 `#if` 分支、新增 pack 包装脚本（复用 stage-package）、如需 CI 则在 workflow 加构建步骤。
- **禁止**：往 `1.6/Sounds/` 或 `dist/` 提交文件（gitignored 生成物）；在 `About/` 提交 `PublishedFileId.txt`；绕过 stage-package 自建 staging 逻辑；把非 OGG 文件放进 Template 音频目录。
- **Steam 打包纪律（2026-08-20）**：Steam 是最终发布步——必须干净树（`pack-steam` 硬门）+ Steam 发布态（用 `build-steam.ps1` 一步入口，flavor 由构造保证）；三渠道 `stage-package` 统一断言 `About.xml <modVersion>` == csproj `<Version>`，失配即 throw。
- **本地一键校验（2026-08-23 扩至 11 项）**：`pwsh scripts/verify-local.ps1` 是手动纪律的自动入口——任何 Kernel/协议/设置/XML ABI/脚手架改动后先跑它再谈提交；输出每检查一行，红即停。离线缓存环境可加 `-NoRestore`。
- 脚本为无参数/单参数 CLI，幂等（先清后建）；本地调试用 `pwsh -File scripts/pack-dev.ps1`（需先 `dotnet build`）。
