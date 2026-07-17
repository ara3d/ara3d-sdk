public class SqueezeDeformer : IModifier
{
    [Range(0f, 1f)] public float Strength { get; set; } = 0.5f;
    [Range(0f, 10f)] public float Width { get; set; } = 1f;
    [Range(0, 2)] public int Axis = 2;

    public Point3D Deform(Point3D p, Bounds3D bounds)
    {
        var min = bounds.Min;
        var max = bounds.Max;
        var center = bounds.Center;

        var axisDir = Axis == 0 ? Vector3.UnitX : Axis == 1 ? Vector3.UnitY : Vector3.UnitZ;
        var axisMin = Axis == 0 ? min.X : Axis == 1 ? min.Y : min.Z;
        var axisMax = Axis == 0 ? max.X : Axis == 1 ? max.Y : max.Z;
        var axisCenter = Axis == 0 ? center.X : Axis == 1 ? center.Y : center.Z;

        var axisPos = Axis == 0 ? p.X : Axis == 1 ? p.Y : p.Z;
        var halfExtent = (axisMax - axisMin) * 0.5f;
        var normalized = (axisPos - axisCenter) / MathF.Max(halfExtent, 1e-6f);

        var falloff = MathF.Exp(-MathF.Abs(normalized) * Width);
        var squeezeAmount = Strength * falloff;

        var fromCenter = p - center;
        var axialComponent = Vector3.Dot(fromCenter, axisDir) * axisDir;
        var radialComponent = fromCenter - axialComponent;

        var newRadial = radialComponent * (1f - squeezeAmount);
        var result = center + axialComponent + newRadial;
        return (Point3D)result;
    }

    public TriangleMesh3D Eval(TriangleMesh3D mesh)
    {
        var bounds = mesh.DerivedBounds();
        return mesh.Deform(p => Deform(p, bounds));
    }
}