﻿namespace Ara3D.Studio.Samples;

public class Clone : IModelModifier
{
    public bool AtFaceCenters;

    public static Element ToElement(TriangleMesh3D mesh, Point3D position, Material mat)
        => new(mesh, mat, Matrix4x4.CreateTranslation(position)); 

    public Model3D Eval(Model3D m, EvalContext eval)
    {
        var mat = m.Materials.FirstOrDefault() ?? Material.Default;
        var instancedMesh = PlatonicSolids.TriangulatedCube;
        var mergedMesh = m.ToMesh();
        var points = AtFaceCenters ? mergedMesh.Triangles.Map(f => f.Center) : mergedMesh.Points;
        return Model3D.Create(points.Select(p => ToElement(instancedMesh, p, mat)));
    }

}