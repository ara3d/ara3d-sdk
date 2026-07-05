using System.Runtime.CompilerServices;

namespace Ara3D.IO.StepParser
{
    public readonly struct StepDefinition
    {
        public readonly int Id;
        public readonly StepToken NameToken;
        public readonly StepToken AttributesToken;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public StepDefinition(int id, StepToken nameToken, StepToken attrToken)
        {
            Id = id;
            NameToken = nameToken;
            AttributesToken = attrToken;
        }
    }
}