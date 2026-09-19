# MEMORY

## 当前状态
- 产品版本 **0.3.3**：GitHub 首发因 R6 当日撤回、同版本重发（tag `v0.3.3` → `cd90a9e`），实机验收通过（release 包 + 维护者听感；特例跳过 dev 包前置测试）。证据 `docs/release_review/release-0.3.3-review-zh.md`。
- 渠道：**GitHub Release 与 Steam Workshop 均已上线 0.3.3**（Steam 由维护者 2026-09-19 上传，页面只读核验 `Mod version: 0.3.3`；页面未含中文描述块与 R6 条目，是否补录待定）；线上旧版（含 R6 缺陷的 0.3.0）已被替换。
- 分支模型：`dev` 已删除；每个 minor 在自己分支开发（本次 `0.3.x`）→ PR 入 `main`（受保护）。发布锚 = tag `v0.3.3` → `cd90a9e`（不追记移动的 main HEAD）。
- 身份与开放动作：`AGENTS.md`；`TODO.md`；现状/证据边界/裁决入口 `docs/maintenance-status-zh.md`。

## 权威入口
- 合同 3 份（架构/设置 UI/日志）+ 流程 `docs/release-runbook-zh.md` + 证据 `docs/release_review/`；无 codemap，结构以源码为准。作者正本 `.github/skills/squeaky-voicepack-authoring/SKILL.md`。

## 耐久约束（不得回退）
- 路由中立：VoicePack 的精确 `raceDefName` 是唯一路由入口，无种族特判；HAR 仅反射增强，缺失静默降级，不崩溃。
- 卸载安全：不向存档写永久状态，设置/profile 只在 Config；1.6 的 comp 不进存档（按当前 def 重建）。
- 日志 `srdiag`：v1 28 事件字节冻结 + v2 4 事件；协议扩展须同步 `tools/SqueakLogCharacterization`。
- 作者 XML ABI 公开稳定：字段只增不改、17 动作键 append-only、fail-closed；内部 kernel/schema 仍是 0.x 窗口。
- Eat 默认 job 级派发（招牌手感）不得默认收窄；两级开关语义见架构合同。
- 发布三命令契约：`verify-local.ps1`（11 项）→ `check-pack-readiness.ps1 -RequireReleaseMetadata` → `privacy-audit.ps1 -FullHistory -PrePush`；不新增人工仪式。
- SR 1.0 形态（2026-09-19 裁定）：对 US **硬依赖**（FL 传递，SR 不单独声明）；Def 归属按 US 约定；设置**不自动导入**，只提醒玩家自留后在 US 重选；Steam 3A 声明下方加**无具体时间**退役公告，US 未上工坊前**不随 0.3.3 上传**。共存期双装配双响**正常**（无 U1 硬门）；开放项（packageId 归属、legacy 渠道、过渡版、切换验收）见维护状态。
- 隐私：历史 known-debt 维持 A（不重写 + 台账纪律）；重写方案 `docs/privacy-history-rewrite-plan-zh.md`，执行需单独授权；日志与个人路径不入库。

## 指针
- 冷归档（仅历史冲突或明确请求时读）：`OBLIVIONIS.md`。Claim Pack 只证明当时渠道状态，不证明当前版本。
