# Source/SqueakyRatkin/Logging/

## Responsibility

模组唯一的日志出口。对外提供 **closed typed facade**（`SqueakLog` 的 25 个事件方法 + `Configure`/`ResetSession` + 3 个只读属性），把业务代码的强类型调用翻译为固定的英文 human sentence + 可选的 `srdiag fmt=1` 机器字段，并路由到 Verse `Log`。事件 ID、可见性、级别、human 文案、payload 字段顺序全部由本目录内部注册表锁定，业务代码无法自定义；`docs/logging-protocol.md`（`../../../docs/logging-protocol.md`）是该 v1 合同的规范性记录。

本目录仅含两个文件：`codemap.md` 与唯一实现文件 `SqueakLog.cs`（约 250 行，含 6 个类型）。整个模块没有 Harmony patch、没有实例状态、没有 I/O。

## Key Files/Symbols

- `SqueakLog.cs` — 唯一实现文件。包含：
  - `SqueakDevLoggingMode`（public enum：`Auto=0`、`Enabled=1`、`Disabled=2`）— 持久化设置值，顺序即兼容面。
  - `SqueakLogVisibility`（internal：`Daily`/`DevOnly`）、`SqueakLogLevel`（internal：`Info`/`Warning`/`Error`）。
  - `SqueakLogEvent`（internal enum，28 个成员）— 内部事件注册表主键。
  - `SqueakLogData`（internal readonly struct）— 11 个可空字段的强类型 payload 载体。
  - `SqueakLog`（public static class）— 上帝类本体（见 Design 风险标记）。
  - `SqueakLogText`（internal static class）— 唯一已被拆出的部分：`SanitizeExceptionMessage`、`PercentEncode`。

`SqueakLog` 内部私有成员按职责分组：

| 成员 | 职责 |
| --- | --- |
| `Emit(evt, data, once)` | 总入口：visibility 门 → once 门 → 文本拼装 → sink；整体 `catch { }` 吞异常 |
| `Definition(evt)` / `EventId(evt)` | 两个 switch 表达式：注册表（visibility/level/human）与 ID 映射 |
| `Suffix` / `Add` / `V` | srdiag 记录构建与值编码 |
| `ClaimOnce` / `onceLock` / `onceKeys` / `ResetSession` | once 会话注册表（Ordinal HashSet，上限 `OnceLimit=1024`） |
| `Sink` | Verse 路由：`Log.Warning` / `Log.Error` / `Log.Message` |
| `Configure` | 模式解析（`#if SQUEAKY_DEV` / `SQUEAKY_STEAM` / `SQUEAKY_GITHUB`）+ 构建身份读取 |

## Design

**Closed typed facade（对外契约）。** 业务代码只能调用 25 个事件方法；方法签名决定了该事件的 payload schema（例如 `PackRejected(string pack, int count)` 内部硬编码 `reason="duplicate_key"`）。public 面包括：

- 属性：`EffectiveDevLogging`、`ShouldEmitDev`（恒等于前者）、`Mode`；方法：`Configure`、`ResetSession`、上述 25 个事件方法。
- 新增事件必须在 `SqueakLogEvent`、`Definition`、`EventId`、facade 四处以锁步方式扩展，缺一即编译错误或 `ArgumentOutOfRangeException`。

**内部事件注册表（现状）。** `Definition` 与 `EventId` 是内联在 facade 类中的两个巨型 switch 表达式，28 个事件的 visibility/level/human 与 ID 各一份；human 文案为固定英文、不本地化，`mod.start.identity` 的文案在运行时嵌入 build/buildId。28 个事件 ID（兼容面，不得改）：

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

**Record building（`Suffix`）— srdiag v1 字段顺序（兼容面，不得改）。** 每行以 `srdiag fmt=1` 开头；核心字段恒出现、顺序固定：

```
fmt lvl vis evt action target pack build build_id
```

事件相关字段按以下固定顺序、仅在有值时出现（`Add` 跳过 null）：

```
reason sound source count dispatched suppressed_detail enabled
ex_type ex_inner ex_site ex_msg
```

（例外字段仅当 `Exception != null` 时追加，`ex_site` 取 `TargetSite` 的声明类型全名 + 方法名。）编码规则：缺失/字面 `N/A` → `-`；bool → 小写 `true`/`false`；`IFormattable` 用 invariant culture；字符串先经 `SqueakLogText.SanitizeExceptionMessage`（CR/LF 与控制字符清理、路径正则替换为 `<path>`、截断 256）再 `PercentEncode`（UTF-8 字节；安全集 `A-Z a-z 0-9 . _ ~ : / @ + -`，其余 `%XX` 大写十六进制）。

**Once / Session（`ClaimOnce`）。** once key（兼容面，不得改）为单行拼接：

```
coahuilite.squeakyratkin|log-v1|{enum成员名}|{V(Action)}|{V(Target)}|{V(Pack)}|{V(Reason)}|{异常FullName或-}
```

注意：key 用的是 `SqueakLogEvent` **枚举成员名**（如 `PackRejected`），不是点分 EventId；值经 `V()` 百分号编码；异常类型名不编码。`onceLock` 下检查/写入，重复 key 丢弃；`onceKeys.Count >= 1024` 时整体清空再接受新 key（clear-and-accept）。`ResetSession` 清空注册表。17 个事件走 once（多为错误/警告）；11 个不走（8 个方法：启动、模式变更、成功路径、状态变更 — `LoggingModeChanged` 一个方法覆盖 4 个 Auto/显式模式事件）。

**Sink。** 仅按 `SqueakLogLevel` 选 Verse 路由；visibility 与 level 正交：`Daily` 在日志无效时只发 human 句、有效时追加 srdiag；`DevOnly` 在无效时**不发任何内容**。

**配置状态。** `Configure` 校验枚举（非法 → `Auto`）；`#if SQUEAKY_DEV` 下 `EffectiveDevLogging = mode != Disabled`，否则 `= mode == Enabled`。build 标识：`SQUEAKY_STEAM` → `steam` + 版本号 `+` 前段；`SQUEAKY_GITHUB` → `github` + 完整 InformationalVersion；否则 `dev`。

**上帝类风险（仅标记，不设计重构）。** `SqueakLog` 一个静态类同时承担：模式/构建会话状态、事件注册表（visibility/level/human）、事件 ID 映射、once 会话注册表（含锁）、srdiag 记录构建、值编码、Verse sink 路由、25 方法 public facade、`Configure`/`ResetSession`。静态可变状态（`mode`、`build`、`buildId`、`onceKeys`）中仅 onceKeys 有锁保护；`Emit` 整体吞异常。`SqueakLogText` 是唯一已拆出的关注点。本风险已在 `../../../MEMORY.md`、`../../../TODO.md` 记为 P0（先 characterization 再机械拆分，见 Change Guidance）。

## Data & Control Flow

```
业务调用方 ──(facade 事件方法)──▶ SqueakLog.Emit(evt, data, once)
                                    ├─ Definition(evt)          [注册表查找]
                                    ├─ visibility 门：DevOnly && !EffectiveDevLogging → return
                                    ├─ once 门：ClaimOnce 失败 → return
                                    ├─ 文本：Prefix + human (+ " || " + Suffix 当有效时)
                                    └─ Sink → Verse Log.{Message|Warning|Error}
```

- **启动**：`../Mod.cs` 构造函数 `StartupIdentity()`（`Harmony.PatchAll` 之前）→ `LongEventHandler.ExecuteWhenFinished` 中 `StartupReady(Harmony.GetPatchedMethods().Count())`。
- **配置入口**：`../SqueakyRatkinSettings.cs` 是唯一调用 `Configure` 的地方（`ApplySettings` 与 `SetDevLoggingMode`）；生效状态翻转时调 `SqueakDebug.ResetLoggingSession()` → `SqueakLog.ResetSession()` 清 once 注册表（音频采样/计时器由 `../Debug/SqueakDebug.cs` 一并清）。设置 UI 经 `SqueakyRatkinSettings.EffectiveDevLogging` 透传读取 `SqueakLog.EffectiveDevLogging`，文案走 `SR.DevTools.Logging.*` 本地化 key。
- **DevOnly 双重门控契约**：facade 内 9 个 DevOnly 方法自带 `ShouldEmitDev` 门；调用方（`../HarRatkinXenotypeDiscovery.cs`、`../SqueakXenotypeCatalog.cs`、`../Patches/Patch_Verb_Attack.cs`、`../Debug/SqueakDebugActions.cs` 等）也先查 `ShouldEmitDev` 再构造诊断字符串/字段，保证无效时零构造开销。
- **速率控制边界**：5 秒/60 秒节流状态**不在 Logging 内**，属 `../Debug/SqueakDebug.cs`（per-action `AudioSample` 与 summary 计时器）；`AudioDispatchOk`/`TriggerOutcomeSummary` 只是被节流后的纯发射点。once 门与速率门相互独立。
- **数据特例**：`XenotypeDiscoveryCandidate` 把同一 source 串镜像进 `reason`（once key 含 reason，同一 target 不同来源集合可各发一条）；`PackRejected` 固定 `reason="duplicate_key"`。

## Integration

- **入向调用方（全部）**：`../Mod.cs`（4 处）、`../SqueakyRatkinSettings.cs`（Configure/EffectiveDevLogging/LoggingModeChanged）、`../CompSqueaker.cs`（TriggerAttemptFailed/AudioNoSound/AudioDispatchFailed）、`../HarRatkinXenotypeDiscovery.cs`、`../SqueakXenotypeCatalog.cs`（CatalogRefreshFailed/PackRejected/XenotypeDiscovery*）、`../SqueakRuntimeResolver.cs`（ResolverRebuildFailed/TargetRejected）、`../Debug/SqueakAudioBrowser.cs`、`../Debug/SqueakDebug.cs`、`../Debug/SqueakDebugActions.cs`、`../Debug/SqueakDiagnosticsOverlay.cs`、`../Patches/Patch_MapInterface_DiagnosticsOverlay.cs`、`../Patches/Patch_MentalBreak.cs`、`../Patches/Patch_Verb_Attack.cs`。调用方向单向：业务 → facade；facade 从不回调业务代码（仅编译期引用 `SqueakyRatkinMod` 取程序集版本）。
- **出向**：仅 Verse `Log`（`Log.Message`/`Warning`/`Error`）。机器消费者为外部 srdiag 解析器，凭 `../../../docs/logging-protocol.md` 解析。
- **依赖**：System（`Reflection`/`Text`/`Text.RegularExpressions`/`Globalization`）+ Verse。`SqueakDevLoggingMode` 作为设置持久化 int 与 `SqueakyRatkinSettings` 共享。
- **兼容边界**：public facade 签名、`SqueakDevLoggingMode` 枚举值、28 个事件 ID、human 文案、visibility/level、once key 格式、srdiag v1 字段顺序与编码均为兼容面；`docs/logging-protocol.md` 是现行合同的唯一权威记录。

## Change Guidance

- **必须先 characterization 再动手**：`../../../MEMORY.md` / `../../../TODO.md` P0 明文规定 — 拆分 internal registry/build/once/formatter/sink 之前，必须先建立 `srdiag v1` characterization checks，且不得改变 public typed facade、字段顺序、once key 或事件 ID。任何重构第一步应是锁定 `Emit` 产出（含空 payload、null/`N/A`、异常块、1024 清空、吞异常等边界行为）的字符级快照。
- **改动禁区**：事件 ID、srdiag 字段顺序与编码、once key 拼接、human 文案、可见性/级别、`Auto=0/Enabled=1/Disabled=2` 枚举值 — 任一变更都会破坏外部解析器与既有会话日志。
- **新增事件**：四处以锁步扩展（enum 成员 → `Definition` → `EventId` → facade 方法），并同步更新 `docs/logging-protocol.md` 注册表表。
- **机械拆分前提**：注册表（Definition/EventId）、once、formatter（Suffix/V/Add）、sink、facade 各自内聚可拆，但拆后必须保留唯一 public facade 与既有行为；`SqueakLogText` 已是拆分范例。
- **易错点提醒**：once key 用枚举成员名而非 EventId；`xenotype.discovery.candidate` 的 reason/source 镜像是有意的 once-key 语义；DevOnly 双门控（调用方先查 `ShouldEmitDev` 再构造字符串）是约定，改动时须同时维护两侧。
