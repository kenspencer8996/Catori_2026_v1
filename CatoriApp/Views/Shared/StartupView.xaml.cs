using CatoriApp.Controllers;
namespace CatoriApp.Views.Shared
{
    /// <summary>
    /// Interaction logic for StartupView.xaml
    /// </summary>
    public partial class StartupView : Window
    {
        StartupViewController _controller;
        string _version = "1.0.1";
        public StartupView()
        {
            InitializeComponent();
            _controller = new StartupViewController(this);
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH_mm_ss");

            cLogger.LogFilePath = System.IO.Path.Combine("c:\\Logs", "CatoriCity2026WPF" + timestamp + ".Log");
            cLogger.Log("CatoriCity2026WPF Version: " + _version);
            Left = 0;
            Top = 0;
            string tooltipimagechest = System.IO.Path.Combine(GlobalAllApps.ImageFolder, "Treasure", "CHestClosed.png");

            ImageTextToolTip toolTip = new ImageTextToolTip
            {
                Title = "Treasure Field",
                Description = "View the treasure field to see the clues and information about the bad guys hidden treasure.",
                Icon = UIUtility.GetImageControl(tooltipimagechest, 32, 32, 0).Source
            };
            TreasureHuntButton.ToolTip = toolTip;
        }

        private void CityScapeButton_Click(object sender, RoutedEventArgs e)
        {
            CityScapeView cityScapeView = new CityScapeView();
            cityScapeView.ShowDialog();
            cityScapeView.Close();
        }
        private void CityScapePath_MouseLeftButtonUp(object sender, RoutedEventArgs e)
        {   
            CityScapeView cityScapeView = new CityScapeView();
            cityScapeView.ShowDialog();
            cityScapeView.Close();
        }
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void TreasureHuntButton_Click(object sender, RoutedEventArgs e)
        {
            TreasureFieldView view = new TreasureFieldView(Width,Height);
            view.Owner = this;
            view.ShowDialog();
            view.Close();
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            SettingsView view = new SettingsView();
            view.Owner = this;
            view.ShowDialog();
        }

        private void Location1Button_Click(object sender, RoutedEventArgs e)
        {
            ShowFactoryView(2);
        }

        private void Location2Button_Click(object sender, RoutedEventArgs e)
        {
            ShowFactoryView(1);
        }

        private void Location3Button_Click(object sender, RoutedEventArgs e)
        {
            ShowFactoryView(3);
        }

        private void Location4Button_Click(object sender, RoutedEventArgs e)
        {
            ShowFactoryView(4);
        }
        private void ShowFactoryView(int locationNumber)
        {
            FactoryView view = new FactoryView(locationNumber);
            view.Owner = this;
            view.ShowDialog();
        }

        private void RobotArmButton_Click(object sender, RoutedEventArgs e)
        {
            //RobotTestView view = new RobotTestView();
            //view.ShowDialog();
        }

        private void ProductMaintenanceButton_Click(object sender, RoutedEventArgs e)
        {

            ProductBuilderView view = new ProductBuilderView();
            view.ShowDialog();
        }
    }
}


