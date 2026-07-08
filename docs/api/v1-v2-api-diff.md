# V1 → V2 API-diff report (Plato roadmap Phase 2.5)

*Compares the public API surface of the V1 (instance-member) and V2 (C# 14 extension-member)
Plato-generated libraries. Companion to `docs/plato-roadmap.md` Phase 2; gate for the 2.6 trial swap.*

Date: 2026-07-07

## Methodology

- **V1 side:** the Phase 0.4 baseline `docs/api/plato-generated-api-baseline.txt` (reflection dump of
  the built `Ara3D.Geometry` assembly by `tools/ApiSnapshot`, sorted, deterministic).
- **V2 side:** a throwaway assembly built outside the repo tree (net8.0, `LangVersion 14`, SDK
  10.0.301, x64) importing `src/Plato.Generated.V2/Plato.Generated.V2.projitems` +
  `src/Plato.Intrinsics/Plato.Intrinsics.projitems` and referencing Ara3D.Collections/Memory/Utils,
  snapshotted with the same unmodified ApiSnapshot tool (8,965 lines).
- **Scope filter:** the V1 baseline covers all of `Ara3D.Geometry`, which also contains handwritten
  code (Topology, AABB tree, GeometryUtil, …). The comparison is restricted to the **389 types present
  in both assemblies** (the generated + intrinsic surface): 6,622 V1 member lines in scope. The
  **164 V1-only types are handwritten and out of scope.**
- **Classification, not diffing.** C# 14 extension members appear via reflection as *static methods on
  the library classes* (`v.Length` → `static Number get_Length(Vector3 a)` on the library class), so a
  naive line diff would report every moved member twice. Instead, each V1 member missing from its type
  in V2 was matched against an index of all V2 static methods:
  - instance method `Ret Name(p…)` on `X` → `static Ret Name(X x, p…)` ⇒ **MOVED**
  - instance property `Ret Name { get; }` on `X` → `static Ret get_Name(X x)` ⇒ **MOVED**
  - static member `X.Name` → same signature on a library class ⇒ **MOVED**
  - no counterpart ⇒ **REMOVED**; V2 members with no V1 origin ⇒ **ADDED**
  - Matching is exact on member kind, return type, name, and parameter *types* (names stripped).
- Compiler-generated extension-block skeleton types (`<G>$…`/`<M>$…`, 1,952 snapshot lines) are
  excluded: they are exported but unspeakable, and invisible to source code. (They do inflate the raw
  reflection surface of the V2 assembly; harmless, but worth knowing when eyeballing DLLs.)

## Summary

| Classification | Count | Share of V1 in-scope surface |
|---|---:|---:|
| UNCHANGED (identical member on the same type) | 5,122 | 77.3 % |
| MOVED (instance/struct member → extension member, call syntax preserved) | 1,486 | 22.4 % |
| REMOVED (no V2 counterpart) | 14 | 0.2 % — **all 14 are handwritten code, not emitter output; emitter-attributable removals: 0** |
| ADDED (V2 member with no V1 counterpart) | 0 | — |

Moved = 626 properties + 860 methods, drawn from 89 source types, landing in 17 library classes:

| Target library class | Moved members |
|---|---:|
| Core | 537 |
| ArrayLibrary | 218 |
| Meshes | 171 |
| Algebra | 100 |
| IIntervalLibrary | 87 |
| Geometry | 78 |
| Curves / Transforms / Vectors | 70 each |
| PolarCurves | 26 |
| IBoundsLibrary | 20 |
| Angles | 16 |
| AngularCurves2D | 10 |
| Integers | 6 |
| AngularCurves3D | 4 |
| Curves3D | 2 |
| Curves2D | 1 |

New types in V2: the 16 library classes above minus `Curves`/`Constants` (which existed by name in V1)
plus `Angles` etc. — exactly the one-static-class-per-Plato-library organization decided in 2.1.
No generated *struct* type was added or removed.

## REMOVED — full list, each explained

Every entry is handwritten C# living in `src/Ara3D.Geometry` that compiles into the V1 assembly but
(correctly) is not part of the V2 generated-only assembly. **None is an emitter bug.**

**1–12. `Ara3D.Geometry.Curves` — twelve static members** (`Helix`, `LineCurve` ×2, `Mix`,
`QuadraticBezier`, `RemapInput` ×2, `Reverse`, `Spiral`, `Lerp(Point3D,Point3D,Number)`,
`RuledSurface`, `Circle { get; }`).
Source: handwritten `src/Ara3D.Geometry/Curves.cs` (`public static class Curves`). In V1 there is *no
generated* `Curves` class — the name is handwritten-only. In V2 the emitter generates a `Curves`
library class (from the Plato `Curves` library), so the diff pairs the two classes by name and the
handwritten members show as missing. Out of scope for the emitter comparison, **but this is a real
Phase 2.6 finding: name collision.** When `Ara3D.Geometry.csproj` imports V2, generated
`public static class Curves` (Curves.g.cs) and handwritten `public static class Curves` (Curves.cs)
collide (CS0101). Resolution at 2.6: declare both `partial` (both are static classes in the same
namespace, so merging works), or rename/fold the handwritten helpers. Same category as the
`Sphere`/`Cylinder` collisions documented in roadmap 0.6 — add `Curves` to that list. (The emitter's
`*Library` suffix rule handled library-vs-*generated-type* collisions; it cannot know about
handwritten classes.)

**13. `Ara3D.Geometry.PolyLine3D | method PolyLine3D Transform(Transform3D t)`.**
Source: handwritten `partial struct PolyLine3D` in `src/Ara3D.Geometry/Polygons.cs:24`. This partial
also explains the single type-header difference found: V1's `PolyLine3D` implements
`IDeformable3D<PolyLine3D>`/`ITransformable3D<PolyLine3D>`, V2's does not — the interfaces are added
by the handwritten partial, not by the emitter. Merges back automatically at 2.6 (generated structs
are still `partial`).

**14. `Ara3D.Geometry.Vector3 | method Vector3 NormalizedCross(Vector3 other)`.**
Source: handwritten `partial struct Vector3` in `src/Ara3D.Geometry/GeometryUtil.cs:13`. Same story:
returns at 2.6 via the partial merge. (Note it shares the fate of the Phase 0.3 Dot/Cross
reconciliation — it calls the instance intrinsics, so no ambiguity is expected.)

## ADDED — full list

None. The mapping was verified to be a **bijection**: the 17 library classes expose exactly 1,486
public member signatures, each one consumed as the counterpart of exactly one V1 member — no leftover
library members, no V2 struct members without a V1 origin. (The 2.2–2.4 note of "1,503 moved members"
counted emitted declarations at generation time; the reflection-level count is 1,486 — the small delta
is declarations that land in the classes shared with V1 (`Constants`/`Extensions`/`Interfaces`, whose
members line-match exactly and are counted as UNCHANGED here) or that fold to one metadata signature.)

## Spot checks — call-syntax compatibility (10 representative members)

| # | V1 (baseline line, abbreviated) | V2 (snapshot line, abbreviated) | Call site | Status |
|---|---|---|---|---|
| 1 | `Vector3 \| prop Number Length { get; }` | identical, still on `Vector3` | `v.Length` | KEPT (intrinsic) |
| 2 | `Vector3 \| method Vector3 Lerp(Vector3 b, Number t)` | identical, still on `Vector3` | `v.Lerp(b, t)` | KEPT (interface obligation) |
| 3 | `Vector3 \| prop static Vector3 UnitX { get; }` | identical, still on `Vector3` | `Vector3.UnitX` | KEPT (static stays in struct) |
| 4 | `Vector3 \| method Number Dot(Vector3 right)` | identical, still on `Vector3` | `v.Dot(u)` | KEPT (intrinsic) |
| 5 | `Vector3 \| operator static Vector3 op_Addition(…)` | identical, still on `Vector3` | `a + b` | KEPT (operators stay in struct) |
| 6 | `Vector3 \| prop Vector3 ClampZeroOne { get; }` | `Core \| method static Vector3 get_ClampZeroOne(Vector3 x)` | `v.ClampZeroOne` | MOVED — extension *property*, syntax identical |
| 7 | `Angle \| prop Number Degrees { get; }` | `Angles \| method static Number get_Degrees(Angle x)` | `angle.Degrees` | MOVED — extension property, syntax identical |
| 8 | `Vector3 \| method Vector3 Project(Vector3 other)` | `Vectors \| method static Vector3 Project(Vector3 v, Vector3 other)` | `v.Project(u)` | MOVED — extension method, syntax identical |
| 9 | `Vector3 \| prop Vector2 XY { get; }` | `Vectors \| method static Vector2 get_XY(Vector3 v)` | `v.XY` | MOVED — swizzles preserved as extension properties |
| 10 | `QuadGrid3D \| method QuadGrid3D Deform(Func<Point3D,Point3D> f)` | `Meshes \| method static QuadGrid3D Deform(QuadGrid3D x, Func<Point3D,Point3D> f)` | `grid.Deform(f)` | MOVED — extension method, syntax identical |

Rows 6–10 rely on C# 14 extension-member lowering (verified in the 2.1 spike, and exercised for real
by the V2 conformance suite, 142/36/0 identical to V1): a consumer compiled with `LangVersion 14`
writes exactly the same source as against V1. A consumer on an older compiler sees the moved members
only as static `get_XY(v)`-style methods — that is the one hard compatibility boundary.

## Verdict — is V2 call-site-compatible enough for the 2.6 trial swap?

**Yes.** At the member level the diff is as clean as it can be: **zero emitter-attributable removals,
zero additions** — every one of the 6,622 in-scope V1 members is either byte-identical on its
original type (77 %) or present in extension form that preserves the original call syntax (22 %); the
14 "removed" lines are all handwritten `Ara3D.Geometry` code that re-merges automatically (partials)
or was never generated in the first place. The unintended-silent-shadowing risk that motivated this
gate did not materialize: kept members and moved members are disjoint by construction, and no
same-name residue was found.

The residual 2.6 risks are project-level, not member-level:

1. **`Curves` class collision** (finding above) — handwritten `Curves.cs` vs generated `Curves.g.cs`;
   fix with `partial` or a rename, alongside the known `Sphere`/`Cylinder` resolutions from 0.6.
2. **Toolchain floor:** `Ara3D.Geometry` and every project that *calls* moved members with instance
   syntax must build with `LangVersion 14` (pin the .NET 10 SDK via `global.json`, per the 2.1
   decision). Downstream consumers of the compiled DLL on older compilers keep working but see the
   moved members as static methods only — release-notes line.
3. Cosmetic: the V2 assembly exports ~1,950 lines worth of unspeakable extension-skeleton types; no
   action needed, but the API-snapshot tooling should keep filtering them if V2 becomes the baseline.

## Reproduction

1. Build a scratch csproj (outside the repo) importing `Plato.Generated.V2.projitems` +
   `Plato.Intrinsics.projitems`, net8.0/`LangVersion 14`/x64, referencing
   Ara3D.Collections/Memory/Utils.
2. `dotnet run --project tools/ApiSnapshot -- <scratch>.dll v2-api.txt`
3. Classify against `docs/api/plato-generated-api-baseline.txt` with the matching rules in
   *Methodology* (exact member-kind + return type + name + parameter types; receiver prepended for
   instance→extension matching; `get_` prefix for property→extension-property matching).
