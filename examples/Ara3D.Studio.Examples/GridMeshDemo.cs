using System;
using System.ComponentModel.DataAnnotations;
using Ara3D.Collections;
using Ara3D.Models;
using Ara3D.SceneEval;
using Ara3D.Studio.API;
using Ara3D.Geometry;

namespace Ara3D.Studio.Samples;

public class GridMeshDemo : IModelGenerator
{
    [Range(1, 100)] public int Rows = 1;
    [Range(1, 100)] public int Columns = 1;
    [Range(0f, 10f)] public float Scale = 1f;
    [Range(-360f, 360f)] public float XRotation;
    [Range(-360f, 360f)] public float YRotation;
    [Range(-360f, 360f)] public float ZRotation;
    
    public static IReadOnlyList2D<T> ToArray2D<T>(T[] xs, int rows)
    {
        var cols = xs.Length / rows;
        if (xs.Length % cols != 0) throw new Exception($"Number of values {xs.Length} not divisible by {rows}");
        return new FunctionalReadOnlyList2D<T>(cols, rows, (col, row) => xs[row * cols + col]);
    }

    public Model3D Eval(EvalContext eval)
    {
        var x00 = new Point3D(-0.5f, -0.5f, 0);
        var x01 = new Point3D(-0.5f, +0.5f, 0);
        var x10 = new Point3D(+0.5f, -0.5f, 0);
        var x11 = new Point3D(+0.5f, +0.5f, 0);
        var points = ToArray2D([x00, x01, x10, x11], 2);
        var grid = new GridMesh(points, false, false);
        var mesh = grid.Triangulate();
        return mesh;
    }
}