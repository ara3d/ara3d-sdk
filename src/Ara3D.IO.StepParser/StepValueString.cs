using System.Diagnostics;
using Ara3D.Memory;

namespace Ara3D.IO.StepParser;

public class StepValueString : StepValue
{
    public readonly ByteSlice Value;

    public static StepValueString Create(StepToken token)
    {
        var span = token.Slice;
        Debug.Assert(token.Type == StepTokenType.SingleQuotedString || token.Type == StepTokenType.DoubleQuotedString);
        Debug.Assert(span.Length >= 2);
        Debug.Assert(span.First() == '\'' || span.First() == '"');
        Debug.Assert(span.Last() == '\'' || span.Last() == '"');
        return new StepValueString(span.Trim(1, 1));
    }

    public StepValueString(ByteSlice value)
        => Value = value;

    public override string ToString()
        => $"'{Value}'";
}