namespace Ara3D.Studio.Samples.Generators;

/// <summary>
/// A lathe-style solid of revolution (vase / goblet). A 2D profile drawn in the XZ plane is
/// swept around the vertical (Z) axis. The profile radius interpolates from
/// <see cref="BaseRadius"/> at the bottom to <see cref="TopRadius"/> at the top, with a cosine
/// <see cref="WaistPinch"/> term that narrows the middle for a bulged/pinched silhouette.
/// <see cref="ProfileSteps"/> controls the vertical resolution of the profile; <see cref="Segments"/>
/// controls the number of revolution steps around the axis.
/// </summary>
[Category(Cat.Structures)]
[Description("A lathe-style solid of revolution (vase/goblet) formed by revolving a 2D profile around the vertical axis.")]
public class RevolveProfile : IGenerator
{
    [Range(3, 128)] public int Segments = 48;
    [Range(2, 64)] public int ProfileSteps = 16;
    [Range(0f, 5f)] public float BaseRadius = 1;
    [Range(0f, 5f)] public float TopRadius = 0.6f;
    [Range(0f, 10f)] public float Height = 2;
    [Range(0f, 1f)] public float WaistPinch = 0.3f;

    public Point3D ProfilePoint(Integer i)
    {
        var t = (int)i / (float)ProfileSteps;
        var radius = BaseRadius + (TopRadius - BaseRadius) * t - WaistPinch * MathF.Sin(t * MathF.PI);
        return (radius, 0, Height * t);
    }

    public QuadGrid3D Eval()
        => (ProfileSteps + 1).MapRange(ProfilePoint).Revolve(Vector3.UnitZ, Segments);
}

/// <summary>
/// Lofts between two circular rings of differing radius to make a smooth transition surface
/// (truncated cone, or a hyperboloid when twisted). The bottom ring uses <see cref="BottomRadius"/>,
/// the top ring <see cref="TopRadius"/>, separated by <see cref="Height"/>. <see cref="TwistDegrees"/>
/// rotates the top ring relative to the bottom, giving the ruled/hyperboloid look. <see cref="Sides"/>
/// sets the ring resolution and <see cref="Rows"/> the number of loft steps between the rings.
/// </summary>
[Category(Cat.Surfaces)]
[Description("A lofted transition surface between two circular rings (truncated cone / twisted hyperboloid).")]
public class LoftProfiles : IGenerator
{
    [Range(3, 128)] public int Sides = 48;
    [Range(0f, 5f)] public float BottomRadius = 1;
    [Range(0f, 5f)] public float TopRadius = 0.5f;
    [Range(0f, 10f)] public float Height = 2;
    [Range(-180f, 180f)] public float TwistDegrees = 0;
    [Range(2, 64)] public int Rows = 16;

    public Point3D PointAt(int i, int j)
    {
        var v = j / (float)Rows;
        var radius = BottomRadius + (TopRadius - BottomRadius) * v;
        var angle = i / (float)Sides * MathF.PI * 2 + v * (TwistDegrees * MathF.PI / 180);
        return (radius * MathF.Cos(angle), radius * MathF.Sin(angle), Height * v);
    }

    public QuadGrid3D Eval()
        => new QuadGrid3D(new FunctionalReadOnlyList2D<Point3D>(Sides + 1, Rows + 1, PointAt), false, false);
}
