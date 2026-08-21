global using CatoriApp.Core.Objects;
global using CatoriApp.Game.Objects.Services.People;
global using CatoriApp.Game.ViewModels.People;
global using CatoriApp.Game.Views.Controls;
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
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            EventManager.RegisterClassHandler(typeof(Window),System.Windows.Input.Keyboard.PreviewKeyDownEvent,
                new System.Windows.Input.KeyEventHandler(GlobalAvatarKeyDown),true);
            EventManager.RegisterClassHandler(typeof(Window),FrameworkElement.LoadedEvent,
                new RoutedEventHandler(WindowLoaded),true);
        }

        private static void GlobalAvatarKeyDown(object sender,System.Windows.Input.KeyEventArgs e)
        {
            if(e.Key!=System.Windows.Input.Key.A||!System.Windows.Input.Keyboard.Modifiers.HasFlag(System.Windows.Input.ModifierKeys.Control))return;
            if(sender is Window owner&&owner is not Views.Shared.AvatarSelectorWindow)
            {
                Views.Shared.AvatarSelectorWindow.ShowFor(owner);e.Handled=true;
            }
        }

        private static void WindowLoaded(object sender,RoutedEventArgs e)
        {
            if(sender is Window window&&window is not Views.Shared.AvatarSelectorWindow)
                Views.Shared.AvatarSelectorWindow.ApplyCurrentTo(window);
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



