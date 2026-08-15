# Source/SqueakyRatkin/UI/

## Responsibility

设置工作台的 UI composition layer：把 `SqueakyRatkinSettings` 的四个页面、局部编辑缓冲、诊断面板和音频浏览入口组合到 RimWorld 的 `Dialog_Options` 外壳中。这里负责 measure → arrange → draw、输入命中区域、页面/折叠/滚动状态及用户意图；业务设置写入、runtime resolver、catalog 与保存协调仍由设置核心/Mod 层拥有。

## Key Files/Symbols

- [SqueakyRatkinSettings.cs](../SqueakyRatkinSettings.cs)：顶层 `DrawSettings(Rect)`、`SettingsTab`、页面导航/footer、Basics 页，以及 `Apply*AndQueue` 写入桥接、`BeginSettingsSession`/`EndSettingsSession`、七击解锁计数。
- [SqueakyRatkinSettings.SoundMoodUI.cs](SqueakyRatkinSettings.SoundMoodUI.cs)：`DrawSoundMoodSettings`、`CommitMoodEditorNow`、mood/action 选择、即时音色编辑与 material/final preview。
- [SqueakyRatkinSettings.XenotypeUI.cs](SqueakyRatkinSettings.XenotypeUI.cs)：voice-pack mode、race/xenotype target list、catalog 状态、master/detail 与 narrow list/editor、Remix 双确认、orphan/unavailable 处理。
- [XenotypePresetDraft.cs](XenotypePresetDraft.cs)：单个 `xenotypeDefName` 的 UI-only field-preserving draft；`Dirty`/`Revision` 由 `MarkChanged` 推进，`Commit` 重建该目标的 canonical preset。
- [SqueakyRatkinSettings.DiagnosticsUI.cs](SqueakyRatkinSettings.DiagnosticsUI.cs)：解锁后的 `DrawDeveloperSettings`，action statistics、audio-path diagnostics、logging cards、audio browser 与 build identity。
- [SqueakySettingsUI.cs](SqueakySettingsUI.cs)：共享 IMGUI primitives（`Button`、`Toggle`、`SelectableCard`、`SettingSelector`、`SearchField`、`FilterChip`、`HelpToggle`、`StatusPanel`、`EmptyState`）及 `Dialog_SqueakyCompactMessageBox`。
- [../SqueakSettingsGameContext.cs](../SqueakSettingsGameContext.cs)：安全采样 `IsPlaying`、`HasPlayableMapUI`、`Map`、`SelectedPawn`；preview/diagnostics 只能通过设置拥有的 draw context 使用。
- [../Mod.cs](../Mod.cs) 与 [../Patches/Patch_SettingsWindowClose.cs](../Patches/Patch_SettingsWindowClose.cs)：设置窗口注册、原生 close flags、`WindowStack.TryRemove` 关闭入口、close flush 与 session 结束。
- [../../../docs/settings-ui-product-contract-zh.md](../../../docs/settings-ui-product-contract-zh.md)：产品边界（immediate persistence、No-Biotech capability、状态/布局合同）。

## Design

- **Shell + gated tabs**：`DrawSettings` 每帧先 `SqueakSettingsGameContext.Capture()`、`SqueakRuntimeResolver.TickPendingRuntimeChanges()`，再按 `VisibleSettingsTabs()` 绘制导航、body frame 与固定 footer。普通状态只有 `Basics`、`SoundMood`、`Xenotype`；`developerToolsEnabled` 为真才加入 `Developer`。导航在窄宽度切换为 wrapped compact tabs，body 各页独立 scroll owner；footer/status/version 槽位固定。
- **Immediate settings, local UI buffers only**：Basics 控件改动直接写正式字段并调用 `NotifyCheapRuntimeChanged`、`NotifyDistanceRuntimeChanged` 或 action policy/resolver 发布，再 `QueuePersistence()`。SoundMood 的 `editBuffer` 和 Xenotype 的 `XenotypePresetDraft` 是输入/字段保留所需的局部 UI copy，不是 Apply/Revert transaction：dirty 编辑在当前绘制或离开页面/目标时 canonicalize 并排队保存；mood 通过 `moodOverrides`，xenotype 通过 `xenotypePresets`。
- **Responsive bounded layout**：页面先计算内容高度，内容超出 viewport 时预留 scrollbar gutter（通常减 16）；折叠、搜索、过滤、detail 展开后 clamp scroll。Xenotype 宽屏是 target list + editor 两个明确 scroll，窄屏是 list/editor step，每步只有一个主要 scroll；controls 的 help rect 与 action rect 分离，`HelpIndicator` 会消费在 help 内开始的 MouseDown，避免点击穿透。
- **State visibility**：保存状态由 Mod 层拥有，footer 只显示 `Saving`/`Saved`/`Failed`；失败保留 dirty generation，不由 UI 静默清除。Xenotype 行显式显示 `Available`、`Dormant`、`TargetUnavailable`、`Orphan`、canonical conflict 与候选/启用数量；空列表使用 `EmptyState`，失联目标提供 `Forget` 与 destructive confirmation。
- **Modal confirmation boundary**：`Dialog_SqueakyCompactMessageBox` 设置 `absorbInputAroundWindow=true`、`closeOnClickedOutside=false`、`closeOnCancel=true`、`doCloseX=true`，并通过 `PreClose` 只在没有 action 时调用 `closeAction`。Remix 以 `RequestVoicePackMode` → `ShowFinalRemixConfirmation` → `ConfirmRemixMode` 两步确认，任何取消/关闭保持旧 canonical mode。

## Data & Control Flow

### Settings shell and session

1. `SqueakyRatkinMod.DoSettingsWindowContents` → `Settings.BeginSettingsSession()` → `TickQueuedSettingsSave()` → `Settings.DrawSettings(inRect)`。
2. `DrawSettings` 捕获游戏 context、消费 `RequestXenotypeTabOnNextDraw`，测量导航/footer/body，并只绘制当前 page；跨页 `RequestSettingsTab` 先 dirty-gated commit SoundMood/Xenotype，再强制 `FlushPendingRuntimeForPreview()`，最后切换 `activeTab`。
3. `SqueakyRatkinMod.OpenSettings` 反射创建原生 `Dialog_Options`，设置 `closeOnClickedOutside=false`、`closeOnCancel=true`（原生 Esc 语义）、`doCloseX=true`，登记到 `settingsWindows` 后 `Find.WindowStack.Add(dialog)`。
4. 原生 X 或 Esc 最终走 `WindowStack.TryRemove(Window,bool)`；[Patch_SettingsWindowClose](../Patches/Patch_SettingsWindowClose.cs) 的 Harmony prefix 只接受本 Mod 所有的 options window，先 `FlushPendingRuntimeChanges(true)`，再 `NotifySettingsWindowClosing` → 移除 `settingsWindows` → `Settings.EndSettingsSession()` → `FlushQueuedSettingsSave(true, true)`。外部/其他 mod 的窗口不会触发本设置 session。
5. `BeginSettingsSession` 用 `Time.frameCount` 检测绘制中断（帧号 != 上次帧号 + 1 即视为重新打开设置），重新打开时 `versionClickCount=0`；`EndSettingsSession` 在窗口关闭时再次清零（双保险）。未完成的七击计数所有者是 `SqueakyRatkinSettings.versionClickCount`（不是 helper、dialog 或 Mod）。

### Page 1 — Basics / 发声规则

`DrawSettingsContents` 以一个主 `scrollPos` 绘制 frequency toggles（time-speed/talking/audible-population）、global cooldown slider、可折叠 distance preset/range/chart、global action scope rows（包括 Draft/Undraft pair 与 Player/System groups）及 auxiliary `localizeDebugActions`。toggle/slider 采用临时局部值，接受后立即写正式字段：cheap values → `ApplyCheapAndQueue`；距离 → `ApplyDistanceAndQueue`；action menu → `SetActionGlobalScope` → `ApplyActionScopeAndQueue`（`SqueakGlobalActionPolicy.Publish` + discrete resolver notify + persistence）。

### Page 2 — SoundMood / 心情音色

`DrawSoundMoodSettings` 维护一个 `soundMoodScroll`；先选 Good/Neutral/Bad/Break 和 action，再 `EnsureBuffer` 从 saved override 或 XML default 建立 `editBuffer`。override toggle、preset、sliders 与 jitter normalization 改 buffer；同一绘制末尾 `CommitMoodEditorNow` 将 buffer clone 写入/移除 `moodOverrides`，调用 `NotifyGlobalMoodRuntimeChanged()` 与 `QueuePersistence()`。切 mood、切 page 也先 commit，因此没有 Apply/Revert。

Preview 状态由 `moodExplicitlyResolved`、`moodClipIndex`、`materialPreviewStatus`/failure 和 `finalPreviewStatus`/failure 持有。Resolve 使用 `SqueakSoundAvailabilityCache.Resolve`；material preview 取 `SqueakOnCameraPreviewAdapter` 的 `onCamera` sub-sound、`SampleOneShot.TryMakeAndPlay`，失败原因留在 status panel；final preview 要求 `CurrentDrawContext` 有 playable map + 当前地图 selected Pawn，先 commit/flush，再 `CompSqueaker.PreviewFinal` 并显示 dispatch 或具体不可用原因。

### Page 3 — Xenotype / 语音来源与异种

`DrawXenotypeManagement` 读取 `SqueakXenotypeCatalog.Current`，通过 `CommitVoicePackMode` 将 Off/Fallback/Remix 写入 runtime/persistence；Biotech 未激活时 UI 仍可进入但 catalog/xenotype target 状态降为 dormant，设置核心的 `GetVoicePackSelectionStatus` 负责资格与保存数据边界。Refresh 先 commit/flush 当前 continuous edit，再 refresh catalog/runtime，尽量保留 selected target。

`EnsureXenotypeRowCache` 从 catalog `GetTargetCandidates(voicePackSelections, xenotypePresets)` 建立本地化显示名、icon、defName/source、behavior/audio counts 与 conflict/dormant/unavailable status；search + Configured/Candidates/Enabled/Orphan filter 只改变显示。Race default 是独立 target layer；xenotype target 选择调用 `RebuildXenotypeDraft`，从 records 按 list order last-wins 合并字段。

宽屏 `DrawXenotypeAssignment` 画 list/editor 两 pane；窄屏通过 `xenotypeNarrowEditorStep` 在 list 与 editor 间切换。Behavior editor 的 optional enabled/interval/probability、mood factors/jitter 改 draft，发生 change 后 `MarkChanged` → `CommitXenotypeEditorNow` → canonical `xenotypePresets`、`NotifyContinuousXenotypeRuntimeChanged`、`QueuePersistence`。AudioPacks editor 直接对 Race/Xenotype domain 使用 `SetVoicePackSelection`，每次 selection 立即 discrete resolver notify + save；`voicePackMode=Off`、空候选、search 无结果均有明确状态。

失联/冲突的 selected keys 会保留并显示 `Dormant`、`TargetUnavailable`、`Orphan` 或 conflict；新增 pack 选择在 conflict 时阻止但既有选择可移除。`Forget unavailable target` 先打开 confirmation，确认后 `ForgetXenotypeTarget` 同时删除该 defName 的 behavior preset 与 xenotype voice-pack selection；普通 catalog refresh 不静默删档。

### Page 4 — Developer / 开发与诊断

Footer version rect 在 `!developerToolsEnabled` 时由 `DrawSettingsFooter` 处理 click：每次 `versionClickCount++`，达到 7 后 `EnableDeveloperToolsNow()` 排队保存、清零并发 message；解锁前没有 Developer tab。Developer page 有 audio browser open、禁用工具（确认后停止 statistics、关闭 audio-path diagnostics、回退 Basics）、logging mode cards（每次 `SetDevLoggingMode` + persistence）、build identity。

`DrawActionStatistics` 通过 `CurrentDrawContext.TryGetSelectedSqueaker` 控制 Start eligibility；Start/Stop/Reset/Copy 分别驱动 `SqueakActionStatistics`，table 和 status cache 只在 Repaint/revision/time bucket 更新，并有唯一 `statisticsScroll`。`DrawAudioPathDiagnostics` 直接控制 `SqueakAudioPathDiagnostics.Enabled`，支持 Clear/Copy；Repaint 时按 revision 复制 newest-first records，唯一 `audioPathScroll` 浏览空态/records，`audioDetailIndex` 展开 tooltip detail。Audio browser 本身由 `SqueakAudioBrowser.Open()` 创建独立 `Dialog_SqueakAudioBrowser`。

## Integration

- **Inbound**：RimWorld `Mod.DoSettingsWindowContents` 驱动 shell；`OpenSettings(selectXenotypeTab)` 是通知/诊断入口，可在首帧消费 tab request；`WindowStack` 提供 native options、FloatMenu 与 modal dialogs。
- **Runtime publication**：Basics 连接 `CompSqueaker` cheap/distance fields、`SqueakGlobalActionPolicy` 与 `SqueakRuntimeResolver`；SoundMood 由 playback 直接读取 global mood；Xenotype continuous edits 使用 resolver 的 coalesced path，mode/selection/catalog 使用 discrete path。
- **Persistence**：所有 `QueuePersistence` 都进入 [Mod.cs](../Mod.cs) 的 generation/debounce coordinator（约 350 ms）；`TickQueuedSettingsSave` 在窗口绘制时推进，关闭 prefix 强制 flush；footer 读取 save state 但不拥有状态。
- **Capability gates**：页面可在主菜单/无地图打开；`SqueakSettingsGameContext.Capture` 只在 Playing + Game + Root_Play + TickManager + CurrentMap + MapUI 通过后接触 selector。Final preview/statistics 的 Pawn/map gate 由 context 持有；Xenotype 的 Biotech gate 由 catalog/status 层持有，No-Biotech 不访问 Xenotype DefDatabase 或 pawn genes。
- **Compatibility boundary**：UI 不改变 action eligibility/resolver/audio fallback/Scribe schema，不 patch 其他 UI，不静默删除 orphan/or unavailable persisted keys；Remix、forget、diagnostics 的确认和门控保持各自既有状态机。

## Change Guidance

- 新页面或新 tab 必须接入 `SettingsTab`、`VisibleSettingsTabs`、navigation measure/draw、body dispatch 和 tab transition commit；不要在 partial/helper 旁另建 router/store/command bus。
- 新设置控件必须遵循现有 primitive 的 separated help/action rect，使用既有 `Measure*` 与单一 scroll owner；measure 阶段不得写设置、触发 resolver、catalog 或音频副作用。
- 新业务写入应修改设置核心的 canonical method，再由 UI 调用对应 runtime notification 与 `QueuePersistence`；不要把 catalog、resolver 或 save generation 逻辑放进 `SqueakySettingsUI`。
- Xenotype 新字段应扩展 `XenotypePresetDraft.FromRecords`/`Commit` 的 field-preserving last-wins 语义，并保持 Biotech inactive 的 dormant 降级、conflict retention 与 explicit forget confirmation。
- 任何关闭路径不得只处理自绘按钮：原生 X 与 Esc 依赖 `Dialog_Options` flags，实际移除依赖 `Patch_SettingsWindowClose` → `NotifySettingsWindowClosing`；若替换窗口类型，必须保持 ownership recognition、`EndSettingsSession` 与 close flush 链路。
- 七击解锁的未完成计数继续由 `SqueakyRatkinSettings.versionClickCount` 持有，并在 `BeginSettingsSession`（帧号检测重新打开）清零、`EndSettingsSession`（关闭时）双保险清零；不要把计数放入绘制 helper 或跨 session 静态状态。
- 维持四页产品合同及现有状态可见性：普通玩家不显示 Developer 占位入口；Saving/Saved/Failed 不改变 footer 高度；No-Biotech、主菜单、无 Pawn、空/失联/冲突 catalog 均需安全且可解释。
