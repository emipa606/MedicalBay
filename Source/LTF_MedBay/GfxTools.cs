using System;
using UnityEngine;
using Verse;

namespace LTF_MedBay;

[StaticConstructorOnStartup]
public class GfxTools
{
    public static void DrawDot(Vector3 benchPos, Material dotM, Mesh mesh, float x, float z, Thing bench, bool pulse)
    {
        var pos = benchPos;
        pos.x += x;
        pos.z += z;
        var material = dotM;
        if (pulse)
        {
            var num = ((float)Math.Sin((Time.realtimeSinceStartup + (397f * (bench.thingIDNumber % 571))) * 4f) + 1f) *
                      0.5f;
            num = 0.3f + (num * 0.7f);
            material = FadedMaterialPool.FadedVersionOf(dotM, num);
        }

        var s = new Vector3(0.32f, 1f, 0.29f);
        var matrix = default(Matrix4x4);
        matrix.SetTRS(pos, Quaternion.AngleAxis(0f, Vector3.up), s);
        Graphics.DrawMesh(mesh, matrix, material, 0);
    }

    public static void DrawDot(Vector3 benchPos, Vector3 dotS, Matrix4x4 matrix, Material dotM, float x, float z)
    {
        var pos = benchPos;
        pos.x += x;
        pos.z += z;
        matrix.SetTRS(pos, Quaternion.AngleAxis(0f, Vector3.up), dotS);
        if (MeshPool.plane14 == null)
        {
            Tools.Warn("14 null", true);
        }
        else
        {
            Graphics.DrawMesh(MeshPool.plane14, matrix, dotM, 0);
        }
    }

    public static void DrawDot(Vector3 benchPos, Material dotGfx, Mesh mesh, Thing bench, float x, float z,
        bool pulse = false, float width = 1f, float height = 1f)
    {
        var pos = benchPos;
        pos.x += x;
        pos.z += z;
        var material = dotGfx;
        if (pulse)
        {
            var alpha = PulseOpacity(bench);
            material = FadedMaterialPool.FadedVersionOf(dotGfx, alpha);
        }

        var s = new Vector3(width, 1f, height);
        var matrix = default(Matrix4x4);
        matrix.SetTRS(pos, Quaternion.AngleAxis(0f, Vector3.up), s);
        Graphics.DrawMesh(mesh, matrix, material, 0);
    }

    public static void DrawBenchSize(Vector3 benchPos, Material dotGfx, Mesh mesh, Thing bench)
    {
        DrawDot(benchPos, dotGfx, mesh, bench, 0f, 0f, false, bench.def.size.x, bench.def.size.z);
    }

    public static void DrawBarResized(Vector3 benchPos, Mesh mesh, int i)
    {
        var num = 0f;
        Material material;
        switch (i)
        {
            case < 8:
                material = Gfx.WorkBarS;
                break;
            case < 21:
                material = Gfx.WorkBarM;
                num += 0.001f;
                break;
            default:
                material = Gfx.WorkBarL;
                num += 0.003f;
                break;
        }

        var x = -1.29f + (i * 0.0346f);
        var z = 1.38f + num;
        FlickerBar(benchPos, material, mesh, x, z);
    }

    public static void DrawBar(Vector3 benchPos, Mesh mesh, float x, float z, int i)
    {
        var num = 0f;
        Material material;
        switch (i)
        {
            case < 8:
                material = Gfx.WorkBarS;
                break;
            case < 21:
                material = Gfx.WorkBarM;
                num += 0.013f;
                break;
            default:
                material = Gfx.WorkBarL;
                num += 0.02f;
                break;
        }

        var x2 = -1.145f + (i * 0.0825f);
        var z2 = 0.562f + num;
        FlickerBar(benchPos, material, mesh, x2, z2);
    }

    public static void FlickerBar(Vector3 benchPos, Material dotM, Mesh mesh, float x, float z)
    {
        var pos = benchPos;
        pos.x += x;
        pos.z += z;
        var material = dotM;
        if (Rand.Chance(0.85f))
        {
            material = FadedMaterialPool.FadedVersionOf(dotM, 0.65f);
        }

        var s = new Vector3(0.15f, 1f, 0.09f);
        var matrix = default(Matrix4x4);
        matrix.SetTRS(pos, Quaternion.AngleAxis(0f, Vector3.up), s);
        Graphics.DrawMesh(mesh, matrix, material, 0);
    }

    public static float PulseOpacity(Thing thing)
    {
        var num = ((float)Math.Sin((Time.realtimeSinceStartup + (397f * (thing.thingIDNumber % 571))) * 4f) + 1f) *
                  0.5f;
        return 0.3f + (num * 0.7f);
    }

    public static void TendingFillBar(TendingRequest TR)
    {
        var r = default(GenDraw.FillableBarRequest);
        r.center = TR.WorkRequest.Patient.DrawPos;
        r.center.z += 0.6f;
        r.size = Gfx.BarSize;
        r.fillPercent = TR.WorkRequest.ProgressRatio;
        r.filledMat = Gfx.TRBarFilledMat;
        r.unfilledMat = Gfx.BarUnfilledMat;
        r.margin = 0.15f;
        r.rotation = Rot4.North;
        GenDraw.DrawFillableBar(r);
    }
}