namespace Ara3D.Studio.Samples.Selection;

/// <summary>
/// Selects faces whose centroid falls inside a box given as fractions of the mesh bounds.
/// </summary>
[Category(Cat.Select)]
public class SelectByBox : IModifier
{
    [Range(0f, 1f)] public float MinX = 0f;
    [Range(0f, 1f)] public float MaxX = 1f;
    [Range(0f, 1f)] public float MinY = 0f;
    [Range(0f, 1f)] public float MaxY = 1f;
    [Range(0f, 1f)] public float MinZ = 0f;
    [Range(0f, 1f)] public float MaxZ = 0.5f;
    public SelectionCombine Combine = SelectionCombine.Replace;

    public FlowObject Eval(TriangleMesh3D mesh, EvalContext ctx)
    {
        var bounds = mesh.DerivedBounds();
        var size = bounds.Size;
        var min = bounds.Min;
        bool InRange(float value, float lo, float hi, float origin, float extent)
        {
            var t = extent > 1e-9f ? (value - origin) / extent : 0.5f;
            return t >= lo && t <= hi;
        }
        return ctx.SelectFaces(mesh, Combine, i =>
        {
            var c = mesh.Triangles[i].Center;
            return InRange(c.X, MinX, MaxX, min.X, size.X)
                && InRange(c.Y, MinY, MaxY, min.Y, size.Y)
                && InRange(c.Z, MinZ, MaxZ, min.Z, size.Z);
        });
    }
}
