using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;
using VectorEditor.Services.AppColorsService;

namespace VectorEditor.Converters
{
    public class BrushesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var appColorService = AppColorService.GetInstance;
                var brush = (Brush) value;
                return !appColorService.Colors.Contains(brush) ? appColorService.Colors.First(b => brush != null && b.ToString() == brush.ToString()) : brush;
            }
            catch
            {
                return value;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}