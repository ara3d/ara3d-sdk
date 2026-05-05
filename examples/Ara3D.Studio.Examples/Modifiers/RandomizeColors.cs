namespace Ara3D.Studio.Samples;

public class RandomizeColors : IModifier
{
    [Range(0, 1000)] public int Seed { get; set; }

    public static Color RandomColor(Random rng)
        => Color.Create(rng.NextSingle(), rng.NextSingle(), rng.NextSingle(), 1);

    public Model3D Eval(Model3D input)
    {
        var rng = new Random(Seed);
        var mb = new Model3DBuilder();
        mb.Meshes.AddRange(input.Meshes); 
        for (var i = 0; i < input.Instances.Count; i++)
        {
            var inst = input.Instances[i];
            var color = RandomColor(rng);
            mb.AddInstance(inst.MeshIndex, inst.Matrix4x4, new Material(color, 0, 0.5f));
        }

        return mb.Build();
    }
}