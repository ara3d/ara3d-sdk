namespace Ara3D.Studio.Samples.Modifiers;

[Category(Cat.Deform)]
[Description("Bends the mesh into an arc, rotating points progressively about an axis based on their position along another axis.")]
public class Bend : IModifier
{
    [Range(-360f, 360f)] public float Angle { get; set; }
    [Range(0, 2)] public int AlongAxis = 2;
    [Range(0, 2)] public int BendAxis = 0;

    public Vector3 BendVector => Vector3.Zero.WithComponent(BendAxis, 1);

    public Point3D Deform(Point3D p, Bounds3D bounds)
    {
        var t = p.InverseLerp(bounds)[AlongAxis] - 0.5f;
        var a = t * Angle;
        var center = bounds.Center.Vector3;
        var axisAngle = new AxisAngle(BendVector, a.Degrees());
        return p.Translate(-center).Transform(axisAngle).Translate(center);
    }

    public TriangleMesh3D Eval(TriangleMesh3D mesh)
    {
        var bounds = mesh.DerivedBounds();
        return mesh.Deform(p => Deform(p, bounds));
    }
}
