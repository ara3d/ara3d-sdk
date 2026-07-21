using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;

namespace Ara3D.Revit.Utils;

public static class DwgExportExtensions
{
    /// <summary>
    /// Exports the given views to a single DWG file, using the given options or Revit's own
    /// defaults. Returns Revit's own success flag rather than throwing (P8).
    /// </summary>
    public static bool ExportToDwg(this IReadOnlyList<View> views, string folder, string name, DWGExportOptions? options = null)
        => views[0].Document.Export(folder, name, views.Select(v => v.Id).ToList(), options ?? new DWGExportOptions());

    /// <summary>One-view convenience over the IReadOnlyList&lt;View&gt; overload.</summary>
    public static bool ExportToDwg(this View view, string folder, string name, DWGExportOptions? options = null)
        => new[] { view }.ExportToDwg(folder, name, options);
}
