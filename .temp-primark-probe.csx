using System;
using System.Linq;
using Ara3D.Ifc.Mesher.Approach1;
using Ara3D.IfcLoader;
using Ara3D.IfcMeshingComparison.Harness;

var ifcPath = new Ara3D.Utils.FilePath(@"c:\Users\cdigg\git\studio\data\20210221PRIMARK.ifc");
OracleEntityMap.Write(ifcPath);
var map = OracleEntityMap.Build(ifcPath);
using var stepFile = new IfcFile(ifcPath, includeGeometry: false);
var (model, diagnostics) = ModelAssembler.BuildModel(stepFile);
var candidateEntities = model.Instances.Where(i => i.EntityIndex >= 0).Select(i => i.EntityIndex).ToHashSet();
var oracleInstByEntity = map.OracleInstances.GroupBy(i => i.EntityIndex).ToDictionary(g => g.Key, g => g.Count());
var oracleOnly = map.ProductRepresentationTrees.Where(t => !candidateEntities.Contains(t.EntityId) && oracleInstByEntity.ContainsKey(t.EntityId)).ToList();
Console.WriteLine($"oracle-only={oracleOnly.Count}");
var geomHist = new Dictionary<string,int>();
var failSamples = new List<string>();
foreach (var t in oracleOnly.Take(50))
{
  var entity = stepFile.EntityResolver.GetEntity(t.EntityId);
  var mesh = ModelAssembler.BuildEntityMesh(new MeshingContext(stepFile), entity);
  var status = mesh is null ? "NULL" : $"tris={mesh.Value.FaceIndices.Count}";
  // walk body items
  var items = new List<string>();
  void Walk(int id, int depth) {
    if (depth>6) return;
    var e = stepFile.EntityResolver.GetEntityOrDefault(id);
    if (e is null) return;
    var n = e.GetEntityName();
    if (n is "IFCEXTRUDEDAREASOLID" or "IFCBOOLEANCLIPPINGRESULT" or "IFCBOOLEANRESULT" or "IFCARBITRARYCLOSEDPROFILEDEF" or "IFCCOMPOSITECURVE" or "IFCPOLYGONALBOUNDEDHALFSPACE" or "IFCHALFSPACESOLID" or "IFCISHAPEPROFILEDEF" or "IFCLSHAPEPROFILEDEF" or "IFCRECTANGLEHOLLOWPROFILEDEF" or "IFCARBITRARYPROFILEDEFWITHVOIDS")
      items.Add($"#{id}:{n}");
  }
  // crude: use representation tree from map
  var tree = map.ProductRepresentationTrees.First(x => x.EntityId == t.EntityId);
  foreach (var node in tree.Nodes.Take(30))
    Walk(node.EntityId, 0);
  var key = string.Join("+", items.Select(i => i.Split(':')[1]).Distinct().OrderBy(x=>x));
  if (string.IsNullOrEmpty(key)) key = "(no-geom-tags)";
  geomHist[key] = geomHist.GetValueOrDefault(key) + 1;
  if (failSamples.Count < 15)
    failSamples.Add($"#{t.EntityId} mesh={status} [{key}]");
}
foreach (var kv in geomHist.OrderByDescending(k=>k.Value))
  Console.WriteLine($"  {kv.Value}x {kv.Key}");
foreach (var s in failSamples) Console.WriteLine(s);
