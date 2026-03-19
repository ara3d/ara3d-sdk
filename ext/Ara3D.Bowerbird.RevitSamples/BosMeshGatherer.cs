using Ara3D.BimOpenSchema;
using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using Document = Autodesk.Revit.DB.Document;
using Material = Ara3D.Models.Material;

namespace Ara3D.Bowerbird.RevitSamples;

/// <summary>
/// A mesh with an index, a symbol and a transform  
/// </summary>
public sealed record GeometryPart
(
    Transform Transform,
    int MeshIndex,
    Material? Material
);

/// <summary>
/// A geometry is a set of geometric parts associated with an element and a document.
/// It has a default material.  
/// </summary>
public sealed record Geometry
(
    Element Element,
    EntityIndex EntityIndex,
    Material? DefaultMaterial,
    IReadOnlyList<GeometryPart> Parts
);

/// <summary>
/// Collects all meshes
/// </summary>
public class BosMeshGatherer
{
    public BosRevitBuilder BosRevitBuilder { get; }
    public Options Options => BosRevitBuilder.Options;

    public Document CurrentDocument => BosDocumentBuilder.Document;
    public BosDocumentBuilder BosDocumentBuilder { get; private set; }

    public List<Mesh> MeshList { get; } = [];
    public List<Geometry> Geometries { get; } = [];
  
    private readonly Dictionary<string, IReadOnlyList<GeometryPart>> _symbolCache = new();

    public BosMeshGatherer(BosRevitBuilder bldr)
    {
        BosRevitBuilder = bldr;
    }

    public void AddElement(BosDocumentBuilder docBuilder, Element e)
    {
        BosDocumentBuilder = docBuilder;
        var transform = docBuilder.DocumentContext.Transform;
        var g = ComputeGeometry(e, transform);
        if (g != null)
            Geometries.Add(g);
    }

    public Models.Material? GetMaterial(Element e, GeometryElement ge)
    {
        if (ge.MaterialElement == null)
            return null;
        var matId = ge.MaterialElement.Id;
        if (BosDocumentBuilder.MaterialLookup.TryGetValue(matId.Value, out var r))
            return r;

        var material = ge.MaterialElement.ToAra3DMaterial() 
                       ?? e.Category?.Material?.ToAra3DMaterial();

        if (material == null) return null;
        BosDocumentBuilder.MaterialLookup.Add(matId.Value, material.Value);
        return material;
    }

    public Geometry ComputeGeometry(Element e, Transform transform)
    {
        try
        {
            var geometryElement = e.get_Geometry(Options);
            if (geometryElement == null) return null;

            var material = GetMaterial(e, geometryElement);

            var parts = new List<GeometryPart>();
            TraverseElementGeometry(geometryElement, transform, parts);
            if (parts.Count == 0) return null;
            var ei = BosDocumentBuilder.GetElementIndex(e.Id);
            return new Geometry(e, ei, material, parts);
        }
        catch
        {
            return null;
        }
    }

    public void TraverseElementGeometry(
        GeometryElement geom,
        Transform transform,
        List<GeometryPart> parts)
    {
        if (geom == null)
            return;

        foreach (var obj in geom)
        {
            switch (obj)
            {
                case Solid solid:
                    if (ShouldKeep(CurrentDocument, solid))
                        AddSolidMeshes(solid, transform, parts);
                    break;

                case Mesh mesh:
                    if (ShouldKeep(CurrentDocument, mesh))
                        AddMeshInstance(mesh, transform, parts);
                    break;

                case GeometryInstance gi:
                    ProcessInstanceWithCaching(gi, transform, parts);
                    break;

                case GeometryElement subGeom:
                    TraverseElementGeometry(subGeom, transform, parts);
                    break;
            }
        }
    }   

    public void ProcessInstanceWithCaching(GeometryInstance gi, Transform worldFromParent, List<GeometryPart> parts)
    {
        var templates = GetOrBuildSymbolTemplates(gi);

        var worldFromSymbol = worldFromParent.Multiply(gi.Transform);

        foreach (var template in templates)
        {
            var worldTransform = worldFromSymbol.Multiply(template.Transform);
            parts.Add(template with { Transform = worldTransform });
        }
    }

    public string GetSymbolCacheKey(SymbolGeometryId symbolId)
        => symbolId?.AsUniqueIdentifier();

    private IReadOnlyList<GeometryPart> GetOrBuildSymbolTemplates(GeometryInstance gi)
    {
        if (gi == null) return [];

        var symbolId = gi.GetSymbolGeometryId();
        var key = GetSymbolCacheKey(symbolId);
        if (string.IsNullOrEmpty(key))
            return [];

        if (_symbolCache.TryGetValue(key, out var existing))
            return existing;

        var templates = new List<GeometryPart>();

        var symbolGeom = gi.GetSymbolGeometry();
        BuildSymbolTemplates(symbolGeom, Transform.Identity, templates);
        _symbolCache[key] = templates;
        return templates;
    }

    public void BuildSymbolTemplates(GeometryElement geom, Transform transform, List<GeometryPart> templates)
    {
        if (geom == null) 
            return;
        foreach (var obj in geom)
        {
            switch (obj)
            {
                case Solid solid:
                    if (ShouldKeep(CurrentDocument, solid))
                        AddSolidMeshes(solid, transform, templates);
                    break;

                case Mesh mesh:
                    if (ShouldKeep(CurrentDocument, mesh))
                        AddMeshInstance(mesh, transform, templates);
                    break;

                case GeometryInstance nestedGi:
                    BuildSymbolTemplates(
                        nestedGi.GetSymbolGeometry(), 
                        transform.Multiply(nestedGi.Transform), templates);
                    break;

                case GeometryElement subGeom:
                    BuildSymbolTemplates(subGeom, transform, templates);
                    break;
            }
        }
    }

    public int AddMesh(Mesh mesh)
    {
        MeshList.Add(mesh);
        return MeshList.Count - 1;
    }

    public void AddSolidMeshes(Solid solid, Transform transform, List<GeometryPart> parts)
    {
        foreach (Face face in solid.Faces)
            AddGeometryPart(
                face?.Triangulate(), 
                transform,
                parts,
                CurrentDocument.GetAra3DMaterial(face));
    }

    public void AddGeometryPart(Mesh mesh, Transform transform, List<GeometryPart> parts, Material? mat)
    {
        try
        {
            if (mesh == null || mesh.NumTriangles == 0)
                return;

            var index = AddMesh(mesh);
            var part = new GeometryPart(transform, index, mat);
            parts.Add(part);
        }
        catch
        {
            // Swallow exception
        }
    }

    private void AddMeshInstance(Mesh mesh, Transform transform, List<GeometryPart> parts)
        => AddGeometryPart(mesh, transform, parts, CurrentDocument.GetAra3DMaterial(mesh));

    public static bool ShouldKeep(Document doc, GeometryObject obj)
    {
        var styleId = obj.GraphicsStyleId;
        if (styleId == ElementId.InvalidElementId)
            return true;

        var style = doc.GetElement(styleId) as GraphicsStyle;
        if (style == null)
            return true;

        var cat = style.GraphicsStyleCategory;
        if (cat == null)
            return true;

        // Explicitly skip light source subcategories
        var catName = cat.Name ?? string.Empty;
        if (catName.Equals("Light Source", StringComparison.OrdinalIgnoreCase))
            return false;

        return true;
    }
}

