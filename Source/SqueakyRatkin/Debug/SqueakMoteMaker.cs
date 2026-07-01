using System.Linq;
using UnityEngine;
using Verse;

namespace SqueakyRatkin;

/// <summary>
/// SR_Mote_TextBg 的可配置文字偏移(世界坐标,格数)。
/// 默认 offsetY=0.8 让文字浮在 pawn 头顶而非脚下中心(exactPosition)。
/// 改 1.6/Defs/MoteDefs/SR_Mote.xml 的 modExtensions 即可调位置,无需重编译。
/// </summary>
public class SqueakMoteOffset : DefModExtension
{
    public float offsetX = 0f;
    public float offsetY = 0.8f;
}

public class MoteTextWithBackground : MoteText
{
    public override void DrawGUIOverlay()
    {
        if (string.IsNullOrEmpty(text))
        {
            return;
        }

        SqueakMoteOffset? ext = def.modExtensions.OfType<SqueakMoteOffset>().FirstOrDefault();
        float ox = ext?.offsetX ?? 0f;
        float oy = ext?.offsetY ?? 0f;

        Vector2 worldPos = new(exactPosition.x + ox, exactPosition.z + oy);
        const float edge = 0.04f;

        GenMapUI.DrawText(worldPos + new Vector2(-edge, 0f), text, Color.black);
        GenMapUI.DrawText(worldPos + new Vector2(edge, 0f), text, Color.black);
        GenMapUI.DrawText(worldPos + new Vector2(0f, -edge), text, Color.black);
        GenMapUI.DrawText(worldPos + new Vector2(0f, edge), text, Color.black);
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
