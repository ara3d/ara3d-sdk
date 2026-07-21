using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;

namespace Ara3D.Revit.Utils;

public static class PdfExportExtensions
{
    /// <summary>
    /// Exports the given views/sheets to PDF, using the given options or a combined-file
    /// default. Returns Revit's own success flag rather than throwing (P8).
    /// </summary>
    public static bool ExportToPdf(this IReadOnlyList<View> views, string folder, PDFExportOptions? options = null)
        => views[0].Document.Export(folder, views.Select(v => v.Id).ToList(), options ?? new PDFExportOptions { Combine = true });

    /// <summary>One-view convenience over the IReadOnlyList&lt;View&gt; overload.</summary>
    public static bool ExportToPdf(this View view, string folder, PDFExportOptions? options = null)
        => new[] { view }.ExportToPdf(folder, options);
}
