using Ara3D.Models;

namespace Ara3D.Studio.API;

// Host-driven scripted-component contracts. Unlike the contracts in Ara3D.Flow
// (IGenerator/IModifier/IAsset/...), these reach into a running host — an exporter writes
// files, a tool drives IHostApplication, a command acts on host-managed flow objects — so
// they stay in Ara3D.Studio.API alongside the host interfaces.

public interface IExporter
{
    public void Export(IReadOnlyList<Model3D> models, string filePath);
    public string FileType { get; }
    public string FileExtension { get; }
}

/// <summary>
/// A script that drives the host: a menu command, an interactive tool, a CLI workflow,
/// or a test. Runs to completion (a modeless tool may return once its UI is open);
/// the result feeds exit codes and reports in automation hosts.
/// </summary>
public interface ITool : IScriptedComponent
{
    string Name { get; }
    bool CanRun(IHostApplication app);
    Task<ToolResult> RunAsync(IHostApplication app, CancellationToken ct);
}

/// <summary>
/// An executable command, designed for specific models.
/// </summary>
public interface IModelCommand : IScriptedComponent
{
    string Name { get; }
    void Execute(FlowObject fo);
    bool CanExecute(FlowObject fo);
}
