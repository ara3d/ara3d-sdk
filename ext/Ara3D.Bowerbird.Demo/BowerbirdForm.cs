using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Ara3D.Logging;
using Ara3D.Utils;

namespace Ara3D.Bowerbird.Demo
{
    public partial class BowerbirdForm : Form
    {
        public BowerbirdService Service { get; }
        public ILogger Logger { get; }
        public ICommandExecutor Executor { get; }

        public BowerbirdForm(BowerbirdService service = null, ICommandExecutor executor = null)
        {
            InitializeComponent();

            Logger = Logger.Create("Bowerbird", OnLogMsg);
            Logger.Log($"Welcome to Bowerbird by https://ara3d.com");

            Executor = executor ?? new DefaultCommandExecutor { Logger = Logger };

            if (service == null)
            {
                const string AppName = "Bowerbird Demo App";
                var commandsFolder = SpecialFolders.LocalApplicationData.RelativeFolder(
                    "Ara 3D", AppName, "Commands");
                commandsFolder.Create();
                var libsFolder = SpecialFolders.LocalApplicationData.RelativeFolder(
                    "Ara 3D", AppName, "Libraries");
                var options = new BowerbirdOptions(AppName, commandsFolder, libsFolder);
                service = new BowerbirdService(options, Logger);
            }

            Service = service;
            Service.CatalogChanged += (_, _) => UpdateForm();
            HideLegacyControls();
            UpdateForm();
        }

        void HideLegacyControls()
        {
            checkBoxAutoRecompile.Visible = false;
            checkBoxParse.Visible = false;
            checkBoxEmit.Visible = false;
            checkBoxLoad.Visible = false;
            listBoxAssemblies.Visible = false;
            tabPage6.Text = "Compile log";
            tabPage7.Visible = false;
            label1.Text = "Commands root";
            label2.Text = "Selected command folder";
            RecompileButton.Text = "Compile selected command";
        }

        public void UpdateListBox(ListBox listBox, IEnumerable<object> items)
        {
            listBox.Items.Clear();
            if (items == null)
                return;
            foreach (var x in items)
                listBox.Items.Add(x);
        }

        public void UpdateForm()
        {
            var selected = GetSelectedDescriptor();
            textBoxSourceFiles.Text = Service.Options.CommandsRoot;
            textBoxLibraryDir.Text = selected?.Folder ?? "";
            textBoxOutputDll.Text = selected?.OutputDll ?? "";

            UpdateListBox(listBoxCommands, Service.Catalog.Commands.Select(c => c.DisplayName));
            UpdateListBox(listBoxFiles, selected?.SourceFiles.Select(f => (object)f));
            UpdateListBox(listBoxErrors, Service.LastResult?.Diagnostics);

            if (selected != null && selected.CompileLogPath.Exists())
                UpdateListBox(listBoxTypes, selected.CompileLogPath.ReadAllLines().Select(l => (object)l));
            else
                UpdateListBox(listBoxTypes, null);
        }

        public void OnLogMsg(string msg)
        {
            richTextBoxLog.AppendText(msg + Environment.NewLine);
        }

        private void aboutBowerbirdButtonClick(object sender, EventArgs e)
        {
            ProcessUtil.OpenUrl("http://github.com/ara3d/bowerbird");
        }

        private void clearLogButonClick(object sender, EventArgs e)
        {
            richTextBoxLog.Clear();
        }

        public CommandDescriptor GetSelectedDescriptor()
        {
            var i = listBoxCommands.SelectedIndex;
            if (i < 0 || i >= Service.Catalog.Commands.Count)
                return null;
            return Service.Catalog.Commands[i];
        }

        private void listBoxCommands_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            RunSelectedCommand();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ProcessUtil.OpenFolderInExplorer(textBoxSourceFiles.Text);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            var folder = textBoxLibraryDir.Text;
            if (!folder.IsNullOrWhiteSpace())
                ProcessUtil.OpenFolderInExplorer(folder);
        }

        private void listBoxFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        public FilePath GetSelectedFile()
        {
            var selected = GetSelectedDescriptor();
            var i = listBoxFiles.SelectedIndex;
            if (selected == null || i < 0 || i >= selected.SourceFiles.Count)
                return default;
            return selected.SourceFiles[i];
        }

        private void listBoxFiles_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            var file = GetSelectedFile();
            if (file.Exists())
                file.OpenDefaultProcess();
        }

        private void contextMenuStripCommands_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            contextMenuStripCommands.Items[0].Enabled = GetSelectedDescriptor() != null;
        }

        private void contextMenuStripFiles_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            contextMenuStripFiles.Items[0].Enabled = GetSelectedFile().Exists();
        }

        private void runSelectedCommandToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RunSelectedCommand();
        }

        public void RunSelectedCommand()
        {
            var descriptor = GetSelectedDescriptor();
            if (descriptor == null)
                return;

            Service.RunCommand(descriptor, null, Executor);
            UpdateForm();
        }

        private void openSelectedFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var file = GetSelectedFile();
            if (file.Exists())
                file.OpenDefaultProcess();
        }

        private void RecompileButton_Click(object sender, EventArgs e)
        {
            var descriptor = GetSelectedDescriptor();
            if (descriptor == null)
                return;

            Service.CompileCommand(descriptor);
            UpdateForm();
        }

        private void listBoxCommands_SelectedIndexChanged(object sender, EventArgs e)
            => UpdateForm();
    }
}
