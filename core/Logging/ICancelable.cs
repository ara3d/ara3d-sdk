using System;

namespace Ara3D.Logging
{
    /// <summary>
    /// Combines a Cancellation event observer (CancellationToken) and a cancellation requester (CancelTokenSource). 
    /// Intentionally merging two very closely related concerns that rarely have advantage in being separated. 
    /// https://stackoverflow.com/questions/14215784/why-cancellationtoken-is-separate-from-cancellationtokensource
    /// https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken?view=netstandard-2.1
    /// https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtokensource?view=netstandard-2.1
    /// </summary>
    public interface ICancelable
    {
        bool IsCancelRequested { get; }
        void Cancel();
    }

    public class CancelException : Exception
    { }

    public static class CancelableUtil
    {
        public static T ThrowIfCanceled<T>(this T cancelable) where T : ICancelable
        {
            if (cancelable?.IsCancelRequested == true)
                throw new CancelException();
            return cancelable;
        }
    }
}
