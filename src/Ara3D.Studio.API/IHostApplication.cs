namespace Ara3D.Studio.API;

/// <summary>
/// The full driver surface of a Studio host: everything a tool, workflow, or automation script
/// may command. Aggregates the always-present capability groups; genuinely optional capabilities
/// are nullable properties (null = the host does not have them — check, don't catch).
/// Generators and modifiers see only the narrow <see cref="IEvalServices"/> slice.
/// </summary>
public interface IHostApplication : IEvalServices, IAssetHost, IExportHost
{
    /// <summary>Null on hosts without a viewport (e.g. CPU-only headless runners).</summary>
    IViewportHost? Viewport { get; }
}
