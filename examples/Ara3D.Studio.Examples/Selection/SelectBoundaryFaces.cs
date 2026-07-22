namespace Ara3D.Studio.Samples.Selection;

/// <summary>
/// Selects faces that have at least one boundary (open) edge.
/// </summary>
[Category(Cat.Select)]
public class SelectBoundaryFaces : IModifier
{
    public SelectionCombine Combine = SelectionCombine.Replace;

    public FlowObject Eval(TriangleMesh3D mesh, EvalContext ctx)
    {
        var topology = mesh.DerivedTopology();
        var boundary = new bool[topology.FaceCount];
        foreach (var he in topology.GetBoundaryHalfEdges())
            boundary[(int)topology.GetAssociatedFaceId(he)] = true;
        return ctx.SelectFaces(mesh, Combine, i => boundary[i]);
    }
}
