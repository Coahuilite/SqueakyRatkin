# Squeaky Ratkin 0.2.2 Release Review

> 本文记录 0.2.2 发布事实与 Claim Pack。历史事实优先于事后推断；外部人工状态保持 unverified，直至维护者观察并记录。

## GitHub Release Claim Pack

| 项 | 值 |
|---|---|
| 版本 | 0.2.2 |
| 标签 | `v0.2.2`（严格 SemVer，基版本 = csproj `<Version>`） |
| 源码提交 | `03ebb6be20896071185193d94e8bf1006862f969`（main，PR #10 squash） |
| 发布时间 | 2026-08-16T15:17:38Z（UTC+8 2026-08-16 23:17） |
| 构建 flavor | GitHub（`SqueakyBuildFlavor=GitHub`，CI tag 触发） |
| DLL 身份 | FileVersion `0.2.2.0`；Informational `v0.2.2+03ebb6be2089` |
| CI | Release workflow run `31955162848` success |
| 资产 | `SqueakyRatkin-v0.2.2.zip`（1,573,976 B） |
| DLL SHA256 | `3B0D1E8C969C44180C88BFA45C157A077F2392ED9345D5ED692290BA6C223C1A` |
| 包内容 | **115 文件**；0 PDB；0 `PublishedFileId.txt`；0 codemap.md；`LoadFolders.xml` 无 `IfModActive` 门控；Template↔built-in OGG 镜像校验通过 |
| 隐私审计 | 完整可达范围扫描 0 真实命中；dev↔main 树一致（diff 0 行） |

## Steam staging / 发布观察

- staging 包：115 文件；0 PDB / 0 `PublishedFileId.txt` / 0 codemap.md；DLL FileVersion `0.2.2.0`；`LoadFolders.xml` 无门控；OGG 镜像校验通过。
- 发布观察：unverified（维护者待手动上传 Workshop；`PublishedFileId.txt` 只写入本地上传副本）。

## 渠道状态

- GitHub：完整。
- Workshop：unverified。

## 0.2.2 内容回顾（卫生与可读化，无玩家可见行为变化）

- 日志模块拆分为公开门面 + internal 协议模块，`tools/SqueakLogCharacterization` 双 flavor 护栏锁定 28-event `srdiag v1` 协议。
- `CompSqueaker` 相位计算去重、社交任务标记集中、死代码删除。
- `About.xml` 增加 `<modVersion>`；AGENTS.md 补充版本主源规则。
- 内部通用化设计笔记（0.3.x 规划输入）落地；Kiiro 实验不包含于本发布。
- 发布前分叉处理：merge origin/main 进 dev（changelog/记忆按意图取 dev 侧），删除 main 旧路径重复 review 文件，随 PR #10 完成 docs 重组收口。
