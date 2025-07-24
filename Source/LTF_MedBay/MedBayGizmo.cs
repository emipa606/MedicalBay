using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace LTF_MedBay;

public class MedBayGizmo
{
    private static void ShowReport(WaitingRoom waitingRoom)
    {
        var stringBuilder = new StringBuilder();
        _ = string.Empty;
        stringBuilder.AppendLine(waitingRoom.Report());
        var window = new Dialog_MessageBox(stringBuilder.ToString());
        Find.WindowStack.Add(window);
    }

    public static IEnumerable<Gizmo> MainGizmo(Comp_LTF_MedBay MBComp)
    {
        if (MBComp.powerComp.CheckPower())
        {
            foreach (var item in WaitingRoomGizmo(MBComp))
            {
                yield return item;
            }

            var enumerable = ManualTendingStart(MBComp);
            if (!enumerable.EnumerableNullOrEmpty())
            {
                yield return enumerable.First();
            }

            enumerable = ManualRegenStart(MBComp);
            if (!enumerable.EnumerableNullOrEmpty())
            {
                yield return enumerable.First();
            }

            enumerable = ManualStartAll(MBComp);
            if (!enumerable.EnumerableNullOrEmpty())
            {
                yield return ManualStartAll(MBComp).First();
            }
        }

        foreach (var item2 in DebugGizmo(MBComp))
        {
            yield return item2;
        }
    }

    private static IEnumerable<Gizmo> WaitingRoomGizmo(Comp_LTF_MedBay MBComp)
    {
        if (!MBComp.ValidRoom)
        {
            yield break;
        }

        var potentialPatientCount = MBComp.MyWaitingRoom.PotentialPatientCount;
        yield return new Command_Action
        {
            icon = Gfx.IconWaitingLog,
            defaultLabel = "Log",
            defaultDesc = $"{potentialPatientCount} patient{(potentialPatientCount > 1 ? "s" : "")}",
            action = delegate { ShowReport(MBComp.MyWaitingRoom); }
        };
    }

    private static IEnumerable<Gizmo> ManualTendingStart(Comp_LTF_MedBay MBComp)
    {
        if (MBComp.MyWaitingRoom.MyHealingManager.AutomaticTending || !MBComp.HasTendingRequest ||
            MBComp.MyWaitingRoom.MyHealingManager.ManualTendingStart)
        {
            yield break;
        }

        var tRCount = MBComp.MyWaitingRoom.MyHealingManager.TRCount;
        yield return new Command_Action
        {
            icon = Gfx.IconStartTend,
            defaultLabel = "Start tending",
            defaultDesc = $"{tRCount} tending request{(tRCount > 1 ? "s" : "")} in queue",
            action = delegate { MBComp.MyWaitingRoom.MyHealingManager.ManualTendingStart = true; }
        };
    }

    private static IEnumerable<Gizmo> ManualRegenStart(Comp_LTF_MedBay MBComp)
    {
        if (MBComp.MyWaitingRoom.MyHealingManager.AutomaticRegen || !MBComp.HasRegenerationRequest ||
            MBComp.MyWaitingRoom.MyHealingManager.ManualRegenStart)
        {
            yield break;
        }

        var rRCount = MBComp.MyWaitingRoom.MyHealingManager.RRCount;
        yield return new Command_Action
        {
            icon = Gfx.IconStartRegen,
            defaultLabel = "Start regen",
            defaultDesc = $"{rRCount} regen request{(rRCount > 1 ? "s" : "")} in queue",
            action = delegate { MBComp.MyWaitingRoom.MyHealingManager.ManualRegenStart = true; }
        };
    }

    private static IEnumerable<Gizmo> ManualStartAll(Comp_LTF_MedBay MBComp)
    {
        if (MBComp.MyWaitingRoom.MyHealingManager.AutomaticRegen ||
            !MBComp.MyWaitingRoom.MyHealingManager.AutomaticTending || !MBComp.HasTendingRequest ||
            MBComp.MyWaitingRoom.MyHealingManager.ManualTendingStart || !MBComp.HasRegenerationRequest ||
            MBComp.MyWaitingRoom.MyHealingManager.ManualRegenStart)
        {
            yield break;
        }

        var num = MBComp.MyWaitingRoom.MyHealingManager.RRCount + MBComp.MyWaitingRoom.MyHealingManager.TRCount;
        yield return new Command_Action
        {
            icon = Gfx.IconStartAll,
            defaultLabel = "Start all",
            defaultDesc = $"{num} healing request{(num > 1 ? "s" : "")} in queue",
            action = delegate
            {
                MBComp.MyWaitingRoom.MyHealingManager.ManualTendingStart = true;
                MBComp.MyWaitingRoom.MyHealingManager.ManualRegenStart = true;
            }
        };
    }

    private static IEnumerable<Gizmo> DebugGizmo(Comp_LTF_MedBay MBComp)
    {
        if (!DebugSettings.godMode)
        {
            yield break;
        }

        yield return new Command_Action
        {
            icon = Gfx.IconDebug,
            defaultLabel = "process debug",
            defaultDesc = Tools.DebugStatus(MBComp.processDebug),
            action = delegate { MBComp.processDebug = !MBComp.processDebug; }
        };
        yield return new Command_Action
        {
            icon = Gfx.IconDebug,
            defaultLabel = "gfx debug",
            defaultDesc = Tools.DebugStatus(MBComp.gfxDebug),
            action = delegate { MBComp.gfxDebug = !MBComp.gfxDebug; }
        };
    }
}