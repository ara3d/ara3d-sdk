namespace Ara3D.Studio.API;

/// <summary>
/// Host lifecycle control. Optional: <see cref="IHostApplication.Lifecycle"/> is null on hosts
/// that own their own lifecycle and cannot be told to stop through the API (an in-process
/// generator/modifier sandbox, or a host with no meaningful teardown). A CLI runner or automated
/// test calls <see cref="Shutdown"/> to end a driven host deterministically; the exit code feeds
/// the process exit code and the run report (see <see cref="ToolResult"/>).
/// </summary>
public interface ILifecycleHost
{
    /// <summary>
    /// Request the host tear down and stop, reporting <paramref name="exitCode"/> (0 = success)
    /// to the process or runner. Safe to call from any thread. Idempotent — a second call after
    /// shutdown has begun is a no-op.
    /// </summary>
    void Shutdown(int exitCode = 0);
}
