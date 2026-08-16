# TODO

## 0.2.1 — 已知问题修复
- [x] 悬浮诊断重构：pawn 头上改为单字符标记（绿=就绪/金=受阻），详情移入可拖动诊断面板窗口（`SqueakDiagnosticsPanel`，不暂停游戏、不吸收输入、revision 缓存）；面板头部与每行显式显示 Pawn 种族 defName。仅诊断展示，不参与资格、路由或播放决策。
- [x] 修复七击未完成点击计数：`BeginSettingsSession` 改用 `Time.frameCount` 检测绘制中断，重新打开设置时清零 `versionClickCount`（去掉 `settingsSessionActive` 对称配对状态机）；关闭时 `EndSettingsSession` 双保险清零。只清未完成计数，不改已解锁状态。
- [x] 修复 fork NewRatkinPlus 因包名不匹配导致 SR 不生效：移除 `LoadFolders.xml` 的 `IfModActive="Solaris.RatkinRaceMod"` 硬门控，改无条件加载，发声注入按 XPath `defName="Ratkin"` 匹配。

## 0.2.2 — 代码卫生与可读化
- [ ] 全库代码梳理：先 characterization 再拆分；不得改变 public typed facade / 字段顺序 / once key / 事件 ID / 日志行为。产出问题清单与拆解顺序。
- [ ] `SqueakLog` 职责治理（卫生第一刀）：按 characterization 基线收敛职责，不新增日志事件、不改封闭门面。
- [ ] `srdiag v1` characterization checks：把当前日志/路由行为固化为可断言基线（仓库现无测试工程，需先定载体）。
- [ ] `About.xml` 增加 `<modVersion>`（方案 A 已确认）：csproj `<Version>` 为主源、About.xml 跟随，release 前检查两处一致；同步更新 AGENTS.md 的"唯一手动维护版本"表述（AGENTS.md 修改需用户确认）。

## Kiiro 实验（`kiiro-experiment` 分支独立进行，不属于 0.2.2）
- [x] 分支策略：从 dev 开 `kiiro-experiment`（起点 `88cbe1c`）；实验分支不发布。
- [x] 薄装配 adapter（`a0c1708`）：隐藏实验开关 + `SQUEAKY_EXPERIMENTAL` 编译门（仅 Dev flavor）；启动深克隆 Ratkin 的 `CompProperties_Squeaker` 挂入 `Kiiro_Race.comps`；开关 OFF 启动=不装配=默认行为。
- [x] 实机验证 A/B/C/E + 受控 DLC 全关基线：开/关/静默/池路由均按预期，经 Player.log 交叉核验。
- [ ] D 暂缓：Biotech + Kiiro Baseliner 异种域验证。
- [ ] 日志覆盖缺口（用户指出，暂不实现）：装配/探针阶段无显式记录，仅记录待后续决定。
- [ ] merge 回 dev：待内部通用化设计落地、以通用形态重新实现后再议（当前不 merge）。

## 发布卫生（0.2.1 复盘产生）
- [x] 建立版本无关的 release runbook（`docs/release-runbook-zh.md`：阶段 0-4 流程、main/dev 分叉 merge 核验、`merge -s ours` 适用条件、tag 重发步骤、stage 包内容核验、Workshop 页面核对、隐私审查门禁）。
- [x] 提取 Release Claim Pack 固定模板（已写入 `docs/release-runbook-zh.md` 末尾，字段：version/source commit/tag/build flavor+identity/CI/artifact hash/包内容/privacy/渠道观察）。

## 0.3.x — 内部通用化与拆分筹备（承接长期架构）
- [ ] 起草一页「内部普遍化设计笔记」（文档先行）：race 发现、域模型、装配决策、池路由、迁移；Kiiro 实验结论为输入。
- [ ] 先在单一 Squeaky Ratkin 模组内部完成并验证 Universal core；当 SR 实质只剩 VoicePack 内容时，再物理拆出 Universal Squeaker 前置并让现有 SR Workshop 项目依赖它，禁止过早发布空壳前置或保留双实现。
- [ ] 设计 VoicePack 单种族声明与路由：每包只能声明一个 `raceDefName`；Race / Xenotype 分别按 `raceDefName` 与 `(raceDefName, xenotypeDefName)` 形成域；同域多个合格包组成带权池并公平抽取。先冻结加载注入窗口、XML ABI、权重语义、旧包迁移和 Scribe schema。
- [ ] 设计 XML 驱动的 per-race Vanilla fallback profile：精确 `raceDefName` + 15 action→SoundDef 映射；有 profile 的 race 自动启用，无 profile 的 race 只能由玩家显式启用并提示静音风险。选择链固定为 `(race,xenotype) VoicePack → race VoicePack → race fallback → 无声`；不得硬编码 C# race switch 或复制原版资产。

## 明确延后
- [ ] 仅在 `TicksAbs` 再次复现时调查归因。

## 待重新确认
- [ ] 发布门禁、英文 VoicePack 作者指南和第三方 VoicePack 示例等旧候选方向不再视为已定计划；需要维护者重新确认后才能恢复。
