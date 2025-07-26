using System.Diagnostics;
using Ara3D.Memory;

namespace Ara3D.IO.StepParser;

public class StepValueNumber : StepValue
{
    public readonly double Value;
    
    public StepValueNumber(double value)
        => Value = value;

    public override string ToString()
        => $"{Value}";

    public static StepValueNumber Create(StepToken token)
    {
        Debug.Assert(token.Type == StepTokenType.Number);
        var span = token.Slice;
        return new(span.ToDouble());
    }
}