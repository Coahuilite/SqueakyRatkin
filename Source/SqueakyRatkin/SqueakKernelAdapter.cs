using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.Sound;
using SqueakyRatkin.Kernel;

namespace SqueakyRatkin;

/// <summary>
/// 内核↔适配层接缝（§4.1 边界）：SoundDef 收敛为 string key + ISoundGate 函子；
/// Verse.Rand 收敛为 IRollSource；catalog 域包投影为 VoicePackEntry[]。
/// 2b-2：域身份端到端 = AudioDomain（记录自身 raceDefName/xenotypeDefName），无注入字面量域。
/// 投影规则 = 旧 ResolvedAudioPack 构建规则（0.2.4）：非 null、去 _Preview 后缀、Distinct、
/// defName Ordinal 排序；HasSounds 过滤；Weight = 1（等权）。
/// </summary>
internal static class SqueakKernelAdapter
{
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

    /// <summary>产品设置枚举到内核选择枚举的唯一映射点。</summary>
    public static SelectionMode ToSelectionMode(SqueakVoicePackMode mode) => mode switch
    {
        SqueakVoicePackMode.Fallback => SelectionMode.Fallback,
        SqueakVoicePackMode.Remix => SelectionMode.Remix,
        _ => SelectionMode.Off,
    };

    /// <summary>内置表（0.3.0 种子 = SqueakActionDefinitions.AudioKey 单源投影，15 动作全列）。
    /// 条目 race = ProductDomainFilter 数据单源（无散落 Ratkin 字面量）。</summary>
    public static BuiltInFallbackTable BuildBuiltIn()
    {
        Dictionary<string, string> keys = new(StringComparer.Ordinal);
        for (int i = 0; i < SqueakActionDefinitions.Count; i++)
        {
            SqueakAction action = (SqueakAction)i;
            string? actionKey = ActionKey.For(action);
            if (actionKey != null) keys[actionKey] = SqueakActionDefinitions.Get(action).AudioKey;
        }
        return new BuiltInFallbackTable(new[] { new FallbackProfile(new RaceKey(SqueakProductDomainFilter.PrimaryRaceDefName), 1, keys) });
    }

    /// <summary>2b-2: AudioDomain 域键端到端。选择集按记录自身 (raceDefName, xenotypeDefName) 域键组织；
    /// 候选 pack 按声明的 raceDefName 精确匹配域（跨 race 池隔离），不再有注入字面量域。</summary>
    public static List<VoicePackEntry> BuildEntries(SqueakXenotypeCatalogSnapshot catalog, IReadOnlyDictionary<AudioDomain, HashSet<string>> selections)
    {
        List<VoicePackEntry> entries = new();
        foreach (KeyValuePair<AudioDomain, HashSet<string>> pair in selections)
        {
            AudioDomain domain = pair.Key;
            if (pair.Value == null || pair.Value.Count == 0) continue;
            IReadOnlyList<SqueakVoicePackDef>? candidates = null;
            if (domain.Xenotype == null)
                candidates = catalog.RacePacks;
            else if (ModsConfig.BiotechActive && catalog.XenotypePacksByDefName.TryGetValue(domain.Xenotype.Value.DefName, out IReadOnlyList<SqueakVoicePackDef>? packs))
                candidates = packs;
            AddDomain(entries, candidates, pair.Value, domain);
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

    private static void AddDomain(List<VoicePackEntry> entries, IReadOnlyList<SqueakVoicePackDef>? candidates, HashSet<string> keys, AudioDomain domain)
    {
        if (candidates == null || keys == null || keys.Count == 0) return;
        foreach (SqueakVoicePackDef pack in candidates)
        {
            // 域键精确匹配：pack 声明的 raceDefName 必须等于选择域的 race（跨 race 池隔离）。
            if (!string.Equals(pack.raceDefName, domain.Race.DefName, StringComparison.Ordinal)) continue;
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
