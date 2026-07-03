using System;
using Ara3D.Utils;
using Autodesk.Revit.UI;
using Microsoft.Win32;
using System.Text;
using System.Windows.Forms;
using Ara3D.Logging;

namespace Ara3D.Bowerbird.RevitSamples;

public class CommandUpgradeFiles : NamedCommand
{
    public override string Name => "Upgrade Files";

    public override void Execute(object arg)
    {
        var app = (arg as UIApplication);
        var dlg = new OpenFolderDialog();
        dlg.Multiselect = false;
        if (dlg.ShowDialog() != true)
            return;
        
        var folder = new DirectoryPath(dlg.FolderName);
        var outputFolder = folder.RelativeFolder("upgrade");
        var sb = new StringBuilder();
        var logger = Logger.Create(sb);

        foreach (var f in folder.GetFiles("*.rvt"))
        {
            try
            {
                var doc = app.OpenNoDialog(f);

                try
                {
                    var outputFile = f.ChangeDirectory(outputFolder);
                    logger.Log($"Saving file to {outputFile.GetFileName()}");
                    doc.SaveCopy(outputFile);
                }
                finally
                {
                    doc.Close(false);
                }

            }
            catch (Exception e)
            {
                logger.Log($"Saving file to {e.Message}");
            }
        }
    }
}