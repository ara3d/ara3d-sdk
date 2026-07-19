namespace Ara3D.Studio.API;

/// <summary>
/// A thread-safe <see cref="ILifecycleHost"/> that records the first shutdown request rather than
/// terminating anything. A CLI runner or automated test drives a host, then reads
/// <see cref="ShutdownRequested"/> / <see cref="ExitCode"/> to end its own loop and set the process
/// exit code — no <c>Environment.Exit</c>, so a hosting test process survives. First call wins;
/// later calls are no-ops, per the <see cref="ILifecycleHost"/> contract.
/// </summary>
public sealed class RecordingLifecycleHost : ILifecycleHost
{
    private readonly object _gate = new();
    private bool _requested;

    /// <summary>True once <see cref="Shutdown"/> has been called at least once.</summary>
    public bool ShutdownRequested
    {
        get { lock (_gate) return _requested; }
    }

    /// <summary>Exit code from the first <see cref="Shutdown"/> call; 0 until then.</summary>
    public int ExitCode { get; private set; }

    public void Shutdown(int exitCode = 0)
    {
        lock (_gate)
        {
            if (_requested)
                return;
            _requested = true;
            ExitCode = exitCode;
        }
    }
}
