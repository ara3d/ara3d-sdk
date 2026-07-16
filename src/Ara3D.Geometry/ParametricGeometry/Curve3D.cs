namespace Ara3D.Geometry;

public class Curve3D : IDeformable3D<Curve3D>
{
    private Func<Number, Point3D> _func;

    public Curve3D(Func<Number, Point3D> func)
        => _func = func;

    public Point3D Eval(Number t)
        => _func(t);

    public IReadOnlyList<Point3D> Sample(Integer n)
        => n.LinearSpace.Map(Eval);

    public Curve3D Deform(Func<Point3D, Point3D> f)
        => new(t => f(Eval(t)));

    public Curve3D Transform(Transform3D t)
        => Deform(t.TransformPoint);
}
