namespace Ara3D.MCP;

/// <summary>Marshals work onto a host UI or main thread.</summary>
public interface IUiThreadInvoker
{
    T Invoke<T>(Func<T> action);

    void Invoke(Action action);
}
