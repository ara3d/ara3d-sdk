namespace Ara3D.Logging
{
    public class Logger : ILogger
    {
        public string Name { get; }
        public ILogWriter Writer { get; }

        public Logger(ILogWriter writer, string name)
        {
            Writer = writer;
            Name = name ?? "";
        }

        public ILogger Log(string message, LogLevel level)
        {
            Writer.Write(new LogEntry(message, Name, level));
            return this;
        }

        public static ILogger Console
            = new Logger(LogWriter.ConsoleWriter, "");

        public static ILogger Debug
            = new Logger(LogWriter.DebugWriter, "");

        public static ILogger Null
            = new Logger(LogWriter.NullWriter, "");
    }
}