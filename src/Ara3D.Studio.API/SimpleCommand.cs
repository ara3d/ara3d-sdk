namespace Ara3D.Studio.API;

public abstract class SimpleCommand : ITool
{
    public abstract string Name { get; }
    public abstract void Execute();

    public bool CanRun(IHostApplication app)
        => true;

    public Task<ToolResult> RunAsync(IHostApplication app, CancellationToken ct)
    {
        Execute();
        return Task.FromResult(ToolResult.Ok());
    }
}
