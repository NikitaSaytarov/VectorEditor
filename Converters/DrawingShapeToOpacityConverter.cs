using System;
using System.Globalization;
using System.Windows.Data;

namespace VectorEditor.Converters
{
    public class DrawingShapeToOpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = (bool) value;

            if (val)
                return 0.2d;

            return 1d;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}