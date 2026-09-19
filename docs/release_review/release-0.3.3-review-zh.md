# Release 0.3.3 观察记录

观察日期/时区：2026-09-19（UTC+8）。本次批准渠道：**GitHub 正式 Release**；Steam 未上传（阻断解除后由维护者人工执行）。证据范围：本地三命令门禁 + GitHub CI/资产 + Steam 暂存包；Workshop 页面本次未观察（线上仍是 0.3.0 口径）。

| 项 | 证据 |
| --- | --- |
| 版本、tag、源码提交 | `0.3.3`；annotated tag `v0.3.3`；main `c7fb868`（PR #26 squash，tree 与 `0.3.x` `c19c548` 相同） |
| 构建 | 三渠道各 116 文件；DLL FileVersion `0.3.3.0`、ProductVersion `0.3.3+c19c548…`；GitHub 资产由 Release CI 构建；Steam 暂存 `dist/steam/SqueakyRatkin` version.txt = `SqueakyRatkin 0.3.3` / `build=steam` / `commit=c7fb868` |
| CI/资产 | PR CI `35434718274` success；main push CI `35434792274` success；Release CI `35434798559` success；资产 `SqueakyRatkin-v0.3.3.zip` = 1,590,750 B，SHA256 `b7eb7dac8ddc72a91e814f40d69904d7890ad2c1ade266218c3e4f6e2007a9f9`（与 API digest 一致；zip 134 entries = 116 文件 + 目录项） |
| 本地验证 | `verify-local.ps1 -NoRestore` 11/11；`check-pack-readiness.ps1 -RequireReleaseMetadata` all checks passed（`[claim]` version=0.3.3、localHead=`c19c548`）；`privacy-audit.ps1 -FullHistory -PrePush` CLEAN（223 revisions，未接受命中 0，5 条 `[known-debt]` 接受） |
| GitHub | **完整**：Release `v0.3.3` 已发布（非 prerelease）+ 单资产 |
| Workshop | **unverified**：本次未上传；暂存包已就绪；解除阻断时按 runbook 阶段 3 由维护者人工上传并做页面只读核验 |
| 流程变更 | 分支模型落地：`dev` 分支删除（本地 + 远端）；本版本在 `0.3.x` 开发并 merge 到 `main`；后续每个 minor 各自分支 |
| 限制与未决 | R6 Remix 空层问题未修（终审已复现）；R4/R5 日志 `pawn=<label>` 与旧隐私禁令冲突未裁决；R3 作者 ABI 起点未精确到 tag；历史 `[known-debt]` 5 文件未重写；Steam 上传/页面核验待维护者 |

内容摘要：0.3.1/0.3.2 工作并入本版（race 声明路由、年龄变体、fallback/彩蛋、玩家触发身份门控、XML ABI 固化、日志重排），新增 Eat 两级粒度开关（默认行为不变），文档收敛为 10 份现行文档并把发布证据移入 `docs/release_review/`，记忆压缩进 `OBLIVIONIS.md`。
