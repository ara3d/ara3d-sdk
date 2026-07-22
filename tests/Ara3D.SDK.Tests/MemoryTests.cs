using Ara3D.Memory;

namespace Ara3D.SDK.Tests
{
    /// <summary>
    /// Regression coverage for UnmanagedList (studio-148 R6 / ara3d-127): growth must
    /// preserve contents, and Convert must transfer count/capacity in the right order.
    /// </summary>
    public static class MemoryTests
    {
        [Test]
        public static void AddGrowthPreservesContents()
        {
            using var list = new UnmanagedList<int>();
            for (var i = 0; i < 1000; i++)
                list.Add(i);

            Assert.That(list.Count, Is.EqualTo(1000));
            for (var i = 0; i < 1000; i++)
                Assert.That(list[i], Is.EqualTo(i));
        }

        [Test]
        public static void AddAtExactCapacityBoundary()
        {
            using var list = new UnmanagedList<int>(64);
            for (var i = 0; i < 65; i++)
                list.Add(i);

            Assert.That(list.Count, Is.EqualTo(65));
            Assert.That(list[63], Is.EqualTo(63));
            Assert.That(list[64], Is.EqualTo(64));
        }

        [Test]
        public static void BytesLengthTracksCount()
        {
            using var list = new UnmanagedList<int>();
            list.AddRange(new[] { 1, 2, 3 });

            Assert.That(list.Count, Is.EqualTo(3));
            Assert.That(list.Bytes.Count, Is.EqualTo(3 * sizeof(int)));
        }

        [Test]
        public static void SetCountThenClear()
        {
            using var list = new UnmanagedList<int>();
            list.SetCount(10);
            Assert.That(list.Count, Is.EqualTo(10));

            list.Clear();
            Assert.That(list.Count, Is.EqualTo(0));
            Assert.That(list.Bytes.Count, Is.EqualTo(0));
        }

        [Test]
        public static void CopyFromReplacesContents()
        {
            using var list = new UnmanagedList<int>();
            list.AddRange(new[] { 9, 9, 9, 9 });
            list.CopyFrom(new[] { 1, 2 } as IReadOnlyList<int>);

            Assert.That(list.Count, Is.EqualTo(2));
            Assert.That(list[0], Is.EqualTo(1));
            Assert.That(list[1], Is.EqualTo(2));
        }

        [Test]
        public static unsafe void ConvertReportsElementCountNotCapacity()
        {
            // Regression: Convert passed (count, capacity) to a (capacity, count)
            // constructor, so the converted list claimed capacity as its Count and
            // exposed garbage past the real data.
            var list = new UnmanagedList<int>();
            for (var i = 0; i < 10; i++)
                list.Add(i + 1);

            using var bytes = (UnmanagedList<byte>)list.Convert<byte>();

            Assert.That(bytes.Count, Is.EqualTo(10 * sizeof(int)));
            Assert.That(bytes[0], Is.EqualTo(1));
            Assert.That(bytes[4], Is.EqualTo(2));

            // Ownership moved: the source no longer holds the memory.
            Assert.That(list.Count, Is.EqualTo(0));
            Assert.That(list.Memory, Is.Null);
        }

        [Test]
        public static void ConvertToSameTypeReturnsSameInstance()
        {
            using var list = new UnmanagedList<int>();
            list.Add(42);
            Assert.That(list.Convert<int>(), Is.SameAs(list));
        }

        [Test]
        public static void ConvertRoundTripPreservesValues()
        {
            var ints = new UnmanagedList<int>();
            ints.AddRange(new[] { 10, 20, 30 });

            var bytes = (UnmanagedList<byte>)ints.Convert<byte>();
            using var back = (UnmanagedList<int>)bytes.Convert<int>();

            Assert.That(back.Count, Is.EqualTo(3));
            Assert.That(back[0], Is.EqualTo(10));
            Assert.That(back[1], Is.EqualTo(20));
            Assert.That(back[2], Is.EqualTo(30));
        }
    }
}
