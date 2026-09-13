# TODO

> **0.3.3 会话交接**：[`docs/handoff-0.3.3-zh.md`](docs/handoff-0.3.3-zh.md) —— 未提交状态清单、已定案、发布分派（发布会话）、US 兼容（U1–U4）与迁移裁决（Q1–Q10）。更新与发布流程在后续会话推进。

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
- [x] **动作门开放策略决策（2026-08-18）**：三方向比拼（封闭 Apple / 受控开放 Android 清单 / 全面开放 Intent），采纳**受控开放**——`SqueakActionDef` 注册 + `SqueakCompat` 门面 + 全闸门复用，0.4.x US 拆分与消费者同窗口落地；0.3.x 仅两件零成本前置：内核动作键出生即字符串（`ActionKey`，内置=枚举名）、srdiag v2 `action` 字段定型字符串键。决策文档 §2.2。**2026-08-22 修订：机制改在 US 仓库 0.3.x 并行窗口开发、0.4 随 US 首版发布（见下）。**
- [x] 0.3.0 前置：抓取 0.2.4 真实设置 fixture 入库（§6.2，改码前；无 schema 新装/Off/Fallback/多 selection/last-wins/消失 PackKey/Biotech inactive/损坏）
- [x] 0.3.0 枚举与键：纯枚举按领域归属提取（动作域/包域，namespace 不变）+ `ActionKey` 边界映射（内置=枚举名）
- [x] 0.3.0 `Kernel/` 骨架：域键/域池（`SqueakPoolRegistry`，PackKey 序数排序+带权累计权重）+ 选择链/fallback 合并/调制合成（`Select(ctx,mode,gate,rolls)`）+ 内置表种子（= `SqueakActionDefinitions.AudioKey`）
- [x] 0.3.0 `tools/KernelCharacterization`：纯度门 + 单测（41 断言）+ 语料生成器（S1-S5 + F03-F07 fixture 驱动，3782 例）；语义规范评审随提交（`docs/0.3x-equivalence-review-zh.md`：14 行对照 + 换面/不换面审计 + dev 豁免）
- [x] 0.3.0 resolver 接入：`BuildSnapshot` 经内核 builder + 注入 `(Ratkin,*)`（`SqueakKernelAdapter`）；换面/不换面分类审计（4 换面项承接 + 不换面项零改动 + dev 豁免单列）
- [x] 0.3.0 同变更删除 `ChoosePack`/`vanilla` 字典/`Or`/`ResolvedAudioPack`（无长期 shim）
- [x] 0.3.0 语料固化 + 验证门离线面全绿（内核语料 3782 例回放零 delta/SqueakLog 双 flavor v1 不变/主模组 Dev+Steam flavor 0 error/fixture 9 场景字节稳定）
- [x] 0.3.0 发布门槛实机面（2026-08-19）：A 池归属 147 次 dispatch 全命中内置 Example、B 时机计数区间正常（精确对照按维护者决定放弃）、C 设置 UI 确认 Ratkin 正常识别、H No-DLC 基线绿（旧档 NRE 为 RimWorld 内核 DLC 降级噪音，非 SR）——八面全绿；实机证据已落检查表（2026-08-20 维护者二次实测 + Player.log 87 派发核验；no_sound 6 条未复现按瞬态关闭；`docs/0.3x-release-gate-checklist-zh.md` 更新为全绿）
- [x] 0.3.0 错误路径修复 + 卫生梳理（2026-08-20）：`BuildFallback` 恢复种子内置表（重建失败仍放 SR_* 兜底，修静音回归）；`CollectKnownSounds` 恢复 `_Preview` 排除（对齐旧 `ResolvedAudioPack` 构造）；KernelGate 统一取 `ctx.Production`；删 `DomainPool.TotalWeight` 死计算；`SelectBuiltIn` 空键防御（主模组双 flavor 0 warning/0 error）；KernelCharacterization 增补失败路径 2 断言（41→43）。已提交（C12）。
- [x] 0.3.0 三项发布决策定案（2026-08-20）：① 双轨 = 本地 dev 包试用（`build-dev.ps1` 产物；dev/steam 包均本地纪律，CI 只为 GitHub 包服务；不设 prerelease tag，维护者自选渠道分发，观察 2 周起）；② 措辞 = 内核重构 + 拆分预告（不提拆分模组名称，只预告独立前置 mod，草案见决策文档 §5）；③ 热修 = `vX.Y.Z-hotfixN` 方案 a（z 不变，GitHub 自动 prerelease 标记为已知代价，下个 z 正式合并）。执行进度（2026-08-21）：merge 完成、GitHub 已发布核验；剩 Steam 人工上传 + 页面核验 + Claim Pack。
- [x] merge 0.3.x → dev（2026-08-21 ff 完成：dev=`8fd7cc6`；dev→main PR#23 squash：main=`c06a90b`；merge 前三 harness 已绿）
- [x] 0.3.0 Steam 发布收尾（2026-08-21）：上传完成（非 SteamCMD）、公开 API 页面核验完成（result=1、同一 item、描述含 0.3.0、Updated 10:02 UTC+8）、Release Claim Pack 已写（`docs/release_review/release-0.3.0-review-zh.md`）；剩余：Steam 编辑器删除旧「内置 Example 默认启用」公告（线上仍存在，维护者人工操作）→ 复核后渠道转「完整」
- [ ] 提案：音频包 XML 简化（2026-08-21 评估待定）——① 指南级零风险修正：`<sustain>false</sustain>` 为 Verse 默认可省略、ranges 可选并文档化默认、部分覆盖包的删块流程写明；② manifest→XML 生成器脚本（作者写 15 行清单代替 120 行模板，部分包天然支持）；③ C# 运行时 SoundDef 生成（最大简化，触及启动序/stage 镜像断言/0.3.1 XML ABI，**仅第三方需求信号出现才做**）。SoundDef 无 ParentName，XPath 继承不可行（已排除）。
- [x] 作者指南统一为单一 SKILL 正本（2026-08-23 维护者定案并已实现）：全量正文迁至 `.github/skills/squeaky-voicepack-authoring/SKILL.md`（frontmatter name+description 触发词；正文人类可读优先、agent 按第 13 节执行），原 `docs/voice-pack-author-guide-zh.md` 删除；**SKILL 自包含：不依赖外部文档/模板，唯一允许的专用脚本 = `scripts/new-voicepack.ps1`（脚手架）与 `scripts/verify-voicepack-xml-abi.ps1`（维护者锁）**；verify-local 第 10 项的「作者指南」标记源 = SKILL.md；同批落地 `scripts/new-voicepack.ps1` 脚手架（packageId/PackDefName/Actions → 目录 + 最小 XML + 占位音频目录 + README，幂等拒绝覆盖）。
- [x] 0.3.1（2026-08-21/22）**离线面 + 核心实机面完成**（提交链 C20–C32；verify-local 9 项全绿，双语料 3782+10404 例回放零 delta；维护者 dev 包实测：Off 模式内置表兜底+模式往返、成人崩溃、婴儿 fits 静默零误报、迁移幂等隐式通过、Player.log 零 SR 错误；详情 MEMORY 0.3.1 条目）
- [ ] 0.3.1 剩余低频实机项（继续游玩自然覆盖，不阻塞）：Remix 模式；损坏设置文件失败路径（保留旧列表+不覆盖）；Config 副本手改 delta 合并/删除重建；婴儿发声年龄调制听感（voxPitch 1.6）；彩蛋开关 `allowEasterEggSounds=true` 无回归；No-DLC dormant；Eat/Wounded/Social/Death/Draft/Undraft/Attack/Work/Equip 动作触发；UI 统计页 17 行动作目视。
- [x] 0.3.1 **发布流程暂缓**（2026-08-22 维护者指示）→ **已解除并跳过 0.3.1**（2026-08-23）：0.3.2 承载全部 0.3.1 工作发布 GitHub prerelease `v0.3.2-pre1`；Steam 阻断不执行。
- [ ] **0.3.2/0.3.3（2026-08-22 重排；2026-08-23 定案：0.3.2 正式版跳过，全部工作随 0.3.3 发布）＝功能调整 + XML ABI 固化 + Eat 两级开关**（SR 0.3.x UI 不变，原 UI 专项转 US 仓库；ABI 文档按「首个携带 0.3.1 XML ABI 的发布版本」表述）：
  - [x] 玩家触发内容身份门控（0.3.2 已提交 C34；离线门全绿）：`PlayerSelection`/`ActiveCommand` 来源要求 `Pawn.IsPlayerControlled`；`PlayerSelection` 额外要求可响应 = `!Downed && Awake()`。gate 放 `NotifyExternal` 既有触发闸层（plan.Configured 后、TryTrigger 前），fail-closed 静默、不耗冷却/不新增 outcome/不动日志；纯标志进 `SqueakTriggerInvocation`（`IsPlayerInitiated`/`RequiresResponsivePawn`）+ `TriggerInvocationRules` 单测扩展；Verse 采样（`IsPlayerControlled`/`Downed`/`RestUtility.Awake`）声明 out-of-scope。**`Selector.Select(playSound:false)` 已定案 = 一并过滤**（RimWorld 1.6 源码核验：全部 `playSound:false` 调用点均为复活/区域创建/培养舱/基因提取/传送等程序性后续选择，非玩家点击反馈；patch 以 `__1` 位置注入过滤）。
  - [x] 身份门控实机矩阵·维护者已覆盖（2026-08-23）：睡眠、非玩家 pawn、精神崩溃玩家 pawn 均静默；清醒可控响正常派发。
  - [ ] 身份门控实机矩阵·余项自然覆盖：倒地/敌对/访客/野化静默；任务加入鼠族与奴隶响；Draft/Equip/Attack/周期动作零变化；`playSound:false` 程序性选择静默（日志已含 `pawn_faction`/`pawn_ctrl` 便于反查）。
  - [x] XML ABI 固化（音频包作者面优先，2026-08-22 维护者定案）：① 合同新增「VoicePack 作者 XML ABI」公开稳定面（节点表/默认值/validator 规则/兼容政策：字段只增不改、action 键 append-only、fail-closed）；② 决策文档 §1.1/§2.4/§5 修订：作者 XML 面自首个携带版本起冻结（内部面仍 0.x 窗口）、`IsEgg` 纳入公开作者 ABI；③ **verify-local 新增第 10 项 VoicePack XML ABI 一致性锁（`scripts/verify-voicepack-xml-abi.ps1`：示例 XML × validator × 作者指南三向对照 + C# 17 键顺序 + 彩蛋测试包；`-NoRestore` 离线开关）**；④ 作者指南升格为 ABI 合同一部分（加稳定性声明；2026-08-23 正文与 skill 正本统一为 `.github/skills/squeaky-voicepack-authoring/SKILL.md`，原 docs 指南删除，配套 `scripts/new-voicepack.ps1` 脚手架）；⑤ `action` 枚举→string 仍 0.4.x（对作者 XML 不可见、零迁移，不提前）。
  - [x] 彩蛋测试包（0.3.2 测试产物，gitignored）：`dist/SqueakyRatkinEggTestVoices/`（packageId `coahuilite.squeakyratkin.eggtest`，`SR_EggTest_Select` 单 Select 动作 `IsEgg=true` + 3 个示例 clip 副本；README 含开关两态验收矩阵）。
  - [x] 彩蛋路由实机确认（2026-08-23 维护者）：设置 `allowEasterEggSounds=True` + 蛋包已选中；Player.log 5 条 `Select -> SR_EggTest_Select_Select (race_pack, egg)` 且 `egg=true`（Lightbloom/Castanea），同会话 `egg=false` 11 条对照，SR 零 warning/error。
  - [x] `pawn_faction`/`pawn_ctrl` 日志识别字段实机确认（2026-08-23 维护者，最新 dev 包）：12:08 会话 SR 41 行全 info；`pawn_ctrl` player 10 / nonplayer 26，`pawn_faction` PlayerColony 27 / Rakinia 9；PlayerColony 非可控 pawn（精神崩溃/动物等）在 Attack/Call/Move/Sleep 走 StateEvent/周期路径且被正确标 nonplayer，无 Select nonplayer 泄漏；身份矩阵从此可由日志反查。
  - [x] 发配日志彩蛋适配 + dev 日志重排简化（0.3.2 已提交 C35）：`ChainResult.IsEgg` → `SqueakSoundChoice.IsEgg` → 装配器；成功路径不再并列发 `audio.dispatch.ok`(v1)+`audio.route.selected`(v2) 两条，只发一条 v2 明细（`sound tier egg suppressed_detail pawn pawn_id pawn_faction pawn_ctrl`，`pawn_ctrl` 为 `player|nonplayer` 二值，人类句 `Audio route: <action> -> <sound> (<tier>[, egg][, nonplayer]).`）；v1 28 事件字节与 `audio.dispatch.ok` registry 记录均不动；characterization 双 flavor 已同步并通过。
  - [x] 相关文档同步：codemap 链（根/Source/Debug/Logging/Patches/scripts）已同步 0.3.2 身份闸/日志重排/第 10 项/彩蛋测试包；CHANGELOG 与发布审查随发布时执行。
  - [x] **GitHub prerelease `v0.3.2-pre1` 发布并核验（2026-08-23，维护者授权）**：0.3.x C34–C40 提交并推送；dev merge 0.3.x + merge origin/main 并推送；PR #24 squash → main `60f7d88`（tree == dev）；tag → Release CI `32620324890` success → prerelease 资产 `SqueakyRatkin-v0.3.2-pre1.zip` 116 文件核验（SHA256 与 API digest 一致）；隐私审计 0 真实命中。**Steam 发布阻断不执行**；观察记录 `docs/release_review/release-0.3.2-pre1-review-zh.md`。
  - [x] **`.slim` 工具残留排除（2026-08-23 维护者）**：`.gitignore` 整目录忽略；`main`/`dev`/`0.3.x`/`kiiro-experiment`/`0.2.4-FINAL` 分支 tip 已删除 `.slim/codemap.json`（main 经 PR #25）。
  - [ ] 历史/tag 清理（**待维护者授权**）：发布 tags `v0.2.1`–`v0.3.2-pre1` 与历史提交仍可达 `.slim/codemap.json`（含本地绝对路径）；清理需重写全部受影响 refs/tags 并 force-push，按隐私事件流程单独执行。
  - [x] **0.3.3 本地推进完成（2026-09-13，停在远端推送前）**：流程冗余评估 + 最小仪式落地（新增 `scripts/privacy-audit.ps1` / `scripts/check-pack-readiness.ps1`；runbook 重写；`AGENTS.md` 增 External-state boundaries；`ci.yml`/`release.yml` 接线；评估 [`docs/release_review/process-redundancy-review-zh.md`](docs/release_review/process-redundancy-review-zh.md)）；Eat 两级开关 + 文档/记忆本地提交（未推送）；dev 包 `dist/dev/SqueakyRatkin` 已出；证据 = verify-local 11/11、readiness 全绿、`privacy-audit -FullHistory` 0 未接受命中（5 条 known-debt）。
  - [ ] **0.3.3 远端发布（需授权；入口 [`docs/handoff-0.3.3-zh.md`](docs/handoff-0.3.3-zh.md) §10）**：push dev → PR dev→main → merge → tag `v0.3.3` → release CI（已含 11 项 + 发布面门）→ 资产核验 → 双语 CHANGELOG `未发布 — 0.3.3` 换时间 → 完整 Claim Pack `docs/release_review/release-0.3.3-review-zh.md`（字段取 `[claim]` 快照 + CI 输出）→ 渠道核验。**Steam 仍阻断**；解除时另需同步 `docs/steam-workshop-page-copy-draft.md` 的版本号/下载链接/字符数（0.3.2 未上 Steam，页面仍是 0.3.0 口径）。
  - [x] **流程简化裁决 V1–V3（2026-09-13 定案）**：V1 追认本地 commit 免授权；V3 接受 CI 加门 + SDK 对齐 10.0.x；V2 先出专项方案不动 git——[`docs/release_review/privacy-history-rewrite-plan-zh.md`](docs/release_review/privacy-history-rewrite-plan-zh.md)（方案 B 执行需单独授权；Q1–Q5 待裁决）。
  - [x] **Eat 触发粒度两级开关（2026-08-23 维护者决定并实施，承接 Steam 评论反馈）**：默认保留 job 级派发（整个 Ingest job 含端食物赶路）；父 `eatOnlyDuringChewing`（默认关）→「正在摄入营养」（零营养成瘾品不算；啤酒 0.08/仙馔 0.2 等带营养成瘾品已算）；子 `eatIncludeDrugs`（显示名「使用成瘾品」，默认关、父关时禁用且强制 false）→ vanilla `ChewIngestible` toil（零营养成瘾品也算，toil 名本进程未确认时回落完整 job 级 fail-open）。纯规则 `SqueakEatOccurrence`（三模式 + 两默认值 + toil 名常量）+ harness 单测；设置 ×2/Scribe/fixture host 镜像/双语 Keyed/合同 §3/SKILL §7/settings-ui 合同/双语 CHANGELOG/Source + UI codemap 已同步（详情见 MEMORY）。独立对抗复核（fresh context）3 项应修 + 2 项 nit 已处理。
  - [ ] Eat 开关实机验证（维护者，dev 包）：三态 × 场景（meal 端食物赶路 / 烟卷 / 薄片 / 啤酒 / 仙馔 / 营养膏 / 背包吃 / 动物 / 尸体）；重点：父关与旧实现一致、父开+子开时食物行为与「父开子关」一致、关父项后子项灰显且归 false、切换即时生效与重启保留。
  - [x] Eat 开关发布归属与版本号（**2026-08-23 维护者定案：推进 0.3.3**）：csproj `<Version>` 与 `About/About.xml <modVersion>` 已 bump 到 0.3.3，双语 CHANGELOG 顶端标题改为 `未发布 — 0.3.3`（0.3.2 仅 GitHub prerelease，其工作并入 0.3.3）；Steam 仍阻断。
- [ ] **US 兼容性（SR 退役为纯音频包前的硬前置，2026-08-23 检查完成）**：技术报告 [`docs/us-sr-compatibility-check-zh.md`](docs/us-sr-compatibility-check-zh.md)；**迁移/退役方案 [`docs/us-sr-migration-plan-zh.md`](docs/us-sr-migration-plan-zh.md)**。当前双开安全（US 只承认 US 型包）；**风险**：US 自动装配逃逸门只认自己程序集的 comp 类型，任一 US 型 Ratkin 包或 legacy 桥落地 → Ratkin 双 comp → 双响（两仓 15/16 patch 同目标）。**归属与顺序硬门（存量玩家约束）**：修复落 US 侧（U1 跨程序集检测 + 让位 + skip 日志），SR 本窗口零运行时改动；顺序 = `U1 落地 → US 型 Ratkin 包/桥 → SR 1.0 内容化`，倒置即事故（先包/桥=双响；SR 先退役=静默）。待办：① 转述 US 侧 U1–U4；② 冻结 `SqueakyRatkin.CompProperties_Squeaker`/`CompSqueaker` 全名与 XML 装配契约；③ 1.0 前置链 SR → US → FerriteLib 且 SR 1.0 不编译引用 US 类型（缺前置=警告+静默，不报错）；④ legacy 桥类型名重叠期归属；⑤ 报告 §5 双开矩阵纳入两侧发布门。
- [ ] **迁移方案待裁决（Q1–Q10，见迁移方案 §7/§8.5）**：是否插入 SR 0.5 过渡版；是否另开冻结 legacy Workshop 条目；P2 设置导入做不做；公告窗口长度（建议 4–8 周）；1.0 的 US 前置取硬/软；legacy 线是否承诺 1.7 兼容维护；**新增 Q7–Q10（身份归属）**：packageId 应跟内容走（推荐 B：pack-only 继承人继承 `coahuilite.squeakyratkin` 就地升级，legacy 拿新 id 冻结），否则迁移玩家音源选择（PackKey）会被重置；新 pack-only 线包应使用 US 原生 def 类型；legacy 与新线因 defName 冲突不可同装。**已核实的事实基线**：1.6 comp 不进存档（`ThingWithComps.InitializeComps` 按当前 def 重建）⇒ comp 换型/退役对存档零影响；依赖缺失=列表警告不崩档；重复 packageId 被 RimWorld 忽略（`Log.Error`）；重复 defName 会被追加随机后缀（PackKey 失配）。
- [ ] **0.3.x 路线调整（2026-08-22 维护者定案）**：SR 在 0.3.x 期间 **UI 保持不变**；内核升级为 **US 通用状态**，并新开 US 仓库（**已确认 repo=`coahuilite/UniversalSqueaker`、packageId=`coahuilite.universalsqueaker`；命名空间/前缀/日志按推荐组合 `UniversalSqueaker`/`US_`/`usdiag`，Workshop 显示名与许可待最终确认；2026-08-22 维护者指示暂不建仓，创建为外部操作需另行授权**）0.3.x 期间同步开发；SR 0.3.2 身份门控与 XML ABI 固化照常，作者 XML ABI 由 US 继承；原 UI 专项（fallback 编辑器/重建按钮/浏览器下放/Race 域编辑器/Debug 合并/三档缩放）全部转入 US 仓库开发面，不在 SR 落地。
- [ ] **0.4（US+SR 双仓同步上架）**：US 首版与 SR 0.4 同步上架创意工坊；US 暂不作为 SR 依赖（两 mod 独立共存）；**2026-08-22 共存策略定案 = 可同时启用 + 服务域划分**（0.4 期间 SR 独占 Ratkin、US 0.4 不含任何 Ratkin 装配/profile/attachment、只服务其他种族；不写 `<incompatibleWith>`；打包门断言 US 0.4 无 Ratkin 条目；实机双开矩阵验证同一种族只经一方单响）；SR 0.4 **只做 bugfix**，所有共享面修复**同步反馈到 US**；动作门机制（`SqueakActionDef`/`SqueakActionRegistry`/`SqueakCompat`/`allowExternalActions`/`action` 枚举→string）在 US 仓库 0.3.x 窗口落地、随 US 0.4 首版发布；SR 0.4 不引入新机制。**待裁决 Q1（2026-08-23）**：维护者口径「Ratkin 早已是 US 首发支持包之一」与本条「US 0.4 不含任何 Ratkin 装配」冲突；裁决前维持本条，但 US 一旦服务 Ratkin 必须先落地跨程序集 squeak-comp 检测（兼容性报告 §2 F1/F4、§6 Q1）。
- [ ] **SR 1.0.0（US 成为前置）**：SR 下一版本从 0.4 直接推进到 1.0.0，SR 退化为纯音频包（内容仓库）；此时 US 作为 SR 前置依赖存在；1.0.0 = 作者 XML ABI 官方冻结版本；US 在该时点的版本号待定。切换前置时执行 US/SR 设置、保存、Workshop 迁移演练（真实 save modlist）+ 六项拆分门可重复证据。
- [x] 作者包 legacy 桥**离线原型**（已提交 C37）：`tools/LegacyBridgePrototype/` 编译期证明（真实 Verse API：`SqueakyRatkin.SqueakVoicePackDef : UniversalSqueaker.SqueakVoicePackDef` 薄继承 + `DefDatabase<Legacy>.AllDefs` 泛型约束 + canonical 字段形状）+ `tools/LegacyBridgeHarness/` 运行时语义（共享同一份桥源：层级字段填充方向、`SR_` vs `US_` 前缀上下文、AllDefs 枚举 + 单点 upcast 引用一致、拆除面源码隔离）；README 已列实机步骤。**剩余（维护者实机，US 程序集场景）**：真 Verse XML 加载（legacy 节点薄继承实例化 + `DefDatabase<Legacy>.AllDefs` 枚举 + `<sound>` 交叉引用解析）+ Player.log 零红字；通过后再把契约写入 US 仓库。
- [ ] 动作门落地（2026-08-22 重定至 **US 仓库**，0.3.x 并行开发、0.4 随 US 首版发布）：`SqueakActionDef`/`SqueakActionRegistry`/`SqueakCompat`；`SqueakVoicePackAction.action` 枚举→string（节点名不变零迁移）；validator 键解析（内置 known/外部未注册 dormant/非法 error）；**玩家总闸 `allowExternalActions`（设置项默认开，policy 层实现，UI 可见）**；作者指南动作注册章节随 US 0.4 公开；内置 17 键语料回放零 delta 验收；无第三方需求信号则退回封闭（§2.2 放弃信号）；**八道闸门决策出生即纯**（策略纯函数 + 注入函子，路由核心公理 §2.3）。
- [ ] 通用内核路线（2026-08-22 重定）：0.3.x SR 内核升级为 US 通用状态（SR 内验证通过后由新 US 仓库并行承接）；US 首版（0.4）前禁止发布空壳前置或保留双实现；SR 1.0.0 收缩为纯音频包依赖 US。
- [x] 设计 VoicePack 单种族声明与路由 → **并入 0.3.1**（`raceDefName` 必填、域模型、池路由、Scribe 迁移同批落地）
- [x] 设计 XML 驱动的 per-race Vanilla fallback profile → **并入 0.3.1**（内置表正式数据 + `SqueakFallbackProfileStore`，17 action 映射，无 profile 默认静音）
- [x] `SqueakyRatkinSettings` 拆 partial → **并入 0.3.1**（随事务性迁移同窗口；Scribe 契约面存档 fixture）
- [x] `XenotypeUI` 拆出 Race 域编辑器 → **转入 US 仓库开发面**（2026-08-22：SR 0.3.x UI 不变；三档缩放验证随同）
- [x] Debug 入口合并 → **转入 US 仓库开发面**（2026-08-22：SR 0.3.x UI 不变）

## 提案 / 待决策（未定案，需研判）

- [x] **彩蛋语音提案 → 已定案（2026-08-20）**：裁决见决策文档 §2.4——加性池成员（开关开才入池同权混抽，关 = 仅普通条目）、默认关、action 条目粒度、0.3.1 XML ABI 同批落地（2026-08-21 重排）、第三方作者指南 0.4.x 同窗口开放；**同日 YAGNI 修订：取消种族运算符/条件表达式，仅保留彩蛋标签**（种族定位由 pack `raceDefName` 路由承担）；Kiiro 许可门不变（彩蛋内容不豁免）。原待定项①-⑥随裁决关闭。**2026-08-22 修订：`IsEgg` 自 0.3.2 起纳入公开作者 ABI（原「0.4.x 才向第三方作者开放」口径废止）。**
- [x] **Eat 两级选项 → 已定案并实施（2026-08-23 维护者）**：父「仅真正进食」+ 子「使用成瘾品」（方案 B，`ChewIngestible`，toil 名未确认时回落完整 job 级）；规格与验收矩阵见 [`docs/handoff-eat-occurrence-granularity-zh.md`](docs/handoff-eat-occurrence-granularity-zh.md) §5.1/§6/§7.1，实施与实机项见上方 0.3.2 列表。

## 明确延后
- [ ] 仅在 `TicksAbs` 再次复现时调查归因。

## 待重新确认
- [ ] 发布门禁、英文 VoicePack 作者指南和第三方 VoicePack 示例等旧候选方向不再视为已定计划；需要维护者重新确认后才能恢复。
- [ ] **US 仓库剩余身份待定**：已确认 repo=`coahuilite/UniversalSqueaker`、packageId=`coahuilite.universalsqueaker`；待最终确认 Workshop 显示名与许可；命名空间/前缀/日志按推荐组合（`UniversalSqueaker`/`US_`/`usdiag`）。**2026-08-22 暂不建仓**；创建与远端推送属外部操作，另行授权后执行。
