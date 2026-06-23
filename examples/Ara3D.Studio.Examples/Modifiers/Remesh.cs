namespace Ara3D.Studio.Samples.Modifiers;

/// <summary>
/// Remeshes a triangle mesh toward a target edge length using split, collapse, and tangential smoothing.
/// Suitable as a lightweight preprocessing step before FEM meshing.
/// </summary>
[Category(nameof(Categories.Meshes))]
public class Remesh : IModifier
{
    /// <summary>Target edge length in model units. Zero uses the input mesh average edge length.</summary>
    [Range(0f, 1000f)]
    public float TargetEdgeLength;

    [Range(1, 20)]
    public int Iterations = 3;

    [Range(0, 10)]
    public int SmoothingIterations = 2;

    [Range(0f, 1f)]
    public float SmoothingStrength = 0.5f;

    public bool CollapseShortEdges = true;

    public TriangleMesh3D Eval(TriangleMesh3D mesh)
    {
        var settings = new IsotropicRemesher.Settings(
            TargetEdgeLength,
            Iterations,
            SmoothingIterations,
            SmoothingStrength,
            CollapseShortEdges: CollapseShortEdges);

        return mesh.Remesh(settings);
    }
}
