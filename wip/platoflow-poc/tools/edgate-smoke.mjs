// edgate-smoke — the 5-check survivor of tools/edgate.mjs (plan §4.1, T7).
// Everything else edgate proved now lives in vitest (geom goldens, drive
// specs, jsdom params/editor-dom/chrome/tooltip suites). This gate proves ONLY
// what headless vitest cannot: a real Chrome boot, the DOM↔canvas coordinate
// seam, painted pixels, and TRUSTED input (untrusted synthetic PointerEvents
// make Gratify's setPointerCapture throw — NOTES wave 3).
//
//   1. harness boots on :5216 (a private vite is spawned if none is up)
//   2. a param popover opens over the canvas from a trusted click, positioned
//      over the row it edits (DOM↔canvas seam)
//   3. chart.bar paints accent pixels in its body
//   4. one trusted-input drag: a scrub on a number row edits the value
//      end-to-end (T1 landed mid-wave, so this slot is the scrub already;
//      it was the card-move drag before that — see plan §4.1)
//   5. no console errors
//
// No magic coordinates (§4.4): a tiny esbuild bundle of geom.ts + kinds.ts is
// evaluated in-page, so every click point comes from the SAME nodeLayout the
// app drew with, against the live doc.
import { spawn } from "node:child_process";
import { appendFileSync, mkdirSync, mkdtempSync, rmSync } from "node:fs";
import { hostname, tmpdir } from "node:os";
import { dirname, join, resolve } from "node:path";
import { fileURLToPath } from "node:url";
import { createRequire } from "node:module";

const T0 = Date.now();
const ROOT = resolve(dirname(fileURLToPath(import.meta.url)), "..");
const WEB = join(ROOT, "web");
const URL_UNDER_TEST = process.argv[2] ?? "http://localhost:5216/editor.html";
const PORT = 9200 + Math.floor(Math.random() * 700);
const CHROME = "C:/Program Files/Google/Chrome/Application/chrome.exe";

// ── private vite on :5216 (reuse one that is already up) ─────────────────────
const sleep = (ms) => new Promise((r) => setTimeout(r, ms));
const up = async (url) => { try { return (await fetch(url)).ok; } catch { return false; } };

let vite = null;
async function ensureHarness() {
  if (await up(URL_UNDER_TEST)) return;            // reuse a harness someone left up
  vite = spawn("npx.cmd", ["vite", "--port", "5216", "--strictPort"],
    { cwd: WEB, stdio: "ignore", shell: true });
  for (let i = 0; i < 80; i++) {
    if (await up(URL_UNDER_TEST)) return;
    await sleep(250);
  }
  throw new Error("private vite on :5216 never came up");
}

/** shell:true on Windows means vite is a grandchild — kill the whole tree. */
function killVite() {
  if (!vite) return;
  try { spawn("taskkill", ["/pid", String(vite.pid), "/T", "/F"], { stdio: "ignore" }); } catch {}
  vite = null;
}

// ── the layout bundle (geom + kinds, evaluated in-page) ──────────────────────
async function layoutBundle() {
  const esbuild = createRequire(join(WEB, "package.json"))("esbuild");
  const r = await esbuild.build({
    stdin: {
      contents: `export * from "./src/editor/geom";
                 export { KINDS, kindInfo } from "./src/kinds";
                 export { rect, v } from "gratify";`,
      resolveDir: WEB,
      loader: "ts",
    },
    alias: { gratify: resolve(ROOT, "../../../submodules/gratify/src/gratify") },
    bundle: true, format: "iife", globalName: "pfLayout", write: false,
  });
  return r.outputFiles[0].text;
}

// ── CDP scaffolding (same recipe as tools/intgate.mjs) ───────────────────────
const profile = mkdtempSync(join(tmpdir(), "edsmoke-"));
const chrome = spawn(CHROME, [
  "--headless=new", `--remote-debugging-port=${PORT}`, `--user-data-dir=${profile}`,
  "--window-size=1400,900", "--no-first-run", "--no-default-browser-check",
  "--enable-unsafe-swiftshader", "--disable-extensions", "about:blank",
], { stdio: "ignore" });

async function waitForDevtools() {
  for (let i = 0; i < 60; i++) {
    if (await up(`http://127.0.0.1:${PORT}/json/version`)) return;
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
      `"${ts}","${hostname()}","${ROOT.replaceAll("\\", "/")}","edgate-smoke.mjs","edgate-smoke","${result}",${seconds},"${detail}"\n`);
  } catch { /* timing log is best-effort */ }
}

async function main() {
  const [bundle] = await Promise.all([layoutBundle(), ensureHarness(), waitForDevtools()]);
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
  const waitFor = async (expr, timeoutMs = 15000, label = expr) => {
    const t0 = Date.now();
    while (Date.now() - t0 < timeoutMs) {
      try { if (await evaluate(expr)) return; } catch {}
      await sleep(120);
    }
    throw new Error(`timeout waiting for ${label}`);
  };
  const mouse = (type, x, y) =>
    cdp.send("Input.dispatchMouseEvent", { type, x, y, button: "left", buttons: type === "mousePressed" ? 1 : 0, clickCount: 1, pointerType: "mouse" });
  const click = async (x, y) => { await mouse("mouseMoved", x, y); await mouse("mousePressed", x, y); await mouse("mouseReleased", x, y); await sleep(150); };

  // ── 1. boot ────────────────────────────────────────────────────────────────
  await waitFor("!!window.editorApp && !!window.gratify", 20000, "harness boot");
  await sleep(600);                                // starter graph + first fake eval
  await evaluate(bundle);                          // pfLayout: geom + kinds in-page
  // In-page helpers: EVERYTHING geometric goes through the live doc + nodeLayout.
  await evaluate(`window.__pf = (() => {
    const L = window.pfLayout;
    const layoutOf = (id) => {
      const doc = window.editorApp.getDoc();
      const n = doc.nodes.find((x) => x.id === id);
      const info = L.kindInfo(n.kind);
      const l = L.nodeLayout(info,
        { params: n.params, wiredInputs: L.wiredInputsOf(doc.edges, id, info) },
        { helpOpen: !!window.gratify.doc.helpOpen[id], zoom: 1 },
        window.gratify.doc.status[id]);
      return { n, info, l, card: L.rect(n.x, n.y, l.w, l.h) };
    };
    const b = () => document.getElementById("graph-canvas").getBoundingClientRect();
    const page = (p) => ({ x: b().left + p.x, y: b().top + p.y });   // zoom 1, pan 0 at boot
    return {
      paramRow(id, name) {
        const { l, card } = layoutOf(id);
        const r = L.paramRowRect(l, card, l.paramNames.indexOf(name));
        return { c: page(r.center), x: b().left + r.x, y: b().top + r.y, w: r.w, h: r.h };
      },
      body(id) {
        const { l, card } = layoutOf(id);
        const r = L.bodyRect(l, card);
        return { x: b().left + r.x, y: b().top + r.y, w: r.w, h: r.h };
      },
    };
  })(), 'ok'`);
  check("boot: harness + in-page layout bundle up", true);

  // ── 2. DOM editor over the canvas (DOM↔canvas seam). modelPick routes to
  // the T8 searchable picker since W2 — same seam, different organ. ─────────
  const row = await evaluate(`JSON.stringify(window.__pf.paramRow('load1','model'))`).then(JSON.parse);
  await click(row.c.x, row.c.y);
  await waitFor("!!document.querySelector('.pf-picker')", 4000, "picker open");
  const pop = await evaluate(`(() => {
    const p = document.querySelector('.pf-picker');
    const r = p.getBoundingClientRect();
    return { hasFilter: !!p.querySelector('input'), rows: p.querySelectorAll('[data-value],li,button').length, left: r.left, top: r.top };
  })()`);
  const near = Math.abs(pop.left - row.x) <= 80 && Math.abs(pop.top - row.y) <= 80;
  check("picker opens over the clicked row (trusted click, layout-derived coords)",
    pop.hasFilter && near, JSON.stringify({ pop, row: { x: row.x, y: row.y } }));
  await click(900, 700);                           // close it again

  // ── 3. chart.bar paints accent pixels ─────────────────────────────────────
  const body = await evaluate(`JSON.stringify(window.__pf.body('chart1'))`).then(JSON.parse);
  const accents = await evaluate(`(() => {
    const cv = document.getElementById('graph-canvas');
    const ctx = cv.getContext('2d');
    const dpr = window.devicePixelRatio || 1;
    const img = ctx.getImageData(${body.x} * dpr, ${body.y} * dpr, ${body.w} * dpr, ${body.h} * dpr);
    let colored = 0;
    for (let i = 0; i < img.data.length; i += 4) {
      const r = img.data[i], g = img.data[i + 1], bl = img.data[i + 2];
      if (r > 120 && r > g + 40 && r > bl + 40) colored++;   // sink accent #ff7064
    }
    return colored;
  })()`);
  check("chart.bar paints accent pixels in its body", accents > 300, `${accents} accent px`);

  // ── 4. trusted-input drag: scrub a number row (T1) ────────────────────────
  // Was the card-move drag until T1 landed; now the scrub IS the drag survivor
  // (§4.1). cmap1.min starts at 0; SCRUB_PX_PER_STEP=2 → +40px = +20.
  const before = await evaluate("window.editorApp.getDoc().nodes.find(n=>n.id==='cmap1').params.min");
  const srow = await evaluate(`JSON.stringify(window.__pf.paramRow('cmap1','min'))`).then(JSON.parse);
  await mouse("mouseMoved", srow.c.x, srow.c.y);
  await mouse("mousePressed", srow.c.x, srow.c.y);
  for (let i = 1; i <= 8; i++) { await mouse("mouseMoved", srow.c.x + (40 * i) / 8, srow.c.y); await sleep(20); }
  await mouse("mouseReleased", srow.c.x + 40, srow.c.y);
  await sleep(250);
  const after = await evaluate("window.editorApp.getDoc().nodes.find(n=>n.id==='cmap1').params.min");
  const cardStill = await evaluate("(() => { const n = window.editorApp.getDoc().nodes.find(n=>n.id==='cmap1'); return n.x === 320 && n.y === 260; })()");
  check("trusted drag scrubs the value (and does not move the card)",
    before === 0 && after === 20 && cardStill, `min ${before} → ${after}, cardStill=${cardStill}`);

  // ── 5. console hygiene ────────────────────────────────────────────────────
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
  killVite();
  try { rmSync(profile, { recursive: true, force: true }); } catch {}
  process.exit(failed.length ? 1 : 0);
}

main().catch((e) => {
  console.error("GATE ERROR", e);
  recordTiming("ERROR", ((Date.now() - T0) / 1000).toFixed(1), String(e).slice(0, 120));
  chrome.kill();
  killVite();
  process.exit(1);
});
