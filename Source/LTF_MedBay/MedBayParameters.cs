using System;
using RimWorld;
using Verse;

namespace LTF_MedBay;

public static class MedBayParameters
{
    [Flags]
    public enum TargetFaction
    {
        [Description("Nobody")] Nobody = 0,
        [Description("No faction")] NoFaction = 1,
        [Description("Player faction")] Player = 2,
        [Description("Ally faction")] Ally = 4,
        [Description("Enemy faction")] Enemy = 8,
        [Description("Regular = player+ally")] Regular = 6,

        [Description("BlueAndRed = player+enemy")]
        BlueAndRed = 0xA,

        [Description("BlueAndGrey = player+nofaction")]
        BlueAndGrey = 3,

        [Description("BlueAndGreen = enemy+ally")]
        BlueAndGreen = 0xC,

        [Description("RedAndGrey = enemy+nofaction")]
        RedAndGrey = 9,

        [Description("GreenAndGrey = nofaction+ally")]
        GreenAndGrey = 5,

        [Description("Friendly = nofaction+player+ally")]
        Friendly = 7,

        [Description("Friendly = nofaction+player+ally")]
        EverybodyButPlayer = 0xD,

        [Description("Friendly = nofaction+player+ally")]
        EverybodyButAlly = 0xB,

        [Description("Friendly = nofaction+player+ally")]
        EverybodyButNoFaction = 0xE,

        [Description("Everybody = nofaction+player+ally+enemy")]
        Everybody = 0xF
    }

    [Flags]
    public enum TargetGenre
    {
        [Description("Nobody")] Nobody = 0,
        [Description("Human")] Human = 1,
        [Description("Alien")] Alien = 2,
        [Description("Animal")] Animal = 4,
        [Description("Mechanoid")] Mechanoid = 8,

        [Description("Humanoid = Alien + human")]
        Humanoid = 3,

        [Description("Terran = Animal + human")]
        Terran = 5,

        [Description("Gundam = mechanoid + human")]
        Gundam = 9,

        [Description("ManyA = Alien + Animal")]
        ManyA = 6,

        [Description("WarOfWorlds = Alien + Mechanoid")]
        WarOfWorlds = 0xA,

        [Description("SamuraiPizzaCat = mechanoid + animal")]
        SamuraiPizzaCat = 0xC,

        [Description("EverybodyButMecha = human+alien+animal")]
        EverybodyButMecha = 7,

        [Description("EverybodyButHuman = alien+animal+Mechanoid")]
        EverybodyButHuman = 0xE,

        [Description("EverybodyButAnimals = human+alien+Mechanoid")]
        EverybodyButAnimals = 0xB,

        [Description("EverybodyButAliens = human+animal+Mechanoid")]
        EverybodyButAliens = 0xD,

        [Description("Everybody = human+alien+animal+Mechanoid")]
        Everybody = 0xF
    }

    extension(TargetGenre targetGenre)
    {
        private bool TargetsGenre(TargetGenre targetGenreDef)
        {
            return (targetGenre & targetGenreDef) != 0;
        }

        public bool TargetsHuman()
        {
            return targetGenre.TargetsGenre(TargetGenre.Human);
        }

        public bool TargetsAlien()
        {
            return targetGenre.TargetsGenre(TargetGenre.Alien);
        }

        public bool TargetsAnimal()
        {
            return targetGenre.TargetsGenre(TargetGenre.Animal);
        }

        public bool TargetsMechanoid()
        {
            return targetGenre.TargetsGenre(TargetGenre.Mechanoid);
        }

        public TargetGenre TogglesGenre(TargetGenre targetGenreDef)
        {
            if (targetGenre.TargetsGenre(targetGenreDef))
            {
                return (TargetGenre)(targetGenre - targetGenreDef);
            }

            return (TargetGenre)((int)targetGenre + (int)targetGenreDef);
        }
    }

    extension(TargetFaction targetFaction)
    {
        private bool TargetsFaction(TargetFaction targetFactionDef)
        {
            return (targetFaction & targetFactionDef) != 0;
        }

        public bool TargetsNoFaction()
        {
            return targetFaction.TargetsFaction(TargetFaction.NoFaction);
        }

        public bool TargetsPlayer()
        {
            return targetFaction.TargetsFaction(TargetFaction.Player);
        }

        public bool TargetsAlly()
        {
            return targetFaction.TargetsFaction(TargetFaction.Ally);
        }

        public bool TargetsEnemy()
        {
            return targetFaction.TargetsFaction(TargetFaction.Enemy);
        }

        public TargetFaction TogglesFaction(TargetFaction targetFactionDef)
        {
            if (targetFaction.TargetsFaction(targetFactionDef))
            {
                return (TargetFaction)(targetFaction - targetFactionDef);
            }

            return (TargetFaction)((int)targetFaction + (int)targetFactionDef);
        }

        public bool IsFactionParametersCompatible(Faction patientFaction,
            bool debug = false)
        {
            Tools.Warn(
                $"Entering IsFactionParametersCompatible; TargetFaction: {targetFaction.DescriptionAttr()}; patientFaction: {patientFaction} ==null?: {patientFaction == null}; playerFaction: {Faction.OfPlayer}",
                debug);
            if (targetFaction == TargetFaction.Nobody)
            {
                return false;
            }

            if (targetFaction.TargetsNoFaction() && patientFaction == null)
            {
                return true;
            }

            if (targetFaction.TargetsPlayer() && patientFaction == Faction.OfPlayer)
            {
                return true;
            }

            if (targetFaction.TargetsAlly() && patientFaction.AllyOrNeutralTo(Faction.OfPlayer))
            {
                return true;
            }

            return targetFaction.TargetsEnemy() && patientFaction.HostileTo(Faction.OfPlayer);
        }
    }

    public static bool IsGenreParametersCompatible(this TargetGenre targetGenre, Pawn patient, bool debug = false)
    {
        Tools.Warn(
            $"Entering IsGenreParametersCompatibletargetGenre: {targetGenre.DescriptionAttr()}; patient: {patient.LabelShort}",
            debug);
        if (targetGenre == TargetGenre.Nobody)
        {
            return false;
        }

        if (targetGenre.TargetsAnimal() && patient.IsAnimal())
        {
            return true;
        }

        if (targetGenre.TargetsHuman() && patient.IsHuman())
        {
            return true;
        }

        if (targetGenre.TargetsAlien() && patient.IsAlien())
        {
            return true;
        }

        return targetGenre.TargetsMechanoid() && patient.IsMechanoid();
    }
}