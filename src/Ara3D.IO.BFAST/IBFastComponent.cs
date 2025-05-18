using System.IO;

namespace Ara3D.IO.BFAST;

/// <summary>
/// Anything that can be added to a BFAST must have a size and write to a stream.
/// </summary>
public interface IBFastComponent
{
    long GetSize();
    void Write(Stream stream);
}