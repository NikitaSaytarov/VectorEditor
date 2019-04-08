using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using VectorEditor.Domain.Data;

namespace VectorEditor.Converters
{
    public class VerticesConverter : IValueConverter
    {
        public object Convert(
            object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is IEnumerable<Vertex> vertices
                ? new PointCollection(vertices.Select(v => new Point(v.X,v.Y)))
                : null;
        }

        public object ConvertBack(
            object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}