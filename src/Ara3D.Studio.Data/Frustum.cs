using System.Numerics;

namespace Ara3D.Studio.Data
{
    public readonly struct Frustum
    {
        public readonly Plane Left;
        public readonly Plane Right;
        public readonly Plane Bottom;
        public readonly Plane Top;
        public readonly Plane Near;
        public readonly Plane Far;

        /// <summary>
        /// Constructs a Frustum by extracting planes from the provided view-projection matrix.
        /// </summary>
        public Frustum(Matrix4x4 m)
        {
            // Left Plane
            Left = new Plane(m.M14 + m.M11, m.M24 + m.M21, m.M34 + m.M31, m.M44 + m.M41);
            Left = NormalizePlane(Left);

            // Right Plane
            Right = new Plane(m.M14 - m.M11, m.M24 - m.M21, m.M34 - m.M31, m.M44 - m.M41);
            Right = NormalizePlane(Right);

            // Bottom Plane
            Bottom = new Plane(m.M14 + m.M12, m.M24 + m.M22, m.M34 + m.M32, m.M44 + m.M42);
            Bottom = NormalizePlane(Bottom);

            // Top Plane
            Top = new Plane(m.M14 - m.M12, m.M24 - m.M22, m.M34 - m.M32, m.M44 - m.M42);
            Top = NormalizePlane(Top);

            // Near Plane
            Near = new Plane(m.M14 + m.M13, m.M24 + m.M23, m.M34 + m.M33, m.M44 + m.M43);
            Near = NormalizePlane(Near);

            // Far Plane 
            Far = new Plane(m.M14 - m.M13, m.M24 - m.M23, m.M34 - m.M33, m.M44 - m.M43);
            Far = NormalizePlane(Far);
        }

        /// <summary>
        /// Normalizes a plane so that the normal vector has a length of 1.
        /// </summary>
        /// <param name="plane">The plane to normalize.</param>
        /// <returns>The normalized plane.</returns>
        private Plane NormalizePlane(Plane plane)
        {
            var magnitude = plane.Normal.Length();
            if (magnitude > float.Epsilon)
            {
                return new Plane(
                    plane.Normal / magnitude,
                    plane.D / magnitude
                );
            }

            // If the plane cannot be normalized, return it unchanged or handle accordingly
            throw new InvalidOperationException("Cannot normalize a plane with zero length normal.");
        }

        /// <summary>
        /// Checks if a point is inside the frustum.
        /// </summary>
        /// <param name="point">The point to test.</param>
        /// <returns>True if the point is inside all frustum planes; otherwise, false.</returns>
        public bool Contains(Vector3 point)
            => PlaneDistance(Left, point) >= 0 &&
                PlaneDistance(Right, point) >= 0 &&
                PlaneDistance(Bottom, point) >= 0 &&
                PlaneDistance(Top, point) >= 0 &&
                PlaneDistance(Near, point) >= 0 &&
                PlaneDistance(Far, point) >= 0;
        

        /// <summary>
        /// Checks if a bounding sphere is inside the frustum.
        /// </summary>
        /// <param name="center">The center of the sphere.</param>
        /// <param name="radius">The radius of the sphere.</param>
        /// <returns>True if the sphere is inside or intersects the frustum; otherwise, false.</returns>
        public bool IntersectsSphere(Vector3 center, float radius)
            => PlaneDistance(Left, center) >= -radius &&
                PlaneDistance(Right, center) >= -radius &&
                PlaneDistance(Bottom, center) >= -radius &&
                PlaneDistance(Top, center) >= -radius &&
                PlaneDistance(Near, center) >= -radius &&
                PlaneDistance(Far, center) >= -radius;
        
        /// <summary>
        /// Checks if an axis-aligned bounding box (AABB) is inside the frustum.
        /// </summary>
        public bool Intersects(Bounds bounds)
        {
            // For each plane, check if the AABB is completely on the negative side
            foreach (var plane in GetPlanes())
            {
                // Compute the positive vertex (farthest in the direction of the plane normal)
                var positive = new Vector3(
                    plane.Normal.X >= 0 ? bounds.Max.X : bounds.Min.X,
                    plane.Normal.Y >= 0 ? bounds.Max.Y : bounds.Min.Y,
                    plane.Normal.Z >= 0 ? bounds.Max.Z : bounds.Min.Z);

                if (PlaneDistance(plane, positive) < 0)
                {
                    // The AABB is completely outside this plane
                    return false;
                }
            }

            // The AABB is inside or intersects all planes
            return true;
        }

        /// <summary>
        /// Returns an array of all frustum planes.
        /// </summary>
        public Plane[] GetPlanes()
            => [Left, Right, Bottom, Top, Near, Far];
        
        /// <summary>
        /// Computes the signed distance from a plane to a point.
        /// </summary>
        public static float PlaneDistance(Plane plane, Vector3 point)
            => Vector3.Dot(plane.Normal, point) + plane.D;
    }
}