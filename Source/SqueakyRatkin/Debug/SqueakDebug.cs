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

    /// <summary>Detailed logging is independent; successful-dispatch motes are controlled by the audio-path diagnostics switch above.</summary>
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
            SqueakLog.AudioDispatchOk(action.ToString(), pawn.thingIDNumber.ToString(), def.defName, sample.suppressed, pawn.LabelShort, pawn.ThingID);
            // srdiag v2 route record shares the same success-path rate control as audio.dispatch.ok.
            // action carries the string action key (built-in = enum name, byte-identical to v1);
            // race/xenotype carry exact DefNames; tier uses the protocol vocabulary.
            SqueakLog.AudioRouteSelected(
                SqueakyRatkin.Kernel.ActionKey.For(action) ?? action.ToString(),
                pawn.def?.defName ?? SqueakProductDomainFilter.PrimaryRaceDefName,
                pawn.genes?.Xenotype?.defName,
                pawn.thingIDNumber.ToString(),
                def.defName,
                ProtocolTier(choice.Source),
                choice.PoolStableKey);
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
