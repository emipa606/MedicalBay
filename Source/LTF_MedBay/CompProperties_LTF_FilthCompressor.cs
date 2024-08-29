using System.Collections.Generic;
using Verse;

namespace LTF_MedBay;

public class CompProperties_LTF_FilthCompressor : CompProperties
{
    public readonly List<string> aimedFilth = [];
    public readonly List<ThingDef> aimedFilthDefs = [];

    public readonly float FuelConsumptionDivider = 32f;

    public bool debug;

    public bool isElectricityPowered;

    public bool isFuelPowered;

    public CompProperties_LTF_FilthCompressor()
    {
        compClass = typeof(Comp_LTF_FilthCompressor);
    }
}