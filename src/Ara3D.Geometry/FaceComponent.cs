namespace Ara3D.Geometry;

public readonly record struct FaceComponent(
    int ComponentIndex,
    IReadOnlyList<FaceId> Faces,
    int BoundaryEdgeCount,
    bool HasBoundary,
    bool HasNonManifoldEdges);

public static class FaceComoponentExtensions
{
    public static IReadOnlyList<FaceComponent> GetConnectedFaceComponents(
        this Topology self,
        bool crossNonManifoldEdges = true)
    {
        var visited = new bool[self.FaceCount];
        var result = new List<FaceComponent>();

        for (var start = 0; start < self.FaceCount; start++)
        {
            if (visited[start])
                continue;

            var faces = new List<FaceId>();
            var stack = new Stack<FaceId>();
            var hasBoundary = false;
            var hasNonManifold = false;
            var boundaryEdgeCount = 0;

            visited[start] = true;
            stack.Push((FaceId)start);

            while (stack.Count > 0)
            {
                var f = stack.Pop();
                faces.Add(f);

                foreach (var he in self.GetHalfEdgeIds(f))
                {
                    var ue = self.GetUndirectedEdge(he);
                    var siblings = self.GetHalfEdgeIds(ue);

                    if (siblings.Count == 1)
                    {
                        hasBoundary = true;
                        boundaryEdgeCount++;
                        continue;
                    }

                    if (siblings.Count > 2)
                    {
                        hasNonManifold = true;
                        if (!crossNonManifoldEdges)
                            continue;
                    }

                    foreach (var sibling in siblings)
                    {
                        var nf = self.GetFaceId(sibling);
                        var ni = (int)nf;

                        if (ni == (int)f || visited[ni])
                            continue;

                        visited[ni] = true;
                        stack.Push(nf);
                    }
                }
            }

            result.Add(new FaceComponent(
                result.Count,
                faces,
                boundaryEdgeCount,
                hasBoundary,
                hasNonManifold));
        }

        return result;
    }
}