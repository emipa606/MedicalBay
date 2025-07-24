using UnityEngine;
using Verse;

namespace LTF_MedBay;

[StaticConstructorOnStartup]
public class Gfx
{
    public enum Layer
    {
        over = 4,
        under = -1
    }

    public static Mesh DotsMesh = MeshPool.plane14;

    public static readonly Mesh WorkMesh = MeshPool.plane10;

    private static readonly string BuildingPath = "Things/Building/";

    private static readonly string MedBayPath = $"{BuildingPath}MedBay/";

    private static readonly string overlayDir = $"{MedBayPath}Overlay/";

    public static readonly Vector2 BarSize = new(0.6f, 0.2f);

    public static readonly Material TRBarFilledMat =
        SolidColorMaterials.SimpleSolidColorMaterial(new Color(0.43f, 0.78f, 0.84f));

    public static readonly Material BarUnfilledMat =
        SolidColorMaterials.SimpleSolidColorMaterial(new Color(0.3f, 0.3f, 0.3f));

    public static readonly Material MedBayScreenActive =
        MaterialPool.MatFrom($"{overlayDir}ScreenMedBayActive", ShaderDatabase.Transparent);

    public static readonly Material MedBayScreenStandBy =
        MaterialPool.MatFrom($"{overlayDir}ScreenMedBayStandBy", ShaderDatabase.Transparent);

    public static readonly Material
        WorkBarS = MaterialPool.MatFrom($"{overlayDir}WorkBarS", ShaderDatabase.Transparent);

    public static readonly Material
        WorkBarM = MaterialPool.MatFrom($"{overlayDir}WorkBarM", ShaderDatabase.Transparent);

    public static readonly Material
        WorkBarL = MaterialPool.MatFrom($"{overlayDir}WorkBarL", ShaderDatabase.Transparent);

    public static readonly int WorkBarNum = 27;

    public static readonly Graphic EcgAnimGfx = GraphicDatabase.Get<Graphic_EcgOverlay>($"{overlayDir}Anim/ecg",
        ShaderDatabase.TransparentPostLight, Vector2.one, Color.white);

    private static readonly string GizmoPath = "UI/Commands/";

    private static readonly string AutoPath = $"{GizmoPath}Auto/";

    private static readonly string ManualPath = $"{GizmoPath}Manual/";

    private static readonly string WaitingRoomPath = $"{GizmoPath}WaitingRoom/";

    private static readonly string FactionPath = $"{WaitingRoomPath}Faction/";

    private static readonly string RacePath = $"{WaitingRoomPath}Race/";

    public static readonly Texture2D ITabMat_Hidden = ContentFinder<Texture2D>.Get($"{GizmoPath}HiddenLeft");

    public static readonly Texture2D ITabMat_Expanded = ContentFinder<Texture2D>.Get($"{GizmoPath}ExpandedLeft");

    public static readonly Texture2D IconWaitingLog = ContentFinder<Texture2D>.Get($"{GizmoPath}WaitingRoomLog");

    public static readonly Texture2D IconWaitingParams = ContentFinder<Texture2D>.Get($"{GizmoPath}WaitingRoomParams");

    public static readonly Texture2D IconDebug = ContentFinder<Texture2D>.Get($"{GizmoPath}Debug");

    public static readonly Texture2D IconStartRegen = ContentFinder<Texture2D>.Get($"{ManualPath}StartRegen");

    public static readonly Texture2D IconStartTend = ContentFinder<Texture2D>.Get($"{ManualPath}StartTend");

    public static readonly Texture2D IconStartAll = ContentFinder<Texture2D>.Get($"{ManualPath}StartAll");

    public static readonly Texture2D IconRegenOn = ContentFinder<Texture2D>.Get($"{AutoPath}RegenOn");

    public static readonly Texture2D IconRegenOff = ContentFinder<Texture2D>.Get($"{AutoPath}RegenOff");

    public static readonly Texture2D IconTendingOff = ContentFinder<Texture2D>.Get($"{AutoPath}TendingOff");

    public static readonly Texture2D IconTendingOn = ContentFinder<Texture2D>.Get($"{AutoPath}TendingOn");

    public static readonly Texture2D IconAutoOff = ContentFinder<Texture2D>.Get($"{AutoPath}AutomaticOff");

    public static readonly Texture2D IconAutoOn = ContentFinder<Texture2D>.Get($"{AutoPath}AutomaticOn");

    public static readonly Texture2D
        IconFactionParams = ContentFinder<Texture2D>.Get($"{WaitingRoomPath}FactionParams");

    public static readonly Texture2D IconKindParams = ContentFinder<Texture2D>.Get($"{WaitingRoomPath}RaceParams");

    public static readonly Texture2D IconAutoParams = ContentFinder<Texture2D>.Get($"{WaitingRoomPath}AutomaticParams");

    public static readonly Texture2D IconAlienOff = ContentFinder<Texture2D>.Get($"{RacePath}AlienOff");

    public static readonly Texture2D IconAlienOn = ContentFinder<Texture2D>.Get($"{RacePath}AlienOn");

    public static readonly Texture2D IconAnimalOff = ContentFinder<Texture2D>.Get($"{RacePath}AnimalOff");

    public static readonly Texture2D IconAnimalOn = ContentFinder<Texture2D>.Get($"{RacePath}AnimalOn");

    public static readonly Texture2D IconHumanOff = ContentFinder<Texture2D>.Get($"{RacePath}HumanOff");

    public static readonly Texture2D IconHumanOn = ContentFinder<Texture2D>.Get($"{RacePath}HumanOn");

    public static readonly Texture2D IconMechanoidOff = ContentFinder<Texture2D>.Get($"{RacePath}MechanoidOff");

    public static readonly Texture2D IconMechanoidOn = ContentFinder<Texture2D>.Get($"{RacePath}MechanoidOn");

    public static readonly Texture2D IconKindEverybody = ContentFinder<Texture2D>.Get($"{RacePath}KindEverybody");

    public static readonly Texture2D IconKindNobody = ContentFinder<Texture2D>.Get($"{RacePath}KindNobody");

    public static readonly Texture2D IconAllyOff = ContentFinder<Texture2D>.Get($"{FactionPath}AllyOff");

    public static readonly Texture2D IconAllyOn = ContentFinder<Texture2D>.Get($"{FactionPath}AllyOn");

    public static readonly Texture2D IconEnemyOff = ContentFinder<Texture2D>.Get($"{FactionPath}EnemyOff");

    public static readonly Texture2D IconEnemyOn = ContentFinder<Texture2D>.Get($"{FactionPath}EnemyOn");

    public static readonly Texture2D IconNoFactionOff = ContentFinder<Texture2D>.Get($"{FactionPath}NoFactionOff");

    public static readonly Texture2D IconNoFactionOn = ContentFinder<Texture2D>.Get($"{FactionPath}NoFactionOn");

    public static readonly Texture2D IconPlayerOff = ContentFinder<Texture2D>.Get($"{FactionPath}PlayerOff");

    public static readonly Texture2D IconPlayerOn = ContentFinder<Texture2D>.Get($"{FactionPath}PlayerOn");

    public static readonly Texture2D IconFactionEverybody =
        ContentFinder<Texture2D>.Get($"{FactionPath}FactionEverybody");

    public static readonly Texture2D IconFactionNobody = ContentFinder<Texture2D>.Get($"{FactionPath}FactionNobody");
}