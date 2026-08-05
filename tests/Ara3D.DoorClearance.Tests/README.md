# Ara3D.DoorClearance.Tests

A demonstration of the pipeline **building code → machine-readable rule → machine checker**, run
over a real IFC model. A paraphrased accessibility provision (door clear width) is encoded as a
JSON rule file, a small checker evaluates every door in the model against every rule, and each
(door, rule) pair gets exactly one verdict backed by evidence and a citation. A human override
round-trips through the IFC file itself as an appended property set, byte-exactly reversible.

Not in scope: this is not a code-compliance product. The citations are illustrative paraphrases in
the style of NBC 2020 3.8.3.12 — **not legal text** — and the geometric checks are deliberately
coarse (see "What is simplified").

## Running

```
dotnet test tests/Ara3D.DoorClearance.Tests
```

The project is intentionally **not** in `Ara3D.SDK.sln`. Tests need the IFC test kit at
`C:\Users\cdigg\git\nrc-ifc-llm\IFC-Test-Kit\duplex.ifc`; when absent every test is skipped with
`Assert.Ignore`, matching the convention in `tests/Ara3D.Ifc.Tests`.

Outputs land in `artifacts/Ara3D.DoorClearance.Tests/`:

- `verdicts.csv` — one row per (door, rule): globalId, ruleId, verdict, evidence, citation.
  Sorted, culture-invariant, LF newlines, no timestamp — byte-deterministic and SHA256-hashed.
- `run-log.txt` — model path, entity/door counts, rule list, verdict counts, the CSV hash,
  OS/.NET versions, and the UTC timestamp (kept out of the hashed file on purpose).
- `duplex-override-*.ifc` — working copies used by the override round-trip test.

## The rules (`rules/door-clearance-rules.json`)

| Rule | Kind | What it checks |
|---|---|---|
| DC-W1 | property-threshold | Declared `IFCDOOR.OverallWidth` ≥ 850 mm. |
| DC-W2 | property-threshold | `Pset_DoorCommon.ClearWidth` ≥ 850 mm. Duplex does not author this property, so every door is **inconclusive** — the checker refuses to guess a true clear width. |
| DC-M1 | measured-vs-declared | Leaf width encoded in the Revit family size name (`"0762 x 2032mm"`) vs the declared `OverallWidth`, tolerance 25 mm. |
| DC-Z1 | zone-unobstructed | Clearance zone box at the door's world placement (footprint expanded by one door width, door height tall) must contain no `IFCFURNISHINGELEMENT` placement origin. Storey filter: applies to **Level 1** only; other doors are **not_applicable**. |

Verdict vocabulary: `pass`, `fail`, `not_applicable` (fails the applicability filter),
`inconclusive` (required property or geometry unavailable — never silently passed).

## Expected verdict distribution on duplex.ifc

14 doors × 4 rules = 56 verdicts:

| Rule | pass | fail | not_applicable | inconclusive |
|---|---|---|---|---|
| DC-W1 | 8 | 6 | 0 | 0 |
| DC-W2 | 0 | 0 | 0 | 14 |
| DC-M1 | 14 | 0 | 0 | 0 |
| DC-Z1 | 4 | 2 | 8 | 0 |

The six DC-W1 failures are the four 762 mm and two 813 mm doors; the two 1250 mm and six 864 mm
doors pass. The two DC-Z1 failures are Level 1 doors with a furnishing element origin inside the
clearance zone.

## The override story

`OverrideTests` picks a failing door and records a reviewer's override *in the model*: an appended
`Ara3D_Compliance` property set (`Verdict`, `OverrideVerdict`, `OverrideReason`, `ReviewedBy`)
built with `IfcPropertySetBuilder` and spliced in with `IfcPatcher`. `IfcDiff` verifies exactly
the expected entities were added and nothing else changed; removing them restores the file
byte-for-byte. The source model is never touched — edits happen on a copy in `artifacts/`.

## What is simplified (honest notes)

- **Leaf width is not clear width.** `OverallWidth` is the nominal leaf/opening size; true clear
  width (open leaf face to opposite stop) is smaller. DC-W2 exists precisely to show the checker
  saying "inconclusive" instead of conflating the two.
- **Citations are illustrative.** Wording is paraphrased in the style of NBC 2020 3.8.3.12 and is
  not the legal text of any code.
- **The geometric rule is AABB-level, from STEP placements, not meshes.** Loading tessellated
  geometry needs the native web-ifc dll and the mesher stack, so DC-Z1 instead composes each
  element's `IFCLOCALPLACEMENT` → `IFCAXIS2PLACEMENT3D` chain exactly (rotations included) and
  clashes an axis-aligned zone box against furnishing placement *origins* — not their extents,
  and the zone box ignores swing direction (it expands the footprint on all horizontal sides).
- **"Measured" in DC-M1 is the family-name leaf size**, standing in for a mesh-measured width.
- Storey attribution comes from `IFCRELCONTAINEDINSPATIALSTRUCTURE` only.

## Structure

Shared byte-exact IFC machinery (`IfcSourceFile`, `IfcDiff`, `IfcPatcher`, `IfcPropertySetBuilder`,
`IfcGuid`, `IfcStepText`) is source-linked from `tests/Ara3D.Ifc.Tests`. New code:

- `ComplianceRules.cs` — rule records + JSON loading.
- `ModelFacts.cs` — extracts doors, storeys, psets, and furnishing placements from the STEP data.
- `StepPlacement.cs` — exact placement-chain composition (`Vec3`/`Pose`).
- `ComplianceChecker.cs` — pure evaluation: facts × rules → sorted verdict records.
- `VerdictReport.cs` — deterministic CSV + run log + SHA256.
