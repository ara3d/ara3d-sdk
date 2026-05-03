using System.Globalization;
using Ara3D.Geometry;

namespace Ara3D.PropKit;

public class PropDescriptorVector4 : TypedPropDescriptor<Vector4>
{
    public override Vector4 MinValue { get; }
    public override Vector4 MaxValue { get; }
    public override Vector4 DefaultValue { get; }

    public static Vector4 Default = new(0, 0, 0, 0);

    public PropDescriptorVector4(string name, string displayName, string description = "", string units = "",
        bool isReadOnly = false)
        : this(name, displayName, description, units, isReadOnly, Default, -100f, 100f)
    { }

    public PropDescriptorVector4(string name, string displayName, string description, string units,
        bool isReadOnly, Vector4 defaultValue, float minValue, float maxValue)
        : base(name, displayName, description, units, isReadOnly)
    {
        DefaultValue = defaultValue;
        MinValue = new Vector4(minValue, minValue, minValue, minValue);
        MaxValue = new Vector4(maxValue, maxValue, maxValue, maxValue);
    }

    public override object Validate(object value)
    {
        if (value is Ara3D.Geometry.Vector4 v)
            return Validate(v.Value);
        return base.Validate(value);
    }

    public override Vector4 Validate(Vector4 value)
    {
        return new Vector4(
            Math.Clamp(value.X, MinValue.X, MaxValue.X),
            Math.Clamp(value.Y, MinValue.Y, MaxValue.Y),
            Math.Clamp(value.Z, MinValue.Z, MaxValue.Z),
            Math.Clamp(value.W, MinValue.W, MaxValue.W)
        );
    }

    public override bool IsValid(Vector4 value)
    {
        return
            value.X >= MinValue.X && value.X <= MaxValue.X &&
            value.Y >= MinValue.Y && value.Y <= MaxValue.Y &&
            value.Z >= MinValue.Z && value.Z <= MaxValue.Z &&
            value.W >= MinValue.W && value.W <= MaxValue.W;
    }

    public override bool AreEqual(Vector4 value1, Vector4 value2)
    {
        const float epsilon = 0.00001f;

        return
            Math.Abs(value1.X - value2.X) <= epsilon &&
            Math.Abs(value1.Y - value2.Y) <= epsilon &&
            Math.Abs(value1.Z - value2.Z) <= epsilon &&
            Math.Abs(value1.W - value2.W) <= epsilon;
    }

    public override object FromString(string value)
    {
        var parts = value.Split(',');
        return new Vector4(
            float.Parse(parts[0], CultureInfo.InvariantCulture),
            float.Parse(parts[1], CultureInfo.InvariantCulture),
            float.Parse(parts[2], CultureInfo.InvariantCulture),
            float.Parse(parts[3], CultureInfo.InvariantCulture));
    }

    public override string ToString(Vector4 value) => value.ToString();

    protected override bool TryParse(string value, out Vector4 parsed)
    {
        parsed = default;
        var parts = value.Split(',');
        if (parts.Length != 4) return false;
        if (!float.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out var x)) return false;
        if (!float.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out var y)) return false;
        if (!float.TryParse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture, out var z)) return false;
        if (!float.TryParse(parts[3], NumberStyles.Float, CultureInfo.InvariantCulture, out var w)) return false;
        parsed = new Vector4(x, y, z, w);
        return true;

    }
}