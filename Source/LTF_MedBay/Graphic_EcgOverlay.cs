using System;
using UnityEngine;
using Verse;

namespace LTF_MedBay;

public class Graphic_EcgOverlay : Graphic_Collection
{
    private const int BaseTicksPerFrameChange = 7;

    private const int ExtraTicksPerFrameChange = 5;

    private const float MaxOffset = 0.05f;

    public override Material MatSingle
    {
        get
        {
            var num = (int)Math.Floor(Find.TickManager.TicksGame / (float)BaseTicksPerFrameChange) % subGraphics.Length;
            return subGraphics[num].MatSingle;
        }
    }

    public override void DrawWorker(Vector3 loc, Rot4 rot, ThingDef thingDef, Thing thing, float extraRotation)
    {
        if (thingDef == null)
        {
            Log.ErrorOnce($"Graphic_Animation DrawWorker with null thingDef: {loc}", 3427324);
            return;
        }

        if (subGraphics == null)
        {
            Log.ErrorOnce($"Graphic_Animation has no subgraphics {thingDef}", 358773632);
            return;
        }

        var vector2 = new Vector2(1f, 1f);
        var num = 4f;
        var drawPos = thing.DrawPos;
        extraRotation = 0f;
        if (thing.Rotation == Rot4.South)
        {
            extraRotation = 45f;
        }

        var num2 = 0.57f;
        var num3 = 0.4f;
        drawPos.x += 0.52f;
        drawPos.z += 0.52f;
        var s = new Vector3(vector2.x * num2, 1f, vector2.y * num3);
        var matrix = default(Matrix4x4);
        drawPos.y += num;
        matrix.SetTRS(drawPos, Quaternion.AngleAxis(extraRotation, Vector3.up), s);
        Graphics.DrawMesh(MeshPool.plane14, matrix, MatSingle, (int)num);
    }
}