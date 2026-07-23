using Ara3D.Logging;

namespace Ara3D.Studio.API;

/// <summary>
/// The host surface a generator or modifier may touch during Eval: logging, the derived-data
/// cache, and change notification after asynchronous recomputes. This is deliberately narrow —
/// tools and workflows drive the host and receive the full <see cref="IHostApplication"/> instead.
/// </summary>
public interface IEvalServices
{
    ILogger Logger { get; }
    IDerivedDataCache DerivedDataCache { get; }
    SynchronizationContext SynchronizationContext { get; }
    void Invalidate(object obj);
    void RefreshUI(object obj);

    /// <summary>
    /// Pointer state for in-canvas widgets; null on hosts without a viewport.
    /// Default null so headless implementations need no change.
    /// </summary>
    IViewportInput? ViewportInput => null;
}
