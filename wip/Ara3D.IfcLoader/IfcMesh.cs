namespace Ara3D.IfcLoader
{
    public class IfcMesh
    {
        public readonly uint Id;
        public readonly IntPtr ApiPtr;
        public readonly IntPtr MeshPtr;
        
        public readonly int NumVertices;
        public readonly int NumIndices;
        public readonly IntPtr Vertices;
        public readonly IntPtr Indices;
        public readonly IntPtr Color;
        public readonly IntPtr Transform;

        public IfcMesh(IntPtr apiPtr, IntPtr meshPtr)
        {
            ApiPtr = apiPtr;
            MeshPtr = meshPtr;
            Id = WebIfcDll.GetMeshId(ApiPtr, MeshPtr);
            NumIndices = WebIfcDll.GetNumIndices(ApiPtr, MeshPtr);
            NumVertices = WebIfcDll.GetNumVertices(ApiPtr, MeshPtr);
            Vertices = WebIfcDll.GetVertices(ApiPtr, MeshPtr);
            Indices = WebIfcDll.GetIndices(ApiPtr, MeshPtr);
            Color = WebIfcDll.GetColor(ApiPtr, MeshPtr);
            Transform = WebIfcDll.GetTransform(ApiPtr, MeshPtr);
        }
    }
}