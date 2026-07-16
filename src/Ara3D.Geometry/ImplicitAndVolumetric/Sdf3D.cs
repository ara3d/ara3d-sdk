namespace Ara3D.Geometry;

public class Sdf3D : ITransformable3D<Sdf3D>
{
    public Sdf3D(Func<Point3D, Number> f)
        => _func = f;
    
    private Func<Point3D, Number> _func;

    public Number Eval(Point3D p)
        => _func(p);

    public Sdf3D Transform(Transform3D t)
        => new(p => Eval(t.Invert.TransformPoint(p)));
}