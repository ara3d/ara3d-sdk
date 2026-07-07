namespace Ara3D.BimOpenSchema.Harmonizer;

/// <summary>
/// The physical quantity a numeric parameter measures, used to pick a unit conversion.
/// </summary>
public enum QuantityKind
{
    None,
    Length,
    Area,
    Volume,
    Angle,
}

/// <summary>
/// Converts values from generator-specific units to SI.
/// Revit stores numbers in its internal units (feet-based lengths, radians for angles),
/// regardless of the display unit recorded on the descriptor.
/// IFC values are assumed to be SI already (TODO: parse IFCUNITASSIGNMENT to be sure).
/// </summary>
public static class UnitConversion
{
    public const double FeetToMeters = 0.3048;

    public static double RevitInternalToSI(double value, QuantityKind kind) => kind switch
    {
        QuantityKind.Length => value * FeetToMeters,
        QuantityKind.Area => value * (FeetToMeters * FeetToMeters),
        QuantityKind.Volume => value * (FeetToMeters * FeetToMeters * FeetToMeters),
        _ => value,
    };

    public static double ToSI(double value, QuantityKind kind, SourceKind source) => source switch
    {
        SourceKind.Revit => RevitInternalToSI(value, kind),
        _ => value,
    };

    public static Point ToSI(Point p, SourceKind source) => source switch
    {
        SourceKind.Revit => new((float)(p.X * FeetToMeters), (float)(p.Y * FeetToMeters), (float)(p.Z * FeetToMeters)),
        _ => p,
    };

    public static string SIUnitLabel(QuantityKind kind) => kind switch
    {
        QuantityKind.Length => "m",
        QuantityKind.Area => "m^2",
        QuantityKind.Volume => "m^3",
        QuantityKind.Angle => "rad",
        _ => "",
    };
}
