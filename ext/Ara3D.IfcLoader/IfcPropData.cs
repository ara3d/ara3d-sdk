using Ara3D.IO.StepParser;
using Ara3D.Utils;
using System.Diagnostics;
using Ara3D.IfcTypes.Ifc2x3;

namespace Ara3D.IfcLoader;    

public enum IfcPropKind
{
    SingleValue,
    ListValue,
    EnumeratedValue,
    BoundedValue,
    ReferenceValue,
    TableValue,
    ComplexProperty,
    Quantity,
    Other,
}

public record struct IfcPropSet(
    int Id,
    string Name,
    int[] Ids); 

public record struct IfcPropValue(
    int Id,
    string Name,
    string EntityName,
    IfcPropKind Kind,
    StepToken? Value);

public sealed class IfcPropData
{
    public readonly List<(int Id, Exception Ex)> Errors = [];
    public readonly Dictionary<int, IfcPropValue> PropValues = [];
    public readonly Dictionary<int, IfcPropSet> PropSets = [];
    public readonly MultiDictionary<int, int> ObjectToPropSets = [];

    public IfcPropData(IfcFile file)
    {
        foreach (var e in file.EntityResolver.GetEntities())
        {
            var id = e.Id;
            var entityCode = e.GetEntityCode();

            switch (entityCode)
            {
                case IfcPropertySet.ENTITY_CODE:
                    Debug.Assert(e.Attributes.Count == 5);
                    PropSets[id] = new IfcPropSet(id, e.GetString(2), e.GetIdList(4));
                    break;

                case IfcRelDefinesByProperties.ENTITY_CODE:
                    ParseRelDefinesByProperties(e);
                    break;

                case IfcPropertySingleValue.ENTITY_CODE:
                    ParseProperty(e, 2, IfcPropKind.SingleValue);
                    break;

                case IfcPropertyListValue.ENTITY_CODE:
                    ParseProperty(e, 2, IfcPropKind.ListValue);
                    break;

                case IfcPropertyEnumeratedValue.ENTITY_CODE:
                    ParseProperty(e, 2, IfcPropKind.EnumeratedValue);
                    break;

                case IfcPropertyBoundedValue.ENTITY_CODE:
                    // NOTE: only stores lower (3), upper is (4)
                    ParseProperty(e, 3, IfcPropKind.BoundedValue);
                    break;

                case IfcPropertyReferenceValue.ENTITY_CODE:
                    ParseProperty(e, 3, IfcPropKind.ReferenceValue);
                    break;

                case IfcPropertyTableValue.ENTITY_CODE:
                    ParseProperty(e, 4, IfcPropKind.TableValue);
                    break;

                case IfcTypes.Ifc4.IfcComplexProperty.ENTITY_CODE:
                    ParseProperty(e, 3, IfcPropKind.ComplexProperty);
                    break;

                case IfcElementQuantity.ENTITY_CODE:
                    ParseElementQuantity(e);
                    break;

                case IfcQuantityLength.ENTITY_CODE:
                case IfcQuantityArea.ENTITY_CODE:
                case IfcQuantityVolume.ENTITY_CODE:
                case IfcQuantityCount.ENTITY_CODE:
                case IfcQuantityWeight.ENTITY_CODE:
                case IfcQuantityTime.ENTITY_CODE:
                    ParseProperty(e, 3, IfcPropKind.Quantity);
                    break;
            }
        } ;
    }

    public void ParseRelDefinesByProperties(IfcEntity entity)
    {
        Debug.Assert(entity.Attributes.Count == 6);

        // RelatedObjects at 4, RelatingPropertyDefinition at 5
        var objectIds = entity.GetIdList(4);
        var propSetId = entity.GetId(5);
        if (propSetId == 0 || objectIds.Length == 0)
            return;
        foreach (var objectId in objectIds)
            ObjectToPropSets.Add(objectId, propSetId);
    }

    public void ParseProperty(IfcEntity e, int valIndex, IfcPropKind kind)
    {
        // (Name, Description, NominalValue, Unit)
        var name = e.GetString(0).StripQuotes();
        var value = e.GetValue(valIndex);

        PropValues[e.Id] = new (e.Id, name, e.GetEntityName(), kind, value);
    }

    public void ParseElementQuantity(IfcEntity e)
    {
        // (Name, Desc, MethodOfMeasurement, Quantities)
        var name = e.GetString(0).StripQuotes();
        var qtyIds = e.GetIdList(3);

        // Treat as a property set (very convenient downstream)
        PropSets[e.Id] = new IfcPropSet(e.Id, name, qtyIds);
    }

    public IEnumerable<IfcPropValue> GetProperties(IfcPropSet propSet)
    {
        foreach (var id in propSet.Ids)
            if (PropValues.TryGetValue(id, out var propValue))
                yield return propValue;
    }

    public IEnumerable<IfcPropSet> GetPropSets(int objectId)
        => ObjectToPropSets.TryGetValue(objectId, out var propSetIds)
            ? propSetIds.Select(id => PropSets[id])
            : [];

    public IEnumerable<IfcPropValue> GetProperties(int objectId)
        => GetPropSets(objectId).SelectMany(GetProperties);
}

public static class IfcPropDataExtensions
{
    public static IEnumerable<IfcPropValue> GetProperties(this IfcPropSet propSet, IfcPropData pd)
        => pd.GetProperties(propSet);


    public static IEnumerable<IfcPropValue> GetProperties(this IfcPropData pd, IfcPropSet propSet)
        => pd.GetProperties(propSet);

}