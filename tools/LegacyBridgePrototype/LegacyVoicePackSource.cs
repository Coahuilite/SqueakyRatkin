using System.Collections.Generic;
using Verse;

namespace UniversalSqueaker;

// 单点 legacy 桥（未来 US 程序集内；SR 无 DLL 的接管版本才存在）。
// DefDatabase<Legacy>.AllDefs 枚举 → UpcastAll 一次性 upcast 为 canonical → 后续 validator/内核零感知。
// 拆除 = 删除 SqueakyRatkinLegacyVoicePackDef.cs + 本文件，并移除目录刷新中的唯一调用点。

public static class LegacyVoicePackSource
{
    public static IReadOnlyList<SqueakVoicePackDef> CollectAll()
    {
        List<SqueakVoicePackDef> result = new();
        foreach (SqueakyRatkin.SqueakVoicePackDef legacy in DefDatabase<SqueakyRatkin.SqueakVoicePackDef>.AllDefs)
            result.Add(legacy);
        return result;
    }

    /// <summary>Pure upcast point; never revalidates, reinterprets, or clones the defs.</summary>
    public static IEnumerable<SqueakVoicePackDef> UpcastAll(IEnumerable<SqueakyRatkin.SqueakVoicePackDef> legacyDefs)
    {
        foreach (SqueakyRatkin.SqueakVoicePackDef legacy in legacyDefs)
            yield return legacy;
    }
}
