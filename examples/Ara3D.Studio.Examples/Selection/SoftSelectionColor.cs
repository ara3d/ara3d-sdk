namespace Ara3D.Studio.Samples.Selection;

/// <summary>
/// Visualizes a soft (weighted) vertex selection as a heat map: the cold color at weight 0,
/// the hot color at weight 1, lerped per vertex. Expects an upstream SoftSelect; with no
/// weights present every vertex takes the cold color. Colors vertices in place (no unweld),
/// so the gradient stays smooth across shared edges.
/// </summary>
[Category(Cat.Select)]
[Description("Heat-map visualization of a soft (weighted) vertex selection, cold at weight 0 and hot at weight 1.")]
public class SoftSelectionColor : IModifier
{
    public Vector3 ColdColor = new(0.15f, 0.18f, 0.28f);
    public Vector3 HotColor = new(1f, 0.45f, 0.05f);

    public ColoredTriangleMesh3D Eval(TriangleMesh3D mesh, EvalContext ctx)
    {
        var weights = ctx.GetSoftSelection(mesh);
        var colors = new Vector3[mesh.Points.Count];
        for (var i = 0; i < colors.Length; i++)
            colors[i] = ColdColor.Lerp(HotColor, weights?[i] ?? 0f);
        return mesh.ToColored(colors);
    }
}
