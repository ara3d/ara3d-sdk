namespace Ara3D.Geometry;

public static class AngleUtils
{
    public static Angle AngularDistance(this Angle self, Angle other)
        => (other - self).Normalize().Abs;

    public static Angle AngularLerp(this Angle self, Angle other, float amount)
        => self + (other - self).Normalize() * amount;

    public static Angle Normalize(this Angle self)
    {
        const float TwoPi = MathF.PI * 2f;
        var radians = self.Value % TwoPi;
        if (radians < -MathF.PI)
            radians += TwoPi;
        else if (radians >= MathF.PI)
            radians -= TwoPi;
        return radians;
    }
}