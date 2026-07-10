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
        var meshBuckets = new Dictionary<long, List<int>>();

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

                var parts = new List<ScopedPart>();
                CollectScopedParts(ctx, representation, Matrix4x4.Identity, entity.Id, parts);
                if (parts.Count == 0)
                    return;

                if (voidRelations.TryGetValue(entity.Id, out var openingIds))
                    parts = CarveOpenings(ctx, parts, openingIds, productMatrix, openingSolidCache);

                foreach (var part in parts)
                {
                    var meshIdx = GetOrAddMesh(builder, meshBuckets, part.DedupScope, part.Part.Mesh);
                    // Row-vector convention (System.Numerics): world = local * part * product.
                    var matrix = part.Part.Transform * productMatrix;
                    builder.AddInstance(meshIdx, matrix, Material.Default, part.Part.EntityIndex);
                }
            }, entity.GetEntityName(), $"product #{entity.Id}");
        }

        EmitAggregatedVoidHosts(ctx, builder, meshBuckets, voidRelations, openingSolidCache);
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
        Dictionary<long, List<int>> meshBuckets,
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
                var meshIdx = GetOrAddMesh(builder, meshBuckets, parentId, mesh);
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
                worldMeshes.Add(MeshHelpers.Transform(part.Mesh, part.Transform * childWorld));
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

    static List<ScopedPart> CarveOpenings(
        MeshingContext ctx,
        List<ScopedPart> parts,
        List<int> openingIds,
        Matrix4x4 productMatrix,
        Dictionary<int, List<TriangleMesh3D>> openingSolidCache)
    {
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

        var result = new List<ScopedPart>(parts.Count);
        foreach (var part in parts)
        {
            var toLocal = (part.Part.Transform * productMatrix).Invert;
            var mesh = part.Part.Mesh;
            ctx.Try(() =>
            {
                var localPrisms = worldSolids.Select(w => MeshHelpers.Transform(w, toLocal)).ToList();
                mesh = OpeningCarver.CarveConvex(mesh, localPrisms);
            }, "IFCRELVOIDSELEMENT", $"carve product #{part.Part.EntityIndex}");
            result.Add(part with { Part = new CollectedPart(mesh, part.Part.Transform, part.Part.EntityIndex) });
        }
        return result;
    }

    static int GetOrAddMesh(
        Model3DBuilder builder,
        Dictionary<long, List<int>> meshBuckets,
        int dedupScope,
        TriangleMesh3D mesh)
    {
        var bucketKey = CombineBucketKey(dedupScope, ComputeMeshFingerprint(mesh));
        if (meshBuckets.TryGetValue(bucketKey, out var bucket))
        {
            foreach (var idx in bucket)
            {
                if (MeshesTopologyEqual(builder.Meshes[idx], mesh))
                    return idx;
            }
        }
        else
        {
            bucket = new List<int>();
            meshBuckets[bucketKey] = bucket;
        }

        var meshIdx = builder.Meshes.Count;
        builder.Meshes.Add(mesh);
        bucket.Add(meshIdx);
        return meshIdx;
    }

    static long CombineBucketKey(int dedupScope, int fingerprint)
        => ((long)dedupScope << 32) | (uint)fingerprint;

    readonly record struct ScopedPart(CollectedPart Part, int DedupScope);

    /// <summary>
    /// Like <see cref="GeometryPartCollector.CollectParts"/> but tags each part with a dedup scope
    /// (representation-map or shape-representation express id) so identical bolt caps from different
    /// type maps stay separate, matching web-ifc mesh granularity.
    /// </summary>
    static void CollectScopedParts(
        MeshingContext ctx,
        IfcEntity entity,
        Matrix4x4 parentTransform,
        int productEntityId,
        List<ScopedPart> parts,
        int dedupScope = 0)
    {
        switch (entity.GetEntityName())
        {
            case "IFCPRODUCTDEFINITIONSHAPE":
                foreach (var repId in MeshHelpers.ReadIds(entity, IfcProductRepresentation.Instance.Representations))
                    CollectScopedParts(ctx, ctx.GetEntity(repId), parentTransform, productEntityId, parts, dedupScope);
                return;

            case "IFCSHAPEREPRESENTATION" or "IFCREPRESENTATION":
                if (!IsBodyRepresentation(entity))
                    return;

                foreach (var itemId in MeshHelpers.ReadIds(entity, IfcRepresentation.Instance.Items))
                    CollectScopedParts(ctx, ctx.GetEntity(itemId), parentTransform, productEntityId, parts, entity.Id);
                return;

            case "IFCMAPPEDITEM":
                var map = MeshHelpers.ResolveRequired(ctx, entity, IfcMappedItem.Instance.MappingSource);
                var rep = MeshHelpers.ResolveRequired(ctx, map, IfcRepresentationMap.Instance.MappedRepresentation);
                if (!GeometryDispatcher.TryGetMappedItemTransform(ctx, entity, out var mappingTransform))
                    return;
                CollectScopedParts(ctx, rep, parentTransform * mappingTransform, productEntityId, parts, map.Id);
                return;

            case "IFCFACEBASEDSURFACEMODEL":
                ctx.Diagnostics.RecordSupported("IFCFACEBASEDSURFACEMODEL");
                foreach (var faceId in MeshHelpers.ReadIds(entity, IfcFaceBasedSurfaceModel.Instance.FbsmFaces))
                {
                    var mesh = Brep.BuildFaceBasedSurfaceElement(ctx, ctx.GetEntity(faceId));
                    if (mesh.FaceIndices.Count > 0)
                        parts.Add(new ScopedPart(new CollectedPart(mesh, parentTransform, productEntityId), dedupScope));
                }
                return;

            case "IFCSTYLEDITEM":
                var styledItem = MeshHelpers.ResolveOptional(ctx, entity, IfcStyledItem.Instance.Item);
                if (styledItem is not null)
                    CollectScopedParts(ctx, styledItem, parentTransform, productEntityId, parts, dedupScope);
                return;

            default:
                var built = GeometryDispatcher.TryBuild(ctx, entity);
                if (built is not null && built.Value.FaceIndices.Count > 0)
                    parts.Add(new ScopedPart(new CollectedPart(built.Value, parentTransform, productEntityId), dedupScope));
                return;
        }
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

    /// <summary>
    /// Fast pre-filter for mesh dedup. Samples vertices across the mesh (not only the first ring —
    /// bolt caps share identical coarse hex heads while shanks differ) plus spread triangle indices.
    /// Collisions are resolved by <see cref="MeshesTopologyEqual"/> in <see cref="GetOrAddMesh"/>.
    /// </summary>
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

            var pointCount = mesh.Points.Count;
            var sampleCount = Math.Min(pointCount, 16);
            for (var s = 0; s < sampleCount; s++)
            {
                var i = sampleCount <= 1 ? 0 : (int)((long)s * (pointCount - 1) / (sampleCount - 1));
                var p = mesh.Points[i];
                h = h * 397 ^ p.X.GetHashCode();
                h = h * 397 ^ p.Y.GetHashCode();
                h = h * 397 ^ p.Z.GetHashCode();
            }

            var faceCount = mesh.FaceIndices.Count;
            var triSamples = Math.Min(faceCount, 4);
            for (var s = 0; s < triSamples; s++)
            {
                var ti = triSamples <= 1 ? 0 : (int)((long)s * (faceCount - 1) / (triSamples - 1));
                var t = mesh.FaceIndices[ti];
                h = h * 397 ^ t.A;
                h = h * 397 ^ t.B;
                h = h * 397 ^ t.C;
            }

            return h;
        }
    }

    static bool MeshesTopologyEqual(TriangleMesh3D a, TriangleMesh3D b)
    {
        if (a.Points.Count != b.Points.Count || a.FaceIndices.Count != b.FaceIndices.Count)
            return false;

        for (var i = 0; i < a.Points.Count; i++)
        {
            if (!a.Points[i].Equals(b.Points[i]))
                return false;
        }

        for (var i = 0; i < a.FaceIndices.Count; i++)
        {
            var fa = a.FaceIndices[i];
            var fb = b.FaceIndices[i];
            if (fa.A != fb.A || fa.B != fb.B || fa.C != fb.C)
                return false;
        }

        return true;
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
