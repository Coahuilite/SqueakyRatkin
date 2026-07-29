using HarmonyLib;
using System.Reflection;
using UnityEngine;
using Verse;

namespace SqueakyRatkin;

[HarmonyPatch]
public static class Patch_MapInterface_DiagnosticsOverlay
{
    private static MethodBase? target;
    private static bool missingHookWarningLogged;

    internal static bool HookAvailable { get; private set; }

    // MapInterface is not public in every reference assembly. Resolve and cache the exact
    // parameterless GUI hook before Harmony attempts to patch it, so an incompatible game
    // version safely skips this optional diagnostic feature rather than breaking PatchAll.
    private static bool Prepare()
    {
        HookAvailable = false;
        System.Type? type = AccessTools.TypeByName("RimWorld.MapInterface")
            ?? AccessTools.TypeByName("Verse.MapInterface")
            ?? AccessTools.TypeByName("MapInterface");
        target = type == null ? null
            : AccessTools.DeclaredMethod(type, "MapInterfaceOnGUI_BeforeMainTabs", System.Type.EmptyTypes)
                ?? AccessTools.DeclaredMethod(type, "MapInterfaceOnGUI", System.Type.EmptyTypes);
        if (target != null)
        {
            HookAvailable = true;
            return true;
        }

        if (!missingHookWarningLogged)
        {
            missingHookWarningLogged = true;
            if (SqueakLog.ShouldEmitDev) SqueakLog.DiagnosticsHookUnavailable();
        }

        return false;
    }

    private static MethodBase? TargetMethod()
    {
        return target;
    }

    private static void Postfix()
    {
        EventType? eventType = Event.current?.type;
        if (eventType == EventType.Layout)
        {
            SqueakDiagnosticsOverlay.RefreshIfDue();
        }
        else if (eventType == EventType.Repaint)
        {
            SqueakDiagnosticsOverlay.DrawCached();
        }
    }
}
