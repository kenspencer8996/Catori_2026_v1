using Microsoft.Win32;

namespace CatoriApp.Game.Views.Manufacturing
{
    public partial class ImportView : Window
    {
        private readonly ProductBuilderViewController _controller = new();

        public ImportView()
        {
            InitializeComponent();
        }

        private void ProductImport_Click(object sender, RoutedEventArgs e)
        {
            var file = GetCsvFile();
            if (file == null)
                return;

            var rows = _controller.ImportProductsFromCsv(file);
            StatusTextBlock.Text = $"Loaded {rows.Count} products from {System.IO.Path.GetFileName(file)}.";
        }

        private void BOMImport_Click(object sender, RoutedEventArgs e)
        {
            var file = GetCsvFile();
            if (file == null)
                return;

            var rows = _controller.ImportBOMCsv(file);
            StatusTextBlock.Text = $"Loaded {rows.Count} BOM rows from {System.IO.Path.GetFileName(file)}.";
        }

        private void ComponentImport_Click(object sender, RoutedEventArgs e)
        {
            var file = GetCsvFile();
            if (file == null)
                return;

            var rows = _controller.ImportComponentsCsv(file);
            StatusTextBlock.Text = $"Loaded {rows.Count} components from {System.IO.Path.GetFileName(file)}.";
        }

        private void InventoryImport_Click(object sender, RoutedEventArgs e)
        {
            var file = GetCsvFile();
            if (file == null)
                return;

            var rows = _controller.ImportInventoryCsv(file);
            StatusTextBlock.Text = $"Loaded {rows.Count} inventory rows from {System.IO.Path.GetFileName(file)}.";
        }

        private static string? GetCsvFile()
        {
            var dialog = new OpenFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*"
            };

            return dialog.ShowDialog() == true ? dialog.FileName : null;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}