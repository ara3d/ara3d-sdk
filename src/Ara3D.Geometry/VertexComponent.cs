namespace Ara3D.Geometry;

public readonly record struct VertexComponent(
    int ComponentIndex,
    IReadOnlyList<VertexId> Vertices);

public static class VertexComponentExtensions
{
    public static IReadOnlyList<VertexComponent> GetConnectedVertexComponents(this Topology self)
    {
        var visited = new bool[self.VertexCount];
        var result = new List<VertexComponent>();

        for (var start = 0; start < self.VertexCount; start++)
        {
            if (visited[start])
                continue;

            var vertices = new List<VertexId>();
            var stack = new Stack<VertexId>();

            visited[start] = true;
            stack.Push((VertexId)start);

            while (stack.Count > 0)
            {
                var v = stack.Pop();
                vertices.Add(v);

                foreach (var n in self.GetNeighborVertexIds(v))
                {
                    var ni = (int)n;
                    if (visited[ni])
                        continue;

                    visited[ni] = true;
                    stack.Push(n);
                }
            }

            result.Add(new VertexComponent(result.Count, vertices));
        }

        return result;
    }
}