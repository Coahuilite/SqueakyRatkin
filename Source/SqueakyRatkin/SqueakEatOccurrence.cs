namespace SqueakyRatkin;

// Eat 触发粒度纯决策（2026-08-23 维护者决定，承接 Steam 评论反馈）：默认保持 job 级派发——整个
// Ingest job（取食 → 端食物走到餐桌 → 吞咽 → 收尾）都算 Eat，这是招牌手感，不得默认收窄。
// 两级玩家开关 → 三种模式：
//   ① 父关 = WholeJob：整段 Ingest job 都算 Eat；
//   ② 父开、子关 = GainingNutrition：只有 vanilla「正在摄入营养」时算（零营养成瘾品不算）；
//   ③ 父开、子开 = ChewingToil：按 vanilla「咀嚼/点燃」toil 判定（成瘾品也算）；
//      该 toil 名在本进程内未被确认存在时回落**完整 job 级**（fail-open，绝不静默丢失发声）。
// 语义边界：vanilla 的营养权威要求 `CachedNutrition > 0`，因此啤酒（0.08）、仙馔（0.2）等带营养的
// 成瘾品在模式②下就已经算 Eat；模式③靠 toil 名覆盖烟卷/薄片等零营养成瘾品。
// 本文件零 Verse 引用（与 SqueakActionPlan/SqueakTimingModel 同属漏斗纯文件，harness 显式链接）；
// Verse 采样（CurJob / curDriver / 营养判定）留在适配层 CompSqueaker。

/// <summary>Eat 的 occurrence 粒度模式。玩家开关只决定“在哪一阶段算 Eat”，不改变任何路由或音频选择。</summary>
public enum SqueakEatOccurrenceMode { WholeJob, GainingNutrition, ChewingToil }

/// <summary>Eat 动作的 occurrence 粒度规则（纯函数）。</summary>
public static class SqueakEatOccurrence
{
    /// <summary>父开关出厂默认：false = 不要求“真正进食”，整个 Ingest job 都算 Eat（含端食物赶路）。
    /// 单一来源：设置字段初始化直接取此常量（Scribe 默认值与 fixture host 镜像同步，由 harness 单测 +
    /// fixtures 零 delta 门共同锁定）。</summary>
    public const bool ChewingOnlyDefault = false;

    /// <summary>子开关出厂默认：false = 不含成瘾品（父开关关闭时该值必须保持 false，不落盘 true）。</summary>
    public const bool IncludeDrugsDefault = false;

    /// <summary>vanilla Core 咀嚼/点燃 toil 的 debugName：`Toils_Ingest.ChewIngestible` 由
    /// `ToilMaker.MakeToil("ChewIngestible")` 创建，`Toil.ToString()` 即返回该 debugName。
    /// 依赖名称而非公开接口；名称一旦消失（Ludeon 改名 / 非 vanilla 驱动），判定按“未确认”处理，
    /// 回落完整 job 级而不是静默。</summary>
    public const string ChewingToilDebugName = "ChewIngestible";

    /// <summary>两级开关 → 有效模式。父关时无论子项取值一律 WholeJob（父关必须让子项失效）。</summary>
    public static SqueakEatOccurrenceMode ResolveMode(bool chewingOnly, bool includeDrugs)
        => !chewingOnly ? SqueakEatOccurrenceMode.WholeJob
            : includeDrugs ? SqueakEatOccurrenceMode.ChewingToil
            : SqueakEatOccurrenceMode.GainingNutrition;

    /// <summary><paramref name="gainingNutritionNow"/> = vanilla `IEatingDriver.GainingNutritionNow` 采样；
    /// <paramref name="chewToilActive"/> = 当前 toil 名命中 <see cref="ChewingToilDebugName"/>；
    /// <paramref name="chewToilNameConfirmed"/> = 适配层是否已在本次进程内确认过该 toil 名存在。</summary>
    public static bool AllowsOccurrence(SqueakEatOccurrenceMode mode, bool gainingNutritionNow,
        bool chewToilActive, bool chewToilNameConfirmed) => mode switch
    {
        SqueakEatOccurrenceMode.WholeJob => true,
        SqueakEatOccurrenceMode.GainingNutrition => gainingNutritionNow,
        // 未确认 toil 名（改名 / 非 vanilla 驱动 / 本进程尚未采样到）→ 回落完整 job 级；
        // 已确认时按咀嚼/点燃判定，营养并集作为附加保险。
        _ => !chewToilNameConfirmed || chewToilActive || gainingNutritionNow,
    };
}
