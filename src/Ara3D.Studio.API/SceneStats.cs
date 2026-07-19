namespace Ara3D.Studio.API;

/// <summary>
/// Cheap counts over an evaluated scene, for automation asserts (studio-061). A workflow can
/// check these without touching geometry types directly.
/// </summary>
public readonly record struct SceneStats(int MeshCount, int InstanceCount, int TriangleCount, int VertexCount)
{
    public bool IsEmpty
        => MeshCount == 0 && InstanceCount == 0;

    public static readonly SceneStats Empty = new(0, 0, 0, 0);
}
