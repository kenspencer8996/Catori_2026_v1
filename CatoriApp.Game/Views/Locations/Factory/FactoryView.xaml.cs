namespace CatoriApp.Game.Views.Locations.Factory
{
    /// <summary>
    /// Interaction logic for FactoryView.xaml
    /// </summary>
    public partial class FactoryView : Window
    {
        int _locationNumber;
        public FactoryView(int locationNumber)
        {
            InitializeComponent();
            _locationNumber = locationNumber; 
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LocationUCPanel.Content = new FactoryInterior_UC(_locationNumber);
            
        }
    }
}



