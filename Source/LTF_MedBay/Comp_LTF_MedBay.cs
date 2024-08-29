using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace LTF_MedBay;

[StaticConstructorOnStartup]
public class Comp_LTF_MedBay : ThingComp
{
    public bool AutomaticRegen = true;

    public bool AutomaticTending = true;

    public Building bench;

    public MedBayParameters.TargetFaction FactionParams = MedBayParameters.TargetFaction.Regular;

    public MedBayParameters.TargetGenre GenreParams = MedBayParameters.TargetGenre.Everybody;

    public bool gfxDebug;

    public bool HideAutoSection;

    public bool HideFactionSection;

    public bool HideRaceSection;

    private bool Initialized;

    public WaitingRoom MyWaitingRoom;

    public CompPowerTrader powerComp;

    public bool processDebug;

    public CompQuality qualityComp;

    public CompRefuelable refuelableComp;

    private IEnumerable<Pawn> PotentialPatients => MyWaitingRoom?.TendablePawns.Concat(MyWaitingRoom?.RegenerablePawns);

    private int PatientNum => MyWaitingRoom.PotentialPatientCount;

    private HealingManager MyHealingManager => MyWaitingRoom.MyHealingManager;

    private List<TendingRequest> tendingRequests => MyHealingManager.tendingRequests;

    private List<RegenerationRequest> regenRequests => MyHealingManager.regenRequests;

    public CompProperties_LTF_MedBay Props => (CompProperties_LTF_MedBay)props;

    public bool CheckMedBay
    {
        get
        {
            if (!bench.CheckBuilding())
            {
                Tools.Warn("CheckMedBay Null bench ", processDebug);
                return false;
            }

            if (!powerComp.CheckPower())
            {
                Tools.Warn("CheckMedBay no powerComp ", processDebug);
                return false;
            }

            if (!qualityComp.CheckQuality())
            {
                Tools.Warn("CheckMedBay no qualityComp ", processDebug);
                return false;
            }

            if (!refuelableComp.CheckRefuelable())
            {
                Tools.Warn("CheckMedBay no refuelableComp ", processDebug);
                return false;
            }

            if (bench.GetRoom(RegionType.Normal | RegionType.Portal) != null)
            {
                return true;
            }

            Tools.Warn("bench.GetRoom == null", processDebug);
            return false;
        }
    }

    public string QualityLog
    {
        get
        {
            var result = string.Empty;
            if (qualityComp.CheckQuality())
            {
                result = $"\nQuality:{qualityComp.Quality.GetLabel()}";
            }

            return result;
        }
    }

    public bool ValidRoom
    {
        get
        {
            var room = bench.GetRoom(RegionType.Normal | RegionType.Portal);
            if (room is { Dereferenced: false, OpenRoofCount: 0 })
            {
                return !room.PsychologicallyOutdoors;
            }

            return false;
        }
    }

    public bool InvalidRoom => !ValidRoom;

    public bool HasHealingRequest => MyHealingManager.HasRequest;

    public bool HasTendingRequest => MyHealingManager.HasTendingRequest;

    public bool HasRegenerationRequest => MyHealingManager.HasRegenerationRequest;

    public bool HasPowerOn => powerComp is { PowerOn: true };

    public void MyQualityInit()
    {
        if ((qualityComp = bench.SetQuality(processDebug)) == null)
        {
            Tools.Warn("Cant find quality", processDebug);
        }
    }

    public void MyPowerInit()
    {
        if ((powerComp = bench.SetPower(processDebug)) == null)
        {
            Tools.Warn("Cant find power", processDebug);
        }
    }

    public void MyRefuelableInit()
    {
        if ((refuelableComp = bench.SetRefuelable(processDebug)) == null)
        {
            Tools.Warn("Cant find refuelable", processDebug);
        }
    }

    private void MedBayInit()
    {
        processDebug = Props.debug;
        Tools.Warn("Entering MedBayInit", processDebug);
        bench = (Building)parent;
        MyPowerInit();
        MyQualityInit();
        MyRefuelableInit();
        Tools.Warn("Comp MedBayInit", processDebug);
        QualityWeightedBenchFactors();
        if (!bench.Map.regionAndRoomUpdater.Enabled)
        {
            return;
        }

        if (!CheckMedBay)
        {
            Tools.Warn("MedBayInit : Bad medbay", processDebug);
            return;
        }

        Tools.Warn("new WaitingRoom", processDebug);
        MyWaitingRoom = new WaitingRoom(this);
        Tools.Warn("UpdateRoomValues", processDebug);
        MyWaitingRoom.UpdateRoomValues(this);
        Tools.Warn("checking Props.regenerableHediffDef", processDebug);
        if (Props.regenerableHediffDef.NullOrEmpty())
        {
            Tools.Warn("MedBayInit : Props.regenerableHediffDef empty", processDebug);
            return;
        }

        Initialized = true;
        Tools.Warn("MedBayInit success", processDebug);
    }

    private void UpdateRoom()
    {
        MyWaitingRoom.UpdateParams(bench.GetRoom(RegionType.Normal | RegionType.Portal), bench.Faction, GenreParams,
            FactionParams);
    }

    private void QualityWeightedBenchFactors()
    {
        if (qualityComp == null)
        {
            Tools.Warn("sad bench has no quality", processDebug);
        }
    }

    public void ResetHealingRequest()
    {
        MyHealingManager.ResetAll();
    }

    public override void PostDraw()
    {
        base.PostDraw();
        var drawPos = bench.DrawPos;
        drawPos.y += 3f / 64f;
        if (!Initialized)
        {
            Tools.Warn("PostDraw failed CheckMedBay", gfxDebug);
            return;
        }

        if (!HasPowerOn)
        {
            Tools.Warn("no power", gfxDebug);
            return;
        }

        var hasHealingRequest = HasHealingRequest;
        GfxTools.DrawBenchSize(drawPos, hasHealingRequest ? Gfx.MedBayScreenActive : Gfx.MedBayScreenStandBy,
            Gfx.WorkMesh, bench);
        if (!HasHealingRequest)
        {
            return;
        }

        if (HasRegenerationRequest)
        {
            var firstRegenRequest = MyWaitingRoom.MyHealingManager.GetFirstRegenRequest();
            if (firstRegenRequest != null)
            {
                var num = Mathf.RoundToInt(firstRegenRequest.WorkRequest.ProgressRatio * Gfx.WorkBarNum);
                for (var i = 1; i < num + 1; i++)
                {
                    GfxTools.DrawBarResized(drawPos, Gfx.WorkMesh, i);
                }
            }
        }

        if (HasTendingRequest)
        {
            foreach (var tendingRequest in tendingRequests)
            {
                if (tendingRequest.WorkRequest.HasProgress)
                {
                    GfxTools.TendingFillBar(tendingRequest);
                }
            }
        }

        var drawPos2 = parent.DrawPos;
        Gfx.EcgAnimGfx.Draw(drawPos2, Rot4.North, parent);
    }

    public override void PostSpawnSetup(bool respawningAfterLoad)
    {
        MedBayInit();
    }

    public override void CompTick()
    {
        if (!Initialized)
        {
            MedBayInit();
            Tools.Warn("Impossibru", processDebug);
            return;
        }

        if (!HasPowerOn)
        {
            Tools.Warn("Power Off, wont do chips", processDebug);
            if (HasHealingRequest)
            {
                ResetHealingRequest();
            }

            return;
        }

        if (Tools.TrueEvery5Sec)
        {
            if (MyWaitingRoom.InvalidRoom && ValidRoom)
            {
                UpdateRoom();
            }

            if (MyWaitingRoom.ValidRoom && MyWaitingRoom.DidRoomchange())
            {
                MyWaitingRoom.UpdateRoomValues(this);
            }
        }
        else if (Tools.TrueEverySec && MyWaitingRoom.ValidRoom)
        {
            MyWaitingRoom.UpdateRoomPawns(this);
        }

        if (!refuelableComp.HasFuel)
        {
            if (HasHealingRequest)
            {
                MyWaitingRoom.MyHealingManager.ResetAll();
            }
        }
        else if (HasHealingRequest)
        {
            MyWaitingRoom.MyHealingManager.Tick(processDebug);
        }
    }

    public override void PostExposeData()
    {
        base.PostExposeData();
        Scribe_Values.Look(ref GenreParams, "LTF_MedBay_GenreParams");
        Scribe_Values.Look(ref FactionParams, "LTF_MedBay_FactionParams");
        Scribe_Values.Look(ref AutomaticTending, "LTF_MedBay_AutomaticTending");
        Scribe_Values.Look(ref AutomaticRegen, "LTF_MedBay_AutomaticRegen");
        Scribe_Values.Look(ref HideAutoSection, "LTF_MedBay_HideAutoSection");
        Scribe_Values.Look(ref HideFactionSection, "LTF_MedBay_HideFactionSection");
        Scribe_Values.Look(ref HideRaceSection, "LTF_MedBay_HideRaceSection");
    }

    public override string CompInspectStringExtra()
    {
        var text = base.CompInspectStringExtra();
        var empty = string.Empty;
        if (!powerComp.CheckPower())
        {
            return text;
        }

        return text + empty;
    }

    [DebuggerHidden]
    public override IEnumerable<Gizmo> CompGetGizmosExtra()
    {
        foreach (var item in MedBayGizmo.MainGizmo(this))
        {
            yield return item;
        }
    }

    public override void PostDrawExtraSelectionOverlays()
    {
        if (!HasHealingRequest)
        {
            return;
        }

        if (HasTendingRequest)
        {
            foreach (var tendingRequest in tendingRequests)
            {
                if (tendingRequest.WorkRequest.HasTarget && tendingRequest.WorkRequest.HasProgress)
                {
                    GenDraw.DrawLineBetween(parent.TrueCenter(), tendingRequest.WorkRequest.Patient.TrueCenter(),
                        SimpleColor.Cyan);
                }
            }
        }

        if (!HasRegenerationRequest)
        {
            return;
        }

        foreach (var regenRequest in regenRequests)
        {
            if (regenRequest.WorkRequest.HasTarget && regenRequest.WorkRequest.HasProgress)
            {
                GenDraw.DrawLineBetween(parent.TrueCenter(), regenRequest.WorkRequest.Patient.TrueCenter(),
                    SimpleColor.Green);
            }
        }
    }
}