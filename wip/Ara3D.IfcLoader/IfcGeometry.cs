namespace Ara3D.IfcLoader
{
    public class IfcGeometry
    {
        public readonly IntPtr ApiPtr;
        public readonly IntPtr GeometryPtr;
        public readonly int NumMeshes;
        public readonly uint Id;

        public IfcGeometry(IntPtr apiPtr, IntPtr geometryPtr)
        {
            ApiPtr = apiPtr;
            GeometryPtr = geometryPtr;
            Id = WebIfcDll.GetMeshId(ApiPtr, GeometryPtr);
            NumMeshes = WebIfcDll.GetNumMeshes(ApiPtr, GeometryPtr);
        }
        public IfcMesh GetMesh(int i) 
            => new IfcMesh(ApiPtr, WebIfcDll.GetMesh(ApiPtr, GeometryPtr, i));

        public int GetNumMeshes()
            => NumMeshes;

        public IEnumerable<IfcMesh> GetMeshes()
        {
            for (int i = 0; i < NumMeshes; ++i)
                yield return GetMesh(i);
        }
    }
}