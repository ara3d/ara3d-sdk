using System.Collections;
using Ara3D.Collections;

namespace Ara3D.Geometry;

public class FaceGroups : IReadOnlyList<FaceGroup>
{
    public List<FaceGroup> Groups { get; } = [];
    public int[] GroupIds { get; }

    public int NumFaces => GroupIds.Length;
    public int Count => Groups.Count;
    public FaceGroup this[int index] => Groups[index];

    public FaceGroups(int numFaces)
    {
        GroupIds = new int[numFaces];
        Array.Fill(GroupIds, -1);
    }

    public bool IsAssigned(int faceId)
        => GroupIds[faceId] >= 0;

    public void AssignBreadthFirst(
        Func<int, IEnumerable<int>> getNeighbors,
        Predicate<FaceGroup>? isGroupComplete = null)
    {
        Array.Fill(GroupIds, -1);
        Groups.Clear();

        isGroupComplete ??= _ => false;

        var queue = new Queue<int>();
        var queued = new bool[NumFaces];

        FaceGroup? group = null;

        FaceGroup StartGroup()
        {
            var newGroup = new FaceGroup(Groups.Count);
            Groups.Add(newGroup);
            return newGroup;
        }

        void Enqueue(int faceId)
        {
            if (queued[faceId])
                return;

            queued[faceId] = true;
            queue.Enqueue(faceId);
        }

        for (var seedFaceId = 0; seedFaceId < NumFaces; seedFaceId++)
        {
            if (IsAssigned(seedFaceId))
                continue;

            group = null;
            Enqueue(seedFaceId);

            while (queue.Count > 0)
            {
                var faceId = queue.Dequeue();

                if (IsAssigned(faceId))
                    continue;

                group ??= StartGroup();

                group.Add(faceId);
                GroupIds[faceId] = group.GroupId;

                foreach (var neighborId in getNeighbors(faceId))
                    if (!queued[neighborId])
                        Enqueue(neighborId);

                if (isGroupComplete(group))
                    group = null;
            }
        }
    }

    public static FaceGroups Create(int numFaces, Func<int, IEnumerable<int>> getNeighbors, Predicate<FaceGroup>? isGroupComplete = null)
    {
        var faceGroups = new FaceGroups(numFaces);
        faceGroups.AssignBreadthFirst(getNeighbors, isGroupComplete);
        return faceGroups;
    }

    public static FaceGroups Create(TriangleMesh3D mesh, Predicate<FaceGroup>? isGroupComplete = null)
    {
        var topo = mesh.GetTopology();
        var numFaces = mesh.FaceIndices.Count;
        var getNeighbors = new Func<int, IEnumerable<int>>(faceId => topo.GetFaceNeighborIds((FaceId)faceId).Select(f => (int)f));
        return Create(numFaces, getNeighbors, isGroupComplete);
    }

    public IEnumerator<FaceGroup> GetEnumerator()
        => Groups.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();
}

public static class FaceGroupsExtensions
{
    public static IReadOnlyList<TriangleMesh3D> Split(this FaceGroups groups, TriangleMesh3D mesh)
        => groups.Groups.Select(g => mesh.GetMeshFromFaces(g.FaceIds));

    public static TriangleMesh3D Merge(this IReadOnlyList<FaceGroup> groups, TriangleMesh3D mesh)
    {
        var indices = new List<Integer3>();
        foreach (var group in groups)
            foreach (var f in group.FaceIds)
                indices.Add(mesh.FaceIndices[f]);
        return mesh.WithFaceIndices(indices);
    }
}