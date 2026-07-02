using Ara3D.Bowerbird;

namespace Ara3D.Bowerbird.TestSamples.Args;

public class ArgsCommand : NamedCommand
{
    public override void Execute(object parameter)
        => Console.WriteLine($"Parameter: {parameter ?? "(null)"}");
}
