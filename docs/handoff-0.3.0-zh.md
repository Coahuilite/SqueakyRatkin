# Handoff：0.3.0 内核重构实施状态（harness 无关）

> 目的：跨会话冷启动交接。不依赖任何特定工具环境；路径相对仓库根；命令在仓库根执行。
> 最后更新：2026-08-19（提交 `e6a1ed7` 之后；本文件可能未提交，见工作区状态）。
> 会话必读协议：`AGENTS.md`（身份/记忆协定/隐私边界）→ `MEMORY.md` → `TODO.md`。

## 1. 项目与分支

- RimWorld 1.6 模组「鼠辈啁啾 / Squeaky Ratkin」，packageId `coahuilite.squeakyratkin`，C# namespace `SqueakyRatkin`
- 分支 `0.3.x`（自 dev 切出，隔离 0.2 线；本地领先 `origin/0.3.x` 若干提交**未推送**）；版本已 bump `0.3.0`（`Source/SqueakyRatkin/SqueakyRatkin.csproj` 为主源，`About/About.xml` 跟随）
- 架构唯一入口：`docs/0.3x-refactor-architecture-decision-zh.md`（§1 术语、§1.1 0.x 窗口、§5 阶段映射与验证门、§8 实施纪律）

## 2. 现状：0.3.0 换链已完成（对等实现替换）

0.3.0 = 实现替换：旧 `ChoosePack`/`Or` 链/vanilla 字典 → 内核 `Kernel/SqueakPoolRegistry.Select`。语义由 `docs/0.3x-equivalence-review-zh.md` 锁定（14 行语义规范对照 + 换面/不换面审计 + dev 豁免）。

**新架构三件套**：
- `Source/SqueakyRatkin/Kernel/`（零 Verse 引用编译集；纯度门 = KernelCharacterization 链接编译强制，引用 Verse/Unity 即编译失败）：
  - `Domain.cs`：`AudioDomain(RaceKey, XenotypeKey?)` 值类型域键 + 状态分类
  - `Pool.cs`：`VoicePackEntry`/`ActionSoundSet`/`ChainTier`/`ChainResult`/`SelectionContext`/`IRollSource`/`ISoundGate`
  - `SqueakPoolRegistry.cs`：`Build`（域分组 + PackKey 序数排序）/`Select`（Off→内置表；Fallback→Xeno→Race→内置三级短路；Remix→三级等权折叠）/`PoolsFor`
  - `BuiltInFallbackTable.cs`：内置表种子 = `SqueakActionDefinitions.AudioKey` 单源投影（15 动作全列）；`DomainFilter`（0.4.x 删）
  - `Modulation.cs`：调制合成纯逻辑（0.3.2 接线，0.3.0 未启用）
- `Source/SqueakyRatkin/SqueakKernelAdapter.cs`：唯一接缝——catalog 域包投影（注入 `(Ratkin,*)`，投影规则 = 旧 `ResolvedAudioPack`：去 `_Preview`/Distinct/defName Ordinal 排序）、gate/rolls 实例化（包 `SqueakSoundAvailabilityCache` / `Verse.Rand`）、`ChainResult`→`SqueakSoundChoice`、`KnownMapSoundDefs` 收集（catalog 全量含未选择）
- `SqueakRuntimeResolver.cs`：`BuildSnapshot` 经内核构建 + `Choose` 走 `registry.Select`；旧 `ChoosePack`/vanilla 字典/`Or`/`ResolvedAudioPack` 已同变更删除（无 shim）

**边界**：动作/包域枚举按领域归属（`SqueakActionDomain.cs`/`SqueakVoicePackDomain.cs`，namespace 不变，存档按名序列化安全）；`ActionKey`（内置=枚举名）唯一转换点；行为/mood/时序/距离/vocal/人口全部原地（不换面项，v0.2.4→HEAD diff 审计仅枚举移动）。

## 3. 验证资产（三个 harness + 语料，全部仓库内离线可跑）

| 资产 | 位置 | 作用 | 命令（仓库根） |
| --- | --- | --- | --- |
| SettingsFixtureGenerator | `tools/SettingsFixtureGenerator/` | 0.2.4 设置 fixture ×9 场景（Scribe 规则桩移植自 RimWorld 1.6 源码 + 0.2.4 真实记录代码链接）；load→save 字节稳定自检 | `dotnet run --project tools/SettingsFixtureGenerator -c Release` |
| KernelCharacterization | `tools/KernelCharacterization/` | 纯度门 + 41 单测断言 + 黄金语料生成/回放（3782 例零 delta） | `dotnet run --project tools/KernelCharacterization -c Release` |
| SqueakLogCharacterization | `tools/SqueakLogCharacterization/` | srdiag v1 协议 28 事件双 flavor 锁定（v1 冻结红线） | `dotnet run --project tools/SqueakLogCharacterization -c Release` |
| 语料 | `fixtures/corpus/corpus-0.3.0.txt` | 输入→期望 ChainResult；**0.3.1 切核回放基线**；场景构造 `tools/KernelCharacterization/Scenarios.cs` 冻结不得改 | — |
| fixture | `fixtures/input/` `fixtures/expected/` | 0.2.4 设置样本（真实序列化输出 + 损坏变换） | — |

主模组构建：`dotnet build Source/SqueakyRatkin/SqueakyRatkin.csproj`（Dev 默认；Steam flavor：`-p:SqueakyBuildFlavor=Steam`）。

## 4. 进度

- 0.3.0 任务 15 项：**13 done，2 blocked**（§6 待办）
- 提交链 C1–C10（`41ca953` → `e6a1ed7`）：fixture ×9 → 枚举提取 + ActionKey → Kernel 骨架 → KernelCharacterization + 语料 1622 例 → 等价评审 → resolver 接入 + 旧路径删除 → fixture 驱动语料 3782 例 → 发布门槛检查表 + TODO/MEMORY 同步 → 版本 bump 0.3.0 → zip 打包脚本化
- 发布门槛（`docs/0.3x-release-gate-checklist-zh.md`）：**离线面全绿**（C 设置 fixture 字节稳定 / D 日志 v1 不变 / E 零改动审计 / F Def 内容零变化 / G 性能分配论证）；A/B/H 实机面待办
- 测试包：`dist/dev/SqueakyRatkin-dev-v0.3.0-e6a1ed7.zip`（zip SHA256 `670f8e2baced79c977f86a550ca9241e0175f97520dee237db673316a91ffe9d`；dll SHA256 `af5e2a182f5db37e365eb58607e00400a4aaa429802a4e1b6b6201212cc4c71b`；version.txt = 0.3.0/dev/e6a1ed7）

## 5. 冷启动步骤（新会话）

1. 读 `AGENTS.md` → `MEMORY.md` → `TODO.md`（记忆协定，先读再动）
2. 读本文件 → `docs/0.3x-refactor-architecture-decision-zh.md` §5 → `docs/0.3x-equivalence-review-zh.md` → `docs/0.3x-release-gate-checklist-zh.md`
3. 确认环境：`git status`（0.3.x，干净）、`git log --oneline -3`（`e6a1ed7` 在头）、`git branch --show-current`
4. 跑三个 harness（§3 命令）确认基线全绿；主模组 Dev + Steam flavor 构建 0 error
5. 处理 §6 待办；任何推送前过 AGENTS.md 隐私审查门禁

## 6. 待办与决策点（需维护者）

1. **实机验证**（发布门槛 A/B/C/H 面）：0.2.4 vs 0.3.0 同存档听感与时机对比、设置 UI 快照断言（race 行 == {Ratkin}、catalog 非装配域条目 == 0）、受控 DLC 全关基线（No-DLC/Biotech 关/HAR 缺失）；G 面性能对比可选——检查表「实机待办清单」
2. **三项发布决策**：① GitHub prerelease 双轨是否采用 ② 发布说明措辞（"底层架构升级"是否算内部通用化泄漏）③ 0.3.0.x 热修路线确认——定案后执行双轨发布（GitHub prerelease → Steam 正式；外部有效操作需显式授权）
3. **推送**：本地领先 origin 若干提交未推送（含本 handoff 文档若已提交）；推送前隐私审查
4. 后续阶段：0.3.1（`SqueakVoicePackDef.raceDefName` + validator、catalog DomainFilter 闸 + `GetTargetCandidates` assembled-only 投影、事务性 Scribe 迁移、srdiag v2、试验名单开关、两处接缝切核 + 语料回放零 delta）——决策文档 §5 0.3.1 小节；0.3.0 发布后启动

## 7. 红线（下会话不得违反）

- `SqueakAction` append-only 序数稳定（0.3.2 在末尾 append Crying/Giggling）；srdiag v1 协议冻结；`SR_` 前缀 defName 契约；存档零写入；`base.WriteSettings()` 单通道；HAR 反射非依赖（缺失静默降级）；No-DLC 基线
- 已发布功能 = 玩家必用 → 设置全项验证（唯一豁免：dev 隐藏功能/七次点击解锁面）
- 语料场景构造冻结（`Scenarios.cs`）；任何 Kernel 改动必须过 KernelCharacterization 全绿；fixture 生成器链接的 0.2.4 文件移动时同步更新 csproj 链接
- 提交/推送/发布需显式授权；推送前隐私审查完整可达范围
