using System.Windows;

namespace VectorEditor
{
    public partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            // ReSharper disable once ObjectCreationAsStatement
            new Setup.ApplicationInitializer(this);
            base.OnStartup(e);
        }
    }
}
