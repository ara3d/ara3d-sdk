using Ara3D.Data;

namespace Ara3D.Studio.API;

public interface IModelAsset : IAsset
{
    IRenderScene Import();  
}