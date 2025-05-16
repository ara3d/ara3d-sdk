using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Ara3D.Utils
{
    /// <summary>
    /// Simplifies making an HTTP get request
    /// </summary>
    public static class HttpRequest
    {
        public static async Task<HttpResponseMessage> GetAsync(
            this Uri uri,
            CancellationToken token = default)
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(uri, token);
                response.EnsureSuccessStatusCode();
                return response;
            }
        }

        public static bool IsSuccess(this HttpStatusCode statusCode)
        {
            int code = (int)statusCode;
            return code >= 200 && code < 300;
        }
    }
}
