using System.Diagnostics;
using Ara3D.Logging;
using Ara3D.Utils;

namespace Ara3D.IfcLoader;

public class IfcFileStats
{
    public IfcFile? File { get; }
    public TimeSpan LoadTime { get; }
    public bool Success => File != null;
    public string ErrorMessage { get; }
    public FilePath FilePath { get; }
    public long FileSize => FilePath.GetFileSize();
    public string FileName => FilePath.GetFileName();

    public IfcFileStats(FilePath fp, bool loadGeometry, ILogger logger)
    {
        var sw = Stopwatch.StartNew();
        FilePath = fp;
        try
        {
            File = IfcFile.Load(fp, loadGeometry, logger);
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        LoadTime = sw.Elapsed;
    }

    public static IfcFileStats Load(FilePath fp, bool loadGeometry = false, ILogger logger = null)
        => new(fp, loadGeometry, logger);
}