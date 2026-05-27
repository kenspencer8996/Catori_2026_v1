using CatoriApp.Game.Views;
using System.Collections.ObjectModel;
namespace CatoriApp.Game.Controllers.Manufacturing
{
    public class ProductBuilderViewController
    {
        ProductBuilderView _view;
        LocationInventoryService locationInventoryService;
        ProductService productservice;
        BillOfMaterialsService bomservice;
        ComponentService componentService;
        public ObservableCollection<ProductViewModel> products;
        public ObservableCollection<InventoryItemViewModel> inventoryVM;
        public ObservableCollection<ComponentViewModel> componentVM;
        public ObservableCollection<BomItemViewModel> BomVM;
        public ProductBuilderViewController(ProductBuilderView view)
        {
            _view = view;
            productservice = new ProductService();
            componentService = new ComponentService();
            locationInventoryService = new LocationInventoryService();
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
            var inv =await locationInventoryService.GetAllAsync();
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

        public async Task UpdateProductAsync(ProductViewModel pm) 
        {
            try
            {
                await productservice.SaveAsync(pm);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving product: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public async Task RemoveProductAsync(ProductViewModel pm)
        {
            try
            {
                await productservice.DeleteAsync(pm);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving product: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public async Task UpdateBOMAsync(BomItemViewModel bom)
        {
            try
            {
                await bomservice.SaveAsync(bom);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving bom: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public async Task RemoveBOMAsync(BomItemViewModel bom)
        {
            try
            {
                await bomservice.DeleteAsync(bom);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting bom: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
        public async Task AddComponentToBOMAsync(ComponentViewModel cvm)
        {
            try
            {
                await componentService.SaveAsync(cvm);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving product: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public async Task RemoveComponentAsync(ComponentViewModel cvm)
        {
            try
            {
                await componentService.SaveAsync(cvm);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting component: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
       
    }
}