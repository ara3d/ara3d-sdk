using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Ara3D.Utils
{
    public static class StringUtil
    {
        public static bool IsNullOrEmpty(this string self)
            => string.IsNullOrEmpty(self);

        public static bool IsNullOrWhiteSpace(this string self)
            => string.IsNullOrWhiteSpace(self);

        public static string IfEmpty(this string self, string other)
            => self.IsNullOrWhiteSpace() ? other : self;

        public static string ElidedSubstring(this string self, int start, int length, int max)
            => (length > max) ? self.Substring(start, max) + "..." : self.Substring(start, length);

        public static string ToUIName(this string self)
        {
            if (string.IsNullOrEmpty(self))
                return "";
            var sb = new StringBuilder();
            for (var i = 0; i < self.Length; ++i)
            {
                if (i > 0 && char.IsUpper(self[i]))
                {
                    sb.Append(' ');
                }

                sb.Append(self[i]);
            }

            return sb.ToString();
        }

        public static string ToUtf8(this byte[] bytes)
            => Encoding.UTF8.GetString(bytes);

        public static string ToAscii(this byte[] bytes)
            => Encoding.ASCII.GetString(bytes);

        public static byte[] ToBytesUtf8(this string s)
            => Encoding.UTF8.GetBytes(s);

        public static byte[] ToBytesAscii(this string s)
            => Encoding.ASCII.GetBytes(s);

        public static string ToHex(this byte[] bytes, bool upperCase = false)
            => string.Join("", bytes.Select(b => b.ToString(upperCase ? "X2" : "x2")));

        public static string Base64ToHex(this string base64)
            => Convert.FromBase64String(base64).ToHex();

        public static string ToBase64(this byte[] bytes)
            => Convert.ToBase64String(bytes);

        public static string[] SplitAtNull(this string s)
            => s.Split('\0');

        public static (string, string) SplitAtIndex(this string s, int index)
            => (index >= 0 ? s.Substring(0, index) : "", s.Substring(index + 1));

        public static (string, string) SplitAtChar(this string s, char c)
            => s.SplitAtIndex(s.IndexOf(c));

        public static string[] SplitWhitespace(this string value)
            => value.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);

        public static string JoinEllided<T>(string separator, IEnumerable<T> values, int count = 5)
        {
            var tmp = values.Take(count).ToList();
            var prefix = string.Join(separator, tmp);
            if (tmp.Count < count)
                return prefix;
            var last = values.Last();
            return $"{prefix} ... {last}";
        }

        public static string JoinWithNull(this IEnumerable<string> strings)
            => string.Join("\0", strings);

        public static string JoinDistinct(this IEnumerable<string> strings, string delim = ";")
            => string.Join(delim, strings.Distinct().OrderBy(x => x));

        public static string Drop(this string s, int n)
            => s.Substring(0, Math.Max(0, s.Length - n));

        public static string JoinStrings<T1, T2>(this IDictionary<T1, T2> self, string sep = ";")
            => self.Select(kv => $"{kv.Key}={kv.Value}").JoinStrings(sep);

        public static string JoinStrings<T>(this IEnumerable<T> self, string sep = ";")
            => string.Join(sep, self);

        public static string JoinStringsWithComma<T>(this IEnumerable<T> self)
            => self.JoinStrings(", ");

        public static string JoinStringsWithNewLine<T>(this IEnumerable<T> self)
            => self.JoinStrings(Environment.NewLine);

        /// <summary>
        /// Remove starting and ending quotes.
        /// </summary>
        public static string StripQuotes(this string s)
            => s.Length >= 2 && s[0] == '"' && s[s.Length - 1] == '"' ? s.Substring(1, s.Length - 2) : s;

        /// <summary>
        /// Creates a string using the given quote delimiters. If endQuote is null, the beginQuote is used at the end as well.
        /// </summary>
        public static string Quote(this string s, string beginQuote = "\"", string endQuote = null)
            => $"{beginQuote}{s}{endQuote ?? beginQuote}";

        public static string ToHtmlAnchor(this string self)
            => string.IsNullOrEmpty(self) ? "_" : self.ToLowerInvariant().ReplaceNonAlphaNumeric("-");

        public static string ToIdentifier(this string self)
            => string.IsNullOrEmpty(self) ? "_" : self.ReplaceNonAlphaNumeric("_");

        public static string ReplaceNonAlphaNumeric(this string self, string replace)
            => Regex.Replace(self, "[^a-zA-Z0-9]", replace);

        public static string ToBitConverterLowerInvariant(this byte[] bytes)
            => BitConverter.ToString(bytes).Replace("-", string.Empty).ToLowerInvariant();

        public static Regex RegexUppercaseAcronym = new Regex("([A-Z]+)([A-Z][a-z])", RegexOptions.Compiled);
        public static Regex RegexUppercase = new Regex("([a-z0-9])([A-Z])", RegexOptions.Compiled);

        public static string SplitCamelCase(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;
            var result = RegexUppercaseAcronym.Replace(input, "$1 $2");
            result = RegexUppercase.Replace(result, "$1 $2");
            result = char.ToUpper(result[0]) + result.Substring(1);
            return result;
        }

        public static string LetterUpperCharsString = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public static string DigitCharsString = "0123456789";
        public static char[] LetterUpperChars = LetterUpperCharsString.ToCharArray();
        public static char[] LetterLowerChars = LetterUpperCharsString.ToLowerInvariant().ToCharArray();
        public static char[] DigitChars = DigitCharsString.ToCharArray();
        public static char[] LetterChars = LetterUpperChars.Concat(LetterLowerChars).ToArray();
        public static char[] DigitOrLetterChars = DigitChars.Concat(LetterChars).ToArray();
        public static Random Random = new Random();

        public static string CreateRandomIdentifier(int length)
        {
            var c = new char[length];
            for (var i = 0; i < length; ++i)
                c[i] = DigitOrLetterChars[Random.Next(DigitOrLetterChars.Length)];
            return new string(c);
        }

        public static string RemoveChars(this string text, string chars)
            => text.Where(c => !chars.Contains(c)).Aggregate(new StringBuilder(), (sb, c) => sb.Append(c)).ToString();

        public static string CapitalizeFirst(this string text)
            => text.IsNullOrEmpty() ? text : text[0].ToUpper() + text.Substring(1);

        public static string DecapitalizeFirst(this string text)
            => text.IsNullOrEmpty() ? text : text[0].ToLower() + text.Substring(1);

        /// <summary>
        /// Removes the end part of a string if it matches a sub-string.
        /// </summary>
        public static string RemoveSuffix(this string text, string subtext)
        {
            var i = text.LastIndexOf(subtext);
            if (i < 0) return text;
            return text.Substring(0, i);
        }

        /// <summary>
        /// Removes the start part of a string if it matches a sub-sting.
        /// </summary>
        public static string RemovePrefix(this string text, string subtext)
        {
            if (text.StartsWith(subtext))
                return text.Substring(subtext.Length);
            return text;
        }

        public static string EscapeQuotes(this string text)
            => text.Replace("\"", "\\\"");

        public static string UnescapeQuotes(this string text)
            => text.Replace("\\\"", "\"");
    }
}