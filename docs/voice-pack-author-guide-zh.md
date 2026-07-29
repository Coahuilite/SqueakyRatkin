# Squeaky Ratkin 语音包作者指南

面向 **RimWorld 1.6** 的独立语音包作者。本文说明如何为 [Squeaky Ratkin](https://github.com/Coahuilite/SqueakyRatkin) 制作可分发、零 C# 的语音包。

## 1. 前置依赖与目标

语音包需要玩家同时启用：

- Squeaky Ratkin；
- NewRatkinPlus（`Solaris.RatkinRaceMod`）。

**Race 范围**语音包不需要 Biotech；**Xenotype 范围**语音包是 Biotech 可用时的可选增强，并且应在自己的 `About.xml` 和加载规则中声明/排序到 Biotech 及目标异种来源模组之后。不要把这些扩展包依赖当成主模组的依赖。

主模组提供原版豚鼠（Vanilla）回退、预览播放通路和一个普通内置 Race Example；自定义音频仍应作为独立模组提供，不要修改主模组的 SoundDef，也不要把音频安装进主模组目录。

## 2. 先复制示例包，再改成自己的包

从 `Extras/SqueakyRatkinExampleVoices/` 复制一份开始。它是可直接启用的 Race-only Template，拥有独立 `packageId`、PackDef、SoundDefs、PackKey/Catalog 身份和资源根；它不 Patch、覆盖或复用主模内置 Example。

复制后，**必须**全部替换为自己的身份：

- `About/About.xml` 中的 `packageId`、模组名称和作者；
- 所有 `SR_ExampleTemplate_*` DefName；改成带你自己的包 token 的全局唯一名称，例如 `SR_MyStudio_Race_Call`；
- 所有 `clipFolderPath` 中的 Template token，以及对应的实际音频目录；路径统一写为 `<lowercase packageId>/<PackDef.defName>/<Action>/`；
- 自己的描述、许可与发布信息。

不得冒用 `coahuilite.squeakyratkin.examplevoices`、Coahuilite 身份或 `SR_ExampleTemplate_*` 名称。你的派生包必须使用自己的稳定身份。发布后请保持自己的 `packageId` 和 PackDef 的 DefName 不变，以免玩家已保存的选择失联。

## 3. 选择范围：Race、Xenotype，或两者都做

每个 `SqueakVoicePackDef` 只表示一个选择项、一个权重单位和一个校验单位：一个 Def 只能是 Race，或只能对应一个 Xenotype，不能混合多个范围或目标。

| 范围 | 用途 | 需要的字段 | 运行时关系 |
| --- | --- | --- | --- |
| `Race` | 覆盖全部 Ratkin 的通用语音 | `<scope>Race</scope>`，**不写** `targetDefName` | 是 Xenotype 未命中或不可用时的回退层 |
| `Xenotype` | 只为一个异种提供特色语音 | `<scope>Xenotype</scope>` 和一个 `targetDefName` 字符串 | Biotech 可用时按目标精确匹配 |
| 两者 | 同一发行包可分别放一个 Race Def 和若干 Xenotype Def | 每个 Def 仍独立 | Xenotype 层可覆盖动作；缺动作时可回退 Race |

`targetDefName` 的唯一技术含义是目标 `XenotypeDef.defName`：**完全一致、区分大小写**。例如 `RK_XenoType_Ratkin` 不能翻译、不能改小写，也不能写 `XenotypeDef` XML 强引用。

本地化名称、`LabelCap` 和图标仅用于设置页显示与搜索；它们不参与目标匹配、选择保存或恢复。实际带有 `CompSqueaker` 的 Ratkin pawn 当前 Xenotype `defName` 才是运行时权威。缺少 Biotech 时不会解析 Xenotype Def 或 pawn 基因，Race 仍可用并继续回退 Vanilla。

## 4. 目录与完整最小 XML 示例

以下示例使用包 token `MyStudio`。实际发布前，请把 `com.example.mystudio.ratkinvoices`、`My Studio` 和 `MyStudio` 全部替换成自己的真实身份。为便于阅读，示例只为 `Call` 提供声音；它是完整、可加载的最小 Race + Xenotype 包。其余动作按相同结构追加即可。

目录树与所有 `clipFolderPath` 的唯一资源根规则是 `<lowercase packageId>/<PackDef.defName>/<Action>/`。因此本例 Race 的 `Call` 目录是 `1.6/Race/Sounds/com.example.mystudio.ratkinvoices/SR_MyStudio_Race/Call/`，Xenotype 的 `Call` 目录是 `1.6/Biotech/Sounds/com.example.mystudio.ratkinvoices/SR_MyStudio_DefaultRatkinXenotype/Call/`；下方目录树严格采用这一规则。

```text
MyStudioRatkinVoices/
|- About/About.xml
|- LoadFolders.xml
`- 1.6/
   |- Race/
   |  |- Defs/SoundDefs/MyStudio_Race_Sounds.xml
   |  `- Sounds/com.example.mystudio.ratkinvoices/SR_MyStudio_Race/Call/
   |     |- call_01.ogg
   |     `- call_02.ogg
   `- Biotech/
      |- Defs/SoundDefs/MyStudio_Xenotype_Sounds.xml
      `- Sounds/com.example.mystudio.ratkinvoices/SR_MyStudio_DefaultRatkinXenotype/Call/
         `- call_01.ogg
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

Race 内容的硬依赖只有主模组和 NewRatkinPlus。若你的发行包只有 Race 内容，可省略 `loadAfter` 中的 Biotech。Xenotype 内容应由下面的加载规则单独门控；如果你的包专门面向第三方 Xenotype，也应在自己的元数据中声明那个来源模组。

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

这会让 Race 内容独立于 Biotech 加载，Xenotype 内容只在 Biotech 启用时读取。`targetDefName` 是普通字符串，不会形成缺目标时的 XML cross-ref 红字。

### `1.6/Race/Defs/SoundDefs/MyStudio_Race_Sounds.xml`

```xml
<?xml version="1.0" encoding="utf-8"?>
<Defs>
  <SqueakyRatkin.SqueakVoicePackDef>
    <defName>SR_MyStudio_Race</defName>
    <scope>Race</scope>
    <actions>
      <li><action>Call</action><sounds><li>SR_MyStudio_Race_Call</li></sounds></li>
    </actions>
  </SqueakyRatkin.SqueakVoicePackDef>

  <SoundDef>
    <defName>SR_MyStudio_Race_Call</defName>
    <sustain>false</sustain>
    <context>MapOnly</context>
    <subSounds><li><grains><li Class="AudioGrain_Folder">
      <clipFolderPath>com.example.mystudio.ratkinvoices/SR_MyStudio_Race/Call</clipFolderPath>
    </li></grains><volumeRange>45~55</volumeRange><pitchRange>0.95~1.05</pitchRange>
    <distRange>15~70</distRange></li></subSounds>
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
    <actions>
      <li><action>Call</action><sounds><li>SR_MyStudio_Xenotype_DefaultRatkin_Call</li></sounds></li>
    </actions>
  </SqueakyRatkin.SqueakVoicePackDef>

  <SoundDef>
    <defName>SR_MyStudio_Xenotype_DefaultRatkin_Call</defName>
    <sustain>false</sustain>
    <context>MapOnly</context>
    <subSounds><li><grains><li Class="AudioGrain_Folder">
      <clipFolderPath>com.example.mystudio.ratkinvoices/SR_MyStudio_DefaultRatkinXenotype/Call</clipFolderPath>
    </li></grains><volumeRange>45~55</volumeRange><pitchRange>0.95~1.05</pitchRange>
    <distRange>15~70</distRange></li></subSounds>
  </SoundDef>
</Defs>
```

`FloatRange` 使用 `~`，如 `45~55`、`0.95~1.05` 和 `15~70`，不要写成 `(45,55)`。所有 DefName 都必须以 `SR_` 开头并包含你的包 token。

一个动作目录可放多个真实音频文件；`AudioGrain_Folder` 会收集目录中的所有受支持 clip 并随机选择。通常每个动作只需要一个 SoundDef，让一个目录收集多文件；不要为每个 clip 建一个 SoundDef，除非你有意要把它们拆成不同的 SoundDef 候选。Template 的 Xenotype 骨架是不可加载的 TXT 指引，不是静音 placeholder；将自己的真实音频放入已加载范围下与 `clipFolderPath` 对应的 Action 目录，不能留下空文件、静音占位或混入不打算播放的素材。

### 生产音频强制契约

全部固定的 15 个生产 Action 都是 **one-shot**。每个被 PackDef 引用的 `SoundDef` 必须有 `SR_` 前缀、`<sustain>false</sustain>`、`<context>MapOnly</context>`，且至少一个 `SubSound`。每个 SubSound 必须有至少一个 grain，并且必须省略 `onCamera` 或明确写成 `<onCamera>false</onCamera>`；不得混合 camera 与 map context。禁止 `sustain=true`、loop，或任何依靠状态维持的长音频。违反任一项时，整个 PackDef 都会被运行时拒绝，Race/Vanilla 仍可正常回退。

Workbench 的单 clip 试听只证明该 clip 可被试听，**不**证明生产 SoundDef 符合上述结构或可进入 VoicePack 候选。较长但 non-sustain 的 clip 仍是 one-shot；它可能因多个事件而重叠，请自行剪辑并控制素材长度。

## 5. 固定的 15 个 Action 与部分覆盖

可提供的固定 Action 为：

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

允许**部分覆盖**：你的 PackDef 不必列全 15 项。例如只制作 `Call`、`Select` 和 `Wounded` 也有效。缺失动作会继续依模式回退：FALLBACK 依次 Xenotype → Race → Vanilla；REMIX 只在当前实际可播放的 Xenotype、Race、Vanilla 层间等权选择；OFF 始终只用 Vanilla。一个 PackDef 只提供声音，不能改触发行为、心情、频率、距离或 Action 范围。

## 6. 从杂乱素材到 Action 的人工试听流程

不要仅凭文件名自动分类。对来源不统一、命名杂乱的素材，建议先建立仓库外或包内的原始素材备份，再按以下流程人工试听：

1. 建立 `Raw/`、`Reviewed/` 和按 Action 划分的目标目录；原始素材只读保留，不在原处批量改名或覆盖。
2. 逐个试听，在 `Reviewed/` 记录来源文件、时长、听感与建议 Action；不确定的先放 `Unsorted/`。
3. 以游戏语境分类：疼痛放 `Wounded`，死亡反应放 `Death`，点击反馈放 `Select`，不要因为都是“尖叫”就全部放进 `Call`。
4. 同一 clip 只在确有不同语境价值时复制到多个 Action；否则避免重复，减少随机时的重复感。
5. 在每个 Action 目录实际试听并调平响度后，再写入 SoundDef；可以先只覆盖最有把握的几个 Action。

建议先做 Race 的少量动作，再按需要增加 Xenotype 变体。Race 是 Xenotype 不可用、未提供该动作或未被选中时的重要 fallback，不应只依赖 Xenotype 包。

## 7. 音频格式与处理建议

推荐最终交付 **OGG Vorbis、mono 和合理采样率**。官方 Template 当前使用 22050 Hz mono，可作为质量与体积取舍的参考，但不强制第三方包使用该采样率。mono 也适合地图内距离衰减；无论采样率为何，都应保持干净的起止和不过载的电平。

RimWorld/当前加载链可兼容 WAV，因此 WAV 可作为中间审阅或兼容格式，但不是本指南推荐的最终交付格式。不要仅靠改扩展名把 WAV 伪装成 OGG；这不会转码。MP3 不推荐，也不作为本指南的交付格式。

- 剪掉明显的首尾静音、录音点击声和无意义空白；保留少量自然起音即可。
- 先做响度归一化，再用峰值检查避免 clipping；不要为了“更响”让波形削顶。
- 保持素材的中性基础音高和音量。主模组会在运行时按心情调制，不需要制作“开心/悲伤/愤怒”矩阵。
- 一次性短音通常更适合这些 Action；把过长的素材剪成有清晰起止的片段。较长的 non-sustain clip 仍是 one-shot，但可能和后续事件重叠。

### 可选：使用 ffmpeg 批量转码

本文不要求安装 ffmpeg。只有当你已经自行安装并希望批处理时，先在 PowerShell 检查命令可用：

```powershell
ffmpeg -version
ffprobe -version
```

两条命令都能显示版本后，以下 Windows 示例把一个来源文件编码为推荐的 OGG Vorbis；请按实际路径替换，并先保留源文件。`-q:a 5` 是适中的 Vorbis 质量设定，可按素材和发行体积自行调整：

```powershell
ffmpeg -i ".\Raw\call source.wav" -ar 22050 -ac 1 -c:a libvorbis -q:a 5 ".\Reviewed\call_01.ogg"
ffprobe -v error -show_entries stream=codec_name,sample_rate,channels -of default=noprint_wrappers=1 ".\Reviewed\call_01.ogg"
```

若系统找不到命令，请安装并配置自己的音频工具后再执行；不要把“本机已安装 ffmpeg”当作本指南的前提。

## 8. 安全复制与规范重命名脚本

以下 PowerShell 脚本从一个已人工分类的来源目录复制 **OGG** 音频到你的 Action 目录，并将文件规范命名为 `call_01.ogg`、`call_02.ogg` 等。它会先验证源目录和目标父目录；目标 Action 目录不存在时会创建。脚本只选择来源目录顶层的 OGG 文件：没有 OGG 时失败，其他格式不会被复制。默认遇到同名目标文件时失败，**不会覆盖**原始素材或既有目标；只有明确传入 `-Force` 才允许覆盖同名目标。复制不转码：若需要将 WAV 审阅素材用于最终交付，请先用上节或其他工具实际编码为 OGG，再运行脚本；不要把 WAV 重命名为 `.ogg`。

将脚本保存为 `Copy-VoiceAction.ps1`，然后在语音包根目录运行。

```powershell
[CmdletBinding()]
param(
    [Parameter(Mandatory)] [string] $SourceDirectory,
    [Parameter(Mandatory)] [string] $DestinationDirectory,
    [Parameter(Mandatory)] [string] $Prefix,
    [switch] $Force
)

$source = Resolve-Path -LiteralPath $SourceDirectory -ErrorAction Stop
if (-not (Test-Path -LiteralPath $source -PathType Container)) {
    throw "SourceDirectory is not a directory: $SourceDirectory"
}

$destinationParent = Split-Path -Parent $DestinationDirectory
if ([string]::IsNullOrWhiteSpace($destinationParent) -or
    -not (Test-Path -LiteralPath $destinationParent -PathType Container)) {
    throw "Destination parent must already exist: $destinationParent"
}

if (-not (Test-Path -LiteralPath $DestinationDirectory -PathType Container)) {
    New-Item -ItemType Directory -Path $DestinationDirectory -ErrorAction Stop | Out-Null
}

$files = @(Get-ChildItem -LiteralPath $source -File | Where-Object {
    $_.Extension -ieq '.ogg'
} | Sort-Object Name)
if ($files.Count -eq 0) {
    throw "No .ogg files found in: $source. Convert reviewed WAV files to OGG before copying."
}

$index = 1
foreach ($file in $files) {
    $newName = '{0}_{1:D2}{2}' -f $Prefix, $index, $file.Extension.ToLowerInvariant()
    $target = Join-Path $DestinationDirectory $newName
    if ((Test-Path -LiteralPath $target) -and -not $Force) {
        throw "Refusing to overwrite existing file: $target. Re-run with -Force only if intentional."
    }
    Copy-Item -LiteralPath $file.FullName -Destination $target -Force:$Force -ErrorAction Stop
    $index++
}
```

示例调用（先自行试听确认 `Reviewed/Call` 内确实都是 Call 素材）：

```powershell
.\Copy-VoiceAction.ps1 `
  -SourceDirectory ".\Reviewed\Call" `
  -DestinationDirectory ".\1.6\Race\Sounds\com.example.mystudio.ratkinvoices\SR_MyStudio_Race\Call" `
  -Prefix "call"
```

不要直接对示例包目录运行覆盖式批处理。尤其不要把 `-Force` 用于尚未备份的素材或正式 Template 音频目录；先人工确认来源、目标 Action 和许可，再有意识地替换真实文件。

## 9. 安装、加载顺序与首次最小测试

1. 将你的语音包目录作为独立 RimWorld 模组安装，不要塞入主模组目录。
2. 在模组列表中启用 NewRatkinPlus、Squeaky Ratkin 和你的包；把你的包排在主模组与 NewRatkinPlus 之后。Xenotype 扩展还应排在 Biotech 与目标异种来源之后。
3. 启动游戏，打开 Squeaky Ratkin 设置，选择语音模式：
   - **OFF**：仅 Vanilla，用于确认主模组回退路径；
   - **FALLBACK**：Xenotype → Race → Vanilla，适合检查覆盖与回退；
   - **REMIX**：当前可播放的 Xenotype、Race、Vanilla 层等权混音。
4. 在 Race 或对应 Xenotype 范围中启用你的 PackDef；发现包不等于自动启用。
5. 最小测试先只放一条可听 `Call` 音频：物理位置和 `clipFolderPath` 均为 `<lowercase packageId>/<PackDef.defName>/Call/`。在 FALLBACK 下启用 Race 包，触发 Ratkin Call，确认有声；再暂时移除/注释该动作，确认仍会回退而不是静音。随后再测 Xenotype 的同一动作与部分覆盖。

本文提供的是 XML 和文件布局契约，不宣称已在某个具体游戏环境中完成实机加载或播放验证。请在自己的目标模组组合、RimWorld 版本和发行前环境中完成实际测试。

## 10. 常见错误

| 现象 | 常见原因与处理 |
| --- | --- |
| Xenotype 包不匹配 | `targetDefName` 拼写或大小写不等于实际 `XenotypeDef.defName`；不要使用显示名。 |
| 无 Biotech 时 Xenotype 没有声音 | 正常：Xenotype 层不评估；提供并启用 Race 包即可继续回退。 |
| 有包但仍听到 Vanilla | 模式是 OFF、PackDef 未启用、当前动作未覆盖，或目录没有可播放文件。切到 FALLBACK 并检查选择。 |
| 一个动作偶尔静音 | 不要保留只含损坏/空文件的目录；检查 SoundDef、`clipFolderPath`、实际目录和音频格式。 |
| 空文件或不该播放的素材被选中 | 删除空文件、静音测试素材或错误素材；`AudioGrain_Folder` 会收集该目录所有 clip。 |
| XML 解析 FloatRange 失败 | `volumeRange`、`pitchRange`、`distRange` 用 `~`，不要用逗号或圆括号。 |
| 与别的作者包串音/冲突 | 确保所有 DefName 有 `SR_` 前缀和你的包 token，且 `clipFolderPath` 也有独立 token。 |
| 同名目标表现异常 | 同名 Xenotype 解析有歧义时会安全地不使用该 Xenotype 层；修正目标模组组合或改用 Race fallback。 |

## 11. 发布检查清单与许可

发布前逐项确认：

- [ ] 使用自己的 `packageId`、名称、作者和包 token，未冒用示例或 `coahuilite` 身份。
- [ ] 每个 DefName 以 `SR_` 开头且全局唯一；每个 `clipFolderPath` 有你的包 token。
- [ ] Race Def 不含 `targetDefName`；每个 Xenotype Def 只有一个 exact、case-sensitive 的目标字符串。
- [ ] Xenotype Def 由 Biotech 加载规则门控，且没有 XML `XenotypeDef` 强 cross-ref。
- [ ] 每个已列 Action 至少有可播放音频；未覆盖的 Action 已按预期回退。
- [ ] 每个生产 SoundDef 都是 `sustain=false`、`MapOnly`，至少一个带 grain 的 SubSound，且所有 SubSound 的 `onCamera` 均省略或为 `false`；没有 loop 或状态维持式长音频。
- [ ] 已在实际触发中检查生产结构；Workbench 单 clip 试听不作为生产可用性的证明。
- [ ] 多文件放在同一个 `<lowercase packageId>/<PackDef.defName>/<Action>/` 目录并由 `AudioGrain_Folder` 收集；没有空文件、静音占位或意外重复素材。
- [ ] 音频已人工试听，剪去多余静音，做过响度处理，并确认没有 clipping。
- [ ] 在 OFF、FALLBACK、REMIX 下测试过包选择与回退；Race 和 Xenotype 范围分别检查。
- [ ] `About.xml`、加载顺序、依赖和第三方目标模组要求已写清。
- [ ] 只分发你有权分发的音频，并在 About、README 或发布页清楚写明许可与署名。

Squeaky Ratkin 的代码采用 MPL-2.0；你的音频及语音包文本的许可由你自行决定和声明。不要重新分发 RimWorld 原版音频或其他无授权素材；如需使用原版内容，只引用可公开使用的 Def/路径机制，不把原始资产打入包内。
