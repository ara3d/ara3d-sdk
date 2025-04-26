using System;

namespace Ara3D.Utils
{
    public class ErrorData
    {
        public readonly Exception Exception;
        public readonly string Message;
        public readonly string HelpLink;
        public readonly string Type;
        public readonly bool Caught;
        public readonly int HResult;
        public readonly string StackTrace;

        public ErrorData(Exception e, bool caught = true)
        {
            Exception = e;
            Message = e.Message;
            HelpLink = e.HelpLink ?? "";
            Type = e.GetType().Name;
            Caught = caught;
            HResult = e.HResult;
            StackTrace = e.StackTrace ?? "";
        }
    }
}