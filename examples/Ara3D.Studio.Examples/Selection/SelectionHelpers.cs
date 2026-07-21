namespace Ara3D.Studio.Samples.Selection;

public enum SelectionCombine
{
    Replace,
    Add,
    Subtract,
    Intersect,
}

/// <summary>
/// Shared plumbing for selection-creator modifiers: build a face SelectionSet from
/// a predicate, combine it with any existing selection, and return a FlowObject
/// (creators must return FlowObject so the selection attribute survives the pipeline).
/// </summary>
public static class SelectionHelpers
{
    public static FlowObject SelectFaces(this EvalContext ctx, TriangleMesh3D mesh, SelectionCombine combine, Func<int, bool> predicate)
    {
        var faceCount = mesh.FaceIndices.Count;
        var indices = new List<int>();
        for (var i = 0; i < faceCount; i++)
            if (predicate(i))
                indices.Add(i);
        return ctx.SelectFaces(mesh, combine, SelectionSet.Create(FlowAttribute.AttributeDomain.Face, indices, faceCount));
    }

    public static FlowObject SelectFaces(this EvalContext ctx, TriangleMesh3D mesh, SelectionCombine combine, SelectionSet selection)
    {
        var existing = ctx.GetFaceSelection(mesh);
        var combined = existing == null || combine == SelectionCombine.Replace
            ? selection
            : combine switch
            {
                SelectionCombine.Add => existing.Union(selection),
                SelectionCombine.Subtract => existing.Subtract(selection),
                SelectionCombine.Intersect => existing.Intersect(selection),
                _ => selection,
            };
        return ctx.Input
            .WithNewContent(mesh, FlowAttribute.AttributeDomainMask.All)
            .WithSelection(combined);
    }
}
