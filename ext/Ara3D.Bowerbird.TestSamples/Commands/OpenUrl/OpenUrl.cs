using Ara3D.Bowerbird;
using Ara3D.Utils;

namespace Ara3D.Bowerbird.TestSamples.OpenUrl;

public class OpenUrlCommand : NamedCommand
{
    public override void Execute()
        => ProcessUtil.OpenUrl("https://ara3d.com");
}
