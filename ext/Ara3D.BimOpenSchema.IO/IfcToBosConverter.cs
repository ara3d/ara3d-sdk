using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Ara3D.DataTable;
using Ara3D.IfcLoader;
using Ara3D.IO.StepParser;
using Ara3D.Logging;
using Ara3D.Models;
using Ara3D.Utils;
using Parquet;

namespace Ara3D.BimOpenSchema.IO;

public class IfcToBosConverter
{
    public const EntityIndex InvalidEntityIndex = (EntityIndex)(-1);

    public IfcFile IfcFile;
    public IfcPropData PropData;
    public Model3D Model3D;
    public IfcInstanceTypeRelations TypeRelations;
    public BimDataBuilder BimDataBuilder = new();
    public BimGeometryBuilder BimGeometryBuilder = new();
    public BimGeometry BimGeometry => BimDataBuilder.Geometry;
    public List<IfcEntity> BosEntities;
    public Dictionary<int, EntityIndex> IfcIdToBosId = new();
    public Dictionary<string, EntityIndex> CatEntities = new();
    public FilePath Input;
    public IDataSet DataSet;

    public EntityIndex GetCatEntityIndex(int ifcId)
    {
        var entity = GetEntity(ifcId);
        var entityName = entity.GetEntityName();
        if (!CatEntities.TryGetValue(entityName, out var r))
            throw new Exception($"Could not find category index of {entityName}");

        return r;
    }

    public IfcEntity GetEntity(int id)
        => IfcFile.EntityResolver.GetEntity(id);

    public IfcEntity GetEntityOrDefault(int id)
        => IfcFile.EntityResolver.GetEntityOrDefault(id);
    
    public EntityIndex GetBosEntityIndexFromIfc(int id)
        => IfcIdToBosId.GetValueOrDefault(id, InvalidEntityIndex);

    public static readonly HashSet<string> HiddenIfcNames = new([
        "IFCSITE",
        "IFCBUILDING",
        "IFCBUILDINGSTOREY",
        "IFCSPACE",
        "IFCSPATIALZONE",
        "IFCZONE",
        "IFCSITE",  
        "IFCGRID",
        "IFCGRIDAXIS",
        "IFCANNOTATION",
        "IFCVIRTUALGRIDINTERSECTION",   
    ]);

    public static readonly HashSet<string> NonElementIfcClassNames = new([
        "IFCINDEXEDPOLYGONALFACE",
        "IFCPOLYLOOP",
        "IFCFACE",
        "IFCFACEOUTERBOUND",
        "IFCCARTESIANPOINT",
        "IFCRELDEFINESBYPROPERTIES",
        "IFCPROPERTYSET",
        "IFCPROPERTYSINGLEVALUE",
        "IFCAXIS2PLACEMENT3D",
        "IFCSHAPEREPRESENTATION",
        "IFCQUANTITYLENGTH",
        "IFCSTYLEDITEM",
        "IFCLOCALPLACEMENT",
        "IFCQUANTITYAREA",
        "IFCPRODUCTDEFINITIONSHAPE",
        "IFCCARTESIANPOINTLIST3D",
        "IFCMAPPEDITEM",
        "IFCPOLYGONALFACESET",
        "IFCINDEXEDPOLYCURVE",
        "IFCQUANTITYVOLUME",
        "IFCELEMENTQUANTITY",
        "IFCCARTESIANPOINTLIST2D",
        "IFCEXTRUDEDAREASOLID",
        "IFCMATERIALCONSTITUENT",
        "IFCINDEXEDCOLOURMAP",
        "IFCCOLOURRGBLIST",
        "IFCARBITRARYCLOSEDPROFILEDEF",
        "IFCDIRECTION"
    ]);

    public static bool IsMaybeIfcElement(IfcEntity entity)
    {
        var name = entity.GetEntityName();
        return !NonElementIfcClassNames.Contains(name)
            && !name.StartsWith("IFCREL", StringComparison.Ordinal);
    }

    public static void Convert(FilePath input, FilePath output, ILogger logger = null)
        => new IfcToBosConverter(input, logger).SaveToBos(output);

    public IfcToBosConverter(FilePath input, ILogger logger = null)
    {
        Input = input;

        IfcFile = new IfcFile(input, true, logger);

        logger?.Log("Loaded file");
        var model = IfcFile.Model;
        var doc = IfcFile.Document;

        // NOTE: if I want this to go a LOT faster, I can skip STUPID definitions. 
        // We have properties, we have property sets, we have entities, which have properties. 
        // We definitely don't care about all of the geometric bull-shit. Even storing that ... is borderline pointless.
        // Ideally I would throw it away, as the thing loads. 

        logger?.Log("Creating prop data");
        PropData = new IfcPropData(IfcFile);
        logger?.Log($"{PropData.Errors.Count} errors");
        logger?.Log($"{PropData.ObjectToPropSets.Count} objects with prop-sets");
        logger?.Log($"{PropData.PropValues.Count} prop-values");
        logger?.Log($"{PropData.PropSets.Count} prop-sets");

        logger?.Log("Creating type to instance relations");
        TypeRelations = new IfcInstanceTypeRelations(doc);
        logger?.Log($"Found {TypeRelations.InstancesToTypes.Count} instances and {TypeRelations.TypeIds.Count} types");

        logger?.Log("Creating list of IFC entities to convert to BOS entities");

        BosEntities = IfcFile.EntityResolver.GetEntities().Where(IsMaybeIfcElement).ToList();

        logger?.Log($"Found {BosEntities.Count} entities, among {IfcFile.EntityResolver.EntityLookup.Count} total entities");

        // TODO: compute this correctly 
        var docIndex = (DocumentIndex)(-1);

        logger?.Log($"Creating categories");
        var uniqueNames = BosEntities.Select(e => e.GetEntityName().DecodeIfc()).Distinct().ToList();
        foreach (var stepEntityName in uniqueNames)
        {
            var ei = BimDataBuilder.AddEntity(-1, "", docIndex, stepEntityName, (EntityIndex)(-1), (EntityIndex)(-1));
            CatEntities.Add(stepEntityName, ei);
        }

        logger?.Log($"Found and created {uniqueNames.Count} categories from entity names in IFC file");

        logger?.Log($"Creating {TypeRelations.TypeIds.Count} type entities");
        foreach (var id in TypeRelations.TypeIds)
        {
            var e = GetEntity(id);
            var catEi = GetCatEntityIndex(id);
            var name = e.GetIfcRootName();
            var gid = e.GetIfcRootGlobalId();
            var ei = BimDataBuilder.AddEntity(id, gid, docIndex, name, catEi, InvalidEntityIndex);
            IfcIdToBosId.Add(id, ei);
        }

        // NOTE: this is all of the ids. Types have already been processed.   
        var ids = BosEntities.Select(e => e.Id).ToList();

        logger?.Log($"Founds {ids.Count} potential instance entities");
        var catIds = new HashSet<int>();
        foreach (var id in ids)
        {
            if (IfcIdToBosId.ContainsKey(id))
                continue;

            var e = GetEntity(id);
            var catEi = GetCatEntityIndex(id);
            var typeEi = InvalidEntityIndex;
            var name = e.GetIfcRootName();
            var gid = e.GetIfcRootGlobalId();
            if (TypeRelations.InstancesToTypes.TryGetValue(id, out var typeId))
                typeEi = GetBosEntityIndexFromIfc(typeId);
            catIds.Add((int)catEi);
            var ei = BimDataBuilder.AddEntity(id, gid, docIndex, name, catEi, typeEi);
            IfcIdToBosId.Add(id, ei);
        }

        logger?.Log("Creating structural relations");
        var structuralRels = new IfcStructuralRelations(IfcFile);
        var addedRelations = 0;
        foreach (var rel in structuralRels.Relations)
        {
            var a = GetBosEntityIndexFromIfc(rel.From);
            var b = GetBosEntityIndexFromIfc(rel.To);
            if (a == InvalidEntityIndex || b == InvalidEntityIndex)
                continue;
            BimDataBuilder.AddRelation(a, b, IfcStructuralRelationMapping.ToBos(rel.Kind));
            addedRelations++;
        }
        logger?.Log($"Added {addedRelations} structural relations");

        // TEMP:
        logger?.Log($"Found {catIds.Count} unique cat ids");
        var d = CatEntities.ToDictionary(kv => (int)kv.Value, kv => kv.Key);
        var catNames = catIds.Select(ci => d.GetValueOrDefault(ci) ?? "_NOCAT_").Distinct().OrderBy(c => c).ToList();
        foreach (var n in catNames)
            logger?.Log($"Category: {n}");

        logger?.Log($"Computing {PropData.ObjectToPropSets.Count} object-property set pairs");
        foreach (var kv in PropData.ObjectToPropSets)
        {
            var objectId = kv.Key;
            var propSetIdList = kv.Value;

            var bosId = GetBosEntityIndexFromIfc(objectId);
            if (bosId == InvalidEntityIndex)
                continue;

            foreach (var propSetId in propSetIdList)
            {
                var propSet = PropData.PropSets[propSetId];
                var propSetName = propSet.Name.DecodeIfc();
                foreach (var id in propSet.Ids)
                {
                    if (!PropData.PropValues.TryGetValue(id, out var p))
                        continue;

                    // This is a function so that we can recursively resolve entities that are used as property values.
                    // We assume that entities within entities have only one property, which is a very common pattern in IFC files.
                    void ProcessPropValue(string name, StepToken val)
                    {
                        if (val.IsId)
                        {
                            var refId = GetBosEntityIndexFromIfc(p.Value.Value.AsId());
                            BimDataBuilder.AddParameter(bosId, refId, name, "", propSetName);
                        }

                        if (val.IsEntity)
                        {
                            var (type, val2) = val.AsSimpleEntity(IfcFile.Document);
                            ProcessPropValue(name, val2);
                        }
                        else if (val.IsNumber)
                        {
                            var num = val.AsNumber();
                            BimDataBuilder.AddParameter(bosId, num, name, "", propSetName);
                        }
                        else if (val.IsString)
                        {
                            var str = val.AsString();
                            BimDataBuilder.AddParameter(bosId, str, name, "", propSetName);
                        }
                        else
                        {
                            var str = val.ToString() ?? "";
                            BimDataBuilder.AddParameter(bosId, str, name, "", propSetName);
                        }
                    }

                    var propName = p.Name.DecodeIfc();
                    if (!p.Value.HasValue)
                        BimDataBuilder.AddParameter(bosId, "", propName, "", propSetName);
                    else
                        ProcessPropValue(propName, p.Value.Value);
                }
            }
        }

        logger?.Log($"Computed all parameters. Found {BimDataBuilder.Parameters.Count} parameters and {BimDataBuilder.Descriptors.Count} descriptors");

        logger?.Log("Creating geometry");
        Model3D = IfcFile.ToModel3D();

        var tmp = new HashSet<int>();

        var geometryIdPatch = new MultiDictionary<int, int>();
        foreach (var def in doc.Definitions)
        {
            var g = model.GetGeometry((uint)def.Id);
            if (g != null)
            {
                var n = (int)g.Id;
                Debug.Assert(n != def.Id);
                geometryIdPatch.Add(n, def.Id);

                tmp.Add(def.Id);
            }
        }

        var cntTotal = 0;
        var cntNotFound = 0;
        var cntNotFound2 = 0;
        foreach (var inst in Model3D.Instances)
        {
            cntTotal++;
            if (!geometryIdPatch.ContainsKey(inst.EntityIndex))
                cntNotFound++;

            if (!tmp.Contains(inst.EntityIndex))
                cntNotFound2++;
        }

        logger?.Log($"Missing {cntNotFound} of {cntTotal}");
        logger?.Log($"Also missing {cntNotFound2} of {cntTotal}");

        Debug.Assert(BimGeometryBuilder.Meshes.Count == 0);
        foreach (var m in Model3D.Meshes)
            BimGeometryBuilder.AddMesh(m);

        var missingIdCount = 0;
        foreach (var inst in Model3D.Instances)
        {
            var matIndex = BimGeometryBuilder.AddMaterial(inst.Material);
            var tfmIndex = BimGeometryBuilder.AddTransform(inst.Matrix4x4);

            if (!geometryIdPatch.TryGetValue(inst.EntityIndex, out var ifcIdList))
            {
                //logger?.Log($"Count not find {inst.EntityIndex} in the geometryIdPatch");
                missingIdCount++;
                continue;
            }

            if (ifcIdList.Count > 1)
            {
                logger?.Log($"Found multiple ids for {inst.EntityIndex}");
                continue;
            }

            var ifcId = ifcIdList[0];

            var entityIndex = IfcIdToBosId.GetValueOrDefault(ifcId, InvalidEntityIndex);

            if (entityIndex < 0)
            {
                //logger?.Log($"IfcIdToBosID does not contain {ifcId}");
                //missingIdCount++;
            }

            var entity = IfcFile.EntityResolver.GetEntity(ifcId);
            var hidden = entity != null && HiddenIfcNames.Contains(entity.GetEntityName());
            var flags = hidden ? (byte)1 : inst.Flags;
            BimGeometryBuilder.AddInstance((int)entityIndex, matIndex, inst.MeshIndex, tfmIndex, flags);
        }

        logger?.Log($"Found {missingIdCount} missing IDs");

        logger?.Log("Building geometry");
        BimDataBuilder.Geometry = BimGeometryBuilder.BuildModel();

        logger?.Log("Building BIM data");
        var bimData = BimDataBuilder.Build();

        logger?.Log("Creating dataset");
        DataSet = bimData.ToDataSet();
    }

    public void SaveToBos(FilePath output, ILogger logger = null)
    {
        // Writing output 

        logger?.Log($"Creating Zip Archive");
        var fs = new FileStream(output, FileMode.Create, FileAccess.Write, FileShare.None);
        using var zip = new ZipArchive(fs, ZipArchiveMode.Create, leaveOpen: false);

        var parquetCompressionMethod = CompressionMethod.Brotli;
        var parquetCompressionLevel = CompressionLevel.Optimal;
        var zipCompressionLevel = CompressionLevel.Fastest;

        logger?.Log($"Writing non-geometry data to Zip file {output}");
        DataSet.WriteParquetToZip(
            zip,
            parquetCompressionMethod,
            parquetCompressionLevel,
            zipCompressionLevel);

        logger?.Log($"Writing geometry data to Zip file {output}");
        BimDataBuilder.Geometry.WriteParquetToZip(
            zip,
            parquetCompressionMethod,
            parquetCompressionLevel,
            zipCompressionLevel);

        logger?.Log($"Completed");
    }
}