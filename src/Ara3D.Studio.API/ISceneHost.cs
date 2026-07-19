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
    void AddToScene(object evaluator);

    /// <summary>Removes every pipeline and node.</summary>
    void ClearScene();

    /// <summary>
    /// Sets a public property on an already-added evaluator and marks the scene for
    /// re-evaluation (e.g. to simulate a slider change in a regression test).
    /// </summary>
    void SetParameter(object evaluator, string propertyName, object value);

    /// <summary>Forces a full evaluation and returns the resulting model (null if empty).</summary>
    IModel3D? Evaluate();

    /// <summary>The most recently evaluated model, or null if nothing has evaluated yet.</summary>
    IModel3D? CurrentModel { get; }

    /// <summary>Counts over <see cref="CurrentModel"/> for asserts.</summary>
    SceneStats GetStats();
}
