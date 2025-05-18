namespace Ara3D.IO.G3D
{
    /// <summary>
    /// A G3dSubMesh is a section of a G3dMesh.
    /// It is defined so that it has one material.
    /// It is created after the G3dMesh is created. 
    /// </summary>
    public class G3dSubMesh
    {
        public readonly int Index;
        public readonly int IndexOffset;
        public readonly int IndexCount;
        public readonly int MaterialIndex;
        public readonly int MeshIndex;
        public readonly int NumFaces;

        public G3dSubMesh(G3D g3d, int subMeshIndex, int meshIndex)
        {
            Index = subMeshIndex;
            MeshIndex = meshIndex;
            MaterialIndex = g3d.SubmeshMaterials[Index];
            IndexCount = g3d.SubmeshIndexCount[Index];
            IndexOffset = g3d.SubmeshIndexOffsets[Index];
            NumFaces = IndexCount / 3;
        }
    }
}