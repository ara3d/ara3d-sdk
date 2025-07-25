﻿using Ara3D.IO.StepParser;

namespace Ara3D.IO.IfcParser
{
    // https://standards.buildingsmart.org/IFC/RELEASE/IFC2x3/TC1/HTML/ifckernel/lexical/ifcreldefinesbyproperties.htm
    public class IfcPropSetRelation : IfcRelation
    {
        public IfcPropSetRelation(IfcGraph graph, StepDefinition lineData, StepValueId from, StepValueList to)
            : base(graph, lineData, from, to)
        {
        }

        public IfcPropSet PropSet
        {
            get
            {
                var node = Graph.GetNode(From);
                var r = node as IfcPropSet;
                if (r == null)
                    throw new System.Exception($"Expected a property set not {node} from id {From}");
                return r;
            }
        }
    }
}    