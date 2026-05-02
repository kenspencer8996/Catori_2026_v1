using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace CatoriCity2025WPF.ViewModels
{
    public class ManufacturingViewModel : ViewmodelBase
    {
        private ProductViewModel? _selectedProduct;
        private BomItemViewModel? _selectedBomItem;
        private InventoryItemViewModel? _selectedInventoryItem;

        public ObservableCollection<ProductViewModel> Products { get; } = new();
        public ObservableCollection<ProductViewModel> FinishedProducts { get; } = new();
        public ObservableCollection<ProductViewModel> Components { get; } = new();

        public ProductViewModel? SelectedProduct
        {
            get => _selectedProduct;
            set => SetProperty(ref _selectedProduct, value);
        }

        public BomItemViewModel? SelectedBomItem
        {
            get => _selectedBomItem;
            set => SetProperty(ref _selectedBomItem, value);
        }

        public InventoryItemViewModel? SelectedInventoryItem
        {
            get => _selectedInventoryItem;
            set => SetProperty(ref _selectedInventoryItem, value);
        }
    

        public void AddComponentToSelectedProduct(ProductViewModel component)
        {
            if (SelectedProduct == null)
                return;

            SelectedProduct.BomItems.Add(new BomItemViewModel
            {
                ParentProductId = SelectedProduct.ProductId,
                ComponentId = component.ProductId,
                ComponentName = component.ProductName,
                ComponentCode = component.ProductCode,
                Quantity = 1,
                ScrapFactor = 0,
                EffectiveDate = DateTime.Today
            });
        }

    } 
}
