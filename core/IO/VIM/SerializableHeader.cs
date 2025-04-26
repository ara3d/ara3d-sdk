using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Ara3D.Serialization.VIM
{
    public struct SerializableHeader
    {
        public const string FormatVersionField = "vim";
        public const string IdField = "id";
        public const string RevisionField = "revision";
        public const string GeneratorField = "generator";
        public const string CreationDateField = "created";
        public const string SchemaField = "schema";
        public const string BuildField = "build"; // optional

        public const char Separator = '=';
        public const char EndOfLineChar = '\n';
        public const string EndOfLineString = "\n";
        public const string PersistingIdSeparator = "::";
        public const string DummyPersistingIdPrefix = "unknown_";

        public static readonly string[] RequiredFields =
        {
            FormatVersionField,
            IdField,
            RevisionField,
            GeneratorField,
            CreationDateField,
            SchemaField,
        };

        public readonly SerializableVersion FileFormatVersion;
        public readonly Guid Id;
        public readonly Guid Revision;
        public readonly string Generator;
        public readonly DateTime CreationDate; // NOTE: should be parsed and written using the Invariant culture.
        public readonly SerializableVersion Schema;
        public readonly IReadOnlyDictionary<string, string> Values; // Optional values.

        /// <summary>
        /// Private constructor
        /// </summary>
        private SerializableHeader(
            SerializableVersion fileFormatVersion,
            Guid id,
            Guid revision,
            string generator,
            DateTime creationDate,
            SerializableVersion schema,
            IReadOnlyDictionary<string, string> values)
        {
            FileFormatVersion = fileFormatVersion;
            Id = id;
            Revision = revision;
            Generator = generator;
            CreationDate = creationDate;
            Schema = schema;
            Values = values;
        }

        /// <summary>
        /// Returns a new dictionary containing optional values. Applied upon serialization.
        /// </summary>
        private static IReadOnlyDictionary<string, string> AddOptionalValues(
            IReadOnlyDictionary<string, string> dictionary,
            string versionString)
        {
            var d = dictionary.ToDictionary(
                kv => kv.Key,
                kv => kv.Value);

            if (!d.ContainsKey(BuildField))
                d.Add(BuildField, versionString);

            return d;
        }

        /// <summary>
        /// Constructor used during serialization of a new VIM.
        /// </summary>
        public SerializableHeader(string generator, SerializableVersion schema, string versionString, IReadOnlyDictionary<string, string> values = null)
            : this(
            new SerializableVersion(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            generator,
            DateTime.UtcNow,
            schema,
            AddOptionalValues(values ?? new Dictionary<string, string>(), versionString))
        { }

        /// <summary>
        /// Parses the input. Throws exceptions if the input does not define a correctly formatted header.
        /// </summary>
        public static SerializableHeader Parse(string input)
        {
            var lines = input.Split(EndOfLineChar)
                .Where(str => !string.IsNullOrEmpty(str));

            SerializableVersion fileFormatVersion = null;
            Guid? id = null;
            Guid? revision = null;
            string generator = null;
            DateTime? creationDate = null;
            SerializableVersion schema = null;
            var values = new Dictionary<string, string>();

            var requiredSet = new HashSet<string>(RequiredFields);

            foreach (var line in lines)
            {
                var tokens = line.Split(Separator);
                var numTokens = tokens.Length;
                
                // skip empty lines.
                if (numTokens == 0)
                    continue;

                if (numTokens != 2)
                    throw ParsingException.TokenizationException(line, numTokens);

                var fieldName = tokens[0];
                var fieldValue = tokens[1];

                requiredSet.Remove(fieldName);

                switch (fieldName)
                {
                    case FormatVersionField:
                        {
                            if (fileFormatVersion != null)
                                throw ParsingException.DuplicateField(fieldName);

                            if (!SerializableVersion.TryParse(fieldValue, out fileFormatVersion))
                                throw ParsingException.ParsingField(fieldName, fieldValue);

                            break;
                        }
                    case IdField:
                        {
                            if (id != null)
                                throw ParsingException.DuplicateField(fieldName);

                            if (!Guid.TryParse(fieldValue, out var result))
                                throw ParsingException.ParsingField(fieldName, fieldValue);

                            id = result;

                            break;
                        }
                    case RevisionField:
                        {
                            if (revision != null)
                                throw ParsingException.DuplicateField(fieldName);

                            if (!Guid.TryParse(fieldValue, out var result))
                                throw ParsingException.ParsingField(fieldName, fieldValue);

                            revision = result;
                            break;
                        }
                    case GeneratorField:
                        {
                            if (generator != null)
                                throw ParsingException.DuplicateField(fieldName);

                            generator = fieldValue;

                            break;
                        }
                    case CreationDateField:
                        {
                            if (creationDate != null)
                                throw ParsingException.DuplicateField(fieldName);

                            try
                            {
                                var parsed = DateTime.Parse(fieldValue, CultureInfo.InvariantCulture);
                                creationDate = parsed;
                            }
                            catch
                            {
                                throw ParsingException.ParsingField(fieldName, fieldValue);
                            }

                            break;
                        }
                    case SchemaField:
                        {
                            if (schema != null)
                                throw ParsingException.DuplicateField(fieldName);

                            if (!SerializableVersion.TryParse(fieldValue, out schema))
                                throw ParsingException.ParsingField(fieldName, fieldValue);

                            break;
                        }
                    default:
                        // An unknown value; add it to the values dictionary.
                        values[fieldName] = fieldValue;
                        break;
                }
            }

            // Ensure all required fields have been accounted for.
            if (requiredSet.Count > 0)
                throw ParsingException.MissingFields(requiredSet.ToArray());

            return new SerializableHeader(
                fileFormatVersion,
                id ?? Guid.Empty,
                revision ?? Guid.Empty,
                generator,
                creationDate ?? default,
                schema,
                values);
        }

        /// <summary>
        /// Returns the string representation of the VIM header.
        /// </summary>
        public override string ToString()
        {
            var fields = new List<(string Key, string Value)>
            {
                (FormatVersionField, FileFormatVersion.ToString()),
                (IdField, Id.ToString()),
                (RevisionField, Revision.ToString()),
                (GeneratorField, Generator),
                (CreationDateField, CreationDate.ToString(CultureInfo.InvariantCulture)),
                (SchemaField, Schema.ToString()),
            };

            foreach (var kv in Values)
                fields.Add((kv.Key, kv.Value));

            return string.Join(EndOfLineString, fields.Select(f => $"{f.Key}{Separator}{f.Value}"));
        }

        public override bool Equals(object obj)
            => obj is SerializableHeader other && ToString() == other.ToString();

        public override int GetHashCode()
            => ToString().GetHashCode();

        public static string CreatePersistingId(Guid id, Guid revision)
            => string.Join(PersistingIdSeparator, id.ToString(), revision.ToString());

        /// <summary>
        /// Used to generate an unknown persistence ID to avoid id collisions with other unknown references.
        /// </summary>
        public static string CreateDummyPersistingId()
            => string.Join(PersistingIdSeparator, $"{DummyPersistingIdPrefix}{Guid.NewGuid()}", $"{DummyPersistingIdPrefix}{Guid.NewGuid()}");

        /// <summary>
        /// The PersistingId is the combination of the Id and the Revision values.<br/>
        /// This value is used to persist a reference to this VIM file.<br/>
        /// <br/>
        /// When a VIM file is updated, the relevant Guids may evolve in either of the following ways:
        /// <list type="bullet">
        /// <item>The Id and the Revision are both completely regenerated.</item>
        /// <item>The Id is preserved for continuity, and the Revision is regenerated.</item>
        /// </list>
        /// </summary>
        public string PersistingId
            => CreatePersistingId(Id, Revision);
    }
}
