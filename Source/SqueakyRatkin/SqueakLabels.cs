using Verse;

namespace SqueakyRatkin;

/// <summary>本地化 helper:动作/心情显示名走 Keyed(SR.Action.*/SR.Mood.*),供 UI/调试复用。</summary>
public static class SqueakLabels
{
    public static string Action(SqueakAction a) => ("SR.Action." + a).Translate();
    public static string Mood(SqueakMood m) => ("SR.Mood." + m).Translate();
    public static string SettingsCategory => "SR.SettingsCategory".Translate();
}
