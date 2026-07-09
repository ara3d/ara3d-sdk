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
                var meshes = data.Meshes.ToList();
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
