# Ara3D.SceneEval

These are the data structures used to describe the evaluation dependencies for a 3D scene
which forms a directed acyclic graph (also known as a DAG). 

The graph is responsible for generating at runtime parts of the renderable scene. 
It generates collections of meshes, materials, and transforms. Collectively known as a Model.

Each node in the graph is evaluated on each frame if either:
1. One of its properties changes
2. One of its inputs changes 

If nothing changes a graph node will serve the previously computed model.

Graph nodes with no inputs (dependencies) are also known as sources.
They could be loaded assets, or they could be procedurally created geometry. 

Graph nodes with one or more inputs are called operators. They might transform, 
deform, or filter the models that flow through the graph. 

## SceneEvalGraph

A collection of root `SceneEvalNode` instances that form the root of the graph.
Roots can be changed, which will trigger a notification.
The graphs must not have cycles. This is checked everytime the graph is modified.

## SceneEvalNode

A wrapper around an object that has an `Eval` method, and a set of input 
nodes to that object (which are supplied to the `Eval`). Any public fields
or properties exposed on that object are exposed via an `IPropContainer` interface. 


## SceneEvalContext

An object supplied to nodes during evaluation which contains additional context 
information for the current evaluation, and an opportunity to cancel work asynchronously via
a `CancellationToken`.