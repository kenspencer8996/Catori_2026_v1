namespace CatoriApp.Game.Views.Manufacturing
{
    public partial class ProductBOMListView : Window
    {
        private readonly ProductBuilderViewController _controller = new();
        private readonly ProductViewModel? _parentProduct;
        private ObservableCollection<BomItemViewModel> _bomItems = new();

        public ProductBOMListView(ProductViewModel? parentProduct = null)
        {
            _parentProduct = parentProduct;
            InitializeComponent();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadBOMAsync();
        }

        private async Task LoadBOMAsync()
        {
            await _controller.LoadBOM();
            var items = _controller.BomVM.AsEnumerable();

            if (_parentProduct != null && _parentProduct.ProductId > 0)
                items = items.Where(b => b.ParentProductId == _parentProduct.ProductId);

            _bomItems = new ObservableCollection<BomItemViewModel>(items);
            BOMDataGrid.ItemsSource = _bomItems;
            //titlet.Text = _parentProduct == null ? "Product BOM" : $"BOM: {_parentProduct.ProductName}";
            StatusTextBlock.Text = $"Loaded {_bomItems.Count} BOM items.";
        }

        private BomItemViewModel? SelectedBom => BOMDataGrid.SelectedItem as BomItemViewModel;

        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
            var bom = new BomItemViewModel
            {
                ParentProductId = _parentProduct?.ProductId ?? 0,
                Quantity = 1,
                ScrapFactor = 0,
                EffectiveDate = DateTime.Today
            };

            _bomItems.Add(bom);
            BOMDataGrid.SelectedItem = bom;
            EditBOM(bom);
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedBom != null)
                EditBOM(SelectedBom);
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedBom == null)
                return;

            await _controller.UpdateBOMAsync(SelectedBom);
            BOMDataGrid.Items.Refresh();
            StatusTextBlock.Text = "BOM item saved.";
        }

        private void DataGridRow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is DataGridRow row && row.Item is BomItemViewModel bom)
                EditBOM(bom);
        }

        private void EditBOM(BomItemViewModel bom)
        {
            var view = new ProductBOMEditorView(bom)
            {
                Owner = this
            };

            if (view.ShowDialog() == true)
            {
                BOMDataGrid.Items.Refresh();
                StatusTextBlock.Text = "BOM item saved.";
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}