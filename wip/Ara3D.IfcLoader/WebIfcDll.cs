using System.Runtime.InteropServices;

namespace Ara3D.IfcLoader
{
    public static class WebIfcDll
    {
        // NOTE: make sure the DLL is in the same directory as the built DLLs or Executable. 
        private const string DllName = "web-ifc-dll.dll"; 
        
        // InitializeApi
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr InitializeApi();

        // FinalizeApi
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void FinalizeApi(IntPtr api);

        // LoadModel
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr LoadModel(IntPtr api, string fileName);

        // GetGeometry
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr GetGeometryFromId(IntPtr api, IntPtr model, uint id);

        // GetGeometry
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetNumGeometries(IntPtr api, IntPtr model);

        // GetGeometry
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr GetGeometryFromIndex(IntPtr api, IntPtr model, int index);

        // GetGeometryId
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint GetGeometryId(IntPtr api, IntPtr geometry);

        // GetNumMeshes
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetNumMeshes(IntPtr api, IntPtr geometry);

        // GetMesh
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr GetMesh(IntPtr api, IntPtr geometry, int index);

        // GetMeshId
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint GetMeshId(IntPtr api, IntPtr mesh);

        // GetTransform
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr GetTransform(IntPtr api, IntPtr mesh);

        // GetColor
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr GetColor(IntPtr api, IntPtr mesh);

        // GetNumVertices
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetNumVertices(IntPtr api, IntPtr mesh);

        // GetVertices
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr GetVertices(IntPtr api, IntPtr mesh);

        // GetNumIndices
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetNumIndices(IntPtr api, IntPtr mesh);

        // GetIndices
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr GetIndices(IntPtr api, IntPtr mesh);
    }
}