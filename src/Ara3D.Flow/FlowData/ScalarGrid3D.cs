using Ara3D.Geometry;

namespace Ara3D.Studio.API;

/// <summary>
/// A regular 3D grid of scalar values over a bounded region (tracker studio-168) — the explicit,
/// stored-value cousin of the lazy <see cref="VoxelizedField"/>. Density fields, sampled SDFs,
/// simulation output. Its default rendering draws a box at every cell whose value is at or below
/// <see cref="Iso"/> (the "inside" test), reusing the voxel box path.
/// </summary>
public sealed class ScalarGrid3D
{
    public Bounds3D Bounds { get; }
    public Integer3 Dims { get; }
    public IReadOnlyList<float> Values { get; }

    /// <summary>Cells with value &lt;= Iso are considered inside and drawn.</summary>
    public float Iso { get; }

    public int NumColumns => Dims.A;
    public int NumRows => Dims.B;
    public int NumLayers => Dims.C;

    public ScalarGrid3D(Bounds3D bounds, Integer3 dims, IReadOnlyList<float> values, float iso = 0f)
    {
        Bounds = bounds;
        Dims = dims;
        Values = values;
        Iso = iso;
    }

    public float Get(int i, int j, int k)
        => Values[i + j * NumColumns + k * NumColumns * NumRows];
}
