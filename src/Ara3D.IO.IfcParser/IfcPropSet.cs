using System.Collections.Generic;
using System.Diagnostics;
using Ara3D.IO.StepParser;

namespace Ara3D.IO.IfcParser
{
    // This merges two separate entity types: IfcPropertySet and IfcElementQuantity.
    // Both of which are derived from IfcPropertySetDefinition. 
    // This is something that can be referred to by a PropertySetRelation
    // An IfcElementQuantity has an additional "method of measurement" property.
    // https://standards.buildingsmart.org/IFC/RELEASE/IFC2x3/TC1/HTML/ifckernel/lexical/ifcpropertyset.htm
    // https://standards.buildingsmart.org/IFC/RELEASE/IFC2x3/TC1/HTML/ifcproductextension/lexical/ifcelementquantity.htm
    public class IfcPropSet : IfcNode
    {
        public readonly StepValueList PropertyIdValueList;

        public IfcPropSet(IfcGraph graph, StepDefinition lineData, StepValueList propertyIdValueList)
            : base(graph, lineData)
        {
            Debug.Assert(IsIfcRoot);
            Debug.Assert(lineData.AttributeValues.Count is 5 or 6);
            Debug.Assert(Type is "IFCPROPERTYSET" or "IFCELEMENTQUANTITY");
            PropertyIdValueList = propertyIdValueList;
        }

        public IEnumerable<IfcProp> GetProperties()
        {
            for (var i = 0; i < NumProperties; ++i)
            {
                var id = PropertyId(i);
                var node = Graph.GetNode(id);
                if (node is not IfcProp prop)
                    throw new System.Exception($"Expected a property not {node} from id {id}");
                yield return prop;
            }
        }

        public bool IsQuantity => LineData.AttributeValues.Count == 6;
        public string MethodOfMeasurement => IsQuantity ? this[4].AsString() : null;
        public int NumProperties => PropertyIdValueList.Values.Count;
        public uint PropertyId(int i) => PropertyIdValueList.Values[i].AsId();
    }
}