# ara3d-sdk — agent guide (Plato-related)

This SDK **consumes** Plato output; it must build standalone without the Plato toolchain.
Plato compiler + stdlib source live in `../submodules/Plato` (see its CLAUDE.md). Roadmap:
`../docs/plato-roadmap.md`.

## Plato-related layout
- `src/Plato.Generated/` — checked-in generated C# (V1, instance-member style). Produced by
  `..\..\tools\regen-plato.ps1 -Apply`; NEVER hand-edit (drift is diff-gated in CI).
- `src/Plato.Intrinsics/` — **SYNCED COPY** (see its README). Source of truth is
  `submodules/Plato/Plato.Intrinsics`; sync via `regen-plato.ps1 -Apply`. Never edit here.
- `src/Ara3D.Geometry/` — handwritten geometry algorithms layered on the generated code
  (imports both shared projects). Known name collisions with Plato solids: `Sphere`, `Cylinder`,
  `Curves` (resolved at V2 adoption).
- `tests/Ara3D.SDK.GeometryTests` — expected 15/15. `tests/Ara3D.SDK.Benchmarks` — BenchmarkDotNet
  baseline (`baseline-2026-07-07.md`); don't modify to measure new code — build scratch probes instead.
- `tools/ApiSnapshot` + `tools/api-snapshot.ps1` — deterministic public-API dump →
  `docs/api/plato-generated-api-baseline.txt` (8,907 lines). Re-run after any generated-surface change.
- Conformance suites live in the Plato repo (`submodules/Plato/conformance`), not here.

## Hard rules
1. Pre-existing user work in the tree (`Ara3D.SDK.sln`, `ext/Ara3D.IfcLoader`, `wip/`,
   `tests/Ara3D.IfcMeshingComparison`, `.gitignore` edits) — never touch or stage.
2. New test/tool projects are NOT added to the .sln.
3. `Directory.Build.props` defines package-version variables; projects outside this repo's root
   (e.g. in the Plato repo) must pin versions explicitly.
4. No git commits unless the mission says so.

## Mission protocol
Same as the Plato repo: maintain `PROGRESS.md`, finish with a roadmap DONE note, `COMMIT_MSG.txt`,
and a ≤300-word report (files / gates table / surprises / rerun commands).
