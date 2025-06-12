using Ara3D.Collections;

namespace Ara3D.Geometry;

public interface IVoxels : IReadOnlyList3D<Voxel>
{
    Bounds3D Bounds { get; }
    Voxel GetVoxel(int i, int j, int k);
}