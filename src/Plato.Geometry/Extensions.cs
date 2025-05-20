namespace Plato;

public static class PlatoGeometryExtensions
{
    public static IArray<T> ToIArray<T>(this IReadOnlyList<T> list)
        => new ReadOnlyListAdapter<T>(list);
}