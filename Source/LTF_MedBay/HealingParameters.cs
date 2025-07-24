using System;
using Verse;

namespace LTF_MedBay;

public static class HealingParameters
{
    public enum CoveragePriority
    {
        [Description("Small")] Small = 1,
        [Description("Big")] Big = 2,
        [Description("Random")] Random = 4
    }

    public enum DepthPriority
    {
        [Description("Inside")] Inside = 1,
        [Description("Outside")] Outside = 2,
        [Description("Random")] Random = 4
    }

    public enum HeightPriority
    {
        [Description("Bottom")] Bottom = 1,
        [Description("Middle")] Middle = 2,
        [Description("Top")] Top = 4,
        [Description("Random")] Random = 8
    }

    public enum PriorityPriority
    {
        TagFirst = 1,
        DepthFirst = 2,
        CoverageFirst = 4,
        HeightFirst = 8,
        TagSecond = 16,
        DepthSecond = 32,
        CoverageSecond = 64,
        HeightSecond = 128,
        TagThird = 256,
        DepthThird = 512,
        CoverageThird = 1024,
        HeightThird = 2048,
        TagFourth = 4096,
        DepthFourth = 8192,
        CoverageFourth = 16384,
        HeightFourth = 32768,
        Regular = 33825
    }

    public enum TagPriority
    {
        [Description("BloodPumpingSource")] BloodPumpingSource = 1,
        [Description("MetabolismSource")] MetabolismSource = 2,
        [Description("BreathingSource")] BreathingSource = 4,
        [Description("Random")] Random = 8
    }

    [Flags]
    public enum TypePriority
    {
        [Description("Tending")] Tending = 1,
        [Description("Regen")] Regen = 2,
        [Description("Random")] Random = 4
    }

    private static bool prioritizeType(this TypePriority typePriority, TypePriority typePriorityDef)
    {
        return (typePriority & typePriorityDef) != 0;
    }

    public static bool TendingFirst(this TypePriority priority)
    {
        return priority.prioritizeType(TypePriority.Tending);
    }

    public static bool RegenFirst(this TypePriority priority)
    {
        return priority.prioritizeType(TypePriority.Regen);
    }
}