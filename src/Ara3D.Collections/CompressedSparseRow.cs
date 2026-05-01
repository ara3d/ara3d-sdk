using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ara3D.Collections;

namespace Ara3D.Utils
{
    public sealed class Csr<T> : IReadOnlyList<IReadOnlyList<T>>
    {
        public int Count { get; }
        public int[] Offsets { get; }
        public T[] Values { get; }

        public ReadOnlySpan<T> GetSpan(int row)
            => Values.AsSpan(Offsets[row], RowCount(row));

        public int RowCount(int row)
            => Offsets[row + 1] - Offsets[row];

        public IReadOnlyList<T> this[int index]
            => Values.SubArray(Offsets[index], RowCount(index));

        private Csr(int count, int[] offsets, T[] values)
        {
            Count = count;
            Offsets = offsets;
            Values = values;
        }

        public static Csr<T> Build(
            int rowCount,
            int itemCount,
            Func<int, int> getRow,
            Func<int, T> getValue,
            bool distinctPerRow = false)
        {
            if (rowCount < 0) throw new ArgumentOutOfRangeException(nameof(rowCount));
            if (itemCount < 0) throw new ArgumentOutOfRangeException(nameof(itemCount));

            var counts = new int[rowCount];
            var rows = new int[itemCount];

            for (var i = 0; i < itemCount; i++)
            {
                var row = getRow(i);

                if ((uint)row >= (uint)rowCount)
                    throw new InvalidOperationException(
                        $"CSR row {row} for item {i} is outside valid range [0, {rowCount}).");

                rows[i] = row;
                counts[row]++;
            }

            var offsets = new int[rowCount + 1];

            for (var row = 0; row < rowCount; row++)
                offsets[row + 1] = offsets[row] + counts[row];

            var values = new T[offsets[^1]];
            var cursors = new int[rowCount];
            Array.Copy(offsets, cursors, rowCount);

            for (var i = 0; i < itemCount; i++)
            {
                var row = rows[i];
                values[cursors[row]++] = getValue(i);
            }

#if DEBUG
            for (var row = 0; row < rowCount; row++)
                System.Diagnostics.Debug.Assert(cursors[row] == offsets[row + 1]);
#endif

            return distinctPerRow
                ? DistinctRows(rowCount, offsets, values)
                : new Csr<T>(rowCount, offsets, values);
        }

        private static Csr<T> DistinctRows(int rowCount, int[] oldOffsets, T[] oldValues)
        {
            var comparer = EqualityComparer<T>.Default;
            var newOffsets = new int[rowCount + 1];
            var temp = new List<T>(oldValues.Length);

            for (var row = 0; row < rowCount; row++)
            {
                var start = oldOffsets[row];
                var end = oldOffsets[row + 1];

                for (var i = start; i < end; i++)
                {
                    var value = oldValues[i];

                    var exists = false;
                    for (var j = newOffsets[row]; j < temp.Count; j++)
                    {
                        if (comparer.Equals(temp[j], value))
                        {
                            exists = true;
                            break;
                        }
                    }

                    if (!exists)
                        temp.Add(value);
                }

                newOffsets[row + 1] = temp.Count;
            }

            return new Csr<T>(rowCount, newOffsets, temp.ToArray());
        }

        public IEnumerator<IReadOnlyList<T>> GetEnumerator()
        {
            for (var i = 0; i < Count; i++)
                yield return this[i];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
