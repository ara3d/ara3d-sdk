namespace Ara3D.Studio.Samples.Selection;

/// <summary>How a soft-selection weight falls from 1 at the center to 0 at the radius.</summary>
public enum FalloffCurve
{
    Linear,
    Smooth,
    Gaussian,
}

/// <summary>
/// Shared plumbing for soft (weighted) vertex selection. A soft selection is a per-vertex
/// weight in [0,1] carried as a vertex-domain FlowAttribute named "SoftSelection", alongside
/// the binary SelectionSet. Producers call WithSoftSelection; viewers and weighted deformers
/// read it back with GetSoftSelection (null when absent or stale for the given mesh).
/// </summary>
public static class SoftSelectionHelpers
{
    public const string AttributeName = FlowAttribute.CommonNames.SoftSelection;

    /// <summary>Heat-map endpoints shared by the gizmo and the SoftSelectionColor viewer, so the
    /// two can never drift: cold slate at weight 0, hot orange at weight 1.</summary>
    public static readonly Vector3 ColdColor = new(0.15f, 0.18f, 0.28f);
    public static readonly Vector3 HotColor = new(1f, 0.45f, 0.05f);

    /// <summary>Cool slate-blue for the falloff shell and radius grips — reads against both the
    /// orange heat map and a dark viewport.</summary>
    public static readonly Vector4 ShellColor = new(0.62f, 0.68f, 0.85f, 1f);

    /// <summary>Heat-map color at weight w as an opaque Vector4 for gizmo styles.</summary>
    public static Vector4 HeatColor(float w)
        => ColdColor.Lerp(HotColor, w).ToVector4(1f);

    public static FlowObject WithSoftSelection(this FlowObject fo, IReadOnlyList<float> weights)
    {
        var attributes = new List<FlowAttribute>(fo.Attributes.Count + 1);
        for (var i = 0; i < fo.Attributes.Count; i++)
            if (!IsSoftSelection(fo.Attributes[i]))
                attributes.Add(fo.Attributes[i]);
        attributes.Add(new FlowAttribute(AttributeName, FlowAttribute.AttributeDomain.Vertex, weights.ToArray()));
        return fo.WithNewAttributes(attributes);
    }

    public static float[]? GetSoftSelection(this EvalContext ctx, TriangleMesh3D mesh)
    {
        var attributes = ctx.Input.Attributes;
        for (var i = 0; i < attributes.Count; i++)
            if (IsSoftSelection(attributes[i]) && attributes[i].ArrayData is float[] weights && weights.Length == mesh.Points.Count)
                return weights;
        return null;
    }

    private static bool IsSoftSelection(FlowAttribute a)
        => a.Domain == FlowAttribute.AttributeDomain.Vertex && a.Name == AttributeName;

    /// <summary>Weight at world distance d from the falloff center of the given radius:
    /// 1 at the center, easing to 0 at (and beyond) the radius by the chosen curve.</summary>
    public static float Weight(FalloffCurve curve, float d, float radius)
    {
        if (radius <= 0f)
            return d <= 0f ? 1f : 0f;
        var t = Math.Clamp(1f - d / radius, 0f, 1f); // 1 at center, 0 at the radius
        return curve switch
        {
            FalloffCurve.Linear => t,
            FalloffCurve.Smooth => t * t * (3f - 2f * t),
            FalloffCurve.Gaussian => t <= 0f ? 0f : MathF.Exp(-6f * (1f - t) * (1f - t)),
            _ => t,
        };
    }

    /// <summary>Normalized distance (0 at center, 1 at the radius) where the weight equals w.
    /// Bisects <see cref="Weight"/> so the gizmo can never disagree with the falloff the modifier
    /// actually applies — do not re-derive analytic inverses per curve.</summary>
    public static float InverseWeight(FalloffCurve curve, float w)
    {
        float lo = 0f, hi = 1f; // Weight is monotone decreasing in d
        for (var i = 0; i < 24; i++)
        {
            var mid = (lo + hi) * 0.5f;
            if (Weight(curve, mid, 1f) > w)
                lo = mid;
            else
                hi = mid;
        }
        return (lo + hi) * 0.5f;
    }
}
