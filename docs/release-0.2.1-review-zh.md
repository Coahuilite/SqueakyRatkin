# 0.2.1 发布回顾与 Claim Pack

> 本文记录 0.2.1 发布事实与 Claim Pack。历史事实优先于事后推断；外部人工状态保持 unverified，直至维护者观察并记录。

## GitHub Release Claim Pack

| 项 | 值 |
|---|---|
| 版本 | 0.2.1 |
| 标签 | `v0.2.1`（严格 SemVer，基版本 = csproj `<Version>`） |
| 源码提交 | `57dfd1f58235921b0f30b36403edd074c1b256b5`（main，PR #8 squash） |
| 发布时间 | 2026-08-15T17:29:04Z（UTC+8 2026-08-16 01:29） |
| 构建 flavor | GitHub（`SqueakyBuildFlavor=GitHub`，CI tag 触发） |
| DLL 身份 | FileVersion `0.2.1.0`；Informational `v0.2.1+57dfd1f58235` |
| CI | Release workflow run `31898389054` success |
| 资产 | `SqueakyRatkin-v0.2.1.zip`（1,591,453 B） |
| DLL SHA256 | `2ADD376E17D346E696AEC21CC40A61DEF87BDA3869990428A6D2BBD786A6D478` |
| 包内容 | 121 文件；0 PDB；0 `PublishedFileId.txt`；Template↔built-in OGG 镜像校验通过 |
| 隐私审计 | 完整树扫描（凭据/本地路径/`PublishedFileId`）0 命中；dev↔main 树一致（diff 0 行） |

## 发布流程观察

- 本次流程：dev 原子提交 `2936879` → push → dev CI success → PR #8 → 冲突（main/dev 自 0.1.1 分叉）→ 本地 merge `origin/main` 进 dev（dev 优先，22 文件冲突）→ 重新审计 + GitHub flavor build 0 errors → push → PR CI success → squash merge（main `57dfd1f`）→ tag `v0.2.1` → release CI success → artifact 核验。
- 冲突解决策略：dev 优先。main 独有提交 `1b1fe9e`（"fix: include debug actions in release builds"，移除 `#if SQUEAKY_DEV` 门）在 dev 中已等价存在（dev 版 `SqueakDebugActions.cs` 无该门），未丢失实质内容；main 其余差异为文档等价措辞改写。
- CI 的 Node 20 弃用 warning（checkout/setup-dotnet/softprops 被强制运行在 Node 24）为非阻断警告，与 0.2.0 时一致。

## Steam staging（未发布，人工步骤待执行）

| 项 | 值 |
|---|---|
| 包 | `dist/steam/SqueakyRatkin`（121 文件，`pack-steam.ps1` 产物） |
| DLL FileVersion | `0.2.1.0`（Steam flavor，运行时日志只记包版本） |
| DLL SHA256 | `D02415F0F8E92DB1F82958DA876FE692F50B94BDFBC2603EBDBCEBBE2BABD527` |
| PublishedFileId | 0（stage 排除） |

Steam 发布为人工步骤：复制 stage 到本地上传副本 → 把既有 item ID 只写入副本 `About/PublishedFileId.txt` → 以同一作者 Update（后续版本绝不用 `Initial Workshop Upload`）→ 人工核对同一 item URL/ID、visibility、预览与描述。仓库证据不能证明 Workshop 外部状态。

## 渠道状态

- **GitHub**：完整（tag + CI success + 预期 asset + DLL 身份/哈希核验）。
- **Workshop**：unverified——待维护者上传并人工观察后，在本文补充观察记录。
