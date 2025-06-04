using Ara3D.Collections;

namespace Plato.Geometry;

public static class PlatoGeometryExtensions
{
    public static IReadOnlyList<Integer> Range(this int self)
        => new ReadOnlyList<Integer>(self, i => i);

    public static IReadOnlyList<Integer> Range(this Integer self)
        => new ReadOnlyList<Integer>(self, i => i);
}