# 发布流程

> 唯一流程入口，2026-09-19 重构，自足可执行。当前进度见 [维护状态](maintenance-status-zh.md)。本地 commit 可执行；push/PR/merge/tag/Release/Workshop 与历史重写须有对应的维护者授权。本文不是授权。

## 三命令契约

| 用途 | 命令 | 覆盖 |
| --- | --- | --- |
| 日常开发 | `pwsh scripts/verify-local.ps1` | 既有 11 项 harness、fixture、构建、ABI/脚手架检查 |
| 发布面 | `pwsh scripts/check-pack-readiness.ps1 -RequireReleaseMetadata` | 默认组合 verify-local；版本轴、仓库红线、暂存包、DLL 身份及 `[claim]` 快照 |
| 每次 push 前 | `pwsh scripts/privacy-audit.ps1 -FullHistory -PrePush` | 跟踪树/提交信息/历史 blob、身份、干净树、main、待推送提交与 tag 集合 |

已有还原缓存、离线构建时前两命令加 `-NoRestore`；浮动版本 `1.6.*` 的隐式 restore 可能失败并污染 assets 文件。CI 显式 restore 后使用 `-NoRestore`。不要在 release 检查已组合 verify-local 后再机械重复；CI 已单独验证时沿用既有 `-SkipVerify` 接线。

`stage-package.ps1` 写时断言与 readiness 读时复核共同负责版本一致、包排除、version.txt、DLL 和音频镜像。**不再手工逐项复核包内容，不新增人工检查仪式。** 自动检查能证明什么以脚本为准，不能替代发布裁决或外部渠道观察。

## 发布顺序

1. **准备具体候选**。版本主源为 csproj `<Version>`，About `<modVersion>` 跟随；同步双语 CHANGELOG 与必要文案，选定待发布提交。开发条目用 Unreleased；实际发布后才以 UTC+8 时间替换。文案有时间不等于渠道已上线。若需试玩，用 `build-dev.ps1` 产物；dirty 包仅作测试证据。
2. **最小发布仪式**：发布面命令全绿 → 完整隐私命令全绿 → 维护者裁决版本、渠道、是否发及 Workshop 文案。非发布 push 仍必须通过完整隐私门并有授权。每次新 push 覆盖其完整可达范围，不因旧结果为绿而免检。
3. **版本分支 → main（2026-09-19 分支模型裁定）**。每个 minor 在**自己的分支**开发（本次 `0.3.x`，从 `main` 建立）；**不再有 `dev` 分支**。需要发布时在该版本分支提交，授权后 push → CI → PR/merge 到 `main`；受保护 main 用 PR，源分支必须是版本分支。合并后核验 `git diff --stat <版本分支> main` 为 0 行并抽查关键行为文件（`LoadFolders.xml`/csproj/`About.xml`）；当前分支是否已同步不能由版本号推定。
4. **GitHub**。在 main 的批准发布提交上，授权后 tag/push，release CI 验证、构建 GitHub flavor 并发布资产。基版本来自 csproj；正式 `vX.Y.Z`，prerelease 带 SemVer 后缀。已接受的 hotfix 命名为 `vX.Y.Z-hotfixN`，须注意它具有 prerelease 语义。资产身份/散列取 CI 和 `[claim]`；有疑问才以 `pack-github.ps1` 复现对照。tag 重发另需授权及更新实际发布时间。
5. **Steam（若本次获准）**。`build-steam.ps1` 构建 Steam flavor 并在干净树打包，readiness 复核 `dist/steam/SqueakyRatkin`。维护者复制到本地上传副本，既有 item ID 仅写该副本的 `About/PublishedFileId.txt`；同作者 Update，不使用 SteamCMD，不把后续更新称为 Initial Workshop Upload。正文从 [双语草稿](steam-workshop-page-copy-draft.md) 取用，由维护者在编辑器与实际页面预览。
6. **独立渠道观察与收尾**。agent 仅以公开 filedetails 玩家视角核验同一 item、描述版本、Updated（记录时区）、visibility、preview、文件大小及 change notes；不进入登录/编辑面。Comments 中有实质疑点时进入 TODO，不增加逐项签字仪式。记录下方 Claim Pack，更新维护状态和必要记忆指针。`archive/` 分支只在最终版本定稿建立。

## 隐私与证据边界

- 工作树、提交信息、历史 blob、身份分别得出结论；CI 默认模式不等于 push 前 FullHistory。作者/提交者使用 GitHub noreply；具体校验规则以脚本为准。
- `[known-debt]` 不是已消除：历史台账是 5 个文件的既有路径债务，当前方案 A 维持、B 待独立授权。新增台账条目须维护者裁决，不通过改文档或掩盖命中绕门。真实凭据泄漏先撤销/轮换，再处理历史。
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
