using System.Runtime.InteropServices;

namespace Ara3D.IfcLoader
{
    /// <summary>
    /// This is the format of a Vertex that is generated from the WebIfcDll.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack=1)]
    public struct IfcVertex
    {
        public double PX, PY, PZ;
        public double NX, NY, NZ;
    }
}