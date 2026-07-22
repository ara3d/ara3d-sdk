namespace Ara3D.Studio.Samples.Demos;

[Category(Cat.ExperimentalDemos)]
public class FacadeFromLines : IModifier
{
    [Range(0f, 10f)] public float Width = 3f;
    [Range(0f, 10f)] public float HeightPerSegment = 4f;
    [Range(1, 50)] public int HeightSegments = 4;

    [Range(0, 5f)] public float MinWindowSpacing = 1f;

    [Range(0, 5f)] public float WindowWidth = 1f;
    [Range(0, 5f)] public float WindowHeight = 2f;
    [Range(0, 5f)] public float WindowDistanceFromFloor = 0.8f;

    [Range(0, 1)] public float WindowInset = 0.2f;
    [Range(1, 5)] public int WindowWidthSegments = 2;
    [Range(1, 5)] public int WindowHeightSegments = 2;
    [Range(0f, 1f)] public float MullionWidth = 0.2f;
    [Range(0f, 1f)] public float PanelInset = 0.05f;

    public float TotalHeight => HeightPerSegment * HeightSegments;

    public Vector3 HeightVector => TotalHeight * Vector3.UnitZ;

    public Quad3D LineToQuad(Line3D line)
        => (line.A, line.B, line.B + HeightVector, line.A + HeightVector);
    
    public Model3D Eval(LineMesh3D mesh, EvalContext context)
    {
        var gen = new FacadeGenerator
        {
            Width = 1,
            HeightSegments = HeightSegments,
            WindowWidth = WindowWidth,
            WindowHeight = WindowHeight,
            WindowDistanceFromFloor = WindowDistanceFromFloor,
            WindowWidthSegments = WindowWidthSegments,
            WindowHeightSegments = WindowHeightSegments,
            HeightPerSegment = HeightPerSegment,
            MullionWidth = MullionWidth,
            PanelInset = PanelInset,
            MinWindowSpacing = MinWindowSpacing,
        };

        var builder = new Model3DBuilder();
        foreach (var line in mesh.Lines)
        {
            var quad = LineToQuad(line);
            gen.BuildFacade(quad, builder);
        }

        return builder.Build();
    }
}