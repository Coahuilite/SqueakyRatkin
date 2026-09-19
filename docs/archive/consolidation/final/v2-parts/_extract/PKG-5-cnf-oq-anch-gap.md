# PKG-5 P0 slices

----- 6. 矛盾与反转（CNF-<n>） L352-366 -----
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


----- 8. 开放问题、阻塞与交接风险（OQ-<n>） L396-418 -----
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


----- 9. 锚点（ANCH-<n>：必须逐字保留） L419-450 -----
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


----- 11. 证据缺口（GAP-<n>） L470-487 -----
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

