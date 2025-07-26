using System.Runtime.CompilerServices;

namespace Ara3D.IO.StepParser
{
    public class StepDefinition
    {
        public readonly long Id;
        public readonly StepValueEntity Entity;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public StepDefinition(long id, StepValueEntity entity)
        {
            Id = id;
            Entity = entity;
        }

        public override string ToString()
            => $"{Id} = {Entity};";
    }
}