namespace Ara3D.Studio.Samples.Selection;

/// <summary>
/// Soft (weighted) vertex selection inside a sphere: every vertex gets a weight in [0,1]
/// that is 1 at the center and eases to 0 at the radius by the chosen falloff curve. The
/// center rides a translation gizmo (an offset from the input bounds center) and a square
/// handle drags the radius. Geometry passes through unchanged — the weights travel as a
/// vertex attribute for downstream tools (see SoftSelectionColor to visualize).
/// </summary>
[Category(Cat.Select)]
[Description("Weighted vertex selection inside a sphere: weight 1 at the center easing to 0 at the radius, driven by a gizmo.")]
public class SoftSelect : IModifier, IGizmoProvider
{
    public Vector3 Center;
    [Range(0f, 10f)] public float Radius = 0f; // 0 = auto: a small fraction of the model size
    public FalloffCurve Falloff = FalloffCurve.Smooth;

    public int AffectedVertices { get; private set; }

    public FlowObject Eval(TriangleMesh3D mesh, EvalContext ctx)
    {
        var bounds = mesh.DerivedBounds();
        var center = bounds.Center.Vector3 + Center;
        // Absolute world radius when set; otherwise a tight default that scales with the model.
        var radius = Radius > 0f ? Radius : (float)bounds.Size.Length * 0.15f;
        var points = mesh.Points;
        var weights = new float[points.Count];
        var affected = 0;
        for (var i = 0; i < points.Count; i++)
        {
            var d = (points[i].Vector3 - center).Length;
            var w = SoftSelectionHelpers.Weight(Falloff, d, radius);
            weights[i] = w;
            if (w > 0f)
                affected++;
        }
        AffectedVertices = affected;
        ctx.Services.RefreshUI(this);
        return ctx.Input
            .WithNewContent(mesh, FlowAttribute.AttributeDomainMask.All)
            .WithSoftSelection(weights);
    }

    // OutputBounds (the default) — this modifier passes geometry through unchanged, so output
    // bounds equal input bounds. It also avoids studio-161 (InputBounds gives no gizmo when the
    // modifier is first in the pipeline, because CachedInput is unset for a first modifier).

    // Ride the falloff center: the gizmo anchor is the bounds center, so offsetting it by
    // Center puts the handles on the actual center the weights are measured from.
    public Vector3 GetGizmoAnchorOffset()
        => Center;

    public IReadOnlyList<GizmoElement> GetGizmoElements()
    {
        var diagonal = new Vector3(1f, 1f, 1f).Normalize;
        var idle = new GizmoStyle(GizmoElements.FreeColor, 0.6f);
        var hover = new GizmoStyle(GizmoElements.FreeColor, 1f, 1.18f);
        var active = new GizmoStyle(GizmoElements.FreeColor, 1f, 1.28f);
        var elements = new List<GizmoElement>(GizmoElements.Translation(nameof(Center)))
        {
            new("radius",
                [new GizmoLine(diagonal * 0.13f, diagonal * 0.92f), new GizmoMarker(diagonal * 0.92f, 8f, GizmoMarkerKind.Square)],
                new AxisDrag(diagonal), new GizmoBinding(nameof(Radius)), idle, hover, active),
        };
        return elements;
    }
}
