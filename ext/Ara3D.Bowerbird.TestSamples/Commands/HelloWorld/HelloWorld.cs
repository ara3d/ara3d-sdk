using System;
using Ara3D.Bowerbird;

namespace Ara3D.Bowerbird.TestSamples.HelloWorld;

public class HelloWorldCommand : NamedCommand
{
    public override void Execute()
        => Console.WriteLine("Hello World!");
}
