using System;
using System.Collections.Generic;
using System.Globalization;

namespace Verse;

/// <summary>
/// Characterization stub: minimal Verse surface required by the linked 0.2.4 record types
/// and the Scribe emulation. FloatRange semantics are copied verbatim from RimWorld 1.6
/// (Source/Verse/FloatRange.cs); other types are empty shells (never constructed with data).
/// </summary>
public struct FloatRange : IEquatable<FloatRange>
{
    public float min;
    public float max;

    public static FloatRange Zero => new(0f, 0f);
    public static FloatRange One => new(1f, 1f);

    public FloatRange(float min, float max) { this.min = min; this.max = max; }

    public static FloatRange FromString(string s)
    {
        CultureInfo invariantCulture = CultureInfo.InvariantCulture;
        string[] array = s.Split('~');
        if (array.Length == 1)
        {
            float num = Convert.ToSingle(array[0], invariantCulture);
            return new FloatRange(num, num);
        }
        return new FloatRange(Convert.ToSingle(array[0], invariantCulture), Convert.ToSingle(array[1], invariantCulture));
    }

    public override string ToString() => min.ToString("G9") + "~" + max.ToString("G9");

    public override int GetHashCode() => min.GetHashCode() * 397 ^ max.GetHashCode();

    public bool Equals(FloatRange other) => min == other.min && max == other.max;
    public override bool Equals(object obj) => obj is FloatRange other && Equals(other);
    public static bool operator ==(FloatRange a, FloatRange b) => a.Equals(b);
    public static bool operator !=(FloatRange a, FloatRange b) => !a.Equals(b);
}

public enum SoundContext { Any, MapOnly }

public abstract class Def
{
    public string defName = "";
    public ModContentPack modContentPack;

    public virtual IEnumerable<string> ConfigErrors() { yield break; }
}

public class ModContentPack
{
    public ModMetaData ModMetaData;
}

public class ModMetaData
{
    public string PackageIdNonUnique = "";
}

public static class DefDatabase<T> where T : Def
{
    public static T GetNamedSilentFail(string defName) => null;
}

public class ModSettings : IExposable
{
    public virtual void ExposeData() { }
}

public static class Log
{
    public readonly struct Entry
    {
        public readonly string Level;
        public readonly string Text;
        public Entry(string level, string text) { Level = level; Text = text; }
    }

    public static readonly List<Entry> Captured = new();

    public static void Message(string text) => Captured.Add(new Entry("info", text));
    public static void Warning(string text) => Captured.Add(new Entry("warning", text));
    public static void Error(string text) => Captured.Add(new Entry("error", text));
    public static void Reset() => Captured.Clear();
}
