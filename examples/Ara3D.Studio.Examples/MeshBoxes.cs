using System;
using System.Collections.Generic;
using System.Linq;

namespace Ara3D.Studio.Samples;

public class MeshBoxes : IModelModifier
{
    public bool Oriented;

    public TriangleMesh3D ToBoundsMesh(TriangleMesh3D mesh)
    {
        var bounds = mesh.Bounds;
        if (!Oriented)
            return PlatonicSolids.TriangulatedCube.Scale(bounds.Size).Translate(bounds.Center);
        var obb = OrientedBoxFit.Fit(mesh.Points.Map(m => m.Vector3));
        var matrix = obb.RotationMatrix;
        var q = Quaternion.CreateFromRotationMatrix(matrix);
        return PlatonicSolids.TriangulatedCube.Scale(bounds.Size).Rotate(q).Translate(bounds.Center);
    }

    public Model3D Eval(Model3D model, EvalContext context)
    {
        var boundsAsMeshes = model.Meshes.Select(ToBoundsMesh).ToList();
        return model.WithMeshes(boundsAsMeshes);
    }
}


/// <summary>
/// An oriented bounding box represented by center, orthonormal axes, and half lengths.
/// </summary>
public readonly record struct OrientedBox(
    Vector3 Center,
    Vector3 AxisX,
    Vector3 AxisY,
    Vector3 AxisZ,
    Vector3 HalfSize)
{
    public Matrix4x4 RotationMatrix
    {
        get
        {
            var x = AxisX;
            var y = AxisY;
            var z = AxisZ;

            // TODO: this neeeds to be a standlone function
            return new Matrix4x4(
                x.X, y.X, z.X, 0f,
                x.Y, y.Y, z.Y, 0f,
                x.Z, y.Z, z.Z, 0f,
                0f, 0f, 0f, 1f);
        }
    }

    public Matrix4x4 RigidTransform
        => RotationMatrix.WithTranslation(Center);

    public Quaternion Rotation
        => Quaternion.CreateFromRotationMatrix(RotationMatrix);
}

    public static class OrientedBoxFit
    {
        /// <summary>
        /// Returns the best-fit oriented bounding box for the given points.
        /// </summary>
        public static OrientedBox Fit(IReadOnlyList<Vector3> pts)
        {
            if (pts.Count == 0) throw new ArgumentException("Point list is empty.", nameof(pts));

            var mean = Mean(pts);
            var c = new double[3, 3];
            foreach (var p in pts)
            {
                var d = p - mean;
                c[0, 0] += d.X * d.X; 
                c[0, 1] += d.X * d.Y; 
                c[0, 2] += d.X * d.Z;
                c[1, 1] += d.Y * d.Y; 
                c[1, 2] += d.Y * d.Z;
                c[2, 2] += d.Z * d.Z;
            }
            var n = pts.Count;
            c[0, 0] /= n; 
            c[0, 1] /= n; 
            c[0, 2] /= n;
            c[1, 0] = c[0, 1]; 
            c[1, 1] /= n; 
            c[1, 2] /= n;
            c[2, 0] = c[0, 2]; 
            c[2, 1] = c[1, 2]; 
            c[2, 2] /= n;

            var eigVals = Eigen3x3.EigenValues(c);
            var axes = Enumerable
                .Range(0, 3)
                .OrderByDescending(i => eigVals[i].Real)
                .Select(i => EigenVector(c, eigVals[i].Real).Normalize)
                .ToArray();

            var ux = axes[0];
            var uy = axes[1];
            var uz = ux.Cross(uy); // enforce right-handed system

            /* ---------- 3. project points to axes ---------- */
            float minX = float.MaxValue, minY = float.MaxValue, minZ = float.MaxValue;
            float maxX = float.MinValue, maxY = float.MinValue, maxZ = float.MinValue;

            foreach (var p in pts)
            {
                var d = p - mean;
                float x = d.Dot(ux);
                float y = d.Dot(uy);
                float z = d.Dot(uz);

                if (x < minX) minX = x; if (x > maxX) maxX = x;
                if (y < minY) minY = y; if (y > maxY) maxY = y;
                if (z < minZ) minZ = z; if (z > maxZ) maxZ = z;
            }

            var half = new Vector3(
                (maxX - minX) * 0.5f,
                (maxY - minY) * 0.5f,
                (maxZ - minZ) * 0.5f);

            var center = mean
                       + ux * ((maxX + minX) * 0.5f)
                       + uy * ((maxY + minY) * 0.5f)
                       + uz * ((maxZ + minZ) * 0.5f);

            return new OrientedBox(center, ux, uy, uz, half);
        }

        private static Vector3 Mean(IReadOnlyList<Vector3> pts)
        {
            var sum = Vector3.Zero;
            foreach (var p in pts) sum += p;
            return sum / pts.Count;
        }

        /// <summary>
        /// Computes a unit eigen-vector for the real eigen-value lam of symmetric 3x3 matrix a.
        /// </summary>
        private static Vector3 EigenVector(double[,] a, double lam)
        {
            /* Build A - lam * I */
            double[,] m =
            {
                { a[0,0] - lam, a[0,1],       a[0,2]       },
                { a[1,0],       a[1,1] - lam, a[1,2]       },
                { a[2,0],       a[2,1],       a[2,2] - lam }
            };

            Vector3 r0 = new((float)m[0, 0], (float)m[0, 1], (float)m[0, 2]);
            Vector3 r1 = new((float)m[1, 0], (float)m[1, 1], (float)m[1, 2]);
            Vector3 r2 = new((float)m[2, 0], (float)m[2, 1], (float)m[2, 2]);

            var v = r0.Cross(r1);
            if (v.LengthSquared() < 1e-8f) v = r0.Cross(r2);
            if (v.LengthSquared() < 1e-8f) v = r1.Cross(r2);
            if (v.LengthSquared() < 1e-8f) v = Vector3.UnitX; // degenerate case

            return v.Normalize;
        }
    }

    public static class Eigen3x3
    {
        public static (double Real, double Imag)[] EigenValues(double[,] m)
        {
            double a00 = m[0, 0], a01 = m[0, 1], a02 = m[0, 2];
            double a10 = m[1, 0], a11 = m[1, 1], a12 = m[1, 2];
            double a20 = m[2, 0], a21 = m[2, 1], a22 = m[2, 2];

            var c2 = a00 + a11 + a22; // trace
            var trA2 =
                a00 * a00 + a01 * a10 + a02 * a20 +
                a10 * a01 + a11 * a11 + a12 * a21 +
                a20 * a02 + a21 * a12 + a22 * a22;
            var c1 = 0.5 * (c2 * c2 - trA2);
            var c0 =
                a00 * (a11 * a22 - a12 * a21) -
                a01 * (a10 * a22 - a12 * a20) +
                a02 * (a10 * a21 - a11 * a20);

            var p = c1 - c2 * c2 / 3.0;
            var q = 2.0 * c2 * c2 * c2 / 27.0 - c2 * c1 / 3.0 + c0;
            var disc = q * q / 4.0 + p * p * p / 27.0;

            static double Cbrt(double v) => v >= 0 ? Math.Pow(v, 1.0 / 3.0) : -Math.Pow(-v, 1.0 / 3.0);
            var eig = new (double, double)[3];

            if (disc > 0.0) // one real root, complex pair
            {
                var s = Math.Sqrt(disc);
                var u = Cbrt(-q / 2.0 + s);
                var v = Cbrt(-q / 2.0 - s);
                var x1 = u + v;

                eig[0] = (x1 + c2 / 3.0, 0.0);
                var real = -x1 / 2.0 + c2 / 3.0;
                var imag = Math.Sqrt(3.0) * (u - v) / 2.0;
                eig[1] = (real, imag);
                eig[2] = (real, -imag);
            }
            else // three real roots
            {
                var phi = Math.Acos(-q / 2.0 / Math.Sqrt(-p * p * p / 27.0));
                var t = 2.0 * Math.Sqrt(-p / 3.0);
                eig[0] = (t * Math.Cos(phi / 3.0) + c2 / 3.0, 0.0);
                eig[1] = (t * Math.Cos((phi + 2.0 * Math.PI) / 3.0) + c2 / 3.0, 0.0);
                eig[2] = (t * Math.Cos((phi + 4.0 * Math.PI) / 3.0) + c2 / 3.0, 0.0);
            }

            return eig;
        }
    }

