using System.Globalization;
using System.Text.RegularExpressions;
using Ara3D.Ifc.Tests;

namespace Ara3D.DoorClearance.Tests;

/// <summary>One door with everything the rules need, copied out of the file so it outlives it.</summary>
public sealed record DoorFact(
    int Id,
    string GlobalId,
    string Name,
    string ObjectType,
    string Storey,
    double? DeclaredWidthMm,
    double? DeclaredHeightMm,
    double? LeafWidthMm,
    double? PsetClearWidthMm,
    Vec3? WorldPosition);

/// <summary>A potential obstruction: a fixed furnishing element and its placement origin.</summary>
public sealed record ObstacleFact(int Id, string GlobalId, Vec3? WorldPosition);

public sealed record ModelFacts(int EntityCount, IReadOnlyList<DoorFact> Doors, IReadOnlyList<ObstacleFact> Obstacles)
{
    private static readonly Regex LeafWidth = new(@"^0*(\d+)", RegexOptions.Compiled);

    public static ModelFacts Extract(IfcSourceFile file)
    {
        var resolver = file.File.EntityResolver;
        var storeyByElement = StoreyByElement(file);
        var clearWidthByElement = PsetClearWidthByElement(file);

        var doors = new List<DoorFact>();
        var obstacles = new List<ObstacleFact>();
        foreach (var span in file.Spans)
        {
            if (string.Equals(span.TypeName, "IFCDOOR", StringComparison.OrdinalIgnoreCase))
            {
                var e = resolver.GetEntity(span.Id);
                doors.Add(new DoorFact(
                    span.Id,
                    e.GetString(0),
                    e.GetStringOrEmpty(2),
                    e.GetStringOrEmpty(4),
                    storeyByElement.GetValueOrDefault(span.Id, ""),
                    MetersToMm(e, 9),
                    MetersToMm(e, 8),
                    ParseLeafWidthMm(e.GetStringOrEmpty(4)),
                    clearWidthByElement.TryGetValue(span.Id, out var cw) ? cw : null,
                    PositionOf(file, span.Id)));
            }
            else if (string.Equals(span.TypeName, "IFCFURNISHINGELEMENT", StringComparison.OrdinalIgnoreCase))
            {
                var e = resolver.GetEntity(span.Id);
                obstacles.Add(new ObstacleFact(span.Id, e.GetString(0), PositionOf(file, span.Id)));
            }
        }

        return new ModelFacts(file.Count, doors, obstacles);
    }

    /// <summary>The leaf width encoded in a Revit family size name such as "0762 x 2032mm".</summary>
    public static double? ParseLeafWidthMm(string objectType)
    {
        var m = LeafWidth.Match(objectType);
        return m.Success ? double.Parse(m.Groups[1].Value, CultureInfo.InvariantCulture) : null;
    }

    private static double? MetersToMm(Ara3D.IfcLoader.IfcEntity e, int index)
        => e.GetValue(index).IsUnassignedOrRedeclared ? null : e.GetNumber(index) * 1000.0;

    /// <summary>World placement origin of a product (attribute 5 = ObjectPlacement).</summary>
    private static Vec3? PositionOf(IfcSourceFile file, int productId)
    {
        var e = file.File.EntityResolver.GetEntity(productId);
        return e.GetValue(5).IsUnassignedOrRedeclared
            ? null
            : StepPlacement.WorldPosition(file.File.EntityResolver, e.GetId(5));
    }

    private static Dictionary<int, string> StoreyByElement(IfcSourceFile file)
    {
        var resolver = file.File.EntityResolver;
        var r = new Dictionary<int, string>();
        foreach (var span in file.Spans)
        {
            if (!string.Equals(span.TypeName, "IFCRELCONTAINEDINSPATIALSTRUCTURE", StringComparison.OrdinalIgnoreCase))
                continue;
            var rel = resolver.GetEntity(span.Id);
            var structure = resolver.GetEntityOrDefault(rel.GetId(5));
            if (structure == null || !string.Equals(structure.GetEntityName(), "IFCBUILDINGSTOREY", StringComparison.OrdinalIgnoreCase))
                continue;
            var storeyName = structure.GetStringOrEmpty(2);
            foreach (var elementId in rel.GetIdList(4))
                r.TryAdd(elementId, storeyName);
        }
        return r;
    }

    /// <summary>
    /// Pset_DoorCommon.ClearWidth (mm) per element, resolved through IFCRELDEFINESBYPROPERTIES.
    /// The value payload is parsed from the property's raw STEP text because the tokenizer parks
    /// typed-measure payloads outside the attribute list.
    /// </summary>
    private static Dictionary<int, double> PsetClearWidthByElement(IfcSourceFile file)
    {
        var resolver = file.File.EntityResolver;
        var r = new Dictionary<int, double>();
        foreach (var span in file.Spans)
        {
            if (!string.Equals(span.TypeName, "IFCRELDEFINESBYPROPERTIES", StringComparison.OrdinalIgnoreCase))
                continue;
            var rel = resolver.GetEntity(span.Id);
            var pset = resolver.GetEntityOrDefault(rel.GetId(5));
            if (pset == null
                || !string.Equals(pset.GetEntityName(), "IFCPROPERTYSET", StringComparison.OrdinalIgnoreCase)
                || !string.Equals(pset.GetStringOrEmpty(2), "Pset_DoorCommon", StringComparison.Ordinal))
                continue;

            foreach (var propId in pset.GetIdList(4))
            {
                var prop = resolver.GetEntityOrDefault(propId);
                if (prop == null || !string.Equals(prop.GetStringOrEmpty(0), "ClearWidth", StringComparison.Ordinal))
                    continue;
                var valueMm = ParseMeasureMm(file, propId);
                if (valueMm == null)
                    continue;
                foreach (var elementId in rel.GetIdList(4))
                    r.TryAdd(elementId, valueMm.Value);
            }
        }
        return r;
    }

    private static double? ParseMeasureMm(IfcSourceFile file, int propId)
    {
        var span = file.GetSpan(propId);
        if (span == null)
            return null;
        var m = Regex.Match(file.GetText(span.Value), @"IFC\w*MEASURE\(([-0-9.Ee+]+)\)");
        return m.Success ? double.Parse(m.Groups[1].Value, CultureInfo.InvariantCulture) * 1000.0 : null;
    }
}
