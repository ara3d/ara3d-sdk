# ext — Windows-only SDK extensions

Projects here depend on Windows APIs, native Windows binaries, or WPF/WinForms. They are
published as individual NuGet packages and included in the `Ara3D.SDK.IO` / `Ara3D.SDK`
meta-packages where noted.

| Project | Role |
| --- | --- |
| [Ara3D.Utils.Wpf](Ara3D.Utils.Wpf) | WPF controls and dialog helpers |
| [Ara3D.IfcLoader](Ara3D.IfcLoader) | IFC → BOS conversion (native `web-ifc` DLL) |
| [Ara3D.IfcTypes](Ara3D.IfcTypes) | Generated IFC entity types (shared project) |

**Not in `ext/`** (see repo root folders):

- [`src/`](../src/) — supported cross-platform libraries and meta-packages
- [`apps/`](../apps/) — standalone desktop apps (e.g. BOS Browser)
- [`plugins/`](../plugins/) — host plug-in systems and Revit/Bowerbird integrations
- [`integrations/`](../integrations/) — optional third-party loaders (e.g. Assimp)
