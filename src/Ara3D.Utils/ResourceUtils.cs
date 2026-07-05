using System.IO;
using System.Text;

namespace Ara3D.Utils
{
    public static class ResourceUtils
    {
        public static string[] ResourceNames;
        
        static ResourceUtils()
        {
            ResourceNames = AssemblyUtil.MainAssembly.GetManifestResourceNames();
        }

        public static string GetResourceString(string resourceName)
        {
            using var str = GetResourceStream(resourceName);
            using var sr = new StreamReader(str, Encoding.UTF8, true);
            return sr.ReadToEnd();
        }

        public static Stream GetResourceStream(string resourceName)
        {
            var s = AssemblyUtil.MainAssembly.GetManifestResourceStream(resourceName);
            if (s == null)
                throw new FileNotFoundException($"Embedded resource not found: {resourceName}, manifest names are {ResourceNames.JoinStringsWithComma()}");
            return s;
        }
    }
}
