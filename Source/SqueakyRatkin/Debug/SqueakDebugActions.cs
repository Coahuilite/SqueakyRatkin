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
}
