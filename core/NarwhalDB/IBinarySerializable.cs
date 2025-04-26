using System;
using System.Collections.Generic;
using Ara3D.Buffers;
using Ara3D.Utils;

namespace Ara3D.NarwhalDB
{
    /// <summary>
    /// All objects placed in the database should implement this interface.
    /// If a class cannot be modified, you can use a proxy that implements the interface
    /// that reads and writes objects of the desired type. 
    /// </summary>
    public unsafe interface IBinarySerializable
    {
        int Size();
        object Read(ref IntPtr ptr,IReadOnlyList<string> strings);
        int Write(byte[] bytes, ref int offset, IndexedSet<string> strings);
    }
}