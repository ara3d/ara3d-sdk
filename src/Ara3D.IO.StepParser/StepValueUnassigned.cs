using System.Diagnostics;

namespace Ara3D.IO.StepParser;

public class StepValueUnassigned : StepValue
{
    public static readonly StepValueUnassigned Default = new();

    public override string ToString()
        => "$";

    public static StepValueUnassigned Create(StepToken token)
    {
        Debug.Assert(token.Type == StepTokenType.Unassigned);
        var span = token.Slice;
        Debug.Assert(span.Length == 1);
        Debug.Assert(span.First() == '$');
        return Default;
    }
}