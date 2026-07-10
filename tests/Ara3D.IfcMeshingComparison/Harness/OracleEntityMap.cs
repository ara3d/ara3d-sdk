using System.Text.Json;
using System.Text.Json.Serialization;
using Ara3D.Geometry;
using Ara3D.IfcLoader;
using Ara3D.Ifc.Mesher.Approach1;
using Ara3D.IfcTypes;
using Ara3D.IO.StepParser;
using Ara3D.Models;
using Ara3D.Utils;

namespace Ara3D.IfcMeshingComparison.Harness;

public sealed record OracleInstanceRecord(
    int EntityIndex,
    int MeshIndex,
    float[] Transform,
    float[] BoundsMin,
    float[] BoundsMax,
    int TriangleCount);

public sealed record RepresentationTreeNode(
    int EntityId,
    string EntityName,
    string? Notes,
    IReadOnlyList<RepresentationTreeNode> Children);

public sealed record OracleEntityMapDocument(
    string FileName,
    DateTime GeneratedUtc,
    int OracleInstanceCount,
    int OracleMeshCount,
    IReadOnlyDictionary<int, int> OracleInstancesPerEntity,
    IReadOnlyList<OracleInstanceRecord> OracleInstances,
    IReadOnlyList<RepresentationTreeNode> ProductRepresentationTrees);

public static class OracleEntityMap
{
    public static OracleEntityMapDocument Build(FilePath ifcPath, Model3D? oracle = null)
    {
        TestFiles.RequireExists(ifcPath);
        oracle ??= LoadOracleModel(ifcPath);

        var instances = oracle.Instances
            .Where(inst => inst.MeshIndex >= 0 && inst.MeshIndex < oracle.Meshes.Count)
            .Select(inst => ToRecord(oracle, inst))
            .ToList();

        var counts = instances
            .GroupBy(i => i.EntityIndex)
            .ToDictionary(g => g.Key, g => g.Count());

        using var stepFile = new IfcFile(ifcPath, includeGeometry: false);
        var trees = BuildProductTrees(stepFile);

        return new OracleEntityMapDocument(
            ifcPath.GetFileName(),
            DateTime.UtcNow,
            oracle.Instances.Count,
            oracle.Meshes.Count,
            counts,
            instances,
            trees);
    }

    public static void Write(FilePath ifcPath, FilePath? outputPath = null, Model3D? oracle = null)
    {
        var document = Build(ifcPath, oracle);
        var path = outputPath ?? MapPath(ifcPath);
        TestFiles.ReportsDir.Create();
        path.CreateDirectory();
        var json = JsonSerializer.Serialize(document, JsonOptions);
        File.WriteAllText(path, json);
    }

    public static FilePath MapPath(FilePath ifcPath)
        => TestFiles.ReportsDir.RelativeFolder("oracle_maps").RelativeFile(ifcPath.GetFileNameWithoutExtension() + ".json");

    static Model3D LoadOracleModel(FilePath ifcPath)
    {
        var bfastPath = WebIfcBfastOracle.OraclePath(ifcPath);
        if (bfastPath.Exists())
        {
            var data = RenderModelBfastSerializer.Load(bfastPath);
            try
            {
                var instances = data.InstanceData.ToList();
                // Clone before Dispose — BFAST mesh buffers are views into the native/mmap payload.
                var meshes = data.Meshes
                    .Select(m => new TriangleMesh3D(m.Points.ToList(), m.FaceIndices.ToList()))
                    .ToList();
                if (instances.Count > 0)
                    return new Model3D(meshes, instances);
            }
            finally
            {
                data.Dispose();
            }
        }

        using var file = TestFiles.LoadWithOracleGeometry(ifcPath);
        return file.ToModel3D();
    }

    static float Sanitize(float value) => float.IsFinite(value) ? value : 0f;

    static OracleInstanceRecord ToRecord(Model3D model, InstanceStruct inst)
    {
        var mesh = model.Meshes[inst.MeshIndex];
        var transformed = MeshHelpers.Transform(mesh, inst.Matrix4x4);
        var bounds = MeshHelpers.GetBounds(transformed);
        return new OracleInstanceRecord(
            inst.EntityIndex,
            inst.MeshIndex,
            MatrixToArray(inst.Matrix4x4),
            [Sanitize((float)bounds.Min.X), Sanitize((float)bounds.Min.Y), Sanitize((float)bounds.Min.Z)],
            [Sanitize((float)bounds.Max.X), Sanitize((float)bounds.Max.Y), Sanitize((float)bounds.Max.Z)],
            mesh.FaceIndices.Count);
    }

    static float[] MatrixToArray(Matrix4x4 m)
        =>
        [
            Sanitize(m.M11), Sanitize(m.M12), Sanitize(m.M13), Sanitize(m.M14),
            Sanitize(m.M21), Sanitize(m.M22), Sanitize(m.M23), Sanitize(m.M24),
            Sanitize(m.M31), Sanitize(m.M32), Sanitize(m.M33), Sanitize(m.M34),
            Sanitize(m.M41), Sanitize(m.M42), Sanitize(m.M43), Sanitize(m.M44),
        ];

    static List<RepresentationTreeNode> BuildProductTrees(IfcFile file)
    {
        var trees = new List<RepresentationTreeNode>();
        foreach (var entity in file.EntityResolver.GetEntities())
        {
            if (!IsProduct(entity))
                continue;

            var representation = ResolveOptional(file, entity, IfcProduct.Instance.Representation);
            if (representation is null)
                continue;

            trees.Add(new RepresentationTreeNode(
                entity.Id,
                entity.GetEntityName(),
                "product",
                [AnnotateRepresentation(file, representation)]));
        }
        return trees;
    }

    static RepresentationTreeNode AnnotateRepresentation(IfcFile file, IfcEntity entity)
    {
        return entity.GetEntityName() switch
        {
            "IFCPRODUCTDEFINITIONSHAPE" => new RepresentationTreeNode(
                entity.Id,
                entity.GetEntityName(),
                "product-definition-shape",
                MeshHelpers.ReadIds(entity, IfcProductRepresentation.Instance.Representations)
                    .Select(id => AnnotateRepresentation(file, file.EntityResolver.GetEntity(id)))
                    .ToList()),
            "IFCSHAPEREPRESENTATION" or "IFCREPRESENTATION" => new RepresentationTreeNode(
                entity.Id,
                entity.GetEntityName(),
                entity.GetString(IfcRepresentation.Instance.RepresentationIdentifier.Index),
                MeshHelpers.ReadIds(entity, IfcRepresentation.Instance.Items)
                    .Select(id => AnnotateRepresentation(file, file.EntityResolver.GetEntity(id)))
                    .ToList()),
            "IFCMAPPEDITEM" => AnnotateMappedItem(file, entity),
            "IFCSTYLEDITEM" => AnnotateStyledItem(file, entity),
            _ => new RepresentationTreeNode(
                entity.Id,
                entity.GetEntityName(),
                "geometry-item",
                []),
        };
    }

    static RepresentationTreeNode AnnotateMappedItem(IfcFile file, IfcEntity mapped)
    {
        var map = ResolveRequired(file, mapped, IfcMappedItem.Instance.MappingSource);
        var rep = ResolveRequired(file, map, IfcRepresentationMap.Instance.MappedRepresentation);
        return new RepresentationTreeNode(
            mapped.Id,
            mapped.GetEntityName(),
            $"maps #{map.Id}",
            [AnnotateRepresentation(file, rep)]);
    }

    static RepresentationTreeNode AnnotateStyledItem(IfcFile file, IfcEntity styled)
    {
        var item = ResolveOptional(file, styled, IfcStyledItem.Instance.Item);
        return item is null
            ? new RepresentationTreeNode(styled.Id, styled.GetEntityName(), "styled-item (empty)", [])
            : new RepresentationTreeNode(
                styled.Id,
                styled.GetEntityName(),
                "styled-item",
                [AnnotateRepresentation(file, item)]);
    }

    static bool IsProduct(IfcEntity entity)
        => entity.Attributes.Count > IfcProduct.Instance.Representation.Index &&
           entity.GetValue(IfcProduct.Instance.Representation.Index).IsId &&
           entity.GetEntityName() is not ("IFCOWNERHISTORY" or "IFCPROJECT");

    static IfcEntity? ResolveOptional(IfcFile file, IfcEntity entity, IfcAttribute attribute)
    {
        var token = entity.GetValue(attribute.Index);
        return token.IsId ? file.EntityResolver.GetEntityOrDefault(token.AsId()) : null;
    }

    static IfcEntity ResolveRequired(IfcFile file, IfcEntity entity, IfcAttribute attribute)
    {
        var resolved = ResolveOptional(file, entity, attribute);
        return resolved ?? throw new InvalidOperationException($"{entity.GetEntityName()} #{entity.Id} missing {attribute.Name}");
    }

    static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter() },
    };
}

/// <summary>
/// One candidate entity paired with the oracle entity whose mesh best matches by shape. When
/// <see cref="OracleEntityId"/> differs from <see cref="CandidateEntityId"/>, the oracle BFAST
/// mis-tagged the mesh (web-ifc artifact — see WP-O1 / WP-W9).
/// </summary>
public sealed record TrustedEntityPair(int CandidateEntityId, int OracleEntityId, double MatchScore);

/// <summary>
/// Shape-based remapping for oracle per-entity mesh comparison. Consult this when scoring against
/// web-ifc oracles on files with known mis-tag clusters (duplex slabs, example shared entities).
/// </summary>
public sealed class OracleTrustedPairing
{
    public IReadOnlyList<TrustedEntityPair> Pairs { get; }
    public int MisTaggedCount { get; }

    readonly Dictionary<int, int> _oracleByCandidate;

    public OracleTrustedPairing(IReadOnlyList<TrustedEntityPair> pairs, int misTaggedCount)
    {
        Pairs = pairs;
        MisTaggedCount = misTaggedCount;
        _oracleByCandidate = pairs.ToDictionary(p => p.CandidateEntityId, p => p.OracleEntityId);
    }

    public int OracleEntityIdFor(int candidateEntityId)
        => _oracleByCandidate.GetValueOrDefault(candidateEntityId, candidateEntityId);

    public bool IsMisTagged(int candidateEntityId)
        => _oracleByCandidate.TryGetValue(candidateEntityId, out var oracleId) && oracleId != candidateEntityId;

    public static OracleTrustedPairing Identity(IEnumerable<int> entityIds)
    {
        var ids = entityIds.ToList();
        return new OracleTrustedPairing(
            ids.Select(id => new TrustedEntityPair(id, id, 1.0)).ToList(),
            0);
    }
}

/// <summary>
/// Detects web-ifc per-entity mesh mis-tags and builds trusted candidate→oracle entity pairings
/// for shape comparison. Mis-tagging is inherent to web-ifc (not introduced by BFAST I/O).
/// </summary>
public static class OracleTrustedPairingBuilder
{
    const double MisTagDirectScoreThreshold = 0.5;
    const double MisTagAlternateScoreThreshold = 0.90;

    public static OracleTrustedPairing Build(Model3D candidate, Model3D oracle)
    {
        var candMeshes = ModelComparer.EntityMeshes(candidate);
        var oracleMeshes = ModelComparer.EntityMeshes(oracle);
        var shared = candMeshes.Keys.Intersect(oracleMeshes.Keys).OrderBy(id => id).ToList();
        if (shared.Count == 0)
            return OracleTrustedPairing.Identity([]);

        var candShapes = shared
            .Where(id => IsComparable(DescribeShape(candMeshes[id])))
            .ToDictionary(id => id, id => DescribeShape(candMeshes[id])!);
        var oracleShapes = shared
            .Where(id => IsComparable(DescribeShape(oracleMeshes[id])))
            .ToDictionary(id => id, id => DescribeShape(oracleMeshes[id])!);

        var comparable = candShapes.Keys.Intersect(oracleShapes.Keys).ToList();
        var pairs = new List<TrustedEntityPair>(comparable.Count);
        var misTagged = 0;

        foreach (var id in comparable)
        {
            var mine = candShapes[id];
            var direct = ShapeSimilarity(mine, oracleShapes[id]);
            var oracleId = id;
            var best = direct;

            if (direct < MisTagDirectScoreThreshold)
            {
                foreach (var (otherId, otherShape) in oracleShapes)
                {
                    if (otherId == id)
                        continue;
                    var s = ShapeSimilarity(mine, otherShape);
                    if (s > best)
                    {
                        best = s;
                        oracleId = otherId;
                    }
                }
            }

            if (oracleId != id && best >= MisTagAlternateScoreThreshold)
                misTagged++;

            pairs.Add(new TrustedEntityPair(id, oracleId, best));
        }

        return new OracleTrustedPairing(pairs, misTagged);
    }

    /// <summary>Oracle per-entity meshes keyed by candidate entity id, using trusted pairing.</summary>
    public static Dictionary<int, TriangleMesh3D> RemapOracleMeshes(
        Model3D candidate,
        Model3D oracle,
        OracleTrustedPairing? pairing = null)
    {
        pairing ??= Build(candidate, oracle);
        var oracleMeshes = ModelComparer.EntityMeshes(oracle);
        var result = new Dictionary<int, TriangleMesh3D>();
        foreach (var p in pairing.Pairs)
        {
            if (oracleMeshes.TryGetValue(p.OracleEntityId, out var mesh))
                result[p.CandidateEntityId] = mesh;
        }
        return result;
    }

    sealed record ShapeDescriptor(
        double Volume,
        double Area,
        double BoundaryLength,
        double SphereRadius,
        float[] ObbExtents,
        double Linearity,
        double Planarity,
        double Scattering);

    static ShapeDescriptor? DescribeShape(TriangleMesh3D mesh)
    {
        if (mesh.FaceIndices.Count < 1 || mesh.Points.Count < 3)
            return null;
        try
        {
            var vectors = SamplePoints(mesh.Points.Select(p => p.Vector3).ToList(), 512);
            var pca = new PrincipalComponentAnalysis(vectors);
            var obb = vectors.FitOrientedBox(pca.Frame);
            return new ShapeDescriptor(
                Math.Abs(MeshHelpers.SignedVolume(mesh)),
                SurfaceArea(mesh),
                BoundaryLength(mesh),
                BoundingSphereRadius(mesh.Points),
                SortedExtents(obb.Size),
                pca.Linearity,
                pca.Planarity,
                pca.Scattering);
        }
        catch
        {
            return null;
        }
    }

    static List<Vector3> SamplePoints(IReadOnlyList<Vector3> points, int maxCount)
    {
        if (points.Count <= maxCount)
            return points.ToList();
        var step = (points.Count - 1) / (double)(maxCount - 1);
        var sampled = new List<Vector3>(maxCount);
        for (var i = 0; i < maxCount; i++)
            sampled.Add(points[(int)Math.Round(i * step)]);
        return sampled;
    }

    static bool IsComparable(ShapeDescriptor? d)
        => d is not null && (d.Area > 1e-9 || d.Volume > 1e-9);

    static double ShapeSimilarity(ShapeDescriptor a, ShapeDescriptor b)
    {
        var volume = RatioSimilarity(a.Volume, b.Volume);
        var area = RatioSimilarity(a.Area, b.Area);
        var obb = (
            ExtentRatioScore(a.ObbExtents[0], b.ObbExtents[0]) +
            ExtentRatioScore(a.ObbExtents[1], b.ObbExtents[1]) +
            ExtentRatioScore(a.ObbExtents[2], b.ObbExtents[2])) / 3.0;
        var sphere = RatioSimilarity(a.SphereRadius, b.SphereRadius);
        var pca = Math.Clamp(1.0 - (
            Math.Abs(a.Linearity - b.Linearity) +
            Math.Abs(a.Planarity - b.Planarity) +
            Math.Abs(a.Scattering - b.Scattering)) / 3.0, 0.0, 1.0);
        var boundary = RatioSimilarity(a.BoundaryLength, b.BoundaryLength);
        return 0.25 * volume + 0.15 * area + 0.25 * obb + 0.10 * sphere + 0.15 * pca + 0.10 * boundary;
    }

    static double RatioSimilarity(double a, double b, double maxRatio = 3.0)
    {
        var absA = Math.Abs(a);
        var absB = Math.Abs(b);
        if (absA <= 1e-9 && absB <= 1e-9)
            return 1.0;
        if (absA <= 1e-9 || absB <= 1e-9)
            return 0.0;
        var ratio = Math.Max(absA, absB) / Math.Min(absA, absB);
        return ratio <= maxRatio ? 1.0 - (ratio - 1.0) / (maxRatio - 1.0) : 0.0;
    }

    static double SurfaceArea(TriangleMesh3D mesh)
    {
        var area = 0.0;
        foreach (var f in mesh.FaceIndices)
        {
            var p = mesh.Points[f.A].Vector3;
            var q = mesh.Points[f.B].Vector3;
            var r = mesh.Points[f.C].Vector3;
            area += 0.5 * Vector3.Cross(q - p, r - p).Length.Value;
        }
        return area;
    }

    static double BoundaryLength(TriangleMesh3D mesh)
    {
        if (mesh.FaceIndices.Count == 0)
            return 0.0;

        var canon = new Dictionary<(int, int, int), int>();
        var pos = new List<Vector3>();
        int Canon(int i)
        {
            var v = mesh.Points[i].Vector3;
            var key = ((int)MathF.Round(v.X * 1e5f), (int)MathF.Round(v.Y * 1e5f), (int)MathF.Round(v.Z * 1e5f));
            if (canon.TryGetValue(key, out var idx))
                return idx;
            idx = pos.Count;
            pos.Add(v);
            canon[key] = idx;
            return idx;
        }

        var edges = new Dictionary<(int, int), int>();
        void AddEdge(int a, int b)
        {
            var k = a < b ? (a, b) : (b, a);
            edges.TryGetValue(k, out var c);
            edges[k] = c + 1;
        }

        foreach (var f in mesh.FaceIndices)
        {
            var a = Canon(f.A);
            var b = Canon(f.B);
            var c = Canon(f.C);
            AddEdge(a, b);
            AddEdge(b, c);
            AddEdge(c, a);
        }

        var length = 0.0;
        foreach (var (edge, count) in edges)
            if (count == 1)
                length += (pos[edge.Item1] - pos[edge.Item2]).Length.Value;
        return length;
    }

    static double BoundingSphereRadius(IReadOnlyList<Point3D> pts)
    {
        if (pts.Count == 0)
            return 0.0;
        var x = FarthestFrom(pts, pts[0].Vector3);
        var y = FarthestFrom(pts, x);
        var center = (x + y) * 0.5f;
        var radius = (y - center).Length.Value;
        foreach (var p in pts)
        {
            var v = p.Vector3;
            var d = (v - center).Length.Value;
            if (d > radius)
            {
                var newRadius = (radius + d) * 0.5f;
                center += (v - center) * ((newRadius - radius) / d);
                radius = newRadius;
            }
        }
        return radius;
    }

    static Vector3 FarthestFrom(IReadOnlyList<Point3D> pts, Vector3 from)
    {
        var best = from;
        var bestDist = -1f;
        foreach (var p in pts)
        {
            var v = p.Vector3;
            var d = (v - from).LengthSquared.Value;
            if (d > bestDist)
            {
                bestDist = d;
                best = v;
            }
        }
        return best;
    }

    static float[] SortedExtents(Vector3 size)
        => new[] { size.X.Value, size.Y.Value, size.Z.Value }.OrderBy(x => x).ToArray();

    static double ExtentRatioScore(float a, float b, double maxRatio = 3.0)
    {
        if (a <= 0 && b <= 0)
            return 1.0;
        if (a <= 0 || b <= 0)
            return 0.0;
        var ratio = Math.Max(a, b) / Math.Min(a, b);
        if (ratio <= 1.0)
            return 1.0;
        return ratio <= maxRatio ? 1.0 - (ratio - 1.0) / (maxRatio - 1.0) : 0.0;
    }
}
