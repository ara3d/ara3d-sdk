using Ara3D.IfcLoader;
using Ara3D.IO.StepParser;
using Ara3D.Utils;

namespace Ara3D.Ifc.Mcp;

/// <summary>Turns a raw property token into the measure type and the value a reader wants.</summary>
public static class IfcPropertyText
{
    /// <summary>A property value is nearly always a typed STEP constructor such as
    /// <c>IFCLABEL('Massivhaus')</c>. The tokenizer puts only the type name at the attribute
    /// position and parks the payload in the following token, so without unwrapping, every
    /// property reads back as the name of its own measure type instead of its value.</summary>
    public static (string Type, string Value) Read(IfcPropValue property, StepDocument document)
    {
        if (property.Value is not { } token || token.IsUnassignedOrRedeclared)
            return ("", "");

        // A quantity holds a bare number, so its own entity name is the only measure type on offer.
        if (!token.IsEntity)
            return (property.EntityName, Clean(token.ToString(document)));

        var measure = token.ToString();
        var payload = token.NextToken(document);
        if (!payload.IsList)
            return (measure, "");

        var items = payload.AsList(document);
        return items.Count switch
        {
            0 => (measure, ""),
            1 => (measure, Clean(items[0].ToString(document))),
            _ => (measure, string.Join(", ", items.Select(item => Clean(item.ToString(document))))),
        };
    }

    private static string Clean(string text)
        => text.StripQuotes().DecodeIfc();
}
