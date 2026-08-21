using System;
using System.Collections.Generic;

namespace SqueakyRatkin.Kernel;

/// <summary>一个域的池：条目按 PackKey 序数排序（跨重建稳定，orphan 语义沿用旧 GetSelected 的 Sort）。</summary>
public sealed class DomainPool
{
    public readonly AudioDomain Domain;
    public readonly IReadOnlyList<VoicePackEntry> Entries;

    internal DomainPool(AudioDomain domain, List<VoicePackEntry> entries)
    {
        Domain = domain;
        entries.Sort((a, b) => StringComparer.Ordinal.Compare(a.PackKey, b.PackKey));
        Entries = entries;
    }
}

/// <summary>
/// 不可变选择注册表（§4.1/§4.2）。0.3.0 语义规范 = 0.2.4 SqueakRuntimeSnapshot.Choose + ChoosePack
/// （等价评审锚）：
///   Off       → 仅内置表（旧 vanilla 字典）
///   Fallback  → XenotypePack → RacePack → BuiltInFallback 三级短路
///   Remix     → 三级（非 None）等权折叠，固定序 [xeno, race, builtin]
///   entry 级过滤 = 旧 pack 级 HasPlayable；sound 级过滤 = 旧 pack.Choose 内过滤；
///   等权抽取 = 旧 Rand.Range(0, N) 的分布等价（rolls.Next01() * N 取整）。
/// 0.3.x 带权 = entry.Weight 累计权重（0.3.0 全 1 = 等权）；年龄优先级 0.3.2 启用。
/// </summary>
public sealed class SqueakPoolRegistry
{
    private readonly IReadOnlyDictionary<AudioDomain, DomainPool> poolsByDomain;
    private readonly BuiltInFallbackTable builtIn;
    private readonly DomainFilter filter;

    public static SqueakPoolRegistry Empty => new(Array.Empty<VoicePackEntry>(), BuiltInFallbackTable.Empty, DomainFilter.Everything);

    public SqueakPoolRegistry(IReadOnlyList<VoicePackEntry> entries, BuiltInFallbackTable builtIn, DomainFilter filter)
    {
        this.builtIn = builtIn ?? throw new ArgumentNullException(nameof(builtIn));
        this.filter = filter ?? throw new ArgumentNullException(nameof(filter));
        Dictionary<AudioDomain, List<VoicePackEntry>> grouped = new();
        foreach (VoicePackEntry entry in entries ?? Array.Empty<VoicePackEntry>())
        {
            if (entry == null || entry.PackKey == null || entry.Actions == null) continue;
            if (!filter.Contains(entry.Domain.Race)) continue;
            if (!(entry.Weight > 0f)) continue;
            if (!grouped.TryGetValue(entry.Domain, out List<VoicePackEntry>? list))
            {
                list = new List<VoicePackEntry>();
                grouped.Add(entry.Domain, list);
            }
            list.Add(entry);
        }
        Dictionary<AudioDomain, DomainPool> pools = new();
        foreach (KeyValuePair<AudioDomain, List<VoicePackEntry>> group in grouped) pools.Add(group.Key, new DomainPool(group.Key, group.Value));
        poolsByDomain = pools;
    }

    /// <summary>选择链求值。<see cref="SelectionMode"/> 是内核 API，适配层负责从设置枚举映射。</summary>
    public ChainResult Select(SelectionContext ctx, SelectionMode mode, ISoundGate gate, IRollSource rolls)
    {
        if (gate == null) throw new ArgumentNullException(nameof(gate));
        if (rolls == null) throw new ArgumentNullException(nameof(rolls));
        ChainResult vanilla = SelectBuiltIn(ctx, gate);
        if (mode == SelectionMode.Off) return vanilla;

        ChainResult x = ctx.Domain.Xenotype != null
            ? SelectTier(PoolFor(ctx.Domain), ChainTier.XenotypePack, ctx, gate, rolls)
            : ChainResult.None;
        ChainResult r = SelectTier(PoolFor(new AudioDomain(ctx.Domain.Race, null)), ChainTier.RacePack, ctx, gate, rolls);

        if (mode == SelectionMode.Fallback) return x.IsNone ? (r.IsNone ? vanilla : r) : x;

        // Remix：三级等权折叠，固定序 [xeno, race, builtin]（旧实现同序）。
        ChainResult tier0 = x, tier1 = r, tier2 = vanilla;
        int count = 0;
        if (!tier0.IsNone) count++;
        if (!tier1.IsNone) count++;
        if (!tier2.IsNone) count++;
        if (count == 0) return ChainResult.None;
        if (count == 1) return !tier0.IsNone ? tier0 : (!tier1.IsNone ? tier1 : tier2);
        int index = RollIndex(count, rolls);
        return index switch
        {
            0 => tier0,
            1 => tier1,
            _ => tier2,
        };
    }

    public DomainPool? PoolFor(AudioDomain domain) => poolsByDomain.TryGetValue(domain, out DomainPool? pool) ? pool : null;

    /// <summary>诊断/编辑器枚举（§4.1 PoolsFor）。</summary>
    public IReadOnlyList<DomainPool> PoolsFor(AudioDomain domain)
    {
        DomainPool? pool = PoolFor(domain);
        return pool == null ? Array.Empty<DomainPool>() : new[] { pool };
    }

    /// <summary>内置表 tier：TryParseBuiltIn（内置键 ↔ 枚举名一致性由 validator 双向锁）+ profile 键 + gate。</summary>
    private ChainResult SelectBuiltIn(SelectionContext ctx, ISoundGate gate)
    {
        if (!builtIn.TryGetSoundKey(ctx.Domain.Race, ctx.ActionKey, out string? key) || key == null) return ChainResult.None;
        return gate.Playable(key, ctx) ? new ChainResult(key, ChainTier.BuiltInFallback, null) : ChainResult.None;
    }

    private static ChainResult SelectTier(DomainPool? pool, ChainTier tier, SelectionContext ctx, ISoundGate gate, IRollSource rolls)
    {
        if (pool == null || pool.Entries.Count == 0) return ChainResult.None;
        // entry 级过滤（= 旧 pack 级 HasPlayable）：动作存在且至少一个 playable sound。
        List<VoicePackEntry> valid = new(pool.Entries.Count);
        foreach (VoicePackEntry entry in pool.Entries)
        {
            if (!entry.TryGetAction(ctx.ActionKey, out ActionSoundSet? set) || !set.HasSounds) continue;
            if (!HasPlayableKey(set, ctx, gate)) continue;
            valid.Add(entry);
        }
        if (valid.Count == 0) return ChainResult.None;
        VoicePackEntry chosen = DrawEntry(valid, rolls);
        string? key = DrawSoundKey(chosen.Actions[ctx.ActionKey], ctx, gate, rolls);
        return key == null ? ChainResult.None : new ChainResult(key, tier, chosen.PackKey);
    }

    private static bool HasPlayableKey(ActionSoundSet set, SelectionContext ctx, ISoundGate gate)
    {
        foreach (string soundKey in set.SoundKeys)
        {
            if (soundKey != null && gate.Playable(soundKey, ctx)) return true;
        }
        return false;
    }

    private static VoicePackEntry DrawEntry(List<VoicePackEntry> valid, IRollSource rolls)
    {
        if (valid.Count == 1) return valid[0];
        // 构造期已过滤 Weight <= 0（见 ctor），此处权重恒正。
        double total = 0;
        foreach (VoicePackEntry entry in valid) total += entry.Weight;
        double roll = rolls.Next01() * total;
        double cumulative = 0;
        foreach (VoicePackEntry entry in valid)
        {
            cumulative += entry.Weight;
            if (roll < cumulative) return entry;
        }
        return valid[valid.Count - 1];
    }

    private static string? DrawSoundKey(ActionSoundSet set, SelectionContext ctx, ISoundGate gate, IRollSource rolls)
    {
        if (set.SoundKeys == null || set.SoundKeys.Count == 0) return null;
        // sound 级过滤（= 旧 pack.Choose 内过滤）：逐个 playable 等权抽取。
        List<string> playable = new(set.SoundKeys.Count);
        foreach (string soundKey in set.SoundKeys)
        {
            if (soundKey != null && gate.Playable(soundKey, ctx)) playable.Add(soundKey);
        }
        if (playable.Count == 0) return null;
        return playable.Count == 1 ? playable[0] : playable[RollIndex(playable.Count, rolls)];
    }

    /// <summary>等权索引：floor(Next01() * count)，clamp 到 count-1（分布等价旧 Rand.Range(0, count)）。</summary>
    private static int RollIndex(int count, IRollSource rolls)
    {
        int index = (int)(rolls.Next01() * count);
        if (index < 0) index = 0;
        if (index >= count) index = count - 1;
        return index;
    }
}
