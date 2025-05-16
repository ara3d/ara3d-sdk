using Ara3D.Data;

namespace Ara3D.Studio.API;

public interface IApplication
{
    IModelLoader ModelLoader { get; }
}
