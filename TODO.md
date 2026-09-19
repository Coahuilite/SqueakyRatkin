# TODO

> 只剩发布相关内容；开放问题、待修复与待裁决统一在维护入口 [`docs/maintenance-status-zh.md`](docs/maintenance-status-zh.md)。已完成历史见 `OBLIVIONIS.md` 与 `docs/release_review/`。

## 发布
- 0.3.3 已重发到 GitHub（2026-09-19：首发因 R6 撤回，同版本修正后重发，`v0.3.3` → main `cd90a9e`，资产 SHA256 与 API digest 一致）；记录见 [`release-0.3.3-review-zh.md`](docs/release_review/release-0.3.3-review-zh.md)。分支模型已落地（`dev` 已删；本次 `0.3.x` → `main`）。
- [ ] **Steam 上传（待阻断解除）**：暂存包 `dist/steam/SqueakyRatkin` 已重建（116 文件，`build=steam`/`commit=cd90a9e`）。步骤 = 粘贴 [`steam-workshop-page-copy-draft.md`](docs/steam-workshop-page-copy-draft.md) 的中英正文与 Change Notes（含 R6 修复条目，**不含**退役公告）→ 维护者人工上传（同 item Update，非 SteamCMD）→ 页面只读核验 → 更新 Claim Pack 的 Workshop 渠道格。
- [ ] **退役公告（等 US 上架工坊）**：草稿已在 [`steam-workshop-page-copy-draft.md`](docs/steam-workshop-page-copy-draft.md)「即将退役公告」就位（中文 253 / 英文 667 字符，无具体时间，插在页面 3A 声明下方）；US 上线后由维护者单独更新主页公告，本次不随 0.3.3 上传。
- [ ] **下一版开工**：从 `main` 新建版本分支（`0.4`，或按路线直推 `1.0`）；`archive/` 分支只在最终版本建立。
- [ ] **0.3.3 发布后观察**：Steam/GitHub 评论与 issue 如有回归则进入修复循环（`vX.Y.Z-hotfixN`），否则按路线向 0.4 推进。
- [ ] **证据退役（延后）**：`docs/release_review/` 的 Claim Pack、流程复盘与终审报告留到下一次证据退役再评估删除或压缩。
