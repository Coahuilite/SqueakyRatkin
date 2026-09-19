## 4. 假设账本（ASM-<n>）
| id | 假设（要旨） | 提出时间 | 是否仍成立 | 变更/失效点 | 验证或反证 | 来源 |
| --- | --- | --- | --- | --- | --- | --- |
| ASM-1 | 兼容边界必须与内部重构窗口分离：作者面可承诺稳定，内部面保留 0.x 修订自由 | 2026-08-22（随 ABI 声明） | 成立 `[F]` | 若出现"作者面被迫破坏性变更"即失效 | 语料内无失效记录 | `src: docs/project-architecture-contract.md §6 L55` |
| ASM-2 | 环境性缺失（DLC/目录/包/预设/音池）应降级而非剥夺资格，且可自动恢复 | 不可得 `[U]` | 成立 `[F]` | 无 | orphan/dormant 自动恢复 + `GlobalOnly`/fallback 回落即为该假设的制度化 | `src: docs/project-architecture-contract.md §2 L15 §6 L61` |
| ASM-3 | 玩家可见的名称/图标/发现信息只是显示与候选提示，永不是资格判据 | 不可得 `[U]` | 成立 `[F]` | 无 | 架构 §2 L17、设置 §产品表面 L9 双写同一假设 | `src: docs/project-architecture-contract.md §2 L17`；`src: docs/settings-ui-product-contract-zh.md §产品表面 L9` |
| ASM-4 | 稳定的键空间与字段序（动作序号、srdiag 字段序、注册表句子）是兼容面本身，须闭合而非开放 | 不可得 `[U]`（v1 层）/ 2026-08-22（动作键 append-only 入公开 ABI） | 成立 `[F]` | 无 | v1 声明"byte-immutable"；动作键声明"append-only" | `src: docs/logging-protocol.md §Closed Typed Facade L7`；`src: docs/project-architecture-contract.md §6 L55` |
| ASM-5 | 触发粒度规则可作为纯数据链入 kernel harness，Verse 采样留在 adapter（可测试性假设） | 2026-09-13 版本层（合同记载） | 成立（合同口径）`[F]`；实测状态本包不可得 `[U]` | 无 | 合同称规则"pure data linked into the kernel harness"；等价/实测证据在 [xref: PKG-2（等价评审）] | `src: docs/project-architecture-contract.md §3 L31` |
| ASM-6 | 不做迁移承诺：无证据即不得推断或 promise 无关设置的迁移；配置维度彼此独立 | 不可得 `[U]` | 成立 `[F]` | 若未来引入迁移工具则需重开 | 明文禁令 | `src: docs/project-architecture-contract.md §5 L49` |
| ASM-7 | RimWorld 已解析的 `CurLifeStage.developmentalStage` 足以映射年龄桶，无需自建年龄判定 | 不可得 `[U]` | 成立 `[F]`（含已知残差：`Toddler` 在 1.6 无原生阶段，仅存为 ABI 桶） | 无 | 合同自承残差 | `src: docs/project-architecture-contract.md §6 L53` |
| ASM-8 | 语音包只贡献声音，绝不改行为/timing/capability/distance/mood；fallback 末端由维护者 C# 数据决定 | 不可得 `[U]` | 成立 `[F]` | 无 | 模式语义 + `BuiltInFallbackCatalog` 归属维护者 | `src: docs/project-architecture-contract.md §6 L57` |
| ASM-9 | 新机器可读事实必须带版本标记（`fmt=2`）且不与 v1 混写，旧解析器才能容忍 | 0.3.1 版本层 | 成立 `[F]` | 无 | once-key 按 `log-v1`/`log-v2` 分域是该假设的推论落地 | `src: docs/logging-protocol.md L50 L75` |
| ASM-10 | 本日志合同足以指导机读解析，但**不**构成实机会话证据（证据自限假设） | 2026-08-22 | 成立（文档自我声明）`[F]` | 若下游把它当实测证据即被误用 | 无反证；也**无正证**（见 GAP-1） | `src: docs/logging-protocol.md §Status L3` |

## 5. 认识论分账（只列 C / I / U）
- **参与者声明 C-1**：`docs/logging-protocol.md` 的作者（身份未记 `[U]`，2026-08-22）声明本合同来自对四个源文件的"source review"，并主动声明它不是实机会话测试证据。是否被独立证据支持：**否**（本包主源内无测试记录；[xref: PKG-2（等价评审 / C1–C19 证据链）] 可能含相关证据，本包不代其判定）。`src: docs/logging-protocol.md §Status L3`
- **参与者声明 C-2**：设置合同声明 2026-08-23 的 Eat 控件例外是"维护者决定，承接 Steam 评论反馈"——外部渠道信号被记为决策依据，但反馈内容、条数、原文均未在语料内出现。`[C]`（人称=维护者；无独立证据）。`src: docs/settings-ui-product-contract-zh.md §非目标与实现路径 L29`
- **参与者声明 C-3**：架构合同声明 Ratkin 内置档案"刻意只有 15 个键"、Template 基线"15 目录 / 41 OGG"。前者是意图声明（`[F]` 文本 + 意图 `[C]`）；后者是可回指文件系统的计数声明，本包按规则未做外部取证，标 `[C]`（自述基线，文档同时声明该计数可变）。`src: docs/project-architecture-contract.md §6 L57 §7 L67`
- **本包推断 I-1**：结论 —— "合同把'什么算破坏兼容'变成可机械判定的集合"。推断链：DEC-31（注册表与字段序即兼容面）+ DEC-14（公开面枚举 + add-only/append-only + fails closed）+ DEC-5（动作键 append-only）⇒ 兼容违规可穷举检查。替代解释：这只让**违规可枚举**，不等于**违规可自动发现**（合同未规定任何检查门；README §2 规则 3 反而禁止本管道新增检查门）。`src: docs/project-architecture-contract.md §3 L21 §6 L55`；`src: docs/logging-protocol.md §Closed Typed Facade L7`
- **本包推断 I-2**：结论 —— "玩家侧最难自证的语义是 fallback 层与静默末端，因为日志 tier 词汇不足以区分它们"。推断链：DEC-13（四末端链）+ DEC-15（15 键档案）+ DEC-36（`pack_fallback`/`built_in_fallback` 保留不发出，`BuiltInFallback` 折叠进 `vanilla`）⇒ 一次实际走内置 fallback 的派发在日志上与走 vanilla 路径不可区分。替代解释：`tier=vanilla` 可能在本项目的词汇里**就**表示"内置末端"，折叠是有意命名而非缺陷（合同未定义 `vanilla` 的语义边界 → 见 U-1）。`src: docs/logging-protocol.md L73`；`src: docs/project-architecture-contract.md §6 L57`
- **本包推断 I-3**：结论 —— "七次点击解锁 + 第四页收纳"意味着日志/诊断面在玩家默认视图下不可见，因此 CNF-1（`pawn` 标签 vs 禁写标签）的实际暴露面受 dev 门控限制。推断链：DEC-23（解锁才见第四页）+ DEC-36（`audio.route.selected` 为 DevOnly）⇒ 默认玩家日志不含该细节行。替代解释：`pawn=<label>` 属 v2 细节后缀，而 Daily 事件（如 `voicepack.pack.rejected`）仍可能携带其他可识别值；且解锁状态本身持久化（`developerToolsEnabled`），一次解锁后长期有效。`src: docs/settings-ui-product-contract-zh.md §产品表面 L7`；`src: docs/logging-protocol.md L73 L23`
- **未决 / 缺失 U-1**：`tier=vanilla` 在产品语义上是否等于"内置 fallback 末端"？缺的是**词汇定义**（合同只列出可发集合与折叠行为，未定义 `vanilla` 指代哪一层）。影响：DEC-13/DEC-15 的可观测性判定与 I-2。`src: docs/logging-protocol.md L73`
- **未决 / 缺失 U-2**：`audio.dispatch.ok` 的"characterization surface"具体由哪个测试/工具承载未记载。缺的是**测试标识**（文件/用例名）。影响：DEC-36 的回归安全判断。`src: docs/logging-protocol.md L73 L81`
- **未决 / 缺失 U-3**：本包未获得任何"实现与合同不一致"的反例记录 —— 缺的是**审计报告或等价评审结论**（在 [xref: PKG-2]）。影响：DEC-1 的"不静默归一"是否已被实践检验。
- **未决 / 缺失 U-4**：`Toddler` 桶何时会被真实填充（其他 mod 提供 1.6 原生 Toddler 阶段？）未记载。影响：DEC-12 的年龄 ABI。`src: docs/project-architecture-contract.md §6 L53`
- **未决 / 缺失 U-5**：`map-lifecycle reset` 明确"当前未实现"，但未记载是否为待办。缺的是**决策记录**（是缺陷还是有意）。影响：DEC-32 的会话膨胀判断。`src: docs/logging-protocol.md L27`
- **未决 / 缺失 U-6**：合同未记载 17 动作 ABI 与"未来 action gates"（DEC-14 未冻结面）之间的关系边界到哪一步。影响：第三方包作者的可预期性。`src: docs/project-architecture-contract.md §6 L55`
- **未决 / 缺失 U-7**：`SqueakEatOccurrence` 三模式在日志协议中**无专属字段或事件**（v2 事件表未含 Eat 模式）；缺的是"该偏好是否要求可观测"的裁决。影响：DEC-7 的支持排障路径。`src: docs/logging-protocol.md §Stable Event Registry L62–L70`
- **未决 / 缺失 U-8**：绝大多数条文没有时间与主体记录（合同为无签名的规范文本）。缺的是**条文级时间层**。影响：§2 时间线只能靠版本层与 `(冻结动作)` 支撑。
## 6. 矛盾与反转（CNF-<n>）
> 权威分层（[xref: prompts/docs-consolidation/README.md §4]）只用于给"若必须选一个，建议以谁为准"，不用于删除冲突记录。以下冲突一律**并列保留**。

| id | 冲突双方（含来源） | 各自时间层 | 冲突点 | 可能解释 [I] | 建议回查点 |
| --- | --- | --- | --- | --- | --- |
| CNF-1 | (a) `src: docs/logging-protocol.md §srdiag v1 Machine Contract L46`：不得把本地化文本、**pawn 标签**、文件系统路径写入协议；(b) `src: docs/logging-protocol.md §Stable Event Registry L67`：`audio.route.selected` 字段表含 `pawn=<label>`；(c) 同文件 `L59`：「Only DefNames are written — never labels, HAR package names, or player-mutable text」 | (a) v1 基线合同；(c) 0.3.1；(b) 0.3.2 扩字段 | 同一文档既禁止 pawn 标签入协议，又在 v2 事件 schema 里规定 `pawn=<label>`；L59 的"永不写标签"与其紧邻的字段表互斥 | [I] L59 的禁令限定在 `race`/`xenotype` 身份字段语境，作者未打算覆盖事件专属字段；[I] 备选解释：0.3.2 扩字段时未回改 L46/L59 的通则（文档内一致性债）；percent-encode 只改变表示形式，不构成匿名化 | 读 `Logging/SqueakLogProtocol.cs` 的 `audio.route.selected` payload 类型与 sanitize 顺序；读 0.3.2 的提交信息/发布评审（[xref: PKG-4]）确认该字段是有意还是遗漏；若涉及隐私面 → [xref: PKG-5（隐私历史债务）] |
| CNF-2 | (a) `src: docs/project-architecture-contract.md §6 L57`：Fallback 链有四层末端（Xenotype pack → Race pack → pack fallback → built-in fallback → silence），Remix 含四层等层选择；(b) `src: docs/logging-protocol.md L73`：`tier` 可发词汇只有 `xenotype_pack`/`race_pack`/`vanilla`/`-`，`pack_fallback`、`built_in_fallback` 保留不发出，`PackFallback`→`race_pack`、`BuiltInFallback`→`vanilla` | (a) 架构合同（末次 2026-09-13）；(b) 0.3.1 定义 + 0.3.2 收缩 | 产品层四/五段语义在可观测层被折叠为三值：**走 pack fallback 与走 Race pack 不可区分；走内置 fallback 与走 vanilla 不可区分** | [I] 折叠是刻意的日志学简化（tier 表达的是"发行来源"而非"解析层"）；[I] 亦可能 tier 尚未与 0.3.x 解析层演进同步 | 回查 resolver 中 tier 枚举定义与装配器映射；确认是否有把 tier 拆细的未决项（本包记 OQ-3）；`SqueakDebug` 诊断叠加层可能暴露真实 tier → 语料内无记载（GAP-4） |
| CNF-3 | `src: docs/logging-protocol.md §Status L3` 状态戳 2026-08-22，但同句记载 0.3.2 事实 | 状态戳层 = 2026-08-22；被记载事实层 = 0.3.2（晚于状态戳） | 文档自称"2026-08-22 记录的合同快照"却包含 0.3.2 变更 ⇒ 快照边界与其声明的时间戳不一致 | [I] 多会话追加时更新了内容句而未更新状态行；`(冻结动作)` 显示该文件末次提交 `56b6f05`（2026-08-23），与"0.3.2 之后仍有内容"并存 ⇒ 追加发生在 08-23 或更早，状态行已成陈旧元数据 | 把状态行的可信度降级：以内容里的版本标记（`0.3.1`/`0.3.2`）而非日期戳作为时间层锚；需要精确归因则查该文件的逐段提交历史（本包按规则未做外部取证） |
| CNF-4 | (a) `src: docs/settings-ui-product-contract-zh.md L29`：Eat 例外的 Scribe 面「只 add-only 增加两个默认 `false` 字段（默认值省略，settings schema 不 bump）」；(b) `src: docs/project-architecture-contract.md §6 L55`：「Internal surfaces (kernel, domain model, **fallback profile schema**, and the future action gates) remain under the 0.x revision window」 | (a) 2026-08-23 裁决 + 2026-09-13 合同文本；(b) 2026-08-22 ABI 声明 | "schema 不 bump"是指玩家 settings 文件；"0.x 修订窗"指内部面 —— 二者不矛盾，但语料**没有**一处说明"玩家 settings schema 是否属于任何已冻结面"，于是"能否 bump settings schema"这一决策既无承诺也无禁区 | [I] 作者意图是：玩家 settings 面既不公开冻结也不承诺永不 bump，仅要求变更走 add-only；[I] 也可能是遗漏 | 回查 `SqueakyRatkinSettings.ExposeData` 与历史 bump 记录；确认是否存在 settings 版本字段（合同未提）→ 记 U/OQ（本包 OQ-4） |
| CNF-5 | (a) `src: docs/project-architecture-contract.md §3 L21`：`Crying`/`Giggling` 无内置 SoundDef 与内置 fallback 条目 ⇒ 无包声明即静默；(b) 同文件 `§6 L57`：Remix 是"跨四层等层选择"，`§6 L57` 又称未声明 pack fallback 的动作保留"冻结的三层 Remix 形状" | 同文档同一版本层（2026-09-13） | Remix 在缺 fallback 的动作上退化为三层；若该动作同时无内置档案（即 `Crying`/`Giggling`），实际可选层只剩 Xenotype/Race —— 合同未写明该情形的抽样形状与是否整体静默 | [I] "frozen three-tier shape"正是为此情形规定的兜底形状，缺内置层时该层自然缺席；[I] 也可能存在"三层权重被重分配"的第三种实现 | 读 resolver 的 Remix 层级装配代码与 harness 用例；或回查 [xref: PKG-2（动作门/路由公理的机械验证）] 是否覆盖此形状 |
| CNF-6 | (a) `src: docs/project-architecture-contract.md §1 L7`：冲突时"identify the mismatch and resolve it in the authoritative layer"（不得静默归一）；(b) 本包实测到同一文档内部 CNF-1…CNF-5 五处未被消解 | (a) 现行元规则；(b) 2026-09-17 观测 | 元规则要求消解，文档现状保留了多处内部不一致（含跨文件） | [I] 元规则针对"实现 vs 合同"，不针对"合同内部措辞一致性"；[I] 亦可能这些点尚未被任何会话识别为冲突 —— 本包即为首次登记 | 交由阶段 B/C 判定是否升级为维护者待办（本包不代拟修复方案） |
| CNF-7 | (a) `src: prompts/docs-consolidation/README.md §2 L35–L41`：唯一写入面应为仓库根临时目录、产物不进仓库历史；(b) 本次调度指令：OUT_DIR = `docs/consolidation`，产物属文档面 | (a) 套件写作时点；(b) 2026-09-17 维护者裁决 | 产物落点与"不提交"设计相反 ⇒ 隐私门（README §2 规则 7：产物入库前须过 `scripts/privacy-audit.ps1`）从"默认不需要"变成**必须** | [I] 维护者有意把产物升格为文档；风险从"泄露于历史"转为"泄露于提交" | 提交前对 `docs/consolidation/**` 执行既有隐私审计脚本（默认模式）；确认 `prompts/**` 是否已入允许面（README §10 的三选一）→ 本包 OQ-11 |
| CNF-8 | (a) `src: docs/project-architecture-contract.md §6 L55`：公开 ABI 自「the first released version that carries the 0.3.1 ABI」起生效；(b) `src: docs/logging-protocol.md L3`：0.3.2 扩展同一日志面且声明 v1 不变 | (a) 2026-08-22；(b) 0.3.2 版本层 | "首个承载 0.3.1 ABI 的发行版本"是哪一版（0.3.1？0.3.2？prerelease 是否算发行？）在语料内不可判定 ⇒ 公开冻结的**起点**未闭合，进而 0.3.2 的扩展是"冻结后的合法 add-only"还是"冻结前塑形"无法归类 | [I] 0.3.1 是承载版本，0.3.2 属冻结后 add-only 扩展（最保守读法）；[I] 若 0.3.1 未正式发行而只有 0.3.2-pre1，则起点后移 | 回查 [xref: PKG-4（版本序列与各渠道状态矩阵）] 中 0.3.1/0.3.2 的发行事实；本包不自行取证 |

## 7. 事故 / 失败 / 成功与后果（INC-<n> / EV-<n>）
> 本包主源为规范文本，**语料内没有事故叙述**；下表只收"条文以否定式记录的既往问题/废止口径"这类间接证据，并如实标注证据强度。

| id | 类别 | 现象 | 根因（若知） | 处置与结果 | 证据强度 | 来源 |
| --- | --- | --- | --- | --- | --- | --- |
| INC-1 | 回归风险（预防性禁令） | 缩放资格门（含 `CurrentZoom <= Close`）会错误阻断距离衰减 | 把"视图大小"误当"听者-声源距离"的代理 | 合同以禁止式废止该做法；现行口径为 `CurrentViewRect.ExpandedBy(10)` + `distRange` 衰减 | 论证（条文自身理由）；废止时点与事故记录不可得 `[U]` | `src: docs/project-architecture-contract.md §4 L39` |
| INC-2 | 失败模式（结构性防护） | 发声被拒后"每 tick 重试"的风险 | 若拒绝不消耗冷却，尝试会高频重复 | 规定"发声拒绝消耗与成功播放相同的每动作及共享尝试冷却" | 论证 | `src: docs/project-architecture-contract.md §4 L37` |
| INC-3 | 静默/可观测性缺陷（本包新登记） | 内置 fallback 与 vanilla 路径、Race pack 与 pack fallback 在日志中同值 | 装配器折叠 tier 词汇 | 未处置（合同把 `pack_fallback`/`built_in_fallback` 标为保留不发出） | 文本证据（规则 + 行为并列），无实测 | `src: docs/logging-protocol.md L73`；关联 CNF-2 |
| INC-4 | 文档一致性缺陷（本包新登记） | 通则"禁写 pawn 标签"与 schema"必写 `pawn=<label>`"并存 | 疑为扩字段时未回改通则 `[I]` | 未处置 | 文本证据（同文件两行直接互斥） | `src: docs/logging-protocol.md L46 L59 L67`；关联 CNF-1 |
| EV-1 | 成功（合同自证边界） | 日志协议主动声明"是兼容/机读合同，不是实机会话证据" | — | 后果：下游不得把本合同当验证结论；本包据此把 GAP-1 记为开放 | 文本明示 `[F]` | `src: docs/logging-protocol.md §Status L3` |
| EV-2 | 成功（版本追加证据） | 17 动作 ABI 实际按 append-only 演化：`Crying`=15、`Giggling`=16，0–14 不变 | — | 后果：append-only 承诺有一次已兑现的历史实例（但**未记时间**） | 文本明示 `[F]`；时间 `[U]` | `src: docs/project-architecture-contract.md §3 L21` |
| EV-3 | 成功（默认口径落地） | 0.2.3 起出厂默认 = Fallback + 内置 Race Example 启用，且只一次性施加于从未设置过模式的配置 | — | 后果：老玩家显式选择与既有选择不被覆盖（幂等保护） | 文本明示 `[F]`；无实测 | `src: docs/project-architecture-contract.md §6 L57` |
| EV-4 | 验证（唯一带外部门禁语义的条目） | staging 校验 Template→内置镜像的动作/键集合与 SHA-256 同一性，且**不断言固定计数** | — | 后果：示例音频可增量演化而不破校验；违规要上报不得静默修复 | 论证（合同描述脚本行为，本包未运行脚本） | `src: docs/project-architecture-contract.md §7 L67` |
| EV-5 | 验证（外部信号入决策） | 2026-08-23 维护者据 Steam 评论反馈放行设置 UI 唯一例外 | 玩家对 Eat 触发粒度的抱怨（内容未载） | 后果：两默认 `false` 字段 + 三级模式进入合同 | `[C]`（文档自述来源；反馈原文不在语料内） | `src: docs/settings-ui-product-contract-zh.md L29`；关联 C-2 |

## 8. 开放问题、阻塞与交接风险（OQ-<n>）
| id | 问题 / 风险 | 类型 | 影响面 | 当前状态 | 来源 |
| --- | --- | --- | --- | --- | --- |
| OQ-1 | 「首个承载 0.3.1 ABI 的发行版本」是哪一版？0.3.2 / 0.3.2-pre1 是否算"released"？ | 依赖外部（发布事实） | 第三方包作者的冻结起点；日志扩展的合规归类（CNF-8） | 未决；需 [xref: PKG-4] 的渠道状态矩阵 | `src: docs/project-architecture-contract.md §6 L55` |
| OQ-2 | `pawn=<label>` 是否被接受为日志常态字段？若否，退役/净化方案为何？ | 待裁决（含隐私面） | 日志兼容面（v2 字段不可随意删）× 隐私承诺 | 未决；本包仅登记冲突，不裁决 | `src: docs/logging-protocol.md L46 L59 L67`；CNF-1 |
| OQ-3 | 是否需要把 `tier` 拆细以区分 `pack_fallback` / `built_in_fallback`？ | 未验证 / 待裁决 | 排障可观测性、fallback 承诺的可证明性 | 未决（保留词汇已定义但未发出） | `src: docs/logging-protocol.md L73`；CNF-2 |
| OQ-4 | 玩家 settings schema 是否承诺永不 bump？add-only 是否为其唯一约束？ | 待裁决 | 存档/设置兼容边界（合同未给 settings 面任何冻结声明） | 未决 | `src: docs/settings-ui-product-contract-zh.md L29`；`src: docs/project-architecture-contract.md §5 L49`；CNF-4 |
| OQ-5 | `Crying`/`Giggling` 在 Remix 且无任何包音频时的抽样形状（三层缺一层 / 两层）未定义 | 未验证 | 17 动作 ABI 与默认静默承诺的一致性 | 未决 | `src: docs/project-architecture-contract.md §3 L21 §6 L57`；CNF-5 |
| OQ-6 | "未来 action gates"（明确未冻结）与"内置动作键 append-only"（明确公开）之间的边界在哪？ | 待裁决 / 需授权 | 作者可预期性；内部重构自由 | 未决 | `src: docs/project-architecture-contract.md §6 L55` |
| OQ-7 | `map-lifecycle reset` 未实现是缺陷还是有意？once-key 上限 1024 触发清空是否会造成同一会话内重复噪音？ | 未验证 | 长会话日志体积与噪声 | 未决 | `src: docs/logging-protocol.md L27 L79` |
| OQ-8 | orphan 条目是否有数量/保留期上限？大量长期失联目标的处理口径未记载 | 待裁决 | 设置文件膨胀与 UI 可读性 | 未决（现行口径：保留 + 显式忘记） | `src: docs/project-architecture-contract.md §6 L61`；`src: docs/settings-ui-product-contract-zh.md L11` |
| OQ-9 | `SqueakyRatkin_Profile_<race>.xml` 的"当前版本"如何判定（更旧副本从 C# 源重建的判据）？ | 未验证 | 回退档案的稳定性 | 未决 | `src: docs/project-architecture-contract.md §6 L59` |
| OQ-10 | 实现权威路径清单是否需要覆盖 `1.6/Assemblies` 之外的 flavor（1.5/1.6 双版本）？合同只列 `1.6/…` 路径 | 依赖外部（发行结构） | 兼容边界的适用范围 | 未决（本包不做仓外取证） | `src: docs/project-architecture-contract.md §5 L43 §7 L73` |
| OQ-11 | `prompts/**` 是否已被维护者追加进收敛任务的允许面？产物落 `docs/` 后是否仍需按 README §10 三选一处置？ | 需授权 | 最终收敛提交的内容边界（混入套件的失败模式） | 未决；需编排方/维护者裁决 | `src: prompts/docs-consolidation/README.md §10 L194`；CNF-7 |
| OQ-12 | 合同无签名/版本/生效日期元数据（除个别内嵌日期）——是否要求为三份合同补"版本头"？ | 待裁决 | 时间层可判定性（U-8 的根因） | 未决 | `src: docs/project-architecture-contract.md §1 L3`；`src: docs/logging-protocol.md L3` |

## 9. 锚点（ANCH-<n>：必须逐字保留）
> 供阶段 B 做逐字校验；每条附来源；**不得改写、不得意译、不得四舍五入**。

- **ANCH-1 标识符 / 类型 / 键 / 前缀 / 文件名（逐字）**：
  `SR_`；`CompSqueaker`；`NewRatkinPlus`；`XenotypeDef.defName`；`ThingDef.defName`；`FactionDef.defName`；`CanUseXenotype`；`IsRatkin`；`GlobalOnly`；`SqueakVoicePackDef`；`raceDefName`；`scope`；`targetDefName`；`weight`；`fallbacks`；`actions`；`action`；`ageTag`；`IsEgg`；`sounds`；`PackKey`；`voicePackMode`；`allowEasterEggSounds`；`BuiltInFallbackCatalog`；`SqueakFallbackProfileStore`；`SqueakyRatkin_Profile_<race>.xml`；`WriteSettings()`；`ModSettings`；`PackFallback`；`BuiltInFallback`；`Race`；`Vanilla`；`SqueakDevLoggingMode`；`developerToolsEnabled`；`Prefs.DevMode`；`ShouldEmitDev`；`SetDevLoggingMode`；`SqueakDebug.ResetLoggingSession`；`SqueakEatOccurrence.ResolveMode`；`SqueakEatOccurrence.ChewingToilDebugName`；`SqueakEatOccurrence.AllowsOccurrence`；`eatOnlyDuringChewing`；`eatIncludeDrugs`；`IEatingDriver.GainingNutritionNow`；`JobDriver.CurToilString`；`ChewIngestible`；`CachedNutrition`；`PostLoadInit`；`SoundInfo.pitchFactor`；`volumeFactor`；`InMap(TargetInfo(Pawn))`；`distRange`；`CurrentViewRect.ExpandedBy(10)`；`Verb.TryCastShot`；`MentalStateHandler.TryStartMentalState`；`MentalFitDef`；`CurLifeStage.developmentalStage`；`JobDefOf.Ingest`；`1.6/Patches/Ratkin_AddSqueakComp.xml`；`CompProperties_Squeaker.distancePresets`；`SR_OfficialExample_Race`；`SR_ExampleTemplate_Race`；`clipFolderPath`；`<lowercase packageId>/<PackDef.defName>/<Action>/`；`Source/SqueakyRatkin/CompSqueaker.cs`；`Source/SqueakyRatkin/Patches/`；`Source/SqueakyRatkin/SqueakyRatkinSettings.cs`；`Source/SqueakyRatkin/SqueakEatOccurrence.cs`；`Source/SqueakyRatkin/SqueakVoicePackModels.cs`；`SqueakSettingsGameContext`；`scripts/stage-package.ps1`；`Logging/SqueakLog.cs`；`Logging/SqueakLogProtocol.cs`；`Debug/SqueakDebug.cs`；`[SqueakyRatkin] `；`srdiag fmt=1`；`srdiag fmt=2`；`log-v1`；`log-v2`；`<path>`；`N/A`。
  `src: docs/project-architecture-contract.md §1 L9 §2 L15 L17 §3 L23 L31 §4 L37 L39 §5 L43 §6 L53 L55 L57 L59 §7 L65 L71 L73`；`src: docs/logging-protocol.md L7 L9 L23 L25 L46 L58 L59 L67 L73`；`src: docs/settings-ui-product-contract-zh.md L9 L11 L29 L31`
- **ANCH-2 动作名与序号（17 个，append-only，逐字，顺序即序号 0–16）**：`Call`、`Eat`、`Sleep`、`Wounded`、`Select`、`Move`、`Social`、`Joy`、`Death`、`Draft`、`Undraft`、`Attack`、`Work`、`Equip`、`MentalBreak`、`Crying`(=15)、`Giggling`(=16)；内置 fallback 档案 Ratkin 键数 = **15**（`Call` 至 `MentalBreak`，缺 `Crying`/`Giggling`）。`src: docs/project-architecture-contract.md §3 L21 §6 L57`
- **ANCH-3 年龄/彩蛋/触发枚举**：`ageTag` ∈ {`Baby`, `Toddler`, `Child`, `Adult`}（缺省 = 全年龄；`Toddler` 为 ABI 桶，RimWorld 1.6 无原生阶段）；映射：Newborn/Baby → Baby、Child → Child、其他/缺失 → Adult；周期方式 ∈ {`EachTime`, `RandomOneShot`, `External`}。`src: docs/project-architecture-contract.md §3 L23 §6 L53`
- **ANCH-4 默认值 / 阈值 / 计数（数值）**：`weight` 默认 `1`；`allowEasterEggSounds` 默认 `false`；`eatOnlyDuringChewing` 默认 `false`；`eatIncludeDrugs` 默认 `false`；啤酒 `Nutrition 0.08`；ambrosia `Nutrition 0.2`（营养权威 `CachedNutrition > 0`）；出厂默认自 0.2.3 = Fallback + 内置 Race Example 启用。`src: docs/project-architecture-contract.md §3 L25 L29 L31 §6 L53 L57`
- **ANCH-5 日志字段序（整段逐字，顺序即合同）**：v1 核心 = `fmt lvl vis evt action target pack build build_id`；v1 事件专属固定序 = `reason sound source count dispatched suppressed_detail enabled ex_type ex_inner ex_site ex_msg`；v2 核心 = `fmt lvl vis evt action target pack race [xenotype] build build_id`；percent-encode 白名单 = `A-Z`、`a-z`、`0-9`、`.`、`_`、`~`、`:`、`/`、`@`、`+`、`-`；事件级 v2 字段序 —— `audio.route.selected` = `sound` `tier` `egg` `suppressed_detail` `pawn` `pawn_id` `pawn_faction` `pawn_ctrl`；`fallback.profile.store_failed` = `ex_type` `ex_inner` `ex_site` `ex_msg`；`settings.origin` = `settings_origin=FreshCreated|LoadedFromFile`；`hook.mental_fit.unavailable` = 无专属字段。`src: docs/logging-protocol.md L31 L34 L40 L41 L44 L52 L55 L62 L66 L67 L69`
- **ANCH-6 注册表计数与 reason 集合**：v1 锁定记录 = **28** 行（fmt=1，byte-immutable）；0.3.1 v2 扩展记录 = **4** 行（`settings.origin`、`audio.route.selected`、`hook.mental_fit.unavailable`、`fallback.profile.store_failed`）；表内合计 32 行（本包按表行实测）。reason 闭包：`voicepack.pack.rejected` ∈ {`duplicate_key`, `domain_filtered`}；`xenotype.discovery.candidate` ∈ {`har_hint_filtered`, `har_official_filtered`}；源集合 ∈ {`declared_pack`, `selection`, `preset`}，以 `+` 连接。`src: docs/logging-protocol.md L62 L64 L66 L68 L69 L85 L87 L120 L122 L124 L126`
- **ANCH-7 距离 / UI / 冷却数值**：底层 SoundDef range `15–70` cells；玩家设置默认 Balanced 预设 `15–50`；`CurrentViewRect.ExpandedBy(10)`；七次点击解锁（第四页）；合并保存约 `350 ms`；相机指示器读 `Find.Camera.transform.position.y` 与 `Find.Camera.orthographicSize`（禁止由 `RootSize` 反推）。`src: docs/project-architecture-contract.md §4 L39`；`src: docs/settings-ui-product-contract-zh.md §产品表面 L7 L11 L15`
- **ANCH-8 日志量控数值**：once-key 上限 `1024`（达上限即清空并接受新键）；每动作细节最多 `每 5 秒` 一条；`trigger.outcome.summary` 至多 `每 60 秒` 一条；异常消息截断 `256` 字符；三个模式值 `Auto`/`Enabled`/`Disabled`。`src: docs/logging-protocol.md L15 L17 L19 L21 L46 L79 L81`
- **ANCH-9 设置 UI 最小验收矩阵（逐字维度）**：语言与比例 = EN、简体中文；`100%`、`125%`、`150%`；空间 = normal、narrow、高度受限（无重叠、负 Rect、不可达控件、滚动串层）；状态 = 主菜单、游戏内、无选择、无 Biotech、可用/空/orphan/dormant/失败 Catalog；输入 = slider、文本、列表、`?`、折叠、Remix 两步确认及每层取消/关闭无穿透；持久化 = 立即运行时生效、约 350 ms 合并保存、close flush、重开/重启保留、失败状态诚实；运行时 = 不引发 GUIClip/ScrollView 配对、NRE 或 Unity 主线程错误，No-DLC 不访问 Biotech 路径。`src: docs/settings-ui-product-contract-zh.md §最小验收矩阵 L35 L37 L38 L39 L40 L41 L42`
- **ANCH-10 授权边界与外部状态边界**：
  - 维护者独有：内置 fallback 档案内容（C# 数据）；staging 不变式的"上报不静默修复"口径。`src: docs/project-architecture-contract.md §6 L57 §7 L67`
  - 玩家可执行的唯一破坏性出口：确认式「忘记此目标」（连带删除行为 preset + Xenotype VoicePack 选择）；普通 catalog refresh **不得**删档；Remix 需两步确认。`src: docs/project-architecture-contract.md §6 L61`；`src: docs/settings-ui-product-contract-zh.md §产品表面 L11 L13`
  - 发行格式建议边界：不得把"运行时校验器/游戏兼容性"表述为"发行建议"（OGG 建议、WAV 可用非建议、MP3 非建议）。`src: docs/project-architecture-contract.md §7 L69`
  - 第三方碰撞边界：项目只校验自身发行 root 与重复扩展名，不全局仲裁第三方 patch/碰撞。`src: docs/project-architecture-contract.md §7 L71`
  - 排障工具跨 flavor 边界：必须编译进 Dev/GitHub/Steam 全部 flavor，但可用性仍受 RimWorld Dev Mode 与活动地图门控。`src: docs/settings-ui-product-contract-zh.md §产品表面 L15`
  - 语料外硬约束指针（不要求包内重建）：[xref: AGENTS.md#（操作硬约束索引，含卸载安全）]。
## 10. 候选教训（LES-<n>）
| id | 教训 | 支撑证据 | 强度 | 适用边界 | 来源 |
| --- | --- | --- | --- | --- | --- |
| LES-1 | 把"兼容面"写成**闭合枚举 + 固定字段序 + fails-closed 校验**，可以让下游机械判定违规（而非靠人读散文） | DEC-31（注册表与字段序即兼容面）、DEC-14（公开 ABI 逐项枚举 + add-only + append-only + fails closed）、DEC-33（缺失写 `-`、未知字段当扩展） | 强（三份合同一致采用同一手法） | 仅适用于**对外可解析产物**（日志、作者 XML ABI）；对内部实现面合同反而刻意保留 0.x 修订自由，不可套用 | `src: docs/logging-protocol.md L7 L44 L50`；`src: docs/project-architecture-contract.md §6 L55` |
| LES-2 | 规范文档应当**自我声明证据边界**：本合同明确"是机读/兼容合同，不是实机会话测试证据"，避免下游把条文升格为验证结论 | DEC-31 / C-1 / EV-1（同句自限） | 强（文本明示） | 适用于任何以"源审阅"为来源的规范文本；不能替代真实回归证据 | `src: docs/logging-protocol.md §Status L3` |
| LES-3 | 冻结声明必须绑定**可指认的版本锚点**；用"first released version that carries X"这类描述性锚会造成起点不可判定 | CNF-8 / OQ-1（冻结起点在语料内无法闭合） | 中等（本包单点观察；跨包是否普遍需阶段 B 判定） | 适用于一切"自某版本起稳定"的承诺；对已用显式版本号 + 日期的条目（DEC-14 的 2026-08-22 行）不适用其前半 | `src: docs/project-architecture-contract.md §6 L55` |
| LES-4 | 在同一份文档里**追加新版本事实却不更新状态行**，会让时间层失效（状态戳早于内容版本层） | CNF-3（状态戳 2026-08-22 却记载 0.3.2；`(冻结动作)` 显示末次提交 2026-08-23） | 中等 | 适用于多会话追加式文档；单次成文的文档不适用 | `src: docs/logging-protocol.md L3`；`(冻结动作) git log -1 docs/logging-protocol.md` |
| LES-5 | 通则式禁令若不与后续 schema 扩展联动复核，会与自家字段表直接互斥（"禁写 pawn 标签" vs `pawn=<label>`） | CNF-1 / INC-4 | 强（同文件两行互斥，文本自证） | 适用于任何含"不得写入 X"通则的机读协议 | `src: docs/logging-protocol.md L46 L59 L67` |
| LES-6 | tentative：产品层分段语义若在可观测层被折叠，兼容承诺仍成立但**排障能力**会静默下降；应在折叠处显式声明"哪个产品层不可归因" | CNF-2 / INC-3（tier 折叠，合同未标注归因损失） | tentative（仅一处证据，且无法排除"折叠即有意设计"的解释 → U-1） | 适用于 tier/枚举映射类设计；不构成"必须拆细"的普适结论 | `src: docs/logging-protocol.md L73` |
| LES-7 | 用**否定式**表述废止口径（"Do not reintroduce…"、"must not be narrowed by default"、"never calls WriteSettings()"）能把落选方案的硬细节留在现行文本里，避免决策史丢失 | DEC-9（zoom 门）、DEC-7（默认粒度）、DEC-16（`WriteSettings()`/防抖队列）；ALT-1（generic framework 否弃） | 强（多处一致手法，且给出理由句） | 适用于合同类规范文本；事故细节与原因仍需过程文档承载（本包无事故叙述 → 见 §12 说明） | `src: docs/project-architecture-contract.md §3 L25 §4 L39 §6 L59` |

## 11. 证据缺口（GAP-<n>）
| id | 想回答的问题 | 为什么现有语料答不了 | 需要什么才能回答 |
| --- | --- | --- | --- |
| GAP-1 | 三份合同的条文是否与当前实现逐条一致？ | 合同自述来源是"源审阅"且明文声明**不是**实测证据；本包按规则不做仓外/代码取证 | 一次实现-合同对照记录（等价评审类）或 harness 用例清单 → 可能在 [xref: PKG-2（等价评审 14 行）] |
| GAP-2 | `audio.route.selected` 的真实输出行形状（字段是否齐、`pawn` 实际值形态）？ | 语料只有 schema，无样例记录（且本包禁止复制日志摘录） | 一份已净化的样例日志或机读解析测试 |
| GAP-3 | 0.3.2（及 0.3.2-pre1）是否正式发行、在哪些渠道？ | 发布事实不在本包主源内 | [xref: PKG-4（各渠道状态矩阵）] |
| GAP-4 | 诊断叠加层（`SqueakDebug` / workbench）是否暴露真实 fallback 层？ | 合同只规定日志 tier 折叠，未描述 overlay 显示内容 | 读 `Debug/SqueakDebug.cs` 或 overlay 相关规范（语料外） |
| GAP-5 | Eat 三级模式是否有回归测试守护（合同要求"must stay documented and tested"）？ | 本包主源只声明要求，未记测试标识（U-2） | 测试用例清单或 CI 记录 → 可能在 [xref: PKG-2（C1–C19 证据链）] |
| GAP-6 | `Toddler` 桶是否存在任何可发行路径（有 mod 提供该阶段时行为如何）？ | 合同只记"1.6 无原生阶段" | 跨 mod 兼容性评估 → [xref: PKG-3（F1–F8 / U1–U4）] |
| GAP-7 | 玩家 settings 文件是否需要版本号/世代字段（DEC-16 的"保留待保存代次"暗示存在代次概念）？ | 合同未定义 settings schema 版本机制 | settings 持久化实现或规范补充 |

