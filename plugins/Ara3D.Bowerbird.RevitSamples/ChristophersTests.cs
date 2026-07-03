using Autodesk.Revit.UI;
using System.Text;
using Ara3D.Logging;

namespace Ara3D.Bowerbird.RevitSamples
{
    public class ChristophersTests : NamedCommand
    {
        public BimOpenSchemaExportSettings ExportSettings
            = new()
            {
                Folder = BimOpenSchemaExportSettings.DefaultFolder,
                IncludeLinks = true,
                IncludeGeometry = true,
            };

        public void ExportFile(UIApplication app, string file)
        {
            var doc = app.OpenAndActivateDocument(file).Document;
            var sb = new StringBuilder();
            var logger = Logger.Create(sb);
            doc?.ExportBimOpenSchema(ExportSettings, logger);
            TextDisplayForm.DisplayText(sb.ToString());
        }

        public override void Execute(object arg)
        {
            var uiapp = arg as UIApplication;
            var files = new[] {
                @"C:\Users\cdigg\data\Ara3D\rvt\Revit_Shipped_Samples_2025\BIM_Projekt_Golden_Nugget-Architektur_und_Ingenieurbau.rvt",
                @"C:\Users\cdigg\data\Ara3D\rvt\Revit_Shipped_Samples_2025\Snowdon Towers Sample Architectural.rvt",
                @"C:\Users\cdigg\data\Ara3D\rvt\Technicalschoolcurrentm.rvt",
                @"C:\Users\cdigg\data\Ara3D\rvt\rmeadvancedsampleproject.rvt",
                @"C:\Users\cdigg\data\Ara3D\rvt\rmebasicsampleproject.rvt",
                @"C:\Users\cdigg\data\Ara3D\rvt\rstadvancedsampleproject.rvt",
                @"C:\Users\cdigg\data\Ara3D\rvt\racadvancedsampleproject.rvt",
                @"C:\Users\cdigg\data\Ara3D\rvt\Autodesk_Hospital_2025\Autodesk_Hospital_Metric_Architectural_Central.rvt",
            };
            foreach (var f in files)
            {
                ExportFile(uiapp, f);
            }
        }
    }
}