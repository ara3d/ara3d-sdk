using Ara3D.Logging;
using Ara3D.Studio.API;

namespace Ara3D.Studio.WpfControls;

public class DefaultHostApplication : IHostApplication
{
    public ILogger Logger { get; } = Ara3D.Logging.Logger.Null;

    public void Invalidate(object obj)
    { }

    public void RebuildUI(object obj)
    { }

    public void AnimateCameraTo(CameraState cameraState)
    { }

    public void SetCameraState(CameraState cameraState)
    { }

    public CameraState GetCameraState()
        => default;
}