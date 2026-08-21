namespace SqueakyRatkin;

// 适配层产品元数据（0.3.1 波 4a：SqueakActionPlan/SqueakTriggerInvocation/SqueakActionDefinition 等
// 纯类型已移入 Kernel/SqueakActionPlan.cs；本文件只保留 SR 产品常量与绑定 SqueakActionConfig 的工厂，
// 不入内核编译单元——「SqueakActionModel 出 harness」）。

/// <summary>Single source for the shipped built-in actions and their SoundDef keys.
/// 0.3.1 波 3c：Crying/Giggling append（序数 15/16）；其 AudioKey 指向不存在的 SoundDef（GetNamedSilentFail
/// → 静默），内置表不列条目，pack 声明才发声（决策 §4.7）。</summary>
public static class SqueakActionDefinitions
{
    public const int Count = 17;
    private static readonly SqueakActionDefinition[] definitions =
    {
        new(SqueakAction.Call, "SR.Action.Call", "SR_Call", SqueakVocalGatePolicy.ApplyTalkingGate, SqueakActionScopeSupport.AnyOccurrence, SqueakActionScope.AnyOccurrence),
        new(SqueakAction.Eat, "SR.Action.Eat", "SR_Eat", SqueakVocalGatePolicy.ApplyTalkingGate, SqueakActionScopeSupport.AnyOccurrence, SqueakActionScope.AnyOccurrence),
        new(SqueakAction.Sleep, "SR.Action.Sleep", "SR_Sleep", SqueakVocalGatePolicy.ApplyTalkingGate, SqueakActionScopeSupport.AnyOccurrence, SqueakActionScope.AnyOccurrence),
        new(SqueakAction.Wounded, "SR.Action.Wounded", "SR_Wounded", SqueakVocalGatePolicy.ApplyTalkingGate, SqueakActionScopeSupport.AnyOccurrence, SqueakActionScope.AnyOccurrence),
        new(SqueakAction.Select, "SR.Action.Select", "SR_Select", SqueakVocalGatePolicy.ApplyTalkingGate, SqueakActionScopeSupport.AnyOccurrence, SqueakActionScope.AnyOccurrence),
        new(SqueakAction.Move, "SR.Action.Move", "SR_Move", SqueakVocalGatePolicy.ApplyTalkingGate, SqueakActionScopeSupport.AnyOccurrence, SqueakActionScope.AnyOccurrence),
        new(SqueakAction.Social, "SR.Action.Social", "SR_Social", SqueakVocalGatePolicy.ApplyTalkingGate, SqueakActionScopeSupport.AnyOccurrence, SqueakActionScope.AnyOccurrence),
        new(SqueakAction.Joy, "SR.Action.Joy", "SR_Joy", SqueakVocalGatePolicy.ApplyTalkingGate, SqueakActionScopeSupport.AnyOccurrence, SqueakActionScope.AnyOccurrence),
        new(SqueakAction.Death, "SR.Action.Death", "SR_Death", SqueakVocalGatePolicy.ExemptTalkingGate, SqueakActionScopeSupport.AnyOccurrence, SqueakActionScope.AnyOccurrence),
        new(SqueakAction.Draft, "SR.Action.Draft", "SR_Draft", SqueakVocalGatePolicy.ApplyTalkingGate, SqueakActionScopeSupport.ActiveCommand, SqueakActionScope.ActiveCommand),
        new(SqueakAction.Undraft, "SR.Action.Undraft", "SR_Undraft", SqueakVocalGatePolicy.ApplyTalkingGate, SqueakActionScopeSupport.ActiveCommand, SqueakActionScope.ActiveCommand),
        new(SqueakAction.Attack, "SR.Action.Attack", "SR_Attack", SqueakVocalGatePolicy.ApplyTalkingGate, SqueakActionScopeSupport.AnyOccurrence | SqueakActionScopeSupport.ActiveCommand, SqueakActionScope.AnyOccurrence),
        new(SqueakAction.Work, "SR.Action.Work", "SR_Work", SqueakVocalGatePolicy.ApplyTalkingGate, SqueakActionScopeSupport.AnyOccurrence | SqueakActionScopeSupport.ActiveCommand, SqueakActionScope.ActiveCommand),
        new(SqueakAction.Equip, "SR.Action.Equip", "SR_Equip", SqueakVocalGatePolicy.ApplyTalkingGate, SqueakActionScopeSupport.ActiveCommand, SqueakActionScope.ActiveCommand),
        new(SqueakAction.MentalBreak, "SR.Action.MentalBreak", "SR_MentalBreak", SqueakVocalGatePolicy.ApplyTalkingGate, SqueakActionScopeSupport.AnyOccurrence, SqueakActionScope.AnyOccurrence),
        new(SqueakAction.Crying, "SR.Action.Crying", "SR_Crying", SqueakVocalGatePolicy.ApplyTalkingGate, SqueakActionScopeSupport.AnyOccurrence, SqueakActionScope.AnyOccurrence),
        new(SqueakAction.Giggling, "SR.Action.Giggling", "SR_Giggling", SqueakVocalGatePolicy.ApplyTalkingGate, SqueakActionScopeSupport.AnyOccurrence, SqueakActionScope.AnyOccurrence),
    };
    public static SqueakActionDefinition Get(SqueakAction action) => definitions[(int)action];
    public static bool IsKnown(SqueakAction action) => (uint)action < Count;
    public static SqueakActionScope NormalizeScope(SqueakAction action, SqueakActionScope scope)
    {
        if (scope == SqueakActionScope.Disabled) return scope;
        SqueakActionDefinition definition = Get(action);
        SqueakActionScopeSupport needed = scope == SqueakActionScope.ActiveCommand ? SqueakActionScopeSupport.ActiveCommand : SqueakActionScopeSupport.AnyOccurrence;
        return (definition.SupportedScopes & needed) != 0 ? scope : definition.DefaultScope;
    }
}

/// <summary>
/// Adapter-side plan factories（0.3.1 波 4a）：把纯 <see cref="SqueakActionPlan"/> 形状绑定到产品元数据
/// （<see cref="SqueakActionDefinitions"/> 的 SR_* 键）与不可链接的 <see cref="SqueakActionConfig"/>。
/// 内核编译集不包含本类（SR 产品常量不入内核编译单元）。
/// </summary>
public static class SqueakActionPlanFactory
{
    public static SqueakActionPlan Unconfigured(SqueakAction action) => new(SqueakActionDefinitions.Get(action), false, SqueakTriggerMode.RandomOneShot, 300, .02f, false, SqueakCooldownClock.GameTicks);

    public static SqueakActionPlan FromLegacy(SqueakActionConfig config) => new(SqueakActionDefinitions.Get(config.action), true, config.mode, config.minIntervalTicks, config.probabilityPerCheck, config.ignoreGlobalCooldown, config.cooldownClock);
}
