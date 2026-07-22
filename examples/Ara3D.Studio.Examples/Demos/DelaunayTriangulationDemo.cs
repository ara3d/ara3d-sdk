using Color = Ara3D.Geometry.Color;
using Material = Ara3D.Models.Material;

namespace Ara3D.Studio.Samples.Demos;

public enum DelaunayPointPattern
{
    Random,
    Grid,
    Circle,
}

public enum CircumcircleDisplay
{
    None,
    LastPoint,
    All,
}

/// <summary>
/// Interactive Delaunay triangulation explainer:
/// - Faces render as a pastel mosaic (double-sided) with wireframe edge tubes, so the
///   triangle structure is visible from any angle.
/// - Drag Insertion from 0 to 1 to watch the triangulation grow point by point: the last
///   inserted point is gold, the triangles it created are highlighted orange (the refilled
///   Bowyer-Watson cavity), and points not yet inserted are dim gray.
/// - Circumcircles visualizes the defining property — no point lies inside any triangle's
///   circumcircle. LastPoint shows the circles of the freshly created triangles; All shows every one.
/// </summary>
[Category(Cat.ExperimentalDemos)]
[Description("Interactive Delaunay triangulation explainer with step-by-step point insertion and circumcircle visualization.")]
public class DelaunayTriangulationDemo : IGenerator
{
    [Range(3, 300)] public int PointCount = 48;
    [Range(0.1f, 10f)] public float Radius = 2f;
    public DelaunayPointPattern Pattern = DelaunayPointPattern.Random;
    [Range(0f, 1f)] public float Insertion = 1f;
    public bool ShowFaces = true;
    public bool ShowEdges = true;
    public bool ShowPoints = true;
    public CircumcircleDisplay Circumcircles = CircumcircleDisplay.LastPoint;
    [Range(0.002f, 0.1f)] public float EdgeRadius = 0.015f;
    [Range(0.01f, 0.3f)] public float PointSize = 0.05f;

    public static Color Hue(float h)
        => new(
            Math.Clamp(Math.Abs(h * 6 - 3) - 1, 0f, 1f),
            Math.Clamp(2 - Math.Abs(h * 6 - 2), 0f, 1f),
            Math.Clamp(2 - Math.Abs(h * 6 - 4), 0f, 1f),
            1f);

    public static Color Pastel(float h)
    {
        var c = Hue(h);
        return new Color(
            (float)c.R * 0.55f + 0.45f,
            (float)c.G * 0.55f + 0.45f,
            (float)c.B * 0.55f + 0.45f,
            1f);
    }

    public static TriangleMesh3D UnitTube(float radius)
        => new RegularPolygon(Point2D.Zero, 8).ToPolyLine3D().Scale(radius)
            .Points.Extrude(1).Triangulate();

    public static TriangleMesh3D Marker(Point3D point, float radius)
        => PlatonicSolids.Icosahedron.Deform(p =>
            (p.X * radius + point.X, p.Y * radius + point.Y, p.Z * radius + point.Z));

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
            points.Add(new Vector3(-Radius + x * spacing, -Radius + y * spacing, 0));

        return points;
    }

    public IReadOnlyList<Point3D> CirclePoints()
    {
        var points = new List<Point3D>(PointCount);
        for (var i = 0; i < PointCount; ++i)
        {
            var angle = i / (float)PointCount * MathF.Tau;
            points.Add(new Vector3(Radius * MathF.Cos(angle), Radius * MathF.Sin(angle), 0));
        }

        return points;
    }

    public IReadOnlyList<Point3D> GetPoints()
        => Pattern switch
        {
            DelaunayPointPattern.Grid => GridPoints(),
            DelaunayPointPattern.Circle => CirclePoints(),
            _ => RandomPoints(42),
        };

    static TriangleMesh3D DoubleSidedTriangle(Vector3 a, Vector3 b, Vector3 c)
        => new(
            new List<Point3D> { a, b, c },
            new List<Integer3> { new(0, 1, 2), new(0, 2, 1) });

    static (Vector3 Center, float Radius, Vector3 U, Vector3 V) Circumcircle(Vector3 a, Vector3 b, Vector3 c)
    {
        var ba = b - a;
        var ca = c - a;
        var n = Vector3.Cross(ba, ca);
        var d = 2f * (float)n.LengthSquared();
        if (d < 1e-12f)
            return (a, 0f, Vector3.UnitX, Vector3.UnitY);

        var baLen = (float)ba.LengthSquared();
        var caLen = (float)ca.LengthSquared();
        var dot = (float)Vector3.Dot(ba, ca);
        var alpha = caLen * (baLen - dot) / d;
        var beta = baLen * (caLen - dot) / d;
        var center = a + ba * alpha + ca * beta;

        var u = ba.Normalize;
        var w = n.Normalize;
        return (center, (float)(center - a).Length(), u, Vector3.Cross(w, u));
    }

    static TriangleMesh3D Ring(Vector3 center, float radius, float width, Vector3 u, Vector3 v)
    {
        const int segments = 48;
        var points = new List<Point3D>(segments * 2);
        var faces = new List<Integer3>(segments * 4);
        for (var i = 0; i < segments; ++i)
        {
            var angle = i / (float)segments * MathF.Tau;
            var dir = u * MathF.Cos(angle) + v * MathF.Sin(angle);
            points.Add(center + dir * (radius - width));
            points.Add(center + dir * (radius + width));
        }

        for (var i = 0; i < segments; ++i)
        {
            var i0 = i * 2;
            var i1 = i * 2 + 1;
            var j0 = (i + 1) % segments * 2;
            var j1 = j0 + 1;
            faces.Add(new Integer3(i0, i1, j1));
            faces.Add(new Integer3(i0, j1, j0));
            faces.Add(new Integer3(i0, j1, i1));
            faces.Add(new Integer3(i0, j0, j1));
        }

        return new TriangleMesh3D(points, faces);
    }

    static bool Touches(Integer3 face, int index)
        => face.A == index || face.B == index || face.C == index;

    public IModel3D Eval()
    {
        var allPoints = GetPoints();
        var inserted = Math.Clamp((int)MathF.Round(Insertion * allPoints.Count), 3, allPoints.Count);
        var points = allPoints.Take(inserted).ToList();
        var lastIndex = inserted - 1;
        var partial = inserted < allPoints.Count;

        var faces = DelaunayTriangulator.Triangulate(
            points.Select(p => new Vector2(p.X, p.Y)).ToList());

        var mb = new Model3DBuilder();

        if (ShowFaces)
        {
            for (var i = 0; i < faces.Count; ++i)
            {
                var f = faces[i];
                var isNew = partial && Touches(f, lastIndex);
                var color = isNew ? new Color(1f, 0.55f, 0.15f, 1f) : Pastel(i / (float)Math.Max(faces.Count, 1));
                mb.AddInstance(
                    DoubleSidedTriangle(points[f.A].Vector3, points[f.B].Vector3, points[f.C].Vector3),
                    new Material(color, 0f, 0.7f));
            }
        }

        if (ShowEdges)
        {
            var tube = mb.AddMeshWithoutInstance(UnitTube(EdgeRadius));
            var material = new Material(new Color(0.25f, 0.25f, 0.3f, 1f), 0.1f, 0.5f);
            var edges = new HashSet<(int, int)>();
            foreach (var f in faces)
            {
                edges.Add(f.A < f.B ? (f.A, f.B) : (f.B, f.A));
                edges.Add(f.B < f.C ? (f.B, f.C) : (f.C, f.B));
                edges.Add(f.C < f.A ? (f.C, f.A) : (f.A, f.C));
            }

            foreach (var (a, b) in edges)
            {
                var line = new Line3D(points[a], points[b]);
                if (line.Length > 1e-6f)
                    mb.AddInstance(tube, line.AlignZAxisTransform(), material);
            }
        }

        if (ShowPoints)
        {
            var insertedMaterial = new Material(new Color(0.1f, 0.6f, 0.85f, 1f), 0.2f, 0.4f);
            var pendingMaterial = new Material(new Color(0.4f, 0.4f, 0.4f, 1f), 0f, 0.8f);
            var lastMaterial = new Material(new Color(1f, 0.8f, 0.1f, 1f), 0.6f, 0.3f);

            for (var i = 0; i < allPoints.Count; ++i)
            {
                if (partial && i == lastIndex)
                    mb.AddInstance(Marker(allPoints[i], PointSize * 1.8f), lastMaterial);
                else if (i < inserted)
                    mb.AddInstance(Marker(allPoints[i], PointSize), insertedMaterial);
                else
                    mb.AddInstance(Marker(allPoints[i], PointSize * 0.6f), pendingMaterial);
            }
        }

        if (Circumcircles != CircumcircleDisplay.None)
        {
            var material = new Material(new Color(1f, 0.75f, 0.2f, 1f), 0.4f, 0.4f);
            var lift = new Vector3(0, 0, EdgeRadius * 2f);
            foreach (var f in faces)
            {
                if (Circumcircles == CircumcircleDisplay.LastPoint && !(partial && Touches(f, lastIndex)))
                    continue;

                var (center, radius, u, v) = Circumcircle(
                    points[f.A].Vector3, points[f.B].Vector3, points[f.C].Vector3);
                if (radius > 1e-6f)
                    mb.AddInstance(Ring(center + lift, radius, EdgeRadius, u, v), material);
            }
        }

        return mb.Build();
    }
}

/// <summary>
/// Delaunay-triangulates the vertices of an input triangle mesh.
/// </summary>
[Category(Cat.Remesh)]
[Description("Delaunay-triangulates the input mesh's vertices in a chosen projection plane.")]
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
[Category(Cat.Remesh)]
[Description("Refines a mesh by inserting points on oversized faces and re-triangulating in-plane, preserving the shape exactly.")]
public class DelaunayRefine : IModifier
{
    [Range(0.05f, 5f)] public float TargetEdgeLength = 0.25f;

    public TriangleMesh3D Eval(TriangleMesh3D mesh)
        => mesh.DelaunayRefine(TargetEdgeLength);
}

/// <summary>
/// Builds the convex hull of a mesh's vertices via 3D Delaunay tetrahedralization.
/// </summary>
[Category(Cat.Remesh)]
[Description("Builds the convex hull of the input mesh's vertices via 3D Delaunay tetrahedralization.")]
public class DelaunayConvexHull : IModifier
{
    public TriangleMesh3D Eval(TriangleMesh3D mesh)
        => mesh.Points.DelaunayHull();
}
