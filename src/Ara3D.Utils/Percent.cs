namespace Ara3D.Utils
{
    public readonly struct Percent
    {
        public Percent(double value) => Value = value;
        public readonly double Value;
        public static implicit operator double(Percent percent) => percent.Value;
        public static implicit operator Percent(double value) => new Percent(value);
        public static Percent FromFraction(double numerator, double denominator) => FromDecimalValue(numerator/denominator);
        public static Percent FromDecimalValue(double fractionalValue) => fractionalValue * 100.0;
        public double AsDecimalValue => Value / 100.0;
    }

    public static class PercentUtil
    {
        public static Percent Percent(this double value) => new Percent(value);
        public static Percent Percent(this int value) => new Percent(value);
        public static Percent Percent(this float value) => new Percent(value);
    }
}