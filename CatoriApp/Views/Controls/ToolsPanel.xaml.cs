using CatoriApp.MachineLayoutDesigner.Views.MachineCatalog;
using System.Diagnostics;
using System.Windows.Media.Animation;

namespace CatoriApp.Views.Controls
{
    public partial class ToolsPanel : UserControl
    {
        public StartupView startupView;
        private const double OpenWidth = 300;
        private const double OpenHeight = 100;
        public ToolsPanel()
        {
            InitializeComponent();
            Width = 0;
            Height = 0;
            Opacity = 0;
            if (GlobalAllApps.IsDeveloperUser() 
                && Debugger.IsAttached)
                MachinesButton.Visibility = Visibility.Visible;
            else
                MachinesButton.Visibility = Visibility.Collapsed;
        }
        private void ProductMaintenanceButton_Click(object sender, RoutedEventArgs e)
        {

            ProductBuilderView view = new ProductBuilderView();
            view.ShowDialog();
        }
        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            SettingsView view = new SettingsView();
            view.Owner = startupView;
            view.ShowDialog();
        }
        private void MachineCatalog_Click(object sender, RoutedEventArgs e)
        {
            MachineCatalogEditorWindow view = new();
            view.Owner = startupView;
            view.ShowDialog();
        }

        private void MachineInstances_Click(object sender, RoutedEventArgs e)
        {
            MachineInstanceEditorWindow view = new();
            view.Owner = startupView;
            view.ShowDialog();
        }

        public void OpenPanel()
        {
            AnimatePanel(OpenWidth, OpenHeight, 1);
        }

        public void ClosePanel()
        {
            AnimatePanel(0, 0, 0);
        }

        private void AnimatePanel(double targetWidth, double targetHeight, double targetOpacity)
        {
            var duration = TimeSpan.FromMilliseconds(300);

            var sb = new Storyboard();

            AddAnimation(sb, this, WidthProperty, targetWidth, duration);
            AddAnimation(sb, this, HeightProperty, targetHeight, duration);
            AddAnimation(sb, this, OpacityProperty, targetOpacity, duration);

            sb.Begin();
        }

        private static void AddAnimation(
            Storyboard sb,
            DependencyObject target,
            DependencyProperty property,
            double to,
            TimeSpan duration)
        {
            var animation = new DoubleAnimation
            {
                To = to,
                Duration = duration,
                AccelerationRatio = 0.2,
                DecelerationRatio = 0.8
            };

            Storyboard.SetTarget(animation, target);
            Storyboard.SetTargetProperty(animation, new PropertyPath(property));

            sb.Children.Add(animation);
        }

        private void UserControl_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ClosePanel();
        }
    }
}
