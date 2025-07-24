using Verse;
using Verse.Sound;

namespace LTF_MedBay;

public class TargetingPawnWorkRequest
{
    private readonly HealingManager ParentHM;

    public readonly Pawn Patient;

    private int Progress;

    public Sustainer sustainer;

    public int WorkAmount = 500;

    public TargetingPawnWorkRequest(Pawn newPawn, HealingManager parentHM)
    {
        if (!newPawn.CheckPawn())
        {
            Tools.Warn("new TargetingPawnWorkRequest newPawn null;", true);
        }

        ParentHM = parentHM;
        Patient = newPawn;
        Tools.Warn($"new TargetingPawnWorkRequest for {Patient.LabelShort}", debug);
    }

    private Comp_LTF_MedBay MBComp => ParentHM.ParentWR.ParentComp;

    private bool debug => MBComp.Props.debug;

    public bool HasProgress => Progress != 0;

    public bool IsComplete => Progress > WorkAmount;

    public float ProgressRatio
    {
        get
        {
            if (WorkAmount == 0)
            {
                return 1f;
            }

            return Progress / (float)WorkAmount;
        }
    }

    public string ProgressPerc => $"{ProgressRatio * 100f:2F}%";

    public bool HasTarget => Patient != null;

    public void ResetProgress()
    {
        Progress = 0;
    }

    private string ProgressString()
    {
        _ = string.Empty;
        return $"[{Progress} / {WorkAmount}]";
    }

    private void ProgressHax()
    {
        Progress = WorkAmount;
    }

    public void Tick()
    {
        Progress++;
    }
}