using System.Collections.Generic;
using Verse;

// LegacyBridgeHarness 的 canonical/legacy 运行时镜像（与 LegacyBridgePrototype 的 Verse 形状逐字段一致）。
// 用途仅是让共享桥逻辑可在无游戏程序集的环境运行；真实加载仍以编译期原型 + 实机 Verse 步骤为准。

namespace UniversalSqueaker
{
    public enum SqueakVoicePackScope { Unspecified = 0, Race = 1, Xenotype = 2 }
    public enum AgeBucket { Baby, Toddler, Child, Adult }

    public class SqueakVoicePackDef : Def
    {
        public SqueakVoicePackScope scope = SqueakVoicePackScope.Unspecified;
        public string raceDefName = "";
        public string targetDefName = "";
        public float weight = 1f;
        public List<SqueakVoicePackFallback> fallbacks = new();
        public List<SqueakVoicePackAction> actions = new();
    }

    public class SqueakVoicePackAction
    {
        public string action = "Call";
        public AgeBucket? ageTag = null;
        public bool IsEgg = false;
        public List<SoundDef> sounds = new();
    }

    public class SqueakVoicePackFallback
    {
        public string action = "Call";
        public SoundDef sound = null!;
    }
}

namespace SqueakyRatkin
{
    public class SqueakVoicePackDef : UniversalSqueaker.SqueakVoicePackDef
    {
    }
}
