# MEMORY

## 当前耐久状态
- 项目为 RimWorld 1.6 模组 **鼠辈啁啾 / Squeaky Ratkin**；永久 `packageId` 为 `coahuilite.squeakyratkin`，产品版本为 `0.2.1`。
- GitHub `v0.2.1` 已核验（tag + CI + asset + DLL 身份）；Claim Pack 见 [`docs/release-0.2.1-review-zh.md`](./docs/release-0.2.1-review-zh.md)。`v0.2.0` 发布事实与旧 Claim Pack 见 [`docs/release-0.2.0-review-zh.md`](./docs/release-0.2.0-review-zh.md)。
- Workshop 页面、实际二进制、visibility、preview 与 description 属人工外部状态；当前仓库不能证明，保持未确认，直至维护者报告并记录观察结果。

## 权威入口
- 仓库导航（外置地图）：根入口 [`codemap.md`](./codemap.md)（纯汇总），具体目录读其 `codemap.md`（如 `Source/SqueakyRatkin/Logging/codemap.md`）。地图可能过期，源码与 `docs/` 现行合同权威。
- 运行时、动作、resolver、VoicePack、Example 与发布边界：[`docs/project-architecture-contract.md`](./docs/project-architecture-contract.md)。
- 设置 UI：[`docs/settings-ui-product-contract-zh.md`](./docs/settings-ui-product-contract-zh.md)；日志协议：[`docs/logging-protocol.md`](./docs/logging-protocol.md)。
- VoicePack 作者约定：[`docs/voice-pack-author-guide-zh.md`](./docs/voice-pack-author-guide-zh.md)；Workshop 页面维护源：[`docs/steam-workshop-page-copy.md`](./docs/steam-workshop-page-copy.md)。

## 工程决定与交接
- 外置地图维护策略：codemap 采用外置增量维护——架构/文件职责变化后只更新受影响子图、根图对应条目与 `.slim/codemap.json`，不因无关改动全量重画；地图会过期，源码与 `docs/` 现行合同权威。导航入口见「权威入口」。
- `0.2.1` 改为已知问题修复版本，三项已完成：悬浮诊断重构（pawn 头上只画单字符标记 `●`，绿=就绪/金=受阻；详情移入可拖动、不暂停游戏、不吸收输入的 `SqueakDiagnosticsPanel` 面板窗口，复用 `SqueakySettingsUI` token，revision 缓存零每帧重建；面板头部与每行显式显示 `pawn.def.defName`，仅诊断展示）、修复七击未完成点击计数（打开设置时按 `Time.frameCount` 帧号检测清零，去除 `settingsSessionActive` 对称配对状态机）、修复 fork NewRatkinPlus 因包名不匹配导致 SR 不生效。fork 根因（已用 RimSage 读 RimWorld 源码确认）：`LoadFolders.xml` 的 `IfModActive="Solaris.RatkinRaceMod"` 是硬门控——`ModContentPack.InitLoadFolders` 在存在匹配版本块且 `list.Count>0` 时直接 `AddFolders(list); return;`，不再走默认版本文件夹回退；`AddFolders` 只加载 `LoadFolder.ShouldLoad` 为 true 的文件夹。故官方 packageId 不在（fork 改包名）时 `1.6/`（DLL + Defs + Patches + Languages）整体不加载。而 SR 实际运行时依赖是 `defName="Ratkin"` 的 def（comp 注入 XPath 与 `HarRatkinXenotypeDiscovery` 均按 defName 识别），并非 packageId，已移除包名门控以兼容保留该 def 的 fork。代码梳理、`SqueakLog` 职责治理与 srdiag v1 characterization 已延后到已知问题修复之后；本版仍不得改变产品逻辑、VoicePack/XML ABI、路由、Scribe schema 或模组装配，也不抽库/抽前置。
- `0.2.2` 计划在隐藏开发者菜单加入 Kiiro 触发兼容实验开关，仅验证通用组件生命周期与 Harmony 触发链能否作用于第二 HAR 种族；当前全局 `RacePacks` 会把 Kiiro 路由进 Ratkin/Example 池，因此不得把“Kiiro 能发声”视为多种族路由成功，也不得宣称正式兼容。
- 长期方向是 Universal Squeaker 前置：基础设施发现 race；有内置 Vanilla fallback profile 的 race 自动启用，无内置 profile 的 race 默认不启用，但允许玩家显式启用并明确提示“无 fallback 且无可用 VoicePack 时不会发声”。VoicePack 每包且只能服务一个 race，同域多个包组成带权池公平抽取，Xenotype 域按 `(raceDefName, xenotypeDefName)` 路由。VoicePack 作为标准 RimWorld 从模组管理，完整启动生效；热重载不支持但不主动阻止。
- 物理拆分决议：先在单一 Squeaky Ratkin 模组内部完成并验证 Universal core 的逻辑分层、race-aware 路由和迁移；当 SR 实质只剩 VoicePack 内容时，再新建 Universal Squeaker 前置 Workshop 模组并让现有 SR 项目依赖它。禁止过早发布抽象壳前置或长期保留双实现。
- Kiiro 架构结论：本地 Workshop 1.6 内容确认其唯一种族为标准 HAR `AlienRace.ThingDef_AlienRace`，精确 `defName=Kiiro_Race`，无自定义 Pawn `thingClass`；长期由 Universal core 按通用 HAR/raceDefName 机制直接支持，不保留永久 Kiiro adapter。`0.2.2` 仅允许一个不发布的本地薄装配 adapter，用来在当前 Ratkin-only 装配下给 `Kiiro_Race` 挂同一共享 `CompSqueaker` 并验证触发链；不得复制 Kiiro 资源/代码，不得新增 Kiiro 专属 resolver/settings/logging。Kiiro Workshop 页面存在明确衍生作品限制，未经作者明确许可不得发布或宣传 Kiiro compat 内容。
- Universal 内置按 race 的 Vanilla fallback profile，作为主动 race 支持的最低音源层，内容继续 XML 驱动：profile 以精确 `raceDefName` 声明 action→SoundDef 映射，例如 Ratkin→Boomrat、Kiiro→Cat、Milira→Goose；不得写成 C# race switch，也不得复制原版资产。选择链为 `(race,xenotype) VoicePack 池 → race VoicePack 池 → race fallback profile → 无声`。此为后期架构决议，不属于 `0.2.1`/`0.2.2` 实现。
- 前置拆分和 VoicePack 目标模型尚未实现；`0.2.1` 不得修改逻辑或 ABI，`0.2.2` 不得用全局 Race 池结果冒充 race-aware 路由。ABI 未冻结前不得修改 XML 标签、域键、Scribe schema 或共享 DLL 边界。
- 当前目标、开放行动、阻塞与明确延后项见 `TODO.md`。冷证据仅供历史冲突或明确请求，不能覆盖现行合同。
