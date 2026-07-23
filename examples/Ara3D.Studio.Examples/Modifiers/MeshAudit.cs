namespace Ara3D.Studio.Samples.Modifiers;

/// <summary>
/// Pass-through health audit. Returns the input mesh unchanged, but computes a full battery of
/// topology and geometry statistics (via <see cref="MeshStatistics"/> / <see cref="MeshFeatures"/>)
/// into read-out properties so the mesh's health can be inspected in the panel without altering it.
/// Marked <c>[OnDemand]</c> because computing the full statistics set is expensive.
/// </summary>
[Category(Cat.Analyze)]
[OnDemand]
[Description("Reports mesh health (counts, boundaries, manifoldness, area, volume, size) without changing the geometry.")]
public class MeshAudit : IModifier
{
    public int Points { get; private set; }
    public int Vertices { get; private set; }
    public int Faces { get; private set; }
    public int Edges { get; private set; }
    public int BoundaryEdges { get; private set; }
    public int BoundaryLoops { get; private set; }
    public int NonManifoldEdges { get; private set; }
    public int Components { get; private set; }

    public bool IsClosed { get; private set; }
    public bool IsWatertight { get; private set; }

    public double SurfaceArea { get; private set; }
    public double Volume { get; private set; }

    public float SizeX { get; private set; }
    public float SizeY { get; private set; }
    public float SizeZ { get; private set; }

    public string Error { get; private set; } = "";

    public TriangleMesh3D Eval(TriangleMesh3D mesh, EvalContext ctx)
    {
        try
        {
            var f = new MeshFeatures(new MeshStatistics(mesh));
            Points = f.PointCount;
            Vertices = f.VertexCount;
            Faces = f.FaceCount;
            Edges = f.EdgeCount;
            BoundaryEdges = f.BoundaryEdgeCount;
            BoundaryLoops = f.BoundaryLoopCount;
            NonManifoldEdges = f.NonManifoldEdgeCount;
            Components = f.ConnectedComponentCount;
            IsClosed = f.IsClosed;
            IsWatertight = f.IsProbablyWatertight;
            SurfaceArea = f.SurfaceArea;
            Volume = f.EstimatedVolume;
            var size = f.AabbSize;
            SizeX = size.X;
            SizeY = size.Y;
            SizeZ = size.Z;
            Error = "";
        }
        catch (Exception e)
        {
            Error = e.Message;
        }
        ctx.Services.RefreshUI(this);
        return mesh;
    }
}

/// <summary>
/// Colors each connected component a distinct hue, making disjoint shells easy to tell apart.
/// Components come from the derived topology; every vertex touched by a component's faces is
/// painted with that component's palette color, and any untouched vertex stays a neutral gray.
/// </summary>
[Category(Cat.Analyze)]
[Description("Colors each connected component a distinct hue so separate shells are easy to distinguish.")]
public class ColorByComponent : IModifier
{
    public int ComponentCount { get; private set; }

    public ColoredTriangleMesh3D Eval(TriangleMesh3D mesh, EvalContext ctx)
    {
        var components = mesh.DerivedTopology().GetConnectedFaceComponents();
        ComponentCount = components.Count;
        ctx.Services.RefreshUI(this);

        var colors = new Vector3[mesh.Points.Count];
        Array.Fill(colors, new Vector3(0.5f, 0.5f, 0.5f));

        for (var i = 0; i < components.Count; i++)
        {
            var color = PaletteColor(i);
            foreach (var faceId in components[i].Faces)
            {
                var f = mesh.FaceIndices[(int)faceId];
                colors[f.A] = color;
                colors[f.B] = color;
                colors[f.C] = color;
            }
        }

        return new ColoredTriangleMesh3D(mesh, colors);
    }

    // Deterministic palette: golden-ratio hue stepping keeps successive components far apart in hue.
    static Vector3 PaletteColor(int i)
        => HueToRgb((float)((i * 0.61803398875) % 1.0));

    // Fully-saturated, full-value HSV-to-RGB for a hue in [0, 1).
    static Vector3 HueToRgb(float h)
    {
        var r = Math.Abs(h * 6 - 3) - 1;
        var g = 2 - Math.Abs(h * 6 - 2);
        var b = 2 - Math.Abs(h * 6 - 4);
        return new Vector3(Clamp01(r), Clamp01(g), Clamp01(b));
    }

    static float Clamp01(float x)
        => x < 0 ? 0 : x > 1 ? 1 : x;
}
