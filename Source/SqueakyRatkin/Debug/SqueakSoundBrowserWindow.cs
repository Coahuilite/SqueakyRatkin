using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace SqueakyRatkin;

public class SqueakSoundBrowserWindow : Window
{
    private Vector2 scrollPosition = Vector2.zero;

    public override Vector2 InitialSize => new(620f, 720f);

    public SqueakSoundBrowserWindow()
    {
        doCloseX = true;
        doCloseButton = true;
        absorbInputAroundWindow = false;
        draggable = true;
    }

    public override void DoWindowContents(Rect inRect)
    {
        List<SoundDef> defs = DefDatabase<SoundDef>.AllDefs
            .Where(d => d.defName.StartsWith("SR_") && !d.defName.EndsWith("_Pure"))
            .OrderBy(d => d.defName)
            .ToList();

        Text.Font = GameFont.Medium;
        Widgets.Label(new Rect(inRect.x, inRect.y, inRect.width, 36f), "SR.Debug.BrowserTitle".Translate());
        Text.Font = GameFont.Small;

        Rect outRect = new(inRect.x, inRect.y + 42f, inRect.width, inRect.height - 42f);
        float rowHeight = 58f;
        Rect viewRect = new(0f, 0f, outRect.width - 16f, defs.Count * (rowHeight + 6f));

        Widgets.BeginScrollView(outRect, ref scrollPosition, viewRect);

        float curY = 0f;
        foreach (SoundDef def in defs)
        {
            Rect rowRect = new(0f, curY, viewRect.width, rowHeight);
            Rect labelRect = new(rowRect.x, rowRect.y, rowRect.width - 84f, rowRect.height);
            Rect buttonRect = new(rowRect.x + rowRect.width - 76f, rowRect.y + 16f, 72f, 24f);

            Widgets.Label(labelRect, $"{def.defName}\n{Describe(def)}");
            if (Widgets.ButtonText(buttonRect, "Play"))
            {
                def.PlayOneShot(SoundInfo.OnCamera());
            }

            curY += rowHeight + 6f;
        }

        Widgets.EndScrollView();
    }

    private static string Describe(SoundDef def)
    {
        SubSoundDef? subSound = def.subSounds?.FirstOrDefault();
        if (subSound == null)
        {
            return "subSound=<none> grains=<none>";
        }

        string grains = subSound.grains == null || subSound.grains.Count == 0
            ? "<none>"
            : string.Join(", ", subSound.grains.Select(DescribeGrain));

        return $"pitch={subSound.pitchRange} volume={subSound.volumeRange} dist={subSound.distRange} grains={grains}";
    }

    private static string DescribeGrain(AudioGrain grain)
    {
        if (grain is AudioGrain_Folder folder)
        {
            return $"Folder:{folder.clipFolderPath}";
        }

        return grain.GetType().Name;
    }
}
