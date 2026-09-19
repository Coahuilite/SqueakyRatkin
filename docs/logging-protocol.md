# srdiag 日志协议

> 2026-09-19 按现行源码重构，自足描述现状：这是兼容/解析合同和已知偏差记录，不是游戏实测证据。实现为 [SqueakLogProtocol.cs](../Source/SqueakyRatkin/Logging/SqueakLogProtocol.cs)，字节期望在 [日志 harness](../tools/SqueakLogCharacterization/Program.cs)。

## 门面、开关与会话

`SqueakLog` 是 closed typed facade：registry 固定事件 ID、可见性、等级、英文人读句及 typed payload；调用方不能传任意事件/句子/schema。每条输出以 `[SqueakyRatkin] ` 和英文句开头，不本地化。详细模式有效时，同一行追加 ` || srdiag fmt=1 ...` 或 `fmt=2 ...`，版本由事件决定。

Daily 在非详细模式仍输出人读句；DevOnly 则不输出。Info/Warning/Error 独立决定 Verse sink，不因 once 或详细模式变等级。

| `SqueakDevLoggingMode` | 详细模式 |
| --- | --- |
| Auto | Dev flavor 开，GitHub/Steam 关 |
| Enabled | 全 flavor 开 |
| Disabled | 全 flavor 关 |

此模式独立于 `Prefs.DevMode` 和七击解锁 `developerToolsEnabled`；解锁只暴露入口。四个模式结果事件的 Daily 句独立输出，关闭详细模式也可报告。DevOnly facade 和调用方均先查 `ShouldEmitDev`，关闭时不得先构造昂贵诊断数据。

有效状态变化时 `ResetLoggingSession` 清派发采样、summary timer 和 once registry，重新开启后每动作首个成功可立即输出；**当前无 map-lifecycle reset**。

## 字段与编码

字段顺序是兼容面。核心字段只出现一次；异常/事件专属字段可缺省，消费者须允许未知扩展字段。

| 版本 | 核心字段（固定顺序） |
| --- | --- |
| v1 | `fmt lvl vis evt action target pack build build_id` |
| v2 | `fmt lvl vis evt action target pack race [xenotype] build build_id` |

v1 事件字段的 formatter 顺序为：

```text
reason sound source count dispatched suppressed_detail enabled pawn pawn_id
ex_type ex_inner ex_site ex_msg
```

`pawn pawn_id` 已存在于 v1 字节锁，原文漏列；本次仅修正文档。v1 原 28 事件 byte-immutable，不往其中混入 v2 字段。

v2 的 action 为 string key（内置=枚举名，未来外部=`packageId.defName`，不代表 SR 已支持外部注册）；race 是精确 `ThingDef.defName`，缺失为 `-`；xenotype 仅域有该维度时出现，值为精确 `XenotypeDef.defName`。

| v2 事件 | 核心之后的字段顺序 |
| --- | --- |
| `settings.origin` | `settings_origin`（FreshCreated / LoadedFromFile） |
| `audio.route.selected` | `sound tier egg suppressed_detail pawn pawn_id pawn_faction pawn_ctrl` |
| `hook.mental_fit.unavailable` | 无 |
| `fallback.profile.store_failed` | `ex_type ex_inner ex_site ex_msg` |

- 数字使用 invariant culture，bool 为 `true` / `false`；编码值空或字面量 N/A 为 `-`，optional null 字段可不输出。
- 字符串使用 UTF-8 percent-encode；不编码集合为 `A-Z a-z 0-9 . _ ~ : / @ + -`。先按 token 分键值，再 percent-decode。
- **实现边界**：普通字段直接编码，不作通用路径脱敏；异常 `ex_msg` 才先将 CR/LF 改空格、去控制字符、替换 DOS/UNC/device/file URI/Unix/相对路径为 `<path>`、截到 256 字符，再编码。不得写原始 `Exception.ToString()`。
- **未决隐私冲突 R5**：实现和字节测试包含 `pawn=<label>`，与旧规范的“不得包含 pawn label”相抵触；编码不等于匿名化。此处记载实际协议，不批准隐私例外。未经另行裁决不改冻结字节；实际日志/个人路径不得收入仓库或文档。Def 身份字段不得换成本地化或玩家可改标签。

## 路由记录与限流

`settings.origin` 在启动读取设置后，每会话 once。Scribe 到达 LoadingVars 代表 LoadedFromFile；缺文件/不可读文件由框架丢弃解析、警告并回字段默认，记 FreshCreated。

自 0.3.2，装配器成功路径仅发合并的 `audio.route.selected` fmt=2，不再并列发 v1 `audio.dispatch.ok`；后者仍保留 registry 与字节锁。sound 为 SoundDef defName，egg 恒 true/false，suppressed_detail 为上次明细以来被抑制的派发数；pawn_id 为 ThingID，pawn_faction 为 FactionDef defName（无阵营 `-`），pawn_ctrl 为 `player` / `nonplayer`。

| 内核层 | 现有日志 tier |
| --- | --- |
| XenotypePack | `xenotype_pack` |
| RacePack、PackFallback | `race_pack` |
| BuiltInFallback | `vanilla` |
| 无 | `-` |

`pack_fallback` / `built_in_fallback` 保留但不输出；现有日志不能区分折叠前的两层。是否应扩展、原设计是否有意接受归因损失，尚无裁决。

once-key 由 event/action/target/pack/reason/exception type 组成，v1/v2 分 `log-v1` / `log-v2` 域并在锁内领取；达到 1024 键时清空后接受新键，会话 reset 也清空。每动作首次成功立即发明细，随后每 5 秒最多一条，summary 每 60 秒最多一条；不改变 warning/error 严重度或可见性。Eat 粒度本轮不新增日志事件。

## 事件注册表

以下明确按版本分组，避免旧文档以“最后四行”推断协议版本。英文句为稳定输出模板；启动身份、设置来源、路由句由闭合参数生成。

### v1：28 个事件

| Event ID | Visibility | Level | Human sentence |
| --- | --- | --- | --- |
| `mod.start.identity` | Daily | Info | `Squeaky Ratkin started with {build} build {build_id}.` |
| `mod.start.ready` | Daily | Info | `Squeaky Ratkin startup completed.` |
| `logging.mode.enabled` | Daily | Info | `Detailed diagnostic logging is enabled.` |
| `logging.mode.disabled` | Daily | Info | `Detailed diagnostic logging is disabled.` |
| `logging.mode.auto_enabled` | Daily | Info | `Detailed diagnostic logging is enabled by Auto mode.` |
| `logging.mode.auto_disabled` | Daily | Info | `Detailed diagnostic logging is disabled by Auto mode.` |
| `settings.open.api_unavailable` | Daily | Warning | `Mod Settings API is unavailable.` |
| `settings.open.failed` | Daily | Warning | `Mod Settings could not be opened.` |
| `voicepack.catalog.refresh_failed` | Daily | Error | `VoicePack catalog refresh failed.` |
| `voicepack.pack.rejected` | Daily | Warning | `A VoicePack was rejected.` |
| `voicepack.resolver.rebuild_failed` | Daily | Error | `VoicePack resolver rebuild failed.` |
| `voicepack.target.rejected` | Daily | Warning | `A Xenotype VoicePack target was rejected.` |
| `xenotype.discovery.unavailable` | DevOnly | Warning | `Xenotype discovery is unavailable.` |
| `xenotype.discovery.failed` | DevOnly | Warning | `Xenotype display discovery failed.` |
| `xenotype.discovery.candidate` | DevOnly | Info | `A HAR Xenotype discovery candidate was evaluated.` |
| `trigger.attempt.failed` | Daily | Error | `Squeak trigger attempt failed.` |
| `audio.dispatch.no_sound` | Daily | Warning | `No fallback SoundDef was found.` |
| `audio.dispatch.failed` | Daily | Error | `Squeak audio dispatch failed.` |
| `audio.dispatch.ok` | DevOnly | Info | `Squeak audio dispatched.` |
| `trigger.outcome.summary` | DevOnly | Info | `Squeak trigger outcome summary was recorded.` |
| `hook.attack.unavailable` | Daily | Error | `Attack squeak hook is unavailable.` |
| `hook.attack.target_skipped` | DevOnly | Warning | `An Attack hook target was skipped.` |
| `hook.mental_break.unavailable` | Daily | Error | `Mental-break squeak hook is unavailable.` |
| `diagnostics.hook.unavailable` | DevOnly | Warning | `Diagnostics overlay hook is unavailable.` |
| `diagnostics.start.failed` | Daily | Warning | `Diagnostics overlay could not start.` |
| `devtools.overlay.changed` | DevOnly | Info | `Diagnostics overlay state changed.` |
| `devtools.camera_indicator.changed` | DevOnly | Info | `Camera indicator state changed.` |
| `devtools.workbench.open_failed` | Daily | Warning | `Animal Voice Workbench could not be opened.` |

### v2：4 个事件

| Event ID | Visibility | Level | Human sentence |
| --- | --- | --- | --- |
| `hook.mental_fit.unavailable` | Daily | Error | `Baby-fits squeak hook is unavailable.` |
| `settings.origin` | Daily | Info | `Mod settings origin: <FreshCreated\|LoadedFromFile>.` |
| `audio.route.selected` | DevOnly | Info | `Audio route: <action> -> <sound> (<tier>[, egg][, nonplayer]).` |
| `fallback.profile.store_failed` | DevOnly | Warning | `Fallback profile store operation failed.` |

## 事件附加语义

`voicepack.pack.rejected` 的 pack 为 PackKey；reason 为 `duplicate_key`（count 为重复实例数）或 `domain_filtered`（race 超出产品域）。

`xenotype.discovery.candidate` 的 target 是精确 Xenotype defName。候选是 assembled-only 投影：`declared_pack` / `selection` / `preset` 按确定顺序以 `+` 连接，enabled=true。HAR-only 提示仅诊断，未保留目标输出 `har_hint_filtered` / `har_official_filtered`，enabled=false，并将原因镜像到 source。reason 参与 v1 once-key，因此同目标的不同来源集可分别输出，而不修改协议键。

修改协议须同步 typed facade、registry、formatter 与 characterization；本次文档重构没有修改运行时代码或测试期望。
