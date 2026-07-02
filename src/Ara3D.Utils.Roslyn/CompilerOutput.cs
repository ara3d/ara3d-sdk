using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using System.Collections.Generic;
using System.Linq;

namespace Ara3D.Utils.Roslyn;

public class CompilerOutput
{
    public ParsedCompilerInput Input { get; }
    public CSharpCompilation CSharpCompilation { get; }
    public EmitResult EmitResult { get; }
    public bool Success => EmitResult.Success;
    public FilePath OutputFilePath => Input.Options.OutputFile;
    public IEnumerable<string> Errors => EmitResult.Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error).Select(d => d.ToString());
    public IEnumerable<string> AllDiagnostics => EmitResult.Diagnostics.Select(d => d.ToString());
    public Dictionary<string, string> TypeToSourceMap { get; }

    public CompilerOutput(ParsedCompilerInput input, CSharpCompilation compilation, EmitResult emitResult)
    {
        Input = input;
        CSharpCompilation = compilation;
        EmitResult = emitResult;
        TypeToSourceMap = ConstructTypeMap(CSharpCompilation);
    }

    public static Dictionary<string, string> ConstructTypeMap(CSharpCompilation compilation)
    {
        var r = new Dictionary<string, string>();

        void Visit(INamespaceSymbol ns)
        {
            foreach (var t in ns.GetTypeMembers())
                AddTypeAndPartials(t);
            foreach (var child in ns.GetNamespaceMembers())
                Visit(child);
        }

        void AddTypeAndPartials(INamedTypeSymbol type)
        {
            // One type may have multiple declaring syntax locations (partials)
            var paths = type.DeclaringSyntaxReferences
                .Select(r => r.SyntaxTree.FilePath)
                .Where(p => !string.IsNullOrEmpty(p))
                .Distinct()
                .ToArray();

            if (paths.Length > 0)
                r[type.ToDisplayString()] = paths[0]; 

            foreach (var nested in type.GetTypeMembers())
                AddTypeAndPartials(nested);
        }

        Visit(compilation.GlobalNamespace);
        return r;
    }
}