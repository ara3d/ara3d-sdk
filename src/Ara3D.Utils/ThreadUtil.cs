using System;
using System.Collections.Generic;
using System.Threading;

namespace Ara3D.Utils
{
    public static class ThreadUtil
    {
        public static void RunThreads(int threadCount, Action<int> action)
        {
            var threads = new List<Thread>();
            for (var i = 0; i < threadCount; i++)
            {
                var thread = new Thread(startData => action((int)startData));
                threads.Add(thread);
                thread.Start(i);
            }

            foreach (var t in threads)
            {
                t.Join();
            }
        }
    }
}