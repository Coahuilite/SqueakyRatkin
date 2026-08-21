# 0.2.4 ModSettings fixture（§6.2）

## 来源（重要）

实机（0.2.4 游戏内保存设置）在本次会话不可用，本批 fixture 是**降级路径产物**：

- 记录类型（`VoicePackSelectionRecord`/`XenotypePresetRecord`/`GlobalActionEnabledRecord`/`XenotypeMoodOverride`/`XenotypeActionBehaviorOverride`）**真实链接 0.2.4 源码**（与 `v0.2.4` tag 逐字节一致）；
- 序列化规则（默认省略、`forceSave`、float G9、`True/False`、字典 keys/values 双列表、`<li>` 嵌套、`<SettingsBlock><ModSettings>` 根结构、UTF-8 声明）移植自 RimWorld 1.6 真实源码（Scribe_Values/Scribe_Collections/ScribeSaver/ScribeExtractor/ParseHelper/FloatRange）；
- 主层 12 值 + 4 集合的契约（字段名/默认值/forceSave/顺序）从 0.2.4 `SqueakyRatkinSettings.cs` L114-249 机械提取，逐行注释可回溯。

每个场景自检：input → load（含 PostLoadInit 修复/迁移）→ save == expected；expected 再 load → save 幂等。

**升级路径**：实机可用后，用 0.2.4 游戏逐个构造 §6.2 场景并保存，以真实 ModSettings XML 替换 `input/` 样本，重跑本生成器断言；若字节 diff 非零，以真实文件为准修正契约。

## 目录

- `input/`：0.3.0 harness 的输入样本（= 0.2.4 序列化输出；02/08 为手写输入文本：空文件/损坏样本）
- `expected/`：加载（含修复/迁移）后的稳定态；0.3.0 的 load→serialize 输出必须与 expected 逐字节一致

## 场景清单（§6.2 八项 + 1 项扩展）

| 场景 | 覆盖 |
| --- | --- |
| 01-new-install-first-save | 无文件新装（不经过 ExposeData）→ 启动链后第一次保存的期望形态（schema=3 forceSave） |
| 02-empty-file-no-schema | 文件存在但无任何节点（无 schema 旧配置）→ schema 迁移写回 |
| 03-explicit-off | 显式 Off 节点，其余默认省略 |
| 04-fallback-seeded | voicePackMode 省略（默认 Fallback）+ voicePackDefaultSeeded + Race 域内置 key |
| 05-multi-selections-lastwins | 多个旧 Race+Xenotype selection + 同域 last-wins 重复 + xenotypePresets（mood/action overrides） |
| 06-orphan-packkey | enabledPackKeys 含未知 key（orphan 保留） |
| 07-biotech-inactive-target | Xenotype selection 目标未激活（dormant 运行时判定） |
| 08-corrupt-missing-fields | 缺 enabledPackKeys + scope 非法 + voicePackMode 非法（加载修复路径，修复态 = expected） |
| 09-mood-overrides-and-global-scope | moodOverrides 字典 + globalActionEnabled legacy bool-only 形态（NormalizeScope 修复） |

## 再生

```
dotnet run --project tools/SettingsFixtureGenerator -c Release
```

依赖：0.2.4 源码文件（`Source/SqueakyRatkin/SqueakVoicePackModels.cs`、`SqueakXenotypePresetModels.cs`、`SqueakGlobalActionPolicy.cs`、`SqueakActionModel.cs`、`Logging/SqueakLog.cs`、`Logging/SqueakLogProtocol.cs`）——0.3.0 若移动/拆分这些文件，csproj 的链接路径需同步更新，并复核「与 v0.2.4 逐字节一致」前提。
