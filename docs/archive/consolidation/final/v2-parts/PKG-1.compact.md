# PKG-1 compact（现行权威合同面 · 阶段 B 第一遍逐包压缩）

## 0. 元数据与压缩账本
- repo_rev `4df9594713adbbba91e0aea788a7c7cd3503ab3f` / frozen_at 2026-09-17 / mode two-pass（本文件＝第一遍，目标 ≤25%·v1）/ 生成者：提炼 agent
- 输入 `docs/consolidation/packages/PKG-1-current-contracts.md` = 58,194 字符（coverage-check §2 实测；包自报 57,202 为补记前值）⇒ 目标 ≤**14,548**
- P0 零丢弃对账（in/out 全等）：DEC 37/37、ALT 1/1、CNF 8/8、OQ 13/13（OQ-1…12 + OQ-22）、ASM 10/10、GAP 7/7、ANCH 10/10、TL 9/9、LES 7/7、INC 4/4、EV 5/5、C 3/3、I 3/3、U 8/8
- 手法：条目表格化；长引用集中 §10（条目内只写 `A§6` 式短指针）；数字/字段序一律指向 §9 不复述；压缩损失见 §12
- 逐字面占比：§9 锚点 ≈4.4k + §3/§6 内的模式名/字段序/计数 ≈1.0k ⇒ 约 38%，**未达 §4 密度例外 ≥50% 举证线**；§9 超 0.06·T 的原因＝必须逐字保留的锚点面，按 02 §4 处置顺序压表达、不删条目
- 编排方补记落位：Eat 日志字段缺失由「缺失」改记＝**PKG-6 已裁决不新增日志事件；OQ-22 仍开**（见 U-7 / OQ-22）
- 只读边界：仅写本文件；隐私：全文无盘符形态、无本机绝对路径、无日志摘录、无凭据、无 `PublishedFileId` 值

## 1. 项目上下文
`鼠辈啁啾` / `Squeaky Ratkin` = 只服务 NewRatkinPlus Ratkin 的**固定 17 动作发声外壳** [F]：身份判定唯一依赖 `CompSqueaker` 与精确 `defName`；动作键 append-only；`SqueakVoicePackDef` XML 面自 0.3.1 ABI 起 public-stable；日志面 v1 字节不可变、v2 受控扩展 [F]。「什么算破坏兼容」因此可机械判定 = `[I]`（I-1，替代解释见 §5）。权威序：accepted product decisions > 本合同 > 历史规划/评审；source/Defs > 本合同 ⇒ 合同**不是**最高权威 [F]。三份合同均为自述式规范面，正确性以源实现为背书而本包无实现证据（GAP-1）。时间范围：0.2.3 默认口径 → 2026-08-22 ABI 声明 → 0.3.1/0.3.2 日志版本层 → 2026-08-23 Eat 例外 → 2026-09-13 合同冻结（`e0608f7`）。

## 2. 主要时间线
| id | 时间 | 事件 / 裁决 | 当时 vs 现行 | src |
| --- | --- | --- | --- | --- |
| TL-1 | 0.2.3 起 | 出厂默认改为 Fallback + 内置 Race Example 启用；仅一次性施加于「从未设置过模式」的配置（Scribe 省略默认 `voicePackMode` 节点），不覆盖显式模式或既有选择 | 新增默认 / 仍生效 | A§6 |
| TL-2 | 2026-08-22 | 面向作者 VoicePack XML ABI 冻结声明（0.3.1 ABI 起 public-stable） | 落地 / 兼容边界主锚 | A§6 |
| TL-3 | 2026-08-22（状态戳） | `logging-protocol.md` 自述为源审阅所得 v1+v2 合同、非实测会话证据 | 自述 / 状态行仍在 | C L3 |
| TL-4 | 0.3.1 版本层 | `srdiag fmt=2` 落地（`race`/`xenotype` 入核心字段序、4 条 v2 事件、once-key 分 `log-v1`/`log-v2`）；动作字符串键「0.3.1 定型」 | 扩展 / v1 不可变 + v2 并存 | C |
| TL-5 | 0.3.2 版本层 | `audio.route.selected` 扩字段并改人读句；装配器改为每动作每 5 秒一条合并 fmt=2 记录，不再并行发 v1 `audio.dispatch.ok` | 收缩重复 / 仍生效（v1 字节锁定但运行时不再发） | C |
| TL-6 | 2026-08-23 | 维护者决定：设置 UI 唯一例外＝一对 Eat 粒度控件（父/子，默认 `false`）；Scribe 只 add-only 两默认 `false` 字段、默认值省略、settings schema 不 bump | 放行 / 仍生效 | B |
| TL-7 | 2026-09-13（`e0608f7`） | 架构合同与设置合同同步更新（Eat 三级模式全文、`SqueakEatOccurrence.cs` 入实现权威路径） | 写入 / 即本次冻结版本 | A§3 A§7 |
| TL-8 | 2026-08-23（`56b6f05`) | 日志协议末次提交，其后未随 09-13 更新 ⇒ 与架构合同 20 天时间层差（不构成条文冲突，但见 CNF-3/CNF-8） | 定稿 / 层差仍在 | C L3 |
| TL-9 | 不可得 `[U]` | 0.3.1 ABI 之后哪个「首个发行版本」承载该 ABI | 未定 / 仍未定（OQ-1） | A§6 |

## 3. 关键决策史（DEC-1…37 + ALT-1）
> 体裁：三份主源是现行规范性合同，绝大多数状态＝现行有效；仅 ALT-1 有语料内备选。时间/主体多为 `[U]`（合同无签名 → U-8/OQ-12）。**数字与字段序不在此复述，指向 §9。**

| id | 状态·标签 | 硬约束要点 | 反转 / 重开 / 关联 | src |
| --- | --- | --- | --- | --- |
| DEC-1 权威分层与「不得静默归一」 | 现行 `[F]`，时间 `[U]` | 冲突须识别并在权威层消解，不得静默归一；本合同自排在 accepted product decisions 之下、source/Defs 排其之下 | 无反转；本管道 README §4 以此为上游；CNF-6 | A§1 |
| DEC-2 产品身份边界 | 现行 `[F]` | 只服务 NewRatkinPlus Ratkin；`it is not a generic HAR-race framework.` | 无反转；通用化另成一线（PKG-3），是否松动本条不由本包判定 | A§1 |
| ALT-1 通用 HAR-race framework | 已否弃 `[F]` | 强项＝受众更大；弱点＝身份判定与兼容边界失控；裁决＝否弃 | **吸收的硬细节**：Def 前缀统一 `SR_`；C# 类型靠命名空间隔离（非类型名冲突规避）保留为架构约束 | A§1 |
| DEC-3 基线与 DLC 端到端可用 | 现行 `[F]` | `Biotech-inactive paths must not enter Xenotype DefDatabase or pawn-gene paths`；缺失项统一解析到 `GlobalOnly`/fallback | ASM-2；DEC-30 UI 降级 | A§2 |
| DEC-4 Xenotype 身份判据与候选并集 | 现行 `[F]` | 身份＝精确大小写敏感 `XenotypeDef.defName`，pawn 当前 `defName` 为运行时权威；仅经 HAR 发现的 Core/Ludeon Xenotype **不**单独产生设置候选行；显式 preset／已持久化选择／声明式 VoicePack 目标仍可见；同名重复对该 Xenotype 层 fails closed、Race/Vanilla 继续；范围排除 gene 级设置、任意 HAR 种族、第三方 Xenotype 冲突仲裁 | 0.3.1 落到日志投影（DEC-36）＝细化非反转 | A§2 |
| DEC-5 固定 17 动作与 append-only 序号 | 现行 `[F]` | 名单与序号见 ANCH-2；新增动作＝source/Def/localization/trigger/documentation 协同变更、**不是插件注册**；XML 供给动作计划 | 追加事件（非废止）：`Crying`=15、`Giggling`=16，0–14 不变（EV-2） | A§3 |
| DEC-6 逐动作触发边界 | 现行 `[F]` | 周期动作由 `CompSqueaker` 采样，XML 决定 `EachTime`/`RandomOneShot`/`External`、interval、probability、cooldown clock、global-cooldown bypass；`Draft`/`Undraft` 仅源自玩家 gizmo 变更；`Attack` 限 Core `Verse`/`RimWorld` 中成功实现 `Verb.TryCastShot` 者，排除 Ability 命名 verb、DLC 程序集、不使用该方法的攻击系统；`Work` 可收窄到玩家强制 job；`Equip` 仅接受玩家发起的 Core Equip job（排除 AI/load/system）；`Move`/`Sleep` 偶发一次性、不支持持续播放；`Crying`/`Giggling` 仅由 `MentalStateHandler.TryStartMentalState` 的 BabyFits postfix 在 `MentalFitDef` 反查通过后通知，**不**拓宽真实 mental-break hook | Eat 粒度例外（DEC-7）明文不改本条 | A§3 |
| DEC-7 Eat 触发粒度：默认整 job + 三模式 | 现行 `[F]`；正文末次 2026-09-13 | 出厂默认按 **job 粒度**采样（每个活动 Core `JobDefOf.Ingest` 都计数，含取食赶路、端到餐桌）＝已发行行为与既定签名，**明文禁止默认收窄**；两玩家项（均默认 `false`）由纯函数 `SqueakEatOccurrence.ResolveMode` 收敛为恰好三级：父 `eatOnlyDuringChewing` 关 → `WholeJob`（无视子项）；父开＋子 `eatIncludeDrugs` 关 → `GainingNutrition`（取自 vanilla `IEatingDriver.GainingNutritionNow`；取食/赶路回落普通动作解析，pather 移动时为 `Move`；零营养成瘾品保持静默）；父子皆开 → `ChewingToil`（判据 `JobDriver.CurToilString == "ChewIngestible"`，即 vanilla toil debugName，由 `SqueakEatOccurrence.ChewingToilDebugName` 锁定；覆盖零营养成瘾品）；toil 名在当前进程未确认 ⇒ 谓词**回落 `WholeJob` 而非静默**，并保留营养采样作额外并集保护 | 无反转；不变式：父关时子值强制 `false`（UI 禁用、关父即清零、`PostLoadInit` 归一任何手改 `true`）；啤酒 `Nutrition 0.08`、ambrosia `0.2` 在模式 2 已计数（营养权威 `CachedNutrition > 0`）；`AllowsOccurrence(mode, gainingNutritionNow, chewToilActive, chewToilNameConfirmed)` 纯数据链入 kernel harness；CNF-4/OQ-4/U-7 | A§3 B |
| DEC-8 设置侧动作策略优先与「静默即绝对」 | 现行 `[F]` | 设置拥有的固定动作策略在 resolver、RNG、timing、vocal、playback **之前**运行；`Disabled is absolute production silence`；显式 preview 有意独立；动作作用域同时由全局策略与解析后 runtime delta 双重执行 | 无 | A§3 |
| DEC-9 发声门、心情与距离 | 现行 `[F]` | 净化后的发声器官效率门可静音**所有**动作（含 `Death`）；启用时钳制后的 `Talking` capacity 是普通动作概率门、`Death` 单独豁免；enablement/probability/timing 先评估；**发声拒绝消耗与成功播放相同的每动作及适用共享尝试冷却**（防每 tick 重试）；心情经 `SoundInfo.pitchFactor`/`volumeFactor` 在派发处施加；一个动作/音频集只有一个 SoundDef、禁止 mood × action 的 SoundDef 矩阵；相机 `CurrentViewRect.ExpandedBy(10)` 视图剔除 + `SoundInfo.InMap(TargetInfo(Pawn))` + `distRange` 衰减；range 数值见 ANCH-7；高速控制缩放冷却而非单个音量 | **废止口径**：`Do not reintroduce a zoom eligibility gate (including CurrentZoom <= Close)`（废止对象＝缩放资格门，时点 `[U]`，原因＝错误阻断距离衰减）；INC-1/INC-2 | A§4 |
| DEC-10 XML 作为行为权威 | 现行 `[F]` | `1.6/Patches/Ratkin_AddSqueakComp.xml` 是行为权威，运行时代码通用适配；`actions`/`moodMods` 数据驱动、距离预设在 `CompProperties_Squeaker.distancePresets`；`documentation categories must not become action-name hardcoding where a field can express the behavior` | 无 | A§5 |
| DEC-11 三层配置合并与「不承诺迁移」边界 | 现行 `[F]` | 合并序＝(1) XML `CompProperties` 默认 → (2) 全局 `ModSettings` 覆盖 → (3) 每 Xenotype delta；缺失预设/字段继承下层；选择距离预设复制其实际范围、手改即变 Custom；展示用本地化名 + Xenotype 图标、`defName` 作技术信息；**legacy audio selections / remix values 不自动迁移**，且不得在无证据时推断或承诺无关设置的迁移；行为/心情配置与音频包选择彼此独立 | 无；CNF-4（settings「不 bump」vs 内部面「0.x 修订窗」张力） | A§5 |
| DEC-12 VoicePack 模型与年龄/彩蛋 ABI | 现行 `[F]` | `SqueakVoicePackDef` ＝一个选择/权重/校验单位；字段集见 DEC-14/ANCH-1；每个 PackDef 必须声明精确大小写敏感 `raceDefName`、缺失即拒绝；Race pack 无 target 且无需 Biotech；Xenotype pack 恰有一个 `targetDefName` 字符串；`weight` 为正有限数；可选 `fallbacks` 把单个内置动作映射到该包 fallback 层 `SR_*` SoundDef；精确年龄变体优先于全年龄变体；`IsEgg` 条目仅在 `allowEasterEggSounds` 开启时入池（重建解析器时离散施加）；一包可发布多个 PackDef 但彼此独立；**稳定 PackKey 身份＝package ID + PackDef defName**；Race 与 Xenotype 选择持久化在**不同域** | 无；残差 `Toddler`（U-4/GAP-6）；默认值见 ANCH-4 | A§6 |
| DEC-13 Off / Fallback / Remix 模式语义 | 现行 `[F]` | **Off**＝仅内置 fallback；**Fallback**＝逐动作 Xenotype pack → Race pack → pack fallback → built-in fallback → 静默；**Remix**＝在当前可播放的 Xenotype/Race/声明式 pack-fallback/built-in-fallback 层之间等层选择；未声明 pack fallback 的动作保留**冻结的三层 Remix 形状**（Xenotype/Race/built-in）；包永不覆盖动作行为、timing、capability、distance 或 mood | 无；冲突 CNF-2/CNF-5；默认口径由 0.2.3 引入沿用（TL-1） | A§6 |
| DEC-14 面向作者 VoicePack XML ABI 冻结 | 现行 `[F]`（冻结声明），2026-08-22 | **冻结面**＝`SqueakVoicePackDef` XML 面：`raceDefName`、`scope`、`targetDefName`、`weight`、`fallbacks`、`actions` 条目（`action`、`ageTag`、`IsEgg`、`sounds`）以及被引用的一次性 `SR_*` SoundDef 合同 ⇒ 对音频包作者 **public-stable**；字段 add-only、内置动作键 append-only、校验 fails closed、`IsEgg` 明确纳入公开 ABI。**未冻结面**＝kernel、domain model、fallback profile schema、未来 action gates（仍在 0.x 修订窗） | 无废止记录；适用锚「from the first released version that carries the 0.3.1 ABI」语料内不可判定 → OQ-1/CNF-8；公开/未冻结边界不清 → OQ-6；LES-3 | A§6 |
| DEC-15 内置 fallback 档案 | 现行 `[F]` | `BuiltInFallbackCatalog`＝维护者所有的 C# 数据：精确 race → 字符串动作键 → 原始 `SR_*` SoundDef 键；Ratkin 档案**刻意只有 15 个键**（`Call` 至 `MentalBreak`），缺 `Crying`/`Giggling`；既无可用包路径又无内置档案的种族 → 静默；它是选择链末端，其持久化另有独立正式 Config 产物（DEC-16） | 无；末端在日志不可归因 → CNF-2 | A§6 |
| DEC-16 `SqueakyRatkin_Profile_<race>.xml` 独立持久化通道 | 现行 `[F]` | `SqueakFallbackProfileStore` 拥有该文件，经 temp + 原子替换写入，**永不调用 `WriteSettings()`、永不加入 ModSettings debounce 队列**；缺失/损坏/更旧副本从 C# 源重建；当前副本若存在字段在场性差异则与 C# 源合并 | 无；「更旧」判据未定义 → OQ-9 | A§6 |
| DEC-17 orphan / dormant 与唯一破坏性出口 | 现行 `[F]` | PackKey 消失的已保存选择保留为 **orphan** 并从解析中排除；因 Biotech/目录/目标缺失而不可用的 Xenotype 绑定是 **dormant** 而非 orphan；两者在同一身份回归时自动恢复；玩家可显式忘记；忘记一个不可用 Xenotype 目标是**已确认的破坏性动作**，同时删除该目标的精确行为 preset 与其 Xenotype VoicePack 选择；catalog refresh 永不隐式执行该删除；包目标 + 已持久化绑定 + 非官方发现提示共同构成 **UI 候选并集**，不是资格白名单 | 无；设置合同同源双写「明确忘记入口 + 破坏性确认」（DEC-25）＝非冲突；膨胀上限未记 → OQ-8 | A§6 |
| DEC-18 两个示例包的平等性 | 现行 `[F]` | 内置 `SR_OfficialExample_Race` 与外部 `SR_ExampleTemplate_Race` 是独立 Race VoicePack，各有独立 Defs / PackKeys / catalog 条目 / roots；二者都不被解析器加权优待；Template 的 Biotech 树是 TXT-only 指导、不是可加载的 Xenotype 内容 | 无 | A§7 |
| DEC-19 Example 音频基线与镜像校验不变式 | 现行有效 `[F]`，且明文声明基线可变；计数 `[C]`（C-3） | Template 目录是唯一手工维护的 Example 源，仓库相对路径 `Extras/SqueakyRatkinExampleVoices/1.6/Race/Sounds/coahuilite.squeakyratkin.examplevoices/SR_ExampleTemplate_Race/`；当前参考基线＝**15 个动作目录 / 41 个 OGG**（每动作计数见 ANCH-10）；这 15 个是「有出厂 Example 音频的动作」，**不是**运行时 ABI（ABI 为 17 名）；主 mod 源树**有意**没有内置 OGG 镜像；`scripts/stage-package.ps1` 复制该源到 staged 内置 root，校验动作/键集合与 SHA-256 同一性、**不断言固定总数或每动作计数** | 无；意外变更、Template→内置镜像关系或其发行被破坏、包/路径 root 被改、多余音频格式、staged 哈希分歧＝**要上报的不变式、不得静默修复**（ANCH-10 授权边界）；EV-4 | A§7 |
| DEC-20 音频格式建议 | 现行 `[F]` | 正式 Example 源仅允许 OGG；OGG Vorbis 是第三方 VoicePack 最终发行的建议格式；WAV 受 RimWorld/当前加载链支持、可用于中间评审但**不是**项目建议的发行格式；**不得把运行时校验器或游戏兼容性表述为发行建议**；本政策不因「是 WAV」而拒绝第三方 WAV；MP3 不是建议的指导格式 | 无 | A§7 |
| DEC-21 作者资源根与第三方碰撞不仲裁 | 现行 `[F]` | 作者资源根恒为 `<lowercase packageId>/<PackDef.defName>/<Action>/`；SoundDef `clipFolderPath`、物理 `Sounds/` 树与安装指导必须一致；项目校验自身发行 root 与重复扩展名、但**不**全局仲裁第三方 patch 或碰撞 | 无 | A§7 |
| DEC-22 实现权威路径清单 | 现行 `[F]`；清单末次随 `e0608f7`（2026-09-13）更新 | 路径全列于 ANCH-1 尾段（含 `Source/SqueakyRatkin/SqueakEatOccurrence.cs`、`scripts/stage-package.ps1`、`docs/logging-protocol.md`）＝「每条硬约束的现行文本位置」的落地 | 无；是否需覆盖 `1.6/Assemblies` 之外 flavor → OQ-10 | A§7 |
| DEC-23 设置 UI 产品表面 | 现行 `[F]` | 普通玩家始终看到三个任务页：**发声规则**、**心情音色**、**语音来源与异种**；连续点击稳定版本入口 **7 次**后（ANCH-7）才出现第四页 **开发与诊断**，未解锁时**无占位、无锁图标、无暗示入口**；第四页收纳诊断、日志、音频浏览/路径与版本信息；不改变 RimWorld Dev Mode、地图或 Pawn 选择的既有门控 | 无 | B |
| DEC-24 Xenotype preset 呈现与「本地化不是资格门」 | 现行 `[F]` | Biotech 激活时每个精确 `XenotypeDef.defName` 对应一个 preset、空/缺失字段回退全局默认；UI 显示本地化名称与图标、可提供 `defName` 技术信息；本地化、图标、发现信息**不是**资格门；未激活 Biotech 时不访问 Xenotype DefDatabase 或 pawn genes | 无（DEC-4 的产品面双写） | B |
| DEC-25 immediate 语义、合并保存与失联目标出口 | 现行 `[F]` | 设置是 **immediate**：语义输入一经接受即发布到运行时，约 `350 ms` 合并保存、关闭窗口必须 flush 最近待保存值；**没有** Apply、Revert、dirty-close guard 或草稿交易；失联目标保留以便来源模组恢复，但必须提供明确的「忘记此目标」入口与破坏性确认，确认后同时删除该目标的行为 preset 与异种语音包选择，普通刷新不得静默删档 | 无 | B |
| DEC-26 外壳稳定区与诚实失败呈现 | 现行 `[F]` | footer/status/version 是稳定区域；保存状态用永久预留槽位，不因 Saving、Saved、Failed 改变导航或页面高度；失败如实呈现、保留待保存代次但不无限自动重试 | 无；「待保存代次」→ GAP-7 | B |
| DEC-27 Remix 两步确认无副作用 | 现行 `[F]` | 切换至 Remix 走既有两步确认；任何取消、关闭或中间步骤都不得改变 canonical 模式，也不得让点击穿透到后方控件 | 无 | B |
| DEC-28 玩家排障工具的跨 flavor 编译与门控拆分 | 现行 `[F]` | 排障工具必须编译进所有 Dev、GitHub 与 Steam flavor；Developer menu 的 DebugAction 可用性受 RimWorld Dev Mode 与活动地图门控；「成功派发记录」开启后正式派发的短暂结果悬浮字仅受该记录开关与 Pawn 在地图内状态控制；相机指示器及其他 live overlay 维持各自门控；相机指示器读取真实 `Find.Camera.transform.position.y` 与 `Find.Camera.orthographicSize`、不得由 `RootSize` 反推；声音 preview 位于 ModSettings workbench、无需 DevMode | 无 | B |
| DEC-29 capability 与响应式布局 | 现行 `[F]` | 主菜单、无地图、无选择 Pawn 均受支持，依赖 MapUI/地图/Pawn 的命令明确说明不可用原因；无 Biotech 时不访问 Xenotype DefDatabase 或 pawn genes、基础设置/保存/动作/mood/Race-Vanilla 回退照常可用；响应式是有限模式（不承诺无限伸缩），以实际可用宽高、文本与控件最小尺寸选 normal/narrow/low-height 排列；先 Measure → Arrange → Draw，测量不写设置也不触发副作用；滚动有唯一所有者（前两页各一个主 scroll；第三页宽屏为明确边界的 master/detail 独立 scroll、窄屏改列表/编辑步骤且每步只有一个主要 scroll）；折叠或内容变化后夹紧 scroll position；固定 footer 从内容 viewport 扣除并预留 scrollbar gutter；短即时解释用 tooltip，回退顺序/No-DLC/Remix/技术身份等关键长说明由可见 `?` 入口页内展示、不得遮挡或与相邻命令共享 hit rect | 无；维度全表见 ANCH-9 | B |
| DEC-30 非目标清单与「唯一例外」 | 现行有效 `[F]`（例外已放行）；2026-08-23 维护者决定、承接 Steam 评论反馈 `[C]` | 不借 UI 改动改变动作资格、resolver、音频回退、Scribe schema 或 DLC 产品语义；不加入跨模组 UI 框架、共享 DLL、router/store/command bus、布局 DSL；不 patch RimWorld/其他模组 UI；不把 tooltip 当作关键知识唯一载体。**唯一例外**＝发声规则·频率组新增一对 Eat 触发粒度控件（父「仅在真正进食（正在摄入营养）时触发 Eat 叫声」＋依赖子项「使用成瘾品」），自带护栏：不改动作资格/作用域、resolver、音频回退或动作 ABI；Scribe 面只 add-only 增加两个默认 `false` 字段（默认值省略、settings schema 不 bump）；实现位于 `Source/SqueakyRatkin/SqueakyRatkinSettings.cs` 及设置 UI partial/helper、游戏上下文由 `SqueakSettingsGameContext` 管理、保存协调器负责 immediate/coalesced save/close flush、helper 只处理局部 Rect/绘制/意图 | 无；C-2/EV-5 | B |
| DEC-31 `SqueakLog` 闭包类型门面 | 现行 `[F]`；2026-08-22 状态戳；来源＝四个源文件的 source review；主体 `[U]` | 内部事件注册表固定每个事件的 event ID、visibility、Verse level、英文人读句与接受的类型化数据 schema；业务代码只能调用事件专属门面方法、**不能**提交自由格式 event ID、人读句、visibility、level 或 payload schema；这些注册表值与 `srdiag fmt=1` 字段序**本身就是兼容面**；每条记录以 `[SqueakyRatkin] ` 开头后接固定英文句、人读文本永不本地化；Daily 在详细诊断无效时只发人读句、有效时同行追加 ` \|\| srdiag fmt=1 ...`；DevOnly 无效时不发、有效时同形状；visibility 与 level 相互独立（`Daily`/`DevOnly` 管资格、`Info`/`Warning`/`Error` 选 sink），Warning 仍是 Verse warning、Error 仍是 Verse 红字，exact-once 行为对三者一致且不改变 sink | 无；跨包正证：0.3.3 功能面刻意不动日志协议 ⇒ 日志兼容面在 0.3.x 内被视为冻结资产 `[F]` 转述 | C |
| DEC-32 详细日志模式与会话重置 | 现行 `[F]`；0.3.1/0.3.2 版本层，日期 `[U]` | `SqueakDevLoggingMode` 三持久化值（见 ANCH-8）：`Auto`＝Dev 构建开、GitHub/Steam 构建关；`Enabled`＝所有 flavor 开；`Disabled`＝所有 flavor 关；模式独立于 `Prefs.DevMode` 与 `developerToolsEnabled`（七次点击排障解锁），解锁只暴露设置；四个模式结果事件区分显式 Enabled/Disabled 与 Auto 解析为 enabled/disabled，其 Daily 人读结果独立发出（包括模式变更把详细诊断关掉时）；DevOnly 门面方法以 `ShouldEmitDev` 守卫、调用方在构造诊断字符串/字段前也必须走该门 ⇒ 关闭状态下在 call-site 诊断构造与 DevOnly payload 格式化之前就返回；`SqueakyRatkinSettings.SetDevLoggingMode` 比较配置前后的有效状态、若变化则 `SqueakDebug.ResetLoggingSession` 清空音频采样、重置汇总计时器、清空 once-key 会话注册表 ⇒ 重新开启后每个动作首次成功派发可立即产出细节 | **当前未实现 map-lifecycle reset**（是否待办未记 → U-5/OQ-7） | C |
| DEC-33 `srdiag` v1 机读合同 | 现行有效（v1 声明为字节不可变）`[F]`；日期 `[U]` | 后缀单行且恰以 `srdiag fmt=1` 开头；核心字段与事件专属字段固定序、percent-encode 白名单逐字见 ANCH-5；值用 invariant-culture 格式化；缺失值与字面 `N/A` 都写成 `-`；解析方先按 key/value 切分再 percent-decode；布尔为小写 `true`/`false`；消费者必须容忍缺失的事件专属字段并把未知未来字段当扩展；**不得把本地化文本、pawn 标签、文件系统路径或原始 `Exception.ToString()` 写入协议**（异常只带 type / inner type / target site / 净化后的消息）；净化先替换 CR/LF 并移除其余控制字符，再把 DOS 盘符路径、UNC 路径、设备路径、`file:` URI、Unix 绝对路径与相对 `./`、`../` 路径替换为 `<path>`、最后截断到 256 字符；percent-encoding 发生在净化之后 | 无；通则与 `pawn=<label>` 互斥 → CNF-1/INC-4 | C |
| DEC-34 `srdiag` v2 受控扩展合同（0.3.1） | 现行 `[F]`；「0.3.1 定型」，日期 `[U]` | 携带 v2 专属事实（settings origin、race/xenotype 身份、route tier）的记录以 `srdiag fmt=2` 开头；v1 解析器继续接受 fmt=1；**任何 v1 事件永不获得 v2 字段**；v2 核心字段序见 ANCH-5；`action` 为字符串动作键（内置＝枚举名、与 v1 值字节相同；外部＝`packageId.defName`；白名单已含 `.` 故外部键原样写入）；`race`＝路由域的精确大小写敏感 `ThingDef.defName`、缺失为 `-`；`xenotype` 仅在该域存在时出现且为精确 `XenotypeDef.defName`；`settings_origin`、`sound`、`tier` 作为事件专属字段按每事件固定顺序追加；编码、净化、截断、异常元数据与 DevOnly 门控与 v1 一致 | 备选：合同**显式排除**「把 v2 字段塞进 fmt=1 记录」（理由＝闭合类型门面无法表达）＝落选路径；0.3.2 扩字段声明为延续非反转 | C |
| DEC-35 事件级 v2 字段合同与 origin / 拒绝原因闭包 | 现行 `[F]`；0.3.1 版本层 | `settings.origin`＝Daily Info、每会话启动时在读到设置对象之后发一次；origin 判定闭包：`LoadedFromFile`＝经 Scribe 成功反序列化（ExposeData 到达 LoadingVars）、`FreshCreated`＝无文件或文件不可读（框架丢弃损坏解析、告警并返回字段默认新实例）；句子由闭合二值集合参数化（先例：`mod.start.identity`）；`voicepack.pack.rejected` 以 pack key 作 `pack`，`reason` 取 `duplicate_key`（默认，同键被加载多次、此时 `count`＝重复实例数）或 `domain_filtered`（0.3.1 race-aware：`raceDefName` 落在产品域白名单之外）；稳定事件注册表为闭合表（计数见 ANCH-6） | 无；`domain_filtered` 是 DEC-2 身份边界的日志侧事实 | C |
| DEC-36 成功路径记录收缩与 once-key 分域 / 限流 | 现行 `[F]`；0.3.2 版本层（状态行同记 `0.3.2 extends audio.route.selected fields and its human sentence; v1 remains byte-immutable`） | `audio.route.selected` 为 DevOnly Info、自 0.3.2 起是**唯一**的成功路径细节记录——派发装配器每动作每 5 秒发一条合并 fmt=2 行、取代原先 `audio.dispatch.ok` + `audio.route.selected` 成对输出；`audio.dispatch.ok` 仍是锁定的 v1 注册表记录与特征化面、但运行时装配器不再发出；`egg=true` 标记来自 `IsEgg` 条目派发（恒以 `true`/`false` 写出以保证字段确定性）；`suppressed_detail` 计数自上一条细节行以来被吸收的派发数；`pawn`/`pawn_id` 携带被退役的装配器调用此前放在 `audio.dispatch.ok` 上的同一身份事实；`pawn_faction` 携带精确 `FactionDef.defName`（无阵营为 `-`）；`pawn_ctrl` 取 `player`/`nonplayer`；`tier` 词汇与折叠见 CNF-2/ANCH-1；once-key＝事件 + action/target/pack/reason/exception type、按协议版本分域 `log-v1`/`log-v2`、在锁下认领故去重线程安全（上限与清空 ANCH-8）；详细日志有效时每个动作首次成功派发立即发一条合并 v2 细节、其后限为每 5 秒一条并计数被抑制项；`trigger.outcome.summary` 最多每 60 秒聚合 dispatched 与 suppressed-detail；限流不改变 warning/error 的可见性或严重度；`xenotype.discovery.candidate` 自 0.3.1 起目标并集是 assembled-only 投影、可见候选携带确定性 `+` 连接的源集合（`declared_pack`、`selection`、`preset`）且 `enabled=true`；HAR 发现只是开发诊断、永不投行，故未被保留的 HAR-only 目标发 `reason=har_hint_filtered` 或 `reason=har_official_filtered`（镜像进 `source`）且 `enabled=false`；把 source 镜像进 reason 是有意设计，因为 v1 once-key 含 `reason` ⇒ 同一目标可为每个实质不同的源集合各发一条而不改变 once-key 合同 | 无（以「保留但不再发出」替代删除 v1 记录）；特征化面由哪个测试承载 → U-2/GAP-5；`pawn` 字段 → CNF-1/OQ-2 | C |
| DEC-37 提示词套件与任务包语料的自引用状态（本包新增，非源文档内容） | 现行 `[C]`；2026-09-17 维护者指示（OUT_DIR 落 `docs/` 下并声明「这也属于文档」） | 本包正文引用 `README.md §4/§5` 作为权威分层与包划分依据，而产物现与 `docs/**` 同处一个提交面 ⇒ 阶段 B/C 输入与被引用框架进入自我指涉；README §10 原要求把套件移出仓库/删除/追加允许面，本次实际选择「扩展允许面」（未在包内改写 README） | 无；收敛提交前需二选一处置 → OQ-11/CNF-7 | D§10 |

## 4. 假设及其演化
| id | 假设要旨 | 提出 | 仍成立？ | 变更/失效点 | 验证或反证 | src |
| --- | --- | --- | --- | --- | --- | --- |
| ASM-1 | 兼容边界必须与内部重构窗口分离：作者面可承诺稳定、内部面保留 0.x 修订自由 | 2026-08-22 | 成立 `[F]` | 出现「作者面被迫破坏性变更」即失效 | 语料内无失效记录 | A§6 |
| ASM-2 | 环境性缺失（DLC/目录/包/预设/音池）应降级而非剥夺资格，且可自动恢复 | `[U]` | 成立 `[F]` | 无 | orphan/dormant 自动恢复 + `GlobalOnly`/fallback 回落＝制度化 | A§2 A§6 |
| ASM-3 | 玩家可见的名称/图标/发现信息只是显示与候选提示，永不是资格判据 | `[U]` | 成立 `[F]` | 无 | 架构 §2 与设置 §产品表面双写同一假设 | A§2；B |
| ASM-4 | 稳定的键空间与字段序（动作序号、srdiag 字段序、注册表句子）是兼容面本身，须闭合而非开放 | `[U]`（v1 层）/ 2026-08-22 | 成立 `[F]` | 无 | v1 声明 byte-immutable；动作键声明 append-only | C；A§6 |
| ASM-5 | 触发粒度规则可作纯数据链入 kernel harness，Verse 采样留在 adapter | 2026-09-13 版本层 | 成立（合同口径）`[F]`；实测 `[U]` | 无 | 合同自述；等价/实测证据在 PKG-2 | A§3 |
| ASM-6 | 不做迁移承诺：无证据即不得推断或 promise 无关设置的迁移；配置维度彼此独立 | `[U]` | 成立 `[F]` | 引入迁移工具则需重开 | 明文禁令 | A§5 |
| ASM-7 | RimWorld 已解析的 `CurLifeStage.developmentalStage` 足以映射年龄桶，无需自建年龄判定 | `[U]` | 成立 `[F]`（含残差 `Toddler` 在 1.6 无原生阶段、仅存 ABI 桶） | 无 | 合同自承残差 | A§6 |
| ASM-8 | 语音包只贡献声音，绝不改行为/timing/capability/distance/mood；fallback 末端由维护者 C# 数据决定 | `[U]` | 成立 `[F]` | 无 | 模式语义 + `BuiltInFallbackCatalog` 归属维护者 | A§6 |
| ASM-9 | 新机器可读事实必须带版本标记（`fmt=2`）且不与 v1 混写，旧解析器才能容忍 | 0.3.1 | 成立 `[F]` | 无 | once-key 按 `log-v1`/`log-v2` 分域＝推论落地 | C |
| ASM-10 | 本日志合同足以指导机读解析，但**不**构成实机会话证据（证据自限） | 2026-08-22 | 成立（文档自我声明）`[F]` | 下游把它当实测证据即被误用 | 无反证、也**无正证**（GAP-1） | C L3 |

## 5. 重大失败 / 成功与认识论分账（INC / EV / C / I / U）
> 本包主源为规范文本，**语料内没有事故叙述**；INC 条目全部来自「以否定式留存的废止口径与结构性防护」。

| id | 类别 | 现象 → 根因 → 处置 | 强度 | src |
| --- | --- | --- | --- | --- |
| INC-1 | 回归风险（预防性禁令） | 缩放资格门（含 `CurrentZoom <= Close`）错误阻断距离衰减 ← 把「视图大小」误当「听者-声源距离」代理 → 合同以禁止式废止，现行＝`CurrentViewRect.ExpandedBy(10)` + `distRange` | 论证；废止时点与事故记录 `[U]` | A§4 |
| INC-2 | 失败模式（结构性防护） | 发声被拒后「每 tick 重试」风险 ← 拒绝若不消耗冷却则尝试高频重复 → 规定拒绝消耗与成功播放相同的每动作及共享尝试冷却 | 论证 | A§4 |
| INC-3 | 可观测性缺陷（本包新登记） | 内置 fallback 与 vanilla、Race pack 与 pack fallback 在日志中同值 ← 装配器折叠 `tier` 词汇 → 未处置（保留词汇标为不发出） | 文本证据，无实测 | C；CNF-2 |
| INC-4 | 文档一致性缺陷（本包新登记） | 通则「禁写 pawn 标签」与 schema「必写 `pawn=<label>`」并存 ← 疑为扩字段时未回改通则 `[I]` → 未处置 | 同文件两行直接互斥 | C；CNF-1 |
| EV-1 | 成功（合同自证边界） | 日志协议主动声明「是兼容/机读合同、不是实机会话证据」→ 下游不得把本合同当验证结论；GAP-1 因此记为开放 | 文本明示 `[F]` | C L3 |
| EV-2 | 成功（append-only 兑现） | `Crying`=15、`Giggling`=16、0–14 不变 → append-only 承诺有一次已兑现的历史实例 | `[F]`；时间 `[U]` | A§3 |
| EV-3 | 成功（默认口径落地） | 0.2.3 起出厂默认＝Fallback + 内置 Race Example 启用、只一次性施加于从未设置过模式的配置 → 老玩家显式选择与既有选择不被覆盖（幂等保护） | `[F]`；无实测 | A§6 |
| EV-4 | 验证（唯一带外部门禁语义的条目） | staging 校验 Template→内置镜像的动作/键集合与 SHA-256 同一性、**不断言固定计数** → 示例音频可增量演化而不破校验；违规须上报 | 论证（本包未运行脚本） | A§7 |
| EV-5 | 验证（外部信号入决策） | 2026-08-23 维护者据 Steam 评论反馈放行设置 UI 唯一例外；根因＝玩家对 Eat 触发粒度的抱怨（内容未载）→ 两默认 `false` 字段 + 三级模式进入合同 | `[C]`（反馈原文不在语料内） | B |

- **C-1** `[C]`：`logging-protocol.md` 作者（身份 `[U]`，2026-08-22）声明合同来自对四个源文件的 source review、并自陈不是实机会话证据；**无独立证据支持**（PKG-2 可能含相关证据，本包不代其判定）。
- **C-2** `[C]`：2026-08-23 Eat 例外＝「维护者决定、承接 Steam 评论反馈」；反馈内容、条数、原文均未在语料内出现。
- **C-3** `[F]`+`[C]` 混合：Ratkin 档案「刻意只有 15 个键」＝文本 `[F]` + 意图 `[C]`；Template「15 目录 / 41 OGG」＝可回指文件系统的自述计数、标 `[C]`（文档同时声明该计数可变），本包未做外部取证。
- **I-1** `[I]`：「合同把什么算破坏兼容变成可机械判定的集合」＝DEC-31 + DEC-14 + DEC-5。替代解释：只让违规**可枚举**、不等于**可自动发现**（合同未规定任何检查门；README §2 规则 3 反而禁止本管道新增检查门）。
- **I-2** `[I]`：「玩家侧最难自证的语义是 fallback 层与静默末端」＝DEC-13 + DEC-15 + DEC-36 ⇒ 走内置 fallback 的派发在日志上与走 vanilla 不可区分。替代解释：`tier=vanilla` 可能**就**表示内置末端、折叠是有意命名（合同未定义 `vanilla` 语义边界 → U-1）。
- **I-3** `[I]`：「七次点击 + 第四页」⇒ 日志/诊断面在玩家默认视图下不可见、CNF-1 的实际暴露面受 dev 门控限制。替代解释：Daily 事件（如 `voicepack.pack.rejected`）仍可能携带其他可识别值；且解锁状态本身持久化（`developerToolsEnabled`）、一次解锁长期有效。
- **U-1** `tier=vanilla` 在产品语义上是否＝「内置 fallback 末端」＝缺**词汇定义**（合同只列可发集合与折叠行为）→ 影响 DEC-13/DEC-15 可观测性判定与 I-2。**U-2** `audio.dispatch.ok` 的 characterization surface 由哪个测试/工具承载未记＝缺**测试标识** → 影响 DEC-36 回归安全判断。**U-3** 本包未获任何「实现与合同不一致」的反例记录＝缺**审计报告/等价评审结论**（PKG-2）→ 影响 DEC-1 是否已被实践检验。**U-4** `Toddler` 桶何时会被真实填充未记载 → DEC-12 年龄 ABI。**U-5** `map-lifecycle reset` 未实现是缺陷还是有意＝缺**决策记录** → DEC-32 会话膨胀判断。**U-6** 17 动作 ABI 与「未来 action gates」的关系边界未记载 → 作者可预期性。**U-7** `SqueakEatOccurrence` 三模式在日志协议中**无专属字段或事件**（v2 事件表未含 Eat 模式）→ 本包内成立且不改写；**编排方跨包回填（2026-09-17）**：PKG-6 主源已记载该裁决＝本项目**明确不新增日志事件**（理由句「日志协议是 characterization 冻结面」）、替代可观测性方案＝可选 Dev 诊断面板（Debug 面、不属日志协议）、未实施 ⇒ 该面不是「缺失」而是「已裁决」，但 **OQ-22 仍开**。**U-8** 绝大多数条文无时间与主体记录（合同为无签名规范文本）＝缺**条文级时间层** → §2 只能靠版本层与 `(冻结动作)` 支撑。

## 6. 矛盾与未决问题（零丢弃）
> 权威分层只用于「若必须选一个建议以谁为准」，不用于删除冲突记录；以下一律**并列保留**。

| id | 冲突双方（含时间层） | 冲突点 | 可能解释 `[I]` / 建议回查点 | src |
| --- | --- | --- | --- | --- |
| CNF-1 | (a) v1 基线通则：不得把本地化文本、**pawn 标签**、文件系统路径写入协议；(c) 0.3.1：`Only DefNames are written — never labels, HAR package names, or player-mutable text`；(b) 0.3.2 扩字段：`audio.route.selected` 字段表含 `pawn=<label>` | 同一文档既禁 pawn 标签入协议、又在 v2 事件 schema 规定 `pawn=<label>`；通则与其紧邻字段表互斥 | [I] 通则限定在 `race`/`xenotype` 身份字段语境、未打算覆盖事件专属字段；[I] 0.3.2 扩字段时未回改通则（一致性债）。percent-encode 只改表示形式、**不构成匿名化**。回查 `Logging/SqueakLogProtocol.cs` 的 payload 类型与 sanitize 顺序；查 0.3.2 提交信息/发布评审（PKG-4）；隐私面 → PKG-5 | C |
| CNF-2 | (a) 架构合同（末次 2026-09-13）：Fallback 链四层末端（Xenotype pack → Race pack → pack fallback → built-in fallback → silence）、Remix 含四层等层选择；(b) 日志 0.3.1 定义 + 0.3.2 收缩：可发 `tier` 只有 `xenotype_pack`/`race_pack`/`vanilla`/`-`，`pack_fallback`、`built_in_fallback` **保留不发出**，`PackFallback`→`race_pack`、`BuiltInFallback`→`vanilla` | 产品层四/五段语义在可观测层**折叠为三值**：走 pack fallback 与走 Race pack 不可区分；走内置 fallback 与走 vanilla 不可区分 | [I] 刻意的日志学简化（tier 表「发行来源」而非「解析层」）；[I] tier 尚未与 0.3.x 解析层演进同步。**归因损失是否已知取舍＝语料未记载、禁止写成「设计如此」**。回查 resolver 的 tier 枚举与装配器映射；`SqueakDebug` overlay 可能暴露真实 tier → GAP-4 | A§6；C |
| CNF-3 | `logging-protocol.md` 状态戳 2026-08-22，但同句记载 0.3.2 事实（末次提交 `56b6f05`＝2026-08-23） | 自称「2026-08-22 记录的合同快照」却含 0.3.2 变更 ⇒ 快照边界与声明的时间戳不一致 | [I] 多会话追加时更新内容句而未更新状态行 ⇒ 状态行已成陈旧元数据。回查：以内容里的版本标记（`0.3.1`/`0.3.2`）而非日期戳作时间层锚；精确归因需逐段提交史（本包未做外部取证） | C L3 |
| CNF-4 | (a) 2026-08-23 裁决 + 2026-09-13 文本：Eat 例外 Scribe 面「只 add-only 两个默认 `false` 字段（默认值省略、settings schema **不 bump**）」；(b) 2026-08-22 ABI 声明：「Internal surfaces (kernel, domain model, **fallback profile schema**, and the future action gates) remain under the 0.x revision window」 | 对象不同、不直接矛盾，但语料**无一处**说明「玩家 settings schema 是否属于任何已冻结面」⇒「能否 bump settings schema」既无承诺也无禁区 | [I] 意图＝settings 面不公开冻结、不承诺永不 bump、仅要求走 add-only；[I] 也可能是遗漏。回查 `SqueakyRatkinSettings.ExposeData` 与历史 bump 记录、是否存在 settings 版本字段（合同未提） | B；A§6 |
| CNF-5 | (a) `Crying`/`Giggling` 无内置 SoundDef 与内置 fallback 条目 ⇒ 无包声明即静默；(b) 同文档同版本层：Remix 是「跨四层等层选择」，又称未声明 pack fallback 的动作保留「冻结的三层 Remix 形状」 | Remix 在缺 fallback 的动作上退化为三层；若该动作同时无内置档案（即 `Crying`/`Giggling`）⇒ 实际可选层只剩 Xenotype/Race，合同未写明该情形的抽样形状与是否整体静默 | [I] 「frozen three-tier shape」正是此情形的兜底形状、缺内置层时该层自然缺席；[I] 也可能存在「三层权重被重分配」的第三种实现。回查 resolver 的 Remix 层级装配代码与 harness 用例；PKG-2 的动作门/路由公理机械验证是否覆盖此形状 | A§3；A§6 |
| CNF-6 | (a) 元规则：冲突时「identify the mismatch and resolve it in the authoritative layer」（不得静默归一）；(b) 本包实测同一文档内部 CNF-1…CNF-5 五处未被消解 | 元规则要求消解，文档现状保留多处内部不一致（含跨文件） | [I] 元规则针对「实现 vs 合同」、不针对合同内部措辞一致性；[I] 亦可能这些点尚未被任何会话识别为冲突——本包即首次登记。交阶段 B/C 判定是否升为维护者待办（本包不代拟修复方案） | A§1 |
| CNF-7 | (a) README §2：唯一写入面应为仓库根临时目录、产物不进仓库历史；(b) 2026-09-17 维护者裁决：OUT_DIR＝`docs/consolidation`、产物属文档面 | 产物落点与「不提交」设计相反 ⇒ 隐私门（入库前须过 `scripts/privacy-audit.ps1` 默认模式）从「默认不需要」变成**必须** | [I] 维护者有意把产物升格为文档、风险从「泄露于历史」转为「泄露于提交」。回查：提交前对 `docs/consolidation/**` 执行既有隐私审计脚本；确认 `prompts/**` 是否已入允许面（README §10 三选一）→ OQ-11 | D§2 |
| CNF-8 | (a) 公开 ABI 自「the first released version that carries the 0.3.1 ABI」起生效（2026-08-22）；(b) 0.3.2 扩展同一日志面且声明 v1 不变 | 「首个承载 0.3.1 ABI 的发行版本」是哪一版（0.3.1？0.3.2？prerelease 算不算发行？）语料内不可判定 ⇒ 公开冻结的**起点**未闭合、0.3.2 的扩展是「冻结后合法 add-only」还是「冻结前塑形」无法归类 | [I] 0.3.1 是承载版本、0.3.2 属冻结后 add-only（最保守读法）；[I] 若 0.3.1 未正式发行而只有 0.3.2-pre1 ⇒ 起点后移。**本条不得删除**（合同文本自身仍是描述性锚点），即使 OQ-1 已由 PKG-4 证据闭合 | A§6；C L3 |

| id | 问题 / 风险 | 类型 | 影响面 | 当前状态 | src |
| --- | --- | --- | --- | --- | --- |
| OQ-1 | 「首个承载 0.3.1 ABI 的发行版本」是哪一版？0.3.2 / 0.3.2-pre1 是否算 released？ | 依赖外部（发布事实） | 第三方包作者的冻结起点；日志扩展的合规归类（CNF-8） | **已由 PKG-4 证据闭合：0.3.1 无正式版；0.3.2 仅 `v0.3.2-pre1` GitHub prerelease（工作并入 0.3.3）** ⇒ 冻结起点后移；**CNF-8 保留不删** | A§6；GAP-3 |
| OQ-2 | `pawn=<label>` 是否被接受为日志常态字段？若否、退役/净化方案为何？ | 待裁决（含隐私面） | 日志兼容面（v2 字段不可随意删）× 隐私承诺 | 未决；本包仅登记冲突、不裁决 | CNF-1 |
| OQ-3 | 是否需要把 `tier` 拆细以区分 `pack_fallback` / `built_in_fallback`？ | 未验证 / 待裁决 | 排障可观测性、fallback 承诺的可证明性 | 未决（保留词汇已定义但未发出） | CNF-2 |
| OQ-4 | 玩家 settings schema 是否承诺永不 bump？add-only 是否为其唯一约束？ | 待裁决 | 存档/设置兼容边界（合同未给 settings 面任何冻结声明） | 未决 | CNF-4 |
| OQ-5 | `Crying`/`Giggling` 在 Remix 且无任何包音频时的抽样形状（三层缺一层 / 两层）未定义 | 未验证 | 17 动作 ABI 与默认静默承诺的一致性 | 未决 | CNF-5 |
| OQ-6 | 「未来 action gates」（明确未冻结）与「内置动作键 append-only」（明确公开）之间的边界在哪？ | 待裁决 / 需授权 | 作者可预期性；内部重构自由 | 未决 | DEC-14；U-6 |
| OQ-7 | `map-lifecycle reset` 未实现是缺陷还是有意？once-key 上限 1024 触发清空是否会造成同一会话内重复噪音？ | 未验证 | 长会话日志体积与噪声 | 未决 | DEC-32；U-5 |
| OQ-8 | orphan 条目是否有数量/保留期上限？大量长期失联目标的处理口径未记载 | 待裁决 | 设置文件膨胀与 UI 可读性 | 未决（现行口径：保留 + 显式忘记） | DEC-17 |
| OQ-9 | `SqueakyRatkin_Profile_<race>.xml` 的「当前版本」如何判定（更旧副本从 C# 源重建的判据）？ | 未验证 | 回退档案的稳定性 | 未决 | DEC-16 |
| OQ-10 | 实现权威路径清单是否需覆盖 `1.6/Assemblies` 之外的 flavor（1.5/1.6 双版本）？合同只列 `1.6/…` 路径 | 依赖外部（发行结构） | 兼容边界的适用范围 | 未决（本包不做仓外取证） | DEC-22 |
| OQ-11 | `prompts/**` 是否已被维护者追加进收敛任务的允许面？产物落 `docs/` 后是否仍需按 README §10 三选一处置？ | 需授权 | 最终收敛提交的内容边界（混入套件的失败模式） | 未决；需编排方/维护者裁决 | CNF-7；DEC-37 |
| OQ-12 | 合同无签名/版本/生效日期元数据（除个别内嵌日期）——是否要求为三份合同补「版本头」？ | 待裁决 | 时间层可判定性（U-8 的根因） | 未决 | DEC-1；CNF-3 |
| OQ-22 | （跨包，载体＝PKG-6）Eat 粒度既已裁决不新增日志事件，替代方案「可选 Dev 诊断面板」是否推进？ | 待裁决 | DEC-7 的支持排障路径；U-7 | **仍开**（面板未实施；本包不代 PKG-6 裁决） | E |

## 7. 候选教训
| id | 教训 | 支撑 | 强度 | 适用边界 | src |
| --- | --- | --- | --- | --- | --- |
| LES-1 | 把兼容面写成**闭合枚举 + 固定字段序 + fails-closed 校验**可让下游机械判定违规，而非靠人读散文 | DEC-31、DEC-14、DEC-33 | 强（三份合同一致采用） | 仅适用于**对外可解析产物**（日志、作者 XML ABI）；内部实现面刻意保留 0.x 修订自由、不可套用 | C；A§6 |
| LES-2 | 规范文档应自我声明**证据边界**（「是机读/兼容合同、不是实机会话测试证据」），避免下游把条文升格为验证结论 | DEC-31 / C-1 / EV-1（同句自限） | 强（文本明示） | 适用于任何以 source review 为来源的规范文本；不能替代真实回归证据 | C L3 |
| LES-3 | 冻结声明必须绑定**可指认的版本锚点**；「first released version that carries X」这类描述性锚会造成起点不可判定 | CNF-8 / OQ-1 | 中等（本包单点观察；跨包是否普遍需阶段 B 判定） | 适用于一切「自某版本起稳定」的承诺；对已用显式版本号 + 日期的条目（DEC-14 的 2026-08-22 行）不适用其前半 | A§6 |
| LES-4 | 追加新版本事实却不更新状态行会让时间层失效（状态戳早于内容版本层） | CNF-3 / TL-8 | 中等 | 适用于多会话追加式文档；单次成文的文档不适用 | C L3 |
| LES-5 | 通则式禁令若不与后续 schema 扩展联动复核，会与自家字段表直接互斥 | CNF-1 / INC-4 | 强（同文件两行互斥、文本自证） | 适用于任何含「不得写入 X」通则的机读协议 | C |
| LES-6 | **tentative**：产品层分段语义若在可观测层被折叠，兼容承诺仍成立但**排障能力**静默下降；应在折叠处显式声明「哪个产品层不可归因」 | CNF-2 / INC-3（合同未标注归因损失） | tentative（仅一处证据、无法排除「折叠即有意设计」→ U-1） | 适用于 tier/枚举映射类设计；不构成「必须拆细」的普适结论 | C |
| LES-7 | 用**否定式**表述废止口径（`Do not reintroduce…`、`must not be narrowed by default`、`never calls WriteSettings()`）能把落选方案的硬细节留在现行文本里、避免决策史丢失 | DEC-9（zoom 门）、DEC-7（默认粒度）、DEC-16（`WriteSettings()`/防抖队列）、ALT-1 | 强（多处一致手法且给出理由句） | 适用于合同类规范文本；事故细节与原因仍需过程文档承载（本包无事故叙述） | A§3 A§4 A§6 |

## 8. 证据缺口（GAP）
| id | 想回答的问题 | 为什么现有语料答不了 | 需要什么才能回答 |
| --- | --- | --- | --- |
| GAP-1 | 三份合同的条文是否与当前实现逐条一致？ | 合同自述来源是 source review 且明文声明**不是**实测证据；本包按规则不做仓外/代码取证 | 实现-合同对照记录（等价评审类）或 harness 用例清单 → PKG-2 |
| GAP-2 | `audio.route.selected` 的真实输出行形状（字段是否齐、`pawn` 实际值形态）？ | 语料只有 schema、无样例记录（且本包禁止复制日志摘录） | 已净化的样例日志或机读解析测试 |
| GAP-3 | 0.3.2（及 0.3.2-pre1）是否正式发行、在哪些渠道？ | 发布事实不在本包主源内 | PKG-4 各渠道状态矩阵（→ OQ-1 已据此闭合） |
| GAP-4 | 诊断叠加层（`SqueakDebug` / workbench）是否暴露真实 fallback 层？ | 合同只规定日志 tier 折叠、未描述 overlay 显示内容 | 读 `Debug/SqueakDebug.cs` 或 overlay 相关规范（语料外） |
| GAP-5 | Eat 三级模式是否有回归测试守护（合同要求 must stay documented and tested）？ | 本包主源只声明要求、未记测试标识（U-2） | 测试用例清单或 CI 记录 → PKG-2（C1–C19 证据链） |
| GAP-6 | `Toddler` 桶是否存在任何可发行路径（有 mod 提供该阶段时行为如何）？ | 合同只记「1.6 无原生阶段」 | 跨 mod 兼容性评估 → PKG-3（F1–F8 / U1–U4） |
| GAP-7 | 玩家 settings 文件是否需要版本号/世代字段（DEC-26「保留待保存代次」暗示存在代次概念）？ | 合同未定义 settings schema 版本机制 | settings 持久化实现或规范补充 |

## 9. 锚点（ANCH-1…10：必须逐字保留；数值不得增删改；来源见 §10）
- **ANCH-1 标识符 / 类型 / 键 / 前缀 / 文件名（逐字）**：`SR_`；`CompSqueaker`；`NewRatkinPlus`；`XenotypeDef.defName`；`ThingDef.defName`；`FactionDef.defName`；`CanUseXenotype`；`IsRatkin`；`GlobalOnly`；`SqueakVoicePackDef`；`raceDefName`；`scope`；`targetDefName`；`weight`；`fallbacks`；`actions`；`action`；`ageTag`；`IsEgg`；`sounds`；`PackKey`；`voicePackMode`；`allowEasterEggSounds`；`BuiltInFallbackCatalog`；`SqueakFallbackProfileStore`；`SqueakyRatkin_Profile_<race>.xml`；`WriteSettings()`；`ModSettings`；`PackFallback`；`BuiltInFallback`；`Race`；`Vanilla`；`SqueakDevLoggingMode`；`developerToolsEnabled`；`Prefs.DevMode`；`ShouldEmitDev`；`SetDevLoggingMode`；`SqueakDebug.ResetLoggingSession`；`SqueakEatOccurrence.ResolveMode`；`SqueakEatOccurrence.ChewingToilDebugName`；`SqueakEatOccurrence.AllowsOccurrence`；`eatOnlyDuringChewing`；`eatIncludeDrugs`；`IEatingDriver.GainingNutritionNow`；`JobDriver.CurToilString`；`ChewIngestible`；`CachedNutrition`；`PostLoadInit`；`SoundInfo.pitchFactor`；`volumeFactor`；`InMap(TargetInfo(Pawn))`；`distRange`；`CurrentViewRect.ExpandedBy(10)`；`Verb.TryCastShot`；`MentalStateHandler.TryStartMentalState`；`MentalFitDef`；`CurLifeStage.developmentalStage`；`JobDefOf.Ingest`；`1.6/Patches/Ratkin_AddSqueakComp.xml`；`CompProperties_Squeaker.distancePresets`；`SR_OfficialExample_Race`；`SR_ExampleTemplate_Race`；`clipFolderPath`；`<lowercase packageId>/<PackDef.defName>/<Action>/`；`Source/SqueakyRatkin/CompSqueaker.cs`；`Source/SqueakyRatkin/Patches/`；`Source/SqueakyRatkin/SqueakyRatkinSettings.cs`；`Source/SqueakyRatkin/SqueakEatOccurrence.cs`；`Source/SqueakyRatkin/SqueakVoicePackModels.cs`；`SqueakSettingsGameContext`；`scripts/stage-package.ps1`；`Logging/SqueakLog.cs`；`Logging/SqueakLogProtocol.cs`；`Debug/SqueakDebug.cs`；`[SqueakyRatkin] `；`srdiag fmt=1`；`srdiag fmt=2`；`log-v1`；`log-v2`；`<path>`；`N/A`。
- **ANCH-2 动作名与序号（17 个，append-only，逐字，顺序即序号 0–16）**：`Call`、`Eat`、`Sleep`、`Wounded`、`Select`、`Move`、`Social`、`Joy`、`Death`、`Draft`、`Undraft`、`Attack`、`Work`、`Equip`、`MentalBreak`、`Crying`(=15)、`Giggling`(=16)；内置 fallback 档案 Ratkin 键数 = **15**（`Call` 至 `MentalBreak`，缺 `Crying`/`Giggling`）。
- **ANCH-3 年龄/彩蛋/触发枚举**：`ageTag` ∈ {`Baby`, `Toddler`, `Child`, `Adult`}（缺省 = 全年龄；`Toddler` 为 ABI 桶，RimWorld 1.6 无原生阶段）；映射：Newborn/Baby → Baby、Child → Child、其他/缺失 → Adult；周期方式 ∈ {`EachTime`, `RandomOneShot`, `External`}。
- **ANCH-4 默认值 / 阈值 / 计数**：`weight` 默认 `1`；`allowEasterEggSounds` 默认 `false`；`eatOnlyDuringChewing` 默认 `false`；`eatIncludeDrugs` 默认 `false`；啤酒 `Nutrition 0.08`；ambrosia `Nutrition 0.2`（营养权威 `CachedNutrition > 0`）；出厂默认自 0.2.3 = Fallback + 内置 Race Example 启用。
- **ANCH-5 日志字段序（整段逐字，顺序即合同）**：v1 核心 = `fmt lvl vis evt action target pack build build_id`；v1 事件专属固定序 = `reason sound source count dispatched suppressed_detail enabled ex_type ex_inner ex_site ex_msg`；v2 核心 = `fmt lvl vis evt action target pack race [xenotype] build build_id`；percent-encode 白名单 = `A-Z`、`a-z`、`0-9`、`.`、`_`、`~`、`:`、`/`、`@`、`+`、`-`；事件级 v2 字段序 —— `audio.route.selected` = `sound` `tier` `egg` `suppressed_detail` `pawn` `pawn_id` `pawn_faction` `pawn_ctrl`；`fallback.profile.store_failed` = `ex_type` `ex_inner` `ex_site` `ex_msg`；`settings.origin` = `settings_origin=FreshCreated|LoadedFromFile`；`hook.mental_fit.unavailable` = 无专属字段。
- **ANCH-6 注册表计数与 reason 集合**：v1 锁定记录 = **28** 行（fmt=1，byte-immutable）；0.3.1 v2 扩展记录 = **4** 行（`settings.origin`、`audio.route.selected`、`hook.mental_fit.unavailable`、`fallback.profile.store_failed`）；表内合计 32 行（按表行实测）。reason 闭包：`voicepack.pack.rejected` ∈ {`duplicate_key`, `domain_filtered`}；`xenotype.discovery.candidate` ∈ {`har_hint_filtered`, `har_official_filtered`}；源集合 ∈ {`declared_pack`, `selection`, `preset`}，以 `+` 连接。
- **ANCH-7 距离 / UI / 冷却数值**：底层 SoundDef range `15–70` cells；玩家设置默认 Balanced 预设 `15–50`；`CurrentViewRect.ExpandedBy(10)`；**7 次**点击解锁（第四页）；合并保存约 `350 ms`；相机指示器读 `Find.Camera.transform.position.y` 与 `Find.Camera.orthographicSize`（禁止由 `RootSize` 反推）。
- **ANCH-8 日志量控数值**：once-key 上限 `1024`（达上限即清空并接受新键）；每动作细节最多**每 5 秒**一条；`trigger.outcome.summary` 至多**每 60 秒**一条；异常消息截断 `256` 字符；三个模式值 `Auto`/`Enabled`/`Disabled`。
- **ANCH-9 设置 UI 最小验收矩阵（逐字维度）**：语言与比例 = EN、简体中文；`100%`、`125%`、`150%`；空间 = normal、narrow、高度受限（无重叠、负 Rect、不可达控件、滚动串层）；状态 = 主菜单、游戏内、无选择、无 Biotech、可用/空/orphan/dormant/失败 Catalog；输入 = slider、文本、列表、`?`、折叠、Remix 两步确认及每层取消/关闭无穿透；持久化 = 立即运行时生效、约 350 ms 合并保存、close flush、重开/重启保留、失败状态诚实；运行时 = 不引发 GUIClip/ScrollView 配对、NRE 或 Unity 主线程错误，No-DLC 不访问 Biotech 路径。
- **ANCH-10 Example 计数 + 授权/外部状态边界**：Example 基线 **15 动作目录 / 41 OGG**，每动作＝Attack 3、Call 4、Death 2、Draft 3、Eat 2、Equip 2、Joy 3、MentalBreak 1、Move 3、Select 3、Sleep 3、Social 3、Undraft 3、Work 3、Wounded 3；作者资源根形态＝`<lowercase packageId>/<PackDef.defName>/<Action>/`；PackKey 公式＝package ID + PackDef defName；发行格式边界＝OGG 建议、WAV 可用非建议、MP3 非建议，且不得把「运行时校验器/游戏兼容性」表述为「发行建议」。**边界**：维护者独有＝内置 fallback 档案内容（C# 数据）+ staging 不变式的「上报不静默修复」口径；玩家可执行的唯一破坏性出口＝确认式「忘记此目标」（连带删除行为 preset + Xenotype VoicePack 选择）、普通 catalog refresh **不得**删档、Remix 需两步确认；第三方碰撞＝项目只校验自身发行 root 与重复扩展名、不全局仲裁；排障工具须编译进 Dev/GitHub/Steam 全部 flavor 但可用性仍受 RimWorld Dev Mode 与活动地图门控；语料外硬约束指针＝[xref: AGENTS.md#（操作硬约束索引，含卸载安全）]。

## 10. 来源回引（source map；条目内短指针在此展开）
| 短 | 源文件（规模 / 末次修改） | 被本包吸收的主题 | 章节/行 | 载体 |
| --- | --- | --- | --- | --- |
| A | `docs/project-architecture-contract.md`（73 行 / 16,049 字节；`e0608f7` 2026-09-13；`(冻结动作)`） | §1→DEC-1/2；§2→DEC-3/4；§3→DEC-5/6/7/8；§4→DEC-9；§5→DEC-10/11；§6→DEC-12…17；§7→DEC-18…22 | §1 L3 L7 L9；§2 L13 L15 L17；§3 L21 L23 L25 L27 L29 L31 L33；§4 L37 L39；§5 L43 L45 L47 L49；§6 L53 L55 L57 L59 L61；§7 L65 L67 L69 L71 L73 | PKG-1 |
| B | `docs/settings-ui-product-contract-zh.md`（42 行 / 6,027 字节；`e0608f7`） | 产品表面→DEC-23…29；capability 与布局→DEC-29；非目标与实现路径→DEC-30（含 2026-08-23 唯一例外）；最小验收矩阵→ANCH-9 | §产品表面 L7 L9 L11 L13 L15；§capability L19 L21 L23；§非目标 L27 L29 L31；§最小验收矩阵 L35 L37 L38 L39 L40 L41 L42 | PKG-1 |
| C | `docs/logging-protocol.md`（126 行 / 13,779 字节；`56b6f05` 2026-08-23） | Closed Typed Facade→DEC-31；Detailed-Logging Mode→DEC-32；`srdiag` v1→DEC-33；`srdiag` v2 (0.3.1)→DEC-34/35；Once and Success-Path Volume Control→DEC-36；Stable Event Registry→DEC-35 + ANCH-5/6；散段 L124 L126→DEC-36 | L3 L7 L9 L11 L15 L17 L19 L21 L23 L25 L27 L31 L34 L40 L41 L44 L46 L48 L50 L52 L55 L58 L59 L60 L62 L64 L66 L67 L68 L69 L71 L73 L75 L77 L79 L81 L85 L87 L120 L122 L124 L126 | PKG-1 |
| D | `prompts/docs-consolidation/README.md` | 权威分层 §4、派发纪律 §5、写入面与隐私门 §2、产物处置 §10 | §2 L35–L41；§10 L177–L194 | CNF-7 / DEC-37 |
| E | `docs/handoff-eat-occurrence-granularity-zh.md` | 编排方补记跨包回填：Eat 粒度不新增日志事件的裁决与替代可观测性方案 | §5.1 L206–L208；§7.1 L262；§8.8 L273 | 载体包 PKG-6；本包 U-7 / OQ-22 |
- 跨包 xref（不重述）：PKG-2（0.3.x 决策史 / 等价评审 / C1–C19）、PKG-3（通用化与 US 拆分）、PKG-4（0.3.1/0.3.2 发行事实 → OQ-1）、PKG-5（隐私历史债务 → CNF-1）、PKG-6（Eat 规格比拼与验收矩阵）。
- v1 §12 声明的未吸收项（原样带下）：B 引言指向 `../modding_documents/rimworld-mod-ui-design-methodology-zh.md` 的通用 IMGUI 建议链接（语料外文件，只留指针）与指向 A 的纯导航链接；C 的 **28 条 v1 完整英文人读句逐行抄录**（重复/例行：句子由闭合注册表锁定，抄录不构成决策信息；关键句已在 DEC-32/35/36 与 ANCH-5/6 保留，完整性核对以 ANCH-6 行数替代）；三份文件中任何可能含本机路径/盘符/日志原文的片段（隐私门：只以形态描述转述净化规则，未复制样例串）。

## 11. 供下游独立挑战的质疑钩子
**G1 「现行有效」的地位（DEC-1…DEC-37 全体）** — 可能错在：全部「现行有效」以合同自述为背书、无任何实现证据（GAP-1、U-3、ASM-10）；「本包内没有反例记录」不等于「没有反例」。回查：`docs/project-architecture-contract.md` 全文 ↔ `Source/SqueakyRatkin/`；等价评审 14 行与 C1–C19 链在 PKG-2。被推翻 ⇒ §3 全表状态列与 LES-1 同时失效。
**G2 冻结起点（OQ-1 / CNF-8 / DEC-14）** — 可能错在：把 PKG-4 的「0.3.1 无正式版、0.3.2 仅 `v0.3.2-pre1`」当成已闭合答案，而合同锚仍是描述性短语；「prerelease 算不算 released」这一子问题仍未答。回查：`A §6 L55` 原文 + PKG-4 渠道状态矩阵。影响 DEC-34/DEC-36 的合规归类与作者可预期性。
**G3 隐私与可观测性（CNF-1 / OQ-2 / CNF-2 / OQ-3）** — 可能错在：三条 `[I]` 解释（L59 语境限定、折叠即有意简化）均无作者佐证；percent-encode **不是**匿名化。回查：`C L46 L59 L67 L73`、`Logging/SqueakLogProtocol.cs`、PKG-5 隐私债务线。影响 I-2、INC-3、INC-4、LES-5、LES-6。
**G4 静默末端语义（DEC-13 / DEC-15 / CNF-5 / OQ-5）** — 可能错在：把「冻结的三层 Remix 形状」当成在 `Crying`/`Giggling` 上仍成立的四层链。回查：`A §6 L57`、resolver 的 Remix 层级装配与 harness 用例。影响「默认静默」承诺的可证明性。
**G5 Eat 例外（DEC-7 / DEC-30 / OQ-22）** — 可能错在：把「已裁决不新增日志事件」读成「无需可观测性」；面板未实施 ⇒ 排障路径实际缺位。回查：`A §3 L25–L31`、`B L29`、E §5.1/§7.1/§8.8、PKG-6 DEC-5 + OQ-22。
**G6 套件自引用（DEC-37 / CNF-7 / OQ-11）** — 可能错在：把维护者一次调度当成永久允许面；本 compact 自身亦落入该未决面。回查：`D §2 L35–L41`、`§10 L194`、coverage-check §6。

## 12. 压缩损失与删减账本
| 丢弃/降级内容 | 原属 | 类型 | 理由 | 是否可回查 |
| --- | --- | --- | --- | --- |
| DEC 条文的「背景与触发信号」与部分「理由与当时假设」散文（保留触发信号中的关键否定式与例外） | PKG-1 §3 | P2 叙述 | 背景几乎全等于 §4 ASM 要旨（已在 §4 全量保留）；同一背景在多条 DEC 中只写一次 | 包内 §3 各行；A/B/C 原文 |
| DEC 表内复述的数字与字段序（改为「见 ANCH-n」） | PKG-1 §3 | 压表达 | §9 逐字保留、零丢失；避免 §3 与 §9 双写 | 包内 §9；C/A 原文 |
| 「关联」网（DEC↔DEC 交叉引用） | PKG-1 §3 | P2 | 只保留反转与冲突指向（→ CNF/OQ/U/INC/EV/LES），其余网由 id 顺序可推 | 包内 §3/§6 |
| C/I/U 的完整推断链展开语句 | PKG-1 §5 | 压表达 | 结论、替代解释、影响面三项全部保留，仅删连接语与重复的 src 复写 | 包内 §5 |
| v1 §13 自检的逐行机械核对与规模自报 | PKG-1 §13 | P2 | 结论已并入 §0 对账（含「DEC 缺 src 0 行；GAP-3/GAP-6 无直接 src」这一不隐藏的登记） | 包内 §13；coverage-check §2 |
| **未删**：任何 DEC / ALT / CNF / OQ / GAP / ASM / ANCH / TL / LES / INC / EV / C / I / U | 全部 | P0 | 02 §6 规则 6 禁止以「重复/过时」为由丢弃；§4 P0 不可压缩条目 | — |
- 实测压缩比与本节字符数见交付回复；瓶颈＝§9 必须逐字的锚点面（约占本文件 1/3）。已用手法：表格化、短指针集中化（§10）、数字指向锚点、语言压缩（未动数字、标识与认识论标签）。**未为达标删任何 P0。**
