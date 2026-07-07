using System.Collections.Generic;

namespace Ara3D.BimOpenSchema.Harmonizer;

/// <summary>
/// Maps source-specific parameter descriptors to canonical parameters.
/// A row matches a parameter by descriptor name, and optionally by descriptor group
/// (needed for unprefixed names like Revit's built-in "Area" or IFC quantity sets).
/// </summary>
public static class ParameterMappings
{
    /// <summary>
    /// Canonical parameter names, without the configurable prefix.
    /// </summary>
    public static class Canonical
    {
        public const string Category = "Category";
        public const string Number = "Number";
        public const string Level = "Level";
        public const string Elevation = "Elevation";
        public const string Area = "Area";
        public const string Volume = "Volume";
        public const string Perimeter = "Perimeter";
        public const string Length = "Length";
        public const string Width = "Width";
        public const string Height = "Height";
        public const string BoundsMin = "Bounds.Min";
        public const string BoundsMax = "Bounds.Max";
        public const string LocationPoint = "Location.Point";
    }

    /// <param name="Source">Which generator this row applies to.</param>
    /// <param name="SourceName">Descriptor name to match, exactly.</param>
    /// <param name="SourceGroup">Descriptor group to match; null matches any group.</param>
    /// <param name="CanonicalName">Canonical name (without prefix).</param>
    /// <param name="Type">The parameter type of the canonical parameter (must match the source parameter's type).</param>
    /// <param name="Quantity">Which unit conversion applies to numeric values.</param>
    public record ParameterMapping(
        SourceKind Source,
        string SourceName,
        string SourceGroup,
        string CanonicalName,
        ParameterType Type,
        QuantityKind Quantity = QuantityKind.None);

    public static readonly ParameterMapping[] All =
    [
        // ----- Revit: curated (prefixed) parameters -----
        new(SourceKind.Revit, "Rvt:Room:Number", null, Canonical.Number, ParameterType.String),
        new(SourceKind.Revit, "Rvt:Room:Volume", null, Canonical.Volume, ParameterType.Number, QuantityKind.Volume),
        new(SourceKind.Revit, "Rvt:Element:Level", null, Canonical.Level, ParameterType.Entity),
        new(SourceKind.Revit, "Rvt:Level:Elevation", null, Canonical.Elevation, ParameterType.Number, QuantityKind.Length),
        new(SourceKind.Revit, "Rvt:Element:Bounds.Min", null, Canonical.BoundsMin, ParameterType.Point, QuantityKind.Length),
        new(SourceKind.Revit, "Rvt:Element:Bounds.Max", null, Canonical.BoundsMax, ParameterType.Point, QuantityKind.Length),
        new(SourceKind.Revit, "Rvt:Element:Location.Point", null, Canonical.LocationPoint, ParameterType.Point, QuantityKind.Length),

        // ----- Revit: built-in (unprefixed) parameters, matched by (group, name) -----
        // The group is the label Revit reports for BuiltInParameterGroup (English UI).
        new(SourceKind.Revit, "Area", "Dimensions", Canonical.Area, ParameterType.Number, QuantityKind.Area),
        new(SourceKind.Revit, "Volume", "Dimensions", Canonical.Volume, ParameterType.Number, QuantityKind.Volume),
        new(SourceKind.Revit, "Length", "Dimensions", Canonical.Length, ParameterType.Number, QuantityKind.Length),
        new(SourceKind.Revit, "Width", "Dimensions", Canonical.Width, ParameterType.Number, QuantityKind.Length),
        new(SourceKind.Revit, "Height", "Dimensions", Canonical.Height, ParameterType.Number, QuantityKind.Length),
        new(SourceKind.Revit, "Perimeter", "Dimensions", Canonical.Perimeter, ParameterType.Number, QuantityKind.Length),

        // ----- IFC: converter (prefixed) parameters -----
        new(SourceKind.Ifc, "Ifc:Room:Number", null, Canonical.Number, ParameterType.String),
        // IFCBUILDINGSTOREY's Elevation attribute, grouped by the entity class name.
        new(SourceKind.Ifc, "Ifc:Elevation", "IFCBUILDINGSTOREY", Canonical.Elevation, ParameterType.Number, QuantityKind.Length),

        // ----- IFC: base quantity sets -----
        new(SourceKind.Ifc, "NetFloorArea", "Qto_SpaceBaseQuantities", Canonical.Area, ParameterType.Number, QuantityKind.Area),
        new(SourceKind.Ifc, "GrossFloorArea", "Qto_SpaceBaseQuantities", Canonical.Area, ParameterType.Number, QuantityKind.Area),
        new(SourceKind.Ifc, "NetVolume", "Qto_SpaceBaseQuantities", Canonical.Volume, ParameterType.Number, QuantityKind.Volume),
        new(SourceKind.Ifc, "GrossVolume", "Qto_SpaceBaseQuantities", Canonical.Volume, ParameterType.Number, QuantityKind.Volume),
        new(SourceKind.Ifc, "Height", "Qto_SpaceBaseQuantities", Canonical.Height, ParameterType.Number, QuantityKind.Length),
        new(SourceKind.Ifc, "GrossPerimeter", "Qto_SpaceBaseQuantities", Canonical.Perimeter, ParameterType.Number, QuantityKind.Length),

        new(SourceKind.Ifc, "NetSideArea", "Qto_WallBaseQuantities", Canonical.Area, ParameterType.Number, QuantityKind.Area),
        new(SourceKind.Ifc, "NetVolume", "Qto_WallBaseQuantities", Canonical.Volume, ParameterType.Number, QuantityKind.Volume),
        new(SourceKind.Ifc, "Length", "Qto_WallBaseQuantities", Canonical.Length, ParameterType.Number, QuantityKind.Length),
        new(SourceKind.Ifc, "Width", "Qto_WallBaseQuantities", Canonical.Width, ParameterType.Number, QuantityKind.Length),
        new(SourceKind.Ifc, "Height", "Qto_WallBaseQuantities", Canonical.Height, ParameterType.Number, QuantityKind.Length),

        new(SourceKind.Ifc, "NetArea", "Qto_SlabBaseQuantities", Canonical.Area, ParameterType.Number, QuantityKind.Area),
        new(SourceKind.Ifc, "NetVolume", "Qto_SlabBaseQuantities", Canonical.Volume, ParameterType.Number, QuantityKind.Volume),
        new(SourceKind.Ifc, "Perimeter", "Qto_SlabBaseQuantities", Canonical.Perimeter, ParameterType.Number, QuantityKind.Length),

        new(SourceKind.Ifc, "Area", "Qto_DoorBaseQuantities", Canonical.Area, ParameterType.Number, QuantityKind.Area),
        new(SourceKind.Ifc, "Height", "Qto_DoorBaseQuantities", Canonical.Height, ParameterType.Number, QuantityKind.Length),
        new(SourceKind.Ifc, "Width", "Qto_DoorBaseQuantities", Canonical.Width, ParameterType.Number, QuantityKind.Length),

        new(SourceKind.Ifc, "Area", "Qto_WindowBaseQuantities", Canonical.Area, ParameterType.Number, QuantityKind.Area),
        new(SourceKind.Ifc, "Height", "Qto_WindowBaseQuantities", Canonical.Height, ParameterType.Number, QuantityKind.Length),
        new(SourceKind.Ifc, "Width", "Qto_WindowBaseQuantities", Canonical.Width, ParameterType.Number, QuantityKind.Length),
    ];

    /// <summary>
    /// Returns a lookup from (name, group) and (name, null) to mapping rows for the given source.
    /// Rows with a null group are matched by name alone.
    /// </summary>
    public static Dictionary<(string name, string group), ParameterMapping> ForSource(SourceKind source)
    {
        var r = new Dictionary<(string, string), ParameterMapping>();
        foreach (var m in All)
            if (m.Source == source)
                r[(m.SourceName, m.SourceGroup)] = m;
        return r;
    }
}
