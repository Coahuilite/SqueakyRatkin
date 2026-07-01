using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace SqueakyRatkin;

[HarmonyPatch]
public static class Patch_Selector_Select
{
    private static MethodBase? TargetMethod()
    {
        return AccessTools.Method(typeof(Selector), "Select", new[] { typeof(Thing), typeof(bool), typeof(bool) })
            ?? AccessTools.Method(typeof(Selector), "Select", new[] { typeof(Thing), typeof(bool) })
            ?? AccessTools.Method(typeof(Selector), "Select", new[] { typeof(Thing) });
    }

    private static void Postfix(Thing t)
    {
        if (t is not Pawn pawn || !CompSqueaker.IsRatkin(pawn))
        {
            return;
        }

        pawn.GetComp<CompSqueaker>()?.Notify_Select();
    }
}
