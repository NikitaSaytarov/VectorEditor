using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;
using MahApps.Metro;

namespace VectorEditor.Services.AppColorsService
{
    public sealed class AppColorService
    {
        private static AppColorService _instance;
        public static AppColorService GetInstance => _instance ?? (_instance = new AppColorService());

        private readonly ObservableCollection<Brush> _colors;
        public ReadOnlyObservableCollection<Brush> Colors { get; }

        private AppColorService()
        {
            var accents = ThemeManager.Accents.Select(x => new BrushConverter().ConvertFromString(x.Resources["AccentColor"]
                .ToString()) as Brush);

            _colors = new ObservableCollection<Brush>();
            Colors = new ReadOnlyObservableCollection<Brush>(_colors);

            foreach (var solidColorBrush in accents)
                _colors.Add(solidColorBrush);
        }
    }
}