# MEMORY

## 身份与当前状态
- RimWorld 1.6 模组 **鼠辈啁啾 / Squeaky Ratkin**；`packageId` `coahuilite.squeakyratkin`；namespace `SqueakyRatkin`；Defs 前缀 `SR_`。
- 产品版本 **0.3.3**：**GitHub 首发当日因 R6 Remix 缺陷撤回，同日同版本重发（`v0.3.3` → main `cd90a9e`，资产下载级核验；Claim Pack `docs/release_review/release-0.3.3-review-zh.md`）**；Steam **未上传**（暂存包 `dist/steam/SqueakyRatkin` 就绪，`commit=cd90a9e`）；上一个 Workshop 上线版本是 0.3.0。
- 分支模型（2026-09-19 维护者裁定，已落地）：**`dev` 已删除（本地 + 远端）**；每个 minor 在自己的分支开发（本次 `0.3.x`），发布时在该分支提交并 merge 到 `main`（受保护，走 PR）；`archive/` 分支只在最终版本建立。main 当前 `7190d23`（发布 tag `v0.3.3` → `cd90a9e`；`0.3.x` 同步于 `92babed`）。
- 当前开放动作、阻塞与待裁决见 `TODO.md`；现状、证据边界与裁决入口见 `docs/maintenance-status-zh.md`。

## 权威入口
- 维护状态与现行 docs 清单：`docs/maintenance-status-zh.md`（无 codemap；结构以源码为准）。
- 合同：架构 `docs/project-architecture-contract.md`、设置 `docs/settings-ui-product-contract-zh.md`、日志 `docs/logging-protocol.md`；流程 `docs/release-runbook-zh.md`；证据与终审报告 `docs/release_review/`。
- 作者正本：`.github/skills/squeaky-voicepack-authoring/SKILL.md`（配套 `scripts/new-voicepack.ps1`、`scripts/verify-voicepack-xml-abi.ps1`）。
- 历史（0.2.x–0.3.2 实施、流程裁决、文档收敛、Kiiro 线）已压缩进 `OBLIVIONIS.md`。

## 耐久约束（不得回退）
- 路由中立：VoicePack 的精确 `raceDefName` 是唯一路由入口，无种族特判；HAR 仅反射增强，缺失静默降级，不得崩溃。
- 卸载安全：不向存档写永久状态，设置/profile 只在 Config；1.6 的 comp 不进存档（`ThingWithComps` 每次按当前 def 重建）。
- 日志 `srdiag`：v1 28 事件字节冻结 + v2 4 事件；任何协议扩展必须同步 `tools/SqueakLogCharacterization`。
- 作者 XML ABI 公开稳定：字段只增不改、17 动作键 append-only、fail-closed；内部 kernel/schema 仍是 0.x 窗口。
- Eat 默认 job 级派发（招牌手感）不得默认收窄；两级开关语义、营养/toil 判定与回落规则见架构合同。
- 发布三命令契约：`verify-local.ps1`（11 项）→ `check-pack-readiness.ps1 -RequireReleaseMetadata` → `privacy-audit.ps1 -FullHistory -PrePush`；不新增人工仪式。
- US 兼容（2026-09-19 维护者裁定）：共存期 SR 与 US **各自装配、各自发声（双 comp / 双响）是正常行为**，玩家另装 SR legacy 独立版时同样正常；**不再要求 US 侧检测让位，也不存在 U1 顺序硬门**。仍需裁决的是切换面：类型名所有权（两 DLL 定义 `SqueakyRatkin.SqueakVoicePackDef` 的 first-wins 风险）、packageId 归属、legacy 渠道、US 前置硬/软、设置迁移、公告窗口。清单见 `docs/maintenance-status-zh.md`「迁移与退役」。
- 隐私：历史 known-debt 维持 A（不重写 + 台账纪律）；重写方案 `docs/privacy-history-rewrite-plan-zh.md`，执行需单独授权。

## 指针
- 冷归档（历史冲突或明确请求时才读）：`OBLIVIONIS.md`。
- 每个已发布版本的 Claim Pack 与权威观察记录在 `docs/release_review/`；它们只证明当时渠道状态，不证明当前版本。
