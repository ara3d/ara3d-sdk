using System.ComponentModel;
using Ara3D.Utils;

namespace Ara3D.Studio.API;

/// <summary>
/// Camera and screenshot capabilities. Optional: <see cref="IHostApplication.Viewport"/> is
/// null on hosts without a viewport (e.g. CPU-only headless runners) — check before use.
/// </summary>
public interface IViewportHost
{
    [Description("Returns the current camera position, target, and orientation.")]
    CameraState GetCameraState();

    [Description("Moves the camera immediately to the given state.")]
    void SetCameraState(CameraState cameraState);

    [Description("Animates the camera to the given state over the given duration in seconds.")]
    void AnimateCameraTo(CameraState cameraState, float duration);

    [Description("Saves a screenshot of the viewport to the given image file path.")]
    void SaveScreenshot(FilePath filePath);
}
