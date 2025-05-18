using Ara3D.Studio.Data;

namespace Ara3D.Studio.API;

public interface IModelAsset : IAsset
{
    IRenderScene Import();  
}