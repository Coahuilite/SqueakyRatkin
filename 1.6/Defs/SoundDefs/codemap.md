# 1.6/Defs/SoundDefs/

## Responsibility

定义全部发声资产契约的两层音池：

1. **Vanilla 回退层**（`SqueakyRatkin_SoundDefs.xml`）：15 个 `SR_<Action>` SoundDef（每个动作 1 个、中性基准），引用原版 clip 路径，不复制原版资产；另有 1 个 `SR_Call_Preview` 作为唯一的通用 on-camera 试听 transport。
2. **内置官方 Example VoicePack**（`SqueakyRatkin_OfficialExample_Race.xml`）：1 个 Race 范围 `SqueakVoicePackDef` + 当前 15 个 `SR_OfficialExample_Race_<Action>` SoundDef（参考基准，数量可变），自带音频（发布打包时注入，见 Change Guidance）。

## Key Files / Symbols

- `SqueakyRatkin_SoundDefs.xml` —— 16 个 SoundDef：`SR_Call, SR_Eat, SR_Sleep, SR_Wounded, SR_Select, SR_Move, SR_Social, SR_Joy, SR_Death, SR_Draft, SR_Undraft, SR_Attack, SR_Work, SR_Equip, SR_MentalBreak`（全 `MapOnly`）+ `SR_Call_Preview`（无 context，含 `onCamera=True` 的 SubSound）。
- `SqueakyRatkin_OfficialExample_Race.xml` —— `SqueakyRatkin.SqueakVoicePackDef` `SR_OfficialExample_Race`（`<scope>Race</scope>`、`<raceDefName>Ratkin</raceDefName>`（0.3.1 必填），无 `targetDefName`）+ 当前 15 个对应 SoundDef（参考基准；`sustain=false`、`MapOnly`、grain 为 `AudioGrain_Folder` → `coahuilite.squeakyratkin/SR_OfficialExample_Race/<Action>`）。
- C# 连接（`Source/SqueakyRatkin/`）：
  - `SqueakActionDefinitions`（`SqueakActionModel.cs`）：`AudioKey = "SR_" + 动作名`，`SqueakRuntimeResolver.GetVanilla()` 用 `DefDatabase<SoundDef>.GetNamedSilentFail(AudioKey)` 构建 Vanilla tier —— **defName 契约的唯一权威来源**。
  - `SqueakVoicePackDef` / `SqueakVoicePackAction` / `SqueakVoicePackValidator`（`SqueakVoicePackModels.cs`）：`ConfigErrors()` 强制校验——pack defName 及所引 SoundDef 均须 `SR_` 前缀、`sustain=false`、`context=MapOnly`、有 SubSound。
  - `ResolvedAudioPack`（`SqueakRuntimeResolver.cs`）：把 pack 的 actions 解析为 `SqueakAction → List<SoundDef>`，**过滤 defName 以 `_Preview` 结尾的 SoundDef**（试听 transport 永不进生产音池）。
  - `SqueakOnCameraPreviewAdapter.SoundDefName = "SR_Call_Preview"`（`UI/SqueakyRatkinSettings.SoundMoodUI.cs`）：`DefDatabase<SoundDef>.GetNamedSilentFail("SR_Call_Preview")` 取第一个 `onCamera` SubSound 供 UI/Dev 试听。

## Design

- **每动作单 Def + 运行时调制**：心情音色不拆 Def，由运行时 `SoundInfo` 的 `pitchFactor/volumeFactor` 叠加（`CompSqueaker` 的 moodMods）；SoundDef 层 `pitchRange` 只保留基础随机（中性 0.95~1.05，`SR_Death` 0.8~0.9）。`volumeRange 45~55`、`distRange 15~70`（后者运行时被 distancePreset 覆盖）。
- **Vanilla 回退 grain 策略**：`AudioGrain_Folder` 展开整个原版 clip 文件夹（如 `Pawn/Animal/Boomrat/Boomrat_Call`、`Pawn/Animal/Eating/Rodent`、`Pawn/Animal/GuineaPig/Death`），`AudioGrain_Clip` 只引用已确认存在的原版 clipPath（如 `Pawn_Guineapig_Pain_08`）；文件夹与单 clip 可同 SubSound 混用。
- **Example 与 17-action ABI 的关系**：`SR_OfficialExample_Race` 的 actions 当前恰为 15 条，对应枚举 0–14；Crying/Giggling（15/16）有意没有内置 SoundDef、Example 条目或 fallback 表项，只有 pack 声明时才发声。Example **无运行时特权**：与第三方 VoicePack 同一选择/权重规则（Fallback：Xenotype pack→Race pack→pack fallback→built-in fallback→无声；Remix：可播放 tier 等权，未声明 fallback 的动作保留既有 Xenotype/Race/内置抽取）；0.2.3 起新装/从未显式配置的安装默认启用（`SqueakyRatkinSettings` 迁移：`voicePackMode` 节点缺失 → Fallback + 种子 Race 选择记录；显式模式不被覆盖）。`SqueakVoicePackScope.Race` 的 pack 无 target，不依赖 Biotech。
- **`SR_Call_Preview` 与 `SR_Call` 职责分离**：`SR_Call` 是生产 Call 音池（播放时由调用方提供已解析的 production/native clip）；`SR_Call_Preview` 只做 on-camera 试听，`_Preview` 后缀被 `ResolvedAudioPack` 排除出生产解析。

## Data & Control Flow

```
Defs 加载（LoadFolders v1.6，无条件）
  → DefDatabase<SoundDef> / DefDatabase<SqueakyRatkin.SqueakVoicePackDef>
  → SqueakRuntimeResolver.BuildSnapshot：
      BuiltInFallbackCatalog 为 Ratkin 查 15 个 `SR_<Action>` 键（Crying/Giggling 无条目 → 无声）
      Race tier = 玩家选中的 Race 范围 pack（含 SR_OfficialExample_Race）
  → 触发时 SqueakRuntimeSnapshot.Choose(context, action, ...)：
      Off → 仅 built-in fallback；Fallback → XenotypePack → RacePack → PackFallback → BuiltInFallback → 无声；Remix → 可播放 tier 等权（未声明 PackFallback 时保留 Xenotype/Race/BuiltInFallback 抽取）
  → SoundDef.PlayOneShot(SoundInfo{pitch/volume = mood 调制 × SoundDef pitchRange})
```

## Integration

- 上游消费者：`CompSqueaker`（生产触发）、`SqueakAudioBrowser`/`SqueakOnCameraPreviewAdapter`（试听）、`SqueakSoundAvailability`（可播放性门控）、`CompSqueaker.ApplyDistanceRange`（写回 `distRange`）。
- 第三方 VoicePack 契约：`.github/skills/squeaky-voicepack-authoring/SKILL.md` —— 音频根 `<lowercase packageId>/<PackDef.defName>/<Action>/`；独立模组发布；不得修改主模组 SoundDef、不得把音频装入主模组目录。
- 与 `Patches/Ratkin_AddSqueakComp.xml` 的关系：patch 里 15 个 `<action>` 名决定本目录前 15 个 `SR_<Action>` 必须存在；Crying/Giggling 由 BabyFits hook 触发且无内置 SoundDef。两者由 `SqueakAction` 枚举值隐式连接（非 XML cross-ref）。

## Change Guidance

- 调音池：改对应 `SR_<Action>` 的 grains/volume/pitch/distRange；保持 `MapOnly`、`sustain=false`、`SR_` 前缀（validator 会红字报错）。
- 新增动作：三处必须同步——`SqueakAction` 枚举（append-only）、`SqueakActionDefinitions.AudioKey`、本目录新增 `SR_<Action>`。
- 改 Example：只改 `SR_OfficialExample_Race.xml`；音频文件在工作树中**不存在**——打包脚本 `scripts/stage-package.ps1` 从 `Extras/SqueakyRatkinExampleVoices/1.6/Race/Sounds/.../SR_ExampleTemplate_Race/` 拷贝为 `1.6/Sounds/coahuilite.squeakyratkin/SR_OfficialExample_Race/<Action>/`，并断言 Template→built-in 镜像一致——实际动作/key 集合与 SHA-256（当前参考为 15 个动作目录、41 个 OGG，数量可变，不校验固定总数）。直接改 def 而不满足该镜像会打包失败。
- 试听 transport：勿把 `SR_Call_Preview` 之外的东西当 on-camera 播放器，勿让生产解析依赖 `_Preview` Def。
