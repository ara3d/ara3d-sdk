using Ara3D.Logging;
using Ara3D.Studio.API;
using Ara3D.Utils;

namespace Ara3D.Studio.WpfControls;

public class DefaultHostApplication : IHostApplication
{
    public ILogger Logger { get; } = Ara3D.Logging.Logger.Null;

    public void Invalidate(object obj)
    { }

    public void RefreshUI(object obj)
    { }

    public void AnimateCameraTo(CameraState cameraState, float duration)
    { }

    public CameraState GetCameraState()
        => default;

    public SynchronizationContext SynchronizationContext
        => SynchronizationContext.Current;

    public Task SaveGeometry(FilePath filePath)
    {
        return Task.CompletedTask;
    }

    public void SaveScreenshot(FilePath filePath)
    {
    }

    public Task<IAsset> LoadAssetAsync(FilePath filePath, bool inApplication)
    {
        throw new NotImplementedException();
    }
}