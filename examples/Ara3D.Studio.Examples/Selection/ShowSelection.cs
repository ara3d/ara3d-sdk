namespace Ara3D.Studio.Samples.Selection;

/// <summary>
/// Visualizes the current face selection: selected faces in the highlight color,
/// the rest dimmed. Faces are unwelded so the highlight has crisp boundaries.
/// Passes geometry through unchanged when no selection is present.
/// </summary>
[Category(Cat.Select)]
public class ShowSelection : IModifier
{
    public Vector3 SelectedColor = new(1f, 0.55f, 0.05f);
    public Vector3 UnselectedColor = new(0.5f, 0.5f, 0.55f);

    public int SelectedFaces { get; private set; }

    public ColoredTriangleMesh3D Eval(TriangleMesh3D mesh, EvalContext ctx)
    {
        var selection = ctx.GetFaceSelection(mesh);
        SelectedFaces = selection?.Count ?? 0;
        ctx.Services.RefreshUI(this);
        var mask = selection?.ToMask() ?? new bool[mesh.FaceIndices.Count];
        var faceCount = mesh.FaceIndices.Count;
        var points = new Point3D[faceCount * 3];
        var faces = new Integer3[faceCount];
        var colors = new Vector3[faceCount * 3];
        for (var i = 0; i < faceCount; i++)
        {
            var face = mesh.FaceIndices[i];
            var color = mask[i] ? SelectedColor : UnselectedColor;
            points[i * 3 + 0] = mesh.Points[face.A];
            points[i * 3 + 1] = mesh.Points[face.B];
            points[i * 3 + 2] = mesh.Points[face.C];
            colors[i * 3 + 0] = color;
            colors[i * 3 + 1] = color;
            colors[i * 3 + 2] = color;
            faces[i] = new Integer3(i * 3, i * 3 + 1, i * 3 + 2);
        }
        return new TriangleMesh3D(points, faces).ToColored(colors);
    }
}
