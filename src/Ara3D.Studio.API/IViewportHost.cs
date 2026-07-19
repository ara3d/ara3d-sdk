using Ara3D.Utils;

namespace Ara3D.Studio.API;

/// <summary>
/// Camera and screenshot capabilities. Optional: <see cref="IHostApplication.Viewport"/> is
/// null on hosts without a viewport (e.g. CPU-only headless runners) — check before use.
/// </summary>
public interface IViewportHost
{
    CameraState GetCameraState();
    void SetCameraState(CameraState cameraState);
    void AnimateCameraTo(CameraState cameraState, float duration);
    void SaveScreenshot(FilePath filePath);
}
