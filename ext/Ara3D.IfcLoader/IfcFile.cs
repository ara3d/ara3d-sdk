using Ara3D.IO.StepParser;
using Ara3D.Logging;
using Ara3D.Utils;

namespace Ara3D.IfcLoader;

public class IfcFile : IDisposable
{
    public enum IfcSchemaEnum { Ifc2x3, Ifc4, Ifc4x3 };

    public StepDocument Document;
    public IfcModel Model;
    public IntPtr ApiPtr;
    public IfcEntityResolver EntityResolver { get; private set; }
    public IfcSchemaEnum SchemaEnum { get; }
    public FilePath FilePath { get; set; }
    public (double X, double Y, double Z) OriginOffset { get; }

    public IfcFile(FilePath fp, bool includeGeometry, ILogger? logger = null)
    {
        if (!File.Exists(fp))
            throw new FileNotFoundException($"File not found: {fp}");
        FilePath = fp;

        logger?.Log($"Starting load of {fp.GetFileName()}");
        LoadNonGeometryData(logger);
        
        if (Document == null)
            throw new Exception($"Could not load Document from {fp}");

        var header = Document.Header;
        SchemaEnum = IfcSchemaEnum.Ifc2x3;
        if (header.FileSchema.StartsWith("IFC4X3", StringComparison.InvariantCultureIgnoreCase))
            SchemaEnum = IfcSchemaEnum.Ifc4x3;
        else if (header.FileSchema.StartsWith("IFC4", StringComparison.InvariantCultureIgnoreCase))
            SchemaEnum = IfcSchemaEnum.Ifc4;

        logger?.Log($"Retrieving schema"); 
        EntityResolver = new(Document);
            
        logger?.Log($"Created EntityResolver found {EntityResolver.EntityLookup.Count} entities");

        OriginOffset = GetSitePlacement();

        if (includeGeometry)
            LoadGeometryData(logger);
    }

    public (double X, double Y, double Z) GetSitePlacement()
    {
        return (0, 0, 0);
    }

    private void LoadGeometryData(ILogger? logger)
    {
        logger?.Log($"Loading IFC geometry");
        ApiPtr = WebIfcDll.InitializeApi();
        Model = new IfcModel(this, ApiPtr, WebIfcDll.LoadModel(ApiPtr, FilePath));
        logger?.Log($"Completed loading IFC geometry");
    }

    private void LoadNonGeometryData(ILogger? logger)
    {
        logger?.Log($"Loading IFC data");
        Document = new StepDocument(FilePath, logger);
        logger?.Log($"Completed loading IFC data");
    }

    public static IfcFile Load(FilePath fp, bool includeGeometry, ILogger logger = null)
        => new(fp, includeGeometry, logger);

    public bool GeometryDataLoaded 
        => Model != null;

    public void Dispose()
    {
        Document.Dispose();
        Document = null;
        Model = null;
        ApiPtr = IntPtr.Zero;
        EntityResolver = null;
    }
}