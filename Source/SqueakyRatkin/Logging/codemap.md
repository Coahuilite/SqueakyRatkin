# Source/SqueakyRatkin/Logging/

## Responsibility

模组唯一的日志出口。对外提供 **closed typed facade**（`SqueakLog` 的 29 个事件方法 + `Configure`/`ResetSession` + 3 个只读属性），把业务代码的强类型调用翻译为固定的英文 human sentence + 可选的 `srdiag fmt=1`/`fmt=2` 机器字段，并路由到 Verse `Log`。事件 ID、可见性、级别、human 文案、payload 字段顺序全部由本目录内部注册表锁定，业务代码无法自定义；`docs/logging-protocol.md`（`../../../docs/logging-protocol.md`）是 v1+v2 合同的规范性记录。0.3.1：v1 28 事件字节零改动；4 个 v2 扩展事件（`settings.origin`、`audio.route.selected`、`hook.mental_fit.unavailable`、`fallback.profile.store_failed`）走 `fmt=2` 与 `log-v2` once 域。0.3.2：`audio.route.selected` 扩展 `egg/suppressed_detail/pawn/pawn_id/pawn_faction/pawn_ctrl` 且 human 句参数化（发配日志重排简化 + 身份门控可读矩阵，v1 仍不动）。

本目录含 `SqueakLog.cs`（唯一 public facade）、`SqueakLogProtocol.cs`（internal protocol modules）与本图。整个模块没有 Harmony patch、没有实例状态、没有业务 I/O；唯一外部副作用仍是 Verse `Log`。

## Key Files/Symbols

- `SqueakLog.cs` — 唯一 public 边界：`SqueakDevLoggingMode`/`SqueakSettingsOrigin` 与 closed typed facade（29 个事件方法、`Configure`/`ResetSession`、3 个只读属性）；保存模式/build 会话状态与唯一 `Emit` 编排，但不再拥有协议实现。
- `SqueakLogProtocol.cs` — internal protocol modules：
  - `SqueakLogVisibility` / `SqueakLogLevel` / `SqueakLogEvent`（32 成员：28 v1 + 4 v2）与 `SqueakLogData`（17 个可空 payload 字段，含 v2 的 race/xenotype/tier/settingsOrigin 与 0.3.2 的 egg/pawnControlled/pawnFaction）；
  - `SqueakLogRegistry`（event → visibility/level/human/协议版本 与 EventId 两张固定表；`HumanSentence` 参数化 settings.origin 与 audio.route.selected）；
  - `SqueakLogOnce`（Ordinal HashSet、锁、1024 clear-and-accept；once 域按协议版本分 `log-v1`/`log-v2`）；
  - `SqueakLogFormatter` / `SqueakLogText`（`srdiag fmt=1` 记录字节不变 + `fmt=2` 扩展记录、值编码、异常路径脱敏）；
  - `SqueakLogSink`（按 level 路由 Verse `Log`）。
- `../../../tools/SqueakLogCharacterization/` — 纯 net472 控制台协议护栏：链接编译两个真实 Logging 源文件，只以 `Verse.Log` 与 `SqueakyRatkinMod` 测试桩隔离 RimWorld；默认与 `-c Dev` 均断言 28 个 v1 事件字节不变 + v2 分支（fmt=2 字段序、settings.origin、log-v2 once、编码、门控）、序列化、once/1024、模式门、吞异常与路径脱敏。

## Design

**Closed typed facade（对外契约）。** 业务代码只能调用 `SqueakLog` 的 29 个事件方法；方法签名决定 payload schema（例如 `PackRejected(string pack, int count)` 固定 `reason="duplicate_key"`）。public 面保持不变：`EffectiveDevLogging`、`ShouldEmitDev`、`Mode` 三个属性；`Configure`、`ResetSession` 与 29 个事件方法。`SqueakLog` 只拥有 mode/build/buildId 会话状态和 `Emit` 编排。v2 事件（`SettingsOrigin`、`AudioRouteSelected`）的 race/xenotype/tier 均为字符串参数——facade 不引用内核类型，链 tier 由调用方（`SqueakDebug.ProtocolTier`）映射为协议词表；0.3.2 起 `AudioRouteSelected` 另收 `isEgg/suppressed/pawnName/pawnId/pawnControlled/pawnFaction` 供单行明细与身份矩阵。

**内部协议实现。** `SqueakLogProtocol.cs` 将不对外的职责按稳定边界拆开：

| 模块 | 职责 |
| --- | --- |
| `SqueakLogRegistry` | 32 个 event（28 v1 + 4 v2）的 visibility/level/human/协议版本定义与 EventId 映射；`HumanSentence` 参数化 settings.origin 与 audio.route.selected |
| `SqueakLogOnce` | once key、Ordinal HashSet、锁、1024 clear-and-accept、`ResetSession`；前缀 `log-v{版本}` |
| `SqueakLogFormatter` / `SqueakLogText` | `srdiag fmt=1` 字段顺序（字节不变）与 `fmt=2` 固定核心序、值编码、异常路径脱敏 |
| `SqueakLogSink` | 按 level 路由 Verse `Log` |

`SqueakLogVisibility`、`SqueakLogLevel`、`SqueakLogEvent`（32 成员：28 v1 + 4 v2）和 `SqueakLogData` 也留在 protocol 文件；它们均为 internal。新增事件仍须在 event enum → `SqueakLogRegistry.Definition` → `SqueakLogRegistry.EventId` → facade 四处锁步扩展，并同步 characterization 与协议文档。

**内部事件注册表。** human 文案为固定英文、不本地化；`mod.start.identity` 在运行时嵌入 build/buildId，`settings.origin` 嵌入 closed 双值 origin，0.3.2 起 `audio.route.selected` 参数化为 `Audio route: <action> -> <sound> (<tier>[, egg]).`。以下 28 个 v1 EventId 是兼容面，字节不得改：

```
mod.start.identity            mod.start.ready
logging.mode.enabled          logging.mode.disabled
logging.mode.auto_enabled     logging.mode.auto_disabled
settings.open.api_unavailable settings.open.failed
voicepack.catalog.refresh_failed  voicepack.pack.rejected
voicepack.resolver.rebuild_failed voicepack.target.rejected
xenotype.discovery.unavailable xenotype.discovery.failed
xenotype.discovery.candidate  trigger.attempt.failed
audio.dispatch.no_sound       audio.dispatch.failed
audio.dispatch.ok             trigger.outcome.summary
hook.attack.unavailable       hook.attack.target_skipped
hook.mental_break.unavailable diagnostics.hook.unavailable
diagnostics.start.failed      devtools.overlay.changed
devtools.camera_indicator.changed devtools.workbench.open_failed
```

0.3.1 新增四个 v2 EventId（fmt=2）：`settings.origin`（Daily once）、`audio.route.selected`（DevOnly 随 5 秒节流）、`hook.mental_fit.unavailable`（Daily）、`fallback.profile.store_failed`（DevOnly）。0.3.2 起 `audio.route.selected` 是成功路径唯一明细（装配器不再并列发 v1 `audio.dispatch.ok`；该 v1 记录仍保留在 registry 与 characterization）：

```
settings.origin               audio.route.selected
```

**Record building（`SqueakLogFormatter.Suffix`）— v1 字段顺序（字节不变）。** 每行以 `srdiag fmt=1` 开头；核心字段恒出现且顺序固定：

```
fmt lvl vis evt action target pack build build_id
```

事件字段仅在非 null 时追加，顺序固定：

```
reason sound source count dispatched suppressed_detail enabled
ex_type ex_inner ex_site ex_msg
```

**v2 字段顺序（`SuffixV2`，仅 version=2 事件）。** 核心序：

```
fmt lvl vis evt action target pack race [xenotype] build build_id
```

`action` 为字符串动作键（内置=枚举名与 v1 字节一致，外部=packageId.defName 免编码）；`race` 恒出现（缺失 `-`），`xenotype` 仅在有异种时出现；只写 DefName。事件字段按事件固定：`settings.origin` → `settings_origin=`；`audio.route.selected` → `sound= tier= egg= suppressed_detail= pawn= pawn_id= pawn_faction= pawn_ctrl=`（egg/suppressed_detail 恒出现，pawn*/faction/ctrl 按装配器输入出现；pawn_ctrl = `player|nonplayer` 二值）；`hook.mental_fit.unavailable` → 无事件字段；`fallback.profile.store_failed` → `ex_type= ex_inner= ex_site= ex_msg=`。

缺失/字面 `N/A` → `-`；bool → 小写；`IFormattable` 用 invariant culture；异常文本清理 CR/LF/控制字符、路径替换为 `<path>`、截断 256，再 UTF-8 百分号编码（v2 沿用同一套）。

**Once / Session（`SqueakLogOnce.Claim`）。** once key 为：

```
coahuilite.squeakyratkin|log-v{版本}|{enum成员名}|{V(Action)}|{V(Target)}|{V(Pack)}|{V(Reason)}|{异常FullName或-}
```

key 用枚举成员名（不是 EventId）；重复 key 丢弃；count 达 1024 时清空再接受当前 key；`ResetSession` 清空注册表。v1 事件 → `log-v1`（字节不变），v2 事件 → `log-v2`（两个 once 域互不碰撞）。18 个事件走 once，12 个不走。

**Sink 与配置状态。** level 决定 Verse `Log` 方法；`Daily` 在详细日志关闭时仍发 human 句，`DevOnly` 则不发。`Configure` 非法枚举回退 `Auto`；`SQUEAKY_DEV` 下 `Auto` 生效，其他 flavor 仅 `Enabled` 生效；build identity 仍按 `SQUEAKY_STEAM` / `SQUEAKY_GITHUB` / dev 分支产生。

**治理结果。** 原 P0 上帝类已机械拆成 facade 与四类 protocol 模块；public facade、28 个事件 ID、human 文案、visibility/level、once key、字段顺序、编码和整体吞异常边界均未改变。`tools/SqueakLogCharacterization` 以默认与 `-c Dev` 两个编译分支锁定这些不变量。

## Data & Control Flow

```
业务调用方 ──(facade 事件方法)──▶ SqueakLog.Emit(evt, data, once)
                                    ├─ SqueakLogRegistry.Definition
                                    ├─ visibility 门：DevOnly && !EffectiveDevLogging → return
                                    ├─ SqueakLogOnce.Claim 失败 → return
                                    ├─ Prefix + human (+ SqueakLogFormatter.Suffix 当有效时)
                                    └─ SqueakLogSink.Emit → Verse Log.{Message|Warning|Error}
```

- **启动**：`../Mod.cs` 构造函数 `SettingsOrigin(...)`（紧接 `GetSettings` 之后；LoadedFromFile = ExposeData 到达 LoadingVars，否则 FreshCreated）→ `StartupIdentity()`（`Harmony.PatchAll` 之前）→ `LongEventHandler.ExecuteWhenFinished` 中 `StartupReady(Harmony.GetPatchedMethods().Count())`。
- **配置入口**：`../SqueakyRatkinSettings.cs` 是唯一调用 `Configure` 的地方（`ApplySettings` 与 `SetDevLoggingMode`）；生效状态翻转时调 `SqueakDebug.ResetLoggingSession()` → `SqueakLog.ResetSession()`，后者委托 `SqueakLogOnce.Reset()`。
- **DevOnly 双重门控契约**：facade 内 9 个 DevOnly 方法先查 `ShouldEmitDev`；调用方也先查，避免详细日志关闭时构造诊断 payload。
- **速率控制边界**：5 秒/60 秒节流状态不在 Logging，而在 `../Debug/SqueakDebug.cs`；0.3.2 起装配器只把节流后的成功明细交给 `AudioRouteSelected`（v2 单行），`TriggerOutcomeSummary` 仍是 60 秒汇总发射点。
- **数据特例**：`XenotypeDiscoveryCandidate` 将 source 同时写入 reason/source（once key 语义）；`PackRejected` 固定 `reason="duplicate_key"`。

## Integration

- **入向调用方**：`../Mod.cs`、`../SqueakyRatkinSettings.cs`、`../CompSqueaker.cs`、`../HarRatkinXenotypeDiscovery.cs`、`../SqueakXenotypeCatalog.cs`、`../SqueakRuntimeResolver.cs`、`../Debug/`、`../Patches/`。调用方向始终是业务 → public facade；facade 仅编译期引用 `SqueakyRatkinMod` 读取程序集版本。
- **出向**：唯一运行时副作用为 Verse `Log.Message` / `Warning` / `Error`；外部 srdiag 消费者以 `../../../docs/logging-protocol.md` 解析输出。
- **验证**：`../../../tools/SqueakLogCharacterization` 直接链接两个真实源文件。运行 `dotnet run --project tools/SqueakLogCharacterization` 与 `dotnet run --project tools/SqueakLogCharacterization -c Dev`；两者都必须通过。
- **兼容边界**：public facade 签名、`SqueakDevLoggingMode`/`SqueakSettingsOrigin` 数值、28 个 v1 EventId、human 文案、visibility/level、once key 拼接、srdiag 字段顺序与编码均不可变；v2 只允许新增事件/字段（extension-only），不回流 v1。

## Change Guidance

- 任何 Logging 改动先跑两条 characterization 命令；它们覆盖 28 个 v1 事件字节、v2 分支、序列化、once/1024（含 log-v1/log-v2 独立域）、Dev/release gate、吞异常、异常嵌套/路径脱敏与编码边界。
- 禁止修改 v1 EventId、human 文案、visibility/level、srdiag v1 字段顺序与编码、once key 拼接及 `Auto=0/Enabled=1/Disabled=2` 枚举值；禁止把 v2 字段塞进 fmt=1 记录。
- 新增事件按 event enum → registry definition → registry EventId → facade 四处锁步，并同步 `../../../docs/logging-protocol.md` 与 characterization。
- protocol 模块保持 internal；不得让业务代码绕过 `SqueakLog` 直接调用 registry/formatter/once/sink，也不得重新把它们并回 facade。
- 易错点：once key 用枚举成员名而非 EventId；`xenotype.discovery.candidate` 的 reason/source 镜像有意影响 once；DevOnly 调用方与 facade 都必须维持前置 `ShouldEmitDev` 门。
