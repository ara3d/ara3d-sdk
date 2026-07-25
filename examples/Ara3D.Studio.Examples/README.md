# Ara 3D Studio examples

Sample **generators** (make geometry) and **modifiers** (transform geometry) for Ara 3D Studio,
plus BIM tools, commands, and demos. Each `.cs` file is a self-contained tool: implement
`IGenerator` / `IModifier`, expose an `Eval` method, and Studio surfaces it in the UI.

**New here? Start with the authoring skill:** [`../../skills/ara3d-authoring/SKILL.md`](../../skills/ara3d-authoring/SKILL.md)
— the full contract (interfaces, `Eval`, parameters/attributes, `EvalContext`, valid input/output
types, deployment) with two annotated examples.

Good first files to read:
- `Generators/MeshGenerators.cs` (`BoxFrame`) — a minimal generator with open faces and connect-legs.
- `Modifiers/PlaneCut.cs` — a minimal modifier (mesh-level and model-level).
- `Modifiers/WeldVertices.cs` — computing read-out values back to the panel.
- `Modifiers/RefineAndCoarsen.cs` — analyzer / heat-map pattern (`ColoredTriangleMesh3D`).
