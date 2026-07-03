# NuGet release

Packages build to `artifacts/` (gitignored). Version is `Ara3DVersion` in `Directory.Build.props`.
`pack.bat` packs every project listed in `build/packages.txt`.

## Release commands

```bat
release-nuget.bat patch smoke    rem bump, build, test, pack, integration tests
release-nuget.bat finish         rem commit version, tag, push to nuget.org
```

Or step by step:

```bat
bump-version.bat patch
publish-nuget.bat smoke
publish-nuget.bat push-only
```

`release-nuget.bat patch` runs smoke + finish in one go. Use `patch smoke` to stop before publish.

Other modes: `publish-nuget.bat smoke-only`, `RELEASE_NUGET_NO_GIT=1`, `test.bat nuget`.

## Prerequisites for publish

Set environment variables (or rely on defaults in `publish-nuget.bat` on your machine):

- `NUGET_EXE`
- `NUGET_API_KEY_FILE` — first line is the API key; never commit this file

Review per-project `<Version>` overrides (e.g. `src\Ara3D.BimOpenSchema.IO`) before publishing.

Meta-packages: `Ara3D.SDK` (full Windows stack), `Ara3D.SDK.Core`, `Ara3D.SDK.Geometry`,
`Ara3D.SDK.IO`. See [`src/README.md`](../src/README.md).

## Integration tests

`tests/Ara3D.SDK.NuGet.Tests` restores `Ara3D.SDK`, `Ara3D.SDK.Core`, and `Ara3D.Collections` from `artifacts/` only.
Run via `test.bat nuget` or as part of `publish-nuget.bat smoke`.
