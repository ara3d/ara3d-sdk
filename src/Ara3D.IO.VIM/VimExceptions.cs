using System;

namespace Ara3D.IO.VIM
{
    public class ParsingException : Exception
    {
        public ParsingException(int errorCode, string message = null, Exception innerException = null)
            : base(message, innerException)
            => HResult = errorCode;

        public static ParsingException TokenizationException(string line, int numTokens)
            => new(HeaderTokenization, $"Unexpected number of tokens: {numTokens} ({line})");

        public static ParsingException DuplicateField(string field) =>
            new(HeaderDuplicateField, $"Duplicate field {field}");

        public static ParsingException ParsingField(string fieldName, string fieldValue)
            => new(HeaderFieldParsing,
                $"Failed to parse field {fieldName} with value {fieldValue}");

        public static ParsingException MissingFields(params string[] fieldNames) =>
            new(HeaderMissingFields, string.Join(", ", fieldNames));

        public const int VimDataFormatError = unchecked((int)0xA0003000);
        public const int HeaderTokenization = VimDataFormatError + 1;
        public const int HeaderDuplicateField = VimDataFormatError + 2;
        public const int HeaderFieldParsing = VimDataFormatError + 3;
        public const int HeaderMissingFields = VimDataFormatError + 4;
    }
}