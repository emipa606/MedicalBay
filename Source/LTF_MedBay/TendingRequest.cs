using System.Linq;
using Verse;

namespace LTF_MedBay;

public class TendingRequest
{
    private readonly HealingManager ParentHM;

    public readonly TargetingPawnWorkRequest WorkRequest;

    public TendingRequest(Pawn newPawn, HealingManager parentHM)
    {
        ParentHM = parentHM;
        WorkRequest = new TargetingPawnWorkRequest(newPawn, ParentHM);
        Tools.Warn($"new TendingRequest for {newPawn.LabelShort}", debug);
    }

    private Pawn pawn => WorkRequest?.Patient;

    private Comp_LTF_MedBay MBComp => ParentHM.ParentWR.ParentComp;

    private bool debug => MBComp.Props.debug;

    public bool HasTendingTodo => GetHediffToTend() != null;

    public Hediff GetHediffToTend(bool localDebug = false)
    {
        if (pawn == null)
        {
            Tools.Warn("BodyPartsTendingEligible cant work with null pawn", localDebug);
            return null;
        }

        var injuriesTendable = pawn.health.hediffSet.GetHediffsTendable();
        if (!injuriesTendable.EnumerableNullOrEmpty())
        {
            return injuriesTendable.First();
        }

        Tools.Warn("BodyPartsTendingEligible found no eligible hediff to tend", localDebug);
        return null;
    }
}