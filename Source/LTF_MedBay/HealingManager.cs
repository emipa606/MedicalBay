using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.Sound;

namespace LTF_MedBay;

public class HealingManager
{
    public readonly WaitingRoom ParentWR;

    public readonly List<RegenerationRequest> regenRequests;

    public readonly List<TendingRequest> tendingRequests;
    public bool AutomaticRegen;

    public bool AutomaticTending;

    public bool ManualRegenStart;

    public bool ManualTendingStart;

    private Sustainer regenSustainer;

    public HealingManager(WaitingRoom initiator)
    {
        ParentWR = initiator;
        Tools.Warn("new Healing manager", debug);
        tendingRequests = [];
        regenRequests = [];
        AutomaticTending = initiator.ParentComp.AutomaticTending;
        AutomaticRegen = initiator.ParentComp.AutomaticRegen;
    }

    private float calculatedMaxRegen => ParentWR.calculatedMaxRegen;

    private bool debug => ParentWR.ParentComp.processDebug;

    public bool HasTendingRequest => !tendingRequests.NullOrEmpty();

    public bool HasRegenerationRequest => !regenRequests.NullOrEmpty();

    public bool HasRequest => HasTendingRequest || HasRegenerationRequest;

    public int TRCount => tendingRequests.NullOrEmpty() ? 0 : tendingRequests.Count;

    public int RRCount => regenRequests.NullOrEmpty() ? 0 : regenRequests.Count;

    public RegenerationRequest GetFirstRegenRequest()
    {
        return !HasRegenerationRequest ? null : regenRequests.First();
    }

    public void ResetAll()
    {
        tendingRequests.RemoveAll(h => h != null);
        regenRequests.RemoveAll(h => h != null);
    }

    public void RegenLoopTick(bool localDebug = false)
    {
        var list = new List<RegenerationRequest>();
        foreach (var regenRequest in regenRequests)
        {
            var patient = regenRequest.WorkRequest.Patient;
            if (patient.IsReserved(localDebug))
            {
                Tools.Warn($"{patient.LabelShort} RR added to 2delete by reservation", localDebug);
                list.Add(regenRequest);
                continue;
            }

            var list2 = regenRequest.BodyPartsRegenerationEligible(calculatedMaxRegen, localDebug);
            if (list2.NullOrEmpty())
            {
                Tools.Warn("Healing manager Tick : no bpr to work with", localDebug);
                list.Add(regenRequest);
                continue;
            }

            Tools.Warn($"{patient.LabelShort} has {list2.Count} regenerable BPR", localDebug);
            if (!regenRequest.WorkRequest.HasProgress)
            {
                ReduceWorkAmount(ref regenRequest.WorkRequest.WorkAmount, list2, regenRequest.WorkRequest.Patient,
                    calculatedMaxRegen, localDebug);
                MyDefs.regenInitSound.PlayOneShot(new TargetInfo(patient.Position, patient.Map));
            }
            else if (regenSustainer == null)
            {
                var info = SoundInfo.InMap(new TargetInfo(patient.Position, patient.Map), MaintenanceType.PerTick);
                regenSustainer = MyDefs.regenLoopSound.TrySpawnSustainer(info);
                regenSustainer.Maintain();
            }
            else if (!regenSustainer.Ended)
            {
                regenSustainer.Maintain();
            }
            else
            {
                Tools.Warn("Saw regen sustainer ending", localDebug);
                regenSustainer.End();
                regenSustainer = null;
            }

            foreach (var item in list2)
            {
                regenRequest.RegenerateBodyPartTick(item, regenRequest.WorkRequest.WorkAmount, calculatedMaxRegen,
                    localDebug);
            }

            regenRequest.WorkRequest.Tick();
            if (!regenRequest.WorkRequest.IsComplete)
            {
                continue;
            }

            MyDefs.regenInitSound.PlayOneShotOnCamera(patient.Map);
            Tools.Warn($"{patient.LabelShort} RR added to 2delete by no more work", localDebug);
            list.Add(regenRequest);
        }

        foreach (var item2 in list)
        {
            regenRequests.Remove(item2);
        }
    }

    public void ReduceWorkAmount(ref int workAmount, List<BodyPartRecord> BprToRegen, Pawn pawn, float maxRegen,
        bool localDebug = false)
    {
        if (BprToRegen.Count >= 3)
        {
            return;
        }

        foreach (var item in BprToRegen)
        {
            var num = item.def.GetMaxHealth(pawn) * maxRegen;
            if (!(pawn.health.hediffSet.GetPartHealth(item) > num * 0.75))
            {
                continue;
            }

            var num2 = (int)(workAmount * 0.75f);
            Tools.Warn($"{item.def.defName} Workamount reduced from {workAmount} to {num2}", localDebug);
            workAmount = num2;
        }
    }

    public void TendingLoopTick(bool localDebug = false)
    {
        var list = new List<TendingRequest>();
        foreach (var tendingRequest in tendingRequests)
        {
            var patient = tendingRequest.WorkRequest.Patient;
            if (patient.IsReserved(localDebug))
            {
                Tools.Warn($"{patient.LabelShort} TR added to 2delete by reservation", localDebug);
                list.Add(tendingRequest);
                continue;
            }

            if (!tendingRequest.WorkRequest.HasProgress)
            {
                var info = SoundInfo.InMap(new TargetInfo(patient.Position, patient.Map), MaintenanceType.PerTick);
                tendingRequest.WorkRequest.sustainer = SoundDefOf.Interact_Tend.TrySpawnSustainer(info);
            }
            else if (!tendingRequest.WorkRequest.IsComplete)
            {
                if (!tendingRequest.WorkRequest.sustainer.Ended)
                {
                    tendingRequest.WorkRequest.sustainer.Maintain();
                }
                else
                {
                    tendingRequest.WorkRequest.sustainer.End();
                }
            }
            else
            {
                MyDefs.TendStartSound.PlayOneShot(new TargetInfo(patient.Position, patient.Map));
            }

            tendingRequest.WorkRequest.Tick();
            if (!tendingRequest.WorkRequest.IsComplete)
            {
                continue;
            }

            var hediffToTend = tendingRequest.GetHediffToTend(localDebug);
            var num = ParentWR.calculatedTendingQualityRange.RandomInRange +
                      (patient.GetStatValue(StatDefOf.PsychicSensitivity) *
                       ParentWR.ParentComp.Props.tendingQualityPlaceboOffset * ParentWR.PlaceboWallsRatio);
            hediffToTend.Tended(num, 1f);
            ParentWR.ParentComp.refuelableComp.ConsumeFuel(num);
            tendingRequest.WorkRequest.ResetProgress();
            if (tendingRequest.HasTendingTodo)
            {
                continue;
            }

            Tools.Warn($"{patient.LabelShort} TR added to 2delete by no more work", localDebug);
            list.Add(tendingRequest);
        }

        foreach (var item in list)
        {
            tendingRequests.Remove(item);
        }
    }

    public void Tick(bool localDebug = false)
    {
        if (HasTendingRequest)
        {
            if (AutomaticTending || ManualTendingStart)
            {
                TendingLoopTick(localDebug);
            }

            if (!HasTendingRequest && !AutomaticTending)
            {
                ManualTendingStart = false;
            }
        }
        else if (HasRegenerationRequest)
        {
            if (AutomaticRegen || ManualRegenStart)
            {
                RegenLoopTick(localDebug);
            }

            if (!HasRegenerationRequest && !AutomaticRegen)
            {
                ManualRegenStart = false;
            }
        }
    }

    public bool AlreadyHasTendingRequest(Pawn pawn)
    {
        if (!HasTendingRequest)
        {
            return false;
        }

        foreach (var tendingRequest in tendingRequests)
        {
            if (tendingRequest.WorkRequest.Patient == pawn)
            {
                return true;
            }
        }

        return false;
    }

    public bool AlreadyHasRegenRequest(Pawn pawn)
    {
        if (!HasRegenerationRequest)
        {
            return false;
        }

        foreach (var regenRequest in regenRequests)
        {
            if (regenRequest.WorkRequest.Patient == pawn)
            {
                return true;
            }
        }

        return false;
    }

    public void DeletePawnTendingRequest(Pawn pawn)
    {
        if (!HasTendingRequest || pawn == null)
        {
            return;
        }

        for (var num = tendingRequests.Count - 1; num >= 0; num--)
        {
            if (tendingRequests[num].WorkRequest.Patient == pawn)
            {
                tendingRequests.RemoveAt(num);
            }
        }
    }

    public void DeletePawnRegenRequest(Pawn pawn)
    {
        if (!HasRegenerationRequest || pawn == null)
        {
            return;
        }

        for (var num = regenRequests.Count - 1; num >= 0; num--)
        {
            if (regenRequests[num].WorkRequest.Patient == pawn)
            {
                regenRequests.RemoveAt(num);
            }
        }
    }
}