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

    private static bool TargetsGenre(this TargetGenre targetGenre, TargetGenre targetGenreDef)
    {
        return (targetGenre & targetGenreDef) != 0;
    }

    public static bool TargetsHuman(this TargetGenre targetGenre)
    {
        return targetGenre.TargetsGenre(TargetGenre.Human);
    }

    public static bool TargetsAlien(this TargetGenre targetGenre)
    {
        return targetGenre.TargetsGenre(TargetGenre.Alien);
    }

    public static bool TargetsAnimal(this TargetGenre targetGenre)
    {
        return targetGenre.TargetsGenre(TargetGenre.Animal);
    }

    public static bool TargetsMechanoid(this TargetGenre targetGenre)
    {
        return targetGenre.TargetsGenre(TargetGenre.Mechanoid);
    }

    public static TargetGenre TogglesGenre(this TargetGenre targetGenre, TargetGenre targetGenreDef)
    {
        if (targetGenre.TargetsGenre(targetGenreDef))
        {
            return (TargetGenre)(targetGenre - targetGenreDef);
        }

        return (TargetGenre)((int)targetGenre + (int)targetGenreDef);
    }

    private static bool TargetsFaction(this TargetFaction targetFaction, TargetFaction targetFactionDef)
    {
        return (targetFaction & targetFactionDef) != 0;
    }

    public static bool TargetsNoFaction(this TargetFaction targetFaction)
    {
        return targetFaction.TargetsFaction(TargetFaction.NoFaction);
    }

    public static bool TargetsPlayer(this TargetFaction targetFaction)
    {
        return targetFaction.TargetsFaction(TargetFaction.Player);
    }

    public static bool TargetsAlly(this TargetFaction targetFaction)
    {
        return targetFaction.TargetsFaction(TargetFaction.Ally);
    }

    public static bool TargetsEnemy(this TargetFaction targetFaction)
    {
        return targetFaction.TargetsFaction(TargetFaction.Enemy);
    }

    public static TargetFaction TogglesFaction(this TargetFaction targetFaction, TargetFaction targetFactionDef)
    {
        if (targetFaction.TargetsFaction(targetFactionDef))
        {
            return (TargetFaction)(targetFaction - targetFactionDef);
        }

        return (TargetFaction)((int)targetFaction + (int)targetFactionDef);
    }

    public static bool IsFactionParametersCompatible(this TargetFaction targetFaction, Faction patientFaction,
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