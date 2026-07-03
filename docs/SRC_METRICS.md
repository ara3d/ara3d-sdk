# src/ code metrics

Snapshot of the supported SDK libraries under [src/](../src/). Generated 2026-07-03.

## Summary

| Metric | Count |
| --- | ---: |
| Projects (`.csproj`) | 27 |
| Source files (`.cs`) | 690 |
| Total lines | 83,584 |
| Code lines | 58,722 |
| Blank lines | 14,748 |
| Comment lines | 10,114 |
| Types | 1,295 |
| Methods / functions | 8,358 |

Code lines are non-blank, non-comment lines. Types and methods are approximate; see [Methodology](#methodology).

## Hand-written vs generated

Files ending in `.g.cs` (Plato codegen, glTF schema extensions, etc.):

| Category | Files | Total lines | Code lines | Types | Methods |
| --- | ---: | ---: | ---: | ---: | ---: |
| Generated (`.g.cs`) | 184 | 18,014 | 12,660 | 446 | 4,432 |
| Hand-written | 506 | 65,570 | 46,062 | 849 | 3,926 |

## Per project

Four meta-packages (`Ara3D.SDK`, `Ara3D.SDK.Core`, `Ara3D.SDK.Geometry`, `Ara3D.SDK.IO`) contain no `.cs` sources; they reference other projects for NuGet packaging.

`Plato.Generated` and `Plato.Intrinsics` are shared items (`.projitems`) compiled into `Ara3D.Geometry`. They are listed separately below; do not add those rows to other project totals (that would double-count shared code).

| Project | Files | Total lines | Code lines | Types | Methods |
| --- | ---: | ---: | ---: | ---: | ---: |
| Ara3D.IO.SharpGLTF | 102 | 25,549 | 16,887 | 251 | 1,124 |
| Ara3D.Geometry | 79 | 13,686 | 9,956 | 172 | 1,016 |
| Plato.Generated (shared) | 160 | 13,037 | 8,999 | 380 | 4,162 |
| Ara3D.Utils | 91 | 8,528 | 6,783 | 116 | 599 |
| Ara3D.BimOpenSchema | 23 | 3,568 | 2,693 | 50 | 254 |
| Plato.Intrinsics (shared) | 18 | 2,979 | 1,652 | 20 | 194 |
| Ara3D.Collections | 19 | 1,901 | 1,223 | 27 | 38 |
| Ara3D.Models | 13 | 1,887 | 1,458 | 19 | 165 |
| Ara3D.IO.VIM | 15 | 1,480 | 1,132 | 23 | 104 |
| Ara3D.PropKit | 35 | 1,469 | 1,161 | 40 | 179 |
| Ara3D.BimOpenSchema.IO | 10 | 1,431 | 1,171 | 10 | 68 |
| Ara3D.IO.G3D | 14 | 1,338 | 954 | 19 | 60 |
| Ara3D.IO.StepParser | 10 | 1,272 | 980 | 10 | 43 |
| Ara3D.Memory | 14 | 1,256 | 936 | 19 | 76 |
| Ara3D.F8 | 4 | 1,041 | 617 | 4 | 89 |
| Ara3D.IO.BFAST | 9 | 906 | 626 | 16 | 54 |
| Ara3D.Utils.Roslyn | 9 | 747 | 605 | 10 | 48 |
| Ara3D.Studio.API | 14 | 638 | 454 | 28 | 32 |
| Ara3D.IO.GltfExporter | 16 | 635 | 415 | 17 | 15 |
| Ara3D.IO.PLY | 3 | 556 | 424 | 12 | 18 |
| Ara3D.DataTable | 18 | 531 | 454 | 19 | 42 |
| Ara3D.Logging | 15 | 439 | 268 | 19 | 30 |
| Ara3D.IO.GeoJson | 5 | 368 | 288 | 15 | 37 |
| Ara3D.WorkItems | 7 | 313 | 214 | 7 | 21 |
| Ara3D.Events | 6 | 207 | 148 | 12 | 6 |
| Ara3D.SDK.Geometry | 0 | 0 | 0 | 0 | 0 |
| Ara3D.SDK.IO | 0 | 0 | 0 | 0 | 0 |
| Ara3D.SDK | 0 | 0 | 0 | 0 | 0 |
| Ara3D.SDK.Core | 0 | 0 | 0 | 0 | 0 |

## Methodology

### Scope

- **Root:** `src/` only (not `ext/`, `apps/`, `plugins/`, `tests/`, or `toolchain/`).
- **Source files:** all `.cs` files, recursively.
- **Excluded:** anything under `bin/` or `obj/` build output folders.
- **Projects:** count of `.csproj` files under `src/`.

### Line classification

Each physical line in a `.cs` file is classified as:

1. **Blank** - empty or whitespace only.
2. **Comment** - starts with `//`, or inside a `/* ... */` block comment.
3. **Code** - everything else.

**Total lines** = blank + comment + code. This is a simple lexer, not a full C# parser; edge cases (e.g. `//` inside a string literal) are rare and not specially handled.

### Types

Counted via regex on lines that declare a top-level type keyword:

`class`, `struct`, `interface`, `enum`, `record`, `delegate`

Optional modifiers (`public`, `static`, `partial`, etc.) may precede the keyword. Nested types declared at deeper indentation are still matched. This **over-counts slightly** when the keyword appears in comments or strings, and **under-counts** file-scoped types or unusual formatting.

### Methods / functions

Counted via regex on lines that look like method declarations: a return type (or modifier chain), an identifier, a parameter list `(...)`, then `{`, `;`, or `=>`.

Includes most named methods, operators declared like methods, and some constructors. **Excludes** (approximately):

- Local functions and lambdas
- Property getters/setters without a classic method signature line
- Methods whose declaration spans multiple lines with the parameter list on a later line

Treat method counts as **useful ballpark figures**, not exact compiler semantics.

### Regenerating

From the repo root:

```bat
powershell -NoProfile -ExecutionPolicy Bypass -File toolchain\count-src-metrics.ps1
```

The script overwrites this file.
