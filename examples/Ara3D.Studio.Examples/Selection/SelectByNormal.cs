namespace Ara3D.Studio.Samples.Selection;

/// <summary>
/// Selects faces whose normal is within an angle tolerance of a direction
/// (e.g. up-facing roofs, down-facing overhangs).
/// </summary>
[Category(Cat.Select)]
[Description("Selects faces whose normal points within an angle tolerance of a given direction.")]
public class SelectByNormal : IModifier
{
    [Range(-1f, 1f)] public float DirectionX = 0f;
    [Range(-1f, 1f)] public float DirectionY = 0f;
    [Range(-1f, 1f)] public float DirectionZ = 1f;
    [Range(0f, 180f)] public float AngleTolerance = 45f;
    public SelectionCombine Combine = SelectionCombine.Replace;

    public FlowObject Eval(TriangleMesh3D mesh, EvalContext ctx)
    {
        var direction = new Vector3(DirectionX, DirectionY, DirectionZ);
        if (direction.Length < 1e-6f)
            return ctx.SelectFaces(mesh, Combine, _ => false);
        direction = direction.Normalize;
        var normals = mesh.DerivedFaceNormals();
        var minDot = MathF.Cos(AngleTolerance * MathF.PI / 180f);
        return ctx.SelectFaces(mesh, Combine, i => normals[i].Dot(direction) >= minDot);
    }
}
