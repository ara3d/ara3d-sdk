using Ara3D.IO.StepParser;

namespace Ara3D.IO.IfcParser
{
    public class IfcRelationAggregate : IfcRelation
    {
        public IfcRelationAggregate(IfcGraph graph, StepDefinition lineData, StepValueId from, StepValueList to)
            : base(graph, lineData, from, to)
        {
        }
    }
}