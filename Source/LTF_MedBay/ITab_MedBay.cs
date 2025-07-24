using RimWorld;
using UnityEngine;
using Verse;

namespace LTF_MedBay;

public class ITab_MedBay : ITab
{
    public static readonly Vector2 WinSize = new(300f, 480f);

    public ITab_MedBay()
    {
        var vector = new Vector2(17f, 17f);
        size = ITab_MedBay_Utility.WinSize + vector;
        labelKey = "Tab_LTF_MedBay_OutsideTitle";
    }

    public override bool IsVisible
    {
        get
        {
            var thing = SelObject as Thing;
            if (thing is not Building building || !MyDefs.MedBayDefName.Contains(building.def.defName))
            {
                return false;
            }

            var comp_LTF_MedBay = building.TryGetComp<Comp_LTF_MedBay>();
            if (comp_LTF_MedBay == null)
            {
                return false;
            }

            if (thing.Faction != null && thing.Faction == Faction.OfPlayer && comp_LTF_MedBay.CheckMedBay &&
                comp_LTF_MedBay.HasPowerOn)
            {
                return comp_LTF_MedBay.ValidRoom;
            }

            return false;
        }
    }

    protected override void FillTab()
    {
        var medBay = Find.Selector.SingleSelectedThing as ThingWithComps;
        ITab_MedBay_Utility.Draw_ITab_MedBay_Settings(
            new Rect(17f, 17f, ITab_MedBay_Utility.WinSize.x, ITab_MedBay_Utility.WinSize.y), medBay);
    }
}