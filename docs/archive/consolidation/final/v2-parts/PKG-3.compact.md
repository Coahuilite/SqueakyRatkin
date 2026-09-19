# PKG-3 compact：通用化设计、US 拆分与迁移/退役

缩写：`note`=internal-universalization-design-note-zh.md；`compat`=us-sr-compatibility-check-zh.md；`mig`=us-sr-migration-plan-zh.md。

## 0. 元数据
PKG-3 / `universalization-us-split` / frozen 2026-09-17 / repo_rev `4df9594713adbbba91e0aea788a7c7cd3503ab3f`。note=已接受规划输入非现行合同（ASM-15）；compat/mig=2026-08-23 专项（mig §8 同日追加）。confidence 中：全停规划/评估口径，U1 落地、六门通过几条、Q1–Q10 裁决均无实施回执（GAP-1/4/7）；主源含废止注记与编号断裂（CNF-2/3/4），表体不可当现行。xref：PKG-1 合同/PKG-2 0.3.x 实施/PKG-4 发布事实/PKG-6 开放裁决表；AGENTS.md、TODO/MEMORY、US 仓 `docs/mod-structure-reference-zh.md` 语料外只写指针。

## 1. 边界与结论
负责：US 目标/非目标、数据模型+两级路由、年龄基线、0.3.x 数据限定+`ProductDomainFilter`、内置 fallback 存储、阶段门+08-22 修订、六门；SR×US 兼容（E1–E12/F1–F8/组合矩阵/双开矩阵/S1–S4/U1–U4）；迁移退役（B1–B5、顺序硬门、P0–P4、三选一、各面清单、R1–R6、身份归属 A/B/C 与 Q1–Q10）。不负责：合同条文（PKG-1）、0.3.x 实施细节（PKG-2）、发布事实（PKG-4）、US 仓文档（语料外）。
一句话：先在同一 SR 程序集内做 race-aware 通用内核、用集中式版本化 filter 把交付数据锁在 Ratkin、过六门才物理拆 US；拆分技术前提=US 侧先落地跨程序集让位检测（U1）——Ratkin 同时被 SR 与 US 承认即双响；存量玩家唯一安全顺序=`U1 落地 → US 型 Ratkin 包/legacy 桥 → SR 1.0 内容化`；身份归属推荐旧 packageId 跟内容走（方案 B）。`[F]`
P0 三条：①DEC-22 顺序硬门（倒置即事故）+DEC-21 归属裁决（SR 零运行时改动、修复落 US）决定 1.0 退役窗口成败；②DEC-19 六门+DEC-12 filter（0.4.x 移除）——"机制通用、限制版本化"核心公理；③DEC-27/28 身份归属（id 跟内容走、新包用 US 原生 def 类型、U1 永久必需）+CNF-1（Q1 未裁决）。

## 2. 时间线 TL-0..9
- TL-0｜0.2.2：阶段表首行（已启动）=日志 characterization/`SqueakLog` 拆分/卫生，属 PKG-2 已实施面。
- TL-1｜0.2.x：笔记定稿=规划输入非合同、产品仍 Ratkin-only（现行仍如此）。
- TL-2｜0.2.3：玩家反馈"年龄段听感差异化"=真实需求；取证=路由与年龄无关、全库无生命阶段分支；随 0.3.x ABI 冻结。
- TL-3｜2026-08-21：1.6 年龄事实基线核验（反编译+wiki [C]）→定案映射（四值、Toddler 无原生、查表不重算）；包内唯一成文基线。
- TL-4｜2026-08-22：**路线修订（以决策文档 §5 为准）**：0.3.x SR UI 不变、原 UI 专项转入新开 US 仓、内核=US 通用状态与 US 0.3.x 并行、0.4=双仓同步上架（US 暂非 SR 依赖、SR 0.4 只 bugfix）、SR 1.0.0 才收缩为纯音频包依赖 US；**0.3.2 原 UI 专项行废止**、拆分两行"按新路线重读"——修订生效但表体未改写（CNF-2/3），引用必以修订注为准。
- TL-5｜08-23：compat（SR dev@0.3.3；US=`coahuilite.universalsqueaker` 本地 `0.4.x`@`c8794ff`+4 个未提交改动；只读审查+逐文件对拍）与 mig（背景=C-1 定调）同日出生；前提失效风险高（ASM-4/16）。
- TL-6｜08-23 追加：mig §8 身份归属（回应"SR 改 legacy+新开 pack-only SR"提案；A/B/C 推荐 B、Q7–Q10）=包内最新口径（I-2）。
- TL-7｜2026-08-24：legacy 桥薄空类例外授权日（US 文档记录、桥尚未实现；日期晚于记录文档自身→CNF-5，GAP-4）。
- TL-8｜未来：0.3.0–0.3.2 门→拆分准备（六门）→拆分发布（0.4.x 首版）→P1/P2→P3（SR 1.0 退役）→P4；全部待验证/待裁决。
- TL-9｜2026-09-17：语料冻结；08-24 后主源内无新时间层。

## 3. 决策 DEC-1..29（细节锚点见 §9；ALT 裁决见各节+速查）
- **DEC-1** 总路线：先在**同一 SR 模组同一程序集**内完成 race-aware 路由、通用装配、迁移验证，过六门后物理拆 US；ALT-1（先发空壳前置/长期双实现）明令禁止——拆分发布=单版本原子事件、六门前"US 不建 Workshop 空壳"、每阶段单活路径（旧路径在迁移完成同一变更中删除，不许长期 shim 掩盖双实现漂移）。已接受（规划层；实施未证实）；DEC-16 改"何时收缩"不改本路线。
- **DEC-2** 非目标：通用化≠什么都接；防 Kiiro 生态与许可风险；Kiiro 分支=受控侦察证据、未来进 dev 的只能是通用装配机制（非 adapter no-squash 搬运）。DEC-13 只加隐藏试验面、未解禁。
- **DEC-3** 数据模型：身份一律精确大小写敏感 DefName（显示名/包名/HAR hint/图标/本地化只作展示或候选发现）。四类键/域：`RaceKey = raceDefName`；`XenotypeKey = xenotypeDefName`；`RaceAudioDomain = (RaceKey)`；`XenotypeAudioDomain = (RaceKey, XenotypeKey)`。五级选择链（自上而下）：`(race, xenotype)` VoicePack 池 → race VoicePack 池 → pack 自带 fallback（可选）→ US 内置 fallback profile（维护者保底）→ 无声。年龄与本模型同期设计、同期冻结 XML ABI（避免第二次 Scribe 迁移）；迁移按 DEC-18 递增 schema 版本。
- **DEC-4** 两级路由：①pack 主导：每个 VoicePack 必须声明且只服务一个 `raceDefName`（Xenotype Pack 另声明 `xenotypeDefName`）；有 pack 即路由——"Ratkin 同理、Kiiro 同理、任何其他种族同理，无 US 内置种族特判"；同域 PackDef 组稳定带权公平池。②US 内置 fallback profile=维护者主动维护最终保底：精确 `raceDefName` 的 15 个 action→原版 SoundDef 引用表（只引用不复制不重分发；"谁进内置表、何时更新由维护者决策，不随社区生态漂移"）。③表外 race 无 pack=无声（显式 opt-in+静音风险提示）。④pack fallback 可选、声明优先、均缺则无声。⑤行为/mood 继承=XML comp 默认→全局设置→`(race,xenotype)` delta，与音频选择分离。⑥硬禁 C# race switch、复制原版资产。0.4.x 移除 filter 后 fallback=唯一保留 race 域匹配处。
- **DEC-5** 内置 fallback 表规划起点=**`{Ratkin, Kiiro}`**（起点非限制域）。Ratkin=双存在（SR 收缩后 Example VoicePack 声明 `raceDefName=Ratkin`+完整 15 action 与 fallback 数据、保持 0.2.x 听感基线，主导路由+在内置表兜底）；Kiiro=仅内置表兜底（无 pack 亦发声、路由原版音频），社区 pack 可补主导层。**Kiiro 许可门（逐字）**："未经作者明确许可不得发布或宣传 Kiiro compat 内容"——条目=XML 数据不含 Kiiro 资源，发布/公告以作者许可为门（Miho 同理）。"加 Kiiro 即扩表、数据限定语义不变"。
- **DEC-6** fallback 存储（已决议）：ALT-2（随包 XML 作内置表载体）弃用——"C# 编译期冻结：改表必编译，防未更新（XML 运行时才暴露）"。①C# 单源=只读目录类（**如 `SqueakBuiltInFallbackCatalog`**=规划示例名，CNF-10）持 race→15 action→SoundDef defName 映射+profile 内容版本（0.3.x 仅 Ratkin）；②SoundDef 本体仍随包 XML（`Defs/SoundDefs/SR_*.xml`，原版引用）不进 Config；③Config 工作副本按 packageId 隔离（如 `SqueakyRatkin_Profile_<race>.xml`）承载玩家 override，**模组更新不覆盖同版本副本**；缺失/损坏/版本落后→单源**重建覆盖（不合并）**（版本升级=原始表变更、旧 override 无意义）；④DefDatabase 就绪后解析（`GetNamedSilentFail` 校验缺失记日志）、race 身份→srdiag v2 候选不塞 v1；⑤设置面三项：fallback 路由编辑器（field-presence delta，复用 `XenotypeMoodOverride` 模式）/主动重建按钮（"重置内置 fallback 为出厂状态"含确认）/`SqueakAudioBrowser` 从 Debug 下放正式设置面；⑥单源只有 Ratkin=非 Ratkin 无 fallback=无声；手改副本属自行 mod 范畴。
- **DEC-7** 年龄基线（08-21，逐字见 ANCH-5）+SR 映射：`AgeBucket {Baby, Toddler, Child, Adult}` 保留四值；**查表不重算**——ALT-16（复制原版 `minAge` 自算阶段）明令禁止（双事实源漂移）；一律 `CurLifeStage.defName` 查表+表外 defName→Adult 兜底+默认 Adult；`Toddler` 1.6 无原生对应=第三方预留桶。
- **DEC-8** 年龄数据面（零迁移）：①`SqueakVoicePackAction` 加可选年龄标签（field-presence，如 `Baby`/`Toddler`/`Child`/`Adult` 或生命阶段 DefName），**未声明=全年龄**，存量包零改动零迁移，选择时按当前生命阶段过滤；②年龄调制轴独立于 mood（pitch/volume/jitter 系数）、XML 数据驱动、不写 C# age switch、与 `SqueakMoodMod` 同构叠加；③`SqueakAction` 枚举不变（append-only）；④Biotech 婴幼儿哭/笑（`MentalStates_BabyFits`，原版专属音频）作为 **Baby 年龄标签动作**纳入、与周期动作正交（0.2.4 排查产出）。随 0.3.1 XML ABI 同批冻结、不提前进 0.2.x。
- **DEC-9** srdiag v2 候选 `SettingsOrigin`（`FreshCreated`=文件缺失用默认值/`LoadedFromFile`=磁盘反序列化），区分"全新安装 vs 设置文件丢失"；ALT-17（单改 locked facade）不采——随需记录 race 身份的 v2 版本化一次扩展。候选未冻结（条文属 PKG-1）。
- **DEC-10** UI 可见性七条（内核通用≠对外表现通用）：①UI 数据驱动渲染**已装配域**（0.3.x 装配表 `{Ratkin}`、常规页投影单 Ratkin 视图）；分配器/编辑器枚举唯一通道=catalog 快照、**禁止直接枚举 DefDatabase/发现列表**；非 Ratkin 候选只进 dev 诊断；"谁有可发声内容，谁出现在 UI"；②不用 Kiiro 式专名适配器作限制层；③泄漏面审计（设置 schema race 域=内部格式玩家不可见；race 身份进 v2；不新增公开泄漏）；④可见性宣告点=US 拆分发布（"单版本原子事件"→CNF-2）；⑤0.3.2 交付"域渲染框架+显式 opt-in 结构"（入口仅在存在可装配非默认 race 时出现）——行被 DEC-16 废止、能力口径保留；⑥社区多 race 请求（绮罗/沃芬/美狐）=US 阶段信号不提前承诺；⑦设置面例外三项（=DEC-6⑤）。
- **DEC-11** 0.3.x 数据限定：机制全通用、交付数据只有 Ratkin（0.4.x 移除；"数据面即限制面"）。ALT-3（数据缺席作主防线）降为次防线（外来条目一现即失效；仍有效作次防线）；ALT-18（C# 各处 `raceDefName == "Ratkin"` 特判）明令禁止。七条（源编号断裂→CNF-4）：装配层（只执行 pack 声明→0.3.x 装配表 `{Ratkin}`；发现只产生候选；Kiiro 等无声明包→不装配→不发声=与 0.2.x 一致）；catalog=装配域非发现域（0.3.x=`(Ratkin, *)`；xenotype 子域沿用 `HarRatkinXenotypeDiscovery` 的 HAR `raceRestriction`/`whiteXenotypeList` 反射限定；UI 唯一枚举通道=catalog 快照；非 Ratkin 发现只进独立候选列表 dev 可见）；薄编程限制层（=DEC-12）；试验性兼容名单（=DEC-13）；域校验层（0.3.1 后 VoicePack 声明 race；允许列表=已装配 profiles 数据；非 Ratkin 包拒绝加载+dev 可见日志；防半支持困惑）；fallback 层（末端无声=机制；Ratkin 自动启用=数据）；交付物防暴露审计+风险护栏+阶段验证门补充。
- **DEC-12** **`ProductDomainFilter`**（示例名 U-4）：集中一处、白名单数据表驱动、随版本冻结的主闸（限制从散落特判升级为"结构上不可能越过"；"机制通用、限制版本化，满足拆分门 1"）；0.3.x 常量=`{Ratkin}`；**三处入口强制经过**=catalog 构建过滤/UI 与分配器枚举投影/内置 fallback 装配。**0.4.x 移除**（交付物明列"移除 ProductDomainFilter（US 无限制域）"）；0.4.x 语义：任何 race 有 pack 声明即路由、fallback 唯一保留 race 域匹配、表外 race 无 pack=无声。
- **DEC-13** 试验性兼容名单：ALT-4a 编译期常量 vs **ALT-4b 配置化双名单**→采 4b（4a 仍是 default 载体与 0.3.x 起点）：default `{Ratkin}` ↔ experimental 如 `{Ratkin, Kiiro, Miho}`，隐藏开关切换**替换非叠加**；隐藏设置项（Scribe 持久化、**UI 不渲染**；release 默认 off；dev flavor/设置文件可改，玩家手改属自行 mod）；沿用 `kiiro-experiment` 纪律：不宣传、不进 changelog 细节、无专名 resolver/settings/logging 分支（名单是数据、机制仍通用）；0.3.x 通用化完成后直接进 dev（不建实验分支）、随 0.3.x 末版发布；Kiiro/Miho 条目=defName 数据（无资源无 UI 展示）、公开宣传仍以各自作者许可为门；0.4.x 随 filter 一并退役。
- **DEC-14** 装配与发现边界五条：①发现只产生候选、不自动使任何 race 可发声（唯一例外=内置 fallback 声明的受支持 race=维护者主动保底非发现产物）；②**路由零 HAR 依赖**：装配/触发/派发/池选择不依赖 HAR；pack 声明 `raceDefName` 即路由、HAR 与非 HAR（含原版 Human 等智人种）同等适用、第三方可为任何 race（含人类）做语音包；HAR 缺失/反射失败→xenotype 发现降级为空、US 完整运行（HAR 反射=发现增强非前提）；③装配只对明确受支持 profile 或玩家显式启用 race 执行；来源=canonical `CompProperties_Squeaker` 模板（ALT-15：不把 Ratkin Def 当永久模板；模板化装配形式保留）；④`CompSqueaker` 继续是 Harmony 派发资格、**不得在 patch 叠加第二个 race-name/`IsRatkin` gate**；⑤ALT-19（专名 adapter/Kiiro no-squash 搬运）禁止。
- **DEC-15** 阶段验证门（交付=证据）：0.2.2（已启动）日志 characterization/`SqueakLog` 职责拆分/低风险去重卫生=主模组 0 error、默认与 Dev flavor harness 过；0.3.0 Ratkin 触发路由 characterization+内部 `RaceKey`/域值对象、仅 Ratkin 走新模型=Ratkin 在 No-DLC 和现有设置下行为等价、无 Scribe/日志 ABI 变化；0.3.1 race-aware catalog/resolver（`(race,xenotype)` 与 race 池）+旧 Ratkin selection 显式幂等迁移=Kiiro 或另一外来 race **per-race** 池端到端实证、不再共享 Ratkin/Example 池；0.3.2 设置 UI 候选+显式 opt-in（原 UI 专项）=UI 不泄漏非装配域、No-DLC 不访问 Xenotype 路径；拆分准备=US/SR 设置、保存、Workshop 迁移演练=六门全过；拆分发布（0.4.x 首个版本）=US 新 item 上线+SR 同 item 收缩为 Example VoicePack 并依赖 US、移除 ProductDomainFilter=六门逐条可重复证据、SR→US+SR 升级路径实机通过。ALT-20（长期 shim/双实现共存）禁止。0.3.2 行被 DEC-16 废止、后两行"按新路线重读"（CNF-2/3）。
- **DEC-16** 08-22 路线修订（六项口径见 TL-4）：ALT-5 原路线（0.3.2 在 SR 做 UI 专项；0.4=US 上线与 SR 收缩同时完成的原子事件；US 即 SR 前置）被取代；吸收=0.3.2 能力口径、"UI 无泄漏断言"与"数据限定四层"继续有效、六门不豁免。**废止**：0.3.2 原 UI 专项行；"US 暂不作为 SR 依赖"（后被 DEC-24 P3 取代：SR 1.0 声明 US 前置——不同窗口时序非矛盾，引用必带阶段标签）。所引"决策文档 §5"未具名、原文未读到→悬置不代拟（U-6/GAP-10；编排方转述疑指 PKG-2 主源 §5=简称歧义，本包不核）；修订六项口径在修订注内完整可溯、本 DEC 结论不依赖被引文档。与 mig §2/§3 一致（P1=0.4 同窗、P3=1.0 退役）。
- **DEC-17** 两硬纪律：①卸载安全——任何版本（SR/US）卸载不得影响存档：存档内不写任何永久 mod 数据（def 经 XPath 运行期注入、设置/profile 副本在 Config 区）；卸载后存档正常加载游玩、仅 squeak 消失；Config 残留无害可删；②迁移不得写存档——0.3.x 迁移与 US 拆分只动 Config/设置；F1 修复不得引入存档写入；P3 验收必复测"卸 US/卸 SR"两向。仓库级现行条文语料外→`[xref: AGENTS.md（小节名未核）]`。现行有效。
- **DEC-18** 迁移方式：①当前 Race 选择显式迁为 `RaceAudioDomain(Ratkin)`、Xenotype 行为/音频目标迁为 `(Ratkin, xenotypeDefName)`；②**递增 schema 版本**实现、必须幂等、可重启、失败不丢旧记录；③迁移前存真实旧设置 fixture；迁移后验证 immediate runtime publish、约 350 ms 合并保存、窗口 close flush、无 Biotech 安全降级、orphan/dormant 语义；④物理拆分时 US 用新 packageId、SR 保持现有 packageId 和 Workshop item、变为依赖 US 的 VoicePack；⑤**必须在真实 save modlist 上验证 `SR → US + SR` 升级路径；不得把 staging 成功当作订阅、保存或 Workshop 状态的证据**。mig §8.1 精化：唯一真正影响正在使用玩家的持久化资产=**PackKey 选择+设置文件**（存档无影响）。
- **DEC-19** **六门（逐字）**，总纲"以下每项都有**可重复证据**后才创建独立 US"：**1.** 逻辑层零 Ratkin 硬编码（数据/fallback profile 除外）；**2.** 至少一个外来 race 完成 per-race pool 的端到端验证；**3.** Ratkin characterization 在内部重构后持续全绿；**4.** 旧 SR settings → US 的迁移设计与实机验证完成；**5.** 保存兼容：`SR → US + SR` 升级路径通过；**6.** SR 同 Workshop item 收缩为 VoicePack、US 新 item、依赖/页面/Claim Pack 的过渡演练完成。ALT-21（先建空壳/提前发布）禁止："达到六门之前，SR 仍是唯一可发布的实现载体；US 不建 Workshop 空壳，Kiiro 不作宣传或发布承诺"。通过条数无记录（GAP-7）。
- **DEC-20** 双开安全判定：①**当前双开安全**——US catalog 只枚举 `UniversalSqueaker.SqueakVoicePackDef`、SR 包是另一程序集/类型 `SqueakyRatkin.SqueakVoicePackDef`；US 无 legacy 桥实现、无 Ratkin 字面量⇒只有 SR 在 Ratkin 上发声、各响各的互不串音；②**落地即双响**——US 逃逸门只认自己程序集的 `UniversalSqueaker.CompProperties_Squeaker`、SR XML patch 挂 `SqueakyRatkin.CompProperties_Squeaker`、类型不匹配⇒US 在 Ratkin `ThingDef.comps` 上**追加第二个** squeak comp；16 patch 有 15 同名同目标、各只 `GetComp<自己类型>()`⇒每事件双响、周期采样双跑；③组合矩阵六行（Ratkin 视角）：仅 SR=单响（现状）；仅 US=不响（无内置 Defs 需外部 US 型包，现状）；SR+US 无 US 型 Ratkin 包且桥未实现=单响仅 SR＝**当前双开安全**；SR+US+US 型 Ratkin 包=双 comp→**双响**（F1/F2，首发包落地即触发）；SR+US 桥已实现（US 承认 SR 旧包）=双 comp→**双响**（F3，桥落地即触发）；SR 1.0（纯音频无 DLL）+US=单响＝目标态；④F6/F7/F8（低 severity）"无需改动"、**保留为回归断言**：设置窗互不劫持（E10）、配置文件互不覆盖+诊断独立（E12）、无 Def/类型/Action 键/日志 once-key 冲突（F7，保留 E4/E12 为验证项）、两侧只改内存 `ThingDef.comps` 不写存档（F8）。双响是**预测**非已发生事故（I-3）。
- **DEC-21** 归属裁决："存量玩家约束下的唯一解"：SR 本窗口**零运行时改动**——只做冻结兼容面（`SqueakyRatkin.CompProperties_Squeaker`/`CompSqueaker` 全名不变、XML 装配契约不变、不写存档）+文档（任何 SR 行为改动立刻作用于存量玩家）；修复落 US（ASM-4 零玩家成本零；B1/B2 存档面天然安全）。ALT-6（SR 反向让位：检测到 US 就不装配）被 **B5 否决**——"只装 US 无 Ratkin 包"时 Ratkin 变哑、直接打在存量玩家身上；SR 仍负 S1–S4 冻结义务。ALT-7：单装配=唯一干净根因解、短期互认门（双方跳过"对面已服务"种族）=底线。唯一允许先落的运行时改动=US 侧 U1、且必须早于任何"US 服务 Ratkin"动作。DEC-28 把 U1 重分类为永久必需（重分类非反转）。
- **DEC-22** 退役窗口前三件事：①Ratkin 装配**唯一写者**（跨程序集检测）；②legacy 桥**类型名所有权**+两 DLL 同时在载重叠期；③**两仓规则冲突**（US 文档"0.4 不允许 Ratkin 装配"↔维护者"Ratkin 已是首发支持包之一"=F4/Q1→CNF-1）。**顺序硬门（逐字）**：`U1 检测落地 → US 型 Ratkin 包 / legacy 桥启用 → SR 1.0 内容化（DLL 退役）`。倒置后果：**R1** 先上 Ratkin 包/桥⇒存量双装玩家**双响**（"最严重"）；**R2** SR 1.0 先于 US 的 Ratkin 能力⇒存量玩家更新后 **Ratkin 静默**；**R3** 重叠期两 DLL 同时定义 `SqueakyRatkin.SqueakVoicePackDef`⇒XML `Class=` 解析 **first-wins**、旧包走错 upcast。ALT-22（不设硬门）禁止；硬门同时进 US 发布门（U4/R1 缓解"两侧发布门断言"）。
- **DEC-23** 行动清单。**SR 侧**：S1 把"Ratkin 装配唯一写者"写进 SR 合同与 MEMORY（0.4 共存期 Ratkin 由 SR 装配、US 服务其他种族，或按 Q1（compat）反向）；S2 保持 `SqueakyRatkin.CompProperties_Squeaker`/`SqueakyRatkin.CompSqueaker` 全名稳定（不迁移、不改名、不拆类型）；S3 1.0 窗口 SR 退化为纯音频包时确认桥与 `SR_*` SoundDef 交叉引用可用、声明 US（与 FerriteLib）前置链路；S4 与 US 对齐 `SqueakyRatkin.SqueakVoicePackDef` 重叠期所有权。**US 侧（需转述）**：U1 `VoicePackCompAttach` 逃逸门改为跨程序集"已存在任意 squeak comp 即跳过"+可核验 skip 原因（如 `foreign_squeak_comp`）；U2 按 Q1（compat）落实"是否服务 Ratkin"、若服务则 U1=**发布硬前置**；U3 桥启用前置（SR 程序集不存在/运行时不加载）+类型名所有权约定；U4 双开矩阵纳入 US 发布门（与 SR 侧同步跑）。**F1–F8 归属**：F1 高（条件性）→U1/U2；F2 高（同源；15/16 patch 双派发、听感"回声/重音"、两套设置冷却各自计时）→根因同 F1；F3 高（窗口期）→桥启用条件与 SR 程序集在载互斥+重叠期类型名统一（U3/S4）；F4 中=规则冲突→维护者裁决 Q1（compat §6）；F5 中=依赖链 `SR → US → FerriteLib`（0.4 共存期 US 单独就要求装 FerriteLib）→两侧 About/README 写清+SR 1.0 声明 US 前置时确认三级链路（S3）；F6/F7/F8 低→无需改动、保留回归断言。ALT-8（类型全名=零新依赖脆弱度低 vs 共同标记接口=更干净但需 SR 1.0 依赖 US 后实现）→待 Q2（compat）；无论哪种 S2 全名稳定先做。US 侧落地无证据（GAP-4/OQ-6）。
- **DEC-24** **P0–P4（逐字阶段名）**：**P0（现在，SR 0.3.3）** SR 照常、US 落地 U1（可先于任何 Ratkin 支持）；方=US；影响=0；公告=无；退出=U1 有测试/日志证据、US 自测 Ratkin 跳过。**P1（SR 0.4 + US 0.4 同窗）** US 服务其他种族并对 Ratkin 让位、SR 不动（只发已有行为）；影响=0；公告="两 mod 可同时启用；Ratkin 由 SR 提供，其他种族交给 US"；退出=§5 双开矩阵全绿。**P2（过渡期，建议 ≥4–8 周，SR 0.4.x patch 层）** US 提供一次性设置导入（可选实现）、备好 Ratkin 能力但**内容包先不发**、SR 只在设置页/说明加一行"未来需要 US"且**不改发声行为**；影响=提示级；公告=1.0 时间表与影响（需 US+FerriteLib）+替代路径三选一；退出=导入或"重设"路径任选其一有实机证据、公告已发满窗口。**P3（SR 1.0 退役）** SR 内容化（无 DLL、不引用 US 类型）、US 启用 legacy 桥承接 SR 包（`SqueakyRatkin.SqueakVoicePackDef`）、Ratkin 由 US 装配；方=SR+US；影响=**行为等价切换**（Ratkin 仍响、音源与选择经 US 路由），只装 SR 不装 US 者静默；公告=前置 ≥1 个完整窗口+Workshop 描述与更新日志明确；退出=US 单装+SR 包可发声、双装无重复、旧存档正常、1.0 依赖缺失=警告+静默而非报错。**P4（收尾）** SR 仓库只维护内容/ABI、旧独立线冻结；公告=收敛说明；退出=旧线无未决兼容问题。ALT-9（插 SR 0.5 过渡版保持独立可用+迁移提示 vs 只用 0.4.x patch）→待 Q1'（mig）；ALT-10（一次性导入（反射读 SR 设置）vs"重设一次"公告）→待 Q3'（mig）；若做导入=失败不阻断启动、结果在 US 设置可核对。P0 退出是否达成无证据。
- **DEC-25** 不迁移玩家三条路（**必须在 1.0 公告里写清**；三选一是公告内容要求）：ALT-14 **①另开冻结 legacy Workshop 条目**（如 "Squeaky Ratkin (Legacy)"、冻结在最后独立版本、建议 ≤0.5.x；代价=1.6→1.7 需偶尔兼容维护，R6 缓解=预留最小维护预算或公告"不承诺跨版本"）；**②GitHub 保留旧 tag/资产+Workshop 描述给手动安装指引**（代价=普通玩家门槛高）；**③什么都不做**（1.0 自动更新后必须装 US+FerriteLib 否则 Ratkin 静默）——与"不影响存量玩家"冲突、不推荐作为默认。**建议=1+2 组合**。选项 1 与 Q9'（mig §8.5）同一裁决两次登记；选项 1≈方案 A/C 的 legacy 承载方式。
- **DEC-26** 各面清单：**存档**=comp 不进存档（B1）两侧都不写；迁移改动不得引入任何 Scribe 写存档、沿用卸载安全。**设置**=SR `Mod_coahuilite.squeakyratkin.xml`（**schema 4/2**：`voicePackSelections`/`xenotypePresets`/`moodOverrides`/`globalActionEnabled`…）vs US 分层调音新模型；P2 一次性导入或"重设一次"；SR 文件**保留不删**、导入失败不阻断启动、结果在 US 设置可核对。**音频包（作者面）**=SR XML ABI 已冻结（包用 `Class="SqueakyRatkin.SqueakVoicePackDef"`）、P3 桥承接、**作者不需要改写**、桥须兼容 `raceDefName`/`ageTag`/`IsEgg`/`fallbacks`、R5 缓解=作者指南 P3 更新、旧类型经桥继续有效不强制改写。**mod 列表/依赖**=SR 无前置、US 硬前置 FerriteLib、P3 SR 声明 US（传递）；SR 1.0 **不编译引用 US 类型**→缺前置=警告+静默；依赖链两侧 About/README 写清（F5/S3 同源）。**日志/诊断**=srdiag 与 usdiag 两套协议**不合并**（ALT-23"窗口内变化：不合并"）、迁移期以 usdiag 为准、SR 旧协议随 DLL 退役。**卸载**=任一侧卸载不影响存档；P3 必复测两向。
- **DEC-27** 身份归属（mig §8 追加，回应"SR 改 legacy+新开 pack-only SR"提案）：**packageId 应该跟内容走**。§8.1 绑定面清单（逐字要点）：音源选择 PackKey=`modContentPack.ModMetaData.PackageIdNonUnique + ":" + defName`（SR/US 同构，`SqueakVoicePackModels.cs` `TryGetPackKey`；持久化 `voicePackSelections.enabledPackKeys`）→换 id⇒已选 PackKey 全变 **orphan**（保留但不再解析）⇒音源被重置需重选；设置文件按 packageId 命名⇒新 id 读不到旧文件设置全丢（除非导入）；fallback 副本⇒`SqueakFallbackProfileStore` 以 `SqueakyRatkinMod.PackageId` 校验/命名⇒旧副本失效重建；Harmony 实例 id（`Harmony = new Harmony(PackageId)`）/日志身份⇒无实质影响；第三方 `IfModActive` 门与 `Patch_ModMetaData_LocalizedMetadata`（`SamePackageId`）⇒外部兼容补丁指向旧 id、跟着 legacy 走；存档⇒无影响。§8.2 两条引擎事实：重复 packageId→`ModLister.TryAddMod`→`Log.Error("Tried loading mod with the same packageId multiple times … Ignoring the duplicates")` 只有一个被加载（Steam 副本走 postfix 但 `PackageIdNonUnique` 仍是原始小写 id⇒PackKey 模糊）⇒"两个 item 共用 `coahuilite.squeakyratkin`"不可行；重复 defName→`DefDatabase.Add` 报错并**给重复 defName 追加随机数字后缀**⇒PackKey 被改名⇒老选择失配⇒legacy 与新线**不能同时启用**（除非 legacy 改 defName=改 PackKey 与作者引用，代价更大）。ALT-12：方案 A（旧 item=legacy 留旧 id、新 item=pack-only）⇒迁移玩家**音源选择重置**+设置不跟随、两 item 长期并行→**不推荐**（若仍选 A 须额外做三件事：US 侧"旧 PackKey→新 PackKey"映射导入、legacy 与新线互斥声明（defName 冲突）、新线包用 US 原生类型）；**方案 B（推荐）**=同一 item/同一 id 就地升级为 pack-only、legacy=**新 id 冻结归档**（或仅 GitHub，建议 `coahuilite.squeakyratkin.legacy`）⇒迁移玩家**零损失**（PackKey/设置文件/订阅/第三方门全不变）、legacy 玩家手动换条目（"他们本就在主动退出框架"）；方案 C（最小）=不提供 Workshop legacy、只留 GitHub 归档、旧 id 给主线=**B 的降级选项**。前提反转见 CNF-8；正式裁决待 Q7'（mig，OQ-13）。
- **DEC-28** 新 pack-only 线包用 **US 原生 def 类型**（`UniversalSqueaker.SqueakVoicePackDef`；ALT-13 用 SR 旧类型被否——legacy DLL 在装时 `Class=` 可能解析到 legacy 类型⇒US catalog 不认该包；SR 旧类型经桥继续有效=作者零改写）。**只要 legacy 长期在线，US 侧 U1（跨程序集让位检测）就从"过渡期修复"升级为"永久必需"**——"分裂方案不会让兼容工作消失，只会让它常态化"（CNF-7）。与 Q10'（mig）挂钩。
- **DEC-29** 桥字段兼容义务（作者零改写成立条件）：P3 桥必须兼容旧包字段 `raceDefName`/`ageTag`/`IsEgg`/`fallbacks`（`ageTag` 与 DEC-8"年龄标签"=同一数据面两处记录→I-4）；重叠期类型名归属受 DEC-22 硬门约束；R3 缓解=发布顺序纪律+两侧发布门断言"桥只在 SR DLL 退役后启用"。桥尚未实现（GAP-4）。

**ALT 速查**（ALT-11=编号空洞、从未定义、勿代拟）：1 空壳/双实现→禁止｜2 随包 XML 载体→弃用｜3 数据缺席→降次防线｜4a 常量 vs 4b 双名单→4b｜5 原路线→被取代｜6 SR 反向让位→否决（B5）｜7 单装配 vs 互认门→单装配根因解｜8 全名 vs 标记接口→待 Q2（compat）｜9 插 0.5→待 Q1'（mig）｜10 导入 vs 重设→待 Q3'（mig）｜12a A→不推荐/12b B→推荐/12c C→B 降级项｜13 US 原生类型→采｜14 三选一→建议 1+2｜15 canonical 模板→采｜16 重算 minAge→禁止｜17 单改 facade→不采｜18 C# 特判→禁止｜19 专名 adapter→禁止｜20 长期 shim→禁止｜21 先建空壳→禁止｜22 不设硬门→禁止｜23 合并诊断协议→不合并

## 4. 假设 ASM-1..16（ASM-3/4/5=B1/B4/B3；B2→ASM-14；B5→DEC-21/ALT-6）
**B1–B5（mig §1 五条事实基线）**：B1 comp 根本不进存档（`ThingWithComps.ExposeData()` 只调 base、`LoadingVars` 按当前 def 重建 comps）；B2 依赖缺失=mod 列表警告（`ModUnsatisfiedDependency`）不崩档——前提 SR 1.0 不编译引用 US 类型 [C]；B3 Workshop 订阅自动更新、玩家无法停在旧版 [C]；B4 US 当前零玩家、零内置内容包（`1.6/Defs`、`1.6/Patches` 空）；B5 SR 反向让位会打在存量玩家身上（修复方向唯一）。
- ASM-1 US=SR 分叉（16 patch 有 15 同名同目标、各绑自己程序集类型⇒双 comp 双派发）｜无变更证据
- ASM-2 类型身份互不可见（E4：挂载集合=被承认包 `raceDefName` 去重并集）｜桥落地即改变
- ASM-3=B1 comp 换型/移除对存档零影响、无残留警告｜对 1.6 成立 [C]、1.7 未验证（Q6'/R6）
- ASM-4=B4 修复放 US 成本为零、放 SR 直接打存量玩家｜**时效性**：US 发布即失效
- ASM-5=B3 退役只能靠长公告+时间窗+不受影响的替代路径｜现行 [C]
- ASM-6 Kiiro 实验只证生命周期与触发链、**不证音源按种族隔离** [C]｜证据边界自限
- ASM-7 现状不足以表多种族域：`SqueakRuntimeResolver` 有 Global/Race/Xenotype 三层概念但 Race packs=全局池、Xenotype key 只有 `xenotypeDefName`；`VoicePackSelectionRecord` Scribe schema=迁移边界｜无实施回执
- ASM-8 逻辑层可零 Ratkin 硬编码（"机制通用、限制版本化"）｜未验证=拆分门 1
- ASM-9 pack 声明 `raceDefName` 即路由、HAR 与否同等；HAR 失败只降级 xenotype 发现｜未验证=拆分门 2
- ASM-10 1.6 年龄判定全 Core 数据驱动、阈值只在 race XML、无 C# 硬编码；`DevelopmentalStage` 无 Toddler｜对 1.6 现行 [C]（`Pawn.cs:2022`）
- ASM-11 `SqueakLog` facade 与 `srdiag v1` 已被 `tools/SqueakLogCharacterization` 锁定；记 race/年龄身份须先设计新协议版本｜现行（v1 冻结）
- ASM-12 跨程序集检测只能靠类型全名或共同标记接口⇒SR 兼容面=全名稳定（S2）；SR 改名/拆类型则 U1 失去锚｜至 Q2（compat）选定
- ASM-13 两 DLL 同名类型时 `Class=` 解析加载序 **first-wins**｜**未验证推断 [C]/[I]**；R3 靠发布门断言
- ASM-14=B2｜条件成立
- ASM-15 笔记=规划输入非合同、受影响结论先提升进合同｜现行
- ASM-16 评估基准=US `0.4.x`@`c8794ff`+**4 个未提交改动**、SR dev@0.3.3｜不可复现（OQ-18）

## 5. 认识论 C/I/U
- C-1 维护者 08-23 定调："SR 是已有玩家的模组，需要足够的退役时间且不得影响正在使用的玩家；US 是框架级继承者"（两文档共引；未见独立记录）。
- C-2 维护者口径"Ratkin 已是 US 首发支持包之一"（SR 侧转述）↔US 仓文档 0.4 共存规则=CNF-1 双方。
- C-3 笔记作者自称 08-21 基线经"RimSage 反编译源码+wiki 一手核验"、mig B1/B2 亦据 RimSage——本包不可核 [C]。
- C-4 Kiiro 实验"已证明"（记录不在主源、边界自限）[C]。
- I-1 拆分/退役**实际阻塞点是治理而非技术**：顺序已定（U1 唯一先行）→F4/Q1 未裁使 U2 悬置→身份问题收敛为 Q7–Q10⇒关键路径=维护者裁决集（替代解释：语料外已裁决，GAP-7）。
- I-2 mig §8=身份归属最新时间层（自标追加+重分类 U1；替代解释：同日追加不构成覆盖→按并存处理，CNF-7）。
- I-3 F1/F2/F3 双响与"SR 1.0 先行→静默"是**预测非已观察事故**（全为只读审查+矩阵推演、验收矩阵未跑）⇒不得写进"已发生问题"清单。
- I-4 `ageTag`（mig §5）与 DEC-8"可选年龄标签"=同一数据面两处记录（替代解释：或指 US 已实现另一字段，无证据）。
- U-1 Q1（compat）缺**裁决本身**；U-2 六门/阶段门通过几条缺实机 CI 证据；U-3 笔记章节多无日期→无法排约束新旧；U-4 `ProductDomainFilter`/`SqueakBuiltInFallbackCatalog`/`SqueakLifeStageResolver`/试验名单以"如"引出=**示例名**（按符号名检索源码可能落空）；U-5 缺 SR→US 转述回执；U-6 "决策文档 §5"悬置不代拟（编排方转述疑指 PKG-2 主源 §5=简称歧义）。

## 6. 矛盾 CNF-1..10
- **CNF-1（不得归一）**：US 是否服务 Ratkin（=Q1 compat §6）。US 仓 `docs/mod-structure-reference-zh.md`（**语料外**、经 E8 转述）"0.4 共存规则：US 仓库内不允许 `SqueakyRatkin.*` 类型、`SR_` Def、Ratkin 装配/profile/attachment"（唯一例外=授权 2026-08-24 的 legacy 桥薄空类 `SqueakyRatkin.SqueakVoicePackDef`）↔维护者口径"Ratkin 已是 US 首发支持包之一"。两条不能同时为真；不裁决则 F1 修复方向无法定。[I] "仓库内手工内容/装配"与"经外部 pack/桥**承认**"两义被一句压扁；例外条款说明规则已朝"允许"松动。权威序上维护者口径高于仓库文档，**但冲突一方在语料外（GAP-3）→冲突保留、不归一**；回查=US 仓现行文本+裁决 [xref: PKG-6]。
- CNF-2：note §UI 第 4 条"可见性宣告点=US 拆分发布（**单版本原子事件**）…**不存在双实现共存窗口**"（无日期）↔08-22 修订注"0.4=双仓同步上架（US 暂不作为 SR 依赖）…SR 1.0.0 才收缩并依赖 US"。[I] 修订把宣告点移到 US 上线、共存窗口=0.4→1.0 有意设计；compat 矩阵与 mig P1/P3 按修订排布。回查"决策文档 §5"（GAP-10）+PKG-4 发布序列。
- CNF-3：同文档阶段表体"拆分发布（**0.4.x 首个版本**）…移除 ProductDomainFilter"↔修订注（SR 1.0.0 才收缩）。"以注代改"时间层并存；表体旧路线验收要求（移除 filter、升级实机过）修订后仍可能有效、绑定版本变了。**引用该表必须同时引修订注**。
- CNF-4：note §数据限定"**四层限定**"↔实际枚举 7+ 条且编号从 2 重断（装配 1/catalog 2/薄层 3/名单 4→域校验 2/fallback 3/审计 4/护栏 5/门补充 6）。**下游不得把"四层"当可核验计数**（引用引条目名不引计数）；替代定义=filter 三处入口；GAP-6。
- CNF-5：compat 前言"日期：2026-08-23"↔E8 记录授权"2026-08-24"⇒文档 08-24 后追加/改写过或日期不可靠。[I] E8 后补/US 文档记录口径/笔误。git 提交时点核对由编排方自行只读执行（本包不做外部取证）。
- CNF-6：compat F4"不裁决则 F1 的修复方向无法定"↔mig §2"归属与顺序（存量玩家约束下的**唯一解**）：修复必须落在 US 侧"。[I] 层次不同——compat 指**Ratkin 内容侧归属**（谁装配=S1 方向）、mig 指**修复落点**（改哪侧）；可并存；转述必须区分"修复方向"与"服务对象"。
- CNF-7：compat 把 U1 定位为过渡期修复（"退役窗口前必须解决"）↔mig §8.4 结论 3"只要 legacy 长期在线，U1 升级为**永久必需**"。[I] 非推翻而是**条件化**（仅当选 A/Q9'（mig）上 Workshop legacy 时成立）。最终包两口径并列：无条件必做（DEC-22）+条件化永久化。
- CNF-8（**反转**）：提案前提"无法继承 packageId"↔mig §8.4 结论 1"**『无法继承 packageId』不是必然，而是方案 A 的前提造成的**；packageId 应该跟内容走"——从"必须换 id"反转为"旧 id 跟主线、legacy 拿新 id"。[I] 隐含前提=Workshop 同 item 原地升级可行（语料只论证 id/defName/PackKey 层，平台层未取证→GAP-11）。回查 PKG-4 先例；OQ-13。
- CNF-9（**命名空间冲突**）：compat §6 Q1–Q4 与 mig §7 Q1–Q6+§8.5 Q7–Q10 共用"Q"前缀但指代不同（compat Q1=US 是否服务 Ratkin vs mig Q1'=是否插 0.5；compat Q3=桥上线重叠期 vs mig Q3'=设置导入）。本包 OQ-n 重编号+双标识；**下游转述（含 US 侧 issue/公告）必须为每个 Q 带文档限定**，否则串号。
- CNF-10（**跨包符号名分歧**）：主源逐字「只读目录类（**如 `SqueakBuiltInFallbackCatalog`**）持有 race→15 action→SoundDef defName 映射与 profile 内容版本」("如"=示例名 U-4）↔**编排方转述**：合同逐字类名=**`BuiltInFallbackCatalog`**（合同 §6 属 PKG-1 主源、本包未读原文）。差一个 `Squeak` 前缀。[I] 示例名实现时定稿为合同名；权威序合同>规划笔记，**若必须取一建议以合同逐字名为准——但冲突记录保留、主源记录不得被改写**；由 PKG-1 核名；最终包凡引须同时标"规划示例名/合同现行名"；`ProductDomainFilter`/`SqueakLifeStageResolver` 同降级为"规划期示例名"。

## 7. 事故/证据 INC-1..5 / EV-1..7（E1–E12 分布注：E1–E4→EV-4；E5→EV-1；E6→EV-3；E7→EV-2；E8→TL-7/CNF-1/5；E9–E12→EV-5）
- INC-1 Kiiro 实验部分验证（可进共享触发漏斗）→受控侦察证据、分支不 merge [C]。
- INC-2 0.2.3 婴幼儿默认听感问题+取证"路由与年龄无关"→"默认启用内置包"缓解、需求进 0.3.x ABI 同期设计。
- INC-3 0.2.4 前 Biotech 哭/笑误报精神崩溃→hook 收窄至 `MentalBreakWorker.TryStart` 已修复；0.3.x 以 Baby 标签动作纳入不影响该修复 [C]。
- INC-4 正常 fresh 路径无日志（框架静默 `new T()`；locked 28 事件无此项）→DEC-9 候选。
- INC-5 **预测性风险（未发生）**：F1/F2/F3 双 comp⇒双响/双冷却/双诊断；修复归 US（U1/U3）；验收=§5 矩阵"F1 修复后必须全绿"。只读审查+推演非实机——下游必须按"预测"引用（I-3/GAP-1）。
- EV-1 方法学=只读审查+**逐文件对拍**→E1–E12+F1–F8+§3 组合矩阵+§5 验收矩阵=DEC-21/22 证据底座；E4/E12=F6/F7 保留验证项。
- EV-2=**E7 否证**：US 全仓 grep `Ratkin`/`SqueakyRatkin`/`SR_` 零命中、`1.6/Defs`、`1.6/Patches` 空⇒无内置内容、无 Ratkin 特判、无 SR 检测（支撑 B4/当前双开安全）。
- EV-3=**E6 同基线**：US `CompProperties_Squeaker.CreateDefault()` 与 SR XML 同值（`216 t`；`Eat EachTime 144`；`Call 864/1.2%`；`Move 504/1.2%`…）⇒双响=**同节奏双发**非互补⇒听感后果不可接受。
- EV-4=**E1–E4 装配机制**：Harmony 派发按 `CompSqueaker` 存在性工作；US 装配在 `ExecuteWhenFinished`（晚于 XML patch）⇒能"看到"SR comp 但类型不匹配仍重复挂；`Mod.cs:103` `VoicePackCompAttach.Apply(SqueakXenotypeCatalog.Current)`；逃逸门判据=`def.comps.Any(comp => comp is CompProperties_Squeaker)`、SR comp 在位时判 false→追加挂载记 `CompAutoAttached`；挂载集合=被承认包 `raceDefName` 去重并集⇒直接导出 U1 修复形式。
- EV-5=**E9–E12 阴性面**：US 硬前置 `coahuilite.ferritelib`（`<modDependencies>`+代码版本断言）；`Patch_RedirectModSettingsWindow.cs` 只拦 owner=`UniversalSqueakerMod`；两仓 `LoadFolders.xml` 均 `/`+`1.6` 无 `IfModActive`；配置文件各带 packageId、Scribe 类型两命名空间⇒F5/F6/F7 结论基础。
- EV-6 RimSage 二手定位（`Verse/ThingWithComps.cs`、`Verse/ModsConfig.cs`）+§8.1 代码身份⇒"迁移唯一持久化风险=PackKey+设置文件" [C]。
- EV-7 B3 平台行为无一手证据 [C]⇒P2 窗口"建议 ≥4–8 周"与三选一要求。

## 8. 开放问题 OQ-1..20（Q 必须带文档限定，CNF-9；全部未决）
| OQ | 问题 | 影响 |
| --- | --- | --- |
| 1 | **Q1（compat §6）：US 是否服务 Ratkin？**"US 服务 Ratkin（需 U1）"vs"不服务（Ratkin 内容侧由 SR/独立包提供）"二选一（=F4/CNF-1） | **阻塞**；DEC-20/21/22/23、F1 修复方向、S1 条款 |
| 2 | **Q2（compat）：跨程序集检测=类型全名 vs 共同标记接口**（后者需 SR 1.0 依赖 US 后实现） | U1 形态、S2 必要性、依赖时序 |
| 3 | **Q3（compat）：桥上线时 SR 程序集是否与 US 同装（重叠期长度）？重叠期谁拥有 `SqueakyRatkin.SqueakVoicePackDef`？** | R3/S4/U3/DEC-28 |
| 4 | **Q4（compat）：0.4 双开是否需"任一侧检测到另一侧服务同一 Domain 即跳过整个 Domain"**——报告自陈只覆盖 comp 装配层、**未覆盖音频包选择层** | 选择层双响缺口（GAP-2） |
| 5 | S1 的"Ratkin 装配唯一写者"是否已写入 SR 合同与 MEMORY（语料外） | [xref: PKG-1]、记忆面回查 |
| 6 | U1–U4"需转述"是否完成、U1 是否落地、P0 退出条件（U1 测试/日志证据+自测 Ratkin 跳过）是否达成 | DEC-22 可执行性；P1–P3 全阻塞在 U1 之后 |
| 7 | **Q1'（mig §7）：是否插入 SR 0.5 过渡版**（独立可用+迁移提示）vs 只用 0.4.x patch | 版本序列、P2/P3 边界 |
| 8 | **Q2'（mig）：是否另开冻结 legacy Workshop 条目**（§4 选项 1） | DEC-25/27、R6 维护预算 |
| 9 | **Q3'（mig）：P2 设置导入做不做**（US 反射读 SR 设置）；不做则"重设一次"公告 | R4（调音丢失/差评）、P2 退出 |
| 10 | **Q4'（mig）：公告窗口长度**（建议 4–8 周或一个完整发布周期） | R2、P2/P3 时序（P3 需前置 ≥1 个完整窗口） |
| 11 | **Q5'（mig）：1.0 的 US 前置硬（`<modDependencies>`）还是软**（不声明、无 US 即静默）；按 B2 均不崩档、差别=列表警告可见性 | F5/R2/P3 退出 |
| 12 | **Q6'（mig）：legacy 独立线跨游戏版本维护预算（是否承诺 1.7）**；R6=1.6→1.7 时条目失效 | 资源承诺 |
| 13 | **Q7'（mig §8.5）：id 归属按 B（主线继承 id，推荐）还是 A（legacy 保留 id）？** | **阻塞**；PackKey 重置、设置跟随、第三方门 |
| 14 | **Q8'（mig）：是否接受"迁移玩家音源选择被重置一次"（A 的代价）？若否，B 是唯一选择**——把 Q7' 变成硬蕴含关系 | 存量体验、DEC-27 可行性 |
| 15 | **Q9'（mig）：legacy 上 Workshop 长期并行（写 `incompatibleWith` 新线？）还是仅 GitHub 归档（方案 C）？** | S2/U1 永久化（CNF-7）、R3/R6 |
| 16 | **Q10'（mig）：新 pack-only 线用 US 原生类型（推荐）还是 SR 旧类型（与 legacy 冲突）？** | DEC-28、作者指南（R5） |
| 17 | 交接风险：三份主源**无一条实施回执**；把 DEC-15/19/24 读成"已完成"会误导归档判断 | "当前状态"叙述必须写"未验证"（GAP-7） |
| 18 | 交接风险：US 基准不可复现（`0.4.x`@`c8794ff`+4 个未提交改动） | E1–E12 可复核性 |
| 19 | 交接风险：US 仓规则文档单向不可达（CNF-1 一方） | Q1 裁决原文核对（GAP-3） |
| 20 | **未验证门：双开实机验收矩阵（F1 修复后必须全绿）五场景**：SR+US 无 US 型 Ratkin 包→Ratkin 单响+US 他族单响；SR+US+US 型 Ratkin 包→**Ratkin 仍单响**且 US 日志显示跳过 Ratkin 装配；SR 旧包经桥被承认→仍单响；事件矩阵（Select/Draft/Undraft/Attack/Wounded/Death/Equip/MentalBreak/BabyFits + 周期 Eat/Move/Call/Work/Social/Joy/Sleep）→每次事件恰好一条声音无双发；卸载 US/卸载 SR→另一侧照常、存档无残留（不得新写存档） | 未跑；P1/P3 退出、拆分门 5/6 |

## 9. 锚点 ANCH（逐字）
**ANCH-1 类型/符号**：`UniversalSqueaker.SqueakVoicePackDef`、`SqueakyRatkin.SqueakVoicePackDef`、`UniversalSqueaker.CompProperties_Squeaker`、`SqueakyRatkin.CompProperties_Squeaker`、`SqueakyRatkin.CompSqueaker`、`CompSqueaker`、`VoicePackCompAttach.cs`（`Apply`）、`SqueakXenotypeCatalog.cs`（`SqueakXenotypeCatalog.Current`、`DefDatabase<SqueakVoicePackDef>.AllDefs`）、`CompProperties_Squeaker.CreateDefault()`、`Patch_Selector_Select.cs`、`Patch_RedirectModSettingsWindow.cs`（owner `UniversalSqueakerMod`）、`Patch_ModMetaData_LocalizedMetadata`、`SamePackageId`、`ModLister.TryAddMod`、`DefDatabase.Add`、`ThingWithComps.ExposeData()`/`InitializeComps()`/`LoadingVars`、`ModsConfig.UnsatisfiedDependencies`、`ModUnsatisfiedDependency`、`GetComp<CompSqueaker>()?.Notify_Select()`、`CompAutoAttached`、`foreign_squeak_comp`（**建议**的 skip 原因名、尚未存在）。
**ANCH-2 packageId/文件/键**：`coahuilite.universalsqueaker`、`coahuilite.squeakyratkin`、`coahuilite.ferritelib`、建议新 id `coahuilite.squeakyratkin.legacy`；PackKey=`modContentPack.ModMetaData.PackageIdNonUnique + ":" + defName`（`SqueakVoicePackModels.cs` `TryGetPackKey`）；`voicePackSelections.enabledPackKeys`；`Mod_coahuilite.squeakyratkin.xml`（**schema 4/2**：`voicePackSelections`/`xenotypePresets`/`moodOverrides`/`globalActionEnabled`…）；`SqueakFallbackProfileStore`；`SqueakyRatkin_Profile_<race>.xml`；`SqueakyRatkin_*`/`UniversalSqueaker_*` 前缀；`Defs/SoundDefs/SR_*.xml`；`LoadFolders.xml`=`/`+`1.6`；`About/About.xml`；legacy 条目建议名 "Squeaky Ratkin (Legacy)"。
**ANCH-3 域模型/ABI**：`RaceKey = raceDefName`、`XenotypeKey = xenotypeDefName`、`RaceAudioDomain = (RaceKey)`、`XenotypeAudioDomain = (RaceKey, XenotypeKey)`；选择链五层 `(race, xenotype) 池 → race 池 → pack 自带 fallback（可选）→ US 内置 fallback profile → 无声`；`VoicePackSelectionRecord`、`SqueakRuntimeResolver`、`SqueakVoicePackAction`、`SqueakMoodMod`、`XenotypeMoodOverride`、`HarRatkinXenotypeDiscovery`（`raceRestriction`/`whiteXenotypeList`）、`XenotypeUI`、`SqueakAudioBrowser`、`SqueakLog`、`tools/SqueakLogCharacterization`、`srdiag fmt=1`/`srdiag v1`/`usdiag`、`SettingsOrigin`（`FreshCreated`/`LoadedFromFile`）。
**ANCH-4 版本/阈值/计数**：`0.2.2`、`0.2.3`、`0.2.4`、`0.3.0`、`0.3.1`、`0.3.2`、`0.3.3`、`0.4.x`、`0.5`、`1.0`/`1.0.0`、`1.6`、`1.7`；**15 个 action**；`srdiag v1` locked **28 事件**；**16 个 patch 文件有 15 个同名同目标**；**约 350 ms 合并保存**；E6 基线 `216 t`、`Eat EachTime 144`、`Call 864/1.2%`、`Move 504/1.2%…`；US 检查点 `0.4.x 分支 @ c8794ff`、工作树 `4 个未提交改动`；P2 窗口 `建议 ≥4–8 周`；legacy 冻结建议 `≤0.5.x`；公告前置 `≥1 个完整窗口`。
**ANCH-5 年龄事实与映射**：`AgeBucket {Baby, Toddler, Child, Adult}`（append-only 四值）；`SqueakLifeStageResolver`（规划示例名 U-4：LifeStageDef defName→AgeBucket 的 XML 数据表，默认 Adult；表外 defName→Adult）；`lifeStageAges`、`(LifeStageDef, minAge)` 升序、`minAge < years`、`pawn.ageTracker.CurLifeStage`、`pawn.DevelopmentalStage = CurLifeStage?.developmentalStage ?? Adult`（`Pawn.cs:2022`）；`DevelopmentalStage : uint`（Verse）=`None/Newborn/Baby/Child/Adult`，**无 Toddler**；原版 Human 1.6 五段=`HumanlikeBaby 0–3`（Baby，voxPitch 1.6）、`HumanlikeChild 3–9`（Child，voxPitch 1.2）、`HumanlikePreTeenager 9–13`（Child，voxPitch 1.2，MayRequire Biotech）、`HumanlikeTeenager 13–18`（Adult 默认值）、`HumanlikeAdult 18+`（Adult）；无 Biotech 时 PreTeenager 条目剔除（Child 3–13）；判定用生物年龄 `AgeBiologicalYearsFloat`；`MentalStates_BabyFits`、`stateEffecter: BabyCrying/BabyGiggling`、`MentalBreakWorker.TryStart`；映射=Baby←`HumanlikeBaby`、Child←`HumanlikeChild`+`HumanlikePreTeenager`、Adult←`HumanlikeTeenager`+`HumanlikeAdult`、**Toddler 无原生对应**。
**ANCH-6 限制机制（示例名 U-4）**：`ProductDomainFilter`（0.3.x 常量=`{Ratkin}`；三处入口=catalog 构建过滤/UI 与分配器枚举投影/内置 fallback 装配；**0.4.x US 发布时移除**）；`SqueakBuiltInFallbackCatalog`（"如"引示例名；合同转述名=`BuiltInFallbackCatalog`，CNF-10）；试验名单 default=`{Ratkin}`、experimental=`{Ratkin, Kiiro, Miho}`（**替换**非叠加；隐藏设置项、Scribe 持久化、UI 不渲染、release 默认 off）；先例名 `kiiro-experiment`；**`{Ratkin, Kiiro}`**=内置 fallback 表规划起点；装配表 0.3.x=`{Ratkin}`；catalog 0.3.x=`(Ratkin, *)`。
**ANCH-7 旧包字段与编号**：桥兼容字段=`raceDefName`/`ageTag`/`IsEgg`/`fallbacks`；顺序硬门=`U1 检测落地 → US 型 Ratkin 包 / legacy 桥启用 → SR 1.0 内容化（DLL 退役）`；编号集 S1–S4（SR 侧）/U1–U4（US 侧）/F1–F8（发现）/E1–E12（证据）/B1–B5（事实基线）/P0–P4（阶段）/R1–R6（风险）；依赖链 `SR → US → FerriteLib`。
**边界条**：①授权（2026-08-24）legacy 桥薄空类 `SqueakyRatkin.SqueakVoicePackDef`=US 仓共存规则**唯一例外**、SR grep 显示尚未实现；②Kiiro 发布/公告以作者明确许可为门（Miho 同理）；③玩家手改 XML/Config=自行 mod 范畴（不受支持、文档不教、raw 编辑不阻止）；④staging 成功≠订阅/保存/Workshop 状态证据；⑤设计笔记不得覆盖现行合同（先提升进 `project-architecture-contract.md` 与设置合同）；⑥隐私：无本机绝对路径/日志摘录/凭据/`PublishedFileId` 值；`foreign_squeak_comp` 等为建议名非日志摘录。

## 10. 教训 LES-1..8
LES-1 有存量订阅者时兼容修复落"零玩家一侧"（前提=另一侧确实零玩家，US 发布后前提消失）｜强。LES-2 两侧改动同影响玩家听感时：发布顺序写成硬门+逐条列倒置后果+进发布门断言｜强。LES-3 不复制上游判定数据（原版 `minAge`），查上游运行时 defName+表外保守默认（Adult）｜强。LES-4 "机制通用+限制版本化"：限制集中于白名单数据表 filter 三处入口，拆分时可整体移除｜中（未经实施验证；长期保留会退化为特判）。LES-5 身份标识跟"内容/订阅"不跟"代码分支"：先列运行时每个绑定面再判可否继承，否则把平台限制误判为必然约束｜强（Workshop 原地升级未取证）。LES-6 同一数据面多文档记录时字段清单交叉核对（年龄标签 vs `ageTag`）｜tentative。LES-7 文档自计数与 Markdown 编号追加时易失真，引用引条目名不引计数｜tentative。LES-8 只读审查定位根因、不能证明修复有效：验收矩阵与退出条件绑定实测+日志证据｜中。

## 11. 缺口 GAP-1..11
GAP-1 F1/F2/F3 是否实机观察到？——全为只读审查+推演、矩阵未跑；需双开实机矩阵+skip 日志。GAP-2 选择层是否双服务？——compat 自陈未覆盖。GAP-3 CNF-1 US 仓规则现行文本？——语料外，需授权读取或裁决文本。GAP-4 U1 落地？桥实现（含"仅 SR DLL 不在载时启用"gate）？——需 US 侧提交/测试/日志（P0 退出定义物）。GAP-5 srdiag v2 是否已冻结？——属 PKG-1/PKG-2 面。GAP-6 "四层限定"指哪四条？——计数/枚举/编号互不一致，或澄清或以三处入口替代。GAP-7 六门/阶段门各完成几条？——主源无实施记录；**最终包应写"未验证"、不得按版本推进推断**（需 PKG-2/PKG-4/语料外 TODO/MEMORY）。GAP-8 SR `schema 4/2` 字段↔US 分层调音模型映射（导入可行性）？——无映射表。GAP-9 年龄 XML ABI 是否已随 0.3.1 同批冻结？——"要求"非"记录"。GAP-10 "决策文档 §5"身份与条文？——未具名缺席（编排方转述疑指 PKG-2 主源 §5=简称歧义，列入跨包对表）。GAP-11 Workshop 同 item 原地升级平台层可行？——方案 B 隐含前提未取证（拆分门 6"过渡演练"正是这项）。

## 12. 未吸收要点
note：`tools/SqueakLogCharacterization` 工具细节与 locked facade 逐条清单（→PKG-1/PKG-2，只保留"已锁定⇒改动须先设计新协议版本"）；数据模型 ASCII 图（与 DEC-3 一致）。compat：E1/E3/E5 C# 判据原文（语义入 EV-4/ANCH-1）；互链（纯导航）。mig：§9 文档关系（纯导航；TODO/MEMORY 只作回查线索）；Harmony id"无实质影响"定性不升 DEC（已入 ANCH-2/DEC-27）。

## 13. 自检
计数：TL 10（0–9）、DEC 29、ALT 25 项（1–23 除 11+4a/4b/12a/b/c；**ALT-11 缺号=编号空洞**）、ASM 16、C 4/I 4/U 6、CNF 10、INC 5/EV 7、OQ 20、LES 8、GAP 11、ANCH 7+边界条。P0 全留（§1 三条）；六门逐字（DEC-19）、顺序硬门逐字（DEC-22/ANCH-7）、P0–P4 逐字（DEC-24）、B1–B5 与 E1–E12 逐字编号+语义、`{Ratkin, Kiiro}`、Kiiro 许可门逐字、`AgeBucket` 四值+五级选择链、三处入口；Q1–Q10 不折叠（OQ 16 行独立、全带 compat/mig 文档限定）；CNF-1 冲突保留未归一（一方语料外）；CNF-10 双名并列（`SqueakBuiltInFallbackCatalog` 规划示例名 vs `BuiltInFallbackCatalog` 合同转述名）未改写。[C]/[I] 未当 [F]；预测性结论均标注。字节自测：v1=134,552 B，本文件目标 ≤19,307 B（约 14.4%）。
