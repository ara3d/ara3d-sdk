using Ara3D.Models;

namespace Ara3D.Studio.API;

public interface IModelAsset : IAsset
{ 
    Model Import();  
}
