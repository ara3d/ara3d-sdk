using Autodesk.Revit.DB;

namespace Ara3D.Revit.Utils;

public static class IfcExportExtensions
{
    /// <summary>
    /// Exports the document to IFC, using the given options or Revit's own defaults.
    /// Returns Revit's own success flag rather than throwing (P8 — file I/O boundary).
    /// </summary>
    public static bool ExportToIfc(this Document doc, string folder, string name, IFCExportOptions? options = null)
        => doc.Export(folder, name, options ?? new IFCExportOptions());
}
