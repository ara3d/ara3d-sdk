﻿namespace Ara3D.IO.StepParser
{

    public enum StepTokenType : byte
    {
        None,
        Ident,
        String,
        Whitespace,
        Number,
        Symbol,
        Id,
        Separator,
        Unassigned,
        Redeclared,
        Comment,
        Unknown,
        BeginGroup,
        EndGroup,
        LineBreak,
        EndOfLine,
        Definition,
    }
}