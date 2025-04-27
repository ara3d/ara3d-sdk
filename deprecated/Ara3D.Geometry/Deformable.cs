using System;
using Ara3D.Mathematics;

namespace Ara3D.Geometry
{
    public static class Deformable
    {
        public static T Translate<T>(this T self, Vector2 vector) 
            where T : IDeformable2D<T>
            => self.Deform2D(x => x + vector);

        public static T Scale<T>(this T self, Vector2 vector) 
            where T : IDeformable2D<T>
            => self.Deform2D(x => x * vector);
    }
}