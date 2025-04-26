namespace Ara3D.Logging
{
    public enum JobStatus
    {
        /// <summary>
        /// Not yet started 
        /// </summary>
        NotStarted,
        
        /// <summary>
        /// In progress. 
        /// </summary>
        InProgress,

        /// <summary>
        /// Cancel requested by user or code
        /// </summary>
        CancelRequested,

        /// <summary>
        /// Stopped explicitly by user or code
        /// </summary>
        Cancelled,

        /// <summary>
        /// Stopped due to thrown exception
        /// </summary>
        Failed,

        /// <summary>
        /// Completed successfully
        /// </summary>
        Completed, 
    }
}