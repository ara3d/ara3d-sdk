namespace Ara3D.Scene
{
    public static class Extensions
    {
        public static IReadOnlyList<T1> Select<T0, T1>(this IReadOnlyList<T0> self, Func<T0, T1> f)
            => new ReadOnlyList<T1>(self.Count, i => f(self[i]));
    }
}