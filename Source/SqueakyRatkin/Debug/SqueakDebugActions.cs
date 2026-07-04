using LudeonTK;
using Verse;

namespace SqueakyRatkin;

#if SQUEAKY_DEV
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

    [DebugAction("Squeaky Ratkin", "Camera HUD: ON", actionType = DebugActionType.Action, allowedGameStates = AllowedGameStates.PlayingOnMap)]
    public static void CameraHudOn()
    {
        SqueakDebug.ShowCameraHeight = true;
        Log.Message("[SqueakyRatkin] Camera HUD ON");
    }

    [DebugAction("Squeaky Ratkin", "Camera HUD: OFF", actionType = DebugActionType.Action, allowedGameStates = AllowedGameStates.PlayingOnMap)]
    public static void CameraHudOff()
    {
        SqueakDebug.ShowCameraHeight = false;
        Log.Message("[SqueakyRatkin] Camera HUD OFF");
    }
}
#endif
