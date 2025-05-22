using Ara3D.Utils;

namespace Ara3D.Studio.API;

public interface IAsset
{
    string Name { get; }
    FilePath FilePath { get; }
}