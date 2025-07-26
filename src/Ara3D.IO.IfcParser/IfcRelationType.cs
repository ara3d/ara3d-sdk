using Ara3D.IO.StepParser;

namespace Ara3D.IO.IfcParser
{
    public class IfcRelationType : IfcRelation
    {
        public IfcRelationType(IfcGraph graph, StepDefinition lineData, StepValueId from, StepValueList to)
            : base(graph, lineData, from, to)
        {
        }
    }
}