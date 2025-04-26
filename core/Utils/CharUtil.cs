namespace Ara3D.Utils
{
    public static class CharUtil
    {
        public static bool IsUpper(this char c)
            => char.IsUpper(c);

        public static bool IsLower(this char c)
            => char.IsLower(c);

        public static bool IsDigit(this char c)
            => char.IsDigit(c);

        public static bool IsControlChar(this char c)
            => char.IsControl(c);

        public static bool IsWhiteSpace(this char c)
            => char.IsWhiteSpace(c);

        public static bool IsLetter(this char c)
            => char.IsLetter(c);

        public static bool IsLetterOrDigit(this char c)
            => char.IsLetterOrDigit(c);

        public static char ToUpper(this char c)
            => char.ToUpperInvariant(c);

        public static char ToLower(this char c)
            => char.ToLowerInvariant(c);
    }
}