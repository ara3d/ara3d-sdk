namespace Ara3D.Studio.Samples;

public class MeshBoxes : IModelModifier
{
    public bool Oriented;
    public bool Disabled;
    public bool ApplyRotation;
    public bool TransposeMatrix;
    public bool SortEigenValues;

    public TriangleMesh3D ToBoundsMesh(TriangleMesh3D mesh)
    {
        var bounds = mesh.Bounds;
        if (!Oriented)
            return PlatonicSolids.TriangulatedCube.Scale(bounds.Size).Translate(bounds.Center);
        var obb = OrientedBoxFit.Fit(mesh.Points.Map(m => m.Vector3));
        var matrix = obb.RotationMatrix;
        Debug.Assert(Math.Abs(matrix.Determinant - 1f) < 1e-4f, "non-rigid matrix");
        if (TransposeMatrix)
            matrix = matrix.Transpose;
        var q = Quaternion.CreateFromRotationMatrix(matrix);
        Debug.Assert(!float.IsNaN(q.X), "bad quaternion – matrix not orthonormal?");

        Vector3 test = Vector3.UnitX * (obb.Size.X * 0.5f);
        Vector3 rotated = test.Transform(q);
        Debug.Assert(Math.Abs(rotated.Length() - test.Length()) < 1e-3f, "scale/shear sneaking in");

        return ApplyRotation 
            ? PlatonicSolids.TriangulatedCube.Scale(obb.Size).Rotate(q).Translate(obb.Center)
            : PlatonicSolids.TriangulatedCube.Scale(obb.Size).Translate(obb.Center);
    }

    public Model3D Eval(Model3D model, EvalContext context)
    {
        if (Disabled)
            return model;
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
    Vector3 Size)
{
    public Matrix4x4 RotationMatrix
    {
        get
        {
            var ux = AxisX.Normalize;
            var uy = AxisY.Normalize; 
            var uz = AxisZ.Normalize;
            if (Vector3.Dot(Vector3.Cross(ux, uy), uz) < 0f)
                uz = -uz;

            return new Matrix4x4(
                ux.X, ux.Y, ux.Z, 0f,
                uy.X, uy.Y, uy.Z, 0f,
                uz.X, uz.Y, uz.Z, 0f,
                0f,   0f,   0f,   1f);
        }
    }

    public Matrix4x4 RigidTransform
        => RotationMatrix.WithTranslation(Center);

    public Quaternion Rotation
        => Quaternion.CreateFromRotationMatrix(RotationMatrix);
}

public class Stats
{
    // Symmetric 3x3 matrix representing covariance
    public double c00;
    public double c01;
    public double c02;
    public double c11;
    public double c12;
    public double c22;

    // Sum of points
    public double sumX;
    public double sumY; 
    public double sumZ;

    // Centroid of points
    public double meanX;
    public double meanY;
    public double meanZ;

    // Computed eigen-values 
    public double eigenX;
    public double eigenY;
    public double eigenZ;

    // number of points 
    public int count; 

    public Stats(IReadOnlyList<Vector3> pts)
    {
        count = pts.Count;

        foreach (var pt in pts)
        {
            sumX += pt.X;
            sumY += pt.Y;
            sumZ += pt.Z;
        }

        meanX = sumX / pts.Count;
        meanY = sumY / pts.Count;
        meanZ = sumZ / pts.Count;

        foreach (var p in pts)
        {
            var dX = (double)p.X - meanX;
            var dY = (double)p.Y - meanY;
            var dZ = (double)p.Z - meanZ;

            c00 += dX * dX;
            c01 += dX * dY;
            c02 += dX * dZ;
            c11 += dY * dY;
            c12 += dY * dZ;
            c22 += dZ * dZ;
        }

        var invN = 1.0 / count;
        c00 *= invN;
        c01 *= invN;
        c02 *= invN;
        c11 *= invN;
        c12 *= invN; 
        c22 *= invN;

        (eigenX, eigenY, eigenZ) = EigenValues(c00, c01, c02, c11, c12, c22);
    }

    public static (double x, double y, double z) 
        EigenValues(
            double a00, double a01, double a02,
            double a11, double a12,
            double a22)
    {
        // coefficients of characteristic polynomial
        var c2 = a00 + a11 + a22;                          
        
        // trace
        var trA2 = a00 * a00 + 2 * a01 * a01 + 2 * a02 * a02 + a11 * a11 + 2 * a12 * a12 + a22 * a22;
        var c1 = 0.5 * (c2 * c2 - trA2);
        var c0 =
            a00 * (a11 * a22 - a12 * a12) -
            a01 * (a01 * a22 - a12 * a02) +
            a02 * (a01 * a12 - a11 * a02);

        var p = c1 - c2 * c2 / 3.0;
        var q = 2.0 * c2 * c2 * c2 / 27.0 - c2 * c1 / 3.0 + c0;
        var disc = q * q / 4.0 + p * p * p / 27.0;

        // Local function for compute the cubic root 
        double Cbrt(double v) 
            => v >= 0 ? Math.Pow(v, 1.0 / 3.0) 
                : -Math.Pow(-v, 1.0 / 3.0);

        // Eigenvalues
        double eX, eY, eZ;

        if (disc > 0.0)                                
        {
            // one real root, complex pair
            var s = Math.Sqrt(disc);
            var u = Cbrt(-q / 2.0 + s);
            var v = Cbrt(-q / 2.0 - s);
            var x1 = u + v;

            eX = (x1 + c2 / 3.0);
            eY = eZ = -x1 / 2.0 + c2 / 3.0;
        
            // identical real part for complex pair
        }
        else                                            
        {
            // three real roots
            var phi = Math.Acos(-q / 2.0 / Math.Sqrt(-p * p * p / 27.0));
            var t = 2.0 * Math.Sqrt(-p / 3.0);
            eX = (t * Math.Cos(phi / 3.0) + c2 / 3.0);
            eY = (t * Math.Cos((phi + 2.0 * Math.PI) / 3.0) + c2 / 3.0);
            eZ = (t * Math.Cos((phi + 4.0 * Math.PI) / 3.0) + c2 / 3.0);
        }

        return (eX, eY, eZ);
    }

    /// <summary>Non-normalized eigen-vector for symmetric 3×3 and eigen-value val.</summary>
    public Vector3 EigenVector(float val)
    {
        // Build A − λI                                                         
        var r0 = new Vector3((float)c00 - val, (float)c01, (float)c02);
        var r1 = new Vector3((float)c01, (float)c11 - val, (float)c12);
        var r2 = new Vector3((float)c02, (float)c12, (float)c22 - val);

        var v = Vector3.Cross(r0, r1);
        
        if (v.LengthSquared() < 1e-10f) v = Vector3.Cross(r0, r2);
        if (v.LengthSquared() < 1e-10f) v = Vector3.Cross(r1, r2);
        if (v.LengthSquared() < 1e-10f) v = Vector3.UnitX;     

        return v;
    }
}

public static class OrientedBoxFit
{
    /// <summary>Best-fit oriented bounding box of the supplied points.</summary>
    public static OrientedBox Fit(IReadOnlyList<Vector3> pts)
    {
        if (pts is null || pts.Count == 0)
            throw new ArgumentException("Point list is empty.", nameof(pts));

        var stats = new Stats(pts);
        var eigenVals = new[]
        {
            (float)stats.eigenX,
            (float)stats.eigenY,
            (float)stats.eigenZ
        };

        // Non-normalized eigen-vectors 
        var eigenVectors = eigenVals.Select(stats.EigenVector).ToArray();

        // --- build orthonormal frame -----------------------------------------------
        Vector3 ux = eigenVectors[0].Normalize;
        Vector3 uy = eigenVectors[1].Normalize;

        // If they are almost parallel, force an alternative axis:
        if (Vector3.Cross(ux, uy).LengthSquared() < 1e-6f)
            uy = ux.NormalizedCross(Math.Abs(ux.X) > 0.8f ? Vector3.UnitY : Vector3.UnitX);

        // Build a true orthonormal frame
        Vector3 uz = ux.NormalizedCross(uy);
        uy = uz.NormalizedCross(ux);   // re-compute to guarantee 90 °

        Debug.Assert(Math.Abs(Vector3.Dot(ux, uy)) < 1e-5f);
        Debug.Assert(Math.Abs(Vector3.Dot(ux, uz)) < 1e-5f);
        Debug.Assert(Math.Abs(Vector3.Dot(uy, uz)) < 1e-5f);

        // Keep it right-handed (now just a sign check)
        if (Vector3.Dot(Vector3.Cross(ux, uy), uz) < 0f)
            uz = -uz;
        // -----------------------------------------------------------------------

        var mean = new Vector3(
            (float)stats.meanX,
            (float)stats.meanY,
            (float)stats.meanZ);

        float minX = float.PositiveInfinity, maxX = float.NegativeInfinity;
        float minY = float.PositiveInfinity, maxY = float.NegativeInfinity;
        float minZ = float.PositiveInfinity, maxZ = float.NegativeInfinity;

        // Project every point onto the local PCA frame
        foreach (var p in pts)
        {
            Vector3 d = p - mean;          // work in mean-centred space

            float px = Vector3.Dot(d, ux);
            float py = Vector3.Dot(d, uy);
            float pz = Vector3.Dot(d, uz);

            if (px < minX) minX = px; if (px > maxX) maxX = px;
            if (py < minY) minY = py; if (py > maxY) maxY = py;
            if (pz < minZ) minZ = pz; if (pz > maxZ) maxZ = pz;
        }

        // Half-sizes of the box (extent along each principal axis)
        Vector3 halfSize = new Vector3(
            (maxX - minX) * 0.5f,
            (maxY - minY) * 0.5f,
            (maxZ - minZ) * 0.5f);

        // World-space centre
        Vector3 centre = mean
                         + ux * ((minX + maxX) * 0.5f)
                         + uy * ((minY + maxY) * 0.5f)
                         + uz * ((minZ + maxZ) * 0.5f);

        return new OrientedBox(centre, ux, uy, uz, halfSize * 2);
    }
}