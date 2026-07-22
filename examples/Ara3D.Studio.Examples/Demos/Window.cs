using Color = Ara3D.Geometry.Color;

namespace Ara3D.Studio.Samples.Demos;

[Category(Cat.ExperimentalDemos)]
public class Window : IGenerator
{
    [Range(0f, 10f)] public float Width = 3f;
    [Range(1, 20)] public int XSegments = 4;
    [Range(0f, 10f)] public float Height = 4f;
    [Range(1, 20)] public int YSegments = 4;
    [Range(0f, 1f)] public float MullionWidth = 0.5f;
    [Range(-10f, 10f)] public float PaneInset = 0.2f;
    
    public static (QuadMesh3D Frame, QuadMesh3D Pane) CreateWindow(Quad3D q, int xSegments, int ySegments, float mullionWidth, float paneInset)
    {
        var bldr = q.Subdivide(xSegments, ySegments).ToBuilder();

        var pane = new List<Quad3D>();

        foreach (var f in bldr.GetFaces())
        {
            var newFace = f.Inset(mullionWidth).Extrude(-paneInset);
            pane.Add(newFace.Quad);
            newFace.Delete();
        }

        return (bldr.ToQuadMesh3D(), pane.ToQuadMesh3D());
    }

    public IModel3D Eval()
    {
        var q = GeometryUtil.XZQuad(Width, Height);
        var (frame, pane) = CreateWindow(q, XSegments, YSegments, MullionWidth, PaneInset);
        var meshes = new[] { frame.Triangulate(), pane.Triangulate() };
        var colorFrame = new Color(0.9f, 0.9f, 0.9f, 1f);
        var colorPane = new Color(0, 0, 0.5f, 0.3f);
        var materialFrame = new Material(colorFrame, 1f, 0.1f);
        var materialPane = new Material(colorPane, 1f, 0f);
        var instances = new[]
        {
            new InstanceStruct(0, Matrix4x4.Identity, 0, materialFrame, 0),
            new InstanceStruct(0, Matrix4x4.Identity, 1, materialPane, 0)
        };
        return new Model3D(meshes, instances);
    }
}