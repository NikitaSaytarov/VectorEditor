using System;
using System.Globalization;
using System.Windows.Data;
using Size = System.Windows.Size;

namespace VectorEditor.Converters
{
    public class CanvasSizeToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var canvasSize = (Size)value;
            return $"{canvasSize.Width}x{canvasSize.Height}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}