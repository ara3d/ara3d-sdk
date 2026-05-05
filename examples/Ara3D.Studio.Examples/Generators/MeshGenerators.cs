namespace Ara3D.Studio.Samples.Generators;

[Category(nameof(Categories.Meshes))]
public class Cylinder : IGenerator
{
    [Range(0f, 10f)] public float Height = 3;
    [Range(1, 100)] public int Segments = 4;
    [Range(3, 100)] public int Sides = 32;
    [Range(0f, 10f)] public float Radius = 1;

    public QuadGrid3D Eval()
        => Sides.GetCircularPoints(Radius).Extrude(Height / Segments, Segments);
}

[Category(nameof(Categories.Meshes))]
public class Cube : IGenerator
{
    [Range(1, 100)] public int Segments = 2;

    public QuadMesh3D Eval()
        => PlatonicSolids.Cube.Subdivide(Segments);
}

[Category(nameof(Categories.Meshes))]
public class Box : IGenerator
{
    [Range(1, 100)] public int Segments = 2;
    public Vector3 Dimensions = new(1,1,3);

    public QuadMesh3D Eval()
        => PlatonicSolids.Cube.Scale(Dimensions).Subdivide(Segments);
}

[Category(nameof(Categories.Meshes))]
public class Prism : IGenerator
{
    [Range(0f, 10f)] public float Height = 1;
    [Range(1, 100)] public int Segments = 1;
    [Range(3, 32)] public int Sides = 3;
    [Range(0f, 10f)] public float Radius = 1;

    public QuadGrid3D Eval()
        => Sides.GetCircularPoints(Radius).Extrude(Height / Segments, Segments);
}

[Category(nameof(Categories.Meshes))]
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

[Category(nameof(Categories.Meshes))]
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

[Category(nameof(Categories.Meshes))]
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

[Category(nameof(Categories.Meshes))]
public class UpArrow : IGenerator
{
    [Range(1, 32)] public int Count = 16;
    [Range(0f, 1f)] public float ShaftWidth = 0.01f;
    [Range(0f, 5f)] public float ShaftHeight = 0.8f;
    [Range(0f, 5f)] public float TipWidth = 0.2f;
    [Range(0f, 5f)] public float TipHeight = 0.2f;

    public QuadGrid3D Eval()
    {
        var totalHeight = ShaftHeight + TipHeight;
        var halfOutLine = new Point3D[]
        {
            (0, 0, 0),
            (ShaftWidth / 2, 0, 0),
            (ShaftWidth / 2, 0, ShaftHeight),
            (TipWidth / 2, 0, ShaftHeight),
            (0, 0, totalHeight),
        };

        return halfOutLine.Revolve(Vector3.UnitZ, Count);
    }
}

[Category(nameof(Categories.Meshes))]
public class PlatonicSolid : IGenerator
{
    public List<string> ShapeNames() =>
        ["Tetrahedron", "Cube", "Octahedron", "Dodecahedron", "Icosahedron"];

    [Options(nameof(ShapeNames))] public int Shape;

    public TriangleMesh3D Eval()
        => PlatonicSolids.GetMesh(Shape);
}

[Category(nameof(Categories.Meshes))]
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