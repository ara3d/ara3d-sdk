namespace Ara3D.Studio.Samples.Modifiers;

[Category(Cat.Deform)]
public class TwistDeformer : IModifier
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

    public TriangleMesh3D Eval(TriangleMesh3D mesh)
    {
        var bounds = mesh.DerivedBounds();
        return mesh.Deform(p => Deform(p, bounds));
    }
}

[Category(Cat.Deform)]
public class SkewDeformer : IModifier
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

    public TriangleMesh3D Eval(TriangleMesh3D mesh)
    {
        var bounds = mesh.DerivedBounds();
        return mesh.Deform(p => Deform(p, bounds));
    }
}

[Category(Cat.Deform)]
public class Spherify : IModifier
{
    [Range(0f, 10f)] public float Radius { get; set; }
    [Range(0f, 1f)] public float Strength { get; set; }

    public Point3D Deform(Point3D p, Bounds3D bounds)
    {
        var center = bounds.Center;
        var v = p - center;
        var dir = v.LengthSquared >= 0.001 ? v.Normalize : Vector3.UnitZ;
        var target = p + dir * Radius;
        return p.Lerp(target, Strength);
    }

    public TriangleMesh3D Eval(TriangleMesh3D mesh)
    {
        var bounds = mesh.DerivedBounds();
        return mesh.Deform(p => Deform(p, bounds));
    }
}

[Category(Cat.Deform)]
public class Cubify: IModifier
{
    [Range(0f, 10f)] public float Radius { get; set; }
    [Range(0f, 1f)] public float Strength { get; set; }

    public Point3D Deform(Point3D p, Bounds3D newBox)
    {
        var v = p - newBox.Center;
        var dir = v.LengthSquared >= 0.001 ? v.Normalize : Vector3.UnitZ;
        var target = p + dir * Radius;
        target = newBox.Clamp(target);
        return p.Lerp(target, Strength);
    }

    public TriangleMesh3D Eval(TriangleMesh3D mesh)
    {
        var bounds = mesh.DerivedBounds();
        var min = bounds.Center - new Vector3(Radius, Radius, Radius);
        var max = bounds.Center + new Vector3(Radius, Radius, Radius);
        return mesh.Deform(p => Deform(p, (min, max)));
    }
}

public class Push : IModifier
{
    [Range(0f, 10f)] public float Distance { get; set; }

    public TriangleMesh3D Eval(TriangleMesh3D mesh)
    {
        var normals = mesh.DerivedAttributes().Vertices.AngleWeightedNormals;
        return mesh.WithPoints(mesh.Points.Zip(normals, (p, n) => p + n * Distance));
    }
}

public class NoiseDeformer : IModifier
{
    [Range(0f, 1f)] public float Amplitude { get; set; } = 1f;
    [Range(1, 10)] public int Exp10 { get; set; } = 1;

    [Range(-10f, 10f)] public float OffsetX { get; set; }
    [Range(-10f, 10f)] public float OffsetY { get; set; }
    [Range(-10f, 10f)] public float OffsetZ { get; set; }

    public Point3D Deform(Point3D p)
    {
        var offset = (OffsetX, OffsetY, OffsetZ);

        // Frequency: 10, 100, 1000, etc.
        var frequency = MathF.Pow(10f, Exp10);

        // Sample position in noise space
        var sample = p.Vector3 * frequency + offset;

        // Expected approximately [-1, +1]
        var n = PerlinNoise.Noise(sample);

        // Displace along a stable direction.
        // Replace Vector3.UnitZ with p.Normal if you later have normals available.
        var displaced = p.Vector3 + Vector3.UnitZ * (n * Amplitude);

        return displaced;
    }

    public TriangleMesh3D Eval(TriangleMesh3D mesh)
    {
        return mesh.Deform(Deform);
    }
}
