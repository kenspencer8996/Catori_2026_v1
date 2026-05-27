using CatoriApp.Game.Controllers;
using Microsoft.Win32;
using static System.Net.Mime.MediaTypeNames;
namespace CatoriApp.Game.Views.Manufacturing
{
    /// <summary>
    /// Interaction logic for ProductBuilderView.xaml
    /// </summary>
    public partial class ProductBuilderView : Window
    {
        ProductBuilderViewController _controller;
        public ProductBuilderView()
        {
            InitializeComponent();
            _controller = new ProductBuilderViewController(this);
            Loaded += ProductBuilderView_Loaded;
            if (GlobalAllApps.IsDeveloperUser())
            {
                ProductImport.Visibility = Visibility.Visible;
                BOMImport.Visibility = Visibility.Visible;
                ComponentImport.Visibility = Visibility.Visible;
                InventoryImport.Visibility = Visibility.Visible;
            }
            else 
            {
                ProductImport.Visibility = Visibility.Collapsed;
                BOMImport.Visibility = Visibility.Collapsed;
                ComponentImport.Visibility = Visibility.Collapsed;
                InventoryImport.Visibility = Visibility.Collapsed;
            }
        }

        private async void ProductBuilderView_Loaded(object sender, RoutedEventArgs e)
        {
            await _controller.LoadProductsAsync();
            ProductsListBox.ItemsSource = _controller.products;
            await _controller.LoadBOM();
        }

        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*"
            };

            if (dialog.ShowDialog() == true)
            {
                MessageBox.Show($"Import preview coming next:\n{dialog.FileName}");
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void SaveProductButton_Click(object sender, RoutedEventArgs e)
        {
            if (ProductsListBox.SelectedItem is not ProductViewModel selectedProduct)
                return;

            if (!ApplyProductFields(selectedProduct))
                return;

            await _controller.UpdateProductAsync(selectedProduct);
            ProductsListBox.Items.Refresh();
            ProductsListBox.SelectedItem = selectedProduct;
        }

        private void ProductImport_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BOMImport_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ComponentImport_Click(object sender, RoutedEventArgs e)
        {

        }

        private void InventoryImport_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ProductsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ProductViewModel selectedProduct = ProductsListBox.SelectedItem as ProductViewModel;
            if (selectedProduct != null) 
            {
                SetProductFields(selectedProduct);
                SelectBOM(selectedProduct.ProductId);
            }
        }

        private void SelectBOM(int productId)
        {
            var bomItems = _controller.BomVM.Where(b => b.ParentProductId == productId).ToList();
            BOMDataGrid.ItemsSource = bomItems;
        }

        private void ProductTypeListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }
        private void SetProductFields(ProductViewModel product)
        {
            NameTextBox.Text = product.ProductName;
            ProductCodeTextBox.Text = product.ProductCode;
            ProductTypeListBox.SelectedIndex = product.ProductType == ProductType.Component ? 1 : 0;
            UnitOfMeasureTextBox.Text = product.UnitOfMeasure;
            CostTextBox.Text = product.CostPerUnit.ToString();
        }

        private bool ApplyProductFields(ProductViewModel product)
        {
            if (!decimal.TryParse(CostTextBox.Text, out var costPerUnit))
            {
                MessageBox.Show("Cost must be a valid number.", "Product Builder", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            product.ProductName = NameTextBox.Text?.Trim() ?? "";
            product.ProductCode = ProductCodeTextBox.Text?.Trim() ?? "";
            product.ProductType = GetSelectedProductType();
            product.UnitOfMeasure = UnitOfMeasureTextBox.Text?.Trim() ?? "";
            product.CostPerUnit = costPerUnit;
            return true;
        }

        private ProductType GetSelectedProductType()
        {
            if (ProductTypeListBox.SelectedItem is ComboBoxItem item
                && item.Content?.ToString() == ProductType.Component.ToString())
                return ProductType.Component;

            return ProductType.Finished;
        }

        private void AddProductButton_Click(object sender, RoutedEventArgs e)
        {
            var thisprod = new ProductViewModel
            {
                ProductName = "New Product",
                ProductCode = "NEWPROD",
                ProductType = ProductType.Finished,
                UnitOfMeasure = "pcs",
                CostPerUnit = 0
            };
            _controller.products.Add(thisprod);
             ProductsListBox.SelectedItem = thisprod;
        }

        private void AddComponentButton_Click(object sender, RoutedEventArgs e)
        {
            var thisprod = new BomItemViewModel
            {
                Quantity = 1,
                ScrapFactor = 0
            };
            _controller.BomVM.Add(thisprod);
            BOMDataGrid.ItemsSource = _controller.BomVM;

        }
    }
}