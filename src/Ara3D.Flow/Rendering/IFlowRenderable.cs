using Ara3D.Geometry;

namespace Ara3D.Studio.API;

/// <summary>
/// One "instruction card" describing how a single kind of flowed value becomes drawable
/// (tracker studio-166). A renderer knows how to draw only a small set of primitives
/// (TriangleMesh3D / LineMesh3D / QuadMesh3D / Model3D / RenderModelData); everything else —
/// curves, surfaces, SDFs, voxels, transform lists — reaches the screen by converting itself,
/// here, into one of those.
///
/// The output does NOT have to be directly drawable: a renderer may return another flow type
/// that itself has a renderer (e.g. Sdf3D → VoxelizedField → Model3D). <see cref="FlowRenderRegistry"/>
/// keeps resolving until it reaches something directly renderable, so each card stays tiny and
/// composes with the others instead of duplicating their work.
///
/// Add support for a new type = add one class implementing this and register it. No renderer,
/// converter, or GL code changes.
/// </summary>
public interface IFlowRenderable
{
    /// <summary>The flow value type this card knows how to realize (e.g. typeof(Curve3D)).</summary>
    Type SourceType { get; }

    /// <summary>
    /// Turn a value of <see cref="SourceType"/> into geometry — either something directly
    /// renderable, or another flow type that has its own renderer. <paramref name="context"/>
    /// supplies sampling parameters (resolution, default bounds for unbounded types like SDFs).
    /// </summary>
    object ToRenderable(object value, FlowRenderContext context);
}
