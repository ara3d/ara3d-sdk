namespace Ara3D.Studio.Samples.Selection;

/// <summary>
/// Deletes the selected faces (or keeps only them, with KeepSelected).
/// Returns a bare mesh, so the now-invalid selection is dropped by the pipeline.
/// </summary>
[Category(Cat.Select)]
[Description("Deletes the selected faces, or with KeepSelected keeps only them.")]
public class DeleteSelected : IModifier
{
    public bool KeepSelected;

    public TriangleMesh3D Eval(TriangleMesh3D mesh, EvalContext ctx)
    {
        var selection = ctx.GetFaceSelection(mesh);
        if (selection == null || selection.IsEmpty)
            return mesh;
        return KeepSelected
            ? mesh.ExtractFaces(selection.Indices).Mesh
            : mesh.DeleteFaces(selection.Indices);
    }
}
