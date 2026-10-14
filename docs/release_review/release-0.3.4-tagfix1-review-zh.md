# Release 0.3.4-tagfix1 预发布记录

> 状态：准备中。维护者已授权发布维护版本及由维护者通过 RimWorld 游戏内 Update 手工更新既有 Steam 工坊条目；本记录尚不代表版本已发布或工坊标签已更新。

## 范围与版本身份

本次只为更新 Steam 创意工坊版本标签，不含运行时或功能改动；RimWorld 1.6 / 1.7 支持不变。产品基础版本保持 0.3.4，About `<modVersion>` 使用 `0.3.4`，DLL `FileVersion` 使用 `0.3.4.0`；GitHub prerelease/tag 使用 `v0.3.4-tagfix1`，Steam 包 `version.txt` 标识 `0.3.4-tagfix1`，DLL `InformationalVersion` 包含该后缀。`0.3.4-tagfix1` 是合法 SemVer 2.0 prerelease 标识。

此次接受沿用既有游戏证据：RimWorld 1.6 下 Steam 0.3.4 与 RimWorld 1.7 下 Steam 0.3.3。没有进行 0.3.4-tagfix1 新实机测试；既有证据边界见 [0.3.4 记录](release-0.3.4-review-zh.md) 与 [维护状态](../maintenance-status-zh.md)。

## 渠道与待补证据

此前 SteamCMD 上传 0.3.4 未更新工坊版本标签；本次改由维护者在游戏内 Update 上传候选。维护者手工录入本版本双语 Change Notes。当前公开页面仍为 0.3.4，发布完成前不推定标签更新成功。GitHub Release notes 继续自动根据 PR 生成。

| 项 | 当前状态 |
| --- | --- |
| 版本分支 / PR | PR #42 为本次发布准备复用；最终标题、合并与对应提交待补 |
| About、csproj、DLL、Steam version.txt | 目标身份已约定；实际构建与包检查结果待补 |
| 本地验证与 CI | `verify-local.ps1 -NoRestore` 的 12 项检查通过（13 行总输出，退出码 0）；合成发布门 16 cases 通过。正式提交/stage/readiness 与 CI 待补 |
| GitHub tag / prerelease / 资产 | 未发布；完成后补 tag、源码提交、资产及 SHA256 |
| Steam 工坊更新 | 待维护者通过游戏内 Update 手工发布并自行验收版本标签；维护者手工录入 Change Notes。无需 agent 线上标签核验 |
| 覆盖边界 | 不新增游戏实测；Steam 玩家侧下载核验状态待记录 |

不把后缀变更或 About 版本字段本身视为 Steam 标签问题已解决。不得将本记录的准备项描述为已通过或已上线。
