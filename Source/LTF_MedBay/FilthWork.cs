using Verse;

namespace LTF_MedBay;

public class FilthWork
{
    public readonly Thing Filth;

    private readonly int WorkBase = 10;

    private int WorkAmount;

    public FilthWork(Thing filth)
    {
        if (filth != null)
        {
            Filth = filth;
        }

        Init();
    }

    public bool IsOver => WorkAmount < 1;

    public void Tick(int num)
    {
        WorkAmount -= num;
    }

    public void Init()
    {
        WorkAmount = WorkBase;
    }
}