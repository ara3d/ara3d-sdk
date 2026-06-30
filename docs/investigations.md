# Investigations

Short, human-readable notes for unresolved issues that need evidence before a fix.

Use the same short ID as `TODO.md` when an investigation backs a backlog item.

---

<a id="ifc-str-001"></a>

## `IFC-STR-001` - STEP / IFC String Decoding Ownership

**Status:** open  
**Priority:** P0 correctness  
**Related TODO:** [`TODO.md`](TODO.md) P0 #45

### Problem

`schependomlaan.ifc` appears to convert to BOS with some IFC / STEP string escape
sequences still visible. A known example pattern is Dutch text encoded with `\S\`,
such as:

- `Buiten jaloezie\S\kn`
- `Ori\S\kntatie arcering`
- `Wandbe\S\kindiging aanpassen auto`
- `Diepte be\S\kindiging`

These should become normal user-facing Unicode strings, for example `jaloezieën`,
`Oriëntatie`, and `beëindiging`.

### Evidence So Far

- `data/schependomlaan.ifc` contains 100 `\S\` escape occurrences.
- No `\X\`, `\X2\`, or `\X4\` occurrences were found in that sample during this pass.
- `StepTokenizer` identifies quoted strings but deliberately stores raw token spans.
- `StepToken.AsString()` strips the surrounding quotes but does not decode STEP string
  escapes.
- `IfcStringDecoder.DecodeIfc()` exists in `ext/Ara3D.IfcLoader` and handles `\S\`,
  `\X\`, `\X2\`, `\X4\`, escaped backslashes, and escaped apostrophes.
- `IfcToBosConverter` already calls `DecodeIfc()` in several visible output paths:
  entity labels, category names, property set names, property names, room numbers,
  and string property values.
- Some lower-level IFC accessors still return undecoded strings. `IfcEntity.GetString()`
  and `GetStringOrEmpty()` strip quotes only; `IfcPropData` stores raw property and
  property-set names from those accessors before the converter decodes them later.
- Prior tests cover a small `\X2\` escaped entity name and property value, but not the
  `\S\` shorthand used heavily by `schependomlaan.ifc`, and not every BOS string field.

### Working Hypotheses

1. **Most likely:** at least one BOS string path still uses `StepToken.ToString()`,
   `StepToken.AsString()`, `IfcEntity.GetString()`, or stored `IfcPropData` names
   without a final `DecodeIfc()` call.

2. **Likely:** decoding belongs in `IfcEntity` / IFC string helper APIs rather than in
   every converter call site. Most IFC consumers want decoded text, and centralizing it
   reduces the chance of missed fields.

3. **Needs care:** decoding probably should not happen inside the tokenizer's hot path.
   The parser currently stores compact raw byte spans; eagerly allocating decoded strings
   for every token could be expensive on large IFC files, especially when many tokens are
   geometry or numeric data that never become user-facing text.

4. **Open design question:** the generic STEP parser may need two APIs: one raw string
   accessor for exact STEP round-tripping / diagnostics, and one decoded accessor for
   normal semantic use. For IFC, the decoded accessor should probably be the default at
   the IFC-loader layer.

### Suggested Next Steps

1. Add a focused slow diagnostic/test for `schependomlaan.ifc` that scans the resulting
   BOS entity names, descriptor names/groups, and string parameter values for raw STEP
   escape markers like `\S\`, `\X\`, `\X2\`, and `\X4\`.

2. Add a small in-memory IFC test using `\S\` escapes, because current tests only prove
   a `\X2\` path.

3. Decide API ownership before broad fixes:
   - Keep `StepToken` raw and allocation-light.
   - Add or change IFC-layer accessors so callers can ask for decoded strings in one place.
   - Preserve an explicit raw accessor for diagnostics and exact STEP text.

4. After the failing field is identified, fix the lowest practical layer and remove
   redundant converter-level `DecodeIfc()` calls only if tests prove all BOS strings
   still decode correctly.

### Notes

This should be treated as a correctness issue, not a cosmetic cleanup. Raw IFC escape
codes in BOS make names and parameters harder to search, compare, and display, and can
hide duplicate values that should normalize to the same human-readable string.
