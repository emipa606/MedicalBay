using System.Collections.Generic;
using Verse;

namespace LTF_MedBay;

public class CompProperties_LTF_MedBay : CompProperties
{
    public readonly float fuelConsumptionPerRegenPoint = 1.75f;

    public readonly float fuelConsumptionPerTendingQuality = 1.25f;

    public readonly List<HediffDef> regenerableHediffDef = [];

    public readonly float regenMax = 0.7f;

    public readonly float regenMaxPlaceboOffset = 0.1f;

    public readonly float regenMaxQualityOffset = 0.15f;

    public readonly float tendingQualityPlaceboOffset = 0.25f;

    public readonly float tendingQualityQualityOffset = 0.1f;
    public bool debug;

    public bool regeneratesBionics;

    public FloatRange tendingQualityRange = new(0.45f, 0.65f);

    public bool treatsMechanoids;

    public CompProperties_LTF_MedBay()
    {
        compClass = typeof(Comp_LTF_MedBay);
    }
}