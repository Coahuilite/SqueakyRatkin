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
        SqueakLog.AudioDispatchOk("Select", "12345", "SR_OfficialExample_Race_Select", 0);
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
            D("info", "dev_only", "audio.dispatch.ok", "Squeak audio dispatched.", action: "Select", target: "12345", trailing: " sound=SR_OfficialExample_Race_Select suppressed_detail=0"),
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
        SqueakLog.AudioDispatchOk("Select", "1", "s", 0);
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
