namespace Ara3D.Studio.Samples;

public class ClonePyramidOnFace : IModelModifier
{
    public static IReadOnlyList<Quad3D> ToQuads(IReadOnlyList<Triangle3D> triangles)
    {
        var r = new List<Quad3D>();
        for (var i = 0; i < triangles.Count; i += 2)
        {
            var a = triangles[i];
            var b = triangles[i + 1];
            var q = new Quad3D(a.A, a.B, b.A, b.B);
            r.Add(q);
        }

        return r;
    }

    /// <summary>
    /// Returns a quaternion that rotates the model's Z axis (0,0,1) to align with the given direction.
    /// </summary>
    public static Quaternion AlignZAxisWith(Vector3 targetZ)
    {
        targetZ = targetZ.Normalize;
        var currentZ = Vector3.UnitZ;

        if (currentZ.Distance(targetZ) < 1e-6f)
            return Quaternion.Identity;

        if (currentZ.Distance(-targetZ) < 1e-6f)
        {
            // 180-degree rotation around any axis perpendicular to Z
            var axis = currentZ.NormalizedCross(Vector3.UnitX);
            if (axis.LengthSquared() < 1e-6f)
                axis = currentZ.NormalizedCross(Vector3.UnitY);
            return Quaternion.CreateFromAxisAngle(axis, MathF.PI);
        }

        var rotationAxis = currentZ.NormalizedCross(targetZ);
        var angle = currentZ.Dot(targetZ).Clamp(-1.0f, 1.0f).Acos;
        return Quaternion.CreateFromAxisAngle(rotationAxis, angle);
    }

    public static Matrix4x4 AlignToQuad(Quad3D q)
        => AlignZAxisWith(q.Normal) * Matrix4x4.CreateTranslation(q.Center);

    public Model3D Eval(Model3D model3D, EvalContext context)
    {
        var firstMesh = model3D.Meshes[0];
        var firstMat = model3D.Materials[0];
        var quads = ToQuads(firstMesh.Triangles);
        var mesh = PlatonicSolids.Tetrahedron;
        var transforms = quads.Map(AlignToQuad);
        return mesh.Clone(firstMat, transforms);
    }
}