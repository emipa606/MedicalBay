using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace LTF_MedBay;

public static class RoomCleaning
{
    public static void CleanRoom(Room room, ref List<FilthWork> FilthList, List<IntVec3> DrainingTilesPosList,
        Comp_LTF_FilthCompressor ParentComp, bool debug = false)
    {
        if (FilthList.NullOrEmpty())
        {
            Tools.Warn("room is clean", debug);
            return;
        }

        if (DrainingTilesPosList.NullOrEmpty())
        {
            Tools.Warn("room has no draining tile", debug);
            return;
        }

        var count = DrainingTilesPosList.Count;
        var list = new List<FilthWork>();
        var filthWork = FilthList.First();
        var curF = filthWork.Filth;
        if (curF == null || curF.Map == null || curF.IsReserved())
        {
            return;
        }

        filthWork.Tick(count);
        var val = Vector3.Distance(
            DrainingTilesPosList.Aggregate((x, y) =>
                !(Vector3.Distance(curF.DrawPos, x.ToVector3()) < Vector3.Distance(curF.DrawPos, y.ToVector3()))
                    ? y
                    : x).ToVector3(), curF.Position.ToVector3());
        val = Math.Max(1f, val);
        if (ParentComp.Props.isFuelPowered)
        {
            ParentComp.refuelableComp.ConsumeFuel(val * count / ParentComp.Props.FuelConsumptionDivider);
        }

        if (filthWork.IsOver || curF.Destroyed)
        {
            Tools.Warn("destroying 1 filth at a time", debug);
            FleckMaker.ThrowAirPuffUp(ParentComp.parent.DrawPos, ParentComp.parent.Map);
            FleckMaker.ThrowAirPuffUp(curF.DrawPos, ParentComp.parent.Map);
            list.Add(filthWork);
            filthWork.Init();
        }

        if (list.NullOrEmpty())
        {
            return;
        }

        var filth = filthWork.Filth;
        if (!filth.Destroyed)
        {
            ((Filth)filth).ThinFilth();
        }

        if (filth.Destroyed)
        {
            FilthList.Remove(filthWork);
        }
    }

    public static void PopulateActors(Room room, List<ThingDef> aimedFilth, out List<FilthWork> FilthList,
        out List<IntVec3> DrainingTilesPosList, bool debug = false)
    {
        FilthList = [];
        DrainingTilesPosList = [];
        int num2;
        int num;
        var num3 = num2 = num = 0;
        foreach (var cell in room.Cells)
        {
            if (cell.GetTerrain(room.Map) == MyDefs.DrainTileDef)
            {
                DrainingTilesPosList.Add(cell);
                if (debug)
                {
                    num++;
                }
            }

            if (debug)
            {
                num3++;
            }

            var thingList = cell.GetThingList(room.Map);
            if (thingList.NullOrEmpty())
            {
                continue;
            }

            foreach (var item in thingList)
            {
                if (!aimedFilth.Contains(item.def))
                {
                    continue;
                }

                FilthList.Add(new FilthWork(item));
                if (debug)
                {
                    num2++;
                }
            }
        }

        Tools.Warn($"cells: {num3}; filth: {num2} drainTile: {num}", debug);
    }
}