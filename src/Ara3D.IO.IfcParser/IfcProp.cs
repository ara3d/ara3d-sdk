using Ara3D.IO.StepParser;

namespace Ara3D.IO.IfcParser
{
    public class IfcProp : IfcNode
    {
        public readonly StepValue Value;

        public new string Name => this[0].AsString();
        public new string Description => this[1].AsString();

        public IfcProp(IfcGraph graph, StepDefinition lineData, StepValue value)
            : base(graph, lineData)
        {
            if (lineData.Count < 2) throw new System.Exception("Expected at least two values in the line data");
            if (!(lineData[0] is StepValueString)) throw new System.Exception("Expected the first value to be a string (Name)");
            Value = value;
        }
    }
}