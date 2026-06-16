using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Ara3D.Bowerbird.RevitSamples;

public class CommandExportPdfs : NamedCommand
{
    public override string Name => "Export PDFs";

    public UIApplication app { get; set; }

    public override void Execute(object arg)
    {
        app = (arg as UIApplication);
        if (app == null)
        {
            throw new Exception($"Passed argument {arg} is either null or not a UI application");
        }

        var uidoc = app.ActiveUIDocument;
        var doc = uidoc.Document;
        ExportAll2DViewsToPdf(doc, @"C:\dev\temp\pdfs");
    }

    public static string GetPdfFileName(Document doc, View view)
    {
        if (doc == null) throw new ArgumentNullException(nameof(doc));
        if (view == null) throw new ArgumentNullException(nameof(view));

        var docName = Clean(GetDocumentName(doc));
        var viewType = Clean(view.ViewType.ToString());
        var viewName = Clean(view.Name);
        var level = Clean(GetLevelName(view));
        var sheetNumber = Clean(GetSheetNumber(doc, view));

        var fileName =
            $"{docName}__" +
            $"Sheet={sheetNumber}__" +
            $"{viewType}__" +
            $"Level={level}__" +
            $"{viewName}";

        return fileName + ".pdf";
    }

    public static string GetDocumentName(Document doc)
    {
        // Remove extension if present
        return Path.GetFileNameWithoutExtension(doc.Title);
    }

    public static string GetLevelName(View view)
    {
        // Works for most plan/section views
        if (view.GenLevel != null)
            return view.GenLevel.Name;

        // Fallback: try parameter
        var param = view.get_Parameter(BuiltInParameter.PLAN_VIEW_LEVEL);
        if (param != null && param.HasValue)
            return param.AsValueString();

        return "NoLevel";
    }

    public static string GetSheetNumber(Document doc, View view)
    {
        // Find sheet(s) this view is placed on
        var sheet = new FilteredElementCollector(doc)
            .OfClass(typeof(ViewSheet))
            .Cast<ViewSheet>()
            .FirstOrDefault(s => s.GetAllPlacedViews().Contains(view.Id));

        return sheet != null ? sheet.SheetNumber : "NoSheet";
    }

    public static string Clean(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return "None";

        foreach (var c in Path.GetInvalidFileNameChars())
            input = input.Replace(c, '_');

        // Optional: normalize whitespace
        input = input.Trim();

        return input;
    }

    public static IReadOnlyList<View> GetPrintable2DViews(Document doc)
        => new FilteredElementCollector(doc)
            .OfClass(typeof(View))
            .Cast<View>()
            .Where(v =>
                !v.IsTemplate &&
                v.CanBePrinted &&
                Is2DView(v))
            .ToList();

    public static int ExportAll2DViewsToPdf(
        Document doc,
        string outputFolder)
    {
        Directory.CreateDirectory(outputFolder);

        var views = GetPrintable2DViews(doc);
        int count = 0;

        foreach (var view in views)
        {
            var fileName = GetPdfFileName(doc, view);

            using (var options = new PDFExportOptions())
            {
                options.Combine = false; 
                options.FileName = EnsurePdfExtension(fileName);

                options.AlwaysUseRaster = false;
                options.ExportQuality = PDFExportQualityType.DPI1200;
                options.RasterQuality = RasterQualityType.Presentation;

                var ids = new List<ElementId> { view.Id };

                bool ok = doc.Export(outputFolder, ids, options);

                if (ok)
                    count++;
            }
        }

        return count;
    }

    public static bool Is2DView(View v)
    {
        switch (v.ViewType)
        {
            case ViewType.FloorPlan:
            case ViewType.CeilingPlan:
            case ViewType.Elevation:
            case ViewType.Section:
            case ViewType.Detail:
            case ViewType.DraftingView:
            case ViewType.AreaPlan:
            case ViewType.EngineeringPlan:
            case ViewType.Legend:
                return true;

            default:
                return false;
        }
    }

    public static string EnsurePdfExtension(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            return "Export.pdf";

        return fileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase)
            ? fileName
            : fileName + ".pdf";
    }
}