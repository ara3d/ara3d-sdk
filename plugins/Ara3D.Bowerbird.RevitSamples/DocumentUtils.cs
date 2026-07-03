using Ara3D.Utils;
using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Ara3D.Bowerbird.RevitSamples;

public static class DocumentUtils
{
    public static IEnumerable<RevitLinkInstance> GetLinks(this Document doc)
        => new FilteredElementCollector(doc)
            .OfClass(typeof(RevitLinkInstance))
            .Cast<RevitLinkInstance>();

    public static IEnumerable<Document> GetLinkedDocuments(this Document doc)
        => doc.GetLinks()
            .Select(li => li.GetLinkDocument())
            .WhereNotNull();

    public static IEnumerable<Element> GetElementsThatAreNotTypes(this Document doc)
        => new FilteredElementCollector(doc).WhereElementIsNotElementType();

    public static IEnumerable<Element> GetElementsThatAreTypes(this Document doc)
        => new FilteredElementCollector(doc).WhereElementIsElementType();

    /// <summary>
    /// Saves an opened document as an upgraded model in the current Revit version.
    /// By default, writes a COPY (recommended). Can also save as a new central.
    /// </summary>
    public static void SaveCopy(
        this Document doc,
        string targetPath,
        bool overwrite = true,
        bool compact = true,
        bool makeCentral = false,
        Action<string>? log = null)
    {
        if (doc == null) throw new ArgumentNullException(nameof(doc));
        if (string.IsNullOrWhiteSpace(targetPath))
            throw new ArgumentException("Target path is empty.", nameof(targetPath));

        log ??= (_ => { });

        var fullTargetPath = Path.GetFullPath(targetPath);
        Directory.CreateDirectory(Path.GetDirectoryName(fullTargetPath)!);

        var sao = new SaveAsOptions
        {
            OverwriteExistingFile = overwrite,
            Compact = compact
        };

        // If you opened a workshared model DETACHED, you can choose to:
        //  - Save as a normal standalone RVT copy (makeCentral=false), OR
        //  - Save as a NEW central (makeCentral=true) at the target path.
        if (doc.IsWorkshared && makeCentral)
        {
            var wso = new WorksharingSaveAsOptions
            {
                SaveAsCentral = true
            };
            sao.SetWorksharingOptions(wso);
        }

        log($"Saving upgraded model to: {fullTargetPath}");
        doc.SaveAs(fullTargetPath, sao);
        log("SaveAs complete.");
    }

}