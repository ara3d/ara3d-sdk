using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ara3D.Logging
{
    // TODO: complete this job thing. Figure out the exact interface I want. 
    /*
    public class Job : IJob
    {
        public string Name { get; }
        public IJob PreviousJob { get; }
        public Task<object> Task { get; }
        public ILogger Logger { get; }

        public IJob GetFirstJob()
        {

        }

        public object Result { get; }
        public object PreviousResult => PreviousJob?.Result;

        public double PreviousProgress => PreviousJob?.TotalProgress ?? 0;
        public double CurrentProgress => PreviousProgress + LocalProgress;
        public double PreviousTotalProgress => PreviousJob?.TotalProgress ?? 0;
        public double TotalProgress => PreviousTotalProgress + LocalTotalProgress;
        public double LocalTotalProgress { get; }
        public double LocalProgress { get; private set; }
        public JobStatus StatusCode { get; private set; } = JobStatus.NotStarted;

        public event EventHandler<IProgress> ProgressChanged;

        public Job(string name, Func<object, object> function, IJob previousJob, ILogger logger, double progressLength)
        {
            Name = name;
            Task = new Task<object>(() => Result = function(PreviousResult));
            PreviousJob = previousJob;
            Logger = logger;
            LocalTotalProgress = progressLength;
        }

        public void UpdateProgress(double newProgress)
        {
            if (newProgress < LocalProgress)
                throw new Exception($"Progress {newProgress} should not be less than current {LocalProgress}");
            if (newProgress > LocalTotalProgress)
                throw new Exception($"Progress {newProgress} should not go above {LocalTotalProgress}");
            LocalProgress = newProgress;
        }

        public bool IsCancelRequested 
            => StatusCode == JobStatus.CancelRequested;

        public void Cancel()
        {
            if (StatusCode == JobStatus.InProgress || StatusCode == JobStatus.NotStarted)
            {
                StatusCode = JobStatus.CancelRequested;
            }
        }

        public ILogger Log(string message, LogLevel level)
            => Logger.Log(message, level);

        public static Job Create(string name, Func<object, object> function, IJob previous = null)
            => new Job(name, function, previous, previous );

        public static Job Create(string name, Action action, IJob previous = null)
            => Create(name, _ => null, previous);


        public event EventHandler<Exception> ExceptionHandler;
        
        public Exception Exception { get; }
        
        public event EventHandler<object> CompletionHandler;
        
        public void Start()
        {
            if (StatusCode == JobStatus.NotStarted)
                Task.Start();
        }
        
    }
    */
}