# MEMORY

## 当前耐久状态
- 项目为 RimWorld 1.6 模组 **鼠辈啁啾 / Squeaky Ratkin**；永久 `packageId` 为 `coahuilite.squeakyratkin`，产品版本为 `0.2.1`。
- 产品版本 `0.2.1` 已发布并核验（GitHub 重发后 `31c4e18`；Steam 页面级核验）；发布事实与两起发布事故见 [`docs/release_review/release-0.2.1-review-zh.md`](./docs/release_review/release-0.2.1-review-zh.md)，`v0.2.0` 见同目录 `release-0.2.0-review-zh.md`。
- Workshop：维护者已于 2026-08-16 报告发布完成；同一 item URL/ID、版本、visibility、预览与描述的观察细节尚未记录，Claim Pack 内保持待观察，直至维护者补充。

## 权威入口
- 仓库导航（外置地图）：根入口 [`codemap.md`](./codemap.md)（纯汇总），具体目录读其 `codemap.md`（如 `Source/SqueakyRatkin/Logging/codemap.md`）。地图可能过期，源码与 `docs/` 现行合同权威。
- 运行时、动作、resolver、VoicePack、Example 与发布边界：[`docs/project-architecture-contract.md`](./docs/project-architecture-contract.md)。
- 设置 UI：[`docs/settings-ui-product-contract-zh.md`](./docs/settings-ui-product-contract-zh.md)；日志协议：[`docs/logging-protocol.md`](./docs/logging-protocol.md)。
- VoicePack 作者约定：[`docs/voice-pack-author-guide-zh.md`](./docs/voice-pack-author-guide-zh.md)；Workshop 页面维护源：[`docs/steam-workshop-page-copy-draft.md`](./docs/steam-workshop-page-copy-draft.md)；发布流程：[`docs/release-runbook-zh.md`](./docs/release-runbook-zh.md)。

## 工程决定与交接
- 外置地图维护策略：codemap 采用外置增量维护——架构/文件职责变化后只更新受影响子图、根图对应条目与 `.slim/codemap.json`，不因无关改动全量重画；地图会过期，源码与 `docs/` 现行合同权威。导航入口见「权威入口」。
- `0.2.1`（已发布）为已知问题修复版本，三项：悬浮诊断重构（pawn 头上单字符标记 + 可拖动面板显示种族 `defName`，仅诊断展示）、七击计数复位、fork NewRatkinPlus 兼容。fork 核心约束：SR 运行时依赖是 `defName="Ratkin"` 的 def 而非 packageId——`LoadFolders.xml` 的 `IfModActive` 是源码确认的硬门控（`InitLoadFolders` 无回退、`AddFolders` 按 `ShouldLoad` 过滤），已移除且不得重新引入；实现细节与证据见 Claim Pack 与 git 历史。
- `0.2.2`（Kiiro 内部实验）分支策略：从 dev 开新分支实验，成功后 **no-squash 全量 merge** 回 dev（保留实验历史）；实验分支不发布。实验内容：隐藏开发者菜单加入 Kiiro 触发兼容开关，向标准 HAR `Kiiro_Race` 装配同一共享 `CompSqueaker` 验证触发链；当前全局 `RacePacks` 会把 Kiiro 路由进 Ratkin/Example 池，不得把“Kiiro 能发声”视为多种族路由成功，也不得宣称正式兼容。已实现（`kiiro-experiment` 分支）：开关与装配器以 `SQUEAKY_EXPERIMENTAL` 编译门限 Dev flavor（Steam/GitHub flavor 物理不含）；装配为启动时深克隆 Ratkin 的 `CompProperties_Squeaker` 挂入 `Kiiro_Race.comps`（幂等、静默、零新增日志、不改 ABI）；开关 OFF 启动 = 不装配 = 默认行为，会话内切换需重启生效。
- 长期方向是 Universal Squeaker 前置：基础设施发现 race；有内置 Vanilla fallback profile 的 race 自动启用，无内置 profile 的 race 默认不启用，但允许玩家显式启用并明确提示“无 fallback 且无可用 VoicePack 时不会发声”。VoicePack 每包且只能服务一个 race，同域多个包组成带权池公平抽取，Xenotype 域按 `(raceDefName, xenotypeDefName)` 路由。VoicePack 作为标准 RimWorld 从模组管理，完整启动生效；热重载不支持但不主动阻止。
- 物理拆分决议：先在单一 Squeaky Ratkin 模组内部完成并验证 Universal core 的逻辑分层、race-aware 路由和迁移；当 SR 实质只剩 VoicePack 内容时，再新建 Universal Squeaker 前置 Workshop 模组并让现有 SR 项目依赖它。禁止过早发布抽象壳前置或长期保留双实现。
- Kiiro 架构结论：本地 Workshop 1.6 内容确认其唯一种族为标准 HAR `AlienRace.ThingDef_AlienRace`，精确 `defName=Kiiro_Race`，无自定义 Pawn `thingClass`；长期由 Universal core 按通用 HAR/raceDefName 机制直接支持，不保留永久 Kiiro adapter；不得复制 Kiiro 资源/代码，不得新增 Kiiro 专属 resolver/settings/logging。Kiiro Workshop 页面存在明确衍生作品限制，未经作者明确许可不得发布或宣传 Kiiro compat 内容。
- Universal 内置按 race 的 Vanilla fallback profile，作为主动 race 支持的最低音源层，内容继续 XML 驱动：profile 以精确 `raceDefName` 声明 action→SoundDef 映射，例如 Ratkin→Boomrat、Kiiro→Cat、Milira→Goose；不得写成 C# race switch，也不得复制原版资产。选择链为 `(race,xenotype) VoicePack 池 → race VoicePack 池 → race fallback profile → 无声`。此为后期架构决议，不属于 `0.2.1`/`0.2.2` 实现。
- 前置拆分和 VoicePack 目标模型尚未实现；`0.2.1` 不得修改逻辑或 ABI，`0.2.2` 不得用全局 Race 池结果冒充 race-aware 路由。ABI 未冻结前不得修改 XML 标签、域键、Scribe schema 或共享 DLL 边界。
- 发布复盘核心教训（细节见 `docs/release-runbook-zh.md` 与 Claim Pack）：main/dev 分叉 merge 时 auto-merged 文件不受 `git checkout --ours` 控制，merge 后必须 `git diff <修复commit> <mergecommit>` 全量核验关键行为文件；stage 排除清单含 `codemap.md`；changelog 时间以最终发布为准。
- `About.xml` 的 `<modVersion>` 方案 A 已确认（0.2.2 实施）：csproj `<Version>` 为主源、`About.xml <modVersion>` 跟随，release 前检查两处一致；同步更新 AGENTS.md 的"唯一手动维护版本"表述（该文件修改需用户确认）。
- 公开文档边界：Workshop 页面专注模组本身（不展示音频统计/开发者排障/迁移说明/制作步骤，规则见 `docs/steam-workshop-page-copy-draft.md`）；changelog 不写开发者功能解锁细节（七击等已模糊化）；页面俏皮句不加解释。
- 当前目标、开放行动、阻塞与明确延后项见 `TODO.md`。冷证据仅供历史冲突或明确请求，不能覆盖现行合同。
