using Ara3D.IO.StepParser;

namespace Ara3D.IfcLoader;

public class IfcInstanceTypeRelations
{
    public readonly Dictionary<int, int> InstancesToTypes = [];
    public readonly HashSet<int> TypeIds = [];

    public bool IsInstance(int id)
        => InstancesToTypes.ContainsKey(id);

    public IfcInstanceTypeRelations(StepDocument doc)
    {
        foreach (var def in doc.Definitions)
            if (def.NameToken.Match("IFCRELDEFINESBYTYPE"u8))
                ParseRelDefinesByType(def.AttributesToken.AsList(doc), doc);
    }

    public IEnumerable<int> InstanceIds 
        => InstancesToTypes.Keys;

    public void ParseRelDefinesByType(IReadOnlyList<StepToken> attributes, StepDocument doc)
    {
        // (0:GlobalId, 1:OwnerHistory, 2:Name, 3:Description, 4:RelatedObjects, 5:RelatingType)
       
        var typeId = attributes[5].AsId();
        TypeIds.Add(typeId);

        var instanceIdList = attributes[4].AsIds(doc);
        foreach (var id in instanceIdList)
            InstancesToTypes[id] = typeId;
    }
}

