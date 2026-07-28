using Ara3D.BimOpenSchema.IO;
using Ara3D.Utils;

namespace Ara3D.Ifc.Mcp;

/// <summary>The BIM Open Schema conversion of one model, plus the DuckDB database built from it.
/// Both are derived artifacts kept in a private temp folder that is deleted on dispose, so they
/// live and die with the <see cref="IfcSession"/> that owns them.
/// Conversion is a second whole-file parse that also loads geometry — <see cref="IfcToBosConverter"/>
/// hardcodes <c>includeGeometry: true</c> — so it is paid once per session, not per query.</summary>
public sealed class IfcBosArtifacts : IDisposable
{
    private readonly string _folder;

    public IfcBosArtifacts(FilePath source)
    {
        Source = source;
        _folder = Path.Combine(Path.GetTempPath(), "ara3d-ifc-mcp", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_folder);

        var stem = Path.GetFileNameWithoutExtension(source.FullPath);
        BosPath = new FilePath(Path.Combine(_folder, stem + ".bos"));
        DatabasePath = new FilePath(Path.Combine(_folder, stem + ".duckdb"));

        Build(source, BosPath);
        BosPath.BosToDuckDB(DatabasePath);
        IfcDuck.CreateViews(DatabasePath);
        BuiltUtc = DateTime.UtcNow;
    }

    public FilePath Source { get; }

    public FilePath BosPath { get; }

    public FilePath DatabasePath { get; }

    public DateTime BuiltUtc { get; }

    public long BosBytes
        => new FileInfo(BosPath.FullPath).Length;

    /// <summary>Runs the converter directly rather than through <c>IfcToBosConverter.Convert</c>,
    /// which never disposes the <c>IfcFile</c> it opens; a long-lived server cannot afford to leak
    /// a pinned whole-file buffer and a native web-ifc model per conversion.</summary>
    private static void Build(FilePath input, FilePath output)
    {
        var converter = new IfcToBosConverter(input);
        try
        {
            converter.SaveToBos(output);
        }
        finally
        {
            converter.IfcFile?.Dispose();
        }
    }

    public void Dispose()
    {
        try
        {
            Directory.Delete(_folder, recursive: true);
        }
        catch (IOException)
        {
        }
    }
}
