using System.Runtime.InteropServices;

namespace Ara3D.IfcLoader
{
    /// <summary>
    /// This is the format of a Color that is generated from the WebIfcDll.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct IfcColor
    {
        public double R, G, B, A;
    }
}