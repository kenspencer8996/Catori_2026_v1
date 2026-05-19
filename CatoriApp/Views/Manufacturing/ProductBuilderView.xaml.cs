using CatoriApp.Controllers;
using Microsoft.Win32;
using static System.Net.Mime.MediaTypeNames;
namespace CatoriApp.Views.Manufacturing
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

        private void SaveProductButton_Click(object sender, RoutedEventArgs e)
        {

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
            ProductTypeListBox.SelectedValue = product.ProductType.ToString();
            UnitOfMeasureTextBox.Text = product.UnitOfMeasure;
            CostTextBox.Text = product.CostPerUnit.ToString();
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


