namespace Ara3D.Studio.Samples;

[Category(Cat.ExperimentalDemos)]
[Description("A surface produced by sweeping a profile along a helical path, demonstrating the sweep operation.")]
public class SweepDemo : IGenerator
{
    [Range(1, 100)] public int SampleCount = 16;
    [Range(-10, 10)] public float Height = 3;
    [Range(-10, 10)] public float Revolutions = 3;

    public static Angle QuarterTurn = 0.25f.Turns();

    public QuadGrid3D Eval()
    {
        var profile = Curves.Circle.RotateX(QuarterTurn);
        var path = Curves.Helix(Height, Revolutions);
        var profilePoints = profile.Sample(SampleCount);
        var pathFrames = path.GetTransforms(SampleCount);
        return profilePoints.Sweep(pathFrames, true, false);
    }
}
