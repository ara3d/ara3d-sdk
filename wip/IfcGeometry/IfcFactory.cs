using System.Reflection;
using Ara3D.IO.StepParser;

namespace Ara3D.IfcGeometry;

/*
public class IfcFactory
{
    public Dictionary<uint, StepInstance> StepInstances  = new Dictionary<uint, StepInstance>();
    public Dictionary<string, Type> Types = new Dictionary<string, Type>();
    public Dictionary<uint, IfcClass> IfcInstances = new Dictionary<uint, IfcClass>();

    public IfcFactory()
    {
        Types = Assembly
            .GetExecutingAssembly()
            .GetTypes()
            .Where(t => t.Name.StartsWith("Ifc"))
            .ToDictionary(t => t.Name.ToUpperInvariant(), t => t);
    }

    public void AddValue(StepInstance inst)
        => StepInstances.Add(inst.Id, inst);
       

    public Type? GetIfcType(StepInstance inst)
        => Types.GetValueOrDefault(inst.EntityType);

    public IfcClass GetCreateIfcInstance(uint id)
    {
        if (IfcInstances.TryGetValue(id, out var ifcInstance))
            return ifcInstance;

        if (!StepInstances.TryGetValue(id, out var stepInstance))
            throw new Exception($"Could not find step instance {id}");

        throw new NotImplementedException();
        /*
            var type = Types.
            var vals = stepInstance.AttributeValues;
            var ctor = type
            if (vals.Count != )

            var ic = Activator.CreateInstance(type) as IfcClass;
            if (ic == null)
                throw new Exception($"Unable to create instance of {type}");
            IfcInstances.Add(inst.Id, ic);
    }
}
*/
