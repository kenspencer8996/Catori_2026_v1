using CatoriCity2025WPF.Controllers;

namespace CatoriCity2025WPF.Views
{
    /// <summary>
    /// Interaction logic for RealestateInteriorView.xaml
    /// </summary>
    public partial class RealestateInteriorView : Window
    {
        RealestateInteriorViewontroller _controller;
        public RealestateInteriorView()
        {
            InitializeComponent();
            _controller = new RealestateInteriorViewontroller(this);
            Loaded += RealestateInteriorView_Loaded;
        }

        private void RealestateInteriorView_Loaded(object sender, RoutedEventArgs e)
        {
            //string filepath = Imagehelper.GetImagePath(model.ImageName);
            //officei.MainImage.Source = UIUtility.GetImageControl(filepath, 100, 100, 0).Source;

            _controller.LoadHouses();
        }

        private void HouseComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedHouse = HouseComboBox.SelectedItem as HouseViewModel;
            _controller.OnHouseSelected(selectedHouse);
        }

        private void SalePriceTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _controller.OnPriceChanged(SalePriceTextBox.Text);
        }

        private void ListForSaleButton_Click(object sender, RoutedEventArgs e)
        {
            _controller.ListHouseForSale();
        }

        private void RemoveFromSaleButton_Click(object sender, RoutedEventArgs e)
        {
            _controller.RemoveFromSale();
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
