using CatoriCity2025WPF.Views;
using CatoriServices.Services;
using System.Collections.ObjectModel;

namespace CatoriCity2025WPF.Controllers
{
    public class ProductBuilderViewController
    {
        ProductBuilderView _view;
        FactoryInventoryService factoryService;
        ProductService productservice;
        BillOfMaterialsService bomservice;
        ComponentService componentService;
        FactoryInventoryService factoryInventory;
        public ObservableCollection<ProductViewModel> products;
        public ObservableCollection<InventoryItemViewModel> inventoryVM;
        public ObservableCollection<ComponentViewModel> componentVM;
        public ObservableCollection<BomItemViewModel> BomVM;
        public ProductBuilderViewController(ProductBuilderView view)
        {
            _view = view;
            productservice = new ProductService();
            componentService = new ComponentService();
            factoryInventory = new FactoryInventoryService();
            bomservice = new BillOfMaterialsService();
        }
        public async Task LoadProductsAsync()
        {
            var productslist = await productservice.GetAllAsync();
            products = new ObservableCollection<ProductViewModel>(productslist);    
        }
        public async Task LoadBOM()
        {
            var bom = await bomservice.GetAllAsync();
            BomVM = new ObservableCollection<BomItemViewModel>(bom);
        }
        public async Task LoadComponents()
        {
            var comp = await componentService.GetAllAsync();
            componentVM = new ObservableCollection<ComponentViewModel>(comp);
        }
        public async Task LoadInventory()
        {
            var inv = await factoryInventory.GetAllAsync();
            inventoryVM = new ObservableCollection<InventoryItemViewModel>(inv);
        }
        public List<ProductViewModel> ImportProductsFromCsv(string filePath)
        {
            List<ProductViewModel> results = new List<ProductViewModel>();
            results = CsvImportService.LoadProducts(filePath);
            return results;
        }
        public List<BomItemViewModel> ImportBOMCsv(string filePath)
        {
            List<BomItemViewModel> results = new List<BomItemViewModel>();
            results = CsvImportService.LoadBomItems(filePath);
            return results;
        }
        public List<InventoryItemViewModel> ImportInventoryCsv(string filePath)
        {
            List<InventoryItemViewModel> results = new List<InventoryItemViewModel>();
            results = CsvImportService.LoadInventory(filePath);
            return results;

        }
        public List<ComponentViewModel> ImportComponentsCsv(string filePath)
        {
            List<ComponentViewModel> results = new List<ComponentViewModel>();
            results = CsvImportService.LoadComponents(filePath);
            return results;
        }

        public void UpdateProduct(ProductViewModel pm) 
        {
            try
            {
                productservice.SaveAsync(pm);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving product: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public void RemoveProduct(ProductViewModel pm)
        {
            try
            {
                productservice.DeleteAsync(pm);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving product: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public void UpdateBOM(BomItemViewModel bom)
        {
            try
            {
                bomservice.SaveAsync(bom);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving bom: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public void RemoveBOM(BomItemViewModel bom)
        {
            try
            {
                bomservice.DeleteAsync(bom);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting bom: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
        public void AddComponentToBOM(ComponentViewModel cvm)
        {
            try
            {
                componentService.SaveAsync(cvm);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving product: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public void RemoveComponent(ComponentViewModel cvm)
        {
            try
            {
                componentService.SaveAsync(cvm);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting component: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
       
    }
}
