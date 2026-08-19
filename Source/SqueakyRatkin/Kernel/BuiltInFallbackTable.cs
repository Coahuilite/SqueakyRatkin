using System;
using System.Collections.Generic;

namespace SqueakyRatkin.Kernel;

/// <summary>
/// 内置 fallback 单源表（§4.6，内核 C# 编译期冻结）。0.3.0 骨架：种子 = SqueakActionDefinitions.AudioKey
/// （15 动作全列，Ratkin 条目 = SR_* 引用听感等价）；0.3.2 正式数据（Crying/Giggling 无内置音频不列条目，
/// pack 声明才发声）+ SqueakFallbackProfileStore（适配层，Config 副本单写者，0.3.2）。
/// FallbackProfile.SoundKeys 键保持枚举（内置专用，构建断言仅内置键，§2.2 内置表不开放外部键）。
/// </summary>
public sealed class FallbackProfile
{
    public readonly RaceKey Race;
    public readonly int Version;
    public readonly IReadOnlyDictionary<SqueakyRatkin.SqueakAction, string> SoundKeys;

    public FallbackProfile(RaceKey race, int version, IReadOnlyDictionary<SqueakyRatkin.SqueakAction, string> soundKeys)
    {
        Race = race;
        Version = version;
        SoundKeys = soundKeys ?? throw new ArgumentNullException(nameof(soundKeys));
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

    /// <summary>Select 内置 tier 查表：内置键（= 枚举名）→ profile 键 → gate 由调用方执行。</summary>
    public bool TryGetSoundKey(RaceKey race, string actionKey, out string? soundKey)
    {
        soundKey = null;
        FallbackProfile? profile = For(race);
        if (profile == null) return false;
        if (!SqueakyRatkin.ActionKey.TryParseBuiltIn(actionKey, out SqueakyRatkin.SqueakAction action)) return false;
        return profile.SoundKeys.TryGetValue(action, out soundKey);
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
