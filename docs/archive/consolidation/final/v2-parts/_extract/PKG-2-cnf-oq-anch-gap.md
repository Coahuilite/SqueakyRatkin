# PKG-2 P0 slices

----- 6. 矛盾与反转（CNF-<n>） L335-347 -----
## 6. 矛盾与反转（CNF-<n>）

| id | 冲突双方（含来源） | 各自时间层 | 冲突点 | 可能解释 [I] | 建议回查点 |
| --- | --- | --- | --- | --- | --- |
| CNF-1 | 派发表要求「C1–C19 证据链」（本包调度消息/README §5 行）vs 冻结语料证据链止于 C14（`src: 检查表 L29–L31`） | 派发=2026-09-17；语料=2026-08-20 落表 | 派发口径含 C15–C19，四份主源无一条目 | I-5：后续链在 PKG-6 主源（handoff-0.3.3）；或派发表数字有误 | [xref: PKG-6（交接与开放裁决）] 是否出现 C15–C19；按源文档为准执行 |
| CNF-2 | 检查表 L31 链中 C1–C10、C12–C14 有号 vs **C11 全文无号**（同节两个未编号提交 `5b51c6a`/`67028b8`） | 2026-08-20 | 编号断档 | I-1（两解均存） | 后续会话/记忆文件中是否曾引用 C11。src: docs/0.3x-release-gate-checklist-zh.md §已锁定证据链 L29–L31 |
| CNF-3 | 语料例数「1622 例」（`src: 等价评审 §1 L12、§5 L56`，随 C4 提交口径）vs「3782 例」（`src: 检查表 L8`、交接 0.3.0 §3 L34/§4 L44，C7 后口径） | 等价评审=08-18~19（C5 时点）；检查表/交接 §3=08-19~20 | 同一文件 `fixtures/corpus/corpus-0.3.0.txt` 两个例数 | 非错误：C4 生成 1622 → C7 fixture 驱动扩至 3782；等价评审 §5 结论未随 C7 改写（文档停留旧层） | 以「C4=1622、C7=3782」双层并记；提炼时勿合并成单一数字 |
| CNF-4 | 交接 0.3.0 §6.2「定案后执行双轨发布（GitHub prerelease → Steam 正式）」vs 决策文档 §5 / 检查表 L23「**不设 GitHub prerelease tag**、本地 dev 包分发」 | 交接=2026-08-19（决策未定时预写）；修订=2026-08-20 同日修订 | 双轨载体是否含 GitHub prerelease | 交接收写「①GitHub prerelease 双轨是否采用」当时确未决；08-20 修订否定 | 发布执行事实以 [xref: PKG-4] 为准。src: docs/handoff-0.3.0-zh.md §6 L59；docs/0.3x-release-gate-checklist-zh.md L23 |
| CNF-5 | 动作门落地窗口：§2.2 主表「机制本体 **0.4.x US 拆分发布时**与消费者同窗口落地」vs L58 修订「机制本体改为在 **US 仓库 0.3.x 并行窗口**开发、0.4 随 US 首版发布」（`src: 决策文档 §2.2 L48–L58`） | 08-18 原文 vs 2026-08-22 修订 | 开发窗口不同（SR 0.4.x vs US 0.3.x），发布时点同为 0.4 | 修订明言「原表述按新路线理解」= 表体未改写、修订附注覆盖；属口径演化非错误 | 与 DEC-12/DEC-13 的 US 落地清单互查 |
| CNF-6 | IsEgg 暴露面：原口径「0.3.x 内为维护者内部标签、第三方作者指南 0.4.x 才开放」vs「自 0.3.2 起纳入公开作者 ABI」（`src: 决策文档 §2.4 L83–L87`） | 08-20 定案 vs 2026-08-22 修订 | 内部标签 vs 公开 ABI | 文档已明文「口径废止」——本条为**有记录的废止**，非未协调矛盾 | 合同现行文本对照 [xref: PKG-1（日志协议/设置合同）] |
| CNF-7 | 交接 0.3.0：L4「本文件**可能未提交**」+ L10「本地领先 `origin/0.3.x` 若干提交**未推送**」vs 检查表 L31「`origin/0.3.x` 已推送同步」「`5b51c6a` handoff」 | 08-19 vs 08-20 | 仓库同步状态相反 | I-3：交接写于推送前，08-20 完成提交+推送；属时间层差异非矛盾，但按规则并存记录 | (冻结动作) 编排方已核 docs 干净（2026-09-17）。src: docs/handoff-0.3.0-zh.md L4、L10；docs/0.3x-release-gate-checklist-zh.md L31 |
| CNF-8 | 热修路线：交接 0.3.0 §6.2「③ 0.3.0.**x** 热修路线确认（未决）」vs 决策文档 §5 L340 定案「`vX.Y.Z-hotfixN`，hotfix **不 bump z**」（`x.y.z` 三节不变） | 08-19 vs 2026-08-20 | 待定项的口头形态「0.3.0.x」（四段号）与最终定案（prerelease 后缀、不动四段号）表述不一致 | [I] 交接口语预写与定案格式差异；定案版为准（同 CNF-4 时间层逻辑） | 下游引用热修格式一律用 `vX.Y.Z-hotfixN`。src: docs/handoff-0.3.0-zh.md §6 L59；docs/0.3x-refactor-architecture-decision-zh.md §5 L340 |


----- 8. 开放问题、阻塞与交接风险（OQ-<n>） L373-388 -----
## 8. 开放问题、阻塞与交接风险（OQ-<n>）

| id | 问题 / 风险 | 类型 | 影响面 | 当前状态 | 来源 |
| --- | --- | --- | --- | --- | --- |
| OQ-1 | 发布执行两项授权未落：merge `0.3.x → dev` 授权 + 渠道发布授权（检查表落表时点仍阻塞） | 需授权 | 0.3.0 能否发布；DEC-9 双轨流程启动 | 本包语料内未解决（U-3）；[xref: PKG-4（发布执行事实）] | src: 检查表 L3、L23 |
| OQ-2 | US 仓库尚未创建（2026-08-22 明示「暂不建仓」，外部操作需另行授权） | 需授权/依赖外部 | DEC-12/DEC-13 全部 US 侧计划 | 未建仓（语料内）；[xref: PKG-3] | src: 决策文档 §5 L376 |
| OQ-3 | US Workshop 显示名与许可「待最终确认」 | 待裁决 | 双开命名面、外部渠道 | 未决 | src: 决策文档 §5 L376 |
| OQ-4 | US 在 SR 1.0.0 时点的版本号待定 | 待裁决 | 前置切换与依赖声明 | 未决 | src: 决策文档 §5 L384 |
| OQ-5 | 0.3.x/0.3.1/0.3.2 与 US 的发布顺序暂不决定（2026-08-22），ABI 用「首个携带 0.3.1 XML ABI 的发布版本」浮动表述 | 待裁决 | 冻结承诺的实际起算版本 | 未决 | src: 决策文档 §5 L369 |
| OQ-6 | 动作门三条放弃信号是持续监控条件（无需求→退回封闭；标准键诉求→转纯约定/封闭；语料非零 delta→撤销键迁移） | 依赖外部/未验证 | DEC-4 路线存活性 | 常设；0.3.x→0.4.x 窗口观察 | src: 决策文档 §2.2 L56 |
| OQ-7 | 6 条 `audio.dispatch.no_sound` 无根因，按「未复现」当事人声明关闭 | 未验证 | B 面绿灯成色；若发布后复现即成 A 面玩家可见缺陷 | 已关闭（本包质疑保留） | src: 检查表 L9 |
| OQ-8 | legacy 桥真 Verse XML 加载/交叉引用解析待维护者实机（US 程序集 + legacy XML 包 + 日志零红字） | 未验证 | SR 1.0.0 前置切换可行性 | 离线完成、实机未做 | src: 决策文档 §5 L385 |
| OQ-9 | 双开共存验证（打包门断言 US 0.4 无 Ratkin 条目 + 实机双开矩阵「同一种族只经一方单响」）尚未到执行窗口 | 未验证（未来门） | 0.4 共存策略成立性 | 计划 | src: 决策文档 §5 L382–L383 |
| OQ-10 | 身份门控实机矩阵余项由维护者判「自然覆盖」；全量矩阵在语料外 TODO.md | 未验证/[C] | 0.3.2 验证门完整性 | 判已覆盖（存疑保留） | src: 决策文档 §5 L365、L371 |
| OQ-11 | C15–C19（若存在）与 C11 缺号：证据链在本包语料外的延伸情况未知 | 待裁决（编排层面） | 提炼阶段合并证据链时需防断链 | 悬置（→GAP-3） | src: 检查表 L31；调度消息派发表 |


----- 9. 锚点（ANCH-<n>：必须逐字保留） L389-405 -----
## 9. 锚点（ANCH-<n>：必须逐字保留）

- **标识符 / 字段名 / 键 / 前缀**：
  - ANCH-1 前缀与协议名：`SR_*`（SoundDef 键前缀，`SR_OfficialExample_Race_*` 实例）、`US_`、`srdiag`/`usdiag`（日志协议）、协议头 `fmt=2`、once key 前缀 `log-v2`、`coexistence=*_active`；repo=`coahuilite/UniversalSqueaker`、packageId=`coahuilite.universalsqueaker` 与 `coahuilite.squeakyratkin`、namespace `UniversalSqueaker.*`。src: 决策文档 §2.2 L50、§5 L376/L383；交接 0.3.0 §1 L9；检查表 L8
  - ANCH-2 内核类型：`RaceKey`/`XenotypeKey`/`AudioDomain`/`AudioDomainStatus{Available,Dormant,TargetUnavailable,Orphan}`/`AudioDomainStatuses.Classify`；`VoicePackEntry`/`ActionSoundSet`/`DomainPool`/`SqueakPoolRegistry`/`SelectionContext{Domain,ActionKey,Age,Production,AllowEggs}`/`SelectionMode{Off,Fallback,Remix}`/`ChainResult{SoundKey,Tier,PoolStableKey,IsEgg}`/`ChainTier{XenotypePack,RacePack,PackFallback,BuiltInFallback}`/`ISoundGate`/`IRollSource`；`ModulationAxis`/`Modulation.ComposeModulation`；`FallbackProfile`/`FallbackDelta`/`CopyDisposition{KeepCopy,RebuildFromSource,MergeDelta}`/`FallbackProfileOperations{DecideCopy,Merge}`/`BuiltInFallbackCatalog`/`BuiltInFallbackTable`；`DomainFilter`（0.4.x 删）/`DomainFilter.Everything`；`ActionKey.For(SqueakAction)`/`TryParseBuiltIn`；`Kernel/BuiltInActionKeys`（17 键唯一权威）。src: 决策文档 §3.1 L124–L128、§4.1 L149–L223
  - ANCH-3 适配层/外围名：`SqueakFallbackProfileStore.cs`、`SqueakLifeStageResolver.cs`、`SqueakRuntimeResolver`、`SqueakXenotypeCatalog`（类名保留）、`SqueakKernelAdapter`（唯一接缝）、`GetTargetCandidates`、`NotifyExternal`、`SqueakCompat.NotifyAction(Pawn, string actionKey)`、`SqueakActionDef`/`SqueakActionRegistry`、`SqueakGlobalActionPolicy`（零下游依赖）、`RecordOutcome`/`ExternalBlocked`、`allowExternalActions`（默认开）、`minIntervalTicks`≥60、`GetScope(string key)`、`SqueakSoundAvailabilityCache`、`SqueakTriggerInvocation`/`TriggerInvocationRules`/`SqueakActionPlan`、`PlayerSelection`/`ActiveCommand`/`IsPlayerInitiated`/`RequiresResponsivePawn`、`Pawn.IsPlayerControlled`、`!Downed && Awake()`、`Selector.Select(playSound:false)`（含 `__1` 位置注入过滤）、`MentalStateHandler.TryStartMentalState`（成功 postfix）、`MentalFitDef` 反向映射、`MentalState_BabyCry`/`MentalState_BabyGiggle`、`MentalBreakWorker.TryStart`（保持不动）、`LegacyVoicePackSource`、`Legacy/`、`SqueakVoicePackDef` 字段面 `raceDefName`/`scope`/`targetDefName`/`weight`/`fallbacks`/`actions(action,ageTag,IsEgg,sounds)`、`ageTag`（`AgeBucket?` field-presence）、`AgeBucket{Baby,Toddler,Child,Adult}`、`XmlToObjectUtils.SearchTypeHierarchy`、`Config 文件 SqueakyRatkin_Profile_<race>.xml`、`XenotypeMoodOverride` 模式、`migrationPersistencePending`、`SettingsOrigin{FreshCreated,LoadedFromFile}`。src: 决策文档 §2.2 L48–L62、§2.4、§3.1 L130–L135、§4.1、§5 L352–L385、§6 L393–L395、§6.3
  - ANCH-4 事件名（v1 28 事件集内的相关项 + v2 明细字段串）：`audio.dispatch.ok`、`audio.route.selected`、`trigger.outcome.summary`、`trigger.attempt.failed`、`audio.dispatch.no_sound`、`pack.race_defaulted`、`mod.start.identity`、`mod.start.ready`、`rebuild_failed`、`refresh_failed`、`PackRejected`、`TargetRejected`、`hook.*unavailable`、`devtools.camera_indicator.changed`；v2 明细字段序 `sound tier egg suppressed_detail pawn pawn_id pawn_faction pawn_ctrl`；人类句 `Audio route: <action> -> <sound> (<tier>[, egg][, nonplayer]).`；v2 记「开关状态/切换事件」。src: 检查表 L8–L20（转述计数不复制日志行）；决策文档 §5 L367、§2.2 L53
  - ANCH-5 工具/文件/fixture：`Source/SqueakyRatkin/Kernel/`（Domain.cs/Pool.cs/Modulation.cs/FallbackProfile.cs/DomainFilter.cs）、`tools/KernelCharacterization/`（Scenarios.cs **冻结不得改动**）、`tools/SqueakLogCharacterization/`、`tools/SettingsFixtureGenerator/`、`fixtures/corpus/corpus-0.3.0.txt`、`fixtures/input/`、`fixtures/expected/`、`tools/LegacyBridgePrototype/`、`tools/LegacyBridgeHarness/`、`dist/SqueakyRatkinEggTestVoices/`（gitignored）、`build-dev.ps1`、`release.yml`、`pack-github`/`pack-steam`/`stage-package`、`verify-local` 第 10 项 XML ABI 一致性锁、`.github/skills/squeaky-voicepack-authoring/SKILL.md`（唯一正文）、`scripts/new-voicepack.ps1`、`SqueakActionDomain.cs`/`SqueakVoicePackDomain.cs`。src: 决策文档 §5 L366；交接 0.3.0 §2 L27、§3 L31–L37；检查表 L27
- **数值 / 版本 / 阈值 / 计数**：
  - ANCH-6 ABI/协议：`SqueakAction` 17 值（序数 0–16；Crying=15、Giggling=16、0–14 不动；append-only）；内置 fallback 表 Ratkin 固定 15 键（Call…MentalBreak，不列 Crying/Giggling）；srdiag v1 = 28 事件/字段序/once key 冻结；`settingsSchemaVersion` 3→4、`voicePackSchemaVersion` 1→2（0.3.0 不 bump：现值 3/1）；五处同步 = 枚举/XML/SoundDef/本地化/统计；八道闸门链；四层链（XenotypePack→RacePack→PackFallback→BuiltInFallback）；`ChainResult`→诊断折叠 PackFallback→RacePack、BuiltInFallback→Vanilla；4 处 `Ratkin` 字面量临时层；UI partial ×3；整文件删除 0 个、符号内嵌删除约 150 行；`SqueakActionModel.cs` Count 15→17。src: 决策文档 §2.2 L62、§3.1 L135–L137、§4.6–§4.7、§5 L352、L358
  - ANCH-7 证据数字：语料 1622 例（C4）→ 3782 例（C7）；断言 41→43；fixture 9 场景（编号面 `S1-S5` + `F03-F07`，映射 U-6）；14 行对照；换面 4 项/不换面 9+1 组/dev 豁免 3 名 + 七次点击解锁面；等价验证「唯一豁免 = dev 隐藏功能」；观察窗 2 周起；350 ms ModSettings 队列；dev 包 116 文件；实机计数 87 派发、count=37、summary 13、dispatched 峰值 52、no_sound 6、egg 5/11、player 10/nonplayer 26；约 500 行内核 + 约 200 行 harness；约 +1500/−600（方案 C diff）。src: 等价评审 §2–§3；检查表 L8–L27；决策文档 §1 L10、§2 L32
  - ANCH-8 提交与版本标识（逐字）：C1 `41ca953` fixture 9 场景；C2 `efc08a0` 枚举提取+ActionKey；C3 `e48d38f` Kernel 骨架；C4 `fb9ab50` 41 断言+语料 1622；C5 `ea5fd2c` 等价评审；C6 `0893cd0` resolver 接内核+旧路径删除；C7 `7e2c3f5` 语料 3782；C8 `5c86315` 检查表（离线面绿）；C9 `1b42a04` 版本 bump 0.3.0；C10 `e6a1ed7` zip 打包脚本化；（未编号）`5b51c6a` handoff、`67028b8` MEMORY/TODO 同步（八面全绿）；C12 `0b0fa48` 错误路径修复；C13 `402d4be` Steam 打包纪律；C14 `2e5fc74` 决策文档/检查表全绿；留档 dev@`8bd383d`（`0.2.4-FINAL` 分支）；dev 包串 `SqueakyRatkin-dev-v0.3.0-67028b8-dirty.zip`、构建标识 0.3.0+`67028b88`…（截断形态照源文）；zip SHA256 `670f8e2baced79c977f86a550ca9241e0175f97520dee237db673316a91ffe9d`；dll SHA256 `af5e2a182f5db37e365eb58607e00400a4aaa429802a4e1b6b6201212cc4c71b`；版本线 0.2.4→0.3.0→0.3.1→0.3.2→0.3.3（PKG-6 管辖）→0.4→1.0.0；热修形态 `vX.Y.Z-hotfixN`。src: 检查表 L19、L27、L31；交接 0.3.0 §4 L44–L46；决策文档 §5 L340、L384
- **授权边界与外部状态边界**：
  - ANCH-9 授权边界：提交/推送/发布需显式授权；推送前隐私审查完整可达范围；US 仓库创建 = 外部操作需另行授权；`0.2.4-FINAL` 留档分支；Kiiro 彩蛋内容「实验分支不发布、未经许可不宣传」；措辞维护边界「不提 US 名称/多种族/Kiiro/通用化」；Workshop 公告义务（旧「预计 0.3.0 失效」随正式发布同步替换、中英双预览+字符数重算）。src: 交接 0.3.0 §7 L68；决策文档 §5 L342、L348、L376；§2.4 L87
  - ANCH-10 外部状态边界：Steam Workshop 自动更新 = 发布即全员无灰度；RimWorld 无商店→无人工审核（治理 = 技术校验+玩家代理+文档礼仪）；HAR 反射非依赖（缺失静默降级）；No-DLC 基线；Biotech 门 `ModsConfig.BiotechActive`；Harmony 版本面；ProductDomainFilter 试验名单 `{Ratkin, Kiiro, Miho}`（默认 `{Ratkin}`、隐藏开关、替换非叠加、UI 不渲染、release 默认 off、0.4.x 整类删除）；卸载安全 = 存档零写入；`base.WriteSettings()` 单通道红线只约束 ModSettings；GitHub CI 只为 GitHub 包服务、dev/steam 打包为本地纪律；`0.3.x` 分支自 dev 切出隔离 0.2 线。src: 决策文档 §5 L325、§2.2 L44、§4.4 L256、§4.6 L265、§5 L338；交接 0.3.0 §1 L10、§7 L65
- **术语口径（逐字）**：「定型」= 0.x 内部冻结（0.4.x US 发布前可修正，代价 = 内部返工）；「冻结」= 已发布契约（修正须版本化兼容）；ABI/兼容面 = 五类持久化/共享格式（①Scribe 设置格式 ②srdiag 日志协议 ③VoicePack XML 包 schema ④枚举序列化值〔`SqueakAction` 序数〕⑤Def 键〔`SR_*` SoundDef 与 PackKey〕）。src: 决策文档 §1 L12


----- 11. 证据缺口（GAP-<n>） L417-426 -----
## 11. 证据缺口（GAP-<n>）

| id | 想回答的问题 | 为什么现有语料答不了 | 需要什么才能回答 |
| --- | --- | --- | --- |
| GAP-1 | 四份并行架构草稿各自全文与原始论据？ | 决策文档 §2 只留比拼表（核心思想/强项/弱点/裁决），草稿本体不在 `docs/**` | 草稿存档或撰写会话记录（语料外）。src: docs/0.3x-refactor-architecture-decision-zh.md §文首 L4、§2 L26–L33 |
| GAP-2 | 0.3.1/0.3.2 是否按计划实施、各验证门实际过门记录？ | 本包主源止于计划文本（决策文档 §5）与 0.3.0 门槛（检查表）；实施记录在后续 handoff | [xref: PKG-6（handoff-0.3.3 含 2026-09-13 接续）] + 发布评审 [xref: PKG-4]。src: 决策文档 §5 L350–L371；docs/0.3x-release-gate-checklist-zh.md L3 |
| GAP-3 | C15–C19 是否存在、内容为何？C11 是并号还是弃号？ | 冻结语料证据链止于 C14（含两个未编号提交）。src: docs/0.3x-release-gate-checklist-zh.md §已锁定证据链 L29–L31 | 后续会话文档（PKG-6）或仓库 log（本管道禁止外部取证） |
| GAP-4 | 「fixture 9 场景」与 `S1-S5`+`F03-F07` 编号体系如何对齐（含损坏修复场景编号）？ | 语料只分别给出「9 场景」枚举与编号引用，无对照表。src: 决策文档 §6.2 L400；docs/0.3x-release-gate-checklist-zh.md L8、L10；docs/0.3x-equivalence-review-zh.md §3 L40 | 语料文件头注释或 Scenarios.cs 文本（代码面在语料外） |
| GAP-5 | 静默回归 INC-1 的确切引入提交与暴露面（哪些玩家路径会静音）？ | 检查表只记修复不记引入点。src: docs/0.3x-release-gate-checklist-zh.md L25–L27 | `git log` 取证（本包禁止）或后续事故复盘文档 |

