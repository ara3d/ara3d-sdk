namespace Ara3D.Studio.Samples;

public class SkewDeformer : IModelModifier
{
    [Range(-5f, 5f)] public float X { get; set; }
    [Range(-5f, 5f)] public float Y { get; set; }
    [Range(-5f, 5f)] public float Z { get; set; }

    [Range(0, 2)] public int Axis = 2;
    public bool Flip;

    public Vector3 MaxTranslation => (X, Y, Z);

    public Point3D Deform(Point3D p, Bounds3D bounds)
    {
        var v = p.InverseLerp(bounds);
        var amount = v[Axis];
        if (Flip) amount = 1f - amount;
        var translation = Vector3.Zero.Lerp(MaxTranslation, amount);
        return p.Translate(translation);
    }

    public TriangleMesh3D Deform(TriangleMesh3D mesh)
    {
        var bounds = mesh.Bounds;
        return mesh.Deform(p => Deform(p, bounds));
    }

    public Model3D Eval(Model3D model, EvalContext context)
        => model.ModifyMeshes(Deform);
}