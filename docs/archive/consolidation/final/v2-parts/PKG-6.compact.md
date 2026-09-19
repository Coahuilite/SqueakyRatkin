# PKG-6 compact v2：交接、功能规格与开放裁决

## 0. 元数据与声明
- 源：PKG-6 v1（77,140B/409 行，frozen 2026-09-17，repo_rev 4df9594…，未拆包）。缩写 H=handoff-0.3.3-zh.md（§1–§9 写于 2026-08-23 未提交层；§10=2026-09-13 接续层）、E=handoff-eat-occurrence-granularity-zh.md（08-23 同日分段）。引用形 H§4L46、E§5.1。
- ANCH：v1 锚点族**无 `ANCH-n` 编号**→§9 保留 9 个族名（设置/字段/UI、规则/实现符号、vanilla 符号、数值、验收/工具、版本/发布面、流程/隐私工具、编号表与兼容标识、授权边界）。
- `coahuilite.squeakyratkin`：按 v1 逐字保留（packageId 字符串术语、非 Steam `PublishedFileId` 数值；出现处 DEC-8/OQ-7/§9）。v1 提示阶段 B 若判定禁写可改形态描述——本 pass 原样保留并加此注。
- 计数：DEC12｜ALT6（含 ALT-0 已否决）｜ASM8｜CNF6｜OQ23（不折叠）｜GAP7｜LES7｜INC4｜EV7｜TL10｜C6 I4 U7｜ANCH 9 族。

## 1. 边界与结论
- 负责：0.3.3 交接面全部决策与状态（产出 A–E、仓库状态、已定案 7 条、开放裁决表、三会话分派、边界）+ 09-13 接续时间层 + Eat 触发粒度规格完整重建（vanilla 事实基线、静默 hole、需求规格、方案 A/B/C/D、两级开关、验收矩阵）+ 两主源全量开放裁决与未验证面。不负责：runbook/Claim Pack（PKG-4）、US 兼容/迁移正文（PKG-3）、现行合同条文（PKG-1）、流程冗余/隐私债务正文（PKG-5）、MEMORY/TODO（语料外）——只保留关于它们的记载（编号表、指针、验收信号）与跨包裁决线。
- 结论：0.3.3 =「Eat 两级开关+三专项文档+版本推进」交接窗口；08-23 写作时全部改动未提交，09-13 接续会话完成本地三块提交与流程简化后停在远端推送前；Eat 已于 08-23 落地「父 job 级默认+子方案 B+fail-open 回落」三级形态；其余发布/迁移/身份/实机裁决全部开放。[F]
- Top-3：① DEC-1+2/3——Eat 默认 job 级=招牌手感与兼容资产；两级开关与 fail-open 已定案已实施：任何后续会话不得默认收窄、不得把回落改成静默。② CNF-1+DEC-10/11——同文档两时间层；现行有效状态=本地已提交、远端未动、外部操作全部待授权。③ OQ 全 23 条（发布三问+迁移/身份/US/实机+对接收方 8 问）=本开放项在 26 文件语料中的目录页。

## 2. 时间线 TL-1..10
- TL-1 ≤v0.3.0（日期 [U]）：现网 Steam 版 `Eat` 即 job 级派发（机制指纹与写作时工作树一致）→现行仍是默认（开关双关行为不变）。
- TL-2 [U]：玩家 Steam 正面评论——端食物横穿地图"连珠炮""太期待吃饭了"⇒议题触发→已吸收为产品立场依据。
- TL-3 08-23：维护者裁决：默认不变（0.4/US 亦不得默认收窄）+ 新增玩家开关收窄到"真正进食"→定案有效（§4.1 不要重开）。
- TL-4 08-23：fresh-context 对抗复核+RimSage 对 1.6 源码/Def 逐条核对 vanilla；早期判"无公开 toil 权威"→核到 `JobDriver.CurToilString` public⇒B 可行→更正成立（B 已落地）。
- TL-5 08-23 同日分段：Eat 先以单开关（A）落地（§7 前言口径），同日父+子两级亦已实施（§5.1/§7.1/注 2）；前言旧层残留→CNF-2。
- TL-6 08-23：H 写作：分支 dev、未提交未推送、本轮 0 提交、19 已跟踪文件 +161/−17、4 untracked→被 §10 更新（并存 CNF-1）。
- TL-7 08-23：已定案 7 条（§4）、开放裁决表（§5）、三会话分派（§6）、开工前三件事（§9）入册→仍生效（"是否 commit"已被 09-13 执行）。
- TL-8 09-13：接续会话：流程冗余评估（R1–R12/V1–V3/N1–N3）+简化落地（两脚本、runbook 重写、AGENTS.md 增 External-state boundaries、ci/release 接线、SDK 10.0.x）+四项可复跑证据→本地生效（远端未验证）。
- TL-9 09-13：工作树已提交（功能/文档记忆/流程三块分离）、dev 包产出；停在远端推送前（push/PR/merge/tag/Release/Steam 未做）=冻结时点最后口径。
- TL-10 09-17：编排方冻结语料（repo_rev 4df9594…）=现行。

## 3. 决策 DEC-1..12（含 ALT-0..5；Eat vanilla 事实基线）

**DEC-1 Eat 默认保持 job 级（招牌手感，不得默认收窄）**——已定案已实施（§4.1 不要重开；08-23 维护者 [F]）。机制指纹：`CompSqueaker.CurrentAction` 之 `IsEating()`=`Pawn.CurJob?.def==JobDefOf.Ingest`、`Eat` 优先级高于 `Move`⇒端食物赶路全程归 Eat；`1.6/Patches/Ratkin_AddSqueakComp.xml` 配 `Eat=EachTime,144 ticks`（`EachTime` 不抽概率，概率门只在 `RandomOneShot`），`Eat` 不豁免全局冷却⇒节拍 `globalMinIntervalTicks=216`≈3.6 秒现实时间一发（`scaleCooldownWithTimeSpeed` 默认开，1×/2×/3× 一致）；横穿地图 20–30 秒≈5–8 发、站桩咀嚼（500 tick≈8.3 秒）≈1–2 发——默认下 Eat 音频主要响在赶去吃饭路上；触发信号=玩家正面反馈 [C-1]。ALT-0"默认收窄"被否决（=删掉招牌行为，无更细记录）。理由=已发布兼容资产（ASM-1）；0.4/US 同样适用；唯一重开口=OQ-21。后果：默认双关与旧实现完全一致（验收重点）。

**DEC-2 两级开关（父 eating-only+子 include-drugs；A/B 混合三级形态）**——已定案已实施（08-23；技术权威=vanilla 源码逐条核对 [F]）。
**Eat vanilla 事实基线（判定前提；1.6 源码/Def [F]，E§2）**：
| 项 | 要点 |
| --- | --- |
| toil 链（`JobDriver_Ingest.MakeNewToils()`） | `ReserveFood`→走/捡（`Toils_Ingest.PickupIngestible`）→`CarryIngestibleToChewSpot`（端食物走到餐桌）→`FindAdjacentEatSurface`→`chewing=Toils_Ingest.ChewIngestible(…)`（真正吞咽；创建先、yield 后）→`FinalizeIngest`；吃尸体可 `JumpIf(chewing,…)` 循环再吃；营养膏机（`PrepareToIngestToils_Dispenser`）、从背包吃（`eatingFromInventory`）、动物非工具路径**共享同一 chewing toil 对象** |
| 咀嚼特征 | `toil.actor.pather.StopDead();`；`ticksLeftThisToil=Mathf.RoundToInt(thing.def.ingestible.baseIngestTicks*durationMultiplier)`；`ToilMaker.MakeToil("ChewIngestible")`⇒`Toil.debugName="ChewIngestible"` |
| 权威① | `IEatingDriver.GainingNutritionNow`（接口唯一成员；=`IngestibleSource` 未销毁∧`def.IsNutritionGivingIngestible`∧`CurToil==chewing`）——接口稳定但语义是**营养**不是"在嚼" |
| 权威② | `JobDriver.CurToilString`（`CurToil` protected、`CurToilIndex`/`CurToilString` public；`Toil.ToString()`=`debugName??"unnamed"`）——`"ChewIngestible"` 即在吞咽/点燃；依赖 Ludeon 不改 debugName |
| 谓词/默认 | `IsNutritionGivingIngestible => IsIngestible && CachedNutrition > 0f`；`baseIngestTicks` 默认 **500** |

真值表 10 行 [F]（建议做成单测；job 级 Eat/营养级/toil 级）：①端食物走去餐桌＝Eat/否/否（"评论里被夸的那段"）②`MealSimple`（未覆写⇒500t）＝是/是③抽大麻烟卷（`baseIngestTicks=720`、`preferability=NeverForNutrition`、无 `<Nutrition>`；`DrugAIUtility.IngestAndTakeDrug`/`JobGiver_Binge`/`JobGiver_TakeCombatEnhancingDrug` 都走 `JobDefOf.Ingest`）＝Eat/**否＝静默 hole**/是④吸食薄片（650t）同烟卷⑤啤酒（`Nutrition 0.08`）＝是/是（带营养药物即通过）⑥营养膏机（取餐后 targetA 换成携带的餐）＝是/是⑦从背包吃＝是/是⑧动物直接吃＝是/是⑨吃尸体（corpse def `StatDefOf.Nutrition=5.2`）＝是/是⑩掠食/喂食/食人宴等其他 JobDef（如 `EatAtCannibalPlatter`）三者皆非 Eat、不在判定范围。

**裁决形态（两级=三级有效模式）**：父（`eatOnlyDuringChewing`）关（默认）×子任意（**父关时强制 false**）⇒`WholeJob`（现状手感）；父开×子关⇒`GainingNutrition`（方案 A）；父开×子开（`eatIncludeDrugs`，UI「使用成瘾品」）⇒`ChewingToil`（方案 B：`CurToilString=="ChewIngestible"`；toil 名未确认时回落 `WholeJob`→DEC-3）。**实现（决策出生即纯，零 Verse）**：`enum SqueakEatOccurrenceMode{WholeJob,GainingNutrition,ChewingToil}`；`ChewingToilDebugName="ChewIngestible"`（单测锁定）；`ResolveMode(eatingOnly,includeDrugs)` 纯函数；四参 `AllowsOccurrence`：`WholeJob=>true`；`GainingNutrition=>gainingNutritionNow`；其余 `_=>!chewToilNameConfirmed||chewToilActive||gainingNutritionNow`（回落＝完整 job 级，"绝不静默；已确认时按咀嚼/点燃判定，营养并集仅作附加保险"）。`IsEating()` 按 mode 惰性采样、`WholeJob` 不采样；`SampleChewingToil()` 命中即置进程级静态 `chewToilNameConfirmed=true`（不 Scribe），仅当 `Pawn.jobs?.curDriver is JobDriver_Ingest` 且 `CurToilString` 与常量 `StringComparison.Ordinal` 相等才 true；public 面、既有字符串、Ordinal 比较无分配。
**备选（含吸收项）**：ALT-1 方案 A（营养判定；接口稳定单行/零营养静默须披露"若产品不接受则不能用"）→不单独采用、吸收为中间档；吸收硬细节：X 边界进 UI 文案、营养并集成 `ChewingToil` 附加保险（`||gainingNutritionNow`）、`ThingDef.IsDrug` 排除被记录未采纳（→OQ-23）。ALT-2 方案 B（toil 字符串；语义即"在嚼"、覆盖抽烟薄片、公开 API/依赖 debugName）→采用为子档（父开子开）；fail-safe 建议落地为 DEC-3 回落+常量单测锁定；其"加日志"被**否决**（DEC-5 不新增日志事件）、替代=Dev 诊断面板（未实施→OQ-22）。ALT-3 方案 C（反射 `JobDriver_Ingest.chewing` 与 `CurToil` 比较；精确但脆弱跨版本风险）→不推荐未采纳；吸收项无（"精确"由 B+进程级确认替代）。ALT-4 方案 D（Harmony postfix `Toils_Ingest.ChewIngestible` 打标；最稳最重）→仅当 B 字符串不可接受时启用，保留后备未启用；"与 debugName 解耦"目标由 confirmed+fail-open 承接。ALT-5 3 态选择卡（整段 job/真正进食（营养）/真正吞咽（含成瘾品）；语义清晰但控件重、需改更多 UI 合同）→SR 不用、取嵌套复选框（最小改动+依赖禁用行）；三档语义=三枚举值；对接收方仍开放→OQ-15。推荐要旨：要"摄入营养"→A+文案写明药物边界；要"在吃/在抽"→B+fail-safe——实际以父子结构同时保留。修订：同日由单开关升级为两级（CNF-2）；"无 toil 权威"被推翻（C-3/CNF-3）；无废止。

**DEC-3 fail-open 回落：toil 名未确认⇒完整 job 级，绝不静默**——已定案已实施（§4.4 不要重开）。`chewToilNameConfirmed`=进程级只读事实（该 toil 名是否存在），采样命中即置 true；未确认期间子开关**不生效**、行为=出厂默认整段 job（不丢声音）；确认后转严格判定。后果明说：「游戏若改了 toil 名，子开关退化为无操作」而非"成瘾品静默"；触发场景=Ludeon 改名/非 vanilla 驱动/本进程未采样到。竞争口径"匹配不到回退 A"部分吸收（营养并集保险）；反向"未确认即静默"评估="会回到静默 hole，不建议"（对接收方仍开放→OQ-18）。后果：单测锁定回落与常量；Dev 面板未实施→OQ-22。

**DEC-4 「真正进食」按钮需求规格**（规范性合同，对接收方现行有效；08-23 面向姊妹项目/内核——最可能 UniversalSqueaker，命名空间/前缀/日志按对方约定替换，判定语义与 vanilla 事实可复用 [F]）。逐条硬要求：1 字段名建议 `eatOnlyDuringChewing`（现用）或按接收方语义（如 `eatRequiresIngestion`）；2 默认值**必须保持 job 级（false）**——默认收窄=删招牌行为；3 UI=现有「发声规则/行为开关」组内复选框+一行灰色短说明（不新开页面）；4 持久化 Scribe add-only 标量、默认值省略（不写节点）**不 bump schema**、老存档/缺节点⇒false；5 即时生效（发布运行时旗标即可，无需重建 resolver/池）；6 文案三要素：默认关=整段进食行程（含端食物赶路）都算／开启=只在「X」时算／X 边界写明（营养判定⇒"零营养摄入物（如大麻烟卷、薄片）不算"；toil 判定⇒"仅吞咽/点燃阶段"）；7 不得改变：动作资格/作用域、resolver、音频回退、冷却、日志协议、动作 ABI；8 若泛化 per-action：粒度偏好放策略层（纯函数+注入采样），不塞进音频选择内核；9 子选项（推荐一并设计）：父开后才可用的第二复选框，勾选=改用方案 B（吞咽/点燃，覆盖成瘾品），默认 false、独立持久化，**父关时子项必须禁用（置灰）且不产生任何效果**。UI 细则（两级实施版）：子行紧挨父行短说明下方、缩进一致（同 34f 行高）；父关时子行**禁用但可见**（置灰+help 禁用原因）不是隐藏——避免"开关消失"困惑（若接收方取隐藏须保证仍可达、不产生负 Rect）；**父关强制子 false 三层保证**＝UI 关父清零+落盘／`PostLoadInit` 对"父关子真"手改配置归一／纯规则把父关解析为 `WholeJob`；高度按常量累加（父 34f+短说明+子 34f）不随状态动态变高（否则 Measure/Draw 易不一致 [C-5]）；文案＝父 Tooltip"零营养成瘾品默认不算，勾选下方可包含"、子 Label 原版措辞「使用成瘾品」、子 Tooltip"按咀嚼/点燃阶段判定，无法识别时回落完整进食流程"、禁用原因键"需先开启上方开关"。持久化：两字段 add-only/false 默认/不写节点；父关+子真**不是合法持久态**。范围与命名（已核原版数据）：方案 B 实际范围=「任意零营养可摄入物」——vanilla 里主要是成瘾品（烟卷 720t、薄片 650t 等）、模组内容可能是任何无营养消耗品；**啤酒（`Beer`）父开关单独开启时已算 `Eat`**（0.08>0⇒走路阶段不响、咀嚼阶段响；仙馔 `Ambrosia` 0.2、`baseIngestTicks 80` 同理）；勾子项不改变它们、只补零营养成瘾品。动因=静默 hole 排查成本（"只有维护者恰好盯着一个正在抽烟的 pawn 才会察觉"）；影响边界：受影响=零营养 ingestible 整个 ingest（社交药物为主、任何 `CachedNutrition==0`），不受影响=营养>0 全部（食物/尸体/营养膏/带营养药酒）。泛化重开=OQ-19/20。

**DEC-5 改动边界与文档同步面（红线）**——已定案已实施。明确不改 [F]：日志协议（**不新增日志事件**——"日志协议是 characterization 冻结面"；ALT-2"加日志"于此被否决）、动作 ABI、resolver/池、冷却、XML ABI。可选增强（记录未实施）：Dev 诊断面板显示当前 Eat 模式与 `chewToilNameConfirmed`（→OQ-22）。文档同步面实清单（"含一个曾漏掉的"）：架构合同 §3、SKILL §7、settings-ui 产品合同例外（与"UI 冻结"口径冲突需**书面记例外**且覆盖父+子两控件）、双语 CHANGELOG ×2、Source codemap、**UI codemap**（曾漏→INC-2）、MEMORY/TODO——已实施。[xref: PKG-1 合同条文/UI 冻结口径本体]

**DEC-6 版本推进 0.3.3；0.3.2 正式版按"跳过"口径**——已定案（口径）；**正式确认悬置**（→CNF-4/OQ-1）。版本=0.3.3；0.3.2 跳过（其工作并入 0.3.3）；`stage-package.ps1` 硬断言 `csproj <Version>`≡`About.xml <modVersion>`、release tag 基版本=csproj；Steam **仍阻断**（0.3.2 从未上 Steam）；GitHub 上一产物=prerelease `v0.3.2-pre1`；发布唯一入口=runbook（阶段 0–4、双轨、`merge -s ours` 条件、Claim Pack 模板在文末，本体 PKG-4）；发布执行时新增 `docs/release_review/release-0.3.3-review-zh.md`。产出 E：双语 CHANGELOG 顶端 `未发布 — 0.3.3`/`Unreleased — 0.3.3`（注明 0.3.2 仅 GitHub prerelease，发布时换实际 UTC+8 时间）。09-13"跳过口径不变"（重确认非裁决）[C]。备选仅一处：0.3.3 走 GitHub full 还是 prerelease 先行→未裁决（OQ-3）。

**DEC-7 US 兼容修复归属=US 侧；SR 本窗口零运行时改动；顺序硬门**——已定案（§4.6 不要重开）；实施在 US 侧、SR 语料外。U1=`VoicePackCompAttach` 逃逸门改**跨程序集**检测（"该种族已有任意 squeak comp 即跳过"）+skip 原因日志（如 `foreign_squeak_comp`）；U2=按 Q1 裁决落实"US 是否服务 Ratkin"，若服务则 U1 是其发布**硬前置**；U3=legacy 桥（薄空 `SqueakyRatkin.SqueakVoicePackDef`）仅在**SR 程序集不再加载**的窗口启用，重叠期类型名归属先约定；U4=双开矩阵（兼容性检查 §5）纳入 US 发布门。顺序硬门=`U1 → US 型 Ratkin 包/桥 → SR 1.0 内容化`。验收信号：SR+US（±US 型 Ratkin 包 ±桥）四种组合 Ratkin **每次事件恰好一条声音**；US 日志出现 Ratkin 跳过记录。F1–F8/Q1–Q4/双开矩阵论证本体 [xref: PKG-3]。

**DEC-8 身份归属推荐：packageId 跟内容走（推荐未裁决，§4.7）**——08-23 方案会话推荐、决策会话待裁 [C→待 F]：pack-only 继承人就地继承 `coahuilite.squeakyratkin`；legacy 拿新 id 冻结。（packageId 字符串标识，非 Steam `PublishedFileId` 数值，不属隐私禁写项；逐字保留注见 §0。）反向选项（id 跟 legacy 实体走）本主源未展开——Q7 完整论证 [xref: PKG-3]；理由细节在迁移方案 §8/§8.5 [U]。

**DEC-9 三会话分派与各自边界（08-23 [F]"可直接转述"；发布线已被 09-13 部分推进）**：6.1 发布会话（SR 0.3.3 更新与发布）=读序 `AGENTS.md`→`MEMORY.md`→`TODO.md`→本文→`docs/release-runbook-zh.md`；做=`verify-local`（全绿）→授权后 commit（建议功能块与文档块分开）→按 runbook 阶段推进→写 `release-0.3.3-review-zh.md`→渠道核验；边界=**不要改语义/默认值**、Steam 涉及人工编辑、外部操作需授权、隐私审查覆盖完整可达范围。6.2 US 会话=DEC-7（兼容改动，SR 侧不改行为）。6.3 决策会话（迁移/身份）=只做裁决与排期 Q1–Q10；产出写回迁移方案（"待裁决"→"已定案"）与 `TODO.md`。§8 授权与隐私边界 [F 记载的自述声明]：本轮未做任何 commit/push/tag/release/上传，工作树即全部交付物；三份新文档与 MEMORY/TODO 均无本机绝对路径、凭据、`PublishedFileId`、日志摘录（自述 [C]，v1 复核未见反证）；US 仓库只读参考（维护者授权路径）未做任何写入；发布/推送/上架前按 `AGENTS.md` 做完整可达范围隐私审查。

**DEC-10 09-13 会话：流程简化落地+本地发布推进（停在远端推送前）**——本地已实施、远端待授权 [F/C 混合逐条]。流程冗余评估（对齐 UniversalSqueaker 三命令+最小仪式）产出 `docs/release_review/process-redundancy-review-zh.md`（R1–R12 逐条裁决、V1–V3 待裁决、N1–N3 明确不做）；核心结论 [C]：「门禁覆盖不缺，冗余在『谁来做』；同一包内容清单曾在 runbook 出现 3 次、隐私审查 4 处人工表述 0 脚本」（本体 [xref: PKG-5]）。落地 [F 记载]：新增 `scripts/privacy-audit.ps1`（三向量+身份、`-FullHistory`/`-PrePush`、`$knownHistoryDebt` 台账）与 `scripts/check-pack-readiness.ps1`（版本轴/红线/暂存包/DLL 身份/`[claim]` 快照）；runbook 重写为「入口契约+最小发布仪式+阶段 0–4」；`AGENTS.md` 增「External-state boundaries」（本地 commit 允许、远端需授权、禁止重新加回人工仪式）；`ci.yml`/`release.yml` 接线（11 项门+隐私门；release 再加发布面门；SDK 对齐 10.0.x）；codemap 与 README 双语版本锚点同步。推进 [C 可复跑]：0.3.2 跳过口径不变；0.3.3 已提交（**三块**——比 §9.1 两块建议多出"流程块"，建议被吸收且扩展执行 [I-3]）；dev 包已产出。与 §2（08-23）构成同文档时间层矛盾→CNF-1；ASM-6。

**DEC-11 推送会话入口与三步仪式（现行有效待执行；09-13 定稿 [F]）**：① 读序 `AGENTS.md`→`MEMORY.md`→`TODO.md`→本文件→runbook；② 推送前三步：`check-pack-readiness -RequireReleaseMetadata`→`privacy-audit -FullHistory -PrePush`→发布裁决（V1–V3 与 Steam 是否恢复）；③ 授权后链路：push dev→PR dev→main→CI 绿→squash merge→tag `v0.3.3`→release CI→资产核验→CHANGELOG 换时间→Claim Pack `release-0.3.3-review-zh.md`→渠道核验。边界（§10.4）：未做 push/PR/merge/tag/Release/Steam 上传或编辑；CI 改动要推送后才实际运行（本机无法实跑 runner、YAML 仅人工校对）；历史/tag 重写未执行——门以 `[known-debt]` 列 5 个文件、"范围比先前记录更大，需按 5 文件重估"（→CNF-5，细节 [xref: PKG-5]）；Steam 文案/包核验随解除阻断执行。

**DEC-12 开工前三件事（H§9）**：① 是否 commit 及拆分（建议功能块+文档/记忆块两块）——**已被 09-13 执行**（三块→DEC-10）；② 0.3.2 是否确认跳过（当前口径：跳过）——**仍开放**（OQ-1）；③ Steam 是否恢复发布、若不恢复 0.3.3 是否只走 GitHub——**仍开放**（OQ-2/3）。

**0.3.3 交接产出 A–E（H§1 五块）**：去向 DEC-1/2/5/6/7/8+TL-6+ANCH——A=Eat 触发粒度功能块、B/C/D=三份新专项文档（随交接正文一并交付、不含正文——CNF-6/I-4）、E=版本同步（DEC-6）。**已定案 7 条（H§4 L46–52）**＝①Eat 默认不收窄（DEC-1）②两级开关（DEC-2）③父关强制子 false/禁用可见（DEC-4）④fail-open 回落（DEC-3）⑤版本 0.3.3+0.3.2 跳过口径（DEC-6）⑥US 兼容归 US 侧+顺序硬门（DEC-7）⑦身份归属推荐未裁决（DEC-8）。

## 4. 假设 ASM-1..8
- ASM-1 玩家喜欢 job 级连珠炮手感⇒默认即招牌不得收窄｜成立 [F]（定案无重开）｜验收"父关与旧实现完全一致"。
- ASM-2 Ludeon 不改 `ChewIngestible` debugName｜软性成立（真值层不可证，被 fail-open 中和）｜改名⇒子开关退化为无操作（非静默）；静默退化仅 Dev 面板可见（未实施→OQ-22）。
- ASM-3 `GainingNutritionNow` 语义=营养≠在嚼、零营养摄入物永不触发｜成立 [F]（1.6 源码核对）｜上游改接口语义则失效。
- ASM-4 public 面足够且零成本（`curDriver` public 字段、`CurToilString` 返回既有字符串、Ordinal 无分配）｜成立 [F]。
- ASM-5 两新字段默认 false+add-only⇒不写节点不 bump⇒fixture 零 delta｜成立 [F 已验证]｜"父关子真"若被写入将破坏 add-only⇒三层强制归一堵死。
- ASM-6 新接线 CI（11 项门+隐私门+发布面门、SDK 10.0.x）推送后按预期运行｜未验证 [U]（本机无法实跑 runner、YAML 仅人工校对）｜首次远端 CI。
- ASM-7 US 仓库只读参考（维护者授权路径）、SR 窗口内零写入｜成立（自述 [C]，无反证）｜§8 边界声明。
- ASM-8 负向自测在隔离一次性仓库执行⇒真实仓库无写脏风险｜成立（自述 [C]；三向量+身份全触发 EXIT 1）｜"写脏真实仓库"事故叙事本体在 PKG-5，此处只存指针。

## 5. 认识论分账 C-1..6 / I-1..4 / U-1..7
- C-1 玩家（Steam 评论者，身份/日期 [U]）"连珠炮""太期待吃饭了"正面定性——部分被 `v0.3.0` 指纹 [F] 佐证。C-2 08-23 交接会话自述：无 commit/push/tag/release/上传；新文档与 MEMORY/TODO 无本机路径/凭据/`PublishedFileId`/日志摘录（隐私自述未独立复核）。C-3 fresh-context 对抗复核方（早期时点）判"vanilla 无公开 toil 权威"——被同文档后续核实**推翻**（"复核结论=快照"直接实例）。C-4 09-13 会话自述可复跑四项实测：`verify-local -NoRestore` 11/11 EXIT 0；`check-pack-readiness -RequireReleaseMetadata` all checks passed；`privacy-audit -FullHistory` 227 revision 未接受命中 0、5 条 `[known-debt]`；负向自测三向量+身份全触发 EXIT 1（v1 未复跑）。C-5 实现会话「本项目 UI 复核专门查过（Measure/Draw 一致性）」——记录本体不在主源。C-6 09-13 冗余核心结论 [C]（论证细节 PKG-5）。
- I-1 §4.5"跳过"定案与 §5/§9.2 开放并存不矛盾 [I]：前者=工作口径（default-of-record）、后者=发布执行会话的正式确认门；09-13"口径不变"佐证两态并存；替代=撰写未对齐（语料无裁定）。I-2 E§7 前言 L223 为成文较早层（两段时间标签同为 08-23，无法用日期裁决先后）；替代=§7 是为姊妹项目写的可移植参考节未随实施更新。I-3 §9.1 两块 commit 建议被吸收并扩展为三块（新增流程块）：建议+三块清单+三类改动恰对应。I-4 §8"三份新文档"vs §2 四个 untracked：计数排除交接正文自身 [I]；替代=笔误。
- U-1 Eat 三态矩阵与 US 双开矩阵是否有任何一次实机执行——缺实机记录（TODO.md 语料外）；影响 DEC-2/7 最终验收。U-2 0.3.2"确认跳过"终局裁决与理由——缺发布会话记录。U-3 V1–V3 内容本体——主源仅存编号；影响 DEC-11 第三步。U-4 隐私债务"先前记录"具体范围——主源未载先前值；影响 CNF-5 定性。U-5 Steam 评论原文/链接/时间——仅转述。U-6 v0.3.0 时代是否另有负面反馈——只收正面单侧；若存在会动摇 DEC-1"招牌"定性。U-7 三块提交 hash/日期——§10 仅声称；`git log` 可恢复但 v1 禁外部取证。

## 6. 矛盾 CNF-1..6
- **CNF-1** H 头 L4"工作树未提交、未推送"+§2"本轮 0 提交"+§8"未做 commit"（§1–§9=2026-08-23 未提交层）↔ §10.1"0.3.3 工作树已提交（功能/文档记忆/流程块分离）"（§10=2026-09-13 已提交、停在推送前层；同文档分层写作，README §4 示例原型）。提交状态相反；§10 后补只更新不抹除、前段忠实记录写作时点 [I]；现行建议口径=以 09-13 层为准：本地已提交、远端未推；回查 `git log`（下游可用；v1 禁取证）。
- **CNF-2** E§7 前言 L223"已实施单开关（A）；两级尚待实施"↔§0.4/§7.1/注 2"两级已实施（08-23）"（同日，日内先后不可证 [I-2]）。L223=较早草稿层残留；引用实施状态以 §7.1+注 2+§4 定案 2–4 条为准。
- **CNF-3** 注 1 复核判"无公开 toil 权威"↔§2.3 `CurToilIndex`/`CurToilString` public（`CurToil` protected）（同日：复核早期↔后续核实）。B 可行性结论反转；"结论被新证据取代"非笔误；该更正=B 立项前提；引用标注反转链 C-3→F。
- **CNF-4** §4.5"版本=0.3.3；0.3.2 按跳过口径"（已定案不要重开）↔§5/§9.2 同项列开放（均 08-23；09-13 §10.1"跳过口径不变"重确认）。定案=工作口径、开放=终局确认门，两态并存 [I-1]；勿把"口径"当"裁决"引用；正式裁决记录在语料外。
- **CNF-5** §10.4"[known-debt] 列 5 个文件（范围比先前记录更大，需按 5 文件重估）"↔"先前记录"数值未载（U-4）（09-13）。债务范围版本间漂移、包内无法定量；前次评估遗漏文件 [I]；回查 [xref: PKG-5 隐私债务 5 文件/漂移记录]，两包数字互为校验。
- **CNF-6** §8"三份新文档均无……"↔§2 untracked 4 个新文档（本文+eat+compatibility+migration）（均 08-23）。"三份"排除交接正文 [I-4]；非实质矛盾，防下游误报；对照产出 B/C/D 恰三份+本文。
（无跨源实质冲突：E 与 H 在 Eat 已定案 1–4 条逐条一致 [F]。）

## 7. 事故与证据 INC-1..4 / EV-1..7
- **INC-1 静默 hole（设计陷阱，已被规格吸收的"预演事故"）**：开关一开，抽烟/吸薄片的 pawn 整个 ingest 期间零 `Eat` 事件——无异常、无 warn/error、无 rejected/cooldown 日志（走路时可能出 `Move`、站定时落 `Call`）：`Eat` 在采样层被换掉了。根因=营养权威语义=营养>0（ASM-3）零营养 ingestible 永不满足；动作统计只记录进入管线的动作、计数归零看不出被谁替代。处置=文案三要素必含边界（DEC-4 条 6）+方案 B 子档+fail-open 回落（DEC-3）；复现步骤文档化（父开→按药物政策抽烟→观察）。证据=论证（源码/Def 核对+复现成文；未见实机执行 [U-1]）。`E§0L13,§3`
- **INC-2 过程遗漏（已补）**：文档同步面清单"**UI codemap**"曾被漏（根因未载）→补入清单并实施（E§6L219,§7.1）。
- **INC-3 复核误判→更正**：判"无公开 toil 权威"若成立则只能 A（静默 hole 无解）；未核到 `CurToilString` public→更正后 B 立项落地（CNF-3）。
- **INC-4 未决风险（非事故）**：隐私历史重写未执行；`-FullHistory` 门列 5 个 `[known-debt]` 文件、范围比先前记录更大（CNF-5）→挂账 `$knownHistoryDebt` 台账，重写需另行授权。实测 [C-4]。
- **EV-1** 默认双关⇒新 fixture（两字段默认 false）零 delta、不 bump schema——回归门（实测）。**EV-2** 构建 0 warning/0 error（Dev+Steam 双 flavor）；工作树 19 文件 +161/−17+4 untracked；verify-local 11/11（最后运行在全部代码/版本写入后、只改文档前）（实测自述可复跑）。**EV-3** 09-13 四项可复跑证据（见 C-4）支撑 DEC-10/11（实测 [C-4]）。**EV-4** 0.3.3 三块提交+dev 包产出；远端未动（TL-9）——[C] 声称+工具可复跑（v1 未复跑）。**EV-5** E§7.1 全表 8 行"已实施"：纯规则文件（纯度门不变）、`CompSqueaker` 静态旗标+进程级 confirmed、设置字段两处发布+Toggle(disabledReason)+父关清零+高度+34f、Scribe 一行+PostLoadInit 归一、fixture 镜像零 delta、单测 `EatOccurrenceRules` 扩为模式+回落+常量+默认值断言、本地化 542 键零重复中英对齐、文档同步面（记载+可复跑）。**EV-6** 出厂手感指纹：`v0.3.0` tag 与工作树机制一致（`IsEating`=job 级、`EachTime 144t`、216t 节拍、3.6 秒一发）⇒"现网 Steam 版本就是这个手感"（论证）。**EV-7** 范围界定成功：带营养成瘾品（啤酒 0.08/仙馔 0.2）父开子关时仍响——本仓库**明确接受**为默认行为（营养权威口径）；勾子项不改变；记为 OQ-17/OQ-23（论证）。注：v1 覆盖表及 DEC-2 曾引"EV-8"，v1 账本 EV 仅 1–7——悬空引用，如实不补。

## 8. 开放问题 OQ-1..23（全列不折叠）
- OQ-1 0.3.2 正式版是否**确认**跳过（口径=跳过、工作并入 0.3.3）｜待裁决→开放（09-13 仍"口径不变"）
- OQ-2 Steam 是否恢复发布（0.3.2 从未上 Steam，阻断中）｜待裁决+需授权（人工编辑面）→开放
- OQ-3 0.3.3 走 GitHub full 还是 prerelease 先行（上一产物 `v0.3.2-pre1`）｜待裁决→开放
- OQ-4 发布裁决 V1–V3（内容 [xref: PKG-5]）与推送前仪式绑定｜待裁决→开放；主源仅存编号 [U-3]
- OQ-5 完整推送链：push dev→PR dev→main→CI 绿→squash merge→tag `v0.3.3`→release CI→资产核验→CHANGELOG 换 UTC+8 时间→Claim Pack `release-0.3.3-review-zh.md`→渠道核验｜需授权→停在远端推送前（冻结时点）
- OQ-6 迁移/身份 Q1–Q10 逐项：过渡版、legacy 条目、设置导入、公告窗口、前置硬/软、跨版本维护、id 归属、音源重置、legacy 上架、新线 def 类型｜待裁决→开放；分派决策会话；条文 [xref: PKG-3]
- OQ-7 身份归属推荐（packageId 跟内容走：继承人就地继承 `coahuilite.squeakyratkin`、legacy 拿新 id 冻结）裁决｜待裁决→推荐未裁决
- OQ-8 US 兼容 Q1–Q4：US 是否服务 Ratkin；检测方式；桥重叠期类型名归属；Domain 级跳过｜待裁决→开放；条文 [xref: PKG-3]
- OQ-9 US 侧 U1–U4 实施进度（SR 只读窗口内零运行时改动）｜依赖外部→未知（GAP-6）
- OQ-10 Eat 三态手工验收矩阵未实机：meal/烟卷/薄片/啤酒/仙馔/营养膏/背包吃/动物/尸体 ×（父关、父开子关、父开子开）；重点=父关与旧实现完全一致、父开子开时食物行为与父开子关一致（并集不改变食物）、父开子关时啤酒/仙馔仍响、关父项后子项灰显且值归 false｜未验证→开放（"实机"类）
- OQ-11 US 双开矩阵未实机（四组合"恰好一条声音"+跳过记录）｜未验证→开放；矩阵本体 [xref: PKG-3]
- OQ-12 CI 接线（11 项门+隐私门+发布面门、SDK 10.0.x）推送前无法实跑；YAML 仅人工校对｜未验证→开放
- OQ-13 历史/tag 重写：5 个 `[known-debt]` 文件需重估（范围比先前记录更大）；重写本身需授权｜需授权+未验证→开放；方案 [xref: PKG-5]
- OQ-14 Steam 文案/包核验随解除阻断执行（本轮未做）｜依赖外部（OQ-2）→开放；文案源 [xref: PKG-4]
- OQ-15（对接收方）采用父+子两级形态还是 3 态选择卡｜待裁决（外部）→开放；SR 已取两级
- OQ-16（对接收方）子项命名范围：「使用成瘾品」原版等价于药物，实现覆盖面是"任意零营养可摄入物"；模组内容可能需更宽措辞｜待裁决（外部）→开放；SR 已用原版措辞
- OQ-17（对接收方）是否接受"带营养成瘾品（啤酒/仙馔）父开子关时仍响"；若要静默需加 `ThingDef.IsDrug` 排除（另一语义决定）｜待裁决（外部）→SR 默认接受 [F 记载]、接收方开放
- OQ-18（对接收方）回落口径：本仓库取"未确认⇒完整 job 级（不丢声音）"；接收方若更在意"严格"可取"未确认即静默"——源文档评估"会回到静默 hole，不建议"｜待裁决（外部）→SR 已定案、外部开放
- OQ-19 是否泛化到其他动作：`Sleep`（在床/未上床）、`Work`、`Social`、`Joy` 目前同样 job 级/粗略判定，同类粒度问题会重复出现｜待裁决→开放（0.4/US）
- OQ-20 粒度偏好放哪层：策略层（纯函数+注入采样）还是动作定义/注册表（与动作门 `SqueakActionDef`/`allowExternalActions` 的关系）｜待裁决→开放（DEC-4 条 8 已给 SR 倾向：策略层）
- OQ-21 默认值是否写进兼容政策（"一旦发布，父关=默认 job 级就不允许再翻"）｜待裁决→开放；主源未见政策落档
- OQ-22 是否需要 Dev 诊断（面板显示当前模式与 `chewToilNameConfirmed`）以发现 B 的静默退化；约束=不新增日志事件｜待裁决（可选增强，未实施）→开放
- OQ-23 若产品要求父开子关时啤酒类带营养药饮也静默⇒追加 `ThingDef.IsDrug` 排除判定（明载"这是另一个语义决定，不是本规格默认行为"）｜悬置（未采纳备选）→开放（挂 OQ-17）

## 9. 锚点 ANCH（9 族；标识符逐字，长说明见 v1）
- **设置/字段/UI**：`eatOnlyDuringChewing`（父，UI「仅在真正进食（正在摄入营养）时触发」）/`eatIncludeDrugs`（子，UI「使用成瘾品」原版措辞）；均默认 `false`；`SqueakyRatkinSettings.cs`（字段+`ApplyToRuntime()`/`NotifyCheapRuntimeChanged()` 两处发布；发布表达式 `eatOnlyDuringChewing && eatIncludeDrugs`）；`SqueakyRatkinSettings.ExposeData.cs`（Scribe add-only 各一行；`PostLoadInit` 父关⇒子 false 归一）；行高 `34f`、高度+34f、`MeasureBasicsContentHeight` 同步、`Toggle(enabled:/disabledReason:)`、禁用原因键"需先开启上方开关"、父 Tooltip 边界句、子 Tooltip"按咀嚼/点燃阶段判定，无法识别时回落完整进食流程"。
- **规则/实现符号**：`Source/SqueakyRatkin/SqueakEatOccurrence.cs`（纯文件、零 Verse、纯度门不变）；`enum SqueakEatOccurrenceMode { WholeJob, GainingNutrition, ChewingToil }`；`ChewingToilDebugName = "ChewIngestible"`；`ChewingOnlyDefault = false`；`ResolveMode(bool eatingOnly, bool includeDrugs)`；四参 `AllowsOccurrence(mode, gainingNutritionNow, chewToilActive, chewToilNameConfirmed)`；`CompSqueaker.cs`——`IsEating()`/`IsGainingNutritionNow()`（`Pawn.jobs?.curDriver is IEatingDriver eating && eating.GainingNutritionNow`）/`SampleChewingToil()`/进程级静态 `chewToilNameConfirmed`（不 Scribe）；`StringComparison.Ordinal`；"决策出生即纯"。
- **vanilla 符号**：`JobDefOf.Ingest`；`JobDriver_Ingest.MakeNewToils()`；`Toils_Ingest.PickupIngestible`/`CarryIngestibleToChewSpot`/`FindAdjacentEatSurface`/`ChewIngestible(...)`/`FinalizeIngest`；`ReserveFood`；`JumpIf(chewing, …)`；`PrepareToIngestToils_Dispenser`；`eatingFromInventory`；`IEatingDriver.GainingNutritionNow`（接口唯一成员）；`ToilMaker.MakeToil("ChewIngestible")`；`Toil.debugName`；`Toil.ToString()` = `debugName ?? "unnamed"`；`JobDriver.CurToil`(protected)/`CurToilIndex`/`CurToilString`(public)；`Pawn.jobs.curDriver`(public)；`toil.actor.pather.StopDead()`；`ticksLeftThisToil = Mathf.RoundToInt(thing.def.ingestible.baseIngestTicks * durationMultiplier)`；`ThingDef.IsNutritionGivingIngestible => IsIngestible && ingestible.CachedNutrition > 0f`；`ThingDef.IsDrug`；`preferability=NeverForNutrition`；`DrugAIUtility.IngestAndTakeDrug`；`JobGiver_Binge`；`JobGiver_TakeCombatEnhancingDrug`；`EatAtCannibalPlatter`；`StatDefOf.Nutrition = 5.2`（尸体）；`CompSqueaker.CurrentAction`；`1.6/Patches/Ratkin_AddSqueakComp.xml`（SR 装配点）。
- **数值**：`baseIngestTicks` 默认 `500`（≈8.3 秒）；烟卷 `720` t（≈12 秒）；薄片 `650` t（≈10.8 秒）；`Beer Nutrition 0.08`；`Ambrosia Nutrition 0.2`、`baseIngestTicks 80`；corpse `Nutrition = 5.2`；`Eat = EachTime, 144 ticks`；`globalMinIntervalTicks = 216` ≈ **3.6 秒**现实时间一发；横穿地图 20–30 秒≈5–8 发；站桩≈1–2 发；`scaleCooldownWithTimeSpeed` 默认开、1×/2×/3× 节拍一致；概率门只在 `RandomOneShot` 生效、`EachTime` 不抽概率。
- **验收/工具**：`tools/KernelCharacterization/{KernelCharacterization.csproj,UnitTests.cs}` 的 `EatOccurrenceRules`（四组合含"父关+子真=`WholeJob`"、三模式、回落、常量、两默认值断言）；`tools/SettingsFixtureGenerator/Contract/SettingsContract.cs`（镜像子字段）；`fixtures/expected/01-new-install-first-save.xml` 零 delta；`scripts/verify-local.ps1`（`-NoRestore` 11/11 全绿；`-PackDev`）；构建 0 warning/0 error（Dev+Steam 双 flavor）；本地化 542 键、零重复、中英对齐。
- **版本/发布面**：`0.3.3`；`0.3.2`（跳过口径）；`v0.3.2-pre1`；计划 tag `v0.3.3`；`v0.3.0`（手感指纹基线）；`csproj <Version>` ≡ `About.xml <modVersion>`（`stage-package.ps1` 硬断言；release tag 基版本=csproj）；`未发布 — 0.3.3` / `Unreleased — 0.3.3`（UTC+8 换时间）；分支 `dev`/`origin/dev`/main；squash merge；`docs/release_review/release-0.3.3-review-zh.md`（Claim Pack）；`merge -s ours`（runbook 内条件，本体 [xref: PKG-4]）。
- **流程/隐私工具（09-13）**：`scripts/privacy-audit.ps1`（三向量+身份、`-FullHistory`、`-PrePush`、`$knownHistoryDebt`）；`scripts/check-pack-readiness.ps1`（版本轴/红线/暂存包/DLL 身份/`[claim]` 快照、`-RequireReleaseMetadata`）；`ci.yml`/`release.yml`；11 项门+隐私门（release 再加发布面门）；SDK 对齐 `10.0.x`；`227 revision`；`5` 条 `[known-debt]`；`19` 个已跟踪文件改动（`+161`/`−17`）+`4` 个 untracked 新文档；`AGENTS.md`「External-state boundaries」。
- **编号表与兼容标识（跨包指针）**：U1–U4；`VoicePackCompAttach`；skip 原因日志 `foreign_squeak_comp`；legacy 桥薄空类型 `SqueakyRatkin.SqueakVoicePackDef`；顺序硬门 `U1 → US 型 Ratkin 包/桥 → SR 1.0 内容化`；packageId `coahuilite.squeakyratkin`（逐字保留，注见 §0）；F1–F8、Q1–Q4、B1–B5、P0–P4、Q1–Q10 [xref: PKG-3]；R1–R12、N1–N3、V1–V3 [xref: PKG-5]；"每次事件恰好一条声音"（验收信号原话）。
- **授权边界**：外部有效操作（commit 属边界内地带：09-13 口径"本地 commit 允许"、远端操作需授权）需维护者显式授权（`AGENTS.md`）；隐私审查覆盖完整可达范围（发布/推送/上架前）；US 仓库只读（维护者授权路径）；不改语义/默认值（发布会话边界）；Steam 涉及人工编辑。

## 10. 教训 LES-1..7
- LES-1 "静默的归属变更"最难排查（不异常、不 warn、不进 rejected 日志）——开关类功能必须把**行为变化映射到用户可感/可查信号**（文案边界要素+真值表+可观测性钩子），否则只有盯现场才能发现（INC-1 全案）｜强｜任何在采样/过滤层改变动作归属的开关，不特指 Eat。
- LES-2 已发布的手感/默认值是兼容资产：新开关默认双关=与旧实现逐位一致；"默认收窄等于删掉招牌行为"（DEC-1+EV-1）｜强｜玩家可感默认行为；延伸明载"0.4/US 同样适用"。
- LES-3 依赖外部标识符（debugName 类字符串）的严格判定，回落方向应选"退回宽松旧行为（不丢声音）"而非"退回严格判定（静默）"；回落副作用="子开关退化为无操作"优于"成瘾品静默"（DEC-3+§8.4 反向被评"不建议"）｜中等｜fail-safe/fail-open 择向；对接收方仍开放 OQ-18。
- LES-4 对抗复核的"无解"结论是时间点快照："无公开 toil 权威"被后续 public 核实推翻且直接解锁最终形态 B 档（INC-3+§2.3）｜中等（单例但更正链可核）｜API 面随版本演化的判据复核，非普适"复核都要复核"。
- LES-5 文档同步面要落成清单并携带"曾漏掉的"标注（UI codemap 曾漏被显式记录），防清单回退（INC-2）｜tentative（单例）｜本仓文档同步面场景。
- LES-6 同一交接文件追加接续记录会产生内部时间层矛盾（"未提交"vs"已提交"）：接续内容自带日期可定位，但忽略 §10 的前段读者会拿到过期状态——引用 handoff 必须带段落时层（CNF-1）｜中等｜handoff/日志类文档；"分段带时层"对本仓是惯例（§10 标题自带日期）非缺陷。
- LES-7 门禁"谁来做"是冗余主源：覆盖不缺时，重复人工表述（同一清单 runbook 3 次、隐私 4 处 0 脚本）应转成脚本+三步仪式+CI 接线；落地后以 AGENTS.md 禁止"重新加回人工仪式"（防回潮）（DEC-10/11+EV-3）｜中等（[C] 结论+已落地结构佐证；细节 PKG-5）｜发布流程域，不泛化到所有人工检查。

## 11. 证据缺口 GAP-1..7
GAP-1 Eat 三态/US 双开矩阵是否已实机执行及结果——实机记录在 TODO.md/后续会话（语料外）→需实机会话记录或 TODO 快照。GAP-2 0.3.2 跳过的终局裁决与理由——只有"口径不变"无裁决动作→需发布会话记录/CHANGELOG 终稿 [xref: PKG-4]。GAP-3 隐私债务"比先前记录更大"的先前口径→PKG-5 主源（rewrite-plan）对照。GAP-4 Q1–Q10/Q1–Q4/V1–V3/R1–R12 逐项内容——主源仅存编号与主题词→PKG-3/PKG-5 展开。GAP-5 Steam 评论原文与时间、是否另有负面反馈（U-5/U-6）→评论渠道访问（语料外）。GAP-6 US 侧 U1–U4 分派后是否动过→US 仓库记录（授权面外）。GAP-7 09-13 三块提交的 commit 标识与顺序→下游 `git log`（供终审）。

## 12. 未吸收与自检
未吸收（承接 v1 §12）：H L3 阅读顺序指针（纯导航）；H§2 逐文件明细表（类别与计数 19/+161/−17/4 untracked 等已入 ANCH/EV-2，逐路径枚举与 E 附重复）；H§7 指向专项文档/记忆面的导航行（只存 xref）；E 附 L275–277 改动清单（与 §7.1/H§2 重复，时层由 CNF-1/TL-6 承载）；E L3–4 接收方约定句（语义在 DEC-4 头部与 OQ-15..22 保留）。隐私：本文件仅仓库相对路径与标识符，无本机路径/凭据/`PublishedFileId`/日志摘录。
自检：DEC12/12｜ALT 0–5 全 6（各含吸收项）｜ASM8｜CNF6｜OQ 23/23 全列未折叠｜GAP7｜LES7｜INC4｜EV7（v1 悬空"EV-8"已注记不补）｜TL10｜C6 I4 U7｜ANCH 9 族标识符逐字。两时间层未抹平：§1–§9=2026-08-23 未提交层 ↔ §10=2026-09-13 已提交停在推送前层（CNF-1；TL-6/TL-9）。必保项核对：Eat 基线（toil 链/两条权威/真值表要点/500/0.08/0.2/720t）✓ 静默 hole✓ A/B/C/D 与吸收✓ 两级开关三级模式✓ fail-open✓ 不新增日志事件✓ 产出 A–E✓ 已定案 7 条✓ 三会话分派✓。尺寸与压缩比见下行。
尺寸：<BYTES>B ≈ <PCT>%（v1 77,140B；目标 ≤11,568B）。
