using CatoriCity2025WPF.Controllers;

namespace CatoriCity2025WPF.Views
{
    /// <summary>
    /// Interaction logic for RealestateInteriorView.xaml
    /// </summary>
    public partial class RealestateInteriorView : Window
    {
        RealestateInteriorViewontroller _controller;
        PersonService _personService;
        public PersonViewModel _catori;
        public RealestateInteriorView()
        {
            InitializeComponent();
            _controller = new RealestateInteriorViewontroller(this);
            _personService = new PersonService();
            Loaded += RealestateInteriorView_LoadedAsync;
        }

        private async void RealestateInteriorView_LoadedAsync(object sender, RoutedEventArgs e)
        {
            //string filepath = Imagehelper.GetImagePath(model.ImageName);
            //officei.MainImage.Source = UIUtility.GetImageControl(filepath, 100, 100, 0).Source;
            _catori = await _personService.GetPersonbyNameAsync("Catori");
            CurrentFundsText.Text = _catori.Funds.ToString("C");


            _controller.LoadHouses();
        }

        private void HouseComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedHouse = HouseComboBox.SelectedItem as HouseViewModel;
            _controller.OnHouseSelected(selectedHouse);
        }

        private void SalePriceTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //_controller.OnPriceChanged(SalePriceTextBox.Text);
        }

        private void BuyHouseButton_Click(object sender, RoutedEventArgs e)
        {
            _controller.BuyHouse();
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            _controller.LoadHouses();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
