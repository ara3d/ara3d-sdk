
namespace Ara3D.Studio.Samples;

public class SweepDemo : IModelGenerator
{
    [Range(1, 100)] public int SampleCount = 16;
    [Range(-10, 10)] public float Height = 3;
    [Range(-10, 10)] public float Revolutions = 3;

    public static Matrix4x4 LookTowards(Vector3 pos, Vector3 dir)
        => Matrix4x4.CreateLookAt(pos, pos + dir, Vector3.UnitZ);

    // NOTE: I need more casts 
    public IReadOnlyList<Transform3D> GetTransforms(Curve3D curve, int count)
    {
        // NOTE: I need more conversions (Point3D to Translation, and IRotation3D to Rotation3D)
        // Below the whole Vector3 conversion isn't great .
        var list = new List<Transform3D>();
        for (var i = 0; i <= count; i++)
        {
            var t0 = (float)i / (count);
            var t1 = t0 + 0.001f;
            var pos0 = curve.Eval(t0);
            var pos1 = curve.Eval(t1);
            var dir = pos1 - pos0;
            var pose = LookTowards(pos0, dir);
            list.Add(pose);
        }

        return list;
    }

    public static Angle QuarterTurn = 0.25f.Turns();

    public Model3D Eval(EvalContext context)
    {
        var profile = Curves.Circle.RotateX(QuarterTurn);
        var path = Curves.Helix(Height, Revolutions);
        var profilePoints = profile.Sample(SampleCount);
        var pathFrames = GetTransforms(path, SampleCount);
        var grid = profilePoints.Sweep(pathFrames, true, false);
        return grid.Triangulate();
    }
}
