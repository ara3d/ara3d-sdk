# SYNCED COPY — do not edit here

The source of truth for `Plato.Intrinsics` is the Plato repository:
`submodules/Plato/Plato.Intrinsics` (studio monorepo path).

This folder is a synced copy kept so the Ara3D SDK builds standalone without
the Plato toolchain (same pattern as `Plato.Generated`). It is synced and
diff-gated by `tools/regen-plato.ps1` (studio repo):

- default mode: diffs this copy against the Plato-repo source of truth and
  exits 1 on drift;
- `-Apply`: copies Plato-repo → here.

Make changes in the Plato repo, then run `tools\regen-plato.ps1 -Apply`.
