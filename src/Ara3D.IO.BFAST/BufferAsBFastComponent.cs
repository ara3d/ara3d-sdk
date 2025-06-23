using System.IO;
using Ara3D.Memory;

namespace Ara3D.IO.BFAST;

/// <summary>
/// A wrapper around a buffer so that it can be used as a BFAST component 
/// </summary>
public class BufferAsBFastComponent : IBFastComponent
{
    public BufferAsBFastComponent(IBuffer buffer)
        => Buffer = buffer;
    public IBuffer Buffer { get; }
    public void Write(Stream stream) => stream.WriteBuffer(Buffer);
    public long GetSize() => Buffer.NumBytes();
}