using Verse;
using Verse.Sound;

namespace SqueakyRatkin;

public static class SqueakDebug
{
    public static bool ShowOverlay = false;
    public static bool ShowCameraIndicator = false;

    public static void NotifySqueak(Pawn pawn, SqueakAction action, SqueakMood mood, SoundDef def)
    {
        if (Prefs.DevMode)
        {
            Log.Message($"[SqueakyRatkin] {pawn?.LabelShort} action={action} mood={mood} sound={def?.defName}");
        }

        if (ShowOverlay && pawn?.Map != null && def != null)
        {
            string text = $"{SqueakLabels.Action(action)} / {SqueakLabels.Mood(mood)}\n{def.defName}";
            SqueakMoteMaker.ThrowSqueakText(pawn.DrawPos, pawn.Map, text);
        }
    }
}
