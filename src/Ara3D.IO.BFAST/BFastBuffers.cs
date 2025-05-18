using System;
using System.Collections.Generic;
using System.Linq;
using Ara3D.Memory;

namespace Ara3D.IO.BFAST;

public class BFastBuffers : IDisposable
{
    public IReadOnlyList<INamedMemoryOwner> Buffers { get; private set; }

    public BFastBuffers(IEnumerable<INamedMemoryOwner> buffers)
        => Buffers = buffers.ToList();

    public void Dispose()
    {
        foreach (var buffer in Buffers)
            buffer.Dispose();
        Buffers = null;
    }
}