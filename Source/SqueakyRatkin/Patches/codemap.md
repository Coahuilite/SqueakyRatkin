# Source/SqueakyRatkin/Patches/

## Responsibility

本目录是全部 Harmony 集成层：16 个 patch 把 RimWorld/Core 事件转译成 mod 自身的通知入口（主要是 `CompSqueaker.Notify_*`、`SqueakPeriodicPopulation`、`SqueakDiagnosticsOverlay`、`SqueakRuntimeResolver`）。Patch 本身不产生音效、不写设置、不渲染——只负责"在正确的时机、对正确的对象"调用下游服务。

- 入口：`SqueakyRatkinMod`（`../Mod.cs`）构造函数执行 `Harmony.PatchAll()`（Harmony id = `coahuilite.squeakyratkin`）。
- 全部文件单一 namespace `SqueakyRatkin`，无显式 patch id（顺序无依赖）。

## Key Files / Symbols

按职责分组（每条 = 文件 → patched symbol → 下游调用）：

**事件通知（→ `CompSqueaker`，`../CompSqueaker.cs`）**
| 文件 | patched symbol | 时机/触发 | 下游 |
|---|---|---|---|
| `Patch_Pawn_PostApplyDamage.cs` | `Pawn.PostApplyDamage` Postfix | 任何伤害结算后 | `CompSqueaker.Notify_Wounded()` |
| `Patch_Verb_Attack.cs` | 动态扫描 Core + RimWorld 程序集内所有 `Verb` 子类声明的 `TryCastShot()` Postfix | 攻击成功（`__result == true`）、caster 是 spawned 且位于当前地图的 Pawn | `Notify_Attack()`（来源按 `IsCurrentJobPlayerCommand()` 区分 ActiveCommand/StateEvent） |
| `Patch_Pawn_Kill.cs` | `Pawn.Kill` Prefix | 击杀/流血死亡统一入口，位于 DeSpawnOrDeselect/SetDead 之前，此时 pawn 仍 spawned、位置有效 | `Notify_Death()` |
| `Patch_Selector_Select.cs` | `Selector.Select` 动态重载链 Postfix | 玩家选中 Pawn（第一参数为 `object`，位置注入 `__0`） | `Notify_Select()` |
| `Patch_DraftGizmo_Toggle.cs` | `Pawn.GetGizmos` Postfix | 包装 vanilla Draft/Undraft `Command_Toggle`（`Command_ColonistDraft` hotKey + "Draft"/"Undraft" tutorTag 识别），在 toggleAction 内比对 `pawn.Drafted` 前后变化 | `Notify_Draft(bool)`；`ConditionalWeakTable` 防重复包装，只包玩家可见 gizmo，不碰 `Pawn_DraftController` |
| `Patch_Pawn_EquipmentAdded.cs` | `Pawn_EquipmentTracker.Notify_EquipmentAdded` Postfix | 装备跟踪器通知（含 AI/加载/系统变更），再经 `IsCurrentEquipJobPlayerCommand()` 过滤为玩家主动 Equip 任务 | `Notify_Equip()` |
| `Patch_MentalBreak.cs` | `MentalBreakWorker.TryStart` Postfix（`Verse.AI.MentalBreakWorker` 或 `RimWorld.MentalBreakWorker` 动态解析） | 精神崩溃成功开始（`__result`）且 pawn 在当前地图 | `Notify_MentalBreak()`；找不到目标时 `SqueakLog.HookMentalBreakUnavailable()`。0.2.4 起收窄至此（MentalBreakDef 驱动唯一通道），不误报 BabyFits |
| `Patch_MentalFit.cs` | `Verse.AI.MentalStateHandler.TryStartMentalState` Postfix | 0.3.1 波 3c BabyFits 窄 hook：`__result` && `stateDef.defName` ∈ {Crying, Giggling} && 经 `MentalFitDef` 反向 map 验证（Biotech 关 → 空 map → 永不通知） | `Notify_MentalFit(Crying|Giggling)`；找不到目标时 `SqueakLog.HookMentalFitUnavailable()`。不扩大/替代 MentalBreak hook |

**周期成员/生命周期（→ `SqueakPeriodicPopulation`，`../SqueakPeriodicPopulation.cs`）**
| 文件 | patched symbol | 时机/触发 | 下游 |
|---|---|---|---|
| `Patch_Pawn_DeSpawn_PeriodicPopulation.cs` | `Pawn.DeSpawn` Prefix | 任何 despawn/地图转移，早于下一个 Comp tick 保证成员表准确 | `NotifyPeriodicDespawn(MapHeld)` → `Unregister` |
| `Patch_Map_Deinit_PeriodicPopulation.cs` | `MapDeiniter.Deinit` Prefix | 地图销毁 | `RemoveMap(map)` |
| `Patch_Root_DiagnosticsLifecycle.cs` | `Root.Update` Postfix | 每帧（包括主菜单等地图 GUI 之外） | `SqueakDiagnosticsOverlay.MaintainLifecycle()`（见特殊路径） |

**诊断 UI（→ `SqueakDiagnosticsOverlay` / `SqueakDebug`）**
| 文件 | patched symbol | 时机/触发 | 下游 |
|---|---|---|---|
| `Patch_MapInterface_DiagnosticsOverlay.cs` | `RimWorld.MapInterface.MapInterfaceOnGUI_BeforeMainTabs`（fallback `MapInterfaceOnGUI`）Postfix；`Prepare()` 先解析并缓存目标，找不到则跳过并仅 dev 警告一次 | Layout 事件 | `SqueakDiagnosticsOverlay.RefreshIfDue()` |
| | | Repaint 事件 | `SqueakDiagnosticsOverlay.DrawCached()` |
| `Patch_GlobalControlsUtility_CameraIndicator.cs` | `GlobalControlsUtility.DoDate` Postfix | 仅 `Prefs.DevMode && SqueakDebug.ShowCameraIndicator`，跳过 Layout | 直接绘制相机高度/orthoSize 文本（上移 `curBaseY` 26f） |

**设置/元数据/调试菜单**
| 文件 | patched symbol | 时机/触发 | 下游 |
|---|---|---|---|
| `Patch_SettingsWindowClose.cs` | `WindowStack.TryRemove(Window, bool)` Prefix | 仅当本 mod 自有的设置窗口关闭（见特殊路径） | `SqueakRuntimeResolver.FlushPendingRuntimeChanges(true)` + `SqueakyRatkinMod.NotifySettingsWindowClosing(window)`；Prefix 恒返回 true 保留正常关闭 |
| `Patch_ModMetaData_LocalizedMetadata.cs` | `ModMetaData.Name` / `Description` getter Postfix | 任何读取本 mod 元数据处（`SamePackageId(PackageId, ignorePostfix)` 识别） | 用 `SR.About.Name` / `SR.About.Description` 键替换结果 |
| `Patch_DebugTabMenu_Actions.cs` | `DebugTabMenu_Actions.GenerateCacheForMethod` Prefix/Postfix | 生成调试动作缓存时；`SetEnabled(bool)`（由 `SqueakyRatkinSettings` 的 localizeDebugActions 选项驱动）控制 | 翻译 `DebugAction_`/`DebugActionCategory_` 键；Postfix 用 `__state` 还原 attribute 避免污染缓存；`SetEnabled` 变更时清空 `Dialog_Debug` 根缓存 |

## Design

- **事件转译模式**：所有事件 patch 都只做"过滤 + 转调"，业务判断集中在 `CompSqueaker.NotifyExternal`（spawned、`MapHeld == Find.CurrentMap`、当前视口 `ExpandedBy(10)` 三闸门）。patch 层保持薄。
- **跨版本防御**：三个 patch 用动态目标解析（`TargetMethod(s)`/`Prepare`）应对 Core API 漂移：
  - `Patch_Selector_Select`：`Select(object,bool,bool)` → `(object,bool)` → `(object)` fallback 链，并用 `__0` 位置参数规避参数名耦合。
  - `Patch_MentalBreak`：`Verse.AI` → `RimWorld` 命名空间 fallback；失败仅记录 dev 日志不炸 PatchAll。
  - `Patch_MapInterface_DiagnosticsOverlay`：`Prepare()` 提前解析类型+方法并缓存，不兼容则 `HookAvailable=false`、整体跳过（可选功能安全降级）。
- **`Patch_Verb_Attack` 扫描式 hook**：反射枚举 Core 与 RimWorld 程序集的全部 `Verb` 子类，收集具体声明（非 static/abstract、无泛型、返回 bool、有方法体）的 `TryCastShot`；显式排除名称含 "Ability" 的类型与 DLC 程序集。目标为空时 `SqueakLog.HookAttackUnavailable()`；跳过目标最多警告 8 次（`MaxSkippedTargetWarnings`）。
- **缓存/状态还原**：`Patch_DraftGizmo_Toggle` 用 `ConditionalWeakTable` 保证 gizmo 包装幂等（每次 GetGizmos 都重建枚举）；`Patch_DebugTabMenu_Actions` 用 `__state` 在 Postfix 还原 attribute，防止共享 attribute 实例被翻译污染。
- **只挂玩家面**：draft 只包装 `Pawn.GetGizmos` 的 gizmo 而非 `Pawn_DraftController.Drafted` setter；equip 只响应玩家主动 Equip 任务。

## Data & Control Flow

```
RimWorld 事件 → Harmony patch（本目录）
  ├─ 事件类（wounded/attack/death/select/draft/equip/mental）
  │    → CompSqueaker.Notify_* → NotifyExternal
  │        → SynchronizePeriodicMembership（注册/注销于 SqueakPeriodicPopulation）
  │        → 闸门（Spawned + CurrentMap + 视口）→ SqueakActionPlan
  │        → SqueakGlobalActionPolicy 作用域 → 运行时上下文/时间窗
  │        → SqueakActionStatistics → 音频调度（详见 ../CompSqueaker.cs 地图）
  ├─ 生命周期（DeSpawn/MapDeinit）→ SqueakPeriodicPopulation.Unregister/RemoveMap
  │    保持按地图的成员集合与当前帧一致（30 tick 维护节流在该类内部）
  └─ 诊断（Root.Update / MapInterface / DoDate / DebugTabMenu）
       → SqueakDiagnosticsOverlay（快照在 Layout、绘制在 Repaint、拆除在 Root.Update）
       → SqueakDebug 开关（SqueakyRatkinSettings 驱动）
```

状态所有者：`CompSqueaker`（每个 pawn 一份，事件消费者）、`SqueakPeriodicPopulation`（按 Map 的成员表 + 共享快照，静态）、`SqueakDiagnosticsOverlay`（静态模式 + 缓存）、`SqueakyRatkinMod.settingsWindows`（设置窗口注册表）。

## Integration

- 上游：RimWorld/Core API（本目录唯一直接依赖面）；`SqueakyRatkinMod.Harmony.PatchAll()` 一次性装配（`../Mod.cs`）。
- 下游（均为本目录调用、不在本目录实现）：
  - `../CompSqueaker.cs` — `Notify_Wounded/Select/Death/Draft/Attack/Equip/MentalBreak`、`NotifyPeriodicDespawn`
  - `../SqueakPeriodicPopulation.cs` — `Register/Unregister/RemoveMap`
  - `../Debug/SqueakDiagnosticsOverlay.cs` — `MaintainLifecycle/RefreshIfDue/DrawCached`
  - `../SqueakRuntimeResolver.cs` — `FlushPendingRuntimeChanges(bool force)`
  - `../Logging/SqueakLog.cs` — `HookAttackUnavailable/HookAttackTargetSkipped/HookMentalBreakUnavailable/DiagnosticsHookUnavailable/ShouldEmitDev`
  - `../Debug/SqueakDebug.cs` — `ShowCameraIndicator`（dev 调试开关）
  - `../SqueakyRatkinSettings.cs` — `Patch_DebugTabMenu_Actions.SetEnabled` 的调用方（localizeDebugActions 选项）
- 兼容边界：`Patch_Verb_Attack` 明确不覆盖 Ability/DLC 攻击 API；诊断 overlay 与 MentalStateHandler/Selector 目标解析失败时降级为"无此功能 + dev 日志"，不阻断其他 patch。

## Change Guidance

- **新增事件**：遵循"patch 薄 + `CompSqueaker.Notify_*` 厚"模式——在 CompSqueaker 加 Notify 方法与 `SqueakAction` 定义，patch 只做类型/地图过滤。触发闸门（CurrentMap、视口）留在 Comp，不要复制到 patch。
- **API 版本漂移**：签名可能变化的 Core 目标一律用 `TargetMethod(s)` + fallback 链或 `Prepare()` 预解析；可选功能用 Prepare 返回 false 安全跳过，必需功能失败至少走 `SqueakLog.Hook*Unavailable()` 留痕。
- **`Root.Update` hook 红线**：`MaintainLifecycle()` 必须保持无快照/格式化/翻译/扫描/绘制工作（每帧执行）；快照只进 `RefreshIfDue`（Layout），绘制只进 `DrawCached`（Repaint）。
- **设置窗口关闭**：`Patch_SettingsWindowClose` 的 Prefix 只对本 mod 自有的窗口执行 flush（`IsOwnedSettingsWindow` 双保险：注册表 + `Dialog_Options.Mod` 归属检查），绝不为第三方 mod 的 Dialog_Options flush；Prefix 必须返回 true 以保留正常关闭行为。
- **Verb 攻击 hook**：新攻击 API 若不声明 `TryCastShot`（或属于 Ability/DLC 程序集）不会被覆盖，需要显式扩展扫描清单，不能假设全覆盖。
- **不要改动**：patch 内不写设置、不直接调度音频、不持有 pawn 强引用做长生命周期缓存。
