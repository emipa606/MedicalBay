using RimWorld;
using Verse;

namespace LTF_MedBay;

public static class Tools
{
    public static bool TrueEverySec => TrueEveryNSec(60);

    public static bool TrueEvery3Sec => TrueEveryNSec(180);

    public static bool TrueEvery5Sec => TrueEveryNSec(300);

    public static void Warn(string warning, bool debug = false)
    {
        if (debug)
        {
            Log.Warning($"[MedBay]: {warning}");
        }
    }

    public static string DebugStatus(bool debug)
    {
        return $"{debug}->{!debug}";
    }

    private static bool TrueEveryNSec(int nTicks)
    {
        return Find.TickManager.TicksGame % nTicks == 0;
    }

    public static bool IsReserved(this Thing T, bool debug = false)
    {
        Warn($"IsReserved {T.def.defName} map is null", debug && T.Map == null);
        Warn($"IsReserved {T.def.defName} reservationManager is null",
            debug && T.Map?.physicalInteractionReservationManager == null);
        if (T.Map == null)
        {
            return true;
        }

        Warn($"{T.def.defName} is already reserved",
            T.Map.physicalInteractionReservationManager != null && debug &&
            T.Map.physicalInteractionReservationManager.IsReserved(new LocalTargetInfo(T)));
        return T.Map.physicalInteractionReservationManager != null &&
               T.Map.physicalInteractionReservationManager.IsReserved(new LocalTargetInfo(T));
    }

    public static float QualityFactor(QualityCategory qualityCategory)
    {
        return (int)qualityCategory / 6f;
    }

    public static string DescriptionAttr<T>(this T source)
    {
        var array = (DescriptionAttribute[])source.GetType().GetField(source.ToString())
            .GetCustomAttributes(typeof(DescriptionAttribute), false);
        return array.Length != 0 ? array[0].description : source.ToString();
    }
}