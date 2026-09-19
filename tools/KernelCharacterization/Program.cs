using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SqueakyRatkin.Kernel;

namespace SqueakyRatkin.KernelCharacterization;

/// <summary>
/// Kernel 验证门（决策 §5 验证门 + §4.9 黄金语料）。
/// 1) 单测：语义规范逐条断言（UnitTests）。
/// 2) 正常运行：设置全项矩阵（场景 × mode × 域 × 17 action × 彩蛋开关 × 多种子 × gate 面）生成确定性
///    输入→期望 ChainResult 字节，并在写入前与 fixtures/corpus/corpus-0.3.1.txt 的冻结基线比较。
///    只有显式 <c>--update-corpus</c> 维护模式可以重建该基线。
/// 3) 双语料回放：corpus-0.3.0.txt（15 动作冻结基线，0.3.0 固化）逐字节回放，任何 delta = 0.3.0 语义回归，
///    该文件永不被 --update-corpus 触碰；corpus-0.3.1.txt（17 动作 + 彩蛋维度）为当前基线。
/// 语料场景构造（Scenarios.cs）在 0.3.x 窗口内不得改动；扩展场景（S6-egg/S7-two-races）只进 0.3.1 语料。
/// </summary>
internal static class Program
{
    private const string LegacyCorpusFileName = "corpus-0.3.0.txt";
    private const string LegacyCorrectedCorpusFileName = "corpus-0.3.0-r6.txt";
    private const string CorpusFileName = "corpus-0.3.1.txt";
    private const string UpdateCorpusArgument = "--update-corpus";

    /// <summary>0.3.3 R6 修正（Remix 仅折叠非 None tier）在 0.3.0 冻结矩阵上造成的确定性行差。
    /// 历史文件 corpus-0.3.0.txt 保持不变，作为修正前语义的证据；其差异必须是本常量条 Remix 行，
    /// 任何非 Remix 差异或条数漂移（= 场景构造或其它语义被改动）都判失败。</summary>
    private const int ExpectedLegacyRemixCorrectionRows = 540;
    private static readonly long[] Seeds = { 1, 2, 3 };
    private static readonly UTF8Encoding Utf8WithoutBom = new(false);

    private static int Main(string[] args)
    {
        if (args.Length > 1 || (args.Length == 1 && !string.Equals(args[0], UpdateCorpusArgument, StringComparison.Ordinal)))
        {
            Console.Error.WriteLine("Usage: SqueakyKernelCharacterization [--update-corpus]");
            return 2;
        }
        try
        {
            return Run(args.Length == 1);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("UNHANDLED: " + ex.GetType().FullName + " :: " + ex.Message);
            return 1;
        }
    }

    private static int Run(bool updateCorpus)
    {
        int failures = 0;
        Console.WriteLine("Unit tests...");
        UnitTests.RunAll(ref failures);

        string corpusDir = Path.Combine(FindRepositoryRoot(), "fixtures", "corpus");
        string corpusPath = Path.Combine(corpusDir, CorpusFileName);
        string legacyHistoryPath = Path.Combine(corpusDir, LegacyCorpusFileName);
        string legacyReplayPath = Path.Combine(corpusDir, LegacyCorrectedCorpusFileName);

        Console.WriteLine("Golden corpus generation (17 actions + egg dimension)...");
        string corpus;
        try
        {
            corpus = GenerateCorpus(legacy: false);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("  corpus generation exception: " + ex.GetType().FullName + " :: " + ex.Message);
            return 1;
        }
        byte[] generated = Utf8WithoutBom.GetBytes(corpus);

        Console.WriteLine(updateCorpus ? "Golden corpus update..." : "Golden corpus replay...");
        if (updateCorpus)
        {
            Directory.CreateDirectory(corpusDir);
            File.WriteAllBytes(corpusPath, generated);
            Console.WriteLine("  corpus updated: " + corpusPath + " (" + CountLines(corpus) + " cases)");
        }
        else if (!File.Exists(corpusPath))
        {
            Console.Error.WriteLine("  FAIL: committed corpus missing: " + corpusPath);
            return 1;
        }

        byte[] baseline = File.ReadAllBytes(corpusPath);
        if (BytesEqual(generated, baseline))
        {
            Console.WriteLine(updateCorpus
                ? "  ok: updated corpus byte-stable"
                : "  ok: committed corpus replay zero delta (byte-identical)");
        }
        else
        {
            Console.Error.WriteLine("  FAIL: corpus delta detected (generated " + generated.Length + " bytes, baseline " + baseline.Length + " bytes)");
            failures++;
        }

        // 0.3.0 矩阵：历史文件 corpus-0.3.0.txt（修正前语义）保持只读；回放目标 = corpus-0.3.0-r6.txt
        // （0.3.3 R6 修正后重建，字节级冻结）。历史文件与修正文件的差异必须是且仅是 ExpectedLegacyRemixCorrectionRows 条 Remix 行。
        Console.WriteLine("Legacy 0.3.0 corpus replay (R6-corrected, frozen)...");
        string legacyCorpus;
        try
        {
            legacyCorpus = GenerateCorpus(legacy: true);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("  legacy corpus generation exception: " + ex.GetType().FullName + " :: " + ex.Message);
            return 1;
        }
        byte[] legacyGenerated = Utf8WithoutBom.GetBytes(legacyCorpus);
        if (updateCorpus)
        {
            Directory.CreateDirectory(corpusDir);
            File.WriteAllBytes(legacyReplayPath, legacyGenerated);
            Console.WriteLine("  legacy corrected corpus updated: " + legacyReplayPath + " (" + CountLines(legacyCorpus) + " cases)");
        }
        else if (!File.Exists(legacyReplayPath))
        {
            Console.Error.WriteLine("  FAIL: committed legacy corrected corpus missing: " + legacyReplayPath);
            return 1;
        }
        byte[] legacyBaseline = File.ReadAllBytes(legacyReplayPath);
        if (BytesEqual(legacyGenerated, legacyBaseline))
        {
            Console.WriteLine("  ok: legacy 0.3.0 corrected corpus replay zero delta (byte-identical, " + CountLines(legacyCorpus) + " cases)");
        }
        else
        {
            Console.Error.WriteLine("  FAIL: legacy corrected corpus delta detected (generated " + legacyGenerated.Length + " bytes, baseline " + legacyBaseline.Length + " bytes) — 场景构造/镜像改动或语义回归；只有显式裁决过的 R6 修正可以改变此文件。");
            failures++;
        }

        if (!updateCorpus) CheckLegacyCorrection(legacyGenerated, legacyHistoryPath, ref failures);

        Console.WriteLine("Replay determinism check...");
        byte[] replay = Utf8WithoutBom.GetBytes(GenerateCorpus(legacy: false));
        if (BytesEqual(generated, replay))
        {
            Console.WriteLine("  ok: deterministic replay zero delta");
        }
        else
        {
            Console.Error.WriteLine("  FAIL: deterministic replay delta detected");
            failures++;
        }

        Console.WriteLine(failures == 0 ? "Kernel characterization passed." : "Kernel characterization FAILED (" + failures + ").");
        return failures == 0 ? 0 : 1;
    }

    /// <summary>历史 0.3.0 语料 vs R6 修正后同一矩阵：只允许 Remix 行的结果差异，且条数固定。
    /// 前缀（scenario|mode|domain|action|seed|gate）必须逐字段相同——保证差异来自结果映射而非用例身份漂移。</summary>
    private static void CheckLegacyCorrection(byte[] generated, string legacyHistoryPath, ref int failures)
    {
        if (!File.Exists(legacyHistoryPath))
        {
            Console.Error.WriteLine("  FAIL: legacy historical corpus missing: " + legacyHistoryPath);
            failures++;
            return;
        }
        string[] history = DataLines(Utf8WithoutBom.GetString(File.ReadAllBytes(legacyHistoryPath)));
        string[] current = DataLines(Utf8WithoutBom.GetString(generated));
        if (history.Length != current.Length)
        {
            Console.Error.WriteLine("  FAIL: legacy correction comparison line count mismatch (history " + history.Length + " vs current " + current.Length + ")");
            failures++;
            return;
        }

        const int LegacyResultFieldIndex = 6; // scenario|mode|domain|action|seed|gate|soundKey|...
        int differing = 0;
        int nonRemix = 0;
        int prefixDrift = 0;
        string firstExample = "";
        for (int i = 0; i < history.Length; i++)
        {
            if (history[i] == current[i]) continue;
            string[] h = history[i].Split('|');
            string[] c = current[i].Split('|');
            if (h.Length < LegacyResultFieldIndex || c.Length < LegacyResultFieldIndex)
            {
                prefixDrift++;
                continue;
            }
            for (int f = 0; f < LegacyResultFieldIndex; f++)
            {
                if (!string.Equals(h[f], c[f], StringComparison.Ordinal)) { prefixDrift++; break; }
            }
            if (!string.Equals(h[1], "Remix", StringComparison.Ordinal)) nonRemix++;
            differing++;
            if (firstExample.Length == 0) firstExample = h[0] + " " + h[3] + " seed=" + h[4] + "/" + h[5] + ": " + h[6] + "|" + h[7] + " -> " + c[6] + "|" + c[7];
        }

        bool ok = differing == ExpectedLegacyRemixCorrectionRows && nonRemix == 0 && prefixDrift == 0;
        string message = "legacy correction delta: rows=" + differing + " (expected " + ExpectedLegacyRemixCorrectionRows + "), non-remix=" + nonRemix + ", prefix-drift=" + prefixDrift;
        if (ok) Console.WriteLine("  ok: " + message + "; first: " + firstExample);
        else
        {
            Console.Error.WriteLine("  FAIL: " + message + "; first: " + firstExample);
            failures++;
        }
    }

    /// <summary>语料数据行（跳过 '#' 头注释；修正文件比历史文件多一行说明）。</summary>
    private static string[] DataLines(string text)
    {
        List<string> lines = new();
        foreach (string line in text.Split('\n'))
        {
            if (line.Length == 0 || line[0] == '#') continue;
            lines.Add(line);
        }
        return lines.ToArray();
    }

    private static bool BytesEqual(byte[] left, byte[] right)
    {
        if (left.Length != right.Length) return false;
        for (int i = 0; i < left.Length; i++) if (left[i] != right[i]) return false;
        return true;
    }

    /// <summary>全矩阵语料生成。确定性：每 case 独立 roll 流（seed 派生），同输入同输出。
    /// legacy=true = 0.3.0 冻结格式（15 动作、无彩蛋列、场景 S1-S5 + F03-F07，逐字节复刻已提交 corpus-0.3.0.txt）；
    /// legacy=false = 0.3.1 格式（17 动作、彩蛋开关维度、场景 S1-S7 + F03-F07）。</summary>
    private static string GenerateCorpus(bool legacy)
    {
        StringBuilder sb = new();
        if (legacy)
        {
            sb.Append("# 0.3.0 golden corpus (R6-corrected) - kernel Select (SqueakPoolRegistry). Lines: scenario|mode|domain|action|seed|gate|soundKey|tier|poolStableKey; '-' = none.\n");
            sb.Append("# Rebuilt by tools/KernelCharacterization; scenarios frozen in Scenarios.cs (S1-S5 constructed, F03-F07 fixture-driven from fixtures/input). Any delta on replay = regression.\n");
            sb.Append("# 0.3.3 R6 correction: Remix folds only non-None tiers; corpus-0.3.0.txt keeps the pre-correction rows as historical evidence.\n");
        }
        else
        {
            sb.Append("# 0.3.1 golden corpus - kernel Select (SqueakPoolRegistry). Lines: scenario|mode|domain|action|seed|gate|eggs|soundKey|tier|poolStableKey; '-' = none.\n");
            sb.Append("# Rebuilt by tools/KernelCharacterization; scenarios frozen in Scenarios.cs (S1-S7 constructed incl. egg/two-race, F03-F07 fixture-driven from fixtures/input). Any delta on replay = regression.\n");
        }
        string fixturesRoot = Path.Combine(FindRepositoryRoot(), "fixtures");
        string[] constructed = legacy ? Scenarios.ScenarioNames : Scenarios.ExtendedScenarioNames;
        foreach (string scenario in constructed)
        {
            SqueakPoolRegistry registry = Scenarios.BuildRegistry(scenario);
            AppendScenarioCases(sb, scenario, registry, Scenarios.DomainsFor(scenario), legacy);
        }
        foreach (string scenario in Scenarios.FixtureScenarioNames)
        {
            SqueakPoolRegistry registry = Scenarios.BuildFixtureRegistry(scenario, fixturesRoot);
            AppendScenarioCases(sb, scenario, registry, Scenarios.DomainsForFixture(scenario), legacy);
        }
        return sb.ToString();
    }

    private static void AppendScenarioCases(StringBuilder sb, string scenario, SqueakPoolRegistry registry, AudioDomain[] domains, bool legacy)
    {
        int actionCount = legacy ? ActionAudioKeyMirror.BuiltInCount : ActionAudioKeyMirror.Count;
        bool[] eggFlags = legacy ? new[] { false } : new[] { false, true };
        foreach (SelectionMode mode in new[] { SelectionMode.Off, SelectionMode.Fallback, SelectionMode.Remix })
        {
            foreach (AudioDomain domain in domains)
            {
                foreach (int actionIndex in Range(0, actionCount))
                {
                    SqueakyRatkin.SqueakAction action = (SqueakyRatkin.SqueakAction)actionIndex;
                    string actionKey = ActionKey.For(action)!;
                    foreach (bool allowEggs in eggFlags)
                    {
                        foreach (long seed in Seeds)
                        {
                            foreach (SimGate gate in new[] { SimGate.All, SimGate.Partial })
                            {
                                ChainResult result = registry.Select(
                                    new SelectionContext(domain, actionKey, AgeBucket.Adult, false, allowEggs),
                                    mode, gate, new LcgRandom(seed));
                                sb.Append(scenario).Append('|').Append(ModeName(mode)).Append('|').Append(domain).Append('|')
                                  .Append(actionKey).Append('|').Append(seed).Append('|').Append(gate == SimGate.All ? "all" : "partial").Append('|');
                                if (!legacy) sb.Append(allowEggs ? "on" : "off").Append('|');
                                sb.Append(result.SoundKey ?? "-").Append('|').Append(TierName(result.Tier)).Append('|')
                                  .Append(result.PoolStableKey ?? "-").Append('\n');
                            }
                        }
                    }
                }
            }
        }
    }

    private static string ModeName(SelectionMode mode) => mode switch
    {
        SelectionMode.Off => "Off",
        SelectionMode.Fallback => "Fallback",
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

    internal static string FindRepositoryRoot()
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
