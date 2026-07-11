using System.Text;
using Ara3D.Geometry;
using Ara3D.Ifc.Mesher.Approach1;
using Ara3D.IfcLoader;
using Ara3D.IfcTypes;
using Ara3D.Models;

namespace Ara3D.IfcMeshingComparison.Harness;

/// <summary>One mapping/profile operator's handedness fingerprint (WP-T2 §5.2).</summary>
public sealed record OperatorAudit(int Id, string Name, bool HasExplicitAxis2, double Determinant);

/// <summary>
/// Corpus-wide determinant / mirror audit: how many transformation operators encode a reflection
/// (negative determinant) or an explicit <c>Axis2</c>, and whether any placement basis is left-handed
/// after <c>FrameFromOriginXZ</c> (it should never be — the mesher forces placements right-handed, so
/// a mirror can only ever enter through an operator).
/// </summary>
public sealed record DeterminantAuditSummary(
    int OperatorCount,
    int MirroredOperatorCount,
    int ExplicitAxis2Count,
    int PlacementCount,
    int MirroredPlacementCount,
    IReadOnlyList<OperatorAudit> MirroredOperators);

/// <summary>
/// Level-by-level placement-chain + operator introspection for transform triage (plan §5.1/§5.2).
/// Prints each <c>IFCLOCALPLACEMENT</c> level and each mapping operator (all axes + scale +
/// determinant) so the first diverging level names the bug, instead of only observing that baked
/// world centers differ.
/// </summary>
public static class PlacementIntrospection
{
    static readonly HashSet<string> OperatorNames = new(StringComparer.Ordinal)
    {
        "IFCCARTESIANTRANSFORMATIONOPERATOR2D",
        "IFCCARTESIANTRANSFORMATIONOPERATOR2DNONUNIFORM",
        "IFCCARTESIANTRANSFORMATIONOPERATOR3D",
        "IFCCARTESIANTRANSFORMATIONOPERATOR3DNONUNIFORM",
    };

    public static DeterminantAuditSummary AuditDeterminants(MeshingContext ctx)
    {
        var operators = 0;
        var mirroredOps = 0;
        var explicitAxis2 = 0;
        var placements = 0;
        var mirroredPlacements = 0;
        var mirrored = new List<OperatorAudit>();

        foreach (var entity in ctx.Resolver.GetEntities())
        {
            var name = entity.GetEntityName();
            if (OperatorNames.Contains(name))
            {
                operators++;
                var hasAxis2 = MeshHelpers.ResolveOptional(ctx, entity, IfcCartesianTransformationOperator.Instance.Axis2) is not null;
                if (hasAxis2)
                    explicitAxis2++;
                var det = TryOperatorDeterminant(ctx, entity);
                if (det < 0)
                {
                    mirroredOps++;
                    mirrored.Add(new OperatorAudit(entity.Id, name, hasAxis2, det));
                }
            }
            else if (name == "IFCAXIS2PLACEMENT3D")
            {
                placements++;
                var det = TryPlacementDeterminant(ctx, entity);
                if (det < 0)
                    mirroredPlacements++;
            }
        }

        return new DeterminantAuditSummary(
            operators, mirroredOps, explicitAxis2, placements, mirroredPlacements, mirrored);
    }

    static double TryOperatorDeterminant(MeshingContext ctx, IfcEntity op)
    {
        try
        {
            return MeshHelpers.LinearDeterminant(Placements.ReadProfileTransformationOperator(ctx, op));
        }
        catch
        {
            return double.NaN;
        }
    }

    static double TryPlacementDeterminant(MeshingContext ctx, IfcEntity placement)
    {
        try
        {
            return MeshHelpers.LinearDeterminant(Placements.ReadAxis2Placement3D(ctx, placement).Matrix);
        }
        catch
        {
            return double.NaN;
        }
    }

    public static string DumpPlacementChain(MeshingContext ctx, int entityId)
    {
        var sb = new StringBuilder();
        var entity = ctx.GetEntity(entityId);
        sb.AppendLine($"### #{entityId} {entity.GetEntityName()}");

        var placement = MeshHelpers.ResolveOptional(ctx, entity, IfcProduct.Instance.ObjectPlacement);
        if (placement is null)
            sb.AppendLine("  (no ObjectPlacement)");
        else
            DumpLocalPlacementChain(ctx, placement, sb);

        var rep = MeshHelpers.ResolveOptional(ctx, entity, IfcProduct.Instance.Representation);
        if (rep is not null)
        {
            var mapped = new List<IfcEntity>();
            CollectMappedItems(ctx, rep, mapped);
            if (mapped.Count == 0)
                sb.AppendLine("  (no IFCMAPPEDITEM in representation)");
            foreach (var m in mapped)
                DumpMappedItem(ctx, m, sb);
        }

        return sb.ToString();
    }

    static void DumpLocalPlacementChain(MeshingContext ctx, IfcEntity placement, StringBuilder sb)
    {
        var chain = new List<IfcEntity>();
        var node = placement;
        while (node is not null && node.GetEntityName() == "IFCLOCALPLACEMENT")
        {
            chain.Add(node);
            node = MeshHelpers.ResolveOptional(ctx, node, IfcLocalPlacement.Instance.PlacementRelTo);
        }
        chain.Reverse();
        sb.AppendLine($"  placement chain: {chain.Count} IFCLOCALPLACEMENT levels (root → leaf)");
        for (var i = 0; i < chain.Count; i++)
        {
            var rel = MeshHelpers.ResolveRequired(ctx, chain[i], IfcLocalPlacement.Instance.RelativePlacement);
            var relFrame = Placements.ReadAxis2Placement3D(ctx, rel);
            var cum = Placements.ReadLocalPlacement(ctx, chain[i]);
            sb.AppendLine(
                $"    L{i}: relOrigin={Fmt(relFrame.Origin.Vector3)} relDet={MeshHelpers.LinearDeterminant(relFrame.Matrix):F3} " +
                $"| cumOrigin={Fmt(cum.Origin.Vector3)} cumDet={MeshHelpers.LinearDeterminant(cum.Matrix):F3}");
        }
    }

    static void CollectMappedItems(MeshingContext ctx, IfcEntity entity, List<IfcEntity> mapped)
    {
        switch (entity.GetEntityName())
        {
            case "IFCPRODUCTDEFINITIONSHAPE":
                foreach (var id in MeshHelpers.ReadIds(entity, IfcProductRepresentation.Instance.Representations))
                    CollectMappedItems(ctx, ctx.GetEntity(id), mapped);
                return;
            case "IFCSHAPEREPRESENTATION" or "IFCREPRESENTATION":
                foreach (var id in MeshHelpers.ReadIds(entity, IfcRepresentation.Instance.Items))
                    CollectMappedItems(ctx, ctx.GetEntity(id), mapped);
                return;
            case "IFCMAPPEDITEM":
                mapped.Add(entity);
                var map = MeshHelpers.ResolveOptional(ctx, entity, IfcMappedItem.Instance.MappingSource);
                var rep = map is null ? null : MeshHelpers.ResolveOptional(ctx, map, IfcRepresentationMap.Instance.MappedRepresentation);
                if (rep is not null)
                    CollectMappedItems(ctx, rep, mapped);
                return;
        }
    }

    static void DumpMappedItem(MeshingContext ctx, IfcEntity mapped, StringBuilder sb)
    {
        var target = MeshHelpers.ResolveOptional(ctx, mapped, IfcMappedItem.Instance.MappingTarget);
        var map = MeshHelpers.ResolveOptional(ctx, mapped, IfcMappedItem.Instance.MappingSource);
        var origin = map is null ? null : MeshHelpers.ResolveOptional(ctx, map, IfcRepresentationMap.Instance.MappingOrigin);
        sb.AppendLine($"  IFCMAPPEDITEM #{mapped.Id}:");
        if (origin is not null)
        {
            var f = Placements.ReadAxis2Placement3D(ctx, origin);
            sb.AppendLine($"    mappingOrigin #{origin.Id}: origin={Fmt(f.Origin.Vector3)} det={MeshHelpers.LinearDeterminant(f.Matrix):F3}");
        }
        if (target is not null)
            DumpOperator(ctx, target, sb);
    }

    static void DumpOperator(MeshingContext ctx, IfcEntity op, StringBuilder sb)
    {
        var name = op.GetEntityName();
        var axis1 = ReadDir(ctx, op, IfcCartesianTransformationOperator3D.Instance.Axis1);
        var axis2 = ReadDir(ctx, op, IfcCartesianTransformationOperator.Instance.Axis2);
        var axis3 = ReadDir(ctx, op, IfcCartesianTransformationOperator3D.Instance.Axis3);
        var scale = MeshHelpers.ReadNumber(op, IfcCartesianTransformationOperator.Instance.Scale);
        var det = TryOperatorDeterminant(ctx, op);
        sb.AppendLine(
            $"    {name} #{op.Id}: axis1={axis1} axis2={axis2} axis3={axis3} scale={scale:F3} " +
            $"det={det:F3}{(det < 0 ? "  <-- MIRROR" : "")}");
    }

    static string ReadDir(MeshingContext ctx, IfcEntity op, IfcAttribute attr)
    {
        var dir = MeshHelpers.ResolveOptional(ctx, op, attr);
        return dir is null ? "(default)" : Fmt(Placements.ReadDirection3D(ctx, dir, Vector3.Zero));
    }

    static string Fmt(Vector3 v)
        => $"({v.X.Value:F3},{v.Y.Value:F3},{v.Z.Value:F3})";
}
