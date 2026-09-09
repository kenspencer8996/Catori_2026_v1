using CatoriApp.Maintenance.ViewModels.Manufacturing;

namespace CatoriApp.Game.Views.Manufacturing
{
    public partial class ProductBOMBuilderView : Window
    {
        private readonly ProductBuilderViewController _controller = new();
        private readonly ProductBOMBuilderViewModel _viewModel = new();
        private readonly ProductViewModel? _initialProduct;

        public ProductBOMBuilderView(ProductViewModel? selectedProduct = null)
        {
            _initialProduct = selectedProduct;
            InitializeComponent();
            DataContext = _viewModel;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadAsync();
        }

        private async Task LoadAsync()
        {
            await _controller.LoadProductsAsync();

            _viewModel.Products.Clear();
            _viewModel.Components.Clear();

            foreach (var product in _controller.products.OrderBy(p => p.ProductName))
                _viewModel.Products.Add(product);

            foreach (var component in _controller.products.Where(p => p.ProductType == ProductType.Component).OrderBy(p => p.ProductName))
                _viewModel.Components.Add(component);

            _viewModel.SelectedProduct = _initialProduct == null
                ? _viewModel.Products.FirstOrDefault()
                : _viewModel.Products.FirstOrDefault(p => p.ProductId == _initialProduct.ProductId) ?? _initialProduct;

            _viewModel.NewBomComponent = _viewModel.Components.FirstOrDefault(p => p.ProductId != _viewModel.SelectedProduct?.ProductId);
            await LoadSelectedProductBomAsync();
        }

        private async void ProductsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            await LoadSelectedProductBomAsync();
        }

        private async Task LoadSelectedProductBomAsync()
        {
            _viewModel.SelectedProductBomItems.Clear();

            if (_viewModel.SelectedProduct == null || _viewModel.SelectedProduct.ProductId <= 0)
            {
                _viewModel.StatusMessage = "Select a saved product to edit its BOM.";
                return;
            }

            await _controller.LoadBOM();
            var bomItems = _controller.BomVM
                .Where(b => b.ParentProductId == _viewModel.SelectedProduct.ProductId)
                .OrderBy(b => b.ComponentName)
                .ToList();

            foreach (var item in bomItems)
            {
                ApplyComponentDisplayFields(item);
                _viewModel.SelectedProductBomItems.Add(item);
            }

            _viewModel.SelectedBomItem = _viewModel.SelectedProductBomItems.FirstOrDefault();
            _viewModel.StatusMessage = $"Loaded {_viewModel.SelectedProductBomItems.Count} BOM components.";
        }

        private async void AddComponentButton_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.SelectedProduct == null || _viewModel.SelectedProduct.ProductId <= 0)
            {
                _viewModel.StatusMessage = "Select a saved parent product first.";
                return;
            }

            var component = _viewModel.NewBomComponent
                ?? _viewModel.Components.FirstOrDefault(p => p.ProductId != _viewModel.SelectedProduct.ProductId);

            if (component == null)
            {
                _viewModel.StatusMessage = "No component products are available.";
                return;
            }

            var item = new BomItemViewModel
            {
                ParentProductId = _viewModel.SelectedProduct.ProductId,
                ComponentId = component.ProductId,
                Quantity = 1,
                ScrapFactor = 0,
                EffectiveDate = DateTime.Today
            };

            ApplyComponentDisplayFields(item);
            await _controller.UpdateBOMAsync(item);
            _viewModel.SelectedProductBomItems.Add(item);
            _viewModel.SelectedBomItem = item;
            _viewModel.StatusMessage = "Component added.";
        }

        private async void DeleteComponentButton_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.SelectedBomItem == null)
                return;

            var item = _viewModel.SelectedBomItem;
            if (item.BomId > 0)
                await _controller.RemoveBOMAsync(item);

            _viewModel.SelectedProductBomItems.Remove(item);
            _viewModel.SelectedBomItem = _viewModel.SelectedProductBomItems.FirstOrDefault();
            _viewModel.StatusMessage = "Component deleted.";
        }

        private async void SaveChangesButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in _viewModel.SelectedProductBomItems)
            {
                ApplyComponentDisplayFields(item);
                await _controller.UpdateBOMAsync(item);
            }

            BomDataGrid.Items.Refresh();
            _viewModel.StatusMessage = "BOM changes saved.";
        }

        private void ComponentComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ComboBox)?.DataContext is not BomItemViewModel item)
                return;

            ApplyComponentDisplayFields(item);
            BomDataGrid.Items.Refresh();
        }

        private void ApplyComponentDisplayFields(BomItemViewModel item)
        {
            var component = _viewModel.Components.FirstOrDefault(p => p.ProductId == item.ComponentId)
                ?? _viewModel.Products.FirstOrDefault(p => p.ProductId == item.ComponentId);

            if (component == null)
                return;

            item.ComponentName = component.ProductName;
            item.ComponentCode = component.ProductCode;
            item.ComponentCost = component.CostPerUnit;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
