using Ara3D.IO.StepParser;

namespace Ara3D.IO.IfcParser
{
    public class IfcRelationSpatial : IfcRelation
    {
        public IfcRelationSpatial(IfcGraph graph, StepInstance lineData, StepId from, StepList to)
            : base(graph, lineData, from, to)
        {
        }
    }
}