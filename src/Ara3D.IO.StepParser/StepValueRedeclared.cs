using System.Diagnostics;

namespace Ara3D.IO.StepParser;

public class StepValueRedeclared : StepValue
{
    public static readonly StepValueRedeclared Default = new();

    public override string ToString()
        => "*";

    public static StepValueRedeclared Create(StepToken token)
    {
        Debug.Assert(token.Type == StepTokenType.Redeclared);
        var span = token.Slice;
        Debug.Assert(span.Length == 1);
        Debug.Assert(span.First() == '*');
        return Default;
    }
}