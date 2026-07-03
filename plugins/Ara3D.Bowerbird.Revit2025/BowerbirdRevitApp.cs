using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Reflection;
using System.Windows.Media.Imaging;
using Ara3D.Bowerbird.Demo;
using Ara3D.Events;
using Ara3D.Logging;
using Ara3D.Studio.Samples.Demos;
using Ara3D.Utils;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using Bitmap = System.Drawing.Bitmap;

namespace Ara3D.Bowerbird.Revit
{
    public class BowerbirdRevitApp : IExternalApplication, IEventErrorHandler
    {
        public const string BOWERBIRD_AUTORUN_ONLOAD_ENV_VAR = "BOWERBIRD_AUTORUN_ONLOAD";

        public static BowerbirdRevitApp Instance { get; private set; }
        public RevitContext RevitContext { get; private set; }

        public UIControlledApplication UicApp { get; private set; }
        public UIApplication UiApp { get; private set; }

        public BowerbirdOptions Options { get; private set; }
        public CommandExecutor CommandExecutor { get; set; }

        public BowerbirdService BowerbirdService { get; private set; }

        public Result OnShutdown(UIControlledApplication application)
            => Result.Succeeded;

        private BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (var memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                var bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();
                return bitmapimage;
            }
        }

        public static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            // TODO: upgrade this code for different Revit versions.
            if (args.Name.Contains("Bowerbird.Revit") && !args.Name.Contains("resources"))
            {
                // NOTE: this is horrible, but we have to do it. The assembly can't be found otherwise?!
                return typeof(BowerbirdRevitApp).Assembly;
            }
            return null;
        }

        public Bitmap GetImage()
        {
            var assembly = Assembly.GetExecutingAssembly();
            using var stream = assembly.GetManifestResourceStream("Ara3D.Bowerbird.Revit2025.Bowerbird-32x32.png");
            return new Bitmap(stream);
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
            _ = typeof(NamedPipeServerStream);

            UicApp = application;
            Instance = this;
            EventBus = new EventBus(this);

            var logger = new Logger(LogWriter.DebugWriter, "Bowerbird");
            CommandExecutor = new CommandExecutor(logger);
            RevitContext = new RevitContext(logger);

            application.ControlledApplication.DocumentOpened += App_DocumentOpened;

            // Store a reference to the UIApplication
            application.Idling += (sender, _) => { UiApp ??= sender as UIApplication; };

            var rvtRibbonPanel = GetOrCreateRibbonPanel("Ara 3D");
            var pushButtonData = new PushButtonData("Bowerbird", "Bowerbird\nC# Scripting",
                Assembly.GetExecutingAssembly().Location,
                typeof(BowerbirdExternalCommand).FullName);
            if (!(rvtRibbonPanel.AddItem(pushButtonData) is PushButton runButton))
                return Result.Failed;
            runButton.LargeImage = BitmapToImageSource(GetImage());
            runButton.ToolTip = "Compile and run per-folder C# commands";

            const string AppName = "Bowerbird for Revit 2025";
            var commandsFolder = SpecialFolders.LocalApplicationData.RelativeFolder(
                "Ara 3D", AppName, "Commands");
            var libsFolder = SpecialFolders.LocalApplicationData.RelativeFolder(
                "Ara 3D", AppName, "Libraries");

            // Try to force loading of "RangeAttribute".
            var landingsType = typeof(StairsWithLandings);
            var rangeType = typeof(RangeAttribute);
            Debug.WriteLine($"{rangeType.Assembly.Location}");

            Options = new BowerbirdOptions(AppName, commandsFolder, libsFolder);
            BowerbirdService = new BowerbirdService(Options, logger);
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            return Result.Succeeded;
        }

        private void App_DocumentOpened(object sender, DocumentOpenedEventArgs e)
        {
            var autoRunScript = Environment.GetEnvironmentVariable(BOWERBIRD_AUTORUN_ONLOAD_ENV_VAR);
            if (autoRunScript.IsNullOrWhiteSpace())
                return;

            BowerbirdService.RunCommandByName(autoRunScript, UiApp, CommandExecutor);
        }

        public BowerbirdForm Window { get; private set; }

        public BowerbirdForm GetOrCreateWindow(BowerbirdService service)
        {
            if (Window == null)
            {
                Window = new BowerbirdForm(service, CommandExecutor);
                Window.Text = Options.AppName;
                BowerbirdService.Logger = Window.Logger;
                Window.FormClosing += (_, args) =>
                {
                    Window.Hide();
                    args.Cancel = true;
                };
            }

            Window.Show();
            return Window;
        }

        public void Run(UIApplication application)
        {
            if (UiApp == null)
                UiApp = application;

            GetOrCreateWindow(BowerbirdService);
        }

        public void Schedule(Action<UIApplication> action, string name = "")
        {
            RevitContext.Schedule(action, name);
        }

        public IEventBus EventBus { get; private set; }

        public void OnError(ISubscriber sub, IEvent ev, Exception ex)
        {
            Debug.WriteLine($"Error occurred in {sub} processing {ev} with exception {ex}");
            Debugger.Break();
        }
    }
}
