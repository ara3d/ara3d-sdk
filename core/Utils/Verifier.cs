using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Ara3D.Utils
{
    /// <summary>
    /// Use this class as a replacement for debugger asserts, if you want to throw exceptions if certain conditions
    /// are false even in your run-time code. 
    /// </summary>
    public static class Verifier
    {
        public static void Assert<T>(T input,
            Predicate<T> predicate,
            string message = "",
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string fileName = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            Assert(predicate(input), () => $"{message}: predicate failed with input {input}", memberName, fileName, lineNumber);
        }

        public static void Assert(bool condition, string message, [CallerMemberName] string memberName = "",
            [CallerFilePath] string fileName = "", [CallerLineNumber] int lineNumber = 0)
        {
            Assert(condition, () => message, memberName, fileName, lineNumber);
        }

        public static void AssertAll<T>(IEnumerable<T> collection, Func<T, bool> condition, string message, [CallerMemberName] string memberName = "",
            [CallerFilePath] string fileName = "", [CallerLineNumber] int lineNumber = 0)
        {
            var i = 0;
            foreach (var x in collection)
            {
                Assert(condition(x), $"Element {i} ({x}) failed assertion: {message}", memberName, fileName,
                    lineNumber);
                i++;
            }
        }

        public static void AssertEquals(object value, object expected, string message = "", [CallerMemberName] string memberName = "",
            [CallerFilePath] string fileName = "", [CallerLineNumber] int lineNumber = 0)
        {
            Assert(value?.Equals(expected) == true, () => $"Value {value} not equal to expected {expected}: {message}", memberName, fileName, lineNumber);
        }

        public static void AssertNotNull(object obj, string name,
            [CallerMemberName] string memberName = "", [CallerFilePath] string fileName = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            Assert(obj != null, $"{name} was null", memberName, fileName, lineNumber);
        }

        public static void Assert(bool condition, Func<string> messageGen = null,  [CallerMemberName] string memberName = "", [CallerFilePath] string fileName = "", [CallerLineNumber] int lineNumber = 0)
        {
            if (!condition)
            {
                var msg = messageGen != null ? $" with message [{messageGen?.Invoke()}]" : "";
                throw new Exception(
                    $"Assertion failed{msg} in member {memberName} at line {lineNumber} in file {fileName}");
            }
        }
    }
}
