using Ara3D.Buffers;

namespace Ara3D.Serialization.VIM
{
    public static class Extensions
    {
        public static string GetColumnNameFromBufferName(this string name)
        {
            var r = name.GetSimplifiedBufferName().Replace('.', ' ').TrimStart();
            var n = r.IndexOf(':');
            return n > 0 ? r.Substring(n + 1) : r;
        }

        public static string GetRelatedTableName(this string name)
        {
            var tablePrefix = "index:";
            if (name.StartsWith(tablePrefix))
            {
                var r = name.Substring(tablePrefix.Length);
                r = r.Substring(0, r.IndexOf(':'));
                return r.GetSimplifiedTableName();
            }
            return "";
        }

        public static string GetSimplifiedTableName(this string name)
            => name[(name.IndexOf('.') + 1)..];

        public static string GetSimplifiedBufferName(this string name)
            => name[(name.IndexOf(':') + 1)..];

        public static string GetSimplifiedBufferName(this INamedBuffer c)
            => c.Name.GetSimplifiedBufferName();
    }
}