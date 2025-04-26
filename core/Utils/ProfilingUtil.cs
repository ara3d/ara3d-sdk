using System;
using System.Diagnostics;
using System.IO;

namespace Ara3D.Utils
{
    public static class ProfilingUtil
    {
        public static (T, TimeSpan) InvokeWithTiming<T>(this Func<T> function)
        {
            var sw = Stopwatch.StartNew();
            return (function(), sw.Elapsed);
        }

        public static (U, TimeSpan) InvokeWithTiming<T, U>(this Func<T, U> function, T input)
        {
            var sw = Stopwatch.StartNew();
            return (function(input), sw.Elapsed);
        }

        /// <summary>
        /// Outputs information about the current memory state to the passed textWriter, or standard out if null.
        /// </summary>
        public static void SnapshotMemory(TextWriter tw = null)
        {
            var process = Process.GetCurrentProcess();
            tw = tw ?? Console.Out;
            if (process != null)
            {
                var memsize = process.PrivateMemorySize64 >> 10;
                tw.WriteLine("Private memory: " + memsize.ToString("n"));
                memsize = process.WorkingSet64 >> 10;
                tw.WriteLine("Working set: " + memsize.ToString("n"));
                memsize = process.PeakWorkingSet64 >> 10;
                tw.WriteLine("Peak working set: " + memsize.ToString("n"));
            }
        }

        public static (long, long) GetMemoryConsumptionAndMSecElapsed(Action action)
        {
            var time = 0L;
            var mem = GetMemoryConsumption(
                () => time = TimingUtils.GetMSecElapsed(action));
            return (mem, time);
        }

        public static Disposer ReportMemoryConsumptionAndTimeElapsed()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            var memBefore = GC.GetTotalMemory(true);
            var sw = Stopwatch.StartNew();
            return new Disposer(() =>
            {
                TimingUtils.OutputTimeElapsed(sw, "Time Elapsed");
                GC.Collect();
                GC.WaitForPendingFinalizers();
                var memConsumption = GC.GetTotalMemory(true) - memBefore;
                Console.WriteLine($"Approximate memory consumption = {PathUtil.BytesToString(memConsumption)}");
            });
        }


        // NOTE: Calling a function generates additional memory
        public static long GetMemoryConsumption(Action action)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            var memBefore = GC.GetTotalMemory(true);
            action();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            return GC.GetTotalMemory(true) - memBefore;
        }

        public static T LoadFileAndReportStats<T>(string fileName, Func<string, T> loadFunc)
        {
            T file = default;
            var (mem, msec) = GetMemoryConsumptionAndMSecElapsed(() => file = loadFunc(fileName));

            // TODO: remove all console references
            Console.WriteLine(
                $"Loading {fileName}\nof size {PathUtil.GetFileSizeAsString(fileName)}\ntakes {TimingUtils.MSecToSecondsString(msec)}\nconsumes {PathUtil.BytesToString(mem)}");
            return file;
        }
    }
}
