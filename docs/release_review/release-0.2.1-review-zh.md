# 0.2.1 发布回顾与 Claim Pack

> 本文记录 0.2.1 发布事实与 Claim Pack。历史事实优先于事后推断；外部人工状态保持 unverified，直至维护者观察并记录。

## GitHub Release Claim Pack（最终，重发后）

| 项 | 值 |
|---|---|
| 版本 | 0.2.1 |
| 标签 | `v0.2.1`（严格 SemVer，基版本 = csproj `<Version>`） |
| 源码提交 | `31c4e18ad0d98e6d5731f8e76e32fb4950ed061c`（main，PR #9 squash，重发目标） |
| 发布时间 | 2026-08-15T17:54:24Z（UTC+8 2026-08-16 01:54） |
| 构建 flavor | GitHub（`SqueakyBuildFlavor=GitHub`，CI tag 触发） |
| DLL 身份 | FileVersion `0.2.1.0`；Informational `v0.2.1+31c4e18ad0d9` |
| CI | Release workflow run `31899583405` success |
| 资产 | `SqueakyRatkin-v0.2.1.zip` |
| DLL SHA256 | `98F66F8343824FFC060436074BB73B1D38E8EC02F72FF21F9814030BB4820A72` |
| 包内容 | **115 文件**；0 PDB；0 `PublishedFileId.txt`；**0 codemap.md**；`LoadFolders.xml` 无 `IfModActive` 门控（已逐项核验）；Template↔built-in OGG 镜像校验通过 |
| 隐私审计 | 完整树扫描（凭据/本地路径/`PublishedFileId`）0 命中；dev↔main 树一致（diff 0 行） |

## 重发记录（第一次发布作废）

- 第一次发布：tag 指向 `57dfd1f`（PR #8 squash），zip 121 文件。发现两个缺陷后删除 release 与 tag 重发：
  1. **codemap 泄漏**：`stage-package.ps1` 递归复制把 6 个 `codemap.md` 导航文档带进包（0.2.0 为 115 文件，本次 121 为回归）。修复：stage 排除列表增加 `codemap.md`（PR #9）。
  2. **LoadFolders 门控回归**：merge `origin/main` 进 dev 时，`LoadFolders.xml` 被 git auto-merge——base 与 ours 相同（无门控）、theirs 变更（main 门控），三方合并采纳了 theirs 的门控版；`git checkout --ours` 只对冲突文件生效，auto-merged 文件保留了合并结果。全面 diff 确认污染面仅此 1 个文件，已恢复无门控（commit `d73a0c3`，随 PR #9 进 main）。
- 教训：main/dev 分叉（squash 历史）+ 三方 merge 时，**auto-merged 文件不受 `checkout --ours` 控制**；发布前必须逐项核验最终 tree 中的关键文件内容（本次 LoadFolders 恰好也是产品关键行为文件）。
- 第二次 merge（PR #9 前）：main `57dfd1f` tree 与 dev `20d5c50` 完全相同，采用 `git merge -s ours` 保持 dev 纯增量。

## 发布流程观察

- 流程链：dev 原子提交 `2936879` → CI → PR #8（冲突以 dev 优先解决）→ squash（main `57dfd1f`）→ 发现两缺陷 → 删除 release/tag → PR #9（codemap 排除 + LoadFolders 恢复 + 发布卫生）→ squash（main `31c4e18`）→ tag `v0.2.1` → release CI success → artifact 逐项核验。
- main 独有提交 `1b1fe9e`（"fix: include debug actions in release builds"）在 dev 中已等价存在，未丢失实质内容。
- CI 的 Node 20 弃用 warning（checkout/setup-dotnet/softprops 被强制运行在 Node 24）为非阻断警告，与 0.2.0 时一致。

## Steam 发布观察（2026-08-16 页面核验）

| 项 | 观察 |
|---|---|
| 页面 URL/ID | `https://steamcommunity.com/sharedfiles/filedetails/?id=3758115669`（与仓库维护源一致，同一 item） |
| 描述 | 新版英文文案已生效（`Mod version: 0.2.1`、无统计/排障/制作步骤、末尾 `Hide The Book of Squeakudges on a high stool!`） |
| Updated | Aug 15（Steam 页面时间），与发布会话吻合 |
| File Size | 1.871 MB |
| Change Notes | 3 条（维护者自行维护，仓库不含 change note） |
| 互动 | 79 评分、37 评论（发布前既有） |
| visibility / preview | 公开可访问；预览图存在 |

注：页面描述与 Updated 时间佐证 0.2.1 已上线；**实际订阅下载的二进制版本未在游戏内验证**——描述/Updated/文件大小与本地 Steam 包（115 文件）一致，但"玩家下载到的 DLL 就是 0.2.1.0"仍属需游戏内确认的边界，当前记为"页面级已核验、二进制级一致但未下载验证"。

## Steam staging 包（本地产物）

| 项 | 值 |
|---|---|
| 包 | `dist/steam/SqueakyRatkin`（**115 文件**，`pack-steam.ps1` 产物，重打包后） |
| LoadFolders | 无 `IfModActive` 门控（已核验） |
| codemap | 0（已核验） |
| DLL FileVersion | `0.2.1.0`（Steam flavor，运行时日志只记包版本） |
| PublishedFileId | 0（stage 排除） |

Steam 发布为人工步骤：复制 stage 到本地上传副本 → 把既有 item ID 只写入副本 `About/PublishedFileId.txt` → 以同一作者 Update（后续版本绝不用 `Initial Workshop Upload`）→ 人工核对同一 item URL/ID、visibility、预览与描述。

## 渠道状态

- **GitHub**：完整（tag + CI success + 预期 asset + DLL 身份/哈希/包内容逐项核验）。
- **Workshop**：页面级已核验（同一 item、描述 0.2.1、visibility、preview、Updated、文件大小）；二进制下载级验证未做（如后续要闭环，可订阅后在游戏内核对 DLL 版本）。
