using Ara3D.Bowerbird;

namespace Ara3D.Bowerbird.TestSamples.Echo;

public class EchoCommand : NamedCommand
{
    public override void Execute()
        => Console.WriteLine("Hello from Echo!");
}
