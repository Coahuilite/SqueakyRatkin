using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using Verse;

namespace SqueakyRatkin;

/// <summary>Core handler namespace varies across game assemblies; resolve it by its stable full type name.</summary>
[HarmonyPatch]
public static class Patch_MentalBreak
{
    private static IEnumerable<MethodBase> TargetMethods()
    {
        MethodBase? target = AccessTools.Method("Verse.AI.MentalStateHandler:TryStartMentalState")
            ?? AccessTools.Method("RimWorld.MentalStateHandler:TryStartMentalState");
        if (target != null) yield return target;
        else SqueakLog.HookMentalBreakUnavailable();
    }

    private static void Postfix(object __instance, bool __result)
    {
        if (!__result || AccessTools.Field(__instance.GetType(), "pawn")?.GetValue(__instance) is not Pawn pawn
            || !pawn.Spawned || pawn.MapHeld != Find.CurrentMap) return;
        pawn.GetComp<CompSqueaker>()?.Notify_MentalBreak();
    }
}
