namespace Ara3D.Studio.Samples;

public class RuledSurface : IModelGenerator
{
    [Range(0, 5)] public float Size = 2f;
    [Range(2, 64)] public int Count = 16;

    public Quad3D SquareQuadXY
        => new (
            (-0.5f, -0.5f, 0),
            (0.5f, -0.5f, 0),
            (0.5f, 0.5f, 0),
            (-0.5f, 0.5f, 0));

    public Model3D Eval(EvalContext context)
    {
        var quad = SquareQuadXY.Scale(Size);
        var curve0 = Curves.QuadraticBezier(quad.A, quad.Center, quad.B);
        var curve1 = Curves.QuadraticBezier(quad.D, quad.Center, quad.C);
        var surface = curve0.RuledSurface(curve1, Count);
        return surface.Triangulate();
    }
}