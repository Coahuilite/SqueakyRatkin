using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Verse;

namespace SqueakyRatkin;

internal enum SqueakLogVisibility { Daily, DevOnly }
internal enum SqueakLogLevel { Info, Warning, Error }
internal enum SqueakLogEvent { ModStartIdentity, ModStartReady, LoggingModeEnabled, LoggingModeDisabled, LoggingModeAutoEnabled, LoggingModeAutoDisabled, SettingsOpenApiUnavailable, SettingsOpenFailed, CatalogRefreshFailed, PackRejected, ResolverRebuildFailed, TargetRejected, XenotypeDiscoveryUnavailable, XenotypeDiscoveryFailed, XenotypeDiscoveryCandidate, TriggerAttemptFailed, AudioNoSound, AudioDispatchFailed, AudioDispatchOk, TriggerOutcomeSummary, HookAttackUnavailable, HookAttackTargetSkipped, HookMentalBreakUnavailable, DiagnosticsHookUnavailable, DiagnosticsStartFailed, OverlayChanged, CameraChanged, WorkbenchOpenFailed }

internal readonly struct SqueakLogData
{
    internal readonly string? Action, Target, Pack, Reason, Sound, Source; internal readonly int? Count, Dispatched, SuppressedDetail; internal readonly bool? Enabled; internal readonly Exception? Exception;
    internal SqueakLogData(string? action = null, string? target = null, string? pack = null, string? reason = null, string? sound = null, string? source = null, int? count = null, int? dispatched = null, int? suppressedDetail = null, bool? enabled = null, Exception? exception = null)
    { Action = action; Target = target; Pack = pack; Reason = reason; Sound = sound; Source = source; Count = count; Dispatched = dispatched; SuppressedDetail = suppressedDetail; Enabled = enabled; Exception = exception; }
}

internal readonly struct SqueakLogDefinition
{
    internal readonly SqueakLogVisibility Visibility;
    internal readonly SqueakLogLevel Level;
    internal readonly string Human;

    internal SqueakLogDefinition(SqueakLogVisibility visibility, SqueakLogLevel level, string human)
    {
        Visibility = visibility;
        Level = level;
        Human = human;
    }
}

internal static class SqueakLogRegistry
{
    internal static SqueakLogDefinition Definition(SqueakLogEvent e, string build, string buildId) => e switch
    {
        SqueakLogEvent.ModStartIdentity => new(SqueakLogVisibility.Daily, SqueakLogLevel.Info, "Squeaky Ratkin started with " + build + " build " + buildId + "."),
        SqueakLogEvent.ModStartReady => new(SqueakLogVisibility.Daily, SqueakLogLevel.Info, "Squeaky Ratkin startup completed."),
        SqueakLogEvent.LoggingModeEnabled => new(SqueakLogVisibility.Daily, SqueakLogLevel.Info, "Detailed diagnostic logging is enabled."),
        SqueakLogEvent.LoggingModeDisabled => new(SqueakLogVisibility.Daily, SqueakLogLevel.Info, "Detailed diagnostic logging is disabled."),
        SqueakLogEvent.LoggingModeAutoEnabled => new(SqueakLogVisibility.Daily, SqueakLogLevel.Info, "Detailed diagnostic logging is enabled by Auto mode."),
        SqueakLogEvent.LoggingModeAutoDisabled => new(SqueakLogVisibility.Daily, SqueakLogLevel.Info, "Detailed diagnostic logging is disabled by Auto mode."),
        SqueakLogEvent.SettingsOpenApiUnavailable => new(SqueakLogVisibility.Daily, SqueakLogLevel.Warning, "Mod Settings API is unavailable."),
        SqueakLogEvent.SettingsOpenFailed => new(SqueakLogVisibility.Daily, SqueakLogLevel.Warning, "Mod Settings could not be opened."),
        SqueakLogEvent.CatalogRefreshFailed => new(SqueakLogVisibility.Daily, SqueakLogLevel.Error, "VoicePack catalog refresh failed."),
        SqueakLogEvent.PackRejected => new(SqueakLogVisibility.Daily, SqueakLogLevel.Warning, "A VoicePack was rejected."),
        SqueakLogEvent.ResolverRebuildFailed => new(SqueakLogVisibility.Daily, SqueakLogLevel.Error, "VoicePack resolver rebuild failed."),
        SqueakLogEvent.TargetRejected => new(SqueakLogVisibility.Daily, SqueakLogLevel.Warning, "A Xenotype VoicePack target was rejected."),
        SqueakLogEvent.XenotypeDiscoveryUnavailable => new(SqueakLogVisibility.DevOnly, SqueakLogLevel.Warning, "Xenotype discovery is unavailable."),
        SqueakLogEvent.XenotypeDiscoveryFailed => new(SqueakLogVisibility.DevOnly, SqueakLogLevel.Warning, "Xenotype display discovery failed."),
        SqueakLogEvent.XenotypeDiscoveryCandidate => new(SqueakLogVisibility.DevOnly, SqueakLogLevel.Info, "A HAR Xenotype discovery candidate was evaluated."),
        SqueakLogEvent.TriggerAttemptFailed => new(SqueakLogVisibility.Daily, SqueakLogLevel.Error, "Squeak trigger attempt failed."),
        SqueakLogEvent.AudioNoSound => new(SqueakLogVisibility.Daily, SqueakLogLevel.Warning, "No fallback SoundDef was found."),
        SqueakLogEvent.AudioDispatchFailed => new(SqueakLogVisibility.Daily, SqueakLogLevel.Error, "Squeak audio dispatch failed."),
        SqueakLogEvent.AudioDispatchOk => new(SqueakLogVisibility.DevOnly, SqueakLogLevel.Info, "Squeak audio dispatched."),
        SqueakLogEvent.TriggerOutcomeSummary => new(SqueakLogVisibility.DevOnly, SqueakLogLevel.Info, "Squeak trigger outcome summary was recorded."),
        SqueakLogEvent.HookAttackUnavailable => new(SqueakLogVisibility.Daily, SqueakLogLevel.Error, "Attack squeak hook is unavailable."),
        SqueakLogEvent.HookAttackTargetSkipped => new(SqueakLogVisibility.DevOnly, SqueakLogLevel.Warning, "An Attack hook target was skipped."),
        SqueakLogEvent.HookMentalBreakUnavailable => new(SqueakLogVisibility.Daily, SqueakLogLevel.Error, "Mental-break squeak hook is unavailable."),
        SqueakLogEvent.DiagnosticsHookUnavailable => new(SqueakLogVisibility.DevOnly, SqueakLogLevel.Warning, "Diagnostics overlay hook is unavailable."),
        SqueakLogEvent.DiagnosticsStartFailed => new(SqueakLogVisibility.Daily, SqueakLogLevel.Warning, "Diagnostics overlay could not start."),
        SqueakLogEvent.OverlayChanged => new(SqueakLogVisibility.DevOnly, SqueakLogLevel.Info, "Diagnostics overlay state changed."),
        SqueakLogEvent.CameraChanged => new(SqueakLogVisibility.DevOnly, SqueakLogLevel.Info, "Camera indicator state changed."),
        SqueakLogEvent.WorkbenchOpenFailed => new(SqueakLogVisibility.Daily, SqueakLogLevel.Warning, "Animal Voice Workbench could not be opened."),
        _ => throw new ArgumentOutOfRangeException(nameof(e))
    };

    internal static string EventId(SqueakLogEvent e) => e switch
    {
        SqueakLogEvent.ModStartIdentity => "mod.start.identity",
        SqueakLogEvent.ModStartReady => "mod.start.ready",
        SqueakLogEvent.LoggingModeEnabled => "logging.mode.enabled",
        SqueakLogEvent.LoggingModeDisabled => "logging.mode.disabled",
        SqueakLogEvent.LoggingModeAutoEnabled => "logging.mode.auto_enabled",
        SqueakLogEvent.LoggingModeAutoDisabled => "logging.mode.auto_disabled",
        SqueakLogEvent.SettingsOpenApiUnavailable => "settings.open.api_unavailable",
        SqueakLogEvent.SettingsOpenFailed => "settings.open.failed",
        SqueakLogEvent.CatalogRefreshFailed => "voicepack.catalog.refresh_failed",
        SqueakLogEvent.PackRejected => "voicepack.pack.rejected",
        SqueakLogEvent.ResolverRebuildFailed => "voicepack.resolver.rebuild_failed",
        SqueakLogEvent.TargetRejected => "voicepack.target.rejected",
        SqueakLogEvent.XenotypeDiscoveryUnavailable => "xenotype.discovery.unavailable",
        SqueakLogEvent.XenotypeDiscoveryFailed => "xenotype.discovery.failed",
        SqueakLogEvent.XenotypeDiscoveryCandidate => "xenotype.discovery.candidate",
        SqueakLogEvent.TriggerAttemptFailed => "trigger.attempt.failed",
        SqueakLogEvent.AudioNoSound => "audio.dispatch.no_sound",
        SqueakLogEvent.AudioDispatchFailed => "audio.dispatch.failed",
        SqueakLogEvent.AudioDispatchOk => "audio.dispatch.ok",
        SqueakLogEvent.TriggerOutcomeSummary => "trigger.outcome.summary",
        SqueakLogEvent.HookAttackUnavailable => "hook.attack.unavailable",
        SqueakLogEvent.HookAttackTargetSkipped => "hook.attack.target_skipped",
        SqueakLogEvent.HookMentalBreakUnavailable => "hook.mental_break.unavailable",
        SqueakLogEvent.DiagnosticsHookUnavailable => "diagnostics.hook.unavailable",
        SqueakLogEvent.DiagnosticsStartFailed => "diagnostics.start.failed",
        SqueakLogEvent.OverlayChanged => "devtools.overlay.changed",
        SqueakLogEvent.CameraChanged => "devtools.camera_indicator.changed",
        SqueakLogEvent.WorkbenchOpenFailed => "devtools.workbench.open_failed",
        _ => throw new ArgumentOutOfRangeException(nameof(e))
    };
}

internal static class SqueakLogOnce
{
    private const int Limit = 1024;
    private static readonly object sync = new();
    private static readonly HashSet<string> keys = new(StringComparer.Ordinal);

    internal static void Reset()
    {
        lock (sync) keys.Clear();
    }

    internal static bool Claim(SqueakLogEvent evt, SqueakLogData data)
    {
        string key = "coahuilite.squeakyratkin|log-v1|" + evt + "|" + SqueakLogFormatter.Value(data.Action) + "|" + SqueakLogFormatter.Value(data.Target) + "|" + SqueakLogFormatter.Value(data.Pack) + "|" + SqueakLogFormatter.Value(data.Reason) + "|" + (data.Exception?.GetType().FullName ?? "-");
        lock (sync)
        {
            if (keys.Contains(key)) return false;
            if (keys.Count >= Limit) keys.Clear();
            keys.Add(key);
            return true;
        }
    }
}

internal static class SqueakLogFormatter
{
    internal static string Suffix(SqueakLogEvent evt, SqueakLogDefinition definition, SqueakLogData data, string build, string buildId)
    {
        StringBuilder builder = new("srdiag fmt=1 lvl=");
        builder.Append(definition.Level.ToString().ToLowerInvariant()).Append(" vis=").Append(definition.Visibility == SqueakLogVisibility.Daily ? "daily" : "dev_only").Append(" evt=").Append(Value(SqueakLogRegistry.EventId(evt))).Append(" action=").Append(Value(data.Action)).Append(" target=").Append(Value(data.Target)).Append(" pack=").Append(Value(data.Pack)).Append(" build=").Append(Value(build)).Append(" build_id=").Append(Value(buildId));
        Add(builder, "reason", data.Reason);
        Add(builder, "sound", data.Sound);
        Add(builder, "source", data.Source);
        Add(builder, "count", data.Count);
        Add(builder, "dispatched", data.Dispatched);
        Add(builder, "suppressed_detail", data.SuppressedDetail);
        Add(builder, "enabled", data.Enabled);
        if (data.Exception != null)
        {
            var site = data.Exception.TargetSite;
            Add(builder, "ex_type", data.Exception.GetType().FullName);
            Add(builder, "ex_inner", data.Exception.InnerException?.GetType().FullName);
            Add(builder, "ex_site", site == null ? null : site.DeclaringType?.FullName + "." + site.Name);
            Add(builder, "ex_msg", SqueakLogText.SanitizeExceptionMessage(data.Exception.Message));
        }

        return builder.ToString();
    }

    internal static string Value(object? value)
    {
        if (value == null) return "-";
        if (value is bool boolean) return boolean ? "true" : "false";
        return SqueakLogText.PercentEncode(value is IFormattable formattable ? formattable.ToString(null, CultureInfo.InvariantCulture) : value.ToString());
    }

    private static void Add(StringBuilder builder, string key, object? value)
    {
        if (value != null) builder.Append(' ').Append(key).Append('=').Append(Value(value));
    }
}

internal static class SqueakLogText
{
    private static readonly Regex Path = new(@"(?ix)(?:\\\\[?.][\\/][^\s]+|\\\\[^\\/\s]+[\\/][^\s]+|[a-z]:[\\/][^\s]+|[a-z]:(?![\\/])[^\s:]+|(?<![\\\w])\\(?![\\?.])[^\\\s]+(?:\\[^\s]+)*|file:(?://)?[^\s]+|(?<!\w)/(?:[^\s]+)|(?<!\w)\.\.?[\\/][^\s]+)", RegexOptions.Compiled);
    internal static string SanitizeExceptionMessage(string? text) { if (string.IsNullOrEmpty(text)) return "-"; string clean = new string(Array.FindAll(text!.Replace('\r', ' ').Replace('\n', ' ').ToCharArray(), c => !char.IsControl(c))); clean = Path.Replace(clean, "<path>"); return clean.Length > 256 ? clean.Substring(0, 256) : clean; }
    internal static string PercentEncode(string? text) { if (string.IsNullOrEmpty(text) || text == "N/A") return "-"; byte[] bytes = Encoding.UTF8.GetBytes(text); StringBuilder builder = new(); foreach (byte value in bytes) { bool safe = value >= 'A' && value <= 'Z' || value >= 'a' && value <= 'z' || value >= '0' && value <= '9' || value == '.' || value == '_' || value == '~' || value == ':' || value == '/' || value == '@' || value == '+' || value == '-'; if (safe) builder.Append((char)value); else builder.Append('%').Append(value.ToString("X2", CultureInfo.InvariantCulture)); } return builder.ToString(); }
}

internal static class SqueakLogSink
{
    internal static void Emit(SqueakLogLevel level, string text)
    {
        if (level == SqueakLogLevel.Warning) Log.Warning(text);
        else if (level == SqueakLogLevel.Error) Log.Error(text);
        else Log.Message(text);
    }
}
