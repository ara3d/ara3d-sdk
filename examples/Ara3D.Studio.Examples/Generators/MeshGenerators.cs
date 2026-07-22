namespace Ara3D.Studio.Samples.Generators;

[Category(Cat.Primitives)]
[Description("A cylinder surface with configurable radius, height, number of sides, and vertical segments.")]
public class Cylinder : IGenerator
{
    [Range(0f, 10f)] public float Height = 3;
    [Range(1, 100)] public int Segments = 4;
    [Range(3, 100)] public int Sides = 32;
    [Range(0f, 10f)] public float Radius = 1;

    public QuadGrid3D Eval()
        => Primitives.Cylinder(Sides, Radius, Height, Segments);
}

[Category(Cat.Primitives)]
[Description("A unit cube, optionally subdivided into an N x N grid of quads per face.")]
public class Cube : IGenerator
{
    [Range(1, 100)] public int Segments = 2;

    public QuadMesh3D Eval()
        => PlatonicSolids.Cube.Subdivide(Segments);
}

[Category(Cat.Primitives)]
[Description("A rectangular box with independent X/Y/Z dimensions, optionally subdivided.")]
public class Box : IGenerator
{
    [Range(1, 100)] public int Segments = 2;

    public Vector3 Dimensions { get; set; } = (1, 1, 1);
    
    public QuadMesh3D Eval()
        => PlatonicSolids.Cube.Scale(Dimensions).Subdivide(Segments);
}

[Category(Cat.Primitives)]
[Description("A straight prism formed by extruding a regular polygon, with configurable sides, radius, and height.")]
public class Prism : IGenerator
{
    [Range(0f, 10f)] public float Height = 1;
    [Range(1, 100)] public int Segments = 1;
    [Range(3, 32)] public int Sides = 3;
    [Range(0f, 10f)] public float Radius = 1;

    public QuadGrid3D Eval()
        => Sides.GetCircularPoints(Radius).Extrude(Height / Segments, Segments);
}

[Category(Cat.Primitives)]
[Description("A pyramid with a regular polygon base and a single apex.")]
public class Pyramid: IGenerator
{
    [Range(0f, 10f)] public float Height = 1;
    [Range(3, 32)] public int Sides = 3;
    [Range(0f, 10f)] public float Radius = 1;

    public TriangleMesh3D Eval()
    {
        var poly = new RegularPolygon(Point2D.Zero, Sides);
        var points = poly.Points.Map(p => p * Radius);
        var top = Vector3.UnitZ * Height;
        
        var mb = new TriangleMesh3DBuilder();
        mb.Points.AddRange(points.To3D());
        
        var topIndex = mb.Points.Count;
        mb.Points.Add(top);

        var bottomIndex = mb.Points.Count;
        mb.Points.Add(Point3D.Default);

        for (var i = 0; i < points.Count; i++)
        {
            var a = i;
            var b = (i + 1) % points.Count;
            mb.Faces.Add((a, b, topIndex));
        }

        for (var i = 0; i < points.Count; i++)
        {
            var a = i;
            var b = (i + 1) % points.Count;
            mb.Faces.Add((b, a, bottomIndex));
        }

        return mb.ToTriangleMesh3D();
    }
}

[Category(Cat.Primitives)]
[Description("A torus (doughnut) surface defined by major and minor radii and row/column resolution.")]
public class Torus : IGenerator
{
    public Vector2 ToUv(int i, int j)
        => (i / (float)NumColumns, j / (float)NumRows);

    public Point3D PointOnTorus(int i, int j)
        => ToUv(i, j).Torus(MajorRadius, MinorRadius);

    [Range(0f, 10f)] public float MajorRadius { get; set; } = 2f;
    [Range(0f, 10f)] public float MinorRadius { get; set; } = 0.2f;

    [Range(2, 128)] public int NumRows { get; set; } = 32;
    [Range(2, 128)] public int NumColumns { get; set; } = 32;

    public QuadMesh3D Eval()
    {
        var points = new FunctionalReadOnlyList2D<Point3D>(NumColumns, NumRows, PointOnTorus);
        return new QuadGrid3D(points, true, true).ToQuadMesh3D();
    }
}

[Category(Cat.Primitives)]
[Description("A hollow cylindrical tube (pipe) with inner and outer radii, revolved around the vertical axis.")]
public class Tube : IGenerator
{
    [Range(1, 32)] public int Count = 16;
    [Range(0f, 10f)] public float InnerRadius = 0.3f;
    [Range(0f, 10f)] public float OuterRadius = 0.5f;
    [Range(0f, 10f)] public float Height = 2;

    public QuadGrid3D Eval()
    {
        var box = new Point3D[]
        {
            (InnerRadius, 0, 0),
            (OuterRadius, 0, 0),
            (OuterRadius, 0, Height),
            (InnerRadius, 0, Height),
            (InnerRadius, 0, 0),
        };

        return box.Revolve(Vector3.UnitZ, Count);
    }
}

[Category(Cat.Primitives)]
[Description("A solid upward-pointing arrow with configurable shaft and tip dimensions.")]
public class UpArrow : IGenerator
{
    [Range(1, 32)] public int Count = 16;
    [Range(0f, 1f)] public float ShaftWidth = 0.05f;
    [Range(0f, 5f)] public float ShaftHeight = 0.8f;
    [Range(0f, 5f)] public float TipWidth = 0.2f;
    [Range(0f, 5f)] public float TipHeight = 0.2f;

    public QuadGrid3D Eval()
        => Primitives.UpArrow(Count, ShaftHeight, ShaftWidth, TipHeight, TipWidth);
}

[Category(Cat.Primitives)]
[Description("One of the five Platonic solids (tetrahedron, cube, octahedron, dodecahedron, icosahedron), selectable by name.")]
public class PlatonicSolid : IGenerator
{
    public List<string> ShapeNames() =>
        ["Tetrahedron", "Cube", "Octahedron", "Dodecahedron", "Icosahedron"];

    [Options(nameof(ShapeNames))] public int Shape;

    public TriangleMesh3D Eval()
        => PlatonicSolids.GetMesh(Shape);
}

[Category(Cat.Primitives)]
public class Plane
{
    [Range(1, 256)] public int NumRows = 16;
    [Range(1, 256)] public int NumColumns = 16;

    public QuadGrid3D Eval()
    {
        var points = new FunctionalReadOnlyList2D<Point3D>(
            NumColumns + 1, NumRows + 1,
            (i, j) => (i / (float)(NumColumns - 1), j / (float)(NumRows - 1), 0));
        return new QuadGrid3D(points, false, false);
    }
}

[Category(Cat.Primitives)]
[Description("A hollow box frame whose faces can be individually opened on the top, bottom, and sides.")]
public class SolidBoxFrame : IGenerator
{
    [Range(0f, 10f)] public float SizeX = 1;
    [Range(0f, 10f)] public float SizeY = 1;
    [Range(0f, 10f)] public float SizeZ = 1;
    [Range(0f, 0.5f)] public float FrameRatio = 0.1f;

    public bool EmptyTop = true;
    public bool EmptyBottom = true;
    public bool EmptySides = true;

    public QuadMesh3D Eval()
        => new BoxFrameMeshBuilder(FrameRatio, EmptyTop, EmptyBottom, EmptySides).Mesh.Scale((SizeX, SizeY, SizeZ));
}