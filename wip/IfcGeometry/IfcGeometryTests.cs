using System.Reflection;
using Ara3D.IfcLoader;
using Ara3D.IO.StepParser;
using Ara3D.Logging;
using Ara3D.Utils;

namespace Ara3D.IfcGeometry
{
    public static class IfcGeometry
    {
        public static FilePath InputFile =
            PathUtil.GetCallerSourceFolder().RelativeFile("..", "..", "data", "AC20-FZK-Haus.ifc");

        public static HashSet<string> GetLocalTypes()
        {
            return Assembly.GetExecutingAssembly().GetTypes().Select(t => t.Name.ToUpperInvariant())
                .Where(n => n.StartsWith("IFC")).ToHashSet();
        }

        public static void OutputDetails(IfcFile file, ILogger logger)
        {
            logger.Log($"Loaded {file.FilePath.GetFileName()}");
            logger.Log($"Graph nodes: {file.Graph.Nodes.Count}");
            logger.Log($"Graph relations: {file.Graph.Relations.Count}");
            logger.Log($"Graph roots: {file.Graph.RootIds.Count}");
            logger.Log($"Property sets: {file.Graph.GetPropSets().Count()}");
            logger.Log($"Property values: {file.Graph.GetProps().Count()}");
            logger.Log($"Express ids: {file.Document.NumRawInstances}");
            logger.Log($"Geometry loaded: {file.GeometryDataLoaded}");
            logger.Log($"Num geometries loaded: {file.Model?.GetNumGeometries()}");
            logger.Log($"Num meshes loaded: {file.Model?.GetGeometries().Select(g => g.GetNumMeshes()).Sum()}");
        }
        [Test]
        public static void Test1()
        {
            var logger = Logger.Console;
            var (rd, file) = RunDetails.LoadGraph(InputFile, false, logger);
            OutputDetails(file, logger);
            Console.WriteLine(rd.Header());
            Console.WriteLine(rd.RowData());
            var localTypes = GetLocalTypes();
            var doc = file.Document;
            var numbers = new List<double>();
            var f = new IfcFactory();
            var d = new Dictionary<string, List<StepInstance>>();
            var cnt = 0;
            foreach (var rawInstance in file.Document.RawInstances)
            {
                if (rawInstance.Type.IsNull)
                    continue;

                var str = rawInstance.Type.ToString().ToUpperInvariant();
                if (!localTypes.Contains(str))
                    continue;

                var inst = doc.GetInstanceWithData(rawInstance);
                GatherNumbers(inst.AttributeValues, numbers);
                cnt++;

                f.AddValue(inst);

                if (!d.ContainsKey(str))
                    d[str] = new List<StepInstance>() { inst };
                else
                    d[str].Add(inst);
            }

            Console.WriteLine($"Found a total of {cnt} instances, and {numbers.Count} numbers");
            foreach (var kv in d.OrderBy(kv => kv.Key))
                Console.WriteLine($"{kv.Key} = {kv.Value.Count}");
        }

        public static void GatherNumbers(List<StepValue> list, List<double> numbers)
        {
            foreach (var tmp in list)
            {
                if (tmp is StepNumber n)
                    numbers.Add(n.Value);
                else if (tmp is StepList stepList)
                    GatherNumbers(stepList.Values, numbers);
            }
        }
    }
}