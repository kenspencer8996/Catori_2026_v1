using CatoriCity2025WPF.Controllers;

namespace CatoriCity2025WPF.Views
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

        private void TestButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Test button clicked!");
        }
    }
}
