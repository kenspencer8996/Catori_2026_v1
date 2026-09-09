namespace CatoriApp.Maintenance.ViewModels.Manufacturing
{
    public class ProductBOMBuilderViewModel : ViewmodelBase
    {
        private ProductViewModel? _selectedProduct;
        private ProductViewModel? _newBomComponent;
        private BomItemViewModel? _selectedBomItem;
        private string _statusMessage = "";

        public ObservableCollection<ProductViewModel> Products { get; } = new();
        public ObservableCollection<ProductViewModel> Components { get; } = new();
        public ObservableCollection<BomItemViewModel> SelectedProductBomItems { get; } = new();

        public ProductViewModel? SelectedProduct
        {
            get => _selectedProduct;
            set => SetProperty(ref _selectedProduct, value);
        }

        public ProductViewModel? NewBomComponent
        {
            get => _newBomComponent;
            set => SetProperty(ref _newBomComponent, value);
        }

        public BomItemViewModel? SelectedBomItem
        {
            get => _selectedBomItem;
            set => SetProperty(ref _selectedBomItem, value);
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }
    }
}
