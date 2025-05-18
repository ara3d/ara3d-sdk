using System.Collections.Generic;

namespace Ara3D.IO.VIM;

public class Element
{
    public long Id { get; set; }
    public string Type { get; set; }
    public string Name { get; set; }
    public string Category { get; set; }
    public List<int> Nodes { get; } = new();
    public List<Parameter> Parameters { get; } = new();
}