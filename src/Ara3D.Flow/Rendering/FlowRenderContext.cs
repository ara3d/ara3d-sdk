using Ara3D.Geometry;

namespace Ara3D.Studio.API;

/// <summary>
/// Sampling parameters handed to an <see cref="IFlowRenderable"/> when it realizes a value
/// (tracker studio-166). These are VIEW-time settings — how finely to sample a curve/surface/
/// grid, and what box to sample an unbounded type (like an infinite <c>Sdf3D</c>) over — kept
/// out of the flowing data so the same value can be previewed coarse while scrubbing and fine
/// at rest. For now the host passes <see cref="Default"/>; per-object overrides
/// (FlowPresentation.RenderSettings) are a later refinement.
/// </summary>
public sealed class FlowRenderContext
{
    /// <summary>
    /// Samples per dimension: polyline points along a curve, columns/rows across a surface,
    /// voxels per axis when sampling a scalar field or SDF.
    /// </summary>
    public int Resolution { get; init; } = 32;

    /// <summary>
    /// The box used to sample types that carry no bounds of their own — an <c>Sdf3D</c> may be
    /// infinite, so it can only be voxelized over an explicit region. Also the size of the
    /// fallback placeholder box drawn for an unregistered type.
    /// </summary>
    public Bounds3D DefaultBounds { get; init; }
        = new(new Point3D(-10, -10, -10), new Point3D(10, 10, 10));

    public static FlowRenderContext Default { get; } = new();
}
