using System.Linq;
using HarmonyLib;
using UnityEngine;
using Verse;

namespace SqueakyRatkin;

/// <summary>
/// 模组入口。启动时加载 Harmony patch,提供 ModSettings(音源开关 + 心情调制 override)。
/// 配置分层:CompProperties(XML 全局默认) ← SqueakyRatkinSettings(玩家 override)。
/// </summary>
public class SqueakyRatkinMod : Mod
{
    public const string PackageId = "coahuilite.squeakyratkin";
    public static Harmony Harmony = null!;
    public static SqueakyRatkinSettings Settings = null!;

#if SQUEAKY_STEAM
    private const string BuildFlavor = "steam";
#elif SQUEAKY_GITHUB
    private const string BuildFlavor = "github";
#else
    private const string BuildFlavor = "dev";
#endif

    public SqueakyRatkinMod(ModContentPack content) : base(content)
    {
        Harmony = new Harmony(PackageId);
        Harmony.PatchAll();
        Settings = GetSettings<SqueakyRatkinSettings>();

        LongEventHandler.ExecuteWhenFinished(() =>
        {
            Settings.ApplyToRuntime();
            int n = DefDatabase<SoundDef>.AllDefs.Count(d => d.defName.StartsWith("SR_"));
            string version = typeof(SqueakyRatkinMod).Assembly.GetName().Version?.ToString() ?? "unknown";
            Log.Message($"[SqueakyRatkin] Loaded [{BuildFlavor}] v{version}. {n} squeak SoundDefs. useCustomOnly={Settings.useCustomOnly}, moodOverrides={Settings.moodOverrides.Count}");
        });
    }

    public override string SettingsCategory() => SqueakLabels.SettingsCategory;

    public override void DoSettingsWindowContents(Rect inRect)
    {
        Settings.DrawSettings(inRect);
    }

    /// <summary>设置窗口关闭/改值时同步运行时。RimWorld 在改设置后会调 Write,这里确保运行时同步。</summary>
    public override void WriteSettings()
    {
        base.WriteSettings();
        Settings.ApplyToRuntime();
    }
}
