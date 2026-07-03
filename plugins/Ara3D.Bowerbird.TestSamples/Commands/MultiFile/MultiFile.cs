using System;
using Ara3D.Bowerbird;

namespace Ara3D.Bowerbird.TestSamples.MultiFile;

public class MultiFileCommand : NamedCommand
{
    public override void Execute()
        => Console.WriteLine(GreeterHelper.Greet("Bowerbird"));
}
