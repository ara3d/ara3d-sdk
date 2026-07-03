using Ara3D.Bowerbird;
using Ara3D.Logging;
using Ara3D.Utils;

namespace Ara3D.Bowerbird.Tests;

public class CommandRunnerTests
{
    [Test]
    public static void HelloWorld_RunTwiceInSameProcess()
    {
        var service = TestBowerbirdHost.Create();
        var descriptor = TestBowerbirdHost.Resolve(service, "HelloWorld");
        var cachePath = descriptor.OutputFolder.RelativeFile(CommandCompileCache.CacheFileName);
        if (cachePath.Exists())
            cachePath.Delete();

        var first = service.RunCommand(descriptor);
        Assert.That(first.Success, Is.True, TestBowerbirdHost.FormatDiagnostics(first));
        Assert.That(first.FromCache, Is.False);

        var second = service.RunCommand(descriptor);
        Assert.That(second.Success, Is.True, TestBowerbirdHost.FormatDiagnostics(second));
        Assert.That(second.FromCache, Is.True);
        Assert.That(second.OutputDll, Is.EqualTo(first.OutputDll));
    }

    [Test]
    public static void Counter_RunTwiceInSameProcess()
    {
        var root = TestCommandsRoot.Create(r =>
            TestCommandsRoot.WriteCommand(r, "Counter", "Counter", "Isolated.CounterCommand", """
                using System;
                using Ara3D.Bowerbird;
                namespace Isolated;
                public class CounterCommand : NamedCommand
                {
                    public static int Count;
                    public override void Execute()
                        => Console.WriteLine($"You have executed this command {++Count} time(s)");
                }
                """));

        var options = new BowerbirdOptions("Bowerbird.Tests", root);
        var service = new BowerbirdService(options, Logger.Console);
        var descriptor = TestBowerbirdHost.Resolve(service, "Counter");

        var first = CaptureRunOutput(service, descriptor);
        var second = CaptureRunOutput(service, descriptor);

        Assert.That(ParseCounterOutput(first), Is.EqualTo(1));
        Assert.That(ParseCounterOutput(second), Is.EqualTo(2));
    }

    static string CaptureRunOutput(BowerbirdService service, CommandDescriptor descriptor)
        => FunctionUtils.CaptureConsoleOut(() =>
        {
            var result = service.RunCommand(descriptor);
            Assert.That(result.Success, Is.True, TestBowerbirdHost.FormatDiagnostics(result));
        });

    static int ParseCounterOutput(string output)
    {
        var match = System.Text.RegularExpressions.Regex.Match(output, @"(\d+) time\(s\)");
        Assert.That(match.Success, Is.True, output);
        return int.Parse(match.Groups[1].Value);
    }
}
