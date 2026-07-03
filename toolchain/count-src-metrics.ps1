Set-Location (Join-Path $PSScriptRoot '..')
$src = Join-Path (Get-Location) 'src'

$csFiles = Get-ChildItem -Path $src -Recurse -File -Filter '*.cs' |
    Where-Object { $_.FullName -notmatch '\\(obj|bin)\\' }
$projects = Get-ChildItem -Path $src -Recurse -File -Filter '*.csproj' |
    Where-Object { $_.FullName -notmatch '\\(obj|bin)\\' }

$typePattern = [regex]'(?m)^\s*(?:\[[^\]]+\]\s*)*(?:(?:public|private|protected|internal|static|sealed|partial|unsafe|readonly|ref|record|abstract|new|file)\s+)*(class|struct|interface|enum|record|delegate)\s+'
$methodPattern = [regex]'(?m)^\s*(?:\[[^\]]+\]\s*)*(?:(?:public|private|protected|internal|static|virtual|override|sealed|partial|async|unsafe|extern|new|readonly|ref|required|file|abstract)\s+)+[\w<>,\.\[\]\?\s]+\s+\w+\s*\([^;]*\)\s*(?:where\s+[^{]+)?\s*(?:=>|;|\{)'

function Get-FileMetrics($text) {
    $lines = $text -split '\r?\n'
    $total = $lines.Count
    $blank = 0
    $comment = 0
    $code = 0
    $inBlockComment = $false
    foreach ($line in $lines) {
        $trim = $line.Trim()
        if ($inBlockComment) {
            $comment++
            if ($trim -match '\*/') { $inBlockComment = $false }
            continue
        }
        if ($trim -match '^/\*') {
            $comment++
            if ($trim -notmatch '\*/') { $inBlockComment = $true }
            continue
        }
        if ($trim -eq '' -or $trim -match '^//') {
            if ($trim -eq '') { $blank++ } else { $comment++ }
            continue
        }
        $code++
    }
    [PSCustomObject]@{
        TotalLines = $total
        CodeLines = $code
        BlankLines = $blank
        CommentLines = $comment
        Types = ($typePattern.Matches($text)).Count
        Methods = ($methodPattern.Matches($text)).Count
    }
}

$grand = [PSCustomObject]@{
    TotalLines = 0; CodeLines = 0; BlankLines = 0; CommentLines = 0; Types = 0; Methods = 0; Files = 0
}

$rows = [System.Collections.Generic.List[object]]::new()

foreach ($p in ($projects | Sort-Object Name)) {
    $dir = $p.Directory.FullName
    $pFiles = $csFiles | Where-Object { $_.FullName.StartsWith($dir) }
    $row = [PSCustomObject]@{
        Project = $p.BaseName
        Files = $pFiles.Count
        TotalLines = 0
        CodeLines = 0
        BlankLines = 0
        CommentLines = 0
        Types = 0
        Methods = 0
    }
    foreach ($f in $pFiles) {
        $m = Get-FileMetrics ([IO.File]::ReadAllText($f.FullName))
        $row.TotalLines += $m.TotalLines
        $row.CodeLines += $m.CodeLines
        $row.BlankLines += $m.BlankLines
        $row.CommentLines += $m.CommentLines
        $row.Types += $m.Types
        $row.Methods += $m.Methods
    }
    $rows.Add($row)
    $grand.Files += $row.Files
    $grand.TotalLines += $row.TotalLines
    $grand.CodeLines += $row.CodeLines
    $grand.BlankLines += $row.BlankLines
    $grand.CommentLines += $row.CommentLines
    $grand.Types += $row.Types
    $grand.Methods += $row.Methods
}

# Shared items not under a .csproj directory
foreach ($shared in @('Plato.Generated', 'Plato.Intrinsics')) {
    $dir = Join-Path $src $shared
    $pFiles = $csFiles | Where-Object { $_.FullName.StartsWith($dir) }
    $row = [PSCustomObject]@{
        Project = $shared + ' (shared)'
        Files = $pFiles.Count
        TotalLines = 0
        CodeLines = 0
        BlankLines = 0
        CommentLines = 0
        Types = 0
        Methods = 0
    }
    foreach ($f in $pFiles) {
        $m = Get-FileMetrics ([IO.File]::ReadAllText($f.FullName))
        $row.TotalLines += $m.TotalLines
        $row.CodeLines += $m.CodeLines
        $row.BlankLines += $m.BlankLines
        $row.CommentLines += $m.CommentLines
        $row.Types += $m.Types
        $row.Methods += $m.Methods
    }
    $rows.Add($row)
}

# Recompute grand totals from all cs files (authoritative)
$grand = [PSCustomObject]@{ Files = 0; TotalLines = 0; CodeLines = 0; BlankLines = 0; CommentLines = 0; Types = 0; Methods = 0 }
foreach ($f in $csFiles) {
    $m = Get-FileMetrics ([IO.File]::ReadAllText($f.FullName))
    $grand.Files++
    $grand.TotalLines += $m.TotalLines
    $grand.CodeLines += $m.CodeLines
    $grand.BlankLines += $m.BlankLines
    $grand.CommentLines += $m.CommentLines
    $grand.Types += $m.Types
    $grand.Methods += $m.Methods
}

$generatedFiles = $csFiles | Where-Object { $_.Name -match '\.g\.cs$' }
$handFiles = $csFiles | Where-Object { $_.Name -notmatch '\.g\.cs$' }
$gen = [PSCustomObject]@{ Files = 0; TotalLines = 0; CodeLines = 0; Types = 0; Methods = 0 }
$hand = [PSCustomObject]@{ Files = 0; TotalLines = 0; CodeLines = 0; Types = 0; Methods = 0 }
foreach ($set in @(@{ Files = $generatedFiles; Out = $gen }, @{ Files = $handFiles; Out = $hand })) {
    foreach ($f in $set.Files) {
        $m = Get-FileMetrics ([IO.File]::ReadAllText($f.FullName))
        $set.Out.Files++
        $set.Out.TotalLines += $m.TotalLines
        $set.Out.CodeLines += $m.CodeLines
        $set.Out.Types += $m.Types
        $set.Out.Methods += $m.Methods
    }
}

function Format-Num($n) { if ($n -is [int] -or $n -is [long]) { $n.ToString('N0') } else { $n } }

$reportDate = Get-Date -Format 'yyyy-MM-dd'
$outPath = Join-Path (Get-Location) 'docs\SRC_METRICS.md'

$sb = [System.Text.StringBuilder]::new()
[void]$sb.AppendLine("# `src/` code metrics")
[void]$sb.AppendLine("")
[void]$sb.AppendLine("Snapshot of the supported SDK libraries under [`src/`](../src/). Generated $reportDate.")
[void]$sb.AppendLine("")
[void]$sb.AppendLine("## Summary")
[void]$sb.AppendLine("")
[void]$sb.AppendLine("| Metric | Count |")
[void]$sb.AppendLine("| --- | ---: |")
[void]$sb.AppendLine("| Projects (``.csproj``) | $(Format-Num $projects.Count) |")
[void]$sb.AppendLine("| Source files (``.cs``) | $(Format-Num $grand.Files) |")
[void]$sb.AppendLine("| Total lines | $(Format-Num $grand.TotalLines) |")
[void]$sb.AppendLine("| Code lines | $(Format-Num $grand.CodeLines) |")
[void]$sb.AppendLine("| Blank lines | $(Format-Num $grand.BlankLines) |")
[void]$sb.AppendLine("| Comment lines | $(Format-Num $grand.CommentLines) |")
[void]$sb.AppendLine("| Types | $(Format-Num $grand.Types) |")
[void]$sb.AppendLine("| Methods / functions | $(Format-Num $grand.Methods) |")
[void]$sb.AppendLine("")
[void]$sb.AppendLine("Code lines are non-blank, non-comment lines. Types and methods are approximate; see [Methodology](#methodology).")
[void]$sb.AppendLine("")
[void]$sb.AppendLine("## Hand-written vs generated")
[void]$sb.AppendLine("")
[void]$sb.AppendLine("Files ending in ``.g.cs`` (Plato codegen, glTF schema extensions, etc.):")
[void]$sb.AppendLine("")
[void]$sb.AppendLine("| Category | Files | Total lines | Code lines | Types | Methods |")
[void]$sb.AppendLine("| --- | ---: | ---: | ---: | ---: | ---: |")
[void]$sb.AppendLine("| Generated (``.g.cs``) | $(Format-Num $gen.Files) | $(Format-Num $gen.TotalLines) | $(Format-Num $gen.CodeLines) | $(Format-Num $gen.Types) | $(Format-Num $gen.Methods) |")
[void]$sb.AppendLine("| Hand-written | $(Format-Num $hand.Files) | $(Format-Num $hand.TotalLines) | $(Format-Num $hand.CodeLines) | $(Format-Num $hand.Types) | $(Format-Num $hand.Methods) |")
[void]$sb.AppendLine("")
[void]$sb.AppendLine("## Per project")
[void]$sb.AppendLine("")
[void]$sb.AppendLine("Four meta-packages (``Ara3D.SDK``, ``Ara3D.SDK.Core``, ``Ara3D.SDK.Geometry``, ``Ara3D.SDK.IO``) contain no ``.cs`` sources; they reference other projects for NuGet packaging.")
[void]$sb.AppendLine("")
[void]$sb.AppendLine("``Plato.Generated`` and ``Plato.Intrinsics`` are shared items (``.projitems``) compiled into ``Ara3D.Geometry``. They are listed separately below; do not add those rows to other project totals (that would double-count shared code).")
[void]$sb.AppendLine("")
[void]$sb.AppendLine("| Project | Files | Total lines | Code lines | Types | Methods |")
[void]$sb.AppendLine("| --- | ---: | ---: | ---: | ---: | ---: |")
foreach ($row in ($rows | Sort-Object { if ($_.TotalLines -eq 0) { [int]::MaxValue } else { -$_.TotalLines } })) {
    [void]$sb.AppendLine("| $($row.Project) | $(Format-Num $row.Files) | $(Format-Num $row.TotalLines) | $(Format-Num $row.CodeLines) | $(Format-Num $row.Types) | $(Format-Num $row.Methods) |")
}
[void]$sb.AppendLine("")
[void]$sb.AppendLine("## Methodology")
[void]$sb.AppendLine("")
[void]$sb.AppendLine("### Scope")
[void]$sb.AppendLine("")
[void]$sb.AppendLine("- **Root:** ``src/`` only (not ``ext/``, ``apps/``, ``plugins/``, ``tests/``, or ``toolchain/``).")
[void]$sb.AppendLine("- **Source files:** all ``.cs`` files, recursively.")
[void]$sb.AppendLine("- **Excluded:** anything under ``bin/`` or ``obj/`` build output folders.")
[void]$sb.AppendLine("- **Projects:** count of ``.csproj`` files under ``src/``.")
[void]$sb.AppendLine("")
[void]$sb.AppendLine("### Line classification")
[void]$sb.AppendLine("")
[void]$sb.AppendLine("Each physical line in a ``.cs`` file is classified as:")
[void]$sb.AppendLine("")
[void]$sb.AppendLine("1. **Blank** - empty or whitespace only.")
[void]$sb.AppendLine("2. **Comment** - starts with ``//``, or inside a ``/* ... */`` block comment.")
[void]$sb.AppendLine("3. **Code** - everything else.")
[void]$sb.AppendLine("")
[void]$sb.AppendLine("**Total lines** = blank + comment + code. This is a simple lexer, not a full C# parser; edge cases (e.g. ``//`` inside a string literal) are rare and not specially handled.")
[void]$sb.AppendLine("")
[void]$sb.AppendLine("### Types")
[void]$sb.AppendLine("")
[void]$sb.AppendLine("Counted via regex on lines that declare a top-level type keyword:")
[void]$sb.AppendLine("")
[void]$sb.AppendLine("``class``, ``struct``, ``interface``, ``enum``, ``record``, ``delegate``")
[void]$sb.AppendLine("")
[void]$sb.AppendLine("Optional modifiers (``public``, ``static``, ``partial``, etc.) may precede the keyword. Nested types declared at deeper indentation are still matched. This **over-counts slightly** when the keyword appears in comments or strings, and **under-counts** file-scoped types or unusual formatting.")
[void]$sb.AppendLine("")
[void]$sb.AppendLine("### Methods / functions")
[void]$sb.AppendLine("")
[void]$sb.AppendLine("Counted via regex on lines that look like method declarations: a return type (or modifier chain), an identifier, a parameter list ``(...)``, then ``{``, ``;``, or ``=>``.")
[void]$sb.AppendLine("")
[void]$sb.AppendLine("Includes most named methods, operators declared like methods, and some constructors. **Excludes** (approximately):")
[void]$sb.AppendLine("")
[void]$sb.AppendLine("- Local functions and lambdas")
[void]$sb.AppendLine("- Property getters/setters without a classic method signature line")
[void]$sb.AppendLine("- Methods whose declaration spans multiple lines with the parameter list on a later line")
[void]$sb.AppendLine("")
[void]$sb.AppendLine("Treat method counts as **useful ballpark figures**, not exact compiler semantics.")
[void]$sb.AppendLine("")
[void]$sb.AppendLine("### Regenerating")
[void]$sb.AppendLine("")
[void]$sb.AppendLine("From the repo root:")
[void]$sb.AppendLine("")
[void]$sb.AppendLine('```bat')
[void]$sb.AppendLine('powershell -NoProfile -ExecutionPolicy Bypass -File toolchain\count-src-metrics.ps1')
[void]$sb.AppendLine('```')
[void]$sb.AppendLine("")
[void]$sb.AppendLine("The script overwrites this file.")

[IO.File]::WriteAllText($outPath, $sb.ToString())
Write-Output "Wrote $outPath"
