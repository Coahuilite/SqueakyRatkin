using LudeonTK;
using Verse;

namespace SqueakyRatkin;

public static class SqueakDebugActions
{
    [DebugAction("Squeaky Ratkin", "Overlay: ON", actionType = DebugActionType.Action, allowedGameStates = AllowedGameStates.PlayingOnMap)]
    public static void OverlayOn()
    {
        SqueakDebug.ShowOverlay = true;
        Log.Message("[SqueakyRatkin] Overlay ON");
    }

    [DebugAction("Squeaky Ratkin", "Overlay: OFF", actionType = DebugActionType.Action, allowedGameStates = AllowedGameStates.PlayingOnMap)]
    public static void OverlayOff()
    {
        SqueakDebug.ShowOverlay = false;
        Log.Message("[SqueakyRatkin] Overlay OFF");
    }

    [DebugAction("Squeaky Ratkin", "Camera Indicator: ON", actionType = DebugActionType.Action, allowedGameStates = AllowedGameStates.PlayingOnMap)]
    public static void CameraIndicatorOn()
    {
        SqueakDebug.ShowCameraIndicator = true;
        Log.Message("[SqueakyRatkin] Camera Indicator ON");
    }

    [DebugAction("Squeaky Ratkin", "Camera Indicator: OFF", actionType = DebugActionType.Action, allowedGameStates = AllowedGameStates.PlayingOnMap)]
    public static void CameraIndicatorOff()
    {
        SqueakDebug.ShowCameraIndicator = false;
        Log.Message("[SqueakyRatkin] Camera Indicator OFF");
    }
}
