using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace SqueakyRatkin;

public static class SqueakDebug
{
    public static bool ShowOverlay { get => SqueakAudioPathDiagnostics.Enabled; set => SqueakAudioPathDiagnostics.Enabled = value; }
    public static bool AudioPathDiagnosticsEnabled { get => SqueakAudioPathDiagnostics.Enabled; set => SqueakAudioPathDiagnostics.Enabled = value; }
    public static bool ShowCameraIndicator = false;
    private sealed class AudioSample { public float nextDetail; public int dispatched; public int suppressed; }
    private static readonly Dictionary<SqueakAction, AudioSample> audioSamples = new();
    private static float nextSummary;

    /// <summary>Called by settings when effective detailed logging changes; it does not affect mote diagnostics.</summary>
    public static void ResetLoggingSession()
    {
        audioSamples.Clear();
        nextSummary = 0f;
        SqueakLog.ResetSession();
    }

    /// <summary>srdiag v2 tier vocabulary (0.3.1): xenotype_pack / race_pack / vanilla / "-" for none.
    /// The adapter collapses ChainTier into SqueakSoundSource before this point; PackFallback folds into
    /// RacePack and BuiltInFallback into Vanilla (logging-protocol.md records the folded emission).</summary>
    private static string ProtocolTier(SqueakSoundSource source) => source switch
    {
        SqueakSoundSource.XenotypePack => "xenotype_pack",
        SqueakSoundSource.RacePack => "race_pack",
        SqueakSoundSource.Vanilla => "vanilla",
        _ => "-",
    };

    public static void NotifySqueak(Pawn pawn, SqueakAction action, SqueakMood mood, SqueakSoundChoice choice)
    {
        SoundDef? def = choice.Sound;
        if (def == null) return;
        NotifyAudioDispatched(pawn, action, choice);
        SqueakAudioPathDiagnostics.RecordDispatched(pawn, action, mood, choice);

        if (AudioPathDiagnosticsEnabled && pawn?.Map != null)
        {
            string text = action + " · " + choice.Source + " · " + def.defName;
            SqueakMoteMaker.ThrowSqueakText(pawn.DrawPos, pawn.Map, text);
        }
    }

    /// <summary>Detailed logging is independent; successful-dispatch motes are controlled by the audio-path diagnostics switch above.
    /// 0.3.2 重排简化：成功路径不再逐次并列发射 audio.dispatch.ok(v1) + audio.route.selected(v2) 两条，
    /// 而只发射一条带 egg/pawn/pawn_faction/pawn_ctrl/suppressed 的 v2 路由记录（v1 audio.dispatch.ok
    /// 仍保留在协议与 characterization 中，业务装配器不再重复调用）。每动作 5 秒窗口内首条明细、
    /// 其余计入 suppressed；60 秒汇总仍走 trigger.outcome.summary。</summary>
    private static void NotifyAudioDispatched(Pawn pawn, SqueakAction action, SqueakSoundChoice choice)
    {
        if (!SqueakLog.EffectiveDevLogging) return;
        SoundDef? def = choice.Sound;
        if (def == null) return;
        float now = Time.realtimeSinceStartup;
        if (!audioSamples.TryGetValue(action, out AudioSample? sample)) { sample = new AudioSample(); audioSamples.Add(action, sample); }
        sample.dispatched++;
        if (now >= sample.nextDetail)
        {
            SqueakLog.AudioRouteSelected(
                SqueakyRatkin.Kernel.ActionKey.For(action) ?? action.ToString(),
                pawn.def?.defName ?? SqueakProductDomainFilter.PrimaryRaceDefName,
                pawn.genes?.Xenotype?.defName,
                pawn.thingIDNumber.ToString(),
                def.defName,
                ProtocolTier(choice.Source),
                choice.PoolStableKey,
                choice.IsEgg,
                sample.suppressed,
                pawn.LabelShort,
                pawn.ThingID,
                pawn.IsPlayerControlled,
                pawn.Faction?.def?.defName ?? "-");
            sample.suppressed = 0;
            sample.nextDetail = now + 5f;
        }
        else sample.suppressed++;
        if (now < nextSummary) return;
        int dispatched = 0, suppressed = 0;
        foreach (AudioSample item in audioSamples.Values) { dispatched += item.dispatched; suppressed += item.suppressed; item.dispatched = 0; item.suppressed = 0; }
        nextSummary = now + 60f;
        SqueakLog.TriggerOutcomeSummary(dispatched, suppressed);
    }
}
