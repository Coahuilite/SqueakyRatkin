using System.Linq;
using HarmonyLib;
using UnityEngine;
using Verse;

namespace SqueakyRatkin;

/// <summary>
/// 模组入口。启动时加载 Harmony patch,提供 ModSettings(音源开关 + 倍速冷却补偿 + 心情调制 override)。
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
            var srDefs = DefDatabase<SoundDef>.AllDefs.Where(d => d.defName.StartsWith("SR_")).ToList();
            int baseCount = srDefs.Count(d => !d.defName.EndsWith("_Pure") && !d.defName.EndsWith("_Preview"));
            int pureCount = srDefs.Count(d => d.defName.EndsWith("_Pure"));
            int previewCount = srDefs.Count(d => d.defName.EndsWith("_Preview"));
            string version = typeof(SqueakyRatkinMod).Assembly.GetName().Version?.ToString() ?? "unknown";
            string moodDetail = Settings.moodOverrides.Count == 0
                ? ""
                : "\n" + string.Join("\n", Settings.moodOverrides.Select(kv =>
                    $"    {kv.Key}: pitch={kv.Value.pitchFactor:0.##} vol={kv.Value.volumeFactor:0.##} jitter={kv.Value.pitchJitter}"));
            Log.Message($"[SqueakyRatkin] === Squeaky Ratkin v{version} [{BuildFlavor}] loaded ===\n" +
                        $"  SoundDefs: {srDefs.Count} (base={baseCount}, pure={pureCount}, preview={previewCount})\n" +
                        $"  Harmony: PatchAll OK ({Harmony.GetPatchedMethods().Count()} methods)\n" +
                        $"  useCustomOnly: {Settings.useCustomOnly} ({(Settings.useCustomOnly ? "pure custom" : "mixed vanilla+custom")})\n" +
                        $"  scaleCooldownWithTimeSpeed: {Settings.scaleCooldownWithTimeSpeed}\n" +
                        $"  globalCooldownMultiplier: {Settings.globalCooldownMultiplier:0.##}x\n" +
                        $"  moodOverrides: {Settings.moodOverrides.Count}{moodDetail}");
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
