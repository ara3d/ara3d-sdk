# Technical Debt Log

A single, scannable list of known shortcuts and areas for improvement, so they can be
planned and tackled deliberately instead of all at once.

**How to use this file** (see [`../AGENTS.md`](../AGENTS.md) §6):

- When you take a shortcut or spot an improvement that is out of scope, add a `// TODO:`
  marker in the code **and** an entry here.
- Each entry: where (file/area), what, and why it matters. Keep it specific and actionable.
- Remove an entry when the debt is paid off.

---

## Open items

| Area / file | What | Why it matters |
| --- | --- | --- |
| _(example)_ `src/Ara3D.IO.Foo/FooReader.cs` | Re-reads the file on every call | Cache the buffer; avoids repeated I/O on hot path |

_No tracked items yet. Add rows above as debt is introduced._

---

## Resolved

Move items here (or delete them) once addressed, with a short note on the resolution.
