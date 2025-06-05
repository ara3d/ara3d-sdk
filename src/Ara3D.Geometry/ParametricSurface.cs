namespace Ara3D.Geometry;

// TODO: this needs to be moved to the Plato source
public class ParametricSurface 
{
    public bool ClosedX { get; }
    public bool ClosedY { get; }

    public Func<Vector2, Vector3> Func { get; }

    public ParametricSurface(Func<Vector2, Vector3> func, bool closedX, bool closedY)
    {

        ClosedX = closedX;
        ClosedY = closedY;
        Func = func;
    }

    public ParametricSurface TransformInput(Func<Vector2, Vector2> f) => new(x => Eval(f(x)), ClosedX, ClosedY);

    public Point3D Eval(Vector2 uv)
        => Func(uv);

    public ParametricSurface Deform(Func<Point3D, Point3D> f)
        => new(uv => f(Eval(uv)), ClosedX, ClosedY);
}