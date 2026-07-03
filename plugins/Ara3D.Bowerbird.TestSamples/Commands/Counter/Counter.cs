using System;
using Ara3D.Bowerbird;

namespace Ara3D.Bowerbird.TestSamples.Counter;

public class CounterCommand : NamedCommand
{
    public static int Count;

    public override void Execute()
        => Console.WriteLine($"You have executed this command {++Count} time(s)");
}
