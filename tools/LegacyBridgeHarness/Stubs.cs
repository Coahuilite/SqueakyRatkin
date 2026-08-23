using System.Collections.Generic;

// 纯 net472 桩：只提供 legacy 桥源文件编译所需的 Verse 最小形状。
// 这不是 RimWorld 实现；真实 Verse 形状由 LegacyBridgePrototype 的 compile-only 构建证明。

namespace Verse
{
    public class Def
    {
        public string defName = "";
    }

    public class SoundDef : Def
    {
    }

    public static class DefDatabase<T> where T : Def
    {
        public static readonly List<T> AllDefs = new();

        public static void Reset() => AllDefs.Clear();
    }
}
