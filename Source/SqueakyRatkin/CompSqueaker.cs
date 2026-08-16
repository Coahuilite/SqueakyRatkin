using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace SqueakyRatkin;

public enum SqueakMood { Good, Neutral, Bad, Break }
// Keep the original nine serialized enum values stable. New built-ins are append-only.
public enum SqueakAction { Call, Eat, Sleep, Wounded, Select, Move, Social, Joy, Death, Draft, Undraft, Attack, Work, Equip, MentalBreak }
public enum SqueakCooldownClock { GameTicks, Realtime }

/// <summary>Last completed trigger attempt. This is runtime-only diagnostic state and is never Scribed.</summary>
public enum SqueakTriggerOutcome
{
    Disabled,
    ProbabilityRejected,
    ActionCooldown,
    GlobalCooldown,
    VocalOrgansSilent,
    TalkingRejected,
    NoSoundFallback,
    Dispatched,
    EligibilityRejected,
    PlaybackFailed,
    PeriodicStartupPending
}

/// <summary>Pure timing inputs sampled once by a caller before evaluating a trigger.</summary>
public readonly struct SqueakTimingInput
{
    public readonly int NowTick;
    public readonly float NowRealtime;
    public readonly float TimeSpeedMultiplier;
    public readonly SqueakActionPlan Plan;
    public readonly ResolvedSqueakContext Context;
    public readonly RuntimeActionDelta ActionDelta;
    public readonly int LastActionTick;
    public readonly float LastActionRealtime;
    public readonly int LastGlobalTick;
    public readonly int GlobalBaseTicks;
    public readonly float MasterMultiplier;
    public readonly bool ScaleWithTimeSpeed;
    public readonly float PeriodicPopulationScale;

    public SqueakTimingInput(int nowTick, float nowRealtime, float timeSpeedMultiplier, SqueakActionPlan plan,
        ResolvedSqueakContext context, RuntimeActionDelta actionDelta, int lastActionTick, float lastActionRealtime,
        int lastGlobalTick, int globalBaseTicks, float masterMultiplier, bool scaleWithTimeSpeed, float periodicPopulationScale)
    {
        NowTick = nowTick; NowRealtime = nowRealtime; TimeSpeedMultiplier = timeSpeedMultiplier;
        Plan = plan; Context = context; ActionDelta = actionDelta;
        LastActionTick = lastActionTick; LastActionRealtime = lastActionRealtime; LastGlobalTick = lastGlobalTick;
        GlobalBaseTicks = globalBaseTicks;
        MasterMultiplier = masterMultiplier; ScaleWithTimeSpeed = scaleWithTimeSpeed; PeriodicPopulationScale = periodicPopulationScale;
    }
}

/// <summary>Pure, immutable result shared by the production gates and overlay diagnostics.</summary>
public readonly struct SqueakTimingEvaluation
{
    public readonly int? ActionIntervalTicks;
    public readonly float? ActionIntervalSeconds;
    public readonly int GlobalCooldownTicks;
    public readonly int? ActionRemainingTicks;
    public readonly float? ActionRemainingSeconds;
    public readonly int GlobalRemainingTicks;
    public readonly bool ActionReady;
    public readonly bool GlobalApplicable;
    public readonly bool GlobalReady;
    public readonly bool TimingReady;

    internal SqueakTimingEvaluation(int? actionIntervalTicks, float? actionIntervalSeconds, int globalCooldownTicks,
        int? actionRemainingTicks, float? actionRemainingSeconds, int globalRemainingTicks, bool actionReady,
        bool globalApplicable, bool globalReady)
    {
        ActionIntervalTicks = actionIntervalTicks; ActionIntervalSeconds = actionIntervalSeconds;
        GlobalCooldownTicks = globalCooldownTicks; ActionRemainingTicks = actionRemainingTicks;
        ActionRemainingSeconds = actionRemainingSeconds; GlobalRemainingTicks = globalRemainingTicks;
        ActionReady = actionReady; GlobalApplicable = globalApplicable; GlobalReady = globalReady;
        TimingReady = actionReady && (!globalApplicable || globalReady);
    }
}

/// <summary>Compact runtime-only trigger result retained only while diagnostics are enabled.</summary>
public readonly struct SqueakRecentOutcome
{
    public readonly SqueakTriggerOutcome Outcome;
    public readonly SqueakAction Action;
    public readonly int Tick;
    public readonly float Realtime;
    public readonly bool CooldownConsumed;
    public readonly SoundDef? Sound;
    public readonly SqueakSoundSource SoundSource;

    internal SqueakRecentOutcome(SqueakTriggerOutcome outcome, SqueakAction action, int tick, float realtime,
        bool cooldownConsumed, SoundDef? sound, SqueakSoundSource soundSource)
    {
        Outcome = outcome; Action = action; Tick = tick; Realtime = realtime;
        CooldownConsumed = cooldownConsumed; Sound = sound; SoundSource = soundSource;
    }
}

/// <summary>Read-only overlay input. Obtaining it never consumes random state or updates trigger state.</summary>
internal readonly struct SqueakDiagnosticSnapshot
{
    public readonly SqueakAction? CurrentTimingAction;
    public readonly bool CurrentActionEnabled;
    public readonly SqueakTriggerMode? CurrentTriggerMode;
    public readonly SqueakCooldownClock? CurrentCooldownClock;
    public readonly float CurrentActionIntervalMultiplier;
    public readonly SqueakTimingEvaluation Timing;
    public readonly SqueakTimingEvaluation BaseTiming;
    public readonly SqueakPeriodicPopulation.Snapshot Population;
    public readonly float MasterMultiplier;
    public readonly XenotypeDef? Xenotype;
    public readonly float XenotypeIntervalMultiplier;
    public readonly float TimeSpeedMultiplier;
    public readonly float EffectiveProbability;
    public readonly float BaseProbability;
    public readonly bool StartupPending;
    public readonly bool EffectiveTimingReady;
    public readonly SqueakVocalCapability VocalCapability;
    public readonly bool TalkingGateApplied;
    public readonly bool CurrentActionDeathExempt;
    public readonly SqueakRecentOutcome? LastEvaluation;
    public readonly SqueakRecentOutcome? LastSignificantOutcome;

    internal SqueakDiagnosticSnapshot(SqueakAction? currentTimingAction, bool currentActionEnabled,
        SqueakTriggerMode? currentTriggerMode, SqueakCooldownClock? currentCooldownClock,
        float currentActionIntervalMultiplier, SqueakTimingEvaluation timing, SqueakTimingEvaluation baseTiming, SqueakPeriodicPopulation.Snapshot population, float masterMultiplier,
        XenotypeDef? xenotype, float xenotypeIntervalMultiplier,
        float timeSpeedMultiplier, float effectiveProbability, float baseProbability, bool startupPending, bool effectiveTimingReady, SqueakVocalCapability vocalCapability,
        bool talkingGateApplied, bool currentActionDeathExempt,
        SqueakRecentOutcome? lastEvaluation, SqueakRecentOutcome? lastSignificantOutcome)
    {
        CurrentTimingAction = currentTimingAction; CurrentActionEnabled = currentActionEnabled;
        CurrentTriggerMode = currentTriggerMode; CurrentCooldownClock = currentCooldownClock;
        CurrentActionIntervalMultiplier = currentActionIntervalMultiplier; Timing = timing; BaseTiming = baseTiming; Population = population;
        MasterMultiplier = masterMultiplier; Xenotype = xenotype; XenotypeIntervalMultiplier = xenotypeIntervalMultiplier;
        TimeSpeedMultiplier = timeSpeedMultiplier; EffectiveProbability = effectiveProbability; BaseProbability = baseProbability;
        StartupPending = startupPending; EffectiveTimingReady = effectiveTimingReady;
        VocalCapability = vocalCapability; TalkingGateApplied = talkingGateApplied;
        CurrentActionDeathExempt = currentActionDeathExempt;
        LastEvaluation = lastEvaluation; LastSignificantOutcome = lastSignificantOutcome;
    }
}

public enum SqueakFinalPreviewStatus { NoEligibleSound, PawnOrMapUnavailable, IneligibleSound, Dispatched, Exception }

public enum SqueakPlaybackAttemptResult { NoEligibleSound, EligibilityRejected, Dispatched, Exception }

public readonly struct SqueakPlaybackAttempt
{
    public readonly SqueakPlaybackAttemptResult Result;
    public readonly SqueakSoundChoice Choice;
    internal SqueakPlaybackAttempt(SqueakPlaybackAttemptResult result, SqueakSoundChoice choice) { Result = result; Choice = choice; }
}

/// <summary>Read-only final-preview plan/result for the Dev audio browser.</summary>
public readonly struct SqueakFinalPreviewResult
{
    public readonly Pawn? Pawn;
    public readonly XenotypeDef? Xenotype;
    public readonly SqueakMood Mood;
    public readonly SoundDef? Sound;
    public readonly SqueakSoundSource Source;
    public readonly string? PoolStableKey;
    public readonly SqueakFinalPreviewStatus Status;
    public readonly SqueakSoundPlayability Playability;
    public readonly string Reason;
    internal SqueakFinalPreviewResult(Pawn? pawn, XenotypeDef? xenotype, SqueakMood mood, SqueakSoundChoice choice,
        SqueakFinalPreviewStatus status, SqueakSoundPlayability playability, string reason = "")
    {
        Pawn = pawn; Xenotype = xenotype; Mood = mood; Sound = choice.Sound; Source = choice.Source;
        PoolStableKey = choice.PoolStableKey; Status = status; Playability = playability;
        Reason = reason;
    }
}

/// <summary>触发模式,由 XML 配置驱动,C# 通用适配。</summary>
public enum SqueakTriggerMode
{
    EachTime,
    RandomOneShot,
    External,
    Sustained
}

/// <summary>单个动作的触发配置。</summary>
public class SqueakActionConfig
{
    public SqueakAction action = SqueakAction.Call;
    public SqueakTriggerMode mode = SqueakTriggerMode.RandomOneShot;
    public int minIntervalTicks = 300;
    public float probabilityPerCheck = 0.02f;
    public bool ignoreGlobalCooldown = false;
    public SqueakCooldownClock cooldownClock = SqueakCooldownClock.GameTicks;
}

public class SqueakDistancePresetConfig
{
    public SqueakDistancePreset preset = SqueakDistancePreset.Balanced;
    public FloatRange range = new(15f, 50f);
}

/// <summary>
/// 挂在 Ratkin pawn 上的自驱动发声组件。
/// 配置三层:CompProperties(XML默认) ← ModSettings(玩家override) ← 运行时。
/// 心情靠运行时 pitchFactor/volumeFactor 调制,每动作只需 1 个 SoundDef + 1 套中性音频。
/// </summary>
public class CompSqueaker : ThingComp
{
    private static readonly string[] SocialJobMarkers = { "Chat", "Social", "Visit", "Lovin", "Entertain" };
    public static bool ScaleCooldownWithTimeSpeed = true;
    public static bool ScaleFrequencyWithTalking = true;
    public static bool ScalePeriodicWithAudiblePopulation = true;
    public static float GlobalCooldownMultiplier = 1f;
    public static bool DiagnosticsEnabled;

    private static readonly Dictionary<SqueakAction, SoundDef?> SoundCacheMixed = new();
    private static readonly HashSet<SqueakAction> MissingSoundWarnings = new();
    private static bool soundCacheInitialized;
    private static FloatRange activeDistanceRange = new(15f, 50f);

    private readonly SqueakActionPlan[] actionPlans = new SqueakActionPlan[SqueakActionDefinitions.Count];
    private readonly int[] lastTriggerTick = new int[SqueakActionDefinitions.Count];
    private readonly float[] lastTriggerRealTime = new float[SqueakActionDefinitions.Count];
    private readonly Dictionary<SqueakMood, SqueakMoodMod> moodModMap = new();
    private int lastAnyTriggerTick = int.MinValue / 2;
    private SqueakRuntimeSnapshot? cachedRuntimeSnapshot;
    private XenotypeDef? cachedXenotype;
    private ResolvedSqueakContext cachedSqueakContext = ResolvedSqueakContext.GlobalOnly;
    private SqueakRecentOutcome? lastEvaluation;
    private SqueakRecentOutcome? lastSignificantOutcome;
    // Runtime-only, non-Scribe anchor. Every Periodic action phase remains rooted at this spawn tick.
    private int startupAnchorTick;
    private bool startupAnchorRecorded;
    private readonly bool[] periodicStartupPhaseMaterialized = new bool[SqueakActionDefinitions.Count];
    private readonly int[] periodicStartupReadyTicks = new int[SqueakActionDefinitions.Count];
    private Map? registeredMap;

    private Pawn Pawn => (Pawn)parent;
    internal Pawn RegisteredPawn => Pawn;
    private CompProperties_Squeaker Props => (CompProperties_Squeaker)props;

    public override void Initialize(CompProperties props)
    {
        base.Initialize(props);
        lastAnyTriggerTick = int.MinValue / 2;
        for (int i = 0; i < SqueakActionDefinitions.Count; i++)
        {
            actionPlans[i] = SqueakActionPlan.Unconfigured((SqueakAction)i);
            lastTriggerTick[i] = int.MinValue / 2;
            lastTriggerRealTime[i] = -1_000_000f;
        }
        foreach (SqueakActionConfig cfg in Props.actions)
        {
            if (cfg != null && SqueakActionDefinitions.IsKnown(cfg.action)) actionPlans[(int)cfg.action] = SqueakActionPlan.FromLegacy(cfg);
        }

        foreach (SqueakMoodMod mod in Props.moodMods)
        {
            moodModMap[mod.mood] = mod;
        }
    }

    private SqueakMood CurrentMood
    {
        get
        {
            if (Pawn.InMentalState)
            {
                return SqueakMood.Break;
            }

            Need_Mood? mood = Pawn.needs?.mood;
            if (mood == null)
            {
                return SqueakMood.Neutral;
            }

            float p = mood.CurLevelPercentage;
            if (p > 0.65f)
            {
                return SqueakMood.Good;
            }

            return p < 0.35f ? SqueakMood.Bad : SqueakMood.Neutral;
        }
    }

    private SqueakAction? CurrentAction
    {
        get
        {
            if (IsSleeping())
            {
                return SqueakAction.Sleep;
            }

            if (IsEating())
            {
                return SqueakAction.Eat;
            }

            if (IsSocializing())
            {
                return SqueakAction.Social;
            }

            if (IsJoyJob())
            {
                return SqueakAction.Joy;
            }

            if (IsMoving())
            {
                return SqueakAction.Move;
            }

            if (IsWorking())
            {
                return SqueakAction.Work;
            }

            return SqueakAction.Call;
        }
    }

    public override void CompTick()
    {
        SynchronizePeriodicMembership();
        if (!Pawn.Spawned || Pawn.MapHeld == null || Pawn.MapHeld != Find.CurrentMap)
        {
            return;
        }

        if (!Find.CameraDriver.CurrentViewRect.ExpandedBy(10).Contains(Pawn.Position))
        {
            return;
        }

        SqueakAction? action = CurrentAction;
        if (action == null) return;
        SqueakActionPlan plan = actionPlans[(int)action.Value];
        if (!plan.Configured) return;

        switch (plan.Mode)
        {
            case SqueakTriggerMode.EachTime:
                TryTrigger(plan, PeriodicInvocationFor(action.Value));
                break;
            case SqueakTriggerMode.RandomOneShot:
                TryTrigger(plan, PeriodicInvocationFor(action.Value));
                break;
            case SqueakTriggerMode.External:
            case SqueakTriggerMode.Sustained:
                break;
        }
    }

    public void Notify_Wounded() => NotifyExternal(SqueakAction.Wounded, SqueakTriggerOrigin.Wounded, SqueakInvocationSource.StateEvent);
    public void Notify_Select() => NotifyExternal(SqueakAction.Select, SqueakTriggerOrigin.Select, SqueakInvocationSource.PlayerSelection);
    public void Notify_Death() => NotifyExternal(SqueakAction.Death, SqueakTriggerOrigin.Death, SqueakInvocationSource.StateEvent);
    public void Notify_Draft(bool drafted) => NotifyExternal(drafted ? SqueakAction.Draft : SqueakAction.Undraft, drafted ? SqueakTriggerOrigin.Draft : SqueakTriggerOrigin.Undraft, SqueakInvocationSource.ActiveCommand);
    public void Notify_Attack() => NotifyExternal(SqueakAction.Attack, SqueakTriggerOrigin.Attack, IsCurrentJobPlayerCommand() ? SqueakInvocationSource.ActiveCommand : SqueakInvocationSource.StateEvent);
    public void Notify_Equip()
    {
        // Equipment tracker notifications also cover AI, loading, and system equipment changes.
        // Equip is deliberately only the player's ordered Core Equip job, regardless of its scope.
        if (!IsCurrentEquipJobPlayerCommand()) return;
        NotifyExternal(SqueakAction.Equip, SqueakTriggerOrigin.Equip, SqueakInvocationSource.ActiveCommand);
    }

    public override void PostSpawnSetup(bool respawningAfterLoad)
    {
        base.PostSpawnSetup(respawningAfterLoad);
        // Avoid resolver or population rebuild work here: batch spawning remains O(N).
        registeredMap = Pawn.Spawned ? Pawn.MapHeld : null;
        SqueakPeriodicPopulation.Register(this, registeredMap);
        if (!startupAnchorRecorded)
        {
            startupAnchorTick = Find.TickManager.TicksGame;
            startupAnchorRecorded = true;
        }
    }

    public override void PostDestroy(DestroyMode mode, Map previousMap)
    {
        SqueakPeriodicPopulation.Unregister(this, previousMap);
        registeredMap = null;
        base.PostDestroy(mode, previousMap);
    }
    internal void NotifyPeriodicDespawn(Map map)
    {
        SqueakPeriodicPopulation.Unregister(this, map);
        if (ReferenceEquals(registeredMap, map)) registeredMap = null;
    }
    public void Notify_MentalBreak() => NotifyExternal(SqueakAction.MentalBreak, SqueakTriggerOrigin.MentalBreak, SqueakInvocationSource.StateEvent);

    private void NotifyExternal(SqueakAction action, SqueakTriggerOrigin origin, SqueakInvocationSource source)
    {
        SynchronizePeriodicMembership();
        if (!Pawn.Spawned || Pawn.MapHeld != Find.CurrentMap)
        {
            return;
        }

        if (!Find.CameraDriver.CurrentViewRect.ExpandedBy(10).Contains(Pawn.Position))
        {
            return;
        }

        SqueakActionPlan plan = actionPlans[(int)action];
        if (!plan.Configured) return;

        TryTrigger(plan, new SqueakTriggerInvocation(origin, source));
    }

    private SqueakTriggerInvocation PeriodicInvocationFor(SqueakAction action)
    {
        // Work is sampled during CompTick like the other periodic actions, but its source is the
        // player's active forced work order when applicable. This lets ActiveCommand narrow only
        // that source while AnyOccurrence retains autonomous work occurrences.
        SqueakInvocationSource source = action == SqueakAction.Work && IsCurrentJobPlayerCommand()
            ? SqueakInvocationSource.ActiveCommand
            : SqueakInvocationSource.Periodic;
        return new SqueakTriggerInvocation(SqueakTriggerOrigin.Periodic, source);
    }

    private void SynchronizePeriodicMembership()
    {
        Map? map = Pawn.Spawned ? Pawn.MapHeld : null;
        if (ReferenceEquals(map, registeredMap)) return;
        SqueakPeriodicPopulation.Unregister(this, registeredMap);
        registeredMap = map;
        SqueakPeriodicPopulation.Register(this, registeredMap);
    }

    private void TryTrigger(SqueakActionPlan plan, SqueakTriggerInvocation invocation)
    {
        SqueakAction action = plan.Definition.Action;
        SqueakActionStatistics.Enter(Pawn, action);
        // This is deliberately before clock/RNG/context/resolver/vocal/playback work. A global disable is
        // production silence, while settings preview remains an explicit non-production path.
        SqueakActionScope scope = SqueakGlobalActionPolicy.Current.GetScope(action);
        if (scope == SqueakActionScope.Disabled) { SqueakActionStatistics.ScopeRejected(Pawn, action); return; }
        if (scope == SqueakActionScope.ActiveCommand && !invocation.IsActiveCommand) { SqueakActionStatistics.ScopeRejected(Pawn, action); return; }
        int now = 0;
        float nowRealtime = 0f;
        bool hasAttemptRealtime = false;
        bool hasAttemptTick = false;
        try
        {
            nowRealtime = Time.realtimeSinceStartup;
            hasAttemptRealtime = true;
            ResolvedSqueakContext context = GetRuntimeContext(out SqueakRuntimeSnapshot snapshot);
            RuntimeActionDelta actionDelta = context.GetAction(action);
            if (!actionDelta.Enabled)
            {
                SqueakActionStatistics.Disabled(Pawn, action);
                RecordOutcome(SqueakTriggerOutcome.Disabled, action, invocation.IsExternal, false, null, SqueakSoundSource.None, nowRealtime);
                return;
            }
            if (actionDelta.Scope == SqueakActionScope.ActiveCommand && !invocation.IsActiveCommand) { SqueakActionStatistics.ScopeRejected(Pawn, action); return; }

            now = Find.TickManager.TicksGame;
            hasAttemptTick = true;
            // The first Periodic production caller refreshes one shared snapshot for this tick,
            // including when player-facing scaling is off: startup materialization still uses the
            // same bounded shared maintenance path.
            SqueakPeriodicPopulation.Snapshot periodicPopulation = invocation.Origin == SqueakTriggerOrigin.Periodic
                ? SqueakPeriodicPopulation.Maintain(activeDistanceRange) : SqueakPeriodicPopulation.GetSnapshot();
            float periodicScale = invocation.Origin == SqueakTriggerOrigin.Periodic && ScalePeriodicWithAudiblePopulation
                ? SanitizePeriodicPopulationScale(periodicPopulation.Scale) : 1f;
            SqueakTimingEvaluation timing = EvaluateTiming(plan, context, actionDelta, now, nowRealtime,
                Find.TickManager.TickRateMultiplier, periodicScale);
            if (invocation.Origin == SqueakTriggerOrigin.Periodic && IsPeriodicStartupPending(action, timing, now))
            {
                RecordOutcome(SqueakTriggerOutcome.PeriodicStartupPending, action, false, false, null, SqueakSoundSource.None, nowRealtime);
                return;
            }

            if (!invocation.SkipsRandomOneShotProbability && plan.Mode == SqueakTriggerMode.RandomOneShot)
            {
                float probability = Mathf.Clamp01(plan.ProbabilityPerCheck * actionDelta.ProbabilityMultiplier) / periodicScale;
                bool passed = Rand.Value < probability;
                SqueakActionStatistics.Probability(Pawn, action, probability, passed);
                if (!passed)
                {
                    RecordOutcome(SqueakTriggerOutcome.ProbabilityRejected, action, false, false, null, SqueakSoundSource.None, nowRealtime);
                    return;
                }
            }

            if (!timing.ActionReady)
            {
                RecordOutcome(SqueakTriggerOutcome.ActionCooldown, action, invocation.IsExternal, false, null, SqueakSoundSource.None, nowRealtime);
                return;
            }

            if (timing.GlobalApplicable && !timing.GlobalReady)
            {
                RecordOutcome(SqueakTriggerOutcome.GlobalCooldown, action, invocation.IsExternal, false, null, SqueakSoundSource.None, nowRealtime);
                return;
            }

            SqueakVocalCapability capability = SampleVocalCapability();
            bool applyTalkingGate = ScaleFrequencyWithTalking && plan.Definition.VocalGatePolicy == SqueakVocalGatePolicy.ApplyTalkingGate;
            float roll = capability.RequiresTalkingRoll(applyTalkingGate) ? Rand.Value : 0f;
            SqueakVocalGateDecision vocalDecision = capability.Decide(applyTalkingGate, roll);
            if (vocalDecision != SqueakVocalGateDecision.Allowed)
            {
                ConsumeAttemptCooldowns(action, now, nowRealtime);
                RecordOutcome(vocalDecision == SqueakVocalGateDecision.VocalOrgansSilent
                    ? SqueakTriggerOutcome.VocalOrgansSilent : SqueakTriggerOutcome.TalkingRejected,
                    action, invocation.IsExternal, true, null, SqueakSoundSource.None, nowRealtime);
                return;
            }

            SqueakPlaybackAttempt attempt = PlayOneShot(action, CurrentMood, context, snapshot);
            ConsumeAttemptCooldowns(action, now, nowRealtime);
            SqueakTriggerOutcome outcome = attempt.Result switch
            {
                SqueakPlaybackAttemptResult.NoEligibleSound => SqueakTriggerOutcome.NoSoundFallback,
                SqueakPlaybackAttemptResult.EligibilityRejected => SqueakTriggerOutcome.EligibilityRejected,
                SqueakPlaybackAttemptResult.Dispatched => SqueakTriggerOutcome.Dispatched,
                _ => SqueakTriggerOutcome.PlaybackFailed,
            };
            RecordOutcome(outcome, action, invocation.IsExternal, true, attempt.Choice.Sound, attempt.Choice.Source, nowRealtime);
        }
        catch (Exception ex)
        {
            SqueakLog.TriggerAttemptFailed(action.ToString(), ex);
            try
            {
                if (!hasAttemptRealtime)
                {
                    nowRealtime = Time.realtimeSinceStartup;
                }
                if (!hasAttemptTick) now = Find.TickManager.TicksGame;
                ConsumeAttemptCooldowns(action, now, nowRealtime);
                RecordOutcome(SqueakTriggerOutcome.PlaybackFailed, action, invocation.IsExternal, true, null, SqueakSoundSource.None, nowRealtime);
            }
            catch { }
        }
    }

    private SqueakTimingEvaluation EvaluateTiming(SqueakActionPlan plan, ResolvedSqueakContext context,
        RuntimeActionDelta actionDelta, int nowTick, float nowRealtime, float timeSpeedMultiplier, float periodicScale = 1f)
    {
        return SqueakTimingModel.Evaluate(new SqueakTimingInput(nowTick, nowRealtime, timeSpeedMultiplier, plan, context,
            actionDelta, lastTriggerTick[(int)plan.Definition.Action], lastTriggerRealTime[(int)plan.Definition.Action], lastAnyTriggerTick,
            Props.globalMinIntervalTicks, GlobalCooldownMultiplier, ScaleCooldownWithTimeSpeed, periodicScale));
    }

    private bool IsPeriodicStartupPending(SqueakAction action, SqueakTimingEvaluation timing, int now)
    {
        if (!startupAnchorRecorded) return false;
        if (!periodicStartupPhaseMaterialized[(int)action]) MaterializePeriodicStartupPhase(action, timing, Find.TickManager.TickRateMultiplier);
        return now < periodicStartupReadyTicks[(int)action];
    }

    private void MaterializePeriodicStartupPhase(SqueakAction action, SqueakTimingEvaluation timing, float tickRateMultiplier)
    {
        int index = (int)action;
        if (periodicStartupPhaseMaterialized[index] || !startupAnchorRecorded) return;
        periodicStartupPhaseMaterialized[index] = true;
        periodicStartupReadyTicks[index] = CalculatePeriodicStartupReadyTick(action, timing, tickRateMultiplier);
    }

    private int CalculatePeriodicStartupReadyTick(SqueakAction action, SqueakTimingEvaluation timing, float tickRateMultiplier)
    {
        if (!startupAnchorRecorded) return int.MinValue;
        int actionTicks = timing.ActionIntervalTicks.GetValueOrDefault();
        if (timing.ActionIntervalSeconds.HasValue)
        {
            actionTicks = Mathf.CeilToInt(timing.ActionIntervalSeconds.Value * 60f * SafeTickRateMultiplier(tickRateMultiplier));
        }
        int governingTicks = timing.GlobalApplicable ? Math.Max(actionTicks, timing.GlobalCooldownTicks) : actionTicks;
        return governingTicks > 0
            ? startupAnchorTick + 1 + (int)(StablePawnActionPhase(Pawn.ThingID, action) % (uint)governingTicks)
            : startupAnchorTick;
    }

    private static float SafeTickRateMultiplier(float value) => float.IsNaN(value) || float.IsInfinity(value) || value <= 0f ? 1f : value;
    private static float SanitizePeriodicPopulationScale(float value) => float.IsNaN(value) || float.IsInfinity(value) || value < 1f ? 1f : value;

    private static uint StablePawnActionPhase(string identity, SqueakAction action)
    {
        unchecked
        {
            uint hash = 2166136261u;
            foreach (char c in identity ?? string.Empty) { hash ^= c; hash *= 16777619u; }
            hash ^= (uint)action; hash *= 16777619u;
            return hash;
        }
    }

    /// <summary>三层合并取心情调制:ModSettings.override > CompProperties.default > 内置默认。</summary>
    private SqueakMoodMod ResolveMoodMod(SqueakMood mood, ResolvedSqueakContext context)
    {
        SqueakMoodMod mod = new() { mood = mood };
        if (moodModMap.TryGetValue(mood, out SqueakMoodMod? def))
        {
            mod = def.Clone();
        }

        Dictionary<SqueakMood, SqueakMoodMod>? ov = SqueakyRatkinMod.Settings?.moodOverrides;
        if (ov != null && ov.TryGetValue(mood, out SqueakMoodMod? ovm) && ovm != null)
        {
            mod = ovm.Clone();
        }

        RuntimeMoodDelta? delta = context.GetMoodDelta(mood);
        if (delta != null)
        {
            if (delta.HasPitchFactor) mod.pitchFactor = delta.PitchFactor;
            if (delta.HasVolumeFactor) mod.volumeFactor = delta.VolumeFactor;
            if (delta.HasPitchJitter) mod.pitchJitter = delta.PitchJitter;
        }

        return mod;
    }

    private SqueakPlaybackAttempt PlayOneShot(SqueakAction action, SqueakMood mood, ResolvedSqueakContext context, SqueakRuntimeSnapshot snapshot)
    {
        SqueakSoundChoice choice = SqueakSoundChoice.None;
        SoundDef? def = null;
        try
        {
            choice = snapshot.ChooseProductionSound(context, action, Pawn);
            def = choice.Sound;
        if (def == null)
        {
            if (MissingSoundWarnings.Add(action))
            {
                SqueakLog.AudioNoSound(action.ToString());
            }
            return new SqueakPlaybackAttempt(SqueakPlaybackAttemptResult.NoEligibleSound, choice);
        }

            SqueakMoodMod mod = ResolveMoodMod(mood, context);
            if (!SqueakSoundAvailabilityCache.TryCreateProductionInfo(def, Pawn, out SoundInfo info, out _))
            {
                return new SqueakPlaybackAttempt(SqueakPlaybackAttemptResult.EligibilityRejected, choice);
            }
            info.pitchFactor = mod.pitchFactor * mod.pitchJitter.RandomInRange;
            info.volumeFactor = mod.volumeFactor;
            def.PlayOneShot(info);
            SqueakDebug.NotifySqueak(Pawn, action, mood, choice);
            return new SqueakPlaybackAttempt(SqueakPlaybackAttemptResult.Dispatched, choice);
        }
        catch (Exception ex)
        {
            string soundKey = def?.defName ?? action.ToString();
            SqueakLog.AudioDispatchFailed(action.ToString(), soundKey, ex);
            return new SqueakPlaybackAttempt(SqueakPlaybackAttemptResult.Exception, choice);
        }
    }

    /// <summary>
    /// Resolves and plays a production-equivalent final one-shot without touching trigger gates, cooldowns, diagnostics,
    /// motes, or persistent state. Random state is restored even when resolver selection needs a random draw.
    /// </summary>
    public SqueakFinalPreviewResult PreviewFinal(SqueakAction action, SqueakMood? moodOverride = null) => PreviewFinal(action, moodOverride, SqueakSettingsGameContext.Capture());

    /// <summary>Settings preview boundary: callers may pass their once-per-frame context so no menu UI path reaches map services.</summary>
    public SqueakFinalPreviewResult PreviewFinal(SqueakAction action, SqueakMood? moodOverride, SqueakSettingsGameContext gameContext)
    {
        SqueakSoundChoice choice = SqueakSoundChoice.None;
        SqueakMood mood = SqueakMood.Neutral;
        Rand.PushState();
        try
        {
            SqueakRuntimeResolver.FlushPendingRuntimeChanges(true);
            if (!gameContext.IsPawnOnCurrentMap(Pawn) || Pawn.Dead)
            {
                return new SqueakFinalPreviewResult(Pawn, null, mood, choice, SqueakFinalPreviewStatus.PawnOrMapUnavailable, SqueakSoundPlayability.MapRequired, "pawn_or_map_unavailable");
            }

            mood = moodOverride ?? CurrentMood;
            SqueakRuntimeSnapshot snapshot = SqueakRuntimeResolver.Current;
            ResolvedSqueakContext context = snapshot.ResolveContext(Pawn);
            choice = snapshot.ChooseProductionSound(context, action, Pawn);
            if (choice.IsNone)
            {
                return new SqueakFinalPreviewResult(Pawn, context.Xenotype, mood, choice, SqueakFinalPreviewStatus.NoEligibleSound, SqueakSoundPlayability.NoAudio, "resolver_no_eligible_sound");
            }
            SqueakSoundPlayability playability = SqueakSoundAvailabilityCache.GetProductionPlayability(choice.Sound, Pawn);
            if (playability != SqueakSoundPlayability.Playable)
            {
                return new SqueakFinalPreviewResult(Pawn, context.Xenotype, mood, choice, SqueakFinalPreviewStatus.IneligibleSound, playability, "sound_not_playable_" + playability);
            }

            SqueakMoodMod mod = ResolveMoodMod(mood, context);
            if (!SqueakSoundAvailabilityCache.TryCreateProductionInfo(choice.Sound, Pawn, out SoundInfo info, out playability))
            {
                return new SqueakFinalPreviewResult(Pawn, context.Xenotype, mood, choice, SqueakFinalPreviewStatus.IneligibleSound, playability, "sound_info_failed_" + playability);
            }
            info.pitchFactor = mod.pitchFactor * mod.pitchJitter.RandomInRange;
            info.volumeFactor = mod.volumeFactor;
            choice.Sound!.PlayOneShot(info);
            return new SqueakFinalPreviewResult(Pawn, context.Xenotype, mood, choice, SqueakFinalPreviewStatus.Dispatched, playability, "dispatched");
        }
        catch { return new SqueakFinalPreviewResult(Pawn, null, mood, choice, SqueakFinalPreviewStatus.Exception, SqueakSoundPlayability.Failed, "exception"); }
        finally { Rand.PopState(); }
    }

    private ResolvedSqueakContext GetRuntimeContext(out SqueakRuntimeSnapshot snapshot)
    {
        snapshot = SqueakRuntimeResolver.Current;
        XenotypeDef? xenotype = null;
        if (ModsConfig.BiotechActive)
        {
            xenotype = Pawn.genes?.Xenotype;
        }

        if (!ReferenceEquals(snapshot, cachedRuntimeSnapshot) || !ReferenceEquals(xenotype, cachedXenotype))
        {
            cachedRuntimeSnapshot = snapshot;
            cachedXenotype = xenotype;
            cachedSqueakContext = snapshot.ResolveContext(Pawn);
        }

        return cachedSqueakContext;
    }

    private void ConsumeAttemptCooldowns(SqueakAction action, int nowTick, float nowRealtime)
    {
        lastTriggerTick[(int)action] = nowTick;
        lastTriggerRealTime[(int)action] = nowRealtime;
        lastAnyTriggerTick = nowTick;
    }

    private void RecordOutcome(SqueakTriggerOutcome outcome, SqueakAction action, bool external, bool cooldownConsumed,
        SoundDef? sound, SqueakSoundSource source, float nowRealtime)
    {
        SqueakActionStatistics.Outcome(Pawn, action, outcome);
        if (DiagnosticsEnabled)
        {
            SqueakRecentOutcome evaluation = new(outcome, action, Find.TickManager.TicksGame,
                nowRealtime, cooldownConsumed, sound, source);
            lastEvaluation = evaluation;
            if (external || IsSignificantOutcome(outcome))
            {
                lastSignificantOutcome = evaluation;
            }
        }
    }

    /// <summary>Clears runtime-only diagnostic history when a diagnostics manager changes sessions.</summary>
    public void ResetDiagnosticState()
    {
        lastEvaluation = null;
        lastSignificantOutcome = null;
    }

    private static bool IsSignificantOutcome(SqueakTriggerOutcome outcome) => outcome != SqueakTriggerOutcome.ProbabilityRejected
        && outcome != SqueakTriggerOutcome.ActionCooldown && outcome != SqueakTriggerOutcome.GlobalCooldown
        && outcome != SqueakTriggerOutcome.PeriodicStartupPending;

    /// <summary>Samples diagnostic state without selecting audio, consuming Rand, changing timestamps, or touching the production context cache.</summary>
    internal SqueakDiagnosticSnapshot GetDiagnosticSnapshot()
    {
        SqueakAction? action = CurrentAction;
        int nowTick = Find.TickManager.TicksGame;
        float nowRealtime = Time.realtimeSinceStartup;
        float timeSpeed = Find.TickManager.TickRateMultiplier;
        ResolvedSqueakContext context = SqueakRuntimeResolver.Current.ResolveContext(Pawn);
        if (action == null)
        {
            return new SqueakDiagnosticSnapshot(action, false, null, null, 1f, default, default, SqueakPeriodicPopulation.GetSnapshot(), GlobalCooldownMultiplier,
                context.Xenotype, context.OverallIntervalMultiplier, timeSpeed, 0f, 0f, false, false, SampleVocalCapability(), false, false,
                lastEvaluation, lastSignificantOutcome);
        }

        SqueakActionPlan plan = actionPlans[(int)action.Value];
        if (!plan.Configured)
        {
            return new SqueakDiagnosticSnapshot(action, false, null, null, 1f, default, default, SqueakPeriodicPopulation.GetSnapshot(), GlobalCooldownMultiplier,
                context.Xenotype, context.OverallIntervalMultiplier, timeSpeed, 0f, 0f, false, false, SampleVocalCapability(), false, false,
                lastEvaluation, lastSignificantOutcome);
        }

        RuntimeActionDelta delta = context.GetAction(action.Value);
        SqueakTriggerInvocation invocation = PeriodicInvocationFor(action.Value);
        bool actionEnabled = IsScopeEligible(action.Value, delta, invocation);
        SqueakPeriodicPopulation.Snapshot population = SqueakPeriodicPopulation.GetSnapshot();
        float periodicScale = ScalePeriodicWithAudiblePopulation && (plan.Mode == SqueakTriggerMode.EachTime || plan.Mode == SqueakTriggerMode.RandomOneShot)
            ? SanitizePeriodicPopulationScale(population.Scale) : 1f;
        SqueakTimingEvaluation timing = EvaluateTiming(plan, context, delta, nowTick, nowRealtime, timeSpeed, periodicScale);
        SqueakTimingEvaluation baseTiming = EvaluateTiming(plan, context, delta, nowTick, nowRealtime, timeSpeed, 1f);
        float baseProbability = plan.Mode == SqueakTriggerMode.RandomOneShot
            ? Mathf.Clamp01(plan.ProbabilityPerCheck * delta.ProbabilityMultiplier) : 1f;
        float probability = baseProbability / periodicScale;
        bool isPeriodic = plan.Mode == SqueakTriggerMode.EachTime || plan.Mode == SqueakTriggerMode.RandomOneShot;
        int startupReadyTick = periodicStartupPhaseMaterialized[(int)action.Value]
            ? periodicStartupReadyTicks[(int)action.Value]
            : CalculatePeriodicStartupReadyTick(action.Value, timing, timeSpeed);
        bool startupPending = isPeriodic && startupAnchorRecorded && nowTick < startupReadyTick;
        return new SqueakDiagnosticSnapshot(action, actionEnabled, plan.Mode, plan.CooldownClock, delta.IntervalMultiplier,
            timing, baseTiming, population, GlobalCooldownMultiplier, context.Xenotype, context.OverallIntervalMultiplier, timeSpeed, probability, baseProbability, startupPending, timing.TimingReady && !startupPending,
            SampleVocalCapability(), ScaleFrequencyWithTalking && plan.Definition.VocalGatePolicy == SqueakVocalGatePolicy.ApplyTalkingGate,
            plan.Definition.VocalGatePolicy == SqueakVocalGatePolicy.ExemptTalkingGate, lastEvaluation, lastSignificantOutcome);
    }

    /// <summary>Mirrors production's global and runtime scope gates without resolving audio or mutating trigger state.</summary>
    private static bool IsScopeEligible(SqueakAction action, RuntimeActionDelta delta, SqueakTriggerInvocation invocation)
    {
        SqueakActionScope globalScope = SqueakGlobalActionPolicy.Current.GetScope(action);
        if (globalScope == SqueakActionScope.Disabled || (globalScope == SqueakActionScope.ActiveCommand && !invocation.IsActiveCommand)) return false;
        return delta.Enabled && (delta.Scope != SqueakActionScope.ActiveCommand || invocation.IsActiveCommand);
    }

    private SqueakVocalCapability SampleVocalCapability() => new(GetVocalOrganEfficiency(),
        Pawn.health?.capacities?.GetLevel(PawnCapacityDefOf.Talking) ?? 1f);

    private float GetVocalOrganEfficiency()
    {
        if (Pawn.health?.hediffSet == null)
        {
            return 1f;
        }

        float source = PawnCapacityUtility.CalculateTagEfficiency(Pawn.health.hediffSet, BodyPartTagDefOf.TalkingSource);
        float pathway = PawnCapacityUtility.CalculateTagEfficiency(Pawn.health.hediffSet, BodyPartTagDefOf.TalkingPathway, 1f);
        float tongue = GetTagEfficiencyOrOneIfMissing(BodyPartTagDefOf.Tongue, 1f);
        return source * pathway * tongue;
    }

    private float GetTagEfficiencyOrOneIfMissing(BodyPartTagDef tag, float maxEfficiency)
    {
        return BodyHasPartTag(tag)
            ? PawnCapacityUtility.CalculateTagEfficiency(Pawn.health!.hediffSet, tag, maxEfficiency)
            : 1f;
    }

    private bool BodyHasPartTag(BodyPartTagDef tag)
    {
        List<BodyPartRecord>? parts = Pawn.RaceProps.body?.AllParts;
        if (parts == null)
        {
            return false;
        }

        foreach (BodyPartRecord part in parts)
        {
            if (part.def.tags != null && part.def.tags.Contains(tag))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>Pure cooldown formula shared by production gates and later diagnostics.</summary>
    internal static class SqueakTimingModel
    {
        private static SqueakTimingEvaluation Evaluate(SqueakTimingInput input, int actionTicks, float actionSeconds, int globalTicks)
        {
            bool realtime = input.Plan.CooldownClock == SqueakCooldownClock.Realtime;
            float actionRemainingSeconds = realtime
                ? Math.Max(0f, actionSeconds - (input.NowRealtime - input.LastActionRealtime))
                : 0f;
            int actionRemainingTicks = realtime
                ? 0
                : RemainingTicks(actionTicks, input.NowTick, input.LastActionTick);
            int globalRemainingTicks = RemainingTicks(globalTicks, input.NowTick, input.LastGlobalTick);
            bool globalApplicable = !input.Plan.IgnoreGlobalCooldown;
            return new SqueakTimingEvaluation(realtime ? null : actionTicks, realtime ? actionSeconds : null, globalTicks,
                realtime ? null : actionRemainingTicks, realtime ? actionRemainingSeconds : null, globalRemainingTicks,
                realtime ? actionRemainingSeconds <= 0f : actionRemainingTicks == 0, globalApplicable,
                globalRemainingTicks == 0);
        }

        public static SqueakTimingEvaluation Evaluate(SqueakTimingInput input)
        {
            int configuredActionTicks = GetConfiguredActionTicks(input.Plan.MinIntervalTicks, input.MasterMultiplier,
                input.Context.OverallIntervalMultiplier, input.ActionDelta.IntervalMultiplier);
            float actionSeconds = input.Plan.CooldownClock == SqueakCooldownClock.Realtime
                ? configuredActionTicks / 60f
                : 0f;
            int actionTicks = input.Plan.CooldownClock == SqueakCooldownClock.Realtime
                ? 0
                : ApplyGameTickTimeSpeed(configuredActionTicks, input.ScaleWithTimeSpeed, input.TimeSpeedMultiplier);
            float populationScale = SanitizePopulationScale(input.PeriodicPopulationScale);
            actionTicks = ScaleTicks(actionTicks, populationScale);
            if (input.Plan.CooldownClock == SqueakCooldownClock.Realtime) actionSeconds *= populationScale;
            int globalTicks = ScaleTicks(GetGlobalCooldownTicks(input.GlobalBaseTicks, input.MasterMultiplier,
                input.ScaleWithTimeSpeed, input.TimeSpeedMultiplier), populationScale);
            return Evaluate(input, actionTicks, actionSeconds, globalTicks);
        }

        public static int GetActionIntervalTicks(
            int baseTicks,
            float masterMultiplier,
            float overallMultiplier,
            float actionMultiplier,
            bool scaleWithTimeSpeed,
            float timeSpeedMultiplier)
        {
            int configuredTicks = GetConfiguredActionTicks(baseTicks, masterMultiplier, overallMultiplier, actionMultiplier);
            return ApplyGameTickTimeSpeed(configuredTicks, scaleWithTimeSpeed, timeSpeedMultiplier);
        }

        public static float GetActionIntervalSeconds(
            int baseTicks,
            float masterMultiplier,
            float overallMultiplier,
            float actionMultiplier)
        {
            return GetConfiguredActionTicks(baseTicks, masterMultiplier, overallMultiplier, actionMultiplier) / 60f;
        }

        public static int GetGlobalCooldownTicks(
            int baseTicks,
            float masterMultiplier,
            bool scaleWithTimeSpeed,
            float timeSpeedMultiplier)
        {
            int configuredTicks = CeilingToTicks(MultiplyGlobalInterval(baseTicks, masterMultiplier));
            return ApplyGameTickTimeSpeed(configuredTicks, scaleWithTimeSpeed, timeSpeedMultiplier);
        }

        private static int GetConfiguredActionTicks(int baseTicks, float masterMultiplier, float overallMultiplier, float actionMultiplier)
        {
            float overall = SanitizeIntervalMultiplier(overallMultiplier);
            float action = SanitizeIntervalMultiplier(actionMultiplier);
            if (baseTicks <= 0 || overall == 0f || action == 0f)
            {
                return 0;
            }

            // Preserve the configured timing stages exactly: Xenotype scaling rounds first,
            // then the global master multiplier rounds that result.
            int legacyActionTicks = CeilingToTicks(baseTicks * (double)overall * action);
            float master = SanitizeIntervalMultiplier(masterMultiplier);
            return legacyActionTicks == 0 || master == 0f
                ? 0
                : CeilingToTicks(legacyActionTicks * (double)master);
        }

        private static double MultiplyGlobalInterval(int baseTicks, float masterMultiplier)
        {
            float master = SanitizeIntervalMultiplier(masterMultiplier);
            if (baseTicks <= 0 || master == 0f)
            {
                return 0d;
            }

            double value = baseTicks * (double)master;
            return double.IsInfinity(value) || value >= int.MaxValue ? int.MaxValue : value;
        }

        private static int ApplyGameTickTimeSpeed(int configuredTicks, bool scaleWithTimeSpeed, float timeSpeedMultiplier)
        {
            if (configuredTicks <= 0)
            {
                return 0;
            }

            double cooldown = configuredTicks;
            if (scaleWithTimeSpeed)
            {
                cooldown *= SanitizeTimeSpeedMultiplier(timeSpeedMultiplier);
            }

            return CeilingToTicks(cooldown);
        }

        private static int CeilingToTicks(double value) => value <= 0d
            ? 0
            : double.IsInfinity(value) || value >= int.MaxValue ? int.MaxValue : (int)Math.Ceiling(value);

        private static float SanitizeIntervalMultiplier(float value) => float.IsNaN(value) || float.IsInfinity(value) ? 1f : Math.Max(0f, value);

        private static float SanitizeTimeSpeedMultiplier(float value) => float.IsNaN(value) || float.IsInfinity(value) ? 1f : Math.Max(1f, value);
        private static float SanitizePopulationScale(float value) => float.IsNaN(value) || float.IsInfinity(value) ? 1f : Math.Max(1f, value);
        private static int ScaleTicks(int ticks, float scale) => ticks <= 0 ? 0 : CeilingToTicks(ticks * (double)scale);

        private static int RemainingTicks(int interval, int now, int last)
        {
            if (interval <= 0 || (long)now - last >= interval)
            {
                return 0;
            }

            long remaining = (long)interval - ((long)now - last);
            return remaining >= int.MaxValue ? int.MaxValue : (int)Math.Max(0L, remaining);
        }

    }

    private static void EnsureSoundCache()
    {
        if (soundCacheInitialized)
        {
            return;
        }

        foreach (SqueakAction a in Enum.GetValues(typeof(SqueakAction)))
        {
            SoundCacheMixed[a] = DefDatabase<SoundDef>.GetNamedSilentFail(SqueakActionDefinitions.Get(a).AudioKey);
        }

        soundCacheInitialized = true;
    }

    public static void ApplyDistanceRange(FloatRange range)
    {
        activeDistanceRange = range;
        SqueakPeriodicPopulation.NotifyDistanceChanged();
        EnsureSoundCache();
        HashSet<SoundDef> defs = new(SqueakRuntimeResolver.Current.KnownMapSoundDefs);
        foreach (SoundDef? def in SoundCacheMixed.Values)
        {
            if (def != null) defs.Add(def);
        }

        foreach (SoundDef def in defs)
        {
            ApplyDistanceRange(def, activeDistanceRange);
        }
    }

    /// <summary>Explicit overlay/diagnostic maintenance; snapshot readers remain side-effect free.</summary>
    internal static void MaintainPeriodicPopulationDiagnostics() => SqueakPeriodicPopulation.Maintain(activeDistanceRange);

    private static void ApplyDistanceRange(SoundDef? def, FloatRange range)
    {
        if (def?.subSounds == null)
        {
            return;
        }

        foreach (SubSoundDef subSound in def.subSounds)
        {
            if (!subSound.onCamera)
            {
                subSound.distRange = range;
            }
        }
    }

    private bool IsEating() => Pawn.CurJob?.def == JobDefOf.Ingest;
    private bool IsSleeping() => Pawn.GetPosture() == PawnPosture.LayingInBed && Pawn.needs?.rest != null;
    private bool IsMoving() => Pawn.pather != null && Pawn.pather.Moving;
    private bool IsJoyJob() => Pawn.CurJob?.def?.joyKind != null;
    private bool IsWorking() => Pawn.CurJob?.workGiverDef != null;
    private bool IsCurrentJobPlayerCommand() => Pawn.CurJob?.playerForced == true;
    private bool IsCurrentEquipJobPlayerCommand() => Pawn.CurJob?.playerForced == true && Pawn.CurJob.def == JobDefOf.Equip;

    private bool IsSocializing()
    {
        string? d = Pawn.CurJob?.def?.defName;
        if (d == null)
        {
            return false;
        }

        foreach (string marker in SocialJobMarkers)
            if (d.IndexOf(marker, StringComparison.OrdinalIgnoreCase) >= 0) return true;
        return false;
    }

}

public class CompProperties_Squeaker : CompProperties
{
    // XML is authoritative. This only aligns a missing-XML fallback with the shipped 216-tick baseline;
    // it is not a migration of the historical C# 120-tick fallback.
    public int globalMinIntervalTicks = 216;
    public bool scaleFrequencyWithTalking = true;
    public List<SqueakActionConfig> actions = new();
    public List<SqueakMoodMod> moodMods = new();
    public List<SqueakDistancePresetConfig> distancePresets = new();

    public CompProperties_Squeaker()
    {
        compClass = typeof(CompSqueaker);
    }
}
