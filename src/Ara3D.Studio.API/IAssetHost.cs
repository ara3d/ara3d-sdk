using System.ComponentModel;
using Ara3D.Utils;

namespace Ara3D.Studio.API;

/// <summary>
/// Asset loading capabilities of a host. Always present, including on headless hosts.
/// </summary>
public interface IAssetHost
{
    /// <summary>
    /// Loads a file into an asset. When <paramref name="inApplication"/> is true the asset is
    /// added to the host's scene (with progress reporting and error handling); otherwise it is
    /// loaded standalone and the caller owns it.
    /// </summary>
    [Description("Loads a file into an asset; when inApplication is true it is added to the scene, otherwise loaded standalone.")]
    Task<IAsset> LoadAssetAsync(
        [Description("Absolute or relative path to the source file (e.g. .glb, .ifc, .bos).")] FilePath filePath,
        [Description("True adds the loaded asset to the host's scene with progress/error handling; false returns it standalone for the caller to own.")] bool inApplication);
}
