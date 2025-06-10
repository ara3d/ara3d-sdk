using Ara3D.Models;

namespace Ara3D.Studio.Samples;

public class ToTriangles : IModelModifier
{
    public static TriangleMesh3D ToMesh(Triangle3D t)
        => new(t.Points, [new Integer3(0, 1, 2)]);

    public Model3D Eval(Model3D model, EvalContext context)
    {
        if (model.Elements.Count == 0) return model;
        var mesh = model.ToMesh();
        
        var triangles = mesh.Triangles;
        var meshes = triangles.Map(ToMesh);
        var offsets = meshes.Map(m => m.Bounds.Center.Vector3);
        meshes = meshes.Zip(offsets, (mesh, offset) => mesh.Translate(-offset));
        var transforms = offsets.Map(Matrix4x4.CreateTranslation);
        var elements = meshes.MapIndices(i => new ElementStruct(i, 0, i, i));

        return new Model3D(meshes, model.Materials, transforms, elements, null);
    }
}