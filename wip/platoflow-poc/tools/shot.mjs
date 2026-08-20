import { spawn } from "node:child_process";
import { mkdtempSync, rmSync, writeFileSync } from "node:fs";
import { tmpdir } from "node:os";
import { join } from "node:path";

const OUT = process.argv[2] ?? "poc.png";
const PORT = 9000 + Math.floor(Math.random() * 900);
const CHROME = "C:/Program Files/Google/Chrome/Application/chrome.exe";
const profile = mkdtempSync(join(tmpdir(), "shot-"));
const chrome = spawn(CHROME, ["--headless=new", `--remote-debugging-port=${PORT}`, `--user-data-dir=${profile}`,
  "--window-size=1600,950", "--no-first-run", "--enable-unsafe-swiftshader", "about:blank"], { stdio: "ignore" });
const sleep = (ms) => new Promise((r) => setTimeout(r, ms));
for (let i = 0; i < 60; i++) { try { if ((await fetch(`http://127.0.0.1:${PORT}/json/version`)).ok) break; } catch {} await sleep(250); }
const tab = await (await fetch(`http://127.0.0.1:${PORT}/json/new`, { method: "PUT" })).json();
const ws = new WebSocket(tab.webSocketDebuggerUrl);
const pending = new Map();
let nextId = 1;
ws.addEventListener("message", (ev) => {
  const m = JSON.parse(ev.data);
  if (m.id && pending.has(m.id)) { pending.get(m.id)(m.result); pending.delete(m.id); }
});
await new Promise((r) => ws.addEventListener("open", r));
const send = (method, params = {}) => new Promise((res) => { const id = nextId++; pending.set(id, res); ws.send(JSON.stringify({ id, method, params })); });
await send("Runtime.enable"); await send("Page.enable");
const ev = async (expression) => (await send("Runtime.evaluate", { expression, returnByValue: true, awaitPromise: true })).result?.value;
const waitFor = async (expr, ms = 60000) => { const t0 = Date.now(); while (Date.now() - t0 < ms) { if (await ev(expr)) return true; await sleep(150); } return false; };

await send("Page.navigate", { url: "http://localhost:5215/" });
await waitFor("!!window.poc");
await ev(`(() => {
  document.querySelector('.pf-menu-trigger')?.click();
  [...document.querySelectorAll('.pf-menu-list button')].find(b=>b.textContent==='carbon-walls')?.click();
  return 'ok';
})()`);
await waitFor(`window.poc.doc?.name==='carbon-walls' && window.poc.result?.status.get('n6')?.state==='ok'`);
// (palette panel is open by default now — no 'p' keypress needed)
await sleep(1200);
await ev(`(() => { const v = window.poc.viewer.viewer; v.renderer.needsUpdate = true; v.renderer.render(); return 'ok'; })()`);
const shot = await send("Page.captureScreenshot", { format: "png" });
writeFileSync(OUT, Buffer.from(shot.data, "base64"));
console.log("saved", OUT);
chrome.kill();
try { rmSync(profile, { recursive: true, force: true }); } catch {}
process.exit(0);
