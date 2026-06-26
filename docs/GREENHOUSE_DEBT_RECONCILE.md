# Greenhouse Debt Scan - Needs Reconciliation

This file is a temporary holding area for debt items found by Dotnet Greenhouse.

Important: this is not the canonical technical debt log. Reconcile these items with
[`TECHNICAL_DEBT.md`](TECHNICAL_DEBT.md) later, then delete this file or move resolved
items into the canonical log.

Generated from:

```bat
greenhouse debt src --repo-root .
greenhouse debt tests --repo-root .
```

## Source TODO markers

These TODOs should be checked against `TECHNICAL_DEBT.md`. Some may already be represented
there at a higher level; others may need new entries or removal if stale.

- `src/Ara3D.BimOpenSchema/BimDataBuilder.cs`: `// TODO: it is very awkward that this is not a BimGeometryBuilder`
- `src/Ara3D.BimOpenSchema/BimGeometryExtensions.cs`: `// TODO: this needs to be replaced by directly talking to the Parquet data columns.`
- `src/Ara3D.BimOpenSchema/BimObjectModelExtensions.cs`: `// TODO: this is a hack.`
- `src/Ara3D.Domo/Repository.cs`: `// TODO: allow more sophisticated value updating (e.g., using function)`
- `src/Ara3D.Domo/Repository.cs`: `// TODO: provide a proper bulk notification (only notify once)`
- `src/Ara3D.Domo/Repository.cs`: `// TODO: if something fails, roll-back the whole transition (e.g., validate everything first)`
- `src/Ara3D.Geometry/GeometryUtil.cs`: `// TODO: many of these functions should live in other places, particular in the math 3D`
- `src/Ara3D.Geometry/GeometryUtil.cs`: `// TODO: I am not sure that the name "Include" make sense.`
- `src/Ara3D.Geometry/GeometryUtil.cs`: `// TODO: create Normal3D, Normal2D, Normal1D, UV`
- `src/Ara3D.Geometry/IsotropicRemesher.cs`: `// TODO: the face key needs to be considered.`
- `src/Ara3D.Geometry/IsotropicRemesher.cs`: `// TODO: this seems like there is a better way to do this.`
- `src/Ara3D.Geometry/IsotropicRemesher.cs`: `// TODO: I can see how this might be useful. It should probably be lifted out to tTopology. It also needs its own record struct.`
- `src/Ara3D.Geometry/Polygons.cs`: `/* TODO: would be nice`
- `src/Ara3D.Geometry/TransformableExtensions.cs`: `// TODO: AxisAngle might be broken.`
- `src/Ara3D.IO.BFAST/BFast.cs`: `// TODO: Check with CD: Should we bail out here?  This means that any`
- `src/Ara3D.IO.GltfExporter/GltfBuilder.cs`: `// TODO: we do something slightly not correct here ... we assume that all instances in a group share the material`
- `src/Ara3D.IO.PLY/PlyImporter.cs`: `// TODO: normals / colors / uv`
- `src/Ara3D.IO.SharpGLTF/Memory/MemoryImage.cs`: `// TODO: more checks required`
- `src/Ara3D.IO.SharpGLTF/Schema2/gltf.BufferView.cs`: `// TODO: clarify under which conditions bytestride needs to be defined or forbidden.`
- `src/Ara3D.IO.SharpGLTF/Schema2/gltf.BufferView.cs`: `// todo: search data on existing buffers for reusability and compression.`
- `src/Ara3D.IO.SharpGLTF/Schema2/gltf.ExtensionsFactory.cs`: `// TODO: check that persistentName has a valid extension name.`
- `src/Ara3D.IO.SharpGLTF/Schema2/gltf.Images.cs`: `// TODO: if external images have not been loaded into _ExternalImageContent`
- `src/Ara3D.IO.SharpGLTF/Schema2/gltf.Node.cs`: `// TODO: nameless nodes with decomposed transform`
- `src/Ara3D.IO.SharpGLTF/Schema2/gltf.TextureInfo.cs`: `// TODO: this may no longer be valid because KHR_animation_pointer requires the object to exist.`
- `src/Ara3D.IO.SharpGLTF/Schema2/Serialization.Binary.cs`: `// todo: buffer[0].Uri must be null`
- `src/Ara3D.IO.SharpGLTF/Transforms/IndexWeight.cs`: `// TODO: adding a positive and a negative weight can lead to a weightless item;`
- `src/Ara3D.IO.StepParser/StepGraph.cs`: `// TODO: delete`
- `src/Ara3D.Logging/ILogger.cs`: `// TODO: should the writer not be inherited from the previous logger?`
- `src/Ara3D.Logging/Job.cs`: `// TODO: complete this job thing. Figure out the exact interface I want.`
- `src/Ara3D.Models/Model3DBuilder.cs`: `// TODO: this is potentially confusing.`
- `src/Ara3D.Models/Model3DExtensions.cs`: `// TODO: I need two paths. One for non-colored meshes, and one for colored.`
- `src/Ara3D.Models/Model3DExtensions.cs`: `// TODO: we need  to be able to work more efficiently with buffers`
- `src/Ara3D.Models/Model3DExtensions.cs`: `// TODO: maybe should be in a geometry extensions class.`
- `src/Ara3D.Models/RenderModelData.cs`: `// TODO: optimization opportunity`
- `src/Ara3D.PropKit/PropAccessor.cs`: `// TODO: delete, I think.`
- `src/Ara3D.PropKit/PropDescriptorVector3.cs`: `// TODO: this and the functions in relawted types (see Vector2 and Vector4) should perhaps be generalized to support converters.`
- `src/Ara3D.ScriptService/ScriptingService.cs`: `// TODO: move to bowerbird`
- `src/Ara3D.Studio.API/FlowObject.cs`: `// TODO: maybe this is where the attributes might get discarded if no longer valid.`
- `src/Ara3D.Studio.API/FlowObject.cs`: `// TODO: this needs to be completed. To implement it easily "ITransformable3D" should exist without it requiring an argument`
- `src/Ara3D.Utils/PathUtil.cs`: `/// Todo: we need to replace strings with FilePath and DirectoryPath`
- `src/Ara3D.Utils/ProfilingUtil.cs`: `// TODO: remove all console references`
- `src/Ara3D.Utils/ZipUtil.cs`: `// TODO: there could be a bug in this code, when I used it I seemed to have some problems with sporadic Zip creation`
- `src/Plato.Intrinsics/Number.cs`: `// TODO: Figure out why these aren't being provided by Plato`
- `src/Plato.Intrinsics/ReadOnlyListMapExtensions.cs`: `// TODO: I have to decide what to do here.`

## Test debt and slow-test signals

These should be reconciled with the testing rules in `AGENTS.md`, especially the rule that
large file-I/O tests should be marked `Category("Slow")`.

- `tests/Ara3D.BimOpenSchema.Tests/BosTests.cs`: Slow test `TestLoadBimDataAndBimGeometry`
- `tests/Ara3D.BimOpenSchema.Tests/GltfMaterialFactory.cs`: `// TODO: validates this code and move it to the models project.`
- `tests/Ara3D.BimOpenSchema.Tests/Tests.cs`: likely long-running or I/O-heavy unmarked test `TestInputFileExists`
- `tests/Ara3D.BimOpenSchema.Tests/Tests.cs`: Slow test `TestReadInputFile`
- `tests/Ara3D.BimOpenSchema.Tests/Tests.cs`: Slow test `TestWriter`
- `tests/Ara3D.BimOpenSchema.Tests/Tests.cs`: Slow test `BimDataObjectModel`
- `tests/Ara3D.SDK.GeometryTests/PlyLoaderTests.cs`: Slow test `TestLoadFile`
- `tests/Ara3D.SDK.Tests/FileTests.cs`: likely long-running or I/O-heavy unmarked test `DataFiles`
- `tests/Ara3D.SDK.Tests/FileTests.cs`: Slow test `OpenVIM`
- `tests/Ara3D.SDK.Tests/FileTests.cs`: Slow test `OpenBFast`

## Path issues to verify

These path literals did not resolve under the repository root during the Greenhouse scan.
They may be genuinely wrong, or they may depend on a working directory assumption that should
be replaced with a helper.

- `tests/Ara3D.SDK.DevTools/UnitTest1.cs`: `..\..\..\..\..\src\Ara3D.PropKit\`
- `tests/Ara3D.SDK.DevTools/UnitTest1.cs`: `..\..\..\..\..\src\Ara3D.PropKit\`

## Reconciliation checklist

- Compare each item above with `TECHNICAL_DEBT.md`.
- Move missing high-value items into the canonical log.
- Remove items already covered at a broader level.
- Fix or mark stale TODOs in source when they no longer describe real work.
- Confirm whether the unmarked long-running tests should be marked `Category("Slow")`.
- Replace incorrect or working-directory-dependent paths with a fixture/path helper.
