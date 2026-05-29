using Ara3D.Studio.Samples;

[Category(nameof(Categories.Converters))]
public class SeparateCurvedParts : IModifier
{
    [Range(0, 20f)]
    public float PushAmount = 2f;

    [Range(0, 180)]
    public float CurveCutOff { get; set; } = 15f;

    public int NumGroups { get; private set; }

    public IModel3D Eval(TriangleMesh3D mesh, EvalContext ctx)
    {
        var center = mesh.Bounds.Center;
        var topo = mesh.GetTopology();
        var numFaces = mesh.FaceIndices.Count;
        var ids = (-1).Repeat(numFaces).ToArray();
        var normals = mesh.Triangles.Map(tri => tri.Normal).ToArray();
        var cutoff = CurveCutOff.Degrees();

        void VisitGroupBreadthFirst(int startFaceIndex, int groupId)
        {
            Debug.Assert(ids[startFaceIndex] < 0);

            var queue = new Queue<int>();
            ids[startFaceIndex] = groupId;
            queue.Enqueue(startFaceIndex);

            while (queue.Count > 0)
            {
                var faceIndex = queue.Dequeue();
                var faceNormal = normals[faceIndex];

                foreach (var neighbor in topo.GetFaceNeighbors((FaceId)faceIndex))
                {
                    var neighborIndex = (int)neighbor.Id;

                    // Already assigned to a group.
                    if (ids[neighborIndex] >= 0)
                        continue;

                    var neighborNormal = normals[neighborIndex];
                    var angle = faceNormal.AngleBetween(neighborNormal);

                    if (Math.Abs(angle) > cutoff)
                        continue;

                    ids[neighborIndex] = groupId;
                    queue.Enqueue(neighborIndex);
                }
            }
        }

        NumGroups = 0;

        for (var faceIndex = 0; faceIndex < numFaces; faceIndex++)
        {
            if (ids[faceIndex] >= 0)
                continue;

            VisitGroupBreadthFirst(faceIndex, NumGroups++);
        }

        var modelBuilder = new Model3DBuilder();

        for (var groupId = 0; groupId < NumGroups; groupId++)
        {
            var faces = Enumerable
                .Range(0, numFaces)
                .Where(faceIndex => ids[faceIndex] == groupId)
                .ToList();

            var localMesh = mesh.GetMeshFromFaces(faces);
            var localMeshCenter = localMesh.Bounds.Center;
            var dir = (localMeshCenter - center).Normalize();
            if (dir.Magnitude > 0.001f)
            {
                var pos = dir * PushAmount;
                modelBuilder.AddInstance(localMesh, Matrix4x4.CreateTranslation(pos), Material.Default);
            }
            else
                modelBuilder.AddInstance(localMesh);
        }

        ctx.Application.RebuildUI(this);
        return modelBuilder.Build();
    }
}