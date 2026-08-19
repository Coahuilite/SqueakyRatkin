using System.Collections.Generic;
using Verse;

namespace Verse.Sound;

/// <summary>Stub shell for the Verse.Sound surface referenced by linked 0.2.4 record types (never data-bearing).</summary>
public class SoundDef : Def
{
    public bool sustain;
    public SoundContext context;
    public List<SubSoundDef> subSounds = new();
}

public class SubSoundDef
{
    public bool onCamera;
    public List<Grain> grains = new();

    public class Grain { }
}
