namespace Ara3D.Studio.Samples.Demos;

/// <summary>
/// Generates a planar point set and Delaunay-triangulates it into a surface mesh.
/// </summary>
[Category(nameof(Categories.Demos))]
public class DelaunayTriangulationDemo : IGenerator
{
    [Range(3, 300)] public int PointCount = 48;
    [Range(0.1f, 10f)] public float Radius = 2f;
    [Range(0, 2)] public int Pattern;
    [Range(0, 3)] public int ProjectionPlane;
    public bool ShowPoints = true;
    [Range(0.01f, 0.3f)] public float PointSize = 0.06f;

    public IReadOnlyList<Point3D> RandomPoints(int seed)
    {
        var random = new Random(seed);
        var points = new List<Point3D>(PointCount);
        for (var i = 0; i < PointCount; ++i)
        {
            var angle = random.NextSingle() * MathF.Tau;
            var r = Radius * MathF.Sqrt(random.NextSingle());
            points.Add(new Vector3(r * MathF.Cos(angle), r * MathF.Sin(angle), 0));
        }

        return points;
    }

    public IReadOnlyList<Point3D> GridPoints()
    {
        var side = Math.Max(2, (int)MathF.Ceiling(MathF.Sqrt(PointCount)));
        var spacing = Radius * 2f / (side - 1);
        var points = new List<Point3D>(side * side);
        for (var y = 0; y < side; ++y)
        for (var x = 0; x < side; ++x)
        {
            points.Add(new Vector3(
                -Radius + x * spacing,
                -Radius + y * spacing,
                0));
        }

        return points;
    }

    public IReadOnlyList<Point3D> CirclePoints()
    {
        var points = new List<Point3D>(PointCount);
        for (var i = 0; i < PointCount; ++i)
        {
            var t = i / (float)PointCount;
            var angle = t * MathF.Tau;
            points.Add(new Vector3(Radius * MathF.Cos(angle), Radius * MathF.Sin(angle), 0));
        }

        return points;
    }

    public IReadOnlyList<Point3D> GetPoints()
    {
        return Pattern switch
        {
            0 => RandomPoints(42),
            1 => GridPoints(),
            2 => CirclePoints(),
            _ => RandomPoints(42)
        };
    }

    public IModel3D Eval()
    {
        var points = GetPoints();
        var plane = (DelaunayTriangulator.ProjectionPlane)Math.Clamp(ProjectionPlane, 0, 3);
        var mesh = points.DelaunayTriangulate(plane);

        var mb = new Model3DBuilder();
        mb.AddMeshWithoutInstance(mesh);

        if (ShowPoints)
        {
            var marker = PlatonicSolids.TriangulatedCube.Scale(PointSize);
            mb.AddModel(marker.Clone(Material.Default, points));
        }

        return mb.Build();
    }
}

/// <summary>
/// Delaunay-triangulates the vertices of an input triangle mesh.
/// </summary>
[Category(nameof(Categories.Meshes))]
public class DelaunayTriangulate : IModifier
{
    [Range(0, 3)] public int ProjectionPlane;

    public TriangleMesh3D Eval(TriangleMesh3D mesh)
    {
        var plane = (DelaunayTriangulator.ProjectionPlane)Math.Clamp(ProjectionPlane, 0, 3);
        return mesh.DelaunayTriangulateVertices(plane);
    }
}

/// <summary>
/// Refines a mesh by inserting interior points on oversized faces and re-triangulating
/// each face in-plane with 2D Delaunay. Shape is preserved exactly; only tessellation density changes.
/// </summary>
[Category(nameof(Categories.Meshes))]
public class DelaunayRefine : IModifier
{
    [Range(0.05f, 5f)] public float TargetEdgeLength = 0.25f;

    public TriangleMesh3D Eval(TriangleMesh3D mesh)
        => mesh.DelaunayRefine(TargetEdgeLength);
}

/// <summary>
/// Builds the convex hull of a mesh's vertices via 3D Delaunay tetrahedralization.
/// </summary>
[Category(nameof(Categories.Meshes))]
public class DelaunayConvexHull : IModifier
{
    public TriangleMesh3D Eval(TriangleMesh3D mesh)
        => mesh.Points.DelaunayHull();
}
