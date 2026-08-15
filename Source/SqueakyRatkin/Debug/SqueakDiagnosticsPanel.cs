using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace SqueakyRatkin;

/// <summary>
/// Draggable, non-pausing diagnostics detail panel for <see cref="SqueakDiagnosticsOverlay"/>.
/// SR-styled (SqueakySettingsUI tokens) mirror of the overlay snapshots: Selected shows one
/// pawn's full detail, Visible shows up to 16 rows. Formatted text is rebuilt only when
/// <see cref="SqueakDiagnosticsOverlay.Revision"/> changes — never a per-frame re-snapshot or
/// re-layout. Closing the panel (native X or Esc) turns the whole diagnostics session off via
/// <see cref="SqueakDiagnosticsOverlay.NotifyPanelClosed"/>.
/// </summary>
internal sealed class SqueakDiagnosticsPanel : Window
{
    private const float TitleRowHeight = 26f;
    private const float ModeBadgeWidth = 96f;
    private const float CloseXReserve = 28f;
    private const float SectionTitleHeight = 21f;
    private const float FieldRowHeight = 19f;
    private const float RowHeight = 22f;
    private const float HeaderHeight = 30f;
    private const float DotWidth = 18f;
    private const float ActionWidth = 76f;
    private const float StateTextWidth = 56f;
    private const float HeaderBadgeWidth = 72f;
    private const float KeepGrabPx = 24f;
    private const float EscArmSeconds = 3f;
    private const float HintHeight = 22f;


    private enum LineKind { Section, Field, FieldWide }

    private readonly struct Line
    {
        public readonly LineKind Kind;
        public readonly string Label;
        public readonly string Value;
        public readonly float Height;

        public Line(LineKind kind, string label, string value = "", float height = FieldRowHeight)
        {
            Kind = kind;
            Label = label;
            Value = value;
            Height = height;
        }
    }

    private readonly struct Row
    {
        public readonly Color Dot;
        public readonly string PawnText;
        public readonly string Action;
        public readonly bool Ready;

        public Row(Color dot, string pawnText, string action, bool ready)
        {
            Dot = dot;
            PawnText = pawnText;
            Action = action;
            Ready = ready;
        }
    }

    private readonly List<Line> lines = new();
    private readonly List<Row> rows = new();
    private float minPanelHeight;
    private int cachedRevision = -1;
    private SqueakDiagnosticsMode cachedMode = SqueakDiagnosticsMode.Off;
    private string cachedPawnText = string.Empty;
    private bool cachedPawnReady;
    private float escArmedUntil = -1f;

    public SqueakDiagnosticsPanel()
    {
        // Non-modal diagnostic panel: never pauses the game, never absorbs surrounding input,
        // keeps camera motion alive. Mirrors Dialog_DevPalette/EditWindow non-modal flags.
        forcePause = false;
        absorbInputAroundWindow = false;
        preventCameraMotion = false;
        draggable = true;
        doCloseX = true;
        closeOnCancel = false; // Esc handled manually: two presses within EscArmSeconds close (armed state machine).
        closeOnAccept = false;
        closeOnClickedOutside = false;
        onlyOneOfTypeAllowed = true;
        focusWhenOpened = false;
        onlyDrawInDevMode = true;
        // Window height only ever grows within a session (never shrinks, so mode/content
        // changes never make the panel jump).
        minPanelHeight = InitialSize.y;
    }

    public override Vector2 InitialSize => new(400f, 560f);

    public override void PreClose()
    {
        base.PreClose();
        SqueakDiagnosticsOverlay.NotifyPanelClosed();
    }

    public override void WindowOnGUI()
    {
        base.WindowOnGUI();
        // Keep the title bar grabbable after dragging: at least KeepGrabPx of the window must
        // stay on screen so the panel can never be dragged fully off-screen.
        windowRect.x = Mathf.Clamp(windowRect.x, Mathf.Min(0f, KeepGrabPx - windowRect.width), UI.screenWidth - KeepGrabPx);
        windowRect.y = Mathf.Clamp(windowRect.y, 0f, Mathf.Max(0f, UI.screenHeight - KeepGrabPx));
    }

    public override void OnCancelKeyPressed()
    {
        // Esc does not close immediately: first press arms a 3s window, second press closes.
        // The direct Window.InnerWindowOnGUI call reaches this override regardless of closeOnCancel.
        float now = Time.realtimeSinceStartup;
        if (now > escArmedUntil)
        {
            escArmedUntil = now + EscArmSeconds;
        }
        else
        {
            escArmedUntil = -1f;
            Close();
        }
        Event.current?.Use();
    }

    public override void DoWindowContents(Rect inRect)
    {
        SqueakySettingsUI.PanelFrame(inRect, SqueakySurfaceKind.Raised);
        Rect inner = inRect.ContractedBy(SqueakySettingsUI.SpaceMd);

        RebuildIfStale();

        // Title row; keep the right edge clear for the vanilla small close X. Height is
        // measured from the rendered title so CJK Medium lines (≈33px) never get clipped.
        Rect titleRect = new(inner.x, inner.y, Mathf.Max(1f, inner.width - ModeBadgeWidth - SqueakySettingsUI.SpaceSm - CloseXReserve), TitleRowHeight);
        string titleText = "SR.Diagnostics.Panel.Title".Translate().ToString();
        Color oldColor = GUI.color;
        GameFont oldFont = Text.Font;
        Text.Font = GameFont.Medium;
        float titleH = Mathf.Max(TitleRowHeight, Text.CalcHeight(titleText, titleRect.width));
        titleRect.height = titleH;
        GUI.color = new Color(1f, .86f, .58f);
        Widgets.Label(titleRect, titleText);
        Text.Font = oldFont;
        GUI.color = oldColor;
        Rect badgeRect = new(inner.xMax - ModeBadgeWidth - CloseXReserve, inner.y + (titleH - 22f) * .5f, ModeBadgeWidth, 22f);
        DrawModeBadge(badgeRect);

        // Selected detail drives the window height: grow (never shrink) so the whole grid is
        // visible without a scrollbar. Visible mode content (≤352px + chrome) always fits 560.
        if (SqueakDiagnosticsOverlay.Mode == SqueakDiagnosticsMode.Selected)
        {
            float needed = 2f * Margin + 2f * SqueakySettingsUI.SpaceMd + titleH + SqueakySettingsUI.SpaceSm
                + HintHeight + HeaderHeight + MeasureSelectedContent();
            minPanelHeight = Mathf.Max(minPanelHeight, needed);
            windowRect.height = Mathf.Min(minPanelHeight, UI.screenHeight - 2f * KeepGrabPx);
        }

        Rect body = new(inner.x, inner.y + titleH + SqueakySettingsUI.SpaceSm,
            inner.width, Mathf.Max(1f, inner.yMax - HintHeight - (inner.y + titleH + SqueakySettingsUI.SpaceSm)));
        switch (SqueakDiagnosticsOverlay.Mode)
        {
            case SqueakDiagnosticsMode.Selected:
                DrawSelected(body);
                break;
            case SqueakDiagnosticsMode.Visible:
                DrawVisible(body);
                break;
            default:
                SqueakySettingsUI.EmptyState(body, "SR.Diagnostics.Panel.Empty".Translate());
                break;
        }

        // Fixed 22px hint slot at the bottom: only filled while Esc-close is armed, so the
        // layout never jumps between armed/unarmed states.
        Rect hintRect = new(inner.x, inner.yMax - HintHeight, inner.width, HintHeight);
        if (Time.realtimeSinceStartup <= escArmedUntil)
        {
            Color hintOldColor = GUI.color;
            TextAnchor hintOldAnchor = Text.Anchor;
            GameFont hintOldFont = Text.Font;
            Text.Font = GameFont.Tiny;
            Text.Anchor = TextAnchor.MiddleCenter;
            GUI.color = SqueakySettingsUI.Gold;
            Widgets.Label(hintRect, "SR.Diagnostics.Panel.CloseHint".Translate());
            Text.Anchor = hintOldAnchor;
            Text.Font = hintOldFont;
            GUI.color = hintOldColor;
        }
    }

    private static void DrawModeBadge(Rect rect)
    {
        SqueakySettingsUI.PanelFrame(rect, SqueakySurfaceKind.Base);
        Color oldColor = GUI.color;
        TextAnchor oldAnchor = Text.Anchor;
        GameFont oldFont = Text.Font;
        Text.Font = GameFont.Tiny;
        Text.Anchor = TextAnchor.MiddleCenter;
        GUI.color = SqueakySettingsUI.Gold;
        Widgets.Label(rect, SqueakDiagnosticsOverlay.Mode == SqueakDiagnosticsMode.Selected
            ? "SR.Diagnostics.Panel.Mode.Selected".Translate()
            : "SR.Diagnostics.Panel.Mode.Visible".Translate());
        Text.Font = oldFont;
        Text.Anchor = oldAnchor;
        GUI.color = oldColor;
    }

    private void DrawSelected(Rect body)
    {
        if (lines.Count == 0)
        {
            SqueakySettingsUI.EmptyState(body, "SR.Diagnostics.Panel.NoPawn".Translate());
            return;
        }

        Rect headerRect = new(body.x, body.y, body.width, HeaderHeight);
        SqueakySettingsUI.PanelFrame(headerRect, SqueakySurfaceKind.Base);
        Rect badgeRect = new(headerRect.xMax - HeaderBadgeWidth - SqueakySettingsUI.SpaceMd, headerRect.y + 3f, HeaderBadgeWidth, headerRect.height - 6f);
        SqueakySettingsUI.StatusPanel(badgeRect, cachedPawnReady
            ? "SR.Diagnostics.Ready".Translate() : "SR.Diagnostics.Blocked".Translate(),
            cachedPawnReady ? SqueakySurfaceKind.Success : SqueakySurfaceKind.Base);
        Color oldColor = GUI.color;
        TextAnchor oldAnchor = Text.Anchor;
        GameFont oldFont = Text.Font;
        Text.Font = GameFont.Small;
        Text.Anchor = TextAnchor.MiddleLeft;
        GUI.color = Color.white;
        SqueakySettingsUI.EllipsizedLabel(
            new Rect(headerRect.x + SqueakySettingsUI.SpaceMd, headerRect.y, Mathf.Max(1f, badgeRect.x - headerRect.x - SqueakySettingsUI.SpaceMd * 2f), headerRect.height),
            cachedPawnText);
        Text.Font = oldFont;
        Text.Anchor = oldAnchor;
        GUI.color = oldColor;

        Rect viewport = new(body.x, headerRect.yMax, body.width,
            Mathf.Max(1f, body.yMax - headerRect.yMax));
        // Two-column field grid drawn directly (no scroll view — the window height adapts to
        // the measured content): sections are full-width rows; a section's fields flow into two
        // column slots (ceil(n/2) rows per section). A leftover half-row is settled before any
        // full-width line (section title or FieldWide) so titles never overlap the previous
        // section's last field.
        float y = viewport.y;
        int column = 0;
        float columnWidth = (viewport.width - SqueakySettingsUI.SpaceMd) * .5f;
        for (int i = 0; i < lines.Count; i++)
        {
            Line line = lines[i];
            if (line.Kind == LineKind.Section)
            {
                if (column != 0)
                {
                    column = 0;
                    y += FieldRowHeight;
                }
                Rect rowRect = new(viewport.x, y, viewport.width, line.Height);
                DrawSectionTitle(rowRect, line.Label);
                y += line.Height;
            }
            else if (line.Kind == LineKind.FieldWide)
            {
                if (column != 0)
                {
                    column = 0;
                    y += FieldRowHeight;
                }
                DrawFieldRow(new Rect(viewport.x, y, viewport.width, line.Height), line.Label, line.Value, labelRatio: .3f, wrap: true);
                y += line.Height;
            }
            else
            {
                Rect slot = new(viewport.x + column * (columnWidth + SqueakySettingsUI.SpaceMd), y, columnWidth, FieldRowHeight);
                DrawFieldRow(slot, line.Label, line.Value);
                column++;
                if (column >= 2)
                {
                    column = 0;
                    y += FieldRowHeight;
                }
            }
        }
    }

    private float MeasureSelectedContent()
    {
        float height = 0f;
        int column = 0;
        for (int i = 0; i < lines.Count; i++)
        {
            if (lines[i].Kind == LineKind.Section)
            {
                if (column != 0)
                {
                    column = 0;
                    height += FieldRowHeight;
                }
                height += lines[i].Height;
            }
            else if (lines[i].Kind == LineKind.FieldWide)
            {
                if (column != 0)
                {
                    column = 0;
                    height += FieldRowHeight;
                }
                height += lines[i].Height;
            }
            else if (++column >= 2)
            {
                column = 0;
                height += FieldRowHeight;
            }
        }
        if (column != 0)
        {
            height += FieldRowHeight;
        }
        return height;
    }

    private void DrawVisible(Rect body)
    {
        if (rows.Count == 0)
        {
            SqueakySettingsUI.EmptyState(body, "SR.Diagnostics.Panel.Empty".Translate());
            return;
        }

        for (int i = 0; i < rows.Count; i++)
        {
            Row row = rows[i];
            Rect rowRect = new(body.x, body.y + i * RowHeight, body.width, RowHeight);
            if (i % 2 == 0)
            {
                Widgets.DrawBoxSolid(rowRect, new Color(.08f, .075f, .067f, .65f));
            }

            Color oldColor = GUI.color;
            TextAnchor oldAnchor = Text.Anchor;
            GameFont oldFont = Text.Font;
            Text.Anchor = TextAnchor.MiddleLeft;
            Text.Font = GameFont.Small;
            GUI.color = row.Dot;
            Widgets.Label(new Rect(rowRect.x + 2f, rowRect.y, DotWidth, rowRect.height), SqueakDiagnosticsOverlay.Mark);
            GUI.color = Color.white;
            // Columns: dot | PawnText (flexible, ellipsized) | Action (fixed, ellipsized) | Ready/Blocked (rightmost).
            float actionX = rowRect.x + DotWidth + SqueakySettingsUI.SpaceSm;
            float stateX = rowRect.xMax - StateTextWidth;
            float actionRectX = stateX - ActionWidth;
            SqueakySettingsUI.EllipsizedLabel(
                new Rect(actionX, rowRect.y, Mathf.Max(1f, actionRectX - actionX - SqueakySettingsUI.SpaceSm), rowRect.height),
                row.PawnText);
            SqueakySettingsUI.EllipsizedLabel(
                new Rect(actionRectX, rowRect.y, ActionWidth, rowRect.height),
                row.Action);
            Text.Anchor = TextAnchor.MiddleRight;
            GUI.color = row.Ready ? SqueakySettingsUI.Success : SqueakySettingsUI.Muted;
            Widgets.Label(new Rect(stateX, rowRect.y, Mathf.Max(1f, rowRect.xMax - stateX), rowRect.height),
                row.Ready ? "SR.Diagnostics.Ready".Translate() : "SR.Diagnostics.Blocked".Translate());
            Text.Anchor = oldAnchor;
            Text.Font = oldFont;
            GUI.color = oldColor;
        }
    }

    private static void DrawSectionTitle(Rect rect, string label)
    {
        Color oldColor = GUI.color;
        GameFont oldFont = Text.Font;
        Text.Font = GameFont.Small;
        GUI.color = SqueakySettingsUI.Gold;
        Widgets.Label(rect, label);
        Widgets.DrawBoxSolid(new Rect(rect.x, rect.yMax - 1f, rect.width, 1f),
            new Color(SqueakySettingsUI.Gold.r, SqueakySettingsUI.Gold.g, SqueakySettingsUI.Gold.b, .25f));
        Text.Font = oldFont;
        GUI.color = oldColor;
    }

    private static void DrawFieldRow(Rect rect, string label, string value, float labelRatio = .45f, bool wrap = false)
    {
        Color oldColor = GUI.color;
        TextAnchor oldAnchor = Text.Anchor;
        GameFont oldFont = Text.Font;
        bool oldWrap = Text.WordWrap;
        Text.Font = GameFont.Small;
        Text.Anchor = TextAnchor.MiddleLeft;
        float labelWidth = rect.width * labelRatio;
        GUI.color = SqueakySettingsUI.Muted;
        SqueakySettingsUI.EllipsizedLabel(new Rect(rect.x, rect.y, labelWidth, rect.height), label);
        Rect valueRect = new(rect.x + labelWidth + SqueakySettingsUI.SpaceSm, rect.y,
            Mathf.Max(1f, rect.xMax - (rect.x + labelWidth + SqueakySettingsUI.SpaceSm)), rect.height);
        if (wrap)
        {
            Text.WordWrap = true;
            Text.Anchor = TextAnchor.MiddleLeft;
            GUI.color = Color.white;
            Widgets.Label(valueRect, value);
        }
        else
        {
            Text.Anchor = TextAnchor.MiddleRight;
            GUI.color = Color.white;
            SqueakySettingsUI.EllipsizedLabel(valueRect, value);
        }
        Text.WordWrap = oldWrap;
        Text.Anchor = oldAnchor;
        Text.Font = oldFont;
        GUI.color = oldColor;
    }

    /// <summary>Rebuilds the formatted text cache on Repaint only when the overlay revision (or mode) changed. Zero per-frame re-snapshot/re-layout work.</summary>
    private void RebuildIfStale()
    {
        if (Event.current == null || Event.current.type != EventType.Repaint)
        {
            return;
        }

        SqueakDiagnosticsMode currentMode = SqueakDiagnosticsOverlay.Mode;
        int currentRevision = SqueakDiagnosticsOverlay.Revision;
        if (cachedRevision == currentRevision && cachedMode == currentMode)
        {
            return;
        }

        cachedRevision = currentRevision;
        cachedMode = currentMode;
        switch (currentMode)
        {
            case SqueakDiagnosticsMode.Selected:
                RebuildSelected();
                break;
            case SqueakDiagnosticsMode.Visible:
                RebuildVisible();
                break;
            default:
                lines.Clear();
                rows.Clear();
                cachedPawnText = string.Empty;
                break;
        }
    }

    private void RebuildSelected()
    {
        lines.Clear();
        cachedPawnText = string.Empty;

        Pawn? pawn = SqueakDiagnosticsOverlay.SelectedPawn;
        SqueakDiagnosticsOverlay.CachedPawn? entry = null;
        if (pawn != null)
        {
            foreach (SqueakDiagnosticsOverlay.CachedPawn candidate in SqueakDiagnosticsOverlay.CachedPawns)
            {
                if (ReferenceEquals(candidate.Pawn, pawn))
                {
                    entry = candidate;
                    break;
                }
            }
        }

        if (entry == null)
        {
            return;
        }

        SqueakDiagnosticSnapshot s = entry.Snapshot;
        cachedPawnReady = SqueakDiagnosticsOverlay.ReadyFor(s);
        cachedPawnText = string.Format("SR.Diagnostics.Panel.PawnHeader".Translate().ToString(), entry.Pawn.LabelShort, entry.Pawn.def.defName);

        string action = s.CurrentTimingAction.HasValue ? SqueakLabels.Action(s.CurrentTimingAction.Value) : FormatNone();
        string actionTiming = s.Timing.ActionIntervalSeconds.HasValue
            ? $"{s.Timing.ActionIntervalSeconds.Value:0.00}s/{s.Timing.ActionRemainingSeconds.GetValueOrDefault():0.00}s"
            : $"{s.Timing.ActionIntervalTicks.GetValueOrDefault()}t/{s.Timing.ActionRemainingTicks.GetValueOrDefault()}t";
        string globalTiming = s.Timing.GlobalApplicable
            ? $"{s.Timing.GlobalCooldownTicks}t/{s.Timing.GlobalRemainingTicks}t"
            : "SR.Diagnostics.Ignored".Translate().ToString();
        string xeno = s.Xenotype?.LabelCap ?? "SR.Diagnostics.Global".Translate().ToString();

        AddSection("SR.Diagnostics.Panel.Section.Trigger");
        AddField("SR.Diagnostics.Panel.Field.Action", action);
        AddField("SR.Diagnostics.Panel.Field.TriggerMode", FormatMode(s.CurrentTriggerMode));
        AddField("SR.Diagnostics.Panel.Field.CooldownClock", FormatClock(s.CurrentCooldownClock));
        AddField("SR.Diagnostics.Panel.Field.ActionTiming", actionTiming);
        AddField("SR.Diagnostics.Panel.Field.GlobalTiming", globalTiming);
        AddField("SR.Diagnostics.Panel.Field.TimingReady", FormatBool(s.EffectiveTimingReady));

        AddSection("SR.Diagnostics.Panel.Section.Multipliers");
        AddField("SR.Diagnostics.Panel.Field.Master", s.MasterMultiplier.ToString("0.##"));
        AddField("SR.Diagnostics.Panel.Field.Xenotype", xeno);
        AddField("SR.Diagnostics.Panel.Field.XenoMult", s.XenotypeIntervalMultiplier.ToString("0.##"));
        AddField("SR.Diagnostics.Panel.Field.ActionMult", s.CurrentActionIntervalMultiplier.ToString("0.##"));
        AddField("SR.Diagnostics.Panel.Field.TimeSpeed", s.TimeSpeedMultiplier.ToString("0.##"));

        AddSection("SR.Diagnostics.Panel.Section.Probability");
        AddField("SR.Diagnostics.Panel.Field.BaseProbability", s.BaseProbability.ToString("0.###"));
        AddField("SR.Diagnostics.Panel.Field.EffectiveProbability", s.EffectiveProbability.ToString("0.###"));

        AddSection("SR.Diagnostics.Panel.Section.Voice");
        AddField("SR.Diagnostics.Panel.Field.TalkingChance", s.VocalCapability.TalkingChance.ToString("0.##"));
        AddField("SR.Diagnostics.Panel.Field.VocalEfficiency", s.VocalCapability.VocalOrganEfficiency.ToString("0.##"));
        AddField("SR.Diagnostics.Panel.Field.TalkingGate", FormatBool(s.TalkingGateApplied));
        AddField("SR.Diagnostics.Panel.Field.DeathExempt", FormatBool(s.CurrentActionDeathExempt));

        AddSection("SR.Diagnostics.Panel.Section.Outcome");
        AddFieldWide("SR.Diagnostics.Panel.Field.LastEvaluation", FormatOutcome(s.LastEvaluation));
        AddFieldWide("SR.Diagnostics.Panel.Field.LastSignificant", FormatOutcome(s.LastSignificantOutcome));

        AddSection("SR.Diagnostics.Panel.Section.Population");
        AddField("SR.Diagnostics.Panel.Field.Candidates", s.Population.CandidateCount.ToString());
        AddField("SR.Diagnostics.Panel.Field.Audible", s.Population.AudibleCount.ToString());
        AddField("SR.Diagnostics.Panel.Field.Scale", s.Population.Scale.ToString("0.##"));

        AddSection("SR.Diagnostics.Panel.Section.Startup");
        AddField("SR.Diagnostics.Panel.Field.StartupPending", FormatBool(s.StartupPending));
    }

    private void RebuildVisible()
    {
        rows.Clear();
        IReadOnlyList<SqueakDiagnosticsOverlay.CachedPawn> entries = SqueakDiagnosticsOverlay.CachedPawns;
        for (int i = 0; i < entries.Count; i++)
        {
            SqueakDiagnosticsOverlay.CachedPawn entry = entries[i];
            SqueakDiagnosticSnapshot s = entry.Snapshot;
            bool ready = SqueakDiagnosticsOverlay.ReadyFor(s);
            string pawnText = string.Format("SR.Diagnostics.Panel.PawnHeader".Translate().ToString(), entry.Pawn.LabelShort, entry.Pawn.def.defName);
            string action = s.CurrentTimingAction.HasValue ? SqueakLabels.Action(s.CurrentTimingAction.Value) : FormatNone();
            rows.Add(new Row(entry.MarkColor, pawnText, action, ready));
        }
    }

    private void AddSection(string key)
    {
        // Measurement-driven section title row: CalcHeight at Small font, so CJK line heights
        // (~19-20px) never clip against the gold underline or the next row. Single-line titles:
        // wrap stays off, so the width only needs to be positive.
        string label = key.Translate();
        GameFont oldFont = Text.Font;
        bool oldWrap = Text.WordWrap;
        Text.Font = GameFont.Small;
        Text.WordWrap = false;
        float innerWidth = windowRect.width - 2f * Margin - 2f * SqueakySettingsUI.SpaceMd;
        float height = Mathf.Max(SectionTitleHeight, Text.CalcHeight(label, Mathf.Max(1f, innerWidth)));
        Text.WordWrap = oldWrap;
        Text.Font = oldFont;
        lines.Add(new Line(LineKind.Section, label, "", height));
    }

    private void AddField(string key, string value)
    {
        lines.Add(new Line(LineKind.Field, key.Translate(), value));
    }

    private void AddFieldWide(string key, string value)
    {
        // Full-width responsive row: value wraps and the row height adapts, measured at the
        // exact drawn value-column width (windowRect minus the vanilla 18px window Margin and
        // the 8px panel inset) so cached height never drifts from the drawn width.
        GameFont oldFont = Text.Font;
        bool oldWrap = Text.WordWrap;
        Text.Font = GameFont.Small;
        Text.WordWrap = true;
        float innerWidth = windowRect.width - 2f * Margin - 2f * SqueakySettingsUI.SpaceMd;
        float valueWidth = innerWidth * .7f - SqueakySettingsUI.SpaceSm;
        float height = Mathf.Max(FieldRowHeight, Text.CalcHeight(value, Mathf.Max(1f, valueWidth)));
        Text.WordWrap = oldWrap;
        Text.Font = oldFont;
        lines.Add(new Line(LineKind.FieldWide, key.Translate(), value, height));
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
}
