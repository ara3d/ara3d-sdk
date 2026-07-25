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
    void SetCameraState(
        [Description("Target yaw/pitch/position; not JSON-coercible through call_host (use GetCameraState to read the current shape).")] CameraState cameraState);

    [Description("Animates the camera to the given state over the given duration in seconds.")]
    void AnimateCameraTo(
        [Description("Target yaw/pitch/position; not JSON-coercible through call_host.")] CameraState cameraState,
        [Description("Animation duration in seconds; 0 snaps immediately.")] float duration);

    [Description("Saves a screenshot of the viewport to the given image file path.")]
    void SaveScreenshot(
        [Description("Destination image file path; extension selects the format (e.g. .png).")] FilePath filePath);

    [Description("Opens the OSPRay path-tracer preview window (creates it on first use).")]
    void OpenPathTracer();

    [Description("Opens the path-tracer window if needed and turns its progressive Render on or off.")]
    void SetPathTracerRendering(
        [Description("True starts/resumes progressive rendering; false pauses it.")] bool enabled);
}
