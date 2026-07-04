using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace SqueakyRatkin;

public enum SqueakMood { Good, Neutral, Bad, Break }
public enum SqueakAction { Call, Eat, Sleep, Wounded, Select, Move, Social, Joy, Death }
public enum SqueakCooldownClock { GameTicks, Realtime }

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

/// <summary>
/// 挂在 Ratkin pawn 上的自驱动发声组件。
/// 配置三层:CompProperties(XML默认) ← ModSettings(玩家override) ← 运行时。
/// 心情靠运行时 pitchFactor/volumeFactor 调制,每动作只需 1 个 SoundDef + 1 套中性音频。
/// </summary>
public class CompSqueaker : ThingComp
{
    private const string RatkinDefName = "Ratkin";

    /// <summary>完全 override:ON=纯自定义(SR_*_Pure),OFF=混合(SR_*)。由 ModSettings 同步。</summary>
    public static bool UseCustomOnly = false;
    public static bool ScaleCooldownWithTimeSpeed = true;
    public static float GlobalCooldownMultiplier = 1f;

    private static readonly Dictionary<SqueakAction, SoundDef?> SoundCacheMixed = new();
    private static readonly Dictionary<SqueakAction, SoundDef?> SoundCachePure = new();
    private static bool soundCacheInitialized;
    private static FloatRange activeDistanceRange = new(12f, 40f);

    private readonly Dictionary<SqueakAction, SqueakActionConfig> configMap = new();
    private readonly Dictionary<SqueakAction, int> lastTriggerTick = new();
    private readonly Dictionary<SqueakAction, float> lastTriggerRealTime = new();
    private readonly Dictionary<SqueakMood, SqueakMoodMod> moodModMap = new();
    private int lastAnyTriggerTick = int.MinValue / 2;

    private Pawn Pawn => (Pawn)parent;
    private CompProperties_Squeaker Props => (CompProperties_Squeaker)props;

    public override void Initialize(CompProperties props)
    {
        base.Initialize(props);
        lastAnyTriggerTick = int.MinValue / 2;
        foreach (SqueakActionConfig cfg in Props.actions)
        {
            configMap[cfg.action] = cfg;
            if (!lastTriggerTick.ContainsKey(cfg.action))
            {
                lastTriggerTick[cfg.action] = int.MinValue / 2;
            }

            if (!lastTriggerRealTime.ContainsKey(cfg.action))
            {
                lastTriggerRealTime[cfg.action] = -1_000_000f;
            }
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

            return SqueakAction.Call;
        }
    }

    public override void CompTick()
    {
        if (!Pawn.Spawned || Pawn.MapHeld == null || Pawn.MapHeld != Find.CurrentMap)
        {
            return;
        }

        if (!Find.CameraDriver.CurrentViewRect.ExpandedBy(10).Contains(Pawn.Position))
        {
            return;
        }

        SqueakAction? action = CurrentAction;
        if (action == null || !configMap.TryGetValue(action.Value, out SqueakActionConfig? cfg))
        {
            return;
        }

        switch (cfg.mode)
        {
            case SqueakTriggerMode.EachTime:
                TryTrigger(action.Value, cfg);
                break;
            case SqueakTriggerMode.RandomOneShot:
                if (Rand.Value < cfg.probabilityPerCheck)
                {
                    TryTrigger(action.Value, cfg);
                }

                break;
            case SqueakTriggerMode.External:
            case SqueakTriggerMode.Sustained:
                break;
        }
    }

    public void Notify_Wounded() => NotifyExternal(SqueakAction.Wounded);
    public void Notify_Select() => NotifyExternal(SqueakAction.Select);
    public void Notify_Death() => NotifyExternal(SqueakAction.Death);

    private void NotifyExternal(SqueakAction action)
    {
        if (!Pawn.Spawned || Pawn.MapHeld != Find.CurrentMap)
        {
            return;
        }

        if (!Find.CameraDriver.CurrentViewRect.ExpandedBy(10).Contains(Pawn.Position))
        {
            return;
        }

        if (!configMap.TryGetValue(action, out SqueakActionConfig? cfg))
        {
            return;
        }

        TryTrigger(action, cfg);
    }

    private void TryTrigger(SqueakAction action, SqueakActionConfig cfg)
    {
        int now = Find.TickManager.TicksGame;
        if (!ActionCooldownElapsed(action, cfg, now))
        {
            return;
        }

        int globalCooldown = GetEffectiveCooldownTicks((int)Math.Ceiling(Props.globalMinIntervalTicks * Math.Max(0f, GlobalCooldownMultiplier)));
        if (!cfg.ignoreGlobalCooldown && now - lastAnyTriggerTick < globalCooldown)
        {
            return;
        }

        PlayOneShot(action, CurrentMood);
        lastTriggerTick[action] = now;
        lastTriggerRealTime[action] = Time.realtimeSinceStartup;
        lastAnyTriggerTick = now;
    }

    private bool ActionCooldownElapsed(SqueakAction action, SqueakActionConfig cfg, int now)
    {
        return cfg.cooldownClock switch
        {
            SqueakCooldownClock.Realtime => Time.realtimeSinceStartup - lastTriggerRealTime[action] >= Math.Max(0, cfg.minIntervalTicks) / 60f,
            _ => now - lastTriggerTick[action] >= GetEffectiveCooldownTicks(cfg.minIntervalTicks),
        };
    }

    /// <summary>三层合并取心情调制:ModSettings.override > CompProperties.default > 内置默认。</summary>
    private SqueakMoodMod ResolveMoodMod(SqueakMood mood)
    {
        SqueakMoodMod mod = new() { mood = mood };
        if (moodModMap.TryGetValue(mood, out SqueakMoodMod? def))
        {
            mod = def;
        }

        Dictionary<SqueakMood, SqueakMoodMod>? ov = SqueakyRatkinMod.Settings?.moodOverrides;
        if (ov != null && ov.TryGetValue(mood, out SqueakMoodMod? ovm) && ovm != null)
        {
            mod = ovm;
        }

        return mod;
    }

    private void PlayOneShot(SqueakAction action, SqueakMood mood)
    {
        SoundDef? def = Get(action);
        if (def == null)
        {
            return;
        }

        SqueakMoodMod mod = ResolveMoodMod(mood);
        SoundInfo info = SoundInfo.InMap(new TargetInfo(Pawn));
        info.pitchFactor = mod.pitchFactor * mod.pitchJitter.RandomInRange;
        info.volumeFactor = mod.volumeFactor;
        def.PlayOneShot(info);
        SqueakDebug.NotifySqueak(Pawn, action, mood, def);
    }

    private static int GetEffectiveCooldownTicks(int baseCooldownTicks)
    {
        int cooldown = Math.Max(0, baseCooldownTicks);
        if (!ScaleCooldownWithTimeSpeed || cooldown == 0)
        {
            return cooldown;
        }

        float multiplier = Math.Max(1f, Find.TickManager.TickRateMultiplier);
        return multiplier <= 1f ? cooldown : (int)Math.Ceiling(cooldown * multiplier);
    }

    private static void EnsureSoundCache()
    {
        if (soundCacheInitialized)
        {
            return;
        }

        foreach (SqueakAction a in Enum.GetValues(typeof(SqueakAction)))
        {
            SoundCacheMixed[a] = DefDatabase<SoundDef>.GetNamedSilentFail($"SR_{a}");
            SoundCachePure[a] = DefDatabase<SoundDef>.GetNamedSilentFail($"SR_{a}_Pure");
        }

        soundCacheInitialized = true;
    }

    private static SoundDef? Get(SqueakAction a)
    {
        EnsureSoundCache();
        return UseCustomOnly ? SoundCachePure[a] : SoundCacheMixed[a];
    }

    public static void ApplyDistanceRange(FloatRange range)
    {
        activeDistanceRange = range;
        EnsureSoundCache();
        foreach (SoundDef? def in SoundCacheMixed.Values)
        {
            ApplyDistanceRange(def, activeDistanceRange);
        }

        foreach (SoundDef? def in SoundCachePure.Values)
        {
            ApplyDistanceRange(def, activeDistanceRange);
        }
    }

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

    private bool IsSocializing()
    {
        string? d = Pawn.CurJob?.def?.defName;
        if (d == null)
        {
            return false;
        }

        return d.IndexOf("Chat", StringComparison.OrdinalIgnoreCase) >= 0
            || d.IndexOf("Social", StringComparison.OrdinalIgnoreCase) >= 0
            || d.IndexOf("Visit", StringComparison.OrdinalIgnoreCase) >= 0
            || d.IndexOf("Lovin", StringComparison.OrdinalIgnoreCase) >= 0
            || d.IndexOf("Entertain", StringComparison.OrdinalIgnoreCase) >= 0;
    }

    public static bool IsRatkin(Pawn? p) => p != null && p.def?.defName == RatkinDefName;
}

public class CompProperties_Squeaker : CompProperties
{
    public int globalMinIntervalTicks = 120;
    public List<SqueakActionConfig> actions = new();
    public List<SqueakMoodMod> moodMods = new();

    public CompProperties_Squeaker()
    {
        compClass = typeof(CompSqueaker);
    }
}
