namespace Ara3D.WorkItems;

/// <summary>
/// This class provides call back opportunities for work queue on each item processed.
/// You can use it to do profiling, logging, debug-tracing, and to centralize error
/// handling. None of these functions should throw errors, if they do they will be caught
/// and processed.  
/// </summary>
public interface IWorkItemListener
{
    /// <summary>
    /// Called once a work item is extracted from the queue and about to be executed. 
    /// </summary>
    void OnWorkStarted(IWorkItemQueue queue, WorkItem work);

    /// <summary>
    /// Called once a work item is completed successfully . 
    /// </summary>
    void OnWorkCompleted(IWorkItemQueue queue, WorkItem work);

    /// <summary>
    /// Called if a work item is started but fails to complete successfully. 
    /// </summary>
    void OnWorkError(IWorkItemQueue queue, WorkItem work, Exception ex);
}