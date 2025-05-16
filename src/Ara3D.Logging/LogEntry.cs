using System;

namespace Ara3D.Logging
{
    public class LogEntry
    {
        public readonly DateTimeOffset DateTime = DateTimeOffset.Now;
        public readonly string Name = "";
        public readonly string Message = "";
        public readonly LogLevel Level = LogLevel.Info;

        public LogEntry() 
        { }

        public LogEntry(string message, string name = "", LogLevel level = LogLevel.Info)
        {
            Message = message;
            Name = name;
            Level = level;
        }
    }
}