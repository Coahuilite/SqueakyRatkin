# PKG-5 compact v2 — 流程教训与隐私历史债务

源包 `PKG-5-process-privacy-debt.md`（DEC 15 / ALT-A·B·C·B1·B2 / ASM 12 / CNF 8 / OQ 16 / GAP 11 / INC 10 / EV 12 / LES 12 / TL 14 / C 6·I 5·U 9 / ANCH 3 组）；主源 `docs/release_review/` 三份 252 行·24366 字节 = 台账 `process-review-zh.md`、评估 `process-redundancy-review-zh.md`、方案 `privacy-history-rewrite-plan-zh.md`；repo_rev 4df9594713adbbba91e0aea788a7c7cd3503ab3f、frozen_at 2026-09-17。xref：PKG-1 合同 / PKG-2 11 项行为面 / PKG-3 US 基准 / PKG-4 三命令与发布事实 / PKG-6 交接。

**一句话结论 `[F]`**：「发布流程是否冗余」已从"删检查"改写为"换执行者"——**门禁覆盖没有缺失，冗余在『谁来做』**（同一不变量最多出现 **4 处**人工表述），处置是收敛为单一读时门而非删断言；判定式 = **采纳 5 / 待裁决 3 / 明确不做 3**，随后 3 项以待裁决身份于 2026-09-13 由 V1–V3 定案。唯一未闭合大宗开放项 = **隐私历史债务**：暴露面是本机路径而非凭据、代价接近全历史重写 ⇒ 当前口径「**方案 A 维持 + 方案 B 备而不执行**」，任何 force-push / tag 重建需**单独授权**（授权缺口开放）。

## 1 0.2.x 事故台账 + 8 条措施（DEC-1/DEC-2、INC-1…INC-8）

台账汇总 0.2.1–0.2.4 各 Claim Pack 与发布实践暴露的流程不足，前言「每条含根因与**已落地**的优化措施。措施以『已实现 / 待实现』标注」；措施 1–3 来自 0.2.4 Claim Pack（列名即「优化措施（**已实现**）」，由表头担保），4–8 为 0.2.4 收尾新增。

| 措施 | 事故/缺陷 → 根因 | 处置与状态 |
|---|---|---|
| 1 INC-1 | 单 DLL 替换测试 + **误用 Steam flavor 打开发包** → 图快绕过门控 | 开发态测试 = Dev flavor + `pack-dev` 整包替换，写入 runbook 阶段 0（已实现） |
| 2 INC-2 | 开发提交误落 `main` → 分支纪律松弛 | 开发/收尾一律 `dev`，`main` 只收 PR squash merge，误落即 cherry-pick 回 `dev`（已实现） |
| 3 INC-3 | changelog 双语**连续两次**三方冲突（0.2.1 教训 (d) **复发**）→ `dev` 未及时吸收 `main` 指针 | 收尾立即 `merge origin/main` 对账；树相同时 `git merge -s ours`（已实现） |
| 4 INC-4 | 草稿版本号/下载链接停留 0.2.3 → 无「版本号 = 发布版本」检查 | 草稿规则 + 清单加「发布后立即 bump 版本号与链接」（已实现，`steam-workshop-page-copy-draft.md`） |
| 5 INC-5 | 自动化误进 Steam 编辑界面（relay 驱动登录态浏览器）→ 未明确认证边界 | runbook 阶段 3 第 15 条改**玩家视角只读**（`read` filedetails），绝不进登录态/编辑界面；编辑 = 维护者人工（已实现） |
| 6 INC-6 | 披露段变更与版本号更新**分两次 PR（#17/#18）** → 一次变更未跑完整一致性检查 | 任何草稿变更后统一核对版本号、下载链接、字符数、清单（已实现） |
| 7 INC-7 | 包内无版本标识（dev 有包外 label，GitHub/Steam 包无从确认新旧）→ 打包链只写 DLL 身份 | 包内 `version.txt` 三渠道统一（`SqueakyRatkin <版本>` / `build=<flavor>` / `commit=<sha>`，dev 带 `-dirty`）写入 `stage-package.ps1`；0.2.5/0.3.x 起生效；**已发布的 0.2.4 各渠道包不含**（是否重发 → OQ-1） |
| 8 INC-8 | 反馈闭环依赖人工（玩家「鼠蛋笑像杀猪」评论与修复关联晚）→ 无评论→TODO 通道 | 页面核验时顺带扫 Comments 区、疑点记入 TODO；**待实现**，仅入 runbook 第 15 条执行时的心智清单（→ OQ-2 / CNF-8） |

**DEC-2 台账冻结（TL-9）**：R6 指 8 条措施与 runbook 重叠、「历史台账与现行流程**同构**」→「保留为历史台账、**不再增长**；新裁决只进 runbook + 本文」；冻结的是**追加**不是效力（措施 1–7 的落地状态仍由台账担保）。DEC-1 机制 = 每条事故必配一条可执行措施（语料无备选记录）；DEC-3 认证边界（R11 追认为永久保留面）、DEC-4 `version.txt` 自证、DEC-5 措施 8 悬置。原则 **LES-1** 教训进 runbook / **LES-2** 一次收尾全量核对 / **LES-3** 认证边界先声明 / **LES-4** 包可自证。

## 2 R1–R12 / N1–N3 / V1–V3（DEC-6…DEC-12）

评估对象 = `docs/release-runbook-zh.md` + 台账 + 脚本 + CI；基准 = US「三命令 + 最小仪式」及其 2026-09-06 维护者裁决（含**禁止重新加回人工仪式**）。

| id | 观察 | 性质 | 裁决 |
|---|---|---|---|
| R1 | `<modVersion>` == csproj `<Version>`：runbook 0.1 + `stage-package` + `release.yml` = **3 处表述** | 复制型 | 降级为脚本，人工不增量，runbook 留「唯一主源」 |
| R2 | 包内容清单（排除项 / `version.txt` / DLL 身份 / OGG 镜像）在阶段 0.4 / 2.10 / 3.12 **出现 3 次** | 复制型 | 收敛为单一读时门 `check-pack-readiness.ps1`（写时断言 + 读时复核各一次），runbook 留指针 |
| R3 | 隐私审查在 0.3 / 1.7 / 2.9 + AGENTS = **4 处人工表述、无脚本、无固定模式、无法复现** | 复制型 | 新增 `privacy-audit.ps1`，runbook 收敛为 1 条命令 |
| R4 | 身份面从未单列检查 | **覆盖缺口（不是冗余）** | 纳入隐私门：author/committer 必须全为 noreply |
| R5 | Claim Pack 字段（`FileVersion`/`Informational`、包文件数、`version.txt`）手工转抄 | 转抄错误 | 门输出 `[claim]` 快照（只用仓内相对路径） |
| R6 | 台账 8 条措施与 runbook 同构 | 双写 | 保留为历史台账、不再增长 |
| R7 | 本地门只在人工会话跑，CI 不跑（`ci.yml` 仅构建 + dev 包） | 依赖人的记性 | CI 接线 → V3 |
| R8 | AGENTS 把本地 `commit` 列为需授权操作 | 人工口令仪式 | 追认免授权 → V1 |
| R9 | 模板 `docs/release-<version>-review-zh.md` vs 实际 `docs/release_review/` | 文档不一致 | **已修正**（统一到 `docs/release_review/`） |
| R10 | 双轨发布（dev 包试用 + GitHub + Steam 三段式） | 产品决策非冗余 | **保留**：Steam 是主渠道，三渠道共用同一读时门 |
| R11 | Steam 页面人工编辑/预览、Workshop 文案一致性 | 认证边界不可自动化 | **保留**（只读核验自动化；编辑仍是维护者人工） |
| R12 | 发布决策（是否发、版本号、渠道、热修方案） | 人的判断 | **保留**：「这就是最小仪式第 3 条」 |

**明确不做 N1–N3**：**N1** 不动 `verify-local` 的 **11 项**——它们锁行为（语料/协议/设置），**不是仪式**，删它们等于删覆盖；**N2** 不引入 US 的 carrier/前置依赖门（SR 无该依赖）；**N3** 不自动生成 CHANGELOG/Release notes 正文（生成式文案仍需人工，**只把字段自动化**）。

**V1–V3 定案（2026-09-13，TL-8）**：**V1 追认**本地 commit 免授权、远端 push/PR/tag/Release/Workshop **仍逐次授权**（落 `AGENTS.md`「External-state boundaries」；**废止**旧口径「本地 commit 属需授权操作」）。**V2 = DEC-13：先出专项方案，不动 git 历史**（当前维持方案 A = 不重写 + 纪律固化；方案 B 执行计划与不可逆点已备；落地物即第三主源）。**V3 接受** CI 跑 11 项门 + 隐私门、runner SDK 与本地证据基线同 major（`8.0.x` → `10.0.x`），「已写入 `ci.yml`/`release.yml`；**推送后首跑即真验**」。

**落地 7 项（DEC-7/8/9，agent 自称 `[C]`）**：`check-pack-readiness.ps1`（版本轴 / 仓库红线 / 暂存包复核 / DLL 身份）+ `privacy-audit.ps1`（工作树 / 提交信息 / 历史 blob **三向量独立扫、结论不得互推** + 身份面；`$knownHistoryDebt` 台账）+ runbook 重写为「三命令入口契约 + 最小发布仪式 + 阶段 0–4 去重复清单」+ AGENTS 增补 + `ci.yml`（`verify-local -NoRestore` → `privacy-audit`，SDK 对齐、concurrency/timeout）+ `release.yml`（`verify-local` → `check-pack-readiness -SkipVerify -RequireReleaseMetadata`）+ `scripts/codemap.md` 同步。**双向棘轮**：人工仪式既不得加回，也不得转成脚本吞掉人的判断；首要反例 = **清单搬进脚本后被悄悄弱化**（§8 六条反向自查：橡皮图章 / `known-debt` 掩盖新泄漏 / 断言被弱化 / 测试自造副作用 / CI 未实跑 / 离线 restore）。

## 3 两起自造事故（本会话证据基，不得升格为普适实践）

- **INC-9 负向自测写脏真实仓库 `[C/F 混合]`**：**负向自测的失败路径把探针提交写进了真实仓库**（自测未在一次性仓库里做）；已 `reset` + `gc --prune` 清除，**HEAD 未受影响**，固化为 **LES-6「门的自测必须在一次性仓库里做」**（单次事件不升格），`[claim] localHead` 能立刻发现异常。§5「**隔离的一次性仓库**，注入**合成**泄露串后自毁」与 §8 事故并存 = **CNF-6**；I-3 `[I]` 措辞疑为事故后补救。
- **INC-10 离线 restore 陷阱（实测）**：浮动版本 `1.6.*` + 无网络时 `dotnet build`（不带 `--no-restore`）以 `NU1301` 失败并**覆写 `obj/project.assets.json`**，此后所有 `--no-restore` 构建报 `MSB3644`（缺 net472 引用程序集），直到以本机 NuGet 全局包缓存目录为 `--source` 离线还原才恢复。定性为**环境事实，不是门缺陷** ⇒ ASM-9 + **LES-9**；缓解 = 新门透传 `-NoRestore` 并写入 runbook。

## 4 隐私历史债务（DEC-13/DEC-14/DEC-15）

**暴露面（不复制原文）**：债务 = **5 个文件 × 1 处**，全在历史 blob；工作树 0 命中、提交信息 0 命中、**凭据 0 命中**、**`PublishedFileId` 值 0 命中**；`gh repo view` 核实 `visibility=PUBLIC`、`isFork=false` ⇒ **真实外露，不是理论风险**。HEAD 处 5 文件已净化 ⇒ 债务只在旧版本、**普通新提交无法消除它**（ASM-8）；分发包 0 `codemap.md`、0 文档、0 `pdb`，发布面不受影响。形态词（不得改写）：「**用户目录形态**」（暴露 Windows 用户目录名）与「**工作区根形态**」（**不含用户名**）；扫描模式只述形态 =「盘符 + 分隔符 + 两个目录名 token」、刻意覆盖 JSON 转义双反斜杠，**token 字面量按隐私门不复制**。CNF-5 **反转**：旧记录只提 `.slim/codemap.json`，实测扩大到 5 个文件（该文件 + `AGENTS.md`/`CONTRIBUTING.md`/`MEMORY.md`/`TODO.md` 旧版本）；旧口径单独保留，它解释了 V2 为何必须重估。

**量化（数字逐字）**：全仓 **230 提交**（门扫描面 **227 revision**，差 3 无口径注记 = CNF-3/U-2/GAP-4）；逐文件触及提交数 **4/7/3/3/2**、最早脏点 `afa00cb3`（初始提交）/`2936879a`/`75ae4167`、漂移 **≈229 / ≈201 / ≈200 / ≈200 / ≈200**（越早越大）。受影响 tag 并集为 9，而「**10 个发布 tag 全部指向受影响提交**」与「`v0.2.0` 是**唯一内容干净**的发布 tag」并存 ⇒ **CNF-4/U-1/OQ-10：10 还是 9 未定，下游不得把 10 当受影响数**（名单见 ANCH-2）。仓内 hash 引用 **54 处**（7–12 位短 sha），重写后全部悬空、需 `commit-map` 机械替换，**无逐条清单**（U-3/GAP-5 → P6 第 12 步不可验收）；跨仓 **1 处** = US 仓 `MEMORY.md` 引用 SR 提交 `b19d68a`（**两次**，脏点后代）⇒ **显式通知义务、不改对方文件**。工具面 `git 2.47.1`、`git-filter-repo` **未安装**、`filter-branch` 可用（ASM-11）、`git ls-remote` 可达；remote 10 tag + 5 heads，本地多一个未推送 `archive/dev-pre-sanitize-0.2.0`（GAP-10）。备份面：**没有 SR 的 mirror 备份** ⇒ 动手前必须先建（**P0 前置阻塞**，OQ-9/U-4）。

**方案 A/B/C（DEC-14）**：**ALT-A 不重写（当前维持）** = 保留 `[known-debt]` 台账 + 无隐私写法纪律 + 每次 push 三向量门（`privacy-audit.ps1 -FullHistory -PrePush`）；成本 **0**；债务留在旧 blob 但新提交不再引入；风险「无（路径非凭据）」——这三件即便日后转 B 也须保留。**ALT-B 定向重写（备而不执行）** = 全历史文本替换（**只替换这 5 处路径文本**）+ tags 重建 + force-push；成本 = ≈229 提交 hash 漂移、10 tag 重建、54 处仓内引用 remap、1 处跨仓通知、所有 fork/缓存失效；效果 = 可达历史中不再有路径；仅在维护者判定"路径外露不可接受"（Q1）时启动。**ALT-C 只改 tag 不改历史（已否决）** = 成本中、效果**无效**（tag 指向的提交内容仍含债务）、风险**伪安全感**；其判据保留为 LES-12（后续一切"轻量消毒"提议的过滤器）。建议：先维持 A，判定路径外露不可接受才按 §5 执行 B（ASM-1/ASM-2/ASM-12）。

**方案 B 计划 P0–P6（14 步 + 回滚；逐条需授权，全部未执行 `[U]`）**：**P0** 1) `git clone --mirror` → `<repo>-mirror-backup.git`；2) `git rev-parse --all` + `git tag -l --format="%(refname:short) %(objectname)"` 为回滚基线。**P1** 3) 装 `git-filter-repo`（需网络；本机 NuGet 源曾 SSL 失败 `[C]`，先验 pip/pipx），否则 **ALT-B2** `git filter-branch --tree-filter` 行级替换 5 个文件（「慢但内置、无需网络」）→ Q2。**P2（只改呈现层）** 4) `replacements.txt` 把「用户目录形态」「工作区根形态」替换为中性占位（如 `<local-path>`）；5) **禁止改动** Scribe 字段名、负向断言的扫描模式、对外契约字符串、品牌名——「这些『看起来像泄漏』的字符串是**行为的一部分**」（triage §3 不在语料 → GAP-8/U-7）。**P3** 6) 重写 → `commit-map`；7) 清空 `$knownHistoryDebt` 后 `-FullHistory` 必须 **0 命中** + 三向量复扫 + 抽查 `git log --all`；8) `verify-local -NoRestore` 全绿。**P4** 9) 核对 10 个 tag 名不变、全指向新提交。**P5（需授权）** 10) `git push --force-with-lease --all` + `git push --force --tags`（顺序节奏需确认 → OQ-12）；11) CI 首跑 + 逐个 Release 页面核验。**P6** 12) `commit-map` 更新 54 处仓内 hash 引用；13) 通知 US 侧 `b19d68a` 新 hash；14) 清空台账、记录闭环。**回滚** = mirror 可完整恢复，**窗口 = 在他人 fetch/tag 重建之前**（ASM-3 未演练）。

**不可逆点与残余风险（授权前必须确认，5 条）**：1) **GitHub 不保证删除**——force-push 只让旧提交不可达，未 GC 对象可能仍可按旧 SHA 直接访问，彻底清除需 GitHub Support purge（额外流程与时间），「**不能承诺『重写即消失』**」；2) **全仓 hash 漂移**——已发布版本 Claim Pack 引用的 squash sha 与其他仓交叉引用悬空（本仓 54 处 + 跨仓 1 处已知）；3) **fork / 克隆 / CI 缓存 / 第三方归档无法回收**；4) **Release 与 tag 关联需逐个复核**，PR/issue 里的提交引用可能显示为 unknown；5) **重写期间禁止任何人推送**（需协调窗口；本仓单人维护 → 风险可控 `[U]`）。

**Q1–Q5（OQ-4…OQ-8，全未决 ⇒ 授权缺口开放）**：**Q1** 是否执行方案 B（路径外露 vs 不可逆成本）→ 建议默认先 A，B 仅在判定路径不可接受时执行；**Q2** `git-filter-repo`（需联网安装）还是内置 `filter-branch` → 先验证安装，否则 filter-branch；**Q3** 替换占位用什么（如 `<local-path>`）→ 中性占位，**不写任何盘符形态**；**Q4** 是否接受「GitHub 可能仍可按旧 SHA 访问」的残余风险 → 接受并记录；如需彻底清除另起 GitHub Support 事务；**Q5** 跨仓通知由谁、何时通知 US 侧 `b19d68a` → 重写完成后立即，写入 US 仓 TODO（**不改对方文件**）。

## 5 TL-1…TL-14

TL-1 0.2.1 期 changelog 双语冲突第一次（后判**复发**）；TL-2 0.2.1–0.2.4 滚动追加 → 现行**已冻结**；TL-3/TL-4 措施 1–3（0.2.4 Claim Pack）+ 4–8（收尾新增；4–7 已实现、8 待实现）；TL-5 0.2.5/0.3.x 起 `version.txt` 生效、0.2.4 重发待定；TL-6 2026-09-06 **US 最小发布仪式裁决**（人工只跑 `-FullHistory` + 机械自检，禁止重新加回人工仪式）；TL-7 2026-09-13（`dev`）评估会话产出 R1–R12/N1–N3 + 7 项落地；TL-8 同日 V1/V2/V3 定案；TL-9 台账冻结；TL-10 `-FullHistory` 使债务 1 → 5 文件反转；TL-11 产出 V2 专项方案，仍是**待授权**活跃方案；TL-12 一次性核实 PUBLIC/非 fork；TL-13 三份主源落库（`cc351d7 2026-09-13` / `92be4d6 2026-09-14`）；TL-14 2026-09-17 冻结语料并按 252 行主源重建。

## 6 ASM-1…ASM-12

ASM-1 债务不含凭据、形态仅为目录名 ⇒ 不重写风险可接受（Q1 变化即失效）。ASM-2 `v0.2.0` 是唯一内容干净的发布 tag（与「10 tag 全受影响」冲突，待回查）。ASM-3 单人维护 ⇒ 窗口可控（未验证）。ASM-4 本地门成本可忽略（**约 11 秒**）⇒ CI 加门可行（runner 未测）。ASM-5 SR 与 US 差异是产品面的、**不是门禁方式的**。ASM-6 US 2026-09-06 裁决可作 SR 基准。ASM-7「人工清单会漏、脚本不会漏」（EV-10 实例）。ASM-8 债务只在旧版本 ⇒ 必须历史重写。ASM-9 离线是常态 ⇒ 透传 `-NoRestore`。ASM-10 文档复制债务串会自触门禁 ⇒ 拼接构造。ASM-11 `filter-branch` 内置可用（未验证）。ASM-12 接受 GitHub 旧 SHA 残余风险（Q4 未决）。

## 7 CNF-1…CNF-8

**CNF-1** §1「待裁决 3 项」vs §7「定案」vs §9 要求回写：§1 未回写 ⇒ 以 §7 为最新口径但**不得删 §1 行**。**CNF-2** 前言「门禁覆盖没有缺失」vs R4「覆盖缺口（不是冗余）」。**CNF-3** 227 revision vs 230 提交（差 3 无注记；不自行调查）。**CNF-4** 「10 tag 全部指向受影响提交」vs「`v0.2.0` 唯一内容干净」⇒ 10/9 并列不择一（I-2 并集 9）。**CNF-5** 债务 1 → 5 文件反转（旧口径单独保留）。**CNF-6** §5「隔离的一次性仓库」vs §8「真实仓库被写脏」。**CNF-7** 前言「已落地」vs 表头担保前 3 项 + 措施 8「待实现」⇒ 以逐行标注为准。**CNF-8** 措施 8 属人工心智清单却未被 R1–R12 覆盖（需语义判断，非机械可判定）。

## 8 EV-1…EV-12

EV-1 `verify-local -NoRestore` **11/11 全绿**（EXIT 0，约 11 秒）；EV-2 `check-pack-readiness -RequireReleaseMetadata -NoRestore` **all checks passed**（含 `[claim]` 快照）；EV-3 `privacy-audit`（默认）CLEAN；EV-4 `-FullHistory` **227 revision 全扫** CLEAN、已知债务 **5 条** `[known-debt]`；EV-5 负向自测（隔离一次性仓库、**合成**串、自毁）三向量 + 身份面**全部触发、EXIT 1**「门不是橡皮图章」；EV-6 PUBLIC / 非 fork；EV-7 `git ls-remote` 可达、`git 2.47.1`、`git-filter-repo` 未安装、`filter-branch` 可用；EV-8 R9 已修正；EV-9 身份面 **3 个唯一身份**全部 GitHub noreply，US「身份唯一」断言**不适用 SR**；EV-10 `dist/` **3 个陈旧暂存包**（dev `0.3.2-EXP`、steam `0.3.0`、github `0.3.2-pre1`）被读时门标 `[note] stale artifact, do not upload it`；EV-11 `FileVersion=0.3.3.0`、`ProductVersion=0.3.3+<40 位 sha>` vs release `v<tag>+<sha12>` ⇒ 门只断 `FileVersion` **全等** + `ProductVersion` **含**产品版本；EV-12 诚实边界「**外部渠道状态一律不因本文件而视为已验证**；CI 改动要推送后才实际运行」。

## 9 OQ-1…16 / GAP-1…11 / C·I·U

OQ-1 0.2.4 是否重发；OQ-2 措施 8 是否落地；OQ-3 CI 加门 + SDK 10.0.x 首跑（"已落地"≠"已运行"）；OQ-4…OQ-8 = Q1…Q5；OQ-9 SR 无 mirror（P0 阻塞）；OQ-10 tag 口径；OQ-11 54 处无清单；OQ-12 force-push 顺序待确认；OQ-13 推送窗口协调；OQ-14 门实现细节不在 `docs/**`（GAP-1）；OQ-15 交接序列 = 两个门 → 本地提交 → **停在远端推送前**（推送需授权），推送会话才做 push dev → PR → merge → tag → Release；OQ-16 §1 回写未完成。GAP-1 断言集合是否严于人工清单；GAP-2 5 处债务形态（只由维护者本地看 `[known-debt]` 明细）；GAP-3 tag 枚举；GAP-4 227/230；GAP-5 54 处位置；GAP-6 CI 首跑；GAP-7 runbook 第 15 条现行文本；GAP-8 triage §3 规则集；GAP-9 0.2.1 教训 (d) 原件；GAP-10 mirror 与 `archive/dev-pre-sanitize-0.2.0`；GAP-11 陈旧包清理。认识论：C-1 US 裁决转述 / C-2 落地 7 项自称 / C-3「先出专项方案」转述 / C-4 环境声称 / C-5 台账截止未具名 / C-6「已实现」自述；I-1 待裁决 3 项 = V1–V3 / I-2 tag 并集 9 / I-3 隔离自测措辞系事故后补救 / I-4 收敛·覆盖新增·豁免三分 / I-5「加检查项」与「换执行者」是同一诊断的两个时点；U-1 tag 口径 / U-2 3 revision / U-3 54 处清单 / U-4 mirror / U-5 CI 真验 / U-6 措施 8 / U-7 triage 归属 / U-8 0.2.4 重发 / U-9 回写一致性。

## 10 LES-5…LES-12（LES-1…4 见 §1）

**LES-5** 重复人工表述就是冗余源：交给单一读时门、人工只留指针；**删的只是重复表述，不是断言**（`stage-package` 写时断言不动）；仅适用"机械可判定"不变量，不适用判断类（R12）与需语义理解（CNF-8）。**LES-6** 门的自测必须在一次性仓库里做。**LES-7** `known-debt` 台账须按「向量 + 精确文件路径」匹配，否则掩盖新泄漏；新增一条要写进维护者裁决记录。**LES-8** 迁移基准时迁"门禁归属"非"清单内容"。**LES-9** 浮动版本 + 无网络 + 隐式 restore 污染构建态，缓解 = 透传 `-NoRestore`。**LES-10** 文档自身不得复制债务串。**LES-11** 台账与现行入口分工以避免同构双写。**LES-12** 落选方案的检验价值（tentative）。

## 11 ANCH（逐字保留面）

**ANCH-1 标识符**：脚本名、开关（`-FullHistory`/`-PrePush`/`-SkipVerify`/`-RequireReleaseMetadata`/`-NoRestore`/`--no-restore`/`--source`/`--mirror`/`--tree-filter`/`--force-with-lease --all`/`--force --tags`）、标记（`[claim]`/`[known-debt]`/`$knownHistoryDebt`/退出码 `0`/`1`）、DLL 与 `version.txt` 字段、错误码链、重写工件、流程动词与文件面锚点（`AGENTS.md`/`CONTRIBUTING.md`/`MEMORY.md`/`TODO.md`/`.slim/codemap.json`/`docs/release_review/`/`steam-workshop-page-copy-draft.md` 等）均逐字见于 §1–§4、§8，不重复列举；仅此处出现：`-G`（`git log` 内容检索）、`OBLIVIONIS.md`、`docs/0.3x-release-gate-checklist-zh.md`（54 处引用点之一，`[xref: PKG-2]`）、`modding_documents/privacy-debt-vector-triage-zh.md`。

**ANCH-2 数值 / 版本 / tag / hash**：8 条措施、R1–R12、N1–N3、V1–V3、Q1–Q5、采纳 5 / 待裁决 3 / 不做 3、最多 **4 处**人工表述、**3 处表述**、清单 **3 份拷贝**、**11 项**、**约 11 秒**、**227 revision**、**230 提交**、**≈229 / ≈201 / ≈200**、**5 文件 × 1 处**、**10 tag**、**54 处**、**1 处跨仓**（`b19d68a` **两次**）、3 个唯一身份、3 个陈旧暂存包、7–12 位短 sha、触及提交数 4/7/3/3/2、SDK `8.0.x` → `10.0.x`、heads 5 个、remote 10 tag。短 sha `afa00cb3`（初始提交）、`2936879a`、`75ae4167`、`b19d68a`；tag `v0.1.0`、`v0.1.0-rc1`、`v0.1.1`、`v0.2.0`（唯一内容干净）、`v0.2.1`、`v0.2.2`、`v0.2.3`、`v0.2.4`、`v0.3.0`、`v0.3.2-pre1` + 本地未推送 `archive/dev-pre-sanitize-0.2.0`；heads `main`/`dev`/`0.3.x`/`kiiro-experiment`/`0.2.4-FINAL`；0.2.1–0.2.4、0.2.5、0.3.x、`0.3.2-EXP`、PR `#17`/`#18`、2026-09-06、2026-09-13。

**ANCH-3 授权与外部状态边界**：本地 commit 允许/免逐次授权；远端 **push / PR / merge / tag / Release / Workshop** 逐次授权；任何 **force-push / tag 重建需单独授权**、方案 B 逐条需授权（**当前未授权，缺口开放**）；第三方平台编辑 = 维护者人工、自动化只读公开视图（`read` filedetails，绝不进登录态/编辑界面）；**禁止重新加回人工仪式**；外部渠道状态一律不因本文件而视为已验证；跨仓锚点 = 显式通知义务、**不改对方文件**；重写期间禁止任何人推送。

## 12 隐私写法与未吸收项

本 compact 无本机绝对路径、无盘符形态字面量、无日志摘录、无凭据、无 `PublishedFileId` 值；含路径字面量的正则**只描述形态**、不复制债务串；路径均为仓库相对。未吸收（源包 §12）：债务路径字面量与含 token 的正则行（**隐私**）；US 侧一手细节（→ PKG-3/PKG-4）；§4 diff 级细节（→ GAP-1）；§1 与 §3 计数的重叠表述；相对链接语法；措施 1–3 的 Claim Pack 上下文（→ PKG-4）；表头分隔行与编号前缀（纯格式）。
