namespace Ara3D.Ifc.Tests;

/// <summary>
/// One IfcPropertySingleValue: a name plus a typed measure literal such as IFCLABEL('Level 1')
/// or IFCREAL(139.5).
/// </summary>
public readonly record struct IfcPropertyValue(string Name, string ValueType, string Literal)
{
    public static IfcPropertyValue Label(string name, string value)
        => new(name, "IFCLABEL", IfcStepText.String(value));

    public static IfcPropertyValue Text(string name, string value)
        => new(name, "IFCTEXT", IfcStepText.String(value));

    public static IfcPropertyValue Identifier(string name, string value)
        => new(name, "IFCIDENTIFIER", IfcStepText.String(value));

    public static IfcPropertyValue Real(string name, double value)
        => new(name, "IFCREAL", IfcStepText.Real(value));

    public static IfcPropertyValue Integer(string name, long value)
        => new(name, "IFCINTEGER", IfcStepText.Integer(value));

    public static IfcPropertyValue Boolean(string name, bool value)
        => new(name, "IFCBOOLEAN", IfcStepText.Boolean(value));

    public string NominalValue
        => $"{ValueType}({Literal})";
}
