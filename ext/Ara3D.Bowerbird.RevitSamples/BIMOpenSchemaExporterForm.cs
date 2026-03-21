using Ara3D.Bowerbird.RevitSamples;
using Ara3D.Utils;
using System;
using System.IO;
using System.Windows.Forms;
using Ara3D.Logging;
using Autodesk.Revit.UI;
using Ara3D.Utils;

namespace Ara3D.BIMOpenSchema.Revit2025
{
    public partial class BIMOpenSchemaExporterForm : Form
    {
        public UIApplication UIApp;
        public Autodesk.Revit.DB.Document CurrentDocument;
        public FilePath CurrentFilePath;
        public BimOpenSchemaExportSettings Settings;

        public static FilePath Ara3dStudioExePath
            => SpecialFolders.LocalApplicationData.RelativeFile("Ara 3D", "Ara 3D Studio", "Ara3D.exe");

        public static DirectoryPath DefaultFolder =>
            SpecialFolders.MyDocuments.RelativeFolder("BIM Open Schema");

        public BIMOpenSchemaExporterForm()
        {
            InitializeComponent();

            Settings = BimOpenSchemaExportSettings.LoadDefaultOrCreate();
            Settings.Folder = DefaultFolder;
            DefaultFolder.Create();
            UpdateControlsFromSettings();

            FormClosing += (_, args) =>
            {
                args.Cancel = true;
                Hide(); // Hide the form instead of closing it
            };
        }

        public void Show(UIApplication uiApp, Autodesk.Revit.DB.Document doc = null)
        {

            if (uiApp == null)
            {
                MessageBox.Show("No Revit UIApplication is available", "Internal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            UIApp = uiApp;

            doc ??= UIApp?.ActiveUIDocument?.Document;
            if (doc == null)
            {
                MessageBox.Show("No Revit document is available. Please open a Revit document first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            CurrentDocument = doc;

            Show();
        }

        public void UpdateControlsFromSettings()
        {
            textBoxOutputFolder.Text = Settings.Folder;
            checkBoxIncludeLinks.Checked = Settings.IncludeLinks;
            checkBoxIncludeGeometry.Checked = Settings.IncludeGeometry;
        }

        public BimOpenSchemaExportSettings GetExportSettingsFromFileAndControls()
        {
            Settings = BimOpenSchemaExportSettings.LoadDefaultOrCreate();
            Settings.Folder = textBoxOutputFolder.Text;
            Settings.IncludeLinks = checkBoxIncludeLinks.Checked;
            Settings.IncludeGeometry = checkBoxIncludeGeometry.Checked;
            return Settings;
        }

        public void Log(string s)
        {
            richTextBoxLog.BeginInvoke(() =>
            {
                try
                {
                    richTextBoxLog.AppendText(s + Environment.NewLine);
                }
                catch
                { }
            });
        }

        public bool DoExport(bool shutDownOnCompletion)
        {
            richTextBoxLog.Clear();

            var settings = GetExportSettingsFromFileAndControls();

            var folder = settings.Folder;
            try
            {
                if (!Directory.Exists(folder))
                {
                    MessageBox.Show($"The folder {folder} does not exist. Please choose a valid folder.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                folder.TestWrite();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"The folder {folder} was not writeable. Error {ex.Message}. Please choose a different folder.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            try
            {
                if (CurrentDocument == null)
                {
                    MessageBox.Show("No active document found. Please open a Revit document and try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                var logWriter = LogWriter.Create(Log);
                var logger = new Logger(logWriter, "BOS Exporter");

                RevitWorkQueue.QueueWork(uiApp =>
                {
                    CurrentFilePath = default;
                    var exporter = new BimOpenSchemaExporter(uiApp, CurrentDocument, settings, logger, false, shutDownOnCompletion);
                    CurrentFilePath = exporter.OutputFilePath;
                });
            }
            catch (Exception ex)
            {
                Log(ex.Message);
                MessageBox.Show($"An error occurred during export: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private void buttonAdvancedSettings_Click(object sender, EventArgs e)
        {
            ProcessUtil.OpenFile(BimOpenSchemaExportSettings.GetUserSettingsPath());
        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.InitialDirectory = Settings.Folder.GetFullPath();
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                textBoxOutputFolder.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void buttonRunExport_Click(object sender, EventArgs e)
        {
            DoExport(false);
        }

        private void buttonMoreInfo_Click(object sender, EventArgs e)
        {
            try
            {
                var url = @"https://www.bimopenschema.com";
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to open link: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            CurrentFilePath.OpenFileInExplorer();
        }

        private void buttonLaunch_Click_1(object sender, EventArgs e)
        {
            if (!Ara3dStudioExePath.Exists())
                MessageBox.Show("Could not find Ara 3D Studio");

            if (CurrentFilePath.Exists())
                Ara3dStudioExePath.Execute(CurrentFilePath.Value.Quote());
            else
                Ara3dStudioExePath.Execute();
        }
    }
}
