using System;
using System.Collections.Generic;
using Ara3D.BimOpenSchema;
using Autodesk.Revit.DB;
using Document = Autodesk.Revit.DB.Document;
using FilePath = Ara3D.Utils.FilePath;

namespace Ara3D.Bowerbird.RevitSamples;

public class BosRevitBuilder
{
    public BosRevitBuilder(
        Options options,
        BimOpenSchemaExportSettings settings,
        Document initialDoc,
        Func<Element, bool> elementFilter)
    {
        InitialDocument = initialDoc;
        BimDataBuilder = new BimDataBuilder();
        Options = options;
        Settings = settings;

        MeshGatherer = new(this);

        // TEMP
        BimDataBuilder.Manifest = new();

        BimDataBuilder.Manifest.GeneratorApplication = "Revit 2025 BIM Open Schema Parquet Exporter";
        BimDataBuilder.Manifest.GeneratorVersion = "0.9.5";
        BimDataBuilder.Manifest.ExportOptions = Settings;
        CreateCommonDescriptors();

        var context = new BosDocumentContext(this, InitialDocument);

        DocumentContexts = settings.IncludeLinks
            ? context.GatherLinkedDocuments()
            : [context];

        DocumentBuilders = new();
        foreach (var dc in DocumentContexts)
        {
            var db = new BosDocumentBuilder(this, dc, elementFilter);
            DocumentBuilders.Add(db);
        }
    }

    public FilePath InputPath 
        => new FilePath(InitialDocument.PathName);

    public void CreateCommonDescriptors()
    {
        foreach (var p in CommonRevitParameters.GetParameters())
        {
            var desc = BimDataBuilder.AddDescriptor(p.Name, "", "RevitAPI", p.Type);
            DescriptorLookup.Add(p.Name, desc);
        }
    }

    public Document InitialDocument { get; }
    public List<BosDocumentContext> DocumentContexts { get; }
    public List<BosDocumentBuilder> DocumentBuilders { get; }
    public BimDataBuilder BimDataBuilder { get; }
    public BimOpenSchemaExportSettings Settings { get; }
    public Options Options { get; }
    public Dictionary<string, DescriptorIndex> DescriptorLookup = new();
    public BimGeometryBuilder GeometryBuilder { get; } = new();
    public BosMeshGatherer MeshGatherer { get; }
}