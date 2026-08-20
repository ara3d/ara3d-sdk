// intgate-smoke — the ~13-check survivor of tools/intgate.mjs (plan §4.1, T7).
// FINAL integration check only; no track's inner loop runs it. Everything the
// fat gate proved about evaluator output now lives in vitest (demos.test.ts,
// demo-charts.spec.ts, wave2 suite); the seven editor: checks live in the
// jsdom/headless editor suites. What remains is what only the real stack can
// prove: the 3D pane paints, each HOST-touching feature round-trips once
// (DuckDB SQL, pset write, MCP intents, Ask AI), and the app boots clean.
//
// Requires BOTH servers: host on :5214 (`dotnet run --project host`) and vite
// on :5215 (`npm run dev` in web/). Never the shared Browser pane (AGENTS.md).
//
// Wait rule (§4.4): node ids repeat across demos (n1..nN), so every wait keys
// on data only the NEW state uniquely produces — a demo-unique summary string,
// a demo-unique grid column, or an MCP-returned fresh node id.
import { spawn } from "node:child_process";
import { appendFileSync, mkdirSync, mkdtempSync, rmSync } from "node:fs";
import { hostname, tmpdir } from "node:os";
import { dirname, join, resolve } from "node:path";
import { fileURLToPath } from "node:url";

const T0 = Date.now();
const ROOT = resolve(dirname(fileURLToPath(import.meta.url)), "..");
const URL_UNDER_TEST = process.argv[2] ?? "http://localhost:5215/";
const HOST = "http://127.0.0.1:5214";
const PORT = 9000 + Math.floor(Math.random() * 900);
const CHROME = "C:/Program Files/Google/Chrome/Application/chrome.exe";

const sleep = (ms) => new Promise((r) => setTimeout(r, ms));

const profile = mkdtempSync(join(tmpdir(), "intsmoke-"));
const chrome = spawn(CHROME, [
  "--headless=new", `--remote-debugging-port=${PORT}`, `--user-data-dir=${profile}`,
  "--window-size=1400,900", "--no-first-run", "--no-default-browser-check",
  "--enable-unsafe-swiftshader", "--disable-extensions", "about:blank",
], { stdio: "ignore" });

async function waitForDevtools() {
  for (let i = 0; i < 60; i++) {
    try { const r = await fetch(`http://127.0.0.1:${PORT}/json/version`); if (r.ok) return; } catch {}
    await sleep(250);
  }
  throw new Error("devtools never came up");
}

let nextId = 1;
function connect(wsUrl) {
  const ws = new WebSocket(wsUrl);
  const pending = new Map();
  const errors = [];
  ws.addEventListener("message", (ev) => {
    const msg = JSON.parse(ev.data);
    if (msg.id && pending.has(msg.id)) {
      const { resolve, reject } = pending.get(msg.id);
      pending.delete(msg.id);
      msg.error ? reject(new Error(JSON.stringify(msg.error))) : resolve(msg.result);
      return;
    }
    if (msg.method === "Runtime.consoleAPICalled" && msg.params.type === "error")
      errors.push(msg.params.args.map((a) => a.value ?? a.description ?? a.type).join(" "));
    if (msg.method === "Runtime.exceptionThrown")
      errors.push(msg.params.exceptionDetails.exception?.description ?? msg.params.exceptionDetails.text);
  });
  const ready = new Promise((res, rej) => { ws.addEventListener("open", res); ws.addEventListener("error", rej); });
  const send = (method, params = {}) => new Promise((resolve, reject) => {
    const id = nextId++;
    pending.set(id, { resolve, reject });
    ws.send(JSON.stringify({ id, method, params }));
  });
  return { ws, send, ready, errors };
}

async function mcp(name, args) {
  const r = await (await fetch(`${HOST}/mcp`, {
    method: "POST", headers: { "content-type": "application/json" },
    body: JSON.stringify({ jsonrpc: "2.0", id: nextId++, method: "tools/call", params: { name, arguments: args } }),
  })).json();
  return r.result ?? r;
}

const results = [];
const check = (name, cond, extra = "") => {
  results.push(`${cond ? "PASS" : "FAIL"} ${name}${extra ? " — " + extra : ""}`);
  console.log(results[results.length - 1]);
};

function recordTiming(result, seconds, detail) {
  try {
    const dir = join(process.env.LOCALAPPDATA ?? tmpdir(), "ara3d");
    mkdirSync(dir, { recursive: true });
    const ts = new Date().toISOString().slice(0, 19);
    appendFileSync(join(dir, "gate-timings.csv"),
      `"${ts}","${hostname()}","${ROOT.replaceAll("\\", "/")}","intgate-smoke.mjs","intgate-smoke","${result}",${seconds},"${detail}"\n`);
  } catch { /* best-effort */ }
}

async function main() {
  await waitForDevtools();
  const tab = await (await fetch(`http://127.0.0.1:${PORT}/json/new?${encodeURIComponent(URL_UNDER_TEST)}`, { method: "PUT" })).json();
  const cdp = connect(tab.webSocketDebuggerUrl);
  await cdp.ready;
  await cdp.send("Runtime.enable");
  await cdp.send("Page.enable");

  const evaluate = async (expression) => {
    const r = await cdp.send("Runtime.evaluate", { expression, returnByValue: true, awaitPromise: true, allowUnsafeEvalBlockedByCSP: true });
    if (r.exceptionDetails) throw new Error(r.exceptionDetails.exception?.description ?? r.exceptionDetails.text);
    return r.result.value;
  };
  const waitFor = async (expr, timeoutMs = 30000, label = expr) => {
    const t0 = Date.now();
    while (Date.now() - t0 < timeoutMs) {
      try { if (await evaluate(expr)) return Date.now() - t0; } catch {}
      await sleep(120);
    }
    throw new Error(`timeout waiting for ${label}`);
  };
  const statusOf = async (id) => JSON.parse(await evaluate(
    `JSON.stringify((window.poc?.result?.status?.get(${JSON.stringify(id)})) ?? null)`));

  /** Load a demo via the menu; the READY expression must be uniquely true for
   *  the new demo's eval result (§4.4 — never a bare "sink ok"). */
  const loadDemo = async (name, readyExpr, label) => {
    // W9-E: the Examples menu rebuilds ASYNC from /api/graphs each time it
    // opens, so the item is not there on the first query — open the menu and
    // poll until the entry materializes, then click it.
    await waitFor(`(() => {
      const list = document.querySelector('.pf-menu-list');
      if (!list) { document.querySelector('.pf-menu-trigger')?.click(); return false; }
      const item = [...list.querySelectorAll('button')].find(b => b.textContent === ${JSON.stringify(name)});
      if (!item) return false;
      item.click();
      return true;
    })()`, 15000, `menu item ${name}`);
    await waitFor(`window.poc.doc?.name === ${JSON.stringify(name)} && (${readyExpr})`, 60000, label);
    await sleep(500);                              // routeOutputs settles DOM + viewer
  };

  // Viewer pixel sampling (non-background pixels; recipe from intgate.mjs).
  const SAMPLE = `(() => {
    try {
      const v = window.poc.viewer.viewer;
      v.renderer.needsUpdate = true;
      v.renderer.render();
      const gl = v.renderer.renderer.getContext();
      const w = gl.drawingBufferWidth, h = gl.drawingBufferHeight;
      const buf = new Uint8Array(w * h * 4);
      gl.readPixels(0, 0, w, h, gl.RGBA, gl.UNSIGNED_BYTE, buf);
      let model = 0, r = 0, g = 0, b = 0;
      for (let i = 0; i < w * h; i++) {
        const R = buf[i*4], G = buf[i*4+1], B = buf[i*4+2];
        if (Math.abs(R-150) <= 8 && Math.abs(G-153) <= 8 && Math.abs(B-159) <= 8) continue;
        model++; r += R; g += G; b += B;
      }
      const d = model || 1;
      return JSON.stringify({ model, r: Math.round(r/d), g: Math.round(g/d), b: Math.round(b/d) });
    } catch (e) { return JSON.stringify({ model: -1, err: String(e) }); }
  })()`;
  const sample = async () => JSON.parse(await evaluate(SAMPLE));
  const dist = (a, b) => Math.abs(a.r - b.r) + Math.abs(a.g - b.g) + Math.abs(a.b - b.b);

  // ── 1. boot ────────────────────────────────────────────────────────────────
  await waitFor("!!window.poc", 30000, "window.poc");
  const bootStatus = await evaluate("document.getElementById('status').textContent");
  check("boot: app ready", /ready/.test(bootStatus), bootStatus);

  // ── 2-5. beat 1: carbon-walls (join + colorBy + pixels + legend) ──────────
  // Unique key: the n4 join summary ("matched N") — the boot doc has no n4.
  await loadDemo("carbon-walls",
    `/matched [1-9]/.test(window.poc.result?.status?.get('n4')?.summary ?? '')
       && window.poc.result?.status?.get('n6')?.state === 'ok'`,
    "carbon-walls join+colorBy ready");
  const n4 = await statusOf("n4");
  check("beat1: join matched", /matched [1-9]/.test(n4?.summary ?? ""), n4?.summary);
  check("beat1: colorBy ok", (await statusOf("n6"))?.state === "ok");
  await sleep(600);
  const beat1Px = await sample();
  check("beat1: model visible + colored", beat1Px.model > 3000, JSON.stringify(beat1Px));
  const legend = await evaluate("document.getElementById('legend').textContent");
  check("beat1: legend shows a numeric domain", /\d/.test(legend ?? ""), String(legend).slice(0, 60));

  // ── 6. scrub → pixels timing ──────────────────────────────────────────────
  const before = await sample();
  const t0 = Date.now();
  await evaluate("window.poc.editor.dispatch({t:'setParam', node:'n5', name:'max', value: 260}), 'ok'");
  let scrubMs = -1;
  for (let i = 0; i < 60; i++) {
    await sleep(50);
    if (dist(await sample(), before) > 6) { scrubMs = Date.now() - t0; break; }
  }
  check("scrub: param change recolors the model", scrubMs >= 0,
    scrubMs >= 0 ? `${scrubMs}ms param→pixels` : "no pixel change");

  // ── 7. SQL against host DuckDB (sql-explore) ──────────────────────────────
  // Unique key: the grid grows a "count" column — no earlier state has one.
  await loadDemo("sql-explore",
    `[...document.querySelectorAll('#grid thead th')].some(t => t.textContent === 'count')`,
    "sql-explore count column in grid");
  const sqlRows = await evaluate("document.querySelectorAll('#grid tbody tr').length");
  check("host: SQL node returns rows from DuckDB", sqlRows >= 10, `${sqlRows} rows`);

  // ── 8. pset write path, dry check (write-pset) ────────────────────────────
  // Unique key: sink.writePset's summary ("ready: N entities × channels […]")
  // — no other demo produces it. Dry: the write itself is NOT run (the fat
  // gate ran it; the host smoke owns the actual /api/append-psets write).
  await loadDemo("write-pset",
    `/ready: \\d+ entities × channels \\[operational_carbon\\]/.test(window.poc.result?.status?.get('n5')?.summary ?? '')`,
    "write-pset sink ready");
  const pset = await statusOf("n5");
  check("host: write-pset pipeline ready (CSV joined, rows staged)",
    pset?.state === "ok" && /ready: [1-9]\d* entities/.test(pset?.summary ?? ""), pset?.summary);

  // ── 9-10. MCP: add_node lands in the live editor; get_graph sees it ───────
  // Unique key: the id the MCP tool returns is fresh — waiting on it can never
  // match stale state.
  const addRes = await mcp("add_node", { kind: "viz.colormap", x: 220, y: 520 });
  // MCP tool results nest JSON in content[0].text.
  let newId = null;
  try { newId = JSON.parse(addRes.content?.[0]?.text ?? "{}").id ?? null; } catch {}
  check("mcp: add_node returned a node id", !!newId, JSON.stringify(addRes).slice(0, 120));
  await waitFor(`window.poc.doc.nodes.some(n => n.id === ${JSON.stringify(newId)})`, 10000, "MCP node in live doc");
  check("mcp: added node appears in the live editor", true);
  // The browser pushes state to the host debounced after eval — poll briefly.
  let graphSeen = false;
  for (let i = 0; i < 25 && !graphSeen; i++) {
    graphSeen = JSON.stringify(await mcp("get_graph", {})).includes(`\\"${newId}\\"`)
      || JSON.stringify(await mcp("get_graph", {})).includes(`"${newId}"`);
    if (!graphSeen) await sleep(200);
  }
  check("mcp: get_graph sees the pushed state", graphSeen, `id ${newId}`);

  // ── 11. Ask AI, key-tolerant ──────────────────────────────────────────────
  // Passes on a real SQL answer OR the documented no-key error; anything else
  // (bad model, crash) fails.
  const ask = await (await fetch(`${HOST}/api/ask`, {
    method: "POST", headers: { "content-type": "application/json" },
    body: JSON.stringify({ model: "duplex", question: "How many entities are there per category?" }),
  })).json();
  const askOk = typeof ask.sql === "string" && /select/i.test(ask.sql);
  const askNoKey = /ANTHROPIC_API_KEY/i.test(ask.error ?? "");
  check("host: Ask AI answers (or reports the no-key error)", askOk || askNoKey,
    askOk ? ask.sql.slice(0, 60) : ask.error);

  // ── 12. console hygiene ───────────────────────────────────────────────────
  const noisy = cdp.errors.filter((e) => !/favicon/i.test(e));
  check("no console errors", noisy.length === 0, noisy.slice(0, 3).join(" | "));

  finish(cdp);
}

function finish(cdp) {
  const secs = ((Date.now() - T0) / 1000).toFixed(1);
  const failed = results.filter((r) => r.startsWith("FAIL"));
  console.log("\n--- results ---");
  console.log(`${results.length - failed.length} passed, ${failed.length} failed in ${secs}s`);
  recordTiming(failed.length ? "FAIL" : "PASS", secs, `${results.length - failed.length}/${results.length} checks`);
  try { cdp?.ws.close(); } catch {}
  chrome.kill();
  try { rmSync(profile, { recursive: true, force: true }); } catch {}
  process.exit(failed.length ? 1 : 0);
}

main().catch((e) => {
  console.error("GATE ERROR", e);
  recordTiming("ERROR", ((Date.now() - T0) / 1000).toFixed(1), String(e).slice(0, 120));
  chrome.kill();
  process.exit(1);
});
