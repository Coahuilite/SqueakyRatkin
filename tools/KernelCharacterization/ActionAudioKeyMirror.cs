using System;
using System.Collections.Generic;

namespace SqueakyRatkin.KernelCharacterization;

/// <summary>
/// Local mirror of the current 15 <c>SqueakActionDefinitions.AudioKey</c> values.
/// KernelCharacterization deliberately does not link that SR product metadata file; update this
/// mirror only in the coordinated action-ABI change that also expands the corpus.
/// </summary>
internal static class ActionAudioKeyMirror
{
    private static readonly string[] audioKeys =
    {
        "SR_Call",
        "SR_Eat",
        "SR_Sleep",
        "SR_Wounded",
        "SR_Select",
        "SR_Move",
        "SR_Social",
        "SR_Joy",
        "SR_Death",
        "SR_Draft",
        "SR_Undraft",
        "SR_Attack",
        "SR_Work",
        "SR_Equip",
        "SR_MentalBreak",
    };

    public static IReadOnlyList<string> All => Array.AsReadOnly(audioKeys);

    public static int Count => audioKeys.Length;

    public static string For(SqueakyRatkin.SqueakAction action)
    {
        int index = (int)action;
        if ((uint)index >= (uint)audioKeys.Length) throw new ArgumentOutOfRangeException(nameof(action));
        return audioKeys[index];
    }
}
