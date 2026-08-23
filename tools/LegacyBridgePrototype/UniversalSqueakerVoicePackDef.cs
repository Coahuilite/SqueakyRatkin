using System.Collections.Generic;
using Verse;
using Verse.Sound;

namespace UniversalSqueaker;

// Legacy-bridge prototype (0.3.2, scratch, not shipped by SR).
// This file mirrors the future US canonical VoicePack root type. It is intentionally shaped after
// the 0.3.1 author XML ABI that US inherits; the kernel/validator only ever sees this canonical type.

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
