namespace Ara3D.Studio.Samples.Modifiers;

[Category(nameof(Categories.Topology))]
public class WeldVertices : IModifier 
{ 
    public int VertexCountBefore { get; private set; }
    public int VertexCountAfter { get; private set; }

    public TriangleMesh3D Eval(TriangleMesh3D mesh, EvalContext ctx)
    {
        var r = mesh.WeldVertices();
        VertexCountBefore = mesh.Points.Count;
        VertexCountAfter = r.Points.Count;
        ctx.Services.RefreshUI(this);
        return r;
    }
}