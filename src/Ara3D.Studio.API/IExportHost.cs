using Ara3D.Utils;

namespace Ara3D.Studio.API;

/// <summary>
/// Geometry export capabilities of a host. Always present, including on headless hosts;
/// the set of supported file extensions may differ per host.
/// </summary>
public interface IExportHost
{
    Task SaveGeometry(FilePath filePath);
}
