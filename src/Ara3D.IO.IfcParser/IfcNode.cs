using Ara3D.IO.StepParser;

namespace Ara3D.IO.IfcParser
{
    public class IfcNode : IfcEntity
    {
        public IfcNode(IfcGraph graph, StepDefinition lineData)
            : base(graph, lineData)
        {
        }
    }
}