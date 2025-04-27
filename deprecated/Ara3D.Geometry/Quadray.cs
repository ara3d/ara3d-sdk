using System.Collections.Generic;
using System.Linq;
using Ara3D.Mathematics;

namespace Ara3D.Geometry
{
    public class Quadray 
    {
        public Int4 Coordinates { get; }
     
        public Quadray(Int4 coordinates)
            => Coordinates = coordinates;

        public static readonly float Sqrt2 = 2.0f.Sqrt();
        public static Vector3 VectorX = (1, 0, -Sqrt2.Inverse());
        public static Vector3 VectorY = (-1, 0, -Sqrt2.Inverse());
        public static Vector3 VectorZ = (0, 1, Sqrt2.Inverse());
        public static Vector3 VectorW = (0, -1, Sqrt2.Inverse());

        public int X => Coordinates.X;
        public int Y => Coordinates.X;
        public int Z => Coordinates.X;
        public int W => Coordinates.X;

        public Vector3 Vector3
            => VectorX * X + VectorY * Y + VectorZ * Z + VectorW * W;

        public static implicit operator Int4(Quadray q) => q.Coordinates;
        public static implicit operator Quadray(Int4 coord) => new Quadray(coord);
    }

    public static class Extensions
    {
        public static IEnumerable<List<int>> Permutations(int size)
        {
            return Permutations(new List<int>(), Enumerable.Range(0, size).ToList());
        }

        public static IEnumerable<List<int>> Permutations(List<int> taken, List<int> remaining)
        {
            if (remaining.Count == 0)
                yield return taken;
            for (var i=0; i < remaining.Count; ++i)
            {
                var next = remaining[i];
                var newTaken = taken.Append(next).ToList();
                var newRemaining = new List<int>();
                if (i > 0)
                    newRemaining.AddRange(remaining.Take(i));
                if (i + 1 < remaining.Count)
                    newRemaining.AddRange(remaining.Skip(i + 1));
                foreach (var p in Permutations(newTaken, newRemaining))
                    yield return p;
            }
        }

        public static IEnumerable<Int4> Permutations(this Int4 input)
        {
            yield return (input.X, input.Y, input.Z, input.W);
            yield return (input.X, input.Y, input.W, input.Z);
            yield return (input.X, input.Z, input.W, input.Y);
            yield return (input.X, input.Z, input.Y, input.W);
            yield return (input.X, input.W, input.Z, input.Y);
            yield return (input.X, input.W, input.Y, input.Z);

            yield return (input.Y, input.X, input.Z, input.W);
            yield return (input.Y, input.X, input.W, input.Z);
            yield return (input.Y, input.Z, input.W, input.X);
            yield return (input.Y, input.Z, input.X, input.W);
            yield return (input.Y, input.W, input.Z, input.X);
            yield return (input.Y, input.W, input.X, input.Z);

            yield return (input.Z, input.Y, input.X, input.W);
            yield return (input.Z, input.Y, input.W, input.X);
            yield return (input.Z, input.X, input.W, input.Y);
            yield return (input.Z, input.X, input.Y, input.W);
            yield return (input.Z, input.W, input.X, input.Y);
            yield return (input.Z, input.W, input.Y, input.X);

            yield return (input.W, input.Y, input.Z, input.X);
            yield return (input.W, input.Y, input.X, input.Z);
            yield return (input.W, input.Z, input.X, input.Y);
            yield return (input.W, input.Z, input.Y, input.X);
            yield return (input.W, input.X, input.Z, input.Y);
            yield return (input.W, input.X, input.Y, input.Z);

        }
    }

    public static class PrimitiveQudrayShapes
    {

    }
}
