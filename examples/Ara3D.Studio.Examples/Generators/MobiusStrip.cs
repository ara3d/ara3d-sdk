using System;
using Ara3D;
using Ara3D.Geometry;
using Ara3D.Studio.API;
using Ara3D.Collections;
using System.ComponentModel.DataAnnotations;

public class MobiusStrip : IGenerator
{
    [Range(0.01f, 10f)] public float Radius = 2f;
    [Range(0.01f, 5f)] public float Width = 0.5f;

    [Range(3, 512)] public int NumColumns = 128;
    [Range(2, 128)] public int NumRows = 16;

    public Point3D PointOnMobius(int i, int j)
    {
        var u = i / (float)(NumColumns - 1) * MathF.Tau;
        var v = (j / (float)(NumRows - 1) - 0.5f) * Width;

        var halfU = u * 0.5f;
        var cosHalf = MathF.Cos(halfU);
        var sinHalf = MathF.Sin(halfU);
        var cosU = MathF.Cos(u);
        var sinU = MathF.Sin(u);

        var x = (Radius + v * cosHalf) * cosU;
        var y = (Radius + v * cosHalf) * sinU;
        var z = v * sinHalf;

        return new Point3D(x, y, z);
    }

    public QuadGrid3D Eval()
    {
        var points = new FunctionalReadOnlyList2D<Point3D>(NumColumns, NumRows, PointOnMobius);
        return new QuadGrid3D(points, true, false);
    }
}