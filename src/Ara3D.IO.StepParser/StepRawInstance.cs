using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Ara3D.Memory;

namespace Ara3D.StepParser
{
    /// <summary>
    /// Contains information about where an instance is within a file.
    /// </summary>  
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [method: MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly unsafe struct StepRawInstance(uint id, ByteSlice type, byte* ptr)
    {
        public readonly uint Id = id;
        public readonly ByteSlice Type = type;
        public readonly byte* Ptr = ptr;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsValid()
            => Id > 0;
    }
}