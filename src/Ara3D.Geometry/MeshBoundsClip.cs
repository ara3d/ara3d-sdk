namespace Ara3D.Geometry;

/// <summary>Clips triangle meshes to axis-aligned bounding boxes.</summary>
public static class MeshBoundsClip
{
    /// <summary>Returns six planes whose positive half-spaces contain the bounds interior.</summary>
    public static IReadOnlyList<Plane> GetInsideClipPlanes(this Bounds3D bounds)
    {
        var (min, max) = bounds;
        return
        [
            new Plane(Vector3.UnitX, -min.X),
            new Plane(-Vector3.UnitX, max.X),
            new Plane(Vector3.UnitY, -min.Y),
            new Plane(-Vector3.UnitY, max.Y),
            new Plane(Vector3.UnitZ, -min.Z),
            new Plane(-Vector3.UnitZ, max.Z),
        ];
    }

    /// <summary>Removes mesh geometry outside the bounds.</summary>
    public static TriangleMesh3D ClipToBounds(this TriangleMesh3D mesh, Bounds3D bounds, float eps = MeshPlaneClip.DefaultEpsilon)
    {
        var result = mesh;
        foreach (var plane in bounds.GetInsideClipPlanes())
            result = result.ClipAbove(plane, eps);
        return result;
    }

    /// <summary>Removes mesh geometry outside the bounds expanded by uniform padding on each side.</summary>
    public static TriangleMesh3D ClipToBounds(this TriangleMesh3D mesh, Bounds3D bounds, Number padding, float eps = MeshPlaneClip.DefaultEpsilon)
        => mesh.ClipToBounds(bounds.Expand(new Vector3(padding, padding, padding)), eps);
}
