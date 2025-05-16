using System;
using System.Diagnostics;
using Ara3D.Utils;

namespace Ara3D.Logging
{
    public class LogWriter : ILogWriter
    {
        public DateTimeOffset Started { get; } = DateTimeOffset.Now;
        public TimeSpan CurrentTimeElapsed => DateTimeOffset.Now - Started;
        public Action<TimeSpan, LogEntry> OnLogEntry { get; }

        public LogWriter(Action<TimeSpan, LogEntry> onLogEntry)
            => OnLogEntry = onLogEntry;

        public static string FormatLogEntry(TimeSpan elapsed, LogEntry entry)
            => $"[{elapsed.ToFixedWidthTimeStamp()}] [{entry.Level}] {entry.Message}";

        public void Write(LogEntry logEntry)
            => OnLogEntry?.Invoke(CurrentTimeElapsed, logEntry);

        public static void DebugWriteLine(string msg)
        {
            Debug.WriteLine(msg);
            Console.WriteLine(msg);
        }

        public static void ConsoleWriteLine(string msg)
            => Console.WriteLine(msg);

        public static ILogWriter Create(Action<string> onLogMessage)
            => new LogWriter((elapsed, logEntry) 
                => onLogMessage(FormatLogEntry(elapsed, logEntry)));

        public static ILogWriter DebugWriter 
            => Create(DebugWriteLine);

        public static ILogWriter ConsoleWriter
            => Create(ConsoleWriteLine);

        public static ILogWriter NullWriter
            => Create(_ => { });
    }
}