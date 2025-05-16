using System.Diagnostics;
using Ara3D.Memory;

namespace Ara3D.StepParser
{
    public readonly struct StepToken
    {
        public readonly ByteSlice Slice;
        public readonly StepTokenType Type;

        public StepToken(ByteSlice slice, StepTokenType type)
        {
            Slice = slice;
            Debug.Assert(slice.Length > 0);
            Type = type;
        }
    }
}