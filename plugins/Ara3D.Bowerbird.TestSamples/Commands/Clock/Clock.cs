using System;
using Ara3D.Bowerbird;

namespace Ara3D.Bowerbird.TestSamples.Clock;

public class ClockCommand : NamedCommand
{
    public override void Execute()
    {
        var now = DateTime.Now;
        Console.WriteLine($"ISO: {now:O}");
        Console.WriteLine($"Local: {now:f}");
    }
}
