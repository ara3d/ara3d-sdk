namespace Ara3D.Studio.Samples.Selection;

/// <summary>
/// Selects faces that touch a sharp edge: an edge whose two face normals differ
/// by at least the given dihedral angle.
/// </summary>
[Category(Cat.Select)]
public class SelectSharpFaces : IModifier
{
    [Range(0f, 180f)] public float MinAngle = 45f;
    public SelectionCombine Combine = SelectionCombine.Replace;

    public FlowObject Eval(TriangleMesh3D mesh, EvalContext ctx)
    {
        var topology = mesh.DerivedTopology();
        var normals = mesh.DerivedFaceNormals();
        var maxDot = MathF.Cos(MinAngle * MathF.PI / 180f);
        var sharp = new bool[topology.FaceCount];
        foreach (var he in topology.GetHalfEdgeIds())
        {
            if (!topology.TryGetTwin(he, out var twin))
                continue;
            var f0 = (int)topology.GetAssociatedFaceId(he);
            var f1 = (int)topology.GetAssociatedFaceId(twin);
            if (normals[f0].Dot(normals[f1]) <= maxDot)
            {
                sharp[f0] = true;
                sharp[f1] = true;
            }
        }
        return ctx.SelectFaces(mesh, Combine, i => sharp[i]);
    }
}
