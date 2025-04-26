using System.Collections.Generic;

namespace Ara3D.Serialization.VIM;

public class Element
{
    public long Id { get; set; }
    public string Type { get; set; }
    public string Name { get; set; }
    public string Category { get; set; }
    public List<int> Nodes { get; } = new List<int>();
    public List<Parameter> Parameters { get; } = new List<Parameter>();
}