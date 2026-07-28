using Ara3D.IO.StepParser;
using Ara3D.Utils;

namespace Ara3D.IfcLoader;

/// <summary>Reads the actual value out of a property token.
///
/// A property value is nearly always a typed STEP constructor such as <c>IFCLABEL('Massivhaus')</c>.
/// The tokenizer records only the type name at the attribute position and parks the payload in the
/// following token (<see cref="StepTokenExtensions.AsList"/> steps over it to keep attribute arity
/// right), so without unwrapping, every property reads back as the name of its own measure type
/// instead of its value.</summary>
public static class IfcPropValueExtensions
{
    /// <summary>The measure type of a property value, e.g. <c>IFCLABEL</c>; empty when the value
    /// is a bare token. For bare numbers (quantities), the property's own entity name is the only
    /// measure type on offer.</summary>
    public static string GetMeasureType(this IfcPropValue property)
        => property.Value is { IsEntity: true } token ? token.ToString() : string.Empty;

    /// <summary>The payload of a property value as decoded text: <c>Massivhaus</c> for
    /// <c>IFCLABEL('Massivhaus')</c>, <c>1.5</c> for a bare number, empty for unset (<c>$</c>).
    /// A list payload (e.g. an enumerated value) joins its items with a comma.</summary>
    public static string GetValueText(this IfcPropValue property, StepDocument document)
    {
        if (property.Value is not { } token || token.IsUnassignedOrRedeclared)
            return string.Empty;

        if (!token.IsEntity)
            return Clean(token.ToString(document));

        var payload = token.NextToken(document);
        if (!payload.IsList)
            return string.Empty;

        var items = payload.AsList(document);
        return items.Count switch
        {
            0 => string.Empty,
            1 => Clean(items[0].ToString(document)),
            _ => string.Join(", ", items.Select(item => Clean(item.ToString(document)))),
        };
    }

    private static string Clean(string text)
        => text.StripQuotes().DecodeIfc();
}
