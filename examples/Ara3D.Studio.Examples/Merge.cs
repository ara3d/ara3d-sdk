namespace Ara3D.Studio.Samples;

public class Merge : IModelModifier
{
    public Model3D Eval(Model3D m, EvalContext eval)
    {
        if (m.Elements.Count == 0) return m;
        var mat = m.Elements[0].Material;
        return new Element(m.ToMesh(), mat);
    }
}