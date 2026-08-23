using System;
using System.Collections.Generic;
using Verse;

namespace UniversalSqueaker;

// 前缀上下文（纯逻辑，LegacyBridgePrototype 编译期与 LegacyBridgeHarness 运行时共享）：
// legacy 包沿用 SR_ 作者 ABI，canonical US 包用 US_。validator 只拿 canonical 类型，
// 按实例运行时类型选前缀，因此 legacy 校验与 canonical 校验共用同一套规则（零复制）。

public static class LegacyVoicePackBridge
{
    public const string LegacyPrefix = "SR_";
    public const string CanonicalPrefix = "US_";

    public static string RequiredPrefixFor(Type defType)
        => defType == typeof(SqueakyRatkin.SqueakVoicePackDef) ? LegacyPrefix : CanonicalPrefix;

    public static bool HasValidIdentity(SqueakVoicePackDef def)
        => !string.IsNullOrEmpty(def.defName)
           && def.defName.StartsWith(RequiredPrefixFor(def.GetType()), StringComparison.Ordinal);

    /// <summary>SoundDef 引用前缀错误（仅检查 defName 引用；真正的 SoundDef 解析由 Verse 交叉引用完成）。</summary>
    public static IEnumerable<string> PrefixErrors(SqueakVoicePackDef def)
    {
        string prefix = RequiredPrefixFor(def.GetType());
        foreach (SqueakVoicePackAction entry in def.actions ?? new List<SqueakVoicePackAction>())
        {
            if (entry == null) continue;
            foreach (var sound in entry.sounds ?? new List<SoundDef>())
                if (sound != null && !string.IsNullOrEmpty(sound.defName) && !sound.defName.StartsWith(prefix, StringComparison.Ordinal))
                    yield return "action " + entry.action + " references SoundDef without " + prefix + " prefix: " + sound.defName;
        }
        foreach (SqueakVoicePackFallback fallback in def.fallbacks ?? new List<SqueakVoicePackFallback>())
        {
            var sound = fallback?.sound;
            if (sound != null && !string.IsNullOrEmpty(sound.defName) && !sound.defName.StartsWith(prefix, StringComparison.Ordinal))
                yield return "fallback references SoundDef without " + prefix + " prefix: " + sound.defName;
        }
    }
}
