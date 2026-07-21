namespace Ara3D.Studio.Samples.Selection;

public enum SelectAllMode
{
    All,
    None,
    Invert,
}

[Category(nameof(Categories.Selection))]
public class SelectAll : IModifier
{
    public SelectAllMode Mode = SelectAllMode.All;

    public FlowObject Eval(TriangleMesh3D mesh, EvalContext ctx)
    {
        var faceCount = mesh.FaceIndices.Count;
        var selection = Mode switch
        {
            SelectAllMode.None => SelectionSet.Empty(FlowAttribute.AttributeDomain.Face, faceCount),
            SelectAllMode.Invert => (ctx.GetFaceSelection(mesh) ?? SelectionSet.Empty(FlowAttribute.AttributeDomain.Face, faceCount)).Invert(),
            _ => SelectionSet.All(FlowAttribute.AttributeDomain.Face, faceCount),
        };
        return ctx.SelectFaces(mesh, SelectionCombine.Replace, selection);
    }
}
