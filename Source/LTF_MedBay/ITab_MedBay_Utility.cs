using UnityEngine;
using Verse;

namespace LTF_MedBay;

public class ITab_MedBay_Utility
{
    public static readonly Vector2 WinSize = new(420f, 610f);

    public static readonly Rect WinScrollable = new(0f, 0f, WinSize.x, WinSize.y);

    public static readonly Rect WinInnerScrollable = new(0f, 0f, WinScrollable.width - 16f, WinScrollable.height);

    private static Vector2 scrollPosition = Vector2.zero;

    public static void Draw_ITab_MedBay_Settings(Rect rect, ThingWithComps MedBay)
    {
        var tabContentRect = rect;
        tabContentRect.x = 0;
        tabContentRect.y = 0;
        tabContentRect.height = rect.height * 2f;
        tabContentRect.width -= 10;
        Widgets.BeginScrollView(rect, ref scrollPosition, tabContentRect);

        var comp_LTF_MedBay = MedBay.TryGetComp<Comp_LTF_MedBay>();
        var iconRect = new Vector2(64f, 64f);
        var vector = new Vector2(32f, 32f);
        var num = 4f;
        var text = "   ";
        var text2 = " ";
        GUI.BeginGroup(tabContentRect);
        Text.Font = GameFont.Medium;
        GUI.DrawTexture(new Rect(0f, 0f, iconRect.x, iconRect.y), Gfx.IconWaitingParams);
        Widgets.Label(ComplementaryTextBox(WinSize, iconRect, new Vector2(0f, 0f)),
            text + "Tab_LTF_MedBay_InsideTitle".Translate());
        Widgets.DrawLineHorizontal(num, iconRect.y + num, WinSize.x - (5f * num));
        if (Widgets.ButtonImage(new Rect(WinSize.x - (5f * num) - vector.x, vector.y / 2f, vector.x, vector.y),
                comp_LTF_MedBay.HideFactionSection && comp_LTF_MedBay.HideAutoSection && comp_LTF_MedBay.HideRaceSection
                    ? Gfx.ITabMat_Hidden
                    : Gfx.ITabMat_Expanded))
        {
            comp_LTF_MedBay.HideAutoSection = comp_LTF_MedBay.HideFactionSection = comp_LTF_MedBay.HideRaceSection =
                !comp_LTF_MedBay.HideFactionSection || !comp_LTF_MedBay.HideAutoSection ||
                !comp_LTF_MedBay.HideRaceSection;
        }

        var num2 = (2f * num) + iconRect.y;
        GUI.DrawTexture(new Rect(0f, num2, iconRect.x, iconRect.y), Gfx.IconAutoParams);
        Widgets.Label(ComplementaryTextBox(WinSize, iconRect, new Vector2(0f, num2)),
            text + "Tab_LTF_MedBay_AutomaticParams".Translate());
        Widgets.DrawLineHorizontal(iconRect.x + 1f, num2 + iconRect.y, WinSize.x - iconRect.y - (9f * num));
        if (Widgets.ButtonImage(new Rect(WinSize.x - (8f * num) - vector.x, num2 + (vector.y / 2f), vector.x, vector.y),
                comp_LTF_MedBay.HideAutoSection ? Gfx.ITabMat_Hidden : Gfx.ITabMat_Expanded))
        {
            comp_LTF_MedBay.HideAutoSection = !comp_LTF_MedBay.HideAutoSection;
        }

        num2 += iconRect.y + num;
        if (!comp_LTF_MedBay.HideAutoSection)
        {
            Text.Font = GameFont.Small;
            var automatics = comp_LTF_MedBay.MyWaitingRoom.MyHealingManager.AutomaticTending ||
                             comp_LTF_MedBay.MyWaitingRoom.MyHealingManager.AutomaticRegen;
            GUI.DrawTexture(new Rect(0f, num2, iconRect.x, iconRect.y), automatics ? Gfx.IconAutoOn : Gfx.IconAutoOff);
            Widgets.Label(ComplementaryTextBox(WinSize, iconRect, new Vector2(0f, num2)),
                text2 + (automatics ? "Tab_LTF_MedBay_AutomaticOn" : "Tab_LTF_MedBay_AutomaticOff").Translate());
            if (Widgets.ButtonImage(
                    new Rect(WinSize.x - (8f * num) - vector.x, num2 + (vector.y / 2f), vector.x, vector.y),
                    automatics ? Widgets.CheckboxOnTex : Widgets.CheckboxOffTex))
            {
                comp_LTF_MedBay.AutomaticTending = comp_LTF_MedBay.AutomaticRegen =
                    comp_LTF_MedBay.MyWaitingRoom.MyHealingManager.AutomaticTending =
                        comp_LTF_MedBay.MyWaitingRoom.MyHealingManager.AutomaticRegen = !automatics;
            }

            num2 += iconRect.y + num;
            automatics = comp_LTF_MedBay.MyWaitingRoom.MyHealingManager.AutomaticTending;
            GUI.DrawTexture(new Rect(0f, num2, iconRect.x, iconRect.y),
                automatics ? Gfx.IconTendingOn : Gfx.IconTendingOff);
            Widgets.Label(ComplementaryTextBox(WinSize, iconRect, new Vector2(0f, num2)),
                text2 + (automatics ? "Tab_LTF_MedBay_TendingOn" : "Tab_LTF_MedBay_TendingOff").Translate());
            if (Widgets.ButtonImage(
                    new Rect(WinSize.x - (8f * num) - vector.x, num2 + (vector.y / 2f), vector.x, vector.y),
                    automatics ? Widgets.CheckboxOnTex : Widgets.CheckboxOffTex))
            {
                comp_LTF_MedBay.AutomaticTending =
                    comp_LTF_MedBay.MyWaitingRoom.MyHealingManager.AutomaticTending = !automatics;
            }

            num2 += iconRect.y + num;
            automatics = comp_LTF_MedBay.MyWaitingRoom.MyHealingManager.AutomaticRegen;
            GUI.DrawTexture(new Rect(0f, num2, iconRect.x, iconRect.y),
                automatics ? Gfx.IconRegenOn : Gfx.IconRegenOff);
            Widgets.Label(ComplementaryTextBox(WinSize, iconRect, new Vector2(0f, num2)),
                text2 + (automatics ? "Tab_LTF_MedBay_RegenOn" : "Tab_LTF_MedBay_RegenOff").Translate());
            if (Widgets.ButtonImage(
                    new Rect(WinSize.x - (8f * num) - vector.x, num2 + (vector.y / 2f), vector.x, vector.y),
                    automatics ? Widgets.CheckboxOnTex : Widgets.CheckboxOffTex))
            {
                comp_LTF_MedBay.AutomaticRegen =
                    comp_LTF_MedBay.MyWaitingRoom.MyHealingManager.AutomaticRegen = !automatics;
            }

            num2 += iconRect.y + num;
        }

        Text.Font = GameFont.Medium;
        GUI.DrawTexture(new Rect(0f, num2, iconRect.x, iconRect.y), Gfx.IconFactionParams);
        Widgets.Label(ComplementaryTextBox(WinSize, iconRect, new Vector2(0f, num2)),
            text + "Tab_LTF_MedBay_PatientFaction".Translate());
        Widgets.DrawLineHorizontal(iconRect.x + 1f, num2 + iconRect.y, WinSize.x - iconRect.y - (9f * num));
        if (Widgets.ButtonImage(new Rect(WinSize.x - (8f * num) - vector.x, num2 + (vector.y / 2f), vector.x, vector.y),
                comp_LTF_MedBay.HideFactionSection ? Gfx.ITabMat_Hidden : Gfx.ITabMat_Expanded))
        {
            comp_LTF_MedBay.HideFactionSection = !comp_LTF_MedBay.HideFactionSection;
        }

        num2 += iconRect.y + num;
        Text.Font = GameFont.Small;
        if (!comp_LTF_MedBay.HideFactionSection)
        {
            Text.Font = GameFont.Small;
            var targets = comp_LTF_MedBay.MyWaitingRoom.FactionParams.TargetsAlly() ||
                          comp_LTF_MedBay.MyWaitingRoom.FactionParams.TargetsEnemy() ||
                          comp_LTF_MedBay.MyWaitingRoom.FactionParams.TargetsNoFaction() ||
                          comp_LTF_MedBay.MyWaitingRoom.FactionParams.TargetsPlayer();
            GUI.DrawTexture(new Rect(0f, num2, iconRect.x, iconRect.y),
                targets ? Gfx.IconFactionEverybody : Gfx.IconFactionNobody);
            Widgets.Label(ComplementaryTextBox(WinSize, iconRect, new Vector2(0f, num2)),
                text2 + (targets ? "Tab_LTF_MedBay_FactionEverybody" : "Tab_LTF_MedBay_FactionNobody").Translate());
            if (Widgets.ButtonImage(
                    new Rect(WinSize.x - (8f * num) - vector.x, num2 + (vector.y / 2f), vector.x, vector.y),
                    targets ? Widgets.CheckboxOnTex : Widgets.CheckboxOffTex))
            {
                comp_LTF_MedBay.FactionParams = comp_LTF_MedBay.MyWaitingRoom.FactionParams =
                    !targets ? MedBayParameters.TargetFaction.Everybody : MedBayParameters.TargetFaction.Nobody;
            }

            num2 += iconRect.y + num;
            targets = comp_LTF_MedBay.MyWaitingRoom.FactionParams.TargetsPlayer();
            GUI.DrawTexture(new Rect(0f, num2, iconRect.x, iconRect.y), targets ? Gfx.IconPlayerOn : Gfx.IconPlayerOff);
            Widgets.Label(ComplementaryTextBox(WinSize, iconRect, new Vector2(0f, num2)),
                text2 + (targets ? "Tab_LTF_MedBay_PlayerOn" : "Tab_LTF_MedBay_PlayerOff").Translate());
            if (Widgets.ButtonImage(
                    new Rect(WinSize.x - (8f * num) - vector.x, num2 + (vector.y / 2f), vector.x, vector.y),
                    targets ? Widgets.CheckboxOnTex : Widgets.CheckboxOffTex))
            {
                comp_LTF_MedBay.FactionParams = comp_LTF_MedBay.MyWaitingRoom.FactionParams =
                    comp_LTF_MedBay.MyWaitingRoom.FactionParams.TogglesFaction(MedBayParameters.TargetFaction.Player);
            }

            num2 += iconRect.y + num;
            targets = comp_LTF_MedBay.MyWaitingRoom.FactionParams.TargetsAlly();
            GUI.DrawTexture(new Rect(0f, num2, iconRect.x, iconRect.y), targets ? Gfx.IconAllyOn : Gfx.IconAllyOff);
            Widgets.Label(ComplementaryTextBox(WinSize, iconRect, new Vector2(0f, num2)),
                text2 + (targets ? "Tab_LTF_MedBay_AllyOn" : "Tab_LTF_MedBay_AllyOff").Translate());
            if (Widgets.ButtonImage(
                    new Rect(WinSize.x - (8f * num) - vector.x, num2 + (vector.y / 2f), vector.x, vector.y),
                    targets ? Widgets.CheckboxOnTex : Widgets.CheckboxOffTex))
            {
                comp_LTF_MedBay.FactionParams = comp_LTF_MedBay.MyWaitingRoom.FactionParams =
                    comp_LTF_MedBay.MyWaitingRoom.FactionParams.TogglesFaction(MedBayParameters.TargetFaction.Ally);
            }

            num2 += iconRect.y + num;
            targets = comp_LTF_MedBay.MyWaitingRoom.FactionParams.TargetsNoFaction();
            GUI.DrawTexture(new Rect(0f, num2, iconRect.x, iconRect.y),
                targets ? Gfx.IconNoFactionOn : Gfx.IconNoFactionOff);
            Widgets.Label(ComplementaryTextBox(WinSize, iconRect, new Vector2(0f, num2)),
                text2 + (targets ? "Tab_LTF_MedBay_NoFactionOn" : "Tab_LTF_MedBay_NoFactionOff").Translate());
            if (Widgets.ButtonImage(
                    new Rect(WinSize.x - (8f * num) - vector.x, num2 + (vector.y / 2f), vector.x, vector.y),
                    targets ? Widgets.CheckboxOnTex : Widgets.CheckboxOffTex))
            {
                comp_LTF_MedBay.FactionParams = comp_LTF_MedBay.MyWaitingRoom.FactionParams =
                    comp_LTF_MedBay.MyWaitingRoom.FactionParams.TogglesFaction(MedBayParameters.TargetFaction
                        .NoFaction);
            }

            num2 += iconRect.y + num;
            targets = comp_LTF_MedBay.MyWaitingRoom.FactionParams.TargetsEnemy();
            GUI.DrawTexture(new Rect(0f, num2, iconRect.x, iconRect.y), targets ? Gfx.IconEnemyOn : Gfx.IconEnemyOff);
            Widgets.Label(ComplementaryTextBox(WinSize, iconRect, new Vector2(0f, num2)),
                text2 + (targets ? "Tab_LTF_MedBay_EnemyOn" : "Tab_LTF_MedBay_EnemyOff").Translate());
            if (Widgets.ButtonImage(
                    new Rect(WinSize.x - (8f * num) - vector.x, num2 + (vector.y / 2f), vector.x, vector.y),
                    targets ? Widgets.CheckboxOnTex : Widgets.CheckboxOffTex))
            {
                comp_LTF_MedBay.FactionParams = comp_LTF_MedBay.MyWaitingRoom.FactionParams =
                    comp_LTF_MedBay.MyWaitingRoom.FactionParams.TogglesFaction(MedBayParameters.TargetFaction.Enemy);
            }

            num2 += iconRect.y + num;
        }

        Text.Font = GameFont.Medium;
        GUI.DrawTexture(new Rect(0f, num2, iconRect.x, iconRect.y), Gfx.IconKindParams);
        Widgets.Label(ComplementaryTextBox(WinSize, iconRect, new Vector2(0f, num2)),
            text + "Tab_LTF_MedBay_PatientRace".Translate());
        Widgets.DrawLineHorizontal(iconRect.x + 1f, num2 + iconRect.y, WinSize.x - iconRect.y - (9f * num));
        if (Widgets.ButtonImage(new Rect(WinSize.x - (8f * num) - vector.x, num2 + (vector.y / 2f), vector.x, vector.y),
                comp_LTF_MedBay.HideRaceSection ? Gfx.ITabMat_Hidden : Gfx.ITabMat_Expanded))
        {
            comp_LTF_MedBay.HideRaceSection = !comp_LTF_MedBay.HideRaceSection;
        }

        num2 += iconRect.y + num;
        Text.Font = GameFont.Small;
        if (!comp_LTF_MedBay.HideRaceSection)
        {
            Text.Font = GameFont.Small;
            var targets = comp_LTF_MedBay.MyWaitingRoom.GenreParams.TargetsAnimal() ||
                          comp_LTF_MedBay.MyWaitingRoom.GenreParams.TargetsHuman() ||
                          comp_LTF_MedBay.MyWaitingRoom.GenreParams.TargetsAlien() ||
                          comp_LTF_MedBay.MyWaitingRoom.GenreParams.TargetsMechanoid();
            GUI.DrawTexture(new Rect(0f, num2, iconRect.x, iconRect.y),
                targets ? Gfx.IconKindEverybody : Gfx.IconKindNobody);
            Widgets.Label(ComplementaryTextBox(WinSize, iconRect, new Vector2(0f, num2)),
                text2 + (targets ? "Tab_LTF_MedBay_RaceEverybody" : "Tab_LTF_MedBay_RaceNobody").Translate());
            if (Widgets.ButtonImage(
                    new Rect(WinSize.x - (8f * num) - vector.x, num2 + (vector.y / 2f), vector.x, vector.y),
                    targets ? Widgets.CheckboxOnTex : Widgets.CheckboxOffTex))
            {
                comp_LTF_MedBay.GenreParams = comp_LTF_MedBay.MyWaitingRoom.GenreParams =
                    !targets ? MedBayParameters.TargetGenre.Everybody : MedBayParameters.TargetGenre.Nobody;
            }

            num2 += iconRect.y + num;
            targets = comp_LTF_MedBay.MyWaitingRoom.GenreParams.TargetsAnimal();
            GUI.DrawTexture(new Rect(0f, num2, iconRect.x, iconRect.y), targets ? Gfx.IconAnimalOn : Gfx.IconAnimalOff);
            Widgets.Label(ComplementaryTextBox(WinSize, iconRect, new Vector2(0f, num2)),
                text2 + (targets ? "Tab_LTF_MedBay_AnimalOn" : "Tab_LTF_MedBay_AnimalOff").Translate());
            if (Widgets.ButtonImage(
                    new Rect(WinSize.x - (8f * num) - vector.x, num2 + (vector.y / 2f), vector.x, vector.y),
                    targets ? Widgets.CheckboxOnTex : Widgets.CheckboxOffTex))
            {
                comp_LTF_MedBay.GenreParams = comp_LTF_MedBay.MyWaitingRoom.GenreParams =
                    comp_LTF_MedBay.MyWaitingRoom.GenreParams.TogglesGenre(MedBayParameters.TargetGenre.Animal);
            }

            num2 += iconRect.y + num;
            targets = comp_LTF_MedBay.MyWaitingRoom.GenreParams.TargetsHuman();
            GUI.DrawTexture(new Rect(0f, num2, iconRect.x, iconRect.y), targets ? Gfx.IconHumanOn : Gfx.IconHumanOff);
            Widgets.Label(ComplementaryTextBox(WinSize, iconRect, new Vector2(0f, num2)),
                text2 + (targets ? "Tab_LTF_MedBay_HumanOn" : "Tab_LTF_MedBay_HumanOff").Translate());
            if (Widgets.ButtonImage(
                    new Rect(WinSize.x - (8f * num) - vector.x, num2 + (vector.y / 2f), vector.x, vector.y),
                    targets ? Widgets.CheckboxOnTex : Widgets.CheckboxOffTex))
            {
                comp_LTF_MedBay.GenreParams = comp_LTF_MedBay.MyWaitingRoom.GenreParams =
                    comp_LTF_MedBay.MyWaitingRoom.GenreParams.TogglesGenre(MedBayParameters.TargetGenre.Human);
            }

            num2 += iconRect.y + num;
            targets = comp_LTF_MedBay.MyWaitingRoom.GenreParams.TargetsAlien();
            GUI.DrawTexture(new Rect(0f, num2, iconRect.x, iconRect.y), targets ? Gfx.IconAlienOn : Gfx.IconAlienOff);
            Widgets.Label(ComplementaryTextBox(WinSize, iconRect, new Vector2(0f, num2)),
                text2 + (targets ? "Tab_LTF_MedBay_AlienOn" : "Tab_LTF_MedBay_AlienOff").Translate());
            if (Widgets.ButtonImage(
                    new Rect(WinSize.x - (8f * num) - vector.x, num2 + (vector.y / 2f), vector.x, vector.y),
                    targets ? Widgets.CheckboxOnTex : Widgets.CheckboxOffTex))
            {
                comp_LTF_MedBay.GenreParams = comp_LTF_MedBay.MyWaitingRoom.GenreParams =
                    comp_LTF_MedBay.MyWaitingRoom.GenreParams.TogglesGenre(MedBayParameters.TargetGenre.Alien);
            }

            num2 += iconRect.y + num;
            if (comp_LTF_MedBay.Props.treatsMechanoids)
            {
                targets = comp_LTF_MedBay.MyWaitingRoom.GenreParams.TargetsMechanoid();
                GUI.DrawTexture(new Rect(0f, num2, iconRect.x, iconRect.y),
                    targets ? Gfx.IconMechanoidOn : Gfx.IconMechanoidOff);
                Widgets.Label(ComplementaryTextBox(WinSize, iconRect, new Vector2(0f, num2)),
                    text2 + (targets ? "Tab_LTF_MedBay_MechanoidOn" : "Tab_LTF_MedBay_MechanoidOff").Translate());
                if (Widgets.ButtonImage(
                        new Rect(WinSize.x - (8f * num) - vector.x, num2 + (vector.y / 2f), vector.x, vector.y),
                        targets ? Widgets.CheckboxOnTex : Widgets.CheckboxOffTex))
                {
                    comp_LTF_MedBay.GenreParams = comp_LTF_MedBay.MyWaitingRoom.GenreParams =
                        comp_LTF_MedBay.MyWaitingRoom.GenreParams.TogglesGenre(MedBayParameters.TargetGenre.Mechanoid);
                }
            }
        }

        GUI.EndGroup();
        Widgets.EndScrollView();
    }

    public static Rect ComplementaryTextBox(Vector2 winSize, Vector2 iconRect, Vector2 offset)
    {
        var result = default(Rect);
        result.x = offset.x + iconRect.x;
        result.y = offset.y + (iconRect.y / 2f);
        result.width = winSize.x - iconRect.x - 16f - offset.x;
        result.height = iconRect.y;
        return result;
    }
}