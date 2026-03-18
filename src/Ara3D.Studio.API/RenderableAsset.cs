using Ara3D.Models;

namespace Ara3D.Studio.API;

public class RenderableAsset : IAsset
{
    public object Value => RenderData;
    public IReadOnlyList<object> Attachments => [];

    public RenderModelData RenderData { get; private set; }

    public RenderableAsset()
        => RenderData = new RenderModelData(3);

    public RenderableAsset(RenderModelData data)
        => RenderData = data;

    public void Update(IModel3D model)
        => RenderData.Update(model);

    public void Dispose()
    {
        RenderData?.Dispose();
        RenderData = null;
    }

    ~RenderableAsset()
        => Dispose();
}