using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace VectorEditor.Converters
{
    public class SelectedShapeConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (!values.All(v => v is string))
                return null;

            var guids = values.Cast<string>().ToArray();
            var allAreSame = guids.All(x => x == guids.First());
            if (allAreSame)
            {
                return new DropShadowEffect()
                {
                    BlurRadius = 20,
                    Color = new SolidColorBrush(Colors.Gold).Color
                };
            }

            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}