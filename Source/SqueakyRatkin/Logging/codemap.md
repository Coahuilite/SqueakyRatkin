# Source/SqueakyRatkin/Logging/

## Responsibility

模组唯一的日志出口。对外提供 **closed typed facade**（`SqueakLog` 的 25 个事件方法 + `Configure`/`ResetSession` + 3 个只读属性），把业务代码的强类型调用翻译为固定的英文 human sentence + 可选的 `srdiag fmt=1` 机器字段，并路由到 Verse `Log`。事件 ID、可见性、级别、human 文案、payload 字段顺序全部由本目录内部注册表锁定，业务代码无法自定义；`docs/logging-protocol.md`（`../../../docs/logging-protocol.md`）是该 v1 合同的规范性记录。

本目录含 `SqueakLog.cs`（唯一 public facade）、`SqueakLogProtocol.cs`（internal protocol modules）与本图。整个模块没有 Harmony patch、没有实例状态、没有业务 I/O；唯一外部副作用仍是 Verse `Log`。

## Key Files/Symbols

- `SqueakLog.cs` — 唯一 public 边界：`SqueakDevLoggingMode` 与 closed typed facade（25 个事件方法、`Configure`/`ResetSession`、3 个只读属性）；保存模式/build 会话状态与唯一 `Emit` 编排，但不再拥有协议实现。
- `SqueakLogProtocol.cs` — internal protocol modules：
  - `SqueakLogVisibility` / `SqueakLogLevel` / `SqueakLogEvent`（28 成员）与 `SqueakLogData`（11 个可空 payload 字段）；
  - `SqueakLogRegistry`（event → visibility/level/human 与 EventId 两张固定表）；
  - `SqueakLogOnce`（Ordinal HashSet、锁、1024 clear-and-accept）；
  - `SqueakLogFormatter` / `SqueakLogText`（`srdiag fmt=1` 记录、值编码、异常路径脱敏）；
  - `SqueakLogSink`（按 level 路由 Verse `Log`）。
- `../../../tools/SqueakLogCharacterization/` — 纯 net472 控制台协议护栏：链接编译两个真实 Logging 源文件，只以 `Verse.Log` 与 `SqueakyRatkinMod` 测试桩隔离 RimWorld；默认与 `-c Dev` 均断言 28 事件、序列化、once/1024、模式门、吞异常与路径脱敏。

## Design

**Closed typed facade（对外契约）。** 业务代码只能调用 `SqueakLog` 的 25 个事件方法；方法签名决定 payload schema（例如 `PackRejected(string pack, int count)` 固定 `reason="duplicate_key"`）。public 面保持不变：`EffectiveDevLogging`、`ShouldEmitDev`、`Mode` 三个属性；`Configure`、`ResetSession` 与 25 个事件方法。`SqueakLog` 只拥有 mode/build/buildId 会话状态和 `Emit` 编排。

**内部协议实现。** `SqueakLogProtocol.cs` 将不对外的职责按稳定边界拆开：

| 模块 | 职责 |
| --- | --- |
| `SqueakLogRegistry` | 28 个 event 的 visibility/level/human 定义与 EventId 映射 |
| `SqueakLogOnce` | once key、Ordinal HashSet、锁、1024 clear-and-accept、`ResetSession` |
| `SqueakLogFormatter` / `SqueakLogText` | `srdiag fmt=1` 字段顺序、值编码、异常路径脱敏 |
| `SqueakLogSink` | 按 level 路由 Verse `Log` |

`SqueakLogVisibility`、`SqueakLogLevel`、`SqueakLogEvent`（28 成员）和 `SqueakLogData` 也留在 protocol 文件；它们均为 internal。新增事件仍须在 event enum → `SqueakLogRegistry.Definition` → `SqueakLogRegistry.EventId` → facade 四处锁步扩展。

**内部事件注册表。** human 文案为固定英文、不本地化；`mod.start.identity` 在运行时嵌入 build/buildId。以下 28 个 EventId 是兼容面，不得改：

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

**Record building（`SqueakLogFormatter.Suffix`）— srdiag v1 字段顺序。** 每行以 `srdiag fmt=1` 开头；核心字段恒出现且顺序固定：

```
fmt lvl vis evt action target pack build build_id
```

事件字段仅在非 null 时追加，顺序固定：

```
reason sound source count dispatched suppressed_detail enabled
ex_type ex_inner ex_site ex_msg
```

缺失/字面 `N/A` → `-`；bool → 小写；`IFormattable` 用 invariant culture；异常文本清理 CR/LF/控制字符、路径替换为 `<path>`、截断 256，再 UTF-8 百分号编码。

**Once / Session（`SqueakLogOnce.Claim`）。** once key 为：

```
coahuilite.squeakyratkin|log-v1|{enum成员名}|{V(Action)}|{V(Target)}|{V(Pack)}|{V(Reason)}|{异常FullName或-}
```

key 用枚举成员名（不是 EventId）；重复 key 丢弃；count 达 1024 时清空再接受当前 key；`ResetSession` 清空注册表。17 个事件走 once，11 个不走。

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

- **启动**：`../Mod.cs` 构造函数 `StartupIdentity()`（`Harmony.PatchAll` 之前）→ `LongEventHandler.ExecuteWhenFinished` 中 `StartupReady(Harmony.GetPatchedMethods().Count())`。
- **配置入口**：`../SqueakyRatkinSettings.cs` 是唯一调用 `Configure` 的地方（`ApplySettings` 与 `SetDevLoggingMode`）；生效状态翻转时调 `SqueakDebug.ResetLoggingSession()` → `SqueakLog.ResetSession()`，后者委托 `SqueakLogOnce.Reset()`。
- **DevOnly 双重门控契约**：facade 内 9 个 DevOnly 方法先查 `ShouldEmitDev`；调用方也先查，避免详细日志关闭时构造诊断 payload。
- **速率控制边界**：5 秒/60 秒节流状态不在 Logging，而在 `../Debug/SqueakDebug.cs`；`AudioDispatchOk`/`TriggerOutcomeSummary` 只是节流后的发射点。
- **数据特例**：`XenotypeDiscoveryCandidate` 将 source 同时写入 reason/source（once key 语义）；`PackRejected` 固定 `reason="duplicate_key"`。

## Integration

- **入向调用方**：`../Mod.cs`、`../SqueakyRatkinSettings.cs`、`../CompSqueaker.cs`、`../HarRatkinXenotypeDiscovery.cs`、`../SqueakXenotypeCatalog.cs`、`../SqueakRuntimeResolver.cs`、`../Debug/`、`../Patches/`。调用方向始终是业务 → public facade；facade 仅编译期引用 `SqueakyRatkinMod` 读取程序集版本。
- **出向**：唯一运行时副作用为 Verse `Log.Message` / `Warning` / `Error`；外部 srdiag 消费者以 `../../../docs/logging-protocol.md` 解析输出。
- **验证**：`../../../tools/SqueakLogCharacterization` 直接链接两个真实源文件。运行 `dotnet run --project tools/SqueakLogCharacterization` 与 `dotnet run --project tools/SqueakLogCharacterization -c Dev`；两者都必须通过。
- **兼容边界**：public facade 签名、`SqueakDevLoggingMode` 数值、28 EventId、human 文案、visibility/level、once key 格式、srdiag 字段顺序与编码均不可变。

## Change Guidance

- 任何 Logging 改动先跑两条 characterization 命令；它们覆盖 28 事件、序列化、once/1024、Dev/release gate、吞异常、异常嵌套/路径脱敏与编码边界。
- 禁止修改 EventId、human 文案、visibility/level、srdiag 字段顺序与编码、once key 拼接及 `Auto=0/Enabled=1/Disabled=2` 枚举值。
- 新增事件按 event enum → registry definition → registry EventId → facade 四处锁步，并同步 `../../../docs/logging-protocol.md`。
- protocol 模块保持 internal；不得让业务代码绕过 `SqueakLog` 直接调用 registry/formatter/once/sink，也不得重新把它们并回 facade。
- 易错点：once key 用枚举成员名而非 EventId；`xenotype.discovery.candidate` 的 reason/source 镜像有意影响 once；DevOnly 调用方与 facade 都必须维持前置 `ShouldEmitDev` 门。
