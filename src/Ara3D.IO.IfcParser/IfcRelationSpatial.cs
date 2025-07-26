using Ara3D.IO.StepParser;

namespace Ara3D.IO.IfcParser
{
    public class IfcRelationSpatial : IfcRelation
    {
        public IfcRelationSpatial(IfcGraph graph, StepDefinition lineData, StepValueId from, StepValueList to)
            : base(graph, lineData, from, to)
        {
        }
    }
}