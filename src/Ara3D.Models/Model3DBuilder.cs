using System.Diagnostics;
using Ara3D.Geometry;

namespace Ara3D.Models;

public class Model3DBuilder 
{
    private bool _frozen;

    public List<TriangleMesh3D> Meshes { get; } = [];
    public List<Material> Materials { get; } = [];
    public List<Matrix4x4> Transforms { get; } = [];
    public List<ElementStruct> ElementRefs { get; } = [];

    public Material DefaultMaterial;

    public Model3DBuilder(Material? defaultMaterial = null)
    {
        DefaultMaterial = defaultMaterial ?? Material.Default;
    }

    public int AddElement(TriangleMesh3D mesh)
        => AddElement(new Element(mesh, DefaultMaterial, Matrix4x4.Identity));

    public int AddElement(TriangleMesh3D mesh, Matrix4x4 transform)
        => AddElement(new Element(mesh, DefaultMaterial, transform));

    public int AddElement(TriangleMesh3D mesh, Material material)
        => AddElement(new Element(mesh, DefaultMaterial, Matrix4x4.Identity));

    public int AddMesh(TriangleMesh3D mesh)
    {
        Debug.Assert(!_frozen);
        Meshes.Add(mesh);
        return Meshes.Count - 1;
    }

    public void AddMeshes(IEnumerable<TriangleMesh3D> meshes)
    {
        Debug.Assert(!_frozen);
        Meshes.AddRange(meshes);
    }

    public int AddMaterial(Material material)
    {
        Debug.Assert(!_frozen);
        Materials.Add(material);
        return Materials.Count - 1;
    }

    public void AddMaterials(IEnumerable<Material> materials)
    {
        Debug.Assert(!_frozen);
        Materials.AddRange(materials);
    }

    public int AddTransform(Matrix4x4 transform)
    {
        Debug.Assert(!_frozen);
        Transforms.Add(transform);
        return Transforms.Count - 1;
    }

    public void AddTransforms(IEnumerable<Matrix4x4> transforms)
    {
        Debug.Assert(!_frozen);
        Transforms.AddRange(transforms);
    }

    public int AddElement(Element element)
        => AddElement(element.Mesh, element.Material, element.Transform);

    public int AddElement(TriangleMesh3D mesh, Material material, Matrix4x4 transform)
        => AddElement(AddMesh(mesh), AddMaterial(material), AddTransform(transform));

    public int AddElement(int meshIndex, int materialIndex, int transformIndex)
    {
        Debug.Assert(!_frozen);
        ElementRefs.Add(new ElementStruct(
            elementIndex: ElementRefs.Count, 
            materialIndex: materialIndex, 
            meshIndex: meshIndex, 
            transformIndex: transformIndex));
        return ElementRefs.Count - 1;
    }

    public void AddElements(IEnumerable<Element> elements)
    {
        Debug.Assert(!_frozen);
        foreach (var element in elements)
        {
            var meshIndex = AddMesh(element.Mesh);
            var materialIndex = AddMaterial(element.Material);
            var transformIndex = AddTransform(element.Transform);
            AddElement(meshIndex, materialIndex, transformIndex);
        }
    }

    public Model3D Build()
    {
        _frozen = true;
        return new Model3D(Meshes, Materials, Transforms, ElementRefs, null);
    }
}