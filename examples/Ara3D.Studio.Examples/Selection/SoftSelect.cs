namespace Ara3D.Studio.Samples.Selection;

/// <summary>
/// Soft (weighted) vertex selection inside a sphere: every vertex gets a weight in [0,1]
/// that is 1 at the center and eases to 0 at the radius by the chosen falloff curve. The
/// center rides a translation gizmo (an offset from the input bounds center); the falloff
/// sphere shows as three world-radius rings with iso-weight rings tracing the curve, and a
/// square grip on each shell ring drags the radius. Geometry passes through unchanged — the
/// weights travel as a vertex attribute for downstream tools (see SoftSelectionColor to visualize).
/// </summary>
[Category(Cat.Select)]
[Description("Weighted vertex selection inside a sphere: weight 1 at the center easing to 0 at the radius, driven by a gizmo.")]
public class SoftSelect : IModifier, IGizmoProvider
{
    public Vector3 Center;
    [Range(0f, 10f)] public float Radius = 0f; // 0 = auto: a small fraction of the model size
    public FalloffCurve Falloff = FalloffCurve.Smooth;

    public int AffectedVertices { get; private set; }

    // The world radius actually used by the last Eval (auto or explicit) — the gizmo draws this
    // so auto mode still shows a truthful sphere. Private: not inspectable.
    private float _effectiveRadius;

    public FlowObject Eval(TriangleMesh3D mesh, EvalContext ctx)
    {
        var bounds = mesh.DerivedBounds();
        var center = bounds.Center.Vector3 + Center;
        // Absolute world radius when set; otherwise a tight default that scales with the model.
        var radius = Radius > 0f ? Radius : (float)bounds.Size.Length * 0.15f;
        _effectiveRadius = radius;
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
        var r = _effectiveRadius;
        if (r <= 0f) // before first Eval: no shell yet, just the center handles
            return GizmoElements.Translation(nameof(Center));

        var shell = SoftSelectionHelpers.ShellColor;
        var elements = new List<GizmoElement>(GizmoElements.Translation(nameof(Center)));

        // Outer shell: three orthogonal world-radius rings reading as a sphere from any angle.
        for (var axis = 0; axis < 3; axis++)
            elements.Add(new($"shell-{axis}",
                [new GizmoRing(default, GizmoElements.AxisDirection(axis), r, 2.5f)],
                Idle: new GizmoStyle(shell, 0.5f))
                { Space = GizmoSpace.World });

        // Iso-weight rings (display-only, ground plane): where weight = 0.75/0.5/0.25. Spacing IS
        // the falloff curve; hotter and more opaque toward the center.
        foreach (var w in (float[])[0.75f, 0.5f, 0.25f])
            elements.Add(new($"iso-{(int)(w * 100)}",
                [new GizmoRing(default, Vector3.UnitZ,
                    r * SoftSelectionHelpers.InverseWeight(Falloff, w), 1.6f)],
                Idle: new GizmoStyle(SoftSelectionHelpers.HeatColor(w), 0.25f + 0.35f * w))
                { Space = GizmoSpace.World });

        // Radius grips: a square on each shell ring at +axis*r, dragged radially. The AxisDrag
        // delta is world distance along the axis and the grip sits at +axis*r, so binding Radius
        // with gain 1 is exact.
        for (var axis = 0; axis < 3; axis++)
        {
            var d = GizmoElements.AxisDirection(axis);
            elements.Add(new($"radius-{axis}",
                [new GizmoMarker(d * r, 7f, GizmoMarkerKind.Square)],
                new AxisDrag(d), new GizmoBinding(nameof(Radius)),
                new GizmoStyle(shell, 0.9f),
                new GizmoStyle(shell, 1f, 1.18f),
                new GizmoStyle(shell, 1f, 1.28f))
                { Space = GizmoSpace.World });
        }

        return elements;
    }
}
