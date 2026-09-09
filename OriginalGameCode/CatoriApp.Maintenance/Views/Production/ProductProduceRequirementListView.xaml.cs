using CatoriApp.Maintenance.Objects.Services.Production;
using CatoriApp.Maintenance.ViewModels.Production;

namespace CatoriApp.Maintenance.Views.Production
{
    public partial class ProductProduceRequirementListView : Window
    {
        private readonly ProductionMaintenanceService _service = new();
        private readonly ProductionMaintenanceListViewModel _viewModel = new();

        public ProductProduceRequirementListView()
        {
            InitializeComponent();
            DataContext = _viewModel;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadAsync();
        }

        private async Task LoadAsync()
        {
            _viewModel.ProductProduceRequirements.Clear();
            foreach (var item in await _service.GetAllProductProduceRequirementsAsync())
                _viewModel.ProductProduceRequirements.Add(item);
            _viewModel.SelectedProductProduceRequirement ??= _viewModel.ProductProduceRequirements.FirstOrDefault();
            _viewModel.StatusMessage = "Product produce requirements loaded.";
        }

        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
            var model = new ProductProduceRequirementViewModel { RequiredCapabilityType = "Assembly", MinimumFactoryLevel = 1 };
            _viewModel.ProductProduceRequirements.Add(model);
            _viewModel.SelectedProductProduceRequirement = model;
            Edit(model);
        }

        private void DataGridRow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is DataGridRow row && row.Item is ProductProduceRequirementViewModel model)
                Edit(model);
        }

        private void Edit(ProductProduceRequirementViewModel model)
        {
            var view = new ProductProduceRequirementEditorView(model) { Owner = this };
            if (view.ShowDialog() == true)
                _viewModel.StatusMessage = "Product produce requirement saved.";
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}