// Track B — pure ViewValue builders. No DOM, no three.js: importable from Node
// so the demo views can be tested and timed outside a browser.

import type { ModelData, ViewValue } from "../contracts";

const PHI = 0.618033988749895;

export function hslToRgb(h: number, s: number, l: number): [number, number, number] {
  const f = (n: number) => {
    const k = (n + h * 12) % 12;
    return l - s * Math.min(l, 1 - l) * Math.max(-1, Math.min(k - 3, 9 - k, 1));
  };
  return [f(0), f(8), f(4)];
}

/** Distinct-ish colours via a golden-ratio hue walk (same trick as the BOS level example). */
export function palette(n: number, seed = 0.1): [number, number, number][] {
  const out: [number, number, number][] = [];
  let h = seed;
  for (let i = 0; i < n; i++) {
    h = (h + PHI) % 1;
    out.push(hslToRgb(h, 0.62, 0.5));
  }
  return out;
}

export const cssRgb = (c: [number, number, number]) =>
  `rgb(${c.map((v) => Math.round(v * 255)).join(",")})`;

/** Everything whose type/category mentions "wall", in red, others ghosted. */
export function wallsView(model: ModelData): ViewValue {
  const picked: number[] = [];
  for (let i = 0; i < model.entityCount; i++) if (/wall/i.test(model.types[i])) picked.push(i);
  const entities = Uint32Array.from(picked);
  const colors = new Float32Array(entities.length * 3);
  for (let k = 0; k < entities.length; k++) {
    colors[k * 3] = 0.85;
    colors[k * 3 + 1] = 0.12;
    colors[k * 3 + 2] = 0.1;
  }
  return { model, entities, colors, ghostOthers: true, label: `walls (${entities.length})` };
}

/** example-bos-level-colors, recreated through the bridge API. */
export function levelView(model: ModelData): {
  view: ViewValue;
  legend: [string, string][];
} {
  const keys: string[] = [];
  const index = new Map<string, number>();
  for (const l of model.levels) {
    const k = l ?? "(no level)";
    if (!index.has(k)) {
      index.set(k, keys.length);
      keys.push(k);
    }
  }
  const colors3 = palette(keys.length);
  const entities = new Uint32Array(model.entityCount);
  const colors = new Float32Array(model.entityCount * 3);
  for (let i = 0; i < model.entityCount; i++) {
    entities[i] = i;
    const c = colors3[index.get(model.levels[i] ?? "(no level)")!];
    colors[i * 3] = c[0];
    colors[i * 3 + 1] = c[1];
    colors[i * 3 + 2] = c[2];
  }
  const legend = keys.map((k, i) => [k, cssRgb(colors3[i])] as [string, string]);
  return {
    view: { model, entities, colors, ghostOthers: false, label: `levels (${keys.length})` },
    legend,
  };
}
