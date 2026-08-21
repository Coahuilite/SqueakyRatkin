namespace SqueakyRatkin;

public enum SqueakVocalGatePolicy { ApplyTalkingGate, ExemptTalkingGate }
public enum SqueakActionScope { Disabled, AnyOccurrence, ActiveCommand }

[System.Flags]
public enum SqueakActionScopeSupport { None = 0, AnyOccurrence = 1, ActiveCommand = 2 }

/// <summary>Fixed built-in action metadata; this deliberately is not an extensible registry.</summary>
public readonly struct SqueakActionDefinition
{
    public readonly SqueakAction Action;
    public readonly string DisplayKey;
    public readonly string AudioKey;
    public readonly SqueakVocalGatePolicy VocalGatePolicy;
    public readonly SqueakActionScopeSupport SupportedScopes;
    public readonly SqueakActionScope DefaultScope;
    internal SqueakActionDefinition(SqueakAction action, string displayKey, string audioKey, SqueakVocalGatePolicy vocalGatePolicy, SqueakActionScopeSupport supportedScopes, SqueakActionScope defaultScope)
    { Action = action; DisplayKey = displayKey; AudioKey = audioKey; VocalGatePolicy = vocalGatePolicy; SupportedScopes = supportedScopes; DefaultScope = defaultScope; }
}

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

/// <summary>Per-Comp fixed action plan populated from XML. Missing actions remain unconfigured.</summary>
public readonly struct SqueakActionPlan
{
    public readonly SqueakActionDefinition Definition;
    public readonly bool Configured;
    public readonly SqueakTriggerMode Mode;
    public readonly int MinIntervalTicks;
    public readonly float ProbabilityPerCheck;
    public readonly bool IgnoreGlobalCooldown;
    public readonly SqueakCooldownClock CooldownClock;
    internal SqueakActionPlan(SqueakActionDefinition definition, bool configured, SqueakTriggerMode mode, int minIntervalTicks, float probabilityPerCheck, bool ignoreGlobalCooldown, SqueakCooldownClock cooldownClock)
    { Definition = definition; Configured = configured; Mode = mode; MinIntervalTicks = minIntervalTicks; ProbabilityPerCheck = probabilityPerCheck; IgnoreGlobalCooldown = ignoreGlobalCooldown; CooldownClock = cooldownClock; }
    internal static SqueakActionPlan Unconfigured(SqueakAction action) => new(SqueakActionDefinitions.Get(action), false, SqueakTriggerMode.RandomOneShot, 300, .02f, false, SqueakCooldownClock.GameTicks);
    internal static SqueakActionPlan FromLegacy(SqueakActionConfig config) => new(SqueakActionDefinitions.Get(config.action), true, config.mode, config.minIntervalTicks, config.probabilityPerCheck, config.ignoreGlobalCooldown, config.cooldownClock);
}

public enum SqueakTriggerOrigin { Periodic, Wounded, Select, Death, Draft, Undraft, Attack, Equip, MentalBreak, Crying, Giggling }
public enum SqueakInvocationSource { Periodic, StateEvent, PlayerSelection, ActiveCommand }

/// <summary>Explicit trigger source; non-periodic hooks preserve legacy probability skipping.</summary>
public readonly struct SqueakTriggerInvocation
{
    public readonly SqueakTriggerOrigin Origin;
    public readonly SqueakInvocationSource Source;
    public bool SkipsRandomOneShotProbability => Origin != SqueakTriggerOrigin.Periodic;
    public bool IsExternal => Origin != SqueakTriggerOrigin.Periodic;
    public bool IsActiveCommand => Source == SqueakInvocationSource.ActiveCommand;
    public SqueakTriggerInvocation(SqueakTriggerOrigin origin, SqueakInvocationSource source) { Origin = origin; Source = source; }
}
