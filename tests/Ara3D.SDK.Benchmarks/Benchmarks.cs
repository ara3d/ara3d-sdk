// Phase 0.5 baseline benchmarks for the Plato-generated geometry library.
// See docs/plato-roadmap.md (P0.5). These pin the *pre-optimizer* performance
// of the current Plato.Generated code, including the allocation costs of the
// IVectorLike MapComponents/ZipComponents machinery and lazy IReadOnlyList Maps.

using BenchmarkDotNet.Attributes;
using Ara3D.Geometry;

namespace Ara3D.SDK.Benchmarks
{
    /// <summary>
    /// (a) Vector3 arithmetic chain: (a+b)*0.5, Dot, Cross, Normalize over 10k vectors.
    /// These operations are intrinsics backed by System.Numerics.Vector3.
    /// </summary>
    [SimpleJob(warmupCount: 1, iterationCount: 3)]
    [MemoryDiagnoser]
    public class Vector3ArithmeticBenchmarks
    {
        private const int N = 10_000;
        private Vector3[] _a = null!;
        private Vector3[] _b = null!;

        [GlobalSetup]
        public void Setup()
        {
            var rng = new Random(1234);
            _a = new Vector3[N];
            _b = new Vector3[N];
            for (var i = 0; i < N; i++)
            {
                _a[i] = new Vector3((float)rng.NextDouble(), (float)rng.NextDouble(), (float)rng.NextDouble());
                _b[i] = new Vector3((float)rng.NextDouble() + 0.5f, (float)rng.NextDouble() + 0.5f, (float)rng.NextDouble() + 0.5f);
            }
        }

        [Benchmark]
        public float ArithmeticChain_10k()
        {
            var sum = 0f;
            for (var i = 0; i < N; i++)
            {
                var mid = (_a[i] + _b[i]) * 0.5f;
                var d = mid.Dot(_b[i]);
                var c = mid.Cross(_a[i]);
                var n = c.Normalize;
                sum += d + n.X;
            }
            return sum;
        }
    }

    /// <summary>
    /// (b) Component-wise ops on Vector3 over 10k vectors.
    /// Abs/Clamp are System.Numerics intrinsics; Lerp goes through the generated
    /// ZipComponents path; ClampZeroOne goes through the generated MapComponents
    /// path (Components.Map(lambda)) -- the allocation-heavy machinery P3 targets.
    /// </summary>
    [SimpleJob(warmupCount: 1, iterationCount: 3)]
    [MemoryDiagnoser]
    public class ComponentWiseBenchmarks
    {
        private const int N = 10_000;
        private Vector3[] _v = null!;
        private Vector3[] _w = null!;
        private Vector3 _lo;
        private Vector3 _hi;

        [GlobalSetup]
        public void Setup()
        {
            var rng = new Random(4321);
            _v = new Vector3[N];
            _w = new Vector3[N];
            for (var i = 0; i < N; i++)
            {
                _v[i] = new Vector3((float)rng.NextDouble() * 4 - 2, (float)rng.NextDouble() * 4 - 2, (float)rng.NextDouble() * 4 - 2);
                _w[i] = new Vector3((float)rng.NextDouble() * 4 - 2, (float)rng.NextDouble() * 4 - 2, (float)rng.NextDouble() * 4 - 2);
            }
            _lo = new Vector3(-1f, -1f, -1f);
            _hi = new Vector3(1f, 1f, 1f);
        }

        [Benchmark]
        public float Abs_10k()
        {
            var sum = 0f;
            for (var i = 0; i < N; i++)
                sum += _v[i].Abs.X;
            return sum;
        }

        [Benchmark]
        public float Lerp_10k()
        {
            var sum = 0f;
            for (var i = 0; i < N; i++)
                sum += _v[i].Lerp(_w[i], 0.25f).X;
            return sum;
        }

        [Benchmark]
        public float Clamp_10k()
        {
            var sum = 0f;
            for (var i = 0; i < N; i++)
                sum += _v[i].Clamp(_lo, _hi).X;
            return sum;
        }

        [Benchmark]
        public float ClampZeroOne_MapComponents_10k()
        {
            var sum = 0f;
            for (var i = 0; i < N; i++)
                sum += _v[i].ClampZeroOne.X;
            return sum;
        }
    }

    /// <summary>
    /// (c) Curve sampling: Circle.ToPolyLine2D(1000).
    /// The generated Sample uses a lazy LinearSpace.Map, so the point list is
    /// enumerated to force evaluation.
    /// </summary>
    [SimpleJob(warmupCount: 1, iterationCount: 3)]
    [MemoryDiagnoser]
    public class CurveSamplingBenchmarks
    {
        private Circle _circle;

        [GlobalSetup]
        public void Setup() => _circle = new Circle(new Point2D(0f, 0f), 1f);

        [Benchmark]
        public float CircleToPolyLine2D_1000()
        {
            var polyLine = _circle.ToPolyLine2D(1000);
            var pts = polyLine.Points;
            var sum = 0f;
            for (var i = 0; i < pts.Count; i++)
                sum += pts[i].X;
            return sum;
        }
    }

    /// <summary>
    /// (d) Bounds accumulation: fold 1,000,000 Point3D into a Bounds3D via Include.
    /// </summary>
    [SimpleJob(warmupCount: 1, iterationCount: 3)]
    [MemoryDiagnoser]
    public class BoundsAccumulationBenchmarks
    {
        private const int N = 1_000_000;
        private Point3D[] _points = null!;

        [GlobalSetup]
        public void Setup()
        {
            var rng = new Random(777);
            _points = new Point3D[N];
            for (var i = 0; i < N; i++)
                _points[i] = new Point3D(
                    (float)rng.NextDouble() * 100 - 50,
                    (float)rng.NextDouble() * 100 - 50,
                    (float)rng.NextDouble() * 100 - 50);
        }

        [Benchmark]
        public float BoundsInclude_1M()
        {
            var bounds = Bounds3D.Empty;
            for (var i = 0; i < N; i++)
                bounds = bounds.Include(_points[i]);
            return bounds.Min.X + bounds.Max.X;
        }
    }

    /// <summary>
    /// (e) Mesh deform: PlatonicSolids.Cube (QuadMesh3D) deformed by a point offset.
    /// Deform maps lazily over Points, so a second benchmark forces evaluation.
    /// </summary>
    [SimpleJob(warmupCount: 1, iterationCount: 3)]
    [MemoryDiagnoser]
    public class MeshDeformBenchmarks
    {
        private QuadMesh3D _cube;

        [GlobalSetup]
        public void Setup() => _cube = PlatonicSolids.Cube;

        [Benchmark]
        public QuadMesh3D Deform_LazyCall()
            => _cube.Deform(p => p + new Vector3(0.1f, 0f, 0f));

        [Benchmark]
        public float Deform_Evaluated()
        {
            var deformed = _cube.Deform(p => p + new Vector3(0.1f, 0f, 0f));
            var pts = deformed.Points;
            var sum = 0f;
            for (var i = 0; i < pts.Count; i++)
                sum += pts[i].X;
            return sum;
        }
    }
}
