namespace Ara3D.Studio.Samples.Modifiers;

/// <summary>
/// Turns each quad face of the input mesh into an inset panel by shrinking the face toward its own
/// centroid, leaving a gap between neighbouring panels — the classic curtain-wall / facade look.
/// Panels get their own vertices (no sharing), so each is a separate quad.
/// </summary>
[Category(Cat.Structures)]
[Description("Insets each quad face toward its centroid, producing separate facade-style panels with gaps between them.")]
public class PanelizeQuads : IModifier
{
    [Range(0f, 0.9f)] public float Inset = 0.15f;

    public int PanelCount { get; private set; }

    public object Eval(object obj, EvalContext ctx)
    {
        if (obj is QuadMesh3D quadMesh)
        {
            var panels = Panelize.BuildPanels(quadMesh, Inset, 0f);
            PanelCount = panels.FaceIndices.Count;
            ctx.Services.RefreshUI(this);
            return panels;
        }

        if (obj is QuadGrid3D quadGrid)
            return Eval(quadGrid.ToQuadMesh3D(), ctx);

        // Panelization needs quad topology; a triangle-mesh model passes through unchanged.
        return obj;
    }
}

/// <summary>
/// Like <see cref="PanelizeQuads"/>, but also lifts every inset panel off the surface along its own
/// quad normal by <see cref="Offset"/>, giving an exploded / louvre look.
/// </summary>
[Category(Cat.Structures)]
[Description("Insets each quad face and pushes it out along its normal, lifting the panels off the surface for an exploded look.")]
public class ExplodePanels : IModifier
{
    [Range(0f, 0.9f)] public float Inset = 0.1f;
    [Range(0f, 2f)] public float Offset = 0.1f;

    public int PanelCount { get; private set; }

    public object Eval(object obj, EvalContext ctx)
    {
        if (obj is QuadMesh3D quadMesh)
        {
            var panels = Panelize.BuildPanels(quadMesh, Inset, Offset);
            PanelCount = panels.FaceIndices.Count;
            ctx.Services.RefreshUI(this);
            return panels;
        }

        if (obj is QuadGrid3D quadGrid)
            return Eval(quadGrid.ToQuadMesh3D(), ctx);

        return obj;
    }
}

/// <summary>
/// Shared panel construction for <see cref="PanelizeQuads"/> and <see cref="ExplodePanels"/>.
/// </summary>
public static class Panelize
{
    /// <summary>
    /// Builds one inset quad per face of <paramref name="q"/>. Each panel's four corners are moved
    /// toward the face centroid by <paramref name="inset"/> and translated along the face normal by
    /// <paramref name="offset"/>. Every panel owns fresh vertices so panels stay separate.
    /// </summary>
    public static QuadMesh3D BuildPanels(QuadMesh3D q, float inset, float offset)
    {
        var points = new List<Point3D>(q.FaceIndices.Count * 4);
        var faces = new List<Integer4>(q.FaceIndices.Count);

        foreach (var fi in q.FaceIndices)
        {
            var quad = q.Quad(fi);
            var centroid = (quad.A + quad.B + quad.C + quad.D) / 4f;
            var normal = (quad.C - quad.A).Cross(quad.D - quad.B).Normalize;
            var push = normal * offset;

            var n = points.Count;
            points.Add(quad.A.Lerp(centroid, inset) + push);
            points.Add(quad.B.Lerp(centroid, inset) + push);
            points.Add(quad.C.Lerp(centroid, inset) + push);
            points.Add(quad.D.Lerp(centroid, inset) + push);
            faces.Add(new Integer4(n, n + 1, n + 2, n + 3));
        }

        return new QuadMesh3D(points, faces);
    }
}
