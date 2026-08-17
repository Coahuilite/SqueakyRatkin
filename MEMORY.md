# MEMORY

## 当前耐久状态
- 项目为 RimWorld 1.6 模组 **鼠辈啁啾 / Squeaky Ratkin**；永久 `packageId` 为 `coahuilite.squeakyratkin`，产品版本 `0.2.4`。
- `0.2.4`（最新）：GitHub 已发布核验（tag `v0.2.4`、Release CI `32044477840` success、资产 115 文件逐项核验）；Workshop 已上传且页面级核验完成（同一 item 3758115669、Updated 17 Aug 09:22、中英文案同步草稿）。Claim Pack：[`docs/release_review/release-0.2.4-review-zh.md`](./docs/release_review/release-0.2.4-review-zh.md)（含三项流程自我批评）。
- `0.2.3`：GitHub 已发布核验；Workshop 未上传。`0.2.1`/`0.2.2`：已发布，细节见各自 Claim Pack（`docs/release_review/`）。
- **包版本标识**：0.2.5 起三渠道（dev/steam/github）包内 `version.txt`（`SqueakyRatkin <版本>`/`build=<flavor>`/`commit=<sha>`，dev 带 `-dirty`），由 `stage-package.ps1` 统一写入；已发布的 0.2.4 各渠道包不含，重发与否由维护者定。
- 流程综合复盘（跨 0.2.x，含新增四项与通用原则）：[`docs/release_review/process-review-zh.md`](./docs/release_review/process-review-zh.md)。

## 权威入口
- 仓库导航：根 [`codemap.md`](./codemap.md)（纯汇总），具体目录读其 `codemap.md`；地图可能过期，源码与 `docs/` 现行合同权威。
- 架构/运行时/动作/发布边界：[`docs/project-architecture-contract.md`](./docs/project-architecture-contract.md)；设置 UI：[`docs/settings-ui-product-contract-zh.md`](./docs/settings-ui-product-contract-zh.md)；日志协议：[`docs/logging-protocol.md`](./docs/logging-protocol.md)。
- VoicePack 作者约定：[`docs/voice-pack-author-guide-zh.md`](./docs/voice-pack-author-guide-zh.md)；Workshop 页面维护源：[`docs/steam-workshop-page-copy-draft.md`](./docs/steam-workshop-page-copy-draft.md)（含版本一致性/只读核验维护规则）；发布流程唯一入口：[`docs/release-runbook-zh.md`](./docs/release-runbook-zh.md)。

## 工程决定与交接
- `0.2.3`（已实现发布；分类=**优化/行为变更**）：默认音源策略改为 Fallback + 内置 Race Example 种子（纯 Vanilla 回退豚鼠 clip 占比高，婴幼儿动作集全部命中——"婴幼儿路由豚鼠叫"根因是默认态落回退层，非年龄路由）。迁移规则：`voicePackMode` 节点缺失（Scribe 省略默认值节点）→ Fallback + 启动链 `EnsureBuiltInRaceDefault()` 种子一次（`voicePackDefaultSeeded` 幂等）；显式模式与已有选择永不被覆盖。schema 2→3。
- `0.2.4`（已实现发布）：精神崩溃音 hook 收窄至 `MentalBreakWorker.TryStart`（MentalBreakDef 驱动唯一通道）；Biotech BabyFits（Giggling/Crying）属正常精神状态发作，不派发崩溃音。日志协议 append-only 扩展（`pawn=`/`pawn_id=` 尾随字段），由 `SqueakLogCharacterization` 锁定；后续 0.3.x 触发基线须锁定此 hook 语义，Crying/Giggling 动作兼容为 0.3.x 年龄域规划。
- `0.3.x` 规划（TODO 详表）：年龄标签路由、年龄维度调制、Crying/Giggling 动作兼容、SettingsOrigin（srdiag v2 候选）；设计笔记 [`docs/internal-universalization-design-note-zh.md`](./docs/internal-universalization-design-note-zh.md) 含六项物理拆分门，是规划输入不覆盖现行合同。长期方向：Universal Squeaker 前置（race-aware、XML 驱动 per-race fallback profile、VoicePack 单种族池、选择链 `(race,xenotype) → race → fallback → 无声`）。
- Kiiro：实验分支不发布不 merge；通用化支持（标准 HAR `defName=Kiiro_Race`），无永久 adapter；Workshop 页面存在衍生作品限制，未经许可不得发布/宣传 Kiiro compat。
- `srdiag v1` characterization 护栏：`tools/SqueakLogCharacterization`（纯 net472 harness，28 事件）；扩展日志协议必须同步 characterization。
- **发布流程边界**：runbook 为唯一入口；Steam 页面核验仅**玩家视角只读** filedetails（绝不尝试登录态/编辑界面，编辑=维护者人工）；workshop 草稿变更后必须核对版本号/下载链接/字符数/检查清单；包内容核验含 `version.txt`。0.2.1 教训（LoadFolders 静默回归、codemap 泄漏、tag 重发 changelog、`merge -s ours`）已并入 runbook 与 process-review。
- 当前目标、开放行动、阻塞与明确延后项见 `TODO.md`。冷证据仅供历史冲突或明确请求，不能覆盖现行合同。
