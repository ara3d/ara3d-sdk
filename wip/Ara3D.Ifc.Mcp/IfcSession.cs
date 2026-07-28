using Ara3D.Ifc.Mesher;
using Ara3D.Ifc.Mesher.Approach1;
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
    private IfcBosArtifacts? _bos;
    private IfcMeshingResult? _meshing;

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

    /// <summary>The BOS conversion and its DuckDB database, built on first use. Unlike the other
    /// indexes this one re-reads the file from disk with geometry enabled, so it is by far the most
    /// expensive thing a session can hold.</summary>
    public IfcBosArtifacts Bos
        => _bos ??= new IfcBosArtifacts(Path);

    public bool BosIsBuilt
        => _bos != null;

    /// <summary>Triangle meshes for the model, built on first use and then kept. The Approach1 mesher
    /// reads STEP geometry definitions in pure C# — its own <c>Build(FilePath)</c> overload opens the
    /// file with <c>includeGeometry: false</c> — so meshing reuses this session's already-open file and
    /// never needs the native web-ifc tessellator or a geometry-enabled reopen. Failure comes back as a
    /// non-success result, not an exception.</summary>
    public IfcMeshingResult Meshing
        => _meshing ??= new Approach1Mesher().Build(File);

    public bool MeshingIsBuilt
        => _meshing != null;

    public IfcBosArtifacts RebuildBos()
    {
        _bos?.Dispose();
        _bos = null;
        return Bos;
    }

    public void Dispose()
    {
        _bos?.Dispose();
        File.Dispose();
    }
}
