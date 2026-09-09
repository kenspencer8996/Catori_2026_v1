namespace CatoriApp.Game.Views.Manufacturing
{
    public partial class ProductBOMEditorView : Window
    {
        private readonly ProductBuilderViewController _controller = new();
        private readonly BomItemViewModel _bom;
        private List<ProductViewModel> _products = new();

        public ProductBOMEditorView(BomItemViewModel bom)
        {
            _bom = bom;
            InitializeComponent();
            DataContext = _bom;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //var components = _controller.components
            await _controller.LoadProductsAsync();
            _products = _controller.products.ToList();
            ParentProductComboBox.ItemsSource = _products;
            ComponentComboBox.ItemsSource = _products.Where(p => p.ProductType == ProductType.Component).ToList();
        }

        private async void OkButton_Click(object sender, RoutedEventArgs e)
        {
            ApplyComponentDisplayFields();
            await _controller.UpdateBOMAsync(_bom);
            DialogResult = true;
            Close();
        }

        private void ComponentComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyComponentDisplayFields();
        }

        private void ApplyComponentDisplayFields()
        {
            if (ComponentComboBox.SelectedItem is not ProductViewModel component)
                component = _products.FirstOrDefault(p => p.ProductId == _bom.ComponentId);

            if (component == null)
                return;

            _bom.ComponentName = component.ProductName;
            _bom.ComponentCode = component.ProductCode;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}