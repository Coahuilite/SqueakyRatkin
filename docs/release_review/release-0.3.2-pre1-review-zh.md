# Release 0.3.2-pre1 Prerelease 观察记录（Claim Pack）

> 依据 `docs/release-runbook-zh.md` 阶段 2 记录 GitHub prerelease 执行证据；Steam 发布按维护者指示阻断不执行。正式 0.3.2 发布时另行按 runbook 完整走通并落最终 Claim Pack。

## GitHub Prerelease Claim Pack

| 项 | 值 |
|---|---|
| 版本 | 0.3.2（prerelease `-pre1`） |
| 标签 | `v0.3.2-pre1`（严格 SemVer 2.0，基版本 = csproj `<Version>` 0.3.2） |
| 源码提交 | main squash `60f7d88893268528464ff8a326e3c90b0f716651`（PR #24） |
| 发布时间 | 2026-08-23 05:26:26Z（UTC+8 13:26） |
| 构建 flavor | GitHub（CI tag 触发） |
| DLL 身份 | FileVersion `0.3.2.0`；Informational `v0.3.2-pre1+60f7d8889326` |
| CI | Release workflow run `32620324890` success（tag `v0.3.2-pre1`） |
| 资产 | `SqueakyRatkin-v0.3.2-pre1.zip`（1,588,909 B） |
| 资产 SHA256 | `d8c785a7a027c37ef5abb4e2eed0f85848044e7e18cc9ab199a453243e9aeb4b`（与 GitHub API digest 一致） |
| 包内容 | 116 文件；0 PDB；0 PublishedFileId.txt；0 codemap.md；包内 `version.txt` = `SqueakyRatkin 0.3.2-pre1 / build=github / commit=60f7d88`；About `<modVersion>` = 0.3.2 |
| 隐私审计 | 0.3.x 推送完整可达范围 24 提交扫描 0 真实命中（PublishedFileId 命中均为文件名纪律文案）；main PR 范围 0 真实命中；0 dist/Assemblies/bin/obj 条目 |
| 分支状态 | `0.3.x` 已推送（C34–C40）；`dev` 已推送并 merge origin/main；main 经 PR #24 squash（tree == dev，diff 0 行） |

## 执行摘要

- 0.3.x 提交链：C34 身份门控 → C35 彩蛋/身份日志 → C36 作者指南/SKILL+脚手架+ABI 锁 → C37 legacy 桥原型 → C38 0.3.2 版本/changelog/docs → C39/C40 fixtures 行尾修复（corpus LF、expected/input 跟随宿主）。
- 关键发布修复：`stage-package.ps1` 支持 SemVer prerelease label（About/csproj 对比基版本）；`.gitattributes` 只对冻结 corpus 强制 LF，避免 SettingsFixtureGenerator 在 Windows CRLF checkout 下误报。
- Steam：**阻断不执行**（未构建/上传 Steam 包、未触碰 Workshop item）。
- 待办：正式 0.3.2 发布时替换 changelog 时间、执行 Steam 阶段 3 与完整收尾。
