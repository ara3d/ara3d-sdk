using Ara3D.Geometry;
using Ara3D.IfcLoader;
using Ara3D.IfcTypes;
using Ara3D.Models;

namespace Ara3D.Ifc.Mesher.Approach1;

/// <summary>Traverses IFC products and assembles a <see cref="Model3D"/> with per-part instancing.</summary>
public static class ModelAssembler
{
    public static (Model3D Model, MeshingDiagnostics Diagnostics) BuildModel(IfcFile file)
    {
        var ctx = new MeshingContext(file);
        var builder = new Model3DBuilder();
        var meshIndexByFingerprint = new Dictionary<int, int>();

        var voidRelations = OpeningCarver.CollectVoidRelations(ctx);
        var openingSolidCache = new Dictionary<int, List<TriangleMesh3D>>();

        foreach (var entity in ctx.Resolver.GetEntities())
        {
            if (!IsProduct(entity))
                continue;

            ctx.Try(() =>
            {
                var representation = MeshHelpers.ResolveOptional(ctx, entity, IfcProduct.Instance.Representation);
                if (representation is null)
                    return;

                var placement = MeshHelpers.ResolveOptional(ctx, entity, IfcProduct.Instance.ObjectPlacement);
                var productMatrix = placement is null
                    ? Matrix4x4.Identity
                    : Placements.ReadLocalPlacement(ctx, placement).Matrix;

                var parts = new List<CollectedPart>();
                GeometryPartCollector.CollectParts(ctx, representation, Matrix4x4.Identity, entity.Id, parts);
                if (parts.Count == 0)
                    return;

                if (voidRelations.TryGetValue(entity.Id, out var openingIds))
                    parts = CarveOpenings(ctx, parts, openingIds, productMatrix, openingSolidCache);

                foreach (var part in parts)
                {
                    var meshIdx = GetOrAddMesh(builder, meshIndexByFingerprint, part.Mesh);
                    var matrix = productMatrix * part.Transform;
                    builder.AddInstance(meshIdx, matrix, Material.Default, part.EntityIndex);
                }
            }, entity.GetEntityName(), $"product #{entity.Id}");
        }

        EmitAggregatedVoidHosts(ctx, builder, meshIndexByFingerprint, voidRelations, openingSolidCache);
        RecordOpeningRelations(ctx);
        return (builder.Build(), ctx.Diagnostics);
    }

    /// <summary>
    /// Emits geometry for elements that declare openings (<c>IFCRELVOIDSELEMENT</c> hosts) but carry
    /// no representation of their own, deriving the host solid from their <c>IFCRELAGGREGATES</c>
    /// children and carving the host's openings from it. This mirrors web-ifc, which must materialise
    /// such a host's solid (from its aggregated parts) before it can subtract the voids — e.g. the
    /// duplex flat roof <c>#22475</c>, whose geometry is the aggregated slab <c>#22492</c>. Aggregate
    /// parents that are not void hosts (stairs, and spatial containers such as building/storey/site)
    /// are left to emit their children individually, matching the oracle.
    /// </summary>
    static void EmitAggregatedVoidHosts(
        MeshingContext ctx,
        Model3DBuilder builder,
        Dictionary<int, int> meshIndexByFingerprint,
        Dictionary<int, List<int>> voidRelations,
        Dictionary<int, List<TriangleMesh3D>> openingSolidCache)
    {
        foreach (var (parentId, childIds) in CollectAggregateChildren(ctx))
        {
            if (!voidRelations.TryGetValue(parentId, out var openingIds))
                continue;
            var parent = ctx.GetEntityOrDefault(parentId);
            if (parent is null || IsProduct(parent))
                continue; // products with their own representation already emitted above

            ctx.Try(() =>
            {
                var worldMeshes = CollectAggregatedChildMeshes(ctx, childIds);
                if (worldMeshes.Count == 0)
                    return;

                var mesh = MeshHelpers.Merge(worldMeshes);
                var worldPrisms = new List<TriangleMesh3D>();
                foreach (var openingId in openingIds)
                {
                    if (!openingSolidCache.TryGetValue(openingId, out var solids))
                        openingSolidCache[openingId] = solids = OpeningCarver.BuildOpeningWorldSolids(ctx, openingId);
                    worldPrisms.AddRange(solids);
                }
                mesh = OpeningCarver.CarveConvex(mesh, worldPrisms);
                if (mesh.FaceIndices.Count == 0)
                    return;

                // The child meshes are baked into world coordinates, so the instance transform is
                // identity (matching how the oracle stores these attributed-up meshes).
                var meshIdx = GetOrAddMesh(builder, meshIndexByFingerprint, mesh);
                builder.AddInstance(meshIdx, Matrix4x4.Identity, Material.Default, parentId);
                ctx.Diagnostics.RecordApproximate("IFCRELAGGREGATES",
                    $"Aggregated void-host geometry attributed to #{parentId}");
            }, parent.GetEntityName(), $"aggregated void host #{parentId}");
        }
    }

    /// <summary>Builds the world-space meshes of an aggregate's children (each placed by its own placement).</summary>
    static List<TriangleMesh3D> CollectAggregatedChildMeshes(MeshingContext ctx, IReadOnlyList<int> childIds)
    {
        var worldMeshes = new List<TriangleMesh3D>();
        foreach (var childId in childIds)
        {
            var child = ctx.GetEntityOrDefault(childId);
            var childRep = child is null ? null : MeshHelpers.ResolveOptional(ctx, child, IfcProduct.Instance.Representation);
            if (childRep is null)
                continue;

            var childParts = new List<CollectedPart>();
            GeometryPartCollector.CollectParts(ctx, childRep, Matrix4x4.Identity, childId, childParts);
            if (childParts.Count == 0)
                continue;

            var placement = MeshHelpers.ResolveOptional(ctx, child!, IfcProduct.Instance.ObjectPlacement);
            var childWorld = placement is null
                ? Matrix4x4.Identity
                : Placements.ReadLocalPlacement(ctx, placement).Matrix;

            foreach (var part in childParts)
                worldMeshes.Add(MeshHelpers.Transform(part.Mesh, childWorld * part.Transform));
        }
        return worldMeshes;
    }

    /// <summary>relating (parent) express id -&gt; aggregated child express ids (IFCRELAGGREGATES).</summary>
    static Dictionary<int, List<int>> CollectAggregateChildren(MeshingContext ctx)
    {
        var map = new Dictionary<int, List<int>>();
        foreach (var e in ctx.Resolver.GetEntities())
        {
            if (e.GetEntityName() != "IFCRELAGGREGATES")
                continue;
            var parentId = MeshHelpers.ReadOptionalId(e, IfcRelAggregates.Instance.RelatingObject);
            if (parentId is null)
                continue;
            var kids = MeshHelpers.ReadIds(e, IfcRelAggregates.Instance.RelatedObjects);
            if (kids.Count == 0)
                continue;
            if (!map.TryGetValue(parentId.Value, out var list))
                map[parentId.Value] = list = new List<int>();
            list.AddRange(kids);
        }
        return map;
    }

    static List<CollectedPart> CarveOpenings(
        MeshingContext ctx,
        List<CollectedPart> parts,
        List<int> openingIds,
        Matrix4x4 productMatrix,
        Dictionary<int, List<TriangleMesh3D>> openingSolidCache)
    {
        // Resolve each opening's solid once, in world coordinates.
        var worldSolids = new List<TriangleMesh3D>();
        foreach (var openingId in openingIds)
        {
            if (!openingSolidCache.TryGetValue(openingId, out var solids))
            {
                solids = OpeningCarver.BuildOpeningWorldSolids(ctx, openingId);
                openingSolidCache[openingId] = solids;
            }
            worldSolids.AddRange(solids);
        }
        if (worldSolids.Count == 0)
            return parts;

        var result = new List<CollectedPart>(parts.Count);
        foreach (var part in parts)
        {
            var toLocal = (productMatrix * part.Transform).Invert;
            var mesh = part.Mesh;
            ctx.Try(() =>
            {
                var localPrisms = worldSolids.Select(w => MeshHelpers.Transform(w, toLocal)).ToList();
                mesh = OpeningCarver.CarveConvex(mesh, localPrisms);
            }, "IFCRELVOIDSELEMENT", $"carve product #{part.EntityIndex}");
            result.Add(new CollectedPart(mesh, part.Transform, part.EntityIndex));
        }
        return result;
    }

    static int GetOrAddMesh(
        Model3DBuilder builder,
        Dictionary<int, int> meshIndexByFingerprint,
        TriangleMesh3D mesh)
    {
        var fingerprint = ComputeMeshFingerprint(mesh);
        if (!meshIndexByFingerprint.TryGetValue(fingerprint, out var meshIdx))
        {
            meshIdx = builder.Meshes.Count;
            builder.Meshes.Add(mesh);
            meshIndexByFingerprint[fingerprint] = meshIdx;
        }
        return meshIdx;
    }

    static int ComputeMeshFingerprint(TriangleMesh3D mesh)
    {
        unchecked
        {
            var h = mesh.FaceIndices.Count;
            h = h * 397 ^ mesh.Points.Count;
            var bounds = MeshHelpers.GetBounds(mesh);
            h = h * 397 ^ bounds.Min.X.GetHashCode();
            h = h * 397 ^ bounds.Min.Y.GetHashCode();
            h = h * 397 ^ bounds.Min.Z.GetHashCode();
            h = h * 397 ^ bounds.Max.X.GetHashCode();
            h = h * 397 ^ bounds.Max.Y.GetHashCode();
            h = h * 397 ^ bounds.Max.Z.GetHashCode();

            var sampleCount = Math.Min(mesh.Points.Count, 16);
            for (var i = 0; i < sampleCount; i++)
            {
                var p = mesh.Points[i];
                h = h * 397 ^ p.X.Value.GetHashCode();
                h = h * 397 ^ p.Y.Value.GetHashCode();
                h = h * 397 ^ p.Z.Value.GetHashCode();
            }

            return h;
        }
    }

    static bool IsProduct(IfcEntity entity)
    {
        var name = entity.GetEntityName();
        if (name.StartsWith("IFCREL", StringComparison.Ordinal) ||
            name is "IFCOWNERHISTORY" or "IFCPROJECT" or "IFCGEOMETRICREPRESENTATIONCONTEXT" or "IFCGEOMETRICREPRESENTATIONSUBCONTEXT" or
            "IFCOPENINGELEMENT")
            return false;

        if (entity.Attributes.Count <= IfcProduct.Instance.Representation.Index)
            return false;
        if (!entity.GetValue(IfcProduct.Instance.Representation.Index).IsId)
            return false;
        return true;
    }

    static void RecordOpeningRelations(MeshingContext ctx)
    {
        foreach (var entity in ctx.Resolver.GetEntities())
        {
            if (entity.GetEntityName() == "IFCRELVOIDSELEMENT")
                ctx.Diagnostics.RecordApproximate("IFCRELVOIDSELEMENT", "Opening subtracted via convex carve");
        }
    }

    public static TriangleMesh3D? BuildEntityMesh(MeshingContext ctx, IfcEntity entity)
    {
        if (IsProduct(entity))
        {
            var representation = MeshHelpers.ResolveOptional(ctx, entity, IfcProduct.Instance.Representation);
            if (representation is null)
                return null;

            var parts = new List<CollectedPart>();
            GeometryPartCollector.CollectParts(ctx, representation, Matrix4x4.Identity, entity.Id, parts);
            if (parts.Count == 0)
                return null;

            var meshes = parts
                .Select(p => MeshHelpers.Transform(p.Mesh, p.Transform))
                .ToList();
            var mesh = meshes.Count == 1 ? meshes[0] : MeshHelpers.Merge(meshes);

            var placement = MeshHelpers.ResolveOptional(ctx, entity, IfcProduct.Instance.ObjectPlacement);
            return placement is null
                ? mesh
                : MeshHelpers.Transform(mesh, Placements.ReadLocalPlacement(ctx, placement).Matrix);
        }
        return GeometryDispatcher.TryBuild(ctx, entity);
    }
}
