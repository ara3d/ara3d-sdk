namespace Ara3D.Geometry;

public class ParametricSurface :
    IDeformable3D<ParametricSurface>
{
    public bool ClosedU { get; }
    public bool ClosedV { get; }

    public Func<Vector2, Vector3> Func { get; }

    public ParametricSurface(Func<Vector2, Vector3> func, bool closedU, bool closedV)
    {
        ClosedU = closedU;
        ClosedV = closedV;
        Func = func;
    }

    public Point3D Eval(Vector2 uv)
        => Func(uv);

    public ParametricSurface Deform(Func<Point3D, Point3D> f)
        => new(uv => f(Eval(uv)), ClosedU, ClosedV);

    public ParametricSurface Transform(Transform3D t)
        => Deform(t.TransformPoint);

    public ParametricSurface WithClosedU(bool closedU)
        => new(Func, closedU, ClosedV);

    public ParametricSurface WithClosedV(bool closedV)
        => new(Func, ClosedU, closedV);

    public ParametricSurface WithClosedUV(bool closedU, bool closedV)
        => new(Func, closedU, closedV);
}

/*
public ParametricSurface TransformInput(Func<Vector2, Vector2> f)
    => new(x => Eval(f(x)), ClosedX, ClosedY);
*/
