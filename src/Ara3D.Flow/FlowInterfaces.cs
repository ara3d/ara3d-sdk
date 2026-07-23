using Ara3D.Logging;
using Ara3D.Utils;

namespace Ara3D.Studio.API;

// Host-free scripted-component contracts. These describe things that flow through or produce
// data in a graph and carry no dependency on a running Studio host, so they live in Ara3D.Flow.
// Host-driven contracts (IExporter, ITool, IModelCommand) stay in Ara3D.Studio.API/Interfaces.cs.

/// <summary>
/// Implementing this interface assures that your script is called on a regular phases
/// </summary>
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
/// A script that loads an asset from a file.
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
