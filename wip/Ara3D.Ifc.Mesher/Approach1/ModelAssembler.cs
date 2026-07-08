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

                foreach (var part in parts)
                {
                    var meshIdx = GetOrAddMesh(builder, meshIndexByFingerprint, part.Mesh);
                    var matrix = productMatrix * part.Transform;
                    builder.AddInstance(meshIdx, matrix, Material.Default, part.EntityIndex);
                }
            }, entity.GetEntityName(), $"product #{entity.Id}");
        }

        RecordOpeningRelations(ctx);
        return (builder.Build(), ctx.Diagnostics);
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
                Booleans.RecordOpeningDiagnostic(ctx);
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
