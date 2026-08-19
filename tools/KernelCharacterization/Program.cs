using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SqueakyRatkin.Kernel;

namespace SqueakyRatkin.KernelCharacterization;

/// <summary>
/// Kernel 验证门（决策 §5 验证门 + §4.9 黄金语料）。
/// 1) 单测：语义规范逐条断言（UnitTests）。
/// 2) 黄金语料生成：设置全项矩阵（场景 × mode × 域 × 15 action × 多种子 × gate 面）→ 确定性
///    输入→期望 ChainResult 行，写入 fixtures/corpus/corpus-0.3.0.txt；生成后立即回放（零 delta 自检）。
/// 0.3.1 切核：以同 harness（仅更新 Kernel 链接）回放 corpus，任何 delta = 回归。
/// 语料场景构造（Scenarios.cs）在 0.3.x 窗口内不得改动。
/// </summary>
internal static class Program
{
    private const string CorpusFileName = "corpus-0.3.0.txt";
    private static readonly long[] Seeds = { 1, 2, 3 };

    private static int Main()
    {
        try
        {
            return Run();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("UNHANDLED: " + ex.GetType().FullName + " :: " + ex.Message);
            return 1;
        }
    }

    private static int Run()
    {
        int failures = 0;
        Console.WriteLine("Unit tests...");
        UnitTests.RunAll(ref failures);

        Console.WriteLine("Golden corpus...");
        string corpusDir = Path.Combine(FindRepositoryRoot(), "fixtures", "corpus");
        Directory.CreateDirectory(corpusDir);
        string corpusPath = Path.Combine(corpusDir, CorpusFileName);
        string corpus;
        try
        {
            corpus = GenerateCorpus();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("  corpus generation exception: " + ex.GetType().FullName + " :: " + ex.Message);
            return 1;
        }
        File.WriteAllText(corpusPath, corpus, new UTF8Encoding(false));
        Console.WriteLine("  corpus written: " + corpusPath + " (" + CountLines(corpus) + " cases)");

        Console.WriteLine("Replay self-check...");
        string replay = GenerateCorpus();
        if (replay == corpus)
        {
            Console.WriteLine("  ok: replay zero delta");
        }
        else
        {
            Console.Error.WriteLine("  FAIL: replay delta detected");
            failures++;
        }

        Console.WriteLine(failures == 0 ? "Kernel characterization passed." : "Kernel characterization FAILED (" + failures + ").");
        return failures == 0 ? 0 : 1;
    }

    /// <summary>全矩阵语料生成。确定性：每 case 独立 roll 流（seed 派生），同输入同输出。</summary>
    private static string GenerateCorpus()
    {
        StringBuilder sb = new();
        sb.Append("# 0.3.0 golden corpus - kernel Select (SqueakPoolRegistry). Lines: scenario|mode|domain|action|seed|gate|soundKey|tier|poolStableKey; '-' = none.\n");
        sb.Append("# Rebuilt by tools/KernelCharacterization; scenarios frozen in Scenarios.cs. Any delta on replay = regression.\n");
        foreach (string scenario in Scenarios.ScenarioNames)
        {
            SqueakPoolRegistry registry = Scenarios.BuildRegistry(scenario);
            foreach (SqueakyRatkin.SqueakVoicePackMode mode in new[] { SqueakyRatkin.SqueakVoicePackMode.Off, SqueakyRatkin.SqueakVoicePackMode.Fallback, SqueakyRatkin.SqueakVoicePackMode.Remix })
            {
                foreach (AudioDomain domain in Scenarios.DomainsFor(scenario))
                {
                    foreach (int actionIndex in Range(0, SqueakyRatkin.SqueakActionDefinitions.Count))
                    {
                        SqueakyRatkin.SqueakAction action = (SqueakyRatkin.SqueakAction)actionIndex;
                        string actionKey = SqueakyRatkin.ActionKey.For(action)!;
                        foreach (long seed in Seeds)
                        {
                            foreach (SimGate gate in new[] { SimGate.All, SimGate.Partial })
                            {
                                ChainResult result = registry.Select(
                                    new SelectionContext(domain, actionKey, AgeBucket.Adult, false),
                                    mode, gate, new LcgRandom(seed));
                                sb.Append(scenario).Append('|').Append(ModeName(mode)).Append('|').Append(domain).Append('|')
                                  .Append(actionKey).Append('|').Append(seed).Append('|').Append(gate == SimGate.All ? "all" : "partial").Append('|')
                                  .Append(result.SoundKey ?? "-").Append('|').Append(TierName(result.Tier)).Append('|')
                                  .Append(result.PoolStableKey ?? "-").Append('\n');
                            }
                        }
                    }
                }
            }
        }
        return sb.ToString();
    }

    private static string ModeName(SqueakyRatkin.SqueakVoicePackMode mode) => mode switch
    {
        SqueakyRatkin.SqueakVoicePackMode.Off => "Off",
        SqueakyRatkin.SqueakVoicePackMode.Fallback => "Fallback",
        _ => "Remix",
    };

    private static string TierName(ChainTier? tier) => tier switch
    {
        ChainTier.XenotypePack => "XenotypePack",
        ChainTier.RacePack => "RacePack",
        ChainTier.PackFallback => "PackFallback",
        ChainTier.BuiltInFallback => "BuiltInFallback",
        _ => "-",
    };

    private static int CountLines(string text)
    {
        int count = 0;
        foreach (char c in text) if (c == '\n') count++;
        return count;
    }

    private static IEnumerable<int> Range(int from, int count) { for (int i = from; i < from + count; i++) yield return i; }

    private static string FindRepositoryRoot()
    {
        DirectoryInfo? dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir != null)
        {
            if (File.Exists(Path.Combine(dir.FullName, "AGENTS.md"))) return dir.FullName;
            dir = dir.Parent;
        }
        throw new InvalidOperationException("Repository root (AGENTS.md) not found from " + AppContext.BaseDirectory);
    }
}
