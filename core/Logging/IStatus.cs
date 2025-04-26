namespace Ara3D.Logging
{
    public interface IStatus<TStatusCode> where TStatusCode : struct
    {
        TStatusCode StatusCode { get; }
    }
}