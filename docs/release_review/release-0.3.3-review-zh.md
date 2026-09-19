# Release 0.3.3 观察记录

观察日期/时区：2026-09-19（UTC+8）。本次批准渠道：**GitHub 正式 Release（首发撤回后同版本重发）**；Steam 未上传（由维护者人工执行）。证据范围：本地三命令门禁 + GitHub CI/资产（下载级核验）+ Steam 暂存包；Workshop 页面本次未观察（线上仍是 0.3.0 口径）。

## 撤回与重发

首发 `v0.3.3`（tag 指向 `c7fb868`，Release 09:28–09:30 UTC 创建/发布，资产 1,590,750 B、SHA256 `b7eb7dac…`、downloadCount 1）在发布当日被撤回：R6 Remix 折叠缺陷经维护者业务判定成立——三层路径按原始位序取 index，缺层时该次事件**静默**、内置层永不入选；默认 Fallback 不受影响，但**开启 Remix 的默认装配**（出厂播种的 `SR_OfficialExample_Race` 声明空 fallbacks）命中 shape A。撤回动作 = 删除 Release 与 annotated tag（`gh release delete v0.3.3 --yes --cleanup-tag`），删除后仓库 Latest 一度回退到 `v0.3.0`。重发 = main `cd90a9e`（PR #32 squash）+ 新 tag `v0.3.3`；**版本号不变**，属同版本修正重发而非新版本。

| 项 | 证据 |
| --- | --- |
| 版本、tag、源码提交 | `0.3.3`；annotated tag `v0.3.3`（重发后指向 `cd90a9e`）；main `cd90a9e`（PR #32 squash，tree 与 `0.3.x` `988e850` 相同） |
| 撤回对象（已删除） | 首发 tag 指向 `c7fb868`；资产 `SqueakyRatkin-v0.3.3.zip` = 1,590,750 B，SHA256 `b7eb7dac8ddc72a91e814f40d69904d7890ad2c1ade266218c3e4f6e2007a9f9`，downloadCount 1 |
| 构建 | 三渠道各 116 文件；DLL FileVersion `0.3.3.0`；Steam 暂存 `dist/steam/SqueakyRatkin` version.txt = `SqueakyRatkin 0.3.3` / `build=steam` / `commit=cd90a9e`；GitHub 资产内 version.txt = `build=github` / `commit=cd90a9e` |
| CI/资产 | PR CI `35436829771` success；main push CI `35436831751` success；Release CI `35436845918` success；资产 `SqueakyRatkin-v0.3.3.zip` = 1,590,744 B，SHA256 `fced520ac755bd4438f00434e8d5430279ab6f022a5b5382bacfdf326decb123`（下载后实测，与 API digest 一致；zip 134 entries = 116 文件 + 目录项） |
| 本地验证 | `verify-local.ps1 -NoRestore` 11/11（作用于 `56b40e1`）；`check-pack-readiness.ps1 -RequireReleaseMetadata` all checks passed；`privacy-audit.ps1 -FullHistory -PrePush` CLEAN（244 revisions，未接受命中 0，5 条 `[known-debt]` 接受） |
| GitHub | **完整**：重发 Release `v0.3.3` 已发布（非 prerelease、非 draft）+ 单资产 |
| Workshop | **unverified**：本次未上传；暂存包已就绪（`commit=cd90a9e`）；解除阻断时按 runbook 阶段 3 由维护者人工上传并做页面只读核验 |
| 流程变更 | 分支模型落地：`dev` 分支删除（本地 + 远端）；本版本在 `0.3.x` 开发并 merge 到 `main`；后续每个 minor 各自分支 |

## R6 修正与守卫

- 修正：`SqueakPoolRegistry.SelectRemixThree` 只在非 None 层上等权折叠（三层与四层同规则，恢复 0.2.4 语义）。
- 断言（`tools/KernelCharacterization` 新增 shape A/B，各 200 种子）：shape A 修正前 `none=105/200`、内置 `0/200`；修正后 `none=0`、`race=105 / builtin=95`。shape B 修正前 `none=115/200`、内置 `0/200`；修正后 `none=0`、`xeno=85 / builtin=115`。
- 语料：`corpus-0.3.1.txt` 按修正语义重建（10406 例）；新增 `corpus-0.3.0-r6.txt`（3783 例）作为 0.3.0 矩阵修正后基线；历史 `corpus-0.3.0.txt` 保留为修正前证据，harness 断言两者差异**为且仅为** 540 条 Remix 行（非 Remix 差异 0、前缀漂移 0）。
- 未做：游戏内 Remix 听感验证。本记录只证明内核行为与渠道状态，不证明实机听感。

内容摘要：0.3.1/0.3.2 工作并入本版（race 声明路由、年龄变体、fallback/彩蛋、玩家触发身份门控、XML ABI 固化、日志重排），新增 Eat 两级粒度开关（默认行为不变），并含 R6 Remix 修正；文档收敛为 10 份现行文档并把发布证据移入 `docs/release_review/`，记忆压缩进 `OBLIVIONIS.md`。

限制与未决：R4/R5 日志 `pawn=<label>` 与旧隐私禁令冲突未裁决；R3 作者 ABI 起点未精确到 tag；历史 `[known-debt]` 5 文件未重写；Steam 上传/页面核验待维护者；R6 实机听感未验。
