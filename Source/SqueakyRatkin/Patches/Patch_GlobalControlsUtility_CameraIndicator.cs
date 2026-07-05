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

        Camera camera = Find.Camera;
        if (camera == null)
        {
            return;
        }

        float height = camera.transform.position.y;
        float viewSize = camera.orthographicSize;

        curBaseY -= LineHeight;
        Rect rect = new(leftX, curBaseY, width, LineHeight);
        TextAnchor previousAnchor = Text.Anchor;
        Text.Anchor = TextAnchor.UpperRight;
        Widgets.Label(rect, "SR.Debug.CameraIndicator".Translate(height.ToString("0.0"), viewSize.ToString("0.0")));
        Text.Anchor = previousAnchor;
    }
}
