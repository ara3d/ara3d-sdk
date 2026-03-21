using System;
using System.Windows;
using System.Windows.Forms;
using MessageBox = System.Windows.MessageBox;

namespace Ara3D.Utils.Wpf
{
    public class CommonDialogs
    {

        public static FilePath? SaveFile(DirectoryPath initialDir, string fileNameWithoutExtension, string fileDescription, string extWithoutDot)
        { ;
            var ext = $"*.{extWithoutDot}";
            var dlg = new SaveFileDialog()
            {
                AddExtension = true,
                DefaultExt = extWithoutDot,
                InitialDirectory = initialDir,
                FileName = fileNameWithoutExtension,
                Filter = $"{fileDescription} ({ext})|{ext}|All Files (*.*)|*.*"
            };

            return dlg.ShowDialog() == DialogResult.OK
                ? dlg.FileName
                : null;
        }

        public static FilePath? OpenFile(DirectoryPath initialDir, string fileName, string fileDescription, string extWithoutDot)
        {
            var ext = $"*.{extWithoutDot}";
            var dlg = new OpenFileDialog()
            {
                AddExtension = true,
                DefaultExt = extWithoutDot,
                InitialDirectory = initialDir,
                FileName = fileName,
                Filter = $"{fileDescription} ({ext})|{ext}|All Files (*.*)|*.*"
            };

            return dlg.ShowDialog() == DialogResult.OK
                ? dlg.FileName
                : null;
        }

        public static bool YesNo(string message, string caption)
            => MessageBox.Show(message, caption, MessageBoxButton.YesNo) == MessageBoxResult.Yes;

        public static bool OkCancel(string message, string caption)
            => MessageBox.Show(message, caption, MessageBoxButton.OKCancel) == MessageBoxResult.OK;

        public static void Error(string message, Exception ex)
            => MessageBox.Show(message + $": {ex?.Message}", "Error");

        public static void FolderExportCompleted(DirectoryPath path)
        {
            if (YesNo($"Export completed. Files saved to:\n{path}. Do you want to open the folder?",
                    "Export Completed"))
                path.OpenFolderInExplorer();
        }

        public static void FileExportCompleted(FilePath path)
        {
            if (YesNo($"Export completed. File saved to:\n{path}. Do you want to open the folder?",
                    "Export Completed"))
                path.OpenFileInExplorer();
        }
    }
}