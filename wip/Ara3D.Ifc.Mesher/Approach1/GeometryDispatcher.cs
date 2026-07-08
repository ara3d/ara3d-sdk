using Ara3D.Geometry;
using Ara3D.IfcLoader;
using Ara3D.IfcTypes;

namespace Ara3D.Ifc.Mesher.Approach1;

/// <summary>Dispatches geometry-bearing IFC entities to mesh builders. Unknown types are diagnostics-only.</summary>
public static class GeometryDispatcher
{
    public static TriangleMesh3D? TryBuild(MeshingContext ctx, IfcEntity entity)
    {
        try
        {
            return entity.GetEntityName() switch
            {
                "IFCEXTRUDEDAREASOLID" => SweptSolids.BuildExtrudedAreaSolid(ctx, entity),
                "IFCATTDRIVENEXTRUDEDSOLID" => SweptSolids.BuildAttDrivenExtrudedSolid(ctx, entity),
                "IFCATTDRIVENCLIPPEDEXTRUDEDSOLID" => SweptSolids.BuildAttDrivenClippedExtrudedSolid(ctx, entity),
                "IFCREVOLVEDAREASOLID" => SweptSolids.BuildRevolvedAreaSolid(ctx, entity),
                "IFCSWEPTDISKSOLID" or "IFCSWEPTDISKSOLIDPOLYGONAL" => SweptSolids.BuildSweptDiskSolid(ctx, entity),
                "IFCSURFACECURVESWEPTAREASOLID" => SweptSolids.BuildSurfaceCurveSweptAreaSolid(ctx, entity),
                "IFCSURFACEOFLINEAREXTRUSION" => SweptSolids.BuildSurfaceOfLinearExtrusion(ctx, entity),
                "IFCFIXEDREFERENCESWEPTAREASOLID" => SweptSolids.BuildFixedReferenceSweptAreaSolid(ctx, entity),
                "IFCTRIANGULATEDFACESET" => Tessellated.BuildTriangulatedFaceSet(ctx, entity),
                "IFCPOLYGONALFACESET" => Tessellated.BuildPolygonalFaceSet(ctx, entity),
                "IFCFACETEDBREP" => Brep.BuildFacetedBrep(ctx, entity),
                "IFCADVANCEDBREP" => Brep.BuildAdvancedBrep(ctx, entity),
                "IFCFACE" or "IFCFACESURFACE" or "IFCADVANCEDFACE" => Brep.BuildSingleFace(ctx, entity),
                "IFCFACEBASEDSURFACEMODEL" => Brep.BuildFaceBasedSurfaceModel(ctx, entity),
                "IFCSHELLBASEDSURFACEMODEL" => Brep.BuildShellBasedSurfaceModel(ctx, entity),
                "IFCBOOLEANCLIPPINGRESULT" => Booleans.BuildBooleanClippingResult(ctx, entity),
                "IFCBOOLEANRESULT" => Booleans.BuildBooleanResult(ctx, entity),
                "IFCHALFSPACESOLID" => BuildHalfSpaceSolid(ctx, entity),
                "IFCSHAPEREPRESENTATION" or "IFCREPRESENTATION" => BuildRepresentation(ctx, entity),
                "IFCPRODUCTDEFINITIONSHAPE" => BuildProductDefinitionShape(ctx, entity),
                "IFCMAPPEDITEM" => BuildMappedItem(ctx, entity),
                "IFCSTYLEDITEM" => BuildStyledItem(ctx, entity),
                "IFCPOLYGONALBOUNDEDHALFSPACE" or "IFCBOUNDINGBOX" or "IFCSECTIONEDSPINE"
                    => RecordUnsupported(ctx, entity, "Recorded for future implementation"),
                _ => LooksLikeProduct(entity) ? BuildProduct(ctx, entity) : RecordUnsupported(ctx, entity, null),
            };
        }
        catch (Exception ex)
        {
            ctx.Diagnostics.RecordUnsupported(entity.GetEntityName(), $"#{entity.Id}: {ex.Message}");
            return null;
        }
    }

    static TriangleMesh3D? RecordUnsupported(MeshingContext ctx, IfcEntity entity, string? note)
    {
        ctx.Diagnostics.RecordUnsupported(entity.GetEntityName(), note);
        return null;
    }

    static TriangleMesh3D? BuildHalfSpaceSolid(MeshingContext ctx, IfcEntity entity)
    {
        ctx.Diagnostics.RecordUnsupported("IFCHALFSPACESOLID", "Standalone half-space has infinite extent; use in boolean clip");
        return null;
    }

    static TriangleMesh3D? BuildStyledItem(MeshingContext ctx, IfcEntity styled)
    {
        ctx.Diagnostics.RecordApproximate("IFCSTYLEDITEM", "Style ignored; geometry from item only");
        var item = MeshHelpers.ResolveOptional(ctx, styled, IfcStyledItem.Instance.Item);
        return item is null ? null : TryBuild(ctx, item);
    }

    static TriangleMesh3D? BuildRepresentation(MeshingContext ctx, IfcEntity representation)
    {
        ctx.Diagnostics.RecordSupported(representation.GetEntityName());
        var meshes = MeshHelpers.ReadIds(representation, IfcRepresentation.Instance.Items)
            .Select(id => TryBuild(ctx, ctx.GetEntity(id)))
            .Where(m => m is not null)
            .Select(m => m!.Value)
            .ToList();
        return meshes.Count == 0 ? null : MeshHelpers.Merge(meshes);
    }

    static TriangleMesh3D? BuildProductDefinitionShape(MeshingContext ctx, IfcEntity shape)
    {
        ctx.Diagnostics.RecordSupported(shape.GetEntityName());
        var meshes = MeshHelpers.ReadIds(shape, IfcProductRepresentation.Instance.Representations)
            .Select(id => TryBuild(ctx, ctx.GetEntity(id)))
            .Where(m => m is not null)
            .Select(m => m!.Value)
            .ToList();
        return meshes.Count == 0 ? null : MeshHelpers.Merge(meshes);
    }

    /// <summary>Returns the mapping transform for an <see cref="IfcMappedItem"/> (target * origin).</summary>
    public static bool TryGetMappedItemTransform(MeshingContext ctx, IfcEntity mapped, out Matrix4x4 mappingTransform)
    {
        mappingTransform = Matrix4x4.Identity;
        var map = MeshHelpers.ResolveRequired(ctx, mapped, IfcMappedItem.Instance.MappingSource);
        var origin = MeshHelpers.ResolveRequired(ctx, map, IfcRepresentationMap.Instance.MappingOrigin);
        var target = MeshHelpers.ResolveRequired(ctx, mapped, IfcMappedItem.Instance.MappingTarget);
        var transform = Placements.ReadCartesianTransformationOperator3D(ctx, target);
        var originFrame = Placements.ReadAxis2Placement3D(ctx, origin);
        mappingTransform = transform * originFrame.Matrix;
        return true;
    }

    /// <summary>Returns map-local mesh and mapping transform (not baked into vertices).</summary>
    public static bool TryBuildMappedItemLocal(
        MeshingContext ctx,
        IfcEntity mapped,
        out TriangleMesh3D mesh,
        out Matrix4x4 mappingTransform)
    {
        mesh = default;
        if (!TryGetMappedItemTransform(ctx, mapped, out mappingTransform))
            return false;

        var map = MeshHelpers.ResolveRequired(ctx, mapped, IfcMappedItem.Instance.MappingSource);
        var rep = MeshHelpers.ResolveRequired(ctx, map, IfcRepresentationMap.Instance.MappedRepresentation);

        if (!ctx.MeshCache.TryGetValue(map.Id, out mesh))
        {
            var parts = new List<CollectedPart>();
            GeometryPartCollector.CollectParts(ctx, rep, Matrix4x4.Identity, productEntityId: -1, parts);
            if (parts.Count == 0)
                return false;

            mesh = parts.Count == 1
                ? parts[0].Mesh
                : MeshHelpers.Merge(parts.Select(p => MeshHelpers.Transform(p.Mesh, p.Transform)).ToList());
            ctx.MeshCache[map.Id] = mesh;
        }

        return true;
    }

    static TriangleMesh3D? BuildMappedItem(MeshingContext ctx, IfcEntity mapped)
    {
        ctx.Diagnostics.RecordSupported("IFCMAPPEDITEM");
        if (!TryBuildMappedItemLocal(ctx, mapped, out var mesh, out var mappingTransform))
            return null;
        return MeshHelpers.Transform(mesh, mappingTransform);
    }

    static TriangleMesh3D? BuildProduct(MeshingContext ctx, IfcEntity product)
    {
        var representation = MeshHelpers.ResolveOptional(ctx, product, IfcProduct.Instance.Representation);
        if (representation is null)
            return null;

        var mesh = TryBuild(ctx, representation);
        if (mesh is null)
            return null;

        var placement = MeshHelpers.ResolveOptional(ctx, product, IfcProduct.Instance.ObjectPlacement);
        return placement is null ? mesh : MeshHelpers.Transform(mesh.Value, Placements.ReadLocalPlacement(ctx, placement).Matrix);
    }

    static bool LooksLikeProduct(IfcEntity entity)
        => entity.Attributes.Count > IfcProduct.Instance.Representation.Index &&
           entity.GetValue(IfcProduct.Instance.Representation.Index).IsId;
}
