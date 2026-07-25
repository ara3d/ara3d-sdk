using System.ComponentModel;
using Ara3D.Models;

namespace Ara3D.Studio.API;

/// <summary>
/// Drives a Studio scene programmatically: what a tool, workflow, or automation script uses to
/// build a pipeline, evaluate it, and read the result back (studio-061). Node identity is the
/// evaluator instance itself (a generator/modifier/asset-source object) — the API layer sits
/// below the eval graph and so does not expose graph node types.
/// </summary>
public interface ISceneHost
{
    /// <summary>Adds a generator, modifier, or asset source as a node in the scene.</summary>
    [Description("Adds a generator, modifier, or asset source as a node in the scene.")]
    void AddToScene(
        [Description("An IGenerator, IModifier, or IAssetSource instance. A modifier attaches after the current selection; a generator or asset source starts a new pipeline.")] object evaluator);

    /// <summary>Removes every pipeline and node.</summary>
    [Description("Removes every pipeline and node from the scene.")]
    void ClearScene();

    /// <summary>
    /// Sets a public property on an already-added evaluator and marks the scene for
    /// re-evaluation (e.g. to simulate a slider change in a regression test).
    /// </summary>
    [Description("Sets a public property on an already-added evaluator and marks the scene for re-evaluation.")]
    void SetParameter(
        [Description("A generator/modifier/asset-source instance already added via AddToScene; must resolve to a node in the current graph.")] object evaluator,
        [Description("Public, writable property name on the evaluator's type (case-sensitive).")] string propertyName,
        [Description("New value, assignment-compatible with the property's declared type.")] object value);

    /// <summary>Forces a full evaluation and returns the resulting model (null if empty).</summary>
    [Description("Forces a full evaluation and returns the resulting model (null if empty).")]
    IModel3D? Evaluate();

    /// <summary>The most recently evaluated model, or null if nothing has evaluated yet.</summary>
    [Description("The most recently evaluated model, or null if nothing has evaluated yet.")]
    IModel3D? CurrentModel { get; }

    /// <summary>Counts over <see cref="CurrentModel"/> for asserts.</summary>
    [Description("Element/vertex/face counts over the current model.")]
    SceneStats GetStats();

    // --- Id-keyed scene-node operations (studio-175 / studio-169) ---
    // These address nodes by their durable string id (EvalNode.PersistentId) and take only
    // JSON-coercible arguments, so MCP's call_host reaches them without a hand-written tool per
    // op. Default implementations let hosts that do not manage an addressable graph opt out; the
    // live SceneService (and the headless host) override them.

    /// <summary>Every node across all pipelines, as stable id-bearing read views.</summary>
    [Description("Lists every scene node as an id-bearing read view (id, name, kind, enabled).")]
    IReadOnlyList<ISceneNode> ListNodes()
        => [];

    /// <summary>The node with the given durable id, or null if none matches.</summary>
    [Description("Returns the scene node with the given durable id, or nothing if none matches.")]
    ISceneNode? GetNode(
        [Description("Durable node id (EvalNode.PersistentId), as returned by AddNode or listed by ListNodes.")] string id)
        => null;

    /// <summary>
    /// Sets a public property on the node with the given id from a string value converted to the
    /// property type, then marks the scene for re-evaluation. String-typed (not object) so it stays
    /// call_host-coercible, mirroring set_selected_node_property.
    /// </summary>
    [Description("Sets a property (from a string value) on the node with the given durable id.")]
    void SetParameter(
        [Description("Durable node id (EvalNode.PersistentId), as returned by AddNode or listed by ListNodes.")] string nodeId,
        [Description("Public, writable property name on the node's evaluator type (case-sensitive).")] string propertyName,
        [Description("New value as a string, parsed/converted to the property's declared type.")] string value)
        => throw new NotSupportedException("This host does not support id-keyed scene editing.");

    /// <summary>
    /// Instantiates a generator/modifier/source script by name and adds it — as a new pipeline
    /// source, or after <paramref name="afterNodeId"/> when given. Returns the new node's id.
    /// </summary>
    [Description("Adds a script by name to the scene (after a node id when given); returns the new node id.")]
    string AddNode(
        [Description("Display or type name of a loaded generator/modifier/source script, resolved via the host's script resolver.")] string scriptName,
        [Description("Durable id of the node to attach after (as a modifier); omit or leave null to start a new pipeline source.")] string? afterNodeId = null)
        => throw new NotSupportedException("This host does not support id-keyed scene editing.");

    /// <summary>Removes the node with the given id.</summary>
    [Description("Removes the scene node with the given durable id.")]
    void RemoveNode(
        [Description("Durable node id (EvalNode.PersistentId), as returned by AddNode or listed by ListNodes.")] string id)
        => throw new NotSupportedException("This host does not support id-keyed scene editing.");

    /// <summary>
    /// Enables or disables the node with the given id. A disabled node is skipped during
    /// evaluation (its pipeline stops contributing), which is the non-destructive way to isolate
    /// or hide part of a scene without deleting it.
    /// </summary>
    [Description("Enables or disables the scene node with the given durable id.")]
    void SetNodeEnabled(
        [Description("Durable node id (EvalNode.PersistentId), as returned by AddNode or listed by ListNodes.")] string id,
        [Description("True to include the node in evaluation; false to skip it non-destructively (it stays in the scene).")] bool enabled)
        => throw new NotSupportedException("This host does not support id-keyed scene editing.");

    /// <summary>Moves the node with the given id to a new index within its pipeline.</summary>
    [Description("Moves the node with the given durable id to a new index within its pipeline.")]
    void MoveNode(
        [Description("Durable node id (EvalNode.PersistentId), as returned by AddNode or listed by ListNodes.")] string id,
        [Description("Zero-based target index within the node's pipeline.")] int index)
        => throw new NotSupportedException("This host does not support id-keyed scene editing.");
}
