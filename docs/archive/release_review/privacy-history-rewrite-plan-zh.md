# 隐私历史债务：重写专项方案（只出方案，不动 git 历史）

> 日期：2026-09-13。范围：Squeaky Ratkin 仓库可达历史与发布 tag 中的本机展开路径债务。
> 本文是 **V2 专项评估**（维护者选择「先出专项方案」）；执行任何 force-push / tag 重建都需要单独授权。
> 按无隐私写法编写：不复制债务字符串本身，只给形态分类与计数。

## 1. 结论摘要

- 暴露面是**本机路径**，不是凭据：工作树 0 命中、提交信息 0 命中、凭据 0 命中、`PublishedFileId` 值 0 命中；**历史 blob 5 个文件各 1 处**。
- 代价接近**全历史重写**：最早脏点落在初始提交附近（漂移 ≈229/230 提交），10 个发布 tag 全部指向受影响提交，需 force-push。
- 仓库为 **PUBLIC**（`gh repo view` 核实 `visibility=PUBLIC`、`isFork=false`），所以这是真实外露，而不是理论风险。
- 建议：**先维持方案 A（不重写 + 纪律固化，已落地）**；若维护者判定路径外露不可接受，再按 §5 的方案 B 执行。理由是债务不含凭据、形态仅为目录名，而重写不可逆面很大（§6）。
- 发布本身不受影响：分发包里 0 `codemap.md`、0 文档、0 `pdb`，债务只存在于源码历史。

## 2. 暴露面事实（不复制原文）

| 文件 | 载体 | 形态分类 | 处数 | 最早出现 |
|---|---|---|---|---|
| `CONTRIBUTING.md` | 历史 blob | 用户目录形态（暴露 Windows 用户目录名） | 1 | 初始提交附近 |
| `AGENTS.md` | 历史 blob | 用户目录形态 | 1 | 0.2.0 处理期 |
| `MEMORY.md` | 历史 blob | 用户目录形态 | 1 | 0.2.0 处理期 |
| `TODO.md` | 历史 blob | 用户目录形态 | 1 | 0.2.0 处理期 |
| `.slim/codemap.json` | 历史 blob | 工作区根形态（不含用户名） | 1 | 0.2.1 发布提交 |

- 两个形态都用「盘符 + 分隔符 + Users/WorkSpace」模式扫描；该模式刻意覆盖 JSON 转义的双反斜杠写法。
- HEAD 处 5 个文件均已净化（工作树 0 命中）——债务只存在于旧版本，因此**普通新提交无法消除它**。

## 3. 重写波及量化（决策输入）

| 文件 | 触及提交数 | 最早脏点 | 漂移提交数（越早越大） | 受影响 tag |
|---|---|---|---|---|
| `CONTRIBUTING.md` | 4 | `afa00cb3`（初始提交） | ≈229 | `v0.1.0`、`v0.1.0-rc1`、`v0.1.1` |
| `.slim/codemap.json` | 7 | `2936879a` | ≈201 | `v0.2.1`、`v0.2.2`、`v0.2.3`、`v0.2.4`、`v0.3.0`、`v0.3.2-pre1`（`v0.2.0` 干净） |
| `AGENTS.md` | 3 | `75ae4167` | ≈200 | 含上述 0.2.1+ 与 0.1.x 后代 |
| `MEMORY.md` | 3 | `75ae4167` | ≈200 | 同上 |
| `TODO.md` | 2 | `75ae4167` | ≈200 | 同上 |

- 全仓 230 提交；`v0.2.0` 是唯一内容干净的发布 tag。
- **仓内 hash 引用 54 处**（`MEMORY.md`/`OBLIVIONIS.md`/`TODO.md`/`docs/0.3x-release-gate-checklist-zh.md` 等，7–12 位短 sha）。重写后这些引用全部悬空，需要按 `commit-map` 机械替换。
- **跨仓锚点 1 处**：US 仓 `MEMORY.md` 引用 SR 提交 `b19d68a`（两次）。该提交是脏点后代 ⇒ 重写后必须显式通知 US 侧新 hash（跨仓锚点改为"显式通知义务"，不改对方文件）。
- **本机工具面**：`git 2.47.1`；`git-filter-repo` **未安装**；`git filter-branch` 可用（内置）。远端可达（`git ls-remote` 正常）。
- **备份面**：工作区存在 ferritelib / UniversalSqueaker 的 mirror 备份，**没有 SR 的 mirror 备份** ⇒ 动手前必须先建（§5 P0）。
- **远端引用面**：remote 10 个 tag（与本地同名同集合）、5 个 heads（`main`/`dev`/`0.3.x`/`kiiro-experiment`/`0.2.4-FINAL`）；本地多一个 `archive/dev-pre-sanitize-0.2.0`（未推送）。

## 4. 方案比较

| 方案 | 内容 | 成本 | 效果 | 风险 |
|---|---|---|---|---|
| **A 不重写（当前）** | 保留 `[known-debt]` 台账；继续「无隐私写法」纪律 + 每次 push 三向量门 | 0 | 债务继续存在于旧 blob；新提交不会再引入 | 无（路径非凭据） |
| **B 定向重写** | 全历史文本替换（只替换这 5 处路径文本），tags 重建，force-push | 高：≈229 提交 hash 漂移、10 tag 重建、54 处仓内引用 remap、1 处跨仓通知、所有 fork/缓存失效 | 可达历史中不再有路径 | 见 §6（不可逆、GitHub 可能仍按旧 SHA 可访问） |
| C 只改 tag 不改历史 | 仅重建 tag | 中 | **无效**：tag 指向的提交内容仍含债务 | 伪安全感 |

## 5. 方案 B 执行计划（逐条需授权）

**P0 备份与快照（只读+本地）**
1. `git clone --mirror` 到工作区命名 `<repo>-mirror-backup.git`（先例：其他仓已有同名形态备份）。
2. 记录 `git rev-parse --all` + `git tag -l --format="%(refname:short) %(objectname)"` 快照，作为回滚基线。

**P1 工具准备**
3. 安装 `git-filter-repo`（需要网络；本机 NuGet 源曾 SSL 失败，故先验证 pip/pipx 可用）；不可行则 fallback：`git filter-branch --tree-filter` 对 5 个文件做行级替换（慢但内置、无需网络）。

**P2 替换规则（关键：只改呈现层）**
4. 生成 `replacements.txt`：把「用户目录形态」与「工作区根形态」分别替换为中性占位（如 `<local-path>`）。
5. **禁止**改动：Scribe 字段名、负向断言的扫描模式、对外契约字符串、品牌名——这些"看起来像泄漏"的字符串是行为的一部分（见 `modding_documents/privacy-debt-vector-triage-zh.md` §3）。

**P3 重写与本地验证**
6. 执行重写；产出 `commit-map`。
7. 本地验证：临时清空 `$knownHistoryDebt` 后台账 → `pwsh scripts/privacy-audit.ps1 -FullHistory` 必须 **0 命中**；三条向量复扫；抽查 `git log --all` 未改坏内容。
8. 内容验证：`pwsh scripts/verify-local.ps1 -NoRestore` 全绿（证明重写未破坏工作树内容）。

**P4 tag 重建**
9. 核对 10 个 tag 名不变、全部指向新提交（filter-repo 默认重写 tag）。

**P5 推送（需授权）**
10. `git push --force-with-lease --all` + `git push --force --tags`（顺序与节奏需维护者确认）。
11. 推送后：CI 首跑、逐个 GitHub Release 页面核验（tag 关联、asset 完整性、sha256）。

**P6 事后对账**
12. 用 `commit-map` 机械更新 54 处仓内 hash 引用。
13. 通知 US 侧：`b19d68a` 的新 hash（跨仓锚点义务）。
14. 清空 `$knownHistoryDebt` 台账；MEMORY/TODO 记录本次隐私事件闭环。

**回滚**：P0 的 mirror 备份可完整恢复；回滚窗口 = 在他人 fetch/tag 重建之前。

## 6. 不可逆点与残余风险（必须在授权前确认）

1. **GitHub 不保证删除**：force-push 只让旧提交不可达；未 GC 对象可能仍可按旧 SHA 直接访问。彻底清除需要 GitHub Support purge（额外流程与时间），**不能承诺"重写即消失"**。
2. **全仓 hash 漂移**：所有已发布版本的 Claim Pack 里引用的 squash sha、与其他仓的交叉引用都会悬空（本仓 54 处 + 跨仓 1 处已知）。
3. **fork / 克隆 / CI 缓存 / 第三方归档**无法回收。
4. **Release 与 tag 关联**需要逐个复核；PR/issue 里的提交引用可能显示为 unknown。
5. **重写期间禁止任何人推送**：需要一次协调窗口（本仓单人维护，风险可控）。

## 7. 待维护者裁决（Q1–Q5）

| # | 问题 | 建议默认 |
|---|---|---|
| Q1 | 是否执行方案 B？（路径外露 vs 不可逆成本） | 先 A；B 仅在判定路径不可接受时执行 |
| Q2 | 若执行 B：用 `git-filter-repo`（需联网安装）还是内置 `filter-branch`？ | 先验证安装；否则 filter-branch |
| Q3 | 若执行 B：替换占位用什么？（如 `<local-path>`） | 中性占位，不写任何盘符形态 |
| Q4 | 是否接受"GitHub 可能仍可按旧 SHA 访问"的残余风险？ | 接受并记录；如需彻底清除另起 GitHub Support 事务 |
| Q5 | 跨仓通知：US 侧 `b19d68a` 锚点由谁通知、何时？ | 重写完成后立即，写入 US 仓的 TODO（不改对方文件） |

## 8. 附：本次评估用到的可复现命令（只读）

```powershell
# 三向量隐私门（push 前）
pwsh scripts/privacy-audit.ps1 -FullHistory -PrePush
# 每文件最早脏点与漂移
git log --all --format=%H "-G[盘符]:[\\/]{1,2}(Users|WorkSpace)" -- <path>
# tag 暴露面
foreach ($t in git tag --list) { git grep -l -I -E -e "<pattern>" $t }
# 仓内 hash 引用计数（7–12 位，与真实提交取交集）
```

> 说明：上面的正则按需自建，本文不复制债务字符串；`+` 与路径形态在脚本里以拼接方式构造，避免文档自身触发门禁。
