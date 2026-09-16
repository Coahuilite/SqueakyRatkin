# 任务书：SR 退役期文档收敛 + 记忆压缩（外部 agent 执行）

> 执行者：外部模型 agent，单会话完成。本文件是唯一任务入口，执行前请完整读完。
> 任务性质：**纯文档/记忆收敛**。不改代码、不加检查门、不动远端。
> 起点判定：以执行时 `dev` 分支 tip 为准；开始前确认 `git status` 干净（若脏，先停下并报告，不要开始移动文件）。

## 0. 交付目标（DoD）

1. `docs/` 下只保留**收敛后仍具现行价值**的文档，互相一致、无过期信息、无历史叙述堆积；
2. 历史文档内容经研判后被吸收或明确废弃，`docs/archive/` 在任务结束时**不存在**；
3. `MEMORY.md` / `TODO.md` 压缩到只含耐久事实与开放动作；已完成/失效内容按格式进入 `OBLIVIONIS.md`；
4. 全程**不新增任何检查门**、不改代码、不推送远端。

## 1. 仓库现状（写作时快照，执行前请自行复核）

- **分支**：`dev`（本地领先 `origin/dev` 若干提交，**未推送**）；`main` = `02382d2`；`0.3.x` = `b19d68a`（树与 `main` 相同）；`0.2.4-FINAL`（远端归档分支）；`archive/dev-pre-sanitize-0.2.0`（仅本地）。
- **kiiro**：`kiiro-experiment` 本地与远端**已删除**（2026-09-13 之后）。docs 中残留的 Kiiro 叙述属于历史，按 §3 研判处理，不再代表活跃分支。
- **版本**：`Source/SqueakyRatkin/SqueakyRatkin.csproj` `<Version>` 与 `About/About.xml` `<modVersion>` 均为 `0.3.3`；双语 CHANGELOG 顶端为 `未发布 — 0.3.3`。
- **发布状态**：dev 包已产出（`dist/dev/SqueakyRatkin-dev-v0.3.3-<sha>.zip`），**等待维护者实机测试**；测试结果决定进入修复循环还是发布流程。远端未推送、无 tag、Steam 仍阻断。
- **门禁（不得新增）**：`scripts/verify-local.ps1`（11 项）、`scripts/check-pack-readiness.ps1`、`scripts/privacy-audit.ps1`；CI（`ci.yml`/`release.yml`）已接线。
- **记忆文件**：`MEMORY.md`（35 行，含 0.2.x–0.3.2 大段历史叙述）、`TODO.md`（95 行，含大量已完成 `[x]`）、`OBLIVIONIS.md`（70 行冷归档；现有格式：`## 日期：主题（归档）` + 要点 + `**状态**：…`）。
- **`AGENTS.md` 驱动条目（已恢复，2026-09-13）**：「Compacted memory lands in `OBLIVIONIS.md`: completed or no-longer-guiding entries from `MEMORY.md` / `TODO.md` move there as dated sections in its existing `date + reason + status` format; if such an entry becomes relevant again, re-summarize it into `MEMORY.md` with `source: OBLIVIONIS.md`.」本任务书的 §5 即该条目的落地。
- **docs 现存清单（行数，写作时）**：

| 文件 | 行数 | 初步归类 |
| --- | --- | --- |
| `project-architecture-contract.md` | 38 | 合同（保留，路径不可变） |
| `settings-ui-product-contract-zh.md` | 25 | 合同（保留，路径不可变） |
| `logging-protocol.md` | 89 | 合同（保留，路径不可变） |
| `release-runbook-zh.md` | 104 | 现行流程（保留；含 2026-09-13 的三命令契约 + 大版本分支→main 简化） |
| `steam-workshop-page-copy-draft.md` | 135 | 在用（Steam 解除阻断时使用） |
| `CHANGELOG.md` / `CHANGELOG.zh-CN.md` | 149 / 149 | 发布面（保留） |
| `us-sr-compatibility-check-zh.md` | 70 | 退役期活跃裁决（保留或与迁移计划合并） |
| `us-sr-migration-plan-zh.md` | 92 | 退役期活跃裁决（保留或与兼容性检查合并） |
| `release_review/release-*.md`（6 份） | 22–59 | **发布证据（Claim Pack），保留** |
| `release_review/privacy-history-rewrite-plan-zh.md` | 88 | 活跃裁决（V2 待授权，保留） |
| `release_review/process-review-zh.md` | 23 | 教训 → 合并 |
| `release_review/process-redundancy-review-zh.md` | 83 | 教训 → 合并 |
| `handoff-0.3.0-zh.md` | 50 | 过期交接 → 吸收后删除 |
| `handoff-0.3.3-zh.md` | 127 | 收敛为发布/交接要点 |
| `handoff-eat-occurrence-granularity-zh.md` | 209 | 交 US 的规格 → 提炼后并入退役交接 |
| `0.3x-refactor-architecture-decision-zh.md` | 319 | 0.3.x 线已收口 → 提炼仍生效的架构约束 |
| `0.3x-equivalence-review-zh.md` | 42 | 历史评审 → 提炼后删除 |
| `0.3x-release-gate-checklist-zh.md` | 23 | 0.3.0 检查表 → 提炼后删除 |
| `internal-universalization-design-note-zh.md` | 113 | 规划输入，已被 US 仓库取代 → 提炼后删除 |

## 2. 硬约束（违反即返工）

- **允许改**：`docs/**`、`MEMORY.md`、`TODO.md`、`OBLIVIONIS.md`。
- **禁止改**：`Source/`、`1.6/`、`About/`、`tools/`、`fixtures/`、`scripts/`、`.github/**`（含 `.github/skills/**` 作者指南）、`Extras/`、`LoadFolders.xml`、根 `codemap.md`、`README*`、`AGENTS.md`、`TASK-docs-consolidation-zh.md`（本文件）。
- **不新增任何检查门/脚本/CI 步骤**（维护者明令：文档不新增检查门）。
- **合同类文件路径不可变**：`docs/project-architecture-contract.md`、`docs/settings-ui-product-contract-zh.md`、`docs/logging-protocol.md` 必须保持原路径（被 `AGENTS.md`/`MEMORY.md`/runbook/作者指南/CI 引用）。其余文件允许合并/删除，但**删除前必须确认没有外部引用**（逐一 grep 引用者）。
- **只做本地 commit**：不 push、不建 PR、不 merge、不 tag、不发布。
- **隐私**：任何文档不得出现本机绝对路径、日志摘录、凭据、`PublishedFileId` 值；仓库内引用一律相对路径。
- **品牌与用语**：`鼠辈啁啾` / `Squeaky Ratkin` 不可改写；中文散文称鼠族；提交信息沿用现有风格（`docs:` / `chore:` 前缀 + 中文正文）。

## 3. 收敛判定准则

**保留（现行权威 / 在用）**：三份合同、`release-runbook-zh.md`、`steam-workshop-page-copy-draft.md`、双语 CHANGELOG、`release_review/release-*.md`（Claim Pack = 发布证据，**不得删除**）、`release_review/privacy-history-rewrite-plan-zh.md`、US 兼容性检查与迁移方案（可合并为一份退役计划）。

**吸收后删除（历史过程 / 已被取代）**：`handoff-0.3.0-zh.md`、`0.3x-equivalence-review-zh.md`、`0.3x-release-gate-checklist-zh.md`、`0.3x-refactor-architecture-decision-zh.md`、`internal-universalization-design-note-zh.md`、`handoff-0.3.3-zh.md`（收敛后）、`handoff-eat-occurrence-granularity-zh.md`（提炼后）。
吸收规则：其中**仍生效的约束**（路由中立、卸载安全、日志协议冻结、XML ABI、动作 17 键 append-only、打包纪律、兼容顺序硬门等）必须以最少文字进入合同或 `docs/lessons-zh.md`；**只有历史过程**的部分直接废弃，不保留全文。

**合并**：`release_review/process-review-zh.md` + `release_review/process-redundancy-review-zh.md` → `docs/lessons-zh.md`（≤10 条原则，每条附来源与年份；不保留两篇原文）。

**新增**：`docs/codemap.md`（docs 索引：文件 / 一句话用途 / 权威级别 / 维护者），仅为导航，不含检查逻辑。

## 4. 执行流程（严格按序）

1. **S1 冻结与建 archive**：确认 `git status` 干净；用 `git mv` 把 `docs/` 下**所有现存文档**（保留 `release_review/` 子目录结构）移入 `docs/archive/`。此后 **archive 内文件只读**：不得编辑、不得格式化。
2. **S2 研判**：通读 archive 全部内容，形成「保留 / 吸收 / 废弃」判定清单（清单可写进提交信息，不必成为最终文档）。
3. **S3 生成收敛文档**：按 §3 在 `docs/` 下写出收敛结果（保留者原文迁回并按需精简；吸收者重写；合并者单文件）。
4. **S4 提交一**：`docs: 退役期文档收敛（archive 内容研判 → 收敛文档）`。
5. **S5 删除 archive**：`git rm -r docs/archive`，提交二：`docs: 移除临时 archive 目录`。
6. **S6 记忆压缩**：按 §5 执行，提交三：`docs: MEMORY/TODO 压缩 + 冷归档入 OBLIVIONIS`。
7. **S7 自检**：按 §6 人工核对（不是新门）。

## 5. 记忆压缩要求

- `MEMORY.md`：只保留（a）项目身份与版本/发布现状 3–5 行；（b）权威入口指针（合同、runbook、作者指南、记忆文件）；（c）仍生效的耐久约束（路由中立、卸载安全、日志协议冻结、XML ABI/17 键、打包纪律、三命令门槛、Eat 两级开关默认与兜底、US 兼容顺序硬门）；（d）待裁决指针。**目标 ≤20 行**；不得出现完成矩阵、提交链、发布清单、会话叙述。
- `TODO.md`：只保留开放动作与阻塞，**目标 ≤30 行、零 `[x]`**；已完成项一律删除（其落点已有 CHANGELOG / Claim Pack / OBLIVIONIS）。
- `OBLIVIONIS.md`：接收被压缩掉、已完成或失效的记忆，沿用现有格式（`## 日期：主题（归档）` + 要点 + `**状态**：…`）；不得与现行合同冲突；冲突时以现行合同为准，并把冲突事实写进对应归档段。
- 压缩时若发现现行文档与 archive 结论冲突：**以 `AGENTS.md` → 合同 → runbook/记忆 的权威序为准**，不得静默归一。

## 6. 完成前自检（人工核对，不新增脚本）

1. `docs/archive/` 不存在；`git status` 干净。
2. `docs/codemap.md` 覆盖 `docs/` 下每个文件（用途 + 权威级别）。
3. 三份合同路径未变；`AGENTS.md` / `MEMORY.md` / `TODO.md` / `docs/release-runbook-zh.md` / `.github/skills/**` 中引用的 docs 路径逐一 grep 有效，无断链。
4. `MEMORY.md` ≤20 行、`TODO.md` ≤30 行且零 `[x]`（行数用 `Get-Content | Measure-Object -Line` 记录进交付说明）。
5. `OBLIVIONIS.md` 新增段落含日期与状态。
6. 提交信息符合 §4；未做任何远端操作（`git log origin/dev..HEAD` 只应包含本地提交）。

## 7. 交付物

- 提交清单（短 hash + 提交信息）；
- 最终 `docs/` 目录树（含行数）；
- `MEMORY.md` / `TODO.md` 最终行数；
- `OBLIVIONIS.md` 新增段落标题清单；
- 一段说明：哪些 archive 文档被吸收进哪份文档、哪些被明确废弃及理由、哪些引用核对结果。

## 8. 不要做的事

- 不执行发布/推送/PR/merge/tag/Workshop 相关任何操作；
- 不改代码、Defs、本地化、脚本、CI、作者指南；
- 不新增检查门或校验脚本；
- 不把 archive 内容原样堆回 `docs/`（必须收敛）；
- 不删除 Claim Pack 证据；
- 不修改 `AGENTS.md`（如需变更，在交付说明里提建议）；
- 不删除本任务书（由维护者确认收敛结果后处理）。
