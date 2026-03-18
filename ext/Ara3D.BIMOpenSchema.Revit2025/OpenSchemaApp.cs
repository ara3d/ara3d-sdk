using Ara3D.Bowerbird.RevitSamples;
using Ara3D.Logging;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using System;
using System.Reflection;
using System.Windows.Media.Imaging;

namespace Ara3D.BIMOpenSchema.Revit2025
{
    public class OpenSchemaApp : IExternalApplication
    {
        public static OpenSchemaApp Instance { get; private set; }
        public UIControlledApplication UicApp { get; private set; }
        public UIApplication UiApp { get; private set; }
        public CommandExecutor CommandExecutor { get; set; }
        public BIMOpenSchemaExporterForm Form { get; private set; }

        public Result OnShutdown(UIControlledApplication application)
            => Result.Succeeded;

        public static BitmapImage GetImage()
        {
            var assembly = Assembly.GetExecutingAssembly();
            using var stream = assembly.GetManifestResourceStream(
                "Ara3D.BIMOpenSchema.Revit2025.ara3d-32x32.png");

            var bmpImg = new BitmapImage();
            bmpImg.BeginInit();
            bmpImg.StreamSource = stream;
            bmpImg.CacheOption = BitmapCacheOption.OnLoad;
            bmpImg.EndInit();
            return bmpImg;
        }

        public RibbonPanel GetOrCreateRibbonPanel(string name)
        {
            foreach (var rb in UicApp.GetRibbonPanels())
                if (rb.Name == name)
                    return rb;
            return UicApp.CreateRibbonPanel(name);
        }

        public Result OnStartup(UIControlledApplication application)
        {
            RevitWorkQueue.Init();

            UicApp = application;
            Instance = this;
            
            var logger = new Logger(LogWriter.DebugWriter, "BIMOpenSchema");
            CommandExecutor = new CommandExecutor(logger);
            
            // Store a reference to the UIApplication
            application.Idling += (sender, _) => { UiApp ??= sender as UIApplication; };
                
            // Register for document - opened notifications
            application.ControlledApplication.DocumentOpened += ControlledApplicationOnDocumentOpened;
            
            var rvtRibbonPanel = GetOrCreateRibbonPanel("Ara 3D");
            var pushButtonData = new PushButtonData("BimOpenSchema", "BIM Open Schema\nParquet Exporter", 
                Assembly.GetExecutingAssembly().Location,
                typeof(OpenSchemaExternalCommand).FullName);
            // https://www.revitapidocs.com/2020/544c0af7-6124-4f64-a25d-46e81ac5300f.htm
            if (!(rvtRibbonPanel.AddItem(pushButtonData) is PushButton runButton))
                return Result.Failed;
            runButton.LargeImage = GetImage();
            runButton.ToolTip = "Export a zip archive of parquet files with extension .BOS.";

            return Result.Succeeded;    
        }

        private void ControlledApplicationOnDocumentOpened(object sender, DocumentOpenedEventArgs e)
        {
            var autoExport = Environment.GetEnvironmentVariable("ARA3D_BOS_AUTO_EXPORT");

            if (autoExport == "TRUE")
            {
                var form = new BIMOpenSchemaExporterForm();
                form.Show(UiApp, e.Document);
                form.DoExport();
                UiApp.PostCommand(RevitCommandId.LookupPostableCommandId(PostableCommand.ExitRevit));
            }
        }

        public void Run(UIApplication application)
        {
            UiApp ??= application;
            Form ??= new BIMOpenSchemaExporterForm();
            Form.Show(UiApp);
        }
    }
}
