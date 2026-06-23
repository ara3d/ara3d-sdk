using Ara3D.Logging;
using Ara3D.Utils;

namespace Ara3D.Studio.API;

public interface IHostApplication
{
    ILogger Logger { get; }
    void Invalidate(object obj);
    void RefreshUI(object obj);
    void AnimateCameraTo(CameraState cameraState, float duration);
    CameraState GetCameraState();
    SynchronizationContext SynchronizationContext { get; }
    Task SaveGeometry(FilePath filePath);
    void SaveScreenshot(FilePath filePath);
    Task<IAsset> LoadAssetAsync(FilePath filePath, bool inApplication);
}