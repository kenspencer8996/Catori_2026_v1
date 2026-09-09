namespace CatoriApp.Game.Views.Manufacturing
{
    public partial class InventoryListView : Window
    {
        private readonly ProductBuilderViewController _controller = new();
        private readonly ProductViewModel? _product;
        private ObservableCollection<InventoryItemViewModel> _inventory = new();

        public InventoryListView(ProductViewModel? product = null)
        {
            _product = product;
            InitializeComponent();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadInventoryAsync();
        }

        private async Task LoadInventoryAsync()
        {
            await _controller.LoadInventory();
            var items = _controller.inventoryVM.AsEnumerable();

            if (_product != null && _product.ProductId > 0)
                items = items.Where(i => i.ProductId == _product.ProductId);

            _inventory = new ObservableCollection<InventoryItemViewModel>(items);
            InventoryDataGrid.ItemsSource = _inventory;
            TitleTextBlock.Text = _product == null ? "Inventory" : $"Inventory: {_product.ProductName}";
            StatusTextBlock.Text = $"Loaded {_inventory.Count} inventory items.";
        }

        private InventoryItemViewModel? SelectedInventory => InventoryDataGrid.SelectedItem as InventoryItemViewModel;

        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
            var item = new InventoryItemViewModel
            {
                ProductId = _product?.ProductId ?? 0,
                ProductName = _product?.ProductName ?? "",
                Location = "Factory",
                QuantityOnHand = 0,
                LastUpdated = DateTime.Now
            };

            _inventory.Add(item);
            InventoryDataGrid.SelectedItem = item;
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedInventory == null)
                return;

            await _controller.UpdateInventoryAsync(SelectedInventory);
            InventoryDataGrid.Items.Refresh();
            StatusTextBlock.Text = "Inventory item saved.";
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