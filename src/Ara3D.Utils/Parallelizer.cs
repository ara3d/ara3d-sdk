using System.Collections.Generic;
using System.Threading;
using System;

namespace Ara3D.Utils
{
    /// <summary>
    /// Allows efficient for loops 
    /// </summary>
    public static class Parallelizer
    {
        public class Options
        {
            public int MaxNumThreads = 32;
        }

        public static void ForLoop(int count, Action<int> processItem, Options options = null)
        {
            ForLoop(count, (from, to) =>
            {
                for (var i = from; i < to; ++i)
                    processItem(i);
            }, options);
        }

        public static void ForLoop(int count, Action<int, int> processRange, Options options = null)
        {
            void DoWork(object obj)
            {
                if (!(obj is Tuple<int, int> range)) 
                    throw new NullReferenceException(nameof(obj));
                processRange(range.Item1, range.Item2);
            }

            options = options ?? new Options();
            var rangeSize = count / options.MaxNumThreads;
            var threads = new List<Thread>();

            for (var i = 0; i < options.MaxNumThreads; ++i)
            {
                var start = i * rangeSize;
                var end = Math.Min(start + rangeSize, count);
                var thread = new Thread(DoWork);
                threads.Add(thread);
                thread.Start(Tuple.Create(start, end));
            }

            foreach (var thread in threads)
                thread.Join();
        }
    }
}