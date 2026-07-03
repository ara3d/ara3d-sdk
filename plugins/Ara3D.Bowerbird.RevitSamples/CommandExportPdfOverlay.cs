using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using Ara3D.Utils;

namespace Ara3D.Bowerbird.RevitSamples;

public class CommandExportPdfOverlay : NamedCommand
{
    public override string Name=> "Export PDF Overlay";

    public UIApplication app { get; set; }

    public override void Execute(object arg)
    {
        app = (arg as UIApplication);
        if (app == null)
            throw new Exception($"Passed argument {arg} is either null or not a UI application");

        var uidoc = app.ActiveUIDocument;
        var doc = uidoc.Document;

        var records = CreatePdfOverlayRecords(
            doc,
            CommandExportPdfs.GetPdfFileName,
            paddingInInternalUnits: 0.25,
            includeAnnotations: false);

        var json = System.Text.Json.JsonSerializer.Serialize(
            records,
            new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true
            });

        var outputFile = new Ara3D.Utils.FilePath(@"C:\dev\temp\pdf_overlay_records.json");
        outputFile.WriteAllText(json);
    }

    public static List<PdfElementOverlayRecord> CreatePdfOverlayRecords(
        Document doc,
        Func<Document, View, string> getPdfFileName,
        double paddingInInternalUnits = 0.1,
        bool includeAnnotations = false,
        bool clampToPage = true)
    {
        if (doc == null) throw new ArgumentNullException(nameof(doc));
        if (getPdfFileName == null) throw new ArgumentNullException(nameof(getPdfFileName));

        var records = new List<PdfElementOverlayRecord>();

        var views = RevitPdfViewIndex.GetPrintable2DViews(doc);

        foreach (var view in views)
        {
            if (view == null)
                continue;

            Rect2D? pageBoxNullable;

            try
            {
                pageBoxNullable = RevitPdfOverlay.GetExportedViewBox2D(view);
            }
            catch
            {
                continue;
            }

            if (pageBoxNullable == null || !pageBoxNullable.Value.IsValid)
                continue;

            var pageBox = pageBoxNullable.Value;

            string pdfFileName;

            try
            {
                pdfFileName = getPdfFileName(doc, view);
            }
            catch
            {
                continue;
            }

            var viewId = view.Id.IntegerValue;
            var viewUniqueId = view.UniqueId;
            var viewName = view.Name;
            var viewType = view.ViewType.ToString();

            ICollection<ElementId> elementIds;

            try
            {
                elementIds = new FilteredElementCollector(doc, view.Id)
                    .WhereElementIsNotElementType()
                    .ToElementIds();
            }
            catch
            {
                // Some views can fail collector creation depending on view type/state.
                continue;
            }

            foreach (var elementId in elementIds)
            {
                if (elementId == null || elementId == view.Id)
                    continue;

                var element = doc.GetElement(elementId);
                if (element == null)
                    continue;

                if (!includeAnnotations && !RevitPdfViewIndex.IsLikelyModelElement(element))
                    continue;

                BoundingBoxXYZ bb;

                try
                {
                    bb = element.get_BoundingBox(view);
                }
                catch
                {
                    continue;
                }

                if (bb == null)
                    continue;

                var elementBox = RevitViewProjection.GetBoundingBoxViewRect2DFast(
                    view,
                    bb,
                    paddingInInternalUnits);

                if (!elementBox.IsValid)
                    continue;

                NormalizedRect2D normalized;

                try
                {
                    normalized = RevitPdfOverlay.ViewRectToNormalizedTopLeftRect(
                        elementBox,
                        pageBox);
                }
                catch
                {
                    continue;
                }

                if (clampToPage)
                    normalized = RevitPdfOverlay.Clamp01(normalized);

                if (normalized.Width <= 0 || normalized.Height <= 0)
                    continue;

                records.Add(new PdfElementOverlayRecord
                {
                    ElementId = element.Id.IntegerValue,
                    ElementUniqueId = element.UniqueId,

                    ViewId = viewId,
                    ViewUniqueId = viewUniqueId,
                    ViewName = viewName,
                    ViewType = viewType,

                    PdfFileName = pdfFileName,

                    X = normalized.X,
                    Y = normalized.Y,
                    Width = normalized.Width,
                    Height = normalized.Height
                });
            }
        }

        return records;
    }
}

public sealed class PdfElementOverlayRecord
{
    public long ElementId { get; set; }
    public string ElementUniqueId { get; set; }

    public long ViewId { get; set; }
    public string ViewUniqueId { get; set; }
    public string ViewName { get; set; }
    public string ViewType { get; set; }
    
    public string PdfFileName { get; set; }

    public double X { get; set; }
    public double Y { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }
}

public readonly struct NormalizedRect2D
{
    public readonly double X;
    public readonly double Y;
    public readonly double Width;
    public readonly double Height;

    public NormalizedRect2D(double x, double y, double width, double height)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }

    public double Right => X + Width;
    public double Bottom => Y + Height;
}

public static class RevitPdfOverlay
{
    public static NormalizedRect2D? GetElementPdfOverlayRectNormalized(
        Document doc,
        ElementId elementId,
        View view,
        double paddingInInternalUnits = 0.0,
        bool clampToPage = true)
    {
        var elementBox = RevitViewProjection.GetElementViewBox2D(
            doc,
            elementId,
            view,
            paddingInInternalUnits);

        if (elementBox == null)
            return null;

        var pageBox = GetExportedViewBox2D(view);

        if (pageBox == null || !pageBox.Value.IsValid)
            return null;

        var normalized = ViewRectToNormalizedTopLeftRect(
            elementBox.Value,
            pageBox.Value);

        if (clampToPage)
            normalized = Clamp01(normalized);

        if (normalized.Width <= 0 || normalized.Height <= 0)
            return null;

        return normalized;
    }

    public static Rect2D? GetExportedViewBox2D(View view)
    {
        if (view == null) throw new ArgumentNullException(nameof(view));

        // For direct view export, this is usually the best available proxy
        // for the drawing area that becomes the PDF page content.
        var crop = view.CropBox;

        if (crop == null)
            return null;

        var corners = RevitViewProjection.GetBoundingBoxCorners(crop)
            .Select(p => RevitViewProjection.ProjectModelPointToView2D(view, p))
            .ToList();

        return RevitViewProjection.Bounds2D(corners);
    }

    public static NormalizedRect2D ViewRectToNormalizedTopLeftRect(
        Rect2D elementViewRect,
        Rect2D exportedViewRect)
    {
        var pageWidth = exportedViewRect.Width;
        var pageHeight = exportedViewRect.Height;

        if (pageWidth <= 0 || pageHeight <= 0)
            throw new ArgumentException("Exported view rectangle has invalid dimensions.");

        // Revit-style normalized coordinates:
        // nx = 0 left, 1 right.
        // nyBottomUp = 0 bottom, 1 top.
        var nx0 = (elementViewRect.MinX - exportedViewRect.MinX) / pageWidth;
        var nx1 = (elementViewRect.MaxX - exportedViewRect.MinX) / pageWidth;

        var ny0BottomUp = (elementViewRect.MinY - exportedViewRect.MinY) / pageHeight;
        var ny1BottomUp = (elementViewRect.MaxY - exportedViewRect.MinY) / pageHeight;

        // Convert to browser/SVG coordinates:
        // y = 0 top, 1 bottom.
        var svgX = nx0;
        var svgY = 1.0 - ny1BottomUp;
        var svgW = nx1 - nx0;
        var svgH = ny1BottomUp - ny0BottomUp;

        return new NormalizedRect2D(svgX, svgY, svgW, svgH);
    }

    public static NormalizedRect2D Clamp01(NormalizedRect2D r)
    {
        var x0 = Clamp(r.X, 0, 1);
        var y0 = Clamp(r.Y, 0, 1);
        var x1 = Clamp(r.Right, 0, 1);
        var y1 = Clamp(r.Bottom, 0, 1);

        return new NormalizedRect2D(
            x0,
            y0,
            Math.Max(0, x1 - x0),
            Math.Max(0, y1 - y0));
    }

    public static double Clamp(double x, double min, double max)
        => x < min ? min : x > max ? max : x;
}


public readonly struct Rect2D
{
    public readonly double MinX;
    public readonly double MinY;
    public readonly double MaxX;
    public readonly double MaxY;

    public Rect2D(double minX, double minY, double maxX, double maxY)
    {
        MinX = Math.Min(minX, maxX);
        MinY = Math.Min(minY, maxY);
        MaxX = Math.Max(minX, maxX);
        MaxY = Math.Max(minY, maxY);
    }

    public double Width => MaxX - MinX;
    public double Height => MaxY - MinY;
    public bool IsValid => Width > 0 && Height > 0;

    public Rect2D Inflate(double amount)
        => new Rect2D(MinX - amount, MinY - amount, MaxX + amount, MaxY + amount);
}

public static class RevitViewProjection
{
    public static Rect2D GetBoundingBoxViewRect2DFast(
    View view,
    BoundingBoxXYZ bb,
    double paddingInInternalUnits = 0.0)
    {
        if (view == null) throw new ArgumentNullException(nameof(view));
        if (bb == null) throw new ArgumentNullException(nameof(bb));

        var min = bb.Min;
        var max = bb.Max;
        var t = bb.Transform ?? Transform.Identity;

        var origin = view.Origin;
        var right = view.RightDirection;
        var up = view.UpDirection;

        var first = ProjectBoundingBoxCornerToView2D(
            t, min.X, min.Y, min.Z, origin, right, up);

        var minX = first.U;
        var minY = first.V;
        var maxX = first.U;
        var maxY = first.V;

        IncludeBoundingBoxCorner(
            t, max.X, min.Y, min.Z, origin, right, up,
            ref minX, ref minY, ref maxX, ref maxY);

        IncludeBoundingBoxCorner(
            t, min.X, max.Y, min.Z, origin, right, up,
            ref minX, ref minY, ref maxX, ref maxY);

        IncludeBoundingBoxCorner(
            t, max.X, max.Y, min.Z, origin, right, up,
            ref minX, ref minY, ref maxX, ref maxY);

        IncludeBoundingBoxCorner(
            t, min.X, min.Y, max.Z, origin, right, up,
            ref minX, ref minY, ref maxX, ref maxY);

        IncludeBoundingBoxCorner(
            t, max.X, min.Y, max.Z, origin, right, up,
            ref minX, ref minY, ref maxX, ref maxY);

        IncludeBoundingBoxCorner(
            t, min.X, max.Y, max.Z, origin, right, up,
            ref minX, ref minY, ref maxX, ref maxY);

        IncludeBoundingBoxCorner(
            t, max.X, max.Y, max.Z, origin, right, up,
            ref minX, ref minY, ref maxX, ref maxY);

        var rect = new Rect2D(minX, minY, maxX, maxY);

        if (paddingInInternalUnits > 0)
            rect = rect.Inflate(paddingInInternalUnits);

        return rect;
    }

    private static UV ProjectBoundingBoxCornerToView2D(
        Transform transform,
        double x,
        double y,
        double z,
        XYZ viewOrigin,
        XYZ viewRight,
        XYZ viewUp)
    {
        var p = transform.OfPoint(new XYZ(x, y, z));
        var d = p - viewOrigin;

        return new UV(
            d.DotProduct(viewRight),
            d.DotProduct(viewUp));
    }

    private static void IncludeBoundingBoxCorner(
        Transform transform,
        double x,
        double y,
        double z,
        XYZ viewOrigin,
        XYZ viewRight,
        XYZ viewUp,
        ref double minX,
        ref double minY,
        ref double maxX,
        ref double maxY)
    {
        var p = ProjectBoundingBoxCornerToView2D(
            transform,
            x,
            y,
            z,
            viewOrigin,
            viewRight,
            viewUp);

        if (p.U < minX) minX = p.U;
        if (p.V < minY) minY = p.V;
        if (p.U > maxX) maxX = p.U;
        if (p.V > maxY) maxY = p.V;
    }

    public static Rect2D? GetElementViewBox2D(
        Document doc,
        ElementId elementId,
        View view,
        double paddingInInternalUnits = 0.0)
    {
        if (doc == null) throw new ArgumentNullException(nameof(doc));
        if (elementId == null) throw new ArgumentNullException(nameof(elementId));
        if (view == null) throw new ArgumentNullException(nameof(view));

        var element = doc.GetElement(elementId);
        if (element == null)
            return null;

        BoundingBoxXYZ bb = null;

        try
        {
            bb = element.get_BoundingBox(view);
        }
        catch
        {
            return null;
        }

        if (bb == null)
            return null;

        var corners = GetBoundingBoxCorners(bb);

        var points2D = corners
            .Select(p => ProjectModelPointToView2D(view, p))
            .ToList();

        var rect = Bounds2D(points2D);

        if (!rect.IsValid)
            return null;

        if (paddingInInternalUnits > 0)
            rect = rect.Inflate(paddingInInternalUnits);

        return rect;
    }

    public static UV ProjectModelPointToView2D(View view, XYZ modelPoint)
    {
        // Coordinates are in Revit internal length units.
        // X is horizontal on the view.
        // Y is vertical on the view.
        var d = modelPoint - view.Origin;

        var x = d.DotProduct(view.RightDirection);
        var y = d.DotProduct(view.UpDirection);

        return new UV(x, y);
    }

    public static Rect2D Bounds2D(IReadOnlyList<UV> pts)
    {
        if (pts == null || pts.Count == 0)
            throw new ArgumentException("No points supplied.", nameof(pts));

        var minX = pts.Min(p => p.U);
        var minY = pts.Min(p => p.V);
        var maxX = pts.Max(p => p.U);
        var maxY = pts.Max(p => p.V);

        return new Rect2D(minX, minY, maxX, maxY);
    }

    public static IEnumerable<XYZ> GetBoundingBoxCorners(BoundingBoxXYZ bb)
    {
        if (bb == null) throw new ArgumentNullException(nameof(bb));

        var min = bb.Min;
        var max = bb.Max;
        var t = bb.Transform ?? Transform.Identity;

        yield return t.OfPoint(new XYZ(min.X, min.Y, min.Z));
        yield return t.OfPoint(new XYZ(max.X, min.Y, min.Z));
        yield return t.OfPoint(new XYZ(min.X, max.Y, min.Z));
        yield return t.OfPoint(new XYZ(max.X, max.Y, min.Z));

        yield return t.OfPoint(new XYZ(min.X, min.Y, max.Z));
        yield return t.OfPoint(new XYZ(max.X, min.Y, max.Z));
        yield return t.OfPoint(new XYZ(min.X, max.Y, max.Z));
        yield return t.OfPoint(new XYZ(max.X, max.Y, max.Z));
    }
}

public static class RevitPdfViewIndex
{
    public static Dictionary<ElementId, List<ElementId>> BuildElementToViewLookup(
        Document doc,
        bool includeAnnotations = false,
        bool requireBoundingBoxInView = true)
    {
        if (doc == null) throw new ArgumentNullException(nameof(doc));

        var result = new Dictionary<ElementId, List<ElementId>>();

        var views = GetPrintable2DViews(doc).ToList();

        foreach (var view in views)
        {
            ICollection<ElementId> ids;

            try
            {
                ids = new FilteredElementCollector(doc, view.Id)
                    .WhereElementIsNotElementType()
                    .ToElementIds();
            }
            catch
            {
                // Some views can fail collector creation depending on view type/state.
                continue;
            }

            foreach (var id in ids)
            {
                if (id == view.Id)
                    continue;

                var e = doc.GetElement(id);
                if (e == null)
                    continue;

                if (!includeAnnotations && !IsLikelyModelElement(e))
                    continue;

                if (requireBoundingBoxInView)
                {
                    BoundingBoxXYZ bb = null;

                    try
                    {
                        bb = e.get_BoundingBox(view);
                    }
                    catch
                    {
                        // Some elements do not support view-specific bounds.
                    }

                    if (bb == null)
                        continue;
                }

                if (!result.TryGetValue(id, out var list))
                {
                    list = new List<ElementId>();
                    result[id] = list;
                }

                list.Add(view.Id);
            }
        }

        return result;
    }

    public static IEnumerable<View> GetPrintable2DViews(Document doc)
    {
        return new FilteredElementCollector(doc)
            .OfClass(typeof(View))
            .Cast<View>()
            .Where(v =>
                !v.IsTemplate &&
                v.CanBePrinted &&
                Is2DView(v));
    }

    public static bool Is2DView(View v)
    {
        if (v == null) return false;

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

    public static bool IsLikelyModelElement(Element e)
    {
        var cat = e.Category;
        if (cat == null)
            return false;

        // This filters out dimensions, text notes, tags, etc.
        // For a document-navigation demo, this is usually what you want.
        return cat.CategoryType == CategoryType.Model;
    }
}