using System.Diagnostics;
using Ara3D.Logging;
using Ara3D.Utils;

namespace Ara3D.Services
{
    /// <summary>
    /// This is a simple logging service. A design choice
    /// was made to not store arbitrary objects like Serilog does. 
    /// Normally we try to define abstractions for our services,
    /// and link against them. This allows us to easily
    /// have multiple implementations, and refactor code. 
    /// </summary>
    public interface ILoggingService : ILogger, IAggregateModelBackedService<LogEntry>
    { }

    /// <summary>
    /// This serves as an example of a service, and
    /// is such a common service that we just built it in. 
    /// </summary>
    public class LoggingService 
        : AggregateModelBackedService<LogEntry>, ILoggingService
    {
        public Stopwatch Stopwatch { get; } = Stopwatch.StartNew();

        public LoggingService(string name, IServiceManager app)
            : base(app)
        {
            Name = name;
        }

        public ILogger Log(string message, LogLevel level = LogLevel.None)
        {
            Repository.Add(new LogEntry(message, Name, level));
            Debug.WriteLine(Stopwatch.Elapsed.ToString("hh\\:mm\\:ss\\.ff") + " - " + message);
            return this;
        }

        public string Name { get; }
    }
}
