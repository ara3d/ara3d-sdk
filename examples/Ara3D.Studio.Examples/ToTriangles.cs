namespace Ara3D.Studio.Samples;

public class ToTriangles : IModelModifier
{
    public static TriangleMesh3D ToMesh(Triangle3D t)
        => new(t.Points, [new Integer3(0, 1, 2)]);

    public static Element ToElement(Triangle3D t, Material mat)
        => new(ToMesh(t), mat, Matrix4x4.Identity);

    public Model3D Eval(Model3D model, EvalContext context)
    {
        if (model.Elements.Count == 0) return model;
        var mesh = model.ToMesh();
        var triangles = mesh.Triangles;
        var mat = model.Elements[0].Material;
        return Model3D.Create(triangles.Select(tri => ToElement(tri, mat)).ToList());
    }
}