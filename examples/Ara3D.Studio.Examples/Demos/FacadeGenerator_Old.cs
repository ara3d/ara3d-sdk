using Ara3D.Studio.Samples;
using Color = Ara3D.Geometry.Color;

namespace Ara3D.Studio.Samples.Demos;

public class FacadeGenerator_Old : IGenerator
{
    [Range(0f, 10f)] public float WidthPerSegment = 3f;
    [Range(1, 20)] public int WidthSegments = 4;
    [Range(0f, 10f)] public float HeightPerSegment = 4f;
    [Range(1, 20)] public int HeightSegments = 4;
    [Range(0f, 1f)] public float InsetAmount = 0.5f;
    [Range(-10f, 10f)] public float PushDistance = 0.2f;
    [Range(1, 4)] public int WindowWidthSegments = 2;
    [Range(1, 4)] public int WindowHeightSegments = 2;
    [Range(0f, 1f)] public float MullionWidth = 0.2f;
    [Range(0f, 5f)] public float PanelInset = 0.1f;

    public float Height => HeightPerSegment * HeightSegments;
    public float Width => WidthPerSegment * WidthSegments;

    public (QuadMesh3D Facade, IReadOnlyList<Quad3D> Voids) CreateFacade(Quad3D q)
    {
        var grid = q.Subdivide(WidthSegments, HeightSegments);
        var bldr = new QuadMesh3DBuilder(); ;
        bldr.Points.AddRange(grid.Points);
        var voids = new List<Quad3D>();

        foreach (var f in grid.FaceIndices)
        {
            var q1 = grid.Points.GetQuad(f);
            var q2 = q1.Inset(InsetAmount);
            var newFace = bldr.InsertFace(f, q2);
            bldr.DeleteLastFace();
            bldr.ExtrudeFace(newFace, -PushDistance);
            voids.Add(bldr.GetLastQuad());
            bldr.DeleteLastFace();
        }

        return (bldr.ToQuadMesh3D(), voids);
    }

    public Model3DBuilder BuildFacade(Quad3D q, Model3DBuilder? builder = null)
    {
        var (facade, voids) = CreateFacade(q);

        var colorPane = new Color(0, 0, 0.5f, 0.3f);
        var colorFrame = new Color(0.9f, 0.9f, 0.9f, 1f);
        var colorFacade = new Color(0.3f, 0.4f, 0.3f, 1f);

        var materialFrame = new Material(colorFrame, 1f, 0.1f);
        var materialPane = new Material(colorPane, 1f, 0f);
        var materialFacade = new Material(colorFacade, 0f, 0.8f);

        builder ??= new();
        builder.AddInstance(facade.Triangulate(), materialFacade);

        foreach (var v in voids)
        {
            var (frame, panes) = Window.CreateWindow(v, WindowWidthSegments, WindowHeightSegments, MullionWidth, PanelInset);

            builder.AddInstance(frame.Triangulate(), materialFrame);
            builder.AddInstance(panes.Triangulate(), materialPane);
        }

        return builder;
    }

    public IModel3D Eval(EvalContext context)
    {
        var q = GeometryUtil.XZQuad(Width, Height);
        return BuildFacade(q).Build();
    }
}