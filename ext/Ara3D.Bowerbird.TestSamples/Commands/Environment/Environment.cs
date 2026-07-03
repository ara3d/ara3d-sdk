using System;
using System.Runtime.InteropServices;
using Ara3D.Bowerbird;

namespace Ara3D.Bowerbird.TestSamples.Environment;

public class EnvironmentCommand : NamedCommand
{
    public override void Execute()
    {
        Console.WriteLine($"OS: {RuntimeInformation.OSDescription}");
        Console.WriteLine($"Framework: {RuntimeInformation.FrameworkDescription}");
        Console.WriteLine($"Architecture: {RuntimeInformation.OSArchitecture}");
        Console.WriteLine($"Machine: {System.Environment.MachineName}");
        Console.WriteLine($"Working directory: {System.Environment.CurrentDirectory}");
    }
}
