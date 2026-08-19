using System;
using System.Collections.Generic;
using SqueakyRatkin.Kernel;

namespace SqueakyRatkin.KernelCharacterization;

/// <summary>
/// 语料场景（0.3.0 固化，0.3.1 切核回放时不得修改场景构造——只更新 Kernel 链接）。
/// 场景 = 池内容 + 内置表，对应设置全项矩阵的「选择状态 × Biotech」维度（§6.2 场景语义）。
/// S1 无选择（空池）；S2 仅内置种子（Race 池 1 条目）；S3 内置+Xeno（Race 池 + (Ratkin,XenoA) 池）；
/// S4 orphan（xeno 域选择存在但池空——域未装配）；S5 dormant（xeno 域存在但 Biotech 关——域不注入）。
/// 每个场景的域注入面：S3 有 (Ratkin,XenoA)；S4/S5 无（适配层不注入）。
/// </summary>
public static class Scenarios
{
    public static readonly RaceKey Ratkin = new("Ratkin");
    public static readonly XenotypeKey XenoA = new("XenoA");

    /// <summary>内置表种子（0.3.0：15 动作全列 = SqueakActionDefinitions.AudioKey 投影，单源）。</summary>
    public static BuiltInFallbackTable BuildBuiltIn()
    {
        Dictionary<SqueakyRatkin.SqueakAction, string> keys = new();
        for (int i = 0; i < SqueakyRatkin.SqueakActionDefinitions.Count; i++)
        {
            SqueakyRatkin.SqueakAction action = (SqueakyRatkin.SqueakAction)i;
            keys[action] = SqueakyRatkin.SqueakActionDefinitions.Get(action).AudioKey;
        }
        return new BuiltInFallbackTable(new[] { new FallbackProfile(Ratkin, 1, keys) });
    }

    public static string[] ScenarioNames { get; } = { "S1-empty", "S2-builtin-seed", "S3-builtin-plus-xeno", "S4-orphan-xeno", "S5-dormant-xeno" };

    /// <summary>场景 → 注册表（语料与回放共用同一构造）。</summary>
    public static SqueakPoolRegistry BuildRegistry(string scenario)
    {
        BuiltInFallbackTable builtIn = BuildBuiltIn();
        switch (scenario)
        {
            case "S1-empty":
                return new SqueakPoolRegistry(Array.Empty<VoicePackEntry>(), builtIn, DomainFilter.Everything);
            case "S2-builtin-seed":
                return new SqueakPoolRegistry(new[]
                {
                    RaceEntry("coahuilite.squeakyratkin:SR_OfficialExample_Race", 2),
                }, builtIn, DomainFilter.Everything);
            case "S3-builtin-plus-xeno":
                return new SqueakPoolRegistry(new[]
                {
                    RaceEntry("coahuilite.squeakyratkin:SR_OfficialExample_Race", 2),
                    XenoEntry("other.mod:SR_XenoAVoice", 2),
                }, builtIn, DomainFilter.Everything);
            case "S4-orphan-xeno":
                // xeno 域选择存在但未装配：域池不注入 = 无 (Ratkin,XenoA) 池。
                return new SqueakPoolRegistry(new[]
                {
                    RaceEntry("coahuilite.squeakyratkin:SR_OfficialExample_Race", 2),
                }, builtIn, DomainFilter.Everything);
            case "S5-dormant-xeno":
                return new SqueakPoolRegistry(new[]
                {
                    RaceEntry("coahuilite.squeakyratkin:SR_OfficialExample_Race", 2),
                }, builtIn, DomainFilter.Everything);
            default:
                throw new ArgumentException("Unknown scenario: " + scenario);
        }
    }

    public static AudioDomain RaceDomain => new(Ratkin, null);
    public static AudioDomain XenoDomain => new(Ratkin, XenoA);

    public static AudioDomain[] DomainsFor(string scenario)
    {
        // S3 的域注入面含 (Ratkin,XenoA)；其余场景只有 (Ratkin,null)。
        return scenario == "S3-builtin-plus-xeno"
            ? new[] { RaceDomain, XenoDomain }
            : new[] { RaceDomain };
    }

    private static VoicePackEntry RaceEntry(string packKey, int soundCount)
    {
        return Entry(packKey, RaceDomain, soundCount, muteLast: false);
    }

    private static VoicePackEntry XenoEntry(string packKey, int soundCount)
    {
        return Entry(packKey, XenoDomain, soundCount, muteLast: true);
    }

    private static VoicePackEntry Entry(string packKey, AudioDomain domain, int soundCount, bool muteLast)
    {
        Dictionary<string, ActionSoundSet> actions = new();
        for (int i = 0; i < SqueakyRatkin.SqueakActionDefinitions.Count; i++)
        {
            SqueakyRatkin.SqueakAction action = (SqueakyRatkin.SqueakAction)i;
            string audioKey = SqueakyRatkin.SqueakActionDefinitions.Get(action).AudioKey;
            List<string> sounds = new(soundCount);
            for (int s = 0; s < soundCount; s++)
            {
                string key = packKey + "_" + audioKey + "_" + s;
                if (muteLast && s == soundCount - 1) key += "_Muted";
                sounds.Add(key);
            }
            actions[ActionKeyFor(action)] = new ActionSoundSet(sounds, null, 1f);
        }
        return new VoicePackEntry(packKey, domain, 1f, actions);
    }

    private static string ActionKeyFor(SqueakyRatkin.SqueakAction action) => SqueakyRatkin.ActionKey.For(action)!;
}
