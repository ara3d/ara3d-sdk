# AGENTS.md — Working in the Ara3D SDK

This is the operating guide for AI agents (and humans) writing code in this repository.
Read it before making changes. It encodes how this codebase is built and maintained.

This project is maintained by **one person** with virtually no outside contributions.
Optimize for that reality: code that is **simple, correct, easy to refactor, and cheap to
understand six months later** beats code that is clever, abstract, or "enterprise-grade".

---

## 1. The two rules that override everything

1. **The code must work.** Correctness first. A working simple version beats a broken elegant one.
2. **The code must be easy to improve and refactor.** Everything below serves this.

If a guideline here ever conflicts with these two rules, the two rules win.

---

## 2. The standard workflow (do things in steps)

Work in small, verifiable increments. Never combine "make it work" with "make it pretty"
in the same step.

1. **Add code + tests** for the smallest useful slice of behavior.
2. **Verify it works** — build and run the tests (see §8). Do not proceed on a red build.
3. **Plan the refactor** — note what should improve. Add `// TODO:` markers in code and a
   matching entry in [`docs/TECHNICAL_DEBT.md`](docs/TECHNICAL_DEBT.md).
4. **Save the state** — this is a natural stopping/commit point. The working version is preserved.
5. **Apply the refactor** — change structure, not behavior. Tests from step 1 prove you
   didn't break anything.

> **Never refactor on a red build.** If tests are failing, get them green first, then refactor.

When given a multi-step task, use a todo list and keep it current.

---

## 3. Core principles

First, the monorepo-wide **API-first design principles** (canonical list in the root
[`AGENTS.md`](../AGENTS.md)) apply to everything here:

- **Write code as if writing a public API** — these SDK libraries literally are one.
- **Eat your own dogfood:** consume existing SDK APIs before adding new ones; awkwardness in an existing API is a reason to improve it, not bypass it.
- **Design for relocation:** code should move cleanly between projects/layers — few, explicit dependencies.
- **Write for the next learner:** someone else must be able to learn and use it quickly.
- **Obvious usage:** correct use discoverable from signatures and names alone.
- **Types and affordances guide correct use:** illegal states unrepresentable; misuse a compile error where possible.
- **Path of least resistance = best practice:** the easiest way must be the right way.
- **Composition and reuse by default:** every new piece is a candidate building block.

These are ordered roughly by how often they apply.

- **Keep it simple at first.** Start with the most direct solution that could work.
- **Use as little code as possible** to achieve the goal. Less code = fewer bugs, easier refactors.
- **Make it work before you improve it.** Resist premature abstraction and premature optimization.
- **Avoid repetition.** The second time you copy-paste, stop and extract a helper.
- **Reuse code when it makes sense** — but do not contort code to force reuse.
- **Minimize side effects.** Prefer functions that take inputs and return outputs.
- **Minimize dependencies.** See §7. The core libraries are nearly dependency-free; keep them that way.
- **Identify and track areas for improvement** instead of fixing everything at once (see §6).
- **Minimize the chance of breaking things when adding code.** Add alongside; don't rewrite
  working code unless that is the task.

### Priority order for geometry code

For the geometry utility library (and modifier/geometry samples), evaluate every function
against these properties, **in this order** — earlier ones win when they conflict:

1. **Correct** — it computes the right answer.
2. **Composable** — it combines cleanly with other functions.
3. **Reusable** — it generalizes beyond the one call site.
4. **Functional** — inputs to outputs; prefer expressions.
5. **Side-effect free** — no mutation of inputs or shared state.
6. **Succinct** — as little code as the above allow.
7. **Easily verifiable** — obvious to read and test for correctness.

A more efficient or mutable variant is a **later** step, and should land as a *separate*
function that can be compared against the canonical functional implementation — never by
compromising the canonical one.

---

## 4. C# style for this repo

This codebase has a distinct, consistent style. Match it.

### Favor immutable extension functions on interfaces

The author's favorite code is **immutable extension functions on interfaces**. Prefer this
shape over methods buried in concrete classes. Example pattern from `Ara3D.Collections`:

```csharp
// Small interface, then behavior added via extension functions that return new values.
public static bool InRange<T>(this IReadOnlyList<T> self, int n)
    => n >= 0 && n < self.Count;

public static bool IsEmpty<T>(this IReadOnlyList<T> self)
    => self.Count == 0;
```

- **Interfaces should be as small as possible to still be useful.** Add capability through
  extension functions, not by widening the interface.
- Prefer **immutable** data: `readonly` fields, `readonly struct`, return new values instead
  of mutating.
- Extract **helper functions, structs, classes, and interfaces** freely when they clarify intent.

### Functional style and LINQ

- **Use functional programming styles when appropriate.** Map/filter/fold over loops when it
  reads more clearly.
- **Be generous with LINQ**, and especially with the `Ara3D.Collections` LINQ-style extensions
  on `IReadOnlyList<T>` (`Select`, `Range`, `UpTo`, `RepeatElements`, `Any`, `IsEmpty`, …).
  Prefer these over hand-rolled loops for list transforms; they exist for performance and clarity.
- Reach for plain `for` loops only in genuinely hot paths where allocation/closures matter.

### Generics

- **Use generics when appropriate** to avoid duplication across types — but don't add type
  parameters a caller never varies. A concrete type now beats a speculative generic.

### `var`, assertions, and argument checking

- **Use `var` a lot.** The type is usually obvious from the right-hand side.
- **Use debug assertions generously** (`Debug.Assert(...)`) to document and enforce invariants.
  These are free in release builds and catch mistakes early.
- **Don't check every argument in every function.** If obviously incorrect usage will throw
  on its own (e.g. a null deref, an out-of-range index), let it throw. Reserve explicit
  validation for public entry points where a clear message genuinely helps.

### Small units

- **Break up long functions, classes, interfaces, and files into smaller ones.** If a function
  doesn't fit on a screen or a file mixes unrelated concerns, split it.
- One primary type per file is the norm here (closely related small types may share a file).

---

## 5. Comments

- **Function comments are one short line** with the most important information. Match the
  existing `/// <summary>` one-liners.
- **Don't comment obvious things.** No `// increment i` narration. No comments that restate
  the code.
- **Never use comments to explain the change you are making** ("// changed this to fix bug").
  That belongs in the commit message, not the source.
- The only long-lived inline marker is `// TODO:` for tracked improvements (see §6).

---

## 6. Tracking improvements (technical debt)

Don't fix everything at once, and don't silently leave messes. When you spot something worth
improving but out of scope:

1. Add a `// TODO:` marker at the spot in the code, with a short description.
2. Add a matching entry to [`docs/TECHNICAL_DEBT.md`](docs/TECHNICAL_DEBT.md) (file path +
   what + why). This is the single place the maintainer scans to plan future work.

Keep TODOs actionable and specific. "TODO: clean up" is useless; "TODO: this re-reads the
file on every call, cache the buffer" is useful.

---

## 7. Dependencies

- The core `Ara3D.SDK` libraries in `src/` are .NET 8. Cross-platform consumers should use
  `Ara3D.SDK.Core` or `Ara3D.SDK.Geometry`. File formats, BOS, and IFC are in `Ara3D.SDK.IO`
  (`net8.0-windows`). The full `Ara3D.SDK` meta-package targets `net8.0-windows` and includes
  Windows extensions from `ext/`.
- **Do not add a NuGet dependency to a `src/` project** to save yourself a little code. Prefer
  writing a small helper. If a dependency is genuinely warranted, stop and propose it first.
- Windows-only or heavier-dependency code belongs in `ext/`. Plug-ins and host apps belong in
  `plugins/` and `apps/`. Optional third-party adapters belong in `integrations/`.
- New standalone tools belong in `toolchain/`; tests and dev utilities in `tests/`.
- **`toolchain/` is never NuGet-packed** — `toolchain/Directory.Build.props` sets `IsPackable=false`;
  only projects in `build/packages.txt` (`src/` and `ext/`) are published.

---

## 8. Building and testing

- **Tests are NUnit 3**, `net8.0`, living under `tests/`. Test methods are marked `[Test]` and
  commonly `static`; logging to `Console`/`Logger.Console` is normal and expected.
- **Be generous with tests, but make them useful and non-trivial.** A test should be able to
  fail for a real reason. Don't write tests that only assert the framework works.
- **Don't be afraid to delete tests** that no longer reflect intended behavior. Stale tests
  that pin obsolete behavior are worse than no test. Deleting them is part of refactoring.
- Tests double as **runnable, debuggable entry points** — you can run a single `[Test]` directly
  from the IDE/console to exercise a code path. Prefer adding such a test over writing a
  throwaway script.

Build and test from the repo root using the helper scripts (preferred over raw `dotnet`
commands so behavior stays consistent):

```bat
build.bat                :: build the solution (Debug); "build.bat Release" for Release
test.bat                 :: run the FULL supported suite (including Slow)
test.bat fast            :: run supported tests, skip tests tagged Category("Slow")
test.bat <area>          :: run only one area's tests
test.bat <area> fast     :: run one area, skip Slow tests (good default for inner loop)
test.bat <area> <name>   :: run only tests in <area> whose full name contains <name>
test.bat knownissues     :: run known-broken behavior tests (opt-in only)
test.bat ifcmesher fast  :: IFC mesher T0 goldens + geometry oracles + coverage gate
test.bat ifcmesher       :: IFC mesher correctness including Slow corpus scans
test.bat ifcmesher parity:: IFC mesher web-ifc parity scorecard (diagnostic)
```

`<area>` is one of `all | sdk | geometry | bim | devtools | nuget | knownissues | ifcmesher`. The supported
default areas map to:

| Area | Test project | Covers (changed `src/` libraries) |
| --- | --- | --- |
| `sdk` | `Ara3D.SDK.Tests` | `Ara3D.IO.VIM`, `Ara3D.IO.BFAST`, `Ara3D.Utils`, `Ara3D.Logging`, `Ara3D.PropKit` |
| `geometry` | `Ara3D.SDK.GeometryTests` | `Ara3D.Geometry`, `Ara3D.IO.PLY` |
| `bim` | `Ara3D.BimOpenSchema.Tests` | `Ara3D.BimOpenSchema`, `Ara3D.BimOpenSchema.IO`, glTF |
| `devtools` | `Ara3D.SDK.DevTools` | Roslyn / `Microsoft.CodeAnalysis` helpers |
| `nuget` | `Ara3D.SDK.NuGet.Tests` | Packed `.nupkg` restore from `artifacts/` (run after `pack.bat`) |
| `ifcmesher` | `Ara3D.IfcMeshingComparison` | Approach1 IFC mesher + geometry oracles (wip) |

`ifcmesher` is excluded from default `test.bat` / `test.bat fast` (same as nuget/knownissues). Categories:

| Category | Role |
| --- | --- |
| `IfcMesherCorrectness` | First-principles oracles (winding, cuts, coverage, correctness scorecard) |
| `IfcMesherParity` | web-ifc BFAST comparison — diagnostic only (`test.bat ifcmesher parity`) |
| `IfcMesherCatalog` | Full studio data catalog evaluation |
| `Slow` | File I/O / large corpus |

One-click gate: `tests\Ara3D.IfcMeshingComparison\RunGeometryGate.bat` (= `test.bat ifcmesher fast`).
IFC fixtures resolve from `tests/.../data/ifc/`, then `studio/data/`, then `ARA3D_IFC_TEST_DIRS`.

`knownissues` maps to `Ara3D.SDK.KnownIssues.Tests`. It is not part of `test.bat`,
`test.bat fast`, `release.bat`, or the solution default test run.

`nuget` is also excluded from default `test.bat` / `test.bat fast` runs. Use `test.bat nuget`
after packing, or `publish-nuget.bat smoke` for the full release dry-run. See
[`docs/NUGET_RELEASE.md`](docs/NUGET_RELEASE.md). Script cheat sheet: [`docs/WORKFLOWS.md`](docs/WORKFLOWS.md).

### 8.1 Scoped testing — don't rerun everything for small changes

The goal is fast iteration on small, localized changes without giving up confidence.
Match the test scope to the blast radius of the change:

- **Tiny / highly localized change** (a single function/file, no public signature change):
  run just the matching area, skip slow file-I/O tests, and narrow by name when helpful —
  `test.bat geometry fast` or `test.bat geometry Delaunay`. Iterate here until green.
- **Area-level change** (one library, behavior changed but API stable): run that whole area —
  `test.bat geometry fast` during iteration; run `test.bat geometry` (includes Slow) if you
  touched I/O or serialization.
- **Cross-cutting change** (shared types like `Ara3D.Collections`/`Ara3D.Memory`/`Ara3D.Utils`,
  a public signature change, or anything many libraries depend on): run the **full suite** —
  `test.bat`.

**Hard rule:** scoped runs are for the *inner loop* only. Before you consider a task done
(§11) or commit, you must run the **full suite at least once** and have it green. Speed is for
iterating; the full run is the safety net.

When unsure which area a change belongs to, use the table above; if it spans more than one,
run the full suite.

Run a single test by name within an area:

```bat
test.bat geometry Delaunay
```

### 8.2 Slow tests (`Category("Slow")`)

Tests that load large files from disk or do heavy I/O are tagged `[Category("Slow")]`.
Use `test.bat fast` or `test.bat <area> fast` to skip them during the inner loop.

Mark a test Slow when it reads real data files (VIM, BOS, PLY, parquet, glTF, etc.) or
otherwise takes noticeably longer than in-memory unit tests. Pure algorithm/geometry tests
stay untagged. Before committing, run the **full** suite (`test.bat`) at least once so Slow
tests are included.

### 8.3 Known-issues tests (`test.bat knownissues`)

Known-issues tests document bugs or incomplete behavior that should fail until the
underlying issue is fixed. They live in `tests/Ara3D.SDK.KnownIssues.Tests`, are tagged
`[Category("KnownIssue")]`, and are intentionally excluded from all normal/default test
runs.

Move a failing test here only when the behavior is still important to track but must not
block normal development. When the bug is fixed, move the test back into the appropriate
supported test project and remove the known-issues copy.

---

## 9. Git and Cursor

- **Do not commit or push unless the user asks.**
- **Local save:** `save.bat "message"` commits without pushing.
- **During work:** `test.bat <area> fast`; run full `test.bat` before done (§11).
- **NuGet release:** use `release-nuget.bat` — see [`docs/NUGET_RELEASE.md`](docs/NUGET_RELEASE.md).

Script cheat sheet: [`docs/WORKFLOWS.md`](docs/WORKFLOWS.md).

---

## 10. Shell and tooling rules (important for agents)

**Do not get stuck fighting PowerShell quoting and newline issues.** When you need to run
something repeatable:

- Prefer a **DOS batch file (`.bat`)** for scripted commands, OR
- Add a **new tool project under `toolchain/`**, OR
- Write an **explicit NUnit `[Test]`** that performs the work and can be run directly from
  the IDE or console.

Avoid multi-line PowerShell scripts and clever inline quoting. If a one-liner won't work
cleanly, write a `.bat` file or a test instead. This keeps work reproducible and avoids
wasted time on shell escaping.

---

## 11. Definition of done for a change

Before considering a task complete:

- [ ] The new behavior **works** and is exercised by at least one useful test.
- [ ] `build.bat` is **green**, and the **full** `test.bat` suite has been run once and passes
      (scoped `test.bat <area>` runs are fine during iteration, but not as the final check).
- [ ] New code follows the style in §4 (small, immutable, extension functions, `var`, LINQ where it helps).
- [ ] No copy-paste duplication left behind (§3).
- [ ] Any known shortcuts are marked with `// TODO:` and logged in `docs/TECHNICAL_DEBT.md` (§6).
- [ ] No unjustified new dependencies in `src/` (§7).
- [ ] Comments are minimal and meaningful (§5).
