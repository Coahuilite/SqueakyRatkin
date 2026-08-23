using System;
using System.Collections.Generic;
using System.IO;
using SqueakyRatkin.Kernel;
using Verse;

namespace SqueakyRatkin.ConfigCopyCharacterization;

/// <summary>
/// Config 副本生命周期 harness（决策 §4.6/§5「Config 三场景 harness」）：生产
/// <c>SqueakFallbackProfileStore</c>（适配层单写者 temp+atomic replace，不经 WriteSettings）在 Scribe
/// stub 上的端到端场景：
///   A 缺失 → RebuildFromSource（写出干净副本，重启幂等）
///   B 损坏 → RebuildFromSource（替换损坏文件 + 记 store-failed 日志）
///   C 版本低 → RebuildFromSource（源版本覆盖副本）
///   D delta 合并 → MergeDelta（field-presence override 合入，副本回写并保留 delta）
///   E 重置覆盖 → 源版本提升时旧 delta 被源覆盖（重建 = 源胜）
///   F 包身份不符（损坏类）→ RebuildFromSource
/// 另含 DomainFilter 闸（未装配 race 不写副本）与「第二遍加载不再变」幂等断言。
/// 每个场景在临时 Config 目录运行，结束时清理。
/// </summary>
internal static class Program
{
    private const string ProfileFileName = "SqueakyRatkin_Profile_Ratkin.xml";
    private static string configDir = null!;
    private static string profilePath = null!;
    private static int failures;

    private static int Main()
    {
        configDir = Path.Combine(Path.GetTempPath(), "sr-config-copy-" + Guid.NewGuid().ToString("N"));
        Verse.GenFilePaths.ConfigFolderPath = configDir;
        profilePath = Path.Combine(configDir, ProfileFileName);
        try
        {
            MissingCopyRebuildsFromSource();
            CorruptCopyRebuildsAndLogs();
            StaleVersionCopyRebuilds();
            DeltaMergesIntoSource();
            ResetOverwriteDropsStaleDelta();
            ForeignPackageIdRebuilds();
            FilterSkipsUnassembledRace();
            if (failures == 0)
            {
                Console.WriteLine("Config copy characterization passed.");
                return 0;
            }
            Console.Error.WriteLine("Config copy characterization FAILED (" + failures + ").");
            return 1;
        }
        finally
        {
            try { if (Directory.Exists(configDir)) Directory.Delete(configDir, true); } catch { }
        }
    }

    private static BuiltInFallbackTable SourceV(int version)
    {
        return new BuiltInFallbackTable(new[]
        {
            new FallbackProfile(new RaceKey("Ratkin"), version, new Dictionary<string, string>
            {
                ["Call"] = "SR_Call",
                ["Eat"] = "SR_Eat",
                ["Sleep"] = "SR_Sleep",
            }),
        });
    }

    private static BuiltInFallbackTable FormalSource() => BuiltInFallbackCatalog.Create("Ratkin");

    private static void MissingCopyRebuildsFromSource()
    {
        ResetState();
        Scenario("A-missing");
        Check(!File.Exists(profilePath), "missing: no copy on disk before first load", ref failures);

        BuiltInFallbackTable source = FormalSource();
        BuiltInFallbackTable loaded = SqueakFallbackProfileStore.LoadOrRebuild(source, DomainFilter.Everything);
        FallbackProfile? profile = loaded.For(new RaceKey("Ratkin"));
        Check(profile != null && profile.SoundKeys.Count == 15 && profile.SoundKeys["Call"] == "SR_Call"
            && !profile.SoundKeys.ContainsKey("Crying") && !profile.SoundKeys.ContainsKey("Giggling"),
            "missing: resolved profile equals formal source (15 mappings, no Crying/Giggling)", ref failures);

        string first = ReadFile();
        Check(File.Exists(profilePath) && first.Contains("<sourceVersion>1</sourceVersion>")
            && !first.Contains("<hasOverrides>True</hasOverrides>"),
            "missing: rebuild wrote a clean version-1 copy without overrides", ref failures);

        FallbackProfile? reload = SqueakFallbackProfileStore.LoadOrRebuild(source, DomainFilter.Everything).For(new RaceKey("Ratkin"));
        Check(reload != null && reload.SoundKeys.Count == 15, "missing: second load keeps the healed copy", ref failures);
        Check(ReadFile() == first, "missing: second load did not rewrite the file (idempotent)", ref failures);
    }

    private static void CorruptCopyRebuildsAndLogs()
    {
        ResetState();
        Scenario("B-corrupt");
        File.WriteAllText(profilePath, "this is not a Scribe xml copy");
        SqueakLog.StoreFailures.Clear();

        BuiltInFallbackTable source = SourceV(3);
        FallbackProfile? profile = SqueakFallbackProfileStore.LoadOrRebuild(source, DomainFilter.Everything).For(new RaceKey("Ratkin"));
        Check(profile != null && profile.SoundKeys["Call"] == "SR_Call" && profile.SoundKeys["Eat"] == "SR_Eat" && profile.SoundKeys["Sleep"] == "SR_Sleep",
            "corrupt: resolved profile equals source", ref failures);
        Check(SqueakLog.StoreFailures.Contains("Ratkin"), "corrupt: store-failed diagnostic captured", ref failures);

        string first = ReadFile();
        Check(first.Contains("<sourceVersion>3</sourceVersion>") && !first.Contains("<hasOverrides>True</hasOverrides>"),
            "corrupt: file replaced with a clean version-3 copy", ref failures);
        Check(ReadFile() != "this is not a Scribe xml copy", "corrupt: garbage file was replaced", ref failures);

        FallbackProfile? reload = SqueakFallbackProfileStore.LoadOrRebuild(source, DomainFilter.Everything).For(new RaceKey("Ratkin"));
        Check(reload != null && reload.SoundKeys.Count == 3, "corrupt: second load stable", ref failures);
        Check(ReadFile() == first, "corrupt: second load did not rewrite the file (idempotent)", ref failures);
    }

    private static void StaleVersionCopyRebuilds()
    {
        ResetState();
        Scenario("C-stale-version");
        WritePlayerCopy(sourceVersion: 1, hasOverrides: false, overrides: null);

        BuiltInFallbackTable source = SourceV(3);
        FallbackProfile? profile = SqueakFallbackProfileStore.LoadOrRebuild(source, DomainFilter.Everything).For(new RaceKey("Ratkin"));
        Check(profile != null && profile.Version == 3 && profile.SoundKeys["Call"] == "SR_Call",
            "stale: older copy rebuilds from current source", ref failures);

        string first = ReadFile();
        Check(first.Contains("<sourceVersion>3</sourceVersion>") && !first.Contains("<hasOverrides>True</hasOverrides>"),
            "stale: file rewritten at current source version without overrides", ref failures);
        Check(SqueakLog.StoreFailures.Count == 0, "stale: version bump is not a corruption event", ref failures);

        FallbackProfile? reload = SqueakFallbackProfileStore.LoadOrRebuild(source, DomainFilter.Everything).For(new RaceKey("Ratkin"));
        Check(reload != null && reload.Version == 3, "stale: second load stable at version 3", ref failures);
        Check(ReadFile() == first, "stale: second load did not rewrite the file (idempotent)", ref failures);
    }

    private static void DeltaMergesIntoSource()
    {
        ResetState();
        Scenario("D-delta-merge");
        WritePlayerCopy(sourceVersion: 3, hasOverrides: true, overrides: new Dictionary<string, string> { ["Call"] = "SR_Call_Override" });

        BuiltInFallbackTable source = SourceV(3);
        FallbackProfile? profile = SqueakFallbackProfileStore.LoadOrRebuild(source, DomainFilter.Everything).For(new RaceKey("Ratkin"));
        Check(profile != null && profile.Version == 3 && profile.SoundKeys["Call"] == "SR_Call_Override"
            && profile.SoundKeys["Eat"] == "SR_Eat" && profile.SoundKeys["Sleep"] == "SR_Sleep",
            "delta: field-presence override merges over source, untouched keys inherit", ref failures);

        string first = ReadFile();
        Check(first.Contains("<hasOverrides>True</hasOverrides>") && first.Contains("SR_Call_Override"),
            "delta: copy rewritten preserving the override delta", ref failures);

        FallbackProfile? reload = SqueakFallbackProfileStore.LoadOrRebuild(source, DomainFilter.Everything).For(new RaceKey("Ratkin"));
        Check(reload != null && reload.SoundKeys["Call"] == "SR_Call_Override", "delta: second load keeps the merged override", ref failures);
        Check(ReadFile() == first, "delta: second load did not rewrite the file (idempotent)", ref failures);
    }

    private static void ResetOverwriteDropsStaleDelta()
    {
        ResetState();
        Scenario("E-reset-overwrite");
        // 玩家 override 副本 + 源版本提升：重建 = 源覆盖（override 不跨版本存活）。
        WritePlayerCopy(sourceVersion: 3, hasOverrides: true, overrides: new Dictionary<string, string> { ["Call"] = "SR_Call_Override" });

        BuiltInFallbackTable source = SourceV(4);
        FallbackProfile? profile = SqueakFallbackProfileStore.LoadOrRebuild(source, DomainFilter.Everything).For(new RaceKey("Ratkin"));
        Check(profile != null && profile.Version == 4 && profile.SoundKeys["Call"] == "SR_Call",
            "reset: source version bump overwrites stale delta (source wins)", ref failures);

        string first = ReadFile();
        Check(first.Contains("<sourceVersion>4</sourceVersion>") && !first.Contains("<hasOverrides>True</hasOverrides>"),
            "reset: file overwritten with a clean version-4 copy", ref failures);
        Check(SqueakLog.StoreFailures.Count == 0, "reset: legitimate rebuild is not a corruption event", ref failures);

        FallbackProfile? reload = SqueakFallbackProfileStore.LoadOrRebuild(source, DomainFilter.Everything).For(new RaceKey("Ratkin"));
        Check(reload != null && reload.SoundKeys["Call"] == "SR_Call", "reset: second load stable at source", ref failures);
        Check(ReadFile() == first, "reset: second load did not rewrite the file (idempotent)", ref failures);
    }

    private static void ForeignPackageIdRebuilds()
    {
        ResetState();
        Scenario("F-foreign-package-id");
        WritePlayerCopy(sourceVersion: 3, hasOverrides: true, overrides: new Dictionary<string, string> { ["Call"] = "SR_Call_Override" }, packageId: "other.mod.owner");

        BuiltInFallbackTable source = SourceV(3);
        FallbackProfile? profile = SqueakFallbackProfileStore.LoadOrRebuild(source, DomainFilter.Everything).For(new RaceKey("Ratkin"));
        Check(profile != null && profile.SoundKeys["Call"] == "SR_Call",
            "foreign: packageId mismatch is corrupt → rebuild from source", ref failures);
        Check(SqueakLog.StoreFailures.Count == 0,
            "foreign: packageId mismatch is silent normalization (only read failures log)", ref failures);

        string first = ReadFile();
        Check(first.Contains("coahuilite.squeakyratkin") && first.Contains("<sourceVersion>3</sourceVersion>"),
            "foreign: file rewritten under the permanent packageId", ref failures);
        FallbackProfile? reload = SqueakFallbackProfileStore.LoadOrRebuild(source, DomainFilter.Everything).For(new RaceKey("Ratkin"));
        Check(reload != null && reload.SoundKeys["Call"] == "SR_Call", "foreign: second load stable", ref failures);
        Check(ReadFile() == first, "foreign: second load did not rewrite the file (idempotent)", ref failures);
    }

    private static void FilterSkipsUnassembledRace()
    {
        ResetState();
        Scenario("G-filter-gate");
        // DomainFilter 闸：未装配 race 不写副本（§4.4）。
        BuiltInFallbackTable loaded = SqueakFallbackProfileStore.LoadOrRebuild(FormalSource(),
            new DomainFilter(new HashSet<RaceKey> { new("Kiiro") }));
        Check(loaded.For(new RaceKey("Ratkin")) == null && loaded.For(new RaceKey("Kiiro")) == null,
            "filter: unassembled race gets no profile", ref failures);
        Check(!File.Exists(profilePath), "filter: no copy file written for a filtered race", ref failures);
    }

    // ---- helpers ----

    private static void ResetState()
    {
        if (File.Exists(profilePath)) File.Delete(profilePath);
        SqueakLog.StoreFailures.Clear();
    }

    private static void Scenario(string name) => Console.WriteLine("Scenario " + name + "...");

    /// <summary>写一个「玩家/上次会话留下的」副本（与生产 store 同一 Scribe 形状）。</summary>
    private static void WritePlayerCopy(int sourceVersion, bool hasOverrides, Dictionary<string, string>? overrides, string packageId = SqueakyRatkinMod.PackageId)
    {
        SqueakFallbackProfileCopy copy = new()
        {
            packageId = packageId,
            sourceVersion = sourceVersion,
            hasOverrides = hasOverrides,
        };
        if (overrides != null)
        {
            foreach (KeyValuePair<string, string> entry in overrides)
                copy.overrides.Add(new SqueakFallbackProfileOverride { actionKey = entry.Key, soundKey = entry.Value });
        }
        Verse.SafeSaver.Save(profilePath, "SqueakyRatkinFallbackProfile", () =>
        {
            SqueakFallbackProfileCopy? saveable = copy;
            Scribe_Deep.Look(ref saveable, "FallbackProfile");
        });
    }

    private static string ReadFile() => File.ReadAllText(profilePath);

    private static void Check(bool condition, string name, ref int failures)
    {
        if (condition) Console.WriteLine("  ok: " + name);
        else { Console.Error.WriteLine("  FAIL: " + name); failures++; }
    }
}
