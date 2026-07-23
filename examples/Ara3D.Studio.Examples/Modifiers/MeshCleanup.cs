namespace Ara3D.Studio.Samples.Modifiers;

/// <summary>
/// Drops faces that carry no surface area: either topologically degenerate (two of the three
/// indices are equal, so the triangle is really an edge or a point) or geometrically degenerate
/// (a sliver whose area falls below the tolerance). Use after imports or boolean operations that
/// can leave zero-area triangles behind, which confuse normals, shading, and downstream remeshing.
/// A tolerance of 0 removes only the index-degenerate faces and leaves all real geometry untouched.
/// </summary>
[Category(Cat.Cleanup)]
[Description("Removes zero-area faces: index-degenerate triangles always, thin slivers below the area tolerance.")]
public class RemoveDegenerateFaces : IModifier
{
    /// <summary>Faces with area below this value are dropped. 0 keeps every non-index-degenerate face.</summary>
    [Range(0f, 0.001f)] public float AreaTolerance = 0f;

    public int FacesBefore { get; private set; }
    public int FacesAfter { get; private set; }

    public TriangleMesh3D Eval(TriangleMesh3D mesh, EvalContext ctx)
    {
        var kept = new List<int>(mesh.FaceIndices.Count);
        for (var i = 0; i < mesh.FaceIndices.Count; i++)
        {
            var f = mesh.FaceIndices[i];
            if (f.A == f.B || f.B == f.C || f.C == f.A)
                continue;
            if (FaceArea(mesh, f) <= AreaTolerance)
                continue;
            kept.Add(i);
        }

        FacesBefore = mesh.FaceIndices.Count;
        FacesAfter = kept.Count;
        ctx.Services.RefreshUI(this);
        return mesh.ExtractFaces(kept).Mesh;
    }

    static float FaceArea(TriangleMesh3D mesh, Integer3 f)
    {
        var a = mesh.Points[f.A].Vector3;
        var b = mesh.Points[f.B].Vector3;
        var c = mesh.Points[f.C].Vector3;
        return Vector3.Cross(b - a, c - a).Length * 0.5f;
    }
}

/// <summary>
/// Removes faces whose three vertices duplicate a face already seen, regardless of winding order
/// (the indices are sorted before comparison, so a face and its flipped copy count as the same).
/// The first occurrence of each triangle is kept. Use to clean up meshes stitched from overlapping
/// patches or exported by tools that emit the same triangle more than once.
/// </summary>
[Category(Cat.Cleanup)]
[Description("Removes faces that duplicate an earlier face (any winding), keeping the first occurrence.")]
public class RemoveDuplicateFaces : IModifier
{
    public int FacesBefore { get; private set; }
    public int FacesAfter { get; private set; }

    public TriangleMesh3D Eval(TriangleMesh3D mesh, EvalContext ctx)
    {
        var seen = new HashSet<(int, int, int)>(mesh.FaceIndices.Count);
        var kept = new List<int>(mesh.FaceIndices.Count);
        for (var i = 0; i < mesh.FaceIndices.Count; i++)
        {
            var f = mesh.FaceIndices[i];
            if (seen.Add(SortedKey(f.A, f.B, f.C)))
                kept.Add(i);
        }

        FacesBefore = mesh.FaceIndices.Count;
        FacesAfter = kept.Count;
        ctx.Services.RefreshUI(this);
        return mesh.ExtractFaces(kept).Mesh;
    }

    static (int, int, int) SortedKey(int a, int b, int c)
    {
        if (a > b) (a, b) = (b, a);
        if (b > c) (b, c) = (c, b);
        if (a > b) (a, b) = (b, a);
        return (a, b, c);
    }
}

/// <summary>
/// Removes small disconnected shells: each connected component (a group of faces joined edge to
/// edge) whose face count is below the threshold is discarded, and the surviving faces are kept.
/// Use to strip stray fragments, floating debris, and noise islands left by scanning or booleans,
/// while preserving the main body. If every component is below the threshold nothing is removed
/// and the original mesh is returned unchanged.
/// </summary>
[Category(Cat.Cleanup)]
[Description("Removes disconnected shells smaller than a face-count threshold, keeping the larger bodies.")]
public class RemoveSmallShells : IModifier
{
    /// <summary>Components with fewer faces than this are removed.</summary>
    [Range(0, 10000)] public int MinFaces = 10;

    public int ComponentsBefore { get; private set; }
    public int ComponentsRemoved { get; private set; }

    public TriangleMesh3D Eval(TriangleMesh3D mesh, EvalContext ctx)
    {
        var components = mesh.DerivedTopology().GetConnectedFaceComponents();
        var kept = new List<int>(mesh.FaceIndices.Count);
        var removed = 0;
        foreach (var component in components)
        {
            if (component.Faces.Count < MinFaces)
            {
                removed++;
                continue;
            }
            foreach (var faceId in component.Faces)
                kept.Add((int)faceId);
        }

        ComponentsBefore = components.Count;
        ComponentsRemoved = removed;
        ctx.Services.RefreshUI(this);
        return kept.Count == 0 ? mesh : mesh.ExtractFaces(kept).Mesh;
    }
}
