using LudeonTK;
using UnityEngine;
using Verse;

namespace SqueakyRatkin;

public static class SqueakDebugActions
{
    [DebugAction("Squeaky Ratkin", "Action statistics: Start selected", actionType = DebugActionType.Action, allowedGameStates = AllowedGameStates.PlayingOnMap)]
    public static void ActionStatisticsStartSelected()
    {
        if (SqueakyRatkinMod.Settings?.developerToolsEnabled == true) SqueakActionStatistics.StartSelectedPawn(Find.Selector.SingleSelectedThing as Pawn);
    }

    [DebugAction("Squeaky Ratkin", "Action statistics: Stop", actionType = DebugActionType.Action, allowedGameStates = AllowedGameStates.PlayingOnMap)]
    public static void ActionStatisticsStop()
    {
        if (SqueakyRatkinMod.Settings?.developerToolsEnabled == true) SqueakActionStatistics.Stop();
    }

    [DebugAction("Squeaky Ratkin", "Action statistics: Reset", actionType = DebugActionType.Action, allowedGameStates = AllowedGameStates.PlayingOnMap)]
    public static void ActionStatisticsReset()
    {
        if (SqueakyRatkinMod.Settings?.developerToolsEnabled == true) SqueakActionStatistics.Reset();
    }
    [DebugAction("Squeaky Ratkin", "Record successful dispatches: ON", actionType = DebugActionType.Action, allowedGameStates = AllowedGameStates.PlayingOnMap)]
    public static void AudioPathDiagnosticsOn()
    {
        if (SqueakyRatkinMod.Settings?.developerToolsEnabled != true) return;
        SqueakDebug.AudioPathDiagnosticsEnabled = true;
        if (SqueakLog.ShouldEmitDev) SqueakLog.OverlayChanged(true);
    }

    [DebugAction("Squeaky Ratkin", "Record successful dispatches: OFF", actionType = DebugActionType.Action, allowedGameStates = AllowedGameStates.PlayingOnMap)]
    public static void AudioPathDiagnosticsOff()
    {
        if (SqueakyRatkinMod.Settings?.developerToolsEnabled != true) return;
        SqueakDebug.AudioPathDiagnosticsEnabled = false;
        if (SqueakLog.ShouldEmitDev) SqueakLog.OverlayChanged(false);
    }

    // Compatibility for older reflection callers; DebugAction registration uses the explicit audio-path names above.
    public static void OverlayOn() => AudioPathDiagnosticsOn();
    public static void OverlayOff() => AudioPathDiagnosticsOff();

    [DebugAction("Squeaky Ratkin", "Successful dispatch records: Clear", actionType = DebugActionType.Action, allowedGameStates = AllowedGameStates.PlayingOnMap)]
    public static void AudioPathDiagnosticsClear()
    {
        if (SqueakyRatkinMod.Settings?.developerToolsEnabled == true) SqueakAudioPathDiagnostics.Clear();
    }

    [DebugAction("Squeaky Ratkin", "Successful dispatch records: Copy latest", actionType = DebugActionType.Action, allowedGameStates = AllowedGameStates.PlayingOnMap)]
    public static void AudioPathDiagnosticsCopyLast()
    {
        if (SqueakyRatkinMod.Settings?.developerToolsEnabled != true) return;
        GUIUtility.systemCopyBuffer = SqueakAudioPathDiagnostics.GetLastReportText();
    }

    [DebugAction("Squeaky Ratkin", "Camera Indicator: ON", actionType = DebugActionType.Action, allowedGameStates = AllowedGameStates.PlayingOnMap)]
    public static void CameraIndicatorOn()
    {
        SqueakDebug.ShowCameraIndicator = true;
        if (SqueakLog.ShouldEmitDev) SqueakLog.CameraChanged(true);
    }

    [DebugAction("Squeaky Ratkin", "Camera Indicator: OFF", actionType = DebugActionType.Action, allowedGameStates = AllowedGameStates.PlayingOnMap)]
    public static void CameraIndicatorOff()
    {
        SqueakDebug.ShowCameraIndicator = false;
        if (SqueakLog.ShouldEmitDev) SqueakLog.CameraChanged(false);
    }

    [DebugAction("Squeaky Ratkin", "Live squeak diagnostics: Selected Ratkin", actionType = DebugActionType.Action, allowedGameStates = AllowedGameStates.PlayingOnMap)]
    public static void DiagnosticsSelected()
    {
        if (Prefs.DevMode && Find.CurrentMap != null)
        {
            SqueakDiagnosticsOverlay.SetMode(SqueakDiagnosticsMode.Selected);
        }
    }

    [DebugAction("Squeaky Ratkin", "Live squeak diagnostics: Ratkin in view", actionType = DebugActionType.Action, allowedGameStates = AllowedGameStates.PlayingOnMap)]
    public static void DiagnosticsVisible()
    {
        if (Prefs.DevMode && Find.CurrentMap != null)
        {
            SqueakDiagnosticsOverlay.SetMode(SqueakDiagnosticsMode.Visible);
        }
    }

    [DebugAction("Squeaky Ratkin", "Live squeak diagnostics: OFF", actionType = DebugActionType.Action, allowedGameStates = AllowedGameStates.PlayingOnMap)]
    public static void DiagnosticsOff()
    {
        SqueakDiagnosticsOverlay.SetMode(SqueakDiagnosticsMode.Off);
    }
}
