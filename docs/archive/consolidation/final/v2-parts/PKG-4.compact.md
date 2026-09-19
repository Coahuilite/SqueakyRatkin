# PKG-4 compact v2：发布流程与发布证据链
单源 = `docs/consolidation/packages/PKG-4-release-evidence.md`（v1，651 行 / 136,005 字节，repo_rev `4df95947…03ab3f`，frozen 2026-09-17）；**未读未用** `PKG-4-release-evidence-r2.md`。隐私硬门：`PublishedFileId.txt` 只记文件名/字段名、数值不落盘；Claim Pack 哈希/资产字节一行制 + 量级带，SHA-256 明细不复写，指针回指 v1 §7/§9 与各 Claim Pack。
## 0. 元数据与覆盖（压缩）
- 主源 11 份：`docs/release-runbook-zh.md`(101 行) + `release_review/release-0.2.0/0.2.1/0.2.2/0.2.3/0.2.4/0.3.0/0.3.2-pre1-review-zh.md`(7 份) + `CHANGELOG.md`(202)/`CHANGELOG.zh-CN.md`(201) + `steam-workshop-page-copy-draft.md`(201)。逐节覆盖表 = v1 §0，不复制。
- 未吸收大面：BBCode 正文体（L33–104/L111–181，产物非决策）、CHANGELOG 双语逐字全文（功能面按本文件 §3 DEC-22 逐版本重建回指行号）、0.2.x 流程台账与隐私债务清偿方案（PKG-5）、等价验收本体（PKG-2）——理由表 = v1 §12。
- xref 指针（只写指针）：PKG-1（srdiag/17 动作/VoicePack XML ABI 合同）、PKG-2（八面门槛、C1–C19 证据链本体、决策文档 §5）、PKG-3（内核拆分实体设计）、PKG-5（R1–R12/V1–V3、隐私债务 5 文件/10 tag/54 处引用）、PKG-6（0.3.3 交接、Eat 粒度规格）、AGENTS.md#External-state-boundaries（授权边界权威文本）。
- confidence 中-高；未拆包（v1 §0 slice_note）。最大不确定性 = Steam 外部人工态"观察时点 vs 冻结时点"漂移（CNF-4/5/7）+ 0.1.x/0.2.0 无完整 Claim Pack（U-4）。
## 1. 边界与一句话结论
- 负责：runbook 唯一入口契约（三命令/最小仪式/授权边界）、分支与发布路径、Claim Pack 模板与各版发布证据（tag/CI/资产/哈希/页面观察的指针与量级）、渠道矩阵 0.1.0→0.3.3、Steam 页面核验纪律、0.3.2 跳过与 Steam 阻断口径、发布卫生事故与措施、CHANGELOG 双语模板与功能面账本、Workshop 文案维护源与版本锚点。不负责：0.2.x 台账/隐私债务方案（PKG-5）、0.3.x 架构与等价（PKG-2）、US 拆分（PKG-3）、Eat 规格（PKG-6）、架构合同（PKG-1）。
- 一句话结论：发布体系已从 0.2.0 复盘自评"发布前执行 4/10"演化为「三命令 + 一版本一 Claim Pack + 渠道状态互相独立 + 外部人工状态只承认页面级观察」的机械化门禁链；0.2.1–0.3.2-pre1 六连发布证据齐全可核，开放面全部集中在 Steam 外部人工态（0.2.2/0.2.3 观察未回填、0.3.0 公告删除复核缺失、二进制下载级验证从未闭环）[F]。
- Top-3（提炼优先）：① DEC-15+CNF-6 版本序列现行口径（0.3.1/0.3.2 无正式版、0.3.2 仅 pre1、Steam 阻断、并入 0.3.3）；② DEC-02/03+DEC-11 三命令契约+最小仪式+隐私三向量独立扫描；③ DEC-08/09 渠道独立词表 + "页面级核验 ≠ 二进制已验证"解读钥匙。
## 2. 时间线（TL-1..18）
一行一事件；各条完整 `src:` 指针（CHANGELOG/对应 Claim Pack 行号）= v1 §2 同号行。

| id | 事件（当时→现行状态见备注） |
|---|---|
| TL-1 | 2026-07-04 18:35 UTC+8 — 0.1.0 首个公开发布（数据驱动啁啾全功能面，DEC-22） |
| TL-2 | 07-05 08:14 UTC+8 — 0.1.1 补丁 + `Initial Workshop Upload` 首次 Steam 上传（全语料唯一一次合法使用） |
| TL-3 | 07-30 00:32 UTC+8 — 0.2.0：15 动作 / VoicePack 体系 / 破坏性变更 |
| TL-4 | [U]（0.2.0 后）— 复盘：待推送历史曾含个人本地状态→dev 干净基点重建；沉淀 5 门禁 + 9 最小原则 + 自评 8/8/4；SSH 应急"不属常规流程" |
| TL-5 | 08-15 — 0.2.1 首发布作废：tag `57dfd1f`、zip 121 文件（codemap 泄漏 + LoadFolders 门控回归）→ 删 release/tag 重发 |
| TL-6 | 2026-08-15T17:54:24Z（UTC+8 08-16 01:54）— 0.2.1 重发（最终）：main `31c4e18`（PR #9 squash）、run `31899583405`、115 文件 |
| TL-7 | 08-16 — 0.2.1 Steam 页面级核验（Updated Aug 15 / 1.871 MB / Change Notes 3）；二进制下载级未做；渠道状态定格历史 |
| TL-8 | 2026-08-16T15:17:38Z — 0.2.2 GitHub（main `03ebb6b`/PR #10/run `31955162848`）；Steam 维护者**报告**完成，页面级"待补"至冻结未回填（OQ-1） |
| TL-9 | 08-17 22:12 UTC+8 — 0.2.3 GitHub（默认音源反转 DEC-13）；staging 已核验但上传未执行、Workshop=unverified；其后是否单独上传无证据（OQ-2） |
| TL-10 | 08-18 00:10 UTC+8 — 0.2.4 GitHub（`53686f9`/PR #15/run `32044477840`）；同日报告上传成功 + 页面核验完成（页面 Updated 17 Aug 09:22 = CNF-4 时区矛盾） |
| TL-11 | 08-18 — 0.2.4 自我批评三条流程教训 → 措施成文（LES-8/9） |
| TL-12 | 2026-08-21T01:35:38Z（UTC+8 09:35）— 0.3.0 GitHub（main squash `c06a90b`/PR #23/dev `a3e26e8`/run `32436779849`/116 文件）；页面观察（UTC+8 10:02）：0.3.0 已上页面、**旧置顶公告仍在线待删**（→CNF-5） |
| TL-13 | 2026-08-23 05:26:26Z（UTC+8 13:26）— `v0.3.2-pre1` GitHub prerelease（main squash `60f7d88`/PR #24/run `32620324890`/1,588,909 B/digest 一致）；Steam **阻断不执行**；被 0.3.3 口径取代（DEC-15/CNF-6） |
| TL-14 | [U]（≥0.3.0）— 文案维护源成文/更新：页面锚点=0.3.0；置顶公告"已于 0.3.0 如期删除"（与 TL-12"仍在线"时间层并存，CNF-5）；0.2.4 婴幼儿 fits 说明并入兼容性小节 |
| TL-15 | 2026-09-06 — 最小仪式对齐 UniversalSqueaker 维护者裁决（外部先例，文本不在语料，GAP-6）；现行（runbook 引用） |
| TL-16 | 2026-09-13 — runbook 精简：重复清单→单一读时门；隐私审查→单一脚本；入口契约与最小仪式成文；现行 |
| TL-17 | 冻结观测（09-17）— CHANGELOG 双语顶端 = `Unreleased — 0.3.3`；0.3.1/0.3.2 未以正式版发布（0.3.2 仅 pre1、工作随 0.3.3 发）；文案锚点仍 0.3.0；无 0.3.3 Claim Pack = 0.3.3 未发布 |
| TL-18 | 2026-09-17 — 语料冻结 repo_rev `4df95947…`；`git status --porcelain -- docs` 空（冻结动作，非源文档事实） |
## 3. 决策账本（DEC-01..22 / ALT-1..8）
各条完整 src 指针 = v1 §3 同号条目；[F]/[I]/[C]/[U] 分账继承 v1。

**DEC-01 唯一入口 + 版本事实归 Claim Pack**[现行规范]：runbook 只写流程与门禁；版本相关事实以当次 `docs/release_review/release-<version>-review-zh.md` 为准。立法背景 = 0.2.0 复盘"无可执行 Claim Pack"（4/10）；2026-09-13 精简修订；Claim Pack 写作在阶段 4 步骤 16，字段优先取自 `[claim]` 快照。
**DEC-02 门禁机械化 + 三命令契约**[现行，P0]：分工公理「能机械判定的都不写人工清单——写时断言 `stage-package.ps1`、读时断言 `check-pack-readiness.ps1`、隐私面 `privacy-audit.ps1`」「新检查一律先落脚本再写进本文；**不再新增人工清单**」。一个门只由一个命令负责：
1. 日常开发门（harness+双 flavor 构建）= `pwsh scripts/verify-local.ps1`（本机/CI）；
2. 发布面（版本轴/仓库红线/暂存包复核/DLL 身份/`[claim]` 快照）= `pwsh scripts/check-pack-readiness.ps1 -RequireReleaseMetadata`（默认含 1；发布会话/release.yml）；
3. 隐私面（三向量+身份+机械自检）= `pwsh scripts/privacy-audit.ps1 -FullHistory -PrePush`（push 前人工/CI 默认模式）。
网络假设（ASM-1/2）：离线/缓存环境一律 `-NoRestore`（csproj 浮动版本 `1.6.*` 隐式 restore 需网络；CI 有网、显式 restore 后跑 `-NoRestore`）。已机械化断言：排除项 `*.pdb`/`*.gitkeep`/`codemap.md`/`PublishedFileId.txt`、`version.txt` 三行、`About.xml == csproj`、Template↔内置 OGG 镜像——「**任何渠道都不再手工逐项核验**」。ALT-1 人工清单逐项核验（0.2.1 前做法，"已逐项核验"字样为化石；因 115→121 手工数错面等弱点被单一读时门取代，语义搬进断言）。
**DEC-03 最小发布仪式（push 前三件事）**[现行，P0]：① `check-pack-readiness -RequireReleaseMetadata` 全绿并打印 `[claim]` 快照，**Claim Pack 直接引用、不转抄**；② `privacy-audit -FullHistory -PrePush`（三向量+身份+工作树干净+待推送提交+tag 集合）；③ 发布裁决本身（版本号、渠道、是否发、Workshop 文案）= 人工不可替代步骤收口，无留痕格式 → OQ-11。对齐 UniversalSqueaker 2026-09-06 裁决（外部先例 C-7/GAP-6）。
**DEC-04 授权边界**[现行，逐字]：「本地 `commit` 不需要授权；**远端 push / PR / merge / tag / Release / Workshop 上架**都需要维护者明确授权」。落点：分支 push/tag 授权后执行；Steam 全人工=天然授权面；隐私历史/tag 重写"另行授权"（DEC-11）。实际行使 = 0.3.2-pre1 Steam 阻断令（DEC-15/EV 行）。权威文本 [xref: AGENTS.md#External-state-boundaries]。
**DEC-05 版本主源+文档同步+changelog 时间替换**[现行]：`Source/SqueakyRatkin/SqueakyRatkin.csproj` `<Version>` 唯一主源，`About/About.xml` `<modVersion>` 跟随，一致性与 DLL 身份由 check-pack 断言。文档与代码同批：CHANGELOG 双语 `Unreleased — X.Y.Z`（开发者解锁细节模糊化、不写 change note、中英同步）；README/MEMORY/根 codemap 锚点=单一维护源+其余指针；Workshop 文案中英对称、**英文以中文版为准**、一次变更一次核对。`Unreleased` → 最终发布时间（UTC+8；**tag 重发后必须再更新**，EV-15 验证）。
**DEC-06 分支与合并策略**[现行]：发布路径 = 大版本分支 → `main`（`0.3.x`→main；main 受保护则 PR squash、源分支仍是大版本分支）；`dev` 只集成不作发布源；merge 后 `git diff --stat` 0 行（tree 相等）+ 关键行为文件（`LoadFolders.xml`/csproj/`About.xml`）抽查；`archive/` 分支只在最终版本定稿时建；`merge -s ours` 只在真实分叉时用（CNF-11）；收尾 merge main 回 dev 对账。ALT-2 dev→main 直发 = 0.2.x 实际路径（PR #8/#9/#10/#13/#15），0.3.x（PR #23/#24）已迁移大版本分支制，属流程演化非冲突；ALT-3 squash vs merge 条件解（受保护则 PR squash，否则 ff/merge 均可；squash 代价 = 必然分叉 ASM-5）。
**DEC-07 GitHub 发布执行链**[现行]：main 树==dev 树核验 → 授权后 tag `vX.Y.Z`（严格 SemVer，基版本 = csproj `<Version>`）→ push → release CI（verify-local + check-pack `-RequireReleaseMetadata` → GitHub flavor 构建打包）→ 资产**以 CI 输出的文件数/哈希为准**。workflow 带 SemVer/项目版本/main 祖先门禁。prerelease = `v0.3.2-pre1`（严格 SemVer 2.0，基版本 0.3.2；stage-package 为此新增 prerelease label、About/csproj 对比基版本，INC-12）。hotfix = `vX.Y.Z-hotfixN`（0.3.0 决策，至今零实例，OQ-9）。ALT-4 资产本机核验 vs CI 为准 = 现行 CI 为准、`pack-github.ps1` 降为复现工具；ALT-5 正式 0.3.2 vs prerelease 通道 = 后者胜出后又被取代（DEC-15/CNF-6）。
**DEC-08 渠道状态词表 + 独立性原则 + 版本→渠道矩阵**[现行，P0]：词表 GitHub = **完整/待补**；Workshop = **unverified/页面级已核验/完整**。原则：「渠道状态彼此独立，不以一个渠道证明另一个渠道」「人工外部状态必须人工观察，未知即 unverified」（0.2.0 最小原则 #1/#6）；0.2.0 前身词表（GitHub `verified`/Steam `unverified/manual`）保留为历史层（CNF-2）。
**矩阵（截至冻结 repo_rev；每格 = 结论 + 证据指针）**：

| 版本 | GitHub | Workshop | 证据 |
|---|---|---|---|
| 0.1.0 | 已发布（仅 CHANGELOG 级；无 Claim Pack，模板未出生） | 无该版本单独证据 | CHANGELOG L141–168 |
| 0.1.1 | 已发布（同上） | `Initial Workshop Upload`（0.1.0 全功能+0.1.1 修复的 Steam flavor；与 0.1.1 同标题时间戳） | CHANGELOG L170–202（EN/ZH） |
| 0.2.0 | 完整（当时词：verified） | unverified/manual（不主张页面/二进制/文案已更新） | review-0.2.0 L15–30 |
| 0.2.1 | 完整（tag+CI+资产+DLL 身份/包内容逐项核验） | 页面级已核验；二进制下载级未做 | review-0.2.1 L5–64 |
| 0.2.2 | 完整 | 维护者报告已发布；页面级观察至冻结未回填（OQ-1/C-1） | review-0.2.2 L5–28 |
| 0.2.3 | 完整 | unverified（staging 已核验，上传未执行） | review-0.2.3 L5–34 |
| 0.2.4 | 完整 | 已上传+页面级核验完成（2026-08-18；EV 行） | review-0.2.4 L5–32 |
| 0.3.0 | 完整 | 页面级已核验；旧公告删除待复核→CNF-5 | review-0.3.0 L3–29 |
| 0.3.1 | **无正式版**（开发快照） | 无 | CHANGELOG L56 |
| 0.3.2 | **正式版未发布；仅 GitHub prerelease `v0.3.2-pre1`**（证据完整） | **阻断不执行**（未构建/上传、未触碰 item） | review-pre1 L5–26；CHANGELOG L56 |
| 0.3.3 | 冻结时未发布（顶端 `Unreleased — 0.3.3`；无 Claim Pack） | 冻结时未发布 | CHANGELOG 双语 L39–57 |

**DEC-09 Steam 发布与页面观察纪律**[现行]：构建 `build-steam.ps1`（Steam flavor + pack-steam 干净树硬门）→ check-pack 复核 `dist/steam/SqueakyRatkin`。上传全人工：复制 stage → 既有 item ID **只写入上传副本** `About/PublishedFileId.txt` → 同一作者 Update（「后续版本绝不用 `Initial Workshop Upload`」）→ 贴中英文案 → 编辑器+实际页面双预览。页面核对只读 filedetails 玩家视角（同 item/描述版本/Updated/visibility/预览/大小/change notes 数 + Comments 反馈扫描），**绝不尝试登录态或编辑界面**。边界条款「页面级核验 ≠ 玩家下载内容已验证」。上传人工**无 SteamCMD**（ALT-6，未采用原因语料无记录不代拟；约束后果 = Steam 侧证据只能页面级 I-8）。隐私硬门：`PublishedFileId.txt` 只存本地上传副本，**不进 Git/stage/发布包**（各 Claim Pack"0 PublishedFileId.txt"行的制度来源）。
**DEC-10 Claim Pack 固定模板与各版偏差**[现行模板；实例历史]：模板 12 行 = 版本/标签（严格 SemVer，基版本=csproj）/源码提交（main squash SHA）/发布时间（UTC 括 UTC+8）/flavor/DLL 身份（FileVersion + Informational `v<V>+<sha12>`）/CI run success/资产（zip 名+字节）/DLL SHA256/包内容（文件数、0 PDB、0 PublishedFileId.txt、0 codemap.md、关键文件核验、OGG 镜像、包内 version.txt）/隐私审计/配套 Steam staging+渠道状态两节。偏差谱（保留不归一）：0.2.0 分节式前身 + 一次性 "artifact manifest"（U-6）；0.2.1 资产无字节数（U-1）+ LoadFolders"已逐项核验"；0.2.2 隐私行"0 真实命中"；0.2.3/0.2.4 新增 `zip SHA256` 行；0.3.0 行名改"资产 SHA256"+version.txt 行+LoadFolders 行消失（CNF-3 [I]≈读时门接管）；pre1 新增"分支状态"行+"与 GitHub API digest 一致"；隐私行措辞三态（CNF-8）。实例不回改（LES-14）。
**DEC-11 隐私审查门禁**[现行硬门，P0]：三向量 = 工作树/提交信息/历史 blob **独立扫、结论不得互推**（「工作树干净不蕴含历史干净」：实测工作树 0 命中、历史 5 文件命中）。身份面：author/committer 全为 GitHub noreply，真实邮箱即失败。known-debt 台账 `$knownHistoryDebt` **当前 5 条**（`.slim/codemap.json` + 4 份 HEAD 已净化文档旧版本），按 `[known-debt]` 列出不判失败；「**新增一条 = 一次维护者裁决**」「历史/tag 重写另行授权」（清单本体在脚本 = U-10/PKG-5）。扫描模式 5 类（凭据/私钥/本地绝对路径/诊断日志摘录/`PublishedFileId.txt` 值——只写模式名，无任何值）。**无隐私写法**：写作时就不写入个人状态/展开路径/日志/凭据/ID 值，而非写后清理（"写后清理"被明确否定为默认路径）。secret 次序：真实命中先撤销/轮换再清理可达历史（LES-3）。**本文件隐私申报**：review-0.2.0 L53 / 0.2.1 L39 / 0.2.4 L32 / 0.3.0 L23 / copy-draft L5 五处含 Workshop item ID 数值与含该值 URL、GitHub 仓 URL 账号登录名——一律不复制；commit/run/版本/字节为非隐私标识按指针保留。
**DEC-12 [历史裁决] 0.2.1 作废与重发**：首发布（tag `57dfd1f`，PR #8 squash）zip 121 文件 vs 基线 115，两缺陷：① `stage-package.ps1` 递归复制带入 **6 个 `codemap.md`** → 排除列表增加（PR #9），现为写/读双断言；② `LoadFolders.xml` 门控回归——merge origin/main 时三方合并 **auto-merge 采纳 theirs（main 的门控版）**，`git checkout --ours` 对 auto-merged 文件无效（ASM-7）；全面 diff 定界仅此 1 文件，恢复无门控（`d73a0c3` 随 PR #9）。处置：删 release+tag → PR #9 → main `31c4e18` → tag `v0.2.1` → run `31899583405`；第二次 merge 用 `git merge -s ours`（main tree 与 dev `20d5c50` 全同）。**废止口径**（08-15/16 生效）：121 文件首发布不复存在，唯一有效 0.2.1 事实 = `31c4e18`。ALT-7 照常发 vs 删除重发：只记录选择，裁决过程 [U]；吸收硬细节 = 重发必须同步改 CHANGELOG 时间（EV-15 遵守证据）。main 独有提交 `1b1fe9e` 在 dev 已等价存在，无内容丢失。[I-10] 产品面"移除 IfModActive 修复"与发布面"恢复无门控"为同一文件两时间层，不互斥。
**DEC-13 [已实施/现行] 0.2.3 默认音源策略反转**：触发 = 默认落 Vanilla 回退层、婴幼儿鼠族听起来像豚鼠（豚鼠音效恰覆盖幼年可触发动作）。**废止旧口径**（0.2.0"内置 Example 不自动启用、默认 Off/Vanilla"；废止时点 2026-08-17）。新口径：新装与**从未显式调整过**的旧配置默认启用内置 Race Example（覆盖 15 动作）；旧配置迁移一次；显式模式与已选语音包**从不被覆盖**；可随时关。实机三态（fresh/显式 Off/已有选择经 GABP 桥）= C-4 会话内声称。对外联动 = 置顶公告上线（I-4 窗口 0.2.3–0.2.4）→ 0.3.0 起删除（DEC-18/CNF-5）；「再调默认策略须重新评估公告」现行前瞻条款；0.3.3 未再触碰默认策略。
**DEC-14 [历史裁决/对外口径现行] 0.3.0 发布决策**：维护性更新，玩家可见与 0.2.4 一致（八面门槛全绿+维护者实测+Player.log 核验 = C-6，本体 PKG-2）；双轨 = 本地 dev 包试用、观察期由实测替代（ALT-8 长观察期公测无其余记录）；热修命名 `vX.Y.Z-hotfixN`；措辞纪律 = 内核重构 + **拆分预告不提名**；打包新规：包内 `version.txt` 身份标签、115→116（+1 = version.txt，I-1）；Roadmap teaser（Notes）：内核未来拆分为独立前置 mod（名称待定）、**正式发布前另行公告**。后置链 C12/C13/C17/C18/C19 本包只登记发布面存在性（载体见 §9）。
**DEC-15 [已实施+已定案，P0] 0.3.2 处置（跳过口径）**：GitHub = 仅 `v0.3.2-pre1` prerelease（严格 SemVer 2.0 基版本 0.3.2；About `<modVersion>`=0.3.2 不带 label，系基版本对比设计非疏漏，I-11）；关键发布修复 = stage-package prerelease label + `.gitattributes` 只对冻结 corpus 强制 LF（CRLF 误报修复，INC-11）。Steam = **阻断不执行**（未构建/未上传/未触碰 Workshop item；维护者指示，DEC-04 行使）。现行口径（`Unreleased — 0.3.3` Notes）：**0.3.1 开发快照与 0.3.2 工作均未以正式版发布；0.3.2 仅作为 GitHub prerelease `v0.3.2-pre1` 发布；其种族路由、年龄/回退、彩蛋条目、婴幼儿发声、身份门控与 XML ABI 工作随 0.3.3 一同发布**。**废止口径单列**：pre1 待办"正式 0.3.2 发布时替换时间+执行 Steam 阶段 3"所预设的未来已被取代（废止时点 = Notes 写作日 [U]，U-8）→ 两时间层并存 CNF-6。改道决策过程不在本包主源（[I] 关联 0.3.3 就绪节奏，不作依据）。
**DEC-16 [现行] CHANGELOG 模板合同（9 规则）**：① 已发布旧前新后、未发布置顶（实践自我否定→CNF-1/I-9）；② 标题 = 本地发布时间 `[YYYY-MM-DD HH:MM UTC+8]`；③ `Initial Workshop Upload` 仅首次上传；④ 一条 bullet 一个改动、按需 Added/Changed/Fixed/Packaging(+Notes)；⑤ bugfix 简要；⑥ 仅双渠道同时受影响才同提；⑦ 不记录未接受计划（INC-2 教训）；⑧ 中英同步（EN L17↔ZH L17 互指）；⑨ Unreleased 阶段开发者解锁细节模糊化。
**DEC-17 [现行] Workshop 文案维护源**：定位 = 页面中英文描述的维护源、非发布记录；**页面是否已更新不能从仓库状态推断**。格式：BBCode 仅保守子集（`[h1][h2][h3][b][i][list][*][olist][url][hr][/hr][code]`）；中英两份独立完整不混排；尽量 ≤Steam 常见 **8000 字符**（近似口径无权威出处，U-9/GAP-8；现值中文 2101 / 英文 4741，代码块外维护、变更后重统计）。禁止项：packageId 展示、重复依赖栏、当 change note 用、音频统计数字、开发者排障、版本迁移说明、VoicePack 制作步骤（路由作者指南只留链接）。术语文风：正式英文 `Ratkin`；玩笑只用 `adorable little mousie`/`mousies`、**禁 `rat-rats`/`mousefolk`**；中文正式用"鼠族"、"鼠鼠/鼠辈"仅于名与玩笑；"3A" 固定 = AI 规划/AI 编程/AI 维护；英文标题逐字 `AI-Generated Work Disclosure`、**不得用 `Masterpiece`**、披露后直入正文；俏皮句不解释、每语言唯一。当前版本锚点 = **0.3.0**（文案内 `模组版本：0.3.0`/`Mod version: 0.3.0`；RimWorld 1.6；下载链接 `releases/tag/v0.3.0`；作者指南链接 `.github/skills/squeaky-voicepack-authoring/SKILL.md` 且中英两份标签均指中文版指南）。发布前核对清单 14 项（版本事实/15 动作/无统计/默认启用事实/No-DLC 基线/链接/packageId/双预览/字符数/权利表述/独立安装边界/术语/非殖民者通路/3A 标题与公告位置/单俏皮句）。
**DEC-18 [已实施；删除动作存时间层矛盾] 置顶公告生命周期**：「内置 Example 默认启用」公告上线窗口 [I] 0.2.3–0.2.4（I-4；0.2.4 页面核验记录描述含"默认开启 + 0.3.0 失效公告"）→ 0.3.0（维护性更新、策略不变）计划删除、事实改由正文小节承载 → 文案源声称"已于 0.3.0 **如期删除**"（写作日 [U]）vs 0.3.0 Claim Pack 当日观察"**仍在线**——待维护者编辑器删除后复核"——两口径见 CNF-5、复核无留痕见 OQ-3。前瞻条款：再调默认策略须重新评估公告；0.2.4 婴幼儿 fits 说明并入兼容性小节（现行中英双份）。
**DEC-19 [已实施] 0.2.4 三条流程纪律**：① 测试部署必须脚本产物且 flavor 对口：开发态 = Dev flavor + `pack-dev.ps1` 整包，发布态才 `pack-steam`，**禁止手搓**（INC-8）；② 开发与收尾提交一律 dev，main 只接受 PR squash merge（"分支保护强制执行"= 外部声称 C-10）（INC-9）；③ 发布收尾后 dev 尽快 `merge -s ours`/对账吸收 main 指针（INC-7→INC-10 复发才有条文，LES-9）。
**DEC-20 [部分升格] 0.2.0 复盘沉淀**：5 条下一版门禁要旨（Claim Pack 先收口再从 clean commit 构建；先 main squash 再 tag；推送前审计最终 tree+完整待推送 range；真实 secret 先轮换再清理；Workshop 更新确认同 item/实际二进制/可见性/预览/description）。9 条最小原则要旨（渠道独立；version/source/build/artifact/release 分别取证；公开产物 = exact clean release commit；每渠道 claim 建 Claim Pack；推送审计覆盖完整新可达 range；人工外部状态人工观察未知即 unverified；事故恢复非日常流程；文档事实机械防漂移；Memory 留耐久决策不留流水）。未固化裁决：事故恢复特殊操作不自动固化、只有单独审阅的通用门禁进规范（解释 INC-4 未收编）[C-3 自评 AGENTS 8/理念 8/发布前执行 4——一次性评价不得升格]。改进候选 3 条当时未修（pack 不自检 DLL flavor/pack-github 不校验 tag 基点/PR CI 不打包），前两项部分被机械化吸收、基点校验至今无记录（OQ-5/I-7）。changelog 时间精度政策未裁决（OQ-4）。
**DEC-21 [现行表述纪律] CI 非阻断警告**：Node 20 弃用 warning 为**非阻断警告，不应描述为已修复**（0.2.0）；0.2.1 复记 checkout/setup-dotnet/softprops 强制运行 Node 24；其后语料无记录（OQ-8）。系"如实记录边界"原则的 CI 面实例。
**DEC-22 CHANGELOG 功能面账本（0.1.0→0.3.3；版本序列要点，不复制双语全文；EN 行号主、ZH 镜像；内容层逐条等价，版式差异 CNF-9/CNF-1）**：
- **0.1.0**（07-04 18:35）：数据驱动 squeak 全功能面（9 触发集；动作+每 pawn 双冷却；心情驱动音高/音量；豚鼠兜底；自定义音频+纯自定义；距离衰减预设+自定义；倍速补偿；语言能力缩频；死亡反馈；心情调制工作台；DebugAction 悬浮字+摄像机高度；EN/zh-CN；Dev/GitHub/Steam 三 flavor+打包脚本）。Fixed 5 项（启动 patch/XML range/mote 红字/高倍速音量/发布包清理）。src: CHANGELOG L141–168。
- **0.1.1**（07-05 08:14）：移除 DebugAction 错误的 Dev-only 编译门（随 GitHub/Steam 发布；悬浮字仍需 Dev Mode）；mod 名称/描述本地化；README 3A 声明；自定义音频权利由提供者声明。Fixed 含**移除 release DLL 本机调试路径信息**（隐私面最早仓内记录）。L170–187。
- **Initial Workshop Upload**（同 0.1.1 时间戳）：首次 Steam 上传 = 0.1.0 全功能 + 0.1.1 修复；原版豚鼠音效仅按引用不重分发。L189–202。
- **0.2.0**（07-30 00:32，破坏性）：Added = 15 固定一次性动作（Draft/Undraft/有边界 Core 攻击/玩家下令 Equip/Mental Break）+ opt-in Race/Xenotype VoicePack + Off/Fallback/Remix + 精确 `defName` 目标 + 内置 Race Example + 可单启 Extras Template（Example 音频唯一维护源）+ 即时设置面（3 常规页+七击 Developer & Diagnostics 页+合并保存+关窗 flush）；Changed = No-DLC Core 基线端到端、心情调制入运行时 SoundInfo factor、第三方音频根 `<lowercase packageId>/<PackDef.defName>/<Action>/`；Fixed = 冷却消耗语义、触发路径限定、HAR-only 不混入候选+带确认 forget、悬浮字门控分离；**Breaking** = 删旧 `1.6/Sounds/Squeak/` 树与 Pure/custom-only 模型、第三方音频改独立 VoicePack、旧 audio-selection 值不自动迁移**且不声称其他设置保留**；Packaging = 41 条公共音频镜像（全文 L110–139）。
- **0.2.1**（08-16 01:54=重发时间）：Fixed = 移除 `LoadFolders.xml` `IfModActive="Solaris.RatkinRaceMod"` 门控（fork 静默失效）→ 内容始终加载 + `defName="Ratkin"` XPath 注入；关窗临时 UI 态重置。Changed = 诊断悬浮层重做（单字符描边 ● + 可拖动不暂停详情面板）。L99–108。
- **0.2.2**（08-16 23:17）：无玩家可见；日志门面拆分/相位去重/删无调用方辅助；`About.xml` 增 `<modVersion>`。L87–97。
- **0.2.3**（08-17 22:12）：默认音源策略变更（DEC-13）+ 婴幼儿豚鼠音色修复。L77–85。
- **0.2.4**（08-18 00:10）：婴幼儿 Giggling/Crying（Biotech BabyFits）不再误报崩溃语音；hook 收窄 `MentalBreakWorker.TryStart`；跨种族/全非崩溃态通用（根因 rimsage 属 PKG-2 面）。L72–75。
- **0.3.0**（08-21 09:35）：音源选择内部实现换为纯决策内核（玩家可见不变）+ 包内 version.txt + roadmap teaser。L59–70。
- **0.3.3（Unreleased — 冻结时）**：语音包精确种族只对该种族发声；年龄变体 `Baby/Toddler/Child/Adult` + 逐动作包回退；Biotech 婴幼儿 `Crying`/`Giggling` 条目（无音频时静默）；作者工具 `new-voicepack.ps1` + 自包含指南（可作 AI skill）；新设置「仅在真正进食时触发 Eat 叫声」**默认关** + 从属选项「使用成瘾品」**默认关**且仅上级开启时可用（零营养不计入；无法识别咀嚼阶段回落完整进食流程）；选中发声限定玩家可控/清醒/未倒地；VoicePack XML 公开稳定合同（字段只增不改、**17 动作键 append-only**、非法包 fail-closed、`IsEgg` 在合同内）；Notes = 0.3.1/0.3.2 口径（DEC-15）+ 诊断日志每动作窗口一行合并路由记录。L39–57（规格 PKG-6、ABI PKG-1 xref）。
- 双语一致性：内容层逐条等价；版式差异两处（CNF-9 ZH 缺空行；CNF-1 双语同病）。
## 4. 假设（ASM-1..12）
一行一Assumption；验证/失效点细节 = v1 §4 同号行。
| ASM-1 | CI 有网、显式 restore 后跑 `-NoRestore`；本机离线/缓存环境才默认加（现行合同） |
| ASM-2 | csproj 浮动版本 `1.6.*`，隐式 restore 需网络（现行） |
| ASM-3 | Steam file_size = 自有打包格式，不与本地 zip 字节直比（页面 1.87x MB 与 zip ~1.57 MB 级并存） |
| ASM-4 | 页面字段一致仅佐证"上线"；"玩家下载 DLL 即该版"需游戏内确认 → 固化边界条款，从未执行（OQ-6） |
| ASM-5 | PR squash 必然造成 main/dev 分叉，需 `merge -s ours` 对账（0.2.4 复发验证） |
| ASM-6 | "常见 8000 字符"是可靠约束 → 未验证（现值远低于线；U-9/GAP-8） |
| ASM-7 | git 三方合并 auto-merged 文件不受 `checkout --ours` 控制（INC-7 实测 → runbook L44 条款） |
| ASM-8 | 包基线 0.2.x=115 / 0.3.0+=116 文件（六实例一致） |
| ASM-9 | 隐私扫描会命中纪律文案自述；须区分真实/未接受/known-debt（EV-11/14 实测） |
| ASM-10 | Steam 一切编辑 = 维护者人工；agent 只读玩家视角（pre1 阻断即该假设行使） |
| ASM-11 | changelog 分钟 = 仓库口径 UTC+8，与真实渠道时间可能漂移（CNF-4 印证；政策未定 OQ-4） |
| ASM-12 | `[claim]` 快照可直接引用不转抄（现行；pre1 早于精简批注，字段来源 [U]） |
## 5. 认识论分账（C-1..11 / I-1..12 / U-1..10）
**C（参与者声明）**：C-1 0.2.2"已发布"维护者报告——无独立页面观察（OQ-1）；C-2 0.2.4 上传成功+同日页面核验**有**观察；C-3 0.2.0 自评 8/8/4 一次性不可复核；C-4 0.2.3 三态 GABP 运行时核验——会话内声称无复跑；C-5 0.2.4 实机三项（婴儿 fits：Pinenut 37212/Rainlin 37215 零崩溃音；儿童真崩溃：Wildtail 37127 仍响应）+rimsage 根因——会话内声称；C-6 0.3.0 玩家可见一致主张（八面门槛+实测+Player.log，本体 PKG-2）；C-7 runbook 引 UniversalSqueaker 09-06 裁决——文本语料外（GAP-6）；C-8 文案源"置顶公告已于 0.3.0 如期删除"——无观察日期，与 0.3.0 当日记录成 CNF-5 对；C-9 0.2.0 Steam stage 以 `e3ed623` dev head 构建验证（115 文件/41 镜像/0 命中）——≠发布 commit 构建、无外证；C-10"分支保护强制执行"——外部仓设置不可核；C-11 pre1 资产哈希与 GitHub API digest 一致——渠道内证，对 Steam 侧外证为无。
**I（本包推断）**：I-1 116=115+`version.txt`；I-2 17 动作键 = 15 + Crying + Giggling；I-3 页面 Updated ≈UTC-7 时钟（0.2.4 滞后发布 12 分钟自洽）；I-4 公告上线窗口 ∈[0.2.3 发布, 0.2.4 核验]；I-5"0.3.0 失效公告"最合理解释 = 预告置顶公告于 0.3.0 失效（异解→U-7）；I-6 0.2.0 时 Workshop 仍挂 0.1.1 时代包（Posted 5 Jul 不变）；I-7 改进候选中 flavor/身份面已吸收，"PR CI 不打包 / pack-github 基点校验"无吸收无废止；I-8 页面级核验 = agent 可触达证据上限（禁登录态+无 SteamCMD）；I-9 CHANGELOG 实序 = 顶端 Unreleased + 0.3.0→0.2.0 逆序 + 0.1.x 正序留底（prepend 惯例未回改→CNF-1）；I-10 DEC-12"main 的门控版"= main 残留旧 IfModActive 态，与 0.2.1 产品修复不互斥；I-11 pre1 `<modVersion>`=0.3.2 与 Informational `v0.3.2-pre1+…` 并存 = 基版本对比设计非疏漏；I-12 Notes 3→6 的 +3 与"0.2.3 上传未执行"矛盾：或曾上传或有未记录条目——本包不裁决（CNF-7/OQ-2）。
**U（未决/缺失）**：U-1 0.2.1 zip 字节数缺（模板遵守度）；U-2 0.2.2 页面快照缺（矩阵格/C-1）；U-3 0.2.3 是否曾上传（I-12/矩阵）；U-4 0.1.x 无任何 tag/CI/资产/哈希证据（矩阵前两行仅 CHANGELOG 级）；U-5 0.3.1 仅 Notes 一句单源；U-6"artifact manifest"无定义去向；U-7"0.3.0 失效公告"无正文快照（DEC-18/I-5）；U-8 0.3.3 条目与"不发正式 0.3.2"改道决策日期缺（DEC-15 时间层只能相对）；U-9 8000 字符无权威出处；U-10 台账 5 条清单本体在脚本、非本包主源（PKG-5）。
## 6. 矛盾与反转（CNF-1..11）
| CNF-1 | 规则「已发布旧前新后、未发布置顶」vs 正文实序（0.3.0→0.2.0 逆、0.1.x 正留底）——同文档规范与正文互相否定；I-9；终裁 = 维护者改规则或重排，本包不代选 |
| CNF-2 | 0.2.0 前身词表（GitHub `verified`/Steam `unverified/manual`）vs runbook 词表（完整/待补；unverified/页面级已核验/完整）——细化提取，历史层保留 |
| CNF-3 | 模板行集 vs 六实例偏差（0.2.1 缺字节数；0.2.3/4 多 zip SHA256；0.3.0 改"资产 SHA256"；pre1 多分支状态行；LoadFolders 行 0.2.4 起消失）——模板滞后实践；LoadFolders 消失 [I]≈读时门接管，回查 check-pack 断言清单 |
| CNF-4 | GitHub UTC+8 发布时间 vs 页面 Updated（`17 Aug 09:22`/`Aug 15`）对不齐——I-3 ≈UTC-7；对策 = 跨渠道时间账显式标时钟（LES-13/GAP-4） |
| CNF-5【必保留】 | 0.3.0 Claim Pack 当日观察：旧置顶公告「**仍在线**——待维护者在 Steam 编辑器删除后复核」（08-21）vs 文案维护源：「已于 0.3.0 **如期删除**」（写作日 [U]）——两时间层并存，后写口径不代表前段不存在、删除后复核无独立留痕；OQ-3 一次带日期复采可关闭 |
| CNF-6【必保留】 | pre1 待办预设「正式 0.3.2 会发生」（08-23）vs `Unreleased — 0.3.3` Notes「0.3.2 仅作 GitHub prerelease…随 0.3.3 发布」（写作 [U]/U-8）——0.3.2 命运"待正式 vs 永不正式"；后写 = 现行；改道动机不在本包主源；0.3.3 是否继承 pre1 证据 = GAP-7 |
| CNF-7 | Change Notes 计数 3（08-16）→6（08-18）vs 有记录上传仅 0.2.2/0.2.4 两次——+3 不匹配；I-12；GAP-2 复采闭合 |
| CNF-8 | 隐私行三称法（`0 命中`/`0 真实命中`/模板`0 未接受命中`）——[I] 同义 = known-debt 台账外真实命中为零；统一一次定义、不抹平实例原文 |
| CNF-9 | ZH 0.2.4/0.2.3 小节间缺空行 vs EN 有（双语同步规则旁）——纯版式漂移、不构成口径冲突 |
| CNF-10 | 0.2.4 时期页面已含「0.3.0 失效公告」（08-18）vs 0.3.0 Notes「正式发布前另行公告」（拆分预告）——I-5 所指不同（置顶失效预告 ≠ 拆分预告）不违规；依赖 U-7 |
| CNF-11 | runbook「`merge -s ours` 只在真实分叉时才用」vs 0.2.4「发布收尾后应尽快对账吸收 main」——[I] squash 收尾本身制造真实分叉，主动对账即分叉第一时刻；两处并存保留 |
## 7. 事故与发布证据（INC-1..13 / EV-1..17）
**INC（事故/遗留；一行一事件，处置细节 = v1 §7 同 id 行）**：INC-1 待推送历史曾含个人本地状态（用户名/主目录/绝对路径/订阅态/秘密形态）→ 原始历史仅本地未推送、dev 干净基点重建，立法三向量+完整可达范围审计；INC-2 changelog 曾把新行为误写入旧版本历史 → 规则 7；INC-3 README/作者指南与产品版本、Vanilla 音池事实漂移 → 单一维护源+指针+一次核对；INC-4 HTTPS 受阻一次性改用 SSH（origin 未改；"非常规流程"未固化，LES-4）；INC-5 main/dev 曾分叉（squash），merge-tree 无冲突正常合流；INC-6 0.2.1 首发布 121 文件 = 6 个 codemap.md 被递归复制（基线 115）→ stage 排除列表（PR #9）；INC-7 LoadFolders.xml auto-merge 污染（`checkout --ours` 无效）→ 全面 diff 定界仅 1 文件、`d73a0c3` 恢复无门控、"发布前逐项核验最终 tree 关键文件"入合同；INC-8 0.2.4 首轮测试仅换单 DLL+开发态误用 pack-steam → 规则①；INC-9 0.2.4 开发提交误落 main → cherry-pick 迁回 dev 并还原 main、规则②；INC-10 PR #15 changelog 双语三方冲突复发（0.2.1 同类教训在先）→ 规则③；INC-11 SettingsFixtureGenerator Windows CRLF 误报 → `.gitattributes` 仅冻结 corpus 强制 LF（C39/C40）；INC-12 stage-package 不支持 prerelease label → 增基版本对比支持；INC-13 0.3.0 旧置顶公告仍在线（外部人工编辑滞后）→ 记"待删除后复核"、文案源后称如期删除（CNF-5）。
**EV（发布证据一行制：版本 → 渠道 → 结论 → 证据指针；哈希/字节只重量级与缺项，SHA-256 不复写，正源 = 各 Claim Pack 与 v1 §7/§9）**：
- EV-1 0.1.x → 双渠道 → 发布完成、仅 CHANGELOG 级单源（无 Claim Pack）→ CHANGELOG L141–202。
- EV-2/3 0.2.0 → GitHub → 完整（main `5818ded…`、tag v0.2.0、run `30478707424`、115 文件、1,569,962 B、资产+DLL 哈希有录）→ review-0.2.0 L5–25；→ Steam → unverified/manual 一概不主张；staging 以 `e3ed623` dev head 实测（115 文件/41 音频镜像/0 隐私命中）= 边界条款成文起点 → L27–30、51–55（C-9）。
- EV-4/5/6 0.2.1 → GitHub → 完整（main `31c4e18…`、tag v0.2.1、run `31899583405`、2026-08-15T17:54:24Z、FileVersion 0.2.1.0、Informational `v0.2.1+31c4e18ad0d9`、115 文件/0 PDB/0 PublishedFileId.txt/0 codemap、LoadFolders 无 IfModActive 逐项核验、OGG 镜像通过、隐私全树 0 命中、dev↔main 0 行；zip 字节数缺 U-1）；staging 115 文件合格 → review-0.2.1 L5–58；→ Workshop → 页面级已核验（同 item（ID 值不复制）、`Mod version: 0.2.1`、Updated Aug 15、1.871 MB、Notes 3、评分 79/评论 37、公开、预览在）；**二进制下载级未验证（明示边界）** → L35–47。
- EV-7 0.2.2 → GitHub → 完整（main `03ebb6b…`、PR #10、run `31955162848`、1,573,976 B、115 文件、隐私 0 真实命中、dev↔main 0 行）→ review-0.2.2 L5–19；→ Workshop → 仅维护者报告（C-1/OQ-1）。
- EV-8 0.2.3 → GitHub → 完整（main `fca5fa6…`、PR #13 "enable built-in Race Example by default"、run `32038162813`、1,574,271 B、115/OGG41/modVersion 0.2.3、zip+DLL 双哈希有录、链 dev `32037789607`→PR `32037960290`→main `32038064450`）→ review-0.2.3 L5–20、38；→ Workshop → unverified：staging 115 文件已核验（staging DLL 哈希有录）但**上传未执行**；行为三态 = C-4。
- EV-9/10 0.2.4 → GitHub → 完整（main `53686f9…`、PR #15 "narrow mental-break hook"、run `32044477840`、1,574,386 B、115、链 `32044181594`→`32044344615`/`32044346462`→`32044414901`→`32044477840`）→ review-0.2.4 L5–19、48；→ Workshop → 已上传+页面核验完成（同 item、标题 Squeaky Ratkin、1.872 MB、Posted 5 Jul、Updated 17 Aug 09:22、Notes 6、public、标签 Mod/1.6、中英描述同步最新草稿含"默认开启+0.2.4 fits+0.3.0 失效公告"）；实机 = C-5；日志增量 = `srdiag v1` 28 事件中其余 27 输出不变（协议本体 PKG-1）。
- EV-11/12 0.3.0 → GitHub → 完整（main squash `c06a90b`、PR #23、dev `a3e26e8`、run `32436779849`、1,578,195 B、**116 文件**、version.txt=`SqueakyRatkin 0.3.0/build=github/commit=c06a90b`；v0.2.4..main 全树 0 真实命中 = 2 处文案自述误报）→ review-0.3.0 L3–18；→ Workshop → 页面+公开 API 只读核验（time_updated 08-21 10:02 UTC+8、file_size 1,879,864 B = Steam 自有格式、staging 116/PublishedFileId 0/version.txt build=steam、上传人工无 SteamCMD）；**旧置顶公告仍在线**（INC-13）→ L20–24。
- EV-13/14 0.3.2-pre1 → GitHub prerelease → 证据完整（main squash `60f7d88`、PR #24、run `32620324890`、2026-08-23 05:26:26Z、FileVersion 0.3.2.0、Informational `v0.3.2-pre1+60f7d8889326`、1,588,909 B、116、version.txt=`…0.3.2-pre1/build=github/commit=60f7d88`、资产哈希与 API digest 一致 C-11、About `<modVersion>`=0.3.2；分支状态：`0.3.x` 已推送（C34–C40）、dev 已 merge origin/main、tree==dev diff 0 行）→ review-pre1 L5–20；→ Workshop → 阻断不执行；→ 隐私 → 0.3.x 完整可达范围 **24 提交** 0 真实命中（PublishedFileId 命中均为文件名纪律文案）、0 个 dist/Assemblies/bin/obj 条目 → L19。
- EV-15/16/17 跨面 → EV-15 五点 CHANGELOG heading 时间 == Claim Pack 发布时间（0.2.1 = 重发时间，验证 DEC-05 重发条款）双源互证；EV-16 0.2.0 九原则中 4 条可直追现行合同落点（Claim Pack/完整 range 审计/exact clean commit/机械检查）；EV-17（DEC-02 引用号，v1 §7 无该行）= runbook 阶段 0–3 门禁全部改为命令引用。
## 8. 开放问题（OQ-1..13）
| OQ-1 | 0.2.2 Workshop 永久停在"维护者报告已发布"（页面历史不可回采则按 [U] 记账） |
| OQ-2 | 0.2.3 是否实际上传过（需外部回查；联动 CNF-7） |
| OQ-3 | 置顶公告删除无带日期复核（CNF-5 一次页面复采可关闭） |
| OQ-4 | changelog 时间精度政策（分钟/日期/cutoff）从未裁决 |
| OQ-5 | 0.2.0 三条脚本改进候选消化度：pack-github 基点校验无记录（I-7） |
| OQ-6 | Steam 二进制下载级验证从未执行 = 全部 Workshop 格上限（需授权+人工） |
| OQ-7 | 0.3.3 无发布面：无 Claim Pack、文案锚点仍 0.3.0、一次核对未做（交接 = PKG-6） |
| OQ-8 | CI Node20 警告是否消除无后续记录 |
| OQ-9 | `vX.Y.Z-hotfixN` 与 `archive/` 归档法零实例未经检验 |
| OQ-10 | known-debt 5 条清偿依赖授权（历史/tag 重写）；纪律依赖其不恶化（方案 = PKG-5） |
| OQ-11 | "发布裁决"无规定留痕格式（pre1 头注"按维护者指示阻断"为孤例）→ DEC-03/15 可审计性风险 |
| OQ-12 | `release-<version>-review-zh.md` 命名无 prerelease/hotfix 规范（pre1 文件名既成先例未收编） |
| OQ-13 | 「0.3.0 失效公告」短语无原文快照（U-7；影响 DEC-18 对外承诺链） |

注：v1 正文交叉引用有编号漂移（DEC-03 把留痕问题写作 OQ-13、DEC-07/14 把 hotfix 零实例写作 OQ-10）；本文件一律以 v1 §8 表号为准，漂移供编排方 coverage-check。
## 9. 锚点压缩（必保留面）
**必保留锚点（压缩；逐字全层 = v1 §9，本文件不复写 SHA-256）**：
- 三命令逐字：`pwsh scripts/verify-local.ps1` / `pwsh scripts/check-pack-readiness.ps1 -RequireReleaseMetadata` / `pwsh scripts/privacy-audit.ps1 -FullHistory -PrePush`；参数/字段：`-NoRestore`、`[claim]`、`$knownHistoryDebt`、`[known-debt]`、`<Version>`（csproj 主源）、`<modVersion>`（About.xml）、`version.txt` 三行、`SqueakyBuildFlavor`；其余脚本：`stage-package.ps1`、`pack-github/steam/dev.ps1`、`build-dev/steam.ps1`、`new-voicepack.ps1`；包排除项：`*.pdb`/`*.gitkeep`/`codemap.md`/`PublishedFileId.txt`（**只记文件名、值不落盘 = 隐私硬门**；该文件仅存本地上传副本）；staging = `dist/steam/SqueakyRatkin`；git 词：`merge -s ours`/`checkout --ours`/`diff --stat`/cherry-pick/merge-tree。
- 授权边界逐字：「本地 `commit` 不需要授权；**远端 push / PR / merge / tag / Release / Workshop 上架**都需要维护者明确授权」。纪律逐字：「以同一作者 Update（后续版本绝不用 `Initial Workshop Upload`）」「**绝不尝试登录态或编辑界面**」「页面级核验 ≠ 玩家下载内容已验证」「新增一条 = 一次维护者裁决」「历史/tag 重写另行授权」「工作树干净不蕴含历史干净」「任何渠道都不再手工逐项核验」「新检查一律先落脚本再写进本文；不再新增人工清单」。
- 版本链：0.1.0→0.1.1（+`Initial Workshop Upload`）→0.2.0→0.2.1→0.2.2→0.2.3→0.2.4→0.3.0→0.3.1（未发布快照）→`v0.3.2-pre1`（0.3.2 无正式版）→`Unreleased — 0.3.3`；命名 `vX.Y.Z`/`vX.Y.Z-hotfixN`/严格 SemVer 2.0。
- 标识符带（明细见 §7 EV 行与 v1 §9，不重复）：commits 短式 14（`5818ded…`…`60f7d88`，含作废态 `57dfd1f`、修复链 `2936879`/`20d5c50`/`d73a0c3`/`1b1fe9e`/staging `e3ed623`/dev `a3e26e8`）；PR #7/#8/#9/#10/#13/#15/#23/#24；CI runs 14（Release 主 run 7 个 + 0.2.3/0.2.4 流程链，均已在 §7 出现）；DLL identity 7（`v0.2.0+5818dedc3f22`…`v0.3.2-pre1+60f7d8889326`）。**SHA-256 共 11 支（资产 5/DLL 4/staging 2）一律不复写**，正源 = 各 Claim Pack。
- 资产字节一行制：GitHub zip = 1,569,962 / 缺（0.2.1，U-1）/ 1,573,976 / 1,574,271 / 1,574,386 / 1,578,195 / 1,588,909 B（0.2.0→0.3.0→pre1）；Steam 侧 = 1.871 MB（0.2.1 页）/ 1.872 MB（0.2.4 页）/ 1,879,864 B（0.3.0 API，自有格式）。
- 计数一行制：文件 115→116（0.3.0+）、121 = 泄漏态；codemap 泄漏 6；OGG 镜像 41；15 动作/17 动作键；srdiag v1 28 事件；推送扫描 24 提交；known-debt 5；扫描模式 5 类；设置 3 常规页+七击 dev 页；文案 2101（中）/4741（英）≤ 常见 8000；Notes 3→6；评分 79/评论 37；音频 22.05 kHz。
- 术语逐字：Ratkin；adorable little mousie/mousies；禁 rat-rats/mousefolk/Masterpiece；`AI-Generated Work Disclosure`；鼠族（正式）/鼠鼠、鼠辈（仅名与玩笑）；3A = AI 规划/AI 编程/AI 维护；`Hide The Book of Squeakudges on a high stool!`；页面标题 Squeaky Ratkin/鼠辈啁啾。
- **C 链登记（必保留）**：本包主源内可见号与载体 = C12/C13/C17/C18/C19（载体 `docs/release_review/release-0.3.0-review-zh.md` L35：C12 BuildFallback 兜底+`_Preview` 过滤+卫生、C13 Steam 打包纪律、C17 一键校验 verify-local.ps1、C18/C19 changelog 0.3.0 条目）与 C34–C40（载体 `release-0.3.2-pre1-review-zh.md` L20/L24：C34 身份门控→C35 彩蛋/身份日志→C36 作者指南/SKILL+脚手架+ABI 锁→C37 legacy 桥原型→C38 0.3.2 版本/changelog/docs→C39/C40 fixtures 行尾修复）；C38↔commit `7c3a956` = 调度方指针（未做仓外取证）。**C 链其余缺席段不在本包主源**；派发口径"C1–C19"与载体分布的偏差由编排方 coverage-check 消解（v1 §13-9）。"八面门槛/决策文档 §5" = PKG-2 锚名，本包仅登记发布面存在性。
## 10. 候选教训（LES-1..14）
| LES-1 | 渠道互独立；人工外部态必须人工观察，未知即 unverified（强） |
| LES-2 | 推送审计覆盖完整新可达 range+最终 tree；三向量结论不互推（强） |
| LES-3 | 真实 secret 先撤销/轮换再清理可达历史；清理 ≠ 轮换（中，预案） |
| LES-4 | 事故恢复特殊操作不自动固化为日常流程；单独审阅的门禁才进规范（强） |
| LES-5 | `checkout --ours` 管不住 auto-merged；发布前逐项核验最终 tree 关键行为文件（强） |
| LES-6 | 公开产物 = exact clean release commit；dirty 产物仅测试证据（强） |
| LES-7 | 能机械判定的一律从人工清单搬进脚本断言（强） |
| LES-8 | 测试部署 = 脚本产物+flavor 对口（开发态 pack-dev/发布态 pack-steam），禁止手搓（中，单事件） |
| LES-9 | squash 收尾后 dev 尽快 `merge -s ours` 吸收 main 指针，否则文档区冲突复发（中，两次代价） |
| LES-10 | Memory 留耐久决策，不留过程流水（中） |
| LES-11 | 对外文案集中维护+一次变更一次核对；页面态永不由仓库态推断（强） |
| LES-12 | 纪律文案自触发扫描命中；须区分真实命中/未接受命中/known-debt（tentative） |
| LES-13 | 跨渠道时间账显式标注各字段时钟（GitHub API UTC/仓库 UTC+8/页面未知时区）（tentative） |
| LES-14 | 模板 = 实例先行、事后收编为最小公共集；实例增量字段（zip SHA256/分支状态/digest）不回改（tentative） |
## 11. 证据缺口（GAP-1..10）
| GAP-1 | 0.1.x 的 tag/zip/哈希/首传包身份 → 需外部归档（早期 release 页/Workshop 历史快照） |
| GAP-2 | Change Notes 6 条实际内容（只计数未抄录）→ 一次带日期页面复采 |
| GAP-3 | 0.2.2/0.2.3 Workshop 真实状态（合 OQ-1/2）→ item 版本历史 |
| GAP-4 | Steam 页面展示时区定义 → GitHub 时间+页面 Updated+观察者时钟三对照 |
| GAP-5 | CHANGELOG 排序终裁（改规则还是重排正文）→ 维护者裁决一条（CNF-1） |
| GAP-6 | UniversalSqueaker 2026-09-06 裁决原文 → 语料外，需跨文档授权 |
| GAP-7 | 0.3.3 是否沿用 pre1 证据/待办 → 未来发布会话 + PKG-6 交接计划 |
| GAP-8 | 8000 字符上限权威出处 → Steam 官方文档/后台实测 |
| GAP-9 | 0.2.0 "artifact manifest" 所指与去向 → 当次发布会话原始记录 |
| GAP-10 | item ID 数值全文档分布 → 隐私规则有意不可答（已定位 5 处），仅审计用 |
## 12. 自检与计数
- **ID 计数（本文件全在）**：DEC 22 / ALT 8（嵌 DEC-02/06/07/09/12/14）/ TL 18 / ASM 12 / C 11 / I 12 / U 10 / CNF 11 / INC 13 / EV 16+1（EV-17 = DEC-02 引用号，v1 §7 无该行，已登记）/ OQ 13 / LES 14 / GAP 10。
- 必保留核对：三命令+最小仪式 = DEC-02/03/§9；授权边界 = DEC-04/§9（逐字）；渠道矩阵 0.1.0→0.3.3 = DEC-08 全表；0.3.1 无正式版、0.3.2 仅 `v0.3.2-pre1`、Steam 阻断 = DEC-15+矩阵行+CNF-6；公告"仍在线 vs 如期删除" = CNF-5/C-8/DEC-18/INC-13；C17–C19 与 C34–C40 可见+载体文件、缺席段已登记 = §9；`PublishedFileId.txt` 只写字段/文件名 = DEC-09/11+§9。
- 降维声明：SHA-256 全表零复制（一行制+指针）；资产字节/计数一行制（§9）；CHANGELOG 双语全文未复制（DEC-22 版本序列要点+行号指针）；BBCode 原文体未复制；完整 src 指针按 v1 同号行/条目回指。
- 隐私：无 item ID 数值、无账号登录名、无绝对路径/盘符/日志摘录/凭据。
- 自测：本文件字符数与 v1（84,941 字符）占比于交付回复中报告（目标 ≤21,235）。
