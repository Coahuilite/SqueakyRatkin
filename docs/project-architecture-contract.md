# 鼠辈啁啾架构与兼容合同

> 2026-09-19 重构。本文规定应保持的行为，自足描述现状；不代表所有行为均已实机验证。当前证据边界、待修复项与待裁决见 [维护状态](maintenance-status-zh.md)。

## 身份、权威与边界

品牌为 **鼠辈啁啾 / Squeaky Ratkin**，packageId 为 `coahuilite.squeakyratkin`，C# namespace 为 `SqueakyRatkin`，Defs 使用 `SR_`。产品支持 NewRatkinPlus Ratkin，由 XML 装配的 `CompSqueaker` 标识；路由机制按 VoicePack 的精确 `raceDefName` 处理域，不按名称、图标或 HAR 包身份特判种族。

明确接受的产品裁决优先于合同；合同规定实现目标，源码记录实际实现。两者冲突须显式登记，不能把现有 bug 写成承诺。[AGENTS.md](../AGENTS.md) 管操作、隐私和授权；UI 与日志分别以 [设置合同](settings-ui-product-contract-zh.md)、[日志协议](logging-protocol.md) 为准；历史规划不覆盖现行规则。

- 无官方 DLC 的基线：RimWorld Core + Harmony + HAR/NewRatkinPlus + SR。全局设置、动作、心情、Race 音源及内置回退必须可用。
- Biotech 未启用时不得访问 Xenotype DefDatabase 或 pawn genes。启用时只按精确且区分大小写的 `XenotypeDef.defName` 绑定；不可用的异种层 fail-closed，Race/内置回退继续工作。不做 gene 级设置、第三方 Xenotype 冲突仲裁或任意 HAR 种族的产品支持承诺。
- HAR 仅经反射增强发现；缺失或失败应降级，不得崩溃。显示/发现信息不是资格门。
- 卸载不得影响存档：不向游戏存档写永久状态；运行时 Def 装配可随卸载消失，设置与 profile 留在 Config。共存期保持 `SqueakyRatkin.CompProperties_Squeaker` / `SqueakyRatkin.CompSqueaker` 全名稳定；跨仓接管须先完成 [迁移条件](maintenance-status-zh.md#迁移与退役)。

## 动作、触发与发声

内置动作按以下序号 **append-only**：

```text
0 Call, 1 Eat, 2 Sleep, 3 Wounded, 4 Select, 5 Move, 6 Social,
7 Joy, 8 Death, 9 Draft, 10 Undraft, 11 Attack, 12 Work,
13 Equip, 14 MentalBreak, 15 Crying, 16 Giggling
```

这是 17 个 one-shot 动作键；内置音频/fallback 只覆盖前 15 个。`Crying` / `Giggling` 无可用包音频时静默。新增动作须联动源码、Defs、本地化、触发、验证及文档；SR 当前不提供插件动作注册机制。

XML 决定周期动作的 `EachTime` / `RandomOneShot` / `External`、间隔、概率、时钟及是否绕过全局冷却；外部动作由事件驱动。固定动作策略在 resolver、RNG、计时和播放之前执行；关闭动作意味着生产路径绝对静默，显式 preview 独立。全局策略与运行时 delta 均执行作用域限制。

| 触发 | 必须保持的边界 |
| --- | --- |
| 玩家选择/主动命令 | `PlayerSelection` / `ActiveCommand` 要求 `Pawn.IsPlayerControlled`；选择还要求 `!Downed && Awake()`。过滤 `Selector.Select(playSound:false)`，失败静默且不消耗冷却 |
| Draft / Undraft / Equip | 前两者只由玩家 gizmo 改变产生；Equip 只接受玩家下达的 Core Equip job，不接受 AI、加载或系统换装 |
| Attack | 只覆盖成功的 Core `Verb.TryCastShot` 实现；排除 Ability 命名 verb、DLC 程序集及不使用该方法的攻击系统 |
| Work / Move / Sleep | Work 可限制为玩家强制 job；Move/Sleep 是偶发 one-shot，不支持持续播放 |
| MentalBreak / BabyFits | 真崩溃 hook 不扩大；Crying/Giggling 仅来自成功的 `TryStartMentalState` postfix 经 `MentalFitDef` 反向映射确认的 BabyFits |

声带效率门作用于全部动作（含 Death）；启用的 `Talking` 能力概率门仅对 Death 豁免。enable/probability/timing 先于声带判定；声带拒绝与成功尝试消耗同样的动作/适用共享冷却，避免逐 tick 重试。

### Eat 三级粒度

默认保持整个 `JobDefOf.Ingest`，包括取食、端食物赶路及收尾，Eat 优先于 Move；不得默认收窄。两个设置默认均为 `false`：

| 父 `eatOnlyDuringChewing` | 子 `eatIncludeDrugs` | 有效模式与判据 |
| --- | --- | --- |
| 关 | 强制为关 | `WholeJob`：整个 Ingest job |
| 开 | 关 | `GainingNutrition`：`IEatingDriver.GainingNutritionNow` |
| 开 | 开 | `ChewingToil`：未确认 toil 名 **或** 正在 `ChewIngestible` **或** 正在摄入营养 |

`ChewIngestible` 来自 `JobDriver.CurToilString`，名称在进程中未确认时回落整个 job，不能静默。带营养成瘾品（啤酒 `0.08`、仙馔 `0.2`）在中档已算 Eat；烟卷/薄片等零营养摄入物靠第三档覆盖。UI 父关立即清零并禁用子项，PostLoadInit 也归一。规则由纯函数 `SqueakEatOccurrence` 提供，Verse 采样留适配层；不改变路由、冷却或 ABI，不新增日志事件。

## 配置与内核

`1.6/Patches/Ratkin_AddSqueakComp.xml` 的 actions/moodMods 与 `CompProperties_Squeaker.distancePresets` 是行为数据源；不要以动作名硬编码替代可表达的字段。

配置逐字段合并：XML 默认 → 全局 ModSettings → 精确 Xenotype delta；缺字段继承。选距离 preset 时复制其真实范围，手改为 Custom。行为/心情配置与音频选择相互独立；音频 schema 的迁移有意受限，不据此许诺无关设置自动迁移。

心情在派发时调制 `SoundInfo.pitchFactor` / `volumeFactor`；不生成 mood × action SoundDef 矩阵。相机按 `CurrentViewRect.ExpandedBy(10)` 裁剪，用 `SoundInfo.InMap(TargetInfo(Pawn))` 和 distRange 衰减；SoundDef 基础范围 `15–70`，Balanced 设置默认 `15–50`，范围外静默。禁止恢复 zoom 资格门（含 `CurrentZoom <= Close`）；高速控制缩放冷却，不单独缩放音量。

路由是选择的唯一事实源；执行层只能经输入与 `ISoundGate` 等注入面影响候选。决策逻辑出生即纯，游戏事实采样留适配层；`Kernel/` 零 Verse 引用，由 harness 链接编译，不为未来消费者预拆程序集。设置迁移先在临时副本完成，成功才替换；失败保留旧 schema/数据并阻止破坏性保存。

## VoicePack XML 与选择链

每个 `SqueakVoicePackDef` 是独立选择、权重及验证单元。包可包含多个 PackDef，但不得合并身份。

| 字段/身份 | 合同 |
| --- | --- |
| `raceDefName` | 必填、精确大小写；缺失拒绝 |
| `scope` / `targetDefName` | Race 无 target、无 Biotech 也可用；Xenotype 只有一个精确 target 字符串 |
| `weight` | 正有限 pack 权重，默认 `1` |
| `fallbacks` | 可选，内置动作键 → 该包 fallback tier 的 `SR_*` SoundDef |
| `actions` | `action`、可选 `ageTag` / `IsEgg`、`sounds`；仅贡献声音，不改行为/时机/能力/距离/心情 |
| 年龄 | `Baby` / `Toddler` / `Child` / `Adult`；缺省 all-age，exact-age 优先。直接映射 `CurLifeStage.developmentalStage`，不重算年龄阈值；Newborn/Baby→Baby，Child→Child，其余/缺失→Adult；1.6 无原生 Toddler |
| `IsEgg` | 加性池成员；`allowEasterEggSounds` 默认 false，打开才加入同权候选，resolver 重建应用 |
| PackKey | package ID + PackDef defName；Race/Xenotype 选择分域持久化 |

**作者 XML 是 public-stable：字段 add-only、内置动作键 append-only、验证 fail-closed，包含 IsEgg 与引用的 one-shot `SR_*` SoundDef 合同。** 原起点是“首个携带 0.3.1 ABI 的发行版本”；具体 tag 的解释未闭合，不能据此收回当前承诺。内部 kernel/domain/profile schema/未来动作门仍有 0.x 修订空间。作者操作正本为 [VoicePack SKILL](../.github/skills/squeaky-voicepack-authoring/SKILL.md)，本文不另造制作指南。

| 模式 | 期望语义 |
| --- | --- |
| Off | 仅内置 fallback；不是总静音开关 |
| Fallback | Xenotype → Race → pack fallback → built-in → 静默，逐动作判断；pack fallback 查询精确 `ctx.Domain`，不跨域借用 |
| Remix | 在当前可播放层之间等权选择，层内按包权重；无声明 pack fallback 时保留旧三层抽样通路（三层与四层同规则：只有非 None 层进入候选表） |

**R6（0.3.3 已修正）**：旧三层 Remix 曾按原始位序取 index，缺层时命中空位（该次事件静默）且后继层不可达——`(None, Race, BuiltIn)` 与 `(Xeno, None, BuiltIn)` 两个形状各约一半事件无声、内置层永不入选。0.3.3 起三层折叠与四层一致：仅在非 None 层上等权抽取（与 0.2.4 的候选集/固定序/等权/单层零抽取短路一致；抽取源自 0.3.0 起为 `floor(Rand.Value*N)`，故为分布等价而非 RNG 流一致）。回归守卫 = `tools/KernelCharacterization` 的 shape A/B 断言 + `fixtures/corpus/corpus-0.3.0-r6.txt` 字节回放；修正前语义作为历史证据保留在 `corpus-0.3.0.txt`，harness 断言两者差异为且仅为既定条数（540）的 Remix 行。证据见 [维护状态](maintenance-status-zh.md) 与 [0.3.3 发布记录](release_review/release-0.3.3-review-zh.md)。

`BuiltInFallbackCatalog` 是维护者拥有的 C# 单源表：精确 race → string action key → `SR_*`。Ratkin 种子仅 `Call` 至 `MentalBreak` 15 键；缺包且无内置 profile 则静默。自 0.2.3，出厂默认 Fallback + 内置 Race Example；仅对从未设置模式的配置幂等播种，不覆盖明确模式和已有选择。

## 持久化与分发

- `SqueakFallbackProfileStore` 单独持有 `SqueakyRatkin_Profile_<race>.xml`，临时文件 + 原子替换，不调用 `WriteSettings()` 或加入设置 debounce。缺失/损坏/旧版本重建；可用副本的 field-presence delta 与 C# 源合并。
- 消失 PackKey 保留为 orphan，暂不解析；因 DLC/catalog/target 不可用的 Xenotype 绑定为 dormant。身份恢复后自动恢复，刷新不删除；玩家确认“忘记此目标”才同时删除该目标行为 preset 与异种音源选择。
- 候选由包声明目标、已存绑定与合格发现信息组成；候选/显示不是音频白名单，HAR-only 提示不直接生成设置目标行。
- 内置 `SR_OfficialExample_Race` 与外部 `SR_ExampleTemplate_Race` 的 Def、PackKey、目录均独立，无特权权重；Template 的 Biotech 树是 TXT 指引而非可加载包。
- 唯一手工音频源在 `Extras/SqueakyRatkinExampleVoices/1.6/Race/Sounds/coahuilite.squeakyratkin.examplevoices/SR_ExampleTemplate_Race/`；基线 15 动作目录/41 OGG 可随内容演进。主模组源树不维护 OGG 镜像；stage 脚本镜像实际集合与 SHA-256，不锁总数。镜像/根路径/格式/散列异常须报告，不能擅自“修复”。
- 作者资源根为 `<lowercase packageId>/<PackDef.defName>/<Action>/`，SoundDef 路径与物理 Sounds 树一致。正式 Example 仅 OGG，第三方推荐 OGG Vorbis；WAV 可加载但不据此推荐发布，也不因 WAV 单独拒绝第三方包；不推荐 MP3。项目不仲裁第三方全局音频键碰撞。
