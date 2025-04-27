using System;
using System.Globalization;
using System.Windows.Data;

namespace Ara3D.Utils.Wpf
{
    [ValueConversion(typeof(float), typeof(string))]
    public class FloatStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value is float f ? f.ToString("N2") : null;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => float.Parse(value as string);        
    }
}