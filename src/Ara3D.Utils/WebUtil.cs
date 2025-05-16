using System;
using System.Diagnostics;

namespace Ara3D.Utils
{
    public static class WebUtil
    {
        /// <summary>
        /// Returns true if the URI is valid.
        /// </summary>
        public static bool IsValidUri(string uri)
        {
            // see: https://stackoverflow.com/a/33573227
            if (!Uri.IsWellFormedUriString(uri, UriKind.Absolute))
                return false;
            Uri tmp;
            if (!Uri.TryCreate(uri, UriKind.Absolute, out tmp))
                return false;
            return tmp.Scheme == Uri.UriSchemeHttp || tmp.Scheme == Uri.UriSchemeHttps;
        }

        /// <summary>
        /// Opens the URI in the browser and returns true if the uri is valid.
        /// </summary>
        public static bool OpenUri(string uri)
        {
            // see: https://stackoverflow.com/a/33573227
            if (!WebUtil.IsValidUri(uri))
                return false;
            Process.Start(uri);
            return true;
        }

    }
}
