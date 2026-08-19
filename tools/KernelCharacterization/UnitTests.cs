using System;
using System.Collections.Generic;
using SqueakyRatkin.Kernel;

namespace SqueakyRatkin.KernelCharacterization;

/// <summary>验证门单测（决策 §5 验证门 + §4 语义规范）。每断言独立输出；失败计数由 Program 汇总。</summary>
public static class UnitTests
{
    public static void RunAll(ref int failures)
    {
        DomainKeys(ref failures);
        ActionKeyMapping(ref failures);
        BuiltInTable(ref failures);
        PoolOrdering(ref failures);
        SelectOff(ref failures);
        SelectFallbackChain(ref failures);
        SelectRemix(ref failures);
        DistributionBounds(ref failures);
        Determinism(ref failures);
        SoundLevelFilter(ref failures);
        EntryLevelFilter(ref failures);
        DomainStatus(ref failures);
        ModulationRules(ref failures);
        AgeDefault(ref failures);
    }

    private static void Check(bool condition, string name, ref int failures)
    {
        if (condition) Console.WriteLine("  ok: " + name);
        else { Console.Error.WriteLine("  FAIL: " + name); failures++; }
    }

    private static void DomainKeys(ref int failures)
    {
        RaceKey a = new("Ratkin");
        RaceKey b = new("Ratkin");
        RaceKey c = new("ratkin"); // Ordinal 区分大小写
        Check(a == b && a.GetHashCode() == b.GetHashCode(), "RaceKey value equality", ref failures);
        Check(a != c, "RaceKey case-sensitive", ref failures);
        Check(new AudioDomain(a, null) == new AudioDomain(b, null), "AudioDomain race-only equality", ref failures);
        Check(new AudioDomain(a, new XenotypeKey("X")) == new AudioDomain(a, new XenotypeKey("X")), "AudioDomain xenotype equality", ref failures);
        Check(new AudioDomain(a, null) != new AudioDomain(a, new XenotypeKey("X")), "AudioDomain null-vs-xeno distinct", ref failures);
        Check(new AudioDomain(a, null).ToString() == "Ratkin", "AudioDomain ToString race", ref failures);
        Check(new AudioDomain(a, new XenotypeKey("XenoA")).ToString() == "Ratkin+XenoA", "AudioDomain ToString xeno", ref failures);
    }

    private static void ActionKeyMapping(ref int failures)
    {
        bool allOk = true;
        for (int i = 0; i < SqueakyRatkin.SqueakActionDefinitions.Count; i++)
        {
            SqueakyRatkin.SqueakAction action = (SqueakyRatkin.SqueakAction)i;
            string? key = SqueakyRatkin.ActionKey.For(action);
            if (key == null || !SqueakyRatkin.ActionKey.TryParseBuiltIn(key, out SqueakyRatkin.SqueakAction parsed) || parsed != action) allOk = false;
        }
        Check(allOk, "ActionKey For/TryParseBuiltIn bidirectional (15 actions)", ref failures);
        Check(SqueakyRatkin.ActionKey.For((SqueakyRatkin.SqueakAction)99) == null, "ActionKey.For unknown null", ref failures);
        Check(!SqueakyRatkin.ActionKey.TryParseBuiltIn("other.mod:SR_X", out _), "TryParseBuiltIn external key false", ref failures);
        Check(!SqueakyRatkin.ActionKey.TryParseBuiltIn("call", out _), "TryParseBuiltIn wrong case false", ref failures);
    }

    private static void BuiltInTable(ref int failures)
    {
        BuiltInFallbackTable table = Scenarios.BuildBuiltIn();
        FallbackProfile? profile = table.For(Scenarios.Ratkin);
        Check(profile != null && profile.SoundKeys.Count == 15, "built-in Ratkin profile has 15 keys", ref failures);
        Check(table.For(new RaceKey("Kiiro")) == null, "unknown race returns null", ref failures);
        Check(table.TryGetSoundKey(Scenarios.Ratkin, "Call", out string? k) && k == "SR_Call", "TryGetSoundKey Call=SR_Call", ref failures);
        Check(table.TryGetSoundKey(Scenarios.Ratkin, "other.mod:SR_X", out _) == false, "TryGetSoundKey external key false", ref failures);
    }

    private static void PoolOrdering(ref int failures)
    {
        SqueakPoolRegistry registry = Scenarios.BuildRegistry("S3-builtin-plus-xeno");
        DomainPool? race = registry.PoolFor(Scenarios.RaceDomain);
        Check(race != null && race.Entries.Count == 1, "S3 race pool 1 entry", ref failures);
        DomainPool? xeno = registry.PoolFor(Scenarios.XenoDomain);
        Check(xeno != null && xeno.Entries.Count == 1, "S3 xeno pool 1 entry", ref failures);
        Check(registry.PoolFor(new AudioDomain(new RaceKey("Kiiro"), null)) == null, "unknown race no pool", ref failures);
        // 排序：构造倒序，池应序数排序
        SqueakPoolRegistry mixed = new(new[]
        {
            ScenariosEntry("zzz.mod:SR_Z", Scenarios.RaceDomain),
            ScenariosEntry("aaa.mod:SR_A", Scenarios.RaceDomain),
        }, Scenarios.BuildBuiltIn(), DomainFilter.Everything);
        DomainPool? pool = mixed.PoolFor(Scenarios.RaceDomain);
        Check(pool != null && pool.Entries[0].PackKey == "aaa.mod:SR_A" && pool.Entries[1].PackKey == "zzz.mod:SR_Z", "pool sorted by PackKey ordinal", ref failures);
    }

    private static void SelectOff(ref int failures)
    {
        foreach (string scenario in Scenarios.ScenarioNames)
        {
            SqueakPoolRegistry registry = Scenarios.BuildRegistry(scenario);
            foreach (AudioDomain domain in Scenarios.DomainsFor(scenario))
            {
                foreach (int i in Range(0, SqueakyRatkin.SqueakActionDefinitions.Count))
                {
                    SqueakyRatkin.SqueakAction action = (SqueakyRatkin.SqueakAction)i;
                    ChainResult result = registry.Select(Ctx(domain, action), SqueakyRatkin.SqueakVoicePackMode.Off, SimGate.All, new LcgRandom(1));
                    string expectedKey = SqueakyRatkin.SqueakActionDefinitions.Get(action).AudioKey;
                    if (!(result.Tier == ChainTier.BuiltInFallback && result.SoundKey == expectedKey && result.PoolStableKey == null))
                    {
                        Check(false, "Off " + scenario + " " + domain + " " + action, ref failures);
                        return;
                    }
                }
            }
        }
        Check(true, "Off mode ignores pools, built-in only, stable PoolStableKey null", ref failures);
    }

    private static void SelectFallbackChain(ref int failures)
    {
        // S2 Race 域：x 跳过（无 xeno）→ r 命中 RacePack。
        SqueakPoolRegistry s2 = Scenarios.BuildRegistry("S2-builtin-seed");
        ChainResult r = s2.Select(Ctx(Scenarios.RaceDomain, SqueakyRatkin.SqueakAction.Call), SqueakyRatkin.SqueakVoicePackMode.Fallback, SimGate.All, new LcgRandom(1));
        Check(r.Tier == ChainTier.RacePack && r.PoolStableKey == "coahuilite.squeakyratkin:SR_OfficialExample_Race" && r.SoundKey != null && r.SoundKey.StartsWith("coahuilite.squeakyratkin:SR_OfficialExample_Race_SR_Call_"), "Fallback S2 race tier wins", ref failures);

        // S3 Xeno 域：x 命中 XenotypePack（1 entry 池）。
        SqueakPoolRegistry s3 = Scenarios.BuildRegistry("S3-builtin-plus-xeno");
        ChainResult x = s3.Select(Ctx(Scenarios.XenoDomain, SqueakyRatkin.SqueakAction.Eat), SqueakyRatkin.SqueakVoicePackMode.Fallback, SimGate.All, new LcgRandom(2));
        Check(x.Tier == ChainTier.XenotypePack && x.PoolStableKey == "other.mod:SR_XenoAVoice", "Fallback S3 xeno tier wins", ref failures);

        // S4 orphan xeno 域：无池 → x None → r 命中 RacePack（orphan 保留 = 域选择存在但无包 → 回退）。
        SqueakPoolRegistry s4 = Scenarios.BuildRegistry("S4-orphan-xeno");
        ChainResult s4r = s4.Select(Ctx(Scenarios.XenoDomain, SqueakyRatkin.SqueakAction.Sleep), SqueakyRatkin.SqueakVoicePackMode.Fallback, SimGate.All, new LcgRandom(3));
        Check(s4r.Tier == ChainTier.RacePack, "Fallback S4 orphan xeno falls to race", ref failures);

        // S1 空池 → builtin。
        SqueakPoolRegistry s1 = Scenarios.BuildRegistry("S1-empty");
        ChainResult s1r = s1.Select(Ctx(Scenarios.RaceDomain, SqueakyRatkin.SqueakAction.Call), SqueakyRatkin.SqueakVoicePackMode.Fallback, SimGate.All, new LcgRandom(4));
        Check(s1r.Tier == ChainTier.BuiltInFallback && s1r.SoundKey == "SR_Call", "Fallback S1 empty falls to built-in", ref failures);

        // 全无声：x 池 entry 全 Muted + race 池空 → builtin 仍可播（内置不受 gate 影响？gate 对 builtin 也执行——SimGate.All 可播；Partial 下 SR_* 无 Muted 后缀 → 可播）。
        SqueakPoolRegistry muted = new(new[]
        {
            MutedEntry("other.mod:SR_XenoAVoice", Scenarios.XenoDomain),
        }, Scenarios.BuildBuiltIn(), DomainFilter.Everything);
        ChainResult m = muted.Select(Ctx(Scenarios.XenoDomain, SqueakyRatkin.SqueakAction.Call), SqueakyRatkin.SqueakVoicePackMode.Fallback, SimGate.Partial, new LcgRandom(5));
        Check(m.Tier == ChainTier.BuiltInFallback && m.SoundKey == "SR_Call", "Fallback muted xeno falls to built-in", ref failures);
    }

    private static void SelectRemix(ref int failures)
    {
        // S3 Xeno 域：tiers=[x,r,vanilla] 全非 None → 等权折叠（固定序 [xeno, race, builtin]）。
        SqueakPoolRegistry s3 = Scenarios.BuildRegistry("S3-builtin-plus-xeno");
        HashSet<ChainTier?> seen = new();
        for (int seed = 1; seed <= 60; seed++)
        {
            ChainResult result = s3.Select(Ctx(Scenarios.XenoDomain, SqueakyRatkin.SqueakAction.Call), SqueakyRatkin.SqueakVoicePackMode.Remix, SimGate.All, new LcgRandom(seed));
            if (result.IsNone) { Check(false, "Remix S3 never none", ref failures); return; }
            seen.Add(result.Tier);
        }
        Check(seen.Contains(ChainTier.XenotypePack) && seen.Contains(ChainTier.RacePack) && seen.Contains(ChainTier.BuiltInFallback), "Remix S3 folds all three tiers", ref failures);

        // S1 空池 → tiers=[vanilla] → vanilla。
        SqueakPoolRegistry s1 = Scenarios.BuildRegistry("S1-empty");
        ChainResult s1r = s1.Select(Ctx(Scenarios.RaceDomain, SqueakyRatkin.SqueakAction.Call), SqueakyRatkin.SqueakVoicePackMode.Remix, SimGate.All, new LcgRandom(1));
        Check(s1r.Tier == ChainTier.BuiltInFallback && s1r.SoundKey == "SR_Call", "Remix S1 single tier", ref failures);
    }

    private static void DistributionBounds(ref int failures)
    {
        // entry 级：S2 Race 池 1 entry × 2 sounds → sound 级等权（400 次，界 0.3-0.7）。
        SqueakPoolRegistry s2 = Scenarios.BuildRegistry("S2-builtin-seed");
        int sound0 = 0, sound1 = 0;
        for (int seed = 1; seed <= 400; seed++)
        {
            ChainResult result = s2.Select(Ctx(Scenarios.RaceDomain, SqueakyRatkin.SqueakAction.Call), SqueakyRatkin.SqueakVoicePackMode.Fallback, SimGate.All, new LcgRandom(seed));
            if (result.SoundKey == null) { Check(false, "S2 always sound", ref failures); return; }
            if (result.SoundKey.EndsWith("_0")) sound0++;
            else sound1++;
        }
        double ratio = sound0 / 400.0;
        Check(ratio > 0.3 && ratio < 0.7, "S2 sound-level uniform (0.3-0.7): " + ratio.ToString("0.000"), ref failures);

        // entry 级：两 entry 池（各有 1 sound）→ 每 entry ~50%。
        SqueakPoolRegistry mixed = new(new[]
        {
            ScenariosEntry("aaa.mod:SR_A", Scenarios.RaceDomain, sounds: 1),
            ScenariosEntry("zzz.mod:SR_Z", Scenarios.RaceDomain, sounds: 1),
        }, Scenarios.BuildBuiltIn(), DomainFilter.Everything);
        int a = 0, z = 0;
        for (int seed = 1; seed <= 400; seed++)
        {
            ChainResult result = mixed.Select(Ctx(Scenarios.RaceDomain, SqueakyRatkin.SqueakAction.Call), SqueakyRatkin.SqueakVoicePackMode.Fallback, SimGate.All, new LcgRandom(seed));
            if (result.PoolStableKey == "aaa.mod:SR_A") a++;
            else z++;
        }
        double ar = a / 400.0;
        Check(ar > 0.3 && ar < 0.7, "two-entry pool uniform (0.3-0.7): " + ar.ToString("0.000"), ref failures);

        // Remix 三级分布有界（S3 Xeno 域，600 次，各级界 0.1-0.5）。
        SqueakPoolRegistry s3 = Scenarios.BuildRegistry("S3-builtin-plus-xeno");
        int tx = 0, tr = 0, tb = 0;
        for (int seed = 1; seed <= 600; seed++)
        {
            ChainResult result = s3.Select(Ctx(Scenarios.XenoDomain, SqueakyRatkin.SqueakAction.Call), SqueakyRatkin.SqueakVoicePackMode.Remix, SimGate.All, new LcgRandom(seed));
            if (result.Tier == ChainTier.XenotypePack) tx++;
            else if (result.Tier == ChainTier.RacePack) tr++;
            else tb++;
        }
        double txr = tx / 600.0, trr = tr / 600.0, tbr = tb / 600.0;
        Check(txr > 0.1 && txr < 0.5 && trr > 0.1 && trr < 0.5 && tbr > 0.1 && tbr < 0.5,
            "Remix tier distribution bounded (" + txr.ToString("0.00") + "/" + trr.ToString("0.00") + "/" + tbr.ToString("0.00") + ")", ref failures);
    }

    private static void Determinism(ref int failures)
    {
        foreach (string scenario in Scenarios.ScenarioNames)
        {
            SqueakPoolRegistry registry = Scenarios.BuildRegistry(scenario);
            foreach (AudioDomain domain in Scenarios.DomainsFor(scenario))
            {
                foreach (SqueakyRatkin.SqueakVoicePackMode mode in new[] { SqueakyRatkin.SqueakVoicePackMode.Off, SqueakyRatkin.SqueakVoicePackMode.Fallback, SqueakyRatkin.SqueakVoicePackMode.Remix })
                {
                    ChainResult first = registry.Select(Ctx(domain, SqueakyRatkin.SqueakAction.Call), mode, SimGate.All, new LcgRandom(42));
                    ChainResult second = registry.Select(Ctx(domain, SqueakyRatkin.SqueakAction.Call), mode, SimGate.All, new LcgRandom(42));
                    if (!(first.SoundKey == second.SoundKey && first.Tier == second.Tier && first.PoolStableKey == second.PoolStableKey))
                    {
                        Check(false, "determinism " + scenario + " " + domain + " " + mode, ref failures);
                        return;
                    }
                }
            }
        }
        Check(true, "same seed same result across scenarios/modes/domains", ref failures);
    }

    private static void SoundLevelFilter(ref int failures)
    {
        // S3 Xeno 池 entry：2 sounds 其中 1 个 _Muted → Partial gate 下 sound 抽取永远命中 playable 子集（1 个）。
        SqueakPoolRegistry s3 = Scenarios.BuildRegistry("S3-builtin-plus-xeno");
        for (int seed = 1; seed <= 50; seed++)
        {
            ChainResult result = s3.Select(Ctx(Scenarios.XenoDomain, SqueakyRatkin.SqueakAction.Call), SqueakyRatkin.SqueakVoicePackMode.Fallback, SimGate.Partial, new LcgRandom(seed));
            if (result.Tier != ChainTier.XenotypePack) { Check(false, "partial gate xeno tier", ref failures); return; }
            if (result.SoundKey == null || result.SoundKey.EndsWith("_Muted")) { Check(false, "muted sound never drawn: " + result.SoundKey, ref failures); return; }
        }
        Check(true, "sound-level filter excludes muted under Partial gate", ref failures);
    }

    private static void EntryLevelFilter(ref int failures)
    {
        // 全 Muted 的 entry 不参与 valid（entry 级过滤）→ 另一 entry 命中。
        SqueakPoolRegistry registry = new(new[]
        {
            MutedEntry("aaa.mod:SR_A", Scenarios.RaceDomain),
            ScenariosEntry("zzz.mod:SR_Z", Scenarios.RaceDomain, sounds: 1),
        }, Scenarios.BuildBuiltIn(), DomainFilter.Everything);
        for (int seed = 1; seed <= 50; seed++)
        {
            ChainResult result = registry.Select(Ctx(Scenarios.RaceDomain, SqueakyRatkin.SqueakAction.Call), SqueakyRatkin.SqueakVoicePackMode.Fallback, SimGate.Partial, new LcgRandom(seed));
            if (result.PoolStableKey != "zzz.mod:SR_Z") { Check(false, "muted entry skipped: " + result.PoolStableKey, ref failures); return; }
        }
        Check(true, "entry-level filter skips fully-muted entries", ref failures);
    }

    private static void DomainStatus(ref int failures)
    {
        Check(AudioDomainStatuses.Classify(new AudioDomain(Scenarios.Ratkin, null), true, true, true) == AudioDomainStatus.Available, "status available", ref failures);
        Check(AudioDomainStatuses.Classify(new AudioDomain(Scenarios.Ratkin, null), true, false, true) == AudioDomainStatus.Orphan, "status orphan (assembled, no selection)", ref failures);
        Check(AudioDomainStatuses.Classify(new AudioDomain(Scenarios.Ratkin, new XenotypeKey("X")), false, true, true) == AudioDomainStatus.TargetUnavailable, "status target unavailable", ref failures);
        Check(AudioDomainStatuses.Classify(new AudioDomain(Scenarios.Ratkin, new XenotypeKey("X")), false, true, false) == AudioDomainStatus.Dormant, "status dormant (biotech off)", ref failures);
    }

    private static void ModulationRules(ref int failures)
    {
        ModulationAxis mood = new(true, 0.8f, false, 1f, false, (1f, 1f));
        ModulationAxis age = new(false, 1f, true, 1.2f, true, (0.9f, 1.1f));
        ModulationAxis composed = Modulation.ComposeModulation(mood, age);
        Check(composed.HasPitch && composed.Pitch == 0.8f, "mood pitch explicit wins", ref failures);
        Check(composed.HasVolume && composed.Volume == 1.2f, "age volume inherited", ref failures);
        Check(composed.HasJitter && composed.Jitter == (0.9f, 1.1f), "age jitter inherited", ref failures);
        ModulationAxis identity = Modulation.ComposeModulation(ModulationAxis.Identity, ModulationAxis.Identity);
        Check(!identity.HasPitch && !identity.HasVolume && !identity.HasJitter, "identity when nothing present", ref failures);
    }

    private static void AgeDefault(ref int failures)
    {
        // 0.3.0：AgeTag 全 null = 全年龄；Select 结果不受 ctx.Age 影响。
        SqueakPoolRegistry s2 = Scenarios.BuildRegistry("S2-builtin-seed");
        ChainResult adult = s2.Select(new SelectionContext(Scenarios.RaceDomain, "Call", AgeBucket.Adult, true), SqueakyRatkin.SqueakVoicePackMode.Fallback, SimGate.All, new LcgRandom(7));
        ChainResult baby = s2.Select(new SelectionContext(Scenarios.RaceDomain, "Call", AgeBucket.Baby, true), SqueakyRatkin.SqueakVoicePackMode.Fallback, SimGate.All, new LcgRandom(7));
        Check(adult.SoundKey == baby.SoundKey && adult.Tier == baby.Tier, "age-neutral in 0.3.0 (all-age default)", ref failures);
    }

    // ---- helpers ----

    private static SelectionContext Ctx(AudioDomain domain, SqueakyRatkin.SqueakAction action)
        => new(domain, SqueakyRatkin.ActionKey.For(action)!, AgeBucket.Adult, false);

    private static IEnumerable<int> Range(int from, int count) { for (int i = from; i < from + count; i++) yield return i; }

    public static VoicePackEntry ScenariosEntry(string packKey, AudioDomain domain, int sounds = 2)
    {
        Dictionary<string, ActionSoundSet> actions = new();
        for (int i = 0; i < SqueakyRatkin.SqueakActionDefinitions.Count; i++)
        {
            SqueakyRatkin.SqueakAction action = (SqueakyRatkin.SqueakAction)i;
            string audioKey = SqueakyRatkin.SqueakActionDefinitions.Get(action).AudioKey;
            List<string> keys = new(sounds);
            for (int s = 0; s < sounds; s++) keys.Add(packKey + "_" + audioKey + "_" + s);
            actions[SqueakyRatkin.ActionKey.For(action)!] = new ActionSoundSet(keys, null, 1f);
        }
        return new VoicePackEntry(packKey, domain, 1f, actions);
    }

    private static VoicePackEntry MutedEntry(string packKey, AudioDomain domain)
    {
        Dictionary<string, ActionSoundSet> actions = new();
        for (int i = 0; i < SqueakyRatkin.SqueakActionDefinitions.Count; i++)
        {
            SqueakyRatkin.SqueakAction action = (SqueakyRatkin.SqueakAction)i;
            string audioKey = SqueakyRatkin.SqueakActionDefinitions.Get(action).AudioKey;
            actions[SqueakyRatkin.ActionKey.For(action)!] = new ActionSoundSet(new[] { packKey + "_" + audioKey + "_Muted" }, null, 1f);
        }
        return new VoicePackEntry(packKey, domain, 1f, actions);
    }
}
