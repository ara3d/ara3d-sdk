using Ara3D.BimOpenSchema;
using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using Document = Autodesk.Revit.DB.Document;

namespace Ara3D.Bowerbird.RevitSamples;

public class BosDocumentContext
{
    public readonly Document Document;
    public readonly BosDocumentContext Parent;
    public readonly RevitLinkInstance LinkInstance;
    public readonly bool IsLink;
    public readonly string LinkName = "";
    public readonly string Path = "";
    public readonly string ExternalPath = "";
    public readonly string Title = "";
    public readonly bool IsDetached;
    public readonly Transform Transform;
    public readonly BosRevitBuilder RevitBuilder;

    public BosDocumentContext(BosRevitBuilder revitBuilder, Document document, BosDocumentContext parent = null, RevitLinkInstance rli = null)
    {
        RevitBuilder = revitBuilder;
        if (rli == null || parent == null)
            if (rli != null || parent != null)
                throw new Exception("If either the RevitLinkInstance is null or the parent is null, both must be null");

        Document = document;
        Parent = parent;

        Transform = rli != null 
            ? parent.Transform.Multiply(rli.GetTransform()) 
            : Transform.Identity;

        LinkInstance = rli;
        IsDetached = document.IsDetached;
        Path = document.PathName;
        Title = document.Title;

        if (LinkInstance == null) 
            return;
        
        IsLink = true;
        LinkName = LinkInstance.Name;
        var typeId = LinkInstance.GetTypeId();
        var extRef = ExternalFileUtils.GetExternalFileReference(Parent.Document, typeId);
        var modelPath = extRef.GetPath();
        ExternalPath = modelPath == null 
            ? "" : ModelPathUtils.ConvertModelPathToUserVisiblePath(modelPath) ?? "";
    }

    public BosDocumentContext Create(BosDocumentContext parent, RevitLinkInstance rli)
    {
        var linkDocument = rli.GetLinkDocument();
        return linkDocument != null ? new BosDocumentContext(RevitBuilder, linkDocument, parent, rli) : null;
    }

    public List<BosDocumentContext> GatherLinkedDocuments()
    {
        var r = new HashSet<BosDocumentContext>();
        GatherLinkedDocuments(r);
        return r.ToList();
    }

    public void GatherLinkedDocuments(HashSet<BosDocumentContext> set)
    {
        if (!set.Add(this))
            return;
        foreach (var link in Document.GetLinks())
        {
            var tmp = Create(this, link);
            tmp?.GatherLinkedDocuments(set);
        }
    }

    public string Key
        => $"{Path}-{Title}-{ExternalPath}";

    public override int GetHashCode()
        => Key.GetHashCode();

    public override bool Equals(object obj)
        => obj is BosDocumentContext de && Key == de.Key;
}