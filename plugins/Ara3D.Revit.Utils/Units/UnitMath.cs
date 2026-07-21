using System;

namespace Ara3D.Revit.Utils;

/// <summary>
/// Pure unit-conversion math — no Revit types, reachable from the Tests project (P10).
/// Revit's internal length unit is feet, internal angle unit is radians; every member
/// self-names its units and nothing converts silently (P7).
/// </summary>
public static partial class Units
{
    private const double FeetToMillimeters = 304.8;
    private const double FeetToMeters = FeetToMillimeters / 1000.0;
    private const double DegreesPerRadian = 180.0 / Math.PI;

    public static double ToMillimeters(this double feet)
        => feet * FeetToMillimeters;

    public static double ToMeters(this double feet)
        => feet * FeetToMeters;

    public static double FromMillimeters(double millimeters)
        => millimeters / FeetToMillimeters;

    public static double FromMeters(double meters)
        => meters / FeetToMeters;

    public static double ToDegrees(this double radians)
        => radians * DegreesPerRadian;

    public static double FromDegrees(double degrees)
        => degrees / DegreesPerRadian;
}
