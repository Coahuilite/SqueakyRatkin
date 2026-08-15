# TODO

## 0.2.1 — 已知问题修复
- [x] 悬浮诊断重构：pawn 头上改为单字符标记（绿=就绪/金=受阻），详情移入可拖动诊断面板窗口（`SqueakDiagnosticsPanel`，不暂停游戏、不吸收输入、revision 缓存）；面板头部与每行显式显示 Pawn 种族 defName。仅诊断展示，不参与资格、路由或播放决策。
- [x] 修复七击未完成点击计数：`BeginSettingsSession` 改用 `Time.frameCount` 检测绘制中断，重新打开设置时清零 `versionClickCount`（去掉 `settingsSessionActive` 对称配对状态机）；关闭时 `EndSettingsSession` 双保险清零。只清未完成计数，不改已解锁状态。
- [x] 修复 fork NewRatkinPlus 因包名不匹配导致 SR 不生效：移除 `LoadFolders.xml` 的 `IfModActive="Solaris.RatkinRaceMod"` 硬门控，改无条件加载，发声注入按 XPath `defName="Ratkin"` 匹配。

## 0.2.2 — Kiiro 内部实验
- [ ] 使用仅本地、不发布的 Kiiro 薄装配 adapter，在 Kiiro 模组已加载且隐藏实验开关启用时，向标准 HAR `Kiiro_Race` 装配同一共享 `CompSqueaker`；不得复制 Kiiro 资源/代码或形成永久 adapter。长期 Universal core 应按通用 HAR/raceDefName 机制直接支持 Kiiro。
- [ ] 在 VoicePack Off 或可辨识隔离探针下验证 Kiiro 的组件生命周期，以及 Select、Wounded、Draft、成功 Attack 和周期触发进入共享漏斗；当前全局 `RacePacks` 会复用 Ratkin/Example 音池，禁止把出声视为 race-aware 路由成功或正式 Kiiro 兼容。
- [ ] 基线组合为 Core + Harmony + HAR + Ancot Library + Kiiro + 本地 adapter，官方 DLC 全关；另行验证 Biotech + 可选 Kiiro gene patch，但不得把 gene patch 作为基础依赖。未经 Kiiro 作者明确许可，不发布或宣传 Kiiro compat 内容。
- [ ] `About.xml` 增加 `<modVersion>`（方案 A 已确认）：csproj `<Version>` 为主源、About.xml 跟随，release 前检查两处一致；同步更新 AGENTS.md 的"唯一手动维护版本"表述（AGENTS.md 修改需用户确认）。

## 发布卫生（0.2.1 复盘产生）
- [x] 建立版本无关的 release runbook（`docs/release-runbook.md`：阶段 0-4 流程、main/dev 分叉 merge 核验、`merge -s ours` 适用条件、tag 重发步骤、stage 包内容核验、Workshop 页面核对、隐私审查门禁）。
- [x] 提取 Release Claim Pack 固定模板（已写入 `docs/release-runbook.md` 末尾，字段：version/source commit/tag/build flavor+identity/CI/artifact hash/包内容/privacy/渠道观察）。

## 长期架构 — 待设计
- [ ] 先在单一 Squeaky Ratkin 模组内部完成并验证 Universal core；当 SR 实质只剩 VoicePack 内容时，再物理拆出 Universal Squeaker 前置并让现有 SR Workshop 项目依赖它，禁止过早发布空壳前置或保留双实现。
- [ ] 设计 VoicePack 单种族声明与路由：每包只能声明一个 `raceDefName`；Race / Xenotype 分别按 `raceDefName` 与 `(raceDefName, xenotypeDefName)` 形成域；同域多个合格包组成带权池并公平抽取。先冻结加载注入窗口、XML ABI、权重语义、旧包迁移和 Scribe schema。
- [ ] 设计 XML 驱动的 per-race Vanilla fallback profile：精确 `raceDefName` + 15 action→SoundDef 映射；有 profile 的 race 自动启用，无 profile 的 race 只能由玩家显式启用并提示静音风险。选择链固定为 `(race,xenotype) VoicePack → race VoicePack → race fallback → 无声`；不得硬编码 C# race switch 或复制原版资产。

## 明确延后
- [ ] 仅在 `TicksAbs` 再次复现时调查归因。
- [ ] 0.2.1 代码卫生（全库代码梳理、`SqueakLog` 职责治理、`srdiag v1` characterization checks）延后到已知问题修复之后；仍须遵守“先 characterization 再拆分、不得改变 public typed facade / 字段顺序 / once key / 事件 ID / 日志行为”。

## 待重新确认
- [ ] 发布门禁、英文 VoicePack 作者指南和第三方 VoicePack 示例等旧候选方向不再视为已定计划；需要维护者重新确认后才能恢复。
