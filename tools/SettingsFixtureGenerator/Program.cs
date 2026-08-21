using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Verse;

namespace SqueakyRatkin.FixtureGenerator;

/// <summary>
/// 0.2.4 ModSettings fixture 生成器（§6.2）。
/// 来源：0.2.4 真实序列化代码（链接真实记录类型 + Scribe 规则桩 + 契约提取），
/// 每个场景做 round-trip 自检（load→save 与 expected 字节一致；expected 再 load→save 幂等）。
/// 实机不可用时的降级产物；实机可用后以 0.2.4 游戏保存的真实文件替换并重新断言。
/// 输出：fixtures/input/*.xml（0.3.0 harness 的输入样本）与 fixtures/expected/*.xml（迁移后稳定态）。
/// </summary>
internal static class Program
{
    private const string BuiltInRacePackKey = "coahuilite.squeakyratkin:SR_OfficialExample_Race";
    private const string MissingPackKey = "coahuilite.squeakyratkin:SR_GonePack_9999";

    private static readonly string Root = FindRepositoryRoot();
    private static readonly string InputDir = Path.Combine(Root, "fixtures", "input");
    private static readonly string ExpectedDir = Path.Combine(Root, "fixtures", "expected");

    private static int Main()
    {
        Directory.CreateDirectory(InputDir);
        Directory.CreateDirectory(ExpectedDir);
        Log.Reset();

        int failures = 0;
        Run("01-new-install-first-save", Scenario01, hasInput: false, ref failures);
        Run("02-empty-file-no-schema", Scenario02, hasInput: true, ref failures);
        Run("03-explicit-off", Scenario03, hasInput: true, ref failures);
        Run("04-fallback-seeded", Scenario04, hasInput: true, ref failures);
        Run("05-multi-selections-lastwins", Scenario05, hasInput: true, ref failures);
        Run("06-orphan-packkey", Scenario06, hasInput: true, ref failures);
        Run("07-biotech-inactive-target", Scenario07, hasInput: true, ref failures);
        Run("08-corrupt-missing-fields", Scenario08, hasInput: true, ref failures);
        Run("09-mood-overrides-and-global-scope", Scenario09, hasInput: true, ref failures);

        if (Log.Captured.Count > 0)
        {
            Console.Error.WriteLine("--- Scribe/parse diagnostics captured ---");
            foreach (Log.Entry entry in Log.Captured) Console.Error.WriteLine($"[{entry.Level}] {entry.Text}");
        }

        Console.WriteLine(failures == 0
            ? $"OK: {9 - failures} scenarios green. fixtures/ written to {Root}\\fixtures"
            : $"FAILED: {failures} scenario(s) failed.");
        return failures == 0 ? 0 : 1;
    }

    private static void Run(string name, Func<(string? input, SqueakyRatkinSettings source)> build, bool hasInput, ref int failures)
    {
        Console.WriteLine("Scenario " + name + "...");
        try
        {
            (string? input, SqueakyRatkinSettings source) = build();
            // 正常场景 input = 0.2.4 序列化输出；仅 02/08 提供手写输入文本（空文件/损坏样本）。
            string? inputXml = hasInput ? (input ?? Save(source)) : null;
            // expected = 加载（含 PostLoadInit 修复/迁移）后的稳定态；01 无文件 = 默认对象直接保存。
            string expected = hasInput ? LoadSave(inputXml!) : Save(source);

            if (hasInput)
            {
                File.WriteAllText(Path.Combine(InputDir, name + ".xml"), inputXml!);
                AssertEqual("load-save matches expected", LoadSave(inputXml!), expected, ref failures);
            }
            AssertEqual("expected is idempotent", LoadSave(expected), expected, ref failures);
            File.WriteAllText(Path.Combine(ExpectedDir, name + ".xml"), expected);
            Console.WriteLine("  green: " + expected.Length + " bytes");
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("  EXCEPTION: " + ex);
            failures++;
        }
    }

    private static void AssertEqual(string what, string actual, string expected, ref int failures)
    {
        if (actual == expected)
        {
            Console.WriteLine("  ok: " + what);
            return;
        }
        Console.Error.WriteLine("  MISMATCH: " + what);
        Console.Error.WriteLine("--- expected ---\n" + expected);
        Console.Error.WriteLine("--- actual ---\n" + actual);
        failures++;
    }

    // ---- serialization drivers ----

    private static string Save(SqueakyRatkinSettings settings)
    {
        Scribe.saver.InitSaving("SettingsBlock");
        SqueakyRatkinSettings target = settings;
        Scribe_Deep.Look(ref target, "ModSettings");
        return Scribe.saver.FinalizeSaving();
    }

    private static string LoadSave(string xml)
    {
        ScribeExtractor.PostLoadInitQueue.Clear();
        Scribe.loader.InitLoading(xml);
        SqueakyRatkinSettings target = null!;
        Scribe_Deep.Look(ref target, "ModSettings");
        foreach (IExposable exposable in ScribeExtractor.PostLoadInitQueue)
        {
            Scribe.mode = LoadSaveMode.PostLoadInit;
            exposable.ExposeData();
        }
        Scribe.mode = LoadSaveMode.Inactive;
        ScribeExtractor.PostLoadInitQueue.Clear();
        return Save(target);
    }

    // ---- scenarios ----

    /// <summary>01 无文件新装：不经过 ExposeData，启动链种子后第一次保存 = 默认对象输出（schema=3 forceSave）。</summary>
    private static (string?, SqueakyRatkinSettings) Scenario01()
    {
        return (null, new SqueakyRatkinSettings());
    }

    /// <summary>02 文件存在但无任何节点（无 schema 节点旧配置）：加载 → schema 迁移写回。</summary>
    private static (string?, SqueakyRatkinSettings) Scenario02()
    {
        const string empty = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n<SettingsBlock>\n\t<ModSettings />\n</SettingsBlock>";
        SqueakyRatkinSettings probe = new();
        return (empty, probe);
    }

    /// <summary>03 显式 Off：voicePackMode 节点存在，其余默认省略。</summary>
    private static (string?, SqueakyRatkinSettings) Scenario03()
    {
        return (null, new SqueakyRatkinSettings { voicePackMode = SqueakVoicePackMode.Off });
    }

    /// <summary>04 Fallback + 内置种子：voicePackMode 省略（默认 Fallback）+ voicePackDefaultSeeded + Race 域内置 key。</summary>
    private static (string?, SqueakyRatkinSettings) Scenario04()
    {
        SqueakyRatkinSettings settings = new()
        {
            voicePackDefaultSeeded = true,
        };
        settings.voicePackSelections.Add(new VoicePackSelectionRecord
        {
            scope = SqueakVoicePackScope.Race,
            targetDefName = "",
            enabledPackKeys = new List<string> { BuiltInRacePackKey },
        });
        return (null, settings);
    }

    /// <summary>05 多个旧 Race+Xenotype selection + 同域 last-wins 重复 + xenotypePresets。</summary>
    private static (string?, SqueakyRatkinSettings) Scenario05()
    {
        SqueakyRatkinSettings settings = new()
        {
            voicePackMode = SqueakVoicePackMode.Fallback,
        };
        // Race 域两条记录（旧版本残留可能）：后者 last-wins。
        settings.voicePackSelections.Add(new VoicePackSelectionRecord
        {
            scope = SqueakVoicePackScope.Race,
            targetDefName = "",
            enabledPackKeys = new List<string> { "coahuilite.squeakyratkin:SR_OldRacePack" },
        });
        settings.voicePackSelections.Add(new VoicePackSelectionRecord
        {
            scope = SqueakVoicePackScope.Race,
            targetDefName = "",
            enabledPackKeys = new List<string> { BuiltInRacePackKey, "coahuilite.squeakyratkin:SR_ExtraRacePack" },
        });
        settings.voicePackSelections.Add(new VoicePackSelectionRecord
        {
            scope = SqueakVoicePackScope.Xenotype,
            targetDefName = "Baseliner_Highmate",
            enabledPackKeys = new List<string> { "other.mod:SR_HighmateVoice" },
        });
        settings.voicePackSelections.Add(new VoicePackSelectionRecord
        {
            scope = SqueakVoicePackScope.Xenotype,
            targetDefName = "Sanguophage",
            enabledPackKeys = new List<string> { "other.mod:SR_SanguoVoice" },
        });
        settings.xenotypePresets.Add(new XenotypePresetRecord
        {
            xenotypeDefName = "Baseliner_Highmate",
            hasOverallIntervalMultiplier = true,
            overallIntervalMultiplier = 1.5f,
            moodOverrides = new List<XenotypeMoodOverride>
            {
                new() { mood = SqueakMood.Good, hasPitchFactor = true, pitchFactor = 1.1f },
            },
            actionOverrides = new List<XenotypeActionBehaviorOverride>
            {
                new() { action = SqueakAction.Call, hasIntervalMultiplier = true, intervalMultiplier = 2f },
            },
        });
        return (null, settings);
    }

    /// <summary>06 消失 PackKey：enabledPackKeys 含未知 key（orphan 保留语义）。</summary>
    private static (string?, SqueakyRatkinSettings) Scenario06()
    {
        SqueakyRatkinSettings settings = new();
        settings.voicePackSelections.Add(new VoicePackSelectionRecord
        {
            scope = SqueakVoicePackScope.Race,
            targetDefName = "",
            enabledPackKeys = new List<string> { MissingPackKey },
        });
        return (null, settings);
    }

    /// <summary>07 Biotech inactive：Xenotype selection 目标未激活（dormant 运行时判定，fixture 保证记录持久形态）。</summary>
    private static (string?, SqueakyRatkinSettings) Scenario07()
    {
        SqueakyRatkinSettings settings = new();
        settings.voicePackSelections.Add(new VoicePackSelectionRecord
        {
            scope = SqueakVoicePackScope.Xenotype,
            targetDefName = "InactiveXeno_NotLoaded",
            enabledPackKeys = new List<string> { "other.mod:SR_InactiveXenoVoice" },
        });
        settings.xenotypePresets.Add(new XenotypePresetRecord
        {
            xenotypeDefName = "InactiveXeno_NotLoaded",
            actionOverrides = new List<XenotypeActionBehaviorOverride>(),
        });
        return (null, settings);
    }

    /// <summary>08 损坏缺字段：删除 enabledPackKeys、scope 非法值、voicePackMode 非法值（加载修复路径）。</summary>
    private static (string?, SqueakyRatkinSettings) Scenario08()
    {
        SqueakyRatkinSettings source = new()
        {
            voicePackMode = SqueakVoicePackMode.Remix,
        };
        source.voicePackSelections.Add(new VoicePackSelectionRecord
        {
            scope = SqueakVoicePackScope.Xenotype,
            targetDefName = "Sanguophage",
            enabledPackKeys = new List<string> { BuiltInRacePackKey },
        });
        source.globalActionEnabled.Add(new GlobalActionEnabledRecord
        {
            action = SqueakAction.Attack,
            enabled = false,
            scope = SqueakActionScope.Disabled,
        });
        string xml = Save(source);

        XmlDocument doc = new();
        doc.LoadXml(xml);
        // 删除 enabledPackKeys 子节点（缺字段）
        XmlNode? selection = doc.SelectSingleNode("//voicePackSelections/li");
        if (selection != null)
        {
            XmlNode? keys = selection["enabledPackKeys"];
            keys?.ParentNode?.RemoveChild(keys);
        }
        // scope 非法值
        XmlNode? scopeNode = doc.SelectSingleNode("//voicePackSelections/li/scope");
        if (scopeNode != null) scopeNode.InnerText = "Bogus";
        // voicePackMode 非法值
        XmlNode? modeNode = doc["ModSettings"]?["voicePackMode"];
        if (modeNode != null) modeNode.InnerText = "Weird";
        else
        {
            XmlElement mode = doc.CreateElement("voicePackMode");
            mode.InnerText = "Weird";
            doc.DocumentElement!.AppendChild(mode);
        }
        return (doc.OuterXml, source);
    }

    /// <summary>09 moodOverrides 字典 + globalActionEnabled（含 scope 缺失 legacy 形态 → NormalizeScope 修复）。</summary>
    private static (string?, SqueakyRatkinSettings) Scenario09()
    {
        SqueakyRatkinSettings source = new()
        {
            voicePackMode = SqueakVoicePackMode.Off,
        };
        source.moodOverrides[SqueakMood.Bad] = new SqueakMoodMod
        {
            mood = SqueakMood.Bad,
            pitchFactor = 0.8f,
            volumeFactor = 0.9f,
            pitchJitter = new FloatRange(0.9f, 1.1f),
        };
        // legacy bool-only 形态：scope 缺省 → PostLoadInit 用 DefaultScope（Attack=AnyOccurrence）
        source.globalActionEnabled.Add(new GlobalActionEnabledRecord
        {
            action = SqueakAction.Attack,
            enabled = true,
            scope = SqueakActionScope.AnyOccurrence,
            scopeWasLoaded = false,
        });
        source.globalActionEnabled.Add(new GlobalActionEnabledRecord
        {
            action = SqueakAction.Social,
            enabled = false,
            scope = SqueakActionScope.Disabled,
            scopeWasLoaded = false,
        });
        return (null, source);
    }

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
