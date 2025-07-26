using System.Collections.Generic;
using System.Linq;
using Ara3D.IO.StepParser;

namespace Ara3D.IO.IfcParser
{
    /// <summary>
    /// Always express a 1-to-many relation
    /// </summary>
    public class IfcRelation : IfcEntity
    {
        public StepValueId From { get; }
        public StepValueList To { get; }

        public IfcRelation(IfcGraph graph, StepDefinition lineData, StepValueId from, StepValueList to)
            : base(graph, lineData)
        {
            if (!IsIfcRoot)
                throw new System.Exception("Expected relation to be an IFC root entity");
            From = from;
            To = to;
        }

        public IEnumerable<uint> GetRelatedIds()
            => To.Values.Select(v => v.AsId());

        public IEnumerable<IfcNode> GetRelatedNodes()
            => Graph.GetNodes(GetRelatedIds());
    }
}