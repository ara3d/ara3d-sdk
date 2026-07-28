using Ara3D.IfcLoader;

namespace Ara3D.Ifc.Mcp;

public readonly record struct IfcEntitySummary(int Id, string Type, string Name, string? GlobalId);

public readonly record struct IfcAttribute(int Index, string Value);

public readonly record struct IfcProperty(
    string PropertySet,
    string Name,
    string Kind,
    string MeasureType,
    string Value);

public readonly record struct IfcRelationEdge(int From, string FromType, int To, string ToType, string Kind);

public readonly record struct IfcTypeCount(string Type, int Count);

/// <summary>A slice of a larger result. <paramref name="Total"/> is the unpaged size, so a caller
/// can tell a complete answer from a truncated one without a second call.</summary>
public readonly record struct IfcPage<T>(int Total, int Skip, int Count, IReadOnlyList<T> Items);

public sealed record IfcSpatialNode(
    int Id,
    string Type,
    string Name,
    int ElementCount,
    IReadOnlyList<IfcSpatialNode> Children);

public static class IfcShapes
{
    public static IfcEntitySummary Summarize(this IfcEntity entity)
        => new(entity.Id, entity.GetEntityName(), entity.GetEntityLabel(), entity.GlobalIdOrNull());

    /// <summary>IfcRoot subtypes carry a 22-character base64 GUID in attribute 0, while other
    /// entities put unrelated data there; shape is the only cheap discriminator, so a value that
    /// does not look like one is reported as absent rather than as a bogus id.</summary>
    public static string? GlobalIdOrNull(this IfcEntity entity)
    {
        var text = entity.GetStringOrEmpty(0);
        if (text.Length != 22)
            return null;

        foreach (var c in text)
            if (!char.IsLetterOrDigit(c) && c != '_' && c != '$')
                return null;

        return text;
    }

    public static IReadOnlyList<IfcAttribute> Attributes(this IfcEntity entity)
    {
        var result = new IfcAttribute[entity.Attributes.Count];
        for (var i = 0; i < result.Length; i++)
            result[i] = new IfcAttribute(i, entity.Attributes[i].ToString());
        return result;
    }

    public static IfcPage<T> Page<T>(IReadOnlyList<T> items, int skip, int take)
    {
        if (skip < 0)
            throw new ArgumentOutOfRangeException(nameof(skip), "skip cannot be negative.");
        if (take < 1)
            throw new ArgumentOutOfRangeException(nameof(take), "take must be at least 1.");

        var start = Math.Min(skip, items.Count);
        var count = Math.Min(take, items.Count - start);
        var slice = new T[count];
        for (var i = 0; i < count; i++)
            slice[i] = items[start + i];
        return new IfcPage<T>(items.Count, start, count, slice);
    }
}
