using Ara3D.Geometry.Compiler;
using Ara3D.Logging;
using Ara3D.Utils;
using Ara3D.Geometry.CSharpWriter;
using Ara3D.Geometry.TypeScriptWriter;
using Logger = Ara3D.Logging.Logger;

namespace Ara3D.Geometry.CLI
{
    public static class Program
    {
        // Usage: Plato.CLI [inputFolder] [outputFolder] [--typescript]
        // With no arguments, the folders come from Config and C# is generated (original behavior).
        public static void Main(string[] args)
        {
            var logger = Logger.Console;

            var typeScript = args.Contains("--typescript");
            var folders = args.Where(a => !a.StartsWith("--")).ToList();
            var inputFolder = new DirectoryPath(folders.Count > 0 ? folders[0] : Config.InputFolder);
            var outputFolder = new DirectoryPath(folders.Count > 1 ? folders[1] : Config.OutputFolder);

            logger.Log("Opening files");
            var files = inputFolder.GetFiles("*.plato");
            var docs = files.Select(f => new Document(f, logger)).ToList();
            var parsingSuccessful = docs.All(e => e.Parser.Succeeded);
            if (!parsingSuccessful)
            {
                logger.Log("Parsing failed for one of the input files, halting");
                return;
            }
            logger.Log("Parsing succeeded for all files");

            logger.Log("Compiling");
            var trees = docs.Select(e => e.Ast);
            var compilation = new Compilation(logger, trees);
            if (!compilation.CompletedCompilation)
            {
                logger.Log("Compilation was not completed");
                return;
            }

            if (typeScript)
            {
                logger.Log("Writing TypeScript Files");
                var output = compilation.ToTypeScript(outputFolder);
                foreach (var kv in output.Files)
                {
                    var fp = outputFolder.RelativeFile(kv.Key);
                    logger.Log($"Writing {kv.Key}");
                    fp.WriteAllText(kv.Value.ToString());
                }
            }
            else
            {
                logger.Log("Writing C# Files");
                var output = compilation.ToCSharp(outputFolder);
                foreach (var kv in output.Files)
                {
                    var fp = outputFolder.RelativeFile(kv.Key);
                    logger.Log($"Writing {kv.Key}");
                    fp.WriteAllText(kv.Value.ToString());
                }
            }

            logger.Log("Completed");
        }
    }
}
