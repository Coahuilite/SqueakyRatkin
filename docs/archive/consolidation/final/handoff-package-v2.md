# 最终压缩任务包 v2（供下游专家模型独立判断）

## 0. 元数据与压缩账本

- repo_rev：`4df9594713adbbba91e0aea788a7c7cd3503ab3f`
- frozen_at：2026-09-17
- mode：two-pass（pass-1 六包 compact 已冻结；本文件 = pass-2 合并）
- 生成者：提炼 agent
- 只读边界：未改 `docs/**` 源文档、未改 packages v1、无 git 写；唯一写入 = 本文件与 `docs/consolidation/final/v2-parts/`
- 隐私：无盘符形态、无本机绝对路径、无日志摘录、无凭据、无 `PublishedFileId` 值；债务路径只描述形态
- 品牌：`鼠辈啁啾` / `Squeaky Ratkin` 原样；中文散文称鼠族
- 维护者插话（优先于 02 §4 字数帽）：压缩服务于终审可独立给出审核意见，不为 compact 而 compact。本文件仍做跨包去重与表格化，但 **P0 条目不删**；§0 同时报 02 §1 散文目标与 02 §4 / README §8.7 三线数字。

### 输入与规模（UTF-8 解码字符数；与 coverage-check 一致）

| 项 | 字符数 |
| --- | --- |
| 全语料 R（26 文件） | 194,352 |
| Σ输入包 S（六包 v1，不含 r2） | 383,391 |
| 02 §1 散文「有效目标」min(0.35S, 0.20R) | 38,870 |
| 严格档 T = min(0.25R, 0.60S) | 48,588 |
| 密度举证档 T = min(0.30R, 0.75S) | 58,306 |
| 硬线 0.60S | 230,034 |

编排方裁定：达标判据以 **02 §4 / README §8.7 三线**为准，02 §1 散文目标只作对照。维护者插话后：若三线未达，**不删 P0 凑数**，在 §12 诚实报告。

pass-1 compact 实测（冻结稿）：PKG-1 37,965 / PKG-2 38,571 / PKG-3 31,568 / PKG-4 30,024 / PKG-5 14,463 / PKG-6 24,790；Σ compact = 177,381（= 0.463·S）。`PKG-4-release-evidence-r2.md` 不在输入。

**pass-2 合并稿实测**：本文件 50,776 字符 = 0.261·R（194,352）/ 0.132·S（383,391）；严格档 T=48,588 未达（超 4.5%）、硬线 0.60·S 远未触；核心三节 §3+§5+§6 = 41.7%（<50%，按 02 §4 例外条款说明：记账/锚点/来源节占 37%）；逐节超限项（对严格档 T=48,588 的上限）：§0（记账实测）、§4（73 条 ASM 全保留）、§7（38 条教训）、§8（51 GAP 合并行）、§9（0.147·T>0.06·T，锚点逐字面）、§11（12 组钩子）——均为 P0/记账/锚点/钩子面，按 02 §4 处置顺序压表达不删条目；§9 已压至族级合并，P0 零删减。

### 输入包清单

| 包 | 文件 | v1 字符 | compact 字符 |
| --- | --- | --- | --- |
| PKG-1 | `packages/PKG-1-current-contracts.md` | 58,194 | 37,965 |
| PKG-2 | `packages/PKG-2-0.3x-decision.md` | 60,861 | 38,571 |
| PKG-3 | `packages/PKG-3-universalization-us-split.md` | 77,230 | 31,568 |
| PKG-4 | `packages/PKG-4-release-evidence.md` | 84,941 | 30,024 |
| PKG-5 | `packages/PKG-5-process-privacy-debt.md` | 55,890 | 14,463 |
| PKG-6 | `packages/PKG-6-handoff-open-rulings.md` | 46,275 | 24,790 |

### 编号规则

- 引用原包一律 `PKG-n#ID`（PKG-4 决策为零填充 `DEC-01`）。
- 本包 DEC/CNF/OQ/GAP/ASM/ANCH 为合并后组号；原 id 映射见各节表头或 `also:`。
- ANCH：PKG-1/2/3 用 `ANCH-n`；PKG-5 三组；PKG-4/6 仅族名。本包 §9 用组号 ANCH-A…，组内标注原 id/族名。

### P0 完整性对账（v1 机械 unique id → 本包去向）

| 类 | in（六包 unique 合计） | out | 说明 |
| --- | --- | --- | --- |
| CNF | 8+8+10+11+8+6 = **51** | **51** | §6 按主题分组，每条原 id 均出现 |
| OQ | 13+11+20+13+16+23 = **96** | **96** | §6；真重复只合并表达并 `also:`，不删号 |
| DEC | 37+16+29+22+15+12 = **131** | **131** | §3 主题合并为 DEC-v2-1…，映射表列全 131 |
| ASM | 10+15+16+12+12+8 = **73** | **73** | §4 去重表达，原 id 全列 |
| GAP | 7+5+11+10+11+7 = **51** | **51** | §8 |
| ALT | 1+8+28+8+0+6 = **51** | **51** | 并入对应 DEC 的备选段 |
| ANCH | 10+10+7+族+3+9族 | 全标识符 | §9；数值不得增删改 |
| INC/EV/LES/TL | 全 | 全 | §2/§5/§7；P1 压表达 |

**数量减少：无。** 主题合并不是删条。PKG-1 的 OQ-22 是跨包编号（载体 PKG-6），v1 已登记，本包并入 Eat 可观测性 OQ。

### 已知压缩损失

- 未复制：Claim Pack SHA-256 全表、BBCode 文案体、CHANGELOG 双语全文、28 条 v1 人读句、mermaid/C# 签名块、债务路径字面量、Player.log 原文。
- 压表达：DEC 背景重复、长 `src:` 集中到 §10、同一假设只写一次。
- **不影响判断的原因**：终审按 `src: PKG-n#ID` 回 v1 或按 §10 回源文件即可拿到全文；P0 条目、矛盾双方、未决清单、锚点标识符仍在本文件。
- `[I-提炼]` 只出现在本 §0 与 §11。

### 编排方跨包接缝（已执行，未自行裁决）

1. Eat 可观测性：PKG-1 U-7「合同无 Eat 日志字段」改记为 PKG-6 已裁决**不新增日志事件**；OQ-22 Dev 诊断面板仍开。
2. C 链：可见 C1–C10、C12–C14（PKG-2）+ C17–C19 + C34–C40（PKG-4）；**缺席 C11、C15、C16、C20–C33**。禁止写成连续 C1–C40。
3. fallback 类名：合同 `BuiltInFallbackCatalog` vs 规划示例 `SqueakBuiltInFallbackCatalog`（PKG-3#CNF-10）。
4. 0.3.1 无正式版；0.3.2 仅 `v0.3.2-pre1` GitHub prerelease，工作并入 0.3.3。PKG-1#CNF-8 合同锚点仍保留。
5. tier 折叠：`pack_fallback`→`race_pack`、`built_in_fallback`→`vanilla`；归因损失是否已知取舍 = **未记载**，禁止写成「设计如此」。
6. 隐私三源并列：PKG-5 债务方案、PKG-1#CNF-1 `pawn=<label>`、PKG-4 门禁。不得因有方案而判闭环。
7. 权威序只标注「若必须选一个，建议以谁为准」，冲突本身保留。

## 1. 项目上下文（现行状态 vs 文档时间层）

`鼠辈啁啾` / `Squeaky Ratkin`（packageId `coahuilite.squeakyratkin`，C# `SqueakyRatkin`）是 RimWorld 1.6 模组：只服务 NewRatkinPlus Ratkin 的固定 17 动作发声外壳 [F]（PKG-1#DEC-2）。身份判定唯一依赖 `CompSqueaker` 与精确 `defName`；动作键 append-only；作者 XML ABI 自「首个携带 0.3.1 ABI 的发行版本」起 public-stable [F]（PKG-1#DEC-14）——该起点在合同内未闭合（PKG-1#CNF-8），发布事实见 PKG-4：0.3.1 无正式版、0.3.2 仅 `v0.3.2-pre1` [F]。

**冻结时点（2026-09-17）现行有效状态 [F，多时间层并存]**：
- 产品：Eat 两级开关已落地（默认双关 = 旧 job 级手感）；VoicePack XML 公开稳定合同已写入架构合同与 0.3.3 CHANGELOG Unreleased。
- 仓库：0.3.3 工作树**本地已提交**（功能/文档记忆/流程三块），**远端未 push**（PKG-6#CNF-1：08-23 层写「0 提交」，09-13 层写「已提交停在推送前」）。
- 发布：GitHub 最新正式 tag = `v0.3.0`；另有 prerelease `v0.3.2-pre1`；Steam 阻断中，页面锚点仍 0.3.0；无 0.3.3 Claim Pack。
- 前瞻：US 仓库并行已定案但 **08-22 明示暂不建仓**（PKG-2#OQ-2）；Q1「US 是否服务 Ratkin」未裁决且冲突一方在语料外（PKG-3#CNF-1）。
- 隐私：工作树 0 命中；历史 blob 5 文件债务；方案 A 维持、B 备而不执行，**未授权**。

**权威序（只用于标注，不删冲突）**：AGENTS.md 操作硬约束 → 三份现行合同 → runbook / 记忆 → 已接受决策与规划输入 → handoff / 检查表 / 评审 → 历史过程文档与旧 CHANGELOG → 冷归档。合同自排在 accepted product decisions 之下、source/Defs 排其之下（PKG-1#DEC-1）⇒ 合同**不是**最高权威 [F]。

本包时间范围：0.1.0（2026-07-04）→ 冻结日 2026-09-17。handoff 的「当前」一律当写作时状态。

## 2. 主要时间线

| 时间 | 事件 / 裁决 | 当时 vs 现行 | 来源 |
| --- | --- | --- | --- |
| 2026-07-04 | 0.1.0 首发 | 当时=首个公开；现行=历史 | PKG-4#TL-1 |
| 07-05 | 0.1.1 + 唯一合法 `Initial Workshop Upload` | 现行=历史；后续禁用该短语 | PKG-4#TL-2 |
| 07-30 | 0.2.0：15 动作 / VoicePack / 破坏性变更 | 现行=基线兼容资产 | PKG-4#TL-3 |
| 08-15/16 | 0.2.1 首发作废（121 文件泄漏）→ 重发 115 文件 | 废止口径生效 | PKG-4#DEC-12 |
| 08-16 | 0.2.2 GitHub 完整；Workshop 仅「维护者报告」 | 页面观察至今未回填 | PKG-4#OQ-1 |
| 08-17 | 0.2.3 默认音源反转（内置 Race Example 默认启用） | 现行；旧 Off/Vanilla 默认废止 | PKG-4#DEC-13；PKG-1#TL-1 |
| 08-18 | 0.2.4 GitHub+页面核验；决策文档「已接受」方案 A；A–H 门槛制定 | 架构当时=设计接受 | PKG-2#TL-1；PKG-4#TL-10 |
| 08-19 | 交接 0.3.0：换链完成、未推送、15 项 13 done | 当时态；被 08-20 取代 | PKG-2#TL-4 |
| 08-20 | 路由公理确认；彩蛋定案+YAGNI 去运算符；三项发布决策（**不设 GitHub prerelease tag**）；实机+后置修复 C12；八面全绿 C13/C14 | 门槛达成；发布执行当时仍阻塞 | PKG-2#TL-5/6 |
| 08-21 | 0.3.0 GitHub+Workshop 页面级；旧置顶公告「仍在线」；0.3.1 计划重排 | 发布已执行（PKG-4）vs 检查表「仍阻塞」（PKG-2#OQ-1）=时间层 | PKG-4#TL-12；PKG-2#DEC-10 |
| 08-22 | ABI 冻结声明；动作门转 US 0.3.x 窗口；IsEgg 入公开 ABI（旧口径废止）；US 并行重定（暂不建仓）；0.4 双仓+七规则+legacy 桥离线原型；Eat 例外尚未写入合同 | 本包语料内架构最新计划口径 | PKG-2#TL-8；PKG-1#TL-2 |
| 08-23 | Eat 两级开关落地；handoff-0.3.3 写作（**未提交**）；compat/mig 同日；SKILL.md 正本；egg/身份日志实机；`v0.3.2-pre1`；Steam 阻断；日志协议末次提交 | Eat 已实施；交接当时=0 提交 | PKG-6#TL-3..7；PKG-4#TL-13 |
| 08-24 | legacy 桥薄空类例外授权（compat 记载；桥尚未实现） | 日期晚于 compat 前言→PKG-3#CNF-5 | PKG-3#TL-7 |
| 09-06 | UniversalSqueaker「三命令+最小仪式」裁决（文本语料外） | 被 SR 09-13 对齐 | PKG-4#TL-15；PKG-5#TL-6 |
| 09-13 | 流程简化落地；V1–V3；runbook 重写；0.3.3 **本地三块提交**、停在远端推送前；架构合同+设置合同更新（Eat 三级模式） | 现行本地面；远端未验证 | PKG-6#TL-8/9；PKG-5#TL-7/8；PKG-1#TL-7 |
| 09-17 | 语料冻结 repo_rev；本管道阶段 A PASS | 现行冻结 | coverage-check |

## 3. 关键决策史

> 合并后组号 DEC-v2-*。每组列出原 `PKG-n#DEC-*`。硬约束要点足以让终审提出反驳；条文全文在合同/决策文档。数字/字段序指向 §9。

### DEC-v2-1 产品身份、17 动作、作者 XML ABI、选择链（PKG-1#DEC-1..22；also PKG-2#DEC-3）

- **状态**：现行有效 [F]。时间：ABI 声明 2026-08-22；合同冻结 2026-09-13 `e0608f7`。
- **身份**：只服务 NewRatkinPlus Ratkin；`it is not a generic HAR-race framework.` ALT-1 通用 HAR 框架已否弃；**吸收**：Def 前缀 `SR_`、C# 靠命名空间隔离。Xenotype 身份=精确大小写敏感 `XenotypeDef.defName`。No-DLC 不得进 Xenotype DefDatabase / pawn-gene 路径。
- **17 动作 append-only**（序号 0–16，见 ANCH-A）：`Crying`=15、`Giggling`=16 追加，0–14 不变。新增动作=协同变更不是插件注册。BabyFits 窄 patch：`TryStartMentalState` 成功 postfix，仅 `MentalState_BabyCry`/`MentalState_BabyGiggle`；不得扩大 `MentalBreakWorker.TryStart`。
- **Eat 合同现行**（PKG-1#DEC-7；规格史见 DEC-v2-13）：默认 **WholeJob**（整个 `JobDefOf.Ingest`，禁止默认收窄）；`eatOnlyDuringChewing`/`eatIncludeDrugs` 默认 `false`；三模式 `WholeJob`/`GainingNutrition`/`ChewingToil`；toil 名未确认 → 回落 WholeJob **而非静默**。
- **选择链**：Off=仅内置 fallback；Fallback=Xenotype→Race→pack fallback→built-in→静默；Remix=等层选择；未声明 pack fallback 保留**冻结三层 Remix 形状**。内置档案 `BuiltInFallbackCatalog` Ratkin **15 键**（缺 Crying/Giggling）。独立 Config `SqueakyRatkin_Profile_<race>.xml`，永不 `WriteSettings()`。orphan/dormant 保留；唯一破坏性出口=玩家确认「忘记此目标」。
- **ABI 冻结面**：`SqueakVoicePackDef` XML（`raceDefName/scope/targetDefName/weight/fallbacks/actions(action,ageTag,IsEgg,sounds)` + `SR_*` SoundDef）public-stable：字段 add-only、动作键 append-only、fail-closed。**未冻结**：kernel/domain/fallback profile schema/未来 action gates。起点短语「first released version that carries the 0.3.1 ABI」未闭合 → CNF-v2-ABI。
- **废止**：禁止重新引入 zoom 门（`CurrentZoom <= Close`）。
- **后果**：Example 基线 15 动作/41 OGG（可变，不断言固定总数）；作者根 `<lowercase packageId>/<PackDef.defName>/<Action>/`；第三方碰撞不仲裁。
- src: PKG-1#DEC-1..22；also: PKG-2#DEC-3, PKG-2#DEC-14

### DEC-v2-2 设置 UI 产品面（PKG-1#DEC-23..30）

三页常显（发声规则 / 心情音色 / 语音来源与异种）；**7 次**点击解锁第四页，未解锁无占位。immediate：约 350 ms 合并保存、关窗 flush；无 Apply/Revert。Remix 两步确认无副作用。排障工具编译进全部 flavor，可用性仍受 Dev Mode/地图门控。**唯一例外**（2026-08-23）：Eat 控件对；Scribe add-only、schema 不 bump。非目标清单仍有效，该例外书面记录。
src: PKG-1#DEC-23..30

### DEC-v2-3 日志协议 v1/v2（PKG-1#DEC-31..36）

`SqueakLog` 闭包类型门面；文件自述「源审阅所得合同、**不是**实测会话证据」[C]。v1：**28** 事件、字段序、once-key、人读句 **byte-immutable**。v2（0.3.1）：`fmt=2`；核心序加入 `race [xenotype]`；4 条扩展事件（`settings.origin`、`audio.route.selected`、`hook.mental_fit.unavailable`、`fallback.profile.store_failed`）；once-key 分域 `log-v1`/`log-v2`。0.3.2：装配器每动作每 5 秒一条合并 fmt=2，不再并行发 v1 `audio.dispatch.ok`。**tier 可发词汇**仅 `xenotype_pack`/`race_pack`/`vanilla`/`-`；`PackFallback`→`race_pack`、`BuiltInFallback`→`vanilla`（CNF-v2-tier）。`audio.route.selected` 含 `pawn=<label>` vs 通则禁写 pawn 标签（CNF-v2-pawn）。Eat **不新增日志事件**（PKG-6 裁决）；替代 Dev 面板未实施（OQ 跨包-22）。
src: PKG-1#DEC-31..36；also: PKG-6#DEC-5

### DEC-v2-4 「内核但薄」与落选吸收（PKG-2#DEC-1,2,15,16）

- 08-18 采纳方案 A：选择语义进零 Verse 内核；约 500+200 行把验收变成确定性语料（ASM 实际行数未知）。
- **ALT 吸收（不得当「没选所以无贡献」）**：B→迁移事务性 + `GetTargetCandidates` assembled-only + BabyFits 源码核验；C→YAGNI（TimingModel 原地、不拆程序集）；D→黄金语料全矩阵升级（设置×15 action×域×多种子；Crying/Giggling 后扩 17）。
- 不建独立程序集：`Kernel/` 文件夹 + harness 链接编译（引用 Verse 即失败）。内核五文件：`Domain.cs/Pool.cs/Modulation.cs/FallbackProfile.cs/DomainFilter.cs`（0.4.x 删后者）。
- 风险 8 项常设（PKG-2#DEC-15）；INC-1 属风险 #1 残余（全绿未覆盖 BuildFallback）。
- 纪律：每阶段单活路径、不为 0.4 预建、阶段门必须机器可复现、受影响结论先提升进合同。
src: PKG-2#DEC-1,2,15,16

### DEC-v2-5 动作门、路由公理、彩蛋（PKG-2#DEC-4,5,6）

- **动作门**：受控开放为主（ALT-4b）；共识八条（17 ABI 不动、外部字符串键、无事件总线、诚实安全模型等）。SR 0.3.x 只做两件零成本事；机制本体改在 **US 仓库 0.3.x 窗口**（08-22 修订，CNF 窗口层）。放弃信号三条常设（OQ 监控）。
- **公理**（08-20 维护者确认 [C]）：路由是核心、其余是叠叠乐；四条（链唯一事实源 / 依赖单向 / 只经注入面 / 语义下沉核心）。新决策逻辑出生即纯。
- **彩蛋**：field-presence `IsEgg`，无运算符（同日 YAGNI）；默认关；开=加性入池同权混抽。08-22：**自 0.3.2 起纳入公开作者 ABI**，旧「0.3.x 内部标签」废止。Kiiro 内容「实验分支不发布、未经许可不宣传」。
src: PKG-2#DEC-4,5,6

### DEC-v2-6 0.3.0 等价、A–H、后置回归（PKG-2#DEC-7,8,9）

- **第一原则**：已发布功能=玩家可能使用→设置全项验证；唯一豁免=dev 隐藏/七次点击面。
- **诚实边界**：等价 ≠ 同 seed 对拍（那需方案 D，已否）。14 行全部判「等价」。例数双层 **1622（C4）与 3782（C7）并存**，勿合并。
- **A–H 08-20 八面全绿**才推 Steam（Steam=发布即全员无灰度）。G=「绿（论证）」未做 dev 计数；E=「绿（审计）」。**同日后置修复**：BuildFallback 丢种子静音回归（C12 `0b0fa48`）+ `_Preview` 排除回归 + 断言 41→43。全绿 ≠ 无回归（LES）。
- 三项发布决策：试用=**本地 dev 包**、不设 GitHub prerelease tag（交接预写被否定）；措辞不提 US/多种族/Kiiro/通用化；热修 `vX.Y.Z-hotfixN` 不 bump z。
- 检查表时点发布仍阻塞；**PKG-4 记录 08-21 0.3.0 已发布**——本包把「仍阻塞」当 08-20 时间层，不覆盖发布事实。
src: PKG-2#DEC-7,8,9；also: PKG-4#DEC-14

### DEC-v2-7 阶段映射与双仓（PKG-2#DEC-10..14；also PKG-3#DEC-16）

- **0.3.1 计划**：全分离收口 + race/年龄/fallback/彩蛋运行时 + XML ABI 定型 + schema 3→4 / 1→2。实施过门记录不在 PKG-2（GAP）；CHANGELOG 0.3.3 Unreleased 列出这些工作随 0.3.3 发。
- **0.3.2 计划**：原 UI 专项**转 US**；身份门控已实现（`IsPlayerControlled`；`playSound:false` 过滤）；SKILL.md 唯一正文。
- **US 并行（08-22）**：repo=`coahuilite/UniversalSqueaker`、packageId=`coahuilite.universalsqueaker`、前缀 `US_`/`usdiag`；**暂不建仓需授权**。
- **0.4**：双仓同步上架、US 暂非 SR 依赖、SR 独占 Ratkin、US 无 Ratkin 装配；**双开七规则**（禁 `SqueakyRatkin.*` 类型、全分离、只操作自身 comp 等）。**SR 1.0.0**：退化为纯音频包、US 成前置；legacy 桥薄继承 `SqueakyRatkin.SqueakVoicePackDef`（离线原型完成，真 Verse XML 待实机）。
- Scribe 事务性：临时 clone 全成功才替换；失败保留旧 schema；不静默删除 orphan/dormant。
src: PKG-2#DEC-10..14

### DEC-v2-8 通用化数据模型与六门（PKG-3#DEC-1..19）

先在同一 SR 程序集内普遍化，过六门才物理拆 US。非目标：不是什么都接；Kiiro 许可门逐字「未经作者明确许可不得发布或宣传」。四类键 + 五级选择链。内置 fallback 规划起点 `{Ratkin, Kiiro}`。存储：C# 单源 + Config 副本（规划示例名 `SqueakBuiltInFallbackCatalog` vs 合同 `BuiltInFallbackCatalog`）。年龄：`AgeBucket {Baby, Toddler, Child, Adult}`，查表不重算，Toddler 无 1.6 原生。`ProductDomainFilter` 0.3.x 常量 `{Ratkin}`，三处入口，**0.4.x 移除**。试验名单 default `{Ratkin}` / experimental `{Ratkin, Kiiro, Miho}` 替换非叠加。

**六门逐字**：1 逻辑层零 Ratkin 硬编码（数据/fallback 除外）；2 至少一个外来 race per-race pool 端到端；3 Ratkin characterization 持续全绿；4 旧 SR settings→US 迁移设计与实机；5 保存兼容 `SR → US + SR`；6 Workshop item 收缩/新 item/依赖/页面/Claim Pack 过渡演练。达到六门前 US 不建 Workshop 空壳。通过条数 **无记录**（写「未验证」不得按版本推进推断）。
src: PKG-3#DEC-1..19

### DEC-v2-9 拆分顺序、双开、身份归属（PKG-3#DEC-20..29；also PKG-6#DEC-7,8）

- **当前双开安全**（US 无 Ratkin 内容、无桥）vs **US 型 Ratkin 包或桥落地即双响**（逃逸门类型不匹配→追加第二 comp）。16 patch 有 15 同名同目标。
- **归属唯一解（存量玩家约束）**：SR 本窗口**零运行时改动**；修复落 US。ALT「SR 检测到 US 就不装配」被否决（只装 US 无 Ratkin 包时鼠族变哑）。
- **顺序硬门（倒置即事故）**：`U1 检测落地 → US 型 Ratkin 包/legacy 桥 → SR 1.0 内容化`。U1=跨程序集「已有任意 squeak comp 即跳过」+ `foreign_squeak_comp` 日志。
- 卸载安全 + 迁移不得写存档（F 记载；仓库级条文 [xref: AGENTS.md]）。
- 身份：**推荐** packageId 跟内容走（继承人继承 `coahuilite.squeakyratkin`，legacy 拿新 id）——**推荐未裁决**。反转：「无法继承 packageId」不是必然，是方案 A 的前提造成的。Workshop 原地升级平台层未取证。
- Q 编号跨文档冲突：compat Q1–Q4 ≠ mig Q1–Q10；转述必须带文档限定。
src: PKG-3#DEC-20..29；also: PKG-6#DEC-7,8

### DEC-v2-10 发布流程机械化（PKG-4#DEC-01..11,16..22；also PKG-5#DEC-6..12）

- runbook=唯一入口；版本事实归 Claim Pack。
- **三命令**：`verify-local.ps1` / `check-pack-readiness.ps1 -RequireReleaseMetadata` / `privacy-audit.ps1 -FullHistory -PrePush`。新检查先落脚本，不再新增人工清单。
- **最小仪式**：两命令全绿 + **发布裁决本身**（无规定留痕格式）。
- **授权**：本地 commit 不需要；远端 push/PR/merge/tag/Release/Workshop **需要明确授权**。V1 追认（废止旧「本地 commit 需授权」）。
- 渠道独立词表；「页面级核验 ≠ 二进制已验证」；Steam 编辑=维护者人工，agent 只读 filedetails。
- 09-13：R1–R12 收敛为读时门；N1 不动 verify-local 11 项；N2 不引入 US carrier 门；N3 不自动生成 CHANGELOG 正文。双向棘轮：不得加回人工仪式，也不得把人的判断吞进脚本。
src: PKG-4#DEC-01..11；PKG-5#DEC-6..12

### DEC-v2-11 版本序列与 Steam 阻断（PKG-4#DEC-12..15,08）

渠道矩阵要点：0.3.1 **无正式版**；0.3.2 **仅** `v0.3.2-pre1` GitHub prerelease，Steam **阻断不执行**；工作并入 0.3.3；冻结时 0.3.3=`Unreleased`、无 Claim Pack。0.2.1 作废重发（codemap 泄漏 + LoadFolders auto-merge）。0.2.3 默认音源反转。置顶公告「仍在线 vs 如期删除」两时间层并存。
src: PKG-4#DEC-08,12..15

### DEC-v2-12 流程事故与隐私债务（PKG-5#DEC-1..15）

8 条措施（1–7 已实现、8 Comments→TODO **待实现**）。台账冻结不再增长。隐私债务：**5 文件 × 1 处**历史 blob；≈229 漂移；10 tag（与「v0.2.0 唯一干净」冲突，10 vs 9 未定）；54 处仓内 hash 引用；1 处跨仓 `b19d68a`。方案 A 维持（当前）；B 定向重写备而不执行（P0–P6 逐条需授权、5 条不可逆点）；C 只改 tag **已否决**（伪安全感）。**GitHub 不保证删除**——不能承诺「重写即消失」。无 SR mirror = B 的 P0 阻塞。
src: PKG-5#DEC-1..15

### DEC-v2-13 Eat 触发粒度（PKG-6#DEC-1..5；also PKG-1#DEC-7,30）

- **招牌手感**：默认 job 级不得收窄（0.4/US 同样适用）。机制：`IsEating()`=`CurJob.def==Ingest`，Eat 高于 Move；`EachTime 144` + 全局冷却 216t ≈ 3.6 秒一发；横穿地图 5–8 发主要响在赶路。触发=玩家正面评论 [C]。
- **vanilla 基线**：toil 链 ReserveFood→Pickup→CarryToChewSpot→FindSurface→`ChewIngestible`→Finalize；权威① `GainingNutritionNow`=营养不是「在嚼」；权威② `CurToilString=="ChewIngestible"`。真值表：烟卷 720t / 薄片 650t = job Eat / 营养**否=静默 hole** / toil 是；啤酒 Nutrition 0.08、ambrosia 0.2 营养级通过。
- **两级=三级**：父关→WholeJob；父开子关→GainingNutrition；父子开→ChewingToil。纯函数零 Verse。
- **fail-open**：toil 名未确认⇒完整 job 级，**绝不静默**。反向「未确认即静默」评估为回到 hole、不建议（对接收方仍开放）。
- **不新增日志事件**。Dev 面板未实施。
- ALT：A 吸收为中间档；B 采用为子档（「加日志」否决）；C 不采纳；D 后备未启用；3 态选择卡 SR 不用。
src: PKG-6#DEC-1..5

### DEC-v2-14 0.3.3 交接与推送门（PKG-6#DEC-6..12）

版本=0.3.3；0.3.2 跳过**口径**（正式确认仍开放）。三会话：发布 / US / 决策。09-13：本地三块提交（比建议两块多出流程块）+ 推送前三步（check-pack → privacy-audit → 发布裁决 V1–V3 与 Steam）。授权后链路：push dev→PR→CI→squash merge→tag `v0.3.3`→…→Claim Pack。**未做**任何远端操作。已定案 7 条=Eat 1–4 + 版本口径 + US 修复归属 + 身份推荐未裁决。
src: PKG-6#DEC-6..12

### 原 DEC id → 本包组号（131/131）

- DEC-v2-1 ← PKG-1#DEC-1..22
- DEC-v2-2 ← PKG-1#DEC-23..30
- DEC-v2-3 ← PKG-1#DEC-31..37（37=套件自引用，非源合同）
- DEC-v2-4 ← PKG-2#DEC-1,2,15,16
- DEC-v2-5 ← PKG-2#DEC-4,5,6
- DEC-v2-6 ← PKG-2#DEC-7,8,9
- DEC-v2-7 ← PKG-2#DEC-3,10,11,12,13,14（DEC-3 冻结口径也入 v2-1）
- DEC-v2-8 ← PKG-3#DEC-1..19
- DEC-v2-9 ← PKG-3#DEC-20..29
- DEC-v2-10 ← PKG-4#DEC-01..11,16..22 + PKG-5#DEC-6..12
- DEC-v2-11 ← PKG-4#DEC-12..15
- DEC-v2-12 ← PKG-5#DEC-1..5,13..15
- DEC-v2-13 ← PKG-6#DEC-1..5
- DEC-v2-14 ← PKG-6#DEC-6..12

PKG-2#DEC-3 同时支撑 v2-1 与 v2-7（冻结口径修订）。无 DEC 被丢弃。

## 4. 假设及其演化（ASM 73 条去重表达；原 id 全列）

> 同一事实多包登记→合并为一行、原 id 全列；时间层与条件差异保留在「状态/失效点」。`B1–B5`＝PKG-3 迁移事实基线（与对应 ASM 同号合并）。标签继承原包；`[C]`＝当事人声称、`[I]`＝推断、`[U]`＝未验证，未标者承原包 `[F]`。

| # | 假设（合并要旨） | 原 id | 状态 | 验证 / 失效点 |
| --- | --- | --- | --- | --- |
| A1 | 兼容边界分层：作者面承诺稳定、内部面留 0.x 修订自由 | PKG-1#ASM-1；PKG-2#ASM-8 | 成立；08-22 收窄（作者 XML 面提前公开稳定，DEC-v2-1/DEC-v2-7） | 作者面被迫破坏性变更即失效 |
| A2 | 环境性缺失（DLC/目录/包）降级不剥夺资格、可自动恢复 | PKG-1#ASM-2 | 成立 | orphan/dormant+`GlobalOnly` 回落即制度化 |
| A3 | 名称/图标/发现信息永不是资格判据 | PKG-1#ASM-3 | 成立 | 架构/设置合同双写 |
| A4 | 稳定键空间与字段序＝兼容面本身，须闭合 | PKG-1#ASM-4 | 成立 | v1 byte-immutable；动作键 append-only |
| A5 | 新机读事实带版本标记（fmt=2）不与 v1 混写 | PKG-1#ASM-9 | 成立 | once-key 按 `log-v1`/`log-v2` 分域 |
| A6 | 决策逻辑出生即纯：规则可作纯数据入 harness、Verse 采样留适配层 | PKG-1#ASM-5；PKG-2#ASM-1,2,3,5 | 成立；成本假设部分受挫（全绿仍漏回归，PKG-2#INC-1） | 纯度门+harness 链接编译强制；实际行数未验证 |
| A7 | 不承诺无关设置迁移；配置维度彼此独立 | PKG-1#ASM-6 | 成立 | 明文禁令；引入迁移工具须重开 |
| A8 | 年龄查表不重算；1.6 无 Toddler 原生 | PKG-1#ASM-7；PKG-2#ASM-14；PKG-3#ASM-10（`Pawn.cs:2022`） | 成立 | `CurLifeStage` 直映；表外→Adult 兜底 |
| A9 | 语音包只贡献声音，不改行为/timing/mood | PKG-1#ASM-8 | 成立 | 模式语义+内置档案归维护者 |
| A10 | 日志合同是兼容合同、不是实机证据 | PKG-1#ASM-10 | 成立（自限） | 无反证也无正证（PKG-1#GAP-1） |
| A11 | 独立程序集在 0.3.x 无消费者 | PKG-2#ASM-4 | 成立至 0.4.x | 届时搬 US csproj |
| A12 | 存档按枚举名序列化；namespace 不变零 churn | PKG-2#ASM-6 | 成立 | fixture 9 场景字节一致 |
| A13 | 等权=均匀分布；同 roll 同结果可测 | PKG-2#ASM-7,12 | 成立 | 分布等价≠逐次对拍（诚实声明） |
| A14 | 恶意 mod 无解→诚实安全模型 | PKG-2#ASM-9 | 成立 | DEC-v2-5 共识八条 |
| A15 | 第三方动作扩展需求可能不出现 | PKG-2#ASM-10 | 悬置 | 放弃信号常设监控（PKG-2#OQ-6） |
| A16 | 0.3.0 注入面=旧选择面，全排序无行为差异 | PKG-2#ASM-11 | 成立（0.3.0 范围） | 0.3.1 多 race 后失效 |
| A17 | G 面可由分配论证替代实测 | PKG-2#ASM-13 | 被接受未闭合 [C] | 「绿（论证）」；dev 计数未做 |
| A18 | `XmlToObjectUtils.SearchTypeHierarchy` 填充继承字段 | PKG-2#ASM-15 | 离线已证 | 真 Verse XML 加载待实机（PKG-2#OQ-8） |
| A19 | SR/US 分叉：16 patch 15 同名同目标→双 comp 双响 | PKG-3#ASM-1 | 成立（现状） | US 型包/桥落地即触发 |
| A20 | 类型身份互不可见（逃逸门判据） | PKG-3#ASM-2 | 成立（现状） | 桥落地即改变 |
| A21 | comp 不进存档；依赖缺失=警告不崩档 | PKG-3#ASM-3(=B1)；PKG-3#ASM-14(=B2) | 成立（1.6 [C]；B2 条件=SR 1.0 不编译引用 US） | 1.7 未验证（Q6'） |
| A22 | 修复落 US 成本为零、落 SR 打存量玩家 | PKG-3#ASM-4(=B4) | 有时效 | US 发布后失效 |
| A23 | 订阅自动更新、玩家停不住旧版 | PKG-3#ASM-5(=B3) | 现行 [C] | 退役只能公告+窗口+替代路径 |
| A24 | Kiiro 实验只证生命周期与触发链、不证按种族隔离 | PKG-3#ASM-6 | 证据边界自限 [C] | — |
| A25 | 现状模型不足以表多种族域 | PKG-3#ASM-7 | 无实施回执 | `VoicePackSelectionRecord` schema=迁移边界 |
| A26 | 逻辑层可零 Ratkin 硬编码（机制通用、限制版本化） | PKG-3#ASM-8 | 未验证=拆分门 1 | — |
| A27 | pack 声明 `raceDefName` 即路由、HAR 同等 | PKG-3#ASM-9 | 未验证=拆分门 2 | HAR 失败只降级 xenotype 发现 |
| A28 | `SqueakLog` facade+srdiag v1 已被锁定 | PKG-3#ASM-11 | 现行 | 记 race/年龄须先设计新协议版本 |
| A29 | 跨程序集检测只能类型全名或标记接口 | PKG-3#ASM-12 | 至 Q2（compat）选定 | SR 全名稳定=S2 义务 |
| A30 | 两 DLL 同名类型 `Class=` 解析 first-wins | PKG-3#ASM-13 | 未验证 [C]/[I] | R3 靠两侧发布门断言 |
| A31 | 笔记=规划输入非合同 | PKG-3#ASM-15 | 现行 | 受影响结论先提升进合同 |
| A32 | US 基准不可复现（`0.4.x`@`c8794ff`+4 未提交改动） | PKG-3#ASM-16 | 记录在案 | E1–E12 可复核性受限 |
| A33 | CI 有网；离线/浮动版本隐式 restore 需网络、须 `-NoRestore` | PKG-4#ASM-1,2；PKG-5#ASM-9 | 现行；PKG-5#INC-10 实测污染链 | 以本地缓存为 `--source` 离线还原可恢复 |
| A34 | Steam file_size=自有格式，不与 zip 直比 | PKG-4#ASM-3 | — | — |
| A35 | 页面字段一致仅佐证「上线」 | PKG-4#ASM-4 | 固化为边界条款 | 二进制下载级验证从未执行（OQ-4#6） |
| A36 | PR squash 必然造成 main/dev 分叉 | PKG-4#ASM-5 | 0.2.4 复发验证 | 需 `merge -s ours` 对账 |
| A37 | 「常见 8000 字符」约束 | PKG-4#ASM-6 | 未验证 | 现值 2101/4741 远低于线 |
| A38 | auto-merged 文件不受 `checkout --ours` 控制 | PKG-4#ASM-7 | INC 实测 | runbook 条款（发布前核验最终 tree） |
| A39 | 包基线 0.2.x=115 / 0.3.0+=116 文件 | PKG-4#ASM-8 | 六实例一致 | 121=泄漏态 |
| A40 | 隐私扫描命中纪律文案自述；须分类 | PKG-4#ASM-9 | 实测 | 真实/未接受/known-debt 三分 |
| A41 | Steam 一切编辑=维护者人工 | PKG-4#ASM-10 | pre1 阻断即行使 | — |
| A42 | changelog 分钟=仓库口径 UTC+8、可能漂移 | PKG-4#ASM-11 | 印证 | 时间精度政策未裁（OQ-4#4） |
| A43 | `[claim]` 快照直接引用不转抄 | PKG-4#ASM-12 | 现行 | pre1 字段来源 [U] |
| A44 | 债务不含凭据、形态仅为目录名 | PKG-5#ASM-1 | Q1 变化即失效 | 三向量实测；PUBLIC/非 fork |
| A45 | `v0.2.0` 是唯一内容干净 tag | PKG-5#ASM-2 | 与「10 tag 全受影响」冲突 | PKG-5#CNF-4；10/9 未定 |
| A46 | 单人维护⇒重写窗口可控 | PKG-5#ASM-3 | 未验证 | — |
| A47 | 本地门约 11 秒⇒CI 加门可行 | PKG-5#ASM-4 | runner 未测 | 首跑即真验 |
| A48 | SR/US 差异是产品面的、不是门禁面的 | PKG-5#ASM-5 | — | — |
| A49 | US 09-06 最小仪式裁决可作 SR 基准 | PKG-5#ASM-6 | [C] 语料外 | — |
| A50 | 人工清单会漏、脚本不会 | PKG-5#ASM-7 | 实例支撑（陈旧包标记） | — |
| A51 | 债务只在旧版本、普通新提交无法消除 | PKG-5#ASM-8 | 实测 | 清除必须历史重写 |
| A52 | 文档复制债务串会自触门禁 | PKG-5#ASM-10 | — | 拼接构造 |
| A53 | `filter-branch` 内置可用 | PKG-5#ASM-11 | 未验证 | Q2（git-filter-repo 需联网安装） |
| A54 | 接受 GitHub 旧 SHA 残余风险 | PKG-5#ASM-12 | Q4 未决 | 彻底清除需 Support purge |
| A55 | 玩家喜欢 job 级连珠炮=招牌 | PKG-6#ASM-1 | 定案无重开 | 验收「父关=旧实现」 |
| A56 | Ludeon 不改 `ChewIngestible` debugName | PKG-6#ASM-2 | 软性；被 fail-open 中和 | 改名⇒子开关退化无操作 |
| A57 | `GainingNutritionNow` 语义=营养≠在嚼 | PKG-6#ASM-3 | 成立 | 上游改接口语义则失效 |
| A58 | public 面足够且零成本 | PKG-6#ASM-4 | 成立 | — |
| A59 | 两新字段默认 false+add-only 不 bump schema | PKG-6#ASM-5 | 成立（fixture 零 delta） | 父关强制子 false 三层归一 |
| A60 | 新 CI 接线推送后按预期运行 | PKG-6#ASM-6 | 未验证 | YAML 仅人工校对 |
| A61 | US 仓只读、SR 窗口零写入 | PKG-6#ASM-7 | 自述无反证 [C] | — |
| A62 | 负向自测在隔离一次性仓库执行 | PKG-6#ASM-8 | 自述 [C] | 与 INC-5#9 并存=PKG-5#CNF-6 |

## 5. 重大失败 / 成功与支撑证据（INC / EV 合并；处置细节与逐版本 src 指针回指各包 compact 同号条）

| 类别 | 现象 | 根因 | 结果 | 强度 | 来源 |
| --- | --- | --- | --- | --- | --- |
| 事故 | 0.2.1 首发布 zip 121 文件（6 个 `codemap.md` 递归复制+LoadFolders 门控回归） | stage 递归复制；三方 auto-merge 采纳 theirs（`checkout --ours` 无效） | 删 release/tag 重发；排除列表+读时断言；「发布前逐项核验最终 tree 关键文件」入合同 | 实测 | PKG-4#INC-6,7；PKG-4#TL-5,6 |
| 事故 | 换链后静音回归：`BuildFallback` 丢失内置表种子 | 未带种子+原 41 断言未覆盖失败面 [I] | C12 `0b0fa48` 恢复；断言 41→43；语料 3782 零 delta | 实测+[I]根因 | PKG-2#INC-1；PKG-2#EV-1,3 |
| 事故 | changelog 双语三方冲突两次复发（0.2.1→PR #15 再发） | squash 收尾未及时吸收 main 指针 | 收尾 `merge origin/main` 对账；树同则 `merge -s ours` | 两次代价 | PKG-4#INC-10；PKG-5 措施 3 |
| 事故 | 待推送历史曾含个人本地状态（形态；原文不复制） | 早期历史 | 原始历史仅本地未推送；dev 干净基点重建；立法三向量+完整可达范围审计 | 会话记录 | PKG-4#INC-1 |
| 事故 | 负向自测把探针提交写进真实仓库 | 自测未在一次性仓库 | reset+gc 清除、HEAD 未受影响；固化 LES「门自测必须一次性仓」 | [C/F 混合] | PKG-5#INC-9；PKG-5#CNF-6 |
| 事故 | 离线隐式 restore 覆写 `obj/project.assets.json`→MSB3644 | 浮动版本 `1.6.*` | 以本地缓存为 `--source` 离线还原；新门透传 `-NoRestore` 入 runbook | 实测（环境事实非门缺陷） | PKG-5#INC-10 |
| 预测（未发生） | F1/F2/F3：US 型 Ratkin 包或 legacy 桥落地→双 comp 双响/双冷却；SR 1.0 先行→Ratkin 静默 | 逃逸门类型不匹配；15/16 patch 同目标 | 顺序硬门 `U1→包/桥→1.0`；验收矩阵未跑 | 只读推演 [I] | PKG-3#INC-5；DEC-v2-9；OQ-3#20 |
| 设计陷阱 | Eat 静默 hole：父开时烟卷/薄片整个 ingest 零 Eat 事件（无异常无日志） | 营养权威=营养>0，零营养永不满足；动作统计看不出被谁替代 | 文案三要素+方案 B 子档+fail-open 回落 | 论证（实机未跑 [U]） | PKG-6#INC-1；DEC-v2-13 |
| 复核误判→更正 | 判「无公开 toil 权威」→核到 `CurToilString` public | 复核=时间点快照 | B 立项落地 | 更正链可核 | PKG-6#INC-3；PKG-6#CNF-3 |
| 遗留 | 0.3.0 旧置顶公告「仍在线」vs 文案源「如期删除」 | 外部人工编辑滞后+复核无留痕 | 一次带日期复采可关闭（OQ-4#3） | 页面观察+声称 | PKG-4#INC-13；PKG-4#CNF-5 |
| 遗留 | Steam 外部人工态：0.2.2 仅报告、0.2.3 上传未执行、二进制下载级验证从未闭环 | 渠道独立+人工观察纪律 | unverified 记账；页面级核验=agent 可触达上限 | 明示边界 | PKG-4#OQ-1,2,6；PKG-4#ASM-4 |
| 成功 | 0.3.0 八面全绿（A–H）+同日后置修复完成 | — | 门槛达成；「全绿≠无回归」入教训 | 实测+论证 | PKG-2#TL-6；PKG-2#EV-4..10 |
| 成功 | `v0.3.2-pre1` 首用 prerelease 通道：SemVer label、digest 一致、24 提交隐私 0 命中 | stage-package 原不支持 prerelease（INC 修复） | 证据完整；Steam 阻断不执行 | 机器+渠道内证 | PKG-4#EV-13,14；PKG-4#INC-12 |
| 成功 | 流程机械化：R1–R12 收敛为三命令读时门、7 项落地、负向自测 EXIT 1（「门不是橡皮图章」） | 冗余在「谁来做」非覆盖缺口（R4 除外） | DEC-v2-10/12；CI 接线推送后首跑未验 | 实测 [C] | PKG-5#EV-1..5；PKG-6#EV-3 |
| 成功 | Eat 两级开关落地：fixture 零 delta、单测锁定回落与常量、11/11 全绿 | — | 默认双关=旧实现逐位一致 | 实测 [C] | PKG-6#EV-1,2,5 |
| 成功 | legacy 桥离线原型（编译期证明+运行时语义四项） | — | 真 Verse XML 加载待实机 | 机器（离线） | PKG-2#EV-13 |
| 成功 | append-only 兑现：`Crying`=15、`Giggling`=16 追加、0–14 不变 | — | ABI 承诺有已兑现实例 | [F] | PKG-1#EV-2 |
| 成功 | staging 镜像校验不断言固定计数（示例音频可演化不破校验） | — | 15 目录/41 OGG 基线可变、违规须上报 | 论证 | PKG-1#EV-4；PKG-1#ANCH-10 |

## 6. 矛盾与未决问题（零丢弃：CNF 51/51、OQ 96/96）

### 6.1 矛盾（51 条全保留；按主题分组，每组保留双方要旨与时间层）

**同文档内部互斥（8）**：PKG-1#CNF-1 `pawn=<label>` 事件字段表 vs 通则禁写 pawn 标签（0.3.2 扩字段未回改通则；percent-encode≠匿名化）；PKG-1#CNF-2 tier 可发词汇三值 vs 产品四层链（折叠致 fallback 不可归因，归因损失是否已知取舍未记载）；PKG-1#CNF-3 日志协议状态戳 08-22 vs 内容含 0.3.2 事实（末次提交 08-23）；PKG-1#CNF-4 Eat 例外 settings schema「不 bump」vs 内部面 0.x 窗口（settings 面无冻结声明）；PKG-1#CNF-5 Remix 冻结三层形状 vs `Crying`/`Giggling` 缺内置层时抽样形状未定义。PKG-1#CNF-6 元规则「不得静默归一」vs 文档内部五处未消解（本包即首次登记）；PKG-1#CNF-7 套件产物落 `docs/` vs README「不提交」设计（隐私门从「默认不需要」变必须）；PKG-1#CNF-8 ABI 起点「first released version that carries the 0.3.1 ABI」不可判定（即使 OQ-1 已闭合仍保留）。——PKG-1 主源为合同文本，互斥均为同文档内部。

**0.3.x 交付与验收（8）**：PKG-2#CNF-1 派发「C1–C19」vs 语料止于 C14（编排方实测：可见 C1–C10、C12–C14+PKG-4 载体 C17–C19/C34–C40，缺席 C11/C15/C16/C20–C33，禁止写连续 C1–C40）；PKG-2#CNF-2 C11 全文无号（`5b51c6a`/`67028b8` 未编号两解）；PKG-2#CNF-3 语料 1622 vs 3782 例（同 corpus 两例数，扩容非错误勿合并）；PKG-2#CNF-4 交接「GitHub prerelease→Steam」vs 08-20「不设 prerelease tag、本地 dev 包」（预写被否定）；PKG-2#CNF-5 动作门窗口 0.4.x vs 08-22 修订「US 0.3.x 并行、0.4 随首版」（口径演化）；PKG-2#CNF-6 IsEgg 内部标签 vs 0.3.2 公开 ABI（有记录的废止）；PKG-2#CNF-7 交接「可能未提交/未推送」vs 检查表「origin/0.3.x 已推送」（时间层）；PKG-2#CNF-8 热修 `0.3.0.x` vs `vX.Y.Z-hotfixN`（定案为准）。

**双仓路线与拆分（10）**：PKG-3#CNF-1 Q1（US 是否服务 Ratkin）US 仓规则文档「0.4 不允许 Ratkin 装配」vs 维护者「Ratkin 已是 US 首发支持包之一」——冲突一方在语料外，**保留不归一**；PKG-3#CNF-2 note「不存在双实现共存窗口」vs 08-22 修订「0.4 双仓同步上架、SR 1.0 才收缩」（共存窗口=0.4→1.0 有意设计）；PKG-3#CNF-3 阶段表体「0.4.x 首版拆分发布」vs 修订注 SR 1.0 收缩（以注代改、引用必须同时引修订注）；PKG-3#CNF-4 「四层限定」vs 实际 7+ 条且编号断裂（下游不得把「四层」当可核验计数，替代=filter 三处入口）；PKG-3#CNF-5 compat 前言 08-23 vs E8 授权 08-24（文档后追加过或日期不可靠）；PKG-3#CNF-6 compat F4「修复方向无法定」vs mig「修复必须落 US」（层次不同：Ratkin 内容侧归属 vs 修复落点，可并存）；PKG-3#CNF-7 U1 过渡期修复 vs legacy 长期在线则永久必需（条件化非推翻）；PKG-3#CNF-8 反转：「无法继承 packageId」不是必然而是方案 A 前提造成的→packageId 跟内容走（隐含 Workshop 原地升级可行、平台层未取证）；PKG-3#CNF-9 compat Q1–Q4 ≠ mig Q1'–Q10'（共用 Q 前缀指代不同，**转述必须带文档限定**）；PKG-3#CNF-10 `SqueakBuiltInFallbackCatalog`（规划示例名）vs `BuiltInFallbackCatalog`（合同逐字名）——权威序合同>规划，若必须取一建议合同名，冲突记录保留、主源记录不得改写。

**发布渠道与外部状态（11）**：PKG-4#CNF-1 CHANGELOG 规则「旧前新后、未发布置顶」vs 正文实序 0.3.0→0.2.0 逆序（终裁=维护者改规则或重排）；PKG-4#CNF-2 0.2.0 前身词表 vs runbook 词表（细化提取、历史层保留）；PKG-4#CNF-3 Claim Pack 模板行集 vs 六实例偏差谱（0.2.1 缺字节、0.3.0 改行名、LoadFolders 行消失 [I]≈读时门接管）；PKG-4#CNF-4 GitHub UTC+8 时间 vs 页面 Updated 时区对不齐（[I]≈UTC-7；跨渠道时间账须标时钟）；PKG-4#CNF-5 0.3.0 当日「公告仍在线待删」vs 文案源「已于 0.3.0 如期删除」（后写口径不代表前段不存在、删除复核无独立留痕）；PKG-4#CNF-6 pre1 待办预设「正式 0.3.2 会发生」vs Notes「0.3.2 仅 prerelease、工作随 0.3.3」（后写=现行；0.3.3 是否继承 pre1 证据开放）；PKG-4#CNF-7 Change Notes 3→6 vs 有记录上传仅两次（+3 不匹配、或曾上传或未记录条目）；PKG-4#CNF-8 隐私行三称法（0 命中/0 真实命中/0 未接受命中——[I] 同义=台账外真实命中为零）；PKG-4#CNF-9 ZH/EN 版式漂移（缺空行、非口径冲突）；PKG-4#CNF-10 「0.3.0 失效公告」两所指（置顶失效预告 vs 拆分预告）不违规但依赖 U-7 快照；PKG-4#CNF-11 `merge -s ours` 只在真实分叉时用 vs squash 收尾本身制造分叉（[I] 主动对账即分叉第一时刻）。

**流程与隐私（14）**：PKG-5#CNF-1 「待裁决 3 项」vs「V1–V3 定案」vs「§1 未回写」（以 §7 为最新口径但不得删 §1 行）；PKG-5#CNF-2 前言「门禁覆盖没有缺失」vs R4「覆盖缺口（不是冗余）」（R4=身份面，并列保留）；PKG-5#CNF-3 227 revision vs 230 提交（差 3 无口径注记、不自行调查）；PKG-5#CNF-4 「10 tag 全部指向受影响提交」vs「`v0.2.0` 唯一内容干净」（10 vs 9 并列不择一，下游不得把 10 当受影响数）；PKG-5#CNF-5 隐私债务 1→5 文件反转（旧口径单独保留，解释 V2 为何重估）；PKG-5#CNF-6 「隔离的一次性仓库」vs「真实仓库被写脏」（措辞疑为事故后补救 [I]）；PKG-5#CNF-7 前言「已落地」vs 表头担保前 3 项+措施 8「待实现」（以逐行标注为准）；PKG-5#CNF-8 措施 8 属人工心智清单却未被 R1–R12 覆盖（需语义判断非机械可判定）；PKG-6#CNF-1 08-23「未提交未推送 0 提交」vs 09-13「本地三块已提交停在推送前」（分层写作，现行=09-13 层）；PKG-6#CNF-2 E§7 前言「已实施单开关」vs 同日两级已实施（较早草稿层残留）；PKG-6#CNF-3 注 1「无公开 toil 权威」vs §2.3 `CurToilString` public（复核结论被推翻，更正=B 立项前提）；PKG-6#CNF-4 「跳过」定案 vs §5/§9.2 同项列开放（定案=工作口径、开放=终局确认门，两态并存勿混引）；PKG-6#CNF-5 「known-debt 5 文件、范围比先前记录更大」vs「先前记录」数值未载（两包数字互为校验）；PKG-6#CNF-6 「三份新文档均无隐私串」vs untracked 4 个（三份排除交接正文，非实质矛盾防误报）。

**PKG-4 自身 11 条独立登记（已并入 6.1 组 4 与组 5 表述）**。

### 6.2 OQ（96 条全保留；按主题分组，id 随主题就近）

**0.3.3 发布线（PKG-4 9 行 + PKG-6 5 + PKG-2 1）**：
| OQ | 问题 | 状态 |
| --- | --- | --- |
| PKG-6#OQ-1 | 0.3.2 正式版是否**确认**跳过（口径=跳过、工作并入 0.3.3） | 口径不变、终局裁决开放 |
| PKG-6#OQ-2 | Steam 是否恢复发布（0.3.2 从未上 Steam、阻断中） | 开放，需授权+人工编辑面 |
| PKG-6#OQ-3 | 0.3.3 GitHub full vs prerelease 先行 | 开放 |
| PKG-6#OQ-4 | 发布裁决 V1–V3 与推送仪式绑定 | 开放；内容本体 [xref: PKG-5] |
| PKG-6#OQ-5 | 完整推送链 push dev→PR→CI→squash→tag `v0.3.3`→资产核验→CHANGELOG 换时间→Claim Pack→渠道核验 | 停在远端推送前（冻结时点） |
| PKG-4#OQ-6 | Steam 二进制下载级验证从未执行=全部 Workshop 格上限 | 需授权+人工 |
| PKG-4#OQ-7 | 0.3.3 无发布面（无 Claim Pack、文案锚点 0.3.0、一次核对未做） | 开放 |
| PKG-4#OQ-9 | `vX.Y.Z-hotfixN` 与 `archive/` 归档法零实例未检验 | 常设 |
| PKG-4#OQ-11 | 「发布裁决」无规定留痕格式（pre1 头注为孤例） | 可审计性风险 |
| PKG-4#OQ-12 | `release-<version>-review-zh.md` 命名无 prerelease/hotfix 规范 | pre1 先例未收编 |
| PKG-4#OQ-13 | 「0.3.0 失效公告」短语无原文快照 | 影响对外承诺链 |
| PKG-4#OQ-4 | changelog 时间精度政策未裁决 | 开放 |
| PKG-4#OQ-8 | CI Node20 警告是否消除无后续记录 | 开放 |
| PKG-4#OQ-10 | known-debt 5 条清偿依赖授权 | 方案 [xref: PKG-5] |
| PKG-2#OQ-1 | merge `0.3.x→dev`+渠道发布授权（检查表时点阻塞） | PKG-4 已闭合 0.3.0 已发布；时层并存 |

**身份/迁移/US（PKG-3 16 行 + PKG-6 1 行交叉）**：
| OQ | 问题 | 状态 |
| --- | --- | --- |
| PKG-3#OQ-1 | **Q1（compat §6）**：US 是否服务 Ratkin？ | **阻塞**；F1 修复方向、S1 条款、DEC-20/21/22/23 |
| PKG-3#OQ-2 | Q2（compat）：跨程序集检测=类型全名 vs 标记接口 | U1 形态、依赖时序 |
| PKG-3#OQ-3 | Q3（compat）：桥上线重叠期、谁拥有 `SqueakyRatkin.SqueakVoicePackDef` | R3/S4/U3 |
| PKG-3#OQ-4 | Q4（compat）：0.4 双开是否需 Domain 级跳过（报告自陈未覆盖选择层） | 选择层双响缺口 |
| PKG-3#OQ-5 | S1「Ratkin 装配唯一写者」是否已写入 SR 合同与 MEMORY | 语料外回查 |
| PKG-3#OQ-6 | U1–U4 转述/落地进度、P0 退出条件是否达成 | P1–P3 全阻塞在 U1 后 |
| PKG-3#OQ-7 | Q1'（mig）：是否插 SR 0.5 过渡版 | 版本序列、P2/P3 边界 |
| PKG-3#OQ-8 | Q2'：是否另开冻结 legacy Workshop 条目 | DEC-25/27、R6 维护预算 |
| PKG-3#OQ-9 | Q3'：P2 设置导入做不做（US 反射读 SR 设置） | R4（调音丢失）、P2 退出 |
| PKG-3#OQ-10 | Q4'：公告窗口长度（建议 4–8 周） | P3 需前置 ≥1 完整窗口 |
| PKG-3#OQ-11 | Q5'：1.0 的 US 前置硬还是软（按 B2 均不崩档、差别=警告可见性） | F5/R2/P3 退出 |
| PKG-3#OQ-12 | Q6'：legacy 独立线跨版本维护预算（承诺 1.7？） | R6 |
| PKG-3#OQ-13 | Q7'：id 归属 B（主线继承，推荐）还是 A（legacy 保留） | **阻塞**；PackKey 重置、设置跟随 |
| PKG-3#Q8' | 是否接受迁移玩家音源选择被重置一次（A 的代价；若否 B 是唯一选择） | Q7' 硬蕴含 |
| PKG-3#OQ-15 | Q9'：legacy 上 Workshop 长期并行 vs 仅 GitHub 归档 | S2/U1 永久化、R3/R6 |
| PKG-3#OQ-16 | Q10'：新 pack-only 线用 US 原生类型还是 SR 旧类型 | DEC-28、作者指南 R5 |
| PKG-3#OQ-17..20 | 交接风险三则+未验证门：无实施回执、US 基准不可复现、US 规则文档单向不可达、双开实机验收矩阵五场景未跑 | GAP-7/GAP-2/GAP-3；矩阵=F1 修复后 P1/P3 退出 |
| PKG-6#OQ-6..9 | 迁移/身份 Q1–Q10 逐项分派；身份归属推荐（packageId 跟内容走）未裁决；US Q1–Q4；U1–U4 实施进度 | 条文 [xref: PKG-3]；GAP-6 |
| PKG-2#OQ-2..5 | US 并行四问：建仓需授权（08-22 暂不建仓、语料内未建）；Workshop 显示名/许可待确认；US 在 SR 1.0.0 时点版本号待定；0.3.x 与 US 发布顺序暂不决定+ABI「首个携带」浮动表述 | 全部开放；PKG-4 已闭合版本序列 |
| PKG-2#OQ-7,9,10,11 | 6 条 no_sound 无根因关闭（本包质疑保留）；双开共存验证未到执行窗口；身份门控矩阵余项「自然覆盖」/全文在 TODO；C15–C19 与 C11 缺号延伸未知 | 未验证/编排；PKG-2#CNF-1、G10 承接 |

**Eat 与日志可观测性（PKG-1 13 + PKG-6 8 行）**：
| OQ | 问题 | 状态 |
| --- | --- | --- |
| PKG-1#OQ-1 | 「首个承载 0.3.1 ABI 的发行版本」=哪一版（已由 PKG-4 证据闭合：0.3.1 无正式版、0.3.2 仅 pre1）；PKG-1#CNF-8 保留不删 | 已闭合、CNF 保留 |
| PKG-1#OQ-2 | `pawn=<label>` 是否被接受为日志常态字段 | 未决（PKG-1#CNF-1） |
| PKG-1#OQ-3 | 是否拆细 tier 区分 pack_fallback/built_in_fallback | 未决（PKG-1#CNF-2） |
| PKG-1#OQ-4 | settings schema 是否承诺永不 bump | 未决（PKG-1#CNF-4） |
| PKG-1#OQ-5 | `Crying`/`Giggling` Remix 缺层抽样形状 | 未决（PKG-1#CNF-5） |
| PKG-1#OQ-6 | 未来 action gates 与内置动作键 append-only 的边界 | 未决 |
| PKG-1#OQ-7 | map-lifecycle reset 未实现是缺陷还是有意；once-key 1024 清空噪音 | 未决 |
| PKG-1#OQ-8 | orphan 数量/保留期上限 | 未决 |
| PKG-1#OQ-9 | profile 副本「当前版本」判据 | 未决 |
| PKG-1#OQ-10 | 实现权威路径清单是否覆盖 1.6/Assemblies 之外 flavor | 未决 |
| PKG-1#OQ-11 | `prompts/**` 允许面、产物落 docs 后 README §10 三选一处置 | 需授权/编排方 |
| PKG-1#OQ-12 | 三份合同无签名/版本/生效日期——是否补版本头 | 未决 |
| PKG-1#OQ-22（跨包 PKG-6#OQ-22） | Eat 粒度已裁决不新增日志事件；替代 Dev 诊断面板是否推进 | **仍开**（面板未实施） |
| PKG-6#OQ-10 | Eat 三态手工验收矩阵未实机（meal/烟卷/薄片/啤酒/仙馔/营养膏/背包吃/动物/尸体 ×三态） | 未验证 |
| PKG-6#OQ-19 | 是否泛化其他动作粒度（Sleep/Work/Social/Joy 同类问题） | 0.4/US |
| PKG-6#OQ-20 | 粒度偏好放策略层还是注册表层 | DEC-4 条 8 已给 SR 倾向 |
| PKG-6#OQ-21 | 默认值是否写进兼容政策（父关=job 级不允许再翻） | 未决 |
| PKG-6#OQ-22 | Dev 诊断面板（显示当前模式与 `chewToilNameConfirmed`）是否实施 | 未实施 |
| PKG-6#OQ-23 | 是否加 `ThingDef.IsDrug` 排除（啤酒/仙馔父开子关时静默） | 悬置备选（挂 OQ-17） |
| PKG-6#OQ-15..18 | （对接收方 US）两级 vs 3 态卡；子项命名覆盖面；啤酒/仙馔仍响接受与否；回落口径未确认⇒宽松 vs 静默 | SR 已定案、外部开放 |
| PKG-6#OQ-11..14 | US 双开矩阵未实机（四组合「恰好一条声音」+跳过记录）；CI 接线推送前无法实跑（YAML 仅人工校对）；历史/tag 重写 5 `[known-debt]` 需按 5 文件重估+授权；Steam 文案/包核验随解除阻断执行 | 未验证/需授权；矩阵 [xref: PKG-3]、方案 [xref: PKG-5] |

**发布流程与隐私（PKG-4 余 4 + PKG-5 16）**：
| OQ | 问题 | 状态 |
| --- | --- | --- |
| PKG-4#OQ-1 | 0.2.2 Workshop 永停「维护者报告已发布」 | 页面历史不可回采则按 [U] 记账 |
| PKG-4#OQ-2 | 0.2.3 是否实际上传过 | 联动 PKG-4#CNF-7 |
| PKG-4#OQ-3 | 置顶公告删除无带日期复核 | PKG-4#CNF-5 一次复采可关闭 |
| PKG-4#OQ-5 | pack-github 基点校验无记录 | I-7 |
| PKG-5#OQ-1 | 0.2.4 是否重发（不含 version.txt） | 维护者定 |
| PKG-5#OQ-2 | 措施 8（Comments→TODO）是否落地 | 待实现 |
| PKG-5#OQ-3 | CI 加门+SDK 10.0.x 首跑（「已落地」≠「已运行」） | 推送后首跑即真验 |
| PKG-5#OQ-4..8 | 隐私 Q1–Q5：是否执行方案 B；filter-repo vs filter-branch；占位符；GitHub 旧 SHA 残余风险；跨仓通知 | **全未决⇒授权缺口开放** |
| PKG-5#OQ-9 | SR 无 mirror 备份=方案 B P0 阻塞 | 动手前必须先建 |
| PKG-5#OQ-10 | tag 口径（10 还是 9） | PKG-5#CNF-4 并列 |
| PKG-5#OQ-11 | 54 处 hash 引用无逐条清单 | P6 第 12 步不可验收 |
| PKG-5#OQ-12 | force-push 顺序待确认 | 方案 B 内 |
| PKG-5#OQ-13 | 推送窗口协调（重写期间禁止推送） | 单人维护风险可控 [U] |
| PKG-5#OQ-14 | 门实现细节不在 docs/** | GAP-5#1 |
| PKG-5#OQ-15 | 交接序列=两门→本地提交→停在推送前 | 冻结口径 |
| PKG-5#OQ-16 | §1 回写未完成 | 待裁决回填 |

### 6.3 认识论交叉分账要点（C/I/U 非台账 id，只列跨包影响判断的项）

- **C 面依赖语料外**：UniversalSqueaker 09-06 最小仪式裁决（PKG-4#C-7/PKG-5#ASM-6）；US 仓规则文档现行文本（PKG-3#PKG-3#CNF-1 一方）；维护者「Ratkin 已是 US 首发支持包」口径；Kiiro「已证明」自述；Eat 反馈原文；分支保护声称；0.2.0 自评 8/8/4（一次性）。
- **I 面高影响**：PKG-3#I-3 F1/F2/F3 是预测非已观察事故（不得写进已发生清单）；PKG-1#I-1 「机械判定违规」≠「自动发现违规」（合同无检查门）；PKG-2#I-1 tier 折叠可能是有意命名（`vanilla` 语义边界未定义）；PKG-5#I-2 tag 并集 9；PKG-6#I-3 两块 commit 建议被吸收扩展为三块。
- **U 面硬缺口**：六门通过条数无记录（PKG-3#GAP-7）；Eat/US 双开矩阵零实机；0.2.2/0.2.3 Workshop 态；0.1.x 无任何证据级；tag 10/9；US 基准不可复现；三块提交 hash 未记（`git log` 可恢复，v1 禁取证）。

## 7. 候选教训（tentative 明示；合并表达，支撑证据回指）

| # | 教训 | 强度 | 适用边界 | 支撑 |
| --- | --- | --- | --- | --- |
| L1 | 闭合枚举+固定字段序+fail-closed 让兼容面可机械判定违规 | 强 | 仅对外可解析产物；内部面保留 0.x 自由 | PKG-1#LES-1,7；DEC-v2-1/3 |
| L2 | 规范文档自我声明证据边界，防下游把条文升格为验证结论 | 强 | source review 型规范 | PKG-1#LES-2, ASM-10 |
| L3 | 冻结声明必须绑定可指认版本锚点；描述性锚造成起点不可判定 | 中等（单点观察） | 「自某版本起稳定」类承诺 | PKG-1#LES-3；PKG-1#CNF-8 |
| L4 | 追加新事实不更新状态行→时间层失效 | 中等 | 多会话追加式文档 | PKG-1#LES-4；PKG-1#CNF-3；PKG-6#LES-6 |
| L5 | 通则式禁令不与 schema 扩展联动复核→与自家字段表互斥 | 强 | 机读协议含「不得写入 X」通则 | PKG-1#LES-5；PKG-1#CNF-1 |
| L6 | **tentative**：产品层折叠进可观测层→兼容承诺成立但排障静默下降；应在折叠处声明「哪个产品层不可归因」 | tentative（单例） | tier/枚举映射类设计 | PKG-1#LES-6；PKG-1#CNF-2 |
| L7 | 「全部门槛绿」≠无回归：换链后同日仍需后置修复；失败路径与主链路同期配断言 | 强 | 实现替换型变更验收 | PKG-2#LES-1；INC-v2#§5 换链行 |
| L8 | 分布等价≠逐次对拍：必须写成诚实声明并给出玩家承诺 | 强 | 随机源输入的重构 | PKG-2#LES-2；A13 |
| L9 | 新机制先取最小面（彩蛋去运算符；动作门只做两件零成本事） | 中 | 面向未来生态的机制预建 | PKG-2#LES-3 |
| L10 | 门槛按证据强度分级并显式写「未做」 | 中 | 检查表记法 | PKG-2#LES-4 |
| L11 | 落选方案的「被吸收硬细节」单列是高价值资产 | 中 | 多方案比拼记录 | PKG-2#LES-6；DEC-v2-4 |
| L12 | 有存量订阅者时兼容修复落「零玩家一侧」；两侧改动同影响玩家时顺序写成硬门+倒置后果+发布门断言 | 强 | 双仓退役窗口 | PKG-3#LES-1,2；DEC-v2-9 |
| L13 | 不复制上游判定数据（原版 minAge）；查运行时 defName+表外保守默认 | 强 | 数据面依赖上游 | PKG-3#LES-3；A8 |
| L14 | 「机制通用+限制版本化」：限制集中白名单 filter 三处入口，拆分时整体移除 | 中（未经实施验证） | 拆分边界 | PKG-3#LES-4；DEC-v2-8 |
| L15 | 身份标识跟「内容/订阅」不跟「代码分支」；先列运行时绑定面再判可否继承 | 强 | 平台迁移 | PKG-3#LES-5；PKG-3#CNF-8 |
| L16 | 只读审查定位根因、不能证明修复有效：验收绑定实测+日志证据 | 中 | 兼容性审查 | PKG-3#LES-8；OQ-3#17..20 |
| L17 | 渠道互独立；人工外部态必须人工观察，未知即 unverified | 强 | 发布渠道 | PKG-4#LES-1,11 |
| L18 | 推送审计覆盖完整新可达 range；三向量结论不互推 | 强 | 隐私门设计 | PKG-4#LES-2 |
| L19 | 真实 secret 先撤销/轮换再清理；清理≠轮换 | 中（预案） | secret 次序 | PKG-4#LES-3 |
| L20 | 事故恢复特殊操作不自动固化为日常流程 | 强 | 流程规范 | PKG-4#LES-4 |
| L21 | `checkout --ours` 管不住 auto-merged；发布前逐项核验最终 tree 关键行为文件 | 强 | git 合并 | PKG-4#LES-5；A38 |
| L22 | 公开产物=exact clean release commit；dirty 产物仅测试证据 | 强 | 打包 | PKG-4#LES-6 |
| L23 | 能机械判定的一律从人工清单搬进脚本断言；删重复表述不是删断言 | 强 | 门禁设计 | PKG-4#LES-7；PKG-5#LES-5 |
| L24 | 测试部署=脚本产物+flavor 对口，禁止手搓 | 中（单事件） | 测试部署 | PKG-4#LES-8 |
| L25 | squash 收尾后 dev 尽快 `merge -s ours` 吸收 main 指针 | 中（两次代价） | 分支管理 | PKG-4#LES-9 |
| L26 | 对外文案集中维护+一次变更一次核对；页面态永不由仓库态推断 | 强 | Workshop 文案 | PKG-4#LES-11 |
| L27 | 跨渠道时间账显式标时钟 | tentative | 跨渠道证据 | PKG-4#LES-13；PKG-4#CNF-4 |
| L28 | 模板=实例先行、事后收编为最小公共集；实例增量字段不回改 | tentative | 模板演进 | PKG-4#LES-14 |
| L29 | 重复人工表述就是冗余源：转单一读时门+三步仪式+CI；落地后禁止重新加回人工仪式 | 中 | 发布流程域 | PKG-5#LES-5,11；PKG-6#LES-7 |
| L30 | 门的自测必须在一次性仓库里做 | 强（单事件不升格） | 门禁自测 | PKG-5#LES-6 |
| L31 | known-debt 台账按「向量+精确路径」匹配，否则掩盖新泄漏 | 中 | 隐私门 | PKG-5#LES-7 |
| L32 | 浮动版本+无网络+隐式 restore 污染构建态；缓解=透传 `-NoRestore` | 强（实测） | 离线构建 | PKG-5#LES-9；A33 |
| L33 | 文档自身不得复制债务串 | 强 | 隐私写法 | PKG-5#LES-10 |
| L34 | 「静默的归属变更」最难排查：开关类功能必须把行为变化映射到用户可感/可查信号 | 强 | 采样/过滤层开关 | PKG-6#LES-1 |
| L35 | 已发布手感/默认值=兼容资产：「默认收窄=删招牌行为」 | 强 | 玩家可感默认行为 | PKG-6#LES-2；DEC-v2-13 |
| L36 | fail-safe 回落方向选「退回宽松旧行为」而非「退回严格判定（静默）」 | 中 | fail-safe 择向 | PKG-6#LES-3；DEC-v2-13 |
| L37 | 对抗复核结论=时间点快照；「无解」判据可被后续核实推翻 | 中（单例可核） | API 面判据复核 | PKG-6#LES-4；PKG-6#CNF-3 |
| L38 | 同一交接文件追加接续记录→内部时间层矛盾；引用 handoff 必须带段落时层 | 中 | handoff 体裁 | PKG-6#LES-6；PKG-6#CNF-1 |

## 8. 证据缺口（GAP 51 条合并为 19 主题行；原 id 全列）

| # | 问题 | 为什么答不了 | 需要什么 | 原 id |
| --- | --- | --- | --- | --- |
| G1 | 三份合同条文与实现是否逐条一致 | 合同自述 source review 非实测 | 实现-合同对照或 harness 清单 | PKG-1#GAP-1 |
| G2 | `audio.route.selected` 真实输出行形状 | 只有 schema 无样例（禁复制日志摘录） | 已净化样例或机读解析测试 | PKG-1#GAP-2 |
| G3 | 0.3.2 是否正式发行、渠道 | 发布事实不在主源 | PKG-4 渠道矩阵（已闭合 OQ-1#1） | PKG-1#GAP-3 |
| G4 | 诊断叠加层是否暴露真实 fallback 层 | 合同只规定折叠 | 读 overlay 规范/代码 | PKG-1#GAP-4 |
| G5 | Eat 三级模式回归测试守护标识 | 主源只声明要求 | 测试清单或 CI 记录 | PKG-1#GAP-5 |
| G6 | `Toddler` 桶可发行路径 | 合同只记 1.6 无原生 | 跨 mod 兼容评估 | PKG-1#GAP-6；PKG-3#GAP 部分 |
| G7 | settings 是否需版本号/世代字段 | 合同未定义 | settings 持久化规范补充 | PKG-1#GAP-7 |
| G8 | 四份并行架构草稿全文 | 只留比拼表 | 草稿存档 | PKG-2#GAP-1 |
| G9 | 0.3.1/0.3.2 是否按计划过门；0.3.1 实施记录 | PKG-2 止于计划+0.3.0 门槛 | PKG-6 接续+PKG-4 发布评审 | PKG-2#GAP-2；PKG-4#U-5 |
| G10 | C15–C19/C11 缺号真相 | 语料缺席 | 编排方实测已部分闭合（C17–C19/C34–C40 在 PKG-4；C15/C16/C20–C33 仍缺席） | PKG-2#GAP-3 |
| G11 | fixture 9 场景 vs S1-S5+F03-F07 对齐 | 无对照表 | Scenarios.cs | PKG-2#GAP-4 |
| G12 | INC-1 引入提交与暴露面 | 只记修复不记引入 | git log（v1 禁） | PKG-2#GAP-5 |
| G13 | F1/F2/F3 是否实机观察到；选择层是否双服务；CNF-1 US 仓规则现行文本；六门通过条数；U1/桥落地；srdiag v2 冻结；「四层」所指；schema↔US 模型映射；年龄 ABI 冻结记录；「决策文档 §5」身份；Workshop 原地升级平台可行性 | 全为只读审查+推演或语料外 | 双开实机矩阵+skip 日志；PKG-1/2/4 对照；授权读取 US 仓 | PKG-3#GAP-1,2,3,4,5,6,7,8,9,10,11 |
| G14 | 0.1.x tag/zip/哈希/首传包身份 | 无 Claim Pack 体系 | 外部归档 | PKG-4#GAP-1；PKG-4#U-4 |
| G15 | Change Notes 6 条实际内容；0.2.2/0.2.3 Workshop 真实态；Steam 页面时区 | 页面历史不可回采 | 带日期页面复采+三对照 | PKG-4#GAP-2,3,4 |
| G16 | CHANGELOG 排序终裁；8000 字符出处；0.2.0 artifact manifest 所指；item ID 数值全文档分布 | 文本语料内无裁决；分布=隐私规则有意不可答 | 维护者裁决/官方文档/当次会话记录；隐私审计定位（已定位 5 处） | PKG-4#GAP-5,8,9,10 |
| G17 | UniversalSqueaker 09-06 裁决原文；0.3.3 是否沿用 pre1 证据 | 语料外 | 授权读取；未来发布会话 | PKG-4#GAP-6,7 |
| G18 | 门实现细节（断言集合）；5 处债务形态明细；tag 枚举；227/230；54 处位置；CI 首跑；runbook 第 15 条；triage §3；0.2.1 教训 (d) 原件；mirror 与 archive 分支；陈旧包清理 | 主源无载/维护者本地 | 脚本对照/授权/后续会话 | PKG-5#GAP-1..11 |
| G19 | Eat 三态/US 双开矩阵实机记录；0.3.2 跳过终局裁决；隐私债务先前口径；Q1–Q10/Q1–Q4/V1–V3 逐项内容；Steam 评论原文；US 侧进度；三块提交标识 | 语料外/待会话 | 实机会话记录；发布/决策会话；`git log` | PKG-6#GAP-1..7 |

## 9. 锚点（ANCH 合并组号；组内原 id 标注；数值不得增删改）

> 逐字保留面按族合并；完整展开见各包 compact §9（来源归 §10）。本节只列**跨包共享**或**高争议**锚点；各包专属锚点在其 compact 原位有效、通过 `PKG-n#ANCH-*` 回指。

**ANCH-A 产品身份与动作（PKG-1#ANCH-1,2,3,4,10）**：品牌 `鼠辈啁啾`/`Squeaky Ratkin`；packageId `coahuilite.squeakyratkin`；C# `SqueakyRatkin`；只服务 NewRatkinPlus Ratkin；17 动作 append-only 序号 0–16（`Call, Eat, Sleep, Wounded, Select, Move, Social, Joy, Death, Draft, Undraft, Attack, Work, Equip, MentalBreak, Crying(=15), Giggling(=16)`）；内置 fallback Ratkin **15 键**（缺 Crying/Giggling）；`ageTag` ∈ {`Baby`,`Toddler`,`Child`,`Adult`}（Toddler 无 1.6 原生）；周期方式 ∈ {`EachTime`,`RandomOneShot`,`External`}；`weight` 默认 1；`allowEasterEggSounds` 默认 false；啤酒 `Nutrition 0.08`、ambrosia `0.2`；出厂默认自 0.2.3=Fallback+内置 Race Example 启用；Example 基线 15 动作目录/41 OGG（可变不断言固定总数）。

**ANCH-B XML ABI 冻结面（PKG-1#DEC-14/ANCH-1）**：`SqueakVoicePackDef` XML：`raceDefName`（必填、精确大小写敏感、缺失拒绝）、`scope`、`targetDefName`、`weight`（正有限）、`fallbacks`、`actions(action,ageTag,IsEgg,sounds)`+`SR_*` SoundDef；public-stable 自「first released version that carries the 0.3.1 ABI」（起点未闭合→PKG-1#CNF-8）；字段 add-only、动作键 append-only、fail-closed；未冻结=kernel/domain model/fallback profile schema/未来 action gates；作者资源根 `<lowercase packageId>/<PackDef.defName>/<Action>/`；PackKey=package ID+PackDef defName。

**ANCH-C 日志协议字段序（PKG-1#ANCH-5,6,8）**：v1 核心=`fmt lvl vis evt action target pack build build_id`；v2 核心=`fmt lvl vis evt action target pack race [xenotype] build build_id`；v1 事件专属固定序=`reason sound source count dispatched suppressed_detail enabled ex_type ex_inner ex_site ex_msg`；v2 事件级——`audio.route.selected`=`sound tier egg suppressed_detail pawn pawn_id pawn_faction pawn_ctrl`；`fallback.profile.store_failed`=`ex_type ex_inner ex_site ex_msg`；`settings.origin`=`settings_origin=FreshCreated|LoadedFromFile`；percent-encode 白名单=`A-Z a-z 0-9 . _ ~ : / @ + -`；v1 锁定 28 事件 byte-immutable；v2 扩展 4 事件；once-key 上限 1024；每动作细节每 5 秒一条；`trigger.outcome.summary` 每 60 秒；异常截断 256 字符；tier 可发词汇仅 `xenotype_pack`/`race_pack`/`vanilla`/`-`。

**ANCH-D 内核与选择链类型（PKG-2#ANCH-2,3）**：`RaceKey/XenotypeKey/AudioDomain/DomainPool/SqueakPoolRegistry/SelectionContext/SelectionMode{Off,Fallback,Remix}/ChainResult{SoundKey,Tier,PoolStableKey,IsEgg}/ChainTier{XenotypePack,RacePack,PackFallback,BuiltInFallback}/ISoundGate/IRollSource/Modulation.ComposeModulation/FallbackProfile/FallbackDelta/CopyDisposition/FallbackProfileOperations/BuiltInFallbackCatalog/BuiltInFallbackTable/DomainFilter(0.4.x 删)`；适配层 `SqueakFallbackProfileStore/SqueakLifeStageResolver/SqueakRuntimeResolver/SqueakXenotypeCatalog(类名保留)/SqueakKernelAdapter(唯一接缝)/GetTargetCandidates/NotifyExternal`；动作门 `SqueakCompat.NotifyAction(Pawn, string actionKey)/SqueakActionDef/SqueakActionRegistry/SqueakGlobalActionPolicy/allowExternalActions(默认开)/minIntervalTicks≥60/八道闸门`；Eat `SqueakEatOccurrence{Mode{WholeJob,GainingNutrition,ChewingToil}; ChewingToilDebugName="ChewIngestible"; ResolveMode; AllowsOccurrence}`；身份门 `SqueakTriggerInvocation/IsPlayerInitiated/RequiresResponsivePawn/Pawn.IsPlayerControlled/!Downed && Awake()/Selector.Select(playSound:false)`。

**ANCH-E 事件名与证据数字（PKG-2#ANCH-4,7）**：`audio.dispatch.ok/audio.route.selected/trigger.outcome.summary/trigger.attempt.failed/audio.dispatch.no_sound/pack.race_defaulted/mod.start.identity/mod.start.ready/rebuild_failed/refresh_failed/PackRejected/TargetRejected/hook.*unavailable/devtools.camera_indicator.changed`；语料 1622（C4）→3782（C7）；断言 41→43；fixture 9 场景；14 行对照；换面 4/不换面 9+1/dev 豁免 3+七次点击；350 ms；实机 87 派发、count=37、summary 13、峰值 52、no_sound 6、egg 5/11、player 10/nonplayer 26。

**ANCH-F 提交链（PKG-2#ANCH-8；PKG-4 §9；PKG-5#ANCH-2）**：PKG-2 段 C1 `41ca953`/C2 `efc08a0`/C3 `e48d38f`/C4 `fb9ab50`/C5 `ea5fd2c`/C6 `0893cd0`/C7 `7e2c3f5`/C8 `5c86315`/C9 `1b42a04`/C10 `e6a1ed7`；未编号 `5b51c6a`/`67028b8`；**C11 缺号**；C12 `0b0fa48`/C13 `402d4be`/C14 `2e5fc74`；PKG-4 段 C12–C19 载体 review-0.3.0 L35、C34–C40 载体 review-pre1 L20/L24（C34 身份门控→C35 彩蛋日志→C36 作者指南/SKILL+ABI 锁→C37 legacy 桥原型→C38 版本/changelog→C39/C40 fixtures 行尾）；**缺席 C15/C16/C20–C33**；`0.2.4-FINAL` 留档 dev@`8bd383d`；dev 包 SHA256 `670f8e2b…ffe9d`（zip）/`af5e2a18…c71b`（dll）。**禁止写成连续 C1–C40。**

**ANCH-G US/双仓标识（PKG-3#ANCH-1,2,3,7）**：repo=`coahuilite/UniversalSqueaker`、packageId=`coahuilite.universalsqueaker`；`UniversalSqueaker.SqueakVoicePackDef`/`SqueakyRatkin.SqueakVoicePackDef`（薄空类桥=08-24 授权唯一例外）；`UniversalSqueaker.CompProperties_Squeaker`/`SqueakyRatkin.CompProperties_Squeaker`；16 patch 15 同名同目标；US 基准 `0.4.x`@`c8794ff`+4 未提交改动；`coahuilite.ferritelib` 硬前置；建议新 id `coahuilite.squeakyratkin.legacy`；`SqueakyRatkin_Profile_<race>.xml`；schema 4/2；顺序硬门 `U1 检测落地 → US 型 Ratkin 包/legacy 桥启用 → SR 1.0 内容化（DLL 退役）`；编号集 S1–S4/U1–U4/F1–F8/E1–E12/B1–B5/P0–P4/R1–R6/Q1–Q10；`foreign_squeak_comp`（建议名）；`AgeBucket{Baby,Toddler,Child,Adult}`；五级选择链；`ProductDomainFilter`（0.3.x 常量 `{Ratkin}`、三处入口、0.4.x 移除）；试验名单 default `{Ratkin}`/experimental `{Ratkin,Kiiro,Miho}`（替换非叠加、隐藏、UI 不渲染、release 默认 off）；内置 fallback 起点 `{Ratkin,Kiiro}`；Kiiro 许可门「未经作者明确许可不得发布或宣传」。

**ANCH-H 发布与隐私工具链（PKG-4 §9；PKG-5#ANCH-1,2,3）**：三命令逐字 `pwsh scripts/verify-local.ps1`/`pwsh scripts/check-pack-readiness.ps1 -RequireReleaseMetadata`/`pwsh scripts/privacy-audit.ps1 -FullHistory -PrePush`；`-NoRestore`/`[claim]`/`$knownHistoryDebt`/`[known-debt]`；`<Version>`（csproj 主源）≡`<modVersion>`（About.xml）断言；`version.txt` 三行；包排除 `*.pdb`/`*.gitkeep`/`codemap.md`/`PublishedFileId.txt`（只记文件名值不落盘）；staging `dist/steam/SqueakyRatkin`；授权边界逐字「本地 `commit` 不需要授权；远端 push / PR / merge / tag / Release / Workshop 上架都需要维护者明确授权」；「绝不尝试登录态或编辑界面」「页面级核验 ≠ 玩家下载内容已验证」「新增一条 = 一次维护者裁决」「历史/tag 重写另行授权」「禁止重新加回人工仪式」「任何渠道都不再手工逐项核验」；tag 链 `v0.1.0`/`v0.1.0-rc1`/`v0.1.1`/`v0.2.0`（唯一内容干净 vs 10 tag 冲突）/`v0.2.1`/`v0.2.2`/`v0.2.3`/`v0.2.4`/`v0.3.0`/`v0.3.2-pre1`+未推送 `archive/dev-pre-sanitize-0.2.0`；命名 `vX.Y.Z`/`vX.Y.Z-hotfixN` 严格 SemVer 2.0；包基线 115→116（121=泄漏态）；资产字节 1,569,962/1,573,976/1,574,271/1,574,386/1,578,195/1,588,909 B；Steam 页 1.871/1.872 MB、API 1,879,864 B；`merge -s ours`；git 2.47.1、filter-branch 可用、git-filter-repo 未装；债务 5 文件×1 处、漂移 ≈229/≈201/≈200/≈200/≈200、触及提交 4/7/3/3/2、`afa00cb3`/`2936879a`/`75ae4167`/`b19d68a`（跨仓两次）、227 revision/230 提交、54 处 hash 引用、10 tag/9 并集、3 唯一身份全 noreply、3 陈旧暂存包、约 11 秒。

**ANCH-I Eat 数值与验收（PKG-6 §9）**：`eatOnlyDuringChewing`（父，UI「仅在真正进食（正在摄入营养）时触发 Eat 叫声」）/`eatIncludeDrugs`（子，UI「使用成瘾品」）均默认 false；父关强制子 false（UI 置灰可见、`PostLoadInit` 归一）；`Eat=EachTime,144 ticks`、`globalMinIntervalTicks=216`≈3.6 秒一发；`baseIngestTicks` 默认 500；烟卷 720t/薄片 650t=零营养静默 hole；corpse `Nutrition=5.2`；`scaleCooldownWithTimeSpeed` 默认开；toil 链 `ReserveFood→PickupIngestible→CarryIngestibleToChewSpot→FindAdjacentEatSurface→ChewIngestible→FinalizeIngest`；权威①`IEatingDriver.GainingNutritionNow`、权威②`JobDriver.CurToilString`；真值表 10 行；本地化 542 键；行高 34f；`tools/KernelCharacterization` `EatOccurrenceRules` 单测。

**ANCH-J 排版与文案术语（PKG-4#DEC-17）**：BBCode 保守子集 `[h1][h2][h3][b][i][list][*][olist][url][hr][/hr][code]`；文案现值中文 2101/英文 4741（≤常见 8000 无权威出处）；正式英文 `Ratkin`；玩笑 `adorable little mousie`/`mousies`，禁 `rat-rats`/`mousefolk`/`Masterpiece`；中文正式「鼠族」、「鼠鼠/鼠辈」仅名与玩笑；「3A」=AI 规划/AI 编程/AI 维护；`AI-Generated Work Disclosure`；`Hide The Book of Squeakudges on a high stool!`；页面标题 Squeaky Ratkin/鼠辈啁啾。

## 10. 来源回引（source map）

| 源文件 | 被本包吸收的主题 | 载体 |
| --- | --- | --- |
| `docs/project-architecture-contract.md` | 权威分层/身份/17 动作/触发/发声门/XML 权威/合并/VoicePack 模型/示例包 | PKG-1 |
| `docs/settings-ui-product-contract-zh.md` | 三页 UI/7 击第四页/immediate/Eat 例外/响应式/非目标 | PKG-1 |
| `docs/logging-protocol.md` | SqueakLog 门面/详细模式/srdiag v1+v2/once 限流/事件注册表 | PKG-1 |
| `docs/0.3x-refactor-architecture-decision-zh.md` | 内核但薄/动作门/公理/彩蛋/阶段映射/双仓/迁移纪律 | PKG-2 |
| `docs/0.3x-equivalence-review-zh.md` | 14 行等价对照+诚实边界 | PKG-2 |
| `docs/0.3x-release-gate-checklist-zh.md` | A–H 门槛/C 链/后置修复 | PKG-2 |
| `docs/handoff-0.3.0-zh.md` | 08-19 交接时间层 | PKG-2 |
| `docs/internal-universalization-design-note-zh.md` | 通用化数据模型/六门/数据限定/filter | PKG-3 |
| `docs/us-sr-compatibility-check-zh.md` | E1–E12/F1–F8/组合矩阵/双开矩阵/U1–U4 | PKG-3 |
| `docs/us-sr-migration-plan-zh.md` | B1–B5/P0–P4/三选一/R1–R6/身份归属 Q1–Q10 | PKG-3 |
| `docs/release-runbook-zh.md` | 三命令入口契约/最小仪式/授权边界/阶段 0–4 | PKG-4 |
| `docs/release_review/release-0.2.0..0.3.2-pre1-review-zh.md`（7 份） | 各版发布证据链/Claim Pack/INC | PKG-4 |
| `docs/CHANGELOG.md` / `CHANGELOG.zh-CN.md` | 版本序列/功能面账本/时间替换纪律 | PKG-4 |
| `docs/steam-workshop-page-copy-draft.md` | 文案维护源/术语/版本锚点 | PKG-4 |
| `docs/release_review/process-review-zh.md` | 0.2.x 事故台账+8 措施 | PKG-5 |
| `docs/release_review/process-redundancy-review-zh.md` | R1–R12/N1–N3/V1–V3/三命令落地 | PKG-5 |
| `docs/release_review/privacy-history-rewrite-plan-zh.md` | 方案 A/B/C/P0–P6/Q1–Q5/不可逆点 | PKG-5 |
| `docs/handoff-0.3.3-zh.md` | 0.3.3 交接/已定案 7 条/开放裁决/09-13 接续 | PKG-6 |
| `docs/handoff-eat-occurrence-granularity-zh.md` | Eat 规格/vanilla 基线/两级开关/验收矩阵 | PKG-6 |
| `prompts/docs-consolidation/README.md` | 管道权威分层/派发纪律/产物处置 | PKG-1#DEC-37 |
| 语料外（只留指针） | US 仓 `mod-structure-reference-zh.md`；US 09-06 裁决原文；TODO/MEMORY/AGENTS.md 本体；Steam 评论；草稿存档；Scenarios.cs | 各包 GAP/CNF 指明 |

## 11. 供下游独立挑战的质疑钩子（按覆盖需要分组；每条给出最可能的误读、回查点与推翻后果）

**K1「现行合同=已验证行为」**——结论引用 DEC-v2-1/2/3（[F] 现行）。可能错在：全部「现行有效」以合同自述为背书、无实现证据（PKG-1#GAP-1）；「包内无反例」≠「无反例」。回查：`project-architecture-contract.md` 全文 ↔ `Source/SqueakyRatkin/`；等价评审 14 行与 C 链（PKG-2）。推翻⇒§3 v2-1..3 状态列与 L1 同时失效。

**K2 八面全绿=可发布**——结论引用 DEC-v2-6。可能错在：把 G「绿（论证）」当实测；把全绿读成无回归（同日后置修复 C12 即反例）；E/F 面只审 diff 不测行为。回查：检查表 L3/L14/L25–L27；PKG-2#INC-1/EV-10。推翻⇒0.3.0 发布决策的验收基础削弱、L7 失效。

**K3 C 链完整**——任何「C1–C19 连续」叙述都错。可见=C1–C10、C12–C14（PKG-2）+C17–C19、C34–C40（PKG-4 载体）；C11 缺号、C15/C16/C20–C33 语料缺席。回查：检查表 L29–L31+review-0.3.0 L35+review-pre1 L20/L24。推翻（发现新载体）⇒更新 §6.1 PKG-2#CNF-1 与 ANCH-F。

**K4 0.3.0/0.3.2 发布状态**——结论引用 DEC-v2-11。可能错在：把 PKG-2#OQ-1「仍阻塞」当 08-20 之后仍阻塞（PKG-4 已记 08-21 发布）；把「0.3.2 仅 pre1」当 0.3.2 工作未发（实为并入 0.3.3）。回查：review-0.3.0 L3–18、review-pre1 L5–20、CHANGELOG 双语 L39–57。推翻⇒渠道矩阵重排。

**K5 ABI 冻结起点**——结论引用 DEC-v2-1/ANCH-B「自首个携带 0.3.1 ABI 的发行版本」。可能错在：把 PKG-4 的证据闭合当合同文本已修订（合同原文未改、PKG-1#CNF-8 保留）；把 prerelease 算作 released（PKG-4 证据=0.3.1 无正式版⇒起点实际落在携带该 ABI 的首个正式版，即 0.3.3 或其后，但**合同文本未写**）。回查：`A §6 L55` 原文+渠道矩阵。推翻⇒作者预期与 0.3.2 扩展合规归类改变。

**K6 tier 折叠是「有意设计」**——结论引用 DEC-v2-3。可能错在：语料从未记载归因损失是否已知取舍；`vanilla` 语义边界未定义。回查：`C L46 L59 L67 L73`；resolver tier 枚举。推翻（找到设计记录）⇒L6 升格为非 tentative。

**K7 Eat fail-open 与「营养并集」**——结论引用 DEC-v2-13。可能错在：把 fail-open 读成「未确认=静默」（反方向）；把啤酒/仙馔父开子关仍响读成 bug（明确接受为默认行为，OQ-17/23 才是改变路径）。回查：E §5.1/§7.1/§8.8；`EatOccurrenceRules` 单测。推翻⇒DEC-v2-13 撤回重裁。

**K8 双开安全与顺序硬门**——结论引用 DEC-v2-9。可能错在：把「当前双开安全」当永久（US 型包/桥落地即双响）；把 F1/F2/F3 当已观察事故（实为只读推演）；把 U1 当过渡修复（legacy 长期在线则永久必需）。回查：compat §2/§3/§5（E1–E12/F1–F8/矩阵）；mig §8.4。推翻⇒顺序硬门重排、P0–P4 时序变。

**K9 六门未验证=可先拆 US**——结论引用 DEC-v2-8。可能错在：把「机制已通用化设计」当「六门已过」（通过条数无记录，GAP-7）；把规划示例名（`SqueakBuiltInFallbackCatalog` 等）当现行符号名。回查：note 全文+合同 §6（PKG-3#CNF-10）。推翻⇒拆分前提重估。

**K10 隐私债务规模**——结论引用 DEC-v2-12。可能错在：把 10 tag 当受影响数（10/9 未定）；把「方案 B 已准备」当「可执行」（P0 mirror 缺失+5 条不可逆点+逐条授权）；把 force-push 读成「GitHub 删除」。回查：privacy-history-rewrite-plan §5/§8；PKG-5#CNF-4/5。推翻⇒B 的成本收益重算。

**K11 压缩完整性**——本文件声称 51 CNF/96 OQ/131 DEC/73 ASM 全保留。可能错在：合并表达丢失某条的独有细节（尤其 CNF-1 组内 8 条的回查指针、PKG-3 OQ 表的「影响」列）。回查：各包 compact §6/§8 同 id 行（这些文件已冻结、字符数见 §0）。推翻⇒按 PKG-n#id 补回，不影响其余结论。

**K12 时间层混读**——本文件多处并存多时间层（0.3.3 本地已提交 vs 08-23 未提交；公告删除两口径；tag 10/9；跳过口径 vs 终局裁决）。可能错在：把后写层当唯一事实、把「口径」当「裁决」。回查：各 CNF 的「时间层」标注；handoff §10 层结构。推翻⇒以实际 repo 状态（`git log`，终审可执行）为最终权威。

## 12. 压缩损失与删减账本

| 丢弃/降级内容 | 原属 | 类型 | 理由 | 是否可回查 |
| --- | --- | --- | --- | --- |
| Claim Pack SHA-256 全表（11 支）、资产字节级明细、BBCode 文案体、CHANGELOG 双语逐字全文、28 条 v1 人读句、mermaid 图体、C# 签名块原文、措辞草案四段、债务路径字面量与 token 正则、Player.log 原文 | PKG-1..5 各 §12 | P2 | 隐私门（禁复制值/摘录）或产物非决策；标识符与计数已在锚点/正文逐字保留 | 各包 compact §9/§12；Claim Pack 原文 |
| DEC/ASM/CNF/OQ/GAP 的「背景散文」「推断链展开语句」「重复 src 复写」 | 全六包 | 压表达 | 结论、双方要旨、替代解释、影响面全部保留；同一背景只写一次 | 包内同 id 行 |
| 逐包 P2 导航（冷启动读序、纯导航链接、表头分隔行） | 全六包 | P2 导航 | 无决策内容 | v1 原文 |
| pass-1 六包 compact 全文 | `final/v2-parts/PKG-{1..6}.compact.md` | 中间产物保留 | 合并底稿；含 pass-1 逐节上限自检与逐包对账 | 文件在仓 |
| 逐包 §13 自检的机械核对过程 | 全六包 | P2 | 结论并入 §0 对账与 §12 | 包内 §13/§0 |
| **未删**：任何 DEC/ALT/CNF/OQ/GAP/ASM/ANCH/INC/EV/LES/TL/C/I/U 条目 | 全部 | P0 | 02 §6 规则 6；维护者裁定 P0 不删 | — |

**压缩比实测**：v2 终稿 50,788 字符 = 0.261·R / 0.132·S；严格档 48,588 未达（超 3.5%），密度档 58,306 以内但未主张密度例外（逐字面 <50%）；按维护者裁定不删 P0 凑数，瓶颈=§9 逐字锚点面与 §6 零丢弃面，已用手法见 §0/§12。

## 13. 自检（02 §7 逐条）

1. **盲信测试**：只读本文件，对 DEC-v2-1（ABI 边界）、DEC-v2-6（八面全绿）、DEC-v2-9（顺序硬门）均可找到「可能错在哪 + 回查哪个文件哪一节」——见 §11 K1/K2/K8。通过。
2. **P0 对账**：CNF 51/51、OQ 96/96、DEC 131/131、ASM 73/73、GAP 51/51、ALT 51/51（并入 DEC 备选段）、ANCH 全标识符（§9+各包 §9）、INC/EV/LES/TL 全——§0 表与 §6/§7/§8/§9 一致。数量减少：无。
3. **标签保真**：`[C]/[I]/[U]` 未升格；合并条目取最保守标签；预测性结论（F1/F2/F3）标注「未发生」；`[I-提炼]` 仅出现在 §0 与 §11/K11、§6.3 编排方实测标注处。
4. **矛盾完整**：§6.1 六组 51 条逐条有双方要旨与来源；语料外一方（US 仓规则文档）标注保留不归一。
5. **溯源完整**：§3 每组有 `src: PKG-n#DEC-*`；§4/§5/§6/§7/§8 每行有原 id 指针；§9 归 §10；无来源的结论条目=无。
6. **数值逐字**：ANCH-A..J 与 §5/§6 数字直接取自六包 compact 锚点节（抽验：17 动作序号、28/4 事件、1622→3782、115→116、5 文件×1 处、≈229、227/230、54 处、10 tag/9、15/17 动作键、144/216 ticks、500/720/650t、0.08/0.2/5.2、1024/256、34f/542 键——与 PKG-1/2/3/5/6 §9 一致）。
7. **隐私**：全文无盘符形态、无本机绝对路径、无日志摘录、无凭据、无 `PublishedFileId` 值（形态描述除外，已登记）；隐私扫描工具面（脚本/模式）只述形态。
8. **预算结构**：§3+§5+§6 实测占比 41.7%（<50%）——低于原因=必填的记账/锚点/来源节（§0/§9/§10/§12/§13）占 37%，按 02 §4 例外条款在 §0 说明；§9 超逐节上限的原因=锚点逐字面，已按处置顺序①合并到族级、删来源复写，标识符与计数零删减。
9. **无新增事实**：全部事实性陈述可在六包 v1 或其 compact 找到对应；编排方裁定（跨包接缝 §0、C 链实测、coverage 结论）为上游既定输入，非本包新事实。
