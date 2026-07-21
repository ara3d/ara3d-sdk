namespace Ara3D.Studio.Samples.Selection;

/// <summary>
/// Selects whole connected components, filtered by a face-count band —
/// e.g. small debris shells (low MaxFaces) or the main body (high MinFaces).
/// </summary>
[Category(nameof(Categories.Selection))]
public class SelectComponent : IModifier
{
    [Range(0, 1000000)] public int MinFaces = 0;
    [Range(0, 1000000)] public int MaxFaces = 1000000;
    public SelectionCombine Combine = SelectionCombine.Replace;

    public int ComponentCount { get; private set; }

    public FlowObject Eval(TriangleMesh3D mesh, EvalContext ctx)
    {
        var components = mesh.DerivedTopology().GetConnectedFaceComponents();
        ComponentCount = components.Count;
        ctx.Services.RefreshUI(this);
        var selected = new bool[mesh.FaceIndices.Count];
        foreach (var component in components)
        {
            if (component.Faces.Count < MinFaces || component.Faces.Count > MaxFaces)
                continue;
            foreach (var face in component.Faces)
                selected[(int)face] = true;
        }
        return ctx.SelectFaces(mesh, Combine, i => selected[i]);
    }
}
