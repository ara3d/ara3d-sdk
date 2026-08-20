// Minimal RFC4180-ish CSV reader: quoted fields with "" escapes, CRLF or LF, ragged rows padded.
// Typing is PER COLUMN, not per cell (wave 9): a column is numeric iff every non-empty
// cell looks numeric AND the header is not id-like; quoting never decides the type.
import type { Cell, TableValue } from "../contracts";

const NUMERIC = /^[+-]?(\d+\.?\d*|\.\d+)([eE][+-]?\d+)?$/;

export const numericish = (s: string): boolean => NUMERIC.test(s.trim());

/** Headers that name identifiers: their numeric-looking values ("007", GlobalId-ish
 *  codes) must survive as strings — an id is a key, not a quantity. */
const ID_LIKE = /(^|_|\s)(id|gid|guid|globalid)$/i;

/** Split CSV text into rows of {text, quoted} fields. */
function scan(text: string): { text: string; quoted: boolean }[][] {
  const rows: { text: string; quoted: boolean }[][] = [];
  let row: { text: string; quoted: boolean }[] = [];
  let field = "";
  let quoted = false;
  let inQuotes = false;
  let i = 0;
  const push = () => { row.push({ text: field, quoted }); field = ""; quoted = false; };
  const endRow = () => { push(); rows.push(row); row = []; };

  while (i < text.length) {
    const c = text[i];
    if (inQuotes) {
      if (c === '"') {
        if (text[i + 1] === '"') { field += '"'; i += 2; continue; }
        inQuotes = false; i++; continue;
      }
      field += c; i++; continue;
    }
    if (c === '"') { inQuotes = true; quoted = true; i++; continue; }
    if (c === ",") { push(); i++; continue; }
    if (c === "\r") { if (text[i + 1] === "\n") i++; endRow(); i++; continue; }
    if (c === "\n") { endRow(); i++; continue; }
    field += c; i++;
  }
  if (field.length > 0 || quoted || row.length > 0) endRow();
  return rows;
}

export function parseCsv(text: string): TableValue {
  const clean = text.replace(/^﻿/, "");
  const raw = scan(clean).filter(r => !(r.length === 1 && !r[0].quoted && r[0].text.trim() === ""));
  if (raw.length === 0) return { columns: [], rows: [] };
  const columns = raw[0].map((f, i) => (f.text.trim() || `col${i + 1}`));

  // Pass 1: raw text cells, padded to the header width. Empty cells are null either way.
  const cells: (string | null)[][] = raw.slice(1).map(r => {
    const out: (string | null)[] = new Array(columns.length).fill(null);
    for (let i = 0; i < columns.length; i++) {
      const f = r[i];
      out[i] = f === undefined || f.text === "" ? null : f.text;
    }
    return out;
  });

  // Pass 2: column-level type inference. Numeric iff the column has data, EVERY
  // non-empty cell is numeric-looking, and the header is not id-like.
  const numericCol = columns.map((name, c) => {
    if (ID_LIKE.test(name)) return false;
    let filled = 0;
    for (const r of cells) {
      const t = r[c];
      if (t === null) continue;
      filled++;
      if (!numericish(t)) return false;
    }
    return filled > 0;
  });

  const rows: Cell[][] = cells.map(r =>
    r.map((t, c) => (t === null ? null : numericCol[c] ? Number(t.trim()) : t)));
  return { columns, rows };
}
