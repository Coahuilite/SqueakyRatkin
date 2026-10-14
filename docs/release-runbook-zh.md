# 发布流程

> 唯一流程入口，2026-10-14 更新。当前进度见 [维护状态](maintenance-status-zh.md)。本地 commit 可执行；push/PR/merge/tag/Release/Workshop 与历史重写须有对应的维护者授权。本文不是授权。

## 三命令契约

| 用途 | 命令 | 覆盖 |
| --- | --- | --- |
| 日常开发 | `pwsh scripts/verify-local.ps1` | 12 项 harness、fixture、构建、ABI/脚手架与合成发布门回归检查 |
| 发布面 | `pwsh scripts/check-pack-readiness.ps1 -RequireReleaseMetadata -StageFlavor steam`（或 `github` / `dev`） | 默认组合 verify-local；指定渠道必须存在且匹配当前版本、DLL 与干净提交，输出 `[claim]` 快照 |
| 每次 push 前 | `pwsh scripts/privacy-audit.ps1 -FullHistory -PrePush` | 跟踪树/提交信息/历史 blob、身份、干净树、main、待推送提交与 tag 集合 |

已有还原缓存、离线构建时前两命令加 `-NoRestore`；浮动版本 `1.6.*` 的隐式 restore 可能失败并污染 assets 文件。CI 显式 restore 后使用 `-NoRestore`。不要在 release 检查已组合 verify-local 后再机械重复；CI 已单独验证时沿用既有 `-SkipVerify` 接线。

每个新 clone 执行一次 `pwsh scripts/install-hooks.ps1`，自动 pre-push hook 运行 `-FullHistory`；已有自定义 hooksPath 时先协调，不覆盖。CI 与 Release CI 使用完整历史 checkout 并运行 `-FullHistory`。发布前仍单独加 `-PrePush` 检查干净树、main 与 tag 集合。

`stage-package.ps1` 写时断言与 readiness 读时复核共同负责版本一致、包排除、version.txt、DLL 和音频镜像。**不再手工逐项复核包内容，不新增人工检查仪式。** 自动检查能证明什么以脚本为准，不能替代发布裁决或外部渠道观察。

## 发布顺序

1. **准备具体候选**。版本主源为 csproj `<Version>`，About `<modVersion>` 跟随；同步双语 CHANGELOG 与必要文案，校验并正式提交，再按渠道构建、打包。旧 `dist` 输出只是缓存，不能代替当前候选；发布检查必须指定 `-StageFlavor`。开发条目用 Unreleased；实际发布后才以 UTC+8 时间替换。文案有时间不等于渠道已上线。若需试玩，用 `build-dev.ps1` 产物；dirty 包仅作测试证据。
2. **最小发布仪式**：发布面命令全绿 → 完整隐私命令全绿 → 维护者裁决版本、渠道、是否发及 Workshop 文案。非发布 push 仍必须通过完整隐私门并有授权。每次新 push 覆盖其完整可达范围，不因旧结果为绿而免检。
3. **版本分支 → main**。每个 minor 从 `main` 建立自己的开发分支，例如 `0.3.x`；不使用已删除的 `dev` 分支。需要发布时在版本分支提交，授权后 push → CI → PR/merge 到受保护的 `main`。发布证据记录实际源码提交与 tag，不以版本号推定分支同步，也不重复脚本已覆盖的逐文件核对。
4. **GitHub**。在 main 的批准发布提交上，授权后 tag/push，release CI 验证、构建 GitHub flavor 并发布资产；GitHub Release notes 自动从 PR 信息生成，CHANGELOG 不接入该生成流程。基版本来自 csproj；正式 `vX.Y.Z`，prerelease 带 SemVer 后缀。已接受的 hotfix 命名为 `vX.Y.Z-hotfixN`，须注意它具有 prerelease 语义。资产身份/散列取 CI 和 `[claim]`；有疑问才以 `pack-github.ps1` 复现对照。tag 重发另需授权及更新实际发布时间。
5. **Steam（若本次获准）**。`build-steam.ps1` 构建 Steam flavor 并在干净树打包，readiness 复核 `dist/steam/SqueakyRatkin`。上传既有条目可用同作者的游戏内 Update，或经明确授权的 SteamCMD；不把后续更新称为 Initial Workshop Upload。游戏内上传时，维护者复制到上传副本，既有 item ID 仅写该副本的 `About/PublishedFileId.txt`。SteamCMD 上传时，VDF 与登录辅助文件放在仓库、发布包之外；VDF 必须指定正确的 `appid` 与既有非零 `publishedfileid`，`contentfolder` 指向已验收的 Steam 候选。登录与 Steam Guard 由维护者在工具窗口直接完成，密码、验证码不进入聊天、脚本或仓库。工具安装与启动自检本身不构成登录或发布授权。

   SteamCMD 仅更新 VDF 列出的字段；文件上传不会自动同步页面正文或游戏版本标签。正文从 [页面草稿](steam-workshop-page-copy-draft.md) 取用，仍须维护者审阅；Change Notes 从 [英文 CHANGELOG](CHANGELOG.md) / [中文 CHANGELOG](CHANGELOG.zh-CN.md) 的对应版本条目取用。核对实际上传日志与页面，不能只凭工具进程退出码宣布成功。参数依据为 [Steamworks 的 SteamCmd Integration](https://partner.steamgames.com/doc/features/workshop/implementation#SteamCmd_Integration)。

   **版本标签**由 RimWorld 游戏上传器从 `About.xml` 的 `supportedVersions` 生成，通过 `SteamUGC.SetItemTags` 提交，不能假定网页编辑页提供手动入口。需要同步时，用对应候选的上传副本在游戏内 Update 既有条目；该操作同时更新标题、预览和内容，既有条目的页面正文不会由游戏上传器重写。标签仅影响工坊筛选，不替代包内版本支持声明。实现依据为 RimWorld `Workshop.SetWorkshopItemDataFrom`，机制说明见 [Steam Workshop Item Tags](https://partner.steamgames.com/doc/features/workshop/tags?l=english)。
6. **独立渠道观察与收尾**。agent 仅以公开 filedetails 玩家视角核验同一 item、描述版本、Updated（记录时区）、visibility、preview、文件大小及 change notes；不进入登录/编辑面。Comments 中有实质疑点时进入 TODO，不增加逐项签字仪式。记录下方 Claim Pack，更新维护状态和必要记忆指针。`archive/` 分支只在最终版本定稿建立。

## 隐私与证据边界

- RimWorld 1.6 / 1.7 共用 `1.6/` 内容，支持声明与加载映射须随包保留。通常按实际候选包验收；维护者可明确接受版本号维护沿用旧运行证据。0.3.4 已获此裁定，记录接受依据而不改写旧证据的版本号，也不声称新增游戏运行。

- 工作树、提交/tag 信息、历史文本、历史二进制元数据、身份分别得出结论。PNG 读取全部文本/EXIF 块并解压文本，OGG/WAV 读取元数据而非压缩音频；图片视觉内容仍由人工检查。作者/提交者使用 GitHub noreply；具体校验规则以脚本为准。
- `[known-debt]` 不是已消除：历史例外位于 `scripts/privacy-history-exceptions.json`，只匹配精确 blob/path/pattern，当前文件不豁免。当前方案 A 维持、B 待独立授权。新增台账条目须维护者裁决，不通过改文档或掩盖命中绕门。真实凭据泄漏先撤销/轮换，再处理历史。
- 默认树扫描只覆盖 git 跟踪文件，其规则也不是任意日志/隐私文本识别器。写作时就避免个人路径、原始日志、凭据、item ID 值；不得把扫描通过扩张为“所有数据都安全”。
- 页面观察只支持页面级结论，不证明玩家下载二进制；一个渠道不证明另一个渠道。无法观察的字段记 unverified，旧 Claim Pack 不证明新版本已发布。

## Claim Pack 最小记录

版本事实完成后存入 `docs/release_review/release-<version>-review-zh.md`，当前状态只保留链接。prerelease 用 SemVer 后缀命名（如 `release-0.3.2-pre1-review-zh.md`）；保留原证据，不为了统一版式回改旧评审。

```text
# Release <version> 观察记录
观察日期/时区；本次批准的渠道；证据范围。

| 项 | 证据 |
| --- | --- |
| 版本、tag、源码提交 | 精确值及 main 来源 |
| 构建 | flavor、DLL FileVersion/InformationalVersion、version.txt 身份 |
| CI/资产 | run、结果、资产名/字节数/SHA256；引用 [claim] 和 CI 输出 |
| 本地验证 | 实际运行命令、时间、结果；known-debt 单列 |
| GitHub | 完整 / 待补，观察依据 |
| Workshop | unverified / 页面级已核验 / 完整，观察依据 |
| 限制与未决 | 未做的二进制验证、阻断及待处理项 |
```

表格是证据归档格式，不是新增发布门；不粘贴个人日志、本机绝对路径或 item ID 值。
