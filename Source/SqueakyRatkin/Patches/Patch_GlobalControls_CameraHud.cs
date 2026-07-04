#if SQUEAKY_DEV
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace SqueakyRatkin;

[HarmonyPatch(typeof(GlobalControls), nameof(GlobalControls.GlobalControlsOnGUI))]
public static class Patch_GlobalControls_CameraHud
{
    private const float PanelWidth = 200f;
    private const float LineHeight = 24f;

    private static void Postfix()
    {
        if (!Prefs.DevMode || !SqueakDebug.ShowCameraHeight || Find.CurrentMap == null || Event.current?.type == EventType.Layout)
        {
            return;
        }

        CameraDriver camera = Find.CameraDriver;
        FloatRange sizeRange = camera.config.sizeRange;
        float rootSize = camera.RootSize;
        float t = Mathf.InverseLerp(sizeRange.min, sizeRange.max, rootSize);
        float height = 15f + (t * 50f);

        Rect rect = new(UI.screenWidth - PanelWidth, UI.screenHeight - 186f, PanelWidth, LineHeight);
        Text.Anchor = TextAnchor.UpperRight;
        Widgets.Label(rect, $"Cam h: {height:0.0} / size: {rootSize:0.0}");
        Text.Anchor = TextAnchor.UpperLeft;
    }
}
#endif
