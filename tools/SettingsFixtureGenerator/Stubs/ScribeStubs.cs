using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;

namespace Verse;

public interface IExposable
{
    void ExposeData();
}

public enum LoadSaveMode { Inactive, Saving, LoadingVars, ResolvingCrossRefs, PostLoadInit }

public enum LookMode { Undefined, Value, Deep, Reference, Def }

/// <summary>
/// Characterization stub of Verse.Scribe + Scribe_Values + Scribe_Collections + Scribe_Deep
/// (RimWorld 1.6). Saving/LoadingVars semantics are ported from the real sources:
///   Source/Verse/Scribe.cs (mode, EnterNode/ExitNode)
///   Source/Verse/ScribeSaver.cs (InitSaving: documentElementName root, tab indent)
///   Source/Verse/Scribe_Values.cs (default-omission, forceSave, float G9, IsNull)
///   Source/Verse/Scribe_Collections.cs (li children, keys/values dictionary shape)
///   Source/Verse/ScribeExtractor.cs (ValueFromNode, SaveableFromNode)
/// PostLoadInit runs after the whole LoadingVars pass, mirroring Scribe.loader.initer.
/// </summary>
public static class Scribe
{
    public static LoadSaveMode mode = LoadSaveMode.Inactive;
    public static ScribeSaver saver = new();
    public static ScribeLoader loader = new();

    public static bool EnterNode(string nodeName)
    {
        return mode == LoadSaveMode.Saving ? saver.EnterNode(nodeName) : loader.EnterNode(nodeName);
    }

    public static void ExitNode()
    {
        if (mode == LoadSaveMode.Saving) saver.ExitNode();
        else loader.ExitNode();
    }
}

public sealed class ScribeSaver
{
    // XmlWriter overrides the declaration encoding with the writer's encoding: use a UTF-8 StringWriter
    // so the emitted declaration matches RimWorld's real ModSettings files (encoding="utf-8").
    private sealed class Utf8StringWriter : StringWriter
    {
        public override System.Text.Encoding Encoding => System.Text.Encoding.UTF8;
    }

    private Utf8StringWriter stream = null!;
    private XmlWriter writer = null!;

    public void InitSaving(string documentElementName)
    {
        Scribe.mode = LoadSaveMode.Saving;
        stream = new Utf8StringWriter();
        XmlWriterSettings settings = new()
        {
            Indent = true,
            IndentChars = "\t",
        };
        writer = XmlWriter.Create(stream, settings);
        writer.WriteStartDocument();
        EnterNode(documentElementName);
    }

    public string FinalizeSaving()
    {
        ExitNode();
        writer.WriteEndDocument();
        writer.Flush();
        writer.Close();
        Scribe.mode = LoadSaveMode.Inactive;
        string text = stream.ToString();
        stream.Dispose();
        return text;
    }

    public bool EnterNode(string nodeName)
    {
        if (writer == null) return false;
        writer.WriteStartElement(nodeName);
        return true;
    }

    public void ExitNode()
    {
        if (writer != null) writer.WriteEndElement();
    }

    public void WriteElement(string elementName, string value) => writer.WriteElementString(elementName, value);

    public void WriteAttribute(string attributeName, string value) => writer.WriteAttributeString(attributeName, value);
}

public sealed class ScribeLoader
{
    private XmlNode root = null!;
    private readonly Stack<XmlNode> parentStack = new();

    public XmlNode curXmlParent { get; set; } = null!;

    public void InitLoading(string xml)
    {
        XmlDocument doc = new();
        doc.LoadXml(xml);
        root = doc.DocumentElement!;
        curXmlParent = root;
        parentStack.Clear();
        // Mirrors ScribeLoader.InitLoading: entering LoadingVars mode.
        Scribe.mode = LoadSaveMode.LoadingVars;
    }

    public bool EnterNode(string label)
    {
        XmlNode child = curXmlParent[label];
        if (child == null) return false;
        parentStack.Push(curXmlParent);
        curXmlParent = child;
        return true;
    }

    public void ExitNode()
    {
        curXmlParent = parentStack.Count > 0 ? parentStack.Pop() : root;
    }
}

public static class Scribe_Values
{
    public static void Look<T>(ref T value, string label, T defaultValue = default, bool forceSave = false)
    {
        if (Scribe.mode == LoadSaveMode.Saving)
        {
            if (!forceSave && (value != null || defaultValue == null))
            {
                if (value == null) return;
                if (value.Equals(defaultValue)) return;
            }
            if (value == null)
            {
                if (!Scribe.EnterNode(label)) return;
                try { Scribe.saver.WriteAttribute("IsNull", "True"); }
                finally { Scribe.ExitNode(); }
                return;
            }
            string text = value switch
            {
                float num => num.ToString("G9"),
                _ => value.ToString()!,
            };
            Scribe.saver.WriteElement(label, text);
        }
        else if (Scribe.mode == LoadSaveMode.LoadingVars)
        {
            value = ScribeExtractor.ValueFromNode(Scribe.loader.curXmlParent[label], defaultValue);
        }
    }
}

public static class Scribe_Collections
{
    public static void Look<T>(ref List<T> list, string label, LookMode lookMode = LookMode.Undefined, params object[] ctorArgs)
    {
        if (Scribe.EnterNode(label))
        {
            try
            {
                if (Scribe.mode == LoadSaveMode.Saving)
                {
                    if (list == null) { Scribe.saver.WriteAttribute("IsNull", "True"); return; }
                    foreach (T item in list)
                    {
                        switch (lookMode)
                        {
                            case LookMode.Value:
                            {
                                T value = item;
                                Scribe_Values.Look(ref value, "li", default, forceSave: true);
                                break;
                            }
                            case LookMode.Deep:
                            {
                                T target = item;
                                Scribe_Deep.Look(ref target, "li", ctorArgs);
                                break;
                            }
                        }
                    }
                }
                else if (Scribe.mode == LoadSaveMode.LoadingVars)
                {
                    XmlAttribute isNull = Scribe.loader.curXmlParent.Attributes["IsNull"];
                    if (isNull != null && isNull.Value.Equals("true", StringComparison.InvariantCultureIgnoreCase))
                    {
                        list = null;
                    }
                    else
                    {
                        list = new List<T>(Scribe.loader.curXmlParent.ChildNodes.Count);
                        foreach (XmlNode child in Scribe.loader.curXmlParent.ChildNodes)
                        {
                            switch (lookMode)
                            {
                                case LookMode.Value:
                                    list.Add(ScribeExtractor.ValueFromNode(child, default(T)));
                                    break;
                                case LookMode.Deep:
                                    list.Add(ScribeExtractor.SaveableFromNode<T>(child, ctorArgs));
                                    break;
                            }
                        }
                    }
                }
            }
            finally { Scribe.ExitNode(); }
        }
        else if (Scribe.mode == LoadSaveMode.LoadingVars)
        {
            list = null;
        }
    }

    public static void Look<K, V>(ref Dictionary<K, V> dict, string label, LookMode keyLookMode, LookMode valueLookMode)
    {
        List<K> keys = null!;
        List<V> values = null!;
        Look(ref dict, label, keyLookMode, valueLookMode, ref keys, ref values);
    }

    private static void Look<K, V>(ref Dictionary<K, V> dict, string label, LookMode keyLookMode, LookMode valueLookMode, ref List<K> keysWorkingList, ref List<V> valuesWorkingList)
    {
        if (Scribe.EnterNode(label))
        {
            try
            {
                if (Scribe.mode == LoadSaveMode.Saving && dict == null)
                {
                    Scribe.saver.WriteAttribute("IsNull", "True");
                    return;
                }
                if (Scribe.mode == LoadSaveMode.LoadingVars)
                {
                    XmlAttribute isNull = Scribe.loader.curXmlParent.Attributes["IsNull"];
                    if (isNull != null && isNull.Value.Equals("true", StringComparison.InvariantCultureIgnoreCase))
                    {
                        dict = null;
                        return;
                    }
                    dict = new Dictionary<K, V>();
                }
                if (Scribe.mode == LoadSaveMode.Saving || Scribe.mode == LoadSaveMode.LoadingVars)
                {
                    keysWorkingList = new List<K>();
                    valuesWorkingList = new List<V>();
                    if (Scribe.mode == LoadSaveMode.Saving && dict != null)
                    {
                        foreach (KeyValuePair<K, V> item in dict)
                        {
                            keysWorkingList.Add(item.Key);
                            valuesWorkingList.Add(item.Value);
                        }
                    }
                }
                if (Scribe.mode == LoadSaveMode.Saving || dict != null)
                {
                    Look(ref keysWorkingList, "keys", keyLookMode);
                    Look(ref valuesWorkingList, "values", valueLookMode);
                }
                if (Scribe.mode == LoadSaveMode.LoadingVars)
                {
                    BuildDictionary(dict, keysWorkingList, valuesWorkingList);
                }
            }
            finally { Scribe.ExitNode(); }
        }
        else if (Scribe.mode == LoadSaveMode.LoadingVars)
        {
            dict = null;
        }
    }

    private static void BuildDictionary<K, V>(Dictionary<K, V> dict, List<K> keys, List<V> values)
    {
        if (dict == null || keys == null || values == null) return;
        int count = Math.Min(keys.Count, values.Count);
        for (int i = 0; i < count; i++)
        {
            dict[keys[i]] = values[i];
        }
    }
}

public static class Scribe_Deep
{
    // Mirrors the real signature: no generic constraints; instances are created via Activator in ScribeExtractor.
    public static void Look<T>(ref T target, string label, params object[] ctorArgs)
    {
        if (Scribe.EnterNode(label))
        {
            try
            {
                if (Scribe.mode == LoadSaveMode.Saving)
                {
                    if (target != null)
                    {
                        IExposable exposable = (IExposable)target;
                        exposable.ExposeData();
                    }
                    else Scribe.saver.WriteAttribute("IsNull", "True");
                }
                else if (Scribe.mode == LoadSaveMode.LoadingVars)
                {
                    target = ScribeExtractor.SaveableFromNode<T>(Scribe.loader.curXmlParent, ctorArgs);
                }
                else if (Scribe.mode == LoadSaveMode.PostLoadInit && target != null)
                {
                    IExposable exposable = (IExposable)target;
                    exposable.ExposeData();
                }
            }
            finally { Scribe.ExitNode(); }
        }
        else if (Scribe.mode == LoadSaveMode.LoadingVars)
        {
            target = default!;
        }
    }
}

public static class ScribeExtractor
{
    /// <summary>Pending PostLoadInit targets, run after the LoadingVars pass (mirrors Scribe.loader.initer).</summary>
    public static readonly List<IExposable> PostLoadInitQueue = new();

    public static T ValueFromNode<T>(XmlNode subNode, T defaultValue)
    {
        if (subNode == null) return defaultValue;
        XmlAttribute isNull = subNode.Attributes["IsNull"];
        if (isNull != null && isNull.Value.Equals("true", StringComparison.InvariantCultureIgnoreCase)) return default;
        try
        {
            return ParseHelper.FromString<T>(subNode.InnerText);
        }
        catch (Exception ex)
        {
            // Mirrors ScribeExtractor.ValueFromNode: parse failure logs and falls back to default(T).
            Log.Error("Exception parsing node " + subNode.OuterXml + " into a " + typeof(T) + ":\n" + ex);
            return default;
        }
    }

    public static T SaveableFromNode<T>(XmlNode subNode, object[] ctorArgs)
    {
        if (subNode == null) return default!;
        XmlAttribute isNull = subNode.Attributes["IsNull"];
        if (isNull != null && isNull.Value.Equals("true", StringComparison.InvariantCultureIgnoreCase)) return default!;
        T instance = (T)Activator.CreateInstance(typeof(T), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, ctorArgs, null)!;
        IExposable exposable = (IExposable)instance;
        XmlNode previous = Scribe.loader.curXmlParent;
        Scribe.loader.curXmlParent = subNode;
        try { exposable.ExposeData(); }
        finally { Scribe.loader.curXmlParent = previous; }
        PostLoadInitQueue.Add(exposable);
        return instance;
    }
}

/// <summary>
/// Minimal ParseHelper mirror for the value types used by 0.2.4 settings:
/// float (invariant), bool (bool.Parse), enum (Enum.Parse, exact case), FloatRange ("min~max"),
/// string (verbatim), int (invariant).
/// </summary>
public static class ParseHelper
{
    public static T FromString<T>(string str)
    {
        Type type = typeof(T);
        if (type == typeof(float)) return (T)(object)float.Parse(str, System.Globalization.CultureInfo.InvariantCulture);
        if (type == typeof(bool)) return (T)(object)bool.Parse(str);
        if (type == typeof(int)) return (T)(object)int.Parse(str, System.Globalization.CultureInfo.InvariantCulture);
        if (type == typeof(string)) return (T)(object)str;
        if (type == typeof(FloatRange)) return (T)(object)FloatRange.FromString(str);
        if (type.IsEnum) return (T)Enum.Parse(type, str);
        throw new InvalidOperationException("Unsupported fixture parse type: " + type);
    }
}
