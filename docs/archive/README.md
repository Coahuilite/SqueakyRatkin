# 归档索引

2026-09-19：先完成 [独立终审](consolidation/final/handoff-package-final.md)，再据其结论收敛 SR 文档。现行入口为 [docs/codemap.md](../codemap.md)。本目录保留旧文件字节，不再作为当前状态或操作指令；旧提示词中的删除/发布/提交步骤没有在本轮执行。

旧文件的历史路径 `docs/<path>` 映射为 `docs/archive/<path>`，层级原样保留；双语 CHANGELOG 例外，仍在 docs 根且字节不变。旧文档内部的仓库根路径/相对引用没有批量改写，须按上述映射及写作时点阅读，不能把旧链接当作现行导航。

## 内容去向

| 原文（本目录内） | 当前承载/保留原因 |
| --- | --- |
| [project-architecture-contract.md](project-architecture-contract.md) | [重建架构合同](../project-architecture-contract.md)；保留旧规范以核查遗漏与矛盾 |
| [settings-ui-product-contract-zh.md](settings-ui-product-contract-zh.md) | [重建 UI 合同](../settings-ui-product-contract-zh.md) |
| [logging-protocol.md](logging-protocol.md) | [重建日志协议](../logging-protocol.md)，v1 字段与事件分组已纠正 |
| [release-runbook-zh.md](release-runbook-zh.md) | [重建 runbook](../release-runbook-zh.md)，统一发布源与三命令职责 |
| [steam-workshop-page-copy-draft.md](steam-workshop-page-copy-draft.md) | [0.3.3 待发布草稿](../steam-workshop-page-copy-draft.md)，旧页源不是当前外部状态 |
| [0.3x-refactor-architecture-decision-zh.md](0.3x-refactor-architecture-decision-zh.md) | 纯内核/路由/ABI 原则入架构；方案比拼、阶段表、反转留历史 |
| [0.3x-equivalence-review-zh.md](0.3x-equivalence-review-zh.md) | 历史等价审查；终审区分语料零 delta 与语义正确 |
| [0.3x-release-gate-checklist-zh.md](0.3x-release-gate-checklist-zh.md) | 历史 A–H、C 链与验证强度，不作为新版本绿灯 |
| [handoff-0.3.0-zh.md](handoff-0.3.0-zh.md) | 旧交接时间层 |
| [handoff-0.3.3-zh.md](handoff-0.3.3-zh.md) | [维护状态](../maintenance-status-zh.md) 取代日常入口；保留追加写作层 |
| [handoff-eat-occurrence-granularity-zh.md](handoff-eat-occurrence-granularity-zh.md) | 三态/回落/默认入架构与 UI；vanilla 分析、备选及完整实机矩阵留此 |
| [internal-universalization-design-note-zh.md](internal-universalization-design-note-zh.md) | 规划依据；六门及未验证状态进入维护状态，不假定 US 已完成 |
| [us-sr-compatibility-check-zh.md](us-sr-compatibility-check-zh.md) | 旧快照 E/F/S/U 清单；兼容顺序和待裁决入维护状态 |
| [us-sr-migration-plan-zh.md](us-sr-migration-plan-zh.md) | 身份/设置/桥/渠道备选；“零损失”推论按终审 R7 限定 |
| [process-review-zh.md](release_review/process-review-zh.md) | 事故证据保留；有效纪律入 runbook，教训的证据边界在终审 R9 |
| [process-redundancy-review-zh.md](release_review/process-redundancy-review-zh.md) | V1/V2/V3 与流程取舍；不再次引入人工仪式 |
| [privacy-history-rewrite-plan-zh.md](release_review/privacy-history-rewrite-plan-zh.md) | 仍待独立授权的方案；当前状态入维护入口，旧计数不是执行依据 |

## 发布证据

以下七份原文完整保留。页面证据不等于下载二进制验证；更晚版本不得直接沿用为已发布证明。

| Claim Pack | 记录性质 |
| --- | --- |
| [0.2.0](release_review/release-0.2.0-review-zh.md) | 发布证据与复盘 |
| [0.2.1](release_review/release-0.2.1-review-zh.md) | 作废/重发后的记录 |
| [0.2.2](release_review/release-0.2.2-review-zh.md) | Workshop 为维护者报告，观察缺口保留 |
| [0.2.3](release_review/release-0.2.3-review-zh.md) | GitHub 与 Steam 分账 |
| [0.2.4](release_review/release-0.2.4-review-zh.md) | 发布证据与流程自评 |
| [0.3.0](release_review/release-0.3.0-review-zh.md) | GitHub 完整与 Workshop 页面级证据 |
| [0.3.2-pre1](release_review/release-0.3.2-pre1-review-zh.md) | GitHub prerelease，Steam 阻断 |

## 收敛审查材料

- [终审报告](consolidation/final/handoff-package-final.md)：R1–R10 独立判断、回查、完整 CNF/OQ/GAP 原陈述及编号。
- [v2](consolidation/final/handoff-package-v2.md)：原第二轮结果，原样保留；不是已核实的当前状态。
- [覆盖记录](consolidation/coverage-check.md)、`consolidation/packages/`、`consolidation/final/v2-parts/`：上游原包、compact、提取片段及当时账本，包括非交付 r2 样张。原有 `packages/` gitignore 仍生效；文件存在不代表已进入 Git。
- [原文件指纹](consolidation/final/source-manifest.json)：整理前 56 个文件的原路径、字节数与 SHA-256（包含两份未移动的 CHANGELOG，不含本轮新终审/索引）。用于证明归档没有改写原材料，不是新增运行脚本或发布门。

本目录保留是用户本轮明确要求；旧 `TASK-docs-consolidation-zh.md` 的“任务结束删除 archive”不适用。原档中未完成的疑点仍可追溯，但只有 [维护状态](../maintenance-status-zh.md) 与现行 TODO 承担当前行动入口。
