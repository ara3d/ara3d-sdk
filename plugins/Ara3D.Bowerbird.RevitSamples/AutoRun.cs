using System.Diagnostics;
using Ara3D.Utils;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Ara3D.Bowerbird.RevitSamples
{
    public class AutoRun : NamedCommand
    {
        public string Name => "AutoRun";

        public void Execute(object arg)
        {
            var app = (UIApplication)arg;
            var uiDoc = app.ActiveUIDocument;
            var view = uiDoc.Document.GetDefault3DView();
            if (view != null) 
                uiDoc.ActiveView = view;
            var output = PathUtil.CreateTempFile().ChangeExtension("png");
            ExportCurrentViewToPng(uiDoc.Document, output);
            output.OpenDefaultProcess();
            Process.GetCurrentProcess().Kill();
        }

        public static void ExportCurrentViewToPng(Document doc, Utils.FilePath filePath)
        {
            var img = new ImageExportOptions();
            img.ZoomType = ZoomFitType.FitToPage;
            img.PixelSize = 1024;
            img.ImageResolution = ImageResolution.DPI_600;
            img.FitDirection = FitDirectionType.Horizontal;
            img.ExportRange = ExportRange.CurrentView;
            img.HLRandWFViewsFileType = ImageFileType.PNG;
            img.FilePath = filePath;
            img.ShadowViewsFileType = ImageFileType.PNG;
            doc.ExportImage(img);
        }
    }
}