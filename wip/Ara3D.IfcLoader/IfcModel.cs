namespace Ara3D.IfcLoader
{
    public class IfcModel
    {
        public IntPtr ApiPtr;
        public IntPtr ModelPtr;
        public IfcFile File;

        public IfcModel(IfcFile file, IntPtr apiPtr, IntPtr modelPtr)
        {
            ApiPtr = apiPtr;
            ModelPtr = modelPtr;
            File = file;
        }

        public IfcGeometry? GetGeometry(uint id)
        {
            var gPtr = WebIfcDll.GetGeometryFromId(ApiPtr, ModelPtr, id);
            return gPtr == IntPtr.Zero ? null : new IfcGeometry(ApiPtr, gPtr);
        }

        public int GetNumGeometries()
            => WebIfcDll.GetNumGeometries(ApiPtr, ModelPtr);

        public IEnumerable<IfcGeometry> GetGeometries()
        {
            var numGeometries = WebIfcDll.GetNumGeometries(ApiPtr, ModelPtr);
            for (int i = 0; i < numGeometries; ++i)
            {
                var gPtr = WebIfcDll.GetGeometryFromIndex(ApiPtr, ModelPtr, i);
                if (gPtr != IntPtr.Zero)
                    yield return new IfcGeometry(ApiPtr, gPtr);
            }
        }
    }
}