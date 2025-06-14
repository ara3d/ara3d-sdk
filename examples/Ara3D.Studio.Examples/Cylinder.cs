﻿using Ara3D.Domo;
using System.Numerics;

namespace Ara3D.Studio.Samples;

public class Cylinder : IModelGenerator
{
    public string Name => "Cylinder Demo";

    [Range(3, 100), Description("The number of sides of the cylinder.")]
    public int Sides { get; set; } = 32;

    [Range(0.01, 100), Description("The Radius of the cylinder.")]
    public float Radius { get; set; } = 0.5f;

    [Range(0.01, 100), Description("The height of the cylinder.")]
    public float Height { get; set; } = 2f;

    public bool Flip;

    public Model3D Eval(EvalContext context)
    {
        throw new NotImplementedException("TODO");
        /*
        var polygon = Polygons.RegularPolygon(Sides).Deform(p => p * Radius);
        var mesh = polygon.Extrude(Height).Triangulate();
        var mesh2 = Flip
            ? new TriangleMesh3D(mesh.Points, mesh.FaceIndices.Map(f => new Integer3(f.C, f.B, f.A)))
            : mesh;

        Debug.WriteLine($"# points {mesh2.Points.Count}");
        Debug.WriteLine($"# faces {mesh.FaceIndices.Count}");

        return Model3D.Create(mesh2);
        */
    }
}