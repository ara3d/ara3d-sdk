# Command cheat sheet

| Task | Command |
| --- | --- |
| Build | `build.bat` |
| Fast tests | `test.bat <area> fast` |
| Full tests | `test.bat` |
| Commit (no push) | `save.bat "message"` |
| Pack | `pack.bat` |
| NuGet dry-run | `release-nuget.bat patch smoke` |
| NuGet publish | `release-nuget.bat finish` |

**Inner loop:** change → `build.bat` → `test.bat <area> fast` → full `test.bat` before done.

**Opt-in tests:** `test.bat knownissues`, `test.bat nuget` (after pack).

Coding rules and test areas: [`AGENTS.md`](../AGENTS.md).  
Prioritized backlog: [`TODO.md`](TODO.md). Technical debt detail: [`TECHNICAL_DEBT.md`](TECHNICAL_DEBT.md).  
NuGet release: [`NUGET_RELEASE.md`](NUGET_RELEASE.md).
