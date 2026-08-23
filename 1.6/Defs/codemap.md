# 1.6/Defs/

## Responsibility

本 mod 在 RimWorld 1.6 下的全部 Def 定义（纯 XML，无 C# 编译依赖），划分为两个子目录：

- **SoundDefs/** —— 发声资产契约：15 个 `SR_<Action>` Vanilla 回退 SoundDef + `SR_Call_Preview` 试听 transport；内置官方 Example Race VoicePack（`SR_OfficialExample_Race` + 当前 15 个 `SR_OfficialExample_Race_<Action>`，参考基准）。详见 [SoundDefs/codemap.md](SoundDefs/codemap.md)。
- **MoteDefs/** —— 调试浮字 mote `SR_Mote_TextBg`（thingClass `MoteTextWithBackground` + `SqueakMoteOffset` modExtension），仅供 Debug 子系统显示发声结果。详见 [MoteDefs/codemap.md](MoteDefs/codemap.md)。

## Key Files / Symbols

- `SoundDefs/SqueakyRatkin_SoundDefs.xml`、`SoundDefs/SqueakyRatkin_OfficialExample_Race.xml`、`MoteDefs/SR_Mote.xml`。
- 共享 defName 契约：所有本目录 defName 均以 `SR_` 前缀开头（`SqueakVoicePackValidator` 强制校验 pack 及其引用的 SoundDef；`SqueakActionDefinitions.AudioKey` 规定 Vanilla 回退名为 `SR_<Action>`）。
- C# 类型连接（均在 `Source/SqueakyRatkin/`）：`SqueakVoicePackDef`、`SqueakActionDefinitions`、`SqueakRuntimeResolver.GetVanilla()`（SoundDef 按名查找）、`MoteTextWithBackground` / `SqueakMoteOffset` / `SqueakMoteMaker`（`Debug/SqueakMoteMaker.cs`）。

## Design

- 音色双层：每动作 1 个中性 SoundDef（SoundDef 层只含基础 pitchRange 随机），心情调制全部由运行时 `SoundInfo` factor 叠加——XML Def 数与动作数严格 1:1（15），Example 与第三方包遵循同一选择/权重规则，无特权。
- 纯数据契约：本目录不承载任何加载顺序或条件逻辑（条件加载在 `../LoadFolders.xml` / 父级 `1.6/` 处理）；Def 之间无 XML cross-ref 强引用（VoicePack 的 `<sounds>` 是字符串查找）。

## Data & Control Flow

```
LoadFolders.xml v1.6（无条件）→ 加载 1.6/Defs/**
  → DefDatabase<SoundDef> 当前 16 + 15；DefDatabase<SqueakyRatkin.SqueakVoicePackDef> 1；ThingDef 1
  → SqueakRuntimeResolver 构建 Vanilla/Race/Xenotype tier（SoundDefs 子图）
  → SqueakDebug → SqueakMoteMaker 按名取 SR_Mote_TextBg（MoteDefs 子图）
```

## Integration

- 上游：`Patches/Ratkin_AddSqueakComp.xml` 的 15 个 `<action>` 名决定 SoundDefs 必须提供的 `SR_<Action>`；`SqueakAction` 枚举值是两边共享的隐式契约。
- 下游：`CompSqueaker`/`SqueakRuntimeResolver`（播放）、`SqueakSoundAvailability`（可播放性）、UI 试听（`SR_Call_Preview`）、Debug 浮字（`SR_Mote_TextBg`）。
- No-DLC 边界：本目录无 DLC 条件；Race/Vanilla 层在无任何 DLC 时完整可用，Biotech 只消费 Xenotype 范围 pack（本 mod 工作树内无 Xenotype pack Def）。

## Change Guidance

- 新增/改名/删除任何 defName 前，先核对 `SR_` 前缀规则与 `SqueakActionDefinitions`（17 动作固定、枚举 append-only；Crying/Giggling 为 15/16 且无内置 SoundDef，默认静默）。
- 改音频池/Example 见 SoundDefs 子图（注意 Example 音频由打包脚本从 Extras 镜像注入，工作树中不存在）；改浮字位置/生命周期见 MoteDefs 子图。
- 校验入口：加载时 `ConfigErrors()`（pack 契约）与打包时 `stage-package.ps1`（Template→built-in 镜像断言：实际键集合 + SHA-256；当前参考 41 OGG/15 个有音频的动作，数量可变）。
