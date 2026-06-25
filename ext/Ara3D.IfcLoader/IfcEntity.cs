using Ara3D.IO.StepParser;
using Ara3D.Utils;

namespace Ara3D.IfcLoader;

public class IfcEntity
{
    public IfcEntity(int Id,
        StepDefinition Definition,
        IReadOnlyList<StepToken> Attributes,
        StepDocument Document)
    {
        this.Id = Id;
        this.Definition = Definition;
        this.Attributes = Attributes;
        this.Document = Document;
    }

    public int Id { get; init; }
    public StepDefinition Definition { get; init; }
    public IReadOnlyList<StepToken> Attributes { get; init; }
    public StepDocument Document { get; init; }

    public string GetIfcRootName()
        => GetString(2);

    public string GetIfcRootGlobalId()
        => GetString(0);

    public StepToken GetValue(int index)
        => Attributes.Count > index ? Attributes[index] : default;

    public string GetString(int index)
        => Attributes.Count > index ? Attributes[index].ToString().StripQuotes() : string.Empty;

    public IReadOnlyList<StepToken> GetArray(int index)
        => Attributes.Count > index ? GetAttribute(index).AsList(Document) : [];

    public int[] GetIdList(int index)
    {
        var vals = GetArray(index);
        var r = new int[vals.Count];
        for (var i = 0; i < vals.Count; i++)
            r[i] = vals[i].AsId();
        return r;
    }

    public double[] GetNumberList(int index)
    {
        var vals = GetArray(index);
        var r = new double[vals.Count];
        for (var i = 0; i < vals.Count; i++)
            r[i] = vals[i].AsNumber();
        return r;
    }

    public StepToken GetAttribute(int n)
        => Attributes[n];

    public int GetId(int index)
        => Attributes.Count > index ? Attributes[index].AsId() : -1;

    public double GetNumber(int index)
        => Attributes.Count > index ? Attributes[index].AsNumber() : 0;

    public uint GetEntityCode()
        => Definition.NameToken.Span.Fnv1a32bit();

    public string GetEntityName()
        => Definition.NameToken.ToString();
}