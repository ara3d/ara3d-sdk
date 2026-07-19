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
    Task<IAsset> LoadAssetAsync(FilePath filePath, bool inApplication);
}
