# squeaky_ratkin/ — 根地图（Repository Atlas）

> 全部 13 张子目录 codemap 的聚合入口。子图按目录就近存放（`<目录>/codemap.md`），本图只做职责汇总、跨层数据流与导航，不复制子图实现细节；子图内容以各自文件为准。
> 核验时间：2026-08-13（13 张子图全部已读）。

## Project Responsibility

**鼠辈啁啾 / Squeaky Ratkin** 是 RimWorld 1.6 语音发声模组：为 HAR 体系的 Ratkin 种族（`AlienRace.ThingDef_AlienRace[defName="Ratkin"]`，def 由 NewRatkinPlus 提供）挂载每 pawn 的发声组件，按 15 个 `SqueakAction`（Eat / Call / Move / Sleep / Social / Joy / Work / Wounded / Select / Death / Draft / Undraft / Attack / Equip / MentalBreak）触发音效，支持心情调制（mood pitch/volume/jitter）、距离预设、VoicePack 分层选音（Vanilla 回退 / Race / Xenotype）、设置工作台、诊断工具与结构化日志。

- **身份**：packageId `coahuilite.squeakyratkin`（大小写敏感，Extras 内嵌包依赖此 ID）；产品版本 `0.2.1`；`supportedVersions` 仅 1.6。
- **依赖分层**：Harmony（`brrainz.harmony`）与 HAR（`erdelf.HumanoidAlienRaces`）为硬依赖（`modDependencies`）；NewRatkinPlus（`Solaris.RatkinRaceMod`）运行时必需但元数据仅 `loadAfter` 软声明；`LoadFolders.xml` 无条件加载本体，发声注入按 XPath `defName="Ratkin"` 匹配（缺 def 时静默 no-op，兼容保留该 def 的 fork）；全部官方 DLC 与 HugsLib **零引用**（No-DLC 契约：Biotech 增强全部经 `ModsConfig.BiotechActive` 门控，HAR 交互全反射）。
- **配置三层**：XML 默认（`1.6/Patches`）← 玩家 ModSettings override（`SqueakyRatkinSettings`）← 运行时发布（resolver/policy 不可变快照）。
- **发布形态**：Dev / GitHub / Steam 三种 build flavor 的 mod 包，统一由 `scripts/stage-package.ps1` 组装。

## System Entry Points

| 入口 | 位置 | 说明 |
|---|---|---|
| 游戏加载 | `About/About.xml` + 根 `LoadFolders.xml` | `ModLister` 读元数据 → 依赖校验/排序 → `InitLoadFolders`（NewRatkinPlus 激活时挂 `[1.6, /]`，1.6 优先）→ 载入 `1.6/` 的 Patches / Defs / Assemblies / Languages |
| C# 装配点 | `Source/SqueakyRatkin/Mod.cs`（`SqueakyRatkinMod` ctor） | 唯一 Mod 入口：`Harmony.PatchAll()`（id `coahuilite.squeakyratkin`）→ `ExecuteWhenFinished` 启动链（resolver 主线程初始化、catalog 刷新、设置应用、迁移 flush） |
| Harmony 事件转译 | `Source/SqueakyRatkin/Patches/` | 15 个 patch：伤害/攻击/死亡/选中/整编/装备/精神崩溃/周期成员/诊断生命周期/设置窗口关闭等 → `CompSqueaker.Notify_*` 等下游 |
| 设置工作台 | `Mod.DoSettingsWindowContents` → `UI/` | 四页：Basics / SoundMood / Xenotype / Developer（七击解锁）；业务写入 → `QueuePersistence` → 350 ms 防抖保存 |
*23|| 调试入口 | `Debug/` + `[DebugAction]` 菜单 | overlay（单字符标记 + 可拖动诊断面板）、音频路径环形缓冲、触发漏斗统计、音频浏览工作台；四层门控（DevMode / `developerToolsEnabled` / `EffectiveDevLogging` / `AudioPathDiagnostics.Enabled`） |
| 日志出口 | `Logging/SqueakLog.cs` | 唯一日志出口：closed typed facade（25 事件方法覆盖 28 事件 ID）+ `srdiag fmt=1` 机器字段 → Verse `Log` |
| 构建/打包 | `scripts/pack-*.ps1` + `.github/workflows/` | `dotnet build -p:SqueakyBuildFlavor=<F>` → `stage-package.ps1` → `dist/<flavor>/` |

## Architecture / Data Flow

```mermaid
flowchart LR
    subgraph XML["内容层 1.6/（数据契约）"]
        P["Patches/Ratkin_AddSqueakComp.xml<br/>注入 CompProperties_Squeaker"]
        SD["Defs/SoundDefs<br/>SR_* 回退 + OfficialExample"]
        MD["Defs/MoteDefs<br/>SR_Mote_TextBg"]
    end
    subgraph CS["运行时层 Source/SqueakyRatkin/"]
        H["Patches/（Harmony ×15）"] -->|Notify_*| C["CompSqueaker<br/>TryTrigger 闸门链"]
        C -->|选择查询| R["SqueakRuntimeResolver<br/>不可变快照 Vanilla/Race/Xenotype"]
        C -->|mood 调制 + PlayOneShot| A["音频"]
        C -->|RecordOutcome| D["Debug/ 统计/mote/overlay/浏览器"]
        D --> L["Logging/SqueakLog"]
        U["UI/ 设置四页"] -->|Notify*RuntimeChanged| R
        U -->|QueuePersistence| S["SqueakyRatkinSettings + Mod.cs 保存队列"]
    end
    P -->|SqueakAction 枚举 + SR_ 前缀 defName| C
    SD --> R
    MD --> D
    subgraph PK["发布层 scripts/ + workflows/"]
        B["dotnet build<br/>SQUEAKY_* flavor 常量"] --> ST["stage-package.ps1<br/>Template→built-in 镜像 SHA256 校验"]
        ST -->|"dist/{dev,github,steam}"| CH["zip / SteamCMD / GitHub Release"]
    end
    CS --> B
```

跨层流要点（细节见各子图）：

1. **XML/patch → Comp/runtime**：`1.6/Patches` 在加载期把 `CompProperties_Squeaker` 注入 Ratkin.comps；XML 与 C# 之间的兼容边界是 `SqueakAction` 枚举（15 值，append-only，序数稳定）与 `SR_` 前缀 defName 契约（`SqueakActionDefinitions.AudioKey` = `"SR_" + 动作名`）。新增动作必须三处同步：枚举 + `AudioKey` + `SR_<Action>` SoundDef，再补运行时 hook。
2. **触发 → 选音 → 播放**：Harmony patch 转译为 `CompSqueaker.Notify_*` / `CompTick` → 闸门链（spawned + CurrentMap + 视口 + plan.Configured）→ 全局作用域 `SqueakGlobalActionPolicy` → 快照上下文 → 周期人口缩放 → 时序模型 → 概率/vocal 门 → `ChooseProductionSound`（Off=仅 Vanilla；Fallback/Remix=层内 `HasPlayable` 过滤后随机）→ mood 调制后 `PlayOneShot`。失败路径全部 `RecordOutcome` + 统计埋点。
3. **设置 → 运行时**：UI 只表达意图，canonical 写入在 `SqueakyRatkinSettings`；经 `Notify*RuntimeChanged` 发布（cheap=静态字段、continuous=75/150 ms 防抖、discrete=立即），resolver 主线程单发布者发布不可变快照；磁盘写入只走 `base.WriteSettings()`（Mod 层 generation + 350 ms 防抖合并）。
4. **诊断/日志**：`Debug/` 只观察不决策（revision + 不可变快照 + Layout/Repaint 分离）；所有失败/边界事件经 `Logging/SqueakLog` 枚举化出口（once 去重 + `srdiag fmt=1` 机器字段）。
5. **build → stage → channel**：`SqueakyBuildFlavor` 决定 `SQUEAKY_DEV/GITHUB/STEAM` 常量（运行时 `BuildIdentity`、dev 日志默认、dev-only 设置项）；`stage-package.ps1` 是唯一 staging 引擎（断言 DLL 存在、Template→built-in 音频实际键集合与 SHA256 镜像、删除 `PublishedFileId.txt`/`*.pdb`/`*.gitkeep`）；csproj `<Version>` 是全部 flavor 的单一事实来源（release tag 基版本强制一致）。

## Repository Directory Map

所有子图均为「Responsibility / Key Files / Design / Data & Control Flow / Integration / Change Guidance」六段结构，逐级下钻即可。

| 目录 | 子图 | 一句话职责 |
|---|---|---|
| `About/` | [About/codemap.md](About/codemap.md) | mod 元数据与发布边界：`About.xml`（packageId、依赖分层、仅支持 1.6）、商店图标；本地图同时覆盖根 `LoadFolders.xml` 与 `.github/workflows/`（CI/Release 流水线） |
| `1.6/` | [1.6/codemap.md](1.6/codemap.md) | 1.6 内容包：加载期 comp 注入 + 全部发声/浮字 Def + 随包 DLL + 本地化；无条件加载，发声注入按 XPath `defName="Ratkin"` 匹配 |
| `1.6/Defs/` | [1.6/Defs/codemap.md](1.6/Defs/codemap.md) | 纯 XML Def 契约层（无 C# 编译依赖）：SoundDefs 与 MoteDefs 两个子域的入口 |
| `1.6/Defs/SoundDefs/` | [1.6/Defs/SoundDefs/codemap.md](1.6/Defs/SoundDefs/codemap.md) | 两层音池：15 个 `SR_<Action>` Vanilla 回退 SoundDef + `SR_Call_Preview` 试听 transport + `SR_OfficialExample_Race` 官方 Example VoicePack（音频打包时注入） |
| `1.6/Defs/MoteDefs/` | [1.6/Defs/MoteDefs/codemap.md](1.6/Defs/MoteDefs/codemap.md) | 唯一调试浮字 `SR_Mote_TextBg`（`MoteTextWithBackground` + `SqueakMoteOffset` 偏移），仅 Debug 子系统消费 |
| `1.6/Patches/` | [1.6/Patches/codemap.md](1.6/Patches/codemap.md) | 加载期补丁：向 Ratkin.comps 注入 `CompProperties_Squeaker`（15 action 触发配置 + 4 moodMods + 3 distancePresets，全数据驱动） |
| `Source/` | [Source/codemap.md](Source/codemap.md) | C# 运行时源码根：装配边界与入口；实质内容在 `Source/SqueakyRatkin/`（见下） |
| `Source/SqueakyRatkin/` | [Source/SqueakyRatkin/codemap.md](Source/SqueakyRatkin/codemap.md) | 运行时核心与单一装配点：Mod 生命周期、`CompSqueaker` 触发/播放、resolver/catalog/policy 不可变快照发布、周期人口缩放、全部持久化数据模型 |
| `Source/SqueakyRatkin/Patches/` | [Source/SqueakyRatkin/Patches/codemap.md](Source/SqueakyRatkin/Patches/codemap.md) | Harmony 集成层（15 patch）：RimWorld 事件 → `CompSqueaker.Notify_*` / 周期成员 / 诊断生命周期 / 设置窗口关闭；patch 薄、业务判断在 Comp |
| `Source/SqueakyRatkin/UI/` | [Source/SqueakyRatkin/UI/codemap.md](Source/SqueakyRatkin/UI/codemap.md) | 设置工作台 composition layer：四页 UI、局部编辑缓冲（draft）、诊断面板与音频浏览入口；不拥有业务写入与保存协调 |
| `Source/SqueakyRatkin/Debug/` | [Source/SqueakyRatkin/Debug/codemap.md](Source/SqueakyRatkin/Debug/codemap.md) | 诊断与开发者工具：overlay、DebugAction 菜单、音频路径追踪、触发漏斗统计、mote、音频浏览工作台（四层门控） |
| `Source/SqueakyRatkin/Logging/` | [Source/SqueakyRatkin/Logging/codemap.md](Source/SqueakyRatkin/Logging/codemap.md) | 唯一日志出口：`SqueakLog` closed typed facade（25 事件方法覆盖 28 事件 ID + `srdiag fmt=1`），注册表/once/formatter/sink 内聚于一个静态类（P0 治理目标） |
| `scripts/` | [scripts/codemap.md](scripts/codemap.md) | 构建/打包流水线：`stage-package.ps1` 唯一 staging 引擎 + dev/github/steam 三个 flavor 包装脚本；守护 Template→built-in 音频镜像、版本纪律与发布包边界。 |

相邻但无独立子图：`Extras/SqueakyRatkinExampleVoices/`（随包分发的独立示例包，Template 音频唯一维护源）、`docs/`（权威合同，见 Navigation Rules）、`1.6/Languages/`（本地化）、`1.6/Assemblies/` 与 `dist/`（gitignored 构建态）。

## Current Change Surface

**权威状态**：[`TODO.md`](TODO.md)（0.2.1 已知问题修复 / 0.2.2 Kiiro 实验 / 明确延后 / 待重新确认）与 [`MEMORY.md`](MEMORY.md)（当前耐久状态、权威入口、工程决定与交接）。当前 0.2.1 三项已知问题修复（`TODO.md`）：

1. **悬浮诊断重构**：pawn 头上改为单字符标记（`●`，绿=就绪/金=其余，带黑阴影），复杂状态移入可拖动诊断面板窗口（Selected 单 pawn 分节详情 / Visible ≤16 行列表，均显式显示 Pawn 种族 `defName`，仅诊断展示不参与资格/路由/播放决策）；面板按 overlay `Revision` 缓存格式化文本，X/Esc 关闭联动 `SetMode(Off)`。
2. **修复七击开发选项未完成点击计数不归零**（已修复）：`BeginSettingsSession` 改用 `Time.frameCount` 检测绘制中断，重新打开设置时清零 `versionClickCount`。只清未完成计数，不改变已解锁状态。
3. **修复 fork NewRatkinPlus 因包名不匹配导致 SR 不生效**（已修复）：移除 `LoadFolders.xml` 的 `IfModActive="Solaris.RatkinRaceMod"` 硬门控，改无条件加载，发声注入按 XPath `defName="Ratkin"` 匹配。

其余：`TODO.md` 明确延后项（`TicksAbs` 再次复现时再调查归因、0.2.1 代码卫生）与待重新确认项（发布门禁、英文 VoicePack 作者指南、第三方 VoicePack 示例包方向——需维护者确认后才能恢复）。打包侧当前无开放任务；任何内容契约改动（音频数量/版本/依赖）必须同步 `scripts/stage-package.ps1` 校验表。

## Navigation Rules

1. **先根后子**：从本图按目录定位子图，沿六段结构（Responsibility / Key Files / Design / Data & Control Flow / Integration / Change Guidance）逐级下钻；子图之间以相对链接互引（如 `1.6/` → `Source/SqueakyRatkin/` → `Debug/` `Logging/` `UI/` `Patches/`）。
2. **权威合同**（`docs/`，根图不重复其内容）：`project-architecture-contract.md`（运行时/动作/resolver/VoicePack/Example/发布边界）、`settings-ui-product-contract-zh.md`（设置 UI 产品合同）、`logging-protocol.md`（srdiag v1 日志协议，唯一权威记录）、`voice-pack-author-guide-zh.md`（第三方 VoicePack 接入）、`steam-workshop-page-copy.md`（Workshop 页面维护源）。
3. **契约红线**（跨子图共享，改动前必读对应子图 Change Guidance）：No-DLC/HugsLib 零引用；`SR_` 前缀 defName；`SqueakAction` 枚举 append-only（新增动作三处同步：枚举 + `SqueakActionDefinitions.AudioKey` + `SR_<Action>` SoundDef，再补运行时 hook）；`packageId` 大小写敏感；Template↔built-in 音频实际键集合与 SHA256 镜像一致；csproj `<Version>` 与 git tag 基版本一致；`srdiag fmt=1` 字段顺序与 28 个事件 ID 为兼容面。
4. **状态所有权**：`dist/`、`1.6/Assemblies/`、staged `1.6/Sounds/` 为构建态（gitignored，脚本全权管理）；仓库维护态为 `About/`、`1.6/`（除 Assemblies）、`Extras/`、`Source/`。
5. **本图维护约定**：本图是纯汇总层（不复制子图实现细节）；子图更新后只同步「Directory Map」一行、相关入口与红线即可。`AGENTS.md` 不在本图管辖内（其更新需用户另行确认），本图不注册、不引用它。
