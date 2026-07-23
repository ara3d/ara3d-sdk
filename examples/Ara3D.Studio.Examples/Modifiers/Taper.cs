namespace Ara3D.Studio.Samples.Modifiers;

[Category(Cat.Deform)]
[Description("Tapers the mesh, scaling the cross-section perpendicular to an axis by a factor that grows linearly from base to tip.")]
public class Taper : IModifier
{
    [Range(-1f, 3f)] public float Amount { get; set; }
    [Range(0, 2)] public int Axis = 2;

    public Point3D Deform(Point3D p, Bounds3D bounds)
    {
        var center = bounds.Center;
        var t = p.InverseLerp(bounds)[Axis];
        var scale = 1 + Amount * t;

        var result = p.Vector3;
        for (var c = 0; c < 3; c++)
        {
            if (c == Axis) continue;
            var newC = center[c] + (p[c] - center[c]) * scale;
            result = result.WithComponent(c, newC);
        }
        return result;
    }

    public TriangleMesh3D Eval(TriangleMesh3D mesh)
    {
        var bounds = mesh.DerivedBounds();
        return mesh.Deform(p => Deform(p, bounds));
    }
}
