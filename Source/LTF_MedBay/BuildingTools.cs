using RimWorld;
using UnityEngine;
using Verse;

namespace LTF_MedBay;

public static class BuildingTools
{
    public static bool CheckBuilding(this Building building)
    {
        int num;
        if (building is { Map: not null })
        {
            _ = building.Position;
            num = 1;
        }
        else
        {
            num = 0;
        }

        return (byte)num != 0;
    }

    public static bool CheckPower(this CompPowerTrader powerComp)
    {
        return powerComp != null;
    }

    public static CompPowerTrader SetPower(this Building bench, bool debug = false)
    {
        if (bench == null)
        {
            Tools.Warn("No bench provided to retrieve comp", debug);
            return null;
        }

        var compPowerTrader = bench.TryGetComp<CompPowerTrader>();
        if (compPowerTrader == null)
        {
            Tools.Warn("No CompPowerTrader found", debug);
        }

        return compPowerTrader;
    }

    public static bool CheckQuality(this CompQuality QComp)
    {
        return QComp != null;
    }

    public static CompQuality SetQuality(this Building bench, bool debug = false)
    {
        if (bench == null)
        {
            Tools.Warn("No bench provided to retrieve comp", debug);
            return null;
        }

        var compQuality = bench.TryGetComp<CompQuality>();
        if (compQuality == null)
        {
            Tools.Warn("No comp found", debug);
        }

        return compQuality;
    }

    public static bool CheckRefuelable(this CompRefuelable RComp)
    {
        return RComp != null;
    }

    public static CompRefuelable SetRefuelable(this Building bench, bool debug = false)
    {
        if (bench == null)
        {
            Tools.Warn("No bench provided to retrieve comp", debug);
            return null;
        }

        var compRefuelable = bench.TryGetComp<CompRefuelable>();
        if (compRefuelable == null)
        {
            Tools.Warn("No CompRefuelable found", debug);
        }

        return compRefuelable;
    }

    public static Color StuffColor(Building building, bool debug = false)
    {
        var color = default(Color);
        ThingDef stuff;
        if ((stuff = building.Stuff) != null)
        {
            color = stuff.stuffProps.color;
        }

        var warning = stuff == null ? $"found no color for {building.Label}" : $"Found {color}";

        Tools.Warn(warning, debug);
        return color;
    }

    public static bool IsWall(this Building building)
    {
        return building.def.defName.Contains("Wall");
    }

    public static bool IsVent(this Building building)
    {
        return building.def == MyDefs.VentBuildingDef;
    }

    public static bool IsCooler(this Building building)
    {
        return building.def == MyDefs.CoolerBuildingDef;
    }

    public static bool IsPlaceboWall(this Building building)
    {
        return building.IsWall() && building.IsPlacebo();
    }

    public static bool IsPlaceboDoor(this Building building)
    {
        return building.def.IsDoor && building.IsPlacebo();
    }

    public static bool IsPlacebo(this Building building)
    {
        return building.def.MadeFromStuff && building.Stuff == MyDefs.MediStoneStuffDef;
    }

    public static float AgregatedScore(Room room)
    {
        return room.GetStat(RoomStatDefOf.Impressiveness) + room.GetStat(RoomStatDefOf.Wealth) +
               room.GetStat(RoomStatDefOf.Space) + room.GetStat(RoomStatDefOf.Beauty) +
               room.GetStat(RoomStatDefOf.Cleanliness);
    }
}