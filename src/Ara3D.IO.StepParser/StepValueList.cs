using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Ara3D.Utils;

namespace Ara3D.IO.StepParser;

public class StepValueList : StepValue
{
    public List<StepValue> Values = new();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string ToString()
        => $"({Values.JoinStringsWithComma()})";
}