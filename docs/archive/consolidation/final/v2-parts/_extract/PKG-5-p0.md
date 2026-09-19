## 4. 假设账本（ASM-<n>）

| id | 假设（要旨） | 提出时间 | 是否仍成立 | 变更/失效点 | 验证或反证 | 来源 |
|---|---|---|---|---|---|---|
| ASM-1 | 债务不含凭据、形态仅为目录名 ⇒ 不重写的风险可接受 | 2026-09-13 | 未证伪（但依赖「目录名外露可接受」这一价值判断，属维护者裁量） | Q1 判定变化即失效 | 凭据 0 命中、`PublishedFileId` 值 0 命中（门实测） | `src: privacy-history-rewrite-plan-zh.md §1 L9、L12；§4 L49` |
| ASM-2 | `v0.2.0` 是唯一内容干净的发布 tag | 2026-09-13 | 与「10 个发布 tag 全部指向受影响提交」冲突（CNF-4），成立性待回查 | 若 tag 计数口径确认，则二者之一需修订 | 无独立反证 | `src: privacy-history-rewrite-plan-zh.md §3 L33、L38 与 §1 L10` |
| ASM-3 | 本仓单人维护 ⇒ 重写协调窗口风险可控 | 2026-09-13 | 未验证（依赖仓库无其他贡献者这一未记事实） | 若出现协作者即失效 | 无 | `src: privacy-history-rewrite-plan-zh.md §6 L91` |
| ASM-4 | 本地门成本可忽略（`verify-local` 约 11 秒）⇒ CI 加门可行 | 2026-09-13 | 成立（本机实测），但 CI runner 上耗时未测 | CI 首跑 | `pwsh scripts/verify-local.ps1 -NoRestore` 11/11 全绿 EXIT 0 | `src: process-redundancy-review-zh.md §5 L72、§7 V3 L92` |
| ASM-5 | SR 与 US 的差异是产品面的、不是门禁方式的 ⇒ 可迁移「门禁归属」而非清单内容 | 2026-09-13 | 成立（作为评估前提被沿用） | 若 SR 出现 US 无对应物的门（N2 已排除 carrier 门） | 全文按此执行 | `src: process-redundancy-review-zh.md §2 L33、§3 N2 L55` |
| ASM-6 | US 2026-09-06 最小仪式裁决可作 SR 基准（含「禁止重新加回人工仪式」） | 2026-09-06 立、2026-09-13 引 | 成立 | 无 | 落 `AGENTS.md` 增补 | `src: process-redundancy-review-zh.md §2 L30、§4 L65` |
| ASM-7 | 「人工清单会漏、脚本不会漏」 | 2026-09-13 | 成立（本会话有一次正向实例） | 无 | `dist/` 里 3 个陈旧暂存包被新读时门标 `[note] stale artifact, do not upload it`，文档称「这正是『人工清单会漏、脚本不会漏』的实例」 | `src: process-redundancy-review-zh.md §6 L83` |
| ASM-8 | 债务只在旧版本（HEAD 已净化）⇒ 普通新提交无法消除，必须历史重写 | 2026-09-13 | 成立（三向量独立扫的工作树 0 命中支持） | 若工作树再引入同类串则门会直接 FAIL | 工作树 0 命中 + `-FullHistory` 5 条 `[known-debt]` | `src: privacy-history-rewrite-plan-zh.md §2 L26；§1 L9` |
| ASM-9 | 离线/受限网络是常态 ⇒ 新门须透传 `-NoRestore` 并在 runbook 写明 | 2026-09-13 | 成立（本轮实测触发） | 无 | `NU1301` → `MSB3644` 事故链 | `src: process-redundancy-review-zh.md §8 L101` |
| ASM-10 | 文档自身若复制债务串会触发门禁 ⇒ 脚本以拼接构造，文档只给形态与计数 | 2026-09-13 | 成立（两份文档均自我声明无隐私写法） | 无 | 「上面的正则按需自建，本文不复制债务字符串；`+` 与路径形态在脚本里以拼接方式构造，避免文档自身触发门禁」 | `src: privacy-history-rewrite-plan-zh.md §8 L115；§前言 L5；process-redundancy-review-zh.md §前言 L6` |
| ASM-11 | `git filter-branch` 内置可用 ⇒ 无网络也有 fallback 重写路径 | 2026-09-13 | 未验证（"可用"= 存在性判断，非成功性判断） | P1 实跑 | 无 | `src: privacy-history-rewrite-plan-zh.md §3 L41、§5 L60` |
| ASM-12 | 接受「GitHub 可能仍可按旧 SHA 访问」的残余风险（建议默认） | 2026-09-13 | 待裁决（Q4 未决） | Q4 裁定 | 无 | `src: privacy-history-rewrite-plan-zh.md §6 L87、§7 Q4 L100` |

---

## 5. 认识论分账（只列 C / I / U）

- **参与者声明 C-<n>**
  - **C-1**：US 侧「2026-09-06 最小发布仪式裁决」（含禁止重新加回人工仪式）由 SR 文档转述，非本仓一手记录；`[C]` 时间 2026-09-13 引用、2026-09-06 裁决时点。独立支持：无（US 仓文件不在语料内）。`src: process-redundancy-review-zh.md §2 L30`
  - **C-2**：「本会话已落地」7 项文件变更是 agent 对自己工作的声称 `[C]`（2026-09-13），文档未给 diff/提交号；部分由 §5 实测证据间接支持（EV-2 证明读时门存在且可跑），`AGENTS.md`/workflow 内容本身未在本包主源内复核。`src: process-redundancy-review-zh.md §4 L60–L68`
  - **C-3**：「维护者选择『先出专项方案』」（V2 归属维护者裁决）是文档转述的维护者口径 `[C]`（2026-09-13）。`src: privacy-history-rewrite-plan-zh.md §前言 L4；process-redundancy-review-zh.md §7 V2 L91`
  - **C-4**：「本机实测约 11 秒」「本机 NuGet 源曾 SSL 失败」是会话内环境声称 `[C]`（2026-09-13），无第三方复现。`src: process-redundancy-review-zh.md §5 L72；privacy-history-rewrite-plan-zh.md §5 L60`
  - **C-5**：「台账截止：2026-09-13 起的流程裁决不再追加到本文」未具名裁决主体 `[C]`。`src: process-review-zh.md §前言 L5`
  - **C-6**：0.2.4 收尾阶段「已实现」标注（措施 4/5/6/7）由台账自述，本包未回查 runbook/草稿文件现行文本 `[C]`。`src: process-review-zh.md §本次复盘新增 L19–L22`

- **本包推断 I-<n>**
  - **I-1**：§1「待裁决 3 项」= V1–V3 三项。推断链：§1 判定行为「采纳 5 / 待裁决 3 / 不做 3」→ §7 恰列 V1、V2、V3 且标题为「2026-09-13 定案」→ §9 第 3 条要求「把『待裁决』改为『已定案』」。替代解释：3 项指 R 清单中另三条未落地项（但 R1–R12 的处置词均无"待裁决"字样）。`src: process-redundancy-review-zh.md §1 L21、§7 L88–L92、§9 L107`
  - **I-2**：受影响 tag 数若按 §3 表逐条并集计算，得 9 个（0.1.x 三个 + 0.2.1/0.2.2/0.2.3/0.2.4/0.3.0/0.3.2-pre1），与「10 个 tag」总数相差恰为 `v0.2.0`。推断链：§3 两行显式列名 + §3 L38「`v0.2.0` 是唯一内容干净的发布 tag」。替代解释：受影响面为 10、tag 总数为 11（含未推送的 `archive/dev-pre-sanitize-0.2.0`）。→ 本包采 CNF-4 并列、不择一。`src: privacy-history-rewrite-plan-zh.md §3 L32–L33、L38、§1 L10`
  - **I-3**：§5「负向自测（隔离的一次性仓库，注入合成泄露串后自毁）」的措辞很可能是 INC-9 事故**之后**的补救口径。推断链：§8 承认本轮出现过失败路径把探针提交写进真实仓库 → 现描述强调「隔离」「一次性」「自毁」三项防护。替代解释：自测一开始就在隔离仓库做，§8 说的是另一次操作。`src: process-redundancy-review-zh.md §5 L76、§8 L99`
  - **I-4**（归类时点 2026-09-17；被归类材料时点 2026-09-13）：R1–R3/R7/R8 属「人工表述收敛」类，R4 属「覆盖新增」类，R10–R12 属「豁免」类——三分法是本包对 12 条裁决的结构化归类。推断链：各条「冗余性质」列措辞（复制型冗余 / 覆盖缺口 / 产品决策 / 认证边界 / 人的判断）。用途：供阶段 B 按后果分配篇幅。`src: process-redundancy-review-zh.md §3 L39–L50`
  - **I-5**：措施 1 与措施 2 的根因（「图快绕过门控」「分支纪律松弛」）与 DEC-6 的「人工纪律不可靠」是同一诊断在两个时点的表述。推断链：0.2.x 用「加检查项」回应、2026-09-13 用「换执行者」回应；R6 又把台账定为历史面。`src: process-review-zh.md §已记录的三项 L11–L12；process-redundancy-review-zh.md §1 L12–L19、§3 R6 L44`

- **未决 / 缺失 U-<n>**
  - **U-1**：10 tag 与「`v0.2.0` 干净」的张力（缺的是：受影响 tag 的**定义与枚举口径**，而非数据）→ 直接影响 DEC-15 的 P4「核对 10 个 tag」验收判据与"重写代价"报价。`src: privacy-history-rewrite-plan-zh.md §1 L10、§3 L33–L38、§5 L72`
  - **U-2**：227 revision（扫描面）vs 230 提交（全仓）之间 3 个 revision 的归属未记（缺的是计数方法说明）→ 影响"最早脏点/漂移"数字的可复核性。`src: process-redundancy-review-zh.md §5 L75；privacy-history-rewrite-plan-zh.md §3 L38`
  - **U-3**：54 处仓内 hash 引用无逐条清单（只给「`MEMORY.md`/`OBLIVIONIS.md`/`TODO.md`/`docs/0.3x-release-gate-checklist-zh.md` 等」）→ 缺"完整位置集合"，P6 第 12 步无法验收。`src: privacy-history-rewrite-plan-zh.md §3 L39`
  - **U-4**：SR mirror 备份是否已在 2026-09-13 之后建立，语料无记录 → P0 前置状态未知。`src: privacy-history-rewrite-plan-zh.md §3 L42、§5 L56`
  - **U-5**：CI 加门后的实际运行结果（V3 的"真验"），语料明写未验证 → DEC-9 的"已实施"只到配置层。`src: process-redundancy-review-zh.md §5 L77、§7 V3 L92`
  - **U-6**：措施 8（玩家评论→TODO 通道）在台账冻结后是否落地，语料无后续。`src: process-review-zh.md §本次复盘新增 L23`
  - **U-7**：`modding_documents/privacy-debt-vector-triage-zh.md` 属哪一仓、其 §3 的完整禁止改动清单，本包主源不含 → P2 第 5 步的规则集只是摘要。`src: privacy-history-rewrite-plan-zh.md §5 L64；process-redundancy-review-zh.md §2 L29`
  - **U-8**：0.2.4 各渠道包是否重发以补 `version.txt`，语料只写「由维护者定」，无裁决结果。`src: process-review-zh.md §本次复盘新增 L22`
  - **U-9**：三份主源与 `AGENTS.md`/`MEMORY.md`/`TODO.md` 的回写一致性（§9 第 3 条要求回写记忆面），记忆面不在语料内，无法核对。`src: process-redundancy-review-zh.md §9 L107`

---

## 6. 矛盾与反转（CNF-<n>）

| id | 冲突双方（含来源） | 各自时间层 | 冲突点 | 可能解释 [I] | 建议回查点 |
|---|---|---|---|---|---|
| CNF-1 | src: `process-redundancy-review-zh.md §1 L21`「采纳 5 项（已落地）、**待裁决 3 项**、明确不做 3 项」 vs 同文 `§7 L86–L92`「裁决记录（2026-09-13 **定案**）V1/V2/V3」；同文 `§9 L107` 要求「V1–V3 裁决后回写本文件（把『待裁决』改为『已定案』）」 | §1 写于裁决前、§7 写于裁决后（同日；亦即 §1 未回写） | 同一份文档同时呈现"3 项待裁决"与"3 项已定案" | [I] §1 是评估摘要的时间残层；[I-1] 3 项 = V1–V3 | 以 §7 为最新口径但**不得删 §1 行**；阶段 B 须同时保留两层 |
| CNF-2 | src: `process-redundancy-review-zh.md §前言 L5`「**门禁覆盖没有缺失**，冗余在『谁来做』」 vs 同文 `§3 R4 L42`「身份面从未单列检查」/「冗余性质：**覆盖缺口（不是冗余）**」 | 同一会话（前言为总结层，R4 为清单层） | "覆盖无缺失"与"存在从未检查的向量"并存 | [I] 前言的"覆盖"指既有清单类不变量，身份面属**新增向量**而非既有清单缺项；[I] 或前言口径偏乐观 | 核 `privacy-audit.ps1` 是否把身份面作为独立第五向量（语料外 → GAP-1） |
| CNF-3 | src: `process-redundancy-review-zh.md §5 L75 / §6 L81`「**227 个 revision** 全扫」 vs src: `privacy-history-rewrite-plan-zh.md §3 L38`「全仓 **230 提交**」 | 同为 2026-09-13，不同会话产物 | 同一仓库两个总数（差 3） | [I] 扫描用 `git log --all` 类口径（不含某些 ref 或含 merge 差异）；[I] 两者在不同分支/引用集合快照之间 | 不自行调查（规则 9）；回查需 `git rev-list --count --all` 与门实现 |
| CNF-4 | src: `privacy-history-rewrite-plan-zh.md §1 L10`「10 个发布 tag **全部指向受影响提交**」+ 同文 `§5 L72`「核对 **10 个 tag** 名不变」 vs 同文 `§3 L33`「（`v0.2.0` 干净）」+ `§3 L38`「`v0.2.0` 是**唯一内容干净**的发布 tag」 | 同一文档内部（§1 摘要 vs §3 量表） | 受影响 tag 是 10 还是 9；tag 总数是否含 `v0.2.0` | [I-2] 摘要把总数当受影响数；[I] "10"含未推送的 `archive/dev-pre-sanitize-0.2.0`（`§3 L43`） | 按冻结 revision 枚举 `git tag -l` 并逐 tag `git grep -l -I -E`（本包禁外部取证 → GAP-3） |
| CNF-5 | **反转**：src: `process-redundancy-review-zh.md §6 L81`（新口径，实测「命中 **5 个文件**——`.slim/codemap.json` 与 4 份 HEAD 已净化文档（`AGENTS.md`/`CONTRIBUTING.md`/`MEMORY.md`/`TODO.md`）的旧版本」；该行同时自述旧口径「既记录只提 `.slim/codemap.json`」） | 旧口径不晚于 2026-09-13 之前；新口径 2026-09-13（`-FullHistory` 实扫） | 债务范围 1 文件 → 5 文件（波及面须重估） | [I] 旧记录只覆盖工作树/单向量，历史 blob 向量是新门才有的能力 | 新口径为准但**旧口径单独保留**（它解释了为何 V2 必须重估）；核 `$knownHistoryDebt` 台账 5 条 |
| CNF-6 | src: `process-redundancy-review-zh.md §5 L76`「负向自测（**隔离的一次性仓库**，注入合成泄露串后自毁）」 vs 同文 `§8 L99`「本轮出现过一次『负向自测的**失败路径把探针提交写进了真实仓库**』的事故」 | 同一会话内两个记录层（§5 结果层、§8 风险层） | 自测究竟在隔离仓库还是曾污染真实仓库 | [I-3] §5 描述的是事故后重做的最终形态；[I] 两类自测各做了一次 | 回查 `dist/` 与工作区 reflog 不在本包面；以 LES-6 的边界表述保留 |
| CNF-7 | src: `process-review-zh.md §前言 L3`「每条含根因与**已落地**的优化措施。措施以『已实现 / 待实现』标注」 vs 同文 `§已记录的三项 L7` 表头直接把状态写进列名「优化措施（**已实现**）」 + `§本次复盘新增 L23` 措施 8 为「待实现」 | 0.2.4 收尾（2026-09-13 冻结前） | 「已落地」的总述与逐行状态标注不完全同构；前 3 项的"已实现"由表头而非逐行担保 | [I] 表格是两次增补的产物（3 项旧 + 5 项新），表头措辞未随之统一 | 无实质后果；阶段 B 引用措施状态时以**逐行标注**为准 |
| CNF-8 | src: `process-review-zh.md §本次复盘新增 L23`（措施 8 = 心智清单，无脚本） vs src: `process-redundancy-review-zh.md §前言 L5 / §1 L16`（机械可判定的不变量不该留人工表述；隐私审查「4 处人工表述、**0 脚本**」被当作冗余病灶） | 措施 8 属 0.2.4 收尾；总纲属 2026-09-13 | 「疑点记入 TODO 的 Comments 扫描」按 2026-09-13 口径属同类人工表述，却未被 R1–R12 清单覆盖 | [I] 该动作依赖对反馈的语义判断，不属"机械可判定不变量"；[I] 台账已冻结（R6）使评估未回看旧措施 | 若阶段 B/C 要提议"给它也加门"，须先确认其是否机械可判定 |

---

## 7. 事故 / 失败 / 成功与后果（INC-<n> / EV-<n>）

| id | 类别 | 现象 | 根因（若知） | 处置与结果 | 证据强度 | 来源 |
|---|---|---|---|---|---|---|
| INC-1 | 事故（措施 1） | 测试用单 DLL 替换 + **误用 Steam flavor 打开发包** | 图快绕过门控 | 开发态测试 = Dev flavor + `pack-dev` 整包替换，写入 runbook 阶段 0（已实现） | 台账记录（传闻/复盘）`[C]` | `src: process-review-zh.md §已记录的三项 L11` |
| INC-2 | 事故（措施 2） | 开发提交误落 `main` | 分支纪律松弛 | 开发/收尾提交一律 `dev`，`main` 只收 PR squash merge；误落即 cherry-pick 回 `dev`（已实现） | 台账记录 `[C]` | `src: process-review-zh.md §已记录的三项 L12` |
| INC-3 | 回归（措施 3） | changelog 双语**连续两次**三方冲突（0.2.1 教训 (d) 复发） | 发布收尾后 `dev` 未及时吸收 `main` 指针 | 收尾立即 `merge origin/main` 对账；`main` squash 树与 `dev` merge 树相同时用 `git merge -s ours`（已实现） | 台账记录，含"复发"事实 `[C]` | `src: process-review-zh.md §已记录的三项 L13` |
| INC-4 | 事故（措施 4） | workshop 草稿版本号/下载链接停留在 0.2.3，fits 说明 PR 后才发现 | 草稿无「版本号 = 发布版本」一致性检查 | 草稿维护规则 + 检查清单加「发布后立即 bump 版本号与链接」（已实现，见 `steam-workshop-page-copy-draft.md`） | 台账记录 `[C]` | `src: process-review-zh.md §本次复盘新增 L19` |
| INC-5 | 事故（措施 5） | 自动化误尝试进入 Steam 编辑界面（relay 驱动登录态浏览器） | 未明确页面更新认证边界 | runbook 阶段 3 第 15 条改为玩家视角只读：`read` filedetails 页面核验，绝不尝试登录态/编辑界面；编辑 = 维护者人工（已实现） | 台账记录 `[C]` | `src: process-review-zh.md §本次复盘新增 L20` |
| INC-6 | 流程返工（措施 6） | 披露段内容变更与版本号更新**分两次 PR（#17/#18）** | 一次变更未跑完整一致性检查 | 任何草稿变更后统一核对：版本号、下载链接、字符数、检查清单（已实现，见措施 4 条目） | 台账记录（PR 号可核）`[C]` | `src: process-review-zh.md §本次复盘新增 L21` |
| INC-7 | 缺陷（措施 7） | 包内无版本标识：dev 有包外 label，GitHub/Steam 包无从确认新旧 | 打包链只写 DLL 身份，无独立明文标识 | 包内 `version.txt` 三渠道统一（`SqueakyRatkin <版本>` / `build=<flavor>` / `commit=<sha>`，dev 带 `-dirty`），写入 `stage-package.ps1`，核验清单同步；0.2.5/0.3.x 起生效；已发布的 0.2.4 各渠道包不含 | 台账记录 `[C]` | `src: process-review-zh.md §本次复盘新增 L22` |
| INC-8 | 缺口（措施 8） | 反馈闭环依赖人工发现：玩家「鼠蛋笑像杀猪」评论与修复关联晚 | 无评论→TODO 通道 | 页面核验时顺带扫 Comments 区未处理反馈，疑点记入 TODO（**待实现**，纳入 runbook 第 15 条执行时的心智清单） | 台账记录 `[C]`；落地状态 `[U]` | `src: process-review-zh.md §本次复盘新增 L23` |
| INC-9 | **事故（自造）** | 负向自测的**失败路径把探针提交写进了真实仓库** | 门的自测未在一次性仓库里做（教训表述） | 已 `reset` + `gc --prune` 清除，**HEAD 未受影响**；教训固化为「门的自测必须在一次性仓库里做」；文档指出 `[claim] localHead` 输出能立刻发现异常 | **本会话实测**（同会话亲历，结果含"HEAD 未受影响"声称）`[C/F 混合]` | `src: process-redundancy-review-zh.md §8 L99` |
| INC-10 | **环境陷阱（实测）** | 离线环境隐式 restore 破坏构建态：csproj 用浮动版本 `1.6.*`，无网络时 `dotnet build`（不带 `--no-restore`）以 `NU1301` 失败并**覆写 `obj/project.assets.json`**，此后所有 `--no-restore` 构建报 `MSB3644`（找不到 net472 引用程序集） | 浮动版本 + 隐式 restore 在无网络下的副作用（文档定性为**环境事实，不是门缺陷**） | 直到从本机 NuGet 全局包缓存做一次离线还原（以缓存目录为 `--source`）才恢复；缓解 = 新门提供 `-NoRestore` 透传并在 runbook 写明 | **实测/复跑**（本轮实测，有错误码链） | `src: process-redundancy-review-zh.md §8 L101` |
| EV-1 | 验证 | `pwsh scripts/verify-local.ps1 -NoRestore`：**11/11 全绿**（EXIT 0；本机约 11 秒） | — | 支撑 ASM-4 与 CI 加门成本判断 | 实测 | `src: process-redundancy-review-zh.md §5 L72` |
| EV-2 | 验证 | `pwsh scripts/check-pack-readiness.ps1 -RequireReleaseMetadata -NoRestore`：**all checks passed**（EXIT 0；组合 verify-local 11/11 + 全部发布面检查 + `[claim]` 快照） | — | 支撑 DEC-7 | 实测/复跑 | `src: process-redundancy-review-zh.md §5 L73` |
| EV-3 | 验证 | `pwsh scripts/privacy-audit.ps1`（默认）：CLEAN（EXIT 0） | — | 工作树向量干净 | 实测 | `src: process-redundancy-review-zh.md §5 L74` |
| EV-4 | 验证 | `pwsh scripts/privacy-audit.ps1 -FullHistory`：**227 个 revision 全扫** CLEAN（EXIT 0）——未接受命中 0；已知债务 **5 条**以 `[known-debt]` 列出 | — | 触发 CNF-5 反转与 V2 专项方案 | 实测 | `src: process-redundancy-review-zh.md §5 L75、§6 L81` |
| EV-5 | 验证（负向） | 负向自测（隔离的一次性仓库，注入**合成**泄露串后自毁）：三向量 + 身份面**全部触发、EXIT 1** —— 「门不是橡皮图章」 | — | 支撑 DEC-8 与 LES-6 边界（见 CNF-6） | 实测 | `src: process-redundancy-review-zh.md §5 L76` |
| EV-6 | 验证（外部状态） | `gh repo view` 核实 `visibility=PUBLIC`、`isFork=false` | — | 把债务定性为"真实外露而非理论风险" | 实测（一次性，无后续复核） | `src: privacy-history-rewrite-plan-zh.md §1 L11` |
| EV-7 | 验证（可达性） | `git ls-remote` 正常（远端可达）；`git 2.47.1`；`git-filter-repo` 未安装；`git filter-branch` 可用 | — | 方案 B 工具面前提（ASM-11） | 实测 | `src: privacy-history-rewrite-plan-zh.md §3 L41` |
| EV-8 | 成功（文档一致性） | R9 已修正：runbook 模板路径统一到 `docs/release_review/` | 模板与实际目录不一致 | 已修正 | 台账声称 `[C]` | `src: process-redundancy-review-zh.md §3 L47` |
| EV-9 | 发现（不变量适配） | 身份面 **3 个唯一身份**，全部 GitHub noreply（维护者两个显示名 + GitHub 自身的 bot committer）；US 的「身份必须唯一」断言不适用 SR，SR 断的是隐私相关不变量（全部 noreply），显示名漂移与 bot 提交不误伤 | — | 修正 DEC-8 的 R4 移植方式 | 实测（扫描） | `src: process-redundancy-review-zh.md §6 L82` |
| EV-10 | 发现（门的正向价值） | `dist/` 里有 **3 个陈旧暂存包**（dev `0.3.2-EXP`、steam `0.3.0`、github `0.3.2-pre1`），新读时门把它们标为 `[note] stale artifact, do not upload it` | 人工清单会漏 | ASM-7 的实例支撑 | 实测 | `src: process-redundancy-review-zh.md §6 L83` |
| EV-11 | 发现（形状差异） | 本地 DLL 身份 `FileVersion=0.3.3.0`、`ProductVersion=0.3.3+<40 位 sha>`；release 通道由 CI 显式注入 `v<tag>+<sha12>`；二者形状不同 ⇒ 读时门只断言 `FileVersion` **全等** + `ProductVersion` **含**产品版本 | — | DEC-4/DEC-7 的断言口径依据 | 实测 | `src: process-redundancy-review-zh.md §6 L84` |
| EV-12 | 交接诚实边界 | 「未验证面：CI 改动要推送后才实际运行；Steam 渠道本轮不涉及。**外部渠道状态一律不因本文件而视为已验证**」 | — | 约束下游不得据本文宣称渠道状态 | 文档明述 `[F]` | `src: process-redundancy-review-zh.md §5 L77` |

---

## 8. 开放问题、阻塞与交接风险（OQ-<n>）

| id | 问题 / 风险 | 类型 | 影响面 | 当前状态 | 来源 |
|---|---|---|---|---|---|
| OQ-1 | 已发布的 0.2.4 各渠道包不含 `version.txt`，**是否重发** | 待裁决 | 已发布分发物一致性、玩家侧辨识包新旧 | 台账把决定权交维护者，无裁决记录 | `src: process-review-zh.md §本次复盘新增 L22` |
| OQ-2 | 措施 8（Comments 区反馈 → TODO 通道）是否已落地 | 未验证 | 玩家反馈闭环 | 台账冻结时仍「待实现」，其后语料无记录（U-6） | `src: process-review-zh.md §本次复盘新增 L23` |
| OQ-3 | CI 加门 + SDK 10.0.x 的**首跑**结果 | 未验证 / 依赖外部（需 push 授权） | 门禁真实覆盖 | 配置已写入，「推送后首跑即真验」 | `src: process-redundancy-review-zh.md §5 L77、§7 V3 L92` |
| OQ-4 | **Q1** 是否执行方案 B？（路径外露 vs 不可逆成本） | 待裁决 | 全历史、10/9 tag、跨仓 | 建议默认：先 A；B 仅在判定路径不可接受时执行 | `src: privacy-history-rewrite-plan-zh.md §7 L97` |
| OQ-5 | **Q2** 若执行 B：用 `git-filter-repo`（需联网安装）还是内置 `filter-branch`？ | 待裁决 | 重写可行性与耗时 | 建议默认：先验证安装；否则 filter-branch | `src: privacy-history-rewrite-plan-zh.md §7 L98、§5 L60` |
| OQ-6 | **Q3** 若执行 B：替换占位用什么（如 `<local-path>`）？ | 待裁决 | 门自身扫描、后续文档写法 | 建议默认：中性占位，不写任何盘符形态 | `src: privacy-history-rewrite-plan-zh.md §7 L99、§5 L63` |
| OQ-7 | **Q4** 是否接受「GitHub 可能仍可按旧 SHA 访问」的残余风险 | 待裁决（需授权确认） | 重写效果的根本成色 | 建议默认：接受并记录；如需彻底清除另起 GitHub Support 事务 | `src: privacy-history-rewrite-plan-zh.md §7 L100、§6 L87` |
| OQ-8 | **Q5** 跨仓通知：US 侧 `b19d68a` 锚点由谁通知、何时 | 待裁决 / 依赖外部 | 跨仓引用完整性 | 建议默认：重写完成后立即，写入 US 仓的 TODO（**不改对方文件**） | `src: privacy-history-rewrite-plan-zh.md §7 L101、§3 L40、§5 L80` |
| OQ-9 | **P0 前置阻塞**：SR 无 mirror 备份（工作区只有其他仓的） | 需授权 + 未验证 | 方案 B 的回滚可行性 | 未建立（U-4）；「动手前必须先建」 | `src: privacy-history-rewrite-plan-zh.md §3 L42、§5 L56` |
| OQ-10 | 受影响 tag 计数（10 vs 9 + `v0.2.0` 干净）口径未定 | 待回查（依赖 CNF-4） | 重写代价报价、P4 验收 | 未决（U-1） | `src: privacy-history-rewrite-plan-zh.md §1 L10、§3 L33–L38` |
| OQ-11 | 54 处仓内 hash 引用无逐条清单 | 缺失证据（依赖 U-3） | P6 第 12 步 remap 的可验收性 | 未决 | `src: privacy-history-rewrite-plan-zh.md §3 L39` |
| OQ-12 | force-push 顺序与节奏（`--force-with-lease --all` 然后 `--force --tags`）需维护者确认 | 需授权 | 推送窗口风险 | 计划就绪、未授权 | `src: privacy-history-rewrite-plan-zh.md §5 L75` |
| OQ-13 | 重写窗口内「禁止任何人推送」的协调（本仓单人） | 依赖外部 | 回滚窗口有效性（窗口 = 他人 fetch 之前） | 风险自评可控（ASM-3），未演练 | `src: privacy-history-rewrite-plan-zh.md §6 L91、§5 L83` |
| OQ-14 | 三命令门的**实现细节**（11 项内容、隐私门扫描模式、`$knownHistoryDebt` 条目）不在 `docs/**` 语料内 | 依赖外部 / 缺失证据 | 任何"覆盖未减"的判断只能依赖文档声称 | 未决（GAP-1） | `src: process-redundancy-review-zh.md §4 L62–L63` |
| OQ-15 | 发布推进的交接序列：`check-pack-readiness -RequireReleaseMetadata` + `privacy-audit -FullHistory -PrePush` → 本地提交 → **停在远端推送前**（推送需授权）；推送会话才做 push dev → PR → merge → tag → Release，Claim Pack 用 `[claim]` 快照 + CI 输出填写 | 交接风险 / 需授权 | 下一会话的动作边界 | 现行待办 | `src: process-redundancy-review-zh.md §9 L105–L106` |
| OQ-16 | 回写债务：§9 第 3 条要求把「待裁决」改「已定案」并回写 `MEMORY.md`/`TODO.md`，但 §1 实际未回写（CNF-1），记忆面不在语料内无法核对 | 交接风险 | 文档间一致性 | 部分未完成（U-9） | `src: process-redundancy-review-zh.md §9 L107、§1 L21` |

---

## 9. 锚点（ANCH-<n>：必须逐字保留）

**ANCH-1 标识符 / 命令 / 开关 / 字段名**（提炼阶段做逐字校验）
- 脚本：`scripts/privacy-audit.ps1`、`scripts/check-pack-readiness.ps1`、`scripts/verify-local.ps1`、`stage-package.ps1`、`pack-dev`、`scripts/codemap.md` `[F]` `src: process-redundancy-review-zh.md §4 L62–L68`
- 开关：`-FullHistory`、`-PrePush`、`-SkipVerify`、`-RequireReleaseMetadata`、`-NoRestore`、`--no-restore`、`--source`、`-G`（`git log` 内容检索）、`--tree-filter`、`--mirror`、`--force-with-lease --all`、`--force --tags` `[F]` `src: process-redundancy-review-zh.md §2 L27–L28、§4 L62–L63、§5 L72–L75、§8 L101；privacy-history-rewrite-plan-zh.md §5 L56、L60、L75、§8 L107、L109`
- 门输出/台账键与标记：`[claim]`、`[claim] localHead`、`[known-debt]`、`$knownHistoryDebt`、`[note] stale artifact, do not upload it`、退出码 `0`/`1` `[F]` `src: process-redundancy-review-zh.md §4 L62–L63、§5 L75、§6 L83、§8 L99`
- 身份/版本字段：`About.xml <modVersion>`、csproj `<Version>`、`version.txt`、`SqueakyRatkin <版本>`、`build=<flavor>`、`commit=<sha>`、`-dirty`、`FileVersion`、`Informational`、`FileVersion=0.3.3.0`、`ProductVersion=0.3.3+<40 位 sha>`、`v<tag>+<sha12>`、OGG 镜像、`pdb` `[F]` `src: process-review-zh.md §本次复盘新增 L22；process-redundancy-review-zh.md §1 L14–L15、§3 R5 L43、§6 L84；privacy-history-rewrite-plan-zh.md §1 L13`
- 错误码 / 文件：`NU1301`、`MSB3644`、`obj/project.assets.json`、浮动版本 `1.6.*`、net472 引用程序集 `[F]` `src: process-redundancy-review-zh.md §8 L101`
- 重写工件：`replacements.txt`、`commit-map`、`<local-path>`、`<repo>-mirror-backup.git`、`git 2.47.1`、`git-filter-repo`、`git filter-branch`、`gh repo view`、`visibility=PUBLIC`、`isFork=false`、`git ls-remote`、`git rev-parse --all`、`git tag -l --format="%(refname:short) %(objectname)"` `[F]` `src: privacy-history-rewrite-plan-zh.md §1 L11、§3 L41、§5 L56–L67`
- 流程动词：`git merge -s ours`、cherry-pick、squash merge、`reset` + `gc --prune`、relay 驱动登录态浏览器、filedetails 页面 `[F]` `src: process-review-zh.md §已记录的三项 L12–L13、§本次复盘新增 L20；process-redundancy-review-zh.md §8 L99`
- 引用文件面：`.slim/codemap.json`、`AGENTS.md`、`CONTRIBUTING.md`、`MEMORY.md`、`TODO.md`、`OBLIVIONIS.md`、`docs/0.3x-release-gate-checklist-zh.md`、`docs/release_review/`、`docs/release-<version>-review-zh.md`、`steam-workshop-page-copy-draft.md`、`modding_documents/privacy-debt-vector-triage-zh.md` `[F]` `src: process-review-zh.md §本次复盘新增 L19；process-redundancy-review-zh.md §3 R9 L47、§6 L81、§2 L29；privacy-history-rewrite-plan-zh.md §3 L39、§5 L64`

**ANCH-2 数值 / 版本 / tag / hash / 计数** `[F]`（除标注外均可回指）
- 计数：8 条措施、R1–R12、N1–N3、V1–V3、Q1–Q5、采纳 5 / 待裁决 3 / 不做 3、同一不变量最多 **4 处**人工表述、3 处表述、清单 **3 份拷贝**、11 项、约 11 秒、227 revision、230 提交、≈229、≈201、≈200、5 文件 × 1 处、10 tag、54 处、1 处跨仓（`b19d68a` 两次）、3 个唯一身份、3 个陈旧暂存包、7–12 位短 sha、触及提交数 4/7/3/3/2、SDK `8.0.x` → `10.0.x`、heads 5 个。`src: process-review-zh.md §已记录的三项 L7–L13、§本次复盘新增 L15–L23；process-redundancy-review-zh.md §1 L12–L19、§3 L39–L50、§5 L72–L75、§6 L81–L84、§4 L66；privacy-history-rewrite-plan-zh.md §1 L9–L10、§3 L30–L43、§4 L50、§5 L79`
- 提交短 sha：`afa00cb3`（初始提交 / `CONTRIBUTING.md` 最早脏点）、`2936879a`（`.slim/codemap.json`）、`75ae4167`（`AGENTS.md`/`MEMORY.md`/`TODO.md`）、`b19d68a`（US 仓引用的跨仓锚点）。`src: privacy-history-rewrite-plan-zh.md §3 L32–L36、L40`
- tag 名：`v0.1.0`、`v0.1.0-rc1`、`v0.1.1`、`v0.2.0`（唯一内容干净）、`v0.2.1`、`v0.2.2`、`v0.2.3`、`v0.2.4`、`v0.3.0`、`v0.3.2-pre1`；本地未推送 ref：`archive/dev-pre-sanitize-0.2.0`。`src: privacy-history-rewrite-plan-zh.md §3 L32–L33、L38、L43`
- heads：`main`、`dev`、`0.3.x`、`kiiro-experiment`、`0.2.4-FINAL`。`src: privacy-history-rewrite-plan-zh.md §3 L43`
- 版本号 / 日期 / PR 号：0.2.1–0.2.4、0.2.5、0.3.x、`0.3.2-EXP`（dev 包）、`0.3.0`（steam 包）、`0.3.2-pre1`（github 包）、PR `#17`/`#18`、2026-09-06（US 裁决）、2026-09-13（本包三条主源的记述日期）。`src: process-review-zh.md §前言 L3、§本次复盘新增 L21–L22；process-redundancy-review-zh.md §前言 L3–L4、§6 L83；privacy-history-rewrite-plan-zh.md §前言 L3`
- 形态分类词（不得改写）：「用户目录形态」（暴露 Windows 用户目录名）与「工作区根形态」（**不含用户名**）。扫描模式在两文档中描述为「盘符 + 分隔符 + 两个目录名 token」，**token 字面量按隐私门不在本包复制**（该文档描述了盘符形态的扫描模式，只描述形态不复制原文）；该模式刻意覆盖 JSON 转义的双反斜杠写法。`src: privacy-history-rewrite-plan-zh.md §2 L19–L25、§8 L109、L115`
- 隐私门零命中清单（逐字）：工作树 0 命中、提交信息 0 命中、**凭据 0 命中**、**`PublishedFileId` 值 0 命中**。`src: privacy-history-rewrite-plan-zh.md §1 L9；process-redundancy-review-zh.md §6 L81`

**ANCH-3 授权边界与外部状态边界** `[F]`
- 本地 commit 允许 / 免逐次授权；远端 **push / PR / merge / tag / Release / Workshop** 逐次授权。`src: process-redundancy-review-zh.md §1 L18、§3 R8 L46、§7 V1 L90`
- 任何 **force-push / tag 重建**需**单独**授权；方案 B 逐条需授权。`src: privacy-history-rewrite-plan-zh.md §前言 L4、§5 L53`
- 第三方平台编辑 = 维护者人工；自动化只读公开视图（`read` filedetails，绝不进登录态/编辑界面）。`src: process-review-zh.md §本次复盘新增 L20、§通用原则提炼 L28；process-redundancy-review-zh.md §3 R11 L49`
- **禁止重新加回人工仪式**（最小仪式双向棘轮）。`src: process-redundancy-review-zh.md §前言 L4、§2 L30、§4 L65`
- 外部渠道状态一律不因本文件而视为已验证。`src: process-redundancy-review-zh.md §5 L77`
- 跨仓锚点 = 显式通知义务，**不改对方文件**；重写期间禁止任何人推送。`src: privacy-history-rewrite-plan-zh.md §3 L40、§6 L91`
- 本包自身写入面（调度方覆盖）：唯一允许写入 = `docs/consolidation/packages/PKG-5-process-privacy-debt.md`；`docs/` 下不创建其他文件；无 git 写操作；不新增检查门/脚本/CI。

---

## 10. 候选教训（LES-<n>）

| id | 教训 | 支撑证据 | 强度 | 适用边界 | 来源 |
|---|---|---|---|---|---|
| LES-1 | **教训进 runbook**：每次事故落一条可执行检查项，不留在 Claim Pack 里沉底 | 0.2.x 8 条措施中 7 条已实现且多数指向 runbook/脚本 | 强 | 发布流程类事故（非产品缺陷）；本包时期仍是"人读清单"形态 | `src: process-review-zh.md §通用原则提炼 L30` |
| LES-2 | **一次收尾，全量核对**：草稿/文档任何变更后跑一致性检查（版本、链接、字符数、清单），不拆到多个 PR | 措施 4（链接停留 0.2.3）、措施 6（#17/#18 两次 PR） | 中等 | 面向外部渠道的文案/元数据变更 | `src: process-review-zh.md §通用原则提炼 L27、§本次复盘新增 L19、L21` |
| LES-3 | **认证边界先声明**：第三方平台编辑 = 维护者人工；自动化只读公开视图 | INC-5（relay 误入编辑界面）；R11 后续追认为保留面 | 强 | 有登录态的外部平台 | `src: process-review-zh.md §通用原则提炼 L28、§本次复盘新增 L20；process-redundancy-review-zh.md §3 R11 L49` |
| LES-4 | **包可自证**：分发物必须带独立明文标识，核验不依赖 DLL 反读 | 措施 7 落地 + `[claim]` 快照沿用同一动机 | 中等 | 多渠道分发（三渠道） | `src: process-review-zh.md §通用原则提炼 L29、§本次复盘新增 L22` |
| LES-5 | **同一不变量的重复人工表述就是冗余源**：把机械可判定的不变量交给单一读时门，人工只留指针；删的只是重复表述，不是断言 | R1（3 处表述）、R2（3 份拷贝）、R3（4 处人工 + 0 脚本）、§6 陈旧包实例 | 强 | 仅适用于"机械可判定"的不变量；不适用于判断类动作（R12）与需要语义理解的动作（CNF-8） | `src: process-redundancy-review-zh.md §1 L12–L17、§3 L39–L41、§6 L83、§8 L98` |
| LES-6 | **门的自测必须在一次性仓库里做**（负向自测的失败路径会真实写仓库） | 单次事故 INC-9（已 `reset` + `gc --prune`，HEAD 未受影响） | 中等（单次事件，不升格为普适最佳实践） | 会执行 commit / gc 的门自测 | `src: process-redundancy-review-zh.md §8 L99、§5 L76` |
| LES-7 | **`known-debt` 台账须按「向量 + 精确文件路径」匹配**，否则掩盖新泄漏；新增一条要写进维护者裁决记录 | §8 第 2 条（设计约束）+ EV-4 的 5 条 `[known-debt]` | 中等（设计意图，未见对抗性验证） | 任何"已知债务白名单"型机制 | `src: process-redundancy-review-zh.md §8 L97、§5 L75` |
| LES-8 | **迁移外部基准时迁移"门禁归属"而非"清单内容"**：产品面差异会否掉部分断言（US 身份唯一断言不适用 SR，改断"全部 noreply"） | §2 L33 + §6 L82 | 中等 | 同族仓库/同工具链之间借用流程 | `src: process-redundancy-review-zh.md §2 L33、§6 L82` |
| LES-9 | **浮动依赖版本 + 无网络 + 隐式 restore 会污染构建态**（`NU1301` 覆写 assets 文件，后续 `--no-restore` 全报 `MSB3644`）；门的缓解 = 透传 `-NoRestore` | INC-10 实测链条 | 强（实测） | 离线/受限网络构建环境 + 浮动版本 | `src: process-redundancy-review-zh.md §8 L101` |
| LES-10 | **文档自身不得复制债务串**：只给形态分类与计数，正则/路径以拼接构造，避免产物自身触发门禁 | 两份文档的无隐私写法自述 + §8 说明 | 强（同一策略在两文档独立采用） | 任何会被隐私门扫描的产物面（含本包） | `src: privacy-history-rewrite-plan-zh.md §前言 L5、§8 L115；process-redundancy-review-zh.md §前言 L6` |
| LES-11 | **历史台账与现行入口分工**：旧台账冻结、新裁决只进现行文档，避免同构双写（前提：先例、备份、回滚基线等"耐久事实"要在冻结前被写下） | R6 + 台账截止声明；P0 备份"先例"来自其他仓 | 中等 | 多入口文档治理 | `src: process-review-zh.md §前言 L5；process-redundancy-review-zh.md §3 R6 L44；privacy-history-rewrite-plan-zh.md §5 L56` |
| LES-12 | **落选方案的检验价值**：C（只改 tag）虽被否，其判据「tag 指向的提交内容仍含债务 ⇒ 伪安全感」是后续所有"轻量消毒"提议的过滤器 | §4 C 行 | tentative | 仅隐私/历史重写类提议 | `src: privacy-history-rewrite-plan-zh.md §4 L51` |

---

## 11. 证据缺口（GAP-<n>）

| id | 想回答的问题 | 为什么现有语料答不了 | 需要什么才能回答 | 来源 |
|---|---|---|---|---|
| GAP-1 | 三命令门的实际断言集合是否等于（或严于）被删的人工清单 | 脚本与 workflow 不在 `docs/**` 语料内；§8 只有声称 | 读 `scripts/privacy-audit.ps1`、`check-pack-readiness.ps1`、`.github/workflows/ci.yml`/`release.yml`（本包禁外部取证） | `src: process-redundancy-review-zh.md §4 L62–L63、§8 L98` |
| GAP-2 | 5 处债务的具体形态与所在版本 | 隐私硬门：不得复制；文档只给形态分类 + 处数 | 由维护者本地跑 `-FullHistory` 并看 `[known-debt]` 明细（不进产物） | `src: privacy-history-rewrite-plan-zh.md §2 L17–L23；process-redundancy-review-zh.md §5 L75` |
| GAP-3 | 受影响 tag 到底是 10 还是 9（CNF-4/U-1） | 同一文档两处口径矛盾；枚举需 git 取证 | 在冻结 revision 上 `git tag -l` + 逐 tag `git grep -l -I -E` | `src: privacy-history-rewrite-plan-zh.md §1 L10、§3 L33–L38、§5 L72` |
| GAP-4 | 227 revision 与 230 提交的差额归属（CNF-3/U-2） | 两文档计数口径未注记 | `git rev-list --count` 与 `--all` 口径对照 + 门的枚举实现 | `src: process-redundancy-review-zh.md §5 L75、§6 L81；privacy-history-rewrite-plan-zh.md §3 L38` |
| GAP-5 | 54 处仓内 hash 引用的完整位置 | 文档只举 4 个文件名 + 「等」 | 对全仓跑短 sha 引用扫描（7–12 位）并与真实提交取交集 | `src: privacy-history-rewrite-plan-zh.md §3 L39` |
| GAP-6 | V3 的 CI 首跑是否通过、SDK 10.0.x 是否改变构建结果 | 明写「未验证」「推送后才实际运行」 | 推送授权后的 CI run 结论 | `src: process-redundancy-review-zh.md §5 L77、§7 V3 L92、§4 L66–L67` |
| GAP-7 | 措施 8 是否已落地；runbook 现行第 15 条是否含 Comments 扫描 | 台账冻结；runbook 非本包主源 | 读 `docs/release-runbook-zh.md`（`[xref: PKG-4]`） | `src: process-review-zh.md §本次复盘新增 L23` |
| GAP-8 | P2 第 5 步「禁止改动」的完整规则集与其为何是行为的一部分 | 被引文件 `modding_documents/privacy-debt-vector-triage-zh.md` 不在语料（U-7） | 取得该文件 §3（并确认属哪一仓） | `src: privacy-history-rewrite-plan-zh.md §5 L64；process-redundancy-review-zh.md §2 L29` |
| GAP-9 | 0.2.1 教训 (d) 的原始记录与本次复发的差异 | 原文在 0.2.1 Claim Pack，非本包主源 | 读 `docs/release_review/release-0.2.1-review-zh.md`（`[xref: PKG-4]`） | `src: process-review-zh.md §已记录的三项 L13` |
| GAP-10 | SR mirror 备份是否已建立；`archive/dev-pre-sanitize-0.2.0` 的角色（是否须一并重写/保留） | 语料只记"没有 SR 的 mirror 备份"与该 ref 未推送 | 本地备份面盘点 + 该 ref 是否纳入 `--all` 集合 | `src: privacy-history-rewrite-plan-zh.md §3 L42–L43、§5 L56` |
| GAP-11 | `dist/` 3 个陈旧包是否应清理，以及清理是否与发布卫生冲突 | 只记录 `[note] stale artifact` 现象 | 维护者决定 + 现行 runbook 卫生条款（PKG-4 面） | `src: process-redundancy-review-zh.md §6 L83` |

---

