namespace CatoriApp.Game.Views.Manufacturing
{
    public partial class ProductsListView : Window
    {
        private readonly ProductBuilderViewController _controller = new();
        private ObservableCollection<ProductViewModel> _products = new();
        private ObservableCollection<ComponentViewModel> componentVM = new();

        public ProductsListView()
        {
            InitializeComponent();
            //ImportButton.Visibility = GlobalAllApps.IsDeveloperUser()
            //    ? Visibility.Visible
            //    : Visibility.Collapsed;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadProductsAsync();
        }

        private async Task LoadProductsAsync()
        {
            await _controller.LoadProductsAsync();
            _products = _controller.products;
            ProductsDataGrid.ItemsSource = _products;

            await _controller.LoadComponentsAsync();
            componentVM = _controller.componentVM;

            foreach (var column in ProductsDataGrid.Columns.OfType<DataGridComboBoxColumn>())
                column.ItemsSource = Enum.GetValues(typeof(ProductType));

            StatusTextBlock.Text = $"Loaded {_products.Count} products.";
        }

        private ProductViewModel? SelectedProduct => ProductsDataGrid.SelectedItem as ProductViewModel;

        private void NewProductButton_Click(object sender, RoutedEventArgs e)
        {
            var product = new ProductViewModel
            {
                ProductName = "New Product",
                ProductCode = "NEWPROD",
                ProductType = ProductType.Finished,
                UnitOfMeasure = "pcs",
                CostPerUnit = 0,
                CreatedAt = DateTime.Now
            };

            _products.Add(product);
            ProductsDataGrid.SelectedItem = product;
            EditProduct(product);
        }

        private void EditProductButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedProduct != null)
                EditProduct(SelectedProduct);
        }

        private async void SaveProductButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedProduct == null)
                return;

            await SaveProductAsync(SelectedProduct);
        }

        private void ComponentsButton_Click(object sender, RoutedEventArgs e)
        {
            var view = new ComponentListView
            {
                Owner = this
            };
            view.ShowDialog();
        }

        private void BOMButton_Click(object sender, RoutedEventArgs e)
        {
            var view = new ProductBOMBuilderView(SelectedProduct)
            {
                Owner = this
            };
            view.ShowDialog();
        }
        private void BomText_Click(object sender, RoutedEventArgs e)
        {
            var view = new ProductBOMBuilderView(SelectedProduct)
            {
                Owner = this
            };
            view.ShowDialog();
        }
        private void InventoryButton_Click(object sender, RoutedEventArgs e)
        {
            var view = new InventoryListView(SelectedProduct)
            {
                Owner = this
            };
            view.ShowDialog();
        }

        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            var view = new ImportView
            {
                Owner = this
            };
            view.ShowDialog();
        }

        private void DataGridRow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is DataGridRow row && row.Item is ProductViewModel product)
                EditProduct(product);
        }

        private void EditProduct(ProductViewModel product)
        {
            var view = new ProductEditorView(product)
            {
                Owner = this
            };

            if (view.ShowDialog() == true)
            {
                ProductsDataGrid.Items.Refresh();
                StatusTextBlock.Text = "Product saved.";
            }
        }

        private async Task SaveProductAsync(ProductViewModel product)
        {
            await _controller.UpdateProductAsync(product);
            ProductsDataGrid.Items.Refresh();
            StatusTextBlock.Text = "Product saved.";
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
