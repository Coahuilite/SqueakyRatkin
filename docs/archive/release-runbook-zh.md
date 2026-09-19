# Release Runbook（版本无关）

> 唯一发布流程入口。版本相关事实以当次 `docs/release_review/release-<version>-review-zh.md`（Claim Pack）为准；本文只描述流程与门禁。
> 本地 `commit` 不需要授权；**远端 push / PR / merge / tag / Release / Workshop 上架**都需要维护者明确授权（见 `AGENTS.md`「External-state boundaries」）。
> 门禁分工：**能机械判定的都不写成本文的人工清单**——写时断言在 `stage-package.ps1`，读时断言在 `check-pack-readiness.ps1`，隐私面在 `privacy-audit.ps1`。本文只负责顺序、授权点与人工不可替代的步骤。
> 2026-09-13 精简：三处重复的包内容清单收敛为单一读时门；隐私审查收敛为单一脚本；入口契约与最小仪式见下。评估记录见 `docs/release_review/process-redundancy-review-zh.md`。

## 入口契约（三个命令，一个门只由一个命令负责）

| 场景 | 命令 | 归属 |
|---|---|---|
| 日常开发门（harness + 双 flavor 构建） | `pwsh scripts/verify-local.ps1` | 本机 / CI |
| 发布面（版本轴 / 仓库红线 / 暂存包复核 / DLL 身份 / `[claim]` 快照） | `pwsh scripts/check-pack-readiness.ps1 -RequireReleaseMetadata`（默认组合上面的 verify-local） | 发布会话 / release.yml |

> 离线/已还原缓存环境：以上命令一律加 `-NoRestore`（csproj 用浮动版本 `1.6.*`，隐式 restore 需要网络）。CI 有网络，故 workflow 里显式 restore 后跑 `-NoRestore`。
| 隐私面（三向量 + 身份 + 机械自检） | `pwsh scripts/privacy-audit.ps1 -FullHistory -PrePush` | push 前人工 / CI 默认模式 |

包内容排除项（`*.pdb` / `*.gitkeep` / `codemap.md` / `PublishedFileId.txt`）、`version.txt` 三行、`About.xml == csproj`、Template↔内置 OGG 镜像由 `stage-package.ps1` 在写入时断言，`check-pack-readiness.ps1` 在发布前复核——**任何渠道都不再手工逐项核验**。

## 最小发布仪式（对齐 UniversalSqueaker 2026-09-06 维护者裁决）

push 前人工只有三件事：

1. `pwsh scripts/check-pack-readiness.ps1 -RequireReleaseMetadata` —— 发布面必须全绿（并打印 `[claim]` 快照，Claim Pack 直接引用，不转抄）。
2. `pwsh scripts/privacy-audit.ps1 -FullHistory -PrePush` —— 三向量 + 身份 + 工作树干净 + 待推送提交 + tag 集合确认。
3. 发布裁决本身：版本号、渠道、是否发、Workshop 文案。

新检查一律先落脚本再写进本文；**不再新增人工清单**。

## 阶段 0 · dev 发布前准备

1. **版本**：`Source/SqueakyRatkin/SqueakyRatkin.csproj` `<Version>` 唯一主源，`About/About.xml` `<modVersion>` 跟随。两处一致性与 DLL 身份由 `check-pack-readiness` 断言，不再手查。
2. **文档同步**（与代码同批 commit）：
   - CHANGELOG 双语加 `Unreleased — X.Y.Z`：开发者功能解锁细节模糊化、不写 change note、中英同步；
   - README / MEMORY / 根 codemap 的版本锚点同步（写法上只保留一处唯一维护源，其余用指针，减少每次发布的同步面）；
   - Workshop 页面文案（如页面内容有变）：中英对称、英文以中文版为准、页面专注模组本身（无音频统计/开发者排障/迁移说明/制作步骤，作者内容只留指南链接）、俏皮句不加解释；**一次变更一次核对**（版本号、下载链接、字符数、清单），规则见 `docs/steam-workshop-page-copy-draft.md`。
3. **门**：`check-pack-readiness`（含 verify-local）+ `privacy-audit`（默认模式）。
4. **打包**（需要 dev 试用或 Steam 时）：`build-dev.ps1` / `build-steam.ps1`（Steam 一步入口，含干净树硬门）；包内容不再人工逐项核验，改由 `check-pack-readiness` 复核暂存包并输出文件数/版本。

## 阶段 1 · 大版本分支 → main

5. 原子 commit（本地，无需授权）→ **授权后** push 对应大版本分支（如 `0.3.x`）与 `dev`；CI 通过（CI 已含 verify-local + privacy-audit）。
6. **发布路径 = 大版本分支 → `main`**（例：`0.3.x` → `main`）。`main` 受保护时开 PR，**源分支仍是大版本分支**；`dev` 只做集成，不作为发布合并源。
7. merge 后核验：`git diff --stat <大版本分支> main` 为 0 行（tree 相等）；关键行为文件（`LoadFolders.xml`/csproj/`About.xml`）抽查；受保护则 PR squash，非受保护时 ff/merge 均可。
8. `archive/` 分支**只在最终版本定稿时建立**（例如 1.0.0 归档上一代发布线）；中间版本不建 archive 分支。历史 `merge -s ours` 分叉处理只在出现真实分叉时才用。

## 阶段 2 · GitHub 发布

9. main 树 == dev 树核验（`git diff --stat` 0 行）。隐私面已由 CI + push 前 `-FullHistory` 覆盖，不再单独重复扫描。
10. **授权后** tag `vX.Y.Z`（严格 SemVer，基版本 = csproj `<Version>`）→ push → release CI（先跑 verify-local + `check-pack-readiness -RequireReleaseMetadata`，再构建 GitHub flavor 并打包）→ 资产核验：以 CI 输出的文件数/哈希为准，需要本机对照时用 `pack-github.ps1` 复现。
11. changelog 时间替换：`Unreleased` → 最终发布时间（UTC+8；**tag 重发后必须再更新**）。

## 阶段 3 · Steam 发布（人工 + 页面核验）

12. `build-steam.ps1`（Steam flavor 构建 + pack-steam 干净树硬门）→ `check-pack-readiness` 复核 `dist/steam/SqueakyRatkin`。
13. 维护者：复制 stage 到本地上传副本 → 把既有 item ID **只写入副本** `About/PublishedFileId.txt` → 以同一作者 Update（后续版本绝不用 `Initial Workshop Upload`）。
14. 维护者：粘贴中英文案 → Steam 编辑器 + 实际页面双预览。
15. **页面核对（只读 filedetails 玩家视角）**：同一 item URL/ID、描述版本、Updated、visibility、preview、文件大小、change notes 数；顺带扫 Comments 区未处理反馈（疑点记入 TODO）。**绝不尝试登录态或编辑界面**（Steam 编辑 = 维护者人工操作）。

## 阶段 4 · 收尾

16. Release Claim Pack 写入 `docs/release_review/release-<version>-review-zh.md`（模板见下；字段优先取自 `check-pack-readiness` 的 `[claim]` 快照与 CI 输出）。
17. dev 对账：merge main 回 dev，验证无意外树变化。
18. MEMORY / TODO 更新。

## 隐私审查门禁（push 前，含临时暂存）

- 命令：`pwsh scripts/privacy-audit.ps1 -FullHistory`；CI 每次 push/PR 跑默认模式（工作树 + 提交信息 + 身份）。
- **三个向量独立扫，结论不得互推**：工作树 / 提交信息 / 历史 blob——工作树干净不蕴含历史干净（SR 实测：工作树 0 命中，历史 5 个文件命中）。
- 身份面：author / committer 必须全部是 GitHub noreply 地址（含 GitHub 自身的 bot committer）；出现真实邮箱即失败。
- 已知历史债务：`privacy-audit.ps1` 内 `$knownHistoryDebt` 台账（当前 5 条：`.slim/codemap.json` 与 4 份 HEAD 已净化文档的旧版本）。命中按 `[known-debt]` 列出、不判失败；**新增一条 = 一次维护者裁决**，历史/tag 重写另行授权。
- 扫描模式：凭据（`sk-…`/API key/token）、私钥、本地绝对路径、诊断日志摘录、`PublishedFileId.txt` 值。
- 文档按**无隐私写法**编写：写作时不写入个人本地状态、展开路径、日志摘录、凭据或 ID 值，而非写后清理。

## Release Claim Pack 固定模板

```text
## GitHub Release Claim Pack
| 项 | 值 |
|---|---|
| 版本 | X.Y.Z |
| 标签 | vX.Y.Z（严格 SemVer，基版本 = csproj <Version>） |
| 源码提交 | <main squash 完整 SHA> |
| 发布时间 | <UTC 时间>（UTC+8 <时间>） |
| 构建 flavor | GitHub（CI tag 触发） |
| DLL 身份 | FileVersion <V>；Informational v<V>+<sha12> |
| CI | Release workflow run <id> success |
| 资产 | <zip 名>（<字节数> B） |
| DLL SHA256 | <hash> |
| 包内容 | 文件数；0 PDB；0 PublishedFileId.txt；0 codemap.md；关键文件内容核验；OGG 镜像校验；包内 version.txt（版本/flavor/commit） |
| 隐私审计 | 完整树扫描 0 未接受命中；dev↔main 树一致 |
```

## Steam staging / 发布观察
- staging 包：文件数、排除项、DLL FileVersion、PublishedFileId=0、包内 version.txt（版本/flavor/commit 与预期一致）。
- 页面观察：同一 item URL/ID、描述版本、Updated、visibility、preview、文件大小、change notes。
- 二进制下载级验证边界如实记录（页面级核验 ≠ 玩家下载内容已验证）。

## 渠道状态
- GitHub：完整 / 待补。
- Workshop：unverified / 页面级已核验 / 完整。
