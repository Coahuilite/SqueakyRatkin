# 1.6/Patches/

## Responsibility

加载期（defs 解析阶段）把发声组件 `SqueakyRatkin.CompProperties_Squeaker` 注入 Ratkin 种族的 `comps` 列表，使 `CompSqueaker`（ThingComp）随每个 Ratkin pawn 生成而挂载。全部调参（触发模式/间隔/概率/心情音色/距离）数据驱动，只改本目录 XML，无需重编译 DLL。

## Key Files / Symbols

- `Ratkin_AddSqueakComp.xml` —— 唯一补丁文件。
- 目标 XPath：`/Defs/AlienRace.ThingDef_AlienRace[defName="Ratkin"]`（由 Solaris.RatkinRaceMod 即 NewRatkinPlus/HAR 提供，非本 mod 定义）。
- C# 连接（均在 `Source/SqueakyRatkin/`）：
  - `CompProperties_Squeaker` / `CompSqueaker`（`CompSqueaker.cs`）——XML 节点 Class 名直接对应；构造函数 `compClass = typeof(CompSqueaker)`。
  - `SqueakActionConfig`（action/mode/minIntervalTicks/probabilityPerCheck/ignoreGlobalCooldown/cooldownClock）、`SqueakTriggerMode`（EachTime/RandomOneShot/External）、`SqueakCooldownClock`（GameTicks/Realtime）——同文件。
  - `SqueakMoodMod`（mood/pitchFactor/pitchJitter/volumeFactor）、`SqueakDistancePreset`（Conservative/Balanced/Strong/Custom）——`SqueakyRatkinSettings.cs`。
  - `SqueakDistancePresetConfig`（preset + `FloatRange range`，字符串 `"15~65"` 由 Verse 解析）——`CompSqueaker.cs`。
  - `SqueakAction` 枚举（15 值，序数必须稳定、append-only）与 `SqueakActionDefinitions`（`SqueakActionModel.cs`）——动作名 ↔ AudioKey `SR_<Action>` 的运行时契约。

## Design

两段式补丁（同一文件内两个 Operation，按序执行）：

1. `PatchOperationConditional` 检查 `/Defs/.../Ratkin/comps` 是否存在；不存在时 `nomatch` 分支用 `PatchOperationAdd` 先建空 `<comps/>`（保证追加目标节点一定存在）。
2. `PatchOperationAdd` 向 `/comps` 追加一个 `<li Class="SqueakyRatkin.CompProperties_Squeaker">`，含三块配置：

**actions（15 条，与 `SqueakAction` 枚举一一对应）**

| action | mode | minIntervalTicks | probabilityPerCheck | 备注 |
|---|---|---|---|---|
| Eat | EachTime | 144 | – | 每次触发 |
| Call | RandomOneShot | 864 | 0.012 | |
| Move | RandomOneShot | 504 | 0.012 | |
| Sleep | RandomOneShot | 1080 | 0.008 | |
| Social | RandomOneShot | 504 | 0.016 | |
| Joy | RandomOneShot | 504 | 0.016 | |
| Work | RandomOneShot | 720 | 0.012 | |
| Wounded | External | 216 | – | 伤害 hook |
| Select | External | 18 | – | ignoreGlobalCooldown + Realtime |
| Death | External | 0 | – | ignoreGlobalCooldown |
| Draft / Undraft | External | 36 | – | ignoreGlobalCooldown + Realtime |
| Attack | External | 216 | – | |
| Equip | External | 216 | – | |
| MentalBreak | External | 0 | – | ignoreGlobalCooldown |

顶层：`globalMinIntervalTicks=216`（全局冷却基准，C# 默认值同）、`scaleFrequencyWithTalking=true`。

**moodMods（4 条）**：Good `pitchFactor=1.2 / pitchJitter=0.97~1.03 / volumeFactor=1.3`；Neutral `1.0 / 0.97~1.03 / 1.0`；Bad `0.8 / 0.97~1.03 / 0.7`；Break `1.1 / 0.6~1.5 / 1.5`。运行时叠加到 SoundDef 的 pitch/volume（见 SoundDefs 图），XML 即默认层（配置顺序：XML ← ModSettings override ← 运行时）。

**distancePresets（3 条）**：Conservative `15~65`、Balanced `15~50`（C# 默认）、Strong `15~40`。运行时 `CompSqueaker.ApplyDistanceRange` 把所选 range 写回已知 Map SoundDef 每个 subSound 的 `distRange`（覆盖 SoundDef XML 里的基础 15~70）。

## Data & Control Flow

```
LoadFolders.xml (v1.6，无条件加载)
  → RimWorld 解析 1.6/Patches（在 AlienRace 与 Ratkin def 加载后应用）
  → CompProperties_Squeaker 注入 Ratkin.comps
  → 每个 Ratkin pawn 生成 → CompSqueaker 附加
  → 触发源（Source/SqueakyRatkin/Patches/ 的 Harmony hooks：Select/Attack/MentalBreak/Death/Draft/Equip/Wounded 等 + 周期 tick）
  → SqueakAction 值 → 查询 action plan（mode/interval/probability）→ SqueakRuntimeResolver 选音 → 播放
```

方向：XML 只声明“配置”；C# 枚举值（`SqueakAction`、`SqueakTriggerMode`、`SqueakMood`）是 XML 与代码之间的兼容边界。运行时 hooks 见 `Source/SqueakyRatkin/Patches/`（本图不展开）。

## Integration

- 依赖：About.xml `loadAfter` Solaris.RatkinRaceMod（def 来源）、brrainz.harmony、erdelf.HumanoidAlienRaces；Ratkin def 不存在时本补丁无目标，属加载顺序错误，不是静默降级。
- 与 SoundDefs 的关系：`actions` 的 15 个动作名即 `SR_<Action>` SoundDef 的命名来源（`SqueakActionDefinitions.AudioKey`），由 `SqueakRuntimeResolver.GetVanilla()` 在运行时按名查找。
- 与 MoteDefs 无直接依赖。
- No-DLC 边界：本补丁不含任何 DLC 条件；Biotech 仅影响运行时异种层（`ModsConfig.BiotechActive` 门控在 C#，见 resolver）。Core+Harmony+HAR/NewRatkinPlus 即可完整工作。

## Change Guidance

- 改触发节奏/概率：只编辑 actions 的对应 `<li>`；数值单位是 ticks（60 ticks = 1 秒）。
- 改心情音色：moodMods；`pitchJitter` 为 `"min~max"` 字符串。
- 改距离：distancePresets（注意与 SoundDef 基础 distRange 的覆盖关系）。
- 新增动作：必须同步 ① `SqueakAction` 枚举（append-only，不得改既有序数）② `SqueakActionDefinitions`（AudioKey）③ 对应 `SR_<Action>` SoundDef ④ 运行时 hook；否则配置存在但永不触发或无声。
- 删除/改名 `<li Class="SqueakyRatkin.CompProperties_Squeaker">` 会使 DLL 类型找不到 → 加载期错误；XML 与 `Assemblies/SqueakyRatkin.dll` 版本必须匹配。
