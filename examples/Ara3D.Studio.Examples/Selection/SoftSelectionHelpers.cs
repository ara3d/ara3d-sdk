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
    public const string AttributeName = "SoftSelection";

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
}
