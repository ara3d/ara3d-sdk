using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Ara3D.SDK.DevTools
{
    public class StripBodiesRewriter : CSharpSyntaxRewriter
    {
        public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            // remove block or expression body, keep signature + semicolon
            var semicolon = SyntaxFactory.Token(SyntaxKind.SemicolonToken);
            return node
                .WithBody(null)
                .WithExpressionBody(null)
                .WithSemicolonToken(semicolon)
                .WithTriviaFrom(node);
        }

        public override SyntaxNode VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
        {
            var semicolon = SyntaxFactory.Token(SyntaxKind.SemicolonToken);
            return node
                .WithBody(null)
                .WithInitializer(null)
                .WithSemicolonToken(semicolon)
                .WithTriviaFrom(node);
        }

        public override SyntaxNode VisitDestructorDeclaration(DestructorDeclarationSyntax node)
        {
            var semicolon = SyntaxFactory.Token(SyntaxKind.SemicolonToken);
            return node
                .WithBody(null)
                .WithSemicolonToken(semicolon)
                .WithTriviaFrom(node);
        }

        public override SyntaxNode VisitOperatorDeclaration(OperatorDeclarationSyntax node)
        {
            var semicolon = SyntaxFactory.Token(SyntaxKind.SemicolonToken);
            return node
                .WithBody(null)
                .WithExpressionBody(null)
                .WithSemicolonToken(semicolon)
                .WithTriviaFrom(node);
        }

        public override SyntaxNode VisitConversionOperatorDeclaration(ConversionOperatorDeclarationSyntax node)
        {
            var semicolon = SyntaxFactory.Token(SyntaxKind.SemicolonToken);
            return node
                .WithBody(null)
                .WithExpressionBody(null)
                .WithSemicolonToken(semicolon)
                .WithTriviaFrom(node);
        }

        public override SyntaxNode VisitPropertyDeclaration(PropertyDeclarationSyntax node)
        {
            // If it has a normal accessor list (get; set; etc.)
            if (node.AccessorList != null)
            {
                // Strip out any bodies / expression‐bodies and force a “;” on each accessor
                var strippedAccessors = node.AccessorList.Accessors
                    .Select(acc => acc
                        .WithBody(null)
                        .WithExpressionBody(null)
                        .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken))
                        .WithTriviaFrom(acc)
                    );

                // Recreate the AccessorList from our stripped accessors
                var newAccessorList = SyntaxFactory.AccessorList(SyntaxFactory.List(strippedAccessors));

                return node
                    .WithAccessorList(newAccessorList)
                    .WithTriviaFrom(node);
            }

            // If it had an expression‐body auto‐property, convert to simple get;
            if (node.ExpressionBody != null)
            {
                var getOnly = SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                    .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
                var newList = SyntaxFactory.AccessorList(SyntaxFactory.SingletonList(getOnly));

                return node
                    .WithExpressionBody(null)
                    .WithAccessorList(newList)
                    .WithTriviaFrom(node);
            }

            return node;
        }

        public override SyntaxNode VisitAccessorDeclaration(AccessorDeclarationSyntax node)
        {
            // fallback in case any slipped through
            if (node.Body != null || node.ExpressionBody != null)
            {
                return node
                    .WithBody(null)
                    .WithExpressionBody(null)
                    .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken))
                    .WithTriviaFrom(node);
            }

            return node;
        }
    
        public static StringBuilder ExtractCode(string srcDir, StringBuilder? sb = null)
        {          
            if (!Directory.Exists(srcDir))
                throw new Exception($"Error: Directory not found: {srcDir}");

            var rewriter = new StripBodiesRewriter();
            sb ??= new StringBuilder();

            foreach (var csFile in Directory.EnumerateFiles(srcDir, "*.cs", SearchOption.AllDirectories))
            {
                if (csFile.Contains(".g."))
                    continue;
                try
                {
                    var text = File.ReadAllText(csFile);
                    var tree = CSharpSyntaxTree.ParseText(text);
                    var root = tree.GetRoot();
                    var stripped = (CompilationUnitSyntax)rewriter.Visit(root);
                    var code = stripped.ToFullString().Trim();                    
                    sb.AppendLine(code);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Failed on {csFile}: {ex.Message}");
                }
            }

            return sb;
        }

        [Test, Explicit]
        public static void GetProjectSummary()
        {
            var dir = @"C:\Users\cdigg\git\studio\ara3d-sdk\core\Ara3D.PropKit\";
            var sb = ExtractCode(dir);
            Console.WriteLine(sb.ToString());
        }
    }
}