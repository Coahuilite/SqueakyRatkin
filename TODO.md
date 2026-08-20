# TODO

## 0.2.1 — 已知问题修复
- [x] 悬浮诊断重构：pawn 头上改为单字符标记（绿=就绪/金=受阻），详情移入可拖动诊断面板窗口（`SqueakDiagnosticsPanel`，不暂停游戏、不吸收输入、revision 缓存）；面板头部与每行显式显示 Pawn 种族 defName。仅诊断展示，不参与资格、路由或播放决策。
- [x] 修复七击未完成点击计数：`BeginSettingsSession` 改用 `Time.frameCount` 检测绘制中断，重新打开设置时清零 `versionClickCount`（去掉 `settingsSessionActive` 对称配对状态机）；关闭时 `EndSettingsSession` 双保险清零。只清未完成计数，不改已解锁状态。
- [x] 修复 fork NewRatkinPlus 因包名不匹配导致 SR 不生效：移除 `LoadFolders.xml` 的 `IfModActive="Solaris.RatkinRaceMod"` 硬门控，改无条件加载，发声注入按 XPath `defName="Ratkin"` 匹配。

## 0.2.2 — 代码卫生与可读化
- [x] 全库代码梳理：scout 已覆盖 Source/SqueakyRatkin 全部 47 个 C# 文件，产出 18 条问题清单与 top5 顺序；关键结论已进入 `MEMORY.md`/Logging 子图。
- [x] `SqueakLog` 职责治理（卫生第一刀）：在 characterization 护栏下机械拆为 public facade 与 internal `SqueakLogProtocol`（registry / once / formatter / sink）；不新增日志事件、不改 public facade、事件 ID、字段顺序、once key、human 文案或整体吞异常边界。
- [x] `srdiag v1` logging characterization checks：`tools/SqueakLogCharacterization` 链接真实 `SqueakLog.cs` 的纯 net472 控制台 harness（Verse.Log 桩、无游戏目录）已覆盖 28 事件、序列化、once/1024 清空、Dev/release gate、吞异常、路径脱敏与双 flavor。它只锁日志协议；`CompSqueaker` 触发/路由基线须在其拆分前单列。
- [x] `About.xml` 增加 `<modVersion>`（方案 A）：与 csproj `<Version>` 同步、一致性检查完成；AGENTS.md 版本主源表述随本次发布准备补充。
- [x] `CompSqueaker` 相位计算去重：`MaterializePeriodicStartupPhase` 与诊断路径共用 `CalculatePeriodicStartupReadyTick`；主模组与双 flavor logging harness 构建/运行通过。
- [x] 字面量外置（本轮低风险项）：`CompSqueaker.IsSocializing` 的五个 job marker 已集中为 `SocialJobMarkers`；Ratkin 适配字面量与浏览器语义数组仍保留在各自数据/适配边界，待 Universal 阶段统一。
- [x] 清理无调用方 internal helper：删除 `SqueakPeriodicPopulation.GetScale`（全树零调用）；public zero-call APIs 未删，需 ABI 决策。
## Kiiro 实验（`kiiro-experiment` 分支独立进行，不属于 0.2.2）

- [x] 分支策略：从 dev 开 `kiiro-experiment`（起点 `88cbe1c`）；实验分支不发布、当前不 merge。
- [x] 薄装配 adapter（`a0c1708`）：隐藏实验开关 + `SQUEAKY_EXPERIMENTAL` 编译门（仅 Dev flavor）；启动深克隆 Ratkin 的 `CompProperties_Squeaker` 挂入 `Kiiro_Race.comps`；开关 OFF 启动=不装配=默认行为。
- [x] 实机验证 A/B/C/E + 受控 DLC 全关基线：开/关/静默/池路由均按预期，经 Player.log 交叉核验。
- [ ] D 暂缓：Biotech + Kiiro Baseliner 异种域验证。
- [ ] 日志覆盖缺口（用户指出，暂不实现）：装配/探针阶段无显式记录；先保留为后续通用化 characterization 设计输入。
- [ ] merge 回 dev：待内部通用化设计落地、以通用形态重新实现后再议（当前不 merge）。

## 发布卫生（0.2.1 复盘产生）
- [x] 建立版本无关的 release runbook（`docs/release-runbook-zh.md`：阶段 0-4 流程、main/dev 分叉 merge 核验、`merge -s ours` 适用条件、tag 重发步骤、stage 包内容核验、Workshop 页面核对、隐私审查门禁）。
- [x] 提取 Release Claim Pack 固定模板（已写入 `docs/release-runbook-zh.md` 末尾，字段：version/source commit/tag/build flavor+identity/CI/artifact hash/包内容/privacy/渠道观察）。

## 0.3.x — 内部通用化与拆分筹备（承接长期架构）
- [x] 起草「内部普遍化设计笔记」（[`docs/internal-universalization-design-note-zh.md`](docs/internal-universalization-design-note-zh.md)）：冻结 race 发现/域模型/装配边界/池路由/迁移顺序与六项物理拆分门；该笔记是规划输入，不覆盖现行合同。
- [x] **0.3.x 架构决策（2026-08-18，分支 `0.3.x`）**：四方案并行比拼（内核但薄 / 契约先行分层 / 数据驱动极简 / 双内核渐进替换），采纳「内核但薄」——决策逻辑进零 Verse 引用 `Kernel/` 编译集 + `tools/KernelCharacterization` 黄金语料，不拆程序集；吸收契约派的迁移事务性/UI 泄漏点修复/BabyFits 源码核验 hook，与对拍派的黄金语料升级。实施唯一入口：[`docs/0.3x-refactor-architecture-decision-zh.md`](docs/0.3x-refactor-architecture-decision-zh.md)。
- [x] **动作门开放策略决策（2026-08-18）**：三方向比拼（封闭 Apple / 受控开放 Android 清单 / 全面开放 Intent），采纳**受控开放**——`SqueakActionDef` 注册 + `SqueakCompat` 门面 + 全闸门复用，0.4.x US 拆分与消费者同窗口落地；0.3.x 仅两件零成本前置：内核动作键出生即字符串（`ActionKey`，内置=枚举名）、srdiag v2 `action` 字段定型字符串键。决策文档 §2.2。
- [x] 0.3.0 前置：抓取 0.2.4 真实设置 fixture 入库（§6.2，改码前；无 schema 新装/Off/Fallback/多 selection/last-wins/消失 PackKey/Biotech inactive/损坏）
- [x] 0.3.0 枚举与键：纯枚举按领域归属提取（动作域/包域，namespace 不变）+ `ActionKey` 边界映射（内置=枚举名）
- [x] 0.3.0 `Kernel/` 骨架：域键/域池（`SqueakPoolRegistry`，PackKey 序数排序+带权累计权重）+ 选择链/fallback 合并/调制合成（`Select(ctx,mode,gate,rolls)`）+ 内置表种子（= `SqueakActionDefinitions.AudioKey`）
- [x] 0.3.0 `tools/KernelCharacterization`：纯度门 + 单测（41 断言）+ 语料生成器（S1-S5 + F03-F07 fixture 驱动，3782 例）；语义规范评审随提交（`docs/0.3x-equivalence-review-zh.md`：14 行对照 + 换面/不换面审计 + dev 豁免）
- [x] 0.3.0 resolver 接入：`BuildSnapshot` 经内核 builder + 注入 `(Ratkin,*)`（`SqueakKernelAdapter`）；换面/不换面分类审计（4 换面项承接 + 不换面项零改动 + dev 豁免单列）
- [x] 0.3.0 同变更删除 `ChoosePack`/`vanilla` 字典/`Or`/`ResolvedAudioPack`（无长期 shim）
- [x] 0.3.0 语料固化 + 验证门离线面全绿（内核语料 3782 例回放零 delta/SqueakLog 双 flavor v1 不变/主模组 Dev+Steam flavor 0 error/fixture 9 场景字节稳定）
- [x] 0.3.0 发布门槛实机面（2026-08-19）：A 池归属 147 次 dispatch 全命中内置 Example、B 时机计数区间正常（精确对照按维护者决定放弃）、C 设置 UI 确认 Ratkin 正常识别、H No-DLC 基线绿（旧档 NRE 为 RimWorld 内核 DLC 降级噪音，非 SR）——八面全绿；实机证据已落检查表（2026-08-20 维护者二次实测 + Player.log 87 派发核验；no_sound 6 条未复现按瞬态关闭；`docs/0.3x-release-gate-checklist-zh.md` 更新为全绿）
- [x] 0.3.0 错误路径修复 + 卫生梳理（2026-08-20）：`BuildFallback` 恢复种子内置表（重建失败仍放 SR_* 兜底，修静音回归）；`CollectKnownSounds` 恢复 `_Preview` 排除（对齐旧 `ResolvedAudioPack` 构造）；KernelGate 统一取 `ctx.Production`；删 `DomainPool.TotalWeight` 死计算；`SelectBuiltIn` 空键防御（主模组双 flavor 0 warning/0 error）；KernelCharacterization 增补失败路径 2 断言（41→43）。已提交（C12）。
- [x] 0.3.0 三项发布决策定案（2026-08-20）：① 双轨 = 本地 dev 包试用（`build-dev.ps1` 产物；dev/steam 包均本地纪律，CI 只为 GitHub 包服务；不设 prerelease tag，维护者自选渠道分发，观察 2 周起）；② 措辞 = 内核重构 + 拆分预告（不提拆分模组名称，只预告独立前置 mod，草案见决策文档 §5）；③ 热修 = `vX.Y.Z-hotfixN` 方案 a（z 不变，GitHub 自动 prerelease 标记为已知代价，下个 z 正式合并）。双轨发布执行仍阻塞（merge 授权 + 渠道发布授权）。
- [ ] merge 0.3.x → dev（择日；分支无交叉提交预期 fast-forward；merge 前跑三 harness 基线，见 `docs/handoff-0.3.0-zh.md` §5）
- [ ] 0.3.1：**半分离→全分离收口（2026-08-20 维护者确认）**——① `FallbackProfile.SoundKeys` 键 `SqueakAction`→字符串键 + 内核内置键清单（`Kernel/BuiltInActionKeys`，17 键唯一权威，0.3.2 append Crying/Giggling）+ `TryGetSoundKey` 纯字符串查表（键 ∈ 清单校验）；② `Select` mode 改内核自有枚举（SR 侧一处映射，同 ActionKey 模式）；③ `ActionKey` 移入 `Kernel/`（消除内核→根文件依赖）；④ `SqueakActionModel` 退出 harness 链接清单（SR 产品常量不入内核编译单元）；⑤ 决策文档 §3.1/§4.1/§5 同步修订（Packs 删除分期修正、键保持枚举改清单校验、迁移清单=US 编译集 Kernel/+0 根文件）；⑥ 临时层拆除清单：4 处 `Ratkin` 字面量（含 resolver `Choose` 域构造）+ `ComposeDomainKey` 两段式逐条登记。另含：`SqueakVoicePackDef.raceDefName`/validator/Example 声明；catalog filter 闸+域化收集+`GetTargetCandidates` assembled-only 投影（修 canonical/har hint 泄漏点）；事务性 Scribe 迁移（schema 3→4/1→2，fixture 先行）；srdiag v2（SettingsOrigin+race 身份+**action 字段定型字符串键**）；试验名单开关；两处接缝切核+旧路径同变更删除；外来 race per-race 池实证（合成输入，不进交付）；**内置动作五处同步一致性断言**（枚举/XML/SoundDef/本地化/统计，0.3.2 Crying/Giggling 前置护栏）。
- [ ] 0.3.2：`BuiltInFallbackTable`+`SqueakFallbackProfileStore`（Config 副本单写者）；pack fallback 字段；年龄全套（`ageTag`/`SqueakLifeStageResolver`/`ComposeModulation`/Crying+Giggling append 15/16+`TryStartMentalState` hook）；合同提升（17 动作、Fallback 末端→profile→无声、fallback 写通道工件化）；UI fallback 编辑器/重建/浏览器下放；XML ABI 定型；**漏斗纯逻辑提取**（`SqueakTimingModel`/`SqueakTriggerInvocation`/`SqueakActionPlan` → 纯文件 + harness 链接，新通路时机面机械化）；**彩蛋标签落地**（§2.4 YAGNI 修订：仅 `IsEgg` 标记无运算符，种族定位由 pack `raceDefName` 承担；开关默认关、XML ABI 同批 schema bump、语料彩蛋维度）。
- [ ] 0.4.x 动作门落地（US 拆分窗口）：`SqueakActionDef`/`SqueakActionRegistry`/`SqueakCompat`；`SqueakVoicePackAction.action` 枚举→string（节点名不变零迁移）；validator 键解析（内置 known/外部未注册 dormant/非法 error）；**玩家总闸 `allowExternalActions`（设置项默认开，policy 层实现，UI 可见）**；作者指南动作注册章节公开；内置 17 键语料回放零 delta 验收；无第三方需求信号则退回封闭（§2.2 放弃信号）；**八道闸门决策出生即纯**（策略纯函数 + 注入函子，路由核心公理 §2.3）。
- [ ] 先在单一 Squeaky Ratkin 模组内部完成并验证 Universal core；当 SR 实质只剩 VoicePack 内容时，再物理拆出 Universal Squeaker 前置并让现有 SR Workshop 项目依赖它，禁止过早发布空壳前置或保留双实现。
- [ ] 设计 VoicePack 单种族声明与路由：每包只能声明一个 `raceDefName`；Race / Xenotype 分别按 `raceDefName` 与 `(raceDefName, xenotypeDefName)` 形成域；同域多个合格包组成带权池并公平抽取。先冻结加载注入窗口、XML ABI、权重语义、旧包迁移和 Scribe schema。
- [ ] 设计 XML 驱动的 per-race Vanilla fallback profile：精确 `raceDefName` + 17 action→SoundDef 映射；有 profile 的 race 自动启用，无 profile 的 race 只能由玩家显式启用并提示静音风险。选择链固定为 `(race,xenotype) VoicePack → race VoicePack → pack fallback → 内置 profile → 无声`；不得硬编码 C# race switch 或复制原版资产。
- [ ] `SqueakyRatkinSettings` 拆 partial：ExposeData+迁移 / 运行时桥分离；Scribe 契约面需存档 fixture。
- [ ] `XenotypeUI` 拆出 Race 域编辑器；UI 三档缩放验证成本高，单独排期。
- [ ] Debug 入口合并（通用化重构后执行）：删除 7 个重复 DebugAction（统计×3、记录开关×2、清除/复制×2），相机指示器与 overlay 模式开关迁入开发者页；同步 settings-ui 合同 DebugAction 门控表述与 Debug 子图「四层门控」描述。

## 提案 / 待决策（未定案，需研判）

- [x] **彩蛋语音提案 → 已定案（2026-08-20）**：裁决见决策文档 §2.4——加性池成员（开关开才入池同权混抽，关 = 仅普通条目）、默认关、action 条目粒度、0.3.2 XML ABI 同批落地、第三方作者指南 0.4.x 同窗口开放；**同日 YAGNI 修订：取消种族运算符/条件表达式，仅保留彩蛋标签**（种族定位由 pack `raceDefName` 路由承担）；Kiiro 许可门不变（彩蛋内容不豁免）。原待定项①-⑥随裁决关闭。

## 明确延后
- [ ] 仅在 `TicksAbs` 再次复现时调查归因。

## 待重新确认
- [ ] 发布门禁、英文 VoicePack 作者指南和第三方 VoicePack 示例等旧候选方向不再视为已定计划；需要维护者重新确认后才能恢复。
- [ ] 0.3.0 发布三项决策（0.3.0 开发后期定案，详见决策文档 §5 发布门槛小节）：① GitHub prerelease 双轨发布是否采用；② 发布说明"底层架构升级"措辞是否合规；③ 0.3.0.x 热修版本策略确认。
