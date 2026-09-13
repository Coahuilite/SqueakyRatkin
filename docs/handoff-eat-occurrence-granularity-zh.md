# 交接说明：动作触发粒度与「真正进食」判定（Eat / JobDefOf.Ingest）

> 接收方：实现同类「动作 → 声音」触发的姊妹项目/内核（当前最可能是 UniversalSqueaker，命名空间/前缀/日志按对方约定替换；判定语义与 vanilla 事实部分可直接复用）。
> 来源：本仓库 `dev` 分支未提交改动 + 一次 fresh-context 只读对抗复核；vanilla 结论均按 RimWorld 1.6 源码与 Def 逐条核对（RimSage）。本文只含仓库相对路径与 vanilla 符号，无本机路径、日志摘录或凭据。
> 日期：2026-08-23。

## 0. TL;DR（三句话）

1. 触发粒度是 **job 级**：`Pawn.CurJob.def == JobDefOf.Ingest` 成立期间（取食 → 端食物走到餐桌 → 吞咽 → 收尾）全程都算 `Eat`。
2. 要加「只在真正进食时触发」的按钮，必须先选判定权威；vanilla 有两条**公开但语义不同**的路径：
   - `IEatingDriver.GainingNutritionNow` ⇒ 要求**摄入营养 > 0**：零营养摄入物（大麻烟卷、薄片等社交药物）**永远不触发**；
   - `JobDriver.CurToilString == "ChewIngestible"` ⇒ 真·吞咽/点燃阶段，覆盖药物，但依赖 vanilla 的 toil debugName 字符串。
3. 选错的代价不是报错，而是**静默 hole**：开关一开，抽烟的 pawn 在整个 ingest 期间一个 `Eat` 事件都没有，日志连 rejected 记录都不会有——因为 `Eat` 在采样层就被换成了 `Move`/`Call`。
4. 已落地的形态（2026-08-23 维护者决定，本仓库已实施）：**父开关**「仅在真正进食（正在摄入营养）时触发」+ **子选项**「使用成瘾品」（子项默认关、父项关闭时禁用且强制保持 `false`）——父关 = job 级（现状）；父开子关 = 方案 A（营养判定）；父开子开 = 方案 B（`ChewIngestible`），且 **toil 名无法确认时回落完整 job 级**。完整规格见 §5.1。

## 1. 背景：现象与原始实现

- 玩家反馈（Steam 评论，正面）：鼠族端着食物横穿地图时"连珠炮"般叫，因为"太期待吃饭了"。
- 机制指纹（`v0.3.0` tag 与当前工作树一致，即现网 Steam 版本就是这个手感）：
  - `CompSqueaker.CurrentAction` 中 `IsEating()` = `Pawn.CurJob?.def == JobDefOf.Ingest`，且 `Eat` 在优先级上高于 `Move`；所以"端食物赶路"全程归 `Eat`。
  - XML（`1.6/Patches/Ratkin_AddSqueakComp.xml`）：`Eat = EachTime, 144 ticks`；`EachTime` 不抽概率（概率门只在 `RandomOneShot` 生效）；`Eat` 不豁免全局冷却 ⇒ 实际节拍 = `globalMinIntervalTicks = 216` ≈ **3.6 秒现实时间一发**（`scaleCooldownWithTimeSpeed` 默认开，1×/2×/3× 节拍一致）。
  - 横穿地图 20–30 秒 ≈ 5–8 发；站桩咀嚼（默认 500 tick ≈ 8.3 秒）≈ 1–2 发。**默认状态下 `Eat` 音频主要响在"赶去吃饭"的路上。**
- 产品决定（2026-08-23 维护者）：**默认不变**（这是招牌手感，0.4/US 重构同样不得默认收窄）；新增玩家开关把 `Eat` 收窄到"真正进食"。

## 2. vanilla 事实基线（排查依据，逐条可复现）

### 2.1 `JobDefOf.Ingest` 的 toil 链

`JobDriver_Ingest.MakeNewToils()`（`Source/RimWorld/JobDriver_Ingest.cs`）：

1. `ReserveFood` → 走到食物 → 捡起食物（`Toils_Ingest.PickupIngestible`）；
2. `Toils_Ingest.CarryIngestibleToChewSpot`（端食物走到餐桌/进食点）；
3. `Toils_Ingest.FindAdjacentEatSurface`；
4. `chewing = Toils_Ingest.ChewIngestible(...)`（真正吞咽；在 `MakeNewToils` 里先创建、最后 yield）；
5. `Toils_Ingest.FinalizeIngest`；吃尸体可 `JumpIf(chewing, …)` 循环再吃。

营养膏机路径（`PrepareToIngestToils_Dispenser`）、从背包吃（`eatingFromInventory`）、非工具使用者（动物）路径**共享同一个 `chewing` toil 对象**。

### 2.2 咀嚼 toil 的可识别特征

`Toils_Ingest.ChewIngestible`（`Source/RimWorld/Toils_Ingest.cs`）：

- `toil.actor.pather.StopDead();` ⇒ 进入咀嚼那一刻起**站桩**；
- `ticksLeftThisToil = Mathf.RoundToInt(thing.def.ingestible.baseIngestTicks * durationMultiplier)`；
- toil 由 `ToilMaker.MakeToil("ChewIngestible")` 创建 ⇒ `Toil.debugName = "ChewIngestible"`。

### 2.3 两条权威的精确语义

| 权威 | 位置 | 语义 | 稳定性 |
| --- | --- | --- | --- |
| `IEatingDriver.GainingNutritionNow` | `Source/RimWorld/IEatingDriver.cs`（接口只有这一个成员）；实现见 `JobDriver_Ingest.GainingNutritionNow` | `IngestibleSource` 未销毁 **且** `def.IsNutritionGivingIngestible` **且** `CurToil == chewing` | 接口稳定；但语义是**营养**，不是"在嚼" |
| `JobDriver.CurToilString` | `Source/Verse/AI/JobDriver.cs`：`CurToil` 为 `protected`，`CurToilIndex` / `CurToilString` 为 `public`；`Toil.ToString()` = `debugName ?? "unnamed"`（`Source/Verse/AI/Toil.cs`） | `"ChewIngestible"` 即在吞咽/点燃 | 字符串依赖 Ludeon 不改 debugName |

`ThingDef.IsNutritionGivingIngestible`（`Source/Verse/ThingDef.cs`）：

```csharp
public bool IsNutritionGivingIngestible => IsIngestible && ingestible.CachedNutrition > 0f;
```

`IngestibleProperties.baseIngestTicks` 默认 **500**（`Source/RimWorld/IngestibleProperties.cs`）。

### 2.4 真值表（建议直接做成单测）

| 场景 | 使用的 JobDef | job 级（默认） | 营养级判定 | toil 级判定 | 备注 |
| --- | --- | --- | --- | --- | --- |
| 端食物走去餐桌 | Ingest | Eat | 否 | 否 | 评论里被夸的那段 |
| 吃 meal（`MealSimple` 未覆写 ⇒ 500 t） | Ingest | Eat | 是 | 是 | |
| 抽大麻烟卷（`baseIngestTicks=720`、`preferability=NeverForNutrition`、无 `<Nutrition>`） | Ingest | Eat | **否（静默 hole）** | 是 | `DrugAIUtility.IngestAndTakeDrug`、`JobGiver_Binge`、`JobGiver_TakeCombatEnhancingDrug` 都走 `JobDefOf.Ingest` |
| 吸食薄片（`baseIngestTicks=650`，同上） | Ingest | Eat | **否** | 是 | |
| 啤酒（`Nutrition 0.08`） | Ingest | Eat | 是 | 是 | 带营养的药物即通过 |
| 营养膏机 | Ingest | Eat | 是（取餐后 targetA 换成携带的餐） | 是 | |
| 从背包吃 | Ingest | Eat | 是 | 是 | |
| 动物直接吃 | Ingest | Eat | 是 | 是 | 非工具使用者路径 |
| 吃尸体 | Ingest | Eat | 是（生成的 corpse def 设 `StatDefOf.Nutrition = 5.2`） | 是 | |
| 掠食/喂食/食人宴等 | 其他 JobDef（如 `EatAtCannibalPlatter`） | 非 Eat | 非 Eat | 非 Eat | 不在本判定范围 |

## 3. 问题本体：静默 hole

### 3.1 复现步骤

1. 打开「仅在真正进食（正在摄入营养）时触发 Eat 叫声」开关。
2. 让一个 pawn 按药物政策/娱乐/嗑药去抽大麻烟卷或吸食薄片（720/650 tick ≈ 12/10.8 秒窗口；按 216 t 节拍，正常情况下应有约 3 发 `Eat`）。
3. 观察：整个 ingest 期间**没有 `Eat` 事件**；走路时可能出 `Move`，站定时落 `Call`。日志里没有 rejected / disabled / cooldown 记录。

### 3.2 为什么排查时容易漏

- 不是异常、不是 warn/error、也不是"被冷却拒绝"，而是**动作归属变了**；
- 动作统计只记录真正进入触发管线的动作，因此 `Eat` 计数会直接归零，看不出"被谁替代"；
- 只有当维护者恰好盯着一个正在抽烟的 pawn 才会察觉。

### 3.3 影响边界

- **受影响**：零营养 ingestible 的整个 ingest 过程（社交药物为主；任何 `CachedNutrition == 0` 的条目）。
- **不受影响**：所有营养 > 0 的食物/尸体/营养膏（含带营养的药酒等）。

## 4. 需要新增的按钮 — 需求规格

| 项 | 要求 |
| --- | --- |
| 字段名 | 建议 `eatOnlyDuringChewing`（本仓库现用）或按接收方语义命名（如 `eatRequiresIngestion`） |
| 默认值 | **必须保持 job 级（false）**：这是已发布手感，默认收窄等于删掉招牌行为 |
| UI 位置 | 现有「发声规则/行为开关」组内的一个复选框 + 一行灰色短说明（不要新开页面） |
| 持久化 | Scribe add-only 标量；默认值省略（不写节点），**不 bump schema 版本**；老存档/缺节点 → 默认 false |
| 生效方式 | 即时生效（纯开关发布到运行时旗标/策略即可，无需重建 resolver/池） |
| 文案必须含 | ① 默认关 = 整段进食行程（含端食物赶路）都算；② 开启 = 只在「X」时算；③ **X 的边界要写明**：按营养判定就写"零营养摄入物（如大麻烟卷、薄片）不算"；按 toil 判定就写"仅吞咽/点燃阶段" |
| 不得改变 | 动作资格/作用域、resolver、音频回退、冷却、日志协议、动作 ABI |
| 若泛化为 per-action | 粒度偏好放策略层（纯函数 + 注入采样），保持"决策出生即纯"；不要塞进音频选择内核 |
| 子选项（可选，推荐一并设计） | 父项开启后才可用的第二个复选框：勾选 = 改用方案 B（吞咽/点燃阶段，覆盖成瘾品）。子项默认 false、独立持久化；**父项关闭时子项必须禁用（置灰）且不产生任何效果**。完整规格见 §5.1 |

## 5. 实现方案对比

| 方案 | 判定 | 优点 | 代价 |
| --- | --- | --- | --- |
| **A. 营养判定** | `CurJob.def == Ingest && (!chewOnly \|\| gainingNutritionNow)` | 接口稳定、单行；本仓库现用 | 零营养摄入物静默（须写进文案；若产品不接受则不能用） |
| **B. toil 字符串判定** | `CurJob.def == Ingest && curDriver.CurToilString == "ChewIngestible"` | 语义就是"在嚼"，覆盖抽烟/薄片；公开 API 可见 | 依赖 vanilla `debugName`；建议 fail-safe：匹配不到时回退 A 或回退 job 级，并加日志/单测 |
| **C. 反射私有字段** | 反射 `JobDriver_Ingest.chewing` 与 `CurToil` 比较 | 精确 | 需缓存 FieldInfo，脆弱、跨版本风险高，不推荐 |
| **D. Harmony 打标** | postfix `Toils_Ingest.ChewIngestible` 给 toil/driver 挂自建标记 | 最稳、与游戏解耦于 debugName | 最重；仅当 B 的字符串不可接受时采用 |

推荐：产品语义若是"真正摄入营养"→ **方案 A + 文案写明药物边界**；若是"真正在吃/在抽"→ **方案 B + fail-safe**。

### 5.1 两级选项设计规格（父 + 子，已在本仓库实施）

产品形态：**父开关**「仅在真正进食时触发 Eat 叫声」+ **子选项**「使用成瘾品」（用原版的中文措辞）。两个持久化字段，三种有效模式：

| 父（eating-only） | 子（include-drugs） | 有效模式 | 判定 |
| --- | --- | --- | --- |
| 关（默认） | 任意（**父关时强制 false**） | `WholeJob` | 整个 `JobDefOf.Ingest` job（现状手感） |
| 开 | 关 | `GainingNutrition` | 方案 A：`GainingNutritionNow` |
| 开 | 开 | `ChewingToil` | 方案 B：`CurToilString == "ChewIngestible"`，toil 名未确认时回落 `WholeJob` |

**纯规则（决策出生即纯，零 Verse）：**

```csharp
public enum SqueakEatOccurrenceMode { WholeJob, GainingNutrition, ChewingToil }

public const string ChewingToilDebugName = "ChewIngestible"; // vanilla toil debugName，单测锁定

public static SqueakEatOccurrenceMode ResolveMode(bool eatingOnly, bool includeDrugs)
    => !eatingOnly ? SqueakEatOccurrenceMode.WholeJob
     : includeDrugs ? SqueakEatOccurrenceMode.ChewingToil
                    : SqueakEatOccurrenceMode.GainingNutrition;

// 适配层按 mode 惰性采样；WholeJob 不采样
public static bool AllowsOccurrence(SqueakEatOccurrenceMode mode, bool gainingNutritionNow,
    bool chewToilActive, bool chewToilNameConfirmed) => mode switch
{
    SqueakEatOccurrenceMode.WholeJob => true,
    SqueakEatOccurrenceMode.GainingNutrition => gainingNutritionNow,
    // 回落 = 完整 job 级：toil 名未确认（Ludeon 改名 / 非 vanilla 驱动 / 本进程尚未采样到）
    // 时按整段 Ingest job 处理，绝不静默；已确认时按咀嚼/点燃判定，营养并集仅作附加保险
    _ => !chewToilNameConfirmed || chewToilActive || gainingNutritionNow,
};
```

**适配层采样（Verse 侧）：**

```csharp
private bool IsEating()
{
    if (Pawn.CurJob?.def != JobDefOf.Ingest) return false;
    return SqueakEatOccurrence.ResolveMode(EatOnlyDuringChewing, EatIncludeDrugs) switch
    {
        SqueakEatOccurrenceMode.WholeJob => true,
        SqueakEatOccurrenceMode.GainingNutrition => IsGainingNutritionNow(),
        _ => SqueakEatOccurrence.AllowsOccurrence(SqueakEatOccurrenceMode.ChewingToil,
            IsGainingNutritionNow(), SampleChewingToil(), chewToilNameConfirmed),
    };
}

private bool SampleChewingToil() // 命中即把 chewToilNameConfirmed 置为 true（进程级静态，不 Scribe）
{
    if (Pawn.jobs?.curDriver is not JobDriver_Ingest driver) return false;
    if (!string.Equals(driver.CurToilString, SqueakEatOccurrence.ChewingToilDebugName, StringComparison.Ordinal)) return false;
    chewToilNameConfirmed = true;
    return true;
}
```

成本说明：`Pawn_JobTracker.curDriver` 是 public 字段、`JobDriver.CurToilString` 是 public 属性，返回既有 `debugName` 字符串（无分配）；`Ordinal` 比较无分配。仅在「父开 + 当前 job 为 Ingest」时求值。

**回落语义（本仓库定为「完整 job 级」，实现时务必照此）：**

- `chewToilNameConfirmed` 是进程级只读事实（该 toil 名是否存在），一旦采样命中即置 true。
- 未确认期间子开关**不生效**，行为等于出厂默认（整段 job）——不丢声音；确认后转为严格判定。
- 这意味着"游戏若改了 toil 名，子开关退化为无操作"，而不是"成瘾品静默"。

**UI 要求：**

- 子行紧挨父行的灰色短说明下方，缩进一致（同 34f 行高）。
- 父关时子行**禁用但可见**（置灰 + help 显示禁用原因），不是隐藏——避免"开关消失"的困惑；若接收方偏好隐藏，须保证下次绘制仍可达且不产生负 Rect。
- **父关必须把子项强制为 false**：UI 关闭父项时清零 + 落盘；`PostLoadInit` 对"父关子真"的手改配置归一为 false（防御性第三层，且纯规则本身也把父关解析为 `WholeJob`）。
- 高度测量按常量累加（父 34f + 父短说明 + 子 34f），**不要做成随状态动态变高**，否则 Measure/Draw 极易不一致（本项目 UI 复核专门查过这一点）。
- 文案：父 Tooltip 写明"零营养成瘾品默认不算，勾选下方可包含"；子 Label 用原版措辞「使用成瘾品」；子 Tooltip 写明"按咀嚼/点燃阶段判定，无法识别时回落完整进食流程"；另需一条禁用原因键（"需先开启上方开关"）。

**持久化：**

- 两个字段都 add-only、默认 false、默认值省略（不写节点、不 bump schema）。
- 父关 + 子真**不是合法持久态**：三层保证（UI 清零 / PostLoadInit 归一 / 纯规则父关优先）使其无法产生效果，也不会被写回。

**范围与命名注意（重要，已核过原版数据）：**

- 方案 B 的实际范围是"任意零营养可摄入物"，vanilla 里主要是成瘾品（烟卷 720 t、薄片 650 t 等零营养条目），模组内容里可能是任何无营养消耗品。
- **啤酒（`Beer`）在父开关单独开启时就已经算 `Eat`**：`Nutrition 0.08 > 0` ⇒ `IsNutritionGainingIngestible` 为真 ⇒ 走路阶段仍然不响、但**咀嚼阶段会响**；仙馔（`Ambrosia`，`Nutrition 0.2`、`baseIngestTicks 80`）同理。勾选子项不改变它们的行为，只补上零营养成瘾品。
- 若产品希望"父开子关时啤酒也静默"，需要额外一条"排除药物"判定（`ThingDef.IsDrug`），这是**另一个语义决定**，不是本规格的默认行为。

**可观测性（可选）：**

- 若担心 B 因 debugName 改名而静默退化，可在 Dev 诊断面板（非日志协议）显示当前 Eat 模式与 `chewToilNameConfirmed`；本项目明确**不新增日志事件**（日志协议是 characterization 冻结面）。

**与 3 态选择卡的对比：**

- 3 态（整段 job / 真正进食（营养）/ 真正吞咽（含成瘾品））语义更清晰，但控件更重、占高更多、需要改更多 UI 合同；SR 采用嵌套复选框（最小改动 + 依赖禁用行）。

## 6. 验收与回归门

- 单测（纯规则）：`ResolveMode` 四组合（含"父关 + 子真 = `WholeJob`"）+ 三模式判定 + **未确认 toil 名时回落完整 job 级** + `ChewingToilDebugName == "ChewIngestible"` + 两个默认值断言。
- 手工验收矩阵（三态）：meal / 烟卷 / 薄片 / **啤酒** / **仙馔** / 营养膏 / 背包吃 / 动物 / 尸体 ×（父关、父开子关、父开子开）；重点确认：父关与旧实现**完全一致**；父开子开时食物行为与「父开子关」一致（并集不改变食物）；父开子关时啤酒/仙馔仍会响（营养 > 0）；关父项后子项灰显且值归 false。
- 回归门：默认值不产生新存档节点 → settings fixture 零 delta；构建 0 warning；日志协议 characterization 不变。
- 文档同步面（本仓库这次的实际清单，含一个曾漏掉的）：架构合同、**UI codemap**、动作/作者指南、双语 CHANGELOG、UI 产品合同（若与"UI 冻结"口径冲突需书面记例外，且例外条目要覆盖父+子两个控件）、MEMORY/TODO。

## 7. 本仓库参考实现（可移植结构）

> 状态：§7 描述的是**已实施的单开关（方案 A）**；父 + 子两级形态见 §5.1（尚待实施）。

- 纯规则：`Source/SqueakyRatkin/SqueakEatOccurrence.cs`（`ChewingOnlyDefault = false`；`AllowsOccurrence(bool chewingOnly, bool gainingNutritionNow)`；零 Verse 引用）。
- 适配层采样：`Source/SqueakyRatkin/CompSqueaker.cs` — `IsEating()` + `IsGainingNutritionNow() => Pawn.jobs?.curDriver is IEatingDriver eating && eating.GainingNutritionNow`；开关关时短路，不采样。
- 设置字段与发布：`SqueakyRatkinSettings.cs`（字段默认取纯常量；`ApplyToRuntime()` / `NotifyCheapRuntimeChanged()` 两处发布静态旗标）。
- Scribe：`SqueakyRatkinSettings.ExposeData.cs` 一行；fixture host 镜像 `tools/SettingsFixtureGenerator/Contract/SettingsContract.cs`。
- UI：发声规则页「频率」组一个复选框 + 灰色短说明（子行见 §5.1），并在 `MeasureBasicsContentHeight` 同步高度。
- 单测/harness：`tools/KernelCharacterization/UnitTests.cs` 的 `EatOccurrenceRules`；纯文件显式链接进 harness（纯度门）。
- 本地化：`1.6/Languages/{English,ChineseSimplified}/Keyed/SqueakyRatkin.xml` 三条键（Label/Short/Tooltip），中英同序、零重复。

当前本仓库的实际判定（两级形态，节选自 `CompSqueaker.cs`）：

```csharp
private bool IsEating()
{
    if (Pawn.CurJob?.def != JobDefOf.Ingest) return false;
    return SqueakEatOccurrence.ResolveMode(EatOnlyDuringChewing, EatIncludeDrugs) switch
    {
        SqueakEatOccurrenceMode.WholeJob => true,
        SqueakEatOccurrenceMode.GainingNutrition => IsGainingNutritionNow(),
        _ => SqueakEatOccurrence.AllowsOccurrence(SqueakEatOccurrenceMode.ChewingToil,
            IsGainingNutritionNow(), SampleChewingToil(), chewToilNameConfirmed),
    };
}
```

### 7.1 落地记录与改动面（本仓库已实施）

| 文件 | 改动 | 实际结果 |
| --- | --- | --- |
| `SqueakEatOccurrence.cs` | 模式枚举 + `ResolveMode` + 四参 `AllowsOccurrence` + 两默认常量 + `ChewingToilDebugName` | 已实施，纯文件，纯度门不变 |
| `CompSqueaker.cs` | 静态 `EatIncludeDrugs` + 进程级 `chewToilNameConfirmed` + `IsEating()` switch + `SampleChewingToil()` | 已实施 |
| `SqueakyRatkinSettings.cs` | 子字段 + 两处发布（`eatOnlyDuringChewing && eatIncludeDrugs`）+ 子行 `Toggle(enabled:/disabledReason:)` + 父关清零子项 + 高度 +34f | 已实施 |
| `SqueakyRatkinSettings.ExposeData.cs` | 子项 Scribe 一行 + PostLoadInit 归一（父关 ⇒ 子 false） | 已实施 |
| `tools/SettingsFixtureGenerator/Contract/SettingsContract.cs` | 镜像子字段 | 已实施（fixtures 零 delta 已验证） |
| `tools/KernelCharacterization/UnitTests.cs` | `EatOccurrenceRules` 扩为模式 + 回落 + 常量 + 默认值断言 | 已实施 |
| 双语 Keyed ×2 | 子项 Label/Tooltip/禁用原因 + 父 Tooltip 边界句改写 | 已实施（542 键、零重复、中英对齐） |
| 文档同步面 | 合同 §3、SKILL §7、settings-ui 合同例外（覆盖父+子）、CHANGELOG ×2、Source codemap、**UI codemap**、MEMORY/TODO | 已实施 |

明确不改：日志协议（不新增事件）、动作 ABI、resolver/池、冷却、XML ABI。可选增强：Dev 诊断面板显示当前模式与 `chewToilNameConfirmed`（Debug 面）。

## 8. 给对方的待决问题

1. 是否采用 §5.1 的父 + 子两级形态？（父 = 仅真正进食；子 = 使用成瘾品 → 方案 B）还是改成 3 态选择卡？
2. 子项命名与文案范围：`使用成瘾品` 在原版等价于药物，但实现覆盖面是"任意零营养可摄入物"；模组内容可能需要更宽措辞。
3. 是否接受"啤酒/仙馔等带营养成瘾品在父开子关时仍然会响"（本仓库默认接受，因为营养权威 `CachedNutrition > 0`）；若要它们在父开子关时静默，需要额外一条"排除药物"判定（`ThingDef.IsDrug`）。
4. 回落的验收口径：本仓库取"toil 名未确认 ⇒ 完整 job 级（不丢声音）"；若接收方更在意"严格"，可改成"未确认即静默"，但那会回到静默 hole，不建议。
5. 是否泛化到其他动作？`Sleep`（在床/未上床）、`Work`、`Social`、`Joy` 目前同样是 job 级或粗略判定，同类"粒度"问题会重复出现。
6. 粒度偏好放哪一层：策略层（纯函数 + 注入采样）还是动作定义/注册表（与动作门 `SqueakActionDef` / `allowExternalActions` 的关系）？
7. 默认值是否写进兼容政策（一旦发布，"父关 = 默认 job 级"就不允许再翻）。
8. 是否需要一项 Dev 诊断（如诊断面板显示当前模式与 `chewToilNameConfirmed`）以便发现 B 的静默退化；注意不要新增日志事件。

## 附：本仓库本轮改动（`dev` 分支，未提交；仅作参考）

新增 `Source/SqueakyRatkin/SqueakEatOccurrence.cs`；改 `CompSqueaker.cs`、`SqueakyRatkinSettings.cs`、`SqueakyRatkinSettings.ExposeData.cs`、`Source/SqueakyRatkin/codemap.md`、`Source/SqueakyRatkin/UI/codemap.md`、`tools/KernelCharacterization/{KernelCharacterization.csproj,UnitTests.cs}`、`tools/SettingsFixtureGenerator/Contract/SettingsContract.cs`、两份 Keyed XML、`docs/{project-architecture-contract.md,settings-ui-product-contract-zh.md,CHANGELOG.md,CHANGELOG.zh-CN.md}`、`.github/skills/squeaky-voicepack-authoring/SKILL.md`、`MEMORY.md`、`TODO.md`。

注 1：复核方当时判断 vanilla 无公开 toil 权威；后续核到 `JobDriver.CurToilString`（public）可用，方案 B 因此可行并已在本仓库落地。
注 2：§5.1 的父 + 子两级形态已在本仓库实施（2026-08-23）；本文档同时作为对方的落地规格。
