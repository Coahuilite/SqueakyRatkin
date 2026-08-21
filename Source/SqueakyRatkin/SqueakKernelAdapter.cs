using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.Sound;
using SqueakyRatkin.Kernel;

namespace SqueakyRatkin;

/// <summary>
/// 内核↔适配层接缝（§4.1 边界）：SoundDef 收敛为 string key + ISoundGate 函子；
/// Verse.Rand 收敛为 IRollSource；catalog 域包投影为 VoicePackEntry[]（0.3.0 注入 (Ratkin,*)）。
/// 投影规则 = 旧 ResolvedAudioPack 构建规则（0.2.4）：非 null、去 _Preview 后缀、Distinct、
/// defName Ordinal 排序；HasSounds 过滤；Weight = 1（等权）。
/// </summary>
internal static class SqueakKernelAdapter
{
    private static readonly AudioDomain RatkinRaceDomain = new(new RaceKey("Ratkin"), null);

    /// <summary>playability 函子：携带旧 Playable 的 pawn/map/target 上下文；production 语义统一取 ctx.Production
    /// （与调用方同源，避免 gate 捕获标志与 SelectionContext 错配）。</summary>
    private sealed class KernelGate : ISoundGate
    {
        private readonly Pawn? pawn;
        private readonly Map? map;
        private readonly TargetInfo? target;

        public KernelGate(Pawn? pawn, Map? map, TargetInfo? target)
        {
            this.pawn = pawn;
            this.map = map;
            this.target = target;
        }

        public bool Playable(string soundKey, SelectionContext ctx)
        {
            if (soundKey == null) return false;
            SoundDef? sound = DefDatabase<SoundDef>.GetNamedSilentFail(soundKey);
            if (sound == null) return false;
            return (ctx.Production ? SqueakSoundAvailabilityCache.GetProductionPlayability(sound, pawn) : SqueakSoundAvailabilityCache.GetNativePlayability(sound, map, target)) == SqueakSoundPlayability.Playable;
        }
    }

    /// <summary>随机源：包 Verse.Rand（预览/诊断走 PushState 由调用方控制）。</summary>
    private sealed class RandRollSource : IRollSource
    {
        public double Next01() => Rand.Value;
    }

    public static ISoundGate GateFor(Pawn? pawn, Map? map, TargetInfo? target)
        => new KernelGate(pawn, map, target);


    public static IRollSource Rolls => new RandRollSource();

    /// <summary>内置表（0.3.0 种子 = SqueakActionDefinitions.AudioKey 单源投影，15 动作全列）。</summary>
    public static BuiltInFallbackTable BuildBuiltIn()
    {
        Dictionary<SqueakAction, string> keys = new();
        for (int i = 0; i < SqueakActionDefinitions.Count; i++)
        {
            SqueakAction action = (SqueakAction)i;
            keys[action] = SqueakActionDefinitions.Get(action).AudioKey;
        }
        return new BuiltInFallbackTable(new[] { new FallbackProfile(new RaceKey("Ratkin"), 1, keys) });
    }

    /// <summary>选择面投影：Race 域 (Ratkin,null) + Xenotype 域 (Ratkin,target)（Biotech 由调用方决定注入面）。</summary>
    public static List<VoicePackEntry> BuildEntries(SqueakXenotypeCatalogSnapshot catalog, IReadOnlyDictionary<string, HashSet<string>> selections)
    {
        List<VoicePackEntry> entries = new();
        AddDomain(entries, catalog.RacePacks, selections, SqueakVoicePackScope.Race, "");
        if (ModsConfig.BiotechActive)
        {
            foreach (KeyValuePair<string, IReadOnlyList<SqueakVoicePackDef>> group in catalog.XenotypePacksByDefName)
            {
                AddDomain(entries, group.Value, selections, SqueakVoicePackScope.Xenotype, group.Key);
            }
        }
        return entries;
    }

    /// <summary>KnownMapSoundDefs 收集（旧 AddKnown 语义：catalog 全量 pack 音频，含未选择；与旧 ResolvedAudioPack
    /// 构造一致，排除 _Preview 后缀 transport；距离应用覆盖依赖）。</summary>
    public static HashSet<SoundDef> CollectKnownSounds(SqueakXenotypeCatalogSnapshot catalog)
    {
        HashSet<SoundDef> known = new();
        foreach (SqueakVoicePackDef pack in catalog.PackByKey.Values)
        {
            foreach (SoundDef sound in ProjectSounds(pack)) known.Add(sound);
        }
        return known;
    }

    /// <summary>ChainResult → SqueakSoundChoice（tier → source 映射；key 查表缺失 → None 防御）。</summary>
    public static SqueakSoundChoice ToChoice(ChainResult result)
    {
        if (result.SoundKey == null) return SqueakSoundChoice.None;
        SoundDef? sound = DefDatabase<SoundDef>.GetNamedSilentFail(result.SoundKey);
        if (sound == null) return SqueakSoundChoice.None;
        SqueakSoundSource source = result.Tier switch
        {
            ChainTier.XenotypePack => SqueakSoundSource.XenotypePack,
            ChainTier.RacePack => SqueakSoundSource.RacePack,
            ChainTier.PackFallback => SqueakSoundSource.RacePack,
            _ => SqueakSoundSource.Vanilla,
        };
        return new SqueakSoundChoice(sound, source, result.PoolStableKey);
    }

    private static void AddDomain(List<VoicePackEntry> entries, IReadOnlyList<SqueakVoicePackDef> candidates, IReadOnlyDictionary<string, HashSet<string>> selections, SqueakVoicePackScope scope, string target)
    {
        if (!selections.TryGetValue(VoicePackSelectionRecord.ComposeDomainKey(scope, target), out HashSet<string>? keys)) return;
        AudioDomain domain = scope == SqueakVoicePackScope.Race
            ? RatkinRaceDomain
            : new AudioDomain(new RaceKey("Ratkin"), new XenotypeKey(target));
        foreach (SqueakVoicePackDef pack in candidates)
        {
            if (!pack.TryGetPackKey(out string key) || !keys.Contains(key)) continue;
            VoicePackEntry? entry = BuildEntry(pack, key, domain);
            if (entry != null) entries.Add(entry);
        }
    }

    private static VoicePackEntry? BuildEntry(SqueakVoicePackDef pack, string key, AudioDomain domain)
    {
        Dictionary<string, ActionSoundSet> actions = new();
        foreach (SqueakVoicePackAction entry in pack.actions ?? new List<SqueakVoicePackAction>())
        {
            if (entry == null) continue;
            string? actionKey = ActionKey.For(entry.action);
            if (actionKey == null || actions.ContainsKey(actionKey)) continue;
            List<string> sounds = ProjectSounds(entry);
            if (sounds.Count == 0) continue;
            actions[actionKey] = new ActionSoundSet(sounds, null, 1f);
        }
        if (actions.Count == 0) return null;
        return new VoicePackEntry(key, domain, 1f, actions);
    }

    private static List<string> ProjectSounds(SqueakVoicePackAction entry)
    {
        return (entry.sounds ?? new List<SoundDef>())
            .Where(s => s != null && !s.defName.EndsWith("_Preview", StringComparison.Ordinal))
            .Select(s => s!.defName)
            .Distinct(StringComparer.Ordinal)
            .OrderBy(n => n, StringComparer.Ordinal)
            .ToList();
    }

    private static List<SoundDef> ProjectSounds(SqueakVoicePackDef pack)
    {
        List<SoundDef> result = new();
        foreach (SqueakVoicePackAction entry in pack.actions ?? new List<SqueakVoicePackAction>())
        {
            if (entry == null) continue;
            foreach (SoundDef sound in entry.sounds ?? new List<SoundDef>())
            {
                if (sound == null || sound.defName.EndsWith("_Preview", StringComparison.Ordinal) || result.Contains(sound)) continue;
                result.Add(sound);
            }
        }
        return result;
    }
}
