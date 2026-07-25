using System.ComponentModel;

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
    [Description("Requests the host tear down and stop, reporting the given exit code.")]
    void Shutdown(
        [Description("Process/runner exit code to report; 0 = success, non-zero = failure. Defaults to 0.")] int exitCode = 0);
}
