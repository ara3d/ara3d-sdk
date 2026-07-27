using System.Text;

namespace Ara3D.Ifc.Tests;

/// <summary>
/// Builds the STEP lines that attach a named property set to an existing element:
/// N IfcPropertySingleValue lines, one IfcPropertySet, and one IfcRelDefinesByProperties.
/// Ids are allocated sequentially from <see cref="NextId"/>; GlobalIds are deterministic hashes
/// of a caller-supplied key so repeated runs produce identical bytes.
/// </summary>
public sealed class IfcPropertySetBuilder
{
    public readonly int OwnerHistoryId;
    public int NextId;

    private readonly List<string> _lines = new();
    private readonly List<int> _ids = new();

    public IfcPropertySetBuilder(int firstNewId, int ownerHistoryId)
    {
        NextId = firstNewId;
        OwnerHistoryId = ownerHistoryId;
    }

    public IReadOnlyList<string> Lines
        => _lines;

    /// <summary>Every id allocated so far, in emission order.</summary>
    public IReadOnlyList<int> Ids
        => _ids;

    public void AddPropertySet(int elementId, string psetName, IReadOnlyList<IfcPropertyValue> props, string guidKey)
    {
        var propIds = new int[props.Count];
        for (var i = 0; i < props.Count; i++)
        {
            propIds[i] = Allocate();
            Emit(propIds[i], "IFCPROPERTYSINGLEVALUE",
                $"{IfcStepText.String(props[i].Name)},$,{props[i].NominalValue},$");
        }

        var psetId = Allocate();
        Emit(psetId, "IFCPROPERTYSET",
            $"{GuidLiteral(guidKey + ":pset")},#{OwnerHistoryId},{IfcStepText.String(psetName)},$,{IfcStepText.IdList(propIds)}");

        var relId = Allocate();
        Emit(relId, "IFCRELDEFINESBYPROPERTIES",
            $"{GuidLiteral(guidKey + ":rel")},#{OwnerHistoryId},$,$,(#{elementId}),#{psetId}");
    }

    private int Allocate()
    {
        _ids.Add(NextId);
        return NextId++;
    }

    private void Emit(int id, string typeName, string attributes)
        => _lines.Add(new StringBuilder().Append('#').Append(id).Append('=')
            .Append(typeName).Append('(').Append(attributes).Append(");").ToString());

    private static string GuidLiteral(string key)
        => IfcStepText.String(IfcGuid.Deterministic(key).ToIfcGuid());
}
