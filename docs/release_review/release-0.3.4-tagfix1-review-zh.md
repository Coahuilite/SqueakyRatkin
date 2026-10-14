# Release 0.3.4-tagfix1 发布记录

> 状态：GitHub prerelease 与资产已发布并核验；Steam 尚未上传。维护者已授权版本发布，并将通过 RimWorld 游戏内 Update 手工更新既有工坊条目及自行验收标签。

## 范围与版本身份

本次只为更新 Steam 创意工坊版本标签，不含运行时或功能改动；RimWorld 1.6 / 1.7 支持不变。产品基础版本保持 0.3.4，About `<modVersion>` 使用 `0.3.4`，DLL `FileVersion` 使用 `0.3.4.0`；GitHub prerelease/tag 使用 `v0.3.4-tagfix1`，Steam 包 `version.txt` 标识 `0.3.4-tagfix1`，DLL `InformationalVersion` 包含该后缀。`0.3.4-tagfix1` 是合法 SemVer 2.0 prerelease 标识。

PR #42 已合并，tag `v0.3.4-tagfix1` 指向 main 的发布提交。此次接受沿用既有游戏证据：RimWorld 1.6 下 Steam 0.3.4 与 RimWorld 1.7 下 Steam 0.3.3。没有进行 0.3.4-tagfix1 新实机测试；既有证据边界见 [0.3.4 记录](release-0.3.4-review-zh.md) 与 [维护状态](../maintenance-status-zh.md)。

## 渠道与发布证据

此前 SteamCMD 上传 0.3.4 未更新工坊版本标签；本次改由维护者在游戏内 Update 上传候选。维护者手工录入本版本双语 Change Notes 并自行验收标签，agent 不代为线上核验。先前公开页面的核验版本为 0.3.4，本轮未做新的工坊页面观察。GitHub Release notes 继续自动根据 PR 生成。

| 项 | 当前状态 |
| --- | --- |
| 版本分支 / PR | PR #42 已合并；tag `v0.3.4-tagfix1` 指向 main squash 提交 `d06b607c7b1720bb3c32a28db3e57e989a61d434` |
| About、csproj、DLL、Steam version.txt | About/csproj 基础版本 0.3.4；DLL FileVersion 0.3.4.0，InformationalVersion `0.3.4-tagfix1+d06b607c7b1720bb3c32a28db3e57e989a61d434`。Steam 包 116 files，`version.txt` 身份正确；无 PDB / PublishedFileId |
| 构建与资产 | readiness 在 tag 源码提交 clean 状态通过。ZIP `SqueakyRatkin-v0.3.4-tagfix1-steam.zip`，1,590,918 bytes，SHA256 `d8ab8e905082bced75a150cc7e0cd74ecb0b2c66fb976ccf1fbc441cd317f858`；ZIP 实读 116 files。DLL SHA256 `9b141cd9f176f7136f631eb9afca2f87dc2a2661ee759a4c0a3020a6a489589e` |
| 本地验证与 CI | `verify-local.ps1 -NoRestore` 12 项检查通过；合成发布门 16 cases 通过；完整历史隐私门通过，既有 76 项精确历史例外保留。新 push CI `38558588595`、PR CI `38558589434`、main CI `38559003556` 与 Release CI `38559177634` 均成功 |
| GitHub prerelease | [v0.3.4-tagfix1](https://github.com/Coahuilite/SqueakyRatkin/releases/tag/v0.3.4-tagfix1) 已于 2026-10-14 19:50:41（UTC+8）发布；`draft=false`、`prerelease=true`，tag 指向上述 main 发布提交 |
| GitHub 资产 | `SqueakyRatkin-v0.3.4-tagfix1.zip`，1,590,798 bytes，SHA256/API digest `753aae154ce16a150859fa118dce6415afd151bfed344ef2157f62296eddbe4d`；下载核验副本位于 `dist/verification/github-v0.3.4-tagfix1`。ZIP 实读 116 files、无 PDB/PublishedFileId、仅 1 个 SR DLL；About 基础版本 0.3.4、支持 1.6/1.7，双 LoadFolders 映射正确；version.txt 为 tagfix1/buildgithub/对应 commit。DLL 资源读取 FileVersion 0.3.4.0、ProductVersion `v0.3.4-tagfix1+d06b607c7b17`；未执行程序集 |
| Steam 工坊更新 | Steam 候选 ZIP 与仓库外含原工坊标记的手工上传副本均已准备；ZIP 身份见上文 Steam 候选记录。待维护者游戏内 Update、手工录入 Change Notes 并自行验收标签；未上传或检查公开页面 |
| 覆盖边界 | 不新增游戏实测；Steam 玩家侧下载核验状态待记录 |

旧 0.3.4 候选已归档到 `dist/archive-0.3.4-20261014T113826Z/`；本次 Steam 包固定于发布 tag，不随后续文档收尾提交重建。

不把后缀变更或 About 版本字段本身视为 Steam 标签问题已解决。本轮未执行 Steam 上传，玩家侧下载与新页面观察均未做。
