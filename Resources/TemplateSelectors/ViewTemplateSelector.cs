using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace VectorEditor.Resources.TemplateSelectors
{
    public sealed class ViewTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            //if (item != null)
            //{
            //    var viewModelType = item.GetType();
            //    var view = viewModelType.Name.Replace("Model", string.Empty);

            //    var templateString = "<DataTemplate xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" " +
            //                         "xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" " +
            //                         "xmlns:views=\"clr -namespace:VectorEditor.Views\">";
            //    templateString += $"<views:{view}/>";
            //    templateString += "</DataTemplate>";

            //    var dataTemplate = (DataTemplate)XamlReader.Parse(templateString);
            //    return dataTemplate;
            //}

            return base.SelectTemplate(item, container);
        }
    }
}