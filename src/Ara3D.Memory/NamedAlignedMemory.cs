namespace Ara3D.Memory;

public class NamedAlignedMemory : INamedMemoryOwner
{
    public IMemoryOwner MemoryOwner { get; private set; }
    public string Name { get; }

    public NamedAlignedMemory(IMemoryOwner memoryOwner, string name)
    {
        Name = name;
        MemoryOwner = memoryOwner;
    }

    public void Dispose()
    {
        MemoryOwner?.Dispose();
        MemoryOwner = null;
    }

    public ByteSlice Bytes 
        => MemoryOwner.Bytes;
}