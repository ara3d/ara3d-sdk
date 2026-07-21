using Ara3D.Geometry;

namespace Ara3D.Studio.API;

/// <summary>
/// The FlowAttribute carrying a SelectionSet through the graph. One per domain
/// on a FlowObject; replaced by WithSelection, dropped by WithNewContent unless
/// the modifier declares the domain's indexing preserved.
/// </summary>
public sealed class SelectionAttribute : FlowAttribute
{
    public SelectionSet Selection { get; }

    public SelectionAttribute(SelectionSet selection)
        : base(CommonNames.Selection, selection.Domain, selection._indices)
        => Selection = selection;
}

public static class FlowSelectionExtensions
{
    public static SelectionSet? GetSelection(this FlowObject fo, FlowAttribute.AttributeDomain domain)
    {
        for (var i = 0; i < fo.Attributes.Count; i++)
            if (fo.Attributes[i] is SelectionAttribute sa && sa.Selection.Domain == domain)
                return sa.Selection;
        return null;
    }

    public static FlowObject WithSelection(this FlowObject fo, SelectionSet selection)
    {
        var attributes = new List<FlowAttribute>(fo.Attributes.Count + 1);
        for (var i = 0; i < fo.Attributes.Count; i++)
            if (!(fo.Attributes[i] is SelectionAttribute sa && sa.Selection.Domain == selection.Domain))
                attributes.Add(fo.Attributes[i]);
        attributes.Add(new SelectionAttribute(selection));
        return fo.WithNewAttributes(attributes);
    }

    public static FlowObject WithoutSelections(this FlowObject fo)
    {
        var attributes = new List<FlowAttribute>(fo.Attributes.Count);
        for (var i = 0; i < fo.Attributes.Count; i++)
            if (fo.Attributes[i] is not SelectionAttribute)
                attributes.Add(fo.Attributes[i]);
        return fo.WithNewAttributes(attributes);
    }

    public static SelectionSet? GetFaceSelection(this EvalContext ctx)
        => ctx.Input.GetSelection(FlowAttribute.AttributeDomain.Face);

    public static SelectionSet? GetVertexSelection(this EvalContext ctx)
        => ctx.Input.GetSelection(FlowAttribute.AttributeDomain.Vertex);

    /// <summary>
    /// The face selection if present and still valid for the given mesh; null otherwise.
    /// </summary>
    public static SelectionSet? GetFaceSelection(this EvalContext ctx, TriangleMesh3D mesh)
    {
        var selection = ctx.GetFaceSelection();
        return selection != null && selection.Matches(mesh.FaceIndices.Count) ? selection : null;
    }

    /// <summary>
    /// The vertex selection if present and still valid for the given mesh; null otherwise.
    /// </summary>
    public static SelectionSet? GetVertexSelection(this EvalContext ctx, TriangleMesh3D mesh)
    {
        var selection = ctx.GetVertexSelection();
        return selection != null && selection.Matches(mesh.Points.Count) ? selection : null;
    }
}
