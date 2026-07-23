namespace Ara3D.Studio.Samples.Selection;

/// <summary>
/// Translates each vertex by Offset scaled by its soft-selection weight, so the falloff of
/// an upstream SoftSelect turns a rigid move into a smooth bulge or pull. Vertices at weight
/// 1 move the full Offset, weight 0 stay put, and everything between eases by the falloff
/// curve. With no soft selection present the mesh passes through unchanged (the weighted
/// move has nothing to act on). The move direction rides a translation gizmo.
/// </summary>
[Category(Cat.Deform)]
[Description("Moves vertices by an offset scaled by their soft-selection weight, turning a falloff into a smooth bulge or pull.")]
public class SoftSelectionMove : IModifier, IGizmoProvider
{
    public Vector3 Offset = new(0f, 0f, 10f);

    public TriangleMesh3D Eval(TriangleMesh3D mesh, EvalContext ctx)
    {
        var weights = ctx.GetSoftSelection(mesh);
        if (weights == null)
            return mesh;
        var points = mesh.Points;
        var moved = new Point3D[points.Count];
        for (var i = 0; i < points.Count; i++)
            moved[i] = points[i] + Offset * weights[i];
        return mesh.WithPoints(moved);
    }

    public IReadOnlyList<GizmoElement> GetGizmoElements()
        => GizmoElements.Translation(nameof(Offset));
}
