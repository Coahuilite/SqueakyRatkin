# 1.6/Defs/MoteDefs/

## Responsibility

定义唯一的调试浮字 mote `SR_Mote_TextBg`：在 pawn 头顶以世界坐标绘制短文本（带描边背景），供调试诊断显示发声结果。纯展示层，不参与音频解析。

## Key Files / Symbols

- `SR_Mote.xml` —— 单文件单 Def：
  - `ThingDef ParentName="MoteBase"`，`defName=SR_Mote_TextBg`
  - `thingClass=SqueakyRatkin.MoteTextWithBackground`
  - `graphicData.texPath=Things/Mote/Transparent`（原版透明纹理，仅占位）
  - `drawGUIOverlay=true`、`altitudeLayer=MetaOverlays`
  - `mote`: `realTime=true`（真实时间计时）、`solidTime=0.5`、`fadeInTime=0.2`、`fadeOutTime=1.5`
  - `modExtensions`：`<li Class="SqueakyRatkin.SqueakMoteOffset">` `offsetX=0`、`offsetY=1.3`（XML 覆盖 C# 默认 0.8）
- C# 连接（`Source/SqueakyRatkin/Debug/SqueakMoteMaker.cs`）：
  - `MoteTextWithBackground : MoteText` —— `DrawGUIOverlay()` 按 `def.modExtensions` 里的 `SqueakMoteOffset` 加偏移，`GenMapUI.DrawText` 四向描边 + 白色正文。
  - `SqueakMoteOffset : DefModExtension` —— 偏移字段（C# 默认 offsetX=0/offsetY=0.8）。
  - `SqueakMoteMaker.ThrowSqueakText(loc, map, text)` —— `DefDatabase<ThingDef>.GetNamedSilentFail("SR_Mote_TextBg")` 查找 → `ThingMaker.MakeThing` → 设 exactPosition/text → `GenSpawn.Spawn`。**Def 缺失时静默返回**（不报错）。
  - 调用方：`SqueakDebug.NotifySqueak`（`Debug/SqueakDebug.cs`，pawn.DrawPos 处抛文字），仅调试功能使用。

## Design

- 偏移双默认：C# 类内默认 `offsetY=0.8`，XML modExtension 覆盖为 `1.3`——调整位置只改 XML，无需重编译（类注释明示该设计意图）。
- `drawGUIOverlay` + `MetaOverlays` 决定绘制走 GUI overlay 通道而非网格纹理，配合 `realTime=true` 使文字生命周期不受游戏变速影响。

## Data & Control Flow

```
SqueakDebug.NotifySqueak(pawn, action, mood, choice)
  → SqueakMoteMaker.ThrowSqueakText(pawn.DrawPos, pawn.Map, "action · source · defName")
  → DefDatabase<ThingDef>["SR_Mote_TextBg"] → MakeThing → Spawn
  → 每帧 MoteTextWithBackground.DrawGUIOverlay（实时计时，solid→fadeOut 消亡）
```

## Integration

- 仅被 Debug 子系统（`Source/SqueakyRatkin/Debug/`）消费；生产发声链路（`CompSqueaker` → `SqueakRuntimeResolver`）完全不依赖此 Def。
- 不依赖 DLC；与 SoundDefs/Patches 无共享契约。

## Change Guidance

- 调文字位置：改 `SR_Mote.xml` 的 `SqueakMoteOffset`（offsetY 单位是格数，正值向上）；`SqueakMoteMaker.cs` 的 0.8 只是 C# 兜底默认。
- 调生命周期：`mote` 的 solidTime/fadeInTime/fadeOutTime。
- 改名 `defName` 需同步 `SqueakMoteMaker.ThrowSqueakText` 的字符串查找（当前为硬编码 `"SR_Mote_TextBg"`）；删除该 Def 只会让调试浮字静默失效，不影响发声功能。
