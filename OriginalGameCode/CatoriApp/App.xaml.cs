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
        private int _handlingDispatcherFailure;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            DispatcherUnhandledException += OnDispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += OnDomainUnhandledException;
            TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
            EventManager.RegisterClassHandler(typeof(Window),System.Windows.Input.Keyboard.PreviewKeyDownEvent,
                new System.Windows.Input.KeyEventHandler(GlobalAvatarKeyDown),true);
            EventManager.RegisterClassHandler(typeof(Window),System.Windows.Input.Keyboard.PreviewKeyDownEvent,
                new System.Windows.Input.KeyEventHandler(AddImageKeyDown),true);
        }

        private static void AddImageKeyDown(object sender,System.Windows.Input.KeyEventArgs e)
        {
            if(e.Key!=System.Windows.Input.Key.F||!System.Windows.Input.Keyboard.Modifiers.HasFlag(System.Windows.Input.ModifierKeys.Control))return;
            if(System.Windows.Input.Keyboard.FocusedElement is System.Windows.Controls.Primitives.TextBoxBase)return;
            if(sender is not Window owner)return;
            Microsoft.Win32.OpenFileDialog dialog=new(){Title="Add image",Filter="Images|*.png;*.jpg;*.jpeg;*.bmp;*.gif|All files|*.*"};
            if(dialog.ShowDialog(owner)!=true)return;
            var image=new CatoriUCLibrary.Views.MyImage.MyImageUC{ImagePath=dialog.FileName,HorizontalAlignment=HorizontalAlignment.Left,VerticalAlignment=VerticalAlignment.Top};
            Panel host;
            if(owner.Content is Panel panel)host=panel;
            else
            {
                object oldContent=owner.Content;owner.Content=null;
                Grid grid=new();if(oldContent is UIElement element)grid.Children.Add(element);owner.Content=grid;host=grid;
            }
            Panel.SetZIndex(image,int.MaxValue-10);host.Children.Add(image);
            if(host is Canvas){Canvas.SetLeft(image,Math.Max(20,(owner.ActualWidth-image.Width)/2));Canvas.SetTop(image,Math.Max(20,(owner.ActualHeight-image.Height)/2));}
            else image.Margin=new Thickness(Math.Max(20,(owner.ActualWidth-image.Width)/2),Math.Max(20,(owner.ActualHeight-image.Height)/2),0,0);
            image.Focus();e.Handled=true;
        }

        private static void GlobalAvatarKeyDown(object sender,System.Windows.Input.KeyEventArgs e)
        {
            if(e.Key!=System.Windows.Input.Key.A||!System.Windows.Input.Keyboard.Modifiers.HasFlag(System.Windows.Input.ModifierKeys.Control))return;
            if(sender is Window owner&&owner is not Views.Shared.AvatarSelectorWindow)
            {
                Views.Shared.AvatarSelectorWindow.ShowFor(owner);e.Handled=true;
            }
        }

        private void OnDispatcherUnhandledException(object sender,
            System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            CatoriShared.Diagnostics.GameSafetyLog.Error("WPF.Dispatcher",
                "Unexpected UI-thread exception. The current action was cancelled.", e.Exception);
            e.Handled = true;
            System.Windows.Input.Mouse.OverrideCursor = null;

            if (Interlocked.Exchange(ref _handlingDispatcherFailure, 1) != 0)
                return;
            try
            {
                MessageBox.Show(MainWindow,
                    "Something unexpected happened. The current action was stopped, but the game can continue.",
                    "Catori recovered", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            finally
            {
                Interlocked.Exchange(ref _handlingDispatcherFailure, 0);
            }
        }

        private static void OnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
        {
            CatoriShared.Diagnostics.GameSafetyLog.Error("Tasks",
                "An unobserved background task failed.", e.Exception);
            e.SetObserved();
        }

        private static void OnDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception exception = e.ExceptionObject as Exception
                ?? new Exception($"Non-exception failure: {e.ExceptionObject}");
            CatoriShared.Diagnostics.GameSafetyLog.Error("AppDomain",
                $"Fatal process-level exception. IsTerminating={e.IsTerminating}.", exception);
        }
    }

}



