namespace Ara3D.Studio.Samples.Demos;

/// <summary>
/// Colors the face under the mouse. [PointerTracking] re-evaluates this node as the pointer or
/// camera moves; the world-space cursor ray from ctx.Services.ViewportInput is intersected with
/// every triangle and the nearest hit is highlighted. The mesh is unwelded (three unique vertices
/// per face) so the highlight stays crisp instead of bleeding across shared vertices. On hosts
/// without a viewport the ray is unavailable and the mesh renders entirely in the base color.
/// </summary>
[Category(nameof(Categories.Demos))]
[PointerTracking]
public class ColorHoveredFace : IModifier
{
    public Vector3 BaseColor = new(0.7f, 0.7f, 0.7f);
    public Vector3 HighlightColor = new(1f, 0.3f, 0.1f);

    public int HoveredFace { get; private set; } = -1;

    public ColoredTriangleMesh3D Eval(TriangleMesh3D mesh, EvalContext ctx)
    {
        var triangles = mesh.Triangles;
        HoveredFace = FindNearestHitFace(triangles, ctx.Services.ViewportInput);
        ctx.Services.RefreshUI(this);

        var points = new List<Point3D>(triangles.Count * 3);
        var indices = new List<Integer3>(triangles.Count);
        var colors = new List<Vector3>(triangles.Count * 3);
        for (var i = 0; i < triangles.Count; i++)
        {
            var t = triangles[i];
            points.Add(t.A);
            points.Add(t.B);
            points.Add(t.C);
            indices.Add(new Integer3(i * 3, i * 3 + 1, i * 3 + 2));
            var color = i == HoveredFace ? HighlightColor : BaseColor;
            colors.Add(color);
            colors.Add(color);
            colors.Add(color);
        }
        return new TriangleMesh3D(points, indices).ToColored(colors);
    }

    public static int FindNearestHitFace(IReadOnlyList<Triangle3D> triangles, IViewportInput input)
    {
        if (input == null)
            return -1;
        var ray = input.CursorRay;
        var best = -1;
        var bestDistance = float.MaxValue;
        for (var i = 0; i < triangles.Count; i++)
        {
            if (ray.Intersects(triangles[i], out var distance) && distance < bestDistance)
            {
                bestDistance = distance;
                best = i;
            }
        }
        return best;
    }
}
