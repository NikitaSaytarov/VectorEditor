using System.Windows;
using System.Windows.Controls;
using VectorEditor.Core.MVVM.Base;

namespace VectorEditor.Core.MVVM
{
    public class CustomUserControl : UserControl
    {
        protected ViewModelBase ViewModel { get; set; }

        public CustomUserControl()
        {
            Loaded += OnLoaded;
            DataContextChanged += OnDataContextChanged;
            Unloaded += OnUnloaded;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is ViewModelBase @base)
                ViewModel = @base;
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            ViewModel = null;
        }

        protected virtual async void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (ViewModel == null)
                if (DataContext != null)
                    ViewModel = (ViewModelBase)DataContext;

            if (ViewModel != null)
                await ViewModel.ViewLoadedAsync();
        }
    }
}