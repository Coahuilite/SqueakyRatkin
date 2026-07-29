using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace SqueakyRatkin;

public enum SqueakDiagnosticsMode { Off, Selected, Visible }

/// <summary>Low-frequency diagnostic snapshots; repaint only draws strings prepared during Layout.</summary>
public static class SqueakDiagnosticsOverlay
{
    private const int MaxVisiblePawns = 16;
    private const float SelectedRefreshSeconds = 0.25f;
    private const float VisibleRefreshSeconds = 0.5f;
    private const float ShadowOffset = 0.035f;

    private sealed class CachedPawn
    {
        public Pawn Pawn = null!;
        public CompSqueaker Comp = null!;
        public string Text = string.Empty;
        public Color Color = Color.white;
    }

    private static readonly List<CachedPawn> cachedPawns = new();
    private static readonly Dictionary<Pawn, CachedPawn> entriesByPawn = new();
    private static readonly HashSet<Pawn> refreshedPawns = new();
    private static SqueakDiagnosticsMode mode;
    private static Map? cachedMap;
    private static Pawn? selectedPawn;
    private static float nextRefreshRealtime;
    private static bool unavailableHookWarningLogged;

    public static void SetMode(SqueakDiagnosticsMode newMode)
    {
        ClearSession();
        if (newMode == SqueakDiagnosticsMode.Off)
        {
            return;
        }

        if (!Patch_MapInterface_DiagnosticsOverlay.HookAvailable)
        {
            if (!unavailableHookWarningLogged)
            {
                unavailableHookWarningLogged = true;
                SqueakLog.DiagnosticsStartFailed();
            }

            return;
        }

        if (!Prefs.DevMode || Find.CurrentMap == null)
        {
            return;
        }

        mode = newMode;
        cachedMap = Find.CurrentMap;
        CompSqueaker.DiagnosticsEnabled = true;
    }

    /// <summary>Per-frame teardown guard. It must remain free of snapshot, formatting, translation, scan, and draw work.</summary>
    public static void MaintainLifecycle()
    {
        if (mode == SqueakDiagnosticsMode.Off)
        {
            return;
        }

        Map? map = Find.CurrentMap;
        if (!Prefs.DevMode || map == null || !ReferenceEquals(cachedMap, map))
        {
            ClearSession();
        }
    }

    /// <summary>Layout-only lifecycle and snapshot work. Never call from Repaint.</summary>
    public static void RefreshIfDue()
    {
        MaintainLifecycle();

        if (mode == SqueakDiagnosticsMode.Off)
        {
            return;
        }

        // Keep diagnostics fresh even when production population scaling is disabled. Individual
        // Comp diagnostic snapshots only read the resulting immutable shared state.
        CompSqueaker.MaintainPeriodicPopulationDiagnostics();

        Map map = cachedMap!;

        float now = Time.realtimeSinceStartup;
        if (mode == SqueakDiagnosticsMode.Selected)
        {
            Pawn? pawn = Find.Selector.SingleSelectedThing as Pawn;
            if (!ReferenceEquals(pawn, selectedPawn) || now >= nextRefreshRealtime)
            {
                RefreshSelected(pawn, now);
            }
        }
        else if (now >= nextRefreshRealtime)
        {
            RefreshVisible(map, now);
        }
    }

    /// <summary>Repaint-only cached drawing. Do not add snapshot, translation, or formatting work here.</summary>
    public static void DrawCached()
    {
        Map? map = cachedMap;
        if (mode == SqueakDiagnosticsMode.Off || map == null || !Prefs.DevMode || !ReferenceEquals(Find.CurrentMap, map))
        {
            return;
        }

        CellRect view = Find.CameraDriver.CurrentViewRect.ExpandedBy(1);
        for (int i = 0; i < cachedPawns.Count; i++)
        {
            CachedPawn entry = cachedPawns[i];
            Pawn pawn = entry.Pawn;
            if (!pawn.Spawned || pawn.Dead || pawn.MapHeld != map || !view.Contains(pawn.Position))
            {
                continue;
            }

            DrawCachedText(new Vector2(pawn.DrawPos.x, pawn.DrawPos.z + 1.15f), entry.Text, entry.Color);
        }
    }

    private static void RefreshSelected(Pawn? pawn, float now)
    {
        selectedPawn = pawn;
        nextRefreshRealtime = now + SelectedRefreshSeconds;
        refreshedPawns.Clear();
        CellRect view = Find.CameraDriver.CurrentViewRect.ExpandedBy(1);
        if (pawn?.Spawned == true && !pawn.Dead && pawn.MapHeld == cachedMap && view.Contains(pawn.Position))
        {
            CompSqueaker? comp = pawn.GetComp<CompSqueaker>();
            if (comp != null)
            {
                RefreshSnapshot(pawn, comp, true);
            }
        }

        RemoveUnrefreshedPawns();
    }

    private static void RefreshVisible(Map map, float now)
    {
        nextRefreshRealtime = now + VisibleRefreshSeconds;
        CellRect view = Find.CameraDriver.CurrentViewRect.ExpandedBy(1);
        IReadOnlyList<Pawn> pawns = map.mapPawns.AllPawnsSpawned;
        refreshedPawns.Clear();
        for (int i = 0; i < pawns.Count && refreshedPawns.Count < MaxVisiblePawns; i++)
        {
            Pawn pawn = pawns[i];
            if (pawn.Dead || !view.Contains(pawn.Position))
            {
                continue;
            }

            CompSqueaker? comp = pawn.GetComp<CompSqueaker>();
            if (comp != null)
            {
                RefreshSnapshot(pawn, comp, false);
            }
        }

        RemoveUnrefreshedPawns();
    }

    private static void RefreshSnapshot(Pawn pawn, CompSqueaker comp, bool detailed)
    {
        if (!entriesByPawn.TryGetValue(pawn, out CachedPawn? entry))
        {
            comp.ResetDiagnosticState();
            entry = new CachedPawn { Pawn = pawn, Comp = comp };
            entriesByPawn.Add(pawn, entry);
            cachedPawns.Add(entry);
        }

        refreshedPawns.Add(pawn);
        SqueakDiagnosticSnapshot snapshot = comp.GetDiagnosticSnapshot();
        entry.Text = detailed ? FormatSelectedSnapshot(pawn, snapshot) : FormatVisibleSnapshot(pawn, snapshot);
        entry.Color = snapshot.EffectiveTimingReady && snapshot.CurrentActionEnabled
            && snapshot.VocalCapability.VocalOrganEfficiency > SqueakVocalCapability.VocalSilenceThreshold
            ? new Color(0.75f, 1f, 0.75f) : new Color(1f, 0.85f, 0.55f);
    }

    private static string FormatSelectedSnapshot(Pawn pawn, SqueakDiagnosticSnapshot s)
    {
        string action = s.CurrentTimingAction.HasValue ? SqueakLabels.Action(s.CurrentTimingAction.Value) : FormatNone();
        string modeText = FormatMode(s.CurrentTriggerMode);
        string clock = FormatClock(s.CurrentCooldownClock);
        string actionTiming = s.Timing.ActionIntervalSeconds.HasValue
            ? $"{s.Timing.ActionIntervalSeconds.Value:0.00}s/{s.Timing.ActionRemainingSeconds.GetValueOrDefault():0.00}s"
            : $"{s.Timing.ActionIntervalTicks.GetValueOrDefault()}t/{s.Timing.ActionRemainingTicks.GetValueOrDefault()}t";
        string globalTiming = s.Timing.GlobalApplicable
            ? $"{s.Timing.GlobalCooldownTicks}t/{s.Timing.GlobalRemainingTicks}t"
            : "SR.Diagnostics.Ignored".Translate().ToString();
        string xeno = s.Xenotype?.LabelCap ?? "SR.Diagnostics.Global".Translate().ToString();
        return string.Format("SR.Diagnostics.Line1".Translate().ToString(), pawn.LabelShort, action,
                FormatBool(s.CurrentActionEnabled), modeText, clock)
            + "\n" + string.Format("SR.Diagnostics.Line2".Translate().ToString(), s.MasterMultiplier.ToString("0.##"), xeno,
                s.XenotypeIntervalMultiplier.ToString("0.##"), s.CurrentActionIntervalMultiplier.ToString("0.##"),
                s.TimeSpeedMultiplier.ToString("0.##"), s.EffectiveProbability.ToString("0.##"),
                s.VocalCapability.TalkingChance.ToString("0.##"), s.VocalCapability.VocalOrganEfficiency.ToString("0.##"),
                FormatBool(s.TalkingGateApplied), FormatBool(s.CurrentActionDeathExempt))
            + "\n" + string.Format("SR.Diagnostics.Line3".Translate().ToString(), actionTiming, globalTiming,
                FormatBool(s.EffectiveTimingReady),
                FormatOutcome(s.LastEvaluation), FormatOutcome(s.LastSignificantOutcome))
            + "\n" + string.Format("SR.Diagnostics.Line4".Translate().ToString(),
                s.Population.CandidateCount, s.Population.AudibleCount, s.Population.Scale.ToString("0.##"),
                s.BaseProbability.ToString("0.###"), s.EffectiveProbability.ToString("0.###"),
                FormatTiming(s.BaseTiming), FormatTiming(s.Timing), FormatBool(s.StartupPending));
    }

    private static string FormatVisibleSnapshot(Pawn pawn, SqueakDiagnosticSnapshot s)
    {
        string action = s.CurrentTimingAction.HasValue ? SqueakLabels.Action(s.CurrentTimingAction.Value) : FormatNone();
        string state = s.CurrentActionEnabled && s.EffectiveTimingReady
            && s.VocalCapability.VocalOrganEfficiency > SqueakVocalCapability.VocalSilenceThreshold
            ? "SR.Diagnostics.Ready".Translate().ToString()
            : "SR.Diagnostics.Blocked".Translate().ToString();
        return string.Format("SR.Diagnostics.VisibleLine".Translate().ToString(), pawn.LabelShort, action, state,
            FormatOutcome(s.LastSignificantOutcome));
    }

    private static string FormatMode(SqueakTriggerMode? triggerMode) => triggerMode switch
    {
        SqueakTriggerMode.EachTime => "SR.Diagnostics.Mode.EachTime".Translate().ToString(),
        SqueakTriggerMode.RandomOneShot => "SR.Diagnostics.Mode.RandomOneShot".Translate().ToString(),
        SqueakTriggerMode.External => "SR.Diagnostics.Mode.External".Translate().ToString(),
        SqueakTriggerMode.Sustained => "SR.Diagnostics.Mode.Sustained".Translate().ToString(),
        _ => FormatNone()
    };

    private static string FormatClock(SqueakCooldownClock? cooldownClock) => cooldownClock switch
    {
        SqueakCooldownClock.GameTicks => "SR.Diagnostics.Clock.GameTicks".Translate().ToString(),
        SqueakCooldownClock.Realtime => "SR.Diagnostics.Clock.Realtime".Translate().ToString(),
        _ => FormatNone()
    };

    private static string FormatBool(bool value) => value
        ? "SR.Diagnostics.Bool.True".Translate().ToString()
        : "SR.Diagnostics.Bool.False".Translate().ToString();

    private static string FormatNone() => "SR.Diagnostics.None".Translate().ToString();

    private static string FormatTiming(SqueakTimingEvaluation timing) => timing.ActionIntervalSeconds.HasValue
        ? $"{timing.ActionIntervalSeconds.Value:0.00}s/{timing.GlobalCooldownTicks}t"
        : $"{timing.ActionIntervalTicks.GetValueOrDefault()}t/{timing.GlobalCooldownTicks}t";

    private static string FormatOutcome(SqueakRecentOutcome? outcome) => outcome.HasValue
        ? string.Format("SR.Diagnostics.Outcome".Translate().ToString(), SqueakLabels.Action(outcome.Value.Action),
            FormatOutcomeToken(outcome.Value.Outcome), FormatBool(outcome.Value.CooldownConsumed))
        : FormatNone();

    private static string FormatOutcomeToken(SqueakTriggerOutcome outcome) => outcome switch
    {
        SqueakTriggerOutcome.Disabled => "SR.Diagnostics.Outcome.Disabled".Translate().ToString(),
        SqueakTriggerOutcome.ProbabilityRejected => "SR.Diagnostics.Outcome.ProbabilityRejected".Translate().ToString(),
        SqueakTriggerOutcome.ActionCooldown => "SR.Diagnostics.Outcome.ActionCooldown".Translate().ToString(),
        SqueakTriggerOutcome.GlobalCooldown => "SR.Diagnostics.Outcome.GlobalCooldown".Translate().ToString(),
        SqueakTriggerOutcome.VocalOrgansSilent => "SR.Diagnostics.Outcome.VocalOrgansSilent".Translate().ToString(),
        SqueakTriggerOutcome.TalkingRejected => "SR.Diagnostics.Outcome.TalkingRejected".Translate().ToString(),
        SqueakTriggerOutcome.NoSoundFallback => "SR.Diagnostics.Outcome.NoSoundFallback".Translate().ToString(),
        SqueakTriggerOutcome.Dispatched => "SR.Diagnostics.Outcome.Dispatched".Translate().ToString(),
        SqueakTriggerOutcome.EligibilityRejected => "SR.Diagnostics.Outcome.EligibilityRejected".Translate().ToString(),
        SqueakTriggerOutcome.PlaybackFailed => "SR.Diagnostics.Outcome.PlaybackFailed".Translate().ToString(),
        SqueakTriggerOutcome.PeriodicStartupPending => "SR.Diagnostics.Outcome.PeriodicStartupPending".Translate().ToString(),
        _ => FormatNone()
    };

    private static void DrawCachedText(Vector2 position, string text, Color color)
    {
        GenMapUI.DrawText(position + new Vector2(ShadowOffset, ShadowOffset), text, Color.black);
        GenMapUI.DrawText(position, text, color);
    }

    private static void RemoveUnrefreshedPawns()
    {
        for (int i = cachedPawns.Count - 1; i >= 0; i--)
        {
            CachedPawn entry = cachedPawns[i];
            if (!refreshedPawns.Contains(entry.Pawn))
            {
                entry.Comp.ResetDiagnosticState();
                entriesByPawn.Remove(entry.Pawn);
                cachedPawns.RemoveAt(i);
            }
        }
    }

    private static void ClearSession()
    {
        ClearTrackedPawns();
        cachedMap = null;
        selectedPawn = null;
        nextRefreshRealtime = 0f;
        mode = SqueakDiagnosticsMode.Off;
        CompSqueaker.DiagnosticsEnabled = false;
    }

    private static void ClearTrackedPawns()
    {
        for (int i = 0; i < cachedPawns.Count; i++)
        {
            cachedPawns[i].Comp.ResetDiagnosticState();
        }
        cachedPawns.Clear();
        entriesByPawn.Clear();
        refreshedPawns.Clear();
    }
}
