#if SQUEAKY_DEV
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace SqueakyRatkin;

[HarmonyPatch(typeof(GlobalControlsUtility), nameof(GlobalControlsUtility.DoDate))]
public static class Patch_GlobalControlsUtility_CameraIndicator
{
    private const float LineHeight = 26f;

    private static void Postfix(float leftX, float width, ref float curBaseY)
    {
        if (!Prefs.DevMode || !SqueakDebug.ShowCameraIndicator || Find.CurrentMap == null || Event.current?.type == EventType.Layout)
        {
            return;
        }

        CameraDriver camera = Find.CameraDriver;
        FloatRange sizeRange = camera.config.sizeRange;
        float rootSize = camera.RootSize;
        float t = Mathf.InverseLerp(sizeRange.min, sizeRange.max, rootSize);
        float height = 15f + (t * 50f);

        curBaseY -= LineHeight;
        Rect rect = new(leftX, curBaseY, width, LineHeight);
        TextAnchor previousAnchor = Text.Anchor;
        Text.Anchor = TextAnchor.UpperRight;
        Widgets.Label(rect, "SR.Debug.CameraIndicator".Translate(height.ToString("0.0"), rootSize.ToString("0.0")));
        Text.Anchor = previousAnchor;
    }
}
#endif
