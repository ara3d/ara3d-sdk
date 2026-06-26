# NuGet packaging and release

This document describes how Ara3D SDK NuGet packages are versioned, built, tested, and published.

## Overview

| Path / script | Purpose |
| --- | --- |
| `artifacts/` | Build output folder for `.nupkg` files (gitignored) |
| `Directory.Build.props` | Central `Ara3DVersion` used by all packable projects |
| `build/packages.txt` | Manifest of projects packed by `pack.bat` |
| `bump-version.bat` | Increment or set `Ara3DVersion` |
| `pack.bat` | Pack every project in `build/packages.txt` |
| `release.bat` | Build, run scoped tests, then pack |
| `publish-nuget.bat` | Full publish, push-only, or smoke-test the release pipeline |
| `release-nuget.bat` | End-to-end release: bump, smoke, commit, tag, publish |
| `tests/Ara3D.SDK.NuGet.Tests/` | Integration tests that restore packages from `artifacts/` |

Packages are **not** committed to git. Publish to [nuget.org](https://www.nuget.org) (or another feed) from `artifacts/`.

## Version numbers

All projects inherit version metadata from the repo root `Directory.Build.props`:

```xml
<Ara3DVersion>1.6.0</Ara3DVersion>
<Version Condition="'$(Version)' == ''">$(Ara3DVersion)</Version>
```

Bump the shared version before a release:

```bat
bump-version.bat patch       rem 1.6.0 -> 1.6.1
bump-version.bat minor       rem 1.6.0 -> 1.7.0
bump-version.bat major       rem 1.6.0 -> 2.0.0
bump-version.bat 1.7.0       rem explicit version
```

Some projects override `<Version>` locally (for example `ext\Ara3D.BimOpenSchema.IO`). Review those overrides before publishing.

## Building packages

Pack every published project (Release by default):

```bat
pack.bat
```

`pack.bat` reads `build/packages.txt` and runs `dotnet pack` for each listed `.csproj`. Output lands in `artifacts/`.

`release.bat` is the pre-publish gate: it builds key surfaces, runs the normal test areas (`sdk`, `geometry`, `bim`, `devtools`), then calls `pack.bat`.

## NuGet integration tests

`tests/Ara3D.SDK.NuGet.Tests` is a separate test project that:

- Uses `NuGet.config` to restore **only** from `artifacts/` (not nuget.org)
- References `Ara3D.SDK` and `Ara3D.Collections` at `$(Ara3DVersion)`
- Verifies required `.nupkg` files exist and that restored assemblies load

Run after packing:

```bat
test.bat nuget
```

These tests are tagged `Category("Slow")` because they require packaged output in `artifacts/`. They are skipped by `test.bat fast`.

## Publishing

### Smoke test (recommended before every real publish)

Runs the full build/test/pack pipeline and NuGet integration tests. **Does not push** to nuget.org and **does not require** an API key:

```bat
publish-nuget.bat smoke
```

If packages are already in `artifacts/` and you only want to re-run integration tests:

```bat
publish-nuget.bat smoke-only
```

### Publish to nuget.org

Prerequisites (override with environment variables if needed):

- `NUGET_EXE` — defaults to `C:\Users\cdigg\git\studio\devops\nuget.exe`
- `NUGET_API_KEY_FILE` — defaults to `C:\dev\keys\nuget.txt` (first line is the API key; never commit this file)

Full release (build, test, pack, push):

```bat
publish-nuget.bat
```

Push packages that are already built:

```bat
publish-nuget.bat push-only
```

The script reads the API key from disk at push time, uses `-SkipDuplicate` for idempotent re-runs, and clears the key from the environment when finished.

## Recommended release process

Use `release-nuget.bat` for the full flow, or run the steps manually.

### Automated (`release-nuget.bat`)

Full release (bump, smoke test, git commit/tag, push to nuget.org):

```bat
release-nuget.bat patch
release-nuget.bat minor
release-nuget.bat major
release-nuget.bat 1.7.0
```

Bump and smoke test only — stop before commit/publish so you can review:

```bat
release-nuget.bat patch smoke
```

After a successful smoke run, commit, tag, and publish:

```bat
release-nuget.bat finish
```

Set `RELEASE_NUGET_NO_GIT=1` to skip git commit and tag steps.

### Manual steps

1. **Bump version** — `bump-version.bat patch` (or `minor` / `major` / explicit version)
2. **Review overrides** — check projects with a local `<Version>` element
3. **Smoke test** — `publish-nuget.bat smoke`
4. **Commit and tag** — e.g. `git tag -a v1.6.1 -m "Release 1.6.1"`
5. **Publish** — `publish-nuget.bat push-only` (if smoke already packed)
6. **Verify** — confirm packages on nuget.org

For day-to-day development, keep using `test.bat` / `test.bat fast`. NuGet integration tests are part of the release path, not the normal inner loop.

## Local feed (optional)

`Ara3D.SDK.csproj` can copy packed output to `custom/piping-labs/local-nuget-source` when that folder exists on your machine. That is a developer convenience; `artifacts/` remains the canonical pack output.
