namespace Ara3D.PropKit;

public class PropDescriptorStringList : TypedPropDescriptor<int>
{
    public IReadOnlyList<string> Options { get; }

    public PropDescriptorStringList(IReadOnlyList<string> options, string name, string displayName, string description = "", string units = "", bool isReadOnly = false)
        : base(name, displayName, description, units, isReadOnly)
    {
        Options = options;
    }


    public override int MinValue => 0;
    public override int MaxValue => Options.Count - 1;
    public override int DefaultValue => 0;

    public override int Validate(int value) => Math.Clamp(value, 0, Options.Count - 1);
    public override bool IsValid(int value) => value >= 0 && value < Options.Count;
    public override bool AreEqual(int value1, int value2) => value1 == value2;
    public override object FromString(string value) => int.Parse(value);
    public override string ToString(int value) => value.ToString();
    protected override bool TryParse(string value, out int parsed) => int.TryParse(value, out parsed);
}