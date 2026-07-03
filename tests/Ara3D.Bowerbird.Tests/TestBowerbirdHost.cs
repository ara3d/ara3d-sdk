using Ara3D.Bowerbird;
using Ara3D.Logging;
using Ara3D.Utils;

namespace Ara3D.Bowerbird.Tests;

public static class TestBowerbirdHost
{
    public static BowerbirdService Create()
    {
        var options = new BowerbirdOptions("Bowerbird.Tests", TestPaths.TestSamplesCommands);
        return new BowerbirdService(options, Logger.Console);
    }

    public static CommandDescriptor Resolve(BowerbirdService service, string folderName)
        => service.Catalog.ResolveByName(folderName)
            ?? throw new InvalidOperationException($"Sample not found: {folderName}");

    public static CommandRunResult Compile(BowerbirdService service, string folderName)
        => service.CompileCommand(Resolve(service, folderName));

    public static string FormatDiagnostics(CommandRunResult result)
        => string.Join(Environment.NewLine, result.Diagnostics);
}

public static class TestSampleCommandCache
{
    static readonly Lazy<Dictionary<string, INamedCommand>> Commands = new(LoadAll);

    public static BowerbirdService Service { get; } = TestBowerbirdHost.Create();

    public static INamedCommand Get(string folderName)
        => Commands.Value[folderName];

    public static string CaptureOutput(string folderName, object? parameter = null)
        => FunctionUtils.CaptureConsoleOut(() => Get(folderName).Execute(parameter));

    static Dictionary<string, INamedCommand> LoadAll()
    {
        var commands = new Dictionary<string, INamedCommand>();
        foreach (var folderName in TestSampleCommandsTests.SampleFolderNames)
        {
            var result = TestBowerbirdHost.Compile(Service, folderName);
            if (!result.CompileSuccess)
                throw new InvalidOperationException($"{folderName}: {TestBowerbirdHost.FormatDiagnostics(result)}");

            commands[folderName] = result.Command
                ?? throw new InvalidOperationException($"Command not loaded: {folderName}");
        }

        return commands;
    }
}
