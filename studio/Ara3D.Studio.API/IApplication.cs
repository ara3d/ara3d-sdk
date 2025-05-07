using Ara3D.Data;

namespace Ara3D.Studio.API;

public interface IApplication
{
    IImporter Importer { get; }
    void LoadScene(IRenderScene scene);
}
