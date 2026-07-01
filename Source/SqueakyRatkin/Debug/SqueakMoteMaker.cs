using UnityEngine;
using Verse;

namespace SqueakyRatkin;

public class MoteTextWithBackground : MoteText
{
    public override void DrawGUIOverlay()
    {
        if (string.IsNullOrEmpty(text))
        {
            return;
        }

        Vector2 worldPos = new(exactPosition.x, exactPosition.z);
        const float offset = 0.04f;

        GenMapUI.DrawText(worldPos + new Vector2(-offset, 0f), text, Color.black);
        GenMapUI.DrawText(worldPos + new Vector2(offset, 0f), text, Color.black);
        GenMapUI.DrawText(worldPos + new Vector2(0f, -offset), text, Color.black);
        GenMapUI.DrawText(worldPos + new Vector2(0f, offset), text, Color.black);
        GenMapUI.DrawText(worldPos, text, Color.white);
    }
}

public static class SqueakMoteMaker
{
    public static void ThrowSqueakText(Vector3 loc, Map map, string text)
    {
        ThingDef? moteDef = DefDatabase<ThingDef>.GetNamedSilentFail("SR_Mote_TextBg");
        if (moteDef == null)
        {
            return;
        }

        MoteTextWithBackground? mote = ThingMaker.MakeThing(moteDef) as MoteTextWithBackground;
        if (mote == null)
        {
            return;
        }

        mote.exactPosition = loc;
        mote.text = text;
        mote.textColor = Color.black;
        GenSpawn.Spawn(mote, loc.ToIntVec3(), map);
    }
}
