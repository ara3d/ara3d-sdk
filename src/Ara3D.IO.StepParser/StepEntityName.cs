using Ara3D.Memory;

namespace Ara3D.IO.StepParser;

public class StepEntityName
{
    public readonly ByteSlice Name;

    public StepEntityName(ByteSlice name)
        => Name = name;
    
    public override string ToString()
        => Name.ToAsciiString();
}