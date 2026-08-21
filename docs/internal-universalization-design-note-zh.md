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
  → pack 自带 fallback（可选）
  → US 内置 fallback profile（维护者保底）
  → 无声
```

- **路由两级并存，控制权分离**：①**pack 主导**——每个 VoicePack 必须声明且只服务一个 `raceDefName`（Xenotype Pack 还必须声明 `xenotypeDefName`）；有声明支持该 race 的 pack，US 即把 pack 内音频路由给该 race 的 pawn——Ratkin 同理、Kiiro 同理、任何其他种族同理，无 US 内置种族特判。同域的 PackDef 组成稳定、带权且公平的池。②**US 内置 fallback profile 是维护者主动维护的最终保底**——精确 `raceDefName` 的 15 个 action→原版 SoundDef 引用表（只引用原版资产，不复制、不重分发），在无 pack 启用（或 pack 缺该 action 且无自带 fallback）时将受支持 race 的语音路由到原版音频；**谁进内置表、何时更新由维护者决策**，不随社区生态漂移。表外 race 无 pack = 无声（显式 opt-in + 静音风险提示保留，0.3.2 结构）。
- **内置表规划起点 = `{Ratkin, Kiiro}`（fallback 兜底表，不是限制域）**：有 pack 声明的**任何** HAR race 都经内核路由（无限制）；内置表只决定"无 pack 时谁有原版兜底"。
  - **Ratkin**：双存在——SR 收缩后的 Example VoicePack（声明 `raceDefName=Ratkin`、完整 15 action 与 fallback 数据，保持 0.2.x 听感基线）主导路由；同时在内置表（包缺失/关闭时仍有原版兜底）。
  - **Kiiro**：仅内置表兜底（无 pack 亦发声，路由原版音频）；社区 pack 可补充主导层。
  - **Kiiro 入表的发布边界**：内置表条目是 XML 数据（不含 Kiiro 资源）；但 Kiiro 相关发布与公告仍受其 Workshop 衍生作品限制约束（既有决议：未经作者明确许可不得发布或宣传 Kiiro compat 内容）——条目可随作者许可核验同步推进，公告与发布以许可为门。
- **pack 的 fallback 声明为可选字段**：pack 声明优先于 US 内置表；均缺则无声。
- 行为/mood 的继承继续是 XML comp 默认 → 全局设置 → `(race,xenotype)` delta；音频选择与行为设置保持分离。
- 不得写 C# race switch，也不得复制原版资产。

## 年龄维度（规划输入，0.2.3 玩家反馈产生）

事实基础：当前路由与年龄无关（0.2.3 排查已取证，全库无生命阶段分支）；婴幼儿默认听感问题已由默认启用内置包缓解，但"不同年龄段听感差异化"仍是真实产品需求。年龄支持必须与 race-aware 域模型**同期设计、同期冻结 XML ABI**，避免第二次 Scribe 迁移。

**RimWorld 1.6 年龄认定事实基线（2026-08-21，RimSage 反编译源码 + wiki 一手核验）**：每 race 在 ThingDef XML 定义 `lifeStageAges` 列表（`(LifeStageDef, minAge)` 升序）；判定 = 按生物年龄取 `minAge < years` 的最后一条 → `pawn.ageTracker.CurLifeStage`；`pawn.DevelopmentalStage` = `CurLifeStage?.developmentalStage ?? Adult`（Pawn.cs:2022）。发育阶段枚举 `DevelopmentalStage : uint`（Verse）= None/Newborn/Baby/Child/Adult，**无 Toddler**。原版 Human race 1.6 五段：`HumanlikeBaby` 0–3（Baby，voxPitch 1.6）/`HumanlikeChild` 3–9（Child，voxPitch 1.2）/`HumanlikePreTeenager` 9–13（Child，voxPitch 1.2，MayRequire Biotech）/`HumanlikeTeenager` 13–18（Adult 默认值）/`HumanlikeAdult` 18+（Adult）。无 Biotech 时 PreTeenager 条目剔除（Child 3–13）；判定机制全 Core 数据驱动，婴儿/儿童生成内容是 Biotech 的。判定用生物年龄（`AgeBiologicalYearsFloat`），年龄阈值只存在于 race XML 数据，无 C# 硬编码。

**SR 映射口径（据此定案）**：内核 `AgeBucket {Baby, Toddler, Child, Adult}` 保留四值（ABI 已定型，append-only）；`SqueakLifeStageResolver` 按 LifeStageDef defName → AgeBucket 的 XML 数据表映射（默认 Adult）。1.6 原版映射：HumanlikeBaby→Baby、HumanlikeChild→Child、HumanlikePreTeenager→Child、HumanlikeTeenager→Adult、HumanlikeAdult→Adult；**Toddler 桶在 RimWorld 1.6 无原生生命阶段对应**（保留为第三方 race 预留桶，原版表不产生 Toddler）。SR 不得自行按年龄阈值重算阶段（复制原版 minAge 数据 = 双事实源漂移风险），一律经 `CurLifeStage.defName` 查表；表外 defName → Adult。

- **标签形式**：`SqueakVoicePackAction` 增加可选年龄标签（field-presence，如 `Baby`/`Toddler`/`Child`/`Adult` 或 RimWorld 生命阶段 DefName）；**未声明 = 全年龄适用**，第三方存量包零改动、零迁移。选择时按 pawn 当前生命阶段过滤可用条目。
- **年龄调制轴**：独立于 mood 的调制维度（pitch/volume/jitter 的年龄系数），XML 数据驱动（per-race 或全局默认），不得写 C# age switch；与 `SqueakMoodMod` 同构叠加。
- **兼容边界**：`SqueakAction` 枚举不变（append-only）；年龄标签只扩展 `SqueakVoicePackAction` 与调制数据模型；srdiag 协议如需记录年龄身份，先设计新协议版本。
- **实现窗口**：作为设计输入随 0.3.1 的 race-aware catalog/resolver 与 XML ABI 定型同批冻结；不提前进入 0.2.x。
- **Crying/Giggling 动作兼容（0.2.4 排查产出）**：Biotech 婴幼儿的哭/笑是 `MentalFitDef` 驱动的 mental state（`MentalStates_BabyFits`，`stateEffecter: BabyCrying/BabyGiggling` 是原版专属音频）。0.3.x 年龄域设计时把它们作为 Baby 年龄标签动作纳入（VoicePack 可为哭/笑提供专属音频，与既有周期动作正交）。0.2.4 已修复其被误报为精神崩溃的问题（hook 收窄至 `MentalBreakWorker.TryStart`），此兼容不影响该修复。

## 日志协议候选（srdiag v2，规划输入）

- `SettingsOrigin` 事件：记录本次会话 ModSettings 来源（`FreshCreated`=文件缺失用字段默认值 / `LoadedFromFile`=磁盘反序列化），用于排障区分"全新安装"与"设置文件丢失"。现状：正常 fresh 路径在框架（`LoadedModManager.ReadModSettings` 静默 `new T()`，仅反序列化异常时 Warning）与 SR 侧（locked 28 事件无此项）均无日志。随 0.3.x 日志协议版本化（该版本本来需记录 race 身份）一次扩展，不单独改动 locked facade。

## UI 可见性与玩家体验边界（0.3.x 全窗口与拆分发布）

**内核通用 ≠ 对外表现通用。** 通用化全程（0.3.0 起至 US 拆分发布）玩家可见体验与 0.2.x 保持一致，防止"半成品多种族"观感与承诺 gap：

1. **UI 数据驱动渲染已装配域**：设置 UI（含包分配器/编辑器）按已装配 races（有 profile 或已装配内容者）渲染；0.3.x 期间装配表 = `{Ratkin}`，三个常规页结构与文案保持现状，投影为单 Ratkin 域视图。**分配器/编辑器枚举唯一通道 = catalog 快照（已装配域），禁止直接枚举 DefDatabase/发现列表**；非 Ratkin race/xenotype 候选只进 dev 诊断（候选列表 + 诊断面板），release 设置面不可见。谁有可发声内容，谁出现在 UI——社区 race 获得 VoicePack/profile 时 UI 自动承接，无需改代码，也不预先展示空 race 页。
2. **装配层 = 通用机制 + profile 数据**：仅 Ratkin profile 装配（0.3.x）；不引入 Kiiro 式专名适配器作为"限制层"——数据面即限制面，通用装配机制本身只对受支持 profile 执行（装配边界 1-2 条）。
3. **泄漏面审计**：设置 schema 的 race 域是内部格式（玩家不可见）；srdiag race 身份进 v2 协议（已有候选）；诊断面板显示 defName 为现状功能，不新增公开泄漏。
4. **可见性宣告点 = US 拆分发布（单版本原子事件）**：US 新 item 上线 + SR 同 item 原地收缩为 VoicePack 并依赖 US，一次完成；不存在双实现共存窗口。宣告多种族时已有 ≥1 外来 race per-race 池实证与 profile 数据，承诺与交付同步。
5. **0.3.2 "设置 UI 候选"细化**：交付域渲染框架与显式 opt-in 结构（无 profile race 的玩家开关），玩家可见面不变；opt-in 入口仅在存在可装配的非默认 race 时出现。
6. **玩家反馈顺延**：社区评论中的多 race 请求（绮罗/沃芬/美狐等）是 US 阶段需求信号，不提前在 SR UI/页面承诺。
7. **设置面例外（Ratkin 域内正式化）**：fallback 路由编辑器、主动重建按钮、原版音频浏览器下放均为 Ratkin 单域能力 UI 化（0.2.x 只能 raw XML 编辑的通道转正），**不展示 race 维度、不出现非 Ratkin 条目**——不属于通用面暴露，与第 1 条（渲染已装配域）一致。

## 数据配置限定：0.3.x 只服务 Ratkin（防过早暴露）

机制全通用（拆分门 1：逻辑层零 Ratkin 硬编码），**交付数据只有 Ratkin**——数据面即限制面，四层限定全部数据驱动，无 C# 特例：

1. **装配层**：装配只执行 pack 声明（与 US 阶段同构：内置包声明 `raceDefName=Ratkin` + 15 action 音频与可选 fallback 数据）。发现只产生候选；0.3.x 仓库内唯一声明 = Ratkin → 装配表 `{Ratkin}`，其余 HAR race（如 Kiiro）无声明包 → 不装配 → 不发声，与 0.2.x 行为一致。
2. **catalog = 装配域，不是发现域**：`SqueakXenotypeCatalog` 及其通用化形态只注册已装配域条目（0.3.x = `(Ratkin, *)`，xenotype 子域沿用 `HarRatkinXenotypeDiscovery` 的 HAR `raceRestriction`/`whiteXenotypeList` 反射限定——0.2.x 现状机制）。**UI 的唯一枚举通道 = catalog 快照**（现状 `XenotypeUI` 已如此）；非 Ratkin race/xenotype 的发现结果只进独立候选列表（dev 诊断可见），不进 catalog、不进 UI。
3. **薄编程限制层（版本限定 filter，主闸）——仅限 0.3.x 的临时机制**：引入集中式产品域过滤器（如 `ProductDomainFilter`），显式把产品域主动限制在 Ratkin 域（0.3.x 常量 = `{Ratkin}`）。三处入口强制经过它：catalog 构建过滤、UI/分配器枚举投影、内置 fallback 装配。语义要点：filter 是**集中一处、白名单数据表驱动、随版本冻结**的对象，不是散落各处的 `raceDefName == "Ratkin"` 特判——机制通用、限制版本化，满足拆分门 1；数据缺席（无非 Ratkin 条目）从主防线降为次防线，filter 是结构上不可能越过的主闸。**0.4.x US 发布时移除 filter**——US 内核不存在限制域，对 HAR 种族通用兼容（任何 race 有 pack 声明即路由）；**fallback 是唯一保留 race 域匹配的地方**（内置表按 race 匹配音源以实现兜底），表外 race 无 pack = 无声。
4. **试验性兼容名单（filter 的配置化升级，0.3.x 通用化完成后引入）**：
   - **形态**：filter 名单从编译期常量改为**独立配置承载**——两份名单：default（`{Ratkin}`）与 experimental（如 `{Ratkin, Kiiro, Miho}`），由隐藏试验性开关切换（开 → 名单**替换**为 experimental，非叠加）。
   - **开关可见性**：试验性开关是隐藏设置项（Scribe 持久化，**UI 不渲染**——防过早暴露通用面 + Kiiro/Miho 许可门未过的宣传约束）；release 默认 off；dev flavor/设置文件可改（玩家手改属自行 mod 范畴）。沿用 `kiiro-experiment` 先例纪律：不宣传、不进 changelog 细节（模糊表述边界）、无专名 resolver/settings/logging 分支（名单是数据，机制仍通用）。
   - **git/发布线**：该机制是**通用形态**（配置驱动名单，非 Kiiro adapter），0.3.x 通用化完成后**直接进 dev**（不建实验分支）；随 0.3.x 末版发布（默认 off 无玩家可见影响）；0.4.x 移除 filter 时本机制一并退役（US 无限制域，名单无意义）。
   - **许可衔接**：experimental 名单内的 Kiiro/Miho 条目是 defName 数据（无资源、无 UI 展示）；公开宣传/公告仍以各自作者许可为门。
2. **域校验层**：0.3.1 后 VoicePack 声明 `raceDefName`；catalog/resolver 允许列表 = 已装配 profiles（数据），非 Ratkin 包拒绝加载 + dev 可见日志。不写 `raceDefName == "Ratkin"` 类 C# 特例。防止第三方包造成"半支持"困惑。
3. **fallback 层**：选择链末端"无声"是机制；Ratkin 有 profile 自动启用是数据。
4. **交付物防暴露审计（0.3.x 全窗口）**：页面文案不变（Ratkin 专属承诺）；作者指南不新增 profile schema 章节（US 拆分前不公开、标注未冻结）；README/changelog 不写"内部通用化"（只写玩家可见变化）；设置 UI 无新可见项；srdiag v1 冻结（race 身份等 v2）。
5. **风险护栏**：玩家手改 XML 自加 profile 属自行 mod 范畴（不受支持、文档不教）；Kiiro 实验 adapter 不 merge；0.3.1 外来 race 池实证是内部测试证据，不进交付物。
6. **阶段验证门补充**：0.3.0 装配表 = {Ratkin} + 行为等价基线 + 其他 race 零装配；0.3.1 外来 race per-race 池内部端到端实证（生产数据仍只 Ratkin）、无 profile race 默认静音实测 + Ratkin fallback 数据可验证。**UI 无泄漏断言**（0.3.0 起每阶段）：设置页/分配器渲染的 race 行 == {Ratkin}、xenotype 行 == Ratkin 限定集（快照对比 0.2.x 基线）；catalog 内非装配域条目数 == 0。

## 内置 fallback profile 存储设计（已决议）

**载体 = C# 单源 + Config 工作副本；不设分发 profile 文件**（无随包 XML、无双源）：

1. **C# 单源**：维护者数据的唯一事实源——只读目录类（如 `SqueakBuiltInFallbackCatalog`）持有 race→15 action→SoundDef defName 映射与 profile 内容版本；内置表谁进谁出 = 该类条目（0.3.x 仅 Ratkin；US 阶段加 Kiiro 即扩表，数据限定语义不变）。C# 编译期冻结：改表必编译，防未更新（XML 运行时才暴露，弃用为内置表载体）。**数据驱动不违背**：内置表是维护者控制的数据（编译期数据仍是数据）；第三方扩展面（VoicePack）保持 XML Def，两条线分开。
2. **SoundDef 本体仍随包 Def XML**（`Defs/SoundDefs/SR_*.xml`，原版引用）——RimWorld DefDatabase 加载机制边界，不进 Config；Config 副本存的是映射表结构，不含音频定义。
3. **Config 工作副本**：启动时按 packageId 隔离写入（如 `SqueakyRatkin_Profile_<race>.xml`），承载**玩家 override + 持久化**；模组更新不覆盖同版本副本。副本缺失/损坏/内容版本 < 单源版本 → 以单源重建覆盖（不合并——版本升级意味着原始表变更，旧 override 无意义）。
4. **生命周期**：启动时 DefDatabase 就绪后解析 SoundDef 引用（`GetNamedSilentFail` 校验，缺失记日志）；加载/校验/重建事件走日志（race 身份 → srdiag v2 候选，不临时塞 v1 字段）。
5. **设置面（玩家正式入口，替代 raw 文件编辑）**——三项均为 Ratkin 域内正式化（现有能力 UI 化，非通用面暴露）：
   - **fallback 路由编辑器**：设置 UI 内按 15 action 选择音源（SoundDef），保存到 Config 副本；采用 field-presence delta（复用 `XenotypeMoodOverride` 模式：只存玩家改过的 action，与单源默认合并，未改项随单源版本演进）。
   - **主动重建按钮**：设置页"重置内置 fallback 为出厂状态"→ 以单源覆盖 Config 副本（含确认提示；不触碰玩家其他设置）。
   - **原版音频浏览器下放**：`SqueakAudioBrowser` 从 Debug 迁入正式设置面（搜索/试听/选择原版音频，只引用不复制），作为路由编辑器的音源选择入口。
6. **与 0.3.x 数据限定的衔接**：单源只有 Ratkin 条目 = 非 Ratkin 无 fallback = 无声（装配面天然受限）；玩家手改 Config 副本属自行 mod 范畴（设置面为正式通道，raw 编辑不阻止）。

## 装配与发现边界

最终实现应以通用 race profile/registry 驱动，而不是以 `Kiiro_Race` 等专名 adapter 驱动：

1. **发现**只产生候选，不自动使任何 race 可发声；唯一例外是 **US 内置 fallback profile 声明的受支持 race**——那是维护者主动保底（无 pack 时路由原版音频），不是发现产物，控制权在维护者。
2. **路由零 HAR 依赖（HAR 反射 = 发现增强，非前提）**：装配/触发/派发/池选择全不依赖 HAR——pack 声明 `raceDefName` 即路由，对 HAR 种族与**非 HAR 种族（含原版 Human 等智人种）同等适用**，第三方可为任何 race（含人类）做语音包；HAR 缺失/反射失败 → xenotype 发现降级为空，US 完整运行。HAR 反射层仅用于支持 HAR 种族的异种发现（`raceRestriction`/`whiteXenotypeList`），是增强面不是前提面。
3. **装配**只对明确受支持的 profile 或玩家显式启用的 race 执行；配置来源应是 canonical `CompProperties_Squeaker` 模板，而不是把 Ratkin 的 Def 当作永久模板来源。
4. `CompSqueaker` 继续是 Harmony 派发资格；不得在 patch 中叠加第二个 race-name/`IsRatkin` gate。
5. Kiiro 分支保留为受控侦察证据；未来进入 dev 的只能是通用装配机制，不是 adapter 的 no-squash 搬运。

## 分阶段实施与验证门

| 阶段 | 交付物 | 必须成立的证据 |
| --- | --- | --- |
| 0.2.2（已启动） | 日志协议 characterization、`SqueakLog` 职责拆分、低风险去重/卫生 | 主模组 0 error；默认与 Dev flavor logging harness 通过 |
| 0.3.0 | Ratkin 触发/路由 characterization；内部 `RaceKey`/域值对象；仅 Ratkin 走新内部模型 | Ratkin 在 No-DLC 和现有设置下的行为等价；无 Scribe/日志 ABI 变化 |
| 0.3.1 | race-aware catalog/resolver：`(race,xenotype)` 与 race 池；旧 Ratkin selection 的显式、幂等迁移 | Kiiro 或另一外来 race 的**per-race**池端到端实证；不再共享 Ratkin/Example 池 |
| 0.3.2 | 设置 UI 候选与显式 opt-in 结构（UI 专项；profile 驱动装配与 per-race fallback 已在 0.3.1 落地） | UI 不泄漏非装配域；No-DLC 不访问 Xenotype 路径 |
| 拆分准备 | US/SR 设置、保存、Workshop 迁移演练 | 下列六项拆分门全部通过 |
| 拆分发布（**0.4.x 首个版本**） | US 新 item 上线 + SR 同 item 收缩为 Example VoicePack 并依赖 US；移除 ProductDomainFilter（US 无限制域） | 六项拆分门逐条可重复证据；SR→US+SR 升级路径实机通过 |

每个阶段只引入一条活运行时路径。旧路径应在迁移完成的同一变更中删除，不能用长期 shim 掩盖双实现漂移。

## 设置、存档与 Workshop 迁移

- **卸载安全硬纪律**：任何版本（SR/US）卸载后不得影响存档——存档内不写任何永久 mod 数据（def 经 XPath 运行期注入；设置/profile 副本在 Config 区）；卸载后存档正常加载游玩，仅 squeak 消失；Config 残留文件无害可删。0.3.x 迁移与 US 拆分必须保持此性质（迁移只动 Config/设置，不动存档）。

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
