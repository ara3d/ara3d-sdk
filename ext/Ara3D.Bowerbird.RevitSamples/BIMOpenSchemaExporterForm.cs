using Ara3D.Bowerbird.RevitSamples;
using Ara3D.Utils;
using System;
using System.IO;
using System.Windows.Forms;
using Ara3D.Logging;
using Autodesk.Revit.UI;

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

            buttonLanchAra3D.Enabled = true;
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

        private void chooseFolderButton_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.InitialDirectory = Settings.Folder.GetFullPath();
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                exportDirTextBox.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        public void UpdateControlsFromSettings()
        {
            exportDirTextBox.Text = Settings.Folder;
            checkBoxIncludeLinks.Checked = Settings.IncludeLinks;
            checkBoxMeshGeometry.Checked = Settings.IncludeGeometry;
            comboBoxLod.SelectedIndex =
                Settings.DetailLevel == BimOpenSchemaExportSettings.DetailLevelEnum.Coarse ? 0 :
                Settings.DetailLevel == BimOpenSchemaExportSettings.DetailLevelEnum.Medium ? 1 :
                2;
        }

        public BimOpenSchemaExportSettings GetExportSettingsFromControls()
        {
            Settings.Folder = exportDirTextBox.Text;
            Settings.IncludeLinks = checkBoxIncludeLinks.Checked;
            Settings.IncludeGeometry = checkBoxMeshGeometry.Checked;
            Settings.DetailLevel
                = comboBoxLod.SelectedIndex == 0 ? BimOpenSchemaExportSettings.DetailLevelEnum.Coarse
                : comboBoxLod.SelectedIndex == 1 ? BimOpenSchemaExportSettings.DetailLevelEnum.Medium
                : BimOpenSchemaExportSettings.DetailLevelEnum.Fine;
            return Settings;
        }

        public void Log(string s)
        {
            richTextBox1.BeginInvoke(() =>
            {
                try
                {
                    richTextBox1.AppendText(s + Environment.NewLine);
                }
                catch
                { }
            });
        }

        private void buttonExport_Click(object sender, EventArgs e)
        {
            DoExport();
        }

        public bool DoExport()
        {
            richTextBox1.Clear();

            var settings = GetExportSettingsFromControls();

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
                    _ = new BimOpenSchemaExporter(uiApp, CurrentDocument, settings, logger, false));
            }
            catch (Exception ex)
            {
                Log(ex.Message);
                MessageBox.Show($"An error occurred during export: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private void buttonLaunchAra3D_Click(object sender, EventArgs e)
        {
            if (!Ara3dStudioExePath.Exists())
                MessageBox.Show("Could not find Ara 3D Studio");

            if (CurrentFilePath.Exists())
                Ara3dStudioExePath.Execute(CurrentFilePath.Value.Quote());
            else
                Ara3dStudioExePath.Execute();
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        { }

        private void buttonHelp_Click(object sender, EventArgs e)
        {
            try
            {
                var url = @"www.bim-open-schema";
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

        private void textBoxOverview_TextChanged(object sender, EventArgs e)
        {

        }

        private void labelSubtitle_Click(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }
    }
}
