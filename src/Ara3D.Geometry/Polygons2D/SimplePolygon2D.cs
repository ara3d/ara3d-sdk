namespace Ara3D.Geometry;

public interface IPolygon2D
{
    IReadOnlyList<Vector2> Points { get; }
}

public interface ISimplePolygon2D : IPolygon2D { }
public interface IConvexPolygon2D : ISimplePolygon2D { }
public interface IConcavePolygon2D : IPolygon2D { }

public interface ISimplePolygonWithHoles2D : IPolygon2D
{
    ISimplePolygon2D Outer { get; }
    IReadOnlyList<ISimplePolygon2D> Holes { get; }
}

public class Polygon2D(IReadOnlyList<Vector2> points) : IPolygon2D
{
    public IReadOnlyList<Vector2> Points { get; } = points;
}

public class ConvexPolygon2D(IReadOnlyList<Vector2> points) : Polygon2D(points), IConvexPolygon2D
{
    public IReadOnlyList<Vector2> Points { get; } = points;
}

public class SimplePolygonWithHoles(ISimplePolygon2D boundary, IReadOnlyList<ISimplePolygon2D> holes) : ISimplePolygon2D
{
    public ISimplePolygon2D Boundary { get; } = boundary;
    public IReadOnlyList<ISimplePolygon2D> Holes { get; } = holes;
    public IReadOnlyList<Vector2> Points => Boundary.Points;
}

public class SimplePolygon2D(IReadOnlyList<Vector2> points) : ISimplePolygon2D
{
    public IReadOnlyList<Vector2> Points { get; } = points;
}

public interface IAnalyzedPolygon2D : IPolygon2D
{
    PolygonAnalysis Analysis { get; }
}

public class PolygonAnalysis : IAnalyzedPolygon2D
{
    public IReadOnlyList<Angle> Angles { get; }
    public int WindingNumber { get; }
    public bool IsSimple { get; }
    public bool IsConvex { get; }
    public bool Clockwise { get; }
    public Bounds2D Bounds { get; }
    public Vector2 Centroid { get; }
    public bool IsRegular { get; }
    public IPolygon2D Polygon { get; }

    public IReadOnlyList<Vector2> Points => Polygon.Points;
    public PolygonAnalysis Analysis => this;

    public PolygonAnalysis(IPolygon2D polygon)
    {
        Polygon = polygon;

        Bounds = polygon.GetBounds();
        Centroid = polygon.Centroid();
        Angles = polygon.Angles();
        WindingNumber = polygon.WindingNumber();
        Clockwise = WindingNumber < 0;
        IsConvex = polygon.IsConvex();
        IsSimple = polygon.IsSimple();
        IsRegular = polygon.IsRegular(1e-5f, 1e-4f);
    }
}