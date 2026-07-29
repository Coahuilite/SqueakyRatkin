using HarmonyLib;
using Verse;

namespace SqueakyRatkin;

/// <summary>Keeps diagnostics teardown reliable outside the map GUI, including main-menu transitions.</summary>
[HarmonyPatch(typeof(Root), nameof(Root.Update))]
public static class Patch_Root_DiagnosticsLifecycle
{
    private static void Postfix()
    {
        SqueakDiagnosticsOverlay.MaintainLifecycle();
    }
}
