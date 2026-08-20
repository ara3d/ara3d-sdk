// Re-export shim. parts.ts was split in W0 (plan §2.5) so parallel tracks stop
// colliding in one file — the real parts now live in:
//   cards.ts    NodeCard + chips + paramDisplay      (T1/T5/T9/T10)
//   widgets.ts  in-node widget parts (W0: scaffold)  (T1, T5)
//   wires.ts    WirePart, RubberWire                 (T3, T11)
//   surface.ts  Surface, HUD, makeView               (T3, T13)
//   palette.ts  palette parts                        (T3)
// Existing imports (index.ts, harness.ts, tests) keep working through here.
export * from "./cards";
export * from "./widgets";
export * from "./wires";
export * from "./surface";
export * from "./palette";
