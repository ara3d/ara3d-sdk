using System.Text.RegularExpressions;
using Ara3D.Geometry;
using Ara3D.Ifc.Mesher.Approach1;
using Ara3D.IfcLoader;
using Ara3D.IfcMeshingComparison.Harness;
using Ara3D.IfcMeshingComparison.Tests.Support;
using Ara3D.IO.StepParser;
using Ara3D.Utils;

namespace Ara3D.IfcMeshingComparison.Tests.PureCSharp;

/// <summary>PRIMARK residual oracle-only IFCDISCRETEACCESSORY triage (post–composite-curve).</summary>
[TestFixture]
public sealed class WpPrimarkResidualTests
{
    static FilePath PrimarkIfc => new(@"c:\Users\cdigg\git\studio\data\20210221PRIMARK.ifc");

    [Test]
    [Explicit("Classify remaining PRIMARK oracle-only accessories by geometry path")]
    [Category("Slow")]
    public void Primark_OracleOnly_GeometryPathHistogram()
    {
        TestFiles.RequireExists(PrimarkIfc);
        var map = OracleEntityMap.Build(PrimarkIfc);
        using var stepFile = new IfcFile(PrimarkIfc, includeGeometry: false);
        var ctx = new MeshingContext(stepFile);
        var (model, _) = ModelAssembler.BuildModel(stepFile);
        var candidateEntities = model.Instances
            .Where(i => i.EntityIndex >= 0)
            .Select(i => i.EntityIndex)
            .ToHashSet();
        var oracleInst = map.OracleInstances
            .GroupBy(i => i.EntityIndex)
            .ToDictionary(g => g.Key, g => g.Count());

        var oracleOnly = map.ProductRepresentationTrees
            .Where(t => !candidateEntities.Contains(t.EntityId) && oracleInst.ContainsKey(t.EntityId))
            .ToList();
        TestContext.WriteLine($"Oracle-only products: {oracleOnly.Count}");

        var hist = new Dictionary<string, int>();
        var nullMesh = 0;
        var builtMesh = 0;
        foreach (var tree in oracleOnly)
        {
            var tags = new SortedSet<string>();
            CollectGeomTagsFromTree(tree, tags);
            var key = tags.Count == 0 ? "(none)" : string.Join("+", tags);
            hist[key] = hist.GetValueOrDefault(key) + 1;

            var mesh = ModelAssembler.BuildEntityMesh(ctx, ctx.GetEntity(tree.EntityId));
            if (mesh is null || mesh.Value.FaceIndices.Count == 0)
                nullMesh++;
            else
                builtMesh++;
        }

        TestContext.WriteLine($"BuildEntityMesh: null/empty={nullMesh}, built={builtMesh}");
        foreach (var (key, count) in hist.OrderByDescending(kv => kv.Value))
            TestContext.WriteLine($"  {count}x {key}");

        TestContext.WriteLine("Sample oracle-only (first 12):");
        foreach (var tree in oracleOnly.Take(12))
        {
            var tags = new SortedSet<string>();
            CollectGeomTagsFromTree(tree, tags);
            var mesh = ModelAssembler.BuildEntityMesh(ctx, ctx.GetEntity(tree.EntityId));
            var status = mesh is null || mesh.Value.FaceIndices.Count == 0
                ? "NULL"
                : $"tris={mesh.Value.FaceIndices.Count}";
            TestContext.WriteLine($"  #{tree.EntityId} {status} [{string.Join("+", tags)}]");
        }
    }

    [Test]
    [Category("Slow")]
    public void Primark_ClippedAccessory_Sample_BuildsOrExplains()
    {
        // #40020 already gated in WpW5; pick another clipping residual if present.
        TestFiles.RequireExists(PrimarkIfc);
        using var file = new IfcFile(PrimarkIfc, includeGeometry: false);
        var ctx = new MeshingContext(file);
        foreach (var id in new[] { 40020, 92859 })
        {
            var entity = ctx.GetEntityOrDefault(id);
            if (entity is null)
                continue;
            var mesh = ModelAssembler.BuildEntityMesh(ctx, entity);
            TestContext.WriteLine(
                $"#{id} {entity.GetEntityName()}: " +
                (mesh is null ? "NULL" : $"tris={mesh.Value.FaceIndices.Count}"));
        }
    }

    /// <summary>
    /// WP-G1 — cluster the oracle-only IFCDISCRETEACCESSORY (and peers) failures by
    /// (profile-def type + curve segment types + normalized thrown-exception message), so the top
    /// failure modes are actionable. Uses the shared MeshingContext and snapshots Diagnostics.Messages
    /// per entity to attribute the reason.
    /// </summary>
    [Test]
    [Explicit("WP-G1: cluster PRIMARK oracle-only build failures by profile/curve/exception")]
    [Category("Slow")]
    public void Primark_OracleOnly_FailureClusters()
    {
        TestFiles.RequireExists(PrimarkIfc);
        var map = OracleEntityMap.Build(PrimarkIfc);
        using var stepFile = new IfcFile(PrimarkIfc, includeGeometry: false);
        var ctx = new MeshingContext(stepFile);
        var (model, _) = ModelAssembler.BuildModel(stepFile);
        var candidateEntities = model.Instances
            .Where(i => i.EntityIndex >= 0)
            .Select(i => i.EntityIndex)
            .ToHashSet();
        var oracleInst = map.OracleInstances
            .GroupBy(i => i.EntityIndex)
            .ToDictionary(g => g.Key, g => g.Count());
        var oracleOnly = map.ProductRepresentationTrees
            .Where(t => !candidateEntities.Contains(t.EntityId) && oracleInst.ContainsKey(t.EntityId))
            .ToList();

        var byEntityType = new Dictionary<string, int>();
        var clusters = new Dictionary<string, (int Count, int Sample, string Reason)>();
        foreach (var tree in oracleOnly)
        {
            var entity = ctx.GetEntityOrDefault(tree.EntityId);
            if (entity is null)
                continue;
            byEntityType[entity.GetEntityName()] = byEntityType.GetValueOrDefault(entity.GetEntityName()) + 1;

            var before = ctx.Diagnostics.Messages.Count;
            var mesh = ModelAssembler.BuildEntityMesh(ctx, entity);
            var built = mesh is not null && mesh.Value.FaceIndices.Count > 0;
            if (built)
                continue;

            var newMessages = ctx.Diagnostics.Messages.Skip(before).ToList();
            var reason = newMessages.Count == 0 ? "(no diagnostic — silent null)" : Normalize(newMessages[^1]);

            var profiles = new SortedSet<string>(StringComparer.Ordinal);
            var curves = new SortedSet<string>(StringComparer.Ordinal);
            CollectTypes(ctx, tree.EntityId, profiles, curves, new HashSet<int>(), 0);
            var profTag = profiles.Count == 0 ? "-" : string.Join("+", profiles);
            var curveTag = curves.Count == 0 ? "-" : string.Join("+", curves);
            var key = $"{profTag} | {curveTag} | {reason}";

            var prev = clusters.GetValueOrDefault(key);
            clusters[key] = (prev.Count + 1, prev.Count == 0 ? tree.EntityId : prev.Sample, reason);
        }

        TestContext.WriteLine($"Oracle-only: {oracleOnly.Count}");
        foreach (var (name, count) in byEntityType.OrderByDescending(kv => kv.Value))
            TestContext.WriteLine($"  entityType {count}x {name}");
        TestContext.WriteLine("\nFailure clusters [profileDefs | curves | reason] (count, sample):");
        foreach (var (key, v) in clusters.OrderByDescending(kv => kv.Value.Count))
            TestContext.WriteLine($"  {v.Count}x  #{v.Sample}  {key}");
    }

    /// <summary>WP-G1 — inspect the actual failing profile rings to attribute the ear-clip failure.</summary>
    [Test]
    [Explicit("WP-G1: inspect sample failing rings (self-intersection vs small-scale epsilon)")]
    [Category("Slow")]
    public void Primark_SampleFailingRings_Inspect()
    {
        TestFiles.RequireExists(PrimarkIfc);
        using var stepFile = new IfcFile(PrimarkIfc, includeGeometry: false);
        var ctx = new MeshingContext(stepFile);
        foreach (var accId in new[] { 79939, 56249 })
        {
            var profileIds = new List<int>();
            CollectProfileEntities(ctx, accId, profileIds, new HashSet<int>(), 0);
            TestContext.WriteLine($"#{accId}: profileEntities={string.Join(",", profileIds)}");
            foreach (var pid in profileIds)
            {
                var profile = ctx.GetEntity(pid);
                try
                {
                    var poly = ProfileBuilder.Build(ctx, profile);
                    var b = poly.Outer.GetBounds();
                    var selfInt = PolygonTriangulator.HasSelfIntersection(poly.Outer);
                    var (minEdge, nearDup, collinear) = RingStats(poly.Outer);
                    TestContext.WriteLine(
                        $"  profile #{pid} {profile.GetEntityName()}: outer={poly.Outer.Count} holes={poly.Holes.Count} " +
                        $"extent=({b.Size.X.Value:F5},{b.Size.Y.Value:F5}) selfIntersect={selfInt} " +
                        $"minEdge={minEdge:E3} nearDup={nearDup} nearCollinear={collinear}");
                    try
                    {
                        var tris = poly.Triangulate();
                        TestContext.WriteLine($"    Triangulate OK: {tris.Count} tris");
                    }
                    catch (Exception ex)
                    {
                        TestContext.WriteLine($"    Triangulate THREW: {ex.Message}");
                    }
                }
                catch (Exception ex)
                {
                    TestContext.WriteLine($"  profile #{pid} {profile.GetEntityName()}: BUILD THREW {ex.Message}");
                }
            }
        }
    }

    /// <summary>Ring shape stats in the profile's own units, relative to its bounding-box extent.</summary>
    static (float MinEdge, int NearDuplicate, int NearCollinear) RingStats(IReadOnlyList<Vector2> ring)
    {
        var n = ring.Count;
        if (n < 3)
            return (0, 0, 0);
        var size = ring.GetBounds().Size;
        var extent = MathF.Max(MathF.Abs((float)size.X.Value), MathF.Abs((float)size.Y.Value));
        var minEdge = float.PositiveInfinity;
        var nearDup = 0;
        var collinear = 0;
        for (var i = 0; i < n; i++)
        {
            var a = ring[(i - 1 + n) % n];
            var b = ring[i];
            var c = ring[(i + 1) % n];
            var edge = (float)b.Distance(c);
            minEdge = MathF.Min(minEdge, edge);
            if (edge < extent * 1e-4f)
                nearDup++;
            // Relative cross: |(b-a)x(c-b)| normalized by extent^2.
            var cross = MathF.Abs(PolygonTriangulator.Cross(a, b, c)) / (extent * extent);
            if (cross < 1e-5f)
                collinear++;
        }
        return (minEdge, nearDup, collinear);
    }

    static void CollectProfileEntities(MeshingContext ctx, int id, List<int> profileIds, HashSet<int> visited, int depth)
    {
        if (depth > 10 || !visited.Add(id))
            return;
        var entity = ctx.GetEntityOrDefault(id);
        if (entity is null)
            return;
        if (entity.GetEntityName().Contains("PROFILEDEF", StringComparison.Ordinal) && !profileIds.Contains(id))
            profileIds.Add(id);
        foreach (var token in entity.Attributes)
            CollectProfileFromToken(ctx, token, profileIds, visited, depth);
    }

    static void CollectProfileFromToken(MeshingContext ctx, StepToken token, List<int> profileIds, HashSet<int> visited, int depth)
    {
        if (token.IsId)
            CollectProfileEntities(ctx, token.AsId(), profileIds, visited, depth + 1);
        else if (token.IsList)
            foreach (var child in token.AsList(ctx.File!.Document))
                CollectProfileFromToken(ctx, child, profileIds, visited, depth);
    }

    static readonly Regex IdNum = new(@"#\d+|\b\d+(\.\d+)?\b", RegexOptions.Compiled);

    static string Normalize(string message)
        => IdNum.Replace(message, "N").Trim();

    static void CollectTypes(
        MeshingContext ctx, int id, SortedSet<string> profiles, SortedSet<string> curves, HashSet<int> visited, int depth)
    {
        if (depth > 10 || !visited.Add(id))
            return;
        var entity = ctx.GetEntityOrDefault(id);
        if (entity is null)
            return;

        var name = entity.GetEntityName();
        if (name.Contains("PROFILEDEF", StringComparison.Ordinal))
            profiles.Add(name);
        else if (name.Contains("CURVE", StringComparison.Ordinal)
                 || name is "IFCLINE" or "IFCCIRCLE" or "IFCELLIPSE" or "IFCPOLYLINE"
                 || name.Contains("BSPLINE", StringComparison.Ordinal)
                 || name.Contains("HALFSPACE", StringComparison.Ordinal))
            curves.Add(name);

        foreach (var token in entity.Attributes)
            CollectFromToken(ctx, token, profiles, curves, visited, depth);
    }

    static void CollectFromToken(
        MeshingContext ctx, StepToken token, SortedSet<string> profiles, SortedSet<string> curves, HashSet<int> visited, int depth)
    {
        if (token.IsId)
            CollectTypes(ctx, token.AsId(), profiles, curves, visited, depth + 1);
        else if (token.IsList)
            foreach (var child in token.AsList(ctx.File!.Document))
                CollectFromToken(ctx, child, profiles, curves, visited, depth);
    }

    static void CollectGeomTagsFromTree(RepresentationTreeNode node, SortedSet<string> tags)
    {
        switch (node.EntityName)
        {
            case "IFCEXTRUDEDAREASOLID":
            case "IFCBOOLEANCLIPPINGRESULT":
            case "IFCBOOLEANRESULT":
            case "IFCPOLYGONALBOUNDEDHALFSPACE":
            case "IFCHALFSPACESOLID":
            case "IFCCOMPOSITECURVE":
            case "IFCARBITRARYCLOSEDPROFILEDEF":
            case "IFCARBITRARYPROFILEDEFWITHVOIDS":
            case "IFCISHAPEPROFILEDEF":
            case "IFCLSHAPEPROFILEDEF":
            case "IFCRECTANGLEHOLLOWPROFILEDEF":
            case "IFCTRIMMEDCURVE":
            case "IFCCIRCLE":
                tags.Add(node.EntityName);
                break;
        }

        foreach (var child in node.Children)
            CollectGeomTagsFromTree(child, tags);
    }
}
