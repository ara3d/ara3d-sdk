using Ara3D.Geometry;
using Ara3D.Logging;
using Ara3D.Models;
using Ara3D.Utils;

namespace Ara3D.Studio.API;

public interface IExporter
{
    public void Export(IReadOnlyList<Model3D> models, string filePath);
    public string FileType { get; }
    public string FileExtension { get; }
}

// Implementing this interface assures that your script is called on a regular phases
public interface IAnimated
{ }

/// <summary>
/// A scripted component, is one that is loaded from a plug-in DLL or a C# source file 
/// </summary>
public interface IScriptedComponent
{ }

/// <summary>
/// An asset is a piece of data that was loaded from disk, or created manually. 
/// It has a core data element that can flow through the graph (which can be modified and rendered)
/// and a list of attachments which can be understood by modifiers in the graph.
/// An example of attachment is BIM Data. 
/// </summary>
public interface IAsset : IDisposable
{
    object Value { get; }
    IReadOnlyList<object> Attachments { get; }
}

/// <summary>
/// This is an object that can appear in a graph and represents a loadable asset. 
/// </summary>
public interface IAssetSource : IDisposable
{
    IAsset Eval(EvalContext context);
    Task<IAsset> InitialLoad(ILogger logger);
}

/// <summary>
/// This is a 
/// </summary>
public interface ILoader : IScriptedComponent
{
    Task<IAsset> Load(FilePath filePath, ILogger logger);
}

/// <summary>
/// A script that generates objects.
/// </summary>
public interface IGenerator : IScriptedComponent
{
}

/// <summary>
/// A script that converts objects into other objects.
/// It will have an Eval function that takes at least one argument,
/// and optionally an EvalContext 
/// </summary>
public interface IModifier : IScriptedComponent
{
}

/// <summary>
/// An executable command 
/// </summary>
public interface IScriptedCommand : IScriptedComponent
{
    string Name { get; }
    void Execute(IHostApplication app);
    bool CanExecute(IHostApplication app);
}

/// <summary>
/// An executable command, designed for specific models.    
/// </summary>
public interface IModelCommand : IScriptedCommand
{
    string Name { get; }
    void Execute(FlowObject fo);
    bool CanExecute(FlowObject fo);
}

/// <summary>
/// A modeless tool, usually with a UI.
/// </summary>
public interface ITool : IScriptedComponent
{
    bool CanExecute(EvalContext context);
    void Execute(EvalContext context);
}