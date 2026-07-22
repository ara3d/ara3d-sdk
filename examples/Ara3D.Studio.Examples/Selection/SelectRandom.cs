namespace Ara3D.Studio.Samples.Selection;

[Category(Cat.Select)]
public class SelectRandom : IModifier
{
    [Range(0f, 1f)] public float Fraction = 0.5f;
    [Range(0, 1000)] public int Seed = 42;
    public SelectionCombine Combine = SelectionCombine.Replace;

    public FlowObject Eval(TriangleMesh3D mesh, EvalContext ctx)
    {
        var random = new Random(Seed);
        return ctx.SelectFaces(mesh, Combine, _ => random.NextSingle() < Fraction);
    }
}
