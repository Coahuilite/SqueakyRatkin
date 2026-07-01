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
        // Selector.Select 实际签名: Select(object obj, bool playSound, bool forceDesignatorDeselect)
        // 第一参数是 object(不是 Thing),按重载链 fallback。
        return AccessTools.Method(typeof(Selector), "Select", new[] { typeof(object), typeof(bool), typeof(bool) })
            ?? AccessTools.Method(typeof(Selector), "Select", new[] { typeof(object), typeof(bool) })
            ?? AccessTools.Method(typeof(Selector), "Select", new[] { typeof(object) });
    }

    // 用位置注入 __0 取第一参数,避免与原方法参数名耦合(原方法参数名为 obj,不是 t)。
    private static void Postfix(object __0)
    {
        if (__0 is not Pawn pawn || !CompSqueaker.IsRatkin(pawn))
        {
            return;
        }

        pawn.GetComp<CompSqueaker>()?.Notify_Select();
    }
}
