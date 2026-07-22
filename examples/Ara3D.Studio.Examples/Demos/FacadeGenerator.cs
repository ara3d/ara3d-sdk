using Color = Ara3D.Geometry.Color;
using Material = Ara3D.Models.Material;

namespace Ara3D.Studio.Samples.Demos;

[Category(Cat.ExperimentalDemos)]
public class FacadeGenerator : IGenerator
{
    [Range(0f, 100f)] public float Width = 3f;
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

    public float Height => HeightPerSegment * HeightSegments;

    public Quad3D GetWindowQuadFromSegment(Quad3D q)
    {
        var len = q.GetBottomLength();
        var xSpacing = (len - WindowWidth) / 2f;
        var height = q.GetHeight();
        var fromTop = height - WindowHeight - WindowDistanceFromFloor;
        return q.InsetAbs(xSpacing, xSpacing, WindowDistanceFromFloor, fromTop);
    }

    public (QuadMesh3D Facade, IReadOnlyList<Quad3D> Voids) CreateFacade(Quad3D q)
    {
        var len = q.GetBottomLength();
        var lenSegments = (int)Math.Ceiling(len / (WindowWidth + MinWindowSpacing));

        var bldr = q.Subdivide(lenSegments, HeightSegments).ToBuilder();
        var voids = new List<Quad3D>();

        foreach (var f in bldr.GetFaces())
        {
            var q2 = GetWindowQuadFromSegment(f.Quad);
            var newFace = f.Insert(q2).Extrude(-WindowInset);
            voids.Add(newFace.Quad);
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

    public IModel3D Eval()
    {
        var q = GeometryUtil.XZQuad(Width, Height);
        return BuildFacade(q).Build();
    }
}