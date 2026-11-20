using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace LTF_MedBay;

public static class PawnTools
{
    extension(Pawn pawn)
    {
        public bool HasBlood()
        {
            return pawn.RaceProps.BloodDef != null;
        }

        public bool CheckPawn()
        {
            return pawn is { Map: not null };
        }

        public bool IsAnimal()
        {
            return pawn.RaceProps.Animal;
        }

        public bool IsHuman()
        {
            if (pawn.RaceProps.Humanlike)
            {
                return pawn.def.defName == "Human";
            }

            return false;
        }

        public bool IsAlien()
        {
            if (pawn.RaceProps.Humanlike)
            {
                return pawn.def.defName != "Human";
            }

            return false;
        }

        public bool IsMechanoid()
        {
            return pawn.RaceProps.IsMechanoid;
        }

        public bool IsBetterThanNatural(BodyPartRecord bpr)
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

        public bool HasTendableInjury()
        {
            return pawn.health.hediffSet.HasTendableHediff();
        }

        public bool IsRegenEligible(float bodyPartMaxHealth, bool regeneratesBionics = false,
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

        public bool IsReserved(bool debug = false)
        {
            Tools.Warn($"{pawn.LabelShort} is already reserved",
                debug && pawn.Map.reservationManager.IsReservedByAnyoneOf(pawn, Faction.OfPlayer));
            return pawn.Map.reservationManager.IsReservedByAnyoneOf(pawn, Faction.OfPlayer);
        }

        public string PawnResumeString()
        {
            var empty = string.Empty;
            empty = $"{empty}({(pawn.Faction == null ? "no faction" : pawn.Faction.def.label)})";
            var ageTracker = pawn.ageTracker;
            if (ageTracker == null)
            {
                return null;
            }

            var years = ageTracker.AgeBiologicalYears;
            return
                $"{pawn.LabelShort.CapitalizeFirst()}, {years} y/o {pawn.gender.GetLabel()}, {pawn.def?.label}{empty}";
        }
    }
}