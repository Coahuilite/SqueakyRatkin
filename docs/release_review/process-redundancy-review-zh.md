# 发布流程冗余评估与简化（对齐 UniversalSqueaker）

> 日期：2026-09-13。分支 `dev`。评估对象：SR 的发布流程（`docs/release-runbook-zh.md` + `docs/release_review/process-review-zh.md` + 脚本 + CI）。
> 基准：UniversalSqueaker（US）的「三命令 + 最小仪式」流程与其 2026-09-06 维护者裁决。
> 结论口径：**门禁覆盖没有缺失，冗余在「谁来做」**——机械可判定的不变量被写进 runbook 反复人工执行，同一不变量最多出现 4 处人工表述。
> 本文按无隐私写法编写：不含本机绝对路径、日志摘录、凭据或 `PublishedFileId` 值。

## 1. 结论摘要

评估前 → 评估后（不变量视角）：

| 不变量 | 评估前 | 评估后 |
|---|---|---|
| `About.xml <modVersion>` == csproj `<Version>` | runbook 手查 1 处 + `stage-package` 写时断言 + `release.yml` 断言（3 处表述） | 单一读时门 `check-pack-readiness.ps1`；runbook 不再手查 |
| 包内容（排除项 / `version.txt` / DLL 身份 / OGG 镜像） | **同一张清单在阶段 0 / 2 / 3 出现 3 次**，每次人工逐项核验 | `stage-package` 写时断言 + `check-pack-readiness` 读时复核（各一次） |
| 隐私审查 | 4 处人工表述（阶段 0.3 / 1.7 / 2.9 + AGENTS），**0 脚本** | `privacy-audit.ps1` 三向量 + 身份（CI 默认模式；push 前 `-FullHistory -PrePush`） |
| Claim Pack 字段（DLL 身份 / 包文件数 / `version.txt`） | 手工逐项转抄 | 门输出 `[claim]` 快照，直接引用 |
| 本地 commit | AGENTS 列为「需要授权」的操作 | 本地 commit 允许；远端操作（push/PR/merge/tag/Release/Workshop）需授权 |
| CI 覆盖 | 只构建 + 打 dev 包（本地门只在人工会话里跑） | CI 跑 `verify-local`（11 项）+ 隐私门；tag 构建再加发布面门 |

判定：**采纳 5 项（已落地）、待裁决 3 项、明确不做 3 项**。

## 2. 基准：US 的简化流程（可核实事实）

| US 事实 | 位置 |
|---|---|
| 三命令分工：`verify-local.ps1`（开发门）/ `check-pack-readiness.ps1`（发布面，默认组合开发门）/ `privacy-audit.ps1`（隐私面，`-FullHistory` 供 push 前） | US `scripts/`、`MEMORY.md` |
| CI 每次 push/PR 跑 `verify-local.ps1 -NoRestore` + `privacy-audit.ps1`（默认模式）；tag 构建先跑 `verify-local` + `check-pack-readiness -SkipVerify -RequireReleaseMetadata` | US `.github/workflows/ci.yml`、`release.yml` |
| 隐私门三向量（工作树 / 提交信息 / 历史 blob）独立扫，结论不得互推；身份面单列 | US `scripts/privacy-audit.ps1` + `modding_documents/privacy-debt-vector-triage-zh.md` |
| **最小发布仪式裁决（2026-09-06）**：push 前人工只跑 `privacy-audit.ps1 -FullHistory` + 机械自检；其余全部自动化；**禁止重新加回人工仪式** | US `AGENTS.md`、`MEMORY.md` |
| 本地 commit 允许；remote/push/PR/tag/release 需授权 | US `AGENTS.md`「External-state boundaries」 |

SR 与 US 的差异是产品面的（SR 有内置音频/Extras/三渠道、Steam 是主渠道），**不是门禁方式的**——所以可迁移的是「门禁归属」，不是清单内容。

## 3. SR 现状：冗余清单与逐条裁决

| # | 现状（位置 × 次数） | 冗余性质 | 裁决 |
|---|---|---|---|
| R1 | 版本一致性：runbook 阶段 0.1 手查 + `stage-package` + `release.yml` | 同一不变量 3 处表述，人工那处不增量 | **降级为脚本**（`check-pack-readiness` 断言，runbook 只留「唯一主源」表述） |
| R2 | 包内容逐项核验：阶段 0.4 / 2.10 / 3.12 **同一张清单 3 份拷贝** | 复制型冗余；写时断言已保证 | **收敛为单一读时门** + runbook 指针 |
| R3 | 隐私审查：阶段 0.3 / 1.7 / 2.9 + AGENTS 各一次人工「完整可达范围」扫描 | 无脚本、无固定模式、无法复现 | **新增脚本**（`privacy-audit.ps1`），runbook 收敛为 1 条命令 |
| R4 | 身份面从未单列检查 | 覆盖缺口（不是冗余） | **纳入隐私门**（author/committer 必须全为 noreply） |
| R5 | Claim Pack 字段手工转抄（DLL `FileVersion`/`Informational`、包文件数、`version.txt`） | 人工搬运 + 转抄错误面 | **门输出 `[claim]` 快照**（只用仓内相对路径） |
| R6 | `process-review-zh.md` 的 8 条措施与 runbook 内容重叠 | 历史台账与现行流程同构 | **保留为历史台账、不再增长**；新裁决只进 runbook + 本文 |
| R7 | 本地门只在人工会话跑，CI 不跑（`ci.yml` 仅构建 + dev 包） | 门禁依赖人的记性 | **CI 接线**（`verify-local` + 隐私门；release 再加发布面门） |
| R8 | AGENTS 把本地 `commit` 列为需授权操作 | 每次本地推进都要一次人工口令 | **采纳 US 口径**：本地 commit 允许，远端操作需授权 |
| R9 | runbook 模板写 `docs/release-<version>-review-zh.md`，实际目录是 `docs/release_review/` | 文档自身不一致 | **已修正**（统一到 `docs/release_review/`） |
| R10 | 双轨发布（dev 包试用 + GitHub + Steam 三段式） | 产品决策，不是冗余 | **保留**（SR 的 Steam 是主渠道；三渠道共用同一读时门） |
| R11 | Steam 页面人工编辑/预览、Workshop 文案一致性 | 认证边界决定不可自动化 | **保留**（只读核验自动化；编辑仍是维护者人工） |
| R12 | 发布决策（是否发、版本号、渠道、热修方案） | 人的判断 | **保留**（这就是最小仪式第 3 条） |

「不做」清单：

- N1：不动 `verify-local` 的 11 项——它们锁行为（语料/协议/设置），不是仪式；删它们等于删覆盖。
- N2：不引入 US 的 carrier/前置依赖门（SR 无该依赖）。
- N3：不自动生成 CHANGELOG/Release notes 正文（生成式文案仍需人工；只把**字段**自动化）。

## 4. 本会话已落地（文件与职责）

| 文件 | 变更 | 职责 |
|---|---|---|
| `scripts/privacy-audit.ps1` | 新增 | 三向量独立扫描（工作树 / 提交信息 / 历史 blob）+ 身份面；`$knownHistoryDebt` 台账；`-FullHistory`、`-PrePush` 两个开关；退出码 0/1 |
| `scripts/check-pack-readiness.ps1` | 新增 | 版本轴 / 仓库红线 / 暂存包复核 / DLL 身份；`-SkipVerify`、`-RequireReleaseMetadata`；通过后打印 `[claim]` 快照 |
| `docs/release-runbook-zh.md` | 重写 | 入口契约（三命令）+ 最小发布仪式 + 阶段 0–4 去掉重复清单、只留顺序/授权点/人工步骤 |
| `AGENTS.md` | 增补 | 「External-state boundaries」：本地 commit 允许；最小仪式裁决（2026-09-13）；禁止重新加回人工仪式 |
| `.github/workflows/ci.yml` | 修改 | restore（glob）→ `verify-local -NoRestore` → `privacy-audit` → 既有构建/打包；SDK 由 8.0.x 对齐到本地证据基线 10.0.x；concurrency + timeout |
| `.github/workflows/release.yml` | 修改 | restore → `verify-local` → `check-pack-readiness -SkipVerify -RequireReleaseMetadata` → 既有构建/打包/Release |
| `scripts/codemap.md` | 同步 | 新脚本条目、CI/Release 流程、三命令入口契约 |

## 5. 实测证据（本会话，可复跑）

- `pwsh scripts/verify-local.ps1 -NoRestore`：**11/11 全绿**（EXIT 0；本机实测约 11 秒，故 CI 加门成本可忽略）。
- `pwsh scripts/check-pack-readiness.ps1 -RequireReleaseMetadata -NoRestore`：**all checks passed**（EXIT 0；组合 verify-local 11/11 + 全部发布面检查 + `[claim]` 快照）。
- `pwsh scripts/privacy-audit.ps1`（默认）：CLEAN（EXIT 0）。
- `pwsh scripts/privacy-audit.ps1 -FullHistory`：**227 个 revision 全扫**，CLEAN（EXIT 0）——未接受命中 0；已知债务 5 条以 `[known-debt]` 列出。
- **负向自测**（隔离的一次性仓库，注入合成泄露串后自毁）：三向量 + 身份面全部触发、EXIT 1 —— 门不是橡皮图章。
- 未验证面：CI 改动要推送后才实际运行；Steam 渠道本轮不涉及。**外部渠道状态一律不因本文件而视为已验证。**

## 6. 本轮新发现（写入记忆的耐久事实）

1. **历史隐私债务范围比既有记录更大**：既记录只提 `.slim/codemap.json`；实测 227 revision 扫描命中 **5 个文件**——`.slim/codemap.json` 与 4 份 HEAD 已净化文档的旧版本（`AGENTS.md`/`CONTRIBUTING.md`/`MEMORY.md`/`TODO.md`）。**凭据 0 命中、`PublishedFileId` 值 0 命中**。历史/tag 重写的波及面因此需要按 5 个文件重估（仍待授权）。
2. **身份面：3 个唯一身份**，全部是 GitHub noreply（维护者两个显示名 + GitHub 自身的 bot committer）。US 的「身份必须唯一」断言不适用于 SR；SR 断的是隐私相关不变量（全部 noreply），显示名漂移与 bot 提交不误伤。
3. **`dist/` 里有 3 个陈旧暂存包**（dev `0.3.2-EXP`、steam `0.3.0`、github `0.3.2-pre1`）。新增的读时门会把它们标为 `[note] stale artifact, do not upload it`——这正是「人工清单会漏、脚本不会漏」的实例。
4. 本地 DLL 身份：`FileVersion=0.3.3.0`、`ProductVersion=0.3.3+<40 位 sha>`；release 通道由 CI 显式注入 `v<tag>+<sha12>`。二者形状不同，读时门只断言 `FileVersion` 全等 + `ProductVersion` 含产品版本。

## 7. 裁决记录（2026-09-13 定案）

| 编号 | 事项 | 裁决 | 落地 |
|---|---|---|---|
| V1 | 本地 commit 是否免逐次授权（US 口径） | **追认**：本地 commit 免授权；远端 push/PR/tag/Release/Workshop 仍逐次授权 | `AGENTS.md`「External-state boundaries」保留 |
| V2 | 历史/tag 重写 | **先出专项方案，不动 git 历史**：当前维持方案 A（不重写 + 纪律固化）；方案 B 执行计划与不可逆点已备 | 专项方案 [`privacy-history-rewrite-plan-zh.md`](./privacy-history-rewrite-plan-zh.md)（5 文件 × 1 处；漂移 ≈229/230；10 tag；仓内 54 处 hash 引用；跨仓 1 处） |
| V3 | CI 加门 + SDK 对齐 10.0.x | **接受**：CI 跑 11 项门 + 隐私门；runner SDK 与本地证据基线同 major | 已写入 `ci.yml`/`release.yml`；推送后首跑即真验 |

## 8. 风险与反例（这次简化可能错在哪）

- **门变橡皮图章**：因此保留负向自测（注入泄露必须 FAIL），并让 `-FullHistory` 输出可复现的计数（227 revision / 5 文件）。
- **`known-debt` 掩盖新泄漏**：台账按「向量 + 精确文件路径」匹配，**任何新文件、新向量、新模式的命中一律失败**；新增一条要写进维护者裁决记录。
- **清单搬进脚本后被悄悄弱化**：新门刻意覆盖旧清单的全部条目（排除项/版本三行/DLL 身份/OGG 镜像/无 `PublishedFileId`），且 `stage-package` 的写时断言保持不动——删的只是重复表述，不是断言。
- **测试流程本身制造副作用**：本轮出现过一次「负向自测的失败路径把探针提交写进了真实仓库」的事故（已 `reset` + `gc --prune` 清除，HEAD 未受影响）。教训：**门的自测必须在一次性仓库里做**；这也是 `[claim] localHead` 输出能立刻发现异常的原因。
- **CI 未实跑**：workflow 语法未在本机执行验证（本地无该 runner），推送首跑是第一次真验；失败会红在 CI 而不是静默。
- **离线环境下的隐式 restore 会破坏构建态**（本轮实测）：csproj 使用浮动版本 `1.6.*`，无网络时 `dotnet build`（不带 `--no-restore`）会以 `NU1301` 失败并**覆写 `obj/project.assets.json`**，之后所有 `--no-restore` 构建报 `MSB3644`（找不到 net472 引用程序集），直到从本机 NuGet 全局包缓存做一次离线还原（以缓存目录为 `--source`）。缓解：新门提供 `-NoRestore` 透传并在 runbook 写明；这是**环境事实**，不是门缺陷。

## 9. 后续

1. 发布推进按新 runbook 走：`check-pack-readiness -RequireReleaseMetadata` + `privacy-audit -FullHistory -PrePush`，本地提交后停在远端推送前（推送需授权）。
2. 推送会话：授权后 push dev → PR → merge → tag → Release；Claim Pack 用 `[claim]` 快照 + CI 输出填写。
3. V1–V3 裁决后回写本文件（把「待裁决」改为「已定案」）与 `MEMORY.md`/`TODO.md`。
