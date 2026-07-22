namespace Ara3D.Studio.Samples.Modifiers;

[Category(Cat.Cleanup)]
[Description("Removes a contiguous range of faces from the mesh (or model), by start index and count.")]
public class DeleteFaces : IModifier
{
    [Range(0, 1000)] public int Index;
    [Range(1, 1000)] public int Count; 

    public object Eval(object obj)
    {
        if (obj is TriangleMesh3D triMesh)
        {
            var listA = triMesh.FaceIndices.Take(Index);
            var listB = triMesh.FaceIndices.Skip(Index + Count);
            return new TriangleMesh3D(triMesh.Points, listA.Concat(listB));
        }

        if (obj is QuadMesh3D quadMesh)
        {
            var listA = quadMesh.FaceIndices.Take(Index);
            var listB = quadMesh.FaceIndices.Skip(Index + Count);
            return new QuadMesh3D(quadMesh.Points, listA.Concat(listB));
        }
        
        if (obj is QuadGrid3D quadGrid)
        {
            return Eval(quadGrid.ToQuadMesh3D());
        }

        if (obj is IModel3D model)
        {
            return new Model3D(model.Meshes.Select(m => (TriangleMesh3D)Eval(m)), model.Instances);
        }

        return obj;
    }
}