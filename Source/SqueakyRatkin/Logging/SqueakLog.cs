using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Verse;

namespace SqueakyRatkin;

public enum SqueakDevLoggingMode { Auto = 0, Enabled = 1, Disabled = 2 }
internal enum SqueakLogVisibility { Daily, DevOnly }
internal enum SqueakLogLevel { Info, Warning, Error }
internal enum SqueakLogEvent { ModStartIdentity, ModStartReady, LoggingModeEnabled, LoggingModeDisabled, LoggingModeAutoEnabled, LoggingModeAutoDisabled, SettingsOpenApiUnavailable, SettingsOpenFailed, CatalogRefreshFailed, PackRejected, ResolverRebuildFailed, TargetRejected, XenotypeDiscoveryUnavailable, XenotypeDiscoveryFailed, XenotypeDiscoveryCandidate, TriggerAttemptFailed, AudioNoSound, AudioDispatchFailed, AudioDispatchOk, TriggerOutcomeSummary, HookAttackUnavailable, HookAttackTargetSkipped, HookMentalBreakUnavailable, DiagnosticsHookUnavailable, DiagnosticsStartFailed, OverlayChanged, CameraChanged, WorkbenchOpenFailed }

internal readonly struct SqueakLogData
{
    internal readonly string? Action, Target, Pack, Reason, Sound, Source; internal readonly int? Count, Dispatched, SuppressedDetail; internal readonly bool? Enabled; internal readonly Exception? Exception;
    internal SqueakLogData(string? action = null, string? target = null, string? pack = null, string? reason = null, string? sound = null, string? source = null, int? count = null, int? dispatched = null, int? suppressedDetail = null, bool? enabled = null, Exception? exception = null)
    { Action = action; Target = target; Pack = pack; Reason = reason; Sound = sound; Source = source; Count = count; Dispatched = dispatched; SuppressedDetail = suppressedDetail; Enabled = enabled; Exception = exception; }
}

/// <summary>Closed logging facade. Event schema, human text, and protocol emission are not writable by business code.</summary>
public static class SqueakLog
{
    private const string Prefix = "[SqueakyRatkin] "; private const int OnceLimit = 1024;
    private static readonly object onceLock = new(); private static readonly HashSet<string> onceKeys = new(StringComparer.Ordinal);
    private static SqueakDevLoggingMode mode; private static string build = "dev", buildId = "unknown";
    public static bool EffectiveDevLogging { get; private set; }
    public static bool ShouldEmitDev => EffectiveDevLogging;
    public static SqueakDevLoggingMode Mode => mode;

    public static void Configure(SqueakDevLoggingMode requested)
    {
        mode = Enum.IsDefined(typeof(SqueakDevLoggingMode), requested) ? requested : SqueakDevLoggingMode.Auto;
#if SQUEAKY_DEV
        EffectiveDevLogging = mode != SqueakDevLoggingMode.Disabled;
#else
        EffectiveDevLogging = mode == SqueakDevLoggingMode.Enabled;
#endif
        Assembly assembly = typeof(SqueakyRatkinMod).Assembly; string informational = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? assembly.GetName().Version?.ToString() ?? "unknown";
#if SQUEAKY_STEAM
        build = "steam"; buildId = informational.Split('+')[0];
#elif SQUEAKY_GITHUB
        build = "github"; buildId = informational;
#else
        build = "dev"; buildId = informational;
#endif
    }
    public static void ResetSession() { lock (onceLock) onceKeys.Clear(); }
    public static void StartupIdentity() => Emit(SqueakLogEvent.ModStartIdentity, default, false);
    public static void StartupReady(int patchedMethods) => Emit(SqueakLogEvent.ModStartReady, new SqueakLogData(count: patchedMethods), false);
    public static void LoggingModeChanged(SqueakDevLoggingMode requested, bool enabled) => Emit(requested == SqueakDevLoggingMode.Enabled ? SqueakLogEvent.LoggingModeEnabled : requested == SqueakDevLoggingMode.Disabled ? SqueakLogEvent.LoggingModeDisabled : enabled ? SqueakLogEvent.LoggingModeAutoEnabled : SqueakLogEvent.LoggingModeAutoDisabled, new SqueakLogData(enabled: enabled), false);
    public static void SettingsOpenApiUnavailable() => Emit(SqueakLogEvent.SettingsOpenApiUnavailable, default, false);
    public static void SettingsOpenFailed(Exception ex) => Emit(SqueakLogEvent.SettingsOpenFailed, new SqueakLogData(exception: ex), true);
    public static void CatalogRefreshFailed(Exception ex) => Emit(SqueakLogEvent.CatalogRefreshFailed, new SqueakLogData(exception: ex), true);
    public static void PackRejected(string pack, int count) => Emit(SqueakLogEvent.PackRejected, new SqueakLogData(pack: pack, reason: "duplicate_key", count: count), true);
    public static void ResolverRebuildFailed(Exception ex) => Emit(SqueakLogEvent.ResolverRebuildFailed, new SqueakLogData(exception: ex), true);
    public static void TargetRejected(string target, string reason) => Emit(SqueakLogEvent.TargetRejected, new SqueakLogData(target: target, reason: reason), true);
    public static void XenotypeDiscoveryUnavailable(string reason) { if (!ShouldEmitDev) return; Emit(SqueakLogEvent.XenotypeDiscoveryUnavailable, new SqueakLogData(reason: reason), true); }
    public static void XenotypeDiscoveryFailed(Exception ex) { if (!ShouldEmitDev) return; Emit(SqueakLogEvent.XenotypeDiscoveryFailed, new SqueakLogData(exception: ex), true); }
    public static void XenotypeDiscoveryCandidate(string target, string source, bool retained) { if (!ShouldEmitDev) return; Emit(SqueakLogEvent.XenotypeDiscoveryCandidate, new SqueakLogData(target: target, reason: source, source: source, enabled: retained), true); }
    public static void TriggerAttemptFailed(string action, Exception ex) => Emit(SqueakLogEvent.TriggerAttemptFailed, new SqueakLogData(action: action, exception: ex), true);
    public static void AudioNoSound(string action) => Emit(SqueakLogEvent.AudioNoSound, new SqueakLogData(action: action), true);
    public static void AudioDispatchFailed(string action, string sound, Exception ex) => Emit(SqueakLogEvent.AudioDispatchFailed, new SqueakLogData(action: action, sound: sound, exception: ex), true);
    public static void AudioDispatchOk(string action, string target, string sound, int suppressed) { if (!ShouldEmitDev) return; Emit(SqueakLogEvent.AudioDispatchOk, new SqueakLogData(action: action, target: target, sound: sound, suppressedDetail: suppressed), false); }
    public static void TriggerOutcomeSummary(int dispatched, int suppressed) { if (!ShouldEmitDev) return; Emit(SqueakLogEvent.TriggerOutcomeSummary, new SqueakLogData(dispatched: dispatched, suppressedDetail: suppressed), false); }
    public static void HookAttackUnavailable() => Emit(SqueakLogEvent.HookAttackUnavailable, default, true);
    public static void HookAttackTargetSkipped(string target, string reason) { if (!ShouldEmitDev) return; Emit(SqueakLogEvent.HookAttackTargetSkipped, new SqueakLogData(target: target, reason: reason), true); }
    public static void HookMentalBreakUnavailable() => Emit(SqueakLogEvent.HookMentalBreakUnavailable, default, true);
    public static void DiagnosticsHookUnavailable() { if (!ShouldEmitDev) return; Emit(SqueakLogEvent.DiagnosticsHookUnavailable, default, true); }
    public static void DiagnosticsStartFailed() => Emit(SqueakLogEvent.DiagnosticsStartFailed, default, true);
    public static void OverlayChanged(bool enabled) { if (!ShouldEmitDev) return; Emit(SqueakLogEvent.OverlayChanged, new SqueakLogData(enabled: enabled), false); }
    public static void CameraChanged(bool enabled) { if (!ShouldEmitDev) return; Emit(SqueakLogEvent.CameraChanged, new SqueakLogData(enabled: enabled), false); }
    public static void WorkbenchOpenFailed(Exception ex) => Emit(SqueakLogEvent.WorkbenchOpenFailed, new SqueakLogData(exception: ex), true);

    private static void Emit(SqueakLogEvent evt, SqueakLogData data, bool once)
    {
        try { SqueakLogDefinition definition = Definition(evt); if (definition.Visibility == SqueakLogVisibility.DevOnly && !EffectiveDevLogging) return; if (once && !ClaimOnce(evt, data)) return; string text = Prefix + definition.Human + (EffectiveDevLogging ? " || " + Suffix(evt, definition, data) : ""); Sink(definition.Level, text); } catch { }
    }
    private static bool ClaimOnce(SqueakLogEvent evt, SqueakLogData data)
    {
        string key = "coahuilite.squeakyratkin|log-v1|" + evt + "|" + V(data.Action) + "|" + V(data.Target) + "|" + V(data.Pack) + "|" + V(data.Reason) + "|" + (data.Exception?.GetType().FullName ?? "-");
        lock (onceLock) { if (onceKeys.Contains(key)) return false; if (onceKeys.Count >= OnceLimit) onceKeys.Clear(); onceKeys.Add(key); return true; }
    }
    private static void Sink(SqueakLogLevel level, string text) { if (level == SqueakLogLevel.Warning) Log.Warning(text); else if (level == SqueakLogLevel.Error) Log.Error(text); else Log.Message(text); }
    private static SqueakLogDefinition Definition(SqueakLogEvent e) => e switch
    {
        SqueakLogEvent.ModStartIdentity => new(SqueakLogVisibility.Daily, SqueakLogLevel.Info, "Squeaky Ratkin started with " + build + " build " + buildId + "."), SqueakLogEvent.ModStartReady => new(SqueakLogVisibility.Daily, SqueakLogLevel.Info, "Squeaky Ratkin startup completed."),
        SqueakLogEvent.LoggingModeEnabled => new(SqueakLogVisibility.Daily, SqueakLogLevel.Info, "Detailed diagnostic logging is enabled."), SqueakLogEvent.LoggingModeDisabled => new(SqueakLogVisibility.Daily, SqueakLogLevel.Info, "Detailed diagnostic logging is disabled."), SqueakLogEvent.LoggingModeAutoEnabled => new(SqueakLogVisibility.Daily, SqueakLogLevel.Info, "Detailed diagnostic logging is enabled by Auto mode."), SqueakLogEvent.LoggingModeAutoDisabled => new(SqueakLogVisibility.Daily, SqueakLogLevel.Info, "Detailed diagnostic logging is disabled by Auto mode."),
        SqueakLogEvent.SettingsOpenApiUnavailable => new(SqueakLogVisibility.Daily, SqueakLogLevel.Warning, "Mod Settings API is unavailable."), SqueakLogEvent.SettingsOpenFailed => new(SqueakLogVisibility.Daily, SqueakLogLevel.Warning, "Mod Settings could not be opened."), SqueakLogEvent.CatalogRefreshFailed => new(SqueakLogVisibility.Daily, SqueakLogLevel.Error, "VoicePack catalog refresh failed."), SqueakLogEvent.PackRejected => new(SqueakLogVisibility.Daily, SqueakLogLevel.Warning, "A VoicePack was rejected."), SqueakLogEvent.ResolverRebuildFailed => new(SqueakLogVisibility.Daily, SqueakLogLevel.Error, "VoicePack resolver rebuild failed."), SqueakLogEvent.TargetRejected => new(SqueakLogVisibility.Daily, SqueakLogLevel.Warning, "A Xenotype VoicePack target was rejected."),
        SqueakLogEvent.XenotypeDiscoveryUnavailable => new(SqueakLogVisibility.DevOnly, SqueakLogLevel.Warning, "Xenotype discovery is unavailable."), SqueakLogEvent.XenotypeDiscoveryFailed => new(SqueakLogVisibility.DevOnly, SqueakLogLevel.Warning, "Xenotype display discovery failed."), SqueakLogEvent.XenotypeDiscoveryCandidate => new(SqueakLogVisibility.DevOnly, SqueakLogLevel.Info, "A HAR Xenotype discovery candidate was evaluated."), SqueakLogEvent.TriggerAttemptFailed => new(SqueakLogVisibility.Daily, SqueakLogLevel.Error, "Squeak trigger attempt failed."), SqueakLogEvent.AudioNoSound => new(SqueakLogVisibility.Daily, SqueakLogLevel.Warning, "No fallback SoundDef was found."), SqueakLogEvent.AudioDispatchFailed => new(SqueakLogVisibility.Daily, SqueakLogLevel.Error, "Squeak audio dispatch failed."), SqueakLogEvent.AudioDispatchOk => new(SqueakLogVisibility.DevOnly, SqueakLogLevel.Info, "Squeak audio dispatched."), SqueakLogEvent.TriggerOutcomeSummary => new(SqueakLogVisibility.DevOnly, SqueakLogLevel.Info, "Squeak trigger outcome summary was recorded."),
        SqueakLogEvent.HookAttackUnavailable => new(SqueakLogVisibility.Daily, SqueakLogLevel.Error, "Attack squeak hook is unavailable."), SqueakLogEvent.HookAttackTargetSkipped => new(SqueakLogVisibility.DevOnly, SqueakLogLevel.Warning, "An Attack hook target was skipped."), SqueakLogEvent.HookMentalBreakUnavailable => new(SqueakLogVisibility.Daily, SqueakLogLevel.Error, "Mental-break squeak hook is unavailable."), SqueakLogEvent.DiagnosticsHookUnavailable => new(SqueakLogVisibility.DevOnly, SqueakLogLevel.Warning, "Diagnostics overlay hook is unavailable."), SqueakLogEvent.DiagnosticsStartFailed => new(SqueakLogVisibility.Daily, SqueakLogLevel.Warning, "Diagnostics overlay could not start."), SqueakLogEvent.OverlayChanged => new(SqueakLogVisibility.DevOnly, SqueakLogLevel.Info, "Diagnostics overlay state changed."), SqueakLogEvent.CameraChanged => new(SqueakLogVisibility.DevOnly, SqueakLogLevel.Info, "Camera indicator state changed."), SqueakLogEvent.WorkbenchOpenFailed => new(SqueakLogVisibility.Daily, SqueakLogLevel.Warning, "Animal Voice Workbench could not be opened."), _ => throw new ArgumentOutOfRangeException(nameof(e)) };
    private static string Suffix(SqueakLogEvent evt, SqueakLogDefinition d, SqueakLogData x) { StringBuilder b = new("srdiag fmt=1 lvl="); b.Append(d.Level.ToString().ToLowerInvariant()).Append(" vis=").Append(d.Visibility == SqueakLogVisibility.Daily ? "daily" : "dev_only").Append(" evt=").Append(V(EventId(evt))).Append(" action=").Append(V(x.Action)).Append(" target=").Append(V(x.Target)).Append(" pack=").Append(V(x.Pack)).Append(" build=").Append(V(build)).Append(" build_id=").Append(V(buildId)); Add(b, "reason", x.Reason); Add(b, "sound", x.Sound); Add(b, "source", x.Source); Add(b, "count", x.Count); Add(b, "dispatched", x.Dispatched); Add(b, "suppressed_detail", x.SuppressedDetail); Add(b, "enabled", x.Enabled); if (x.Exception != null) { MethodBase? site = x.Exception.TargetSite; Add(b, "ex_type", x.Exception.GetType().FullName); Add(b, "ex_inner", x.Exception.InnerException?.GetType().FullName); Add(b, "ex_site", site == null ? null : site.DeclaringType?.FullName + "." + site.Name); Add(b, "ex_msg", SqueakLogText.SanitizeExceptionMessage(x.Exception.Message)); } return b.ToString(); }
    private static string EventId(SqueakLogEvent e) => e switch
    {
        SqueakLogEvent.ModStartIdentity => "mod.start.identity", SqueakLogEvent.ModStartReady => "mod.start.ready", SqueakLogEvent.LoggingModeEnabled => "logging.mode.enabled", SqueakLogEvent.LoggingModeDisabled => "logging.mode.disabled", SqueakLogEvent.LoggingModeAutoEnabled => "logging.mode.auto_enabled", SqueakLogEvent.LoggingModeAutoDisabled => "logging.mode.auto_disabled",
        SqueakLogEvent.SettingsOpenApiUnavailable => "settings.open.api_unavailable", SqueakLogEvent.SettingsOpenFailed => "settings.open.failed", SqueakLogEvent.CatalogRefreshFailed => "voicepack.catalog.refresh_failed", SqueakLogEvent.PackRejected => "voicepack.pack.rejected", SqueakLogEvent.ResolverRebuildFailed => "voicepack.resolver.rebuild_failed", SqueakLogEvent.TargetRejected => "voicepack.target.rejected", SqueakLogEvent.XenotypeDiscoveryUnavailable => "xenotype.discovery.unavailable", SqueakLogEvent.XenotypeDiscoveryFailed => "xenotype.discovery.failed", SqueakLogEvent.XenotypeDiscoveryCandidate => "xenotype.discovery.candidate", SqueakLogEvent.TriggerAttemptFailed => "trigger.attempt.failed", SqueakLogEvent.AudioNoSound => "audio.dispatch.no_sound", SqueakLogEvent.AudioDispatchFailed => "audio.dispatch.failed", SqueakLogEvent.AudioDispatchOk => "audio.dispatch.ok", SqueakLogEvent.TriggerOutcomeSummary => "trigger.outcome.summary", SqueakLogEvent.HookAttackUnavailable => "hook.attack.unavailable", SqueakLogEvent.HookAttackTargetSkipped => "hook.attack.target_skipped", SqueakLogEvent.HookMentalBreakUnavailable => "hook.mental_break.unavailable", SqueakLogEvent.DiagnosticsHookUnavailable => "diagnostics.hook.unavailable", SqueakLogEvent.DiagnosticsStartFailed => "diagnostics.start.failed", SqueakLogEvent.OverlayChanged => "devtools.overlay.changed", SqueakLogEvent.CameraChanged => "devtools.camera_indicator.changed", SqueakLogEvent.WorkbenchOpenFailed => "devtools.workbench.open_failed", _ => throw new ArgumentOutOfRangeException(nameof(e)) };
    private static void Add(StringBuilder b, string key, object? value) { if (value != null) b.Append(' ').Append(key).Append('=').Append(V(value)); }
    private static string V(object? value) { if (value == null) return "-"; if (value is bool boolean) return boolean ? "true" : "false"; return SqueakLogText.PercentEncode(value is IFormattable f ? f.ToString(null, CultureInfo.InvariantCulture) : value.ToString()); }
    private readonly struct SqueakLogDefinition { internal readonly SqueakLogVisibility Visibility; internal readonly SqueakLogLevel Level; internal readonly string Human; internal SqueakLogDefinition(SqueakLogVisibility v, SqueakLogLevel l, string h) { Visibility = v; Level = l; Human = h; } }
}

internal static class SqueakLogText
{
    private static readonly Regex Path = new(@"(?ix)(?:\\\\[?.][\\/][^\s]+|\\\\[^\\/\s]+[\\/][^\s]+|[a-z]:[\\/][^\s]+|[a-z]:(?![\\/])[^\s:]+|(?<![\\\w])\\(?![\\?.])[^\\\s]+(?:\\[^\s]+)*|file:(?://)?[^\s]+|(?<!\w)/(?:[^\s]+)|(?<!\w)\.\.?[\\/][^\s]+)", RegexOptions.Compiled);
    internal static string SanitizeExceptionMessage(string? text) { if (string.IsNullOrEmpty(text)) return "-"; string clean = new string(Array.FindAll(text!.Replace('\r', ' ').Replace('\n', ' ').ToCharArray(), c => !char.IsControl(c))); clean = Path.Replace(clean, "<path>"); return clean.Length > 256 ? clean.Substring(0, 256) : clean; }
    internal static string PercentEncode(string? text) { if (string.IsNullOrEmpty(text) || text == "N/A") return "-"; byte[] bytes = Encoding.UTF8.GetBytes(text); StringBuilder b = new(); foreach (byte c in bytes) { bool safe = c >= 'A' && c <= 'Z' || c >= 'a' && c <= 'z' || c >= '0' && c <= '9' || c == '.' || c == '_' || c == '~' || c == ':' || c == '/' || c == '@' || c == '+' || c == '-'; if (safe) b.Append((char)c); else b.Append('%').Append(c.ToString("X2", CultureInfo.InvariantCulture)); } return b.ToString(); }
}
