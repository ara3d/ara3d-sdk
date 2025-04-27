using Ara3D.Collections;
using Ara3D.Mathematics;
using Ara3D.Utils;

namespace Ara3D.Geometry
{
    public class PolyLineCurve<T> : ICurve<T>
    {
        public IPolyLine<T> PolyLine { get; }
        public float TotalLength { get; }
        public IArray<float> SegmentLengths { get; }
        public IArray<float> SegmentStarts { get; }
        public IArray<float> RelativeStarts { get; }
        public IArray<float> RelativeLengths  { get; }
        public bool Closed => PolyLine.Closed;

        public PolyLineCurve(IPolyLine<T> polyLine)
        {
            PolyLine = polyLine;
            SegmentLengths = PolyLine.Segments().Select(s => s.Length).Evaluate();
            SegmentStarts = SegmentLengths.PartialSums().Evaluate();
            Verifier.Assert(SegmentStarts.Count == SegmentLengths.Count + 1);
            Verifier.Assert(SegmentStarts[0].AlmostZero());
            TotalLength = SegmentStarts.Last();
            RelativeStarts = SegmentStarts.Select(x => x / TotalLength).Evaluate();
            RelativeLengths = SegmentLengths.Select(x => x / TotalLength).Evaluate();
        }

        public int CompareSegment(float f, int n)
        {
            if (n < 0) return n;
            var start = RelativeStarts[n];
            if (f < start) return -1;
            var end = RelativeStarts[n + 1];
            if (f > end) return 1;
            return 0;
        }

        public int FindSegmentIndex(float f)
            => RelativeStarts.BinarySearchIndex(i => CompareSegment(f, i));

        public T Eval(float x)
        {
            var i = FindSegmentIndex(x);
            if (i < 0)
                return PolyLine.Points[0];
            var seg = PolyLine.Segment(i);
            if (i >= PolyLine.Points.Count)
                return PolyLine.Points[PolyLine.Points.Count - 1];
            var start = RelativeStarts[i];
            var length = RelativeLengths[i];
            var relAmount = (x - start) / length;
            Verifier.Assert(relAmount >= 0);
            Verifier.Assert(relAmount <= 1);
            return seg.Lerp(relAmount);
        }
    }
}