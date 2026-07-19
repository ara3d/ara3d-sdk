---
name: ara3d-authoring
description: Author Ara 3D Studio tools — generators (make geometry) and modifiers (transform geometry) — as standalone C# scripts. Use when the user says "write a modifier", "create a generator", "make an Ara 3D tool", "new Studio script", asks how modifiers/generators/Eval/parameters/EvalContext work, or wants to add a mesh operation, deformer, analyzer, or heat-map to Ara 3D Studio.
---

# Authoring Ara 3D Studio tools (generators & modifiers)

Ara 3D Studio users extend the app with **standalone C# scripts**. Drop a `.cs` file into the
Scripts folder; Studio compiles it on save and surfaces every tool it finds in the UI with an
auto-generated parameter panel. No project, no build step, no restart.

Two roles cover almost everything:

- **Generator** — makes new geometry from parameters. Implements `IGenerator`, has an `Eval()`
  that takes no geometry input.
- **Modifier** — transforms geometry flowing through the graph. Implements `IModifier`, has an
  `Eval(input)` that takes an upstream geometry object.

Everything below is the *contract the host actually enforces* (by reflection), followed by the
attributes that shape the UI, the valid input/output types, deployment, and two worked examples.

> Paths below are relative to the **`ara3d-sdk` repo root** (where this skill lives). A few cited
> files live in the **Ara 3D Studio host app repo**, not the SDK — those are marked *(host repo)*.

---

## 1. The runtime contract (what the host requires)

The host wraps your object in `EvaluatorWrapper` and calls it by reflection. The rules:

1. **Marker interface.** Implement `IGenerator` or `IModifier` (namespace `Ara3D.Studio.API`).
   Both are empty marker interfaces — they only tag the class for discovery.
2. **A public method named exactly `Eval`.** The wrapper does `type.GetMethod("Eval")`. The name,
   casing, and public visibility all matter. Exactly one `Eval` per class.
3. **`Eval` arguments define the input:**
   - **Generator:** `Eval()` — zero geometry args. May take a single `EvalContext`.
   - **Modifier:** `Eval(TInput)` — the first parameter is the geometry it consumes. The host
     converts the upstream graph object to `TInput` (see §4).
   - **Optional `EvalContext`** may be added as an extra parameter (generator: the only arg;
     modifier: the second arg). Use it for `RefreshUI`, animation time, and host access.
4. **Return type = output.** Whatever `Eval` returns becomes the object flowing downstream and
   gets rendered. Must be a renderable/convertible geometry type (see §4).
5. **Parameters = public fields or properties.** Every public read/write field or property becomes
   an editable control in the panel. Read-only properties (`{ get; private set; }`) become
   non-editable read-outs.

Minimal shapes:

```csharp
public class MyGenerator : IGenerator
{
    [Range(1, 10)] public int Count = 3;
    public TriangleMesh3D Eval() => /* build geometry from Count */;
}

public class MyModifier : IModifier
{
    [Range(0f, 1f)] public float Amount = 0.5f;
    public TriangleMesh3D Eval(TriangleMesh3D mesh) => /* transform mesh using Amount */;
}
```

---

## 2. Parameters and UI attributes

Parameters are plain public fields/properties. Fields and properties both work — pick the house
style of the surrounding file. Attributes refine how they render:

| Attribute | Applies to | Effect | Source namespace |
|---|---|---|---|
| `[Range(min, max)]` | numeric field/prop | slider with bounds | `System.ComponentModel.DataAnnotations` |
| `[Options(nameof(Factory))]` | `int` field/prop | dropdown; `Factory` is a method/prop returning `IEnumerable<string>`, selected value is the index | `Ara3D.PropKit` |
| `[Category(nameof(Categories.X))]` | the class | groups the tool under a named category in the UI | `System.ComponentModel` (+ your `Categories` enum) |
| `[OnDemand]` | the class | do **not** re-evaluate on every parameter change; run only when explicitly requested | `Ara3D.Studio.API` |
| `[Animated]` | the class | tick every frame; read `ctx.AnimationTime` (or implement `IAnimated`) | `Ara3D.Studio.API` |
| `[PointerTracking]` | the class | re-evaluate while the pointer or camera moves; read `ctx.Services.ViewportInput` (null on headless hosts) | `Ara3D.Studio.API` |

Notes:
- Default (no `[OnDemand]`) = **live**: changing any parameter re-runs `Eval` immediately.
- `Categories` is an ordinary `enum` you define/extend per script set. The examples project's is
  `examples/Ara3D.Studio.Examples/Categories.cs` (Meshes, Curves, Sdf, Topology, …).
- Supported parameter types include the numeric primitives, `bool`, `string`, and SDK vector types
  like `Vector3` / `Vector2` (see `Transform`, `Box` in the examples). A `bool` renders as a
  checkbox; a `Vector3` renders as three fields.

### Read-out values (computing stats back to the panel)

Expose computed results as read-only properties and push them to the panel from inside `Eval`:

```csharp
public int VertexCountBefore { get; private set; }
public int VertexCountAfter  { get; private set; }

public TriangleMesh3D Eval(TriangleMesh3D mesh, EvalContext ctx)
{
    var r = mesh.WeldVertices();
    VertexCountBefore = mesh.Points.Count;
    VertexCountAfter  = r.Points.Count;
    ctx.Application.RefreshUI(this);   // repaint the panel with the new read-outs
    return r;
}
```

This is the whole `WeldVertices` example (`examples/Ara3D.Studio.Examples/Modifiers/WeldVertices.cs`).

---

## 3. Optional `EvalContext`

`EvalContext` (`src/Ara3D.Studio.API/EvalContext.cs`) carries:

- `Application` — the `IHostApplication` (logging, `RefreshUI(this)`, scene/camera access).
- `AnimationTime` — seconds, for `[Animated]` tools.
- `Input` — the raw upstream `FlowObject` (attachments, presentation) if you need more than the
  converted geometry — e.g. BIM metadata via `Input.GetAttachment<T>()`.
- `Services.ViewportInput` — pointer state for in-canvas widgets (`IViewportInput`: normalized
  `CursorUV`, world-space `CursorRay`, `HoverObjectId`, `PrimaryDown`, `ConsumePrimaryClick()`).
  Null on hosts without a viewport, so always keep a fallback. Pair with `[PointerTracking]` so
  the host re-evaluates the node as the pointer moves. A "click" is press+release without
  dragging — drags orbit the camera. Worked example: `examples/.../Demos/GridSnapPickDemo.cs`.

Add it only when you need it. Signatures the host accepts:

```csharp
QuadMesh3D    Eval();                                  // generator
QuadMesh3D    Eval(EvalContext ctx);                   // generator + context
TriangleMesh3D Eval(TriangleMesh3D mesh);              // modifier
TriangleMesh3D Eval(TriangleMesh3D mesh, EvalContext ctx); // modifier + context
```

---

## 4. Valid input and output types

**Input (modifier first parameter).** The host converts the upstream object to your parameter type
via `FlowObjectConverters` (`src/Ara3D.Studio.API/FlowObjectConverters.cs`). The two most flexible
choices:

- `IModel3D` — accepts a `TriangleMesh3D`, `ColoredTriangleMesh3D`, `QuadMesh3D`, `QuadGrid3D`,
  `Model3D`, or `RenderModelData` upstream. Use this for **model-level** work (instances,
  materials, whole assemblies).
- `TriangleMesh3D` — accepts `QuadMesh3D`, `QuadGrid3D`, `ColoredTriangleMesh3D`, or an `IModel3D`
  (flattened). Use this for **single-mesh** algorithms.

Any other exact type (e.g. `LineMesh3D`) is accepted **only** when the upstream object already is
that type — no conversion is defined, so the graph must actually produce it.

**Output (return type).** Return a geometry type the host can render or pass on:

- `TriangleMesh3D`, `QuadMesh3D`, `QuadGrid3D` — meshes/surfaces.
- `ColoredTriangleMesh3D` — per-vertex colored mesh; this is the **analyzer / heat-map** pattern
  (compute a scalar per vertex/face, map to color). See `Modifiers/ColorSharpEdges.cs`.
- `IModel3D` / `Model3D` — instanced models with materials; build with `Model3DBuilder`.
- `LineMesh3D` — line overlays (boundaries, arrows, diagrams).

Model-level ops iterate `model.Instances`, remap meshes through a `Model3DBuilder`, and prefilter
by `model.GetInstanceBounds()` — see the `ModelHorizontalSlice` half of the PlaneCut example.

---

## 5. Two worked examples (read these first)

### Generator — `BlockMesh`
`examples/Ara3D.Studio.Examples/Generators/BlockMesh.cs`

```csharp
public class BlockMesh : IGenerator
{
    [Range(0f, 10f)] public float SizeX = 1;
    [Range(0f, 10f)] public float SizeY = 1;
    [Range(0f, 10f)] public float SizeZ = 1;

    public bool EmptyTop;
    public bool EmptyBottom;
    public bool EmptySides;

    public QuadMesh3D Eval()
    {
        var shape = new CellGridBuilder3D(3, 3, 3).Remove(1, 1, 1);
        if (EmptyTop)    shape.Remove(1, 1, 2);
        if (EmptyBottom) shape.Remove(1, 1, 0);
        if (EmptySides)  shape.Remove(1,0,1).Remove(1,2,1).Remove(0,1,1).Remove(2,1,1);
        return shape.ToMesh().Scale((SizeX, SizeY, SizeZ));
    }
}
```

Shows: marker interface, `[Range]` sliders, `bool` checkboxes, `Eval()` with no input returning a
mesh. The whole tool is one method building geometry from parameters.

### Modifier — `MeshHorizontalSlice`
`examples/Ara3D.Studio.Examples/Modifiers/PlaneCut.cs`

```csharp
[Category(nameof(Categories.Meshes))]
public class MeshHorizontalSlice : IModifier
{
    [Range(0f, 1f)] public float Height { get; set; } = 0.5f;

    public TriangleMesh3D Eval(TriangleMesh3D mesh)
    {
        var bounds = mesh.Bounds;
        if (Height == 0) return ([], []);
        var z = bounds.Min.Z.Lerp(bounds.Max.Z, Height);
        var plane = new Plane(Vector3.UnitZ, -z);
        return mesh.ClipBelow(plane);
    }
}
```

Shows: `[Category]` grouping, a single `[Range]` parameter, `Eval(TriangleMesh3D)` returning a
mesh, and reuse of an existing library op (`ClipBelow`). The same file's `ModelHorizontalSlice` is
the `IModel3D` (instance-level) counterpart — read it to learn the `Model3DBuilder` pattern.

### More examples worth copying
- `Modifiers/WeldVertices.cs` — read-outs + `ctx.Application.RefreshUI(this)`.
- `Modifiers/RefineAndCoarsen.cs` — several modifiers + a helper class in one file; the
  `ColorByTriangleQuality` class is the heat-map/analyzer pattern (`ColoredTriangleMesh3D`).
- `Modifiers/FaceNormalArrows.cs` — building an `IModel3D` overlay with instanced arrow meshes.
- Studio's built-in coding prompt (*host repo*: `src/Ara3D.Studio/Ara3DStudio/codingprompt.txt`)
  contains a large gallery of small generators and deformers (Cylinder, Torus, TwistDeformer,
  NoiseDeformer, GridClone, …) — the canonical "cheat sheet" of idioms.

---

## 6. Reusable idioms (don't reinvent)

- `mesh.Bounds`, `new Topology(mesh)`, `new MeshAttributes(mesh, topo)` — topology/adjacency, normals.
- `mesh.WeldVertices()`, `mesh.Deform(p => ...)`, `mesh.WithPoints(...)`, `mesh.Scale(v)`,
  `mesh.Triangulate()` — common mesh transforms.
- `new Model3DBuilder()` + `AddInstance` / `AddMeshWithoutInstance` / `AddModel` — assemble models.
- `mesh.Clone(material, transforms)` — instance a mesh across many transforms.
- Return `ColoredTriangleMesh3D` for analyzers; map a per-element scalar to color yourself
  (see `ColorByTriangleQuality`).
- The SDK ships list-preserving LINQ (`Select`, `Map`, `Slice`, `Zip`, …) that returns
  `IReadOnlyList` with O(1) count/indexing — prefer it over `IEnumerable` where it exists.

---

## 7. Deploying a script

1. **Scripts folder.** Default `Documents/Ara 3D/Scripts` (override with the `SCRIPTS_PATH`
   environment variable). In Studio: menu **Compiler → Open Scripts Folder**.
2. **Drop the `.cs` file** in that folder (subfolders are watched too). A `DirectoryWatcher`
   recompiles automatically on every save; the tool appears/updates live.
3. **No `using` directives needed.** A `GlobalUsings.cs` in the Scripts folder pre-imports the
   common namespaces (`Ara3D.Geometry`, `Ara3D.Models`, `Ara3D.Studio.API`, `Ara3D.PropKit`,
   `System.Linq`, `System.ComponentModel.DataAnnotations`, …). Canonical list:
   `examples/Ara3D.Studio.Examples/GlobalUsings.cs`. Write scripts with **no namespace and no
   usings**, just the class(es) — matching `codingprompt.txt`.
4. **Extra dependencies.** Put additional referenced DLLs in the Scripts folder's `Libraries`
   subfolder (or list paths in `refs.txt`); they are loaded at compile time.
5. **Compile errors** surface in Studio's compilation panel; the offending tool just won't appear.

---

## 8. Other component roles (when a modifier/generator isn't the right shape)

From `src/Ara3D.Studio.API/Interfaces.cs`:

- `IScriptedCommand` / `IModelCommand` — one-shot **actions** (`Execute`/`CanExecute`), the right
  home for **reports and audits** that don't return geometry (e.g. a mesh-statistics dump).
- `ITool` — modeless UI tool.
- `IExporter` / `ILoader` — custom file write/read.
- `IAsset` / `IAssetSource` — data that enters the graph (with optional attachments like BIM data).

There is **no** `IAnalyzer` interface: an analyzer is just a modifier returning
`ColoredTriangleMesh3D`.

---

## 9. Authoring checklist

- [ ] Class implements `IGenerator` or `IModifier`.
- [ ] Exactly one public `Eval`; generator takes no geometry arg, modifier takes the input type.
- [ ] Input type is `IModel3D` (model-level) or `TriangleMesh3D` (mesh-level) unless a specific
      upstream type is guaranteed.
- [ ] Return type is renderable (`TriangleMesh3D`/`QuadMesh3D`/`QuadGrid3D`/`ColoredTriangleMesh3D`/`IModel3D`/`LineMesh3D`).
- [ ] Parameters are public fields/props with `[Range]`/`[Options]`; class has `[Category]`.
- [ ] Read-outs are `{ get; private set; }` + `ctx.Application.RefreshUI(this)`.
- [ ] Follow the `csharp-style` skill for the geometry code (functional-first, immutable, expression-bodied).
- [ ] No namespace/usings; drop in the Scripts folder; confirm it compiles and appears.

---

## Maintenance (keep this skill true to the code)

This skill documents a reflection-based contract. If any of these change, update the matching
section here. Files marked *(host repo)* live in the Ara 3D Studio application repository, not this
SDK.

| Source of truth | Governs |
|---|---|
| `src/Ara3D.Studio.API/Interfaces.cs` | the marker interfaces / component roles (§1, §8) |
| `src/Ara3D.Studio.WpfControls/EvaluatorWrapper.cs` *(host repo)* | how `Eval` is found + args bound; `RefreshUI` (§1, §2, §3) |
| `src/Ara3D.Studio.API/FlowObjectConverters.cs` | valid input/output conversions (§4) |
| `src/Ara3D.Studio.API/EvalContext.cs` | what `EvalContext` exposes (§3) |
| `src/Ara3D.Studio.API/Attributes.cs`, `src/Ara3D.PropKit/OptionsAttribute.cs` | `[OnDemand]`/`[Animated]`/`[Options]` (§2) |
| `Services/CompilerService.cs`, `Services/FolderService.cs` *(host repo)* | Scripts folder, watching, references (§7) |
| `examples/Ara3D.Studio.Examples/` | the example files cited in §5 |
