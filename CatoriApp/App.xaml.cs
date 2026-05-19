global using CatoriApp.Objects;
global using CatoriApp.Objects.Services;
global using CatoriApp.ViewModels;
global using CatoriApp.Views.Controls;
global using CatoriServices.Objects;
global using CatoriServices.Objects.database;
global using CatoriServices.Objects.Entities;
global using System.Timers;
global using System.Windows;
global using System.Windows.Controls;
global using System.Windows.Media;
namespace CatoriApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected virtual void OnStartup(StartupEventArgs e)
        {
          
        }
        void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            string errorMessage = string.Format("An unhandled exception occurred: {0}", e.Exception.Message);
            MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            // OR whatever you want like logging etc. MessageBox it's just example
            // for quick debugging etc.
            e.Handled = true;
        }
    }

}



