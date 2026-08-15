using System;
using UnityEngine;
using Verse;

namespace SqueakyRatkin;

internal enum SqueakyButtonKind { Primary, Secondary, Danger, Ghost }

internal enum SqueakySurfaceKind { Base, Raised, Emphasized, Warning, Success }

internal static class SqueakySettingsUI
{
    internal static readonly Color Gold = new(.92f, .68f, .30f);
    internal static readonly Color Ink = new(.055f, .052f, .047f, .94f);
    internal static readonly Color Panel = new(.09f, .083f, .071f, .90f);
    internal static readonly Color Raised = new(.115f, .106f, .091f, .96f);
    internal static readonly Color Border = new(.34f, .32f, .28f, .82f);
    internal static readonly Color Muted = new(.68f, .66f, .61f, .92f);
    internal static readonly Color Disabled = new(.43f, .42f, .39f, .82f);
    internal static readonly Color Success = new(.35f, .68f, .48f, .96f);
    internal static readonly Color Selected = new(.235f, .195f, .125f, .96f);
    internal static readonly Color Danger = new(.58f, .19f, .16f, .96f);

    internal const float SpaceXs = 4f;
    internal const float SpaceSm = 6f;
    internal const float SpaceMd = 8f;
    internal const float SpaceLg = 12f;
    internal const float ControlHeight = 32f;
    internal const float HelpSize = 24f;
    internal const float HelpGap = 5f;
    internal const float SelectableCardHorizontalInset = 11f;

    internal static float SelectableCardTextWidth(float cardWidth, bool hasTooltip = false)
    {
        float helpWidth = hasTooltip ? HelpSize + HelpGap : 0f;
        return Mathf.Max(1f, cardWidth - SelectableCardHorizontalInset * 2f - helpWidth);
    }

    internal static bool SelectableCard(Rect rect, string title, string description, bool selected,
        string tooltip = "", bool danger = false)
    {
        bool hovered = Mouse.IsOver(rect);
        Color fill = selected ? (danger ? new Color(.36f, .12f, .10f, .96f) : Selected)
            : hovered ? new Color(.16f, .145f, .12f, .94f) : Panel;
        Widgets.DrawBoxSolid(rect, fill);
        DrawBorder(rect);
        if (selected)
        {
            Widgets.DrawBoxSolid(new Rect(rect.x + 1f, rect.yMax - 4f, rect.width - 2f, 3f), danger ? Danger : Gold);
        }

        bool hasTooltip = !tooltip.NullOrEmpty();
        Rect inner = new(rect.x + SelectableCardHorizontalInset, rect.y + 8f,
            SelectableCardTextWidth(rect.width, hasTooltip), Mathf.Max(1f, rect.height - 16f));
        GameFont oldFont = Text.Font;
        Color oldColor = GUI.color;
        Text.Font = GameFont.Small;
        GUI.color = selected ? (danger ? new Color(1f, .70f, .62f) : new Color(1f, .86f, .58f)) : Color.white;
        float titleHeight = Text.CalcHeight(title, inner.width);
        Widgets.Label(new Rect(inner.x, inner.y, inner.width, titleHeight), title);
        Text.Font = GameFont.Tiny;
        GUI.color = new Color(.82f, .80f, .74f, .92f);
        Widgets.Label(new Rect(inner.x, inner.y + titleHeight + 3f, inner.width, inner.height - titleHeight - 3f), description);
        Text.Font = oldFont;
        GUI.color = oldColor;
        Rect helpRect = new(rect.xMax - HelpSize - 7f, rect.y + 7f, HelpSize, HelpSize);
        if (hasTooltip) HelpIndicator(helpRect, tooltip);
        Rect clickRect = hasTooltip ? new Rect(rect.x, rect.y, Mathf.Max(1f, helpRect.x - rect.x - 2f), rect.height) : rect;
        return Widgets.ButtonInvisible(clickRect);
    }

    internal static bool Button(Rect rect, string label, SqueakyButtonKind kind = SqueakyButtonKind.Secondary,
        bool enabled = true, string disabledReason = "", string tooltip = "")
    {
        bool interactive = GUI.enabled && enabled;
        bool hovered = interactive && Mouse.IsOver(rect);
        bool pressed = hovered && Input.GetMouseButton(0);
        Color fill = kind switch
        {
            SqueakyButtonKind.Primary => new Color(.25f, .205f, .125f, .98f),
            SqueakyButtonKind.Danger => new Color(.28f, .105f, .09f, .98f),
            SqueakyButtonKind.Ghost => new Color(.055f, .052f, .047f, .32f),
            _ => new Color(.105f, .098f, .086f, .96f)
        };
        if (!interactive) fill = new Color(.07f, .068f, .063f, .74f);
        else if (pressed) fill *= .78f;
        else if (hovered) fill += new Color(.045f, .04f, .03f, 0f);
        Widgets.DrawBoxSolid(rect, fill);
        DrawBorder(rect);
        if (kind == SqueakyButtonKind.Primary && interactive)
            Widgets.DrawBoxSolid(new Rect(rect.x + 1f, rect.yMax - 3f, rect.width - 2f, 2f), Gold);
        else if (kind == SqueakyButtonKind.Danger && interactive)
            Widgets.DrawBoxSolid(new Rect(rect.x + 1f, rect.yMax - 3f, rect.width - 2f, 2f), Danger);

        Color old = GUI.color;
        TextAnchor oldAnchor = Text.Anchor;
        GameFont oldFont = Text.Font;
        Text.Font = GameFont.Small;
        Text.Anchor = TextAnchor.MiddleCenter;
        GUI.color = !interactive ? Disabled : kind switch
        {
            SqueakyButtonKind.Primary => new Color(1f, .86f, .61f),
            SqueakyButtonKind.Danger => new Color(1f, .67f, .60f),
            _ => new Color(.91f, .89f, .84f)
        };
        Rect labelRect = rect.ContractedBy(8f, 2f);
        string help = !enabled ? disabledReason : tooltip;
        if (!help.NullOrEmpty()) labelRect.xMax -= HelpSize + HelpGap;
        EllipsizedLabel(labelRect, label);
        Text.Font = oldFont;
        Text.Anchor = oldAnchor;
        GUI.color = old;
        Rect helpRect = new(rect.xMax - HelpSize - 6f, rect.y + (rect.height - HelpSize) * .5f, HelpSize, HelpSize);
        Rect clickRect = help.NullOrEmpty() ? rect : new Rect(rect.x, rect.y, Mathf.Max(1f, helpRect.x - rect.x - 2f), rect.height);
        bool clicked = interactive && Widgets.ButtonInvisible(clickRect);
        if (!help.NullOrEmpty()) HelpIndicator(helpRect, help);
        return clicked;
    }

    internal static bool Toggle(Rect rect, string label, ref bool value, bool enabled = true,
        string disabledReason = "", string tooltip = "")
    {
        bool interactive = GUI.enabled && enabled;
        bool hovered = interactive && Mouse.IsOver(rect);
        Widgets.DrawBoxSolid(rect, hovered ? new Color(.115f, .107f, .093f, .82f) : new Color(.075f, .071f, .064f, .58f));
        Rect switchRect = new(rect.x + 6f, rect.y + (rect.height - 18f) * .5f, 34f, 18f);
        Widgets.DrawBoxSolid(switchRect, value && interactive ? new Color(.31f, .245f, .135f, .98f) : new Color(.12f, .115f, .105f, .98f));
        DrawBorder(switchRect);
        float knobX = value ? switchRect.xMax - 16f : switchRect.x + 2f;
        Widgets.DrawBoxSolid(new Rect(knobX, switchRect.y + 2f, 14f, 14f), value && interactive ? Gold : new Color(.54f, .52f, .48f));
        Color old = GUI.color;
        TextAnchor oldAnchor = Text.Anchor;
        string help = !enabled ? disabledReason : tooltip;
        if (!label.NullOrEmpty())
        {
            GUI.color = interactive ? Color.white : Disabled;
            Text.Anchor = TextAnchor.MiddleLeft;
            float helpSpace = (!tooltip.NullOrEmpty() || (!enabled && !disabledReason.NullOrEmpty())) ? HelpSize + HelpGap : 0f;
            EllipsizedLabel(new Rect(switchRect.xMax + 8f, rect.y, rect.xMax - switchRect.xMax - 14f - helpSpace, rect.height), label);
        }
        Text.Anchor = oldAnchor;
        GUI.color = old;
        Rect helpRect = new(rect.xMax - HelpSize - 6f, rect.y + (rect.height - HelpSize) * .5f, HelpSize, HelpSize);
        if (!help.NullOrEmpty()) HelpIndicator(helpRect, help);
        Rect clickRect = help.NullOrEmpty() ? rect : new Rect(rect.x, rect.y, Mathf.Max(1f, helpRect.x - rect.x - 2f), rect.height);
        if (!interactive || !Widgets.ButtonInvisible(clickRect)) return false;
        value = !value;
        return true;
    }

    internal static bool Tab(Rect rect, string label, bool active)
    {
        return Button(rect, label, active ? SqueakyButtonKind.Primary : SqueakyButtonKind.Ghost);
    }

    /// <summary>
    /// A compact, persistent-setting selector. Unlike RimWorld's command-style ButtonText material,
    /// this stays on the settings surface and uses gold only to mark an active value.
    /// </summary>
    internal static bool SettingSelector(Rect rect, string label, bool active, string tooltip = "")
    {
        bool hovered = Mouse.IsOver(rect);
        Color fill = hovered ? new Color(.145f, .132f, .108f, .96f) : new Color(.075f, .071f, .064f, .96f);
        Widgets.DrawBoxSolid(rect, fill);
        DrawBorder(rect);
        Widgets.DrawBoxSolid(new Rect(rect.x + 1f, rect.y + 1f, 3f, rect.height - 2f),
            active ? Gold : new Color(.29f, .28f, .25f, .72f));

        Color oldColor = GUI.color;
        TextAnchor oldAnchor = Text.Anchor;
        GameFont oldFont = Text.Font;
        Text.Font = GameFont.Small;
        Text.Anchor = TextAnchor.MiddleLeft;
        GUI.color = active ? new Color(1f, .86f, .60f) : new Color(.63f, .62f, .58f);
        const float arrowSlotWidth = 28f;
        Rect arrowSlot = new(rect.xMax - arrowSlotWidth, rect.y, arrowSlotWidth, rect.height);
        Rect helpSlot = tooltip.NullOrEmpty()
            ? new Rect(arrowSlot.x, rect.y, 0f, rect.height)
            : new Rect(arrowSlot.x - HelpSize, rect.y, HelpSize, rect.height);
        float textRight = tooltip.NullOrEmpty() ? arrowSlot.x : helpSlot.x;
        Rect textRect = new(rect.x + 12f, rect.y, Mathf.Max(1f, textRight - rect.x - 16f), rect.height);
        EllipsizedLabel(textRect, label);
        Text.Anchor = TextAnchor.MiddleCenter;
        Text.Font = GameFont.Tiny;
        GUI.color = active ? new Color(.92f, .75f, .43f) : new Color(.48f, .47f, .44f);
        Widgets.Label(arrowSlot, "▾");
        Text.Font = oldFont;
        Text.Anchor = oldAnchor;
        GUI.color = oldColor;

        if (!tooltip.NullOrEmpty()) HelpIndicator(helpSlot, tooltip);
        if (tooltip.NullOrEmpty()) return Widgets.ButtonInvisible(rect);
        Rect mainAction = new(rect.x, rect.y, Mathf.Max(1f, helpSlot.x - rect.x), rect.height);
        Rect arrowAction = new(helpSlot.xMax, rect.y, Mathf.Max(1f, rect.xMax - helpSlot.xMax), rect.height);
        return Widgets.ButtonInvisible(mainAction) || Widgets.ButtonInvisible(arrowAction);
    }

    internal static string SearchField(Rect rect, string value, string hint)
    {
        Widgets.DrawBoxSolid(rect, new Color(.055f, .053f, .049f, .98f));
        DrawBorder(rect);
        GUI.SetNextControlName(hint);
        string next = Widgets.TextField(rect, value ?? "");
        if (GUI.GetNameOfFocusedControl() == hint)
        {
            Widgets.DrawBoxSolid(new Rect(rect.x, rect.yMax - 2f, rect.width, 2f), Gold);
        }
        if (next.NullOrEmpty())
        {
            Color old = GUI.color;
            GameFont oldFont = Text.Font;
            GUI.color = new Color(.72f, .72f, .72f, .72f);
            Text.Font = GameFont.Tiny;
            Widgets.Label(rect.ContractedBy(7f, 5f), hint);
            Text.Font = oldFont;
            GUI.color = old;
        }
        return next;
    }

    internal static bool FilterChip(Rect rect, string label, bool active, string tooltip = "")
    {
        Widgets.DrawBoxSolid(rect, active ? new Color(.28f, .22f, .13f, .95f) : new Color(.10f, .095f, .085f, .9f));
        DrawBorder(rect);
        Color old = GUI.color;
        GUI.color = active ? new Color(1f, .83f, .48f) : new Color(.84f, .82f, .76f);
        TextAnchor oldAnchor = Text.Anchor;
        Text.Anchor = TextAnchor.MiddleCenter;
        Rect labelRect = rect.ContractedBy(6f, 1f);
        if (!tooltip.NullOrEmpty()) labelRect.xMax -= HelpSize + 2f;
        EllipsizedLabel(labelRect, label);
        Text.Anchor = oldAnchor;
        GUI.color = old;
        Rect helpRect = new(rect.xMax - HelpSize - 3f, rect.y + (rect.height - HelpSize) * .5f, HelpSize, HelpSize);
        if (!tooltip.NullOrEmpty()) HelpIndicator(helpRect, tooltip);
        Rect clickRect = tooltip.NullOrEmpty() ? rect : new Rect(rect.x, rect.y, Mathf.Max(1f, helpRect.x - rect.x - 2f), rect.height);
        return Widgets.ButtonInvisible(clickRect);
    }

    internal static void HelpIndicator(Rect rect, string tooltip)
    {
        if (tooltip.NullOrEmpty()) return;
        bool hovered = Mouse.IsOver(rect);
        Color oldColor = GUI.color;
        TextAnchor oldAnchor = Text.Anchor;
        GameFont oldFont = Text.Font;
        Widgets.DrawBoxSolid(rect, hovered ? new Color(.28f, .22f, .13f, .96f) : new Color(.10f, .095f, .085f, .92f));
        DrawBorder(rect, hovered ? Gold : Border);
        GUI.color = hovered ? new Color(1f, .86f, .60f) : Muted;
        Text.Font = GameFont.Tiny;
        Text.Anchor = TextAnchor.MiddleCenter;
        Widgets.Label(rect, "?");
        Text.Font = oldFont;
        Text.Anchor = oldAnchor;
        GUI.color = oldColor;
        TooltipHandler.TipRegion(rect, tooltip);
        ConsumeHelpMouseDown(rect);
    }

    internal static bool HelpToggle(Rect rect, bool active)
    {
        bool hovered = Mouse.IsOver(rect);
        Widgets.DrawBoxSolid(rect, active ? new Color(.28f, .22f, .13f, .96f)
            : hovered ? new Color(.20f, .17f, .12f, .96f) : new Color(.10f, .095f, .085f, .92f));
        DrawBorder(rect, active || hovered ? Gold : Border);
        Color oldColor = GUI.color; TextAnchor oldAnchor = Text.Anchor; GameFont oldFont = Text.Font;
        GUI.color = active || hovered ? new Color(1f, .86f, .60f) : Muted;
        Text.Font = GameFont.Tiny; Text.Anchor = TextAnchor.MiddleCenter;
        Widgets.Label(rect, "?");
        Text.Font = oldFont; Text.Anchor = oldAnchor; GUI.color = oldColor;
        Event current = Event.current;
        bool clicked = current.type == EventType.MouseDown && current.button == 0 && rect.Contains(current.mousePosition);
        ConsumeHelpMouseDown(rect);
        return clicked;
    }

    private static void ConsumeHelpMouseDown(Rect rect)
    {
        Event current = Event.current;
        // Help owns only presses that begin inside its rect. Never consume MouseUp: another IMGUI
        // control may own the hotControl when a drag that started elsewhere is released over help.
        if (current.type == EventType.MouseDown && current.button == 0
            && rect.Contains(current.mousePosition)) current.Use();
    }

    internal static void LabelWithHelp(Rect rect, string label, string tooltip, GameFont font = GameFont.Small,
        Color? color = null)
    {
        Color oldColor = GUI.color;
        GameFont oldFont = Text.Font;
        Text.Font = font;
        if (color.HasValue) GUI.color = color.Value;
        Rect help = new(rect.xMax - HelpSize, rect.y + Mathf.Max(0f, (rect.height - HelpSize) * .5f), HelpSize, HelpSize);
        EllipsizedLabel(new Rect(rect.x, rect.y, Mathf.Max(1f, help.x - rect.x - HelpGap), rect.height), label);
        HelpIndicator(help, tooltip);
        Text.Font = oldFont;
        GUI.color = oldColor;
    }

    internal static void EllipsizedLabel(Rect rect, string text, string tooltip = "", bool ownsHelpIndicator = true)
    {
        string shown = text;
        bool truncated = Text.CalcSize(shown).x > rect.width;
        Rect textRect = rect;
        if (truncated && !tooltip.NullOrEmpty() && ownsHelpIndicator) textRect.xMax -= HelpSize + HelpGap;
        if (truncated)
        {
            while (shown.Length > 1 && Text.CalcSize(shown + "…").x > textRect.width)
            {
                shown = shown.Substring(0, shown.Length - 1);
            }
            shown += "…";
        }
        bool oldWrap = Text.WordWrap;
        Text.WordWrap = false;
        Widgets.Label(textRect, shown);
        Text.WordWrap = oldWrap;
        if (truncated && !tooltip.NullOrEmpty())
        {
            if (ownsHelpIndicator)
                HelpIndicator(new Rect(rect.xMax - HelpSize, rect.y + Mathf.Max(0f, (rect.height - HelpSize) * .5f), HelpSize, HelpSize), tooltip);
            else TooltipHandler.TipRegion(rect, tooltip);
        }
    }

    internal static void PanelFrame(Rect rect, bool emphasized = false)
    {
        PanelFrame(rect, emphasized ? SqueakySurfaceKind.Emphasized : SqueakySurfaceKind.Base);
    }

    internal static void PanelFrame(Rect rect, SqueakySurfaceKind kind)
    {
        Color fill = kind switch
        {
            SqueakySurfaceKind.Raised => Raised,
            SqueakySurfaceKind.Emphasized => new Color(.13f, .112f, .082f, .92f),
            SqueakySurfaceKind.Warning => new Color(.17f, .095f, .073f, .94f),
            SqueakySurfaceKind.Success => new Color(.075f, .135f, .095f, .94f),
            _ => Ink
        };
        Widgets.DrawBoxSolid(rect, fill);
        DrawBorder(rect);
    }

    internal static void DrawBorder(Rect rect, Color? color = null)
    {
        Color stroke = color ?? Border;
        Widgets.DrawBoxSolid(new Rect(rect.x, rect.y, rect.width, 1f), stroke);
        Widgets.DrawBoxSolid(new Rect(rect.x, rect.yMax - 1f, rect.width, 1f), stroke);
        Widgets.DrawBoxSolid(new Rect(rect.x, rect.y, 1f, rect.height), stroke);
        Widgets.DrawBoxSolid(new Rect(rect.xMax - 1f, rect.y, 1f, rect.height), stroke);
    }

    internal static void StatusPanel(Rect rect, string text, SqueakySurfaceKind kind, string tooltip = "")
    {
        PanelFrame(rect, kind);
        Color oldColor = GUI.color;
        TextAnchor oldAnchor = Text.Anchor;
        GameFont oldFont = Text.Font;
        Text.Font = GameFont.Tiny;
        Text.Anchor = TextAnchor.MiddleCenter;
        GUI.color = kind == SqueakySurfaceKind.Success ? Success
            : kind == SqueakySurfaceKind.Warning ? new Color(1f, .67f, .48f) : Muted;
        Rect content = rect.ContractedBy(8f, 2f);
        if (!tooltip.NullOrEmpty()) content.xMax -= HelpSize + HelpGap;
        Widgets.Label(content, text);
        if (!tooltip.NullOrEmpty()) HelpIndicator(new Rect(rect.xMax - HelpSize - 7f, rect.y + 7f, HelpSize, HelpSize), tooltip);
        Text.Font = oldFont;
        Text.Anchor = oldAnchor;
        GUI.color = oldColor;
    }

    internal static void SectionHeader(Rect rect, string label, string description = "", string tooltip = "")
    {
        GameFont oldFont = Text.Font;
        Color oldColor = GUI.color;
        Text.Font = GameFont.Medium;
        GUI.color = Color.white;
        float titleWidth = tooltip.NullOrEmpty() ? rect.width : Mathf.Max(1f, rect.width - HelpSize - HelpGap);
        float titleHeight = Text.CalcHeight(label, titleWidth);
        Widgets.Label(new Rect(rect.x, rect.y, titleWidth, titleHeight), label);
        if (!tooltip.NullOrEmpty()) HelpIndicator(new Rect(rect.xMax - HelpSize, rect.y + 2f, HelpSize, HelpSize), tooltip);
        if (!description.NullOrEmpty())
        {
            Text.Font = GameFont.Tiny;
            GUI.color = Muted;
            Widgets.Label(new Rect(rect.x, rect.y + titleHeight + 3f, rect.width, rect.height - titleHeight - 3f), description);
        }
        Text.Font = oldFont;
        GUI.color = oldColor;
    }

    internal static void EmptyState(Rect rect, string text, SqueakySurfaceKind kind = SqueakySurfaceKind.Base)
    {
        PanelFrame(rect, kind);
        TextAnchor oldAnchor = Text.Anchor;
        Color oldColor = GUI.color;
        Text.Anchor = TextAnchor.MiddleCenter;
        GUI.color = Muted;
        Widgets.Label(rect.ContractedBy(24f), text);
        GUI.color = oldColor;
        Text.Anchor = oldAnchor;
    }
}

internal sealed class Dialog_SqueakyCompactMessageBox : Window
{
    private readonly string messageText;
    private readonly string confirmText;
    private readonly string cancelText;
    private readonly string titleText;
    private readonly System.Action? confirmAction;
    private readonly System.Action? cancelAction;
    private readonly string thirdText;
    private readonly System.Action? thirdAction;
    private readonly SqueakyButtonKind thirdKind;
    private readonly System.Action? closeAction;
    private readonly SqueakyButtonKind confirmKind;
    private readonly SqueakyButtonKind cancelKind;
    private readonly bool reverseButtons;
    private readonly int inputDelayFrames;
    private readonly int openedFrame;
    private bool actionInvoked;
    private Vector2 scrollPosition;

    internal Dialog_SqueakyCompactMessageBox(string text, string confirmText, System.Action? confirmAction,
        string cancelText, System.Action? cancelAction, string title,
        SqueakyButtonKind confirmKind = SqueakyButtonKind.Primary,
        SqueakyButtonKind cancelKind = SqueakyButtonKind.Secondary,
        bool reverseButtons = false, int inputDelayFrames = 1, System.Action? closeAction = null)
    {
        messageText = text;
        this.confirmText = confirmText;
        this.cancelText = cancelText;
        titleText = title;
        this.confirmAction = confirmAction;
        this.cancelAction = cancelAction;
        this.confirmKind = confirmKind;
        this.cancelKind = cancelKind;
        this.reverseButtons = reverseButtons;
        this.inputDelayFrames = inputDelayFrames;
        openedFrame = Time.frameCount;
        this.closeAction = closeAction;
        thirdText = "";
        thirdKind = SqueakyButtonKind.Primary;
        doCloseX = true;
        closeOnCancel = true;
        closeOnClickedOutside = false;
        absorbInputAroundWindow = true;
    }

    internal Dialog_SqueakyCompactMessageBox(string text, string confirmText, System.Action? confirmAction,
        string cancelText, System.Action? cancelAction, string thirdText, System.Action? thirdAction,
        string title, SqueakyButtonKind confirmKind, SqueakyButtonKind cancelKind, SqueakyButtonKind thirdKind,
        System.Action? closeAction = null)
        : this(text, confirmText, confirmAction, cancelText, cancelAction, title, confirmKind, cancelKind)
    {
        this.thirdText = thirdText;
        this.thirdAction = thirdAction;
        this.thirdKind = thirdKind;
        this.closeAction = closeAction;
    }

    public override void PreClose()
    {
        base.PreClose();
        if (!actionInvoked) closeAction?.Invoke();
    }

    public override Vector2 InitialSize
    {
        get
        {
            const float width = 560f;
            GameFont oldFont = Text.Font;
            Text.Font = GameFont.Small;
            float textHeight = Text.CalcHeight(messageText, width - 72f);
            Text.Font = oldFont;
            float maxHeight = Mathf.Max(220f, Mathf.Min(360f, UI.screenHeight - 80f));
            float height = Mathf.Clamp(textHeight + 132f, 220f, maxHeight);
            return new Vector2(Mathf.Min(width, UI.screenWidth - 80f), height);
        }
    }

    public override void DoWindowContents(Rect inRect)
    {
        SqueakySettingsUI.PanelFrame(inRect, SqueakySurfaceKind.Raised);
        Rect inner = inRect.ContractedBy(12f);
        SqueakySettingsUI.SectionHeader(new Rect(inner.x, inner.y, inner.width, 32f), titleText);

        Rect textArea = new(inner.x, inner.y + 40f, inner.width, inner.height - 92f);
        float textHeight = Text.CalcHeight(messageText, Mathf.Max(1f, textArea.width - 16f));
        if (textHeight > textArea.height)
        {
            Rect view = new(0f, 0f, textArea.width - 16f, textHeight + 6f);
            Widgets.BeginScrollView(textArea, ref scrollPosition, view);
            Widgets.Label(new Rect(0f, 0f, view.width, textHeight), messageText);
            Widgets.EndScrollView();
        }
        else
        {
            Widgets.Label(textArea, messageText);
        }

        const float gap = 10f;
        bool third = !thirdText.NullOrEmpty();
        bool single = cancelText.NullOrEmpty();
        float buttonWidth = single ? Mathf.Min(220f, inner.width) : third ? (inner.width - gap * 2f) / 3f : (inner.width - gap) * .5f;
        float left = inner.x;
        Rect confirm = new(single ? inner.xMax - buttonWidth : reverseButtons ? left + buttonWidth + gap : left, inner.yMax - 38f, buttonWidth, 38f);
        Rect cancel = new(reverseButtons ? left : confirm.xMax + gap, confirm.y, buttonWidth, confirm.height);
        Rect thirdButton = new(cancel.xMax + gap, confirm.y, buttonWidth, confirm.height);
        bool ready = Time.frameCount > openedFrame + inputDelayFrames;
        if (SqueakySettingsUI.Button(confirm, confirmText, confirmKind, ready))
        {
            actionInvoked = true;
            Close();
            confirmAction?.Invoke();
        }
        if (!single && SqueakySettingsUI.Button(cancel, cancelText, cancelKind, ready))
        {
            actionInvoked = true;
            Close();
            cancelAction?.Invoke();
        }
        if (third && SqueakySettingsUI.Button(thirdButton, thirdText, thirdKind, ready))
        {
            actionInvoked = true;
            Close();
            thirdAction?.Invoke();
        }
    }
}
