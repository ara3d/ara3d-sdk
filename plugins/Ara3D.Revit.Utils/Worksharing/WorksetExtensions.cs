using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;

namespace Ara3D.Revit.Utils;

public static class WorksetExtensions
{
    public static IReadOnlyList<Workset> GetUserWorksets(this Document doc)
        => new FilteredWorksetCollector(doc).OfKind(WorksetKind.UserWorkset).ToList();

    /// <summary>The user workset with an exact name match, or null (P8). Empty in a non-workshared document.</summary>
    public static Workset? FindWorksetByName(this Document doc, string name)
        => doc.GetUserWorksets().FirstOrDefault(w => w.Name == name);

    /// <summary>Throwing form of <see cref="FindWorksetByName"/> (§3.3 <c>Get…</c> contract).</summary>
    public static Workset GetWorksetByName(this Document doc, string name)
        => doc.FindWorksetByName(name)
           ?? throw new ArgumentException($"No user workset named '{name}' in the document.", nameof(name));

    /// <summary>The workset owning <paramref name="element"/>. Throws if <c>WorksetId</c> doesn't resolve (e.g. non-workshared document).</summary>
    public static Workset GetWorkset(this Element element)
        => element.Document.GetWorksetTable().GetWorkset(element.WorksetId);
}
