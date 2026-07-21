using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace Ara3D.Revit.Utils;

public static class ImageExportExtensions
{
    /// <summary>
    /// Exports a single view to an image file. Uses ExportRange.SetOfViews internally so it
    /// works for any view, not just the document's active view. File write only; no
    /// transaction, no model mutation.
    /// </summary>
    public static void ExportToImage(this View view, string path,
        ImageFileType fileType = ImageFileType.PNG,
        ImageResolution resolution = ImageResolution.DPI_150,
        int pixelSize = 1024)
    {
        var options = new ImageExportOptions
        {
            FilePath = path,
            ZoomType = ZoomFitType.FitToPage,
            PixelSize = pixelSize,
            ImageResolution = resolution,
            FitDirection = FitDirectionType.Horizontal,
            HLRandWFViewsFileType = fileType,
            ShadowViewsFileType = fileType,
            ExportRange = ExportRange.SetOfViews
        };
        options.SetViewsAndSheets(new List<ElementId> { view.Id });
        view.Document.ExportImage(options);
    }
}
