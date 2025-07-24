using System.Collections.Generic;
using System.Linq;
using Verse;

namespace LTF_MedBay;

public class RegenerationRequest
{
    private readonly HealingManager ParentHM;

    public readonly TargetingPawnWorkRequest WorkRequest;

    public RegenerationRequest(Pawn newPawn, HealingManager initiator)
    {
        ParentHM = initiator;
        WorkRequest = new TargetingPawnWorkRequest(newPawn, ParentHM);
        Tools.Warn($"new RegenerationRequest for {newPawn.LabelShort}", debug);
    }

    private Pawn pawn => WorkRequest?.Patient;

    private Comp_LTF_MedBay MBComp => ParentHM.ParentWR.ParentComp;

    private bool debug => MBComp.processDebug;

    private float WeightedBPRHealth(Pawn pawn, BodyPartRecord BPR, float ratio)
    {
        return BPR.def.GetMaxHealth(pawn) * ratio;
    }

    public List<BodyPartRecord> BodyPartsRegenerationEligible(float ratio, bool localDebug = false)
    {
        if (pawn == null)
        {
            Tools.Warn("BodyPartsRegenerationEligible cant work with null pawn", localDebug);
        }

        var list = new List<BodyPartRecord>();
        if (pawn == null)
        {
            return list.NullOrEmpty() ? null : list;
        }

        foreach (var notMissingPart in pawn.health.hediffSet.GetNotMissingParts())
        {
            if ((ParentHM.ParentWR.ParentComp.Props.regeneratesBionics ||
                 !pawn.IsBetterThanNatural(notMissingPart)) && pawn.health.hediffSet.GetPartHealth(notMissingPart) <
                WeightedBPRHealth(pawn, notMissingPart, ratio))
            {
                list.Add(notMissingPart);
            }
        }

        return list.NullOrEmpty() ? null : list;
    }

    public void RegenerateBodyPartTick(BodyPartRecord BPR, int workAmount, float maxHealthRatio,
        bool localDebug = false)
    {
        if (pawn == null)
        {
            Tools.Warn("RegenerateBodyPartTick cant work is null pawn", localDebug);
        }

        var maxHealth = BPR.def.GetMaxHealth(pawn);
        var num = maxHealth * maxHealthRatio;
        if (pawn == null)
        {
            return;
        }

        var partHealth = pawn.health.hediffSet.GetPartHealth(BPR);
        if (partHealth >= num)
        {
            Tools.Warn("Regen job was already done when entering RegenerateBodyPartTick", localDebug);
            return;
        }

        var num2 = num;
        var enumerable = pawn.health.hediffSet.hediffs.Where(h =>
            h.Part == BPR && ParentHM.ParentWR.ParentComp.Props.regenerableHediffDef.Contains(h.def));
        if (enumerable.EnumerableNullOrEmpty())
        {
            Tools.Warn($"No suitable Hediff to lower Severity to increase {BPR.def.defName} health", localDebug);
            return;
        }

        foreach (var item in enumerable)
        {
            var num3 = (num - partHealth) / workAmount;
            if (num3 < 0f)
            {
                Tools.Warn("regen is trying to hurt patient, leaving", localDebug);
            }

            item.Severity -= num3;
            ParentHM.ParentWR.ParentComp.refuelableComp.ConsumeFuel(num3 * MBComp.Props.fuelConsumptionPerRegenPoint);
            if (localDebug)
            {
                num2 -= item.Severity;
            }
        }

        if (!(partHealth >= num))
        {
            return;
        }

        Tools.Warn(
            $"Regen job has finished its job; newHealth: {num2}; oldHealth: {partHealth}; weightedMaxHealth: {num}; maxHealth: {maxHealth}",
            localDebug);
    }
}