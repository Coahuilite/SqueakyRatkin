# PKG-6 P0 slices

----- 6. 矛盾与反转（CNF-1…CNF-6） L282-294 -----
## 6. 矛盾与反转（CNF-1…CNF-6）

| id | 冲突双方（含来源） | 各自时间层 | 冲突点 | 可能解释 [I] | 建议回查点 |
| --- | --- | --- | --- | --- | --- |
| CNF-1 | `handoff-0.3.3-zh.md` 头 L4"工作树未提交、未推送"+ §2 L19"本轮 0 提交"+ §2 L34 / §8 L91"未做 commit" ↔ §10.1 L107"0.3.3 工作树已提交（功能块/文档记忆块/流程块分离）" | 前段 2026-08-23；§10 2026-09-13（同文档分层写作，README §4 的示例原型） | 提交状态相反 | §10 是后补时间层，只更新不抹除；前段忠实记录其写作时点 [I] | `git log` 找三块提交（下游可用；本包禁取证）；现行建议口径：以 09-13 层为准 = 本地已提交、远端未推 |
| CNF-2 | eat 文档 §7 前言 L223"§7 描述的是已实施的单开关（方案 A）；父+子两级形态见 §5.1（尚待实施）" ↔ §0.4 L14"已落地的形态…本仓库已实施"、§7.1 L249–262 落地全表、注 2 L280"两级形态已在本仓库实施（2026-08-23）" | 同为 2026-08-23（日内先后不可证 [I-2]） | 两级形态"尚待实施" vs "已实施" | L223 为较早草稿层残留；§7 主体仍是单开关可移植参考、§7.1 才是两级落地 [I] | 下游若引用"实施状态"，以 §7.1 + 注 2 + §4 已定案 2–4 条为准 |
| CNF-3 | 注 1 L279：复核方当时判"vanilla 无公开 toil 权威" ↔ §2.3 L52 记载 `CurToilIndex`/`CurToilString` 为 public（`CurToil` protected） | 同日两层：复核早期 ↔ 后续核实 | B 方案可行性结论反转 | API 面核实滞后 [I]；属"结论被新证据取代"而非文档笔误 | 该更正同时是方案 B 立项前提（DEC-2 ALT-2）；引用时标注反转链 C-3→F |
| CNF-4 | §4.5 L50"版本 = 0.3.3；0.3.2 正式版按'跳过'口径（其工作并入 0.3.3）"（已定案，"不要重开"） ↔ §5 L58"0.3.2 正式版是否确认跳过"列为开放 + §9.2 L98"是否确认跳过（当前口径：跳过）" | 均 2026-08-23；§10.1 L107（09-13）"跳过口径不变"重确认 | "已定案"与"开放裁决表"同时收录同一事项 | 定案=工作口径、开放=终局确认门，两态并存 [I-1] | 发布会话的正式裁决记录（语料外）；下游勿把"口径"当"裁决"引用 |
| CNF-5 | §10.4 L126"门以 `[known-debt]` 列出 5 个文件（范围比先前记录更大，需按 5 文件重估）" ↔ "先前记录"口径数值主源未载（U-4） | 2026-09-13 | 债务范围版本间漂移，本包内无法定量 | 前次评估遗漏文件 [I] | 回查 [xref: PKG-5（隐私债务 5 文件 / 漂移记录）]；两包数字应互为校验 |
| CNF-6 | §8 L92"三份新文档均无……" ↔ §2 L31 untracked 列表含 4 个新文档（本文 + eat + compatibility + migration） | 均 2026-08-23 | 计数 3 vs 4 | "三份"排除交接文档自身 [I-4]；非实质矛盾，防下游误报 | 对照 §1 产出 B/C/D 恰为三份新专项文档 + 本文 |

（无跨源实质冲突：eat 文档与 handoff-0.3.3 在 Eat 已定案 1–4 条上逐条一致 [F]；`src: docs/handoff-0.3.3-zh.md §4 L46–49 ↔ docs/handoff-eat-occurrence-granularity-zh.md §5.1 L123–129`。）


----- 8. 开放问题、阻塞与交接风险（OQ-1…OQ-23） L311-338 -----
## 8. 开放问题、阻塞与交接风险（OQ-1…OQ-23）

| id | 问题 / 风险 | 类型 | 影响面 | 当前状态 | 来源 |
| --- | --- | --- | --- | --- | --- |
| OQ-1 | 0.3.2 正式版是否**确认**跳过（口径=跳过，工作并入 0.3.3） | 待裁决 | 版本序列/CHANGELOG/渠道 | 开放（09-13 仍"口径不变"） | `src: docs/handoff-0.3.3-zh.md §5 L58, §9 L98, §10.1 L107` |
| OQ-2 | Steam 是否恢复发布（0.3.2 从未上 Steam，阻断中） | 待裁决+需授权（人工编辑面） | 外部渠道 | 开放 | `src: … §3 L40, §9 L99, §10.4 L127` |
| OQ-3 | 0.3.3 走 GitHub full 还是 prerelease 先行（上一产物 `v0.3.2-pre1`） | 待裁决 | 发布节奏 | 开放 | `src: … §5 L58` |
| OQ-4 | 发布裁决 V1–V3（内容 [xref: PKG-5]）与推送前仪式绑定 | 待裁决 | 推送链路 | 开放；主源仅存编号 [U-3] | `src: … §10.1 L105, §10.3 L119` |
| OQ-5 | 完整推送链执行：push dev → PR dev→main → CI 绿 → squash merge → tag `v0.3.3` → release CI → 资产核验 → CHANGELOG 换 UTC+8 时间 → Claim Pack `release-0.3.3-review-zh.md` → 渠道核验 | 需授权 | 已发布版本面 | 停在远端推送前（冻结时点） | `src: … §10.3 L120` |
| OQ-6 | 迁移/身份 Q1–Q10 逐项：过渡版、legacy 条目、设置导入、公告窗口、前置硬/软、跨版本维护、id 归属、音源重置、legacy 上架、新线 def 类型 | 待裁决 | SR→US 路线 | 开放；分派决策会话（§6.3）；条文 [xref: PKG-3] | `src: … §5 L59` |
| OQ-7 | 身份归属推荐（packageId 跟内容走：继承人就地继承 `coahuilite.squeakyratkin`、legacy 拿新 id 冻结）裁决 | 待裁决 | 外部渠道身份 | 推荐未裁决 | `src: … §4 L52` |
| OQ-8 | US 兼容 Q1–Q4：US 是否服务 Ratkin；检测方式；桥重叠期类型名归属；Domain 级跳过 | 待裁决 | U1–U4 前置（Q1 决定 U2） | 开放；条文 [xref: PKG-3] | `src: … §5 L60` |
| OQ-9 | US 侧 U1–U4 实施进度（SR 只读窗口内零运行时改动） | 依赖外部 | 双开兼容 | 未知（GAP-6） | `src: … §4 L51, §6.2 L70–75` |
| OQ-10 | Eat 三态手工验收矩阵未实机：meal/烟卷/薄片/啤酒/仙馔/营养膏/背包吃/动物/尸体 ×（父关、父开子关、父开子开）；重点=父关与旧实现完全一致、父开子开时食物行为与父开子关一致（并集不改变食物）、父开子关时啤酒/仙馔仍响、关父项后子项灰显且值归 false | 未验证 | 已发布手感/功能验收 | 开放（列"实机"类） | `src: … §5 L61; docs/handoff-eat-occurrence-granularity-zh.md §6 L217` |
| OQ-11 | US 双开矩阵未实机（四种组合"恰好一条声音"+跳过记录） | 未验证 | SR+US 组合 | 开放；矩阵本体 [xref: PKG-3] | `src: docs/handoff-0.3.3-zh.md §5 L61, §6.2 L75` |
| OQ-12 | CI 接线（11 项门+隐私门+发布面门、SDK 10.0.x）推送前无法实跑；YAML 仅人工校对 | 未验证 | 发布流水线 | 开放 | `src: … §10.4 L125` |
| OQ-13 | 历史/tag 重写：5 个 `[known-debt]` 文件需重估（范围比先前记录更大）；重写本身需授权 | 需授权+未验证 | 仓库历史隐私 | 开放；方案 [xref: PKG-5] | `src: … §10.4 L126` |
| OQ-14 | Steam 文案/包核验随解除阻断执行（本轮未做） | 依赖外部（OQ-2） | 外部渠道 | 开放；文案源 [xref: PKG-4] | `src: … §10.4 L127` |
| OQ-15 | （对接收方）是否采用父+子两级形态，还是 3 态选择卡 | 待裁决（外部） | US/姊妹项目 UI | 开放；SR 已取两级 | `src: docs/handoff-eat-occurrence-granularity-zh.md §8.1 L266` |
| OQ-16 | （对接收方）子项命名范围：「使用成瘾品」原版等价于药物，实现覆盖面是"任意零营养可摄入物"；模组内容可能需更宽措辞 | 待裁决（外部） | 本地化正确性 | 开放；SR 已用原版措辞 | `src: … §8.2 L267` |
| OQ-17 | （对接收方）是否接受"带营养成瘾品（啤酒/仙馔）父开子关时仍响"；若要静默需加 `ThingDef.IsDrug` 排除（另一语义决定） | 待裁决（外部） | 判定语义 | SR 默认接受 [F 记载]；接收方开放 | `src: … §5.1 L204, §8.3 L268` |
| OQ-18 | （对接收方）回落口径：本仓库取"未确认 ⇒ 完整 job 级（不丢声音）"；接收方若更在意"严格"可取"未确认即静默"——源文档评估"会回到静默 hole，不建议" | 待裁决（外部） | 行为一致性 | SR 已定案；外部开放 | `src: … §8.4 L269` |
| OQ-19 | 是否泛化到其他动作：`Sleep`（在床/未上床）、`Work`、`Social`、`Joy` 目前同样 job 级/粗略判定，同类粒度问题会重复出现 | 待裁决 | 后续版本（0.4/US） | 开放 | `src: … §8.5 L270` |
| OQ-20 | 粒度偏好放哪一层：策略层（纯函数+注入采样）还是动作定义/注册表（与动作门 `SqueakActionDef` / `allowExternalActions` 的关系） | 待裁决 | 架构 | 开放（DEC-4 条 8 已给 SR 倾向：策略层） | `src: … §4 L107, §8.6 L271` |
| OQ-21 | 默认值是否写进兼容政策（"一旦发布，父关 = 默认 job 级就不允许再翻"） | 待裁决 | 存档/设置兼容承诺 | 开放；主源未见政策落档 | `src: … §8.7 L272` |
| OQ-22 | 是否需要 Dev 诊断（面板显示当前模式与 `chewToilNameConfirmed`）以发现 B 的静默退化；约束=不新增日志事件 | 待裁决（可选增强，未实施） | 可观测性 | 开放 | `src: … §5.1 L206–208, §7.1 L262, §8.8 L273` |
| OQ-23 | 若产品要求父开子关时啤酒类带营养药饮也静默 → 需追加 `ThingDef.IsDrug` 排除判定（明载"这是另一个语义决定，不是本规格默认行为"） | 悬置（未采纳备选） | 判定语义 | 开放（挂 OQ-17） | `src: … §5.1 L204` |


----- 9. 锚点（ANCH：必须逐字保留） L339-358 -----
## 9. 锚点（ANCH：必须逐字保留）

**设置/字段/UI**：`eatOnlyDuringChewing`（父，UI「仅在真正进食（正在摄入营养）时触发」）/ `eatIncludeDrugs`（子，UI「使用成瘾品」，原版措辞）；两者默认 `false`；`SqueakyRatkinSettings.cs`（字段+`ApplyToRuntime()`/`NotifyCheapRuntimeChanged()` 两处发布；发布表达式 `eatOnlyDuringChewing && eatIncludeDrugs`）；`SqueakyRatkinSettings.ExposeData.cs`（Scribe add-only 各一行；`PostLoadInit` 父关⇒子 false 归一）；行高 `34f`、高度 +34f、`MeasureBasicsContentHeight` 同步、`Toggle(enabled:/disabledReason:)`、禁用原因键"需先开启上方开关"、父 Tooltip 边界句、子 Tooltip"按咀嚼/点燃阶段判定，无法识别时回落完整进食流程"。`src: docs/handoff-eat-occurrence-granularity-zh.md §5.1 L123/L187–198, §7 L225–231, §7.1 L253–260; docs/handoff-0.3.3-zh.md §4 L47–48`

**规则/实现符号**：`Source/SqueakyRatkin/SqueakEatOccurrence.cs`（纯文件、零 Verse、纯度门不变）；`enum SqueakEatOccurrenceMode { WholeJob, GainingNutrition, ChewingToil }`；`ChewingToilDebugName = "ChewIngestible"`；`ChewingOnlyDefault = false`；`ResolveMode(bool eatingOnly, bool includeDrugs)`；四参 `AllowsOccurrence(mode, gainingNutritionNow, chewToilActive, chewToilNameConfirmed)`；`CompSqueaker.cs` — `IsEating()` / `IsGainingNutritionNow()`（`Pawn.jobs?.curDriver is IEatingDriver eating && eating.GainingNutritionNow`）/ `SampleChewingToil()` / 进程级静态 `chewToilNameConfirmed`（不 Scribe）；`StringComparison.Ordinal`；"决策出生即纯"。`src: docs/handoff-eat-occurrence-granularity-zh.md §5.1 L131–177, §7 L225–230, §7.1 L253–254`

**vanilla 符号**：`JobDefOf.Ingest`；`JobDriver_Ingest.MakeNewToils()`；`Toils_Ingest.PickupIngestible` / `CarryIngestibleToChewSpot` / `FindAdjacentEatSurface` / `ChewIngestible(...)` / `FinalizeIngest`；`ReserveFood`；`JumpIf(chewing, …)`；`PrepareToIngestToils_Dispenser`；`eatingFromInventory`；`IEatingDriver.GainingNutritionNow`（接口唯一成员）；`ToilMaker.MakeToil("ChewIngestible")`；`Toil.debugName`；`Toil.ToString()` = `debugName ?? "unnamed"`；`JobDriver.CurToil`(protected)/`CurToilIndex`/`CurToilString`(public)；`Pawn.jobs.curDriver`(public)；`toil.actor.pather.StopDead()`；`ticksLeftThisToil = Mathf.RoundToInt(thing.def.ingestible.baseIngestTicks * durationMultiplier)`；`ThingDef.IsNutritionGivingIngestible => IsIngestible && ingestible.CachedNutrition > 0f`；`ThingDef.IsDrug`；`preferability=NeverForNutrition`；`DrugAIUtility.IngestAndTakeDrug`；`JobGiver_Binge`；`JobGiver_TakeCombatEnhancingDrug`；`EatAtCannibalPlatter`；`StatDefOf.Nutrition = 5.2`（尸体）；`CompSqueaker.CurrentAction`；`1.6/Patches/Ratkin_AddSqueakComp.xml`（SR 装配点）。`src: docs/handoff-eat-occurrence-granularity-zh.md §2 L25–75, §5 L112–117; docs/handoff-0.3.3-zh.md §7 L85`

**数值**：`baseIngestTicks` 默认 `500`（≈8.3 秒）；烟卷 `720` t（≈12 秒）；薄片 `650` t（≈10.8 秒）；`Beer Nutrition 0.08`；`Ambrosia Nutrition 0.2`、`baseIngestTicks 80`；corpse `Nutrition = 5.2`；`Eat = EachTime, 144 ticks`；`globalMinIntervalTicks = 216` ≈ **3.6 秒**现实时间一发；横穿地图 20–30 秒 ≈ 5–8 发；站桩 ≈ 1–2 发；`scaleCooldownWithTimeSpeed` 默认开、1×/2×/3× 节拍一致；概率门只在 `RandomOneShot` 生效、`EachTime` 不抽概率。`src: docs/handoff-eat-occurrence-granularity-zh.md §1 L21–22, §2 L60/L68/L70/L74, §5.1 L203`

**验收/工具**：`tools/KernelCharacterization/{KernelCharacterization.csproj,UnitTests.cs}` 的 `EatOccurrenceRules`（四组合含"父关+子真=`WholeJob`"、三模式、回落、常量、两默认值断言）；`tools/SettingsFixtureGenerator/Contract/SettingsContract.cs`（镜像子字段）；`fixtures/expected/01-new-install-first-save.xml` 零 delta；`scripts/verify-local.ps1`（`-NoRestore` 11/11 全绿；`-PackDev`）；构建 0 warning / 0 error（Dev + Steam 双 flavor）；本地化 542 键、零重复、中英对齐。`src: docs/handoff-eat-occurrence-granularity-zh.md §6 L214–218, §7.1 L255–259; docs/handoff-0.3.3-zh.md §2 L33, §7 L86, §10.2 L111`

**版本/发布面**：`0.3.3`；`0.3.2`（跳过口径）；`v0.3.2-pre1`；计划 tag `v0.3.3`；`v0.3.0`（手感指纹基线）；`csproj <Version>` ≡ `About.xml <modVersion>`（`stage-package.ps1` 硬断言；release tag 基版本=csproj）；`未发布 — 0.3.3` / `Unreleased — 0.3.3`（UTC+8 换时间）；分支 `dev`/`origin/dev`/main；squash merge；`docs/release_review/release-0.3.3-review-zh.md`（Claim Pack）；`merge -s ours`（runbook 内条件，本体 [xref: PKG-4]）。`src: docs/handoff-0.3.3-zh.md §3 L38–41, §10.3 L120; docs/handoff-eat-occurrence-granularity-zh.md §1 L19`

**流程/隐私工具（09-13）**：`scripts/privacy-audit.ps1`（三向量+身份、`-FullHistory`、`-PrePush`、`$knownHistoryDebt`）；`scripts/check-pack-readiness.ps1`（版本轴/红线/暂存包/DLL 身份/`[claim]` 快照、`-RequireReleaseMetadata`）；`ci.yml`/`release.yml`；11 项门+隐私门（release 再加发布面门）；SDK 对齐 `10.0.x`；`227 revision`；`5` 条 `[known-debt]`；`19` 个已跟踪文件改动（`+161` / `−17`）+ `4` 个 untracked 新文档；`AGENTS.md`「External-state boundaries」。`src: docs/handoff-0.3.3-zh.md §2 L20, §10.1 L106, §10.2 L113`

**编号表与兼容标识（跨包指针）**：U1–U4；`VoicePackCompAttach`；skip 原因日志 `foreign_squeak_comp`；legacy 桥薄空类型 `SqueakyRatkin.SqueakVoicePackDef`；顺序硬门 `U1 → US 型 Ratkin 包/桥 → SR 1.0 内容化`；packageId `coahuilite.squeakyratkin`；F1–F8、Q1–Q4、B1–B5、P0–P4 [xref: PKG-3]；Q1–Q10 [xref: PKG-3]；R1–R12、N1–N3、V1–V3 [xref: PKG-5]；"每次事件恰好一条声音"（验收信号原话）。`src: docs/handoff-0.3.3-zh.md §1 L13–14, §4 L51–52, §5 L59–60, §6.2 L71–75, §7 L83–84, §10.1 L105`

**授权边界**：外部有效操作（commit 属边界内地带：09-13 口径为"本地 commit 允许"、远端操作需授权）需维护者显式授权（`AGENTS.md`）；隐私审查覆盖完整可达范围（发布/推送/上架前）；US 仓库只读（维护者授权路径）；不改语义/默认值（发布会话边界）；Steam 涉及人工编辑。`src: docs/handoff-0.3.3-zh.md §2 L34, §6.1 L68, §8 L89–93, §10.3 L116`


----- 11. 证据缺口（GAP-1…GAP-7） L371-382 -----
## 11. 证据缺口（GAP-1…GAP-7）

| id | 想回答的问题 | 为什么现有语料答不了 | 需要什么才能回答 |
| --- | --- | --- | --- |
| GAP-1 | Eat 三态矩阵/US 双开矩阵是否已实机执行、结果如何 | 两主源均只列"开放/待实机"；实机记录属 `TODO.md`/后续会话（记忆面语料外） | 实机会话记录或 TODO 快照 [xref: MEMORY.md/TODO.md（语料外指针）] |
| GAP-2 | 0.3.2 跳过的终局裁决与理由 | 语料只有"口径不变"，无裁决动作 | 发布会话的裁决记录/CHANGELOG 终稿 [xref: PKG-4] |
| GAP-3 | 隐私债务"比先前记录更大"的先前口径 | 先前记录不在本包主源 | PKG-5 主源（rewrite-plan）对照 |
| GAP-4 | Q1–Q10 / Q1–Q4 / V1–V3 / R1–R12 逐项内容 | 主源仅存编号与主题词 | PKG-3 / PKG-5 展开 |
| GAP-5 | Steam 评论原文与时间、是否另有负面反馈（U-5/U-6） | 仅正面转述一条 | 评论渠道访问（语料外） |
| GAP-6 | US 侧 U1–U4 在分派后是否动过 | SR 仓库只读窗口，US 进展不在语料 | US 仓库记录（授权面外） |
| GAP-7 | 09-13 三块提交的 commit 标识与顺序 | §10 仅声称；本包被禁外部取证（规则 9） | 下游 `git log`（冻结动作豁免外，供终审） |

