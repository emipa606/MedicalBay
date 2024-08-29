using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace LTF_MedBay;

public class WaitingRoom
{
    public readonly HealingManager MyHealingManager;

    public readonly Comp_LTF_MedBay ParentComp;

    public readonly ICollection<Pawn> RoomPawns = new List<Pawn>();
    public float calculatedMaxRegen;

    public FloatRange calculatedTendingQualityRange = new FloatRange(0f, 0f);

    private int DrainingTilesNum;

    public MedBayParameters.TargetFaction FactionParams;

    public MedBayParameters.TargetGenre GenreParams;

    private float LastScore;

    private int NonPlaceboWallsNum;

    private int PlaceboWallsNum;

    public Room room;

    private int WallsNum;

    public WaitingRoom(Comp_LTF_MedBay initiator)
    {
        ParentComp = initiator;
        Tools.Warn("new Healing manager", debug);
        room = initiator.bench.GetRoom(RegionType.Normal | RegionType.Portal);
        GenreParams = initiator.GenreParams;
        FactionParams = initiator.FactionParams;
        MyHealingManager = new HealingManager(this);
    }

    private List<TendingRequest> tendingRequests => MyHealingManager?.tendingRequests;

    private List<RegenerationRequest> regenRequests => MyHealingManager?.regenRequests;

    private int TRCount => MyHealingManager.TRCount;

    private int RRCount => MyHealingManager.RRCount;

    private bool debug => ParentComp.processDebug;

    public float CalculateMaxRegen => ParentComp.Props.regenMax + MaxRegenMedbayQualityFactor + MaxRegenPlaceboFactor;

    public float MaxRegenMedbayQualityFactor =>
        ParentComp.Props.regenMaxQualityOffset * Tools.QualityFactor(ParentComp.qualityComp.Quality);

    public float MaxRegenPlaceboFactor => ParentComp.Props.regenMaxPlaceboOffset * PlaceboWallsRatio;

    public FloatRange CalculateTendingQualityRange
    {
        get
        {
            var tendingQualityRange = ParentComp.Props.tendingQualityRange;
            var calculateTendingQualityMedbayQualityFactor = CalculateTendingQualityMedbayQualityFactor;
            tendingQualityRange.min += calculateTendingQualityMedbayQualityFactor;
            tendingQualityRange.max += calculateTendingQualityMedbayQualityFactor;
            return tendingQualityRange;
        }
    }

    public float CalculateTendingQualityMedbayQualityFactor => Tools.QualityFactor(ParentComp.qualityComp.Quality) *
                                                               ParentComp.Props.tendingQualityQualityOffset;

    public FloatRange TheoricalTendingQualityRange
    {
        get
        {
            var calculateTendingQualityRange = CalculateTendingQualityRange;
            calculateTendingQualityRange.min += ParentComp.Props.tendingQualityPlaceboOffset * PlaceboWallsRatio;
            calculateTendingQualityRange.max += ParentComp.Props.tendingQualityPlaceboOffset * PlaceboWallsRatio;
            calculateTendingQualityRange.min = (float)Math.Round(calculateTendingQualityRange.min, 2);
            calculateTendingQualityRange.max = (float)Math.Round(calculateTendingQualityRange.max, 2);
            return calculateTendingQualityRange;
        }
    }

    public float PlaceboWallsRatio
    {
        get
        {
            if (InvalidRoom)
            {
                return 0f;
            }

            if (WallsNum == 0)
            {
                return 1f;
            }

            return PlaceboWallsNum / (float)WallsNum;
        }
    }

    public bool ValidRoom
    {
        get
        {
            var roomToValidate = room;
            if (roomToValidate is { Dereferenced: false, OpenRoofCount: 0 })
            {
                return !roomToValidate.PsychologicallyOutdoors;
            }

            return false;
        }
    }

    public bool InvalidRoom => !ValidRoom;

    public int RoomPawnsCount => RoomPawns.EnumerableNullOrEmpty() ? 0 : RoomPawns.Count;

    public int PotentialPatientCount
    {
        get
        {
            if (RoomPawns.EnumerableNullOrEmpty())
            {
                return 0;
            }

            if (TendablePawns.EnumerableNullOrEmpty() && RegenerablePawns.EnumerableNullOrEmpty())
            {
                return 0;
            }

            return PotentialPatients.Count();
        }
    }

    public IEnumerable<Pawn> TendablePawns
    {
        get
        {
            return RoomPawns.EnumerableNullOrEmpty() ? null : RoomPawns.Where(p => p.health.HasHediffsNeedingTend());
        }
    }

    public IEnumerable<Pawn> RegenerablePawns
    {
        get
        {
            if (RoomPawns.EnumerableNullOrEmpty())
            {
                return null;
            }

            return RoomPawns.Where(p =>
                p.health.summaryHealth.SummaryHealthPercent < 0.99f &&
                p.IsRegenEligible(calculatedMaxRegen, ParentComp.Props.regeneratesBionics));
        }
    }

    public IEnumerable<Pawn> PotentialPatients => TendablePawns.Union(RegenerablePawns);

    private bool HasHealingRequest => MyHealingManager.HasRequest;

    private bool HasTendingRequest => MyHealingManager.HasTendingRequest;

    private bool HasRegenerationRequest => MyHealingManager.HasRegenerationRequest;

    public void UpdateParams(Room newRoom, Faction newFaction, MedBayParameters.TargetGenre gParams,
        MedBayParameters.TargetFaction fParams)
    {
        room = newRoom;
        GenreParams = gParams;
        FactionParams = fParams;
    }

    public void UpdateRoomValues(Comp_LTF_MedBay compMB)
    {
        if (!compMB.bench.CheckBuilding())
        {
            return;
        }

        if (!UpdateRoomPawns(compMB))
        {
            NoRoomStats();
            return;
        }

        UpdateRoomComposition(compMB);
        LastScore = BuildingTools.AgregatedScore(room);
        calculatedMaxRegen = CalculateMaxRegen;
        calculatedTendingQualityRange = CalculateTendingQualityRange;
    }

    public void NoRoomStats()
    {
        DrainingTilesNum = PlaceboWallsNum = NonPlaceboWallsNum = 0;
        LastScore = 0f;
    }

    public bool DidRoomchange()
    {
        return LastScore != BuildingTools.AgregatedScore(room);
    }

    public void DeleteGonePawns(bool localDebug = false)
    {
        var list = new List<Pawn>();
        if (!RoomPawns.EnumerableNullOrEmpty())
        {
            foreach (var roomPawn in RoomPawns)
            {
                var roomToValidate = roomPawn.GetRoom(RegionType.Normal | RegionType.Portal);
                if (roomToValidate != null && roomToValidate == room)
                {
                    continue;
                }

                Tools.Warn($"UpdateRoomPawns Adding {roomPawn.LabelShort} to delete list", localDebug);
                list.Add(roomPawn);
            }
        }

        if (list.EnumerableNullOrEmpty())
        {
            return;
        }

        foreach (var item in list)
        {
            Tools.Warn($"UpdateRoomPawns Deleting {item.LabelShort}", localDebug);
            RoomPawns.Remove(item);
            if (HasTendingRequest)
            {
                MyHealingManager.DeletePawnTendingRequest(item);
            }

            if (HasRegenerationRequest)
            {
                MyHealingManager.DeletePawnRegenRequest(item);
            }
        }
    }

    private void TryTendingRequest(Pawn pawn, bool localDebug = false)
    {
        if (!pawn.HasTendableInjury())
        {
            Tools.Warn($"{pawn.LabelShort} has NO tendable injury", localDebug);
            return;
        }

        if (MyHealingManager.AlreadyHasTendingRequest(pawn))
        {
            Tools.Warn(
                $"{pawn.LabelShort} tried to get another tending request - should not happen bc waiting room should sort it out",
                localDebug);
            return;
        }

        if (pawn.IsReserved(localDebug))
        {
            Tools.Warn($"{pawn.LabelShort} HasTendableInjury but reserved, skipping", localDebug);
            return;
        }

        Tools.Warn($"{pawn.LabelShort} HasTendableInjury", localDebug);
        var item = new TendingRequest(pawn, MyHealingManager);
        tendingRequests.Add(item);
        Tools.Warn($"TR count: {TRCount}", localDebug);
    }

    private void TryRegenRequest(Pawn pawn, bool localDebug = false)
    {
        if (!pawn.IsRegenEligible(calculatedMaxRegen, ParentComp.Props.regeneratesBionics, localDebug))
        {
            Tools.Warn($"{pawn.LabelShort} Is NOT RegenEligible", localDebug);
            return;
        }

        if (MyHealingManager.AlreadyHasRegenRequest(pawn))
        {
            Tools.Warn(
                $"{pawn.LabelShort} tried to get another regen request - should not happen bc waiting room should sort it out",
                localDebug);
            return;
        }

        if (pawn.IsReserved(localDebug))
        {
            Tools.Warn($"{pawn.LabelShort} IsRegenEligible but reserved, skipping", localDebug);
            return;
        }

        Tools.Warn($"{pawn.LabelShort} IsRegenEligible", localDebug);
        var item = new RegenerationRequest(pawn, MyHealingManager);
        regenRequests.Add(item);
        Tools.Warn($"RR count: {RRCount}", localDebug);
    }

    private void TryAddingRequest(Pawn pawn, bool localDebug = false)
    {
        if (pawn.IsReserved())
        {
            Tools.Warn($"{pawn.LabelShort} is reserved, skipping request tries", localDebug);
            return;
        }

        if (TendablePawns.Contains(pawn))
        {
            TryTendingRequest(pawn, localDebug);
        }

        if (RegenerablePawns.Contains(pawn))
        {
            TryRegenRequest(pawn, localDebug);
        }
    }

    public void AddNewPawns(bool localDebug = false)
    {
        foreach (var cell in room.Cells)
        {
            foreach (var thing in cell.GetThingList(room.Map))
            {
                Pawn curPawn;
                if ((curPawn = thing as Pawn) == null ||
                    !RoomPawns.EnumerableNullOrEmpty() && RoomPawns.Any(x => x == curPawn) ||
                    !GenreParams.IsGenreParametersCompatible(curPawn, localDebug) ||
                    !FactionParams.IsFactionParametersCompatible(curPawn.Faction, localDebug))
                {
                    continue;
                }

                Tools.Warn($"UpdateRoomPawns adding {curPawn.LabelShort} to waiting room", localDebug);
                RoomPawns.Add(curPawn);
                TryAddingRequest(curPawn, localDebug);
            }
        }
    }

    public bool UpdateRoomPawns(Comp_LTF_MedBay compMB)
    {
        var processDebug = compMB.processDebug;
        var validRoom = false;
        if (InvalidRoom)
        {
            if (!RoomPawns.EnumerableNullOrEmpty())
            {
                RoomPawns.Clear();
            }
        }
        else
        {
            validRoom = true;
        }

        DeleteGonePawns(processDebug);
        if (!validRoom)
        {
            return false;
        }

        AddNewPawns(processDebug);
        if (PotentialPatientCount <= 0)
        {
            return true;
        }

        foreach (var potentialPatient in PotentialPatients)
        {
            TryAddingRequest(potentialPatient, processDebug);
        }

        return true;
    }

    public void UpdateRoomComposition(Comp_LTF_MedBay compMB)
    {
        var processDebug = compMB.processDebug;
        Tools.Warn(">>> Entering UpdateRoomComposition", processDebug);
        PlaceboWallsNum = NonPlaceboWallsNum = 0;
        DrainingTilesNum = 0;
        if (InvalidRoom)
        {
            Tools.Warn("Cant UpdateRoomComposition bc room invalid", processDebug);
            return;
        }

        var cells = room.Cells;
        var list = new List<IntVec3>();
        foreach (var cell in room.Cells)
        {
            for (var i = 0; i < 8; i++)
            {
                var intVec = cell + GenAdj.AdjacentCells[i];
                if (!cells.Contains(intVec) && !list.Contains(intVec))
                {
                    list.Add(intVec);
                }
            }
        }

        WallsNum = list.Count;
        foreach (var item in cells.Concat(list))
        {
            foreach (var thing in item.GetThingList(compMB.bench.Map))
            {
                if (thing is not Building building)
                {
                    continue;
                }

                if (building.IsPlaceboWall() || building.IsPlaceboDoor())
                {
                    PlaceboWallsNum++;
                }
                else if (building.IsWall() || building.def.IsDoor || building.IsVent() || building.IsCooler())
                {
                    NonPlaceboWallsNum++;
                }
            }

            if (cells.Contains(item) && item.GetTerrain(compMB.bench.Map) == MyDefs.DrainTileDef)
            {
                DrainingTilesNum++;
            }
        }

        Tools.Warn(">>> Exiting UpdateRoomComposition", processDebug);
    }

    public string Report()
    {
        var empty = string.Empty;
        var roomPawnsCount = RoomPawnsCount;
        var potentialPatientCount = PotentialPatientCount;
        empty += "| Med bay statistics |\n";
        empty += "+---------------------+\n";
        empty = $"{empty}{roomPawnsCount} pawn{(roomPawnsCount > 1 ? "s" : "")} in the room";
        empty = potentialPatientCount <= 0
            ? empty + ".\n"
            : empty + " of which " + potentialPatientCount + (potentialPatientCount > 1 ? " are" : " is") + " hurt.\n";
        if (!RoomPawns.EnumerableNullOrEmpty())
        {
            foreach (var roomPawn in RoomPawns)
            {
                empty = $"{empty}- {roomPawn.PawnResumeString()}";
                if (potentialPatientCount == 0)
                {
                    empty += " - healthy";
                }
                else
                {
                    var tendable = true;
                    if (!TendablePawns.EnumerableNullOrEmpty())
                    {
                        if (MyHealingManager.AlreadyHasTendingRequest(roomPawn))
                        {
                            empty += " in medbay tending queue;";
                            tendable = false;
                        }
                        else if (TendablePawns.Contains(roomPawn))
                        {
                            empty += " tendable;";
                            tendable = false;
                        }
                    }

                    if (!RegenerablePawns.EnumerableNullOrEmpty())
                    {
                        if (MyHealingManager.AlreadyHasRegenRequest(roomPawn))
                        {
                            empty += " in medbay regen queue;";
                            tendable = false;
                        }
                        else if (RegenerablePawns.Contains(roomPawn))
                        {
                            empty += " regenerable;";
                            tendable = false;
                        }
                    }

                    if (tendable)
                    {
                        empty += " healthy;";
                    }

                    empty += roomPawn.IsReserved() ? "(reserved)" : "";
                }

                empty += ".\n";
            }
        }

        if (ValidRoom)
        {
            if (DebugSettings.godMode)
            {
                empty =
                    $"{empty}\n\nRoom stats:\n--------------\nCells: {room.CellCount}\nDrainingTilesNum: {DrainingTilesNum}\n---------\nPlaceboWallsNum: {PlaceboWallsNum}\nNonPlaceboWallsNum: {NonPlaceboWallsNum}\nWallsnum: {WallsNum}\nPlaceboWallsRatio: {PlaceboWallsRatio}\n---------\nLastScore: {LastScore}";
            }

            empty =
                $"{empty}\n\nTending quality range:\n------------------------\nTending duality base: {ParentComp.Props.tendingQualityRange}\nMedbay quality factor: {CalculateTendingQualityMedbayQualityFactor:F}\nRoom placebo factor: {ParentComp.Props.tendingQualityPlaceboOffset * PlaceboWallsRatio:F}(weighted by patient psychic sensitivity)\nFinal tending quality range: {TheoricalTendingQualityRange}";
            empty =
                $"{empty}\n\nMax regeneration:\n--------------------\nBase regeneration: {ParentComp.Props.regenMax}\nMedbay quality factor: {MaxRegenMedbayQualityFactor:F}\nRoom placebo factor: {MaxRegenPlaceboFactor:F}\nFinal regeneration value: {calculatedMaxRegen:F}";
        }

        empty =
            $"{empty}\n\nConsumption:\n----------------\n{ParentComp.Props.fuelConsumptionPerTendingQuality} {MyDefs.medfuelName} units per tending quality\n{ParentComp.Props.fuelConsumptionPerRegenPoint} {MyDefs.medfuelName} units per regeneration point";
        return
            $"{empty}\n\nTreatments in queue:\n-------------------------\nTending treatments left: {TRCount}\nRegeneration treatments left: {RRCount}";
    }
}