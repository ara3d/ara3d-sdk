using System.Collections.Generic;

namespace Ara3D.Serialization.G3D
{
    /// <summary>
    /// A G3dMesh is a section of the G3D data that defines a collection of meshes.
    /// </summary>
    public class G3dMesh
    {
        public readonly int Index;
        public readonly IReadOnlyList<G3dSubMesh> Submeshes;

        public G3dMesh(int index, IReadOnlyList<G3dSubMesh> subMeshes)
        {
            Index = index;
            Submeshes = subMeshes;
        }
    }
}
