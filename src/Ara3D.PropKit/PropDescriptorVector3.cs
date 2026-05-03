using System.Globalization;
using Ara3D.Geometry;

namespace Ara3D.PropKit;

public class PropDescriptorVector3 : TypedPropDescriptor<Vector3>
{
    public override Vector3 MinValue { get; }
    public override Vector3 MaxValue { get; }
    public override Vector3 DefaultValue { get; }

    public static Vector3 Default = new(0, 0, 0);

    public PropDescriptorVector3(string name, string displayName, string description = "", string units = "",
        bool isReadOnly = false)
        : this(name, displayName, description, units, isReadOnly, Default, -100f, 100f)
    { }

    public PropDescriptorVector3(string name, string displayName, string description, string units,
        bool isReadOnly, Vector3 defaultValue, float minValue, float maxValue)
        : base(name, displayName, description, units, isReadOnly)
    {
        DefaultValue = defaultValue;
        MinValue = new Vector3(minValue, minValue, minValue);
        MaxValue = new Vector3(maxValue, maxValue, maxValue);
    }

    public override object Validate(object value)
    {
        // TODO: this and the functions in relawted types (see Vector2 and Vector4) should perhaps be generalized to support converters. 
        // We don't want this system to be know about the Ara3D.Geometry types necessarily 
        if (value is Ara3D.Geometry.Vector3 v)
            return Validate(v.Value);
        return base.Validate(value);
    }

    public override Vector3 Validate(Vector3 value)
    {
        return new Vector3(
            Math.Clamp(value.X, MinValue.X, MaxValue.X),
            Math.Clamp(value.Y, MinValue.Y, MaxValue.Y),
            Math.Clamp(value.Z, MinValue.Z, MaxValue.Z)
        );
    }

    public override bool IsValid(Vector3 value)
    {
        return
            value.X >= MinValue.X && value.X <= MaxValue.X &&
            value.Y >= MinValue.Y && value.Y <= MaxValue.Y &&
            value.Z >= MinValue.Z && value.Z <= MaxValue.Z;
    }

    public override bool AreEqual(Vector3 value1, Vector3 value2)
    {
        const float epsilon = 0.00001f;

        return
            Math.Abs(value1.X - value2.X) <= epsilon &&
            Math.Abs(value1.Y - value2.Y) <= epsilon &&
            Math.Abs(value1.Z - value2.Z) <= epsilon;
    }

    public override object FromString(string value)
    {
        var parts = value.Split(',');
        return new Vector3(
            float.Parse(parts[0], CultureInfo.InvariantCulture),
            float.Parse(parts[1], CultureInfo.InvariantCulture),
            float.Parse(parts[2], CultureInfo.InvariantCulture));
    }

    public override string ToString(Vector3 value) => value.ToString();

    protected override bool TryParse(string value, out Vector3 parsed)
    {
        parsed = default;
        var parts = value.Split(',');
        if (parts.Length != 3) return false;
        if (!float.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out var x)) return false;
        if (!float.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out var y)) return false;
        if (!float.TryParse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture, out var z)) return false;
        parsed = new Vector3(x, y, z);
        return true;
    }
}