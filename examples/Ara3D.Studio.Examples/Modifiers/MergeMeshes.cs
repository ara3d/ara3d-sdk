namespace Ara3D.Studio.Samples.Modifiers;

[Category(Cat.Combine)]
[Description("Merges all instances of a model into a single mesh, optionally welding vertices and keeping colors.")]
public class MergeMeshes : IModifier
{
    public int MergedInstanceCount { get; private set; }
    public int VertexCount { get; private set; }
    public bool KeepColors { get; set; } = true;
    public bool WeldVertices { get; set; } = true;

    public ColoredTriangleMesh3D Eval(IModel3D model, EvalContext ctx)
    {
        MergedInstanceCount = model.Instances.Count;
        var r = model.ToColoredMesh();
        if (WeldVertices)
            // TODO: we will eventually need a system that can recolor vertices after being welded. 
            r = new ColoredTriangleMesh3D(r.Mesh.WeldVertices(), null);
        VertexCount = r.Mesh.Points.Count;
        ctx.Services.RefreshUI(this);
        if (!KeepColors)
            return new(r.Mesh, null);
        return r;
    }
}
