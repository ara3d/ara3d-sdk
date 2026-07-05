using Ara3D.BimOpenSchema;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using static Ara3D.BimOpenSchema.CommonRevitParameters;
using Document = Autodesk.Revit.DB.Document;
using Exception = System.Exception;
using RevitParameter = Autodesk.Revit.DB.Parameter;

namespace Ara3D.Bowerbird.RevitSamples;

public class StructuralLayer
{
    public int LayerIndex;
    public MaterialFunctionAssignment Function;
    public double Width;
    public ElementId MaterialId;
    public bool IsCore;
}

public partial class BosDocumentBuilder
{
    public BosDocumentBuilder(BosRevitBuilder bosRevitBuilder, BosDocumentContext context, Func<Element, bool> elementFilter) 
    {
        DocumentContext = context;
        BosRevitBuilder = bosRevitBuilder;
        DocumentIndex = DataBuilder.AddDocument(Document.Title, Document.PathName);
        DescriptorLookup = bosRevitBuilder.DescriptorLookup;

        // When creating the document builder we create the hash set containing all
        // Non-type element ids and all used element ids
        foreach (var e in Document.GetElements())
        {
            if (!elementFilter(e))
                continue;

            // Check the id value
            var idVal = e.Id.Value;
            
            if (NonTypeElementIds.Add(idVal))
            {
                // Add a placeholder entity 
                var ei = bosRevitBuilder.BimDataBuilder.AddEntity();
                Debug.Assert(!ElementToEntityIndex.ContainsKey(idVal));
                
                // Store the entity index and associate it with the id value
                ElementToEntityIndex.Add(idVal, ei);

                // Get the type ID
                var typeId = e.GetTypeId();
                if (typeId != ElementId.InvalidElementId)
                {
                    // Type element IDs 
                    if (TypeElementIds.Add(typeId.Value))
                    {
                        var ei2 = bosRevitBuilder.BimDataBuilder.AddEntity();
                        Debug.Assert(!ElementToEntityIndex.ContainsKey(typeId.Value));
                        ElementToEntityIndex.Add(typeId.Value, ei2);
                    }
                }
            }
        }
    }

    public void ProcessElements()
    {
        ProcessElements(NonTypeElementIds);
        ProcessElements(TypeElementIds);
    }

    public void ProcessElements(IEnumerable<long> ids)
    {
        foreach (var id in ids)
        {
            try
            {
                ProcessElement(id);
            }
            catch (Exception ex)
            {
                AddError(new ElementId(id), ex);
            }
        }
    }

    public const EntityIndex InvalidEntity = (EntityIndex)(-1);

    public BosDocumentContext DocumentContext { get; }
    public BosRevitBuilder BosRevitBuilder { get; }
    public BimOpenSchemaExportSettings Settings => BosRevitBuilder.Settings;
    public BimDataBuilder DataBuilder => BosRevitBuilder.BimDataBuilder;
    public DocumentIndex DocumentIndex { get; }
    public Document Document => DocumentContext.Document;
    public Dictionary<int, EntityIndex> ProcessedConnectors = new();
    public Dictionary<long, EntityIndex> ProcessedCategories = new();
    public Dictionary<string, DescriptorIndex> DescriptorLookup { get; }
    public Dictionary<long, Models.Material> MaterialLookup { get; } = new();
    public HashSet<long> NonTypeElementIds = new();
    public HashSet<long> TypeElementIds = new();
    public Dictionary<long, EntityIndex> ElementToEntityIndex = new();
    public HashSet<long> ProcessedElements = new();

    public IEnumerable<Element> GetElements()
        => DocumentContext.Document.GetElements();

    public IEnumerable<ElementId> GetElementIds()
        => DocumentContext.Document.GetElementIds();
    
    public EntityIndex GetElementIndex(ElementId id)
        => ElementToEntityIndex.GetValueOrDefault(id.Value, InvalidEntity);

    public static (XYZ min, XYZ max)? GetBoundingBoxMinMax(Element element, View view = null)
    {
        if (element == null) return null;
        var bb = element.get_BoundingBox(view);
        return bb == null ? null : (bb.Min, bb.Max);
    }

    public PointIndex AddPoint(XYZ xyz)
        => AddPoint(DataBuilder, xyz);

    public static PointIndex AddPoint(BimDataBuilder bdb, XYZ xyz)
        => bdb.AddPoint(new((float)xyz.X, (float)xyz.Y, (float)xyz.Z));

    public List<StructuralLayer> GetLayers(HostObjAttributes host)
    {
        var compound = host.GetCompoundStructure();
        if (compound == null) return [];
        var r = new List<StructuralLayer>();
        for (var i = 0; i < compound.LayerCount; i++)
        {
            r.Add(new StructuralLayer()
            {
                Function = compound.GetLayerFunction(i),
                IsCore = compound.IsCoreLayer(i),
                LayerIndex = i,
                MaterialId = compound.GetMaterialId(i),
                Width = compound.GetLayerWidth(i)
            });
        }

        return r;
    }

    public void AddParameter(EntityIndex ei, RevitParameterDesc p, Func<XYZ> f)
    { try { AddParameter(ei, p, f()); } catch { } }

    public void AddParameter(EntityIndex ei, RevitParameterDesc p, XYZ xyz)
        => AddParameter(ei, DescriptorLookup[p], AddPoint(xyz));

    public void AddParameter(EntityIndex ei, RevitParameterDesc p, Func<string> f)
    { try { AddParameter(ei, p, f()); } catch { } }

    public void AddParameter(EntityIndex ei, RevitParameterDesc p, string val)
        => AddParameter(ei, DescriptorLookup[p], val ?? "");

    public void AddParameter(EntityIndex ei, DescriptorIndex di, string val)
    {
        var d = DataBuilder.Get(di);
        if (d.Type != ParameterType.String) throw new Exception($"Expected string not {d.Type}");
        DataBuilder.AddParameter(ei, val, di);
    }

    public void AddParameter(EntityIndex ei, RevitParameterDesc p, Func<DateTime> f)
    { try { AddParameter(ei, p, f()); } catch { } }

    public void AddParameter(EntityIndex ei, RevitParameterDesc p, DateTime val)
        => AddParameter(ei, DescriptorLookup[p], val);

    public void AddParameter(EntityIndex ei, DescriptorIndex di, DateTime val)
    {
        var d = DataBuilder.Get(di);
        if (d.Type != ParameterType.String) throw new Exception($"Expected string not {d.Type}");
        var str = val.ToString("o", CultureInfo.InvariantCulture);
        DataBuilder.AddParameter(ei, str, di);
    }

    public void AddParameter(EntityIndex ei, RevitParameterDesc p, ElementId eId, RelationType rt)
    {
        if (eId != ElementId.InvalidElementId)
        {
            var ei2 = ProcessElement(eId);
            AddParameter(ei, DescriptorLookup[p], ei2);
            DataBuilder.AddRelation(ei, ei2, rt);
        }
    }

    public void AddParameter(EntityIndex ei, RevitParameterDesc p, Element e, RelationType rt)
    {
        if (e != null && e.IsValidObject)
        {
            var ei2 = ProcessElement(e.Id);
            AddParameter(ei, DescriptorLookup[p], ei2);
            DataBuilder.AddRelation(ei, ei2, rt);
        }
    }

    public void AddParameter(EntityIndex ei, RevitParameterDesc p, ElementId eId)
    {
        if (eId != ElementId.InvalidElementId)
            AddParameter(ei, DescriptorLookup[p], ProcessElement(eId));
    }

    public void AddParameter(EntityIndex ei, RevitParameterDesc p, Element e)
    {
        if (e != null && e.IsValidObject) 
            AddParameter(ei, DescriptorLookup[p], ProcessElement(e.Id));
    }

    public void AddParameter(EntityIndex ei, RevitParameterDesc p, EntityIndex val)
        => AddParameter(ei, DescriptorLookup[p], val);

    public void AddParameter(EntityIndex ei, DescriptorIndex di, EntityIndex val)
    {
        var d = DataBuilder.Get(di);
        if (d.Type != ParameterType.Entity) throw new Exception($"Expected entity not {d.Type}");
        DataBuilder.AddParameter(ei, val, di);
    }

    public void AddParameter(EntityIndex ei, RevitParameterDesc p, PointIndex val)
        => AddParameter(ei, DescriptorLookup[p], val);

    public void AddParameter(EntityIndex ei, DescriptorIndex di, PointIndex val)
    {
        var d = DataBuilder.Get(di);
        if (d.Type != ParameterType.Point) throw new Exception($"Expected point not {d.Type}");
        DataBuilder.AddParameter(ei, val, di);
    }

    public void AddParameter(EntityIndex ei, RevitParameterDesc p, Func<int> f)
    { try { AddParameter(ei, p, f()); } catch { } }

    public void AddParameter(EntityIndex ei, RevitParameterDesc p, int val)
        => AddParameter(ei, DescriptorLookup[p], val);

    public void AddParameter(EntityIndex ei, DescriptorIndex di, int val)
    {
        var d = DataBuilder.Get(di);
        if (d.Type != ParameterType.Int) throw new Exception($"Expected int not {d.Type}");
        DataBuilder.AddParameter(ei, val, di);
    }

    public void AddParameter(EntityIndex ei, RevitParameterDesc p, Func<bool> f)
    { try { AddParameter(ei, p, f()); } catch { } }

    public void AddParameter(EntityIndex ei, RevitParameterDesc p, bool val)
        => AddParameter(ei, DescriptorLookup[p], val);

    public void AddParameter(EntityIndex ei, DescriptorIndex di, bool val)
    {
        var d = DataBuilder.Get(di);
        if (d.Type != ParameterType.Int) throw new Exception($"Expected int not {d.Type}");
        DataBuilder.AddParameter(ei, val ? 1 : 0, di);
    }

    public void AddParameter(EntityIndex ei, RevitParameterDesc p, Func<double> f)
    { try { AddParameter(ei, p, f()); } catch { } }

    public void AddParameter(EntityIndex ei, RevitParameterDesc p, double val)
        => AddParameter(ei, DescriptorLookup[p], val);

    public void AddParameter(EntityIndex ei, DescriptorIndex di, double val)
    {
        var d = DataBuilder.Get(di);
        if (d.Type != ParameterType.Number) throw new Exception($"Expected double not {d.Type}");
        DataBuilder.AddParameter(ei, val, di);
    }

    public void AddDotNetTypeAsParameter(EntityIndex ei, object o)
        => AddDotNetTypeAsParameter(ei, o.GetType().Name);

    public void AddDotNetTypeAsParameter(EntityIndex ei, string typeName)
        => AddParameter(ei, ObjectTypeName, typeName);

    public void AddError(ElementId id, Exception ex)
        => AddError(ElementToEntityIndex.GetValueOrDefault(id.Value, InvalidEntity), ex);

    public void AddError(EntityIndex ei, Exception ex)
        => DataBuilder.AddDiagnostic(DiagnosticType.ExporterError, ex.Message, DocumentIndex, ei);

    public EntityIndex ProcessCategory(Category category)
    {
        if (category == null || !category.IsValid)
            return InvalidEntity;

        if (ProcessedCategories.TryGetValue(category.Id.Value, out var result))
            return result;

        var r = DataBuilder.AddEntity(
            category.Id.Value, 
            category.Id.ToString(), 
            DocumentIndex, 
            category.Name,
            InvalidEntity,
            InvalidEntity);

        ProcessedCategories.Add(category.Id.Value, r);

        try
        {
            AddDotNetTypeAsParameter(r, category);
            AddParameter(r, CategoryCategoryType, category.CategoryType.ToString());
            AddParameter(r, CategoryBuiltInType, category.BuiltInCategory.ToString());
        }
        catch (Exception ex)
        {
            AddError(r, ex);
        }

        return r;
    }

    public void ProcessCompoundStructure(EntityIndex ei, HostObjAttributes host)
    {
        var layers = GetLayers(host);
        if (layers == null) return;

        // NOTE: is it possible that there is some redundancy here? 
        // Structural layers. 

        foreach (var layer in layers)
        {
            var index = layer.LayerIndex;
            var layerEi = DataBuilder.AddEntity(
                -1,
                $"{host.UniqueId}${index}",
                DocumentIndex,
                $"{host.Name}[{index}]",
                InvalidEntity,
                InvalidEntity);

            try
            {
                AddDotNetTypeAsParameter(layerEi, layer);
                DataBuilder.AddRelation(ei, layerEi, RelationType.HasLayer);

                AddParameter(layerEi, LayerIndex, index);
                AddParameter(layerEi, LayerFunction, layer.Function.ToString());
                AddParameter(layerEi, LayerWidth, layer.Width);
                AddParameter(layerEi, LayerIsCore, layer.IsCore);
                AddParameter(layerEi, LayerMaterialId, layer.MaterialId, RelationType.HasMaterial);
            }
            catch (Exception ex)
            {
                AddError(layerEi, ex);
            }
        }
    }

    public void ProcessTextNote(EntityIndex ei, TextNote tn)
    {
        AddParameter(ei, TextNoteCoord, tn.Coord);
        AddParameter(ei, TextNoteDir, tn.BaseDirection);
        AddParameter(ei, TextNoteHeight, tn.Height);
        AddParameter(ei, TextNoteWidth, tn.Width);
        AddParameter(ei, TextNoteText, tn.Text);
    }

    public void ProcessMaterial(EntityIndex ei, Material m)
    {
        var color = m.Color;            
        AddParameter(ei, MaterialColorRed, color.Red / 255.0);
        AddParameter(ei, MaterialColorGreen, color.Green / 255.0);
        AddParameter(ei, MaterialColorBlue, color.Blue / 255.0);

        AddParameter(ei, MaterialTransparency, m.Transparency / 100.0);
        AddParameter(ei, MaterialShininess, m.Shininess / 128.0);
        AddParameter(ei, MaterialSmoothness, m.Smoothness / 100.0);
        AddParameter(ei, MaterialCategory, m.MaterialCategory);
        AddParameter(ei, MaterialClass, m.MaterialClass);
    }

    public void ProcessFamily(EntityIndex ei, Family f)
    {
        AddParameter(ei, FamilyStructuralCodeName, f.StructuralCodeName);
        AddParameter(ei, FamilyStructuralMaterialType, f.StructuralMaterialType.ToString());
    }

    public void ProcessFamilyInstance(EntityIndex ei, FamilyInstance f)
    {
        AddParameter(ei, FIToRoom, f.ToRoom);
        AddParameter(ei, FIFromRoom, f.FromRoom);
        AddParameter(ei, FIHost, f.Host, RelationType.HostedBy);
        AddParameter(ei, FISpace, f.Space, RelationType.ContainedIn);
        AddParameter(ei, FIRoom, f.Room, RelationType.ContainedIn);
        AddParameter(ei, FIStructuralMaterial, f.StructuralMaterialId, RelationType.HasMaterial);
        AddParameter(ei, FIStructuralUsage, f.StructuralUsage.ToString());
        AddParameter(ei, FIStructuralMaterialType, f.StructuralMaterialType.ToString());
        AddParameter(ei, FIStructuralType, f.StructuralType.ToString());
    }

    public Element GetElement(Document d, ElementId id, ElementId linkedElementId)
    {
        if (linkedElementId == ElementId.InvalidElementId)
            return d.GetElement(id);

        var linkInstance = d.GetElement(id) as RevitLinkInstance;
        return linkInstance?.GetLinkDocument()?
            .GetElement(linkedElementId);
    }

    public void ProcessLevel(EntityIndex ei, Level level)
    {
        AddParameter(ei, LevelElevation, level.Elevation);
        AddParameter(ei, LevelProjectElevation, level.ProjectElevation);
    }

    public void ProcessMaterials(EntityIndex ei, Element e)
    {
        var matIds = e.GetMaterialIds(false);
        foreach (var id in matIds)
        {
            var matId = ProcessElement(id.Value);
            DataBuilder.AddRelation(ei, matId, RelationType.HasMaterial);
        }
    }
    
    public static string GetUnitLabel(RevitParameter p)
    {
        var spec = p.Definition.GetDataType();
        if (!UnitUtils.IsMeasurableSpec(spec))
            return "";
        var unitId = p.GetUnitTypeId();
        return UnitUtils.GetTypeCatalogStringForUnit(unitId);
    }

    public void ProcessParameters(EntityIndex entityIndex, Element element)
    {
        if (!Settings.IncludeParameters)
            return;

        foreach (RevitParameter p in element.Parameters)
        {
            try 
            {
                if (p == null) continue;
                var def = p.Definition;
                if (def == null) continue;
                var groupId = def.GetGroupTypeId();
                var groupLabel = LabelUtils.GetLabelForGroup(groupId);
                var unitLabel = GetUnitLabel(p);
                switch (p.StorageType)
                {
                    case StorageType.Integer:
                        AddParameter(entityIndex, DataBuilder.AddDescriptor(def.Name, unitLabel, groupLabel, ParameterType.Int), p.AsInteger());
                        break;
                    case StorageType.Double:
                        AddParameter(entityIndex, DataBuilder.AddDescriptor(def.Name, unitLabel, groupLabel, ParameterType.Number), p.AsDouble());
                        break;
                    case StorageType.String:
                        AddParameter(entityIndex, DataBuilder.AddDescriptor(def.Name, unitLabel, groupLabel, ParameterType.String), p.AsString());
                        break;
                    case StorageType.ElementId:
                        AddParameter(entityIndex, DataBuilder.AddDescriptor(def.Name, unitLabel, groupLabel, ParameterType.Entity), ProcessElement(p.AsElementId()));
                        break;
                    case StorageType.None:
                    default:
                        AddParameter(entityIndex, DataBuilder.AddDescriptor(def.Name, unitLabel, groupLabel, ParameterType.String), p.AsValueString());
                        break;
                }
            }
            catch (Exception ex) 
            {
            }
        }
    }


    public static bool TryGetLocationEndpoints(
        LocationCurve lc,
        out XYZ startPoint,
        out XYZ endPoint)
    {
        startPoint = null;
        endPoint = null;
        var curve = lc?.Curve;
        if (curve == null) return false;
        if (!curve.IsBound) return false;
        startPoint = curve.GetEndPoint(0);
        endPoint = curve.GetEndPoint(1);
        return true;
    }

    public void AddParameter(EntityIndex entityIndex, RevitParameterDesc p, Connector c)
    {
        if (c == null || p == null)
            return;
        if (!ProcessedConnectors.TryGetValue(c.Id, out var connectorEntityIndex))
            return;
        AddParameter(entityIndex, p, connectorEntityIndex);
    }

    public EntityIndex ProcessElement(ElementId id)
        => ProcessElement(id, id.Value);

    public EntityIndex ProcessElement(long idVal)
        => ProcessElement(new ElementId(idVal), idVal);
    
    public EntityIndex ProcessElement(ElementId id, long idVal)
    {
        if (id == ElementId.InvalidElementId)
            return InvalidEntity;

        // Get the entity index. If it does not exist, then it was filtered out. 
        if (!ElementToEntityIndex.TryGetValue(idVal, out var ei))
            return InvalidEntity;

        // If the element has already been processed we can just return the index
        if (!ProcessedElements.Add(idVal))
            return ei;

        // Get the element 
        var e = Document.GetElement(id);
        if (e == null)
            return InvalidEntity;

        // Get the category
        var category = e.Category;
        var catIndex = ProcessCategory(category);
        var typeIndex = ProcessElement(e.GetTypeId());

        DataBuilder.UpdateEntity(ei, idVal, e.UniqueId, DocumentIndex, e.Name, catIndex, typeIndex);

        AddDotNetTypeAsParameter(ei, e);

        ProcessParameters(ei, e);
        ProcessMaterials(ei, e);

        var bounds = GetBoundingBoxMinMax(e);
        if (bounds.HasValue)
        {
            AddParameter(ei, ElementBoundsMin, bounds.Value.min);
            AddParameter(ei, ElementBoundsMax, bounds.Value.max);
        }

        AddParameter(ei, ElementLevel, e.LevelId, RelationType.ContainedIn);
        AddParameter(ei, ElementAssemblyInstance, e.AssemblyInstanceId, RelationType.PartOf);

        var location = e.Location;
        if (location != null)
        {
            if (location is LocationPoint lp)
            {
                AddParameter(ei, ElementLocationPoint, AddPoint(DataBuilder, lp.Point));
            }

            if (location is LocationCurve lc)
            {
                if (TryGetLocationEndpoints(lc, out var startPoint, out var endPoint))
                {
                    AddParameter(ei, ElementLocationStartPoint, AddPoint(DataBuilder, startPoint));
                    AddParameter(ei, ElementLocationEndPoint, AddPoint(DataBuilder, endPoint));
                }
            }
        }

        AddParameter(ei, ElementCreatedPhase, e.CreatedPhaseId);
        AddParameter(ei, ElementDemolishedPhase, e.DemolishedPhaseId);
        AddParameter(ei, ElementDesignOption, e.DesignOption, RelationType.MemberOf);
        AddParameter(ei, ElementGroup, e.GroupId, RelationType.MemberOf);

        if (e.Document.IsWorkshared)
            AddParameter(ei, ElementWorksetId, e.WorksetId.IntegerValue);

        if (e.ViewSpecific)
            AddParameter(ei, ElementIsViewSpecific, true);

        AddParameter(ei, ElementOwnerView, e.OwnerViewId);

        TryProcessAs<HostObjAttributes>(e, ei, ProcessCompoundStructure);
        TryProcessAs<Level>(e, ei, ProcessLevel);
        TryProcessAs<Family>(e, ei, ProcessFamily);
        TryProcessAs<FamilyInstance>(e, ei, ProcessFamilyInstance);
        TryProcessAs<Material>(e, ei, ProcessMaterial);
        TryProcessAs<TextNote>(e, ei, ProcessTextNote);
        TryProcessAs<SpatialElement>(e, ei, ProcessSpatialElement);
        
        //TryProcessAs<MEPSystem>(e, ei, DEPRECATED_ProcessMepSystem);
        //TryProcessAs<Zone>(e, ei, DEPRECATED_ProcessZone);

        return ei;
    }

    public void TryProcessAs<T>(Element e, EntityIndex entityIndex, Action<EntityIndex, T> processor)
        where T: Element
    {
        if (e is T val)
        {
            try
            {
                processor(entityIndex, val);
            }
            catch (Exception ex)
            {
                AddError(entityIndex, ex);
            }
        }
    }

    public void ProcessSpatialElement(EntityIndex ei, SpatialElement se)
    {
        //TryProcessAs<Space>(se, ei, DEPRECATED_ProcessSpace);
        //TryProcessAs<Area>(se, ei, DEPRECATED_ProcessArea);

        TryProcessAs<Room>(se, ei, ProcessRoom);

        /*
        var options = new SpatialElementBoundaryOptions();
        IList<IList<BoundarySegment>> segmentLists = se.GetBoundarySegments(options);

        var doc = se.Document;
        for (var i = 0; i < segmentLists.Count; i++)
        {
            var segmentList = segmentLists[i];
            foreach (var segment in segmentList)
            {
                ProcessBoundary(doc, ei, segment, i == 0);
            }
        }
        */
    }


    public void ProcessRoom(EntityIndex ei, Room room)
    {
        AddParameter(ei, RoomBaseOffset, room.BaseOffset);
        AddParameter(ei, RoomLimitOffset, room.LimitOffset);
        AddParameter(ei, RoomNumber, room.Number);
        AddParameter(ei, RoomUnboundedHeight, room.UnboundedHeight);
        AddParameter(ei, RoomUpperLimit, room.UpperLimit);
        AddParameter(ei, RoomVolume, room.Volume);
    }


    public void ProcessDocument()
    {
        var d = Document;

        // NOTE: this creates a pseudo-entity for the document, which is used so that we can associate parameters and meta-data with it. 
        var ei = DataBuilder.AddEntity((int)DocumentIndex, d.CreationGUID.ToString(),
            DocumentIndex, d.Title, InvalidEntity, InvalidEntity);

        AddDotNetTypeAsParameter(ei, d);

        var siteLocation = Document.SiteLocation;
            
        var fi = new FileInfo(d.PathName);
        if (fi.Exists)
        {
            var saveDate = fi.LastWriteTimeUtc;
            AddParameter(ei, DocumentLastSaveTime, saveDate);
            var fileInfo = BasicFileInfo.Extract(d.PathName);
            var docVersion = fileInfo.GetDocumentVersion();
            if (docVersion != null)
            {
                AddParameter(ei, DocumentSaveCount, docVersion.NumberOfSaves);
            }
        }

        var warnings = Document.GetWarnings();
        foreach (var w in warnings)
        {
            var type = w.GetSeverity() == FailureSeverity.Warning 
                ? DiagnosticType.RevitWarning
                : DiagnosticType.RevitError;
            DataBuilder.AddDiagnostic(type, w.GetDescriptionText(), DocumentIndex, ei);
        }

        //DEPRECATED_ProcessConnectors();

        try
        {
            AddParameter(ei, DocumentPath, Document.PathName);
            AddParameter(ei, DocumentTitle, Document.Title);
            AddParameter(ei, DocumentIsDetached, Document.IsDetached);
            AddParameter(ei, DocumentIsLinked, Document.IsLinked);
            AddParameter(ei, DocumentExternalPath, DocumentContext.ExternalPath);
            AddParameter(ei, DocumentLinkName, DocumentContext.LinkName);

            if (Document.IsWorkshared)
                AddParameter(ei, DocumentWorksharingGuid, Document.WorksharingCentralGUID.ToString());

            AddParameter(ei, DocumentCreationGuid, Document.CreationGUID.ToString());
            AddParameter(ei, DocumentElevation, siteLocation.Elevation);
            AddParameter(ei, DocumentLatitude, siteLocation.Latitude);
            AddParameter(ei, DocumentLongitude, siteLocation.Longitude);
            AddParameter(ei, DocumentPlaceName, siteLocation.PlaceName);
            AddParameter(ei, DocumentWeatherStationName, siteLocation.WeatherStationName);
            AddParameter(ei, DocumentTimeZone, siteLocation.TimeZone);

            var projectInfo = Document.ProjectInformation;
            if (projectInfo != null)
            {
                AddParameter(ei, ProjectAddress, projectInfo.Address);
                AddParameter(ei, ProjectAuthor, projectInfo.Author);
                AddParameter(ei, ProjectBuildingName, projectInfo.BuildingName);
                AddParameter(ei, ProjectClientName, projectInfo.ClientName);
                AddParameter(ei, ProjectIssueDate, projectInfo.IssueDate);
                AddParameter(ei, ProjectName, projectInfo.Name);
                AddParameter(ei, ProjectNumber, projectInfo.Number);
                AddParameter(ei, ProjectOrgDescription, projectInfo.OrganizationDescription);
                AddParameter(ei, ProjectOrgName, projectInfo.OrganizationName);
                AddParameter(ei, ProjectStatus, projectInfo.Status);
            }
        }
        catch (Exception ex)
        {
            AddError(ei, ex);
        }
    }


  
}