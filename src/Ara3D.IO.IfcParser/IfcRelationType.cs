using Ara3D.IO.StepParser;

namespace Ara3D.IO.IfcParser
{
    public class IfcRelationType : IfcRelation
    {
        public IfcRelationType(IfcGraph graph, StepInstance lineData, StepId from, StepList to)
            : base(graph, lineData, from, to)
        {
        }
    }
}