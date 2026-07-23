using Ara3D.PropKit;

namespace Ara3D.Studio.API;

/// <summary>
/// The role a node plays in its pipeline: a generator or asset source that begins a pipeline,
/// or a modifier that transforms the flow. An enum (not a string) so it stays a closed set;
/// it serializes by name across the MCP boundary (studio-175).
/// </summary>
public enum SceneNodeKind
{
    Generator,
    Modifier,
    Source
}

/// <summary>
/// A stable, id-bearing read view of one scene-graph node (studio-175), addressable across MCP,
/// tools, and automation without exposing the internal eval-graph types. Backed by
/// <c>EvalNode.PersistentId</c> (studio-051) — the same durable handle node-reference edges key on.
/// </summary>
public interface ISceneNode
{
    /// <summary>Durable node id (a GUID string), stable across save/load and hot-reload.</summary>
    string Id { get; }

    string Name { get; }

    SceneNodeKind Kind { get; }

    bool IsEnabled { get; }

    /// <summary>The node's evaluator parameters, reusing PropKit rather than a new param model.</summary>
    IPropContainer Parameters { get; }
}

/// <summary>
/// An ordered chain of <see cref="ISceneNode"/>s — a generator/source followed by its modifiers —
/// the pipeline-level read view matching one internal <c>EvalPipeline</c>.
/// </summary>
public interface IScenePipeline
{
    IReadOnlyList<ISceneNode> Nodes { get; }
    bool IsEnabled { get; }
}

/// <summary>
/// Flat wire projection of an <see cref="ISceneNode"/> for MCP listings and <c>call_host</c>
/// (the enum serializes by name).
/// </summary>
public readonly record struct SceneNodeInfo(string Id, string Name, SceneNodeKind Kind, bool IsEnabled);

public static class SceneNodeExtensions
{
    public static SceneNodeInfo ToInfo(this ISceneNode node)
        => new(node.Id, node.Name, node.Kind, node.IsEnabled);
}
