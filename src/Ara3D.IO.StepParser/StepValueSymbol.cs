using System.Diagnostics;
using Ara3D.Memory;

namespace Ara3D.IO.StepParser;

public class StepValueSymbol : StepValue
{
    public readonly ByteSlice Name;

    public StepValueSymbol(ByteSlice name)
        => Name = name;

    public override string ToString()
        => $".{Name}.";

    public static StepValueSymbol Create(StepToken token)
    {
        Debug.Assert(token.Type == StepTokenType.Symbol);
        var span = token.Slice;
        Debug.Assert(span.Length >= 2);
        Debug.Assert(span.First() == '.');
        Debug.Assert(span.Last() == '.');
        return new StepValueSymbol(span.Trim(1, 1));
    }
}