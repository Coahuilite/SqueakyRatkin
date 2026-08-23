using SqueakyRatkin.Kernel;
using Verse;

namespace SqueakyRatkin;

/// <summary>
/// Adapts RimWorld's already-resolved current life stage to the Kernel age bucket. RimWorld 1.6
/// exposes DevelopmentalStage directly; see docs/internal-universalization-design-note-zh.md §年龄维度.
/// Newborn/Baby share Baby, Child maps to Child, and Toddler intentionally has no native 1.6 mapping.
/// Missing or all other stages deliberately degrade to Adult.
/// </summary>
internal static class SqueakLifeStageResolver
{
    public static AgeBucket Resolve(Pawn? pawn)
    {
        DevelopmentalStage stage = pawn?.ageTracker?.CurLifeStage?.developmentalStage ?? DevelopmentalStage.Adult;
        return stage == DevelopmentalStage.Newborn || stage == DevelopmentalStage.Baby
            ? AgeBucket.Baby
            : stage == DevelopmentalStage.Child
                ? AgeBucket.Child
                : AgeBucket.Adult;
    }
}
