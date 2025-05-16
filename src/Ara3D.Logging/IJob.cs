using System.Collections.Generic;
using System.Threading.Tasks;


namespace Ara3D.Logging
{
    /// <summary>
    /// A job is a high-level named task that supports logging, cancellation, progress reporting, and error handling.
    /// This can be grouped, or represent a single task. 
    /// They can also be started, stopped.
    /// </summary>
    public interface IJob 
        : ICancelable, IProgress, ILogger, IStatus<JobStatus>, IErrorHandler, ICompletionHandler, IPausable
    {
        IReadOnlyList<IJob> SubJobs { get; }
        Task Start();
        object Result { get; }
    }
}