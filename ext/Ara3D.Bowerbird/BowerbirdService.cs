using System.Diagnostics;
using Ara3D.Events;
using Ara3D.Logging;
using Ara3D.Utils;
using Ara3D.Utils.Roslyn;

namespace Ara3D.Bowerbird;

/// <summary>
/// Manifest-driven command host: catalog scan and compile-on-run execution.
/// </summary>
public class BowerbirdService
    : IEventErrorHandler
{
    public BowerbirdOptions Options { get; }
    public CommandRunner Runner { get; }
    public CommandCatalog Catalog { get; private set; }
    public CommandRunResult LastResult { get; private set; }
    public IEventBus EventBus { get; }
    public ILogger Logger { get; set; }

    public event EventHandler CatalogChanged;

    public BowerbirdService(BowerbirdOptions options, ILogger logger)
    {
        Options = options;
        Logger = logger ?? Logging.Logger.Debug;
        EventBus = new EventBus(this);
        Runner = new CommandRunner(options, Logger);
        RefreshCatalog();
    }

    public void RefreshCatalog()
    {
        Catalog = new CommandCatalog(Options.CommandsRoot, Logger);
        CatalogChanged?.Invoke(this, EventArgs.Empty);
    }

    public CommandRunResult CompileCommand(CommandDescriptor descriptor, CancellationToken token = default)
        => LastResult = Runner.Compile(descriptor, token);

    public CommandRunResult RunCommand(
        CommandDescriptor descriptor,
        object parameter = null,
        ICommandExecutor executor = null,
        CancellationToken token = default)
        => LastResult = Runner.Run(descriptor, parameter, executor, token);

    public CommandRunResult RunCommandByName(
        string displayNameOrFolder,
        object parameter = null,
        ICommandExecutor executor = null,
        CancellationToken token = default)
    {
        var descriptor = Catalog.ResolveByName(displayNameOrFolder);
        if (descriptor == null)
            throw new InvalidOperationException($"Command not found: {displayNameOrFolder}");
        return RunCommand(descriptor, parameter, executor, token);
    }

    public void OnError(ISubscriber sub, IEvent ev, Exception ex)
    {
        Debugger.Break();
        Debug.WriteLine($"Error occurred in {sub} with event {ev}: {ex}");
    }
}
