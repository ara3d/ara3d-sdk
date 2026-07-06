namespace Ara3D.MCP;

/// <summary>Runs actions on a <see cref="SynchronizationContext"/> when needed.</summary>
public sealed class SynchronizationContextInvoker : IUiThreadInvoker
{
    private readonly SynchronizationContext _context;

    public SynchronizationContextInvoker(SynchronizationContext context)
        => _context = context ?? throw new ArgumentNullException(nameof(context));

    public T Invoke<T>(Func<T> action)
    {
        ArgumentNullException.ThrowIfNull(action);

        if (SynchronizationContext.Current == _context)
            return action();

        T result = default!;
        Exception? error = null;

        _context.Send(_ =>
        {
            try
            {
                result = action();
            }
            catch (Exception ex)
            {
                error = ex;
            }
        }, null);

        if (error != null)
            throw error;

        return result;
    }

    public void Invoke(Action action)
        => Invoke(() =>
        {
            action();
            return 0;
        });
}
