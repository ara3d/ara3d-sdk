using Ara3D.Bowerbird;
using Ara3D.Utils;

namespace Ara3D.Bowerbird.Tests;

public class CommandRunnerTests
{
    [Test]
    public static void HelloWorld_RunTwiceInSameProcess()
    {
        var service = TestBowerbirdHost.Create();
        var descriptor = TestBowerbirdHost.Resolve(service, "HelloWorld");

        var first = service.RunCommand(descriptor);
        Assert.That(first.Success, Is.True, TestBowerbirdHost.FormatDiagnostics(first));

        var second = service.RunCommand(descriptor);
        Assert.That(second.Success, Is.True, TestBowerbirdHost.FormatDiagnostics(second));
        Assert.That(second.Compilation.OutputFilePath, Is.Not.EqualTo(first.Compilation.OutputFilePath));
    }

    [Test]
    public static void Counter_RunTwiceInSameProcess()
    {
        var service = TestBowerbirdHost.Create();
        var descriptor = TestBowerbirdHost.Resolve(service, "Counter");

        var first = CaptureRunOutput(service, descriptor);
        var second = CaptureRunOutput(service, descriptor);

        Assert.That(ParseCounterOutput(first), Is.EqualTo(1));
        Assert.That(ParseCounterOutput(second), Is.EqualTo(1));
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
