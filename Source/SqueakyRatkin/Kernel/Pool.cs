using System;
using System.Collections.Generic;

namespace SqueakyRatkin.Kernel;

// 池与链求值类型（§4.1）。SoundKey = SoundDef.defName 字符串（边界收敛：SoundDef 在适配层解析）。

public enum AgeBucket { Baby, Toddler, Child, Adult }

/// <summary>一个动作的声音集合。AgeTag null = 全年龄（0.3.2 启用年龄优先级；0.3.0 全 null）。</summary>
public sealed class ActionSoundSet
{
    public readonly IReadOnlyList<string> SoundKeys;
    public readonly AgeBucket? AgeTag;
    public readonly float Weight;

    public ActionSoundSet(IReadOnlyList<string> soundKeys, AgeBucket? ageTag, float weight)
    {
        SoundKeys = soundKeys;
        AgeTag = ageTag;
        Weight = weight;
    }

    public bool HasSounds => SoundKeys != null && SoundKeys.Count > 0;
}

/// <summary>一个 VoicePack 在内核中的投影（PackKey 序数排序成员）。PackFallback 0.3.2 起。</summary>
public sealed class VoicePackEntry
{
    public readonly string PackKey;
    public readonly AudioDomain Domain;
    public readonly float Weight;
    public readonly IReadOnlyDictionary<string, ActionSoundSet> Actions;
    public readonly IReadOnlyDictionary<string, string>? PackFallback;

    public VoicePackEntry(string packKey, AudioDomain domain, float weight, IReadOnlyDictionary<string, ActionSoundSet> actions, IReadOnlyDictionary<string, string>? packFallback = null)
    {
        PackKey = packKey;
        Domain = domain;
        Weight = weight;
        Actions = actions;
        PackFallback = packFallback;
    }

    public bool TryGetAction(string actionKey, out ActionSoundSet set) => Actions.TryGetValue(actionKey, out set!);
}

public enum ChainTier { XenotypePack, RacePack, PackFallback, BuiltInFallback }

/// <summary>选择结果；SoundKey/Tier/PoolStableKey 全 null = 无声。</summary>
public readonly struct ChainResult
{
    public static readonly ChainResult None = default;

    public readonly string? SoundKey;
    public readonly ChainTier? Tier;
    public readonly string? PoolStableKey;

    public bool IsNone => SoundKey == null;

    public ChainResult(string? soundKey, ChainTier? tier, string? poolStableKey)
    {
        SoundKey = soundKey;
        Tier = tier;
        PoolStableKey = poolStableKey;
    }
}

/// <summary>选择上下文。ActionKey：内置=枚举名，外部=packageId.defName（§2.2）。</summary>
public readonly struct SelectionContext
{
    public readonly AudioDomain Domain;
    public readonly string ActionKey;
    public readonly AgeBucket Age;
    public readonly bool Production;

    public SelectionContext(AudioDomain domain, string actionKey, AgeBucket age, bool production)
    {
        Domain = domain;
        ActionKey = actionKey;
        Age = age;
        Production = production;
    }
}

/// <summary>随机源：适配层提供运行时随机数；预览可注入确定性来源。相同 roll 序列必得相同结果。</summary>
public interface IRollSource
{
    double Next01();
}

/// <summary>playability 函子：适配层包 SqueakSoundAvailabilityCache。</summary>
public interface ISoundGate
{
    bool Playable(string soundKey, SelectionContext ctx);
}
