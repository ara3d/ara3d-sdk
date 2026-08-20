// Branding theme (2026-08-19): warm-neutral "cream" — minimal, flat,
// Notion/Linear-adjacent. Single source of truth for the CANVAS side of the
// palette (gratify tokens). The DOM side lives as --pf-* CSS custom properties
// declared in each html page's <style> (index/editor/viewer harnesses), with
// the same values. Hex targets came from a screenshot spec — approximate on
// purpose, not exact brand tokens.
//
// Import this module (side-effect) BEFORE mounting any gratify island.
import { rgb, setTheme, themes, tokens, type Color } from "gratify";

const cream = {
  bg: rgb(239, 237, 232),        // node-editor canvas (#EFEDE8) — darker than chrome so white cards stand out
  surface: rgb(255, 255, 255),   // node cards / panels
  surfaceHi: rgb(243, 241, 236), // hovered surface
  muted: rgb(213, 210, 203),     // separators, quiet edges (visible on white)
  text: rgb(45, 44, 40),
  textDim: rgb(138, 136, 128),   // #8A8880 — labels, counts
  textBright: rgb(26, 26, 24),   // #1A1A18 — near-black warm charcoal
  accent: rgb(26, 26, 24),       // monochrome accent: selection = charcoal outline
  accent2: rgb(138, 136, 128),
  danger: rgb(192, 57, 43),
};

themes.cream = cream;
setTheme("cream");

// setTheme only retargets — live tokens would fade from dark over ~1 s at
// boot. Snap them so the first frame is already cream.
for (const k of Object.keys(cream) as (keyof typeof cream)[]) {
  const cur = tokens[k] as Color, want = cream[k];
  cur.r = want.r; cur.g = want.g; cur.b = want.b; cur.a = want.a;
}
