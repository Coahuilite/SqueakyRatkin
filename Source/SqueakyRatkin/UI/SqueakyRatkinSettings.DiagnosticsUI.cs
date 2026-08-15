using System;
using System.Globalization;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace SqueakyRatkin;

public partial class SqueakyRatkinSettings
{
    private Vector2 developerScroll;
    private bool actionStatisticsOpen = true;
    private bool audioPathDiagnosticsOpen = true;
    private bool statisticsHelpOpen;
    private int audioDetailIndex = -1;
    private Vector2 statisticsScroll;
    private Vector2 audioPathScroll;
    private SqueakActionStatistics.Snapshot? statisticsSnapshotCache;
    private long statisticsRevisionCache = -1;
    private int statisticsTimeBucket = -1;
    private string statisticsStatusCache = "";
    private string statisticsStatusTooltipCache = "";
    private readonly string[] statisticsDisplayCache = new string[SqueakActionDefinitions.Count];
    private readonly string[] statisticsTooltipCache = new string[SqueakActionDefinitions.Count];
    private SqueakAudioPathDiagnostics.Record[] audioRecordCache = Array.Empty<SqueakAudioPathDiagnostics.Record>();
    private string[] audioDisplayCache = Array.Empty<string>();
    private string[] audioSecondaryCache = Array.Empty<string>();
    private string[] audioTooltipCache = Array.Empty<string>();
    private long audioRevisionCache = -1;

    private void DrawDeveloperSettings(Rect rect)
    {
        RefreshStatisticsCacheOnRepaint(CurrentDrawContext);
        float width = rect.width;
        float contentHeight = MeasureDeveloperContentHeight(width);
        if (contentHeight > rect.height)
        {
            width = Mathf.Max(1f, rect.width - 16f);
            contentHeight = MeasureDeveloperContentHeight(width);
        }
        developerScroll.y = Mathf.Clamp(developerScroll.y, 0f, Mathf.Max(0f, contentHeight - rect.height));
        Rect view = new(0f, 0f, width, Mathf.Max(rect.height, contentHeight));
        Widgets.BeginScrollView(rect, ref developerScroll, view);
        Listing_Standard list = new();
        list.maxOneColumn = true;
        list.Begin(view);
        DrawCompactPageIntro(list, "SR.DevPage.Header".Translate(), "SR.DevPage.Desc".Translate());

        DrawSectionHeader(list, "SR.DevPage.Tools".Translate());
        bool narrow = list.ColumnWidth < 560f;
        Rect tools = list.GetRect(narrow ? 74f : 36f);
        float browserWidth = narrow ? tools.width : Mathf.Min(280f, tools.width * .55f);
        Rect browser = new(tools.x, tools.y, browserWidth, 34f);
        Rect disable = narrow
            ? new Rect(tools.x, tools.y + 40f, Mathf.Min(220f, tools.width), 34f)
            : new Rect(browser.xMax + 8f, tools.y, Mathf.Min(220f, tools.xMax - browser.xMax - 8f), 34f);
        if (SqueakySettingsUI.Button(browser, "SR.DevTools.AudioBrowser".Translate(), SqueakyButtonKind.Primary)) SqueakAudioBrowser.Open();
        if (SqueakySettingsUI.Button(disable, "SR.DevTools.Disable".Translate(), SqueakyButtonKind.Danger))
            Find.WindowStack.Add(new Dialog_SqueakyCompactMessageBox(
                "SR.DevTools.DisableConfirm".Translate(), "SR.Common.Confirm".Translate(), DisableDeveloperToolsNow,
                "SR.Common.Cancel".Translate(), null, "SR.DevTools.Disable".Translate(), SqueakyButtonKind.Danger));
        list.Label("SR.DevTools.AudioBrowser.Short".Translate());

        list.Gap(8f);
        DrawSectionHeader(list, "SR.DevTools.Logging.Header".Translate());
        list.Label("SR.DevTools.Logging.Short".Translate());
        string effective = (EffectiveDevLogging ? "SR.DevTools.Logging.EffectiveOn" : "SR.DevTools.Logging.EffectiveOff").Translate();
        string buildDefault = (DevLoggingAutoDefault ? "SR.DevTools.Logging.BuildDefaultOn" : "SR.DevTools.Logging.BuildDefaultOff").Translate();
        string loggingStatus = "SR.DevTools.Logging.Status".Translate(effective, buildDefault);
        SqueakySettingsUI.StatusPanel(list.GetRect(MeasureCompactStatusHeight(loggingStatus, list.ColumnWidth)), loggingStatus,
            EffectiveDevLogging ? SqueakySurfaceKind.Emphasized : SqueakySurfaceKind.Base);
        DrawDevLoggingModeCards(list.GetRect(MeasureDevLoggingModeCards(list.ColumnWidth)));

        DrawDeveloperTools(list);
        list.Gap(10f);
        DrawSectionHeader(list, "SR.DevPage.Build".Translate());
        string fullVersion = CurrentVersion();
        string buildIdentity = "SR.DevPage.BuildIdentity".Translate(fullVersion);
        SqueakySettingsUI.StatusPanel(list.GetRect(MeasureCompactStatusHeight(buildIdentity, list.ColumnWidth)), buildIdentity, SqueakySurfaceKind.Base);
        list.End();
        Widgets.EndScrollView();
    }

    private float MeasureDeveloperContentHeight(float width)
    {
        float height = MeasureCompactPageIntroHeight("SR.DevPage.Header".Translate(), "SR.DevPage.Desc".Translate(), width);
        height += 34f + (width < 560f ? 74f : 36f) + Text.CalcHeight("SR.DevTools.AudioBrowser.Short".Translate(), width) + 2f;
        string loggingStatus = "SR.DevTools.Logging.Status".Translate(
            (EffectiveDevLogging ? "SR.DevTools.Logging.EffectiveOn" : "SR.DevTools.Logging.EffectiveOff").Translate(),
            (DevLoggingAutoDefault ? "SR.DevTools.Logging.BuildDefaultOn" : "SR.DevTools.Logging.BuildDefaultOff").Translate());
        height += 8f + 34f + Text.CalcHeight("SR.DevTools.Logging.Short".Translate(), width) + 2f
            + MeasureCompactStatusHeight(loggingStatus, width);
        height += MeasureDevLoggingModeCards(width) + 8f;
        height += 34f + (actionStatisticsOpen ? MeasureCompactStatusHeight(statisticsStatusCache, width)
            + (width < 620f ? 74f : 34f) + 248f + 30f
            + (statisticsHelpOpen ? MeasureStatisticsHelpHeight(width) : 0f) : 0f);
        height += 8f + 34f + (audioPathDiagnosticsOpen ? 34f + Text.CalcHeight("SR.AudioPath.Short".Translate(), width) + 2f
            + 34f + 220f + MeasureAudioDetailHeight(width) : 0f);
#if SQUEAKY_EXPERIMENTAL
        height += MeasureKiiroCompatSectionHeight(width);
#endif
        string buildIdentity = "SR.DevPage.BuildIdentity".Translate(CurrentVersion());
        height += 10f + 34f + MeasureCompactStatusHeight(buildIdentity, width) + 20f;
        return height;
    }

    private void DrawDeveloperTools(Listing_Standard list)
    {
        list.Gap(8f);
        DrawCollapsibleHeader(list, "SR.Stats.Header".Translate(), ref actionStatisticsOpen);
        if (actionStatisticsOpen) DrawActionStatistics(list);
        list.Gap(8f);
        DrawCollapsibleHeader(list, "SR.AudioPath.Header".Translate(), ref audioPathDiagnosticsOpen);
        if (audioPathDiagnosticsOpen) DrawAudioPathDiagnostics(list);
#if SQUEAKY_EXPERIMENTAL
        list.Gap(8f);
        DrawKiiroCompatSection(list);
#endif
    }

#if SQUEAKY_EXPERIMENTAL
    private void DrawKiiroCompatSection(Listing_Standard list)
    {
        DrawSectionHeader(list, "SR.KiiroCompat.Header".Translate());
        list.Label("SR.KiiroCompat.Short".Translate());
        bool value = experimentalKiiroCompat;
        if (SqueakySettingsUI.Toggle(list.GetRect(34f), "SR.KiiroCompat.Enable".Translate(), ref value,
                tooltip: "SR.KiiroCompat.Enable.Tooltip".Translate()))
        {
            experimentalKiiroCompat = value;
            QueuePersistence();
        }
        string status = KiiroCompatStatusText();
        SqueakySettingsUI.StatusPanel(list.GetRect(MeasureCompactStatusHeight(status, list.ColumnWidth)), status,
            SqueakKiiroCompatAdapter.AttachedThisSession ? SqueakySurfaceKind.Emphasized : SqueakySurfaceKind.Base);
    }

    private string KiiroCompatStatusText()
    {
        if (SqueakKiiroCompatAdapter.AttachedThisSession) return "SR.KiiroCompat.Status.Attached".Translate();
        if (!ModsConfig.IsActive(SqueakKiiroCompatAdapter.KiiroPackageId)) return "SR.KiiroCompat.Status.Missing".Translate();
        return experimentalKiiroCompat ? "SR.KiiroCompat.Status.Pending".Translate() : "SR.KiiroCompat.Status.Off".Translate();
    }

    private float MeasureKiiroCompatSectionHeight(float width)
    {
        float height = 8f + 34f + Text.CalcHeight("SR.KiiroCompat.Short".Translate(), width) + 2f + 34f;
        return height + MeasureCompactStatusHeight(KiiroCompatStatusText(), width);
    }
#endif

    private void DrawActionStatistics(Listing_Standard list)
    {
        SqueakSettingsGameContext context = CurrentDrawContext;
        RefreshStatisticsCacheOnRepaint(context);
        SqueakActionStatistics.Snapshot? snapshot = statisticsSnapshotCache;
        bool validSelected = context.TryGetSelectedSqueaker(out Pawn? selected, out _);
        bool running = snapshot?.Running == true;
        SqueakySettingsUI.StatusPanel(list.GetRect(MeasureCompactStatusHeight(statisticsStatusCache, list.ColumnWidth)), statisticsStatusCache,
            running ? SqueakySurfaceKind.Success : SqueakySurfaceKind.Base);

        Rect controls = list.GetRect(list.ColumnWidth < 620f ? 74f : 34f);
        DrawFourButtons(controls,
            ("SR.Stats.Start".Translate().ToString(), validSelected, () =>
            {
                if (!SqueakActionStatistics.Start(selected, context)) Messages.Message("SR.Stats.StartFailed".Translate(), MessageTypeDefOf.RejectInput, false);
            }),
            ("SR.Stats.Stop".Translate().ToString(), running, () => SqueakActionStatistics.Stop(context)),
            ("SR.Stats.Reset".Translate().ToString(), true, () => SqueakActionStatistics.Reset(context)),
            ("SR.Stats.Copy".Translate().ToString(), true, () =>
            {
                GUIUtility.systemCopyBuffer = SqueakActionStatistics.GetReportText();
                Messages.Message("SR.Stats.Copied".Translate(), MessageTypeDefOf.NeutralEvent, false);
            }));

        const float rowHeight = 30f;
        Rect tableRect = list.GetRect(248f);
        SqueakySettingsUI.PanelFrame(tableRect, SqueakySurfaceKind.Base);
        Rect inner = tableRect.ContractedBy(6f);
        Rect header = new(inner.x, inner.y, inner.width, 24f);
        DrawStatisticsHeader(header);
        Rect viewport = new(inner.x, header.yMax + 2f, inner.width, inner.yMax - header.yMax - 2f);
        float statisticsContentHeight = SqueakActionDefinitions.Count * rowHeight;
        float statisticsWidth = statisticsContentHeight > viewport.height ? viewport.width - 16f : viewport.width;
        Rect view = new(0f, 0f, Mathf.Max(1f, statisticsWidth), statisticsContentHeight);
        Widgets.BeginScrollView(viewport, ref statisticsScroll, view);
        for (int i = 0; i < SqueakActionDefinitions.Count; i++)
        {
            Rect row = new(0f, i * rowHeight, view.width, rowHeight - 2f);
            if (i % 2 == 0) Widgets.DrawBoxSolid(row, new Color(.08f, .075f, .067f, .65f));
            DrawStatisticsRow(row, (SqueakAction)i, statisticsDisplayCache[i] ?? "", statisticsTooltipCache[i] ?? "");
        }
        Widgets.EndScrollView();
        Rect helpRow = list.GetRect(30f);
        Widgets.Label(new Rect(helpRow.x, helpRow.y, helpRow.width - SqueakySettingsUI.HelpSize - 6f, helpRow.height), "SR.Stats.Short".Translate());
        Rect help = new(helpRow.xMax - SqueakySettingsUI.HelpSize, helpRow.y + 4f, SqueakySettingsUI.HelpSize, SqueakySettingsUI.HelpSize);
        if (SqueakySettingsUI.HelpToggle(help, statisticsHelpOpen)) statisticsHelpOpen = !statisticsHelpOpen;
        if (statisticsHelpOpen)
        {
            string text = "SR.Stats.InlineHelp".Translate(statisticsStatusTooltipCache);
            Rect panel = list.GetRect(MeasureStatisticsHelpHeight(list.ColumnWidth) - 6f);
            SqueakySettingsUI.StatusPanel(panel, text, SqueakySurfaceKind.Base);
        }
    }

    private static void DrawStatisticsHeader(Rect rect)
    {
        Color old = GUI.color; GameFont font = Text.Font;
        GUI.color = SqueakySettingsUI.Muted; Text.Font = GameFont.Tiny;
        Widgets.Label(new Rect(rect.x, rect.y, rect.width * .22f, rect.height), "SR.Stats.Col.Action".Translate());
        Widgets.Label(new Rect(rect.x + rect.width * .22f, rect.y, rect.width * .78f, rect.height), "SR.Stats.Col.Funnel".Translate());
        Text.Font = font; GUI.color = old;
    }

    private static void DrawStatisticsRow(Rect rect, SqueakAction action, string detail, string tooltip)
    {
        float actionWidth = rect.width * .22f;
        SqueakySettingsUI.EllipsizedLabel(new Rect(rect.x + 4f, rect.y, actionWidth - 8f, rect.height), SqueakLabels.Action(action), action.ToString());
        SqueakySettingsUI.EllipsizedLabel(new Rect(rect.x + actionWidth, rect.y, rect.width - actionWidth - 4f, rect.height), detail, tooltip);
    }

    private void RefreshStatisticsCacheOnRepaint(SqueakSettingsGameContext context)
    {
        if (Event.current.type != EventType.Repaint) return;
        long revision = SqueakActionStatistics.Revision;
        int bucket = statisticsSnapshotCache?.Running == true ? Mathf.FloorToInt(context.Realtime * 4f) : -1;
        if (statisticsSnapshotCache != null && revision == statisticsRevisionCache && bucket == statisticsTimeBucket) return;
        SqueakActionStatistics.Snapshot snapshot = SqueakActionStatistics.GetSnapshot(context);
        statisticsSnapshotCache = snapshot;
        statisticsRevisionCache = SqueakActionStatistics.Revision;
        statisticsTimeBucket = snapshot.Running ? Mathf.FloorToInt(context.Realtime * 4f) : -1;
        float endRealtime = snapshot.Running ? snapshot.CaptureRealtime : snapshot.EndRealtime;
        float elapsed = Mathf.Max(0f, endRealtime - snapshot.StartRealtime);
        int displayedEndTick = snapshot.Running ? snapshot.CaptureTick : snapshot.EndTick;
        string pawn = snapshot.Pawn?.LabelShort ?? "SR.Common.None".Translate();
        statisticsStatusCache = "SR.Stats.Status".Translate(pawn, snapshot.Running ? "SR.Stats.Running".Translate() : "SR.Stats.Stopped".Translate(), elapsed.ToString("0.0"), snapshot.StopReason);
        statisticsStatusTooltipCache = "SR.Stats.Status.Tooltip".Translate(snapshot.StartTick, displayedEndTick,
            snapshot.CaptureTick, snapshot.CaptureTimeSpeed.ToString("0.##"), snapshot.LastAttemptTick,
            snapshot.LastAttemptTimeSpeed.ToString("0.##"), snapshot.EndTick, snapshot.TimingSamples, snapshot.PausedSamples);
        foreach (SqueakActionStatistics.ActionSnapshot value in snapshot.Actions)
        {
            string detail;
            SqueakActionConfig? config = GetActionConfig(value.Action);
            if (config?.mode == SqueakTriggerMode.RandomOneShot)
            {
                double expected = value.Checks > 0 ? value.ExpectedProbability / value.Checks : 0d;
                double observed = value.Checks > 0 ? (double)value.Passed / value.Checks : 0d;
                detail = "SR.Stats.Random".Translate(value.StartupPending, value.Checks, value.Passed, expected.ToString("0.000"), observed.ToString("0.000"), value.Dispatched);
            }
            else detail = "SR.Stats.Funnel".Translate(value.Entered, value.ScopeRejected + value.Disabled, value.StartupPending,
                value.ActionCooldown + value.GlobalCooldown, value.VocalSilent + value.TalkingRejected,
                value.NoSound + value.Eligibility + value.PlaybackFailed, value.Dispatched);
            statisticsDisplayCache[(int)value.Action] = detail;
            statisticsTooltipCache[(int)value.Action] = detail;
        }
    }

    private void DrawAudioPathDiagnostics(Listing_Standard list)
    {
        bool enabled = SqueakAudioPathDiagnostics.Enabled;
        if (SqueakySettingsUI.Toggle(list.GetRect(34f), "SR.AudioPath.Enabled".Translate(), ref enabled,
                tooltip: "SR.AudioPath.Enabled.Tooltip".Translate())) SqueakAudioPathDiagnostics.Enabled = enabled;
        list.Label("SR.AudioPath.Short".Translate());
        Rect controls = list.GetRect(34f);
        float half = (controls.width - 8f) * .5f;
        if (SqueakySettingsUI.Button(new Rect(controls.x, controls.y, half, 34f), "SR.AudioPath.Clear".Translate())) SqueakAudioPathDiagnostics.Clear();
        if (SqueakySettingsUI.Button(new Rect(controls.x + half + 8f, controls.y, half, 34f), "SR.AudioPath.Copy".Translate()))
        {
            GUIUtility.systemCopyBuffer = SqueakAudioPathDiagnostics.GetReportText();
            Messages.Message("SR.AudioPath.Copied".Translate(), MessageTypeDefOf.NeutralEvent, false);
        }

        RefreshAudioPathCacheOnRepaint();
        Rect details = list.GetRect(220f);
        SqueakySettingsUI.PanelFrame(details, SqueakySurfaceKind.Base);
        Rect viewport = details.ContractedBy(6f);
        float itemHeight = 82f;
        float audioContentHeight = audioRecordCache.Length * itemHeight;
        float audioWidth = audioContentHeight > viewport.height ? viewport.width - 16f : viewport.width;
        Rect view = new(0f, 0f, Mathf.Max(1f, audioWidth), Mathf.Max(viewport.height, audioContentHeight));
        Widgets.BeginScrollView(viewport, ref audioPathScroll, view);
        if (audioRecordCache.Length == 0) SqueakySettingsUI.EmptyState(new Rect(0f, 0f, view.width, 80f), "SR.AudioPath.Empty".Translate());
        for (int i = 0; i < audioRecordCache.Length; i++)
            if (DrawAudioRecord(new Rect(0f, i * itemHeight, view.width, itemHeight - 6f), audioDisplayCache[i], audioSecondaryCache[i], audioDetailIndex == i))
                audioDetailIndex = audioDetailIndex == i ? -1 : i;
        Widgets.EndScrollView();
        if (audioDetailIndex >= audioTooltipCache.Length) audioDetailIndex = -1;
        if (audioDetailIndex >= 0)
        {
            Rect detail = list.GetRect(MeasureAudioDetailHeight(list.ColumnWidth) - 6f);
            SqueakySettingsUI.StatusPanel(detail, audioTooltipCache[audioDetailIndex], SqueakySurfaceKind.Base);
        }
    }

    private float MeasureStatisticsHelpHeight(float width)
    {
        string text = "SR.Stats.InlineHelp".Translate(statisticsStatusTooltipCache);
        return Mathf.Max(88f, Text.CalcHeight(text, Mathf.Max(1f, width - 16f)) + 16f) + 6f;
    }

    private float MeasureAudioDetailHeight(float width)
    {
        if (audioDetailIndex < 0 || audioDetailIndex >= audioTooltipCache.Length) return 0f;
        return Mathf.Max(96f, Text.CalcHeight(audioTooltipCache[audioDetailIndex], Mathf.Max(1f, width - 16f)) + 16f) + 6f;
    }

    private static bool DrawAudioRecord(Rect rect, string primary, string secondary, bool expanded)
    {
        Widgets.DrawBoxSolid(rect, new Color(.075f, .07f, .062f, .74f));
        SqueakySettingsUI.DrawBorder(rect);
        Rect help = new(rect.xMax - SqueakySettingsUI.HelpSize - 7f, rect.y + 6f, SqueakySettingsUI.HelpSize, SqueakySettingsUI.HelpSize);
        Widgets.Label(new Rect(rect.x + 8f, rect.y + 4f, help.x - rect.x - 14f, 24f), primary);
        Color old = GUI.color; GameFont font = Text.Font; GUI.color = SqueakySettingsUI.Muted; Text.Font = GameFont.Tiny;
        Widgets.Label(new Rect(rect.x + 8f, rect.y + 28f, rect.width - 16f, 20f), secondary);
        Widgets.Label(new Rect(rect.x + 8f, rect.y + 48f, rect.width - 16f, 20f), "SR.AudioPath.RecordHint".Translate());
        Text.Font = font; GUI.color = old;
        return SqueakySettingsUI.HelpToggle(help, expanded);
    }

    private void RefreshAudioPathCacheOnRepaint()
    {
        if (Event.current.type != EventType.Repaint || audioRevisionCache == SqueakAudioPathDiagnostics.Revision) return;
        audioRecordCache = SqueakAudioPathDiagnostics.CopyNewestFirst();
        audioDisplayCache = new string[audioRecordCache.Length];
        audioSecondaryCache = new string[audioRecordCache.Length];
        audioTooltipCache = new string[audioRecordCache.Length];
        for (int i = 0; i < audioRecordCache.Length; i++)
        {
            SqueakAudioPathDiagnostics.Record record = audioRecordCache[i];
            string pack = record.PackDefName.NullOrEmpty() ? "SR.AudioPath.NoPack".Translate() : record.PackLabel;
            string source = record.ModName.NullOrEmpty() ? "—" : record.ModName;
            audioDisplayCache[i] = SqueakLabels.Action(record.Action) + " · " + record.Tier + " · " + record.SoundDefName;
            audioSecondaryCache[i] = pack + " · " + source;
            audioTooltipCache[i] = string.Format(CultureInfo.CurrentCulture, "SR.AudioPath.Record.Tooltip".Translate().ToString(),
                record.Action, record.Tier, record.SoundDefName, record.PackLabel.NullOrEmpty() ? "—" : record.PackLabel,
                record.PackDefName.NullOrEmpty() ? "—" : record.PackDefName, record.Authors.NullOrEmpty() ? "—" : record.Authors,
                record.ModName.NullOrEmpty() ? "—" : record.ModName, record.PackageId.NullOrEmpty() ? "—" : record.PackageId,
                record.TargetDefName.NullOrEmpty() ? "—" : record.TargetDefName);
        }
        audioRevisionCache = SqueakAudioPathDiagnostics.Revision;
    }

    private static void DrawFourButtons(Rect rect,
        (string label, bool enabled, Action action) a, (string label, bool enabled, Action action) b,
        (string label, bool enabled, Action action) c, (string label, bool enabled, Action action) d)
    {
        bool stacked = rect.width < 620f;
        float gap = 6f;
        float width = stacked ? (rect.width - gap) * .5f : (rect.width - gap * 3f) * .25f;
        float height = stacked ? 34f : rect.height;
        Rect[] boxes = stacked
            ? new[] { new Rect(rect.x, rect.y, width, height), new Rect(rect.x + width + gap, rect.y, width, height), new Rect(rect.x, rect.y + height + gap, width, height), new Rect(rect.x + width + gap, rect.y + height + gap, width, height) }
            : Enumerable.Range(0, 4).Select(i => new Rect(rect.x + i * (width + gap), rect.y, width, height)).ToArray();
        (string label, bool enabled, Action action)[] values = { a, b, c, d };
        for (int i = 0; i < values.Length; i++) if (SqueakySettingsUI.Button(boxes[i], values[i].label, enabled: values[i].enabled)) values[i].action();
    }
}
