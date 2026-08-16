# TODO

## 0.2.1 — 已知问题修复
- [x] 悬浮诊断重构：pawn 头上改为单字符标记（绿=就绪/金=受阻），详情移入可拖动诊断面板窗口（`SqueakDiagnosticsPanel`，不暂停游戏、不吸收输入、revision 缓存）；面板头部与每行显式显示 Pawn 种族 defName。仅诊断展示，不参与资格、路由或播放决策。
- [x] 修复七击未完成点击计数：`BeginSettingsSession` 改用 `Time.frameCount` 检测绘制中断，重新打开设置时清零 `versionClickCount`（去掉 `settingsSessionActive` 对称配对状态机）；关闭时 `EndSettingsSession` 双保险清零。只清未完成计数，不改已解锁状态。
- [x] 修复 fork NewRatkinPlus 因包名不匹配导致 SR 不生效：移除 `LoadFolders.xml` 的 `IfModActive="Solaris.RatkinRaceMod"` 硬门控，改无条件加载，发声注入按 XPath `defName="Ratkin"` 匹配。

## 0.2.2 — Kiiro 内部实验
- [x] 分支策略：已从 dev 开 `kiiro-experiment` 分支（起点 `88cbe1c`）；实验分支不发布，merge 回 dev 的时点与授权待定。
- [x] 薄装配 adapter（commit `a0c1708`）：Kiiro 已加载且隐藏实验开关启用时，启动深克隆 Ratkin 的 `CompProperties_Squeaker` 挂入标准 HAR `Kiiro_Race.comps`；`SQUEAKY_EXPERIMENTAL` 编译门限 Dev flavor（Steam/GitHub flavor 物理不含）；开关 OFF 启动=不装配=默认行为。未复制 Kiiro 资源/代码；无新增 resolver/settings/logging 事件。
- [x] 实机验证 A/B/C/E：A 开关关零装配（设置无键 + 零 Kiiro 派发）；B 状态行「本会话已装配」+ 26 条派发多动作覆盖（Wounded 因全局 Disabled 为零）；C 隔离探针 Off 静默、取消 Example 勾选后 Kiiro 走 Fallback 原版层、勾选后走 Example 池；E 回基线 Kiiro 静默且 Ratkin 正常发声。均经 Player.log 交叉核验；归因局限：日志 target 仅 thingIDNumber，不含种族信息。
- [ ] D 暂缓（用户决定）：Biotech + Kiiro Baseliner 异种域验证（仅剩此项；受控 DLC 全关基线已补测通过——开关 ON、官方 DLC 全关时装配与发声正常）。
- [ ] 日志覆盖缺口（用户指出，暂不实现）：部分环节日志打少（装配/探针阶段无显式记录），仅记录，待后续决定。
- [x] 0.2.2 发布定位调整（用户决定）：0.2.2 发布不包含 Kiiro 实验性兼容功能；实验在 `kiiro-experiment` 分支独立继续。
- [ ] `About.xml` 增加 `<modVersion>`（方案 A 已确认）：csproj `<Version>` 为主源、About.xml 跟随，release 前检查两处一致；同步更新 AGENTS.md 的"唯一手动维护版本"表述（AGENTS.md 修改需用户确认）。

## 发布卫生（0.2.1 复盘产生）
- [x] 建立版本无关的 release runbook（`docs/release-runbook-zh.md`：阶段 0-4 流程、main/dev 分叉 merge 核验、`merge -s ours` 适用条件、tag 重发步骤、stage 包内容核验、Workshop 页面核对、隐私审查门禁）。
- [x] 提取 Release Claim Pack 固定模板（已写入 `docs/release-runbook-zh.md` 末尾，字段：version/source commit/tag/build flavor+identity/CI/artifact hash/包内容/privacy/渠道观察）。

## 长期架构 — 待设计
- [ ] 先在单一 Squeaky Ratkin 模组内部完成并验证 Universal core；当 SR 实质只剩 VoicePack 内容时，再物理拆出 Universal Squeaker 前置并让现有 SR Workshop 项目依赖它，禁止过早发布空壳前置或保留双实现。
- [ ] 设计 VoicePack 单种族声明与路由：每包只能声明一个 `raceDefName`；Race / Xenotype 分别按 `raceDefName` 与 `(raceDefName, xenotypeDefName)` 形成域；同域多个合格包组成带权池并公平抽取。先冻结加载注入窗口、XML ABI、权重语义、旧包迁移和 Scribe schema。
- [ ] 设计 XML 驱动的 per-race Vanilla fallback profile：精确 `raceDefName` + 15 action→SoundDef 映射；有 profile 的 race 自动启用，无 profile 的 race 只能由玩家显式启用并提示静音风险。选择链固定为 `(race,xenotype) VoicePack → race VoicePack → race fallback → 无声`；不得硬编码 C# race switch 或复制原版资产。

## 明确延后
- [ ] 仅在 `TicksAbs` 再次复现时调查归因。
- [ ] 0.2.1 代码卫生（全库代码梳理、`SqueakLog` 职责治理、`srdiag v1` characterization checks）延后到已知问题修复之后；仍须遵守“先 characterization 再拆分、不得改变 public typed facade / 字段顺序 / once key / 事件 ID / 日志行为”。

## 待重新确认
- [ ] 发布门禁、英文 VoicePack 作者指南和第三方 VoicePack 示例等旧候选方向不再视为已定计划；需要维护者重新确认后才能恢复。
