using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace LTF_MedBay;

[StaticConstructorOnStartup]
public class Comp_LTF_FilthCompressor : ThingComp
{
    private Building building;

    private List<IntVec3> DrainingTilesPosList;

    private List<FilthWork> FilthList;

    public bool gfxDebug;

    private bool Initialized;

    private float lastRoomScore;

    private Room MyRoom;

    private CompPowerTrader powerComp;

    private bool processDebug;

    public CompRefuelable refuelableComp;

    public CompProperties_LTF_FilthCompressor Props => (CompProperties_LTF_FilthCompressor)props;

    private bool CheckCompressor
    {
        get
        {
            if (!building.CheckBuilding())
            {
                Tools.Warn("CheckMedBay Null bench ", processDebug);
                return false;
            }

            if (Props.isFuelPowered && !refuelableComp.CheckRefuelable())
            {
                Tools.Warn("CheckMedBay no refuelableComp ", processDebug);
                return false;
            }

            if (Props.isElectricityPowered && !powerComp.CheckPower())
            {
                Tools.Warn("CheckMedBay no powerComp ", processDebug);
                return false;
            }

            if (ValidRoom)
            {
                return true;
            }

            Tools.Warn("bench.GetRoom == null", processDebug);
            return false;
        }
    }

    private bool ValidRoom
    {
        get
        {
            if (MyRoom is { Dereferenced: false } myRoom)
            {
                return !myRoom.PsychologicallyOutdoors;
            }

            return false;
        }
    }

    private bool InvalidRoom => !ValidRoom;

    private bool HasPowerOn => powerComp is { PowerOn: true };

    private bool IsPowered
    {
        get
        {
            if (Props.isElectricityPowered && !HasPowerOn)
            {
                return true;
            }

            if (Props.isFuelPowered)
            {
                return !refuelableComp.HasFuel;
            }

            return false;
        }
    }

    private bool HasFilth => !FilthList.NullOrEmpty();

    private bool HasDrainTiles => !DrainingTilesPosList.NullOrEmpty();

    private bool HasWhatItTakes => HasFilth && HasDrainTiles;

    private void RefuelableInit()
    {
        if ((refuelableComp = building.SetRefuelable(processDebug)) == null)
        {
            Tools.Warn("Cant find refuelable", processDebug);
        }
    }

    private void PowerInit()
    {
        if ((powerComp = building.SetPower(processDebug)) == null)
        {
            Tools.Warn("Cant find power", processDebug);
        }
    }

    private void CompressorInit()
    {
        processDebug = Props.debug;
        if (!Props.aimedFilthDefs.Any())
        {
            foreach (var filthDefName in Props.aimedFilth)
            {
                if (filthDefName.Contains("*"))
                {
                    Props.aimedFilthDefs.AddRange(DefDatabase<ThingDef>.AllDefsListForReading.Where(def =>
                        def.defName.Contains(filthDefName.Replace("*", ""))));
                    continue;
                }

                var filthToAdd = DefDatabase<ThingDef>.GetNamedSilentFail(filthDefName);
                if (filthToAdd != null)
                {
                    Props.aimedFilthDefs.Add(filthToAdd);
                }
            }

            Tools.Warn(
                $"Will clean {Props.aimedFilthDefs.Count} filths: {string.Join("\n", Props.aimedFilthDefs.Select(def => def.LabelCap))}",
                processDebug);
        }

        Tools.Warn("Entering compressor Init", processDebug);
        building = (Building)parent;
        PowerInit();
        RefuelableInit();
        if (!building.Map.regionAndRoomUpdater.Enabled)
        {
            return;
        }

        MyRoom = building.GetRoom(RegionType.Normal | RegionType.Portal);
        Tools.Warn("Comp compressorInit", processDebug);
        if (!CheckCompressor)
        {
            Tools.Warn("compressorInit : Bad compressor", processDebug);
            return;
        }

        BuildingTools.AgregatedScore(MyRoom);
        Initialized = true;
        Tools.Warn("compressorInit success", processDebug);
    }

    public override void PostSpawnSetup(bool respawningAfterLoad)
    {
        CompressorInit();
    }

    public override void CompTick()
    {
        if (!Initialized)
        {
            CompressorInit();
            Tools.Warn("Impossibru", processDebug);
            return;
        }

        if (IsPowered)
        {
            Tools.Warn("Power Off, wont do chips", processDebug);
            return;
        }

        if (Tools.TrueEvery5Sec)
        {
            if (InvalidRoom)
            {
                MyRoom = building.GetRoom(RegionType.Normal | RegionType.Portal);
            }

            var num = BuildingTools.AgregatedScore(MyRoom);
            if (num != lastRoomScore)
            {
                RoomCleaning.PopulateActors(MyRoom, Props.aimedFilthDefs, out FilthList, out DrainingTilesPosList,
                    processDebug);
                lastRoomScore = num;
            }
        }

        if (!InvalidRoom && Tools.TrueEvery3Sec && HasWhatItTakes)
        {
            RoomCleaning.CleanRoom(MyRoom, ref FilthList, DrainingTilesPosList, this, processDebug);
        }
    }
}