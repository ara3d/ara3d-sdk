using System.Globalization;
using Ara3D.Geometry;

namespace Ara3D.PropKit;

public class PropDescriptorVector2 : TypedPropDescriptor<Vector2>
{
    public override Vector2 MinValue { get; }
    public override Vector2 MaxValue { get; }
    public override Vector2 DefaultValue { get; }

    public static Vector2 Default = new(0,0);

    public PropDescriptorVector2(string name, string displayName, string description = "", string units = "",
        bool isReadOnly = false)
        : this(name, displayName, description, units, isReadOnly, Default, -100f, 100f)
    { }

    public PropDescriptorVector2(string name, string displayName, string description, string units,
        bool isReadOnly, Vector2 defaultValue, float minValue, float maxValue)
        : base(name, displayName, description, units, isReadOnly)
    {
        DefaultValue = defaultValue;
        MinValue = new Vector2(minValue, minValue);
        MaxValue = new Vector2(maxValue, maxValue);
    }

    public override object Validate(object value)
    {
        if (value is Ara3D.Geometry.Vector2 v)
            return Validate(v.Value);
        return base.Validate(value);
    }

    public override Vector2 Validate(Vector2 value)
    {
        return new Vector2(
            Math.Clamp(value.X, MinValue.X, MaxValue.X),
            Math.Clamp(value.Y, MinValue.Y, MaxValue.Y)
        );
    }

    public override bool IsValid(Vector2 value)
    {
        return
            value.X >= MinValue.X && value.X <= MaxValue.X &&
            value.Y >= MinValue.Y && value.Y <= MaxValue.Y;
    }

    public override bool AreEqual(Vector2 value1, Vector2 value2)
    {
        const float epsilon = 0.00001f;

        return
            Math.Abs(value1.X - value2.X) <= epsilon &&
            Math.Abs(value1.Y - value2.Y) <= epsilon;
    }

    public override object FromString(string value)
    {
        var parts = value.Split(',');
        return new Vector2(
            float.Parse(parts[0], CultureInfo.InvariantCulture),
            float.Parse(parts[1], CultureInfo.InvariantCulture));
    }

    public override string ToString(Vector2 value) => value.ToString();

    protected override bool TryParse(string value, out Vector2 parsed)
    {
        parsed = default;
        var parts = value.Split(',');
        if (parts.Length != 3) return false;

        if (!float.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out var x)) return false;
        if (!float.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out var y)) return false;
        parsed = new Vector2(x, y);
        return true;
    }
}