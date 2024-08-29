using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace LTF_MedBay;

public static class PawnTools
{
    public static bool HasBlood(this Pawn pawn)
    {
        return pawn.RaceProps.BloodDef != null;
    }

    public static bool CheckPawn(this Pawn pawn)
    {
        return pawn is { Map: not null };
    }

    public static bool IsAnimal(this Pawn pawn)
    {
        return pawn.RaceProps.Animal;
    }

    public static bool IsHuman(this Pawn pawn)
    {
        if (pawn.RaceProps.Humanlike)
        {
            return pawn.def.defName == "Human";
        }

        return false;
    }

    public static bool IsAlien(this Pawn pawn)
    {
        if (pawn.RaceProps.Humanlike)
        {
            return pawn.def.defName != "Human";
        }

        return false;
    }

    public static bool IsMechanoid(this Pawn pawn)
    {
        return pawn.RaceProps.IsMechanoid;
    }

    public static bool IsBetterThanNatural(this Pawn pawn, BodyPartRecord bpr)
    {
        var hediffList = new List<Hediff>();
        pawn.health.hediffSet.GetHediffs(ref hediffList, delegate(Hediff h)
        {
            if (h.Part != bpr)
            {
                return false;
            }

            var addedPartProps = h.def.addedPartProps;
            return addedPartProps is { betterThanNatural: true, solid: true };
        });
        return hediffList.Any();
    }

    public static bool HasTendableInjury(this Pawn pawn)
    {
        return pawn.health.hediffSet.HasTendableHediff();
    }

    public static bool IsRegenEligible(this Pawn pawn, float bodyPartMaxHealth, bool regeneratesBionics = false,
        bool debug = false)
    {
        var notMissingParts = pawn.health.hediffSet.GetNotMissingParts();
        if (notMissingParts.EnumerableNullOrEmpty())
        {
            Tools.Warn($"{pawn.LabelShort} is very suspicious with only missing BPR", debug);
            return false;
        }

        Tools.Warn($"{pawn.LabelShort} has {notMissingParts.Count()} non missing BPR", debug);
        foreach (var item in notMissingParts)
        {
            if (pawn.IsBetterThanNatural(item) && !regeneratesBionics)
            {
                Tools.Warn($"{pawn.LabelShort} has a bionic {item.def.defName} and medbay does not treat them",
                    debug);
            }
            else if (pawn.health.hediffSet.GetPartHealth(item) < bodyPartMaxHealth * item.def.GetMaxHealth(pawn))
            {
                return true;
            }
        }

        return false;
    }

    public static bool IsReserved(this Pawn pawn, bool debug = false)
    {
        Tools.Warn($"{pawn.LabelShort} is already reserved",
            debug && pawn.Map.reservationManager.IsReservedByAnyoneOf(pawn, Faction.OfPlayer));
        return pawn.Map.reservationManager.IsReservedByAnyoneOf(pawn, Faction.OfPlayer);
    }

    public static string PawnResumeString(this Pawn pawn)
    {
        var empty = string.Empty;
        empty = $"{empty}({(pawn.Faction == null ? "no faction" : pawn.Faction.def.label)})";
        var ageTracker = pawn.ageTracker;
        if (ageTracker == null)
        {
            return null;
        }

        _ = ageTracker.AgeBiologicalYears;
        if (true)
        {
            return
                $"{pawn.LabelShort.CapitalizeFirst()}, {(pawn.ageTracker?.AgeBiologicalYears).Value} y/o {pawn.gender.GetLabel()}, {pawn.def?.label}{empty}";
        }
    }
}