namespace CatoriApp.Game.Views.Manufacturing
{
    public partial class ProductEditorView : Window
    {
        private readonly ProductBuilderViewController _controller = new();
        private readonly ProductViewModel _product;

        public ProductEditorView(ProductViewModel product)
        {
            _product = product;
            InitializeComponent();
            DataContext = _product;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ProductTypeComboBox.ItemsSource = Enum.GetValues(typeof(ProductType));
        }

        private async void OkButton_Click(object sender, RoutedEventArgs e)
        {
            await _controller.UpdateProductAsync(_product);
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void BOMButton_Click(object sender, RoutedEventArgs e)
        {
            ProductBOMListView view = new ProductBOMListView(_product);
            view.Owner = this;
            view.ShowDialog();
        }
    }
}