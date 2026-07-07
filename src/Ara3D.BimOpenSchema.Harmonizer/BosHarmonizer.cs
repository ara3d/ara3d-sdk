using System.Collections.Generic;

namespace Ara3D.BimOpenSchema.Harmonizer;

/// <summary>
/// Adds canonical ("Bos:" prefixed) categories and parameters to BIM Open Schema data,
/// so that downstream consumers can query data uniformly regardless of whether it was
/// generated from Revit or IFC. The output is lossless: all input tables are preserved
/// unchanged, and canonical values are appended alongside the originals.
/// </summary>
public static class BosHarmonizer
{
    public const string BuildingStoreyCategoryName = "IFCBUILDINGSTOREY";

    public static SourceKind DetectSource(IBimData input)
        => SourceDetector.Detect(input);

    /// <summary>
    /// True if the data already contains canonical descriptors (in the canonical group).
    /// </summary>
    public static bool IsHarmonized(IBimData input, HarmonizerOptions options = null)
    {
        options ??= new HarmonizerOptions();
        foreach (var d in input.Descriptors)
            if (input.Strings[(int)d.Group] == options.CanonicalGroup)
                return true;
        return false;
    }

    /// <summary>
    /// Returns a new BimData containing everything in the input, plus canonical
    /// categories and parameters. Idempotent: already-harmonized data is copied unchanged.
    /// </summary>
    public static BimData Harmonize(IBimData input, HarmonizerOptions options = null)
    {
        options ??= new HarmonizerOptions();
        var source = options.SourceOverride ?? SourceDetector.Detect(input);

        var bdb = new BimDataBuilder();
        bdb.Manifest = input.Manifest ?? new Manifest();

        // The builder is empty, so all input indices are preserved verbatim by the copy.
        bdb.AddBimData(input);
        bdb.Geometry = input.Geometry;

        if (IsHarmonized(input, options))
            return bdb.Build();

        var diagDoc = input.Documents.Length > 0 ? (DocumentIndex)0 : (DocumentIndex)(-1);

        if (source == SourceKind.Unknown)
            bdb.AddDiagnostic(DiagnosticType.ExporterWarning,
                "Harmonizer could not detect the generator (no manifest, no recognizable descriptors); no unit conversion applied.",
                diagDoc, BimDataBuilder.InvalidEntityIndex);
        else
            bdb.AddDiagnostic(DiagnosticType.ExporterInfo,
                $"Harmonized as {source} data. " +
                (source == SourceKind.Revit
                    ? "Numeric values converted from Revit internal units (feet-based) to SI."
                    : "Values assumed to already be SI."),
                diagDoc, BimDataBuilder.InvalidEntityIndex);

        if (options.AddCanonicalCategories)
            AddCanonicalCategories(input, bdb, source, options, diagDoc);

        if (options.AddCanonicalParameters)
        {
            AddCanonicalParameters(input, bdb, source, options);
            if (source == SourceKind.Ifc)
                AddIfcLevels(input, bdb, options);
        }

        return bdb.Build();
    }

    private static void AddCanonicalCategories(
        IBimData input, BimDataBuilder bdb, SourceKind source, HarmonizerOptions options, DocumentIndex diagDoc)
    {
        var lookup = CategoryMappings.ForSource(source);
        var unmapped = new HashSet<string>();
        var catDesc = bdb.AddDescriptor(
            options.CanonicalPrefix + ParameterMappings.Canonical.Category, "", options.CanonicalGroup, ParameterType.String);

        for (var i = 0; i < input.Entities.Length; ++i)
        {
            var e = input.Entities[i];
            if (e.Category < 0)
                continue;
            var catName = input.Strings[(int)input.Entities[(int)e.Category].Name];
            if (lookup.TryGetValue(catName, out var canonical))
                bdb.AddParameter((EntityIndex)i, canonical, catDesc);
            else
                unmapped.Add(catName);
        }

        foreach (var name in unmapped)
            bdb.AddDiagnostic(DiagnosticType.ExporterInfo,
                $"No canonical category mapping for source category '{name}'.",
                diagDoc, BimDataBuilder.InvalidEntityIndex);
    }

    private static void AddCanonicalParameters(
        IBimData input, BimDataBuilder bdb, SourceKind source, HarmonizerOptions options)
    {
        var mappings = ParameterMappings.ForSource(source);

        // Resolve each input descriptor against the mapping table once.
        var descMap = new ParameterMappings.ParameterMapping[input.Descriptors.Length];
        for (var i = 0; i < input.Descriptors.Length; ++i)
        {
            var d = input.Descriptors[i];
            var name = input.Strings[(int)d.Name];
            var group = input.Strings[(int)d.Group];
            if (!mappings.TryGetValue((name, group), out var m))
                mappings.TryGetValue((name, null), out m);
            // Bool aliases Int, so comparing the enum values also accepts Bool for Int.
            if (m != null && m.Type == d.Type)
                descMap[i] = m;
        }

        DescriptorIndex CanonicalDescriptor(ParameterMappings.ParameterMapping m)
            => bdb.AddDescriptor(
                options.CanonicalPrefix + m.CanonicalName,
                m.Type is ParameterType.Number or ParameterType.Point ? UnitConversion.SIUnitLabel(m.Quantity) : "",
                options.CanonicalGroup,
                m.Type);

        foreach (var p in input.Parameters)
        {
            var m = descMap[(int)p.Descriptor];
            if (m == null)
                continue;

            var cd = CanonicalDescriptor(m);
            switch (m.Type)
            {
                case ParameterType.Number:
                    var num = input.Numbers[p.Value];
                    bdb.AddParameter(p.Entity, UnitConversion.ToSI(num, m.Quantity, source), cd);
                    break;
                case ParameterType.Point:
                    var pt = input.Points[p.Value];
                    bdb.AddParameter(p.Entity, UnitConversion.ToSI(pt, source), cd);
                    break;
                case ParameterType.String:
                    bdb.AddParameter(p.Entity, input.Strings[p.Value], cd);
                    break;
                case ParameterType.Entity:
                    bdb.AddParameter(p.Entity, (EntityIndex)p.Value, cd);
                    break;
                default: // Int / Bool
                    bdb.AddParameter(p.Entity, p.Value, cd);
                    break;
            }
        }
    }

    /// <summary>
    /// IFC has no level parameter; level membership is expressed by ContainedIn relations
    /// targeting an IFCBUILDINGSTOREY entity. Surface those as a canonical level parameter,
    /// matching what the Revit level mapping produces.
    /// </summary>
    private static void AddIfcLevels(IBimData input, BimDataBuilder bdb, HarmonizerOptions options)
    {
        var levelDesc = bdb.AddDescriptor(
            options.CanonicalPrefix + ParameterMappings.Canonical.Level, "", options.CanonicalGroup, ParameterType.Entity);

        string CategoryName(EntityIndex ei)
        {
            var cat = input.Entities[(int)ei].Category;
            return cat < 0 ? "" : input.Strings[(int)input.Entities[(int)cat].Name];
        }

        var seen = new HashSet<EntityIndex>();
        foreach (var r in input.Relations)
        {
            if (r.RelationType != RelationType.ContainedIn)
                continue;
            if (r.EntityA < 0 || r.EntityB < 0)
                continue;

            // Be tolerant of relation direction: one endpoint must be a building storey.
            EntityIndex element, level;
            if (CategoryName(r.EntityB) == BuildingStoreyCategoryName)
                (element, level) = (r.EntityA, r.EntityB);
            else if (CategoryName(r.EntityA) == BuildingStoreyCategoryName)
                (element, level) = (r.EntityB, r.EntityA);
            else
                continue;

            if (seen.Add(element))
                bdb.AddParameter(element, level, levelDesc);
        }
    }
}
