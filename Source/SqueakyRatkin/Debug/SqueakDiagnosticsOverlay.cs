using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace SqueakyRatkin;

public enum SqueakDiagnosticsMode { Off, Selected, Visible }

/// <summary>
/// Low-frequency diagnostic session. The only on-pawn overlay draw is a single-character mark
/// (●); all detail lives in the draggable <see cref="SqueakDiagnosticsPanel"/>, which reads the
/// structured snapshot cache through <see cref="CachedPawns"/> and rebuilds formatted text only
/// when <see cref="Revision"/> changes. Layout still owns snapshot work; Repaint only draws marks.
/// </summary>
public static class SqueakDiagnosticsOverlay
{
    private const int MaxVisiblePawns = 16;
    private const float SelectedRefreshSeconds = 0.25f;
    private const float VisibleRefreshSeconds = 0.5f;

    internal const string Mark = "●";

    /// <summary>One cached structured snapshot per tracked pawn. Read-only for the panel; overlay mutates only during Layout refresh.</summary>
    internal sealed class CachedPawn
    {
        public Pawn Pawn = null!;
        public CompSqueaker Comp = null!;
        public SqueakDiagnosticSnapshot Snapshot;
        public string MarkText = string.Empty;
        public Color MarkColor = Color.white;
    }

    private static readonly List<CachedPawn> cachedPawns = new();
    private static readonly Dictionary<Pawn, CachedPawn> entriesByPawn = new();
    private static readonly HashSet<Pawn> refreshedPawns = new();
    private static SqueakDiagnosticsMode mode;
    private static Map? cachedMap;
    private static Pawn? selectedPawn;
    private static float nextRefreshRealtime;
    private static bool unavailableHookWarningLogged;
    private static int revision;
    private static SqueakDiagnosticsPanel? panel;

    /// <summary>Bumped whenever a snapshot entry is updated or removed; the panel rebuilds its formatted cache on change.</summary>
    internal static int Revision => revision;

    internal static SqueakDiagnosticsMode Mode => mode;

    internal static Pawn? SelectedPawn => selectedPawn;

    /// <summary>Read-only entry access for the panel. Only read during Repaint; the overlay only mutates during Layout.</summary>
    internal static IReadOnlyList<CachedPawn> CachedPawns => cachedPawns;

    /// <summary>The single readiness rule shared by the mark color and the panel badges.</summary>
    internal static bool ReadyFor(SqueakDiagnosticSnapshot s) => s.EffectiveTimingReady && s.CurrentActionEnabled
        && s.VocalCapability.VocalOrganEfficiency > SqueakVocalCapability.VocalSilenceThreshold;

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
        OpenPanel();
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

            DrawMark(new Vector2(pawn.DrawPos.x, pawn.DrawPos.z + 1.15f), entry.MarkText, entry.MarkColor);
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
                RefreshSnapshot(pawn, comp);
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
                RefreshSnapshot(pawn, comp);
            }
        }

        RemoveUnrefreshedPawns();
    }

    private static void RefreshSnapshot(Pawn pawn, CompSqueaker comp)
    {
        if (!entriesByPawn.TryGetValue(pawn, out CachedPawn? entry))
        {
            comp.ResetDiagnosticState();
            entry = new CachedPawn { Pawn = pawn, Comp = comp };
            entriesByPawn.Add(pawn, entry);
            cachedPawns.Add(entry);
        }

        refreshedPawns.Add(pawn);
        entry.Snapshot = comp.GetDiagnosticSnapshot();
        entry.MarkText = Mark;
        entry.MarkColor = ReadyFor(entry.Snapshot) ? SqueakySettingsUI.Success : SqueakySettingsUI.Gold;
        revision++;
    }

    private static void DrawMark(Vector2 position, string mark, Color color)
    {
        // GenMapUI.DrawText is locked to Tiny font, so visibility comes from a 4-direction
        // black outline (same pattern as SqueakMoteMaker.MoteTextWithBackground).
        const float edge = 0.05f;
        GenMapUI.DrawText(position + new Vector2(-edge, 0f), mark, Color.black);
        GenMapUI.DrawText(position + new Vector2(edge, 0f), mark, Color.black);
        GenMapUI.DrawText(position + new Vector2(0f, -edge), mark, Color.black);
        GenMapUI.DrawText(position + new Vector2(0f, edge), mark, Color.black);
        GenMapUI.DrawText(position, mark, color);
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
                revision++;
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
        ClosePanel();
    }

    private static void ClosePanel()
    {
        SqueakDiagnosticsPanel? current = panel;
        panel = null;
        // Null the reference before Close() so the panel's PreClose -> NotifyPanelClosed
        // cannot re-enter ClosePanel (the window is still in the stack during PreClose).
        if (current != null && current.IsOpen)
        {
            current.Close();
        }
    }

    /// <summary>Called from the panel's PreClose: the user closed the panel (X/Esc) — tear down the whole diagnostics session. Never re-enters window close.</summary>
    internal static void NotifyPanelClosed()
    {
        panel = null;
        ClearTrackedPawns();
        cachedMap = null;
        selectedPawn = null;
        nextRefreshRealtime = 0f;
        mode = SqueakDiagnosticsMode.Off;
        CompSqueaker.DiagnosticsEnabled = false;
    }

    private static void OpenPanel()
    {
        SqueakDiagnosticsPanel diagnosticsPanel = new();
        panel = diagnosticsPanel;
        Find.WindowStack.Add(diagnosticsPanel);
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
