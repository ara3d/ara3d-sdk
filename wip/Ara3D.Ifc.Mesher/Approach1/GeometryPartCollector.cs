using Ara3D.Geometry;
using Ara3D.IfcLoader;
using Ara3D.IfcTypes;
using Ara3D.Models;

namespace Ara3D.Ifc.Mesher.Approach1;

/// <summary>One tessellated mesh part with local transform, owning IFC product express id, and material.</summary>
public readonly record struct CollectedPart(TriangleMesh3D Mesh, Matrix4x4 Transform, int EntityIndex, Material Material)
{
    public CollectedPart(TriangleMesh3D mesh, Matrix4x4 transform, int entityIndex)
        : this(mesh, transform, entityIndex, Material.Default) { }
}

/// <summary>Walks IFC representation trees and emits per-part meshes (no product-level merging).</summary>
public static class GeometryPartCollector
{
    public static void CollectParts(
        MeshingContext ctx,
        IfcEntity entity,
        Matrix4x4 parentTransform,
        int productEntityId,
        List<CollectedPart> parts,
        Material? material = null)
    {
        switch (entity.GetEntityName())
        {
            case "IFCPRODUCTDEFINITIONSHAPE":
                foreach (var repId in MeshHelpers.ReadIds(entity, IfcProductRepresentation.Instance.Representations))
                    CollectParts(ctx, ctx.GetEntity(repId), parentTransform, productEntityId, parts, material);
                return;

            case "IFCSHAPEREPRESENTATION" or "IFCREPRESENTATION":
                if (!IsBodyRepresentation(entity))
                {
                    var identifier = entity.GetString(IfcRepresentation.Instance.RepresentationIdentifier.Index);
                    var repType = entity.GetString(IfcRepresentation.Instance.RepresentationType.Index);
                    ctx.Diagnostics.RecordApproximate(entity.GetEntityName(), $"Skipping non-body rep '{identifier}/{repType}'");
                    return;
                }

                foreach (var itemId in MeshHelpers.ReadIds(entity, IfcRepresentation.Instance.Items))
                    CollectParts(ctx, ctx.GetEntity(itemId), parentTransform, productEntityId, parts, material);
                return;

            case "IFCMAPPEDITEM":
                CollectMappedItem(ctx, entity, parentTransform, productEntityId, parts, material);
                return;

            case "IFCFACEBASEDSURFACEMODEL":
                CollectFaceBasedSurfaceModel(ctx, entity, parentTransform, productEntityId, parts, material);
                return;

            case "IFCSTYLEDITEM":
                var resolved = StyleResolver.TryResolveMaterial(ctx, entity) ?? material;
                var styledItem = MeshHelpers.ResolveOptional(ctx, entity, IfcStyledItem.Instance.Item);
                if (styledItem is not null)
                    CollectParts(ctx, styledItem, parentTransform, productEntityId, parts, resolved);
                return;

            default:
                var mesh = GeometryDispatcher.TryBuild(ctx, entity);
                if (mesh is not null && mesh.Value.FaceIndices.Count > 0)
                {
                    var mat = material ?? ctx.TryGetItemMaterial(entity.Id) ?? Material.Default;
                    parts.Add(new CollectedPart(mesh.Value, parentTransform, productEntityId, mat));
                }
                return;
        }
    }

    static void CollectFaceBasedSurfaceModel(
        MeshingContext ctx,
        IfcEntity model,
        Matrix4x4 parentTransform,
        int productEntityId,
        List<CollectedPart> parts,
        Material? material)
    {
        ctx.Diagnostics.RecordSupported("IFCFACEBASEDSURFACEMODEL");
        var mat = material ?? ctx.TryGetItemMaterial(model.Id) ?? Material.Default;
        foreach (var faceId in MeshHelpers.ReadIds(model, IfcFaceBasedSurfaceModel.Instance.FbsmFaces))
        {
            var mesh = Brep.BuildFaceBasedSurfaceElement(ctx, ctx.GetEntity(faceId));
            if (mesh.FaceIndices.Count > 0)
                parts.Add(new CollectedPart(mesh, parentTransform, productEntityId, mat));
        }
    }

    static void CollectMappedItem(
        MeshingContext ctx,
        IfcEntity mapped,
        Matrix4x4 parentTransform,
        int productEntityId,
        List<CollectedPart> parts,
        Material? material)
    {
        ctx.Diagnostics.RecordSupported("IFCMAPPEDITEM");

        var map = MeshHelpers.ResolveRequired(ctx, mapped, IfcMappedItem.Instance.MappingSource);
        var rep = MeshHelpers.ResolveRequired(ctx, map, IfcRepresentationMap.Instance.MappedRepresentation);
        if (!GeometryDispatcher.TryGetMappedItemTransform(ctx, mapped, out var mappingTransform))
            return;

        CollectParts(ctx, rep, parentTransform * mappingTransform, productEntityId, parts, material);
    }

    static bool IsBodyRepresentation(IfcEntity representation)
    {
        static bool Matches(string? value) =>
            !string.IsNullOrEmpty(value) &&
            (value.Contains("Body", StringComparison.OrdinalIgnoreCase) ||
             value.Contains("SweptSolid", StringComparison.OrdinalIgnoreCase) ||
             value.Contains("Tessellation", StringComparison.OrdinalIgnoreCase) ||
             value.Contains("Brep", StringComparison.OrdinalIgnoreCase) ||
             value.Contains("Facetation", StringComparison.OrdinalIgnoreCase) ||
             value.Contains("Clipping", StringComparison.OrdinalIgnoreCase) ||
             value.Contains("SurfaceModel", StringComparison.OrdinalIgnoreCase) ||
             value.Contains("MappedRepresentation", StringComparison.OrdinalIgnoreCase));

        var identifier = representation.GetString(IfcRepresentation.Instance.RepresentationIdentifier.Index);
        if (Matches(identifier))
            return true;

        var repType = representation.GetString(IfcRepresentation.Instance.RepresentationType.Index);
        return Matches(repType);
    }
}
