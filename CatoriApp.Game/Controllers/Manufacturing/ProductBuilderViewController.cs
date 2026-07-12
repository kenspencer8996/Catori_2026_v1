using System.Collections.ObjectModel;
namespace CatoriApp.Game.Controllers.Manufacturing
{
    public class ProductBuilderViewController
    {
        private readonly LocationInventoryService locationInventoryService;
        private readonly ProductService productservice;
        private readonly BillOfMaterialsService bomservice;
        private readonly ComponentService componentService;

        public ObservableCollection<ProductViewModel> products = new();
        public ObservableCollection<InventoryItemViewModel> inventoryVM = new();
        public ObservableCollection<ComponentViewModel> componentVM = new();
        public ObservableCollection<BomItemViewModel> BomVM = new();

        public ProductBuilderViewController()
        {
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
        public async Task LoadComponentsAsync()
        {
            var components = await componentService.GetAllAsync();
            componentVM = new ObservableCollection<ComponentViewModel>(components);
        }
        public async Task LoadBOM()
        {
            var bom = await bomservice.GetAllAsync();
            BomVM = new ObservableCollection<BomItemViewModel>(bom);
        }

        public async Task LoadInventory()
        {
            var inv = await locationInventoryService.GetAllAsync();
            inventoryVM = new ObservableCollection<InventoryItemViewModel>(inv);
        }

        public async Task LoadComponents()
        {
            var components = await componentService.GetAllAsync();
            componentVM = new ObservableCollection<ComponentViewModel>(components);
        }

        public List<ProductViewModel> ImportProductsFromCsv(string filePath)
        {
            return CsvImportService.LoadProducts(filePath);
        }

        public List<BomItemViewModel> ImportBOMCsv(string filePath)
        {
            return CsvImportService.LoadBomItems(filePath);
        }

        public List<InventoryItemViewModel> ImportInventoryCsv(string filePath)
        {
            return CsvImportService.LoadInventory(filePath);
        }

        public List<ComponentViewModel> ImportComponentsCsv(string filePath)
        {
            return CsvImportService.LoadComponents(filePath);
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
                MessageBox.Show($"Error deleting product: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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

        public async Task UpdateInventoryAsync(InventoryItemViewModel inventory)
        {
            try
            {
                await locationInventoryService.SaveAsync(inventory);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving inventory: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public async Task RemoveInventoryAsync(InventoryItemViewModel inventory)
        {
            try
            {
                await locationInventoryService.DeleteAsync(inventory);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting inventory: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show($"Error saving component: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public async Task UpdateComponentAsync(ComponentViewModel cvm)
        {
            try
            {
                await componentService.SaveAsync(cvm);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving component: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public async Task RemoveComponentAsync(ComponentViewModel cvm)
        {
            try
            {
                await componentService.DeleteAsync(cvm);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting component: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}