using System;
using System.Diagnostics;
using System.IO;

namespace Ara3D.Utils
{
    public static class TextWriterUtil
    {

        public static CustomTextWriter NewTextWriter(Action<char> onWriteChar)
            => new CustomTextWriter(onWriteChar);

        public static CustomTextWriter Tee(TextWriter original, TextWriter other)
            => new CustomTextWriter((c) =>
            {
                original.Write(c);
                other.Write(c);
            });

        public static CustomTextWriter DebugTextWriter()
            => new CustomTextWriter(c => Debug.Write(c));

        public static void CopyStdOutToDebug()
            => Console.SetOut(Tee(Console.Out, DebugTextWriter()));

        public static Disposer RedirectOut(TextWriter tw)
        {
            var original = Console.Out;
            Console.SetOut(tw);
            return new Disposer(() => Console.SetOut(original));
        }
    }
}