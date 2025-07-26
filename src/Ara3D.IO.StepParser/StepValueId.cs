using System.Diagnostics;

namespace Ara3D.IO.StepParser;

public class StepValueId : StepValue
{
    public readonly long Id;

    public StepValueId(long id)
        => Id = id;

    public override string ToString()
        => $"#{Id}";

    public static unsafe StepValueId Create(StepToken token)
    {
        Debug.Assert(token.Type == StepTokenType.Id);
        var span = token.Slice;
        Debug.Assert(span.Length >= 2);
        Debug.Assert(span.First() == '#');
        var id = 0u;
        for (var i = 1; i < span.Length; ++i)
        {
            Debug.Assert(span.Ptr[i] >= '0' && span.Ptr[i] <= '9');
            id = id * 10 + span.Ptr[i] - '0';
        }
        return new StepValueId(id);
    }
}