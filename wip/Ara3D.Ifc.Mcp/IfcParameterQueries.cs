using Ara3D.IfcLoader;

namespace Ara3D.Ifc.Mcp;

public enum IfcParamOp
{
    Exists,
    Eq,
    Ne,
    Contains,
    Gt,
    Ge,
    Lt,
    Le,
}

/// <summary>The read operations over <see cref="IfcParameterIndex"/>. Each walks only the entries
/// of the parameters it was asked about, never the model.</summary>
public static class IfcParameterQueries
{
    public static IfcParamOp ParseOp(string? text, bool hasValue)
        => text == null
            ? hasValue ? IfcParamOp.Eq : IfcParamOp.Exists
            : Enum.TryParse<IfcParamOp>(text, ignoreCase: true, out var op)
                ? op
                : throw new ArgumentException(
                    $"Unknown operator '{text}'. Use one of: {string.Join(", ", Enum.GetNames<IfcParamOp>()).ToLowerInvariant()}.");

    /// <summary>Every parameter matching the filters, with its value statistics. A type filter
    /// re-counts against that type only and drops parameters no element of it carries.</summary>
    public static IReadOnlyList<IfcParameterInfo> Catalogue(
        IfcParameterIndex index,
        string? name,
        string? propertySet,
        string? type)
    {
        var result = new List<IfcParameterInfo>();
        foreach (var key in index.Keys)
        {
            if (name != null && !key.Name.Contains(name, StringComparison.OrdinalIgnoreCase))
                continue;
            if (propertySet != null && !key.PropertySet.Contains(propertySet, StringComparison.OrdinalIgnoreCase))
                continue;

            var info = index.Info(key);
            if (type == null)
            {
                result.Add(info);
                continue;
            }

            var count = CountOfType(index, key, type);
            if (count > 0)
                result.Add(info with { ElementCount = count });
        }

        return result;
    }

    /// <summary>The distinct values of one parameter with how many elements hold each, ordered by
    /// element count. One call answers what would otherwise be a scan of every element.</summary>
    public static IReadOnlyList<IfcParameterTally> Tally(
        IfcParameterIndex index,
        IReadOnlyList<IfcParamKey> keys,
        string? type)
    {
        var counts = new Dictionary<string, HashSet<int>>(StringComparer.OrdinalIgnoreCase);
        foreach (var key in keys)
            foreach (var value in index.Values(key))
            {
                if (!MatchesType(index, value.ElementId, type))
                    continue;
                if (!counts.TryGetValue(value.Text, out var elements))
                    counts[value.Text] = elements = [];
                elements.Add(value.ElementId);
            }

        var result = new List<IfcParameterTally>(counts.Count);
        foreach (var pair in counts)
            result.Add(new IfcParameterTally(pair.Key, IfcParameterIndex.AsNumber(pair.Key), pair.Value.Count));

        result.Sort((a, b) => b.ElementCount.CompareTo(a.ElementCount));
        return result;
    }

    /// <summary>The elements whose parameter satisfies the predicate, in id order.</summary>
    public static IReadOnlyList<IfcParameterMatch> Find(
        IfcParameterIndex index,
        IfcEntityResolver resolver,
        IReadOnlyList<IfcParamKey> keys,
        IfcParamOp op,
        string? value,
        string? type)
    {
        var number = RequireNumberFor(op, value);
        var result = new List<IfcParameterMatch>();
        var seen = new HashSet<int>();

        foreach (var key in keys)
            foreach (var entry in index.Values(key))
            {
                if (!MatchesType(index, entry.ElementId, type) || !Matches(entry, op, value, number))
                    continue;
                if (!seen.Add(entry.ElementId))
                    continue;

                var entity = resolver.GetEntityOrDefault(entry.ElementId);
                if (entity is { } found)
                    result.Add(new IfcParameterMatch(found.Summarize(), key.PropertySet, key.Name, entry.Text));
            }

        result.Sort((a, b) => a.Entity.Id.CompareTo(b.Entity.Id));
        return result;
    }

    internal static bool MatchesType(IfcParameterIndex index, int elementId, string? type)
        => type == null || index.ElementType(elementId).Equals(type, StringComparison.OrdinalIgnoreCase);

    private static int CountOfType(IfcParameterIndex index, IfcParamKey key, string type)
    {
        var elements = new HashSet<int>();
        foreach (var value in index.Values(key))
            if (MatchesType(index, value.ElementId, type))
                elements.Add(value.ElementId);
        return elements.Count;
    }

    /// <summary>The ordering operators only mean anything against a number, so a non-numeric
    /// argument is rejected up front rather than silently matching nothing.</summary>
    private static double? RequireNumberFor(IfcParamOp op, string? value)
    {
        var number = value == null ? null : IfcParameterIndex.AsNumber(value);
        if (op is IfcParamOp.Gt or IfcParamOp.Ge or IfcParamOp.Lt or IfcParamOp.Le && number == null)
            throw new ArgumentException($"Operator '{op}' needs a numeric 'value'; got '{value}'.");
        if (op is not IfcParamOp.Exists && value == null)
            throw new ArgumentException($"Operator '{op}' needs a 'value'.");
        return number;
    }

    private static bool Matches(IfcParamValue entry, IfcParamOp op, string? value, double? number)
        => op switch
        {
            IfcParamOp.Exists => true,
            IfcParamOp.Eq => Equal(entry, value, number),
            IfcParamOp.Ne => !Equal(entry, value, number),
            IfcParamOp.Contains => entry.Text.Contains(value!, StringComparison.OrdinalIgnoreCase),
            _ => Ordered(entry, op, number),
        };

    /// <summary>Numbers compare as numbers when both sides read as one, so 3 finds 3.0.</summary>
    private static bool Equal(IfcParamValue entry, string? value, double? number)
        => entry.Number is { } left && number is { } right
            ? left.Equals(right)
            : string.Equals(entry.Text, value, StringComparison.OrdinalIgnoreCase);

    private static bool Ordered(IfcParamValue entry, IfcParamOp op, double? number)
        => entry.Number is { } left
           && number is { } right
           && op switch
           {
               IfcParamOp.Gt => left > right,
               IfcParamOp.Ge => left >= right,
               IfcParamOp.Lt => left < right,
               _ => left <= right,
           };
}
