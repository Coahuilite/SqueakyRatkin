using HarmonyLib;
using Verse;

namespace SqueakyRatkin;

[HarmonyPatch(typeof(Pawn), nameof(Pawn.PostApplyDamage))]
public static class Patch_Pawn_PostApplyDamage
{
    private static void Postfix(Pawn __instance)
    {
        if (!CompSqueaker.IsRatkin(__instance))
        {
            return;
        }

        __instance.GetComp<CompSqueaker>()?.Notify_Wounded();
    }
}
