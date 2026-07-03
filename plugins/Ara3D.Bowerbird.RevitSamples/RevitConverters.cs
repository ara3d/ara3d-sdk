using System;
using System.Collections.Generic;
using System.Linq;
using Ara3D.Geometry;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.DirectContext3D;

namespace Ara3D.Bowerbird.RevitSamples
{
    public static class RevitConverters
    {
        public static XYZ ToRevit(this Vector3 self) => new XYZ(self.X, self.Y, self.Z);
        public static XYZ ToRevit(this Point3D self) => new XYZ(self.X, self.Y, self.Z);
        public static UV ToRevit(this Vector2 self) => new UV(self.X, self.Y);
        public static UV ToRevit(this Point2D self) => new UV(self.X, self.Y);
        public static ColorWithTransparency ToRevit(this Color32 self) => new ColorWithTransparency(self.R, self.G, self.B, self.A);

        public static Vector3 ToAra3D(this XYZ self) => new Vector3((float)self.X, (float)self.Y, (float)self.Z);
        public static Vector2 ToAra3D(this UV self) => new Vector2((float)self.U, (float)self.V);
        public static Line3D ToAra3D(this XYZ self, XYZ other) => (self.ToAra3D(), other.ToAra3D());

        public static BoundingBoxXYZ ToRevit(this Bounds3D self) => new BoundingBoxXYZ() { Min = self.Min.ToRevit(), Max = self.Max.ToRevit() };
        public static BoundingBoxUV ToRevit(this Bounds2D self) => new BoundingBoxUV() { Min = self.Min.ToRevit(), Max = self.Max.ToRevit() };

        //public static Geometry.Plane ToAra3D(this ClipPlane self) => (self.Normal.ToAra3D(), (float)self.Origin.DistanceTo(self.Normal));
        public static Bounds3D ToAra3D(this BoundingBoxXYZ self) => new Bounds3D(self.Min.ToAra3D(), self.Max.ToAra3D());
        public static Bounds2D ToAra3D(this BoundingBoxUV self) => new Bounds2D(self.Min.ToAra3D(), self.Max.ToAra3D());

       public static IEnumerable<T4> Zip<T1, T2, T3, T4>(this IEnumerable<T1> xs, IEnumerable<T2> ys, IEnumerable<T3> zs, Func<T1, T2, T3, T4> f)
       {
           using var xe = xs.GetEnumerator();
           using var ye = ys.GetEnumerator();
           using var ze = zs.GetEnumerator();
           while (xe.MoveNext() && ye.MoveNext() && ze.MoveNext())
               yield return f(xe.Current, ye.Current, ze.Current);
       }

        public static void Fill(this VertexBuffer buffer, IEnumerable<XYZ> positions, IEnumerable<XYZ> normals, IEnumerable<ColorWithTransparency> colors)
            => buffer.Fill(positions.Zip(normals, colors, (p, n, c) => new VertexPositionNormalColored(p, n, c)).ToList());

        public static void Fill(this VertexBuffer buffer, IList<VertexPositionNormalColored> verts)
            => buffer.GetVertexStreamPositionNormalColored().AddVertices(verts);

        public static VertexPositionNormalColored ToRevit(this RenderVertex self)
            => new VertexPositionNormalColored(
                self.Position.ToRevit(),
                self.Normal.ToRevit(),
                self.RGBA.ToRevit());

        public static Matrix4x4 ToAra3D(this Transform self)
        {
            if (self.IsIdentity)
                return Matrix4x4.Identity;

            var o = self.Origin.ToAra3D();
            if (self.IsTranslation)
                return Matrix4x4.CreateTranslation(o);

            var x = self.BasisX.ToAra3D();
            var y = self.BasisY.ToAra3D();
            var z = self.BasisZ.ToAra3D();
            return new Matrix4x4(
                x.X, x.Y, x.Z, 0f,
                y.X, y.Y, y.Z, 0f,
                z.X, z.Y, z.Z, 0f,
                o.X, o.Y, o.Z, 1f);
        }

        public static TriangleMesh3D ToAra3D(this Mesh m)
        {
            if (m == null)
                return new([], []);

            var points = m.Vertices.Select(p => new Point3D((float)p.X, (float)p.Y, (float)p.Z)).ToList();
            var n = m.NumTriangles;
            var faces = new Integer3[n];
            for (var i = 0; i < n; i++)
            {
                var tri = m.get_Triangle(i);
                var v0 = (int)tri.get_Index(0);
                var v1 = (int)tri.get_Index(1);
                var v2 = (int)tri.get_Index(2);
                faces[i] = (v0, v1, v2);
            }

            return new(points, faces);
        }
    }
}
