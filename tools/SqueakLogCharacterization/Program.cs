using System;
using System.Linq;
using System.Reflection;
using Verse;
using SqueakyRatkin;

namespace SqueakyRatkin.Characterization;

internal static class Program
{
    private const string Build = "dev";
    private const string BuildId = "0.0.0+char";
    private const string Prefix = "[SqueakyRatkin] ";
#if SQUEAKY_DEV
    private static bool AutoEnablesDevLogging => true;
#else
    private static bool AutoEnablesDevLogging => false;
#endif

    private static int failures;

    private static int Main()
    {
        VerifyAllEventDefinitions();
        VerifyOnceSemantics();
        VerifyOnceLimit();
        VerifyDisabledModeGate();
        VerifySilentFailureBoundary();
        VerifyEncodingAndExceptionMetadata();
        VerifyInvalidLoggingModeFallsBackToAuto();
        VerifyV2Protocol();

        if (failures == 0)
        {
            Console.WriteLine("SqueakLog characterization passed.");
            return 0;
        }

        Console.Error.WriteLine($"SqueakLog characterization failed: {failures} assertion(s).");
        return 1;
    }

    private static void VerifyAllEventDefinitions()
    {
        Reset(SqueakDevLoggingMode.Enabled);
        SqueakLog.StartupIdentity();
        SqueakLog.StartupReady(37);
        SqueakLog.LoggingModeChanged(SqueakDevLoggingMode.Enabled, true);
        SqueakLog.LoggingModeChanged(SqueakDevLoggingMode.Disabled, false);
        SqueakLog.LoggingModeChanged(SqueakDevLoggingMode.Auto, true);
        SqueakLog.LoggingModeChanged(SqueakDevLoggingMode.Auto, false);
        SqueakLog.SettingsOpenApiUnavailable();
        SqueakLog.SettingsOpenFailed(new Exception("settings failed"));
        SqueakLog.CatalogRefreshFailed(new Exception("catalog failed"));
        SqueakLog.PackRejected("coahuilite.squeakyratkin:SR_Example", 2);
        SqueakLog.ResolverRebuildFailed(new Exception("resolver failed"));
        SqueakLog.TargetRejected("target-1", "reason-1");
        SqueakLog.XenotypeDiscoveryUnavailable("HAR not loaded");
        SqueakLog.XenotypeDiscoveryFailed(new Exception("discovery failed"));
        SqueakLog.XenotypeDiscoveryCandidate("Ratkin", "har", true);
        SqueakLog.TriggerAttemptFailed("Select", new Exception("trigger failed"));
        SqueakLog.AudioNoSound("Move");
        SqueakLog.AudioDispatchFailed("Attack", "SR_Attack_1", new Exception("dispatch failed"));
        SqueakLog.AudioDispatchOk("Select", "12345", "SR_OfficialExample_Race_Select", 0, "Mousy", "Thing_Ratkin12345");
        SqueakLog.TriggerOutcomeSummary(7, 2);
        SqueakLog.HookAttackUnavailable();
        SqueakLog.HookAttackTargetSkipped("999", "not player ordered");
        SqueakLog.HookMentalBreakUnavailable();
        SqueakLog.DiagnosticsHookUnavailable();
        SqueakLog.DiagnosticsStartFailed();
        SqueakLog.OverlayChanged(true);
        SqueakLog.CameraChanged(false);
        SqueakLog.WorkbenchOpenFailed(new Exception("workbench failed"));

        AssertLines(nameof(VerifyAllEventDefinitions),
            D("info", "daily", "mod.start.identity", $"Squeaky Ratkin started with {Build} build {BuildId}."),
            D("info", "daily", "mod.start.ready", "Squeaky Ratkin startup completed.", trailing: " count=37"),
            D("info", "daily", "logging.mode.enabled", "Detailed diagnostic logging is enabled.", trailing: " enabled=true"),
            D("info", "daily", "logging.mode.disabled", "Detailed diagnostic logging is disabled.", trailing: " enabled=false"),
            D("info", "daily", "logging.mode.auto_enabled", "Detailed diagnostic logging is enabled by Auto mode.", trailing: " enabled=true"),
            D("info", "daily", "logging.mode.auto_disabled", "Detailed diagnostic logging is disabled by Auto mode.", trailing: " enabled=false"),
            D("warning", "daily", "settings.open.api_unavailable", "Mod Settings API is unavailable."),
            D("warning", "daily", "settings.open.failed", "Mod Settings could not be opened.", trailing: " ex_type=System.Exception ex_msg=settings%20failed"),
            D("error", "daily", "voicepack.catalog.refresh_failed", "VoicePack catalog refresh failed.", trailing: " ex_type=System.Exception ex_msg=catalog%20failed"),
            D("warning", "daily", "voicepack.pack.rejected", "A VoicePack was rejected.", pack: "coahuilite.squeakyratkin:SR_Example", trailing: " reason=duplicate_key count=2"),
            D("error", "daily", "voicepack.resolver.rebuild_failed", "VoicePack resolver rebuild failed.", trailing: " ex_type=System.Exception ex_msg=resolver%20failed"),
            D("warning", "daily", "voicepack.target.rejected", "A Xenotype VoicePack target was rejected.", target: "target-1", trailing: " reason=reason-1"),
            D("warning", "dev_only", "xenotype.discovery.unavailable", "Xenotype discovery is unavailable.", trailing: " reason=HAR%20not%20loaded"),
            D("warning", "dev_only", "xenotype.discovery.failed", "Xenotype display discovery failed.", trailing: " ex_type=System.Exception ex_msg=discovery%20failed"),
            D("info", "dev_only", "xenotype.discovery.candidate", "A HAR Xenotype discovery candidate was evaluated.", target: "Ratkin", trailing: " reason=har source=har enabled=true"),
            D("error", "daily", "trigger.attempt.failed", "Squeak trigger attempt failed.", action: "Select", trailing: " ex_type=System.Exception ex_msg=trigger%20failed"),
            D("warning", "daily", "audio.dispatch.no_sound", "No fallback SoundDef was found.", action: "Move"),
            D("error", "daily", "audio.dispatch.failed", "Squeak audio dispatch failed.", action: "Attack", trailing: " sound=SR_Attack_1 ex_type=System.Exception ex_msg=dispatch%20failed"),
            D("info", "dev_only", "audio.dispatch.ok", "Squeak audio dispatched.", action: "Select", target: "12345", trailing: " sound=SR_OfficialExample_Race_Select suppressed_detail=0 pawn=Mousy pawn_id=Thing_Ratkin12345"),
            D("info", "dev_only", "trigger.outcome.summary", "Squeak trigger outcome summary was recorded.", trailing: " dispatched=7 suppressed_detail=2"),
            D("error", "daily", "hook.attack.unavailable", "Attack squeak hook is unavailable."),
            D("warning", "dev_only", "hook.attack.target_skipped", "An Attack hook target was skipped.", target: "999", trailing: " reason=not%20player%20ordered"),
            D("error", "daily", "hook.mental_break.unavailable", "Mental-break squeak hook is unavailable."),
            D("warning", "dev_only", "diagnostics.hook.unavailable", "Diagnostics overlay hook is unavailable."),
            D("warning", "daily", "diagnostics.start.failed", "Diagnostics overlay could not start."),
            D("info", "dev_only", "devtools.overlay.changed", "Diagnostics overlay state changed.", trailing: " enabled=true"),
            D("info", "dev_only", "devtools.camera_indicator.changed", "Camera indicator state changed.", trailing: " enabled=false"),
            D("warning", "daily", "devtools.workbench.open_failed", "Animal Voice Workbench could not be opened.", trailing: " ex_type=System.Exception ex_msg=workbench%20failed"));
    }

    private static void VerifyOnceSemantics()
    {
        Reset(SqueakDevLoggingMode.Enabled);
        SqueakLog.PackRejected("p1", 1);
        SqueakLog.PackRejected("p1", 2);
        SqueakLog.PackRejected("p2", 1);
        AssertLines(nameof(VerifyOnceSemantics),
            D("warning", "daily", "voicepack.pack.rejected", "A VoicePack was rejected.", pack: "p1", trailing: " reason=duplicate_key count=1"),
            D("warning", "daily", "voicepack.pack.rejected", "A VoicePack was rejected.", pack: "p2", trailing: " reason=duplicate_key count=1"));

        SqueakLog.ResetSession();
        Verse.Log.Reset();
        SqueakLog.PackRejected("p1", 1);
        AssertLines(nameof(VerifyOnceSemantics) + " reset", D("warning", "daily", "voicepack.pack.rejected", "A VoicePack was rejected.", pack: "p1", trailing: " reason=duplicate_key count=1"));
    }

    private static void VerifyOnceLimit()
    {
        Reset(SqueakDevLoggingMode.Enabled);
        for (int i = 0; i < 1024; i++) SqueakLog.PackRejected("cap" + i, 1);
        SqueakLog.PackRejected("p1", 1);
        SqueakLog.PackRejected("cap0", 1);

        AssertEqual(1026, Verse.Log.Captured.Count, nameof(VerifyOnceLimit) + " count");
        AssertEqual(D("warning", "daily", "voicepack.pack.rejected", "A VoicePack was rejected.", pack: "cap0", trailing: " reason=duplicate_key count=1"), Format(Verse.Log.Captured[0]), nameof(VerifyOnceLimit) + " first");
        AssertEqual(D("warning", "daily", "voicepack.pack.rejected", "A VoicePack was rejected.", pack: "cap1023", trailing: " reason=duplicate_key count=1"), Format(Verse.Log.Captured[1023]), nameof(VerifyOnceLimit) + " limit");
        AssertEqual(D("warning", "daily", "voicepack.pack.rejected", "A VoicePack was rejected.", pack: "p1", trailing: " reason=duplicate_key count=1"), Format(Verse.Log.Captured[1024]), nameof(VerifyOnceLimit) + " reclaimed");
        AssertEqual(D("warning", "daily", "voicepack.pack.rejected", "A VoicePack was rejected.", pack: "cap0", trailing: " reason=duplicate_key count=1"), Format(Verse.Log.Captured[1025]), nameof(VerifyOnceLimit) + " cleared prior key");
    }

    private static void VerifyDisabledModeGate()
    {
        Reset(SqueakDevLoggingMode.Disabled);
        SqueakLog.StartupIdentity();
        SqueakLog.AudioDispatchOk("Select", "1", "s", 0, "P", "Thing_Ratkin1");
        SqueakLog.HookAttackUnavailable();
        SqueakLog.OverlayChanged(true);
        SqueakLog.PackRejected("p1", 1);

        AssertLines(nameof(VerifyDisabledModeGate),
            Human("info", $"Squeaky Ratkin started with {Build} build {BuildId}."),
            Human("error", "Attack squeak hook is unavailable."),
            Human("warning", "A VoicePack was rejected."));
    }

    private static void VerifySilentFailureBoundary()
    {
        Reset(SqueakDevLoggingMode.Enabled);
        MethodInfo? emit = typeof(SqueakLog).GetMethod("Emit", BindingFlags.NonPublic | BindingFlags.Static);
        if (emit == null)
        {
            Fail(nameof(VerifySilentFailureBoundary) + ": Emit method was not found.");
            return;
        }

        try
        {
            emit.Invoke(null, new object[] { (SqueakLogEvent)999, default(SqueakLogData), false });
        }
        catch (Exception ex)
        {
            Fail(nameof(VerifySilentFailureBoundary) + ": Emit escaped " + ex.GetType().FullName + ".");
        }

        AssertEqual(0, Verse.Log.Captured.Count, nameof(VerifySilentFailureBoundary) + " output");
    }

    private static void VerifyEncodingAndExceptionMetadata()
    {
        Reset(SqueakDevLoggingMode.Enabled);
        SqueakLog.SettingsOpenFailed(CreateNestedException());
        SqueakLog.AudioDispatchFailed("Attack", "SR_Attack_1", new Exception(new string('a', 300)));
        SqueakLog.TargetRejected("t 1", "N/A");
        SqueakLog.AudioNoSound("中文");
        SqueakLog.XenotypeDiscoveryCandidate("Ratkin", "har", false);
        SqueakLog.PackRejected("p a+b?c", 1);

        AssertLines(nameof(VerifyEncodingAndExceptionMetadata),
            D("warning", "daily", "settings.open.failed", "Mod Settings could not be opened.", trailing: " ex_type=System.ApplicationException ex_inner=System.InvalidOperationException ex_site=SqueakyRatkin.Characterization.Program.CreateNestedException ex_msg=boom%20at%20%3Cpath%3E%20Mods%5Cfile.c%3Cpath%3E%20second%20line"),
            D("error", "daily", "audio.dispatch.failed", "Squeak audio dispatch failed.", action: "Attack", trailing: " sound=SR_Attack_1 ex_type=System.Exception ex_msg=" + new string('a', 256)),
            D("warning", "daily", "voicepack.target.rejected", "A Xenotype VoicePack target was rejected.", target: "t%201", trailing: " reason=-"),
            D("warning", "daily", "audio.dispatch.no_sound", "No fallback SoundDef was found.", action: "%E4%B8%AD%E6%96%87"),
            D("info", "dev_only", "xenotype.discovery.candidate", "A HAR Xenotype discovery candidate was evaluated.", target: "Ratkin", trailing: " reason=har source=har enabled=false"),
            D("warning", "daily", "voicepack.pack.rejected", "A VoicePack was rejected.", pack: "p%20a+b%3Fc", trailing: " reason=duplicate_key count=1"));

        AssertEqual("-", SqueakLogText.PercentEncode(null), nameof(VerifyEncodingAndExceptionMetadata) + " null encoding");
        AssertEqual("-", SqueakLogText.PercentEncode("N/A"), nameof(VerifyEncodingAndExceptionMetadata) + " N/A encoding");
        AssertEqual("a+b%3Fc", SqueakLogText.PercentEncode("a+b?c"), nameof(VerifyEncodingAndExceptionMetadata) + " reserved encoding");
        AssertEqual(256, SqueakLogText.SanitizeExceptionMessage(new string('a', 300)).Length, nameof(VerifyEncodingAndExceptionMetadata) + " exception truncation");
    }

    private static void VerifyInvalidLoggingModeFallsBackToAuto()
    {
        Reset((SqueakDevLoggingMode)999);
        AssertEqual(SqueakDevLoggingMode.Auto, SqueakLog.Mode, nameof(VerifyInvalidLoggingModeFallsBackToAuto) + " mode");
        AssertEqual(AutoEnablesDevLogging, SqueakLog.EffectiveDevLogging, nameof(VerifyInvalidLoggingModeFallsBackToAuto) + " effective mode");
        SqueakLog.StartupReady(1);

        if (AutoEnablesDevLogging)
            AssertLines(nameof(VerifyInvalidLoggingModeFallsBackToAuto), D("info", "daily", "mod.start.ready", "Squeaky Ratkin startup completed.", trailing: " count=1"));
        else
            AssertLines(nameof(VerifyInvalidLoggingModeFallsBackToAuto), Human("info", "Squeaky Ratkin startup completed."));
    }

    /// <summary>srdiag v2 (0.3.1 wave 2c): fmt=2 header, fixed v2 core order with race/[xenotype],
    /// string action keys, tier, settings.origin, and the log-v2 once domain. All v1 asserts above
    /// are untouched and re-verify that the 28-event fmt=1 bytes are unchanged.</summary>
    private static void VerifyV2Protocol()
    {
        Reset(SqueakDevLoggingMode.Enabled);
        SqueakLog.SettingsOrigin(SqueakSettingsOrigin.FreshCreated);
        SqueakLog.AudioRouteSelected("Select", "Ratkin", null, "12345", "SR_OfficialExample_Race_Select", "race_pack", "coahuilite.squeakyratkin:SR_OfficialExample_Race");
        SqueakLog.AudioRouteSelected("coahuilite.squeakyratkin.external_action", "Ratkin", "Baseliner", "777", "SR_Baseliner_Select", "xenotype_pack", "coahuilite.squeakyratkin:SR_Baseliner");
        SqueakLog.AudioRouteSelected("Move", "Ratkin", null, "1", "SR_Move_1", "vanilla", null);
        SqueakLog.FallbackProfileStoreFailed("Ratkin", new Exception("profile write failed"));
        SqueakLog.HookMentalFitUnavailable();

        AssertLines(nameof(VerifyV2Protocol) + " enabled",
            V2("info", "daily", "settings.origin", "Mod settings origin: FreshCreated.", trailing: " settings_origin=FreshCreated"),
            V2("info", "dev_only", "audio.route.selected", "Squeak audio route was selected.", action: "Select", target: "12345", pack: "coahuilite.squeakyratkin:SR_OfficialExample_Race", race: "Ratkin", trailing: " sound=SR_OfficialExample_Race_Select tier=race_pack"),
            V2("info", "dev_only", "audio.route.selected", "Squeak audio route was selected.", action: "coahuilite.squeakyratkin.external_action", target: "777", pack: "coahuilite.squeakyratkin:SR_Baseliner", race: "Ratkin", xenotype: "Baseliner", trailing: " sound=SR_Baseliner_Select tier=xenotype_pack"),
            V2("info", "dev_only", "audio.route.selected", "Squeak audio route was selected.", action: "Move", target: "1", pack: "-", race: "Ratkin", trailing: " sound=SR_Move_1 tier=vanilla"),
            V2("warning", "dev_only", "fallback.profile.store_failed", "Fallback profile store operation failed.", race: "Ratkin", trailing: " ex_type=System.Exception ex_msg=profile%20write%20failed"),
            V2("error", "daily", "hook.mental_fit.unavailable", "Baby-fits squeak hook is unavailable."));
        // log-v2 once: the first settings.origin claim wins per session; ResetSession reopens the domain.
        Reset(SqueakDevLoggingMode.Enabled);
        SqueakLog.SettingsOrigin(SqueakSettingsOrigin.LoadedFromFile);
        SqueakLog.SettingsOrigin(SqueakSettingsOrigin.FreshCreated);
        AssertLines(nameof(VerifyV2Protocol) + " once",
            V2("info", "daily", "settings.origin", "Mod settings origin: LoadedFromFile.", trailing: " settings_origin=LoadedFromFile"));
        SqueakLog.ResetSession();
        Verse.Log.Reset();
        SqueakLog.SettingsOrigin(SqueakSettingsOrigin.FreshCreated);
        AssertLines(nameof(VerifyV2Protocol) + " once reset",
            V2("info", "daily", "settings.origin", "Mod settings origin: FreshCreated.", trailing: " settings_origin=FreshCreated"));

        // log-v1 and log-v2 once domains are independent: identical payload fields claim separate keys.
        Reset(SqueakDevLoggingMode.Enabled);
        SqueakLog.SettingsOrigin(SqueakSettingsOrigin.FreshCreated);
        SqueakLog.PackRejected("p1", 1);
        SqueakLog.PackRejected("p1", 2);
        SqueakLog.SettingsOrigin(SqueakSettingsOrigin.LoadedFromFile);
        AssertLines(nameof(VerifyV2Protocol) + " independent once",
            V2("info", "daily", "settings.origin", "Mod settings origin: FreshCreated.", trailing: " settings_origin=FreshCreated"),
            D("warning", "daily", "voicepack.pack.rejected", "A VoicePack was rejected.", pack: "p1", trailing: " reason=duplicate_key count=1"));

        // v2 values flow through the same percent-encoding/sanitization rules as v1.
        Reset(SqueakDevLoggingMode.Enabled);
        SqueakLog.AudioRouteSelected("some package.action", "Ra tin", null, "t 1", "SR_1", "race_pack", null);
        AssertLines(nameof(VerifyV2Protocol) + " encoding",
            V2("info", "dev_only", "audio.route.selected", "Squeak audio route was selected.", action: "some%20package.action", target: "t%201", race: "Ra%20tin", trailing: " sound=SR_1 tier=race_pack"));

        // Gating: v2 Daily keeps the human-only shape while detailed logging is ineffective; v2 DevOnly is silent.
        Reset(SqueakDevLoggingMode.Disabled);
        SqueakLog.SettingsOrigin(SqueakSettingsOrigin.FreshCreated);
        SqueakLog.AudioRouteSelected("Select", "Ratkin", null, "1", "SR_1", "race_pack", null);
        AssertLines(nameof(VerifyV2Protocol) + " disabled",
            Human("info", "Mod settings origin: FreshCreated."));
    }

    private static Exception CreateNestedException()
    {
        try
        {
            throw new InvalidOperationException("inner");
        }
        catch (Exception inner)
        {
            try
            {
                throw new ApplicationException("boom at C:\\My Mods\\file.cs:42\nsecond line", inner);
            }
            catch (Exception outer)
            {
                return outer;
            }
        }
    }

    private static void Reset(SqueakDevLoggingMode mode)
    {
        SqueakLog.Configure(mode);
        SqueakLog.ResetSession();
        Verse.Log.Reset();
    }

    private static string D(string level, string visibility, string eventId, string human, string action = "-", string target = "-", string pack = "-", string trailing = "")
    {
        return level + "|" + Prefix + human + " || srdiag fmt=1 lvl=" + level + " vis=" + visibility + " evt=" + eventId + " action=" + action + " target=" + target + " pack=" + pack + " build=" + Build + " build_id=" + BuildId + trailing;
    }

    private static string Human(string level, string text) => level + "|" + Prefix + text;
    private static string Format(Verse.Log.Entry entry) => entry.Level + "|" + entry.Text;

    /// <summary>v2 expected line: fixed core order fmt=2 lvl vis evt action target pack race [xenotype] build build_id.
    /// xenotype = null omits the optional field; pass "-" explicitly to assert an explicit dash.</summary>
    private static string V2(string level, string visibility, string eventId, string human, string action = "-", string target = "-", string pack = "-", string race = "-", string? xenotype = null, string trailing = "")
    {
        return level + "|" + Prefix + human + " || srdiag fmt=2 lvl=" + level + " vis=" + visibility + " evt=" + eventId + " action=" + action + " target=" + target + " pack=" + pack + " race=" + race + (xenotype == null ? "" : " xenotype=" + xenotype) + " build=" + Build + " build_id=" + BuildId + trailing;
    }

    private static void AssertLines(string name, params string[] expected)
    {
        if (Verse.Log.Captured.Count != expected.Length)
        {
            Fail(name + ": expected " + expected.Length + " lines, got " + Verse.Log.Captured.Count + ".");
            return;
        }

        for (int i = 0; i < expected.Length; i++)
            AssertEqual(expected[i], Format(Verse.Log.Captured[i]), name + " line " + i);
    }

    private static void AssertEqual<T>(T expected, T actual, string name)
    {
        if (!Equals(expected, actual))
            Fail(name + ": expected '" + expected + "', got '" + actual + "'.");
    }

    private static void Fail(string message)
    {
        failures++;
        Console.Error.WriteLine(message);
    }
}
