using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Verse;

namespace SqueakyRatkin.FixtureGenerator;

/// <summary>
/// Scribe migration fixture verifier (§6.2). Inputs are frozen 0.2.4-shaped XML artifacts:
/// default mode reads them, runs the production v3/v1→v4/v2 transaction, publishes the cheap global policy,
/// serializes, reloads, and proves a second migration changes nothing. --author only regenerates expected/;
/// it never regenerates or edits fixtures/input/.
/// </summary>
internal static class Program
{
    private static readonly string Root = FindRepositoryRoot();
    private static readonly string InputDir = Path.Combine(Root, "fixtures", "input");
    private static readonly string ExpectedDir = Path.Combine(Root, "fixtures", "expected");
    private static readonly string[] LegacyScenarioNames =
    {
        "02-empty-file-no-schema",
        "03-explicit-off",
        "04-fallback-seeded",
        "05-multi-selections-lastwins",
        "06-orphan-packkey",
        "07-biotech-inactive-target",
        "08-corrupt-missing-fields",
        "09-mood-overrides-and-global-scope",
    };

    private static int Main(string[] args)
    {
        bool author = args.Contains("--author", StringComparer.Ordinal);
        bool migrationCheck = args.Length == 0 || author || args.Contains("--migrate-check", StringComparer.Ordinal);
        if (!migrationCheck || args.Any(arg => arg != "--author" && arg != "--migrate-check"))
        {
            Console.Error.WriteLine("Usage: dotnet run --project tools/SettingsFixtureGenerator -- [--migrate-check] [--author]");
            return 2;
        }

        Directory.CreateDirectory(ExpectedDir);
        Log.Reset();
        int failures = 0;

        RunFreshInstall(author, ref failures);
        foreach (string name in LegacyScenarioNames) RunLegacyInput(name, author, ref failures);

        if (Log.Captured.Count > 0)
        {
            Console.Error.WriteLine("--- Scribe/parse diagnostics captured ---");
            foreach (Log.Entry entry in Log.Captured) Console.Error.WriteLine($"[{entry.Level}] {entry.Text}");
        }

        Console.WriteLine(failures == 0
            ? $"OK: {1 + LegacyScenarioNames.Length} migration fixture scenarios green ({(author ? "expected regenerated" : "expected verified")})."
            : $"FAILED: {failures} migration fixture assertion(s) failed.");
        return failures == 0 ? 0 : 1;
    }

    private static void RunFreshInstall(bool author, ref int failures)
    {
        const string name = "01-new-install-first-save";
        Console.WriteLine("Scenario " + name + "...");
        try
        {
            SqueakyRatkinSettings fresh = new();
            AssertSchema("fresh schema", fresh, expectedMigrated: true, ref failures);
            if (fresh.MigrationPersistenceBlockedForFixture) Fail("fresh settings unexpectedly block persistence", ref failures);
            Publish(fresh);
            string first = Save(fresh);
            LoadResult secondLoad = LoadWithProductionMigration(first);
            Publish(secondLoad.Settings);
            string second = Save(secondLoad.Settings);
            AssertEqual("fresh reload is idempotent", second, first, ref failures);
            VerifyOrAuthorExpected(name, first, author, ref failures);
            Console.WriteLine("  green: " + first.Length + " bytes");
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("  EXCEPTION: " + ex);
            failures++;
        }
    }

    private static void RunLegacyInput(string name, bool author, ref int failures)
    {
        Console.WriteLine("Scenario " + name + "...");
        try
        {
            string inputPath = Path.Combine(InputDir, name + ".xml");
            if (!File.Exists(inputPath)) throw new InvalidOperationException("Frozen input fixture is missing: " + inputPath);
            string frozenInput = File.ReadAllText(inputPath);

            LoadResult firstLoad = LoadWithProductionMigration(frozenInput);
            if (name == "08-corrupt-missing-fields")
                AssertTransactionalAbort(firstLoad, ref failures);
            else
                AssertTransactionalSuccess(firstLoad, ref failures);
            AssertScenarioSemantics(name, firstLoad.Settings, ref failures);
            Publish(firstLoad.Settings);
            string first;
            if (name == "08-corrupt-missing-fields")
            {
                // Production blocks base.WriteSettings after a failed transaction, preserving the frozen legacy
                // source for the next startup retry. The harness must not bypass that guard with Save().
                first = frozenInput;
            }
            else
            {
                first = Save(firstLoad.Settings);
            }

            // The second load runs the same production PostLoadInit/transaction path. A stable output proves
            // that the first migration neither repeats nor leaves a partially-persisted schema behind.
            LoadResult secondLoad = LoadWithProductionMigration(first);
            Publish(secondLoad.Settings);
            string second = name == "08-corrupt-missing-fields" ? first : Save(secondLoad.Settings);
            AssertEqual("reload → migrate is idempotent", second, first, ref failures);

            if (name == "08-corrupt-missing-fields")
                AssertTransactionalAbort(secondLoad, ref failures);
            else
                AssertSchema("reloaded migrated schema", secondLoad.Settings, expectedMigrated: true, ref failures);

            AssertEqual("frozen input was not rewritten", File.ReadAllText(inputPath), frozenInput, ref failures);
            VerifyOrAuthorExpected(name, first, author, ref failures);
            Console.WriteLine("  green: " + first.Length + " bytes");
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("  EXCEPTION: " + ex);
            failures++;
        }
    }

    private static void VerifyOrAuthorExpected(string name, string actual, bool author, ref int failures)
    {
        string expectedPath = Path.Combine(ExpectedDir, name + ".xml");
        if (author)
        {
            File.WriteAllText(expectedPath, actual);
            return;
        }
        if (!File.Exists(expectedPath))
        {
            Console.Error.WriteLine("  MISSING expected fixture: " + expectedPath);
            failures++;
            return;
        }
        AssertEqual("matches committed expected", actual, File.ReadAllText(expectedPath), ref failures);
    }

    private static LoadResult LoadWithProductionMigration(string xml)
    {
        ScribeExtractor.PostLoadInitQueue.Clear();
        Scribe.loader.InitLoading(xml);
        SqueakyRatkinSettings target = null!;
        Scribe_Deep.Look(ref target, "ModSettings");

        TransactionSnapshot? beforeRootPostLoad = null;
        foreach (IExposable exposable in ScribeExtractor.PostLoadInitQueue.ToArray())
        {
            if (ReferenceEquals(exposable, target)) beforeRootPostLoad = TransactionSnapshot.Capture(target);
            Scribe.mode = LoadSaveMode.PostLoadInit;
            exposable.ExposeData();
        }
        Scribe.mode = LoadSaveMode.Inactive;
        ScribeExtractor.PostLoadInitQueue.Clear();
        if (beforeRootPostLoad == null) throw new InvalidOperationException("Settings root was not scheduled for PostLoadInit.");
        return new LoadResult(target, beforeRootPostLoad);
    }

    private static string Save(SqueakyRatkinSettings settings)
    {
        Scribe.saver.InitSaving("SettingsBlock");
        SqueakyRatkinSettings target = settings;
        Scribe_Deep.Look(ref target, "ModSettings");
        return Scribe.saver.FinalizeSaving();
    }

    /// <summary>Cheap publish proxy: it exercises the runtime's settings-owned global policy without UI/resolver stubs.
    /// 2b-2: the legacy string-domain bridge (ComposeDomainKey/DomainKey) is deleted; a migrated schema must carry
    /// complete AudioDomain identity fields (raceDefName; Xenotype scope also xenotypeDefName).</summary>
    private static void Publish(SqueakyRatkinSettings settings)
    {
        SqueakGlobalActionPolicy.Publish(settings);
        // Aborted transactions keep the legacy schema so the next startup can retry; only records in a migrated
        // schema are required to project complete AudioDomain identity (mirrors AssertTransactionalSuccess).
        if (settings.settingsSchemaVersion < 4 || settings.voicePackSchemaVersion < 2) return;
        foreach (VoicePackSelectionRecord record in settings.voicePackSelections ?? new List<VoicePackSelectionRecord>())
        {
            if (record == null || record.scope == SqueakVoicePackScope.Unspecified) continue;
            if (string.IsNullOrEmpty(record.raceDefName)) throw new InvalidOperationException("A migrated selection is missing its raceDefName AudioDomain identity.");
            if (record.scope == SqueakVoicePackScope.Race && !string.IsNullOrEmpty(record.xenotypeDefName)) throw new InvalidOperationException("A migrated Race selection carries xenotypeDefName.");
            if (record.scope == SqueakVoicePackScope.Xenotype && string.IsNullOrEmpty(record.xenotypeDefName)) throw new InvalidOperationException("A migrated Xenotype selection is missing xenotypeDefName.");
        }
    }

    private static void AssertTransactionalSuccess(LoadResult result, ref int failures)
    {
        AssertSchema("migrated schema", result.Settings, expectedMigrated: true, ref failures);
        if (!result.Settings.MigrationPersistencePendingForFixture)
        {
            Fail("successful migration did not mark persistence pending", ref failures);
        }
        foreach (VoicePackSelectionRecord record in result.Settings.voicePackSelections ?? new List<VoicePackSelectionRecord>())
        {
            if (record == null) { Fail("migrated selection contains null", ref failures); continue; }
            if (string.IsNullOrEmpty(record.raceDefName)) Fail("migrated selection is missing raceDefName", ref failures);
            if (record.scope == SqueakVoicePackScope.Race && !string.IsNullOrEmpty(record.xenotypeDefName)) Fail("migrated Race selection has xenotypeDefName", ref failures);
            if (record.scope == SqueakVoicePackScope.Xenotype && string.IsNullOrEmpty(record.xenotypeDefName)) Fail("migrated Xenotype selection is missing xenotypeDefName", ref failures);
            if (!string.IsNullOrEmpty(record.legacyTargetDefName)) Fail("migrated selection retained legacy target runtime state", ref failures);
        }
        foreach (XenotypePresetRecord record in result.Settings.xenotypePresets ?? new List<XenotypePresetRecord>())
        {
            if (record == null) { Fail("migrated preset contains null", ref failures); continue; }
            if (string.IsNullOrEmpty(record.raceDefName) || string.IsNullOrEmpty(record.xenotypeDefName))
                Fail("migrated preset identity is incomplete", ref failures);
        }
    }
    private static void AssertScenarioSemantics(string name, SqueakyRatkinSettings settings, ref int failures)
    {
        if (name == "05-multi-selections-lastwins")
        {
            VoicePackSelectionRecord? race = settings.voicePackSelections.LastOrDefault(record => record != null && record.scope == SqueakVoicePackScope.Race);
            if (race == null || race.enabledPackKeys.Count != 2 || race.enabledPackKeys[0] != "coahuilite.squeakyratkin:SR_OfficialExample_Race" || race.enabledPackKeys[1] != "coahuilite.squeakyratkin:SR_ExtraRacePack")
                Fail("last-wins Race selection was not preserved", ref failures);
            if (settings.voicePackSelections.Count(record => record != null && record.scope == SqueakVoicePackScope.Race) != 1)
                Fail("duplicate Race selections were not deduplicated", ref failures);
        }
        else if (name == "06-orphan-packkey")
        {
            VoicePackSelectionRecord? orphan = settings.voicePackSelections.SingleOrDefault(record => record != null && record.scope == SqueakVoicePackScope.Race);
            if (orphan == null || orphan.enabledPackKeys.Count != 1 || orphan.enabledPackKeys[0] != "coahuilite.squeakyratkin:SR_GonePack_9999")
                Fail("orphan PackKey was not retained", ref failures);
        }
        else if (name == "07-biotech-inactive-target")
        {
            VoicePackSelectionRecord? dormant = settings.voicePackSelections.SingleOrDefault(record => record != null && record.scope == SqueakVoicePackScope.Xenotype);
            XenotypePresetRecord? preset = settings.xenotypePresets.SingleOrDefault(record => record != null);
            if (dormant == null || dormant.xenotypeDefName != "InactiveXeno_NotLoaded" || dormant.enabledPackKeys.Count != 1)
                Fail("dormant Xenotype selection was not retained", ref failures);
            if (preset == null || preset.raceDefName != "Ratkin" || preset.xenotypeDefName != "InactiveXeno_NotLoaded")
                Fail("dormant Xenotype preset identity was not retained", ref failures);
        }
    }

    private static void AssertTransactionalAbort(LoadResult result, ref int failures)
    {
        TransactionSnapshot before = result.BeforeRootPostLoad;
        if (result.Settings.settingsSchemaVersion != before.SettingsSchemaVersion || result.Settings.voicePackSchemaVersion != before.VoicePackSchemaVersion)
            Fail("failed migration changed a schema marker", ref failures);
        if (!ReferenceEquals(result.Settings.voicePackSelections, before.Selections) || !ReferenceEquals(result.Settings.xenotypePresets, before.Presets))
            Fail("failed migration replaced a formal record list", ref failures);
        if (!string.Equals(before.Signature, TransactionSnapshot.Capture(result.Settings).Signature, StringComparison.Ordinal))
            Fail("failed migration changed formal record content", ref failures);
        if (result.Settings.MigrationPersistencePendingForFixture)
            Fail("failed migration requested persistence", ref failures);
        if (!result.Settings.MigrationPersistenceBlockedForFixture)
            Fail("failed migration did not block Config persistence", ref failures);
        AssertSchema("failed migration keeps legacy schema", result.Settings, expectedMigrated: false, ref failures);
    }

    private static void AssertSchema(string what, SqueakyRatkinSettings settings, bool expectedMigrated, ref int failures)
    {
        bool actual = settings.settingsSchemaVersion >= 4 && settings.voicePackSchemaVersion >= 2;
        if (actual != expectedMigrated)
            Fail(what + $" expected migrated={expectedMigrated}, got settings={settings.settingsSchemaVersion}, voicePack={settings.voicePackSchemaVersion}", ref failures);
    }

    private static void AssertEqual(string what, string actual, string expected, ref int failures)
    {
        if (actual == expected)
        {
            Console.WriteLine("  ok: " + what);
            return;
        }
        Console.Error.WriteLine("  MISMATCH: " + what);
        Console.Error.WriteLine("--- expected ---\n" + expected);
        Console.Error.WriteLine("--- actual ---\n" + actual);
        failures++;
    }

    private static void Fail(string message, ref int failures)
    {
        Console.Error.WriteLine("  ASSERTION: " + message);
        failures++;
    }

    private readonly struct LoadResult
    {
        internal readonly SqueakyRatkinSettings Settings;
        internal readonly TransactionSnapshot BeforeRootPostLoad;
        internal LoadResult(SqueakyRatkinSettings settings, TransactionSnapshot beforeRootPostLoad)
        {
            Settings = settings;
            BeforeRootPostLoad = beforeRootPostLoad;
        }
    }

    /// <summary>Reference and value snapshot taken immediately before the root Settings PostLoadInit transaction.</summary>
    private sealed class TransactionSnapshot
    {
        internal readonly int SettingsSchemaVersion;
        internal readonly int VoicePackSchemaVersion;
        internal readonly List<VoicePackSelectionRecord> Selections;
        internal readonly List<XenotypePresetRecord> Presets;
        internal readonly string Signature;

        private TransactionSnapshot(SqueakyRatkinSettings settings)
        {
            SettingsSchemaVersion = settings.settingsSchemaVersion;
            VoicePackSchemaVersion = settings.voicePackSchemaVersion;
            Selections = settings.voicePackSelections;
            Presets = settings.xenotypePresets;
            Signature = Describe(settings);
        }

        internal static TransactionSnapshot Capture(SqueakyRatkinSettings settings) => new(settings);

        private static string Describe(SqueakyRatkinSettings settings)
        {
            StringBuilder result = new();
            result.Append(settings.settingsSchemaVersion).Append('|').Append(settings.voicePackSchemaVersion);
            foreach (VoicePackSelectionRecord record in settings.voicePackSelections ?? new List<VoicePackSelectionRecord>())
            {
                if (record == null) { result.Append("|selection:null"); continue; }
                result.Append("|selection:").Append((int)record.scope).Append(':').Append(record.raceDefName).Append(':').Append(record.xenotypeDefName).Append(':').Append(record.legacyTargetDefName);
                foreach (string key in record.enabledPackKeys ?? new List<string>()) result.Append(':').Append(key);
            }
            foreach (XenotypePresetRecord record in settings.xenotypePresets ?? new List<XenotypePresetRecord>())
            {
                if (record == null) { result.Append("|preset:null"); continue; }
                result.Append("|preset:").Append(record.raceDefName).Append(':').Append(record.xenotypeDefName)
                    .Append(':').Append(record.hasOverallIntervalMultiplier).Append(':').Append(record.overallIntervalMultiplier)
                    .Append(':').Append(record.moodOverrides?.Count ?? -1).Append(':').Append(record.actionOverrides?.Count ?? -1);
            }
            return result.ToString();
        }
    }

    private static string FindRepositoryRoot()
    {
        DirectoryInfo? dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir != null)
        {
            if (File.Exists(Path.Combine(dir.FullName, "AGENTS.md"))) return dir.FullName;
            dir = dir.Parent;
        }
        throw new InvalidOperationException("Repository root (AGENTS.md) not found from " + AppContext.BaseDirectory);
    }
}
