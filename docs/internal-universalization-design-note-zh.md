# 内部通用化设计笔记（0.3.x）

**状态：已接受的规划输入，不是当前产品合同。** 当前正式产品仍是 Ratkin-only；本笔记只定义在 Squeaky Ratkin（SR）仓库内孵化 Universal Squeaker（US）的顺序、边界和拆分门。实现前必须将受影响结论提升进 [`project-architecture-contract.md`](./project-architecture-contract.md) 与设置合同；不得以本笔记覆盖现行合同。

## 目标与非目标

目标是先把 SR 的内部逻辑普遍化，再在同一实现经验证后物理拆出 US：

1. `0.3.x` 在 **同一 SR 模组、同一程序集** 内完成 race-aware 路由、通用装配和迁移验证；不得先发布空壳前置，也不得长期保留两套运行时实现。
2. 条件成熟后，新建独立 US Workshop 模组；现有 SR Workshop 项目保持订阅身份并收缩为 Ratkin VoicePack，依赖 US。
3. Ratkin 的当前行为、No-DLC 基线、15 个 action、现有 `srdiag fmt=1` 协议和已保存设置必须有明确的兼容/迁移证据。

非目标：不把 Kiiro 实验包装为正式兼容；不复制第三方资源或代码；不让“全局 Race 池能发声”冒充 race-aware 成功；不在本阶段自动启用没有 fallback profile 的任意种族；不为 Kiiro 或任何单一第三方种族留下永久 resolver/settings/logging 分支。

## 已确认的起点

- Harmony 派发按 `CompSqueaker` 存在性工作；Kiiro 实验已证明：标准 HAR race 在启动时获得同一 comp 配置后，可进入共享触发漏斗。该结论只证明**组件生命周期与触发链**，不证明音源按种族隔离。
- 当前 `SqueakRuntimeResolver` 已有 Global / Race / Xenotype 三层概念，但 Race packs 是全局池，Xenotype key 只有 `xenotypeDefName`；两者都不足以表示多种族域。
- 当前 `VoicePackSelectionRecord` 的 Race 域没有 `raceDefName`，且 Xenotype 域没有 race 维度；其 Scribe schema 是迁移边界。
- `SqueakLog` 的 public facade 和 `srdiag v1` 已由 `tools/SqueakLogCharacterization` 锁定；任何需要记录 race 身份的日志改动必须先设计新协议版本，不能临时塞字段或新增 Kiiro 专名事件。

## 目标数据模型

所有身份使用精确、大小写敏感的 DefName；显示名、包名、HAR hint、图标和本地化只用于展示或候选发现。

```text
RaceKey              = raceDefName
XenotypeKey          = xenotypeDefName
RaceAudioDomain      = (RaceKey)
XenotypeAudioDomain  = (RaceKey, XenotypeKey)

选择链：
(race, xenotype) VoicePack 池
  → race VoicePack 池
  → race fallback profile
  → 无声
```

- 每个 VoicePack 必须声明且只服务一个 `raceDefName`；Xenotype Pack 还必须声明一个 `xenotypeDefName`。同域的 PackDef 组成稳定、带权且公平的池。
- 行为/mood 的继承继续是 XML comp 默认 → 全局设置 → `(race,xenotype)` delta；音频选择与行为设置保持分离。
- fallback profile 是 XML 数据：精确 `raceDefName` 与 15 个 action→SoundDef 映射。拥有内置 profile 的 race 可默认启用；无 profile 的 race 只能由玩家显式启用，并明确提示“无 fallback 且无可用 VoicePack 时不会发声”。不得写 C# race switch，也不得复制原版资产。

## 年龄维度（规划输入，0.2.3 玩家反馈产生）

事实基础：当前路由与年龄无关（0.2.3 排查已取证，全库无生命阶段分支）；婴幼儿默认听感问题已由默认启用内置包缓解，但"不同年龄段听感差异化"仍是真实产品需求。年龄支持必须与 race-aware 域模型**同期设计、同期冻结 XML ABI**，避免第二次 Scribe 迁移。

- **标签形式**：`SqueakVoicePackAction` 增加可选年龄标签（field-presence，如 `Baby`/`Toddler`/`Child`/`Adult` 或 RimWorld 生命阶段 DefName）；**未声明 = 全年龄适用**，第三方存量包零改动、零迁移。选择时按 pawn 当前生命阶段过滤可用条目。
- **年龄调制轴**：独立于 mood 的调制维度（pitch/volume/jitter 的年龄系数），XML 数据驱动（per-race 或全局默认），不得写 C# age switch；与 `SqueakMoodMod` 同构叠加。
- **兼容边界**：`SqueakAction` 枚举不变（append-only）；年龄标签只扩展 `SqueakVoicePackAction` 与调制数据模型；srdiag 协议如需记录年龄身份，先设计新协议版本。
- **实现窗口**：0.3.1（race-aware catalog/resolver）之后作为设计输入进入 0.3.2 冻结；不提前进入 0.2.x。
- **Crying/Giggling 动作兼容（0.2.4 排查产出）**：Biotech 婴幼儿的哭/笑是 `MentalFitDef` 驱动的 mental state（`MentalStates_BabyFits`，`stateEffecter: BabyCrying/BabyGiggling` 是原版专属音频）。0.3.x 年龄域设计时把它们作为 Baby 年龄标签动作纳入（VoicePack 可为哭/笑提供专属音频，与既有周期动作正交）。0.2.4 已修复其被误报为精神崩溃的问题（hook 收窄至 `MentalBreakWorker.TryStart`），此兼容不影响该修复。

## 日志协议候选（srdiag v2，规划输入）

- `SettingsOrigin` 事件：记录本次会话 ModSettings 来源（`FreshCreated`=文件缺失用字段默认值 / `LoadedFromFile`=磁盘反序列化），用于排障区分"全新安装"与"设置文件丢失"。现状：正常 fresh 路径在框架（`LoadedModManager.ReadModSettings` 静默 `new T()`，仅反序列化异常时 Warning）与 SR 侧（locked 28 事件无此项）均无日志。随 0.3.x 日志协议版本化（该版本本来需记录 race 身份）一次扩展，不单独改动 locked facade。

## 装配与发现边界

最终实现应以通用 race profile/registry 驱动，而不是以 `Kiiro_Race` 等专名 adapter 驱动：

1. **发现**只产生候选，不自动使任何 race 可发声。
2. **装配**只对明确受支持的 profile 或玩家显式启用的 race 执行；配置来源应是 canonical `CompProperties_Squeaker` 模板，而不是把 Ratkin 的 Def 当作永久模板来源。
3. `CompSqueaker` 继续是 Harmony 派发资格；不得在 patch 中叠加第二个 race-name/`IsRatkin` gate。
4. Kiiro 分支保留为受控侦察证据；未来进入 dev 的只能是通用装配机制，不是 adapter 的 no-squash 搬运。

## 分阶段实施与验证门

| 阶段 | 交付物 | 必须成立的证据 |
| --- | --- | --- |
| 0.2.2（已启动） | 日志协议 characterization、`SqueakLog` 职责拆分、低风险去重/卫生 | 主模组 0 error；默认与 Dev flavor logging harness 通过 |
| 0.3.0 | Ratkin 触发/路由 characterization；内部 `RaceKey`/域值对象；仅 Ratkin 走新内部模型 | Ratkin 在 No-DLC 和现有设置下的行为等价；无 Scribe/日志 ABI 变化 |
| 0.3.1 | race-aware catalog/resolver：`(race,xenotype)` 与 race 池；旧 Ratkin selection 的显式、幂等迁移 | Kiiro 或另一外来 race 的**per-race**池端到端实证；不再共享 Ratkin/Example 池 |
| 0.3.2 | 通用 profile 驱动装配、per-race fallback、设置 UI 候选与显式 opt-in | 无 profile race 默认静音；有 profile race 的 fallback 可验证；No-DLC 不访问 Xenotype 路径 |
| 拆分准备 | US/SR 设置、保存、Workshop 迁移演练 | 下列六项拆分门全部通过 |

每个阶段只引入一条活运行时路径。旧路径应在迁移完成的同一变更中删除，不能用长期 shim 掩盖双实现漂移。

## 设置、存档与 Workshop 迁移

- 把当前 Race 选择显式迁为 `RaceAudioDomain(Ratkin)`；把当前 Xenotype 行为/音频目标迁为 `(Ratkin, xenotypeDefName)`。迁移通过递增 schema 版本实现，必须幂等、可重启、失败不丢旧记录。
- 迁移前先保存真实旧设置 fixture；迁移后验证 immediate runtime publish、约 350 ms 合并保存、窗口 close flush、无 Biotech 安全降级，以及 orphan/dormant 语义。
- 物理拆分时，US 使用新 packageId；SR 保持现有 packageId 和 Workshop item，变为依赖 US 的 VoicePack。必须在真实 save modlist 上验证 `SR → US + SR` 的升级路径；不得把 staging 成功当作订阅、保存或 Workshop 状态的证据。

## US 物理拆分门

以下每项都有可重复证据后才创建独立 US：

1. 逻辑层零 Ratkin 硬编码（数据/fallback profile 除外）；
2. 至少一个外来 race 完成 per-race pool 的端到端验证；
3. Ratkin characterization 在内部重构后持续全绿；
4. 旧 SR settings → US 的迁移设计与实机验证完成；
5. 保存兼容：`SR → US + SR` 升级路径通过；
6. SR 同 Workshop item 收缩为 VoicePack、US 新 item、依赖/页面/Claim Pack 的过渡演练完成。

达到六门之前，SR 仍是唯一可发布的实现载体；US 不建 Workshop 空壳，Kiiro 不作宣传或发布承诺。
