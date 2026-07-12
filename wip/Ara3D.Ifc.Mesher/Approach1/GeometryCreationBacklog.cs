using Ara3D.IfcLoader;
using Ara3D.IO.StepParser;
using Ara3D.Utils;

namespace Ara3D.Ifc.Mesher.Approach1;

public enum GeometryCreationSupport
{
    Supported,
    Partial,
    Planned,
}

public sealed record GeometryCreationItem(
    string EntityName,
    GeometryCreationSupport Support,
    string Notes,
    string? TestFixtureId = null,
    bool ExpectWatertight = false);

public sealed record EncounteredGeometryItem(string EntityName, int Count, GeometryCreationSupport Support, string Notes);

/// <summary>
/// Curated registry of geometry-bearing IFC entities and corpus scanning.
/// Ported from the former Approach2 scratch mesher backlog.
/// </summary>
public static class GeometryCreationBacklog
{
    public static IReadOnlyList<GeometryCreationItem> KnownItems { get; } =
    [
        // Dispatcher solids
        new("IFCEXTRUDEDAREASOLID", GeometryCreationSupport.Supported, "Solid extrusion for supported swept profiles.", "Winding_ExtrudedBox", ExpectWatertight: true),
        new("IFCATTDRIVENEXTRUDEDSOLID", GeometryCreationSupport.Partial, "Attribute-driven depth extrusion.", "CutOracle_ExtrusionEndClip"),
        new("IFCATTDRIVENCLIPPEDEXTRUDEDSOLID", GeometryCreationSupport.Partial, "Attribute-driven extrusion with half-space clip.", "CutOracle_ExtrusionEndClip"),
        new("IFCREVOLVEDAREASOLID", GeometryCreationSupport.Partial, "Supported profiles revolved around an axis; partial sweeps capped at both ends."),
        new("IFCSWEPTDISKSOLID", GeometryCreationSupport.Partial, "Straight directrix with annular caps; curved paths omit inner radius."),
        new("IFCSWEPTDISKSOLIDPOLYGONAL", GeometryCreationSupport.Partial, "Same path as IfcSweptDiskSolid; fillet radius ignored."),
        new("IFCSURFACECURVESWEPTAREASOLID", GeometryCreationSupport.Partial, "Surface-curve swept solid; limited path support."),
        new("IFCSURFACEOFLINEAREXTRUSION", GeometryCreationSupport.Supported, "Open-curve ribbon extrusion."),
        new("IFCFIXEDREFERENCESWEPTAREASOLID", GeometryCreationSupport.Partial, "Fixed-reference swept area solid."),
        new("IFCTRIANGULATEDFACESET", GeometryCreationSupport.Partial, "Direct triangle index representation; normals and PnIndex ignored.", "Winding_TessellatedFaceSet"),
        new("IFCPOLYGONALFACESET", GeometryCreationSupport.Partial, "Indexed polygon representation; fan triangulation."),
        new("IFCFACETEDBREP", GeometryCreationSupport.Partial, "Classic faceted BRep with closed shells."),
        new("IFCADVANCEDBREP", GeometryCreationSupport.Partial, "Advanced BRep; bounds triangulated, curved surfaces ignored."),
        new("IFCADVANCEDBREPWITHVOIDS", GeometryCreationSupport.Planned, "Advanced BRep with void shells."),
        new("IFCFACE", GeometryCreationSupport.Partial, "Simple polyloop faces with matching-count inner bounds."),
        new("IFCFACESURFACE", GeometryCreationSupport.Partial, "Face-surface wrapper over polyloop bounds."),
        new("IFCADVANCEDFACE", GeometryCreationSupport.Partial, "Triangulates polyloop bounds; curved surfaces ignored."),
        new("IFCFACEBASEDSURFACEMODEL", GeometryCreationSupport.Partial, "Surface model over connected face sets."),
        new("IFCSHELLBASEDSURFACEMODEL", GeometryCreationSupport.Partial, "Shell representation over open/closed shells."),
        new("IFCBOOLEANCLIPPINGRESULT", GeometryCreationSupport.Partial, "Planar half-space clipping against triangle meshes.", "CutOracle_HalfSpace_AgreementMatrix"),
        new("IFCBOOLEANRESULT", GeometryCreationSupport.Partial, "Planar half-space difference; union/intersection unsupported.", "CutOracle_HalfSpace_AgreementMatrix"),
        new("IFCHALFSPACESOLID", GeometryCreationSupport.Partial, "Planar base surfaces used by boolean clipping results.", "CutOracle_HalfSpace_AgreementMatrix"),
        new("IFCPLANE", GeometryCreationSupport.Partial, "Plane base surface for half-space clipping.", "CutOracle_HalfSpace_AgreementMatrix"),
        new("IFCPOLYGONALBOUNDEDHALFSPACE", GeometryCreationSupport.Partial, "Polygonal-bounded half-space used as boolean second operand.", "CutOracle_PolygonalBounded"),
        new("IFCBOUNDINGBOX", GeometryCreationSupport.Planned, "Bounding box placeholder; not meshed."),
        new("IFCSECTIONEDSPINE", GeometryCreationSupport.Planned, "Sectioned spine; not meshed."),
        new("IFCSHAPEREPRESENTATION", GeometryCreationSupport.Supported, "Representation items merge.", "Winding_MappedItem_NegativeScale"),
        new("IFCREPRESENTATION", GeometryCreationSupport.Supported, "Generic representation items merge."),
        new("IFCPRODUCTDEFINITIONSHAPE", GeometryCreationSupport.Supported, "Product shape representations merge.", "CutOracle_OpeningCarve"),
        new("IFCMAPPEDITEM", GeometryCreationSupport.Partial, "Representation-map reuse; mapping transform applied on instance during assembly.", "Winding_MappedItem_NegativeScale"),
        new("IFCSTYLEDITEM", GeometryCreationSupport.Supported, "Appearance wrapper; delegates to item."),
        new("IFCREPRESENTATIONMAP", GeometryCreationSupport.Partial, "Mapped representation source.", "Winding_MappedItem_NegativeScale"),
        new("IFCCARTESIANTRANSFORMATIONOPERATOR3D", GeometryCreationSupport.Partial, "Mapped item transform with origin, axes, and scale.", "Winding_MappedItem_NegativeScale"),
        new("IFCCARTESIANTRANSFORMATIONOPERATOR3DNONUNIFORM", GeometryCreationSupport.Partial, "Mapped item transform with non-uniform scale.", "MeshesMappedItemNonUniform"),

        // Profiles
        new("IFCRECTANGLEPROFILEDEF", GeometryCreationSupport.Supported, "2D rectangular profile with optional axis placement.", "Winding_ExtrudedBox", ExpectWatertight: true),
        new("IFCCIRCLEPROFILEDEF", GeometryCreationSupport.Supported, "Approximated as a configurable segment polygon."),
        new("IFCELLIPSEPROFILEDEF", GeometryCreationSupport.Supported, "Approximated as a configurable segment polygon."),
        new("IFCCIRCLEHOLLOWPROFILEDEF", GeometryCreationSupport.Partial, "Outer and inner polygon rings."),
        new("IFCRECTANGLEHOLLOWPROFILEDEF", GeometryCreationSupport.Partial, "Parameterized hollow rectangle; fillet radii ignored."),
        new("IFCISHAPEPROFILEDEF", GeometryCreationSupport.Partial, "Parameterized I-shape; fillets and slopes ignored."),
        new("IFCLSHAPEPROFILEDEF", GeometryCreationSupport.Partial, "Parameterized L-shape; radii and leg slope ignored."),
        new("IFCARBITRARYCLOSEDPROFILEDEF", GeometryCreationSupport.Partial, "Polylines, composite curves, trimmed arcs, B-spline control polygons."),
        new("IFCARBITRARYPROFILEDEFWITHVOIDS", GeometryCreationSupport.Partial, "Outer and inner bounded curves; simple non-touching holes.", "Winding_HollowProfile", ExpectWatertight: true),
        new("IFCCOMPOSITEPROFILEDEF", GeometryCreationSupport.Partial, "Nested profiles as holes; disjoint sub-profiles unioned at extrusion."),
        new("IFCDERIVEDPROFILEDEF", GeometryCreationSupport.Supported, "Parent profile transformed by 2D/3D Cartesian operator.", "Winding_DerivedProfile_MirrorOperator", ExpectWatertight: true),
        new("IFCTRAPEZIUMPROFILEDEF", GeometryCreationSupport.Partial, "Parameterized trapezium profile."),
        new("IFCARBITRARYOPENPROFILEDEF", GeometryCreationSupport.Supported, "Open profile curves for ribbon extrusion."),

        // Curves / placements / BRep parts
        new("IFCPOLYLINE", GeometryCreationSupport.Partial, "Profile boundaries and straight swept-disk directrices."),
        new("IFCLOCALPLACEMENT", GeometryCreationSupport.Supported, "Recursive local placement for product traversal."),
        new("IFCAXIS2PLACEMENT2D", GeometryCreationSupport.Supported, "Parameterized profile placement."),
        new("IFCAXIS2PLACEMENT3D", GeometryCreationSupport.Supported, "Swept solid position and product placement."),
        new("IFCCARTESIANPOINTLIST2D", GeometryCreationSupport.Supported, "2D coordinate list for indexed profile curves."),
        new("IFCCARTESIANPOINTLIST3D", GeometryCreationSupport.Supported, "3D coordinate list for tessellated face sets.", "Winding_TessellatedFaceSet"),
        new("IFCFACEBOUND", GeometryCreationSupport.Partial, "Polyloop face boundary including inner rings.", "Winding_FaceBound_SameSenseFalse"),
        new("IFCFACEOUTERBOUND", GeometryCreationSupport.Partial, "Outer polyloop boundary for a BRep face."),
        new("IFCPOLYLOOP", GeometryCreationSupport.Partial, "3D point loop used by BRep faces."),
        new("IFCCONNECTEDFACESET", GeometryCreationSupport.Partial, "Connected face set with polyloop faces."),
        new("IFCOPENEDSHELL", GeometryCreationSupport.Partial, "Open shell with polyloop faces."),
        new("IFCCLOSEDSHELL", GeometryCreationSupport.Partial, "Closed shell of polyloop faces."),
        new("IFCCOMPOSITECURVE", GeometryCreationSupport.Partial, "Composite profile curves with SameSense handling."),
        new("IFCCOMPOSITECURVESEGMENT", GeometryCreationSupport.Partial, "Segment wrapper with SameSense for parent curves."),
        new("IFCTRIMMEDCURVE", GeometryCreationSupport.Partial, "Trimmed circle/ellipse/line curves."),
        new("IFCCIRCLE", GeometryCreationSupport.Partial, "2D circle curve for profiles and trims."),
        new("IFCINDEXEDPOLYCURVE", GeometryCreationSupport.Partial, "Indexed point-list paths with line/arc indices."),
        new("IFCLINEINDEX", GeometryCreationSupport.Partial, "Indexed polycurve line segment indices."),
        new("IFCARCINDEX", GeometryCreationSupport.Partial, "Indexed polycurve arc indices sampled as arcs."),
        new("IFCBSPLINECURVE", GeometryCreationSupport.Partial, "Control-polygon approximation for profiles and directrices."),
        new("IFCBSPLINECURVEWITHKNOTS", GeometryCreationSupport.Partial, "Control-polygon approximation; knots ignored."),
        new("IFCRATIONALBSPLINECURVEWITHKNOTS", GeometryCreationSupport.Partial, "Control-polygon approximation; weights and knots ignored."),
        new("IFCINDEXEDPOLYGONALFACE", GeometryCreationSupport.Partial, "Indexed polygon face fan-triangulated from one-based CoordIndex."),
        new("IFCINDEXEDPOLYGONALFACEWITHVOIDS", GeometryCreationSupport.Partial, "Outer and inner CoordIndex rings; planar XY triangulation."),
    ];

    /// <summary>Supported items that need no dedicated micro fixture (placements / wrappers covered transitively).</summary>
    public static IReadOnlyList<string> CoverageExemptions { get; } =
    [
        "IFCLOCALPLACEMENT",
        "IFCAXIS2PLACEMENT2D",
        "IFCAXIS2PLACEMENT3D",
        "IFCCARTESIANPOINTLIST2D",
        "IFCREPRESENTATION",
        "IFCSTYLEDITEM",
        "IFCSURFACEOFLINEAREXTRUSION",
        "IFCARBITRARYOPENPROFILEDEF",
        "IFCCIRCLEPROFILEDEF",
        "IFCELLIPSEPROFILEDEF",
    ];

    static readonly Dictionary<string, GeometryCreationItem> KnownByName =
        KnownItems.ToDictionary(i => i.EntityName);

    public static GeometryCreationItem? TryGet(string entityName)
        => KnownByName.GetValueOrDefault(entityName);

    public static IReadOnlyList<EncounteredGeometryItem> Scan(params FilePath[] files)
    {
        var counts = new Dictionary<string, int>(StringComparer.Ordinal);

        foreach (var file in files.Where(f => f.Exists()))
        {
            using var step = new StepDocument(file);
            var resolver = new IfcEntityResolver(step);
            foreach (var entity in resolver.GetEntities())
            {
                var name = entity.GetEntityName();
                if (IsLikelyGeometryCreationEntity(name))
                    counts[name] = counts.GetValueOrDefault(name) + 1;
            }
        }

        return counts
            .OrderByDescending(kv => kv.Value)
            .ThenBy(kv => kv.Key, StringComparer.Ordinal)
            .Select(kv =>
            {
                var known = KnownByName.GetValueOrDefault(kv.Key);
                return new EncounteredGeometryItem(
                    kv.Key,
                    kv.Value,
                    known?.Support ?? GeometryCreationSupport.Planned,
                    known?.Notes ?? "Encountered in IFC input; needs geometry creation support decision.");
            })
            .ToList();
    }

    public static bool IsLikelyGeometryCreationEntity(string entityName)
        => KnownByName.ContainsKey(entityName) ||
           entityName.Contains("SOLID", StringComparison.Ordinal) ||
           entityName.Contains("BREP", StringComparison.Ordinal) ||
           entityName.Contains("SHELL", StringComparison.Ordinal) ||
           entityName is "IFCFACE" or "IFCFACEBOUND" or "IFCFACEOUTERBOUND" ||
           entityName.Contains("FACESET", StringComparison.Ordinal) ||
           entityName.Contains("PROFILEDEF", StringComparison.Ordinal) ||
           entityName.Contains("CURVE", StringComparison.Ordinal) ||
           entityName.Contains("MAPPEDITEM", StringComparison.Ordinal) ||
           entityName.Contains("BOOLEAN", StringComparison.Ordinal);
}
