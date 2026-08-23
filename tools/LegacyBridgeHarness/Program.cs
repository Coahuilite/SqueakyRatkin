using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using UniversalSqueaker;
using Verse;

namespace SqueakyRatkin.LegacyBridgeHarness;

// Legacy 桥运行时语义 harness（纯 net472，无需游戏程序集）。
// 与 LegacyBridgePrototype 编译期证明组合后覆盖：
//   编译期：薄继承真实 Verse.Def / DefDatabase<Legacy> 泛型约束 / canonical 字段类型
//   运行时：层级字段填充模拟、SR_ vs US_ 前缀上下文、AllDefs 枚举 + 单点 upcast、
//           拆除面（源码层只有桥文件引用 legacy 命名空间）
// 真正在游戏内跑 Verse.DefDatabase 的步骤仍需维护者实机执行（见原型 README）。

internal static class Program
{
    private static int failures;

    private static int Main()
    {
        try
        {
            VerifyTypeShape();
            VerifyHierarchyFieldFill();
            VerifyPrefixContexts();
            VerifyDefDatabaseEnumerationAndUpcast();
            VerifySourceSeparation();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("Legacy bridge harness crash: " + ex.GetType().FullName + " | " + ex.Message);
            return 1;
        }

        if (failures == 0)
        {
            Console.WriteLine("Legacy bridge harness passed (runtime semantics on shared bridge sources).");
            return 0;
        }

        Console.Error.WriteLine("Legacy bridge harness failed: " + failures + " assertion(s).");
        return 1;
    }

    private static void VerifyTypeShape()
    {
        Type legacy = typeof(SqueakyRatkin.SqueakVoicePackDef);
        Type canonical = typeof(UniversalSqueaker.SqueakVoicePackDef);

        Check(legacy.IsSubclassOf(canonical), "legacy root is a subclass of the canonical root");
        Check(canonical.IsSubclassOf(typeof(Def)), "canonical root is a Def subclass");
        Check(legacy.FullName == "SqueakyRatkin.SqueakVoicePackDef", "legacy XML node name stays SqueakyRatkin.SqueakVoicePackDef");
        Check(legacy.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Length == 0,
            "legacy root declares no fields of its own (thin inheritance)");

        HashSet<string> inherited = new(legacy.GetFields(BindingFlags.Public | BindingFlags.Instance).Select(f => f.Name), StringComparer.Ordinal);
        foreach (string expected in new[] { "defName", "scope", "raceDefName", "targetDefName", "weight", "fallbacks", "actions" })
            Check(inherited.Contains(expected), "legacy inherits public field " + expected);

        foreach (FieldInfo field in canonical.GetFields(BindingFlags.Public | BindingFlags.Instance))
            Check(field.FieldType.Namespace != "SqueakyRatkin" && !field.FieldType.Name.StartsWith("SqueakyRatkin", StringComparison.Ordinal),
                "canonical field " + field.Name + " does not reference the legacy namespace");
    }

    private static void VerifyHierarchyFieldFill()
    {
        // 与 Verse XmlToObjectUtils.SearchTypeHierarchy 同方向的层级填充路径（运行时自检）。
        object legacy = FormatterServices.GetUninitializedObject(typeof(SqueakyRatkin.SqueakVoicePackDef));
        SetByHierarchy(legacy, "defName", "SR_Legacy_Race");
        SetByHierarchy(legacy, "raceDefName", "Ratkin");
        SetByHierarchy(legacy, "weight", 1.5f);
        SetByHierarchy(legacy, "scope", UniversalSqueaker.SqueakVoicePackScope.Race);

        Check(Equals("SR_Legacy_Race", GetByHierarchy(legacy, "defName")), "hierarchy fill reaches Def.defName on a legacy instance");
        Check(Equals("Ratkin", GetByHierarchy(legacy, "raceDefName")), "hierarchy fill reaches inherited canonical raceDefName");
        Check(Equals(1.5f, GetByHierarchy(legacy, "weight")), "hierarchy fill reaches inherited canonical weight");
        Check(Equals(UniversalSqueaker.SqueakVoicePackScope.Race, GetByHierarchy(legacy, "scope")), "hierarchy fill reaches inherited canonical scope");
    }

    private static void VerifyPrefixContexts()
    {
        SqueakyRatkin.SqueakVoicePackDef legacy = BuildLegacy("SR_Legacy_Race", "SR_");
        UniversalSqueaker.SqueakVoicePackDef canonical = BuildCanonical("US_Canonical_Race", "US_");

        Check(LegacyVoicePackBridge.RequiredPrefixFor(typeof(SqueakyRatkin.SqueakVoicePackDef)) == "SR_", "legacy runtime type selects SR_ prefix");
        Check(LegacyVoicePackBridge.RequiredPrefixFor(typeof(UniversalSqueaker.SqueakVoicePackDef)) == "US_", "canonical runtime type selects US_ prefix");
        Check(LegacyVoicePackBridge.HasValidIdentity(legacy), "legacy defName passes SR_ identity");
        Check(LegacyVoicePackBridge.HasValidIdentity(canonical), "canonical defName passes US_ identity");

        SqueakyRatkin.SqueakVoicePackDef wrongLegacy = BuildLegacy("US_Wrong_Legacy", "SR_");
        UniversalSqueaker.SqueakVoicePackDef wrongCanonical = BuildCanonical("SR_Wrong_Canonical", "US_");
        Check(!LegacyVoicePackBridge.HasValidIdentity(wrongLegacy), "legacy defName with US_ prefix is rejected fail-closed");
        Check(!LegacyVoicePackBridge.HasValidIdentity(wrongCanonical), "canonical defName with SR_ prefix is rejected fail-closed");

        Check(!LegacyVoicePackBridge.PrefixErrors(legacy).Any(), "legacy SR_ sound refs produce no prefix errors");
        Check(!LegacyVoicePackBridge.PrefixErrors(canonical).Any(), "canonical US_ sound refs produce no prefix errors");

        SqueakyRatkin.SqueakVoicePackDef badRefs = BuildLegacy("SR_Legacy_Race", "US_");
        Check(LegacyVoicePackBridge.PrefixErrors(badRefs).Count() == 2, "wrong-prefix sound refs are reported once per ref (action + fallback)");
    }

    private static void VerifyDefDatabaseEnumerationAndUpcast()
    {
        Verse.DefDatabase<SqueakyRatkin.SqueakVoicePackDef>.Reset();
        SqueakyRatkin.SqueakVoicePackDef first = BuildLegacy("SR_Legacy_Race", "SR_");
        SqueakyRatkin.SqueakVoicePackDef second = BuildLegacy("SR_Legacy_Race_Two", "SR_");
        Verse.DefDatabase<SqueakyRatkin.SqueakVoicePackDef>.AllDefs.Add(first);
        Verse.DefDatabase<SqueakyRatkin.SqueakVoicePackDef>.AllDefs.Add(second);

        List<UniversalSqueaker.SqueakVoicePackDef> collected = LegacyVoicePackSource.CollectAll().ToList();
        Check(collected.Count == 2, "DefDatabase<Legacy>.AllDefs enumeration collects every legacy def");
        Check(ReferenceEquals(collected[0], first) && ReferenceEquals(collected[1], second),
            "CollectAll/UpcastAll pass the same instances by reference (no clone, no reinterpretation)");
        Check(collected.All(d => d.GetType() == typeof(SqueakyRatkin.SqueakVoicePackDef)),
            "upcast preserves runtime legacy type for SR_ prefix context");
    }

    private static void VerifySourceSeparation()
    {
        string prototypeRoot = Path.GetFullPath(Path.Combine(FindProjectRoot(), "..", "LegacyBridgePrototype"));
        Check(Directory.Exists(prototypeRoot), "LegacyBridgePrototype source directory exists next to the harness");
        if (!Directory.Exists(prototypeRoot)) return;

        foreach (string file in Directory.GetFiles(prototypeRoot, "*.cs"))
        {
            string text = File.ReadAllText(file);
            bool isBridgeSurface = file.EndsWith("LegacyVoicePackBridge.cs", StringComparison.Ordinal)
                                   || file.EndsWith("LegacyVoicePackSource.cs", StringComparison.Ordinal)
                                   || file.EndsWith("SqueakyRatkinLegacyVoicePackDef.cs", StringComparison.Ordinal);
            if (!isBridgeSurface)
                Check(text.IndexOf("SqueakyRatkin.", StringComparison.Ordinal) < 0 && text.IndexOf("SqueakyRatkin;", StringComparison.Ordinal) < 0,
                    Path.GetFileName(file) + " (canonical surface) does not reference the legacy namespace");
        }

        string sourceText = File.ReadAllText(Path.Combine(prototypeRoot, "LegacyVoicePackSource.cs"));
        Check(sourceText.IndexOf("DefDatabase<SqueakyRatkin.SqueakVoicePackDef>.AllDefs", StringComparison.Ordinal) >= 0,
            "LegacyVoicePackSource is the single DefDatabase<Legacy> collection point");
        string legacyTypeText = File.ReadAllText(Path.Combine(prototypeRoot, "SqueakyRatkinLegacyVoicePackDef.cs"));
        Check(legacyTypeText.IndexOf("public class SqueakVoicePackDef : UniversalSqueaker.SqueakVoicePackDef", StringComparison.Ordinal) >= 0,
            "legacy root source is the thin subclass declaration only");
    }

    private static SqueakyRatkin.SqueakVoicePackDef BuildLegacy(string defName, string soundPrefix)
    {
        var def = new SqueakyRatkin.SqueakVoicePackDef();
        def.defName = defName;
        def.raceDefName = "Ratkin";
        def.actions = Actions(soundPrefix);
        def.fallbacks = Fallbacks(soundPrefix);
        return def;
    }

    private static UniversalSqueaker.SqueakVoicePackDef BuildCanonical(string defName, string soundPrefix)
    {
        var def = new UniversalSqueaker.SqueakVoicePackDef();
        def.defName = defName;
        def.raceDefName = "Human";
        def.actions = Actions(soundPrefix);
        def.fallbacks = Fallbacks(soundPrefix);
        return def;
    }

    private static List<UniversalSqueaker.SqueakVoicePackAction> Actions(string prefix)
        => new() { new UniversalSqueaker.SqueakVoicePackAction { action = "Select", IsEgg = true, sounds = new List<SoundDef> { Sound(prefix + "Legacy_Select") } } };

    private static List<UniversalSqueaker.SqueakVoicePackFallback> Fallbacks(string prefix)
        => new() { new UniversalSqueaker.SqueakVoicePackFallback { action = "Select", sound = Sound(prefix + "Legacy_Select_Fallback") } };

    private static SoundDef Sound(string defName) => new() { defName = defName };

    private static void SetByHierarchy(object target, string name, object? value)
    {
        for (Type? type = target.GetType(); type != null; type = type.BaseType)
        {
            FieldInfo? field = type.GetField(name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            if (field == null) continue;
            field.SetValue(target, value);
            return;
        }
        throw new MissingFieldException(target.GetType().FullName, name);
    }

    private static object? GetByHierarchy(object target, string name)
    {
        for (Type? type = target.GetType(); type != null; type = type.BaseType)
        {
            FieldInfo? field = type.GetField(name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            if (field != null) return field.GetValue(target);
        }
        throw new MissingFieldException(target.GetType().FullName, name);
    }

    private static string FindProjectRoot()
    {
        DirectoryInfo? dir = new(AppContext.BaseDirectory);
        while (dir != null)
        {
            if (File.Exists(Path.Combine(dir.FullName, "LegacyBridgeHarness.csproj"))) return dir.FullName;
            dir = dir.Parent;
        }
        throw new DirectoryNotFoundException("LegacyBridgeHarness project root not found above " + AppContext.BaseDirectory);
    }

    private static void Check(bool condition, string message)
    {
        if (condition) Console.WriteLine("  ok: " + message);
        else { failures++; Console.Error.WriteLine("  FAIL: " + message); }
    }
}
