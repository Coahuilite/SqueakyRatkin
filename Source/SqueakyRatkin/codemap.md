# Source/SqueakyRatkin/

## Responsibility

本目录是 mod 的**运行时核心与单一装配点**：负责 Mod 生命周期（Harmony 装配、设置窗口、保存队列）、每 pawn 的发声触发与音频调度（`CompSqueaker`）、不可变运行时快照的发布（resolver/catalog/policy）、周期人口缩放，以及全部持久化数据模型（settings、voice-pack、xenotype preset）。子目录只做外围职责，见 [Integration](#integration) 与各自地图：

- `Patches/` — Harmony 事件转译层（见 [Patches/codemap.md](Patches/codemap.md)，已完成）
- `UI/` — 设置工作台 composition layer（见 [UI/codemap.md](UI/codemap.md)，已完成）
- `Debug/` — 诊断 overlay / 统计 / 音频浏览（[Debug/codemap.md](Debug/codemap.md)，见下）
- `Logging/` — 结构化日志（[Logging/codemap.md](Logging/codemap.md)，见下）

## Key Files / Symbols

**生命周期与持久化协调**
- `Mod.cs` — `SqueakyRatkinMod`（唯一 Mod 入口）：ctor 设 `Instance`、`Harmony = new(PackageId)`、`GetSettings<SqueakyRatkinSettings>()`、`Harmony.PatchAll()`；`ExecuteWhenFinished` 启动链；`SettingsCategory`/`DoSettingsWindowContents`/`WriteSettings` override；保存队列 `QueueSettingsSave`/`FlushQueuedSettingsSave`/`PersistSettingsNow`（350 ms 防抖 + generation/failure 重试）；`OpenSettings(bool selectXenotypeTab)`/`IsOwnedSettingsWindow`/`NotifySettingsWindowClosing` 与 `settingsWindows` 注册表；计数器 `SaveQueueRequestCount`/`PhysicalSaveCount`。
- `SqueakyRatkinSettings.cs`（partial，1314 行）— 全部持久化字段：`voicePackMode`/`voicePackSchemaVersion`/`settingsSchemaVersion`、`scaleCooldownWithTimeSpeed`/`scaleFrequencyWithTalking`/`scalePeriodicWithAudiblePopulation`、`localizeDebugActions`/`developerToolsEnabled`/`devLoggingMode`、`globalCooldownMultiplier`、`distancePreset`/`distanceRange`、`moodOverrides`、`voicePackSelections`、`xenotypePresets`、`globalActionEnabled`；`ExposeData`（含 PostLoadInit 迁移：schema 版本、`GlobalActionEnabledRecord` scope 规范化、`SqueakLog.Configure`）；运行时桥 `ApplyToRuntime`/`NotifyCheapRuntimeChanged`/`NotifyDistanceRuntimeChanged`/`NotifyGlobalMoodRuntimeChanged`/`NotifyContinuousXenotypeRuntimeChanged`/`NotifyDiscreteResolverRuntimeChanged`/`QueuePersistence`；静态配置发现 `ConfiguredMoods`/`ConfiguredActions`/`ConfiguredSqueakers()`（从 DefDatabase 的 `CompProperties_Squeaker` 懒加载）；设置 UI shell（`SettingsTab` 枚举、`DrawSettings`、`BeginSettingsSession`/`EndSettingsSession`、七击解锁、Basics 页 + 全部 `Apply*AndQueue` 写入桥）。
- `SqueakSettingsGameContext.cs` — 每帧一次安全采样 `Capture()`/`CaptureRuntime()`（Playing + Root_Play + TickManager + CurrentMap + MapUI 闸门后才碰 Selector）；`IsPawnOnCurrentMap`/`TryGetSelectedSqueaker`。**菜单 UI 禁止直接取 Find 服务，一律经此结构。**

**运行时快照发布（单发布者、不可变）**
- `SqueakRuntimeResolver.cs` — 静态单例快照发布器：`InitializeMainThread()`（主线程绑定，唯一发布者）、`NotifyContinuousResolverChange`（75 ms trailing / 150 ms bounded 防抖）、`NotifyDiscreteResolverChange`（返回前生效）、`TickPendingRuntimeChanges`/`FlushPendingRuntimeChanges(force)`、`TryPublish`→`BuildSnapshot`；只读面 `SqueakRuntimeSnapshot`（`ResolveContext(pawn)` 按 xenotype defName 查找、`ChooseNativeSound`/`ChooseProductionSound`、`KnownMapSoundDefs`、`VoicePackMode`）、`ResolvedSqueakContext`、`ResolvedAudioPack`、`RuntimeActionDelta`/`RuntimeMoodDelta`、`SqueakSoundChoice`（`Or` 链式回退）。计数器 `ResolverRebuildCount`/`RuntimeFlushCount`。
- `SqueakGlobalActionPolicy.cs` — 独立于 resolver 的 volatile 全局作用域门：`Current`/`Publish(settings)`/`GetScope(action)`；`CreateDefaults()` 用定义默认值兜底。注释红线：**无 resolver/catalog/pawn/audio 依赖**，预览路径不查它。
- `SqueakXenotypeCatalog.cs` — volatile 目录快照：`Refresh()`（收集合法且 pack key 唯一的 `SqueakVoicePackDef` → canonical XenotypeDef（重复 defName 进 `AmbiguousCanonicalDefNames`，运行时 fail-closed）→ HAR hints）；`SqueakXenotypeCatalogSnapshot`（`XenotypeByDefName`/`PackByKey`/`RacePacks`/`XenotypePacksByDefName`/`GetTargetCandidates`/`GetVoicePackDomainPacks`）。Race 域与 Biotech 步骤完全独立。
- `HarRatkinXenotypeDiscovery.cs` — 可选 HAR 适配器：全反射（HAR 非构建依赖），Biotech + `ThingDef "Ratkin"` 存在才探测；仅产 UI 提示，**不是** VoicePack 资格门。

**触发与播放（每 pawn）**
- `CompSqueaker.cs`（1097 行）— 核心 `ThingComp`：`CompTick`（周期采样）、`Notify_Wounded/Select/Death/Draft/Attack/Equip/MentalBreak`（外部事件）、`NotifyPeriodicDespawn`、`PostSpawnSetup`（成员注册 + 周期启动锚点 `startupAnchorTick`）、`TryTrigger`（完整触发管线）、`PlayOneShot`、`PreviewFinal`（预览专用，无门/冷却/状态，`Rand.PushState/PopState`）、`GetDiagnosticSnapshot`/`ResetDiagnosticState`、`SqueakTimingModel`（纯时序求值）、静态运行时字段 `ScaleCooldownWithTimeSpeed`/`ScaleFrequencyWithTalking`/`ScalePeriodicWithAudiblePopulation`/`GlobalCooldownMultiplier`/`DiagnosticsEnabled`/`activeDistanceRange`、`ApplyDistanceRange`（运行时改写 `subSound.distRange`）；同文件还定义枚举 `SqueakMood`（Good/Neutral/Bad/Break）、`SqueakAction`（15 值，序列化稳定，append-only）、`SqueakCooldownClock`、`SqueakTriggerMode`、`SqueakActionConfig`/`SqueakDistancePresetConfig`/`CompProperties_Squeaker`（XML 默认层：`globalMinIntervalTicks=216`、`scaleFrequencyWithTalking=true`、actions/moodMods/distancePresets）、诊断快照结构。
- `SqueakActionModel.cs` — `SqueakActionDefinitions`（15 个内置动作的唯一元数据源：`DisplayKey`/`AudioKey`/`VocalGatePolicy`/`SupportedScopes`/`DefaultScope`）、`SqueakActionPlan`（Configured/Mode/MinIntervalTicks/ProbabilityPerCheck/CooldownClock，`Unconfigured` 默认 300 tick/0.02）、`SqueakTriggerInvocation`（Origin/Source；非周期跳过 RandomOneShot 概率）、`SqueakVocalGatePolicy`/`SqueakActionScope`/`SqueakActionScopeSupport`。
- `SqueakVocalCapability.cs` — 纯采样值：`VocalOrganEfficiency`/`TalkingChance`、`Decide()` → `SqueakVocalGateDecision`（阈值 0.001 / 0.999）。无运行时域依赖。
- `SqueakPeriodicPopulation.cs` — 静态按 Map 成员表 + 共享可听快照：`Register`/`Unregister`/`RemoveMap`/`NotifyDistanceChanged`/`Maintain`（**每游戏 tick 最多一次 O(N) 扫描**，30 tick 移动刷新）/`GetSnapshot`（纯读，无副作用）；`Snapshot`（CandidateCount/AudibleCount/View/Listener/Scale= max(1, audible/10)/Stale）。`EnsureOwner()` 以 `Current.Game` 隔离会话。
- `SqueakSoundAvailability.cs` — 需求驱动、已加载资源只读检查：`SqueakSoundAvailabilityCache`（`Resolve`/`PeekState`/`TryGetCached`/`Clear`，按 SoundDef 缓存已加载 clip；不枚举文件不复制数据）、`GetNativePlayability`/`GetProductionPlayability`、`TryCreateProductionInfo`/`TryCreateNativeInfo`/`TryCreateNeutralInfo`（生产播放比原生预览更严）、`SqueakResolvedClip`/`SqueakSoundAvailability`、`SqueakSoundPlayability`/`SqueakSoundContextKind`/`SqueakSoundAvailabilityState`。

**数据模型 / 杂项**
- `SqueakVoicePackModels.cs` — `SqueakVoicePackScope`（Unspecified/Race/Xenotype）、`SqueakVoicePackMode`（Off/Fallback/Remix）、`SqueakVoicePackDef`（一个 Def = 一个域一个可选包，只带音频不带行为/mood）、`SqueakVoicePackAction`、`SqueakVoicePackValidator`（纯生产音频契约，Def 校验与候选准入共用）、`VoicePackSelectionRecord`（canonical last-wins 持久化选择，`ComposeDomainKey(scope, target)` 域键）、`SqueakVoicePackDomainStatus`。
- `SqueakXenotypePresetModels.cs` — 持久化行为/mood 覆盖：`XenotypePresetRecord`、`XenotypeMoodOverride`/`XenotypeActionBehaviorOverride`（field-presence 标志区分继承与合法零值）、`GlobalActionEnabledRecord`（legacy bool 迁移）。
- `SqueakAudioPoolNotificationService.cs` — 进程内一次性缺包通知（`Dialog_SqueakyCompactMessageBox` 传输，后端状态供未来 UI）。
- `SqueakLabels.cs` — 本地化 helper（`SR.Action.*`/`SR.Mood.*`/`SR.SettingsCategory`）。
- `SqueakyRatkin.csproj` — net472/x64、`Krafs.Rimworld.Ref 1.6.*`、`Lib.Harmony 2.4.*`（ExcludeAssets=runtime）、输出 `..\..\1.6\Assemblies`、`SQUEAKY_$(BuildFlavor)` 常量（Dev/Steam/Github，控制 `BuildIdentity`/日志细节）。

## Design

- **三层配置叠合**：`CompProperties_Squeaker`（XML 分发默认）← `SqueakyRatkinSettings`（玩家 ModSettings override，按 mood/action 字段级覆盖）← 运行时发布（resolver/policy/静态字段）。每动作只需 1 个 SoundDef + 中性音频，心情经 pitch/volume/jitter 调制（`ResolveMoodMod`：override > XML 默认 > 内置默认）。
- **不可变快照 + volatile 发布**：resolver/catalog 各自持一个不可变快照，`Volatile.Read/Write` 发布，主线程单发布者（`EnsureMainThread` assert）；消费者每 tick 读 `Current`，从不原地改。连续编辑走防抖（75/150 ms），离散编辑（mode/selection/scope/catalog）立即 flush。
- **门禁分层**：全局作用域（`SqueakGlobalActionPolicy`，无需任何下游依赖、最先判定）→ xenotype `RuntimeActionDelta`（enabled/scope/interval/probability multiplier）→ 周期人口缩放 → `SqueakTimingModel`（per-action 间隔 × master × overall × action 倍率、时间倍速缩放、GameTicks/Realtime 冷却钟、全局冷却、出生锚定的启动相位）→ RandomOneShot 概率（外部触发跳过）→ vocal 门（发声器官效率 + Talking capacity）→ 播放。每条失败路径都 `RecordOutcome` + 统计埋点。
- **音源分层选择**：`ChooseProductionSound` — Off → 仅 vanilla `SR_*`；Fallback/Remix → xenotype packs → race packs → vanilla，每层内 `HasPlayable` 过滤（`SqueakSoundAvailabilityCache` playability）后随机选 pack → 随机 sound。`SqueakSoundChoice.Or` 链式回退；`PoolStableKey` 供池稳定标识。
- **周期人口缩放**：`SqueakPeriodicPopulation` 按 Map 持有成员（`CompSqueaker.PostSpawnSetup/PostDestroy/DeSpawn patch/SynchronizePeriodicMembership` 维护），每 tick 一次共享 `Maintain`（视口 + 监听者 + 距离变化才 `Rebuild`），`Scale = max(1, audible/10)` 反作用于周期性间隔与概率（`ScalePeriodicWithAudiblePopulation` 可关）。距离区间经 `ApplyDistanceRange` 运行时改写已知 SoundDef 的 `subSound.distRange`（非 onCamera）。
- **保存协调**：所有磁盘写入只走 `base.WriteSettings()`（`WriteSettings` override 防递归）；UI 每次业务写入 → `QueuePersistence()` → Mod 层 generation + 350 ms 防抖合并；失败保留 dirty generation，关闭窗口或显式重试才重写；`Saving/Saved/Failed` 状态供 footer 显示。
- **兼容边界**：HAR 全反射（非依赖）；Biotech 缺失时 xenotype 路径降级 dormant、不碰 `XenotypeDef`/genes；xenotype 身份 = DefName（Canonical 仅展示）；序列化枚举（`SqueakAction`/`SqueakMood`）值稳定、append-only；`SqueakVoicePackMode` 独立版本化。

## Data & Control Flow

**启动**（`Mod.cs` ctor 可能在 LongEvent 工作线程）：
`ctor` → `GetSettings` + `PatchAll` → `ExecuteWhenFinished`（首个 Unity 主线程点）→ `SqueakRuntimeResolver.InitializeMainThread()` → `SqueakXenotypeCatalog.Refresh()` → `Settings.ApplyToRuntime()`（先 `SqueakGlobalActionPolicy.Publish` 再 resolver 重建）→ `QueuePendingMigrationPersistence` + 强制 flush → `ApplySettingsRuntimeSideEffects` → `SqueakAudioPoolNotificationService.EvaluateAndMaybeShow` → `SqueakLog.StartupReady`。注：ctor 本身不得消费迁移、读 Unity Time、初始化 resolver/UI。

**触发 → 播放**（生产路径）：
```
CompTick（周期采样 CurrentAction 状态机）──┐
Patches Notify_*（wounded/select/death/draft/…）──┤→ TryTrigger
  闸门: spawned + MapHeld==CurrentMap + 视口ExpandedBy(10) + plan.Configured
  → SqueakGlobalActionPolicy.GetScope（Disabled/ActiveCommand 非激活 → 静默返回）
  → GetRuntimeContext（快照 + 按 pawn xenotype 缓存 ResolvedSqueakContext）
  → RuntimeActionDelta.Enabled/Scope
  → SqueakPeriodicPopulation.Maintain/GetSnapshot → periodicScale
  → SqueakTimingModel（ActionReady/GlobalReady + 启动相位）
  → RandomOneShot 概率（仅周期性）
  → vocal 门（RequiresTalkingRoll → Decide）
  → PlayOneShot: ChooseProductionSound → ResolveMoodMod → TryCreateProductionInfo
    → def.PlayOneShot(info) → SqueakDebug.NotifySqueak（mote）
  → ConsumeAttemptCooldowns + RecordOutcome（每步失败都有明确 outcome + SqueakActionStatistics 埋点）
```
**设置变更 → 运行时**：
UI 控件 → canonical 写入（`SetActionGlobalScope`/`SetVoicePackSelection`/`XenotypePresetDraft.Commit` 等）→ 便宜值走 `NotifyCheapRuntimeChanged`/`NotifyDistanceRuntimeChanged`（静态字段，不重建）→ 动作作用域走 `SqueakGlobalActionPolicy.Publish` + discrete；xenotype 编辑走 continuous 防抖；mode/selection 走 discrete → `QueuePersistence()` → Mod 层防抖 → `base.WriteSettings()`。关闭窗口（`Patch_SettingsWindowClose` → `FlushPendingRuntimeChanges(true)` + `NotifySettingsWindowClosing` → `EndSettingsSession` + `FlushQueuedSettingsSave(true,true)`）。

**Resolver 重建**：
`BuildSnapshot`：全局 `RuntimeActionDelta`（settings）→ per-xenotype `RuntimeBuilder`（`xenotypePresets`）→ 选择集（`voicePackSelections` → pack keys）→ 与 catalog packs 合成为 `ResolvedAudioPack` 列表 → vanilla `SR_*` SoundDef 查表 → 一次发布不可变 `SqueakRuntimeSnapshot`。发布失败 → `BuildFallback`（`SqueakRuntimeSnapshot.GlobalOnly` 兜底）+ `SqueakLog.ResolverRebuildFailed`。

**目录刷新**：DefDatabase 收集 `SqueakVoicePackDef` → validator + 唯一 pack key（重复 → `PackRejected` 日志剔除）→ Race 域先发布 → Biotech 时 canonical XenotypeDef（歧义 defName 移入 fail-closed 集合）+ HAR 反射 hints → 按 targetDefName 分组 → volatile 发布快照；异常 → `CatalogRefreshFailed` + `Empty` 快照。

## Integration

- **→ Patches/**（[Patches/codemap.md](Patches/codemap.md)）：事件 patch 全部扇入 `CompSqueaker.Notify_*`/`NotifyPeriodicDespawn`（wounded/attack/death/select/draft/equip/mentalBreak）、`SqueakPeriodicPopulation.RemoveMap`（地图销毁）、`SqueakRuntimeResolver.FlushPendingRuntimeChanges` 与 `SqueakyRatkinMod.NotifySettingsWindowClosing`（设置关闭）、`SqueakDiagnosticsOverlay` 生命周期；`Patch_ModMetaData_LocalizedMetadata` 读 `SqueakyRatkinMod.PackageId`。装配点在本目录 `Mod.cs`。
- **← UI/**（[UI/codemap.md](UI/codemap.md)）：`DoSettingsWindowContents` → `Settings.DrawSettings`（每帧 capture context + `TickPendingRuntimeChanges`）；XenotypeUI 读 `SqueakXenotypeCatalog.Current`/`GetTargetCandidates`、写 `SetVoicePackSelection`/`ForgetVoicePackSelection`/`ForgetXenotypeTarget`；SoundMoodUI 用 `CompSqueaker.PreviewFinal(action, mood, gameContext)`（仅经 `CurrentDrawContext`）；DiagnosticsUI 驱动 `SqueakActionStatistics`/`SqueakAudioPathDiagnostics`。UI 只做意图，业务写入/保存协调所有权在本目录。
- **← Debug/**（[Debug/codemap.md](Debug/codemap.md)）：`SqueakActionStatistics.Enter/Probability/Outcome/Disabled/ScopeRejected` 在 `TryTrigger` 内埋点；`SqueakDiagnosticsOverlay` 经 `CompSqueaker.GetDiagnosticSnapshot`/`ResetDiagnosticState`/`DiagnosticsEnabled` 与 `MaintainPeriodicPopulationDiagnostics`；`SqueakDebug.NotifySqueak`（mote）由 `PlayOneShot` 调度成功后调用；`SqueakAudioBrowser` 走预览/可用性 API。诊断为运行时只读，不消耗 Rand、不改触发状态。
- **← Logging/**（[Logging/codemap.md](Logging/codemap.md)）：核心所有失败/边界事件经 `SqueakLog` 枚举化入口（`CatalogRefreshFailed`/`PackRejected`/`ResolverRebuildFailed`/`TargetRejected`/`XenotypeDiscovery*`/`TriggerAttemptFailed`/`AudioNoSound`/`AudioDispatchFailed`/`TriggerOutcomeSummary`/`StartupIdentity`/`StartupReady` 等）；`SqueakyRatkinSettings` 的 `devLoggingMode` 经 `SqueakLog.Configure`/`EffectiveDevLogging`/`ShouldEmitDev` 控制详细度，`ResetSession` 由设置变更触发。
- **→ 1.6/ 资源**（`1.6/codemap.md`）：SoundDefs（`SR_*` 键由 `SqueakActionDefinitions.AudioKey` 引用）、MoteDefs、Patches（ThingDef 上挂 `CompSqueaker`）——核心对 Def 名/键的契约入口。

## Change Guidance

- **新增动作**：① `SqueakAction` 枚举 append-only（序列化值不可重排）；② `SqueakActionDefinitions`（`Count`、`DisplayKey`、`AudioKey`=SoundDef 名、`VocalGatePolicy`、`SupportedScopes`、`DefaultScope`）——两处不同步会越界/静默错配；③ XML 侧在 `CompProperties_Squeaker.actions` 配 `SqueakActionConfig`。外部触发动作还要在 `SqueakTriggerOrigin`/`InvocationSource` 与 patch 层同步。
- **新设置**：Scribe 字段 + PostLoadInit 迁移/默认 + 对应 `Notify*RuntimeChanged` + `QueuePersistence()`；写入桥放在 `SqueakyRatkinSettings` canonical 方法，UI 只调用不实现。schema 变更必须 bump `settingsSchemaVersion` 并走既有迁移标记（`migrationPersistencePending`）。
- **Resolver 红线**：所有发布/变更必须发生在主线程（先 `InitializeMainThread`）；快照不可变；连续编辑一律走 `NotifyContinuousResolverChange`（防抖），离散语义（mode/selection/scope/catalog 刷新）走 discrete；resolver 永不写设置。
- **持久化**：磁盘写入只经 `base.WriteSettings()`；不要绕过 Mod 层 generation/防抖；失败 generation 语义（`failedSaveGeneration`/`closeRetryGeneration`）不要简化。
- **触发管线**：新触发源走 `CompSqueaker` 的 `NotifyExternal`/`CompTick` 闸门（spawned + CurrentMap + 视口 + plan.Configured），不要复制闸门到 patch；失败路径必须 `RecordOutcome` + 统计埋点，禁止裸 return。
- **菜单 UI**：任何 `Find.*`/`Map`/`Selector` 访问必须经 `SqueakSettingsGameContext` 采样帧，不得在 measure/draw 中新建游戏服务依赖。
- **运行时 Def 改写**：`ApplyDistanceRange` 只改已加载 SoundDef 的 `distRange`（运行时），不写 DefDatabase、不持久化；新音频必须进 `KnownMapSoundDefs` 或 `SoundCacheMixed` 才能被距离应用覆盖。
- **周期成员**：`Register`/`Unregister`/`RemoveMap` 只由 comp 生命周期与 `Patches/` 维护；`Maintain` 每 tick 一次，`GetSnapshot` 必须保持纯读（stale 标记不算副作用）。
- **兼容边界**：xenotype 身份永远用 DefName；HAR 交互保持反射；Biotech 缺失路径不得访问 `XenotypeDef`/genes；`SqueakVoicePackMode` 独立版本化，不要与行为设置耦合迁移。
