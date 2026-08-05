using Ara3D.IfcLoader;

namespace Ara3D.DoorClearance.Tests;

public readonly record struct Vec3(double X, double Y, double Z)
{
    public static readonly Vec3 Zero = new(0, 0, 0);
    public static readonly Vec3 UnitX = new(1, 0, 0);
    public static readonly Vec3 UnitZ = new(0, 0, 1);

    public static Vec3 operator +(Vec3 a, Vec3 b)
        => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);

    public static Vec3 operator -(Vec3 a, Vec3 b)
        => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);

    public static Vec3 operator *(Vec3 a, double s)
        => new(a.X * s, a.Y * s, a.Z * s);

    public double Dot(Vec3 b)
        => X * b.X + Y * b.Y + Z * b.Z;

    public Vec3 Cross(Vec3 b)
        => new(Y * b.Z - Z * b.Y, Z * b.X - X * b.Z, X * b.Y - Y * b.X);

    public double Length
        => Math.Sqrt(Dot(this));

    public Vec3 Normalized
        => Length > 1e-12 ? this * (1.0 / Length) : this;
}

/// <summary>A rigid transform as three world-space basis vectors plus a translation.</summary>
public readonly record struct Pose(Vec3 X, Vec3 Y, Vec3 Z, Vec3 T)
{
    public static readonly Pose Identity = new(Vec3.UnitX, new Vec3(0, 1, 0), Vec3.UnitZ, Vec3.Zero);

    public Vec3 Apply(Vec3 p)
        => T + X * p.X + Y * p.Y + Z * p.Z;

    public Vec3 Rotate(Vec3 v)
        => X * v.X + Y * v.Y + Z * v.Z;

    /// <summary>This pose expressed under <paramref name="parent"/>.</summary>
    public Pose Under(Pose parent)
        => new(parent.Rotate(X), parent.Rotate(Y), parent.Rotate(Z), parent.Apply(T));
}

/// <summary>
/// Resolves an element's world position by walking its IFCLOCALPLACEMENT chain through
/// IFCAXIS2PLACEMENT3D / IFCCARTESIANPOINT entities parsed straight from STEP. No mesh geometry
/// is loaded; rotations are composed exactly, so positions are correct even under rotated parents.
/// </summary>
public static class StepPlacement
{
    private const int MaxDepth = 64;

    public static Vec3? WorldPosition(IfcEntityResolver resolver, int localPlacementId)
        => WorldPose(resolver, localPlacementId)?.T;

    public static Pose? WorldPose(IfcEntityResolver resolver, int localPlacementId, int depth = 0)
    {
        if (depth > MaxDepth)
            return null;
        var e = resolver.GetEntityOrDefault(localPlacementId);
        if (e == null || !string.Equals(e.GetEntityName(), "IFCLOCALPLACEMENT", StringComparison.OrdinalIgnoreCase))
            return null;

        var local = AxisPlacement(resolver, e.GetId(1)) ?? Pose.Identity;
        if (e.GetValue(0).IsUnassignedOrRedeclared)
            return local;
        var parent = WorldPose(resolver, e.GetId(0), depth + 1);
        return parent == null ? local : local.Under(parent.Value);
    }

    private static Pose? AxisPlacement(IfcEntityResolver resolver, int id)
    {
        var e = resolver.GetEntityOrDefault(id);
        if (e == null || !string.Equals(e.GetEntityName(), "IFCAXIS2PLACEMENT3D", StringComparison.OrdinalIgnoreCase))
            return null;

        var t = Coords(resolver, e, 0) ?? Vec3.Zero;
        var z = (Coords(resolver, e, 1) ?? Vec3.UnitZ).Normalized;
        var xRef = Coords(resolver, e, 2) ?? Vec3.UnitX;
        var x = (xRef - z * xRef.Dot(z)).Normalized;
        if (x.Length < 0.5)
            x = OrthogonalTo(z);
        return new Pose(x, z.Cross(x), z, t);
    }

    private static Vec3 OrthogonalTo(Vec3 z)
        => (Math.Abs(z.Z) < 0.9 ? Vec3.UnitZ.Cross(z) : Vec3.UnitX.Cross(z)).Normalized;

    /// <summary>Reads an IFCCARTESIANPOINT or IFCDIRECTION referenced by attribute <paramref name="index"/>.</summary>
    private static Vec3? Coords(IfcEntityResolver resolver, IfcEntity entity, int index)
    {
        if (entity.GetValue(index).IsUnassignedOrRedeclared)
            return null;
        var e = resolver.GetEntityOrDefault(entity.GetId(index));
        if (e == null)
            return null;
        var values = e.GetNumberList(0);
        return values.Length switch
        {
            2 => new Vec3(values[0], values[1], 0),
            3 => new Vec3(values[0], values[1], values[2]),
            _ => null,
        };
    }
}
