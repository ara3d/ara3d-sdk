using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ara3D.Studio.Samples
{
    public class CurveCloneDemo : IModelGenerator
    {
        [Range(1, 100)] public int Count = 20;
        [Range(0.1f, 10f)] public float Revolutions = 4f;
        [Range(0.1f, 10f)] public float HelixHeight = 2f;
        [Range(0, 3)] public int Curve = 0;
        [Range(0.01, 100f)] public float Scale = 10;

        public Point3D Helix(Number n)
            => new Vector3(
                (n * Revolutions).Turns.Sin, 
                (n * Revolutions).Turns.Cos, 
                n * HelixHeight) * Scale;

        public Point3D Spiral(Number n)
            => new Vector3(
                (n * Revolutions).Turns.Sin, 
                (n * Revolutions).Turns.Cos, 0) * n * Scale;

        public Point3D Circle(Number n)
            => new Vector3(n.Turns.Sin, n.Turns.Cos, 0) * Scale;

        public Point3D SineWave(Number n)
            => new Vector3(n, 0, (n * Revolutions).Turns.Sin) * Scale;

        public Func<Number, Point3D> GetCurve(int curveIndex)
        {
            switch (curveIndex)
            {
                case 0: return Circle;
                case 1: return Spiral;
                case 2: return Helix;
                case 3: return SineWave;
            }

            return Circle; // Default to Circle if index is out of range
        }

        public Model3D Clone(TriangleMesh3D mesh, Material material, IReadOnlyList<Matrix4x4> transforms)
        {
            return new Model3D([mesh], [material], transforms,
                transforms.Count.MapRange(i => new ElementStruct(i, 0, 0, i)), null);
        }

        public Model3D CloneAlong(TriangleMesh3D mesh, Func<Number, Point3D> curveFunc, Integer count)
        {
            var transforms = count.LinearSpaceExclusive.Map(curveFunc).Map(p => Matrix4x4.CreateTranslation(p));
            return Clone(mesh, Material.Default, transforms);
        }

        public Model3D Eval(EvalContext context)
        {
            var mesh = PlatonicSolids.TriangulatedCube;
            var curve = GetCurve(Curve);
            return CloneAlong(mesh, curve, Count);
        }
    }
}
