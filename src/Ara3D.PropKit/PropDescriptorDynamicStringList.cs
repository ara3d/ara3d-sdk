namespace Ara3D.PropKit;

public class PropDescriptorDynamicStringList : TypedPropDescriptor<int>
{
    public Func<IReadOnlyList<string>> OptionsFunc { get; }

    public PropDescriptorDynamicStringList(Func<IReadOnlyList<string>> optionsFunc, string name, string displayName, string description = "", string units = "", bool isReadOnly = false)
        : base(name, displayName, description, units, isReadOnly)
    {
        OptionsFunc = optionsFunc;
    }

    public override int MinValue => 0;
    public override int MaxValue => Count - 1;
    public override int DefaultValue => 0;

    public int Count => OptionsFunc()?.Count ?? 0;

    public override int Validate(int value) => Math.Clamp(value, 0, Count - 1);
    public override bool IsValid(int value) => value >= 0 && value < Count;
    public override bool AreEqual(int value1, int value2) => value1 == value2;
    public override object FromString(string value) => int.Parse(value);
    public override string ToString(int value) => value.ToString();
    protected override bool TryParse(string value, out int parsed) => int.TryParse(value, out parsed);
}