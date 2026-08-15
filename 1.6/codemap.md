# 1.6/

## Responsibility

RimWorld 1.6 版本内容包：把发声组件挂到 Ratkin 种族（加载期 patch）、提供全部发声与浮字 Def、随包分发编译好的 DLL 与本地化文本。此目录**无条件加载**（`../LoadFolders.xml` 的 `v1.6` 条目不再 gated 任何 packageId）；发声组件注入依赖 XPath 目标 `defName="Ratkin"`，缺该 def 时静默 no-op。

## Key Files / Symbols

- `Patches/` —— `Ratkin_AddSqueakComp.xml`：向 `AlienRace.ThingDef_AlienRace[defName="Ratkin"]` 注入 `SqueakyRatkin.CompProperties_Squeaker`（15 动作触发配置 + 4 moodMods + 3 distancePresets，全部数据驱动）。详见 [Patches/codemap.md](Patches/codemap.md)。
- `Defs/` —— 纯数据契约中间图（SoundDefs / MoteDefs 子图入口）。详见 [Defs/codemap.md](Defs/codemap.md)。
- `Defs/SoundDefs/` —— Vanilla 回退音池（15 × `SR_<Action>` + `SR_Call_Preview`）+ 官方 Example Race VoicePack（`SR_OfficialExample_Race` + 15 SoundDef，当前参考基准）。详见 [Defs/SoundDefs/codemap.md](Defs/SoundDefs/codemap.md)。
- `Defs/MoteDefs/` —— 调试浮字 `SR_Mote_TextBg`（`MoteTextWithBackground` + `SqueakMoteOffset`）。详见 [Defs/MoteDefs/codemap.md](Defs/MoteDefs/codemap.md)。
- `Assemblies/SqueakyRatkin.dll`（+ `.pdb`）—— 编译产物，由 `../Source/SqueakyRatkin/` 构建；XML 与 DLL 版本必须匹配。
- `Languages/` —— 英/中 Keyed 本地化（本图不展开）。
- 全局标识：packageId `coahuilite.squeakyratkin`（`../About/About.xml`）。

## Design

- 分层职责：`Patches/` = 加载期组件注入；`Defs/` = 纯数据契约（无 C# 编译依赖）；`Assemblies/` = 运行时逻辑；`Languages/` = 展示文本。XML 与 C# 通过 `SqueakAction` 枚举（15 值，append-only）与 `SR_` 前缀 defName 契约连接，无 XML cross-ref 强引用。
- 配置三层：CompProperties(XML 默认) ← ModSettings(玩家 override) ← 运行时（resolver 快照）；本目录是默认层。
- **No-DLC 边界**：强制基线 = Core + Harmony + HAR/NewRatkinPlus + Squeaky Ratkin、官方 DLC 全禁用——Race VoicePack、Vanilla 回退、15 动作、心情调制、设置全部可用；Biotech 仅为精确 `XenotypeDef.defName` 提供可选增强，且所有 Biotech 路径在 C# 侧以 `ModsConfig.BiotechActive` 门控，No-DLC 运行时绝不触碰 Xenotype DefDatabase 或 pawn genes。DLC 不在 About.xml 依赖中。

## Data & Control Flow

```
RimWorld 1.6 启动 → LoadFolders v1.6（无条件）
  ├─ "/" 根（About 已先期解析；Assemblies 由 C# 加载）
  └─ 1.6/：Patches 注入 comp → Defs 入 DefDatabase → CompSqueaker 随 pawn 挂载
            → SqueakRuntimeResolver 按 SqueakAction 选音（Vanilla/Race/Xenotype tier）
            → 播放（mood 调制）｜Debug 时 SqueakMoteMaker 抛 SR_Mote_TextBg 浮字
```

## Integration

- 上游依赖：`brrainz.harmony`、`erdelf.HumanoidAlienRaces`（modDependencies），`Solaris.RatkinRaceMod`（def 来源，loadAfter 声明）。
- 下游消费方：`Source/SqueakyRatkin/` 的 `CompSqueaker`、`SqueakRuntimeResolver`、`SqueakVoicePackModels`、`Debug/SqueakMoteMaker`、UI 试听适配器；第三方 VoicePack 以独立模组形式按 `docs/voice-pack-author-guide-zh.md` 契约接入（`SR_` 前缀 + `<lowercase packageId>/<PackDef.defName>/<Action>/` 音频根）。
- 打包：`../scripts/stage-package.ps1` 从 `../Extras/SqueakyRatkinExampleVoices` 镜像 Template 的 OGG（当前参考基准 41 个，数量可变）进 `1.6/Sounds/coahuilite.squeakyratkin/SR_OfficialExample_Race/`（工作树中不存在该音频目录）。

## Change Guidance

- 调触发/音色/距离 → `Patches/Ratkin_AddSqueakComp.xml`；调音池/Example → `Defs/SoundDefs/`；调调试浮字 → `Defs/MoteDefs/`。
- 新增动作需三处同步：`SqueakAction` 枚举（append-only，`../Source/SqueakyRatkin/CompSqueaker.cs`）、`SqueakActionDefinitions.AudioKey`（`SqueakActionModel.cs`）、`SR_<Action>` SoundDef；再补运行时 hook。
- 任何 defName 改动保持 `SR_` 前缀；Example 音频改动必须同步 Extras 镜像（打包断言 Template→built-in 实际键集合与 SHA-256 一致），否则发布失败。
- 本目录与 `Source/` 的 DLL 由同一版本构建；改 XML 契约字段（如 `CompProperties_Squeaker`）需同步 `SqueakyRatkin.csproj` 构建产物。
