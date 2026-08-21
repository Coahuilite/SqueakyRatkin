using System;
using System.Collections.Generic;

namespace SqueakyRatkin.Kernel;

/// <summary>
/// 内置 fallback 单源表（§4.6，内核 C# 编译期冻结）。profile 的动作键始终是字符串，
/// 并由 <see cref="BuiltInActionKeys"/> 封闭校验；0.3.1 的 Ratkin 种子仅镜像已有 15 个
/// <c>SqueakActionDefinitions.AudioKey</c>，Crying/Giggling 保留键没有内置 SoundDef 映射。
/// </summary>
public sealed class FallbackProfile
{
    public readonly RaceKey Race;
    public readonly int Version;
    public readonly IReadOnlyDictionary<string, string> SoundKeys;

    public FallbackProfile(RaceKey race, int version, IReadOnlyDictionary<string, string> soundKeys)
    {
        Race = race;
        Version = version;
        SoundKeys = soundKeys ?? throw new ArgumentNullException(nameof(soundKeys));
        foreach (KeyValuePair<string, string> entry in SoundKeys)
        {
            if (!BuiltInActionKeys.Contains(entry.Key))
                throw new ArgumentException("Fallback profile contains a non-built-in action key: " + entry.Key, nameof(soundKeys));
        }
    }

    /// <summary>纯字符串查表：只有内置清单内的键可以命中 profile。</summary>
    public bool TryGetSoundKey(string actionKey, out string? soundKey)
    {
        soundKey = null;
        return BuiltInActionKeys.Contains(actionKey) && SoundKeys.TryGetValue(actionKey, out soundKey);
    }
}

public sealed class BuiltInFallbackTable
{
    public static readonly BuiltInFallbackTable Empty = new(Array.Empty<FallbackProfile>());

    private readonly IReadOnlyDictionary<RaceKey, FallbackProfile> byRace;

    public BuiltInFallbackTable(IReadOnlyList<FallbackProfile> profiles)
    {
        Dictionary<RaceKey, FallbackProfile> map = new();
        foreach (FallbackProfile profile in profiles ?? Array.Empty<FallbackProfile>())
        {
            if (profile == null) continue;
            map[profile.Race] = profile;
        }
        byRace = map;
    }

    public FallbackProfile? For(RaceKey race) => byRace.TryGetValue(race, out FallbackProfile? profile) ? profile : null;

    /// <summary>Select 内置 tier 查表：字符串 action key 经内置清单校验后映射到 profile SoundDef key。</summary>
    public bool TryGetSoundKey(RaceKey race, string actionKey, out string? soundKey)
    {
        soundKey = null;
        FallbackProfile? profile = For(race);
        return profile != null && profile.TryGetSoundKey(actionKey, out soundKey);
    }
}

/// <summary>域过滤器（§4.4，0.4.x 删除）。0.3.0 = Everything；0.3.1 配置化白名单。</summary>
public sealed class DomainFilter
{
    public static readonly DomainFilter Everything = new(null);

    private readonly ISet<RaceKey>? allowed;

    public DomainFilter(ISet<RaceKey>? allowed) => this.allowed = allowed;

    public bool Contains(RaceKey race) => allowed == null || allowed.Contains(race);
}
