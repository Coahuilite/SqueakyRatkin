using System;
using System.Globalization;
using System.Text;
using UnityEngine;
using Verse;

namespace SqueakyRatkin;

/// <summary>Ephemeral single-pawn funnel recorder; it has no Scribe, UI, or automatic logging.</summary>
public static class SqueakActionStatistics
{
    internal struct Counters { public long Entered, ScopeRejected, Disabled, StartupPending, Checks, Passed, Rejected, ActionCooldown, GlobalCooldown, VocalSilent, TalkingRejected, NoSound, Eligibility, PlaybackFailed, Dispatched; public double ExpectedProbability; }
    public readonly struct ActionSnapshot
    {
        public readonly SqueakAction Action; public readonly long Entered, ScopeRejected, Disabled, StartupPending, Checks, Passed, Rejected, ActionCooldown, GlobalCooldown, VocalSilent, TalkingRejected, NoSound, Eligibility, PlaybackFailed, Dispatched; public readonly double ExpectedProbability;
        internal ActionSnapshot(SqueakAction action, Counters c) { Action = action; Entered = c.Entered; ScopeRejected = c.ScopeRejected; Disabled = c.Disabled; StartupPending = c.StartupPending; Checks = c.Checks; Passed = c.Passed; Rejected = c.Rejected; ExpectedProbability = c.ExpectedProbability; ActionCooldown = c.ActionCooldown; GlobalCooldown = c.GlobalCooldown; VocalSilent = c.VocalSilent; TalkingRejected = c.TalkingRejected; NoSound = c.NoSound; Eligibility = c.Eligibility; PlaybackFailed = c.PlaybackFailed; Dispatched = c.Dispatched; }
    }
    public sealed class Snapshot
    {
        public readonly Pawn? Pawn; public readonly bool Running; public readonly string StopReason; public readonly int StartTick, EndTick, CaptureTick, LastAttemptTick; public readonly float StartRealtime, EndRealtime, CaptureRealtime, LastAttemptRealtime, CaptureTimeSpeed, LastAttemptTimeSpeed; public readonly long TimingSamples, PausedSamples; public readonly ActionSnapshot[] Actions;
        /// <summary>Compatibility alias for existing UI; it is the last attempt's sampled speed, not a session-end value.</summary>
        public float LastTimeSpeed => LastAttemptTimeSpeed;
        internal Snapshot(Pawn? pawn, bool running, string reason, int startTick, int endTick, int captureTick, int lastAttemptTick, float startRealtime, float endRealtime, float captureRealtime, float lastAttemptRealtime, float captureSpeed, float lastAttemptSpeed, long samples, long paused, Counters[] values)
        { Pawn = pawn; Running = running; StopReason = reason; StartTick = startTick; EndTick = endTick; CaptureTick = captureTick; LastAttemptTick = lastAttemptTick; StartRealtime = startRealtime; EndRealtime = endRealtime; CaptureRealtime = captureRealtime; LastAttemptRealtime = lastAttemptRealtime; CaptureTimeSpeed = captureSpeed; LastAttemptTimeSpeed = lastAttemptSpeed; TimingSamples = samples; PausedSamples = paused; Actions = new ActionSnapshot[SqueakActionDefinitions.Count]; for (int i = 0; i < Actions.Length; i++) Actions[i] = new ActionSnapshot((SqueakAction)i, values[i]); }
    }
    private static Pawn? pawn; private static Map? map; private static bool running; private static string reason = "not_started"; private static int startTick, endTick, lastAttemptTick; private static float startRealtime, endRealtime, lastAttemptRealtime, lastAttemptSpeed; private static long samples, pausedSamples; private static Counters[] counters = new Counters[SqueakActionDefinitions.Count];
    private static long revision;
    public static long Revision => revision;
    public static long SnapshotBuildCount { get; private set; }
    public static bool IsRunning { get { Validate(SqueakSettingsGameContext.Capture()); return running; } }
    public static Pawn? SelectedPawn { get { Validate(SqueakSettingsGameContext.Capture()); return pawn; } }
    public static bool StartSelectedPawn(Pawn? selected) => Start(selected);
    public static bool Start(Pawn? selected) => Start(selected, SqueakSettingsGameContext.Capture());
    public static bool Start(Pawn? selected, SqueakSettingsGameContext context)
    {
        if (!context.IsPawnOnCurrentMap(selected) || selected?.TryGetComp<CompSqueaker>() == null) return false;
        pawn = selected; map = context.Map; running = true; reason = "running"; startTick = context.Tick; endTick = startTick; lastAttemptTick = startTick; startRealtime = context.Realtime; endRealtime = startRealtime; lastAttemptRealtime = startRealtime; lastAttemptSpeed = context.TickRateMultiplier; samples = pausedSamples = 0; counters = new Counters[SqueakActionDefinitions.Count]; revision++; return true;
    }
    public static void Stop() => Stop("stopped", SqueakSettingsGameContext.Capture());
    public static void Stop(SqueakSettingsGameContext context) => Stop("stopped", context);
    public static void Reset() => Reset(SqueakSettingsGameContext.Capture());
    public static void Reset(SqueakSettingsGameContext context) { Validate(context); Pawn? current = pawn; if (running && current != null) { Start(current, context); return; } counters = new Counters[SqueakActionDefinitions.Count]; int tick = context.HasPlayableMapUI ? context.Tick : lastAttemptTick; startTick = endTick = lastAttemptTick = tick; startRealtime = endRealtime = lastAttemptRealtime = context.Realtime; lastAttemptSpeed = context.TickRateMultiplier; samples = pausedSamples = 0; revision++; }
    public static Snapshot GetSnapshot() => GetSnapshot(SqueakSettingsGameContext.Capture());
    public static Snapshot GetSnapshot(SqueakSettingsGameContext context) { Validate(context); int captureTick = context.HasPlayableMapUI ? context.Tick : lastAttemptTick; float captureRealtime = context.Realtime, captureSpeed = context.TickRateMultiplier; SnapshotBuildCount++; return new Snapshot(pawn, running, reason, startTick, endTick, captureTick, lastAttemptTick, startRealtime, endRealtime, captureRealtime, lastAttemptRealtime, captureSpeed, lastAttemptSpeed, samples, pausedSamples, counters); }
    public static string GetHumanSummary() { Snapshot s = GetSnapshot(); long enter = 0, dispatched = 0; foreach (ActionSnapshot a in s.Actions) { enter += a.Entered; dispatched += a.Dispatched; } return "Action statistics: " + (s.Pawn?.LabelShort ?? "no pawn") + " · " + s.StopReason + " · attempts " + enter + " · dispatched " + dispatched + " · ticks " + s.StartTick + "-" + (s.Running ? s.CaptureTick : s.EndTick); }
    public static string GetReportText()
    {
        Snapshot s = GetSnapshot(); StringBuilder b = new(); b.Append("srstat fmt=1 session running=").Append(s.Running ? "1" : "0").Append(" reason=").Append(s.StopReason).Append(" pawn=").Append(s.Pawn?.ThingID ?? "none").Append(" tick_start=").Append(s.StartTick).Append(" tick_end=").Append(s.EndTick).Append(" realtime_start=").Append(s.StartRealtime.ToString("R", CultureInfo.InvariantCulture)).Append(" realtime_end=").Append(s.EndRealtime.ToString("R", CultureInfo.InvariantCulture)).Append(" capture_tick=").Append(s.CaptureTick).Append(" capture_realtime=").Append(s.CaptureRealtime.ToString("R", CultureInfo.InvariantCulture)).Append(" capture_speed=").Append(s.CaptureTimeSpeed.ToString("R", CultureInfo.InvariantCulture)).Append(" last_attempt_tick=").Append(s.LastAttemptTick).Append(" last_attempt_realtime=").Append(s.LastAttemptRealtime.ToString("R", CultureInfo.InvariantCulture)).Append(" last_attempt_speed=").Append(s.LastAttemptTimeSpeed.ToString("R", CultureInfo.InvariantCulture)).Append(" attempt_context_samples=").Append(s.TimingSamples).Append(" attempt_context_paused=").Append(s.PausedSamples);
        foreach (ActionSnapshot a in s.Actions) b.Append('\n').Append("srstat fmt=1 action=").Append(a.Action).Append(" denominator=try_trigger_enter").Append(" enter=").Append(a.Entered).Append(" scope_reject=").Append(a.ScopeRejected).Append(" disabled=").Append(a.Disabled).Append(" startup_pending=").Append(a.StartupPending).Append(" checks=").Append(a.Checks).Append(" pass=").Append(a.Passed).Append(" reject=").Append(a.Rejected).Append(" expected_p_sum=").Append(a.ExpectedProbability.ToString("R", CultureInfo.InvariantCulture)).Append(" action_cd=").Append(a.ActionCooldown).Append(" global_cd=").Append(a.GlobalCooldown).Append(" vocal_silent=").Append(a.VocalSilent).Append(" talking_reject=").Append(a.TalkingRejected).Append(" no_sound=").Append(a.NoSound).Append(" eligibility_reject=").Append(a.Eligibility).Append(" playback_failed=").Append(a.PlaybackFailed).Append(" dispatched=").Append(a.Dispatched);
        return b.ToString();
    }
    internal static bool IsRecording(Pawn candidate) => running && ReferenceEquals(pawn, candidate) && Valid(candidate, SqueakSettingsGameContext.CaptureRuntime());
    internal static void Enter(Pawn p, SqueakAction a) { if (!IsRecording(p)) return; counters[(int)a].Entered++; SampleTime(); revision++; }
    internal static void ScopeRejected(Pawn p, SqueakAction a) { if (IsRecording(p)) { counters[(int)a].ScopeRejected++; revision++; } }
    internal static void Disabled(Pawn p, SqueakAction a) { if (IsRecording(p)) { counters[(int)a].Disabled++; revision++; } }
    internal static void Probability(Pawn p, SqueakAction a, float probability, bool passed) { if (!IsRecording(p)) return; ref Counters c = ref counters[(int)a]; c.Checks++; c.ExpectedProbability += probability; if (passed) c.Passed++; else c.Rejected++; revision++; }
    internal static void Outcome(Pawn p, SqueakAction a, SqueakTriggerOutcome o)
    {
        if (!IsRecording(p)) return; ref Counters c = ref counters[(int)a]; switch (o) { case SqueakTriggerOutcome.PeriodicStartupPending: c.StartupPending++; break; case SqueakTriggerOutcome.ActionCooldown: c.ActionCooldown++; break; case SqueakTriggerOutcome.GlobalCooldown: c.GlobalCooldown++; break; case SqueakTriggerOutcome.VocalOrgansSilent: c.VocalSilent++; break; case SqueakTriggerOutcome.TalkingRejected: c.TalkingRejected++; break; case SqueakTriggerOutcome.NoSoundFallback: c.NoSound++; break; case SqueakTriggerOutcome.EligibilityRejected: c.Eligibility++; break; case SqueakTriggerOutcome.PlaybackFailed: c.PlaybackFailed++; break; case SqueakTriggerOutcome.Dispatched: c.Dispatched++; break; default: return; } revision++;
    }
    private static bool Valid(Pawn p, SqueakSettingsGameContext context) => context.HasPlayableMapUI && p.Spawned && p.MapHeld == map && map == context.Map;
    private static void SampleTime() { SqueakSettingsGameContext context = SqueakSettingsGameContext.CaptureRuntime(); lastAttemptTick = context.HasPlayableMapUI ? context.Tick : lastAttemptTick; lastAttemptRealtime = context.Realtime; lastAttemptSpeed = context.TickRateMultiplier; samples++; if (lastAttemptSpeed <= 0f) pausedSamples++; }
    private static void Validate(SqueakSettingsGameContext context) { if (running && (pawn == null || !Valid(pawn, context))) Stop(context.HasPlayableMapUI ? "pawn_or_map_invalid" : "game_context_unavailable", context); }
    private static void Stop(string value, SqueakSettingsGameContext context) { if (!running) return; endTick = context.HasPlayableMapUI ? context.Tick : lastAttemptTick; endRealtime = context.Realtime; running = false; reason = value; revision++; }
}
