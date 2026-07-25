using System.ComponentModel;
using Ara3D.Utils;

namespace Ara3D.Studio.API;

/// <summary>
/// Geometry export capabilities of a host. Always present, including on headless hosts;
/// the set of supported file extensions may differ per host.
/// </summary>
public interface IExportHost
{
    [Description("Exports the current scene geometry to the given path (.glb, .bfast, or .bos).")]
    Task SaveGeometry(
        [Description("Destination file path; extension selects the format (.glb, .bfast, .bos). Throws if the scene has no geometry or the extension is unsupported.")] FilePath filePath);
}
