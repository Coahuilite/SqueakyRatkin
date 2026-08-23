---
name: squeaky-voicepack-authoring
description: >-
  制作、修改或诊断 Squeaky Ratkin（coahuilite.squeakyratkin）VoicePack 音频包的完整流程。
  触发词：制作语音包、新建 VoicePack、语音包 XML、语音包报错、语音包校验、PackDef、
  clipFolderPath、SR_ 前缀、IsEgg 彩蛋、ageTag、fallbacks、Xenotype 语音包、发布语音包。
---

# Squeaky Ratkin 语音包作者指南

给语音包作者的人类可读指南；本文件同时是 `squeaky-voicepack-authoring` skill 的正本，AI 助手按第 13 节执行。本文件自包含：制作语音包不需要任何外部文档或模板，唯一允许的专用脚本是 `scripts/new-voicepack.ps1`（可选脚手架）。

## 1. 快速开始

一个 VoicePack 是**独立 RimWorld 模组**，只包含 XML 和音频，不写 C#。它依赖 Squeaky Ratkin 与 NewRatkinPlus，不修改主模组、不把音频装进主模组目录。

**三步得到一个能响的包：**

1. 生成骨架（可选，推荐）：
   ```powershell
   pwsh -NoProfile -File scripts/new-voicepack.ps1 `
     -PackageId com.example.mystudio.ratkinvoices `
     -PackDefName SR_MyStudio_Race `
     -Actions Call,Select
   ```
   脚本会校验参数并生成 About、LoadFolders、最小 XML、音频占位目录和待办 README；目标已存在时拒绝覆盖。
   不用脚本也行：按第 2 节的目录与 About/LoadFolders 模板 + 第 3 节的最小 XML 手工创建即可，不需要复制任何仓库文件。
2. 把真实 OGG 放进 `<lowercase packageId>/<PackDef.defName>/<Action>/`，替换掉 `PUT_AUDIO_HERE.txt`，并填好自己的作者、描述与许可。
3. 安装进游戏：排序在 NewRatkinPlus 与 Squeaky Ratkin 之后，设置选 **FALLBACK**，在 Race Ratkin 下勾选你的 PackDef，触发 `Call` 听音。

先只做 **Race + Call**，跑通后再加动作、年龄变体、彩蛋或 Xenotype 变体。

## 2. 你的包长什么样

```
MyStudioRatkinVoices/
|- About/About.xml
|- LoadFolders.xml
`- 1.6/
   |- Race/
   |  |- Defs/SoundDefs/MyStudio_Race_Sounds.xml
   |  `- Sounds/com.example.mystudio.ratkinvoices/SR_MyStudio_Race/Call/call_01.ogg
   `- Biotech/                # 仅 Xenotype 包需要
      `- Defs/SoundDefs/MyStudio_Xenotype_Sounds.xml
```

音频根规则固定为 `<lowercase packageId>/<PackDef.defName>/<Action>/`。`packageId` 必须全小写（字母/数字/`._-`），PackDef 的 defName 必须 `SR_` 开头且全局唯一；发布后两者都要保持稳定。

完整的 `About/About.xml`（Race-only 包去掉最后一行 Biotech loadAfter）：

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
    <li>ludeon.rimworld.biotech</li><!-- 仅 Xenotype 包 -->
  </loadAfter>
</ModMetaData>
```

完整的 `LoadFolders.xml`（Race-only 包只保留 `1.6/Race` 一行）：

```xml
<?xml version="1.0" encoding="utf-8"?>
<loadFolders>
  <v1.6>
    <li>1.6/Race</li>
    <li IfModActive="ludeon.rimworld.biotech">1.6/Biotech</li><!-- 仅 Xenotype 包 -->
  </v1.6>
</loadFolders>
```

`Xenotype` 包的 Biotech 内容只在 Biotech 启用时读取；`targetDefName` 是普通字符串，目标缺失不会形成 XML 强引用红字。

## 3. 最小 Race + Call XML

```xml
<?xml version="1.0" encoding="utf-8"?>
<Defs>
  <SqueakyRatkin.SqueakVoicePackDef>
    <defName>SR_MyStudio_Race</defName>
    <scope>Race</scope>
    <raceDefName>Ratkin</raceDefName>
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

细节：根节点名严格写 `SqueakyRatkin.SqueakVoicePackDef`；`fallbacks` 不要写成单数 `fallback`；`IsEgg` 不要改成小写；`FloatRange` 用 `~`，不要用逗号或圆括号。

## 4. PackDef 字段参考

一个 Def 是一个可勾选包、一个权重单位和一个校验单位。

| 字段 | 必填 | 说明 |
| --- | --- | --- |
| `defName` | 是 | 必须 `SR_` 开头，全局唯一；这是玩家看到的包身份 |
| `raceDefName` | 是 | 所服务种族的精确、区分大小写 `ThingDef.defName`，鼠族写 `Ratkin` |
| `scope` | 是 | `Race` 或 `Xenotype` |
| `targetDefName` | Xenotype 必填 | 精确、区分大小写的 `XenotypeDef.defName`；Race 包不得写 |
| `weight` | 否 | 包级正有限抽取权重，省略 = 1 |
| `fallbacks` | 否 | 逐动作回退表，每项 `action` + `sound` |
| `actions` | 是 | 至少一个条目：`action` + `sounds`，可带 `ageTag` 与 `IsEgg` |

可选字段示例：

```xml
<SqueakyRatkin.SqueakVoicePackDef>
  <defName>SR_MyStudio_Race</defName>
  <scope>Race</scope>
  <raceDefName>Ratkin</raceDefName>
  <weight>0.75</weight>
  <fallbacks>
    <li><action>Call</action><sound>SR_MyStudio_Race_Call_Fallback</sound></li>
  </fallbacks>
  <actions>
    <li><action>Call</action><ageTag>Baby</ageTag><sounds><li>SR_MyStudio_Race_Baby_Call</li></sounds></li>
    <li><action>Joy</action><IsEgg>true</IsEgg><sounds><li>SR_MyStudio_Race_Egg_Joy</li></sounds></li>
  </actions>
</SqueakyRatkin.SqueakVoicePackDef>
```

- `<weight>` 只影响同一 tier 内包与包之间的抽取，不影响单个 SoundDef 行为。
- `<fallbacks>` 里的 `<sound>` 必须引用有效的 `SR_*` SoundDef；它只在该动作的 Xenotype/Race 包都没有可播放条目之后、内置 profile 之前参与回退。
- `<ageTag>` 取 `Baby`/`Toddler`/`Child`/`Adult`；省略 = 全年龄。同一动作 exact-age 条目优先于 all-age；RimWorld 1.6 原生不产生 `Toddler`，该桶为 ABI 保留。
- `<IsEgg>true</IsEgg>` 把该条目标为彩蛋：不是独立 tier、不替换普通声音；玩家开关默认关，关时该条目不进候选池，开时与同域普通条目同权混抽。省略或 `false` = 普通条目。
- `Crying`/`Giggling` 可以写进 `actions`/`fallbacks`，但主模组内置表没有这两键音频，未由包声明时静默。

同一 action 在同一 `ageTag`（含 all-age）下只能出现一次；action 只能用第 7 节的 17 个内置键。

## 5. 生产 SoundDef 怎么写

每个被 PackDef 引用的 SoundDef 都是 one-shot，按下面的形状写就不会被拒：

- defName 以 `SR_` 开头；
- `<sustain>false</sustain>`；
- `<context>MapOnly</context>`；
- 至少一个 SubSound，每个 SubSound 至少一个 grain（通常 `AudioGrain_Folder` + `clipFolderPath`）；
- `onCamera` 省略或为 `false`。

禁止 `sustain=true`、loop、camera/map context 混用和依赖状态维持的长音频。违反任何一条，整个 PackDef 会被拒绝，但回退链仍继续工作。一个 Action 目录可以放多个真实文件，`AudioGrain_Folder` 会随机选择；不要留空文件、静音占位或未完成素材。

## 6. Xenotype 包（Biotech 可选增强）

- `<scope>Xenotype</scope>` + `<raceDefName>Ratkin</raceDefName>` + `<targetDefName>精确 XenotypeDef.defName</targetDefName>`。
- `targetDefName` 区分大小写（如 `RK_XenoType_Ratkin`），不要翻译、不要写 XML 强引用。
- `LoadFolders.xml` 用 `<li IfModActive="ludeon.rimworld.biotech">1.6/Biotech</li>` 门控。
- 目标缺失时该层不解析，Race 层仍按链回退。

## 7. 17 个固定动作与回退

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
| `Attack` | 普通攻击成功反馈 |
| `Work` | 工作指令/工作状态反馈 |
| `Equip` | 玩家主动装备武器或工具反馈 |
| `MentalBreak` | 精神崩溃开始反应 |
| `Crying` | Biotech 婴幼儿 BabyFits 哭闹；无内置音频，未声明则静默 |
| `Giggling` | Biotech 婴幼儿 BabyFits 咯咯笑；无内置音频，未声明则静默 |

允许部分覆盖（只提供 `Call` 也有效）。未覆盖动作按模式回退：

- **FALLBACK**：Xenotype pack → Race pack → pack 自带 fallback → 内置 profile → 无声；
- **REMIX**：当前可播放的 Xenotype/Race/已声明 pack fallback/内置 profile tier 间等权（无声明 pack fallback 的动作保留三层抽取）；
- **OFF**：仅内置 profile。

语音包只能提供声音，不能改触发、心情、频率、距离或 Action 范围。

## 8. 音频处理与格式

- 推荐 **OGG Vorbis、mono**；官方示例音频用 22050 Hz mono 作参考（非强制）。WAV 仅中间审阅，MP3 不推荐。
- 不要只改扩展名伪装转码；剪掉首尾静音/点击声/空白，做响度处理并避免 clipping。
- 主模组会运行时调制心情音高与音量，不需要制作情绪矩阵。

```powershell
ffmpeg -i ".\Raw\call source.wav" -ar 22050 -ac 1 -c:a libvorbis -q:a 5 ".\Reviewed\call_01.ogg"
```

## 9. 安装与测试

1. 把包作为独立模组安装，排序在 NewRatkinPlus 与 Squeaky Ratkin 之后；Xenotype 内容再排到 Biotech 与目标来源之后。
2. 设置中选择 **FALLBACK**，在 Race 或对应 Xenotype 范围手动勾选 PackDef（发现 ≠ 自动启用）。
3. 先测 Race `Call`：目录与 `clipFolderPath` 都是 `<lowercase packageId>/<PackDef.defName>/Call/`。
   确认播放后移除该动作验证回退，再测 Xenotype 与部分覆盖。
4. 彩蛋作者自测（当前 0.3.2 开关无 UI）：关闭游戏后编辑本机
   `Config/Mod_coahuilite.squeakyratkin_SqueakyRatkinMod.xml`，加入
   `<allowEasterEggSounds>True</allowEasterEggSounds>`，重启后开启的蛋条目参与抽取；
   玩家可见开关随 US 后续版本提供。
5. dev 日志（详细日志开启时）成功派发单行：`Audio route: <action> -> <sound> (<tier>[, egg][, nonplayer]).`
   后缀含 `egg=true|false`、`pawn_faction=<FactionDef.defName>`、`pawn_ctrl=player|nonplayer`。

## 10. 排错

| 现象 | 处理 |
| --- | --- |
| Xenotype 不匹配 | `targetDefName` 必须等于实际 `XenotypeDef.defName`（含大小写），不要用显示名 |
| 有包仍听到 Vanilla | 模式不是 OFF、PackDef 已勾选、动作已覆盖、目录有可播放文件 |
| 动作静音或 PackDef 被拒绝 | 对照第 5 节契约，检查目录、`clipFolderPath`、格式与空/损坏文件 |
| XML FloatRange 失败 | `volumeRange`/`pitchRange`/`distRange` 用 `~` |
| 与其他包串音 | 每个 DefName 与资源根使用自己的稳定包 token |

运行时被拒的包会在日志留下 `voicepack.pack.rejected reason=duplicate_key|domain_filtered`；被拒包不装配，回退链继续工作。先用任意 XML 解析器过一遍结构，再进游戏。

## 11. 发布检查清单

- [ ] 自己的 `packageId`、名称、作者与包 token；每个 DefName 以 `SR_` 开头且全局唯一。
- [ ] 每个 Def 都声明 exact、case-sensitive 的 `raceDefName`；Race Def 不含 `targetDefName`；
      每个 Xenotype Def 只有一个 exact、case-sensitive 目标，并由 Biotech 加载规则门控。
- [ ] `clipFolderPath`、实际目录与 `<lowercase packageId>/<PackDef.defName>/<Action>/` 一致；
      已列 Action 都有可播放音频。
- [ ] 每个生产 SoundDef 满足第 5 节强制契约。
- [ ] 已在 OFF、FALLBACK、REMIX 下检查选择与回退；Workbench 试听不代替实际触发测试。
- [ ] 音频已人工试听、无空文件或意外素材，并完成剪辑与响度处理。
- [ ] 已声明自己的音频许可与署名，只分发有权分发的内容。
- [ ] 不重新分发 RimWorld 原版音频或其他无授权素材。

Squeaky Ratkin 代码采用 MPL-2.0；音频及语音包文本的许可由作者自行决定和声明。

## 12. 兼容承诺

VoicePack XML 是**公开稳定的作者 ABI**：自首个携带 0.3.1 XML ABI 的发布版本起冻结。对作者来说这意味着：

- 你写的字段只会增加、不会被改名或改变含义（字段只增不改）；
- 17 个内置动作键只会追加、不会删除（append-only）；
- 不合法或未来未知的包不会被猜着加载，而是整体拒绝并继续回退（fail-closed）；
- `IsEgg` 属于该公开面，可以放心写进发布包。

主模组内部实现仍可能在 0.x 调整，但不会破坏上述 XML 包契约。

## 13. 给 AI 助手（skill 执行约定）

触发：用户请求制作/修改/诊断语音包。按顺序执行：

1. 确认 packageId（全小写）与 PackDef defName（`SR_` 前缀）；用
   `pwsh -NoProfile -File scripts/new-voicepack.ps1 -PackageId <id> -PackDefName <def> -Actions <键列表>`
   生成骨架，或按第 3 节直接产出最小 XML。
2. 只允许第 7 节的 17 个动作键；`raceDefName` 必须精确；Race 包不写 `targetDefName`，
   Xenotype 包必须写并加 Biotech 加载门控。
3. 音频路径 = `<lowercase packageId>/<PackDef.defName>/<Action>/`；提醒作者替换占位文件并声明许可。
4. 验证：用 XML 解析器检查生成物；对照第 4–5 节字段与 SoundDef 契约逐项检查；
   不要替作者声称已经实机测试，明确要求按第 9 节执行 OFF/FALLBACK/REMIX 与回退测试。
5. 排错优先查第 10 节表格；引用本文件的具体小节编号回答，不要凭记忆改写契约。

## 14. 自包含与专用脚本

本文件是制作语音包的**唯一正本**，不需要读取任何外部文档或复制任何模板；正文已包含全部目录、XML、字段、契约、测试与发布内容。

允许的专用脚本（可选）：

- `scripts/new-voicepack.ps1`：生成 Race-only 包骨架（第 1 节）；
- `scripts/verify-voicepack-xml-abi.ps1`：维护者侧的示例 XML × validator × 本文件三向一致性锁，作者无需运行。
