using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ara3D.IO.StepParser;
using Ara3D.Memory;

namespace Ara3D.IO.IfcParser
{
    public static class Extensions
    {
        public static void Add<TKey, TValue>(this IDictionary<TKey, List<TValue>> self, TKey key, TValue value)
        {
            if (!self.ContainsKey(key))
                self[key] = new List<TValue>();
            self[key].Add(value);
        }

        public static uint AsId(this StepValue value)
            => value is StepValueUnassigned
                ? 0u
                : ((StepValueId)value).Id;

        public static string AsString(this StepValue value)
            => value is StepValueString ss ? ss.AsString() :
                value is StepValueNumber sn ? sn.Value.ToString() :
                value is StepValueId si ? si.Id.ToString() :
                value is StepValueSymbol ssm ? ssm.Name.ToString() :
                "";

        public static double AsNumber(this StepValue value)
            => value is StepValueUnassigned
                ? 0
                : ((StepValueNumber)value).Value;

        public static List<StepValue> AsList(this StepValue value)
            => value is StepValueUnassigned
                ? new List<StepValue>()
                : ((StepValueList)value).Values;

        public static List<uint> AsIdList(this StepValue value)
            => value is StepValueUnassigned
                ? new List<uint>()
                : value.AsList().Select(AsId).ToList();

        // Uses Latin1 encoding (aka ISO-8859-1)
        // Extended characters converted using an IFC specific system 
        public static string AsString(this ByteSlice slice)
            => Encoding.Latin1.GetString(slice.AsSpan()).IfcToUnicode();

        // https://technical.buildingsmart.org/resources/ifcimplementationguidance/string-encoding/
        public static string IfcToUnicode(this string input)
        {
            if (!input.Contains('\\'))
                return input;

            var output = new StringBuilder();
            var i = 0;
            var length = input.Length;
            while (i < length)
            {
                if (input[i] != '\\')
                {
                    // Regular character, append to output
                    output.Append(input[i++]);
                    continue;
                }

                i++; // Move past '\'
                if (i >= length)
                {
                    output.Append('\\');
                    break;
                }

                var escapeChar = input[i++];

                if (escapeChar == 'S' && i < length && input[i] == '\\')
                {
                    i++; // Move past '\'
                    if (i < length)
                    {
                        var c = input[i++];
                        var code = c + 128;
                        output.Append((char)code);
                    }
                    else
                    {
                        output.Append("\\S\\");
                    }
                    continue;
                }
                
                if (escapeChar == 'X')
                {
                    if (i < length && input[i] == '\\')
                    {
                        // Handle \X\XX escape sequence (8-bit character code)
                        i++; // Move past '\'
                        if (i + 1 < length)
                        {
                            var hex = input.Substring(i, 2);
                            i += 2;
                            var code = Convert.ToInt32(hex, 16);
                            output.Append((char)code);
                        }
                        else
                        {
                            output.Append("\\X\\");
                        }

                        continue;
                    }

                    // Handle extended \Xn\...\X0\ escape sequence
                    // Skip 'n' until the next '\'
                    while (i < length && input[i] != '\\')
                        i++;
                    if (i < length)
                        i++; // Move past '\'

                    // Collect hex digits until '\X0\'
                    var hexDigits = new StringBuilder();
                    while (i + 3 <= length && input.Substring(i, 3) != "\\X0")
                    {
                        hexDigits.Append(input[i++]);
                    }

                    if (i + 3 <= length && input.Substring(i, 3) == "\\X0")
                    {
                        i += 3; // Move past '\X0'
                        if (i < length && input[i] == '\\')
                            i++; // Move past '\'

                        var hexStr = hexDigits.ToString();

                        // Process hex digits in chunks of 4 (representing Unicode code points)
                        for (var k = 0; k + 4 <= hexStr.Length; k += 4)
                        {
                            var codeHex = hexStr.Substring(k, 4);
                            var code = Convert.ToInt32(codeHex, 16);
                            output.Append(char.ConvertFromUtf32(code));
                        }
                        continue;
                    }

                    // Invalid format, append as is
                    output.Append("\\X");
                    continue;
                }

                // Unrecognized escape sequence, append as is
                output.Append('\\').Append(escapeChar);
            }

            return output.ToString();
        }

        public static string AsString(this StepValueString ss)
            => ss.Value.AsString();

        public static object ToJsonObject(this StepValue sv)
        {
            switch (sv)
            {
                case StepValueEntity stepEntity:
                {
                    var attr = stepEntity.Attributes;
                    if (attr.Values.Count == 0)
                        return stepEntity.ToString();

                    if (attr.Values.Count == 1)
                        return attr.Values[0].ToJsonObject();
                    
                    return attr.Values.Select(ToJsonObject).ToList();
                }

                case StepValueId stepId:
                    return stepId.Id;

                case StepValueList stepList:
                    return stepList.Values.Select(ToJsonObject).ToList();

                case StepValueNumber stepNumber:
                    return stepNumber.AsNumber();

                case StepValueRedeclared stepRedeclared:
                    return null;
                
                case StepValueString stepString:
                    return stepString.AsString();
                
                case StepValueSymbol stepSymbol:
                    var tmp = stepSymbol.Name.AsString();
                    if (tmp == "T")
                        return true;
                    if (tmp == "F")
                        return false;
                    return tmp;
                
                case StepValueUnassigned stepUnassigned:
                    return null;

                default:
                    throw new ArgumentOutOfRangeException(nameof(sv));
            }
        }
    }
}
