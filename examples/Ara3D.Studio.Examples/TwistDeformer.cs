namespace Ara3D.Studio.Samples;

public class TwistDeformer : IModelModifier
{
    [Range(-10f, 10f)] public float Revolutions { get; set; }
    [Range(0, 2)] public int Axis = 2;

    public Vector3 AxisVector => Vector3.Zero.WithComponent(Axis, 1);

    public Point3D Deform(Point3D p, Bounds3D bounds)
    {
        var v = p.InverseLerp(bounds);
        var amount = v[Axis];
        var axisAngle = new AxisAngle(AxisVector, amount.Turns * Revolutions);
        return p.Transform(axisAngle);
    }

    public TriangleMesh3D Deform(TriangleMesh3D mesh)
    {
        var bounds = mesh.Bounds;
        return mesh.Deform(p => Deform(p, bounds));
    }

    public Model3D Eval(Model3D model, EvalContext context)
        => model.ModifyMeshes(Deform);
}