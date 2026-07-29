using System;
using System.Globalization;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace SqueakyRatkin;

/// <summary>Ephemeral dispatched-audio trace. It records only the production choice already made by PlayOneShot.</summary>
public static class SqueakAudioPathDiagnostics
{
    public const int Capacity = 10;
    public readonly struct Record
    {
        public readonly int GameTick; public readonly float Realtime; public readonly string PawnThingId;
        public readonly SqueakAction Action; public readonly SqueakMood Mood; public readonly SqueakSoundSource Tier;
        public readonly string SoundDefName, PoolStableKey, PackDefName, PackLabel, PackScope, TargetDefName, ModName, PackageId, Authors;
        internal Record(int tick, float realtime, string pawnId, SqueakAction action, SqueakMood mood, SqueakSoundSource tier,
            string soundDefName, string poolKey, string packDefName, string packLabel, string packScope, string target,
            string modName, string packageId, string authors)
        { GameTick = tick; Realtime = realtime; PawnThingId = pawnId; Action = action; Mood = mood; Tier = tier; SoundDefName = soundDefName; PoolStableKey = poolKey; PackDefName = packDefName; PackLabel = packLabel; PackScope = packScope; TargetDefName = target; ModName = modName; PackageId = packageId; Authors = authors; }
    }
    private static readonly Record[] records = new Record[Capacity];
    private static int next, count;
    private static bool enabled;
    private static long revision;
    public static long Revision => revision;
    public static long SnapshotBuildCount { get; private set; }
    public static bool Enabled { get => enabled; set { if (enabled == value) return; enabled = value; revision++; } }
    public static int Count => count;
    public static void Clear() { if (count == 0 && next == 0) return; next = 0; count = 0; revision++; }
    public static Record? Last => count == 0 ? null : records[(next + Capacity - 1) % Capacity];
    public static Record[] GetSnapshot()
    {
        SnapshotBuildCount++; Record[] snapshot = new Record[count]; int start = (next - count + Capacity) % Capacity;
        for (int i = 0; i < count; i++) snapshot[i] = records[(start + i) % Capacity];
        return snapshot;
    }
    /// <summary>Copies newest first in one pass; callers need not reverse an oldest-first snapshot.</summary>
    public static Record[] CopyNewestFirst()
    {
        SnapshotBuildCount++; Record[] snapshot = new Record[count];
        for (int i = 0; i < count; i++) snapshot[i] = records[(next - 1 - i + Capacity) % Capacity];
        return snapshot;
    }
    internal static void RecordDispatched(Pawn pawn, SqueakAction action, SqueakMood mood, SqueakSoundChoice choice)
    {
        if (!Enabled || pawn == null || choice.Sound == null) return;
        SoundDef sound = choice.Sound; SqueakVoicePackDef? pack = null;
        if ((choice.Source == SqueakSoundSource.RacePack || choice.Source == SqueakSoundSource.XenotypePack) && !string.IsNullOrEmpty(choice.PoolStableKey))
            SqueakXenotypeCatalog.Current.PackByKey.TryGetValue(choice.PoolStableKey!, out pack);
        ModContentPack? owner = pack?.modContentPack ?? sound.modContentPack;
        string packLabel = pack == null ? "" : (pack.LabelCap.NullOrEmpty() ? pack.defName : pack.LabelCap);
        SqueakSettingsGameContext context = SqueakSettingsGameContext.CaptureRuntime();
        records[next] = new Record(context.HasPlayableMapUI ? context.Tick : 0, context.Realtime, pawn.ThingID, action, mood, choice.Source,
            sound.defName ?? "", choice.PoolStableKey ?? "", pack?.defName ?? "", packLabel, pack?.scope.ToString() ?? "",
            pack?.targetDefName ?? "", owner?.Name ?? "", owner?.PackageId ?? "", owner?.ModMetaData?.AuthorsString ?? "");
        next = (next + 1) % Capacity; if (count < Capacity) count++; revision++;
    }
    public static string GetHumanDetail(Record? value = null)
    {
        Record? selected = value ?? Last; if (!selected.HasValue) return "No dispatched audio-path record."; Record r = selected.Value;
        return r.Action + " · " + r.Tier + " · " + r.SoundDefName + "\nPawn=" + r.PawnThingId + " tick=" + r.GameTick + " realtime=" + r.Realtime.ToString("0.000", CultureInfo.InvariantCulture)
            + "\nPack=" + (r.PackDefName.NullOrEmpty() ? "(not a pack)" : r.PackDefName + " / " + r.PackScope + " / " + r.TargetDefName)
            + "\nSource mod=" + r.ModName + " / " + r.PackageId + " / " + r.Authors + "\nClip filename is not available from this diagnostic record.";
    }
    public static string GetLastReportText() => Last.HasValue ? Format(Last.Value) : "sraudio fmt=1 state=empty";
    public static string GetReportText()
    {
        Record[] snapshot = GetSnapshot(); if (snapshot.Length == 0) return "sraudio fmt=1 state=empty";
        StringBuilder builder = new(); for (int i = 0; i < snapshot.Length; i++) { if (i > 0) builder.Append('\n'); builder.Append(Format(snapshot[i])); } return builder.ToString();
    }
    private static string Format(Record r) => "sraudio fmt=1 tick=" + r.GameTick + " realtime=" + r.Realtime.ToString("R", CultureInfo.InvariantCulture)
        + " pawn=" + Escape(r.PawnThingId) + " action=" + r.Action + " mood=" + r.Mood + " tier=" + r.Tier
        + " sound=" + Escape(r.SoundDefName) + " pool_key=" + Escape(r.PoolStableKey) + " pack_def=" + Escape(r.PackDefName)
        + " pack_label=" + Escape(r.PackLabel) + " pack_scope=" + Escape(r.PackScope) + " target=" + Escape(r.TargetDefName)
        + " mod_name=" + Escape(r.ModName) + " package_id=" + Escape(r.PackageId) + " authors=" + Escape(r.Authors) + " clip_filename=unavailable";
    private static string Escape(string value) => Uri.EscapeDataString(value ?? "");
}
