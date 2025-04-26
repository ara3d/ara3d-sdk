using System;

namespace Ara3D.Utils
{
    public static class ConsoleCodes
    {
        public static string NL = Environment.NewLine; // shortcut
        public const string NORMAL = "\x1b[39m";
        public const string RED = "\x1b[91m";
        public const string GREEN = "\x1b[92m";
        public const string YELLOW = "\x1b[93m";
        public const string BLUE = "\x1b[94m";
        public const string MAGENTA = "\x1b[95m";
        public const string CYAN = "\x1b[96m";
        public const string GREY = "\x1b[97m";
        public const string BOLD = "\x1b[1m";
        public const string NOBOLD = "\x1b[22m";
        public const string UNDERLINE = "\x1b[4m";
        public const string NOUNDERLINE = "\x1b[24m";
        public const string REVERSE = "\x1b[7m";
        public const string NOREVERSE = "\x1b[27m";
    }
}