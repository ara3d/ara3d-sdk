using Ara3D.Logging;
using Ara3D.Utils;
using SysConsole = System.Console;

namespace Ara3D.Bowerbird.Console;

public static class Program
{
    public static int Main(string[] args)
    {
        var commandsRoot = ParseCommandsRoot(args);
        var logger = Logger.Debug.Create("Bowerbird.Console", msg => SysConsole.WriteLine(msg));
        var options = new BowerbirdOptions("Bowerbird Console", commandsRoot);
        var service = new BowerbirdService(options, logger);

        SysConsole.WriteLine($"Commands root: {service.Options.CommandsRoot}");
        SysConsole.WriteLine();

        if (service.Catalog.Commands.Count == 0)
        {
            SysConsole.WriteLine("No commands found.");
            return 1;
        }

        while (true)
        {
            PrintMenu(service);
            SysConsole.Write("Select command (number or q): ");
            var line = SysConsole.ReadLine()?.Trim();
            if (line.IsNullOrWhiteSpace() || line.Equals("q", StringComparison.OrdinalIgnoreCase))
                return 0;

            if (!int.TryParse(line, out var index) || index < 1 || index > service.Catalog.Commands.Count)
            {
                SysConsole.WriteLine("Invalid selection.");
                continue;
            }

            var descriptor = service.Catalog.Commands[index - 1];
            RunCommand(service, descriptor, logger);
            SysConsole.WriteLine();
        }
    }

    static DirectoryPath ParseCommandsRoot(string[] args)
    {
        for (var i = 0; i < args.Length - 1; i++)
        {
            if (args[i] == "--commands")
                return new DirectoryPath(args[i + 1]);
        }

        return CommandsRootResolver.Resolve();
    }

    static void PrintMenu(BowerbirdService service)
    {
        SysConsole.WriteLine("Available commands:");
        for (var i = 0; i < service.Catalog.Commands.Count; i++)
        {
            var c = service.Catalog.Commands[i];
            var desc = c.Manifest.Description;
            SysConsole.WriteLine($"  {i + 1}. {c.DisplayName}" + (desc.IsNullOrWhiteSpace() ? "" : $" — {desc}"));
        }
    }

    static void RunCommand(BowerbirdService service, CommandDescriptor descriptor, ILogger logger)
    {
        SysConsole.WriteLine($"Running: {descriptor.DisplayName}");
        var executor = new DefaultCommandExecutor { Logger = logger };
        var result = service.RunCommand(descriptor, null, executor);

        if (!result.CompileSuccess)
        {
            SysConsole.WriteLine("Compilation failed:");
            foreach (var d in result.Diagnostics)
                SysConsole.WriteLine($"  {d}");
            SysConsole.WriteLine($"See log: {descriptor.CompileLogPath}");
            return;
        }

        if (!result.Success || result.Exception != null)
        {
            SysConsole.WriteLine($"Load/execute failed: {result.Exception?.Message}");
            return;
        }

        SysConsole.WriteLine("Done.");
    }
}
