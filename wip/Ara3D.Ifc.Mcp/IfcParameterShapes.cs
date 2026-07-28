namespace Ara3D.Ifc.Mcp;

/// <summary>Identifies a parameter by the set that carries it and its own name. Two sets routinely
/// use the same name, so the set is part of the identity rather than a decoration.</summary>
public readonly record struct IfcParamKey(string PropertySet, string Name)
{
    public sealed class IgnoreCase : IEqualityComparer<IfcParamKey>
    {
        public static readonly IgnoreCase Instance = new();

        public bool Equals(IfcParamKey a, IfcParamKey b)
            => string.Equals(a.PropertySet, b.PropertySet, StringComparison.OrdinalIgnoreCase)
               && string.Equals(a.Name, b.Name, StringComparison.OrdinalIgnoreCase);

        public int GetHashCode(IfcParamKey key)
            => HashCode.Combine(
                StringComparer.OrdinalIgnoreCase.GetHashCode(key.PropertySet),
                StringComparer.OrdinalIgnoreCase.GetHashCode(key.Name));
    }
}

/// <summary>One element's reading of a parameter. <paramref name="Number"/> is the numeric reading
/// of the same text, resolved when the index is built so comparisons never reparse.</summary>
public readonly record struct IfcParamValue(int ElementId, string Text, double? Number);

public readonly record struct IfcParameterInfo(
    string PropertySet,
    string Name,
    bool IsQuantity,
    string MeasureType,
    int ElementCount,
    int DistinctValues,
    int NumericCount,
    double? Min,
    double? Max,
    IReadOnlyList<string> SampleValues);

public readonly record struct IfcParameterTally(string Value, double? Number, int ElementCount);

public readonly record struct IfcParameterMatch(
    IfcEntitySummary Entity,
    string PropertySet,
    string Name,
    string Value);

/// <summary>A row of the parameter table. <paramref name="Values"/> lines up positionally with the
/// column list returned alongside it; a parameter the element does not carry reads as null.</summary>
public readonly record struct IfcParameterRow(IfcEntitySummary Entity, IReadOnlyList<string?> Values);
