using Ara3D.IfcLoader;
using Ara3D.Utils;

namespace Ara3D.Ifc.Mcp;

/// <summary>An open IFC file and the indexes derived from it. Relations and property data each
/// cost a whole-file scan, so they are built on first use and then kept. Every <see cref="IfcEntity"/>
/// handed out points into the file's pinned buffer and is invalid once this session is disposed.
/// Geometry is never loaded, which keeps the native web-ifc DLL out of the picture.</summary>
public sealed class IfcSession : IDisposable
{
    private IfcRelations? _relations;
    private IfcPropData? _properties;

    public IfcSession(FilePath path)
    {
        Path = path;
        File = IfcFile.Load(path, includeGeometry: false);
        OpenedUtc = DateTime.UtcNow;
    }

    public FilePath Path { get; }

    public IfcFile File { get; }

    public DateTime OpenedUtc { get; }

    public IfcEntityResolver Resolver
        => File.EntityResolver;

    public string Schema
        => File.Document.Header.FileSchema ?? "";

    public IfcRelations Relations
        => _relations ??= new IfcRelations(File);

    public IfcPropData Properties
        => _properties ??= new IfcPropData(File);

    public void Dispose()
        => File.Dispose();
}
