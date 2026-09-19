# Squeaky Ratkin 0.2.4 发布核验（Release Claim Pack）

> 依据 `docs/release-runbook-zh.md` 阶段 2-3 流程记录。版本 0.2.4 为**修复 + 日志增补**：婴幼儿 Giggling/Crying fits 不再被误报为精神崩溃语音（hook 收窄到游戏正式崩溃入口）；AudioDispatchOk 日志追加 pawn 名称与 id。

## GitHub Release Claim Pack

| 项 | 值 |
|---|---|
| 版本 | 0.2.4 |
| 标签 | v0.2.4（严格 SemVer，基版本 = csproj `<Version>` 0.2.4） |
| 源码提交 | `53686f9222dce06dda1a2cd705e4ca368d635961`（main squash `fix: narrow mental-break hook to real breaks (0.2.4) (#15)`） |
| 发布时间 | 2026-08-18 00:10 UTC+8（16:10 UTC） |
| 构建 flavor | GitHub（CI tag 触发） |
| DLL 身份 | FileVersion `0.2.4.0`；Informational `v0.2.4+53686f9222dc` |
| CI | Release workflow run `32044477840` success |
| 资产 | `SqueakyRatkin-v0.2.4.zip`（1,574,386 B） |
| zip SHA256 | `cfb4f5cbad3b868ca90755ee46835569ebbc6ce62b03ab221edb0214344fbfec` |
| 包内容 | 115 文件；0 PDB；0 PublishedFileId.txt；0 codemap.md；`modVersion` 0.2.4；OGG 41（Template 镜像 SHA256 校验） |
| 隐私审计 | 完整树扫描 0 命中；dev↔main 树一致（changelog 冲突已解决并合并） |

## Steam staging（0.2.4）

| 项 | 值 |
|---|---|
| 构建 flavor | Steam（本地 `pack-steam.ps1`） |
| staging 包 | `dist/steam/SqueakyRatkin`：115 文件；0 PDB；0 PublishedFileId.txt；0 codemap.md；OGG 41 镜像校验 |
| 上传 | 维护者已报告上传成功（2026-08-18）；页面观察待补 |

## 渠道状态

- GitHub：完整。
- Workshop：维护者已上传（0.2.4）；页面级核验完成（2026-08-18）：同一 item URL/ID `3758115669`、标题 "Squeaky Ratkin"、File Size 1.872 MB、Posted 5 Jul、Updated 17 Aug 09:22、6 Change Notes、visibility public、标签 Mod/1.6；中英文描述均同步最新草稿（默认开启 + 0.2.4 婴幼儿 fits 说明 + 0.3.0 失效公告 + 版本 0.2.4 + 至高鼠王俏皮句 + v0.2.4 下载链接）。二进制下载级验证边界：页面核验 ≠ 玩家下载内容已验证。

## 验证记录

- 实机（dev 0.2.4 本地包，GABP/日志通道）：婴儿 Giggling fits（Pinenut 37212、Rainlin 37215）零 MentalBreak 音；儿童真崩溃（Wildtail 37127）崩溃音仍响应；日志增补 `pawn=<名> pawn_id=Ratkin<id>` 全程可读。三项全过。
- 根因（rimsage 源码确认）：Biotech BabyFits（`MentalStates_BabyFits.xml`）是 `MentalFitDef` 驱动的 mental state（Giggling/Crying）；原 hook `MentalStateHandler.TryStartMentalState` 是所有 mental state 的通用入口 → 误报。修复 hook 收窄至 `MentalBreakWorker.TryStart`（`MentalBreakDef` 驱动的正式崩溃唯一通道）；婴儿本无崩溃（`LifeStages.xml` `canDoRandomMentalBreaks=false`，官方注释 "Babies have crying/giggling fits instead of mental breaks"）。
- 日志协议：`SqueakLogData` 追加 `PawnName`/`PawnId`（srdiag 尾部 `pawn=`/`pawn_id=`，`Add` 对 null 跳过，其余 27 事件输出不变）；`SqueakLogCharacterization` 同步并通过。

## 自我批评（流程教训，本版记录）

1. **本地测试包流程**：首轮只替换单 DLL 未走脚本整包；且 0.2.4 开发态测试误用 Steam flavor（`pack-steam`）而非 Dev flavor（`pack-dev`），跳过了 dev 门控流程。教训：**开发态测试 = Dev flavor 构建 + `pack-dev.ps1` 完整包替换本地安装；发布态才走 `pack-steam`；任何测试部署必须用脚本产物，禁止手搓**。
2. **分支纪律**：0.2.4 开发提交曾误落在 main 分支（0.2.3 收尾后停留在 main），经 cherry-pick 迁移到 dev 并还原 main。教训：**开发与发布收尾提交一律在 dev 分支进行，main 只接受 PR squash merge**（已由分支保护强制执行）。
3. **PR 合并冲突**：changelog 双语在 PR #15 出现三方合并冲突（0.2.3 时间标题行两侧同改）。教训：**发布收尾后 dev 应尽快以 `merge -s ours`/对账吸收 main 指针**，避免下次 PR 在文档区重复三方冲突（0.2.1 已有一致教训，本次复发）。

## 备注

- 0.2.4 流程链：dev push CI `32044181594` → PR #15（changelog 冲突解决后）CI `32044344615`/`32044346462` → main CI `32044414901` → Release CI `32044477840` 全 success。
- 0.3.x 规划输入随本版提交：Crying/Giggling 动作兼容（Baby 年龄域动作，`docs/internal-universalization-design-note-zh.md`）。
