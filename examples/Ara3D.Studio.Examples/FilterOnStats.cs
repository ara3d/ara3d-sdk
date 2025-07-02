namespace Ara3D.Studio.Samples;

public class FilterOnStats : IModelModifier
{
    [Range(0, 100000)] public int MinTriangles = 100;

    public Model3D Eval(Model3D model, EvalContext context)
    {
        var table = model.DataSet.GetTable("Meshes");
        if (table == null) return model;
        var column = table.GetColumn("NumTriangles");
        if (column == null) return model;
        var meshesToExclude = new HashSet<int>(
            column.Values.IndicesWhere(v => v is int n && n < MinTriangles));
        var newStructs = model.ElementStructs.Where(s => 
            !meshesToExclude.Contains(s.MeshIndex)).ToList();
        return model.WithStructs(newStructs);
    }
}