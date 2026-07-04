# 文案人工审核归档 / Text Review Archive

用途：集中审核当前仓库中玩家、发布页、发布包、开发者菜单可见的文案。

源码模式阅读规则：
- 每个条目都是独立块，不使用 Markdown 表格。
- `source` 是当前仓库位置，人工审核时不要改。
- `value` / `en_value` / `zh_value` 是当前仓库原文快照。
- 在 `review_*` 下写建议改文；不需要修改就留空。
- 代码、路径、DefName、packageId、URL 通常不翻译；如需改，只在 `note` 写原因。

当前特别说明：
- DebugAction 本地化参考 `Dev In Your Language` 的 `DebugAction_*` Keyed 风格。
- 外部只读检查结论：`I:\SteamLibrary\steamapps\workshop\content\294100\2142743468` 带 DLL 与大量 `DebugAction_*` Keyed；`I:\SteamLibrary\steamapps\workshop\content\294100\2898638732` 为鼠族普通 Def/Keyed 汉化，未见 DebugAction 支持。
- 本模组的 DebugAction 本地化由 ModSettings 开关控制，默认关闭；切换后无需重启游戏，重开调试动作菜单后生效。

---

## 1. About 元数据 / About Metadata

### about.name
source: `About/About.xml:4`

value:
```text
鼠辈啁啾 (Squeaky Ratkin)
```

review_value:
```text
Squeaky Ratkin
```

note: 模组显示名。惯例是保持中文

### about.description
source: `About/About.xml:9-11`

value:
```text
让鼠族(Ratkin)在空闲叫、吃饭、睡觉、受击、选中、移动、社交、娱乐、死亡时,按心情状态(好/中/坏/崩溃)发出吱吱叫。屏幕外视野剔除以省性能,镜头内通过距离衰减自然变轻。

仅作用于 Ratkin 种族,需配合 NewRatkin(鼠族)使用。
```

review_value:
```text
Squeaky Ratkin is a lightweight sound effect mod that injects a little stir into the Ratkin experience. It brings a whimsical medley of chirps and squeaks to these once‑silent little critters, ensuring every action comes with... a little sound things. No longer shall the colonies and the world beyond echo solely with the lamentations of blood and fire — the whispered squeaks of the ratkin will remind you that though they may be small, they are never content to remain silent.

*Not eligible for registration in The Book of Squeakudges*

鼠辈啁啾(Squeaky Ratkin) 是一款轻量级音效模组，为 鼠族(Ratkin) 的游玩体验注入一些*小动静*。它为这些曾经沉默的小家伙们带来随心所欲的百变啁啾，让每一次行动都附带着……那么一点动静。从此，殖民地与远方世界不再只回荡着血与火的哀鸣，鼠鼠们窸窣的吱语会提醒你：它们虽小，却从不甘于沉寂。

*禁止登记在仇恨吱书上*
```

note: 按review原样替换

### about.dependencies
source: `About/About.xml:15-27`

value:
```text
Harmony
Humanoid Alien Races
Ratkin (鼠族)
```

review_value:
```text
```

note: 依赖名称通常不翻译。模组名称需要完全按照对应依赖模组的名称

---

## 2. Keyed / 全局设置

source:
- EN: `1.6/Languages/English/Keyed/SqueakyRatkin.xml`
- ZH: `1.6/Languages/ChineseSimplified/Keyed/SqueakyRatkin.xml`

### SR.SettingsCategory
en_value:
```text
Squeaky Ratkin
```
zh_value:
```text
鼠辈啁啾
```
review_en:
```text
```
review_zh:
```text
```
note: 模组设置页分类名。

### SR.Global.Header
en_value:
```text
Tuning options
```
zh_value:
```text
调整选项
```
review_en:
```text
```
review_zh:
```text
```
note:

### SR.UseCustomOnly.Label
en_value:
```text
Full override (custom audio only, no vanilla default)
```
zh_value:
```text
完全 override(只用自定义音频,不混原版豚鼠 default)
```
review_en:
```text
Custom Audio Full override.
```
review_zh:
```text
自定义音频全覆盖。
```
note:

### SR.UseCustomOnly.Desc
en_value:
```text
ON: only custom audio (SR_*_Pure). OFF: custom + vanilla guinea-pig default mixed.
```
zh_value:
```text
开启:只用自定义音频(SR_*_Pure)。关闭:自定义与原版豚鼠 default 混合。
```
review_en:
```text
ON: only custom audio (SR_*_Pure). OFF: custom + vanilla default(guinea-pig) mixed.
```
review_zh:
```text
```
note: 当前未在 UI 中直接显示，但仍保留为说明文本。

### SR.ScaleCooldownWithTimeSpeed.Label
en_value:
```text
Scale trigger cooldown with time speed (recommended)
```
zh_value:
```text
按游戏倍速放大触发冷却(推荐)
```
review_en:
```text
```
review_zh:
```text
```
note: 默认开启。

### SR.ScaleFrequencyWithTalking.Label
en_value:
```text
Scale squeak frequency with Talking capacity
```
zh_value:
```text
按语言能力缩放鼠叫频率
```
review_en:
```text
```
review_zh:
```text
按语言能力缩放触发频率
```
note: 默认开启；普通发声按最终 Talking 能力降频，Death 有特殊规则。

### SR.LocalizeDebugActions.Label
en_value:
```text
Localize debug actions with this mod's translations (reopen the debug actions menu)
```
zh_value:
```text
使用本模组自带翻译本地化调试动作(重开调试动作菜单后生效)
```
review_en:
```text
```
review_zh:
```text
```
note: 默认关闭；切换不需要重启游戏，但已打开的调试动作菜单需要关闭重开。

### SR.GlobalCooldownMultiplier.Label
en_value:
```text
Global trigger interval: {0}x
```
zh_value:
```text
全局触发间隔：{0}x
```
review_en:
```text
```
review_zh:
```text
```
note: `{0}` 是倍率数值。

---

## 3. Keyed / 距离衰减

### SR.Distance.Header
en_value:
```text
Camera-height attenuation
```
zh_value:
```text
按摄像机高度衰减
```
review_en:
```text
```
review_zh:
```text
```
note:

### SR.Distance.Preset
en_value:
```text
Attenuation preset
```
zh_value:
```text
衰减预设
```
review_en:
```text
```
review_zh:
```text
```
note:

### SR.Distance.Preset.Conservative
en_value:
```text
Conservative
```
zh_value:
```text
保守
```
review_en:
```text
```
review_zh:
```text
```
note: 当前 XML range: `15~65`。

### SR.Distance.Preset.Balanced
en_value:
```text
Balanced
```
zh_value:
```text
适中
```
review_en:
```text
```
review_zh:
```text
```
note: 当前默认 XML range: `15~50`。

### SR.Distance.Preset.Strong
en_value:
```text
Strong
```
zh_value:
```text
强力
```
review_en:
```text
Radical
```
review_zh:
```text
激进
```
note: 当前 XML range: `15~40`。比起强力更应该换为激进，可能需要修改类名

### SR.Distance.Preset.Custom
en_value:
```text
Custom
```
zh_value:
```text
自定义
```
review_en:
```text
```
review_zh:
```text
```
note:

### SR.Distance.FullVolume
en_value:
```text
Attenuation starts
```
zh_value:
```text
衰减开始
```
review_en:
```text
```
review_zh:
```text
```
note:

### SR.Distance.Silent
en_value:
```text
Attenuation ends
```
zh_value:
```text
衰减结束
```
review_en:
```text
```
review_zh:
```text
```
note:

### SR.Distance.Chart.Volume
en_value:
```text
Volume
```
zh_value:
```text
音量
```
review_en:
```text
```
review_zh:
```text
```
note:

### SR.Distance.Chart.Distance
en_value:
```text
Height
```
zh_value:
```text
高度
```
review_en:
```text
```
review_zh:
```text
```
note: UI 图表中显示；底层实际为 camera/listener 到 pawn 的 3D 距离。

### SR.Distance.Chart.Full
en_value:
```text
start {0}
```
zh_value:
```text
开始 {0}
```
review_en:
```text
```
review_zh:
```text
```
note: `{0}` 是衰减开始值。

### SR.Distance.Chart.Silent
en_value:
```text
end {0}
```
zh_value:
```text
结束 {0}
```
review_en:
```text
```
review_zh:
```text
```
note: `{0}` 是衰减结束值。

### SR.Distance.Desc
en_value:
```text
The attenuation band uses the vanilla camera-height scale (15-65) as its tuning reference. In-map sounds stay full below {0}, fade between {0} and {1}, and are silent above {1}. Workbench preview is not distance-attenuated.
```
zh_value:
```text
衰减区间以原版摄像机高度范围(15-65)作为调参参照。地图内声音低于 {0} 保持全音量；{0} 到 {1} 之间逐步衰减；高于 {1} 静音。工作台预览不受距离衰减影响。
```
review_en:
```text
```
review_zh:
```text
```
note: `{0}` / `{1}` 分别是当前开始/结束值。UI 语义以原版摄像机高度 15-65 为调参参照；底层地图内播放仍通过 RimWorld/Unity 的 distRange 距离衰减执行。

---

## 4. Keyed / 心情调制工作台

### SR.Workbench.Header
en_value:
```text
Mood modulation workbench
```
zh_value:
```text
心情调制工作台
```
review_en:
```text
```
review_zh:
```text
```
note:

### SR.Workbench.Header.Desc
en_value:
```text
Pick a mood to edit its override; pick an action to preview. Enable override to replace the XML default. Drag sliders or type exact values, then Preview to hear.
```
zh_value:
```text
选一个心情编辑其 override;选一个动作用于预览。启用 override 即覆盖 XML 默认。拖动滑块或输入精确值,然后点预览试听。
```
review_en:
```text
```
review_zh:
```text
```
note:重写使其与当前实现对齐

### SR.Workbench.Controls
source: `1.6/Languages/*/Keyed/SqueakyRatkin.xml`

en_value:
```text
Mood
Preview action
Enable override (replaces default)
pitchFactor
volumeFactor
pitchJitter
min
max
Preview
Apply
Revert
Defaults
Mood preset
```
zh_value:
```text
心情
预览动作
启用 override(覆盖默认)
音调系数
音量系数
音调抖动
最小
最大
预览
写入
还原
默认值
心情预设
```
review_en:
```text
```
review_zh:
```text
```
note: 对应 `SR.Workbench.*` 控件标签。重写使其与当前实现对齐

### SR.Preset.*
source: `1.6/Languages/*/Keyed/SqueakyRatkin.xml`

en_value:
```text
Sharp
Neutral
Low
Chaos
```
zh_value:
```text
尖锐
中性
低沉
混乱
```
review_en:
```text
```
review_zh:
```text
```
note: 对应 `SR.Preset.Sharp/Neutral/Low/Chaos`。重写使其与当前实现对齐

---

## 5. Keyed / 动作与心情

### SR.Action.*
source: `1.6/Languages/*/Keyed/SqueakyRatkin.xml`

en_value:
```text
Call
Eat
Sleep
Wounded
Select
Move
Social
Joy
Death
```
zh_value:
```text
空闲
进食
睡眠
受击
选中
移动
社交
娱乐
死亡
```
review_en:
```text
```
review_zh:
```text
```
note: 对应 `SR.Action.Call/Eat/Sleep/Wounded/Select/Move/Social/Joy/Death`。重写使其与当前实现对齐

### SR.Mood.*
source: `1.6/Languages/*/Keyed/SqueakyRatkin.xml`

en_value:
```text
Good
Neutral
Bad
Break
```
zh_value:
```text
好心情
中性
坏心情
崩溃
```
review_en:
```text
```
review_zh:
```text
```
note: 对应 `SR.Mood.Good/Neutral/Bad/Break`。重写使其与当前实现对齐

---

## 6. Keyed / 调试动作与 Dev 可见文本

### SR.Debug.OverlayOn / SR.Debug.OverlayOff
source: `1.6/Languages/*/Keyed/SqueakyRatkin.xml`

en_value:
```text
Overlay: ON
Overlay: OFF
```
zh_value:
```text
悬浮字: 开启
悬浮字: 关闭
```
review_en:
```text
```
review_zh:
```text
```
note: 运行时状态/日志用文本，保留旧 key。

### SR.Debug.CameraIndicator
source: `1.6/Languages/*/Keyed/SqueakyRatkin.xml`

en_value:
```text
Camera: height {0}, view size {1}
```
zh_value:
```text
摄像机：高度 {0}，视野 {1}
```
review_en:
```text
```
review_zh:
```text
```
note: `{0}` 是 camera height，`{1}` 是 orthographicSize/view size。

### DebugActionCategory_SqueakyRatkin
source: `1.6/Languages/*/Keyed/SqueakyRatkin.xml`

en_value:
```text
Squeaky Ratkin
```
zh_value:
```text
鼠辈啁啾
```
review_en:
```text
```
review_zh:
```text
```
note: 本模组 DebugAction 分类名。仅在 `localizeDebugActions` 开启并重开调试动作菜单后使用。

### DebugAction_OverlayOn / DebugAction_OverlayOff
source: `1.6/Languages/*/Keyed/SqueakyRatkin.xml`

en_value:
```text
Overlay: ON
Overlay: OFF
```
zh_value:
```text
悬浮字：开启
悬浮字：关闭
```
review_en:
```text
```
review_zh:
```text
```
note: 对应 `SqueakDebugActions.OverlayOn/OverlayOff`。

### DebugAction_CameraIndicatorOn / DebugAction_CameraIndicatorOff
source: `1.6/Languages/*/Keyed/SqueakyRatkin.xml`

en_value:
```text
Camera Indicator: ON
Camera Indicator: OFF
```
zh_value:
```text
摄像机指示器：开启
摄像机指示器：关闭
```
review_en:
```text
```
review_zh:
```text
```
note: 对应 `SqueakDebugActions.CameraIndicatorOn/CameraIndicatorOff`。

---

## 7. README 发布页

source:
- EN: `README.md`
- ZH: `README.zh-CN.md`

### readme.title_and_language_switch
en_value:
```text
Squeaky Ratkin · 鼠辈啁啾
English | 中文
```
zh_value:
```text
鼠辈啁啾 · Squeaky Ratkin
English | 中文
```
review_en:
```text
```
review_zh:
```text
```
note: 实际 Markdown 中含链接和加粗。

### readme.summary
en_value:
```text
A QOL mod that makes Ratkin pawns squeak on idle call / eat / sleep / wounded / select / move / social / joy / death, with mood-tinted pitch (good / neutral / bad / breakdown). Off-screen pawns are culled for performance, and in-view sounds fade naturally with distRange distance attenuation.
```
zh_value:
```text
让鼠族(Ratkin)在空闲叫、吃饭、睡觉、受击、选中、移动、社交、娱乐、死亡时,按心情状态(好/中/坏/崩溃)发出吱吱叫。屏幕外视野剔除以节省性能,镜头内通过 distRange 距离衰减自然变轻。
```
review_en:
```text
```
review_zh:
```text
```
note:与about.xml对齐

### readme.blurb
en_value:
```text
The Chinese name 「鼠辈啁啾」is a playful pun; the mod itself is a lightweight, optional ambience enhancement.
```
zh_value:
```text
中文名「鼠辈啁啾」是个俏皮的谐音;模组本身是一个轻量、可选的氛围增强。
```
review_en:
```text
```
review_zh:
```text
```
note:只说是轻量可选的分为增强，别解释双关谐音

### readme.requirements
en_value:
```text
Requirements
RimWorld 1.6
Harmony
Humanoid Alien Races (HAR)
Ratkin / 鼠族 (NewRatkin)
```
zh_value:
```text
依赖
RimWorld 1.6
Harmony
Humanoid Alien Races (HAR)
Ratkin / 鼠族 (NewRatkin)
```
review_en:
```text
```
review_zh:
```text
```
note: 实际 README 中依赖带 Steam 链接。

### readme.install
en_value:
```text
Install
Steam: subscribe on the Workshop (once published).
GitHub Release: download the release zip and extract into `RimWorld/Mods/SqueakyRatkin/`.
```
zh_value:
```text
安装
Steam:订阅创意工坊(发布后)。
GitHub Release:下载 release zip,解压到 `RimWorld/Mods/SqueakyRatkin/`。
```
review_en:
```text
```
review_zh:
```text
```
note:

### readme.audio
en_value:
```text
Audio
Default source is the vanilla guinea-pig (rodent). To use custom squeaks:
- Place files in `1.6/Sounds/Squeak/<Action>/`, named `SR_<Action>_<n>.ogg`
- Spec: mono, ogg recommended; wav (16-bit) acceptable, 22050 or 44100 Hz, normalized to ~-3dBFS
- Uncomment the matching grain in `1.6/Defs/SoundDefs/SqueakyRatkin_SoundDefs.xml` to enable
- Mood differences are produced via runtime pitch/volume modulation — record only neutral-base variants (2–3 per action)
- Compensate per-audio traits in the mod settings "Modulation Workbench"
```
zh_value:
```text
音频素材
默认音源为原版豚鼠(啮齿类)。要使用自定义鼠叫:
- 放入 `1.6/Sounds/Squeak/<Action>/`,文件名 `SR_<Action>_<n>.ogg`
- 要求:单声道、推荐 ogg;wav(16-bit)也可、22050 或 44100 Hz、归一化到约 -3dBFS
- 取消 `1.6/Defs/SoundDefs/SqueakyRatkin_SoundDefs.xml` 里对应 grain 的注释即生效
- 心情差异靠运行时 pitch/volume 调制,只需录中性基准变体(每动作 2-3 个)
- 在模组设置「调制工作台」按音频特性差异拉 slider 补偿
```
review_en:
```text
```
review_zh:
```text
```
note:重写使其与当前实现对齐

### readme.configuration
en_value:
```text
Configuration
Three layers (bottom → top):
1. Default (author): `actions` / `moodMods` in `1.6/Patches/Ratkin_AddSqueakComp.xml`
2. Player override: mod settings "Modulation Workbench" — toggle a mood's override, slider + input + presets (Sharp/Neutral/Low/Chaos) + preview
3. Source switch: `Full override` (custom-only vs mixed with vanilla default)

Also includes a global trigger interval slider, a distance attenuation preset/custom control, and a `Scale trigger cooldown with time speed` checkbox (default ON). Accelerated play is quieted by reducing real-time trigger density rather than lowering each sound's volume. Select feedback uses a real-time cooldown, so it remains responsive while the game is paused.
```
zh_value:
```text
配置
三层(底→顶):
1. 默认(作者):`1.6/Patches/Ratkin_AddSqueakComp.xml` 的 `actions`/`moodMods`
2. 玩家 override:模组设置「调制工作台」,勾选心情 override,滑块+输入框+预设(尖锐/中性/低沉/混乱)+预览
3. 音源开关:`完全 override`(纯自定义 vs 混合原版 default)

另含全局触发间隔滑块、距离衰减预设/自定义控制,以及「按游戏倍速放大触发冷却」checkbox(默认开)。高倍速降噪通过降低现实时间触发密度实现,不再压低单次声音音量。选中反馈使用现实时间冷却,暂停时仍保持响应。
```
review_en:
```text
```
review_zh:
```text
```
note: README 目前尚未写入新 DebugAction 本地化开关，可视发布需要决定是否补充。重写使其与当前实现对齐

### readme.dev_menu
en_value:
```text
Dev Menu (development mode)
Developer menu → "Squeaky Ratkin": overlay text on/off ×2. Sound preview lives in the mod settings workbench.
```
zh_value:
```text
开发者菜单(开发者模式)
开发者菜单 → "Squeaky Ratkin":悬浮字开关 ×2。声音预览在模组设置工作台中进行。
```
review_en:
```text
```
review_zh:
```text
```
note: 当前 DevAction 本地化支持可由 ModSettings 开启；README 是否补充需决定。重写使其与当前实现对齐

### readme.license
en_value:
```text
License
Code (C# / XML defs / patches): Mozilla Public License 2.0 — file-level copyleft, project-level combinable with proprietary code.
Audio assets (custom audio under `1.6/Sounds/Squeak/`): All Rights Reserved (default, pending final confirmation). The vanilla guinea-pig audio is owned by Ludeon; this mod only references defName/clipFolderPath per Ludeon's mod policy — no vanilla assets are redistributed.
Third-party deps (Harmony / HAR / Ratkin): belong to their authors, under their own licenses.
```
zh_value:
```text
许可
代码(C#/XML Defs/Patches):Mozilla Public License 2.0 —— 文件级 copyleft(修改须开源),项目级可与闭源代码组合。
音频素材(`1.6/Sounds/Squeak/` 下自定义音频):All Rights Reserved(默认,待最终确认)。原版豚鼠音频归 Ludeon 所有,本模组仅按 Ludeon 模组政策引用 defName/clipFolderPath,不分发原版资产。
第三方依赖(Harmony/HAR/Ratkin):归各自作者,遵循各自许可。
```
review_en:
```text
```
review_zh:
```text
```
note:license的汉化应该寻找来源的官方版本，除非实在没有

### readme.development_build_pack
source: `README.md:43-80`, `README.zh-CN.md:43-80`

en_value:
```text
Development
Junction (recommended for local dev)
Make `RimWorld/Mods/SqueakyRatkin` a junction pointing at this workspace root so builds load instantly. `scripts/validate-junction.ps1` auto-checks before each build (warn-only, non-blocking).

Build
`dotnet build Source/SqueakyRatkin/SqueakyRatkin.csproj`
Must be 0 errors. A missing-junction WARNING is normal (non-blocking).

Build Flavor (for distribution)
`-p:SqueakyBuildFlavor=Dev|Steam|GitHub` toggles the startup-log banner (`[dev|steam|github]`). Runtime behavior is identical across flavors. Startup logs identify dev builds by commit, GitHub releases by tag plus commit, and Steam builds by package version.

Pack
Build the intended flavor first; pack scripts only stage/zip existing build output.
Dev local test: build Dev flavor, then `pwsh scripts/pack-dev.ps1` → `dist/dev/SqueakyRatkin/`.
Steam Workshop: build Steam flavor, then `pwsh scripts/pack-steam.ps1` → `dist/steam/SqueakyRatkin/`.
GitHub Release: CI/tag flow builds GitHub flavor, then `pwsh scripts/pack-github.ps1` → `dist/github/SqueakyRatkin-v<ver>.zip`.
Content includes only `About/`, `LoadFolders.xml`, `1.6/` (excludes source / pdb / docs).
```
zh_value:
```text
开发
Junction(本地开发推荐)
让 `RimWorld/Mods/SqueakyRatkin` 指向本工作区根,编译即加载,免手动复制。`scripts/validate-junction.ps1` 每次构建前自动校验(缺失只警告,不阻断)。

构建
`dotnet build Source/SqueakyRatkin/SqueakyRatkin.csproj`
必须 0 errors。junction 缺失的 WARNING 正常(非阻断)。

Build Flavor(分发用)
`-p:SqueakyBuildFlavor=Dev|Steam|GitHub`,影响启动日志 banner(`[dev|steam|github]`)。运行时功能三态相同。启动日志中 dev 版按提交号区分,GitHub 版按 tag+提交区分,Steam 版只显示包版本号。

打包
先构建目标 flavor;打包脚本只整理/压缩已有构建产物。
本地测试:先构建 Dev flavor,再 `pwsh scripts/pack-dev.ps1` → `dist/dev/SqueakyRatkin/`。
Steam 工坊:先构建 Steam flavor,再 `pwsh scripts/pack-steam.ps1` → `dist/steam/SqueakyRatkin/`。
GitHub Release:仅 CI/tag 流程构建 GitHub flavor,再 `pwsh scripts/pack-github.ps1` → `dist/github/SqueakyRatkin-v<ver>.zip`。
内容只含 `About/`、`LoadFolders.xml`、`1.6/`(排除源码/pdb/文档)
```
review_en:
```text
```
review_zh:
```text
```
note: 这里省略了 README 中 junction 选项的完整命令细节；如需审，可直接看源文件。重写使其与当前实现对齐

---

## 8. CONTRIBUTING 贡献指南

source: `CONTRIBUTING.md`

### contrib.audio_intro
value:
```text
本模组欢迎音频与代码两方面的贡献，下文分别说明。
This mod welcomes both audio and code contributions; each is covered below.

音频贡献（无需编程）· Audio (no coding required)
模组默认采用原版豚鼠的叫声——近似啾啾，但并非真正的鼠叫。若您能录制或合成更贴近真实鼠类的吱吱声，欢迎用以替换默认音源。
The default audio is the vanilla guinea-pig — close, but not a real rat. Recordings or synth of something more rat-like are welcome as replacements.
```
review_value:
```text
```
note:

### contrib.audio_submission
value:
```text
提交流程 / Submission
1. 按 `1.6/Sounds/Squeak/AUDIO_GUIDE.txt` 的要求录制与命名（要点：单声道 ogg、22050 Hz、峰值约 -3dBFS）。
   Record and name per `AUDIO_GUIDE.txt` (mono ogg, 22050 Hz, peak ~-3dBFS).
2. 选择一种方式提交 / submit via one of:
   - 熟悉 Git：文件置于 `1.6/Sounds/Squeak/<Action>/`，向 `dev` 分支发起 Pull Request。
     With Git: place files in `1.6/Sounds/Squeak/<Action>/`, PR to `dev`.
   - 不熟悉 Git：开启 Issue 并附上音频文件。
     Without Git: open an issue and attach the audio.
3. 维护者会接入游戏配置，并在发布说明中致谢。
   A maintainer wires it in and credits you on release.
```
review_value:
```text
```
note:

### contrib.audio_terms
source: `CONTRIBUTING.md:24-45`

value:
```text
术语通俗解释 · Plain-language terms
单声道（mono）
ogg
采样率（22050 / 44100 Hz）
音量归一化（-3dBFS）
去除静音（trim）
中性录制（neutral）
SoundDef / grain
太长不看：录制几声鼠叫（单声道、ogg、音量适中不爆音），按 `SR_动作_序号.ogg` 命名（例如 `SR_Call_1.ogg`），放入对应文件夹后提交，其余由维护者完成。
TL;DR: record a few rat squeaks (mono, ogg, not clipping/too quiet), name as `SR_<Action>_<n>.ogg` (e.g. `SR_Call_1.ogg`), place in the folder, submit. The maintainer handles the rest.
```
review_value:
```text
```
note: 详细解释见源文件，本表只列标题与 TL;DR。

### contrib.code_and_conventions
source: `CONTRIBUTING.md:49-90`

value:
```text
代码贡献 · Code
1. Fork 仓库并克隆至本地。Fork and clone.
2. 基于 `dev` 创建特性分支（请勿直接使用 main）：`git checkout dev && git checkout -b my-feature`
   Branch off `dev` (not main).
3. 确保编译通过：`dotnet build Source/SqueakyRatkin/SqueakyRatkin.csproj`，要求 0 错误。
   Build must pass with 0 errors.
4. 向 `dev` 发起 Pull Request，说明改动内容与原因。
   PR to `dev`, describe what and why.

编码规范 · Conventions
- Def 名称使用 `SR_` 前缀（避免全局数据库冲突）；C# 类名不加前缀（由命名空间隔离）。
- 所有面向用户的字符串须通过 Keyed 本地化，中英文同时提供。
- 默认不要扫描项目根目录之外的路径；确需外部参考时，必须先取得用户对具体路径的明确授权（详见 AGENTS.md）。
- 代码采用 MPL-2.0 协议，贡献代码同样适用。
```
review_value:
```text
```
note:

---

## 9. AUDIO_GUIDE 自定义音频指引

source: `1.6/Sounds/Squeak/AUDIO_GUIDE.txt`

### audioGuide.title_intro
value:
```text
鼠辈啁啾 · 自定义音频指引
本模组默认使用原版豚鼠（GuineaPig）的叫声作为音源。若需替换为自定义鼠叫，请遵循以下规范。
The default audio source is the vanilla guinea-pig. To replace it with custom squeaks, follow this guide.
```
review_value:
```text
```
note:

### audioGuide.naming
value:
```text
1. 命名规范 · Naming
文件名格式：SR_<动作>_<序号>.ogg
File name: SR_<Action>_<n>.ogg

动作类别共 9 种 / 9 actions:
  Call（空闲）   Eat（进食）    Sleep（睡眠）   Wounded（受击）
  Select（选中） Move（移动）   Social（社交）  Joy（娱乐）
  Death（死亡）

序号为变体编号，从 1 起递增。
示例 / examples:
  SR_Call_1.ogg
  SR_Call_2.ogg
  SR_Wounded_1.ogg
  SR_Eat_1.ogg

动作名称必须与游戏内 SoundDef（SR_<动作>）及文件夹路径（Squeak/<动作>）一致，否则无法接入。
The <Action> segment must match the SoundDef (SR_<Action>) and the folder path (Squeak/<Action>).
```
review_value:
```text
```
note: 当前已包含 Death。

### audioGuide.format
value:
```text
2. 技术规格 · Format
- 声道：单声道（mono），必需。游戏通过单声道实现距离衰减，立体声会破坏此效果。
  Channels: mono, mandatory. Distance fading relies on mono; stereo breaks it.
- 格式：ogg（推荐，与原版及多数音效模组一致，兼容性最佳）；wav 亦可。请勿使用 mp3。
  Format: ogg (recommended; vanilla and most mods use it); wav acceptable. Avoid mp3.
- 采样率：22050 Hz 或 44100 Hz。
  Sample rate: 22050 Hz or 44100 Hz.
- 音量：峰值归一化至约 -3dBFS，避免削波失真。
  Loudness: normalize peak to ~-3dBFS, no clipping.
- 首尾静音：裁剪干净，避免触发延迟。
  Trim leading/trailing silence to avoid trigger delay.
- 时长：0.3–1.5 秒。睡眠与移动为偶发性一次性音效，非循环长音。
  Duration: 0.3–1.5s. Sleep and Move are occasional one-shots, not loops.
```
review_value:
```text
```
note:

### audioGuide.placement_enable_sourceSwitch
value:
```text
3. 文件位置 · Placement
1.6/Sounds/Squeak/<动作>/SR_<动作>_<序号>.ogg
1.6/Sounds/Squeak/<Action>/SR_<Action>_<n>.ogg

4. 心情处理 · Mood
游戏中的四种心情（好/中/坏/崩溃）由运行时通过音高与音量调制实现（见模组设置「调制工作台」）。
Mood (good/neutral/bad/breakdown) is produced at runtime via pitch/volume modulation (mod settings → Modulation Workbench).

5. 启用方式 · Enable
编辑 1.6/Defs/SoundDefs/SqueakyRatkin_SoundDefs.xml。
找到对应的 SR_<动作> 与 SR_<动作>_Pure，取消下列行的注释：
  <li Class="AudioGrain_Folder"><clipFolderPath>Squeak/<动作></clipFolderPath></li>

6. 音源开关 · Source switch
模组设置中的「完全 override」/ mod settings → "Full override":
- 关闭（默认）：自定义音频与原版豚鼠混合随机播放。
- 开启：仅使用自定义音频，不播放豚鼠（走 SR_<动作>_Pure）。
```
review_value:
```text
```
note: 这里保留原文中的 `<动作>` 占位符格式。

---

## 10. XML 注释文案

### xmlComment.patch.overview
source: `1.6/Patches/Ratkin_AddSqueakComp.xml:2-7`

value:
```text
把发声组件挂到 Ratkin 种族上,并配置:
  actions  - 各动作触发模式/间隔/概率(数据驱动)
  moodMods - 心情→音色调制参数(运行时作用于 pitchFactor/volumeFactor)
调行为/心情音色只需改本文件,无需重编译 DLL。
```
review_value:
```text
```
note:

### xmlComment.soundDefs.overview
source: `1.6/Defs/SoundDefs/SqueakyRatkin_SoundDefs.xml:2-9`

value:
```text
Squeaky Ratkin(鼠辈啁啾) SoundDef —— 每动作 1 个(中性基准),心情靠运行时 pitchFactor/volumeFactor 调制(见 CompSqueaker + moodMods)。
两套: SR_<Action>(混合:豚鼠default+自定义override共存) / SR_<Action>_Pure(纯自定义override)。
开关 CompSqueaker.UseCustomOnly: ON 用 _Pure, OFF 用混合。
default: 原版啮齿类豚鼠(GuineaPig)。自定义音频就位后取消 override 注释 / 改 _Pure grain。
自定义音频约定: 放 1.6/Sounds/Squeak/<Action>/, 文件名 SR_<Action>_<n>.ogg, 单声道。
pitchRange 用中性0.95~1.05, 容许 SoundDef 层基础随机; 心情调制由运行时 pitchFactor 叠加。
```
review_value:
```text
```
note:

### xmlComment.soundDefs.overrideTemplate
source: `1.6/Defs/SoundDefs/SqueakyRatkin_SoundDefs.xml:11,13,15,17,19,21,23,25,27`

value:
```text
override 自定义(放 SR_<Action>_*.ogg): <li Class="AudioGrain_Folder"><clipFolderPath>Squeak/<Action></clipFolderPath></li>
```
review_value:
```text
```
note: 此模板在 Call/Eat/Sleep/Wounded/Select/Move/Social/Joy/Death 各出现一次；当前 value 按模板归并。

### xmlComment.soundDefs.pureTemplate
source: `1.6/Defs/SoundDefs/SqueakyRatkin_SoundDefs.xml:12,14,16,18,20,22,24,26,28`

value:
```text
自定义音频就位后改为纯 Squeak/<Action>: <li Class="AudioGrain_Folder"><clipFolderPath>Squeak/<Action></clipFolderPath></li>
```
review_value:
```text
```
note: 此模板在 `_Pure` SoundDef 各出现一次；当前 value 按模板归并。

### xmlComment.soundDefs.preview
source: `1.6/Defs/SoundDefs/SqueakyRatkin_SoundDefs.xml:30`

value:
```text
Preview SoundDefs: onCamera=true,工作台试听用,无 distRange 距离衰减(听者恒=声源=相机)。游戏内从不引用,仅工作台预览按钮用。pitchRange 中性(0.95~1.05)让工作台 pitchFactor 调制效果明显。
```
review_value:
```text
```
note:

---

## 11. 代码中可见文本与本地化状态

### code.debugAction.attributes
source: `Source/SqueakyRatkin/Debug/SqueakDebugActions.cs:9,16,23,30`

value:
```text
Squeaky Ratkin
Overlay: ON
Overlay: OFF
Camera Indicator: ON
Camera Indicator: OFF
```

review_value:
```text
```

note: C# attribute 仍需编译期常量；实际菜单显示由 `Patch_DebugTabMenu_Actions` 在节点生成时按 `DebugAction_*` / `DebugActionCategory_*` key 替换。该替换受 ModSettings `localizeDebugActions` 控制，默认关闭，重开调试动作菜单后生效。

### code.startupLogs
source: `Source/SqueakyRatkin/Mod.cs`

value:
```text
[SqueakyRatkin] startup/build diagnostic log templates
```

review_value:
```text
```

note: 开发者日志按项目约定硬编码英文，不作为玩家本地化项。

---

## 12. 已检查但不列为待翻译

### excluded.loadFolders
source: `LoadFolders.xml`

value:
```text
/, 1.6
```
reason:
```text
加载目录声明，不是自然语言文案。
```

### excluded.aboutTechnicalMetadata
source: `About/About.xml`

value:
```text
packageId, author, url, steamWorkshopUrl, loadAfter
```
reason:
```text
技术元数据，不翻译。
```

### excluded.patchData
source: `1.6/Patches/Ratkin_AddSqueakComp.xml`

value:
```text
action, mode, minIntervalTicks, probabilityPerCheck, cooldownClock, moodMods, distancePresets
```
reason:
```text
配置数据，不是文案。
```

### excluded.soundDefWiring
source: `1.6/Defs/SoundDefs/SqueakyRatkin_SoundDefs.xml`

value:
```text
defName, context, clipFolderPath, volumeRange, pitchRange, distRange, onCamera
```
reason:
```text
音频接线与技术配置，不是文案。
```

### excluded.internalAgentFiles
source: `MEMORY.md`, `TODO.md`, `AGENTS.md`, `OBLIVIONIS.md`

value:
```text
agent 协作文本
```
reason:
```text
内部协作文件，不属于玩家/发布文案。
```
