using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace SqueakyRatkin;

public partial class SqueakyRatkinSettings
{
    private Vector2 xenotypeListScroll, xenotypeEditorScroll;
    private string? selectedXenotype;
    private SqueakVoicePackScope selectedTargetScope = SqueakVoicePackScope.Xenotype;
    private string selectedTargetName = "";
    private XenotypePresetDraft? xenotypeDraft;
    private readonly Dictionary<string, string> xenotypeNumericBuffers = new();
    private bool xenotypeNarrowEditorStep;
    private string xenotypeSearch = "";
    private XenotypeEditorTab xenotypeEditorTab;
    private SqueakXenotypeCatalogSnapshot? xenotypeRowCacheCatalog;
    private object? xenotypeRowCacheLanguage;
    private readonly Dictionary<string, XenotypeRowSummary> xenotypeRowSummaries = new(StringComparer.Ordinal);
    private readonly List<XenotypeRowSummary> sortedXenotypeRows = new();
    private readonly List<XenotypeRowSummary> filteredXenotypeRows = new();
    private string? xenotypeRowFilterQuery;
    private bool filterConfigured, filterCandidates, filterEnabled, filterOrphan;
    private string behaviorActionSearch = "", audioPackSearch = "", racePackSearch = "";
    private bool remixConfirmationOpen;
    private bool voiceSourceHelpOpen;
    private bool raceLayerHelpOpen;
    private bool conflictRecoveryHelpOpen;

    private enum XenotypeEditorTab { Behavior, AudioPacks }

    private readonly struct XenotypeManagementLayout
    {
        internal readonly Rect Title, Help, Modes, Refresh, Summary, Note, InlineHelp;
        internal readonly float Height;

        internal XenotypeManagementLayout(Rect title, Rect help, Rect modes, Rect refresh, Rect summary,
            Rect note, Rect inlineHelp, float height)
        {
            Title = title; Help = help; Modes = modes; Refresh = refresh; Summary = summary;
            Note = note; InlineHelp = inlineHelp; Height = height;
        }
    }

    private sealed class XenotypeRowSummary
    {
        internal SqueakXenotypeTargetCandidate Candidate;
        internal string DefName = "";
        internal string DisplayName = "";
        internal string SourceName = "";
        internal string SourceLabel = "";
        internal string SearchText = "";
        internal bool TargetUnavailable;
        internal bool HasCanonicalConflict;
        internal bool IsDormant;
        internal bool HasBehaviorOverride;
        internal int EnabledPackCount;
        internal int OrphanCount;
        internal int CandidateCount;
    }

    private void CommitVoicePackMode(SqueakVoicePackMode target)
    {
        if (target == voicePackMode) return;
        CommitContinuousXenotypeEdit();
        voicePackMode = target;
        NotifyDiscreteResolverRuntimeChanged();
        QueuePersistence();
        InvalidateXenotypeRowCache();
    }

    private void RequestVoicePackMode(SqueakVoicePackMode target)
    {
        if (target == voicePackMode || remixConfirmationOpen) return;
        if (target != SqueakVoicePackMode.Remix) { CommitVoicePackMode(target); return; }
        remixConfirmationOpen = true;
        Find.WindowStack.Add(new Dialog_SqueakyCompactMessageBox(
            "SR.VoicePack.RemixConfirm1.Body".Translate(), "SR.VoicePack.RemixConfirm1.Continue".Translate(), ShowFinalRemixConfirmation,
            "SR.Common.Cancel".Translate(), CancelRemixConfirmation, "SR.VoicePack.RemixConfirm.Title".Translate(),
            SqueakyButtonKind.Secondary, SqueakyButtonKind.Primary, closeAction: CancelRemixConfirmation));
    }

    private void ShowFinalRemixConfirmation()
    {
        Find.WindowStack.Add(new Dialog_SqueakyCompactMessageBox(
            "SR.VoicePack.RemixConfirm2.Body".Translate(), "SR.VoicePack.RemixConfirm2.Enable".Translate(), ConfirmRemixMode,
            "SR.Common.Cancel".Translate(), CancelRemixConfirmation, "SR.VoicePack.RemixConfirm.Title".Translate(),
            SqueakyButtonKind.Danger, SqueakyButtonKind.Secondary, reverseButtons: true, inputDelayFrames: 2,
            closeAction: CancelRemixConfirmation));
    }

    private void ConfirmRemixMode() { CommitVoicePackMode(SqueakVoicePackMode.Remix); remixConfirmationOpen = false; }
    private void CancelRemixConfirmation() { remixConfirmationOpen = false; }

    private void DrawXenotypeSettings(Rect rect)
    {
        if (rect.width <= 1f || rect.height <= 1f) return;
        GUI.BeginGroup(rect);
        DrawXenotypeSettingsContents(new Rect(0f, 0f, rect.width, rect.height));
        GUI.EndGroup();
    }

    private void DrawXenotypeSettingsContents(Rect rect)
    {
        const float paneGap = 8f;
        bool low = rect.height < 540f;
        float managementHeight = MeasureXenotypeManagementHeight(rect.width, low);
        Rect management = new(rect.x, rect.y, rect.width, managementHeight);
        DrawXenotypeManagement(management, low);
        float contentY = management.yMax + paneGap;
        Rect content = new(rect.x, contentY, rect.width, Mathf.Max(0f, rect.yMax - contentY));
        if (content.width < 40f || content.height < 40f) return;
        DrawXenotypeAssignment(content);
    }

    private float MeasureXenotypeManagementHeight(float width, bool low)
    {
        return ArrangeXenotypeManagement(new Rect(0f, 0f, width, 1f), low).Height;
    }

    private XenotypeManagementLayout ArrangeXenotypeManagement(Rect rect, bool low)
    {
        const float inset = 7f;
        bool narrow = rect.width < 760f;
        Rect inner = new(rect.x + inset, rect.y + inset, Mathf.Max(1f, rect.width - inset * 2f), 1f);
        const float titleHeight = 24f;
        Rect help = new(inner.xMax - SqueakySettingsUI.HelpSize, inner.y,
            SqueakySettingsUI.HelpSize, SqueakySettingsUI.HelpSize);
        Rect title = new(inner.x, inner.y, Mathf.Max(1f, help.x - inner.x - 6f), titleHeight);
        Rect modes = new(inner.x, title.yMax + 4f,
            narrow ? inner.width : Mathf.Max(1f, inner.width - 156f), 32f);
        Rect refresh;
        Rect summary;
        if (narrow)
        {
            refresh = new Rect(inner.xMax - 120f, modes.yMax + 6f, 120f, 30f);
            summary = new Rect(inner.x, refresh.y, Mathf.Max(1f, refresh.x - inner.x - 7f), 30f);
        }
        else
        {
            refresh = new Rect(inner.xMax - 150f, modes.y, 150f, 32f);
            summary = new Rect(inner.x, modes.yMax + 7f, inner.width, 28f);
        }

        float nextY = summary.yMax;
        Rect note = new(inner.x, nextY, inner.width, 0f);
        Rect inlineHelp = new(inner.x, nextY + 4f, inner.width, 0f);
        if (voiceSourceHelpOpen)
        {
            float helpHeight = MeasureVoiceSourceHelpHeight(rect.width);
            inlineHelp = new Rect(inner.x, nextY + 4f, inner.width, helpHeight);
            nextY = inlineHelp.yMax;
        }
        else if (!low)
        {
            string noteText = (ModsConfig.BiotechActive ? "SR.VoiceSource.Short" : "SR.VoiceSource.NoBiotechShort").Translate();
            GameFont oldFont = Text.Font;
            Text.Font = GameFont.Tiny;
            float noteHeight = Mathf.Max(18f, Text.CalcHeight(noteText, inner.width));
            Text.Font = oldFont;
            note = new Rect(inner.x, nextY + 2f, inner.width, noteHeight);
            nextY = note.yMax;
        }
        return new XenotypeManagementLayout(title, help, modes, refresh, summary, note, inlineHelp,
            nextY - rect.y + inset);
    }

    private void DrawXenotypeManagement(Rect rect, bool low)
    {
        SqueakXenotypeCatalogSnapshot catalog = SqueakXenotypeCatalog.Current;
        SqueakySettingsUI.PanelFrame(rect, SqueakySurfaceKind.Emphasized);
        XenotypeManagementLayout layout = ArrangeXenotypeManagement(rect, low);
        SqueakySettingsUI.SectionHeader(layout.Title, "SR.VoiceSource.Header".Translate());
        if (SqueakySettingsUI.HelpToggle(layout.Help, voiceSourceHelpOpen)) voiceSourceHelpOpen = !voiceSourceHelpOpen;
        DrawCompactVoicePackModes(layout.Modes);
        if (SqueakySettingsUI.Button(layout.Refresh, "SR.Xeno.Refresh".Translate(), SqueakyButtonKind.Secondary))
        {
            CommitContinuousXenotypeEdit();
            string? keep = selectedXenotype;
            RefreshCatalogAndRuntime();
            InvalidateXenotypeRowCache();
            IReadOnlyList<SqueakXenotypeTargetCandidate> refreshedCandidates = SqueakXenotypeCatalog.Current.GetTargetCandidates(voicePackSelections, xenotypePresets);
            if (keep == null || !refreshedCandidates.Any(candidate => candidate.DefName == keep)) { selectedXenotype = null; xenotypeDraft = null; xenotypeNarrowEditorStep = false; }
            else RebuildXenotypeDraft();
        }
        string summary = InstalledVoicePackCompactSummary(catalog);
        DrawClippedLabel(layout.Summary, summary, summary);
        if (layout.Note.height > 0f)
        {
            Color old = GUI.color; GameFont font = Text.Font; GUI.color = SqueakySettingsUI.Muted; Text.Font = GameFont.Tiny;
            Widgets.Label(layout.Note, (ModsConfig.BiotechActive ? "SR.VoiceSource.Short" : "SR.VoiceSource.NoBiotechShort").Translate());
            Text.Font = font; GUI.color = old;
        }
        if (layout.InlineHelp.height > 0f)
        {
            SqueakySettingsUI.StatusPanel(layout.InlineHelp, VoiceSourceHelpText(), SqueakySurfaceKind.Base);
        }
    }

    private float MeasureVoiceSourceHelpHeight(float width)
    {
        return MeasureInlinePanelHeight(VoiceSourceHelpText(), width - 16f);
    }

    private string VoiceSourceHelpText()
    {
        string modeKey = !ModsConfig.BiotechActive ? voicePackMode switch
        {
            SqueakVoicePackMode.Fallback => "SR.VoiceSource.Help.NoBiotech.Fallback",
            SqueakVoicePackMode.Remix => "SR.VoiceSource.Help.NoBiotech.Remix",
            _ => "SR.VoiceSource.Help.NoBiotech.Off"
        } : voicePackMode switch
        {
            SqueakVoicePackMode.Fallback => "SR.VoiceSource.Help.Fallback",
            SqueakVoicePackMode.Remix => "SR.VoiceSource.Help.Remix",
            _ => "SR.VoiceSource.Help.Off"
        };
        string mode = modeKey.Translate();
        return mode + "\n" + "SR.VoiceSource.Help.Common".Translate();
    }

    private void DrawCompactVoicePackModes(Rect rect)
    {
        SqueakVoicePackMode[] modes = { SqueakVoicePackMode.Off, SqueakVoicePackMode.Fallback, SqueakVoicePackMode.Remix };
        const float gap = 6f;
        float width = (rect.width - gap * 2f) / 3f;
        for (int i = 0; i < modes.Length; i++)
        {
            SqueakVoicePackMode mode = modes[i];
            if (SqueakySettingsUI.Tab(new Rect(rect.x + i * (width + gap), rect.y, width, rect.height),
                    ("SR.VoicePack.Mode." + mode).Translate(), voicePackMode == mode)) RequestVoicePackMode(mode);
        }
    }

    private static string InstalledVoicePackCompactSummary(SqueakXenotypeCatalogSnapshot catalog)
    {
        int xenotype = 0;
        foreach (IReadOnlyList<SqueakVoicePackDef> packs in catalog.XenotypePacksByDefName.Values) xenotype += packs.Count;
        return "SR.VoiceSource.InstalledCompact".Translate(catalog.RacePacks.Count, xenotype);
    }

    private void DrawXenotypeAssignment(Rect content)
    {
        SqueakXenotypeCatalogSnapshot catalog = SqueakXenotypeCatalog.Current;
        EnsureXenotypeRowCache(catalog);
        bool narrow = content.width < 760f;
        if (narrow)
        {
            SqueakySettingsUI.PanelFrame(content, SqueakySurfaceKind.Raised);
            GUI.BeginGroup(content);
            Rect local = ContractedPositive(new Rect(0f, 0f, content.width, content.height), 7f);
            if (xenotypeNarrowEditorStep && (selectedTargetScope == SqueakVoicePackScope.Race || xenotypeDraft != null))
                DrawNarrowXenotypeEditor(local, catalog);
            else DrawXenotypeList(local, catalog);
            GUI.EndGroup();
            return;
        }
        float desiredLeftWidth = Mathf.Clamp(content.width * .38f, 260f, 390f);
        float leftWidth = Mathf.Max(1f, Mathf.Min(desiredLeftWidth, content.width - 12f - 420f));
        Rect left = new(content.x, content.y, leftWidth, content.height);
        Rect right = new(left.xMax + 12f, content.y, Mathf.Max(0f, content.width - leftWidth - 12f), content.height);
        if (left.width < 20f || left.height < 20f || right.width < 20f || right.height < 20f) return;
        SqueakySettingsUI.PanelFrame(left, SqueakySurfaceKind.Raised);
        GUI.BeginGroup(left);
        DrawXenotypeList(ContractedPositive(new Rect(0f, 0f, left.width, left.height), 6f), catalog);
        GUI.EndGroup();
        SqueakySettingsUI.PanelFrame(right, SqueakySurfaceKind.Raised);
        GUI.BeginGroup(right);
        DrawXenotypeEditor(ContractedPositive(new Rect(0f, 0f, right.width, right.height), 8f), catalog);
        GUI.EndGroup();

    }

    private void DrawNarrowXenotypeEditor(Rect rect, SqueakXenotypeCatalogSnapshot catalog)
    {
        Rect back = new(rect.x, rect.y, Mathf.Min(190f, rect.width), 32f);
        if (SqueakySettingsUI.Button(back, "SR.Xeno.BackToTargets".Translate(), SqueakyButtonKind.Secondary))
        {
            CommitAndFlushContinuousXenotypeEdit();
            xenotypeNarrowEditorStep = false;
            return;
        }
        Rect editor = new(rect.x, back.yMax + 7f, rect.width, Mathf.Max(1f, rect.yMax - back.yMax - 7f));
        DrawXenotypeEditor(editor, catalog);
    }

    private static Rect ContractedPositive(Rect rect, float amount)
    {
        return new Rect(rect.x + amount, rect.y + amount,
            Mathf.Max(1f, rect.width - amount * 2f), Mathf.Max(1f, rect.height - amount * 2f));
    }

    private static void DrawCenteredStatus(Rect rect, string text)
    {
        SqueakySettingsUI.EmptyState(rect, text);
    }

    private void DrawXenotypeList(Rect rect, SqueakXenotypeCatalogSnapshot catalog)
    {
        float raceHelpOffset = raceLayerHelpOpen ? MeasureRaceLayerHelpHeight(rect.width) + 6f : 0f;
        float raceHeight = MeasureRaceDefaultLayerHeight(rect.width, catalog);
        Rect raceDefault = new(rect.x, rect.y, rect.width, raceHeight);
        DrawRaceDefaultLayer(raceDefault, catalog);
        if (raceLayerHelpOpen)
        {
            Rect raceHelp = new(rect.x, raceDefault.yMax + 4f, rect.width, raceHelpOffset - 6f);
            SqueakySettingsUI.StatusPanel(raceHelp, "SR.Xeno.RaceDefaultLayer.InlineHelp".Translate(), SqueakySurfaceKind.Base);
        }
        Rect searchRect = new(rect.x, raceDefault.yMax + raceHelpOffset + 8f, rect.width, 30f);
        xenotypeSearch = SqueakySettingsUI.SearchField(searchRect, xenotypeSearch, "SR.Xeno.Search.Hint".Translate());

        EnsureXenotypeRowCache(catalog);
        EnsureXenotypeRowFilter();
        Rect firstChipRow = new(rect.x, searchRect.yMax + 3f, rect.width, 25f);
        float firstChipWidth = (firstChipRow.width - 6f) / 3f;
        if (SqueakySettingsUI.FilterChip(new Rect(firstChipRow.x, firstChipRow.y, firstChipWidth, 25f), "SR.Xeno.Filter.Configured".Translate(), filterConfigured)) { filterConfigured = !filterConfigured; xenotypeRowFilterQuery = null; }
        if (SqueakySettingsUI.FilterChip(new Rect(firstChipRow.x + firstChipWidth + 3f, firstChipRow.y, firstChipWidth, 25f), "SR.Xeno.Filter.Candidates".Translate(), filterCandidates)) { filterCandidates = !filterCandidates; xenotypeRowFilterQuery = null; }
        if (SqueakySettingsUI.FilterChip(new Rect(firstChipRow.x + (firstChipWidth + 3f) * 2f, firstChipRow.y, firstChipWidth, 25f), "SR.Xeno.Filter.Enabled".Translate(), filterEnabled)) { filterEnabled = !filterEnabled; xenotypeRowFilterQuery = null; }
        Rect secondChipRow = new(rect.x, firstChipRow.yMax + 3f, rect.width, 25f);
        string orphanLabel = "SR.Xeno.Filter.Orphan".Translate();
        string resultLabel = "SR.Xeno.Search.Results".Translate(filteredXenotypeRows.Count, sortedXenotypeRows.Count);
        bool inlineResult = CanInlineXenotypeResult(secondChipRow.width, orphanLabel, resultLabel);
        Rect orphanRect = inlineResult ? new Rect(secondChipRow.x, secondChipRow.y, secondChipRow.width * .58f, secondChipRow.height) : secondChipRow;
        if (SqueakySettingsUI.FilterChip(orphanRect, orphanLabel, filterOrphan)) { filterOrphan = !filterOrphan; xenotypeRowFilterQuery = null; }
        Rect countRect = inlineResult
            ? new Rect(orphanRect.xMax + 5f, secondChipRow.y, Mathf.Max(1f, secondChipRow.xMax - orphanRect.xMax - 5f), secondChipRow.height)
            : new Rect(rect.x, secondChipRow.yMax + 2f, rect.width, 22f);
        Color oldCount = GUI.color; GUI.color = Color.gray; Text.Font = GameFont.Tiny;
        TextAnchor oldAnchor = Text.Anchor;
        Text.Anchor = inlineResult ? TextAnchor.MiddleRight : TextAnchor.UpperLeft;
        Widgets.Label(countRect, resultLabel);
        Text.Anchor = oldAnchor;
        Text.Font = GameFont.Small; GUI.color = oldCount;

        float listY = (inlineResult ? secondChipRow : countRect).yMax + 3f;
        Rect listRect = new(rect.x, listY, Mathf.Max(1f, rect.width), Mathf.Max(1f, rect.yMax - listY));
        float listContentHeight = filteredXenotypeRows.Count * 82f;
        xenotypeListScroll.y = Mathf.Clamp(xenotypeListScroll.y, 0f, Mathf.Max(0f, listContentHeight - listRect.height));
        float listWidth = listContentHeight > listRect.height ? listRect.width - 16f : listRect.width;
        Rect view = new(0f, 0f, Mathf.Max(1f, listWidth), Mathf.Max(listRect.height, listContentHeight));
        Widgets.BeginScrollView(listRect, ref xenotypeListScroll, view);
        float y = 0f;
        foreach (XenotypeRowSummary summary in filteredXenotypeRows)
        {
            Rect item = new(0f, y, view.width, 76f); y += 82f;
            bool selected = selectedTargetScope == SqueakVoicePackScope.Xenotype && selectedTargetName == summary.DefName;
            Widgets.DrawBoxSolid(item, selected ? SqueakySettingsUI.Selected : Mouse.IsOver(item) ? SqueakySettingsUI.Raised : SqueakySettingsUI.Panel);
            SqueakySettingsUI.DrawBorder(item);
            if (selected) Widgets.DrawBoxSolid(new Rect(item.x + 1f, item.y + 1f, 3f, item.height - 2f), SqueakySettingsUI.Gold);
            Texture2D icon = summary.Candidate.Canonical?.Icon ?? BaseContent.BadTex;
            GUI.DrawTexture(new Rect(item.x + 6f, item.y + 7f, 42f, 42f), icon, ScaleMode.ScaleToFit);
            Text.Font = GameFont.Small;
            Rect helpRect = new(item.xMax - SqueakySettingsUI.HelpSize - 7f, item.y + 6f, SqueakySettingsUI.HelpSize, SqueakySettingsUI.HelpSize);
            Rect nameRect = new(item.x + 56f, item.y + 5f, helpRect.x - item.x - 62f, 25f);
            string displayName = summary.DisplayName;
            DrawClippedLabel(nameRect, displayName, displayName, false);
            Text.Font = GameFont.Tiny; Color old = GUI.color; GUI.color = Color.gray;
            Rect sourceRect = new(item.x + 56f, item.y + 28f, item.width - 62f, 18f);
            string source = summary.SourceLabel + " · " + summary.DefName;
            DrawClippedLabel(sourceRect, source, source, false);
            Rect detailRect = new(item.x + 56f, item.y + 48f, item.width - 62f, 22f);
            bool current = selectedXenotype == summary.DefName && xenotypeDraft != null;
            bool hasBehavior = current ? HasBehaviorOverride(xenotypeDraft!) : summary.HasBehaviorOverride;
            int enabled = summary.EnabledPackCount;
            int orphan = summary.OrphanCount;
            string availability = summary.HasCanonicalConflict ? "SR.Xeno.Status.Conflict".Translate()
                : summary.IsDormant ? "SR.Xeno.Status.Dormant".Translate()
                : summary.TargetUnavailable ? "SR.Xeno.Status.TargetUnavailable".Translate()
                : summary.Candidate.IsHarHint ? "SR.Xeno.Status.HarHint".Translate() : "";
            string detail = BehaviorStatus(hasBehavior) + " · " + AudioStatus(enabled, summary.CandidateCount)
                + (orphan > 0 ? " · " + "SR.Xeno.Status.Orphan".Translate(orphan) : "")
                + (availability.Length > 0 ? " · " + availability : "");
            SqueakySettingsUI.EllipsizedLabel(detailRect, detail, detail, false); GUI.color = old; Text.Font = GameFont.Small;
            string itemHelp = "SR.Xeno.Target.Tooltip.Short".Translate(summary.DefName, source, detail);
            SqueakySettingsUI.HelpIndicator(helpRect, itemHelp);
            Rect itemClick = new(item.x, item.y, helpRect.x - item.x - 2f, item.height);
            if (Widgets.ButtonInvisible(itemClick)) SelectVoicePackTarget(summary);
        }
        Widgets.EndScrollView();
    }

    private static bool CanInlineXenotypeResult(float width, string filter, string result)
    {
        GameFont oldFont = Text.Font;
        Text.Font = GameFont.Small;
        float filterWidth = Text.CalcSize(filter).x + 22f;
        Text.Font = GameFont.Tiny;
        float resultWidth = Text.CalcSize(result).x + 8f;
        Text.Font = oldFont;
        return filterWidth + resultWidth + 5f <= width;
    }

    private static float MeasureRaceLayerHelpHeight(float width)
    {
        return MeasureInlinePanelHeight("SR.Xeno.RaceDefaultLayer.InlineHelp".Translate(), width);
    }

    private float MeasureRaceDefaultLayerHeight(float width, SqueakXenotypeCatalogSnapshot catalog)
    {
        const float horizontalInset = 12f;
        const float verticalInset = 5f;
        float textWidth = Mathf.Max(1f, width - horizontalInset * 2f - SqueakySettingsUI.HelpSize - 8f);
        GameFont oldFont = Text.Font;
        Text.Font = GameFont.Small;
        float titleHeight = Text.CalcHeight("SR.Xeno.RaceDefaultLayer".Translate(), textWidth);
        Text.Font = GameFont.Tiny;
        SqueakVoicePackDomainStatus status = GetVoicePackSelectionStatus(SqueakVoicePackScope.Race, "");
        string detail = "SR.Xeno.RaceDefaultLayer.Desc".Translate(status.EnabledKeys.Count, catalog.RacePacks.Count);
        float detailHeight = Text.CalcHeight(detail, textWidth);
        Text.Font = oldFont;
        return verticalInset * 2f + titleHeight + 1f + detailHeight;
    }

    private static float MeasureInlinePanelHeight(string text, float width)
    {
        GameFont oldFont = Text.Font;
        Text.Font = GameFont.Tiny;
        float height = Mathf.Max(58f, Text.CalcHeight(text, Mathf.Max(1f, width - 16f)) + 16f);
        Text.Font = oldFont;
        return height;
    }

    private void DrawRaceDefaultLayer(Rect rect, SqueakXenotypeCatalogSnapshot catalog)
    {
        bool selected = selectedTargetScope == SqueakVoicePackScope.Race;
        Widgets.DrawBoxSolid(rect, selected ? SqueakySettingsUI.Selected : Mouse.IsOver(rect) ? SqueakySettingsUI.Raised : SqueakySettingsUI.Panel);
        SqueakySettingsUI.DrawBorder(rect);
        Widgets.DrawBoxSolid(new Rect(rect.x + 1f, rect.y + 1f, 4f, rect.height - 2f), selected ? SqueakySettingsUI.Gold : SqueakySettingsUI.Border);
        Text.Font = GameFont.Small;
        Rect help = new(rect.xMax - SqueakySettingsUI.HelpSize - 8f, rect.y + 5f, SqueakySettingsUI.HelpSize, SqueakySettingsUI.HelpSize);
        float textWidth = Mathf.Max(1f, help.x - rect.x - 18f);
        float titleHeight = Text.CalcHeight("SR.Xeno.RaceDefaultLayer".Translate(), textWidth);
        Widgets.Label(new Rect(rect.x + 12f, rect.y + 5f, textWidth, titleHeight), "SR.Xeno.RaceDefaultLayer".Translate());
        Text.Font = GameFont.Tiny; Color old = GUI.color; GUI.color = Color.gray;
        SqueakVoicePackDomainStatus status = GetVoicePackSelectionStatus(SqueakVoicePackScope.Race, "");
        string detail = "SR.Xeno.RaceDefaultLayer.Desc".Translate(status.EnabledKeys.Count, catalog.RacePacks.Count);
        float detailHeight = Text.CalcHeight(detail, textWidth);
        Widgets.Label(new Rect(rect.x + 12f, rect.y + 6f + titleHeight, textWidth, detailHeight), detail);
        if (SqueakySettingsUI.HelpToggle(help, raceLayerHelpOpen)) raceLayerHelpOpen = !raceLayerHelpOpen;
        GUI.color = old; Text.Font = GameFont.Small;
        if (Widgets.ButtonInvisible(new Rect(rect.x, rect.y, help.x - rect.x - 2f, rect.height))) SelectRaceVoicePackTarget();
    }

    private void SelectRaceVoicePackTarget()
    {
        if (selectedTargetScope == SqueakVoicePackScope.Race)
        {
            xenotypeNarrowEditorStep = true;
            return;
        }
        CommitAndFlushContinuousXenotypeEdit();
        selectedTargetScope = SqueakVoicePackScope.Race;
        selectedTargetName = "";
        selectedXenotype = null;
        xenotypeDraft = null;
        xenotypeEditorScroll = Vector2.zero;
        xenotypeNarrowEditorStep = true;
    }

    private static bool ContainsIgnoreCase(string value, string query) => value?.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0;
    private static string GetXenotypeSourceName(XenotypeDef xeno) => xeno.modContentPack?.Name ?? xeno.modContentPack?.PackageId ?? "—";

    private void EnsureXenotypeRowCache(SqueakXenotypeCatalogSnapshot catalog)
    {
        object? language = LanguageDatabase.activeLanguage;
        if (ReferenceEquals(xenotypeRowCacheCatalog, catalog) && ReferenceEquals(xenotypeRowCacheLanguage, language)) return;
        xenotypeRowCacheCatalog = catalog;
        xenotypeRowCacheLanguage = language;
        xenotypeRowSummaries.Clear();
        sortedXenotypeRows.Clear();

        foreach (SqueakXenotypeTargetCandidate candidate in catalog.GetTargetCandidates(voicePackSelections, xenotypePresets))
        {
            XenotypeDef? xeno = candidate.Canonical;
            string sourceName = xeno != null ? GetXenotypeSourceName(xeno)
                : (candidate.HasCanonicalConflict ? "SR.Xeno.Source.Conflict" : "SR.Xeno.Source.Unloaded").Translate();
            XenotypeRowSummary summary = new()
            {
                Candidate = candidate,
                DefName = candidate.DefName,
                DisplayName = xeno?.LabelCap.ToString() ?? candidate.DefName,
                SourceName = sourceName,
                TargetUnavailable = ModsConfig.BiotechActive && xeno == null && !candidate.HasCanonicalConflict,
                HasCanonicalConflict = candidate.HasCanonicalConflict,
                IsDormant = !ModsConfig.BiotechActive
            };
            summary.SourceLabel = "SR.Xeno.ScopeAndSource".Translate("SR.Xeno.Scope.Xenotype".Translate(), summary.SourceName);
            string packageId = xeno?.modContentPack?.PackageId ?? "";
            summary.SearchText = summary.DisplayName + "\n" + summary.DefName + "\n" + summary.SourceName + "\n" + packageId;
            if (catalog.XenotypePacksByDefName.TryGetValue(summary.DefName, out IReadOnlyList<SqueakVoicePackDef>? packs))
            {
                summary.CandidateCount = packs.Count;
                summary.SearchText += "\n" + string.Join("\n", packs.Select(PackSearchText));
            }

            foreach (XenotypePresetRecord record in xenotypePresets)
            {
                if (record == null || record.xenotypeDefName != summary.DefName) continue;
                if (record.hasOverallIntervalMultiplier || (record.actionOverrides?.Any(HasBehaviorOverride) ?? false) || (record.moodOverrides?.Any(HasBehaviorOverride) ?? false)) summary.HasBehaviorOverride = true;
            }

            SqueakVoicePackDomainStatus status = GetVoicePackSelectionStatus(SqueakVoicePackScope.Xenotype, summary.DefName);
            if (status.EnabledKeys != null)
            {
                HashSet<string> distinct = new(status.EnabledKeys.Where(x => !string.IsNullOrWhiteSpace(x)), StringComparer.Ordinal);
                foreach (string key in distinct)
                {
                    if (catalog.PackByKey.TryGetValue(key, out SqueakVoicePackDef? pack) && pack.scope == SqueakVoicePackScope.Xenotype && pack.targetDefName == summary.DefName) summary.EnabledPackCount++;
                    else if (status.State == SqueakVoicePackDomainState.Orphan) summary.OrphanCount++;
                }
            }
            xenotypeRowSummaries[summary.DefName] = summary;
            sortedXenotypeRows.Add(summary);
        }
        sortedXenotypeRows.Sort((left, right) =>
        {
            int result = StringComparer.CurrentCultureIgnoreCase.Compare(left.DisplayName, right.DisplayName);
            if (result == 0) result = StringComparer.CurrentCultureIgnoreCase.Compare(left.SourceName, right.SourceName);
            return result != 0 ? result : StringComparer.OrdinalIgnoreCase.Compare(left.DefName, right.DefName);
        });
        xenotypeRowFilterQuery = null;
    }

    private void EnsureXenotypeRowFilter()
    {
        string query = (xenotypeSearch ?? "").Trim();
        string signature = query + "|" + filterConfigured + filterCandidates + filterEnabled + filterOrphan;
        if (string.Equals(xenotypeRowFilterQuery, signature, StringComparison.Ordinal)) return;
        xenotypeRowFilterQuery = signature;
        filteredXenotypeRows.Clear();
        foreach (XenotypeRowSummary summary in sortedXenotypeRows)
            if ((query.Length == 0 || ContainsIgnoreCase(summary.SearchText, query))
                && (!filterConfigured || summary.HasBehaviorOverride || summary.EnabledPackCount > 0 || summary.OrphanCount > 0)
                && (!filterCandidates || summary.CandidateCount > 0)
                && (!filterEnabled || summary.EnabledPackCount > 0)
                && (!filterOrphan || summary.OrphanCount > 0 || summary.TargetUnavailable || summary.HasCanonicalConflict || summary.IsDormant)) filteredXenotypeRows.Add(summary);
    }

    private void InvalidateXenotypeRowCache()
    {
        xenotypeRowCacheCatalog = null;
        xenotypeRowCacheLanguage = null;
        xenotypeRowFilterQuery = null;
    }

    private static string BehaviorStatus(XenotypePresetDraft state)
    {
        return BehaviorStatus(HasBehaviorOverride(state));
    }

    private static string BehaviorStatus(bool changed) => (changed ? "SR.Xeno.Status.BehaviorOverride" : "SR.Xeno.Status.BehaviorInherited").Translate();
    private static bool HasBehaviorOverride(XenotypePresetDraft state) => state.HasOverall || state.Actions.Values.Any(HasBehaviorOverride) || state.Moods.Values.Any(HasBehaviorOverride);
    private static bool HasBehaviorOverride(XenotypeActionBehaviorOverride value) => value.hasEnabled || value.hasIntervalMultiplier || value.hasProbabilityMultiplier;
    private static bool HasBehaviorOverride(XenotypeMoodOverride value) => value.hasPitchFactor || value.hasVolumeFactor || value.hasPitchJitter;

    private static string AudioStatus(int enabledPoolCount, int candidateCount)
    {
        if (enabledPoolCount > 0) return "SR.Xeno.Status.PacksEnabled".Translate(enabledPoolCount);
        return (candidateCount > 0 ? "SR.Xeno.Status.CandidatesDefault" : "SR.Xeno.Status.NoCandidatesDefault").Translate();
    }

    private void SelectXenotype(string defName)
    {
        if (selectedTargetScope == SqueakVoicePackScope.Xenotype && selectedTargetName == defName && selectedXenotype == defName)
        {
            xenotypeNarrowEditorStep = true;
            return;
        }
        CommitAndFlushContinuousXenotypeEdit();
        selectedTargetScope = SqueakVoicePackScope.Xenotype;
        selectedTargetName = defName;
        selectedXenotype = defName;
        RebuildXenotypeDraft();
        xenotypeEditorScroll = Vector2.zero;
        xenotypeNarrowEditorStep = true;
    }

    private void SelectVoicePackTarget(XenotypeRowSummary summary)
    {
        SelectXenotype(summary.DefName);
    }

    private void RebuildXenotypeDraft()
    {
        xenotypeNumericBuffers.Clear();
        if (selectedXenotype == null) { xenotypeDraft = null; return; }
        SqueakXenotypeCatalogSnapshot catalog = SqueakXenotypeCatalog.Current;
        xenotypeDraft = XenotypePresetDraft.FromRecords(xenotypePresets, selectedXenotype, ConfiguredActions, ConfiguredMoods);
    }

    private void DrawXenotypeEditor(Rect rect, SqueakXenotypeCatalogSnapshot catalog)
    {
        rect.width = Mathf.Max(1f, rect.width);
        rect.height = Mathf.Max(1f, rect.height);
        if (selectedTargetScope == SqueakVoicePackScope.Race)
        {
            DrawRaceVoicePackEditor(rect, catalog);
            return;
        }
        if (xenotypeDraft == null) { DrawCenteredStatus(rect, "SR.Xeno.SelectPrompt".Translate()); return; }
        catalog.XenotypePacksByDefName.TryGetValue(xenotypeDraft.XenotypeDefName, out IReadOnlyList<SqueakVoicePackDef>? packs);
        xenotypeRowSummaries.TryGetValue(xenotypeDraft.XenotypeDefName, out XenotypeRowSummary? current);
        bool filteredOut = current != null && !filteredXenotypeRows.Contains(current);
        float top = rect.y;
        Rect identity = new(rect.x, top, rect.width, 48f);
        SqueakySettingsUI.PanelFrame(identity, SqueakySurfaceKind.Base);
        string displayName = current?.DisplayName ?? xenotypeDraft.XenotypeDefName;
        Widgets.Label(new Rect(identity.x + 9f, identity.y + 3f, identity.width - 18f, 24f), displayName);
        Color identityColor = GUI.color; GameFont identityFont = Text.Font;
        GUI.color = SqueakySettingsUI.Muted; Text.Font = GameFont.Tiny;
        DrawClippedLabel(new Rect(identity.x + 9f, identity.y + 26f, identity.width - 18f, 18f), xenotypeDraft.XenotypeDefName, xenotypeDraft.XenotypeDefName, false);
        Text.Font = identityFont; GUI.color = identityColor;
        top = identity.yMax + 6f;
        if (filteredOut)
        {
            string noticeText = "SR.Xeno.Search.CurrentHidden".Translate(current!.DisplayName);
            const float noticePadding = 7f;
            float noticeContentWidth = Mathf.Max(1f, rect.width - noticePadding * 2f);
            float noticeHeight = Mathf.Max(38f, Text.CalcHeight(noticeText, noticeContentWidth) + noticePadding * 2f);
            Rect notice = new(rect.x, top, rect.width, noticeHeight); SqueakySettingsUI.PanelFrame(notice, SqueakySurfaceKind.Warning);
            Widgets.Label(notice.ContractedBy(noticePadding), noticeText); top = notice.yMax + 6f;
        }
        if (current != null && (current.HasCanonicalConflict || current.TargetUnavailable || current.IsDormant))
        {
            string stateText = current.HasCanonicalConflict ? "SR.Xeno.ConflictNotice".Translate(current.DefName)
                : current.IsDormant ? "SR.Xeno.DormantNotice".Translate(current.DefName)
                : "SR.Xeno.TargetUnavailableNotice".Translate(current.DefName);
            float stateHeight = Mathf.Max(42f, Text.CalcHeight(stateText, Mathf.Max(1f, rect.width - 14f)) + 14f);
            Rect state = new(rect.x, top, rect.width, stateHeight);
            SqueakySettingsUI.StatusPanel(state, stateText, SqueakySurfaceKind.Warning);
            top = state.yMax + 6f;
            if (current.TargetUnavailable)
            {
                Rect forget = new(rect.x, top, rect.width, 34f);
                if (SqueakySettingsUI.Button(forget, "SR.Xeno.ForgetTarget".Translate(), SqueakyButtonKind.Danger))
                    RequestForgetUnavailableTarget(current.DefName);
                top = forget.yMax + 6f;
            }
        }
        Rect tabs = new(rect.x, top, rect.width, 40f); SqueakySettingsUI.PanelFrame(tabs, SqueakySurfaceKind.Base);
        Rect tabInner = tabs.ContractedBy(5f); float tabWidth = (tabInner.width - 6f) * .5f;
        if (SqueakySettingsUI.Tab(new Rect(tabInner.x, tabInner.y, tabWidth, 30f), "SR.Xeno.Tab.Behavior".Translate(), xenotypeEditorTab == XenotypeEditorTab.Behavior)) SelectXenotypeEditorTab(XenotypeEditorTab.Behavior);
        if (SqueakySettingsUI.Tab(new Rect(tabInner.x + tabWidth + 6f, tabInner.y, tabWidth, 30f), "SR.Xeno.Tab.AudioPacks".Translate(), xenotypeEditorTab == XenotypeEditorTab.AudioPacks)) SelectXenotypeEditorTab(XenotypeEditorTab.AudioPacks);
        rect.yMin = tabs.yMax + 6f;
        rect.height = Mathf.Max(1f, rect.height);
        float viewWidth = rect.width;
        float contentHeight = MeasureXenotypeEditorContentHeight(viewWidth, packs ?? Array.Empty<SqueakVoicePackDef>(),
            audioPackSearch, current?.HasCanonicalConflict == true);
        if (contentHeight > rect.height)
        {
            viewWidth = Mathf.Max(1f, rect.width - 16f);
            contentHeight = MeasureXenotypeEditorContentHeight(viewWidth, packs ?? Array.Empty<SqueakVoicePackDef>(),
                audioPackSearch, current?.HasCanonicalConflict == true);
        }
        xenotypeEditorScroll.y = Mathf.Clamp(xenotypeEditorScroll.y, 0f, Mathf.Max(0f, contentHeight - rect.height));
        Rect view = new(0f, 0f, viewWidth, Mathf.Max(rect.height, contentHeight));
        Widgets.BeginScrollView(rect, ref xenotypeEditorScroll, view);
        Listing_Standard list = new(); list.maxOneColumn = true; list.Begin(view);
        if (xenotypeEditorTab == XenotypeEditorTab.Behavior)
        {
            bool changed = false;
            SqueakySettingsUI.LabelWithHelp(list.GetRect(Mathf.Max(28f, Text.CalcHeight("SR.Xeno.BehaviorSummary.Short".Translate(BehaviorStatus(xenotypeDraft)), list.ColumnWidth - 24f))),
                "SR.Xeno.BehaviorSummary.Short".Translate(BehaviorStatus(xenotypeDraft)), "SR.Xeno.BehaviorSummary.Tooltip.Short".Translate());
            list.GapLine(); DrawSectionHeader(list, "SR.Xeno.Overall".Translate());
            changed |= DrawOptionalNumber(list, "overall", "SR.Xeno.Interval".Translate(), ref xenotypeDraft.HasOverall, ref xenotypeDraft.Overall, 0f, 5f);
            DrawSectionHeader(list, "SR.Xeno.Actions".Translate());
            behaviorActionSearch = SqueakySettingsUI.SearchField(list.GetRect(30f), behaviorActionSearch, "SR.Xeno.Search.Actions".Translate());
            foreach (SqueakAction action in ConfiguredActions.Where(action => behaviorActionSearch.Trim().Length == 0 || ContainsIgnoreCase(SqueakLabels.Action(action), behaviorActionSearch.Trim()) || ContainsIgnoreCase(action.ToString(), behaviorActionSearch.Trim()))) changed |= DrawActionEditor(list, action, xenotypeDraft.Actions[action]);
            DrawSectionHeader(list, "SR.Xeno.Moods".Translate());
            foreach (SqueakMood mood in ConfiguredMoods) changed |= DrawMoodEditor(list, mood, xenotypeDraft.Moods[mood]);
            if (changed)
            {
                xenotypeDraft.MarkChanged();
                CommitXenotypeEditorNow();
            }
        }
        else
        {
            DrawVoicePackDomain(list, SqueakVoicePackScope.Xenotype, xenotypeDraft.XenotypeDefName,
                packs ?? Array.Empty<SqueakVoicePackDef>(), ref audioPackSearch, current?.HasCanonicalConflict == true);
        }
        list.End(); Widgets.EndScrollView();
    }

    private void RequestForgetUnavailableTarget(string defName)
    {
        Find.WindowStack.Add(new Dialog_SqueakyCompactMessageBox(
            "SR.Xeno.ForgetTarget.Confirm".Translate(defName), "SR.Xeno.ForgetTarget".Translate(),
            () => ConfirmForgetUnavailableTarget(defName), "SR.Common.Cancel".Translate(), null,
            "SR.Xeno.ForgetTarget.Title".Translate(), SqueakyButtonKind.Danger, SqueakyButtonKind.Secondary,
            reverseButtons: true, inputDelayFrames: 2));
    }

    private void ConfirmForgetUnavailableTarget(string defName)
    {
        if (!string.Equals(selectedXenotype, defName, StringComparison.Ordinal)) return;
        xenotypeDraft = null;
        ForgetXenotypeTarget(defName);
        selectedXenotype = null;
        selectedTargetName = "";
        xenotypeNarrowEditorStep = false;
        xenotypeEditorScroll = Vector2.zero;
        InvalidateXenotypeRowCache();
    }

    private float MeasureXenotypeEditorContentHeight(float width, IReadOnlyList<SqueakVoicePackDef> packs,
        string packSearch, bool conflict)
    {
        if (xenotypeEditorTab == XenotypeEditorTab.AudioPacks)
            return MeasureVoicePackDomainHeight(width, SqueakVoicePackScope.Xenotype, xenotypeDraft!.XenotypeDefName,
                packs, packSearch, conflict) + 42f;

        float height = Mathf.Max(28f, Text.CalcHeight("SR.Xeno.BehaviorSummary.Short".Translate(BehaviorStatus(xenotypeDraft!)), width - 24f));
        height += 12f + 34f + OptionalNumberRowHeight(width) + 34f + 30f;
        string query = behaviorActionSearch.Trim();
        foreach (SqueakAction action in ConfiguredActions)
        {
            if (query.Length > 0 && !ContainsIgnoreCase(SqueakLabels.Action(action), query) && !ContainsIgnoreCase(action.ToString(), query)) continue;
            SqueakActionConfig? cfg = GetActionConfig(action);
            height += Text.CalcHeight(SqueakLabels.Action(action) + "  ·  " + "SR.Xeno.TriggerMode".Translate(cfg?.mode.ToString() ?? "—"), width) + 2f;
            height += (width < 520f ? 58f : 28f) + OptionalNumberRowHeight(width);
            if (cfg?.mode == SqueakTriggerMode.RandomOneShot) height += OptionalNumberRowHeight(width);
            height += 12f;
        }
        height += 34f;
        foreach (SqueakMood mood in ConfiguredMoods)
            height += Text.CalcHeight(SqueakLabels.Mood(mood), width) + 2f + OptionalNumberRowHeight(width) * 2f + 28f + 60f + 12f;
        return height + 28f;
    }

    private float MeasureVoicePackDomainHeight(float width, SqueakVoicePackScope scope, string target,
        IReadOnlyList<SqueakVoicePackDef> packs, string search, bool conflict)
    {
        float height = 28f;
        SqueakVoicePackDomainStatus status = GetVoicePackSelectionStatus(scope, target);
        int selectedCount = status.EnabledKeys?.Count ?? 0;
        if (conflict && selectedCount > 0)
        {
            float textWidth = Mathf.Max(1f, width - 8f - 8f - SqueakySettingsUI.HelpSize - 6f - 110f - 6f);
            height += Mathf.Max(44f, Text.CalcHeight("SR.Xeno.ConflictSelectionRetained".Translate(), textWidth) + 10f);
            if (conflictRecoveryHelpOpen)
                height += MeasureInlinePanelHeight("SR.Xeno.ConflictSelectionRetained.InlineHelp".Translate(), width) + 6f;
        }
        if (voicePackMode == SqueakVoicePackMode.Off) height += 44f;
        height += 30f;
        int shownCount = CountShownVoicePacks(packs, search);
        height += shownCount > 0 ? shownCount * 74f : 30f;
        int unavailable = CountUnavailablePackKeys(status.EnabledKeys, packs);
        if (unavailable > 0)
        {
            string stateKey = status.State == SqueakVoicePackDomainState.Dormant ? "SR.VoicePack.Dormant"
                : status.State == SqueakVoicePackDomainState.TargetUnavailable ? "SR.VoicePack.TargetUnavailable"
                : "SR.VoicePack.Orphan";
            float textWidth = Mathf.Max(1f, width - 8f - 8f - SqueakySettingsUI.HelpSize - 6f - 110f - 6f);
            height += Mathf.Max(42f, Text.CalcHeight(stateKey.Translate(unavailable), textWidth) + 12f);
        }
        return height + 24f;
    }

    private static int CountShownVoicePacks(IReadOnlyList<SqueakVoicePackDef> packs, string search)
    {
        string query = (search ?? "").Trim();
        int count = 0;
        for (int i = 0; i < packs.Count; i++)
            if (VoicePackMatchesSearch(packs[i], query)) count++;
        return count;
    }

    private static bool VoicePackMatchesSearch(SqueakVoicePackDef pack, string query)
    {
        if (query.Length == 0) return true;
        string label = pack.LabelCap.NullOrEmpty() ? pack.defName : pack.LabelCap.ToString();
        return ContainsIgnoreCase(label, query)
            || ContainsIgnoreCase(pack.modContentPack?.Name ?? "", query)
            || ContainsIgnoreCase(pack.modContentPack?.ModMetaData?.AuthorsString ?? "", query)
            || ContainsIgnoreCase(pack.defName ?? "", query)
            || ContainsIgnoreCase(PackKey(pack), query)
            || ContainsIgnoreCase(pack.modContentPack?.PackageId ?? "", query);
    }

    private static int CountUnavailablePackKeys(IReadOnlyList<string>? selectedKeys, IReadOnlyList<SqueakVoicePackDef> packs)
    {
        if (selectedKeys == null) return 0;
        int count = 0;
        for (int i = 0; i < selectedKeys.Count; i++)
            if (!ContainsPackKey(packs, selectedKeys[i])) count++;
        return count;
    }

    private static bool ContainsPackKey(IReadOnlyList<SqueakVoicePackDef> packs, string key)
    {
        for (int i = 0; i < packs.Count; i++)
            if (string.Equals(PackKey(packs[i]), key, StringComparison.Ordinal)) return true;
        return false;
    }

    private static bool ContainsOrdinal(IReadOnlyList<string> values, string value)
    {
        for (int i = 0; i < values.Count; i++)
            if (string.Equals(values[i], value, StringComparison.Ordinal)) return true;
        return false;
    }

    private static void RemoveOrdinal(List<string> values, string value)
    {
        for (int i = values.Count - 1; i >= 0; i--)
            if (string.Equals(values[i], value, StringComparison.Ordinal)) values.RemoveAt(i);
    }

    private void DrawRaceVoicePackEditor(Rect rect, SqueakXenotypeCatalogSnapshot catalog)
    {
        rect.width = Mathf.Max(1f, rect.width);
        rect.height = Mathf.Max(1f, rect.height);
        float viewWidth = rect.width;
        float contentHeight = Mathf.Max(28f, Text.CalcHeight("SR.Xeno.RaceSummary.Short".Translate(), viewWidth - 24f))
            + MeasureVoicePackDomainHeight(viewWidth, SqueakVoicePackScope.Race, "", catalog.RacePacks, racePackSearch, false) + 24f;
        if (contentHeight > rect.height)
        {
            viewWidth = Mathf.Max(1f, rect.width - 16f);
            contentHeight = Mathf.Max(28f, Text.CalcHeight("SR.Xeno.RaceSummary.Short".Translate(), viewWidth - 24f))
                + MeasureVoicePackDomainHeight(viewWidth, SqueakVoicePackScope.Race, "", catalog.RacePacks, racePackSearch, false) + 24f;
        }
        xenotypeEditorScroll.y = Mathf.Clamp(xenotypeEditorScroll.y, 0f, Mathf.Max(0f, contentHeight - rect.height));
        Rect view = new(0f, 0f, viewWidth, Mathf.Max(rect.height, contentHeight));
        Widgets.BeginScrollView(rect, ref xenotypeEditorScroll, view);
        Listing_Standard list = new(); list.maxOneColumn = true; list.Begin(view);
        SqueakySettingsUI.LabelWithHelp(list.GetRect(Mathf.Max(28f, Text.CalcHeight("SR.Xeno.RaceSummary.Short".Translate(), list.ColumnWidth - 24f))),
            "SR.Xeno.RaceSummary.Short".Translate(), "SR.Xeno.RaceSummary.Tooltip.Short".Translate());
        DrawVoicePackDomain(list, SqueakVoicePackScope.Race, "", catalog.RacePacks, ref racePackSearch);
        list.End(); Widgets.EndScrollView();
    }

    private void SelectXenotypeEditorTab(XenotypeEditorTab target)
    {
        if (xenotypeEditorTab == target) return;
        CommitAndFlushContinuousXenotypeEdit();
        xenotypeEditorTab = target;
        xenotypeEditorScroll = Vector2.zero;
    }

    private bool DrawActionEditor(Listing_Standard list, SqueakAction action, XenotypeActionBehaviorOverride value)
    {
        bool changed = false;
        SqueakActionConfig? cfg = GetActionConfig(action);
        list.Label(SqueakLabels.Action(action) + "  ·  " + "SR.Xeno.TriggerMode".Translate(cfg?.mode.ToString() ?? "—"));
        bool narrow = list.ColumnWidth < 520f;
        Rect boolRow = list.GetRect(narrow ? 58f : 28f); bool inherit = !value.hasEnabled;
        Rect inheritRect = narrow ? new Rect(boolRow.x, boolRow.y, boolRow.width, 28f) : new Rect(boolRow.x, boolRow.y, boolRow.width * .5f, 28f);
        Rect enabledRect = narrow ? new Rect(boolRow.x, boolRow.y + 30f, boolRow.width, 28f) : new Rect(boolRow.x + boolRow.width * .52f, boolRow.y, boolRow.width * .48f, 28f);
        bool beforeHas = value.hasEnabled, beforeEnabled = value.enabled;
        SqueakySettingsUI.Toggle(inheritRect, "SR.Xeno.InheritEnabled".Translate(), ref inherit);
        value.hasEnabled = !inherit; bool enabled = value.enabled; SqueakySettingsUI.Toggle(enabledRect, "SR.Xeno.Enabled".Translate(), ref enabled, value.hasEnabled); value.enabled = enabled;
        changed |= beforeHas != value.hasEnabled || beforeEnabled != value.enabled;
        changed |= DrawOptionalNumber(list, "a." + action + ".i", "SR.Xeno.ActionInterval".Translate(), ref value.hasIntervalMultiplier, ref value.intervalMultiplier, 0f, 5f);
        if (cfg?.mode == SqueakTriggerMode.RandomOneShot) changed |= DrawOptionalNumber(list, "a." + action + ".p", "SR.Xeno.Probability".Translate(), ref value.hasProbabilityMultiplier, ref value.probabilityMultiplier, 0f, 5f);
        list.GapLine();
        return changed;
    }

    private bool DrawMoodEditor(Listing_Standard list, SqueakMood mood, XenotypeMoodOverride value)
    {
        bool changed = false;
        list.Label(SqueakLabels.Mood(mood));
        changed |= DrawOptionalNumber(list, "m." + mood + ".p", "SR.Workbench.PitchFactor".Translate(), ref value.hasPitchFactor, ref value.pitchFactor, .5f, 2f);
        changed |= DrawOptionalNumber(list, "m." + mood + ".v", "SR.Workbench.VolumeFactor".Translate(), ref value.hasVolumeFactor, ref value.volumeFactor, 0f, 2f);
        Rect presence = list.GetRect(28f); bool inherit = !value.hasPitchJitter, beforeHas = value.hasPitchJitter;
        SqueakySettingsUI.Toggle(presence, "SR.Xeno.InheritJitter".Translate(), ref inherit); value.hasPitchJitter = !inherit;
        changed |= beforeHas != value.hasPitchJitter;
        float jitterMin = value.pitchJitter.min, jitterMax = value.pitchJitter.max;
        changed |= DrawLiveSlider(list.GetRect(30f), "m." + mood + ".j0", "SR.Workbench.JitterMin".Translate(), ref jitterMin, .5f, 1.5f, value.hasPitchJitter);
        changed |= DrawLiveSlider(list.GetRect(30f), "m." + mood + ".j1", "SR.Workbench.JitterMax".Translate(), ref jitterMax, .5f, 1.5f, value.hasPitchJitter);
        if (jitterMax < jitterMin)
        {
            (jitterMin, jitterMax) = (jitterMax, jitterMin);
            xenotypeNumericBuffers.Remove("m." + mood + ".j0");
            xenotypeNumericBuffers.Remove("m." + mood + ".j1");
            changed = true;
        }
        value.pitchJitter = new FloatRange(jitterMin, jitterMax);
        list.GapLine();
        return changed;
    }

    private bool DrawOptionalNumber(Listing_Standard list, string key, string label, ref bool has, ref float value, float min, float max)
    {
        bool beforeHas = has;
        Rect row = list.GetRect(OptionalNumberRowHeight(list.ColumnWidth)); bool inherit = !has;
        bool narrow = row.width < 520f;
        Rect check = narrow ? new Rect(row.x, row.y, row.width, 28f) : new Rect(row.x, row.y, Mathf.Min(132f, row.width * .28f), row.height);
        SqueakySettingsUI.Toggle(check, "SR.Xeno.Inherit".Translate(), ref inherit); has = !inherit;
        bool changed = beforeHas != has;
        if (narrow) return DrawLiveSlider(new Rect(row.x, row.y + 30f, row.width, 30f), key, label, ref value, min, max, has) || changed;
        return DrawLiveSlider(new Rect(check.xMax + 6f, row.y, row.width - check.width - 6f, row.height), key, label, ref value, min, max, has) || changed;
    }

    private static float OptionalNumberRowHeight(float width) => width < 520f ? 62f : 30f;

    private bool DrawLiveSlider(Rect rect, string key, string label, ref float value, float min, float max, bool enabled)
    {
        Widgets.DrawBoxSolid(rect, new Color(.07f, .067f, .061f, .52f));
        Color old = GUI.color; if (!enabled) GUI.color = Color.gray;
        float labelWidth = Mathf.Min(132f, rect.width * .38f), fieldWidth = 58f;
        Widgets.Label(new Rect(rect.x, rect.y, labelWidth, rect.height), label);
        Rect slider = new(rect.x + labelWidth + 5f, rect.y, rect.width - labelWidth - fieldWidth - 10f, rect.height);
        float before = value;
        if (enabled) value = Widgets.HorizontalSlider(slider, SafeClamp(value, min, max, 1f), min, max);
        if (!xenotypeNumericBuffers.TryGetValue(key, out string buffer)) buffer = value.ToString("0.##");
        if (Math.Abs(value - before) > .0001f) buffer = value.ToString("0.##");
        Rect field = new(rect.xMax - fieldWidth, rect.y, fieldWidth, rect.height);
        if (enabled) Widgets.TextFieldNumeric(field, ref value, ref buffer, min, max); else Widgets.Label(field, value.ToString("0.##"));
        xenotypeNumericBuffers[key] = buffer; GUI.color = old;
        value = SafeClamp(value, min, max, 1f);
        return enabled && Math.Abs(value - before) > .0001f;
    }

    private void DrawVoicePackDomain(Listing_Standard list, SqueakVoicePackScope scope, string target,
        IReadOnlyList<SqueakVoicePackDef> packs, ref string search, bool preventNewSelection = false)
    {
        SqueakVoicePackDomainStatus status = GetVoicePackSelectionStatus(scope, target);
        IReadOnlyList<string> selected = status.EnabledKeys ?? Array.Empty<string>();
        int enabledCount = 0;
        for (int i = 0; i < selected.Count; i++)
            if (ContainsPackKey(packs, selected[i])) enabledCount++;
        SqueakySettingsUI.LabelWithHelp(list.GetRect(28f), "SR.VoicePack.DomainSummary".Translate(enabledCount, packs.Count),
            "SR.VoicePack.Domain.Tooltip.Short".Translate());
        if (preventNewSelection && selected.Count > 0)
        {
            string conflictText = "SR.Xeno.ConflictSelectionRetained".Translate();
            float conflictTextWidth = Mathf.Max(1f, list.ColumnWidth - 8f - 8f - SqueakySettingsUI.HelpSize - 6f - 110f - 6f);
            Rect conflictActions = list.GetRect(Mathf.Max(44f, Text.CalcHeight(conflictText, conflictTextWidth) + 10f));
            SqueakySettingsUI.PanelFrame(conflictActions, SqueakySurfaceKind.Warning);
            Rect forgetRect = new(conflictActions.xMax - 116f, conflictActions.y + 5f, 110f, conflictActions.height - 10f);
            Rect helpRect = new(forgetRect.x - 6f - SqueakySettingsUI.HelpSize, conflictActions.y + 7f,
                SqueakySettingsUI.HelpSize, SqueakySettingsUI.HelpSize);
            Widgets.Label(new Rect(conflictActions.x + 8f, conflictActions.y + 5f,
                Mathf.Max(1f, helpRect.x - conflictActions.x - 14f), conflictActions.height - 10f), conflictText);
            if (SqueakySettingsUI.HelpToggle(helpRect, conflictRecoveryHelpOpen)) conflictRecoveryHelpOpen = !conflictRecoveryHelpOpen;
            if (SqueakySettingsUI.Button(forgetRect,
                    "SR.VoicePack.ForgetSelection".Translate(), SqueakyButtonKind.Danger))
            {
                CommitContinuousXenotypeEdit();
                ForgetVoicePackSelection(scope, target); InvalidateXenotypeRowCache();
            }
            if (conflictRecoveryHelpOpen)
            {
                string helpText = "SR.Xeno.ConflictSelectionRetained.InlineHelp".Translate();
                Rect helpPanel = list.GetRect(MeasureInlinePanelHeight(helpText, list.ColumnWidth));
                SqueakySettingsUI.StatusPanel(helpPanel, helpText, SqueakySurfaceKind.Base);
            }
        }
        if (voicePackMode == SqueakVoicePackMode.Off)
            SqueakySettingsUI.StatusPanel(list.GetRect(44f), "SR.VoicePack.OffNotice".Translate(), SqueakySurfaceKind.Warning);
        search = SqueakySettingsUI.SearchField(list.GetRect(30f), search, "SR.VoicePack.Search".Translate());
        string query = search.Trim();
        int shownCount = 0;
        for (int i = 0; i < packs.Count; i++)
        {
            SqueakVoicePackDef pack = packs[i];
            if (!VoicePackMatchesSearch(pack, query)) continue;
            shownCount++;
            DrawVoicePackRow(list, scope, target, pack, selected, preventNewSelection);
        }
        if (shownCount == 0)
            SqueakySettingsUI.EmptyState(list.GetRect(30f), (packs.Count == 0 ? "SR.VoicePack.None" : "SR.VoicePack.SearchNone").Translate());

        int unavailableCount = CountUnavailablePackKeys(selected, packs);
        if (unavailableCount > 0)
        {
            string stateKey = status.State == SqueakVoicePackDomainState.Dormant ? "SR.VoicePack.Dormant"
                : status.State == SqueakVoicePackDomainState.TargetUnavailable ? "SR.VoicePack.TargetUnavailable"
                : "SR.VoicePack.Orphan";
            float textWidth = Mathf.Max(1f, list.ColumnWidth - 8f - 8f - SqueakySettingsUI.HelpSize - 6f - 110f - 6f);
            float textHeight = Text.CalcHeight(stateKey.Translate(unavailableCount), textWidth);
            Rect row = list.GetRect(Mathf.Max(42f, textHeight + 12f));
            SqueakySettingsUI.PanelFrame(row, SqueakySurfaceKind.Warning);
            Rect forget = new(row.xMax - 116f, row.y + 5f, 110f, row.height - 10f);
            Rect help = new(forget.x - 6f - SqueakySettingsUI.HelpSize, row.y + 7f, SqueakySettingsUI.HelpSize, SqueakySettingsUI.HelpSize);
            Widgets.Label(new Rect(row.x + 8f, row.y + 5f, Mathf.Max(1f, help.x - row.x - 14f), row.height - 10f), stateKey.Translate(unavailableCount));
            if (SqueakySettingsUI.Button(forget, "SR.VoicePack.ForgetUnavailable".Translate(), SqueakyButtonKind.Danger))
            {
                CommitContinuousXenotypeEdit();
                List<string> retained = new();
                for (int i = 0; i < selected.Count; i++)
                    if (ContainsPackKey(packs, selected[i])) retained.Add(selected[i]);
                SetVoicePackSelection(scope, target, retained); InvalidateXenotypeRowCache();
            }
            SqueakySettingsUI.HelpIndicator(help, "SR.VoicePack.Unavailable.Tooltip.Short".Translate(unavailableCount));
        }
    }

    private void DrawVoicePackRow(Listing_Standard list, SqueakVoicePackScope scope, string target,
        SqueakVoicePackDef pack, IReadOnlyList<string> selected, bool preventNewSelection)
    {
        string key = PackKey(pack);
        bool on = ContainsOrdinal(selected, key);
        Rect row = list.GetRect(74f);
        Widgets.DrawBoxSolid(row, Mouse.IsOver(row) ? SqueakySettingsUI.Raised : SqueakySettingsUI.Panel);
        SqueakySettingsUI.DrawBorder(row);
        bool before = on;
        bool mayChange = on || !preventNewSelection;
        SqueakySettingsUI.Toggle(new Rect(row.x + 4f, row.y + 16f, 42f, 42f), "", ref on, mayChange,
            mayChange ? "" : "SR.Xeno.ConflictEnableBlocked".Translate());
        if (on != before)
        {
            List<string> next = new(selected);
            if (on) next.Add(key); else RemoveOrdinal(next, key);
            CommitContinuousXenotypeEdit();
            SetVoicePackSelection(scope, target, next); InvalidateXenotypeRowCache();
        }
        string modName = pack.modContentPack?.Name ?? pack.modContentPack?.PackageId ?? "—";
        string? author = pack.modContentPack?.ModMetaData?.AuthorsString;
        if (author.NullOrEmpty()) author = modName;
        string primary = (pack.LabelCap.NullOrEmpty() ? pack.defName : pack.LabelCap) + " — " + author;
        Rect helpRect = new(row.xMax - SqueakySettingsUI.HelpSize - 6f, row.y + 5f, SqueakySettingsUI.HelpSize, SqueakySettingsUI.HelpSize);
        DrawClippedLabel(new Rect(row.x + 52f, row.y + 3f, helpRect.x - row.x - 58f, 25f), primary, primary, false);
        Color old = GUI.color; GUI.color = Color.gray; Text.Font = GameFont.Tiny;
        string secondary = "SR.VoicePack.Attribution".Translate(modName, author) + " · " + PackCoverage(pack);
        DrawClippedLabel(new Rect(row.x + 52f, row.y + 27f, row.width - 58f, 20f), secondary, secondary, false);
        string state = PackPlayableActionCount(pack) > 0 ? "SR.VoicePack.Status.Ready".Translate() : "SR.VoicePack.Status.Empty".Translate();
        DrawClippedLabel(new Rect(row.x + 52f, row.y + 48f, row.width - 58f, 20f), state, state, false);
        Text.Font = GameFont.Small; GUI.color = old;
        SqueakySettingsUI.HelpIndicator(helpRect, "SR.VoicePack.Row.Tooltip.Short".Translate(primary, state, key));
    }

    private static string PackKey(SqueakVoicePackDef pack) => pack.TryGetPackKey(out string key) ? key : "";
    private static string PackSearchText(SqueakVoicePackDef pack) => (pack.LabelCap.NullOrEmpty() ? pack.defName : pack.LabelCap) + "\n"
        + (pack.modContentPack?.Name ?? "") + "\n" + (pack.modContentPack?.ModMetaData?.AuthorsString ?? "") + "\n"
        + pack.defName + "\n" + PackKey(pack) + "\n" + (pack.modContentPack?.PackageId ?? "");
    private static int PackPlayableActionCount(SqueakVoicePackDef pack)
    {
        int count = 0;
        if (pack.actions == null) return count;
        for (int i = 0; i < pack.actions.Count; i++)
        {
            SqueakVoicePackAction action = pack.actions[i];
            if (action?.sounds == null) continue;
            for (int j = 0; j < action.sounds.Count; j++)
                if (action.sounds[j] != null) { count++; break; }
        }
        return count;
    }
    private static string PackCoverage(SqueakVoicePackDef pack) => "SR.VoicePack.Coverage".Translate(PackPlayableActionCount(pack), ConfiguredActions.Count);

    private static void DrawClippedLabel(Rect rect, string text, string tooltip, bool ownsHelpIndicator = true)
    {
        SqueakySettingsUI.EllipsizedLabel(rect, text, tooltip, ownsHelpIndicator);
    }

    private void CommitXenotypeEditorNow()
    {
        if (xenotypeDraft?.Dirty != true) return;
        xenotypeDraft.Commit(xenotypePresets);
        NotifyContinuousXenotypeRuntimeChanged();
        QueuePersistence();
        InvalidateXenotypeRowCache();
    }

    private void CommitAndFlushContinuousXenotypeEdit()
    {
        CommitContinuousXenotypeEdit();
        FlushPendingRuntimeForPreview();
    }

    /// <summary>Canonicalizes dirty Xenotype state but deliberately leaves its coalesced resolver revision pending for a following discrete mutation.</summary>
    private void CommitContinuousXenotypeEdit() => CommitXenotypeEditorNow();

    private static SqueakActionConfig? GetActionConfig(SqueakAction action) => ConfiguredSqueakers().SelectMany(x => x.actions).FirstOrDefault(x => x.action == action);
    private static float SafeClamp(float value, float min, float max, float fallback) => Mathf.Clamp(float.IsNaN(value) || float.IsInfinity(value) ? fallback : value, min, max);
}
