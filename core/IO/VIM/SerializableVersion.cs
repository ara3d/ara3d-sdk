using System;
using System.Linq;

namespace Ara3D.Serialization.VIM
{
    // We've defined a serializable version since the System.Version type is not readily serializable by Newtonsoft.Json
    [Serializable]
    public class SerializableVersion
    {
        public int Major;
        public int Minor;
        public int Patch;
        public string Qualifier;

        public override string ToString()
            => string.IsNullOrWhiteSpace(Qualifier)
                ? string.Join(".", Major, Minor, Patch)
                : string.Join(".", Major, Minor, Patch, Qualifier);

        /// <summary>
        /// Returns a best-effort parse of the given input.
        /// </summary>
        public static bool TryParse(string input, out SerializableVersion result)
        {
            result = null;

            if (string.IsNullOrEmpty(input))
                return false;

            var tokens = input.Split('.')
                .Select(s => s.Trim())
                .Where(s => !string.IsNullOrEmpty(s))
                .ToArray();

            if (tokens.Length == 0)
                return false;

            result = new SerializableVersion();

            // Token 0 - Major value (required; see .Length == 0 condition above)
            if (tokens.Length > 0)
            {
                if (!int.TryParse(tokens[0], out result.Major))
                    return false;
            }

            // Token 1 - Minor value (optional)
            if (tokens.Length > 1)
            {
                if (!int.TryParse(tokens[1], out result.Minor))
                    return false;
            }

            // Token 2 - Patch value (optional)
            if (tokens.Length > 2)
            {
                if (!int.TryParse(tokens[2], out result.Patch))
                    return false;
            }

            // Token 3 - Qualifier (optional)
            if (tokens.Length > 3)
            {
                result.Qualifier = tokens[3];
            }

            return true;
        }

        public static SerializableVersion Parse(string input)
            => TryParse(input, out var result) ? result : null;

        public const int UnknownValue = -1;

        public static SerializableVersion Unknown
            => new SerializableVersion { Major = UnknownValue, Minor = UnknownValue, Patch = UnknownValue };

        public bool IsEqualTo(SerializableVersion other)
            => Major == other.Major && Minor == other.Minor && Patch == other.Patch && Qualifier == other.Qualifier;

        public bool IsLessThan(SerializableVersion other)
        {
            if (Major < other.Major)
                return true;
            
            if (Major == other.Major && Minor < other.Minor)
                return true;
            
            if (Major == other.Major && Minor == other.Minor && Patch < other.Patch)
                return true;

            var hasQualifier = !string.IsNullOrEmpty(Qualifier);
            var otherHasQualifier = !string.IsNullOrEmpty(other.Qualifier);

            if (!hasQualifier && otherHasQualifier)
                return true;

            if (hasQualifier && !otherHasQualifier)
                return false;

            return string.Compare(Qualifier, other.Qualifier, StringComparison.InvariantCultureIgnoreCase) < 0;
        }

        public bool IsGreaterThanOrEqual(SerializableVersion other)
            => IsEqualTo(other) || other.IsLessThan(this);
    }

    public static class SerializableVersionExtensions
    {
        public static System.Version ToVersion(this SerializableVersion version)
            => new System.Version(version.Major, version.Minor, version.Patch);

        public static SerializableVersion ToSerializableVersion(this System.Version version)
            => new SerializableVersion { Major = version.Major, Minor = version.Minor, Patch = version.Build };
    }
}
