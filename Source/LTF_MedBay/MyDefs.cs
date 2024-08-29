using System.Collections.Generic;
using System.Linq;
using Verse;

namespace LTF_MedBay;

public static class MyDefs
{
    public static string PlaceboWallAdjective = "Therapeutic";

    public static readonly ThingDef MediStoneStuffDef =
        DefDatabase<ThingDef>.AllDefs.First(b => b.defName == "LTF_BlocksMediStone");

    public static readonly ThingDef VentBuildingDef = DefDatabase<ThingDef>.AllDefs.First(b => b.defName == "Vent");

    public static readonly ThingDef CoolerBuildingDef = DefDatabase<ThingDef>.AllDefs.First(b => b.defName == "Cooler");

    public static readonly TerrainDef DrainTileDef =
        DefDatabase<TerrainDef>.AllDefs.First(b => b.defName == "LTF_DrainFloor");

    public static readonly string medfuelName = ThingDef.Named("LTF_MediFuel").label;

    public static readonly List<string> MedBayDefName = ["LTF_MedBay", "LTF_T2MedBay", "LTF_T3MedBay"];

    public static readonly SoundDef regenInitSound =
        DefDatabase<SoundDef>.AllDefs.First(b => b.defName == "LTF_Medbay_RegenInit");

    public static readonly SoundDef regenLoopSound =
        DefDatabase<SoundDef>.AllDefs.First(b => b.defName == "LTF_Medbay_RegenLoop");

    public static readonly SoundDef
        TendStartSound = DefDatabase<SoundDef>.AllDefs.First(b => b.defName == "Tend_Start");
}