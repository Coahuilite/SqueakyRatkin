# coverage-check（阶段 A → 阶段 B 闸门）

- 编排方：Main（01 任务包调度者）｜ 度量与结构门核对时间：2026-09-17
- repo_rev：`4df9594713adbbba91e0aea788a7c7cd3503ab3f` ｜ OUT_DIR：`docs/consolidation`（维护者指定，产物属文档面）
- 判据来源：`prompts/docs-consolidation/README.md` §7（每文件须被某包声明"已吸收章节"或"未吸收 + 理由"）+ §5 派发纪律 / 拆包规则
- **度量口径（全流程统一）**：UTF-8 解码**字符数**（= PowerShell `(Get-Content -Raw).Length`；本次以 Node `readFileSync(...,'utf8').length` 实量）。字节数只作附列，**不参与压缩比**。

---

## 1. 冻结基线（复核 README §3）

- 语料文件数 = **26**（`docs/*.md` 16 + `docs/release_review/*.md` 10）⇒ 与 README §3 清单一致，**无新增文件**（无需开 PKG-7+）。
- 全语料字符数 **R = 194,352**；行数 `wc -l` = 2,885（按换行切分为 2,911，差异来自个别文件无尾换行）。
- `git status --porcelain -- docs` 为空 ⇒ 六包行号对该 revision 有效。
- 六包主源字符数之并集 = 26 文件、无缺漏无重叠（脚本核验：`PKG-1..PKG-6` 主源合计 194,352 = R）。

| 包 | 主源数 | 主源字符 | 占 R |
| --- | --- | --- | --- |
| PKG-1 | 3 | 32,970 | 17.0% |
| PKG-2 | 4 | 50,095 | 25.8% |
| PKG-3 | 3 | 26,906 | 13.8% |
| PKG-4 | 11 | 48,175 | 24.8% |
| PKG-5 | 3 | 14,211 | 7.3% |
| PKG-6 | 2 | 21,995 | 11.3% |

## 2. 交付物与结构门（机械核对，实测值）
| 包 | 文件（`docs/consolidation/packages/`） | 字符 | 字节 | 行 | §0–§13 | `{{…}}` 残留 | 盘符命中 | `PublishedFileId` 值 | 判定 |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| PKG-1 | `PKG-1-current-contracts.md` | 58,194 | 96,744 | 566 | 14/14 | 0 | 0 | 0 | **PASS** |
| PKG-2 | `PKG-2-0.3x-decision.md` | 60,861 | 105,388 | 455 | 14/14 | 0 | 0 | 0 | **PASS** |
| PKG-3 | `PKG-3-universalization-us-split.md` | 77,230 | 134,552 | 573 | 14/14 | 0 | 0 | 0 | **PASS** |
| PKG-4 | `PKG-4-release-evidence.md` | 84,941 | 136,005 | 651 | 14/14 | 0（收尾修复后复测） | 0 | 0 | **PASS** |
| PKG-5 | `PKG-5-process-privacy-debt.md` | 55,890 | 94,574 | 528 | 14/14 | 0 | 0 | 0 | **PASS** |
| PKG-6 | `PKG-6-handoff-open-rulings.md` | 46,275 | 77,140 | 410 | 14/14 | 0 | 0 | 0 | **PASS** |
| 合计 Σ输入 | — | **383,391** | — | — | — | 0 | 0 | 0 | — |
| 非交付 | `PKG-4-release-evidence-r2.md`（救援实例样张） | 30,723 | 50,438 | 367 | 14 + 1 处重复 stub 标题 | 0 | 0 | 0 | **排除** |

- 隐私门：全包对"盘符 + 分隔符"仅 1 处命中，经上下文核验为 false positive（`PKG-6` 内 `Toggle(enabled:/disabledReason:)` 的子串 `d:/`）；`PublishedFileId` 只以字段名/文件名形态出现，**值 0 命中**；无本机绝对路径、无日志摘录、无凭据。
- **`r2` 不得进入 `{{IN_PACKAGES}}`**：它只写完 §0–§5（DEC-1…19、ASM、C/I/U），§6–§13 为骨架，尾部残留一处中途追加产生的**重复 stub 标题**「## 4. 假设账本（ASM-<n>）（待补）」（故其 `## ` 计数为 16 而非 14），且末行是"（待补…）"非完整句 ⇒ 按 02 §2 输入完整性检查判"不可读"。它的 §0 覆盖表 11 行与 canonical 逐文件一致，因此**保留在磁盘**作为 §3 判定的独立复核件，并交出 11 条交叉核对项（见下一条）。
- **r2 交回的 11 条高价值条目 vs canonical PKG-4 抽查（编排方实测）**：**11/11 均已在 canonical 登记** ⇒ PKG-4 无漏收。落点为：置顶公告"仍在线 vs 已如期删除"矛盾 → `CNF-5` + `C-8` + `OQ-3` + `INC-13` + `DEC-18`；0.2.2 页面级观察未回填 → §7 未回填条目；页面标题 `Squeaky Ratkin` / `鼠辈啁啾` 分歧 → `EV-12` + §9 外部先例锚；CHANGELOG 双语逐条等价 + 版式不对称 → `DEC-22`「双语一致性结论」+ `CNF-9`（ZH 0.2.4/0.2.3 小节缺空行）+ `CNF-1`（置顶排序规则冲突）；Claim Pack 字段漂移（0.2.4 缺 DLL SHA256）→ `DEC-10` 偏差谱；0.2.3 staging 未上传 / Workshop 停在 0.2.2 两版 → `DEC-08` 渠道矩阵；0.2.0 九条最小原则 + 脏产物规则 → §0 覆盖表 L67–L77 与 §3/§10；授权边界（本地提交自由、push/PR/merge/tag/Release/Workshop 需授权、历史重写另算）→ §9 授权边界 + `DEC-02/03`；资产字节 / run id / SHA 矩阵与 41 clip 逐动作计数 → `EV-1…EV-16` + §9（一行制，细节留源文件）；`.gitattributes` LF-only 与 `stage-package` SemVer-prerelease 支持 → `DEC-15` / §7；C 编号可见范围 → §9 新增登记。
- 两处口径修正：① r2 把 C 链缺口表述为"gap C14–C33"**不精确**（C14 实际存在于 `docs/0.3x-release-gate-checklist-zh.md`）⇒ 阶段 B 一律采用本文件 §4 的编排方实测枚举；② 实例自报数字口径不一（PKG-4 曾把 84,941 字符报作"字节"并估高行数）⇒ **下游引用产物规模只引用本文件 §2 表**。
- 规模比（阶段 A 不压缩，>1× 属设计预期）：PKG-1 1.77× / PKG-2 1.21× / PKG-3 2.87× / PKG-4 1.76× / PKG-5 3.93× / PKG-6 2.10×；**Σ输入 = 383,391 字符 = 1.97·R**。
- 账本体量（编排方按"节内行首 id"机械计数；因 id 形态差异，ALT/C/I 类偏保守低估，另附实例自报值供对表）：TL 71 / DEC 131 / ALT 23（自报约 53）/ ASM 73 / CNF 51 / INC 40 / EV 63 / OQ 95 / LES 54 / GAP 51 / ANCH 30 个编号 + PKG-4、PKG-6 的无编号族组 / U 49。

## 3. 逐文件覆盖判定（README §7 判据）→ 全部满足

| # | 源文件 | 主包 | §0 覆盖声明核验 | 判定 |
| --- | --- | --- | --- | --- |
| 1 | `docs/project-architecture-contract.md` | PKG-1 | 7 节全进（DEC-1…22 + ANCH） | PASS |
| 2 | `docs/settings-ui-product-contract-zh.md` | PKG-1 | 4 节全进（DEC-23…30 + ANCH-9）；2 条外指链接声明未吸收并给理由 | PASS |
| 3 | `docs/logging-protocol.md` | PKG-1 | 6 节全进（DEC-31…36）；28 条 v1 人读句逐行抄录声明未吸收（重复/例行） | PASS |
| 4 | `docs/0.3x-refactor-architecture-decision-zh.md` | PKG-2 | 逐节映射；mermaid 图体 / C# 签名块 / §5 措辞草案原文登记未吸收 | PASS |
| 5 | `docs/0.3x-equivalence-review-zh.md` | PKG-2 | 14 行逐条入 DEC-7 | PASS |
| 6 | `docs/0.3x-release-gate-checklist-zh.md` | PKG-2 | A–H 门槛入 DEC-8 | PASS |
| 7 | `docs/handoff-0.3.0-zh.md` | PKG-2 | 覆盖表已声明；冷启动步骤=纯导航未吸收 | PASS |
| 8 | `docs/internal-universalization-design-note-zh.md` | PKG-3 | 15 个 `##` 标题逐节映射 DEC-1…19；工具实现细节归 PKG-1/2 未吸收 | PASS |
| 9 | `docs/us-sr-compatibility-check-zh.md` | PKG-3 | §0–§6 全进（E1–E12 / F1–F8 / 矩阵 / S1–S4·U1–U4 / Q1–Q4）；长引未吸收=禁大段复制 | PASS |
| 10 | `docs/us-sr-migration-plan-zh.md` | PKG-3 | §1–§8 全进（B1–B5 / P0–P4 / ALT-14 / R1–R6 / Q1–Q10）；§9 纯导航未吸收 | PASS |
| 11 | `docs/release-runbook-zh.md` | PKG-4 | 9 段全进（含 2026-09-13 精简记录、阶段 0–4、隐私门禁、Claim Pack 模板、渠道词表） | PASS |
| 12 | `docs/release_review/release-0.2.0-review-zh.md` | PKG-4 | 10 段全进 | PASS |
| 13 | `docs/release_review/release-0.2.1-review-zh.md` | PKG-4 | 6 段全进（含重发记录、页面核验表） | PASS |
| 14 | `docs/release_review/release-0.2.2-review-zh.md` | PKG-4 | 4 段全进 | PASS |
| 15 | `docs/release_review/release-0.2.3-review-zh.md` | PKG-4 | 5 段全进 | PASS |
| 16 | `docs/release_review/release-0.2.4-review-zh.md` | PKG-4 | 6 段全进（含自我批评三条） | PASS |
| 17 | `docs/release_review/release-0.3.0-review-zh.md` | PKG-4 | 4 段全进（含 C17–C19） | PASS |
| 18 | `docs/release_review/release-0.3.2-pre1-review-zh.md` | PKG-4 | 3 段全进（含 C34–C40 链） | PASS |
| 19 | `docs/CHANGELOG.md` | PKG-4 | 模板规则 + 11 个版本条目进账；bullet 全文声明未吸收（禁大段复制） | PASS |
| 20 | `docs/CHANGELOG.zh-CN.md` | PKG-4 | 镜像章节 + **双语差异**入 DEC/EV/CNF（差异未被当"重复"丢弃） | PASS |
| 21 | `docs/steam-workshop-page-copy-draft.md` | PKG-4 | 维护规则 / 术语 / 核对清单 / 版本锚点进账；两段 BBCode 文案体声明未吸收并给理由（在用资产须逐字开源文件） | PASS |
| 22 | `docs/release_review/process-review-zh.md` | PKG-5 | 全进；措施 1–3 的 Claim Pack 上下文归 PKG-4 | PASS |
| 23 | `docs/release_review/process-redundancy-review-zh.md` | PKG-5 | R1–R12 / N1–N3 / V1–V3 入 DEC-7…13；diff 级细节未吸收 → 已登记 GAP-1 | PASS |
| 24 | `docs/release_review/privacy-history-rewrite-plan-zh.md` | PKG-5 | 方案 A/B/C + P0–P6 + 不可逆点入 DEC-14/15；含路径字面量的正则行按隐私门只描述形态 | PASS |
| 25 | `docs/handoff-0.3.3-zh.md` | PKG-6 | 全进；§1–§9（2026-08-23）与 §10（2026-09-13）**两时间层单列** | PASS |
| 26 | `docs/handoff-eat-occurrence-granularity-zh.md` | PKG-6 | §5.1 / §7 / §7.1 / §8.8 等入 DEC-2…5、ALT-1/2、§9 锚点 | PASS |

**结论：26/26 满足判据 ⇒ 阶段 A 闸门 PASS。** 未出现"整体跳过"型缺口；所有未吸收项均逐条给理由（隐私 / 禁大段复制 / 纯导航 / 例行状态 / 跨包归属），符合 README §7"覆盖 ≠ 摘要"。

## 4. 派发口径偏差（编排方侧错误，不计 agent 缺失）

**核心项：证据链 C 编号。** 编排方实测冻结语料内真实出现的 C 标识为：

| 区段 | 载体文件 | 归属包 |
| --- | --- | --- |
| C1–C10、C12–C14 | `docs/0.3x-release-gate-checklist-zh.md`、`docs/handoff-0.3.0-zh.md` | PKG-2 |
| C17–C19 | `docs/release_review/release-0.3.0-review-zh.md` L35 | PKG-4 |
| C34–C40 | `docs/release_review/release-0.3.2-pre1-review-zh.md` L20、L24 | PKG-4 |
| **缺席** | — | C11、C15、C16、C20–C33 |

⇒ README §5 给 PKG-2 的「C1–C19」派发措辞**超出其主源可见面**（其主源止于 C14 且 C11 无号）。PKG-2 依 01 §4"要求项缺席"记 CNF-1/CNF-2/GAP-3/OQ-11 且**未代拟** ⇒ 处置正确，按派发纪律"表内数字与源文档不一致以源文档为准"。C17–C19 与 C34–C40 已在 PKG-4 主源内被逐字登记（两实例独立复核一致）。
⇒ **阶段 B 硬要求**：合并 C 链时按"三段可见 + 四段缺席（C11、C15、C16、C20–C33）"并列登记，禁止写成连续序列 C1–C40。

其余派发表偏差：`0.3x-refactor-architecture-decision-zh.md` 快照 319 行 → 实测 428 行 / 37,133 字符（README §3 已自注漂移）；`handoff-eat-occurrence-granularity-zh.md` 快照 209 行 → 实测 280 行 / 14,534 字符（已自注 ≥275）；规模列均只作调度参考，不影响判定。

## 5. 跨包接缝（阶段 B 的 P0 保留 / 合并指令）

1. **Eat 可观测性闭环**：PKG-1 U-7（合同内无 Eat 日志字段）↔ PKG-6 主源 `handoff-eat-occurrence-granularity-zh.md` §5.1 L206–L208 / §7.1 L262 / §8.8 L273（明确不新增日志事件，理由="日志协议是 characterization 冻结面"）+ OQ-22（Dev 诊断面板替代，未实施）。已回填至 `PKG-1-current-contracts.md` 末尾「附. 编排方补记」。⇒ 阶段 B 把它从"缺失"改记"已在 PKG-6 裁决"，但 OQ-22 仍为未决。
2. **C 链三段合并**（见 §4）：PKG-2#CNF-1/CNF-2 + PKG-4 的 C17–C19/C34–C40 登记 → 阶段 B 必须在 §6 保留"C11、C15、C16、C20–C33 在 docs 语料内无记载"这一事实本身。
3. **fallback 类名分歧**：合同逐字 `BuiltInFallbackCatalog`（`project-architecture-contract.md §6 L57`）↔ 规划示例名 `SqueakBuiltInFallbackCatalog`（`internal-universalization-design-note-zh.md L101`，以"如"引出）⇒ PKG-3#CNF-10 已登记，权威序建议以合同为准，冲突保留、主源记录不改。
4. **0.3.1 / 0.3.2 发行事实（PKG-1 的两条未决有了答案，但需保留矛盾）**：PKG-1#OQ-1/CNF-8（ABI 冻结起点"first released version that carries the 0.3.1 ABI"不可判定）↔ PKG-4 口径「0.3.1 无正式版；0.3.2 仅 `v0.3.2-pre1` GitHub prerelease，工作并入 0.3.3」。⇒ 阶段 B：把 PKG-1 的歧义改写为"由 PKG-4 证据闭合"，但**不得**删除 CNF-8（合同文本自身仍是描述性锚点）。
5. **tier 折叠**：PKG-1#CNF-2/OQ-3（`pack_fallback`→`race_pack`、`built_in_fallback`→`vanilla`）↔ PKG-3#DEC-6（内置 fallback 存储设计）↔ PKG-2 的路由公理/机械验证（含 fallback 层）。⇒ 阶段 B 保留为"归因损失是否已知取舍 = 未记载"，禁止写成"设计如此"。
6. **隐私面三源并列**：PKG-5（历史债务方案 + 不复制债务串的写法）、PKG-1#CNF-1（`pawn=<label>` vs 禁写标签通则）、PKG-4（`PublishedFileId.txt` 仅存于上传副本 / 账号登录名脱敏口径 / `privacy-audit.ps1` 门禁）。⇒ 三者在最终包 §6 必须并存，不得因 PKG-5 已出方案而判定问题闭环。
7. **ANCH id 形态归一**：PKG-1/2/3 用 `- **ANCH-n …**`；PKG-5 用 3 个组；**PKG-4 与 PKG-6 的 §9 只有族名、无 `ANCH-<n>` 字面编号**。⇒ 阶段 B 按 02 §6 规则 7 统一为组号，并在其 §0 给出「原 id / 原族名 → 新组号」映射表（否则逐字校验清单不可对账）。
8. **DEC id 编号风格**：PKG-4 用 `DEC-01…DEC-22`（零填充），其余用 `DEC-1…`。⇒ 阶段 B 引用一律写 `PKG-<n>#<ID>`，并给映射表。
9. **发布卫生 vs 流程台账的重叠面**：PKG-4（各 Claim Pack 内嵌一手事故）与 PKG-5（`process-review` 台账化后的 8 条措施）属同一事实的两个时间层 ⇒ 按"信息量最大一份 + 等价来源 `also:`"合并，**口径差异与追述差异不得折叠**。

## 6. 产物落点与套件处置（需维护者裁决；编排方未代改）

1. **`.gitignore` 正在吞掉全部任务包**：`git check-ignore -v` 确认 `.gitignore:23` 的 `packages/` 规则命中 `docs/consolidation/packages/` ⇒ `git status` 看不到这些文件，普通 `git add docs/` 也不会收入。维护者若要把任务包作为文档面入库，需三选一（编排方按 README §2 规则 1/3 不改仓库文件、不新增检查门）：
   - (a) `.gitignore` 追加否定规则（例：`!docs/consolidation/packages/`）；
   - (b) 一次性改目录名（例：`docs/consolidation/task-packages/`）以避开该规则；
   - (c) 维持忽略：把六包当作只服务阶段 B/C 的临时面，收敛提交只带走最终 handoff package（= README §2 的原始设计）。
   无论 (a)(b)：**提交前必须过 `scripts/privacy-audit.ps1`（默认模式）**（README §2 规则 7）。本目录已按"会被提交"的假设做隐私门，但脚本审计仍是唯一有效把关。
   实测佐证（`git status --porcelain --ignored=matching -- docs`）：`?? docs/consolidation/`（只有本文件可见）+ `!! docs/consolidation/packages/`（**六包 + r2 全部被忽略**）；`git diff --stat -- docs` 为空 ⇒ 六包过程未改动任何既有仓库文件（只读边界成立）。另 `prompts/` 本身也是**未跟踪**目录 ⇒ 套件从未入库，本文件与各包对 `README §4/§5` 的引用在 (a)/(b) 路径下会悬空，除非同时提交 `prompts/docs-consolidation/`。
2. **套件自身**：`prompts/**` 不在 `TASK-docs-consolidation-zh.md` §2 允许面内；README §10 尾注要求收敛提交前三选一（移出仓库 / 删除 / 明确追加允许面）。本管道产物已引用 README §4/§5 ⇒ 若选 (c) 之外的路径，需注意"产物引用了将被删除的套件"造成的回指断链。已登记为 PKG-1#OQ-11 / CNF-7。
3. **阶段 B 内部矛盾（套件自身，非 agent 问题）**：02 提示词 §1 的"有效目标 = min(Σ输入×35%, 原始语料×20%)"= **38,870**，与 02 §4 三线判据的严格档 `min(0.25·R, 0.60·Σ)` = **48,588**、README §8.7 严格档（同 02 §4）不一致。⇒ 编排方裁定：以 **02 §4 / README §8.7 的三线判据为准**（README §8.7 是验收条款，02 §1 是散文表述），但阶段 B 须在 §0 同时报两个数字与所选档位。

## 7. 阶段 B 参数（编排方本次下发值）

```text
{{IN_PACKAGES}} = docs/consolidation/packages/PKG-1-current-contracts.md
                  docs/consolidation/packages/PKG-2-0.3x-decision.md
                  docs/consolidation/packages/PKG-3-universalization-us-split.md
                  docs/consolidation/packages/PKG-4-release-evidence.md
                  docs/consolidation/packages/PKG-5-process-privacy-debt.md
                  docs/consolidation/packages/PKG-6-handoff-open-rulings.md
                  （不含 PKG-4-release-evidence-r2.md，理由见 §2）
{{OUT_FILE}}    = docs/consolidation/final/handoff-package-v2.md
{{RAW_STATS}}   = R = 194,352 字符 / 26 文件（字符数口径，见文首声明）
                  Σ输入 S = 383,391 字符（实测；S/R = 1.97；单包值见 §2 表）
                  严格档 T = min(0.25·R, 0.60·S) = 48,588 字符
                  密度举证档 T = min(0.30·R, 0.75·S) = 58,306 字符
{{REPO_REV}}    = 4df9594713adbbba91e0aea788a7c7cd3503ab3f
{{MODE}}        = two-pass（S 达全语料 1.97 倍，远超单轮舒适区；README §6 建议亦为默认 two-pass）
{{OUT_DIR}}     = docs/consolidation
```

## 8. 闸门结论与遗留风险

- **阶段 A：PASS**（26/26 覆盖、六包结构门全通过、隐私门零实质命中、无未声明缺项）。允许进入阶段 B。
- 执行过程事故（原样登记，不粉饰）：
  1. 原 PKG-4 实例自 01:43 起约 **80 分钟未落盘**，三次进度询问均未及时回复（实例确实在写作，非停摆）；编排方按单写者规则另起救援实例写 `-r2`（不同路径、互不覆盖）。原实例随后完成全量 651 行并自行修复 §13 的占位符残留。**净结论**：救援是冗余 token 开销、无损害；r2 的 §0 覆盖表 + 11 条交叉核对项构成一次独立的"PKG-4 是否漏收"复核（结果：11/11 已收，见 §2）。
  2. 教训（**tentative**，单次观察、不得当普适规则）：调度提示词应把"骨架先落盘、之后每节即时落盘"写成硬契约而非建议 —— 救援实例在收到该指令后约 3 分钟即落了 123 行完整骨架，而原实例在无此约束时长时间只在内存里组装。落盘节奏约束应写进 01 提示词 §4 工作流程，而非依赖编排方中途追加消息。
  3. 实例自报数字不可信：原实例曾报"83,545 字节 / 约 1160 行"，实测为 136,005 字节 / 651 行 / 84,941 字符（把字符数当字节数报、行数估高约 75%）。⇒ 编排方一律独立实测覆盖；已要求阶段 B 自行复测 `{{IN_PACKAGES}}` 规模（02 §0 亦如此规定）。
- 遗留给阶段 B/C 的最大风险（六包 §13 汇总，去重后 8 条主线）：
  1. C 链断裂区段（§4）；2. `pawn=<label>` 隐私冲突（PKG-1#CNF-1，建议升为维护者待办）；3. tier 折叠导致 fallback 不可归因（PKG-1#CNF-2）；4. 隐私历史债务的授权缺口与不可逆点（PKG-5）；5. US 是否服务 Ratkin 的 Q1 冲突（PKG-3#CNF-1，冲突一方在语料外）；6. Steam 外部人工态从未闭环（PKG-4：二进制下载级验证、公告删除复核）；7. 0.3.3 停在远端推送前的授权与提交面（PKG-6）；8. 产物落点 / 套件处置（本文件 §6）。
