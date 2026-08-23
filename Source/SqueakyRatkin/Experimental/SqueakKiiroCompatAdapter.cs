#if SQUEAKY_EXPERIMENTAL
using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace SqueakyRatkin;

/// <summary>
/// 「指猫为鼠」实验装配器(仅 Dev/EXP 构建编译)。
/// 启动时,当实验开关开启且 Kiiro Race 已加载:把 Ratkin def 上的 CompProperties_Squeaker
/// 配置深克隆后挂到标准 HAR def Kiiro_Race,使 Kiiro Pawn 生成时带上发声组件。
/// 0.3.2 race-aware 路由下,单独挂组件会命中 Kiiro_Race 空域(静默),因此路由快照
/// (SqueakRuntimeSnapshot.KiiroAsRatkin)同时把 Kiiro_Race 域映射为产品域 Ratkin:
/// 鼠族音池、PackFallback 与内置回退都对 Kiiro Pawn 生效。
/// 只引用第三方 defName 标识符,不复制资源或代码。开关关闭时完全不装配,与 SR 默认行为一致。
/// def 级装配在会话内不可撤销,开关切换需重启生效。
/// </summary>
[StaticConstructorOnStartup]
internal static class SqueakKiiroCompatAdapter
{
    internal const string KiiroPackageId = "Ancot.KiiroRace";
    internal const string KiiroRaceDefName = "Kiiro_Race";
    private const string RatkinRaceDefName = "Ratkin";

    /// <summary>本会话启动时是否已实际装配。开发者页状态行读取。</summary>
    public static bool AttachedThisSession { get; private set; }

    static SqueakKiiroCompatAdapter()
    {
        AttachedThisSession = false;
        try
        {
            AttachIfRequested();
        }
        catch (Exception)
        {
            // 静默失败:实验装配不成功等同于开关未生效,由诊断面板/overlay 观察。
        }
    }

    private static void AttachIfRequested()
    {
        if (SqueakyRatkinMod.Settings == null || !SqueakyRatkinMod.Settings.experimentalKiiroCompat) return;
        if (!ModsConfig.IsActive(KiiroPackageId)) return;
        ThingDef? kiiro = DefDatabase<ThingDef>.GetNamedSilentFail(KiiroRaceDefName);
        if (kiiro == null) return;
        kiiro.comps ??= new List<CompProperties>();
        if (kiiro.comps.Any(c => c is CompProperties_Squeaker)) return; // 幂等
        ThingDef? ratkin = DefDatabase<ThingDef>.GetNamedSilentFail(RatkinRaceDefName);
        CompProperties_Squeaker? source = ratkin?.comps?.OfType<CompProperties_Squeaker>().FirstOrDefault();
        if (source == null) return;
        kiiro.comps.Add(CloneProps(source));
        AttachedThisSession = true;
    }

    private static CompProperties_Squeaker CloneProps(CompProperties_Squeaker source) => new()
    {
        globalMinIntervalTicks = source.globalMinIntervalTicks,
        scaleFrequencyWithTalking = source.scaleFrequencyWithTalking,
        actions = source.actions == null ? new List<SqueakActionConfig>() : source.actions.Where(a => a != null).Select(a => CloneAction(a!)).ToList(),
        moodMods = source.moodMods == null ? new List<SqueakMoodMod>() : source.moodMods.Where(m => m != null).Select(m => m!.Clone()).ToList(),
        distancePresets = source.distancePresets == null ? new List<SqueakDistancePresetConfig>() : source.distancePresets.Where(d => d != null).Select(d => new SqueakDistancePresetConfig { preset = d!.preset, range = d.range }).ToList(),
    };

    private static SqueakActionConfig CloneAction(SqueakActionConfig a) => new()
    {
        action = a.action,
        mode = a.mode,
        minIntervalTicks = a.minIntervalTicks,
        probabilityPerCheck = a.probabilityPerCheck,
        ignoreGlobalCooldown = a.ignoreGlobalCooldown,
        cooldownClock = a.cooldownClock,
    };
}
#endif
