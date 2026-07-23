using Ara3D.Geometry;

namespace Ara3D.Studio.API;

/// <summary>
/// An ordered set of rigid transforms flowing through the graph — the source for instancing,
/// cloning, and scattering (tracker studio-168). Kept as a named type (rather than a bare
/// <c>IReadOnlyList&lt;Matrix4x4&gt;</c>) so it has a stable identity for the render registry
/// and for generator/modifier signatures. Its default rendering instances a small marker at
/// each transform; a downstream modifier that pairs it with a mesh instances that mesh instead.
/// </summary>
public sealed class TransformList
{
    public IReadOnlyList<Matrix4x4> Transforms { get; }
    public int Count => Transforms.Count;

    public TransformList(IReadOnlyList<Matrix4x4> transforms)
        => Transforms = transforms ?? [];

    public static TransformList Create(IReadOnlyList<Matrix4x4> transforms)
        => new(transforms);
}
