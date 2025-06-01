using System;
using System.Linq;
using Ara3D.Models;
using Ara3D.SceneEval;
using Ara3D.Studio.API;
using Plato;
using Plato.Geometry;

namespace Ara3D.Studio.Samples;

public class ToTriangles : IModelModifier
{
    public static TriangleMesh3D ToMesh(Triangle3D t)
        => new(t.Points, new Integer3[] { (0, 1, 2) }.ToIArray());

    public static Model3DNode ToModelNode(Triangle3D t, Material mat)
        => new(Guid.NewGuid(), "", ToMesh(t), Matrix4x4.Identity, mat);

    public Model3D Eval(Model3D model, EvalContext context)
    {
        if (model.Nodes.Count == 0) return model;
        var mesh = model.ToMesh();
        var triangles = mesh.Triangles;
        var mat = model.Nodes[0].Material;
        return triangles.Select(tri => ToModelNode(tri, mat)).ToList();
    }
}