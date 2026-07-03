# Ara3D.SDK.IO

Convenience NuGet bundle for Ara 3D SDK file-format, BIM, and IFC packages.

Includes BFAST, G3D, VIM, PLY, STEP, GeoJSON, glTF export and import (SharpGLTF fork),
BIM Open Schema model and IO, and IFC conversion via `Ara3D.IfcLoader`.

Targets `net8.0-windows` because BOS IO and IFC conversion depend on Windows-native libraries.
For cross-platform file I/O only, reference individual `Ara3D.IO.*` library packages instead.
