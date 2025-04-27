using Ara3D.Collections;
using Ara3D.Mathematics;

namespace Ara3D.Geometry
{
    public static class Bounded
    {
        public static AABox UpdateBounds(this IBounded self, AABox box)
            => box.Merge(self.Bounds);

        public static AABox GetBounds<T>(this IArray<T> items) where T : IBounded
            => items.Any() 
                ? items.Aggregate(items.First().Bounds, (box,item) => UpdateBounds(item, box)) 
                : AABox.Empty;

        public static AABox2D UpdateBounds(this IBounded2D self, AABox2D box)
            => box.Merge(self.Bounds);

        public static AABox2D GetBounds2D<T>(this IArray<T> items) where T : IBounded2D
            => items.Any()
                ? items.Aggregate(items.First().Bounds, (box, item) => UpdateBounds(item, box))
                : AABox2D.Empty;
    }
}