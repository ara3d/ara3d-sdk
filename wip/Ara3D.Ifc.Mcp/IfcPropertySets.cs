using Ara3D.IfcLoader;
using Ara3D.Utils;

namespace Ara3D.Ifc.Mcp;

public readonly record struct IfcPropSetInfo(int Id, string Name, bool IsQuantitySet, IReadOnlyList<int> MemberIds);

/// <summary>Reads the property sets attached to an element straight from their entities.
///
/// This exists because <see cref="IfcPropData"/> mis-parses <c>IFCELEMENTQUANTITY</c>: it takes
/// the name from attribute 0 and the members from attribute 3, but the entity is
/// <c>(GlobalId, OwnerHistory, Name, Description, MethodOfMeasurement, Quantities)</c>, so the
/// name comes back as the GlobalId GUID and the member list is always empty. Every quantity set
/// in a model therefore reads as empty and unnamed. The upstream fix belongs in
/// <c>ext/Ara3D.IfcLoader/IfcPropData.cs</c> (name at 2, members at 5), which is off limits here.
/// The object-to-set mapping in <see cref="IfcPropData.ObjectToPropSets"/> is correct and reused.</summary>
public static class IfcPropertySets
{
    private const int PropertySetNameIndex = 2;
    private const int PropertySetMembersIndex = 4;
    private const int ElementQuantityNameIndex = 2;
    private const int ElementQuantityMembersIndex = 5;

    public static IReadOnlyList<IfcPropSetInfo> ForElement(IfcSession session, int elementId)
    {
        if (!session.Properties.ObjectToPropSets.TryGetValue(elementId, out var setIds))
            return [];

        var result = new List<IfcPropSetInfo>();
        foreach (var setId in setIds)
        {
            var entity = session.Resolver.GetEntityOrDefault(setId);
            if (entity != null)
                result.Add(Read(entity));
        }

        return result;
    }

    public static IfcPropSetInfo Read(IfcEntity entity)
    {
        var isQuantitySet = entity.GetEntityName().Equals("IFCELEMENTQUANTITY", StringComparison.OrdinalIgnoreCase);
        var nameIndex = isQuantitySet ? ElementQuantityNameIndex : PropertySetNameIndex;
        var membersIndex = isQuantitySet ? ElementQuantityMembersIndex : PropertySetMembersIndex;
        return new IfcPropSetInfo(
            entity.Id,
            entity.GetStringOrEmpty(nameIndex).DecodeIfc(),
            isQuantitySet,
            entity.GetIdList(membersIndex));
    }
}
