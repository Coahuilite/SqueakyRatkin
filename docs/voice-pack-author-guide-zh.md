# Squeaky Ratkin 语音包作者指南

面向 **RimWorld 1.6** 的独立语音包作者。本文说明如何为 [Squeaky Ratkin](https://github.com/Coahuilite/SqueakyRatkin) 制作可分发、零 C# 的语音包。

## 快速指南：先做一个可播放的 Race 语音包

1. 复制 `Extras/SqueakyRatkinExampleVoices/`，把副本作为独立模组。
2. 替换自己的 `packageId`、名称、作者、描述和许可；替换全部 `SR_ExampleTemplate_*` token，且发布后保持 `packageId` 与 PackDef DefName 稳定。
3. 首先只做 **Race + Call**，不要一开始制作全部动作或 Xenotype 变体。
4. 将音频放在 `<lowercase packageId>/<PackDef.defName>/Call/`；推荐 OGG Vorbis，不要仅改扩展名伪装转码。
5. 建立一个 `scope=Race` 的 PackDef，列出 `Call`；其 SoundDef 必须以 `SR_` 开头、`sustain=false`、`MapOnly`，并至少含一个 grain。
6. 启用 Squeaky Ratkin、NewRatkinPlus 和你的包；在设置中选 **FALLBACK**，启用 Race PackDef 后实测鼠族的 `Call`。
7. 确认移除该 `Call` 后仍会按 Xenotype → Race → Vanilla 回退，而不是把未覆盖动作当作静音。
8. 发布前改用自己的稳定身份，声明音频权利；不得打包 RimWorld 原版音频或其他无授权素材。

**完成标准：** 包能被发现并手动启用，Race `Call` 在 FALLBACK 实际播放，目录、`clipFolderPath` 和 PackDef 身份一致，且发布物只含有权分发的音频。详细 XML、Xenotype、格式和发布要求见下文第 1–6 节。

## 1. 前置依赖、回退与范围

玩家需要同时启用 Squeaky Ratkin 和 NewRatkinPlus（`Solaris.RatkinRaceMod`）。Race 语音包不需要 Biotech；Xenotype 语音包是 Biotech 启用时的可选增强，应在自己的 `About.xml` 与加载规则中排序到 Biotech 和目标异种来源模组之后。不要把这些可选扩展依赖当成主模组依赖。

主模组提供内置 Vanilla 回退层，其具体音池由各 `SR_<Action>` SoundDef 定义。自定义包只提供独立音频：不要修改主模组 SoundDef，不要把音频安装进主模组目录；原版资产只可按 Def/路径机制引用，不得重新分发。

每个 `SqueakVoicePackDef` 都是一个独立选择项、权重单位和校验单位：只能是 Race，或只能对应一个 Xenotype。

| 范围 | 必需字段 | 运行时关系 |
| --- | --- | --- |
| `Race` | `<scope>Race</scope>`，**不写** `targetDefName` | Xenotype 未命中或不可用时的回退层 |
| `Xenotype` | `<scope>Xenotype</scope>` 与一个 `targetDefName` | Biotech 启用时按目标精确匹配 |
| 两者 | 同一发行包可含多个独立 Def | Xenotype 缺动作时可回退 Race |

`targetDefName` 仅表示精确且区分大小写的 `XenotypeDef.defName`，例如 `RK_XenoType_Ratkin`；不要翻译、改大小写或写成 XML 强引用。显示名、图标和本地化不参与匹配或保存。缺少 Biotech 时 Xenotype 层不解析，Race 仍会回退 Vanilla。

## 2. 身份、目录与完整最小 XML 示例

从 `Extras/SqueakyRatkinExampleVoices/` 复制开始。它是独立 Race-only Template；不得冒用其 `packageId`、作者身份或 `SR_ExampleTemplate_*` 名称。替换自己的 `packageId`、名称、作者、描述、许可、所有 DefName token、`clipFolderPath` 和实际音频目录。全部作者资源根固定为：`<lowercase packageId>/<PackDef.defName>/<Action>/`。

以下示例使用 `MyStudio`，只提供 `Call`，但包含完整的 Race + Xenotype 结构。发布前将示例身份全部替换为自己的稳定身份。

```text
MyStudioRatkinVoices/
|- About/About.xml
|- LoadFolders.xml
`- 1.6/
   |- Race/
   |  |- Defs/SoundDefs/MyStudio_Race_Sounds.xml
   |  `- Sounds/com.example.mystudio.ratkinvoices/SR_MyStudio_Race/Call/call_01.ogg
   `- Biotech/
      |- Defs/SoundDefs/MyStudio_Xenotype_Sounds.xml
      `- Sounds/com.example.mystudio.ratkinvoices/SR_MyStudio_DefaultRatkinXenotype/Call/call_01.ogg
```

### `About/About.xml`

```xml
<?xml version="1.0" encoding="utf-8"?>
<ModMetaData>
  <packageId>com.example.mystudio.ratkinvoices</packageId>
  <name>My Studio Ratkin Voices</name>
  <author>My Studio</author>
  <supportedVersions><li>1.6</li></supportedVersions>
  <description>Independent voice pack for Squeaky Ratkin.</description>
  <modDependencies>
    <li><packageId>coahuilite.squeakyratkin</packageId><displayName>Squeaky Ratkin</displayName></li>
    <li><packageId>Solaris.RatkinRaceMod</packageId><displayName>NewRatkinPlus</displayName></li>
  </modDependencies>
  <loadAfter>
    <li>coahuilite.squeakyratkin</li>
    <li>Solaris.RatkinRaceMod</li>
    <li>ludeon.rimworld.biotech</li>
  </loadAfter>
</ModMetaData>
```

Race-only 包可省略 Biotech 的 `loadAfter`；专门面向第三方 Xenotype 的包还应声明该来源模组。

### `LoadFolders.xml`

```xml
<?xml version="1.0" encoding="utf-8"?>
<loadFolders>
  <v1.6>
    <li IfModActive="coahuilite.squeakyratkin">1.6/Race</li>
    <li IfModActive="ludeon.rimworld.biotech">1.6/Biotech</li>
  </v1.6>
</loadFolders>
```

Race 内容独立于 Biotech 加载；Xenotype 内容仅在 Biotech 启用时读取。`targetDefName` 是普通字符串，目标缺失时不会形成 XML cross-ref 红字。

### `1.6/Race/Defs/SoundDefs/MyStudio_Race_Sounds.xml`

```xml
<?xml version="1.0" encoding="utf-8"?>
<Defs>
  <SqueakyRatkin.SqueakVoicePackDef>
    <defName>SR_MyStudio_Race</defName>
    <scope>Race</scope>
    <actions><li><action>Call</action><sounds><li>SR_MyStudio_Race_Call</li></sounds></li></actions>
  </SqueakyRatkin.SqueakVoicePackDef>
  <SoundDef>
    <defName>SR_MyStudio_Race_Call</defName>
    <sustain>false</sustain>
    <context>MapOnly</context>
    <subSounds><li><grains><li Class="AudioGrain_Folder">
      <clipFolderPath>com.example.mystudio.ratkinvoices/SR_MyStudio_Race/Call</clipFolderPath>
    </li></grains><volumeRange>45~55</volumeRange><pitchRange>0.95~1.05</pitchRange><distRange>15~70</distRange></li></subSounds>
  </SoundDef>
</Defs>
```

### `1.6/Biotech/Defs/SoundDefs/MyStudio_Xenotype_Sounds.xml`

```xml
<?xml version="1.0" encoding="utf-8"?>
<Defs>
  <SqueakyRatkin.SqueakVoicePackDef>
    <defName>SR_MyStudio_DefaultRatkinXenotype</defName>
    <scope>Xenotype</scope>
    <targetDefName>RK_XenoType_Ratkin</targetDefName>
    <actions><li><action>Call</action><sounds><li>SR_MyStudio_Xenotype_DefaultRatkin_Call</li></sounds></li></actions>
  </SqueakyRatkin.SqueakVoicePackDef>
  <SoundDef>
    <defName>SR_MyStudio_Xenotype_DefaultRatkin_Call</defName>
    <sustain>false</sustain>
    <context>MapOnly</context>
    <subSounds><li><grains><li Class="AudioGrain_Folder">
      <clipFolderPath>com.example.mystudio.ratkinvoices/SR_MyStudio_DefaultRatkinXenotype/Call</clipFolderPath>
    </li></grains><volumeRange>45~55</volumeRange><pitchRange>0.95~1.05</pitchRange><distRange>15~70</distRange></li></subSounds>
  </SoundDef>
</Defs>
```

`FloatRange` 使用 `~`，不要写成逗号或圆括号。每个 DefName 必须以 `SR_` 开头并带自己的包 token。

### 生产 SoundDef 强制契约

固定的 15 个生产 Action 都是 **one-shot**。每个被 PackDef 引用的 SoundDef 必须有 `SR_` 前缀、`<sustain>false</sustain>`、`<context>MapOnly</context>`，至少一个 SubSound，且每个 SubSound 至少一个 grain；`onCamera` 必须省略或为 `false`。禁止 `sustain=true`、loop、camera/map context 混用和依赖状态维持的长音频。违反任一项时，整个 PackDef 会被拒绝，Race/Vanilla 仍可回退。

一个 Action 目录可放多个真实文件，由 `AudioGrain_Folder` 收集并随机选择；通常不必为每个 clip 建 SoundDef。不要留下空文件、静音占位或不打算播放的素材。Workbench 单 clip 试听不证明生产 SoundDef 符合此契约。

## 3. 固定 Action 与部分覆盖

| Action | 含义与素材建议 |
| --- | --- |
| `Call` | 一般呼叫或短叫声 |
| `Eat` | 进食时的轻短声 |
| `Sleep` | 睡眠/休息时的轻声 |
| `Wounded` | 受伤、疼痛反应 |
| `Select` | 玩家选中反馈 |
| `Move` | 移动时偶发的短声 |
| `Social` | 社交互动反应 |
| `Joy` | 娱乐或愉快状态 |
| `Death` | 死亡反应；宜短且避免过响 |
| `Draft` | 被征召反馈 |
| `Undraft` | 取消征召反馈 |
| `Attack` | 合资格的普通攻击成功反馈 |
| `Work` | 工作指令/工作状态反馈 |
| `Equip` | 玩家主动装备武器或工具反馈 |
| `MentalBreak` | 精神崩溃开始反应 |

允许部分覆盖：只提供 `Call` 也有效。未覆盖动作按模式回退：**FALLBACK** 为 Xenotype → Race → Vanilla；**REMIX** 在当前可播放的 Xenotype、Race、Vanilla 层间等权选择；**OFF** 仅用 Vanilla。语音包只能提供声音，不能改触发、心情、频率、距离或 Action 范围。

## 4. 音频处理与格式

推荐最终交付 **OGG Vorbis、mono 和合理采样率**。官方 Template 使用 22050 Hz mono，可作质量与体积参考，但不强制第三方采用该采样率。WAV 可用于中间审阅或兼容，但不是推荐发布格式；MP3 不推荐。不要只改扩展名伪装转码。

- 人工试听并按游戏语境分类；不确定素材先保留在未分类区，不要仅按文件名自动分类。
- 剪掉明显首尾静音、点击声和无意义空白，做响度处理并避免 clipping；较长 non-sustain clip 仍可能与后续事件重叠。
- 主模组会运行时调制心情音高和音量，不需要制作情绪矩阵。
- 批处理前先备份；不要覆盖正式 Template，也不要把未备份素材直接批量改名。

如已安装 ffmpeg，可按自己的环境实际替换文件名：

```powershell
ffmpeg -i ".\Raw\call source.wav" -ar 22050 -ac 1 -c:a libvorbis -q:a 5 ".\Reviewed\call_01.ogg"
ffprobe -v error -show_entries stream=codec_name,sample_rate,channels -of default=noprint_wrappers=1 ".\Reviewed\call_01.ogg"
```

## 5. 安装、最小测试与常见问题

1. 将包作为独立 RimWorld 模组安装，排序在 NewRatkinPlus 与 Squeaky Ratkin 之后；Xenotype 内容还应在 Biotech 和目标来源之后。
2. 在设置中选择 **FALLBACK**，并在 Race 或对应 Xenotype 范围手动启用 PackDef；发现包不等于自动启用。
3. 先测试 Race `Call`：目录和 `clipFolderPath` 都必须是 `<lowercase packageId>/<PackDef.defName>/Call/`。确认实际播放后，再移除该动作确认回退；随后再测 Xenotype 与部分覆盖。

| 现象 | 处理 |
| --- | --- |
| Xenotype 不匹配 | 检查 `targetDefName` 是否等于实际 `XenotypeDef.defName`，包括大小写；不要用显示名。 |
| 有包但仍听到 Vanilla | 检查模式不是 OFF、PackDef 已启用、动作已覆盖，且目录有可播放文件。 |
| 动作静音或 PackDef 被拒绝 | 对照第 2 节的 SoundDef 强制契约，检查目录、`clipFolderPath`、格式和空/损坏文件。 |
| XML FloatRange 失败 | `volumeRange`、`pitchRange`、`distRange` 使用 `~`。 |
| 与其他包串音 | 每个 DefName 和资源根都使用自己的稳定包 token。 |

本文说明 XML 和文件布局契约，不替代作者在自己的目标模组组合与发行环境中的实机测试。

## 6. 发布检查清单与许可

- [ ] 使用自己的 `packageId`、名称、作者和包 token；每个 DefName 以 `SR_` 开头且全局唯一。
- [ ] Race Def 不含 `targetDefName`；每个 Xenotype Def 只有一个 exact、case-sensitive 目标字符串，并由 Biotech 加载规则门控。
- [ ] `clipFolderPath`、实际目录和 `<lowercase packageId>/<PackDef.defName>/<Action>/` 规则一致；已列 Action 都有可播放音频。
- [ ] 每个生产 SoundDef 满足 `sustain=false`、`MapOnly`、带 grain 的 SubSound、无 camera/loop/状态维持播放等强制契约。
- [ ] 已在 OFF、FALLBACK、REMIX 下检查选择与回退；Workbench 试听不代替实际触发测试。
- [ ] 音频已人工试听、无空文件或意外素材，并完成适当的剪辑和响度处理。
- [ ] 已声明自己的音频许可与署名，只分发有权分发的内容。

Squeaky Ratkin 的代码采用 MPL-2.0；你的音频及语音包文本的许可由你自行决定和声明。不要重新分发 RimWorld 原版音频或其他无授权素材。
