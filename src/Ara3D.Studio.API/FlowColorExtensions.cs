using Ara3D.Geometry;

namespace Ara3D.Studio.API;

/// <summary>
/// The FlowAttribute carrying per-vertex colors through the graph. One per FlowObject;
/// replaced by WithColors, dropped by WithNewContent unless the modifier declares the
/// vertex domain's indexing preserved. This is the channel form of a colored mesh: any
/// geometry flowing through can carry an optional Color channel instead of needing a
/// dedicated colored-mesh wrapper type.
/// </summary>
public sealed class ColorAttribute : FlowAttribute
{
    public IReadOnlyList<Vector3> Colors { get; }

    public ColorAttribute(IReadOnlyList<Vector3> colors)
        : base(CommonNames.Color, AttributeDomain.Vertex, colors.ToArray())
        => Colors = colors;
}

public static class FlowColorExtensions
{
    /// <summary>The vertex-color channel if present; null otherwise.</summary>
    public static IReadOnlyList<Vector3>? GetColors(this FlowObject fo)
    {
        for (var i = 0; i < fo.Attributes.Count; i++)
            if (fo.Attributes[i] is ColorAttribute ca)
                return ca.Colors;
        return null;
    }

    /// <summary>Attaches the vertex-color channel, replacing any existing one.</summary>
    public static FlowObject WithColors(this FlowObject fo, IReadOnlyList<Vector3> colors)
    {
        var attributes = new List<FlowAttribute>(fo.Attributes.Count + 1);
        for (var i = 0; i < fo.Attributes.Count; i++)
            if (fo.Attributes[i] is not ColorAttribute)
                attributes.Add(fo.Attributes[i]);
        attributes.Add(new ColorAttribute(colors));
        return fo.WithNewAttributes(attributes);
    }

    public static FlowObject WithoutColors(this FlowObject fo)
    {
        var attributes = new List<FlowAttribute>(fo.Attributes.Count);
        for (var i = 0; i < fo.Attributes.Count; i++)
            if (fo.Attributes[i] is not ColorAttribute)
                attributes.Add(fo.Attributes[i]);
        return fo.WithNewAttributes(attributes);
    }

    /// <summary>
    /// The vertex-color channel if present and still valid for the given mesh
    /// (exactly one color per point); null otherwise.
    /// </summary>
    public static IReadOnlyList<Vector3>? GetColors(this FlowObject fo, TriangleMesh3D mesh)
    {
        var colors = fo.GetColors();
        return colors != null && colors.Count == mesh.Points.Count ? colors : null;
    }

    /// <summary>The vertex-color channel if present, independent of any mesh; null otherwise.</summary>
    public static IReadOnlyList<Vector3>? GetColors(this EvalContext ctx)
        => ctx.Input.GetColors();

    /// <summary>
    /// The vertex-color channel if present and still valid for the given mesh
    /// (exactly one color per point); null otherwise.
    /// </summary>
    public static IReadOnlyList<Vector3>? GetColors(this EvalContext ctx, TriangleMesh3D mesh)
        => ctx.Input.GetColors(mesh);
}
