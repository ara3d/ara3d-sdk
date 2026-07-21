using System;
using System.IO;
using Autodesk.Revit.DB;

namespace Ara3D.Revit.Utils;

/// <summary>Path and save-as helpers for the host document.</summary>
public static class DocumentExtensions
{
    /// <summary>
    /// User-visible path for this document: the local/UNC file path for on-disk models, or
    /// the cloud (BIM 360 / ACC) model path for cloud-hosted ones — unlike the native
    /// <see cref="Document.PathName"/>, which is blank for cloud models.
    /// </summary>
    public static string GetPathName(this Document doc)
        => doc.IsModelInCloud
            ? ModelPathUtils.ConvertModelPathToUserVisiblePath(doc.GetCloudModelPath())
            : doc.PathName;

    /// <summary>Saves a copy of the document to <paramref name="targetPath"/>, creating the target directory if needed.</summary>
    public static void SaveCopy(
        this Document doc,
        string targetPath,
        bool overwrite = true,
        bool compact = true,
        bool makeCentral = false)
    {
        if (string.IsNullOrWhiteSpace(targetPath))
            throw new ArgumentException("Target path is empty.", nameof(targetPath));

        var fullTargetPath = Path.GetFullPath(targetPath);
        Directory.CreateDirectory(Path.GetDirectoryName(fullTargetPath)!);

        var options = new SaveAsOptions
        {
            OverwriteExistingFile = overwrite,
            Compact = compact
        };

        if (doc.IsWorkshared && makeCentral)
            options.SetWorksharingOptions(new WorksharingSaveAsOptions { SaveAsCentral = true });

        doc.SaveAs(fullTargetPath, options);
    }
}
